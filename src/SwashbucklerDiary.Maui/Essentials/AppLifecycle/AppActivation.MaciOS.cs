using Foundation;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static partial class AppActivation
    {
        public static void LaunchOrActivate(NSUrl nSUrl)
        {
            if (!AppLifecycle.Default.IsLaunched)
            {
                Launch(nSUrl);
            }
            else
            {
                Activate(nSUrl);
            }
        }

        static void Launch(NSUrl nSUrl)
        {
            AppLifecycle.Default.ActivationArguments = CreateArgumentsFromNSUrl(nSUrl);
        }

        static void Activate(NSUrl nSUrl)
        {
            var activationArguments = CreateArgumentsFromNSUrl(nSUrl);
            AppLifecycle.Default.NotifyActivated(activationArguments);
        }

        private static ActivationArguments CreateArgumentsFromNSUrl(NSUrl nSUrl)
        {
            var urlString = nSUrl.ToString();
            if (SchemeConstants.Schemes.Any(urlString.StartsWith))
            {
                return HandleScheme(nSUrl);
            }

            return new ActivationArguments() { Kind = AppActivationKind.Launch };
        }

        private static ActivationArguments HandleScheme(NSUrl nSUrl)
        {
            return new()
            {
                Kind = AppActivationKind.Scheme,
                Data = nSUrl?.ToString()
            };
        }

        // private static ActivationArguments HandleScheme(NSUrl nSUrl)
        // {
        // }
    }
}
