using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Essentials
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

        public static void LaunchOrActivate(string[] args)
        {
            if (!AppLifecycle.Default.IsLaunched)
            {
                Launch(args);
            }
            else
            {
                Activate(args);
            }
        }

        static void Launch(string[] args)
        {
            AppLifecycle.Default.ActivationArguments = CreateArguments(args);
        }

        static void Activate(string[] args)
        {
            var activationArguments = CreateArguments(args);
            AppLifecycle.Default.Activate(activationArguments);
        }

        private static ActivationArguments CreateArguments(string[] args)
        {
            foreach (var arg in args)
            {
                if (SchemeConstants.Schemes.Any(arg.StartsWith))
                {
                    return HandleScheme(arg);
                }
            }

            return new ActivationArguments() { Kind = AppActivationKind.Launch };
        }

        private static ActivationArguments HandleScheme(string arg)
        {
            return new()
            {
                Kind = AppActivationKind.Scheme,
                Data = arg
            };
        }
    }
}
