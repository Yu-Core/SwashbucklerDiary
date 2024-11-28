using Foundation;
using SwashbucklerDiary.Maui.Essentials;
using UIKit;

namespace SwashbucklerDiary.Maui
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            if (LaunchActivation.Activated is null)
            {
                LaunchActivation.HandleOnLaunched(url);
            }
            else
            {
                LaunchActivation.OnApplicationActivated(url);
            }

            return true;
            //return base.OpenUrl(application, url, options);
        }
    }
}