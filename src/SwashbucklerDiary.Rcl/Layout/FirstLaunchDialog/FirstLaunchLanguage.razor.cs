using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class FirstLaunchLanguage
    {
        private readonly List<DynamicListItem> LanguageListItems = [];

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public EventCallback<string> OnClick { get; set; }

        private string ShowClass => Show ? "" : "d-none";

        protected override void OnInitialized()
        {
            foreach (var language in I18n.Languages)
            {
                var item = new DynamicListItem(this, language.Key, string.Empty, () => OnClick.InvokeAsync(language.Value));
                LanguageListItems.Add(item);
            };

            base.OnInitialized();
        }
    }
}
