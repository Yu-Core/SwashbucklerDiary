using System.Reflection;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public string AppVersion
            => _versionTracking.CurrentVersion.ToString();

        public Task ShowSettingsUI()
        {
            return _alertService.Info(_i18n.T("ShowSettingsUI"));
        }
    }
}
