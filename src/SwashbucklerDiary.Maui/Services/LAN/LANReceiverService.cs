using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Maui.Models;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SwashbucklerDiary.Maui.Services
{
    public class LANReceiverService : ILANReceiverService
    {
        public event Action<long, long>? ReceiveProgressChanged;

        public event Action? ReceiveStart;

        public event Action? ReceiveAborted;

        public event Action<string>? ReceiveCompleted;

        private IPAddress? _multicastAddress; // UDP组播地址

        private int _multicastPort; // UDP组播端口

        private int _tcpPort; //TCP端口

        private UdpClient? udpClient;

        private TcpListener? tcpListener;

        private CancellationTokenSource? udpClientCTS;

        private CancellationTokenSource? tcpClientCTS;

        private readonly ILogger<LANReceiverService> _logger;

        private readonly IPlatformIntegration _platformIntegration;

        private LANDeviceInfo? lanDeviceInfo;

        public bool IsMulticasting => !udpClientCTS?.IsCancellationRequested ?? false;

        public bool IsReceiving => !tcpClientCTS?.IsCancellationRequested ?? false;

        public LANReceiverService(IPlatformIntegration platformIntegration, ILogger<LANReceiverService> logger)
        {
            _platformIntegration = platformIntegration;
            _logger = logger;
        }

        public void Dispose()
        {
            udpClientCTS?.Cancel();
            tcpClientCTS?.Cancel();
            if (udpClient is not null)
            {
                udpClient.Close();
                udpClient = null;
            }

            if (tcpListener is not null)
            {
                tcpListener.Dispose();
                tcpListener = null;
            }
        }

        public void Initialize(string multicastAddress, int multicastPort, int tcpPort, string? deviceName)
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

            Receive();
            Multicast();
        }

        public void Multicast()
        {
            if (udpClient is null)
            {
                return;
            }

            if (_multicastAddress is null)
            {
                return;
            }

            if (udpClientCTS is not null && !udpClientCTS.IsCancellationRequested)
            {
                return;
            }

            udpClientCTS = new();

            Task.Run(async () =>
            {
                string jsonLANDeviceInfo = JsonSerializer.Serialize(lanDeviceInfo);
                byte[] data = Encoding.UTF8.GetBytes(jsonLANDeviceInfo);

                try
                {
                    while (IsMulticasting)
                    {
                        await udpClient.SendAsync(data, new IPEndPoint(_multicastAddress, _multicastPort), udpClientCTS.Token);
                        await Task.Delay(1000);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "stop multicast");
                }
                finally
                {
                    udpClientCTS?.Cancel();
                }
            });
        }

        public void Receive()
        {
            if (tcpListener is null)
            {
                return;
            }

            if (tcpClientCTS is not null && !tcpClientCTS.IsCancellationRequested)
            {
                return;
            }

            tcpClientCTS = new();

            Task.Run(async () =>
            {
                try
                {
                    tcpListener.Start();
                    using TcpClient handler = await tcpListener.AcceptTcpClientAsync(tcpClientCTS.Token);
                    await using NetworkStream stream = handler.GetStream();
                    ReceiveStart?.Invoke();

                    // 接收文件大小
                    byte[] fileSizeBytes = new byte[8];
                    await stream.ReadExactlyAsync(fileSizeBytes, tcpClientCTS.Token);
                    long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

                    // 接收文件名长度和文件名
                    byte[] fileNameLengthBytes = new byte[4];
                    await stream.ReadExactlyAsync(fileNameLengthBytes, tcpClientCTS.Token);
                    int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);
                    byte[] fileNameBytes = new byte[fileNameLength];
                    await stream.ReadExactlyAsync(fileNameBytes, tcpClientCTS.Token);
                    string fileName = Encoding.UTF8.GetString(fileNameBytes);

                    var path = Path.Combine(FileSystem.CacheDirectory, Guid.NewGuid().ToString() + ".zip");
                    using FileStream fileStream = File.Create(path);
                    byte[] buffer = new byte[1024 * 1024];
                    int bytesRead;
                    var readLength = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        readLength += bytesRead;
                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), tcpClientCTS.Token);
                        ReceiveProgressChanged?.Invoke(readLength, fileSize);
                    }

                    if (fileSize != fileStream.Length)
                    {
                        throw new Exception();
                    }

                    //接收成功
                    ReceiveCompleted?.Invoke(path);
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "stop receive");
                    ReceiveAborted?.Invoke();
                }
                finally
                {
                    tcpListener?.Stop();
                    tcpClientCTS?.Cancel();
                }
            });
        }

        public LANDeviceInfo GetLocalLANDeviceInfo()
        {
            string deviceName = LANHelper.GetLocalDeviceName();
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
