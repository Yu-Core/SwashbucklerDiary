namespace SwashbucklerDiary.Shared
{
    public class LogModel
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public int Level { get; set; }

        public string? LevelName { get; set; }

        public string? Message { get; set; }

        public string? MessageTemplate { get; set; }

        public string? Exception { get; set; }

        public string? Properties { get; set; }

        public string? SourceContext { get; set; }

        public string? MachineName { get; set; }

        public int ThreadId { get; set; }
    }
}
