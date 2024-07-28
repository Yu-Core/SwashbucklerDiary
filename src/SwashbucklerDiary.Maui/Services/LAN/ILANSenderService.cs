using SwashbucklerDiary.Maui.Models;

namespace SwashbucklerDiary.Maui.Services
{
    public interface ILANSenderService
    {
        bool IsSearching { get; }

        bool IsSending { get; }

        event Action<LANDeviceInfo>? LANDeviceFound;

        event Action? SearchEnded;

        event Action<long, long>? SendProgressChanged;

        event Action? SendAborted;

        event Action? SendCompleted;

        void Initialize(string multicastAddress, int multicastPort, int tcpPort, int millisecondsOutTime);

        void Dispose();

        void SearchDevices();

        void CancelSearch();

        void Send(string ipAddress, string filePath);

        void CancelSend();
    }
}
