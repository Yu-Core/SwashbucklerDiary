using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public static partial class LaunchActivation
    {
        public static ActivationArguments? ActivationArguments { get; set; }

        public static Action<ActivationArguments>? Activated { get; set; }
    }
}
