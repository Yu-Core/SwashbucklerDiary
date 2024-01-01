using System.Reflection;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string> GetAppVersion()
        {
            var assembly = typeof(PlatformIntegration).Assembly;
            if (assembly == null)
            {
                return Task.FromResult(string.Empty);
            }

            var assemblyFileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (assemblyFileVersionAttribute == null)
            {
                return Task.FromResult(string.Empty);
            }

            return Task.FromResult(assemblyFileVersionAttribute.Version);
        }

        public Task ShowSettingsUI()
        {
            return _alertService.Info(_i18n.T("ShowSettingsUI"));
        }
    }
}
