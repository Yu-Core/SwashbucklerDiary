using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SwashbucklerDiary.Pages
{
    public partial class LANSender : PageComponentBase, IDisposable
    {
        private UdpClient? udpClient;
        private readonly IPAddress multicastAddress = IPAddress.Parse("239.0.0.1"); // 组播地址
        private readonly int multicastPort = 5299; // 组播端口
        private readonly int millisecondsOutTime = 20000;
        private bool _udpListening;
        private bool JoinMulticastGroup;
        private CancellationTokenSource CancellationTokenSource = new();
        private List<LANDeviceInfo> LANDeviceInfos = new List<LANDeviceInfo>();

        [Inject]
        private ILANService LANService { get; set; } = default!;

        public void Dispose()
        {
            UDPListening = false;
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            udpClient = new UdpClient(multicastPort);
            StartListening();
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

        private bool IsCurrentPage => NavigateService.Navigation.Uri.Contains("/lanSender");

        private bool UDPListening
        {
            get => _udpListening && IsCurrentPage;
            set => _udpListening = value;
        }

        private string GetDeviceIcon(DevicePlatformType devicePlatform)
            => LANService.GetDevicePlatformTypeIcon(devicePlatform);

        private void StartListening()
        {
            if(!LANService.IsConnection())
            {
                AlertService.Info(I18n.T("lanSender.No network connection"));
                return;
            }
            if(!JoinMulticastGroup)
            {
                udpClient!.JoinMulticastGroup(multicastAddress);
                JoinMulticastGroup = true;
            }
            if (UDPListening)
            {
                return;
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
    }
}
