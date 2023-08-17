using Microsoft.AspNetCore.Components;
using Serilog;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SwashbucklerDiary.Pages
{
    //自我批评一下，此处代码写的太乱
    public partial class LANReceiver : PageComponentBase, IDisposable
    {
        private bool _udpSending;
        private UdpClient? udpClient;
        private readonly IPAddress multicastAddress = IPAddress.Parse("239.0.0.1");// UDP组播地址
        private int multicastPort;// UDP组播端口

        private bool _tcpListening;
        private bool ShowTransferDialog;
        private bool Transferred;
        private bool StopTransferr;
        private TcpListener? tcpListener;
        private int tcpPort;// Tcp端口
        //private CancellationTokenSource CancellationTokenSource = new();
        private int Ps;
        private long TotalBytesToReceive;
        private long BytesReceived;

        private LANDeviceInfo LANDeviceInfo = new();

        [Inject]
        private ILANService LANService { get; set; } = default!;
        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        public void Dispose()
        {
            StopTransferr = true;
            UDPSending = false;
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient.Dispose();
            }

            TCPListening = false;
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }

            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            SendMulticast();
            _ = StartTcpListening();
            await base.OnInitializedAsync();
        }

        private bool IsCurrentPage => NavigateService.Navigation.Uri.Contains("/lanReceiver");
        private bool UDPSending
        {
            get => _udpSending && IsCurrentPage;
            set => _udpSending = value;
        }
        private bool TCPListening
        {
            get => _tcpListening && IsCurrentPage;
            set => _tcpListening = value;
        }

        private async Task LoadSettings()
        {
            LANDeviceInfo = LANService.GetLocalLANDeviceInfo();
            var deviceName = await SettingsService.Get(SettingType.LANDeviceName);
            if (!string.IsNullOrEmpty(deviceName))
            {
                LANDeviceInfo.DeviceName = deviceName;
            }

            multicastPort = await SettingsService.Get(SettingType.LANScanPort);
            tcpPort = await SettingsService.Get(SettingType.LANTransmissionPort);
        }

        private void SendMulticast()
        {
            if (UDPSending)
            {
                return;
            }

            if (!LANService.IsConnection())
            {
                AlertService.Error(I18n.T("lanSender.No network connection"));
                return;
            }

            var ipAddress = IPAddress.Parse(LANService.GetLocalIPv4());
            udpClient = new UdpClient(new IPEndPoint(ipAddress, multicastPort));
            UDPSending = true;
            Task.Run(async () =>
            {
                string jsonLANDeviceInfo = JsonSerializer.Serialize(LANDeviceInfo);
                byte[] data = Encoding.UTF8.GetBytes(jsonLANDeviceInfo);

                while (UDPSending)
                {
                    udpClient!.Send(data, data.Length, new IPEndPoint(multicastAddress, multicastPort));
                    await Task.Delay(1000);
                }
            });
        }

        private async Task StartTcpListening()
        {
            if (TCPListening)
            {
                return;
            }

            if (!LANService.IsConnection())
            {
                await AlertService.Error(I18n.T("lanSender.No network connection"));
                return;
            }

            _ = Task.Run(async () =>
            {
                var ipEndPoint = new IPEndPoint(IPAddress.Any, tcpPort);
                tcpListener = new(ipEndPoint);
                try
                {
                    tcpListener.Start();

                    using TcpClient handler = await tcpListener.AcceptTcpClientAsync();
                    await using NetworkStream stream = handler.GetStream();

                    await InvokeAsync(() =>
                    {
                        UDPSending = false;
                        ShowTransferDialog = true;
                        StateHasChanged();
                    });

                    byte[] fileSizeBytes = new byte[sizeof(long)];
                    stream.Read(fileSizeBytes, 0, fileSizeBytes.Length);
                    long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

                    var diaries = await LANService.LANReceiverAsync(stream, fileSize, ReceiveProgressChanged);
                    Transferred = true;
                    if (diaries == null || !diaries.Any())
                    {
                        await InvokeAsync(async () =>
                        {
                            await AlertService.Error(I18n.T("Export.Import.Fail"));
                        });
                        return;
                    }

                    await DiaryService.ImportAsync(diaries);
                    await InvokeAsync(async () =>
                    {
                        await AlertService.Success(I18n.T("lanReceiver.Receive successfully"));
                    });
                }
                catch (Exception e)
                {
                    if (IsCurrentPage)
                    {
                        NavigateToBack();
                    }

                    Log.Error($"{e.Message}\n{e.StackTrace}");
                    
                    if(ShowTransferDialog)
                    {
                        await InvokeAsync(async () =>
                        {
                            await AlertService.Error(I18n.T("lanReceiver.Receive failed"));
                        });
                    }
                }
                finally
                {
                    tcpListener.Stop();
                }
            });

        }

        private async Task ReceiveProgressChanged(long readLength, long allLength)
        {
            if (StopTransferr)
            {
                throw new Exception("Stop Transmission");
            }

            var c = (int)(readLength * 100 / allLength);

            if (c > 0 && c % 5 == 0) //刷新进度为每5%更新一次，过快的刷新会导致页面显示数值与实际不一致
            {
                Ps = c; //下载完成百分比
                BytesReceived = readLength / 1024; //当前已经下载的Kb
                TotalBytesToReceive = allLength / 1024; //文件总大小Kb
                await InvokeAsync(StateHasChanged);
            }
        }

        private void StopTransmission()
        {
            if (!Transferred)
            {
                StopTransferr = true;
            }
            else
            {
                NavigateToBack();
            }
        }
    }
}
