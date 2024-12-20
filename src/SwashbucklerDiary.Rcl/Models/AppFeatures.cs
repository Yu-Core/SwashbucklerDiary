namespace SwashbucklerDiary.Rcl.Models
{
    public class AppFeatures
    {
        public string? Name { get; set; }

        public string? Icon { get; set; }

        public string? Path { get; set; }

        public string? Href { get; set; }

        public PlatformInfo[]? DisplayPlatforms { get; set; }

        public PlatformInfo[]? HidePlatforms { get; set; }

        public string[]? HideBreakpoints { get; set; }
    }

    public class PlatformInfo
    {
        public string? Platform { get; set; }

        public string? VersionString { get; set; }

        public Version? Version
            => VersionString is null ? null : new Version(VersionString);
    }
}
