namespace SwashbucklerDiary.Rcl.Services
{
    public class MyBreakpointChangedEventArgs
    {
        public bool XsChanged { get; init; }
        public bool SmChanged { get; init; }
        public bool MdChanged { get; init; }
        public bool LgChanged { get; init; }
        public bool XlChanged { get; init; }
        public bool SmAndDownChanged { get; init; }
        public bool SmAndUpChanged { get; init; }
        public bool MdAndDownChanged { get; init; }
        public bool MdAndUpChanged { get; init; }
        public bool LgAndDownChanged { get; init; }
        public bool LgAndUpChanged { get; init; }
    }
}
