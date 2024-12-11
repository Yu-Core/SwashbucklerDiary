using Foundation;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static partial class LaunchActivation
    {
        public static void OnLaunched(NSUrl nSUrl)
        {
            ActivationArguments = ConvertActivationArguments(nSUrl);
        }

        public static void OnActivated(NSUrl nSUrl)
        {
            var activationArguments = ConvertActivationArguments(nSUrl);
            Activated?.Invoke(activationArguments);
        }

        private static ActivationArguments ConvertActivationArguments(NSUrl nSUrl)
        {
            var urlString = nSUrl.ToString();
            if (SchemeConstants.Schemes.Any(urlString.StartsWith))
            {
                return HandleScheme(nSUrl);
            }

            return new ActivationArguments() { Kind = LaunchActivationKind.Launch };
        }

        private static ActivationArguments HandleScheme(NSUrl nSUrl)
        {
            return new()
            {
                Kind = LaunchActivationKind.Scheme,
                Data = nSUrl?.ToString()
            };
        }

        // private static ActivationArguments HandleScheme(NSUrl nSUrl)
        // {
        // }
    }
}
