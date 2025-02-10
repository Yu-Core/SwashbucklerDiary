using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public static partial class AppActivation
    {
        public static ActivationArguments? Arguments { get; set; }

        public static Action<ActivationArguments>? OnActivated { get; set; }

        public static void Launch(string[] args)
        {
            Arguments = CreateArguments(args);
        }

        public static void Activate(string[] args)
        {
            var activationArguments = CreateArguments(args);
            OnActivated?.Invoke(activationArguments);
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
