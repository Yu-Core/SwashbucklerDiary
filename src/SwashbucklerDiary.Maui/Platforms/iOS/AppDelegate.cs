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
            if (AppActivation.OnActivated is null)
            {
                AppActivation.Launch(url);
            }
            else
            {
                AppActivation.Activate(url);
            }

            return true;
            //return base.OpenUrl(application, url, options);
        }
    }
}