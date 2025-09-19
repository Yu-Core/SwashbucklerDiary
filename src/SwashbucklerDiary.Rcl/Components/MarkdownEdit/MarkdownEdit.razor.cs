using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MarkdownEdit : MyComponentBase
    {
        private const long maxAllowedSize = 512 * 1024 * 1024;

        private bool firstLineIndent;

        private bool codeLineNumber;

        private bool taskListLineThrough;

        private bool rightOutline;

        private bool showAddTable;

        private Dictionary<string, object>? _options;

        private MMarkdown mMarkdown = default!;

        private InputFile? inputFile;

        private DotNetObjectReference<object>? _dotNetObjectReference;

        private ElementReference moblieOutlineElement;

        private MarkdownJSModule? jSModule;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private IAppFileSystem AppFileManager { get; set; } = default!;

        [Inject]
        private ILogger<MarkdownEdit> Logger { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? WrapClass { get; set; }

        [Parameter]
        public bool Autofocus { get; set; }

        [Parameter]
        public bool Outline { get; set; }

        [Parameter]
        public bool? MobileOutline { get; set; }

        [Parameter]
        public EventCallback<bool?> MobileOutlineChanged { get; set; }


        [Parameter]
        public EventCallback OnAfter { get; set; }

        [JSInvokable]
        public async Task OpenAddTableDialog()
        {
            showAddTable = true;
            await InvokeAsync(StateHasChanged);
        }

        [JSInvokable]
        public async Task CloseMobileOutline()
        {
            await InternalMobileOutlineChanged(false);
            await InvokeAsync(StateHasChanged);
        }

        [JSInvokable]
        public async Task SetClipboard(string text)
        {
            await PlatformIntegration.SetClipboardAsync(text);
        }

        public async Task InsertValueAsync(string value)
        {
            if (string.IsNullOrEmpty(Value))
            {
                await Focus();
            }

            await mMarkdown.InsertValueAsync(value);
        }

        public ValueTask<string?> GetValueAsync() => mMarkdown.GetValueAsync();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ReadSettings();
            SetOptions();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                jSModule = new(JS);
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
            await jSModule.TryDisposeAsync();
        }

        private void ReadSettings()
        {
            firstLineIndent = SettingService.Get(s => s.FirstLineIndent);
            codeLineNumber = SettingService.Get(s => s.CodeLineNumber);
            taskListLineThrough = SettingService.Get(s => s.TaskListLineThrough);
            rightOutline = SettingService.Get(s => s.RigthOutline);
        }

        private void SetOptions()
        {
            string lang = I18n.Culture.Name.Replace("-", "_");
            string theme = ThemeService.RealTheme == Shared.Theme.Dark ? "dark" : "light";
            var previewTheme = new Dictionary<string, object?>()
            {
                { "current", theme },
                { "path", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.2/dist/css/content-theme" }
            };
            var previewHljs = new Dictionary<string, object>()
            {
                { "lineNumber", codeLineNumber }
            };
            var previewMarkdown = new Dictionary<string, object?>()
            {
                { "toc", true },
                { "linkBase", MediaResourceManager.LinkBase },
                { "mark", true },
                { "sup", true },
                { "sub", true },
                { "imgPathAllowSpace", true }
            };
            var preview = new Dictionary<string, object?>()
            {
                { "theme", previewTheme },
                { "hljs", previewHljs },
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
                { "tip", I18n.T("Add image") },
                { "className", "" },
                { "icon", "<svg><use xlink:href=\"#vditor-icon-image\"></use></svg>" },
            };

            var btnAudio = new Dictionary<string, object?>()
            {
                { "hotkey", "⇧⌘A" },
                { "name", "audio" },
                { "tipPosition", "n" },
                { "tip", I18n.T("Add audio") },
                { "className", "" },
                { "icon", "<svg><use xlink:href=\"#vditor-icon-audio\"></use></svg>" },
            };
            var btnVideo = new Dictionary<string, object?>()
            {
                { "hotkey", "⇧⌘V" },
                { "name", "video" },
                { "tipPosition", "n" },
                { "tip", I18n.T("Add video") },
                { "className", "" },
                { "icon", "<svg><use xlink:href=\"#vditor-icon-video\"></use></svg>" },
            };
            var btnUpload = new Dictionary<string, object?>()
            {
                { "name", "upload" },
                { "className", "d-none" },
            };
            var btnTable = new Dictionary<string, object?>()
            {
                { "name", "table" },
                { "suffix", "" },
                { "prefix", "" }
            };
            string[] accept = ["image/*", "audio/*", "video/*"];
            var upload = new Dictionary<string, object?>()
            {
                { "max", maxAllowedSize },
                { "accept", accept },
            };
            var outlinePosition = rightOutline ? "right" : "left";
            var outline = new Dictionary<string, object?>()
            {
                { "enable", true },
                { "position", outlinePosition }
            };

            _options = new()
            {
                { "mode", "ir" },
                { "toolbar", new object[]{"headings", "bold", "italic", "strike", "line", "quote","list", "ordered-list", "check", "indent", "outdent", btnTable, "code", "inline-code", "link", btnImage, btnAudio, btnVideo, "undo", "redo", "fullscreen", btnUpload }},
                { "placeholder", I18n.T("Please enter content") },
                { "cdn", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.2" },
                { "lang", lang },
                { "icon","material" },
                { "theme", theme },
                { "preview", preview },
                { "link", link },
                { "typewriterMode", true },
                { "height", "100%" },
                { "upload", upload },
                { "outline", outline },
                { "undoDelay", 120 }
            };
        }

        private async Task AfterMarkdownRender()
        {
            if (jSModule is null)
            {
                return;
            }

            if (_dotNetObjectReference is not null)
            {
                await jSModule.After(_dotNetObjectReference, mMarkdown.Ref, moblieOutlineElement, PlatformIntegration.CurrentPlatform == AppDevicePlatform.macOS);
            }

            if (Autofocus)
            {
                await jSModule.FocusToEnd(mMarkdown.Ref);
            }

            if (OnAfter.HasDelegate)
            {
                await OnAfter.InvokeAsync();
            }
        }

        private async Task HandleToolbarButtonClick(string btnName)
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

        private Task AddImageAsync()
            => AddMediaFilesAsync(MediaResourceManager.AddMultipleImageAsync);

        private Task AddAudioAsync()
            => AddMediaFilesAsync(MediaResourceManager.AddMultipleAudioAsync);

        private Task AddVideoAsync()
            => AddMediaFilesAsync(MediaResourceManager.AddMultipleVideoAsync);

        private async Task AddMediaFilesAsync(Func<Task<IEnumerable<ResourceModel>?>> func)
        {
            AlertService.StartLoading();

            await Task.Run(async () =>
            {
                try
                {
                    var resources = await func.Invoke();
                    await AddMediaFilesAsync(resources);
                }
                catch (Exception e)
                {
                    await AlertService.ErrorAsync(I18n.T("Add failed"));
                    Logger.LogError(e, I18n.T("Add failed"));
                }
                finally
                {
                    AlertService.StopLoading();
                }
            });
        }

        private async Task AddMediaFilesAsync(IEnumerable<ResourceModel>? resources)
        {
            string? insertContent = MediaResourceManager.CreateMediaFilesInsertContent(resources);
            if (insertContent is null) return;

            await InsertValueAsync(insertContent);
        }

        private async Task HandleBeforeAllUpload()
        {
            if (jSModule is null)
            {
                return;
            }

            await jSModule.Upload(mMarkdown.Ref, inputFile?.Element);
        }

        private async Task LoadFiles(InputFileChangeEventArgs e)
        {
            AlertService.StartLoading();

            try
            {
                List<string?> filePaths = [];
                foreach (var browserFile in e.GetMultipleFiles())
                {
                    var kind = MediaResourceManager.GetResourceKind(browserFile.Name);
                    if (kind == MediaResource.Unknown)
                    {
                        continue;
                    }

                    try
                    {
                        var filePath = await AppFileManager.CreateTempFileAsync(browserFile.Name, browserFile.OpenReadStream(maxAllowedSize));
                        filePaths.Add(filePath);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogInformation(ex, "OpenReadStream error");
                    }
                }
                if (filePaths.Count == 0)
                {
                    AlertService.StopLoading();
                    return;
                }

                var insertContent = await MediaResourceManager.CreateMediaFilesInsertContentAsync(filePaths);
                if (string.IsNullOrEmpty(insertContent))
                {
                    AlertService.StopLoading();
                    return;
                }

                await InsertValueAsync(insertContent);
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task Focus()
        {
            if (jSModule is null)
            {
                return;
            }

            await jSModule.Focus(mMarkdown.Ref);
        }

        private async Task InternalMobileOutlineChanged(bool? value)
        {
            MobileOutline = value;
            if (MobileOutlineChanged.HasDelegate)
            {
                await MobileOutlineChanged.InvokeAsync(value);
            }
        }

        private async Task SetMoblieOutlineAsync()
        {
            if (jSModule is null)
            {
                return;
            }

            await jSModule.SetMoblieOutline(mMarkdown.Ref, moblieOutlineElement);
        }
    }
}
