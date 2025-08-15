using System.Net.Sockets;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ILANSenderService
    {
        bool IsSearching { get; }

        bool IsSending { get; }

        event Action<LANDeviceInfo> DeviceDiscovered;
        event Action<LANDeviceInfo> DeviceTimeouted;
        event Action? SearchEnded;
        event Action? SendCanceled;
        event Action? SendAborted;
        event Action? SendCompleted;
        event Action<SocketException>? ConnectFailed;

        void Dispose();

        void Start(string multicastAddress, int multicastPort, int millisecondsOutTime, int tcpPort);

        void Search();

        void CancelSearch();

        Task SendAsync(string ipAddress, string filePath, IProgress<TransferProgressArguments> progress);

        void CancelSend();
    }
}
