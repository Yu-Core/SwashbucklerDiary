using Foundation;
using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Utilities;
using System.Globalization;
using System.Net.Mime;
using System.Runtime.Versioning;
using SystemConfiguration;
using WebKit;

namespace SwashbucklerDiary
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            e.Configuration.AllowsInlineMediaPlayback = true;
            e.Configuration.MediaTypesRequiringUserActionForPlayback = WebKit.WKAudiovisualMediaTypes.None;
            e.Configuration.SetUrlSchemeHandler(new AppDataSchemeHandler(), "appdata");
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.ScrollView.ShowsVerticalScrollIndicator = false; // 关闭滚动条
        }

#nullable disable
        private class AppDataSchemeHandler : NSObject, IWKUrlSchemeHandler
        {
            [Export("webView:startURLSchemeTask:")]
            [SupportedOSPlatform("ios11.0")]
            public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
                if (urlSchemeTask.Request.Url == null)
                {
                    return;
                }

                var path = urlSchemeTask.Request.Url?.Path ?? "";
                path = FileSystem.AppDataDirectory + path;
                if (File.Exists(path))
                {
                    byte[] responseBytes = File.ReadAllBytes(path);
                    string contentType = StaticContentProvider.GetResponseContentTypeOrDefault(path);
                    using (var dic = new NSMutableDictionary<NSString, NSString>())
                    {
                        dic.Add((NSString)"Content-Length", (NSString)(responseBytes.Length.ToString(CultureInfo.InvariantCulture)));
                        dic.Add((NSString)"Content-Type", (NSString)contentType);
                        // Disable local caching. This will prevent user scripts from executing correctly.
                        dic.Add((NSString)"Cache-Control", (NSString)"no-cache, max-age=0, must-revalidate, no-store");
                        
                        using var response = new NSHttpUrlResponse(urlSchemeTask.Request.Url, 200, "HTTP/1.1", dic);
                        urlSchemeTask.DidReceiveResponse(response);
                    }
                    
                    urlSchemeTask.DidReceiveData(NSData.FromArray(responseBytes));
                    urlSchemeTask.DidFinish();
                }
            }

            [Export("webView:stopURLSchemeTask:")]
            public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
            }
        }
    }
}
