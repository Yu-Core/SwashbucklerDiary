namespace SwashbucklerDiary.Rcl.Services
{
    public readonly struct TransferProgressArguments(long transferred, long total)
    {
        public long TransferredBytes { get; } = transferred;
        public long TotalBytes { get; } = total;
    }
}
