using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SwashbucklerDiary.Services
{
    public class LANService : ILANService
    {
        private readonly IAppDataService AppDataService;

        private readonly IPlatformService PlatformService;

        public LANService(IAppDataService appDataService, IPlatformService platformService)
        {
            AppDataService = appDataService;
            PlatformService = platformService;
        }

        public string GetIPPrefix(string ipAddress)
        {
            string[] ipParts = ipAddress.Split('.');

            StringBuilder ipPrefix = new();

            for (int i = 0; i < ipParts.Length - 1; i++)
            {
                ipPrefix.Append(ipParts[i]);
                ipPrefix.Append('.');
            }

            return ipPrefix.ToString();
        }

        public string GetLocalIPv4()
        {
            try
            {
                Socket s = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.Connect(new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53));
                string ip = ((IPEndPoint)s.LocalEndPoint!).Address.ToString();
                s.Close();
                return ip;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                return string.Empty;
            }
        }

        public bool Ping(IPAddress address)
        {
            try
            {
                Ping ping = new();
                PingReply reply = ping.Send(address, 1000);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        public LANDeviceInfo GetLocalLANDeviceInfo()
        {
            string deviceName = GetLocalDeviceName();
            string ipAddress = GetLocalIPv4();
            DevicePlatformType platformType = PlatformService.GetDevicePlatformType();

            return new()
            {
                DeviceName = deviceName,
                IPAddress = ipAddress,
                DevicePlatform = platformType
            };
        }

        public string GetLocalDeviceName()
        {
            return DeviceInfo.Current.Name;
        }

        public bool IsConnection()
        {
            Microsoft.Maui.Networking.NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType == Microsoft.Maui.Networking.NetworkAccess.None)
            {
                return false;
            }

            return true;
        }

        public async Task LANSendAsync(List<DiaryModel> diaries, Stream stream, Func<long, long, Task> func)
        {
            var filePath = await AppDataService.ExportJsonZipFileAsync(diaries);

            using FileStream fileStream = File.OpenRead(filePath);
            // 发送文件总大小
            long fileSize = fileStream.Length;
            byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
            stream.Write(fileSizeBytes, 0, fileSizeBytes.Length);

            // 发送文件内容
            byte[] buffer = new byte[1024 * 1024];
            int bytesRead;
            var readLength = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                readLength += bytesRead;
                await func(readLength, fileSize);
                stream.Write(buffer, 0, bytesRead);
            }
        }

        public async Task<List<DiaryModel>> LANReceiverAsync(Stream stream, long size, Func<long, long, Task> func)
        {
            var path = Path.Combine(FileSystem.CacheDirectory, Guid.NewGuid().ToString() + ".zip");
            using (FileStream fileStream = File.Create(path))
            {
                byte[] buffer = new byte[1024 * 1024];
                int bytesRead;
                var readLength = 0;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    readLength += bytesRead;
                    await func(readLength, size);
                    fileStream.Write(buffer, 0, bytesRead);
                }
            }

            var diaries = await AppDataService.ImportJsonFileAsync(path);
            File.Delete(path);
            return diaries;
        }
    }
}
