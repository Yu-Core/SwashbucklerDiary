using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class FirstLaunchLanguage
    {
        private List<DynamicListItem> LanguageListItems = [];

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public EventCallback<string> OnClick { get; set; }

        private string ShowClass => Show ? "scroll-show px-4" : "d-none";

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetLanguageListItems();
        }

        private void SetLanguageListItems()
        {
            var languageListItems = new List<DynamicListItem>();
            foreach (var culture in I18n.SupportedCultures.OrderByDescending(it => it.Name))
            {
                var item = new DynamicListItem(this, culture.NativeName, string.Empty, () => OnClick.InvokeAsync(culture.Name));
                languageListItems.Add(item);
            }
            ;

            LanguageListItems = languageListItems;
        }
    }
}
