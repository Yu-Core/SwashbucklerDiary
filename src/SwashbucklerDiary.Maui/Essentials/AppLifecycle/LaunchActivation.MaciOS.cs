using Foundation;
using SwashbucklerDiary.Maui.Extensions;
using SwashbucklerDiary.Rcl.Essentials;
using System.Reflection;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static partial class LaunchActivation
    {
        public static void HandleOnLaunched(NSUrl nSUrl)
        {
            ActivationArguments = ConvertActivationArguments(nSUrl);
        }

        public static void OnApplicationActivated(NSUrl nSUrl)
        {
            var activationArguments = ConvertActivationArguments(nSUrl);
            Activated?.Invoke(activationArguments);
        }

        private static ActivationArguments ConvertActivationArguments(NSUrl nSUrl)
        {
            var urlString = nSUrl.ToString();
            if(urlString.StartsWith("swashbucklerdiary")||urlString.StartsWith("xiakeriji")){
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
