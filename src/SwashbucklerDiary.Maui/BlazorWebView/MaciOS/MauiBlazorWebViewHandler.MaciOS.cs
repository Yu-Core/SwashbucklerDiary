using Foundation;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Shared;
using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;
using UIKit;
using WebKit;
using RectangleF = CoreGraphics.CGRect;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
#nullable disable
    public partial class MauiBlazorWebViewHandler
    {
        [SupportedOSPlatform("ios11.0")]
        protected override WKWebView CreatePlatformView()
        {
            Logger.CreatingWebKitWKWebView();

            var config = new WKWebViewConfiguration();

            // By default, setting inline media playback to allowed, including autoplay
            // and picture in picture, since these things MUST be set during the webview
            // creation, and have no effect if set afterwards.
            // A custom handler factory delegate could be set to disable these defaults
            // but if we do not set them here, they cannot be changed once the
            // handler's platform view is created, so erring on the side of wanting this
            // capability by default.
            if (OperatingSystem.IsMacCatalystVersionAtLeast(10) || OperatingSystem.IsIOSVersionAtLeast(10))
            {
                config.AllowsPictureInPictureMediaPlayback = true;
                config.AllowsInlineMediaPlayback = true;
                config.MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None;
            }

            VirtualView.BlazorWebViewInitializing(new BlazorWebViewInitializingEventArgs()
            {
                Configuration = config
            });

            config.UserContentController.AddScriptMessageHandler(CreateWebViewScriptMessageHandler(), "webwindowinterop");
            config.UserContentController.AddUserScript(new WKUserScript(
                new NSString(BlazorInitScript), WKUserScriptInjectionTime.AtDocumentEnd, true));

            // iOS WKWebView doesn't allow handling 'http'/'https' schemes, so we use the fake 'app' scheme
            config.SetUrlSchemeHandler(new SchemeHandler(this), urlScheme: "app");

            var webview = new WKWebView(RectangleF.Empty, config)
            {
                BackgroundColor = UIColor.Clear,
                AutosizesSubviews = true
            };

            if (GetDeveloperToolsEnabled())
            {
                // Legacy Developer Extras setting.
                config.Preferences.SetValueForKey(NSObject.FromObject(true), new NSString("developerExtrasEnabled"));

                if (OperatingSystem.IsIOSVersionAtLeast(16, 4) || OperatingSystem.IsMacCatalystVersionAtLeast(16, 6))
                {
                    // Enable Developer Extras for iOS builds for 16.4+ and Mac Catalyst builds for 16.6 (macOS 13.5)+
                    webview.SetValueForKey(NSObject.FromObject(true), new NSString("inspectable"));
                }
            }

            VirtualView.BlazorWebViewInitialized(CreateBlazorWebViewInitializedEventArgs(webview));

            Logger.CreatedWebKitWKWebView();

            return webview;
        }

        private class SchemeHandler : NSObject, IWKUrlSchemeHandler
        {
            private readonly MauiBlazorWebViewHandler _webViewHandler;

            public SchemeHandler(MauiBlazorWebViewHandler webViewHandler)
            {
                _webViewHandler = webViewHandler;
            }

            [Export("webView:startURLSchemeTask:")]
            [SupportedOSPlatform("ios11.0")]
            public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
                var intercept = InterceptCustomPathRequest(urlSchemeTask);
                if (intercept)
                {
                    return;
                }

                var responseBytes = GetResponseBytes(urlSchemeTask.Request.Url?.AbsoluteString ?? "", out var contentType, statusCode: out var statusCode);
                if (statusCode == 200)
                {
                    using (var dic = new NSMutableDictionary<NSString, NSString>())
                    {
                        dic.Add((NSString)"Content-Length", (NSString)responseBytes.Length.ToString(CultureInfo.InvariantCulture));
                        dic.Add((NSString)"Content-Type", (NSString)contentType);
                        // Disable local caching. This will prevent user scripts from executing correctly.
                        dic.Add((NSString)"Cache-Control", (NSString)"no-cache, max-age=0, must-revalidate, no-store");
                        if (urlSchemeTask.Request.Url != null)
                        {
                            using var response = new NSHttpUrlResponse(urlSchemeTask.Request.Url, statusCode, "HTTP/1.1", dic);
                            urlSchemeTask.DidReceiveResponse(response);
                        }

                    }
                    urlSchemeTask.DidReceiveData(NSData.FromArray(responseBytes));
                    urlSchemeTask.DidFinish();
                }
            }

            private byte[] GetResponseBytes(string? url, out string contentType, out int statusCode)
            {
                var allowFallbackOnHostPage = _webViewHandler.AppOriginUri.IsBaseOfPage(url);
                url = QueryStringHelper.RemovePossibleQueryString(url);

                _webViewHandler.Logger.HandlingWebRequest(url);

                if (_webViewHandler.TryGetResponseContentInternal(url, allowFallbackOnHostPage, out statusCode, out var statusMessage, out var content, out var headers))
                {
                    statusCode = 200;
                    using var ms = new MemoryStream();

                    content.CopyTo(ms);
                    content.Dispose();

                    contentType = headers["Content-Type"];

                    _webViewHandler?.Logger.ResponseContentBeingSent(url, statusCode);

                    return ms.ToArray();
                }
                else
                {
                    _webViewHandler?.Logger.ReponseContentNotFound(url);

                    statusCode = 404;
                    contentType = string.Empty;
                    return [];
                }
            }

            [Export("webView:stopURLSchemeTask:")]
            public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
            }

            private static bool InterceptCustomPathRequest(IWKUrlSchemeTask urlSchemeTask)
            {
                var requestUrl = urlSchemeTask.Request.Url;
                if (requestUrl == null)
                {
                    return false;
                }

                string uri = requestUrl.ToString();
                if (!InterceptLocalFileRequest(uri, out string filePath))
                {
                    return false;
                }

                using var contentStream = File.OpenRead(filePath);
                var length = contentStream.Length;
                long rangeStart = 0;
                long rangeEnd = length - 1;
                int statusCode = 200;

                string contentType = StaticContentProvider.GetResponseContentTypeOrDefault(filePath);
                using (var dic = new NSMutableDictionary<NSString, NSString>())
                {
                    bool partial = urlSchemeTask.Request.Headers.TryGetValue((NSString)"Range", out NSObject rangeNSObject);
                    if (partial)
                    {
                        statusCode = 206;
                        ParseRange(rangeNSObject.ToString(), ref rangeStart, ref rangeEnd);
                        dic.Add((NSString)"Accept-Ranges", (NSString)"bytes");
                        dic.Add((NSString)"Content-Range", (NSString)$"bytes {rangeStart}-{rangeEnd}/{length}");
                    }

                    dic.Add((NSString)"Content-Length", (NSString)(rangeEnd - rangeStart + 1).ToString());
                    dic.Add((NSString)"Content-Type", (NSString)contentType);
                    // Disable local caching. This will prevent user scripts from executing correctly.
                    dic.Add((NSString)"Cache-Control", (NSString)"no-cache, max-age=0, must-revalidate, no-store");
                    using var response = new NSHttpUrlResponse(requestUrl, statusCode, "HTTP/1.1", dic);
                    urlSchemeTask.DidReceiveResponse(response);
                }

                urlSchemeTask.DidReceiveData(NSData.FromArray(ReadStreamRange(contentStream, rangeStart, rangeEnd)));
                urlSchemeTask.DidFinish();
                return true;
            }

            static byte[] ReadStreamRange(FileStream contentStream, long start, long end)
            {
                // 检查结束位置是否大于开始位置
                if (end < start)
                {
                    throw new ArgumentException("结束位置必须大于开始位置");
                }

                // 计算需要读取的字节数
                long numberOfBytesToRead = end - start + 1;
                byte[] byteArray = new byte[numberOfBytesToRead];
                contentStream.Seek(start, SeekOrigin.Begin);
                int bytesRead = contentStream.Read(byteArray, 0, (int)numberOfBytesToRead);
                // 如果读取的字节数小于期望的字节数，说明到达了文件的末尾或发生了其他错误
                if (bytesRead < numberOfBytesToRead)
                {
                    // 创建一个新的缓冲区，只包含实际读取的字节
                    byte[] actualBuffer = new byte[bytesRead];
                    Array.Copy(byteArray, actualBuffer, bytesRead);
                    return actualBuffer;
                }
                return byteArray;
            }
        }
    }

    public partial class MauiBlazorWebViewHandler
    {
        private static readonly Type blazorWebViewHandlerType = typeof(BlazorWebViewHandler);

        private MethodInfo TryGetResponseContentInternalMethod;

        private readonly FieldInfo _webviewManagerField = blazorWebViewHandlerType.GetField("_webviewManager", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new Exception("Field _webviewManager does not exist");

        private readonly PropertyInfo LoggerProperty = blazorWebViewHandlerType.GetProperty("Logger", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new Exception("Property Logger does not exist");

        private readonly FieldInfo BlazorInitScriptField = blazorWebViewHandlerType.GetField("BlazorInitScript", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            ?? throw new Exception("Field BlazorInitScript does not exist");

        private readonly PropertyInfo AppOriginUriProperty = blazorWebViewHandlerType.GetProperty("AppOriginUri", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            ?? throw new Exception("Property AppOriginUri does not exist");

        private object WebviewManager => _webviewManagerField.GetValue(this);

        public ILogger Logger => (ILogger)LoggerProperty.GetValue(this);

        private string BlazorInitScript => (string)BlazorInitScriptField.GetValue(this);

        public Uri AppOriginUri => (Uri)AppOriginUriProperty.GetValue(this);

        private bool GetDeveloperToolsEnabled()
        {
            var developerToolsProperty = blazorWebViewHandlerType.GetProperty("DeveloperTools", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new Exception("Property DeveloperTools does not exist");
            var developerTools = developerToolsProperty.GetValue(this);

            var enabledProperty = developerTools.GetType().GetProperty("Enabled", BindingFlags.Public | BindingFlags.Instance)
                ?? throw new Exception("Property Enabled does not exist");
            return (bool)enabledProperty.GetValue(developerTools);
        }

        public IWKScriptMessageHandler CreateWebViewScriptMessageHandler()
        {
            Type webViewScriptMessageHandlerType = blazorWebViewHandlerType.GetNestedType("WebViewScriptMessageHandler", BindingFlags.NonPublic)
                ?? throw new Exception("Type WebViewScriptMessageHandler does not exist");

            // 获取 MessageReceived 方法信息
            MethodInfo messageReceivedMethod = blazorWebViewHandlerType.GetMethod("MessageReceived", BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new Exception("Method MessageReceived does not exist");

            // 创建 WebViewScriptMessageHandler 实例
            object webViewScriptMessageHandlerInstance = Activator.CreateInstance(webViewScriptMessageHandlerType, [Delegate.CreateDelegate(typeof(Action<Uri, string>), this, messageReceivedMethod)]);
            return (IWKScriptMessageHandler)webViewScriptMessageHandlerInstance;
        }

        private static BlazorWebViewInitializedEventArgs CreateBlazorWebViewInitializedEventArgs(WKWebView wKWebView)
        {
            var blazorWebViewInitializedEventArgs = new BlazorWebViewInitializedEventArgs();
            PropertyInfo property = typeof(BlazorWebViewInitializedEventArgs).GetProperty("WebView", BindingFlags.Public | BindingFlags.Instance)
                ?? throw new Exception("Property BlazorWebViewInitializedEventArgs.WebView does not exist");
            property.SetValue(blazorWebViewInitializedEventArgs, wKWebView);
            return blazorWebViewInitializedEventArgs;
        }

        public bool TryGetResponseContentInternal(string uri, bool allowFallbackOnHostPage, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers)
        {
            TryGetResponseContentInternalMethod ??= WebviewManager.GetType().GetMethod("TryGetResponseContentInternal", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new Exception("Method TryGetResponseContentInternal does not exist");

            // 定义参数
            object[] parameters = [uri, allowFallbackOnHostPage, 0, null, null, null];

            bool result = (bool)TryGetResponseContentInternalMethod.Invoke(WebviewManager, parameters);

            // 获取返回值和输出参数
            statusCode = (int)parameters[2];
            statusMessage = (string)parameters[3];
            content = (Stream)parameters[4];
            headers = (IDictionary<string, string>)parameters[5];
            return result;
        }

    }
}
