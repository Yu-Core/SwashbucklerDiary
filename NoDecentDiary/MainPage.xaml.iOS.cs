using Microsoft.AspNetCore.Components.WebView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            e.Configuration.AllowsInlineMediaPlayback = true;
            e.Configuration.MediaTypesRequiringUserActionForPlayback = WebKit.WKAudiovisualMediaTypes.None;
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.ScrollView.ShowsVerticalScrollIndicator = false;
        }
    }
}
