using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public class LANSenderService : ILANSenderService
    {
        public event Action<LANDeviceInfo>? DeviceDiscovered;
        public event Action<LANDeviceInfo>? DeviceTimeouted;
        public event Action? SearchEnded;
        public event Action? SendCanceled;
        public event Action? SendAborted;
        public event Action? SendCompleted;
        public event Action<SocketException>? ConnectFailed;

        private UdpClient? udpClient;

        private IPAddress? _multicastIPAddress; // UDP组播地址

        private int _multicastPort; // UDP组播端口

        private int _tcpPort; //TCP端口

        private int _millisecondsOutTime = 20000;

        private CancellationTokenSource? udpClientCTS;

        private CancellationTokenSource? tcpClientCTS;

        private readonly ILogger<LANSenderService> _logger;
        private readonly Lock _devicesLock = new();
        private readonly Dictionary<string, LANDeviceInfo> _devices = new();
        private const int DeviceTimeout = 5000;
        private const int DiscoveryInterval = 3000;

        public LANSenderService(ILogger<LANSenderService> logger)
        {
            _logger = logger;
        }

        public bool IsSearching => udpClientCTS is not null && !udpClientCTS.IsCancellationRequested;

        public bool IsSending => tcpClientCTS is not null && !tcpClientCTS.IsCancellationRequested;

        public void Dispose()
        {
            udpClientCTS?.Cancel();
            tcpClientCTS?.Cancel();
            DisposeUdpClient();
        }

        private void DisposeUdpClient()
        {
            if (udpClient is not null)
            {
                try
                {
                    if (_multicastIPAddress is not null)
                    {
                        udpClient.DropMulticastGroup(_multicastIPAddress);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "DropMulticastGroup error");
                }
                finally
                {
                    udpClient.Dispose();
                    udpClient = null;
                }
            }
        }

        public void Start(string multicastAddress, int multicastPort, int millisecondsOutTime, int tcpPort)
        {
            if (IsSearching)
            {
                return;
            }

            _multicastIPAddress = IPAddress.Parse(multicastAddress);
            _multicastPort = multicastPort;
            _tcpPort = tcpPort;
            _millisecondsOutTime = millisecondsOutTime;

            DisposeUdpClient();
            var ipAddress = IPAddress.Parse(LANHelper.GetLocalIPv4Address());
            udpClient = new UdpClient(_multicastPort);
            udpClient.JoinMulticastGroup(_multicastIPAddress, ipAddress);

            Search();
        }

        public void Search()
        {
            StartUdpDiscovery();
            StartCleanupThread();
        }

        private void StartUdpDiscovery()
        {
            if (udpClient is null)
            {
                return;
            }

            if (IsSearching)
            {
                return;
            }

            udpClientCTS = new(_millisecondsOutTime);

            Task.Run(async () =>
            {
                try
                {
                    while (IsSearching)
                    {
                        var receivedResults = await udpClient.ReceiveAsync(udpClientCTS.Token).ConfigureAwait(false);
                        var jsonLANDeviceInfo = Encoding.UTF8.GetString(receivedResults.Buffer);
                        try
                        {
                            LANDeviceInfo? deviceInfo = JsonSerializer.Deserialize<LANDeviceInfo>(jsonLANDeviceInfo);
                            if (deviceInfo is null || deviceInfo.IPAddress is null)
                            {
                                continue;
                            }

                            lock (_devicesLock)
                            {
                                deviceInfo.LastSeen = DateTime.Now;
                                string key = deviceInfo.IPAddress;
                                if (_devices.TryGetValue(key, out var existing))
                                {
                                    existing.LastSeen = DateTime.Now;
                                }
                                else
                                {
                                    deviceInfo.LastSeen = DateTime.Now;
                                    _devices[key] = deviceInfo;
                                    DeviceDiscovered?.Invoke(deviceInfo);
                                }
                            }

                        }
                        catch (JsonException e)
                        {
                            _logger.LogInformation(e, "read LANDeviceInfo error");
                            continue;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("LAN Discovery cancel");
                }
                catch (SocketException e)
                {
                    _logger.LogError(e, "LAN Discovery error");
                    ConnectFailed?.Invoke(e);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "LAN Discovery error");
                }
                finally
                {
                    udpClientCTS.Cancel();
                    udpClientCTS.Dispose();
                    udpClientCTS = null;
                    SearchEnded?.Invoke();
                }
            });
        }

        public async Task SendAsync(string ipAddress, string filePath, IProgress<TransferProgressArguments> progress)
        {
            if (IsSending)
            {
                return;
            }

            tcpClientCTS = new();
            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                    bufferSize: 4096, useAsync: true);

                using var tcpClient = new TcpClient(AddressFamily.InterNetwork);
                await tcpClient.ConnectAsync(ipAddress, _tcpPort).ConfigureAwait(false);
                await using NetworkStream stream = tcpClient.GetStream();

                long fileSize = fileStream.Length;
                string fileName = Path.GetFileName(filePath);

                // 发送文件大小
                await stream.WriteAsync(BitConverter.GetBytes(fileSize), tcpClientCTS.Token).ConfigureAwait(false);

                // 发送文件名
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                await stream.WriteAsync(BitConverter.GetBytes(fileNameBytes.Length), tcpClientCTS.Token).ConfigureAwait(false);
                await stream.WriteAsync(fileNameBytes, tcpClientCTS.Token).ConfigureAwait(false);

                // 使用更大的缓冲区提高传输效率
                byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024); // 使用 1MB 缓冲区
                try
                {
                    int bytesRead;
                    long totalBytesRead = 0;
                    var lastReportTime = DateTime.MinValue;

                    while ((bytesRead = await fileStream.ReadAsync(buffer, tcpClientCTS.Token).ConfigureAwait(false)) > 0)
                    {
                        await stream.WriteAsync(buffer.AsMemory(0, bytesRead), tcpClientCTS.Token).ConfigureAwait(false);
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
                        SendCanceled?.Invoke();
                    }
                    else
                    {
                        SendCompleted?.Invoke();
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("LAN Send cancel");
                SendCanceled?.Invoke();
            }
            catch (SocketException e)
            {
                _logger.LogError(e, "LAN Send error");
                ConnectFailed?.Invoke(e);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "LAN Send error");
                SendAborted?.Invoke();
            }
            finally
            {
                tcpClientCTS?.Cancel();
                tcpClientCTS?.Dispose();
                tcpClientCTS = null;
            }
        }

        public void CancelSearch()
        {
            udpClientCTS?.Cancel();
        }

        public void CancelSend()
        {
            tcpClientCTS?.Cancel();
        }

        private void StartCleanupThread()
        {
            if (udpClientCTS is null) return;

            Task.Run(async () =>
            {
                using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(DiscoveryInterval));

                try
                {
                    while (await timer.WaitForNextTickAsync(udpClientCTS.Token).ConfigureAwait(false))
                    {
                        CleanupDevices();
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    _devices.Clear();
                }
            });
        }

        private void CleanupDevices()
        {
            lock (_devicesLock)
            {
                var timeouted = _devices.Values
                    .Where(d => (DateTime.Now - d.LastSeen).TotalMilliseconds > DeviceTimeout)
                    .ToList();

                foreach (var device in timeouted)
                {
                    if (device.IPAddress is null)
                    {
                        continue;
                    }

                    bool isRemoved = _devices.Remove(device.IPAddress);
                    if (isRemoved)
                    {
                        DeviceTimeouted?.Invoke(device);
                    }
                }
            }
        }
    }
}
