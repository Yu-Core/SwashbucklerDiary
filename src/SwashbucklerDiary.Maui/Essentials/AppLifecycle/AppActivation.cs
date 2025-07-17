using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static partial class AppActivation
    {
        public static void LaunchOrActivate(ActivationArguments arguments)
        {
            if (!AppLifecycle.Default.IsLaunched)
            {
                AppLifecycle.Default.ActivationArguments = arguments;
            }
            else
            {
                AppLifecycle.Default.Activate(arguments);
            }
        }
    }
}
