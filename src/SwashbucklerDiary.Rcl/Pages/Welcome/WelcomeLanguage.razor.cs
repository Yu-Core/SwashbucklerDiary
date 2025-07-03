using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class WelcomeLanguage
    {
        private bool loading;

        private bool disabled;

        private string? selectedLanguage;

        private List<DynamicListItem<CultureInfo>> languageListItems = [];

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public EventCallback<string> OnClick { get; set; }

        private string Class => new CssBuilder()
            .Add("justify-center")
            .Add("align-center")
            .Add("fullscreen-height")
            .Add("d-flex", Show)
            .Add("d-none", !Show)
            .ToString();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetLanguageListItems();
        }

        private void SetLanguageListItems()
        {
            var languageListItems = new List<DynamicListItem<CultureInfo>>();
            var list = I18n.SupportedCultures.OrderByDescending(it => it.Name);
            foreach (var culture in list)
            {
                var item = new DynamicListItem<CultureInfo>(this, string.Empty, string.Empty, _ => HandleOnOK(culture.Name), culture);
                languageListItems.Add(item);
            }

            this.languageListItems = languageListItems;
        }

        private async Task HandleOnOK(string language)
        {
            loading = true;
            disabled = true;
            selectedLanguage = language;

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(language);
            }

            loading = false;
            disabled = false;
            selectedLanguage = null;
        }
    }
}
