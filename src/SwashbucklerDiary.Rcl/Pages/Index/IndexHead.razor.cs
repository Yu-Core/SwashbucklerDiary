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
            int hour = DateTime.Now.Hour;
            if (hour >= 6 && hour < 11)
            {
                return I18n.T("As dawn breaks, may your day be full of energy!")!;
            }
            else if (hour >= 11 && hour < 13)
            {
                return I18n.T("With the warm sun, may your mood be as bright as the clear sky!")!;
            }
            else if (hour >= 13 && hour < 18)
            {
                return I18n.T("In the afternoon hours, may you feel relaxed and fulfilled!")!;
            }
            else if (hour >= 18 && hour < 23)
            {
                return I18n.T("As night falls, may you shed fatigue and unwind!")!;
            }
            else
            {
                return I18n.T("In the quiet of the night, may you sleep peacefully and welcome a wonderful tomorrow!")!;
            }
        }
    }
}