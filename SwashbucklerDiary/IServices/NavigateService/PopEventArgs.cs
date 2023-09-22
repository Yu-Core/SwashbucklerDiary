namespace SwashbucklerDiary.IServices
{
    public class PopEventArgs
    {
        public PopEventArgs(string previousUri, string nextUri)
        {
            PreviousUri = previousUri;
            NextUri = nextUri;
        }

        public string PreviousUri { get; set; }
        public string NextUri { get; set; }
    }
}
