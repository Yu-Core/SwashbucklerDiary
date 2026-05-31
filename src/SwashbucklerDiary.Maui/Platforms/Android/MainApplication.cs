using Android.App;
using Android.Runtime;

namespace SwashbucklerDiary.Maui
{
    [Application(
#if DEBUG
        Label = "@string/app_name_debug"
#else
        Label = "@string/app_name"
#endif
        )]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}