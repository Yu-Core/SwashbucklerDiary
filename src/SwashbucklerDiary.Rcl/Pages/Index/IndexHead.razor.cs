using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class IndexHead
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Parameter]
        public bool ShowDate { get; set; }

        [Parameter]
        public bool ShowWelcomeText { get; set; }

        private string GetWelcomeText()
        {
            int hour = Convert.ToInt16(DateTime.Now.ToString("HH"));
            if (hour >= 6 && hour < 11)
            {
                return I18n.T("Index.Welcome.Morning")!;
            }
            else if (hour >= 11 && hour < 13)
            {
                return I18n.T("Index.Welcome.Noon")!;
            }
            else if (hour >= 13 && hour < 18)
            {
                return I18n.T("Index.Welcome.Afternoon")!;
            }
            else if (hour >= 18 && hour < 23)
            {
                return I18n.T("Index.Welcome.Night")!;
            }
            else if (hour >= 23 || hour < 6)
            {
                return I18n.T("Index.Welcome.BeforeDawn")!;
            }

            return "Hello World";
        }
    }
}