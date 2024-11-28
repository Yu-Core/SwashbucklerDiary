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

        // options always is null
        // public override bool WillFinishLaunching(UIApplication app, NSDictionary options)
        // {
        //     if (options != null)
        //     {
        //         NSObject urlObject;
        //         if (options.TryGetValue(UIApplication.LaunchOptionsUrlKey, out urlObject))
        //         {
        //             var url = urlObject as NSUrl;

        //             // Examine the url here

        //             LaunchActivation.HandleOnLaunched(url);
        //         }
        //     }

        //     return base.WillFinishLaunching(app, options);
        // }
    }
}