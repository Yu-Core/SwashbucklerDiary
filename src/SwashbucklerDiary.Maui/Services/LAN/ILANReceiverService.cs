namespace SwashbucklerDiary.Maui.Services
{
    public interface ILANReceiverService
    {
        event Action<long, long>? ReceiveProgressChanged;

        event Action? ReceiveStart;

        event Action? ReceiveAborted;

        event Action<string>? ReceiveCompleted;

        bool IsMulticasting { get; }

        bool IsReceiving { get; }

        void Initialize(string multicastAddress, int multicastPort, int tcpPort, string? deviceName);

        void Dispose();

        void Multicast();

        void Receive();

        void CancelMulticast();

        void CancelReceive();
    }
}
