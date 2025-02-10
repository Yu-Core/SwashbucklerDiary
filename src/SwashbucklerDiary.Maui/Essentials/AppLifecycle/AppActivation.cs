using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static partial class AppActivation
    {
        public static ActivationArguments? Arguments { get; set; }

        public static Action<ActivationArguments>? OnActivated { get; set; }
    }
}
