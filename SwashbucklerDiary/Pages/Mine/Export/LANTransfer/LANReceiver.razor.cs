using SwashbucklerDiary.Components;
using System.Net.Sockets;
using System.Net;
using System.Text;
using SwashbucklerDiary.Models;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using System.Text.Json;

namespace SwashbucklerDiary.Pages
{
    public partial class LANReceiver : PageComponentBase,IDisposable
    {
        private bool _udpSending;
        private UdpClient? udpClient;
        private IPAddress multicastAddress = IPAddress.Parse("239.0.0.1");// UDP组播地址
        private int multicastPort = 5299;// UDP组播端口
        private LANDeviceInfo LANDeviceInfo = new();

        [Inject]
        private ILANService LANService { get; set; } = default!;

        public void Dispose()
        {
            UDPSending = false;
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            udpClient = new UdpClient();
            LANDeviceInfo = LANService.GetLocalLANDeviceInfo();
            SendMulticast();
        }

        private bool IsCurrentPage => NavigateService.Navigation.Uri.Contains("/lanReceiver");
        private bool UDPSending
        {
            get => _udpSending && IsCurrentPage;
            set => _udpSending = value;
        }

        private void SendMulticast()
        {
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

    }
}
