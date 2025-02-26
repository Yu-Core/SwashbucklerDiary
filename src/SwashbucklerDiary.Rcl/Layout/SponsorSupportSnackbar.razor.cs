using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class SponsorSupportSnackbar : DialogComponentBase
    {
        private async Task ToSupport()
        {
            await InternalVisibleChanged(false);
            To("sponsor");
        }
    }
}