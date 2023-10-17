namespace SwashbucklerDiary.IServices
{
    public class PushEventArgs
    {
        public PushEventArgs(string previousUri, string nextUri,bool isCachePrevious)
        {
            PreviousUri = previousUri;
            NextUri = nextUri;
            IsCachePrevious = isCachePrevious;
        }

        public string PreviousUri { get; set; }

        public string NextUri { get; set; }

        public bool IsCachePrevious { get; set; }
    }
}
