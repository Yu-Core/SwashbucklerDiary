using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MarkdownPreviewOptions
    {
        public ElementReference? OutlineElement { get; set; }
        public ElementReference? MoblieOutlineElement { get; set; }
        public bool AutoPlay { get; set; }
        public string? LinkBase { get; set; }
        public string? ProxyUrl { get; set; }
        public bool LinkCard { get; set; }
        public string[] Schemes { get; set; } = [];
    }
}
