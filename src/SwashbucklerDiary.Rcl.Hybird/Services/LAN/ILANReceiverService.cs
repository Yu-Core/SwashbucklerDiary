using System.Net.Sockets;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ILANReceiverService
    {
        event Action? ReceiveStart;
        event Action? ReceiveCanceled;
        event Action? ReceiveAborted;
        event Action<string>? ReceiveCompleted;
        event Action<SocketException>? ConnectFailed;

        bool IsMulticasting { get; }

        bool IsReceiving { get; }

        void Start(string multicastAddress, int multicastPort, string? deviceName, int tcpPort, IProgress<TransferProgressArguments> progress);

        void Dispose();

        void CancelMulticast();

        void CancelReceive();
    }
}
