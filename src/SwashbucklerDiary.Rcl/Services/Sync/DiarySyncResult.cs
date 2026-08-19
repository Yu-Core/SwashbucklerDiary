namespace SwashbucklerDiary.Rcl.Services
{
    public class DiarySyncResult
    {
        public int Pushed { get; set; }

        public int Pulled { get; set; }

        public int Deleted { get; set; }

        public int Skipped { get; set; }

        public int Conflicts { get; set; }
    }
}
