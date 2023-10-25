using SwashbucklerDiary.Models;
using System.Net;

namespace SwashbucklerDiary.IServices
{
    public interface ILANService
    {
        bool IsConnection();

        string GetLocalIPv4();

        string GetIPPrefix(string ipAddress);

        bool Ping(IPAddress address);

        LANDeviceInfo GetLocalLANDeviceInfo();

        string GetLocalDeviceName();

        Task LANSendAsync(List<DiaryModel> diaries, Stream stream, Func<long, long, Task> func);

        Task<List<DiaryModel>> LANReceiverAsync(Stream stream, long size, Func<long, long, Task> func);
    }
}
