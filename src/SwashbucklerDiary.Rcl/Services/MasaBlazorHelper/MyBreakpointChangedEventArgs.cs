namespace SwashbucklerDiary.Rcl.Services
{
    public class MyBreakpointChangedEventArgs
    {
        public bool XsChanged { get; set; }

        public bool SmChanged { get; set; }

        public bool MdChanged { get; set; }

        public bool LgChanged { get; set; }

        public bool XlChanged { get; set; }

        public bool SmAndDownChanged { get; set; }

        public bool SmAndUpChanged { get; set; }

        public bool MdAndDownChanged { get; set; }

        public bool MdAndUpChanged { get; set; }

        public bool LgAndDownChanged { get; set; }

        public bool LgAndUpChanged { get; set; }
    }
}
