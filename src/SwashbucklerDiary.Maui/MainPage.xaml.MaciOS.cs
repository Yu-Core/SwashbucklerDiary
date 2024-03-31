using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Maui.Platform;
using SwashbucklerDiary.Rcl;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            e.Configuration.AllowsInlineMediaPlayback = true;
            //e.Configuration.MediaTypesRequiringUserActionForPlayback = WebKit.WKAudiovisualMediaTypes.None;
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.ScrollView.ShowsVerticalScrollIndicator = false; // 关闭滚动条
            e.WebView.BackgroundColor = Color.FromArgb(ThemeColor.LightSurface).ToPlatform();
        }
    }
}
