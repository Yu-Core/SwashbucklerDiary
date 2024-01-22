using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MarkdownEdit : MyComponentBase
    {
        private Dictionary<string, object>? _options;

        private MMarkdown MMarkdown = default!;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private VditorJSModule Module { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? WrapClass { get; set; }

        private bool Show => _options is not null;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetOptions();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await AfterMarkdownRender();
            }
        }

        private void SetOptions()
        {
            string lang = I18n.Culture.Name.Replace("-", "_");
            string theme = Dark ? "dark" : "light";
            var previewTheme = new Dictionary<string, object?>()
            {
                { "current", theme },
                { "path", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.9.8/dist/css/content-theme" }
            };
            var previewMarkdown = new Dictionary<string, object?>()
            {
                { "mark", true }
            };
            var preview = new Dictionary<string, object?>()
            {
                { "theme", previewTheme },
                { "markdown", previewMarkdown },
            };
            var link = new Dictionary<string, object?>()
            {
                { "isOpen", false }
            };
            var btnImage = new Dictionary<string, object?>()
            {
                { "hotkey", "⇧⌘I" },
                { "name", "image" },
                { "tipPosition", "n" },
                { "tip", I18n.T("Vditor.AddImage") },
                { "className", "" },
                { "icon", "<svg><use xlink:href=\"#vditor-icon-image\"></use></svg>" },
            };

            var btnAudio = new Dictionary<string, object?>()
            {
                { "hotkey", "⇧⌘A" },
                { "name", "audio" },
                { "tipPosition", "n" },
                { "tip", I18n.T("Vditor.AddAudio") },
                { "className", "" },
                { "icon", "<svg><use xlink:href=\"#vditor-icon-audio\"></use></svg>" },
            };
            var btnVideo = new Dictionary<string, object?>()
            {
                { "hotkey", "⇧⌘V" },
                { "name", "video" },
                { "tipPosition", "n" },
                { "tip", I18n.T("Vditor.AddVideo") },
                { "className", "" },
                { "icon", "<svg><use xlink:href=\"#vditor-icon-video\"></use></svg>" },
            };

            _options = new()
            {
                { "mode", "ir" },
                { "toolbar", new object[]{"headings", "bold", "italic", "strike", "line", "quote","list", "ordered-list" , "check", "code","inline-code","link",btnImage,btnAudio,btnVideo}},
                { "placeholder", I18n.T("Write.ContentPlace")! },
                { "cdn", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.9.8" },
                { "lang", lang },
                { "icon","material" },
                { "theme", theme },
                { "preview", preview },
                { "link", link }
            };
        }

        private async Task AfterMarkdownRender()
        {
            await Task.Delay(1000);
            await Module.PreventInputLoseFocus();
        }

        private async void HandleToolbarButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "image":
                    await AddImageAsync();
                    break;
                case "audio":
                    await AddAudioAsync();
                    break;
                case "video":
                    await AddVideoAsync();
                    break;
            }
        }

        private async Task AddImageAsync()
        {
            string? src = await MediaResourceManager.AddImageAsync();
            if (string.IsNullOrEmpty(src))
            {
                return;
            }

            string html = $"![]({src})";
            await InsertValueAsync(html);
        }

        private async Task AddAudioAsync()
        {
            string? src = await MediaResourceManager.AddAudioAsync();
            if (string.IsNullOrEmpty(src))
            {
                return;
            }

            string html = $"<audio src=\"{src}\" controls ></audio>";
            await InsertValueAsync(html);
        }

        private async Task AddVideoAsync()
        {
            string? src = await MediaResourceManager.AddVideoAsync();
            if (string.IsNullOrEmpty(src))
            {
                return;
            }

            string html = $"<video src=\"{src}\" controls ></video>";
            await InsertValueAsync(html);
        }

        public async Task InsertValueAsync(string value)
        {
            await Module.Focus(MMarkdown.Ref);
            await MMarkdown.InsertValueAsync(value);
            await Module.MoveCursorForward(value.Length);
        }
    }
}
