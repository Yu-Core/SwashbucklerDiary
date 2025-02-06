using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public class LANSenderService : ILANSenderService
    {
        public event Action<LANDeviceInfo>? LANDeviceFound;

        public event Action? SearchEnded;

        public event Action<long, long>? SendProgressChanged;

        public event Action? SendAborted;

        public event Action? SendCompleted;

        private UdpClient? udpClient;

        private IPAddress? _multicastAddress; // UDP组播地址

        private int _multicastPort; // UDP组播端口

        private int _tcpPort; //TCP端口

        private int _millisecondsOutTime = 20000;

        private CancellationTokenSource? udpClientCTS;

        private CancellationTokenSource? tcpClientCTS;

        private readonly ILogger<LANSenderService> _logger;

        public LANSenderService(ILogger<LANSenderService> logger)
        {
            _logger = logger;
        }

        public bool IsSearching => !udpClientCTS?.IsCancellationRequested ?? false;

        public bool IsSending => !tcpClientCTS?.IsCancellationRequested ?? false;

        public void Initialize(string multicastAddress, int multicastPort, int tcpPort, int millisecondsOutTime)
        {
            _multicastAddress = IPAddress.Parse(multicastAddress);
            _multicastPort = multicastPort;
            _tcpPort = tcpPort;
            _millisecondsOutTime = millisecondsOutTime;

            udpClient = new UdpClient(_multicastPort);
            var ipAddress = IPAddress.Parse(LANHelper.GetLocalIPv4Address());
            udpClient.JoinMulticastGroup(_multicastAddress, ipAddress);
            SearchDevices();
        }

        public void Dispose()
        {
            udpClientCTS?.Cancel();
            tcpClientCTS?.Cancel();
            if (udpClient is not null)
            {
                try
                {
                    if (_multicastAddress is not null)
                    {
                        udpClient.DropMulticastGroup(_multicastAddress);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "DropMulticastGroup error");
                }
                finally
                {
                    udpClient.Close();
                    udpClient = null;
                }
            }
        }

        public void SearchDevices()
        {
            if (udpClient is null)
            {
                return;
            }

            if (udpClientCTS is not null && !udpClientCTS.IsCancellationRequested)
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
                        var receivedResults = await udpClient.ReceiveAsync(udpClientCTS.Token);
                        var jsonLANDeviceInfo = Encoding.UTF8.GetString(receivedResults.Buffer);
                        try
                        {
                            LANDeviceInfo? deviceInfo = JsonSerializer.Deserialize<LANDeviceInfo>(jsonLANDeviceInfo);
                            if (deviceInfo is null)
                            {
                                continue;
                            }

                            LANDeviceFound?.Invoke(deviceInfo);
                        }
                        catch (JsonException e)
                        {
                            _logger.LogInformation(e, "read LANDeviceInfo error");
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "stop search devices");
                }

                SearchEnded?.Invoke();
            });
        }

        public void Send(string ipAddress, string filePath)
        {
            if (tcpClientCTS is not null && !tcpClientCTS.IsCancellationRequested)
            {
                return;
            }

            tcpClientCTS = new();
            Task.Run(async () =>
            {
                try
                {
                    using var fileStream = File.OpenRead(filePath);

                    using var tcpClient = new TcpClient(ipAddress, _tcpPort);
                    await using NetworkStream stream = tcpClient.GetStream();

                    long fileSize = fileStream.Length;
                    string fileName = Path.GetFileName(filePath);

                    // 发送文件大小
                    byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
                    await stream.WriteAsync(fileSizeBytes, tcpClientCTS.Token);

                    // 发送文件名长度和文件名
                    byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                    byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
                    await stream.WriteAsync(fileNameLengthBytes, tcpClientCTS.Token);
                    await stream.WriteAsync(fileNameBytes, tcpClientCTS.Token);

                    // 发送文件内容
                    byte[] buffer = new byte[1024 * 1024];
                    int bytesRead;
                    var readLength = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        readLength += bytesRead;
                        await stream.WriteAsync(buffer.AsMemory(0, bytesRead), tcpClientCTS.Token);
                        SendProgressChanged?.Invoke(readLength, fileSize);
                    }

                    SendCompleted?.Invoke();
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e, "stop send");
                    SendAborted?.Invoke();
                }
                finally
                {
                    tcpClientCTS?.Cancel();
                }
            });
        }

        public void CancelSearch()
        {
            udpClientCTS?.Cancel();
        }

        public void CancelSend()
        {
            tcpClientCTS?.Cancel();
        }
    }
}
