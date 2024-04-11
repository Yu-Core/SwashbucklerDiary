namespace SwashbucklerDiary.Shared
{
    public class AudioFileInfo
    {
        public string? Title { get; set; }

        public string[] Artists { get; set; } = [];

        public string? Album { get; set; }

        public TimeSpan Duration { get; set; }

        public string? PictureUri { get; set; }
    }
}
