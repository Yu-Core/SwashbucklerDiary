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
    public partial class LANSender : PageComponentBase, IDisposable
    {
        private UdpClient? udpClient;
        private readonly IPAddress multicastAddress = IPAddress.Parse("239.0.0.1"); // 组播地址
        private  int multicastPort; // 组播端口
        private readonly int millisecondsOutTime = 20000;
        private bool _udpListening;
        private bool JoinMulticastGroup;
        private CancellationTokenSource CancellationTokenSource = new();
        private readonly List<LANDeviceInfo> LANDeviceInfos = new ();

        private bool ShowTransferDialog;
        private bool Transferred;
        private bool StopTransferr;
        private int tcpPort;
        private int Ps;
        private long TotalBytesToReceive;
        private long BytesReceived;

        [Inject]
        private ILANService LANService { get; set; } = default!;
        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;
        public void Dispose()
        {
            StopTransferr = true;
            UDPListening = false;
            if (udpClient != null)
            {
                udpClient.DropMulticastGroup(multicastAddress);
                udpClient.Close();
                udpClient.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            udpClient = new UdpClient(multicastPort);
            StartUDPListening();
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender)
            {
                if (!UDPListening)
                {
                    CancellationTokenSource.Cancel();
                }
            }
        }

        private bool IsCurrentPage => NavigateService.Navigation.Uri.Contains("lanSender");

        private bool UDPListening
        {
            get => _udpListening && IsCurrentPage;
            set => _udpListening = value;
        }

        private string GetDeviceIcon(DevicePlatformType devicePlatform)
            => LANService.GetDevicePlatformTypeIcon(devicePlatform);

        private async Task LoadSettings()
        {
            multicastPort = await SettingsService.Get(SettingType.LANScanPort);
            tcpPort = await SettingsService.Get(SettingType.LANTransmissionPort);
        }

        private void StartUDPListening()
        {
            if (UDPListening)
            {
                return;
            }

            if (!LANService.IsConnection())
            {
                AlertService.Error(I18n.T("lanSender.No network connection"));
                return;
            }

            if (!JoinMulticastGroup)
            {
                JoinMulticastGroup = true;
                var ipAddress = IPAddress.Parse(LANService.GetLocalIPv4());
                udpClient!.JoinMulticastGroup(multicastAddress, ipAddress);
            }

            UDPListening = true;
            CancellationTokenSource = new(millisecondsOutTime);
            Task.Run(async () =>
            {
                while (UDPListening)
                {
                    try
                    {
                        var receivedResults = await udpClient!.ReceiveAsync(CancellationTokenSource.Token);
                        var jsonLANDeviceInfo = Encoding.UTF8.GetString(receivedResults.Buffer);
                        LANDeviceInfo? deviceInfo = JsonSerializer.Deserialize<LANDeviceInfo>(jsonLANDeviceInfo);
                        if (deviceInfo is null)
                        {
                            continue;
                        }

                        if (!LANDeviceInfos.Any(it => it.IPAddress == deviceInfo.IPAddress))
                        {
                            LANDeviceInfos.Add(deviceInfo);
                            await InvokeAsync(StateHasChanged);
                        }
                    }
                    catch (JsonException)
                    {
                        continue;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                UDPListening = false;
                await InvokeAsync(StateHasChanged);
            });
        }

        private void Send(LANDeviceInfo deviceInfo)
        {
            UDPListening = false;
            ShowTransferDialog = true;
            StateHasChanged();

            Task.Run(async () =>
            {
                var ipAddress = deviceInfo.IPAddress;
                try
                {
                    using TcpClient client = new(ipAddress!, tcpPort);
                    await using NetworkStream stream = client.GetStream();

                    var diaries = await DiaryService.QueryAsync();
                    await LANService.LANSendAsync(diaries, stream, SendProgressChanged);

                    Transferred = true;
                    await InvokeAsync(async () =>
                    {
                        await AlertService.Success(I18n.T("lanSender.Send successfully"));
                    });
                }
                catch (Exception e)
                {
                    if (IsCurrentPage)
                    {
                        NavigateToBack();
                    }

                    Log.Error($"{e.Message}\n{e.StackTrace}");
                    if (ShowTransferDialog)
                    {
                        await InvokeAsync(async () =>
                        {
                            await AlertService.Error(I18n.T("lanSender.Send failed"));
                        });
                    }
                }
            });
        }

        private async Task SendProgressChanged(long readLength, long allLength)
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
