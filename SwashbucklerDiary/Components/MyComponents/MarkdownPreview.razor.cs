﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MarkdownPreview
    {
        private ElementReference element;
        private string? _value;
        private Dictionary<string, object> _options = new();
        private bool AfterFirstRender;
        private IJSObjectReference? module;

        [Inject]
        private II18nService I18n { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        private IJSRuntime JS { get; set; } = default!;
        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [Parameter]
        public string? Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? Style { get; set; }

        [JSInvokable]
        public async Task After()
        {
            //点击复制按钮提示复制成功
            var dotNetCallbackRef = DotNetObjectReference.Create(this);
            await module!.InvokeVoidAsync("Copy", new object[2] { dotNetCallbackRef, "CopySuccess" });
            //修复点击链接的一些错误
            await module!.InvokeVoidAsync("FixLink", new object[1] { element });
        }

        [JSInvokable]
        public async Task CopySuccess()
        {
            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AfterFirstRender = true;
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/vditor-helper.js");
                await RenderingMarkdown(Value);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void SetValue(string? value)
        {
            if (_value != value)
            {
                _value = value;
                if(AfterFirstRender)
                {
                    Task.Run(async () =>
                    {
                        await RenderingMarkdown(value);
                    });
                }
            }
        }

        private async Task RenderingMarkdown(string? value)
        {
            if(element.Context == null)
            {
                return;
            }

            if(string.IsNullOrEmpty(value))
            {
                return;
            }

            await SetOptions();
            var dotNetCallbackRef = DotNetObjectReference.Create(this);
            await module!.InvokeVoidAsync("PreviewVditor", new object?[4] { dotNetCallbackRef, element,  value, _options });
        }

        private async Task SetOptions()
        {
            string mode = ThemeService.Dark ? "dark" : "light";
            string lang = await SettingsService.Get(SettingType.Language);
            lang = lang.Replace("-", "_");
            var theme = new Dictionary<string, object?>() 
            { 
                { "current", ThemeService.Dark ? "dark" : "light" },
                { "path", "npm/vditor/3.9.3/dist/css/content-theme" } 
            };

            _options = new()
            {
                { "mode", mode },
                { "cdn", "npm/vditor/3.9.3" },
                { "lang", lang },
                { "theme", theme },
            };
        }
    }
}
