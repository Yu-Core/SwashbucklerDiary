using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public class LANReceiverService : ILANReceiverService
    {
        public event Action? ReceiveStart;
        public event Action? ReceiveAborted;
        public event Action? ReceiveCanceled;
        public event Action<string>? ReceiveCompleted;
        public event Action<SocketException>? ConnectFailed;

        private IPAddress? _multicastAddress; // UDP组播地址

        private int _multicastPort; // UDP组播端口

        private int _tcpPort; //TCP端口

        private UdpClient? udpClient;

        private TcpListener? tcpListener;

        private LANDeviceInfo? lanDeviceInfo;

        private CancellationTokenSource? udpClientCTS;

        private CancellationTokenSource? tcpClientCTS;

        private readonly ILogger<LANReceiverService> _logger;

        private readonly IPlatformIntegration _platformIntegration;

        private readonly IAppFileSystem _appFileSystem;

        public bool IsMulticasting => udpClientCTS is not null && !udpClientCTS.IsCancellationRequested;

        public bool IsReceiving => tcpClientCTS is not null && !tcpClientCTS.IsCancellationRequested;

        public LANReceiverService(
            IPlatformIntegration platformIntegration,
            ILogger<LANReceiverService> logger,
            IAppFileSystem appFileSystem)
        {
            _platformIntegration = platformIntegration;
            _logger = logger;
            _appFileSystem = appFileSystem;
        }

        public void Dispose()
        {
            udpClientCTS?.Cancel();
            tcpClientCTS?.Cancel();

            if (udpClient is not null)
            {
                udpClient.Dispose();
                udpClient = null;
            }

            if (tcpListener is not null)
            {
                tcpListener.Dispose();
                tcpListener = null;
            }
        }

        public void Start(string multicastAddress, int multicastPort, string? deviceName, int tcpPort, IProgress<TransferProgressArguments> progress)
        {
            _multicastAddress = IPAddress.Parse(multicastAddress);
            _multicastPort = multicastPort;
            _tcpPort = tcpPort;

            lanDeviceInfo = GetLocalLANDeviceInfo();
            if (!string.IsNullOrEmpty(deviceName))
            {
                lanDeviceInfo.DeviceName = deviceName;
            }

            var ipAddress = IPAddress.Parse(LANHelper.GetLocalIPv4Address());
            udpClient = new UdpClient(new IPEndPoint(ipAddress, _multicastPort));
            var ipEndPoint = new IPEndPoint(IPAddress.Any, _tcpPort);
            tcpListener = new(ipEndPoint);

            Receive(progress);
            Multicast();
        }

        private void Multicast()
        {
            if (udpClient is null)
            {
                return;
            }

            if (_multicastAddress is null)
            {
                return;
            }

            if (IsMulticasting)
            {
                return;
            }

            udpClientCTS = new();

            Task.Run(async () =>
            {
                try
                {
                    var json = JsonSerializer.Serialize(lanDeviceInfo);
                    var data = Encoding.UTF8.GetBytes(json);
                    var iPEndPoint = new IPEndPoint(_multicastAddress, _multicastPort);

                    while (IsMulticasting)
                    {
                        await udpClient.SendAsync(data, iPEndPoint, udpClientCTS.Token).ConfigureAwait(false);
                        await Task.Delay(1000);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("LAN Multicast cancel");
                }
                catch (SocketException e)
                {
                    _logger.LogError(e, "LAN Multicast error");
                    ConnectFailed?.Invoke(e);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "LAN Multicast error");
                }
                finally
                {
                    udpClientCTS?.Cancel();
                    udpClientCTS?.Dispose();
                    udpClientCTS = null;
                }
            });
        }

        private void Receive(IProgress<TransferProgressArguments> progress)
        {
            if (tcpListener is null)
            {
                return;
            }

            if (IsReceiving)
            {
                return;
            }

            tcpClientCTS = new();

            Task.Run(async () =>
            {
                try
                {
                    tcpListener.Start();

                    using var handler = await tcpListener.AcceptTcpClientAsync(tcpClientCTS.Token).ConfigureAwait(false);
                    await using var stream = handler.GetStream();

                    ReceiveStart?.Invoke();

                    // 读取文件大小
                    var sizeBuffer = new byte[8];
                    await stream.ReadExactlyAsync(sizeBuffer, tcpClientCTS.Token).ConfigureAwait(false);
                    var fileSize = BitConverter.ToInt64(sizeBuffer);

                    // 接收文件名长度和文件名
                    byte[] fileNameLengthBytes = new byte[4];
                    await stream.ReadExactlyAsync(fileNameLengthBytes, tcpClientCTS.Token).ConfigureAwait(false);
                    int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);
                    byte[] fileNameBytes = new byte[fileNameLength];
                    await stream.ReadExactlyAsync(fileNameBytes, tcpClientCTS.Token).ConfigureAwait(false);
                    string fileName = Encoding.UTF8.GetString(fileNameBytes);

                    var tempPath = Path.Combine(_appFileSystem.CacheDirectory, $"{Guid.NewGuid()}.zip");
                    await using var fileStream = File.Create(tempPath);
                    byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024);

                    try
                    {
                        long totalBytesRead = 0;
                        int bytesRead;
                        var lastReportTime = DateTime.MinValue;

                        while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
                        {
                            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), tcpClientCTS.Token).ConfigureAwait(false);
                            totalBytesRead += bytesRead;

                            // 限制进度更新频率，避免过度触发事件
                            if (DateTime.Now - lastReportTime > TimeSpan.FromMilliseconds(100) ||
                                totalBytesRead == fileSize)
                            {
                                progress?.Report(new(totalBytesRead, fileSize));
                                lastReportTime = DateTime.Now;
                            }
                        }

                        if (totalBytesRead != fileSize)
                        {
                            ReceiveCanceled?.Invoke();
                        }
                        else
                        {
                            ReceiveCompleted?.Invoke(tempPath);
                        }
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(buffer);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("LAN Receive cancel");
                    ReceiveCanceled?.Invoke();
                }
                catch (SocketException e)
                {
                    _logger.LogError(e, "LAN Receive error");
                    ConnectFailed?.Invoke(e);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "LAN Receive error");
                    ReceiveAborted?.Invoke();
                }
                finally
                {
                    tcpListener?.Stop();
                    tcpClientCTS?.Cancel();
                    tcpClientCTS?.Dispose();
                    tcpClientCTS = null;
                }
            });
        }

        public LANDeviceInfo GetLocalLANDeviceInfo()
        {
            string deviceName = _platformIntegration.DeviceName;
            string ipAddress = LANHelper.GetLocalIPv4Address();
            AppDevicePlatform devicePlatform = _platformIntegration.CurrentPlatform;

            return new()
            {
                DeviceName = deviceName,
                IPAddress = ipAddress,
                DevicePlatform = devicePlatform
            };
        }

        public void CancelMulticast()
        {
            udpClientCTS?.Cancel();
        }

        public void CancelReceive()
        {
            tcpClientCTS?.Cancel();
        }
    }
}
