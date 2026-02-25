namespace SwashbucklerDiary.Server.Layout
{
    public partial class SetAppLockAlertDialog
    {
        private async Task ToSet()
        {
            await InternalVisibleChanged(false);
            To("appLockSetting");
        }
    }
}