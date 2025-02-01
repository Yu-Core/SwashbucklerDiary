using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Soup;
using SwashbucklerDiary.Gtk.BlazorWebView;
using SwashbucklerDiary.Shared;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using System.Web;
using WebKit;
using File = System.IO.File;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference argument.

namespace GtkSharp.BlazorWebKit;

[SuppressMessage("ApiDesign", "RS0016:Öffentliche Typen und Member der deklarierten API hinzufügen")]
public partial class GtkWebViewManager : Microsoft.AspNetCore.Components.WebView.WebViewManager
{

    protected const string AppHostAddress = "localhost";

    protected static readonly string AppHostScheme = "app";

    /// <summary>
    /// Gets the application's base URI. Defaults to <c>app://localhost/</c>
    /// </summary>
    protected static string AppOrigin(string appHostScheme, string appHostAddress = AppHostAddress) => $"{appHostScheme}://{appHostAddress}/";

    public static readonly Uri AppOriginUri = new(AppOrigin(AppHostScheme, AppHostAddress));

    protected Task<bool>? WebviewReadyTask;

    protected string MessageQueueId = "webview";

    string _hostPageRelativePath;
    Uri _appBaseUri;

    UserScript? _script;

    public delegate void WebMessageHandler(IntPtr contentManager, IntPtr jsResult, IntPtr arg);

    public WebView? WebView { get; protected set; }

    protected ILogger<GtkWebViewManager>? Logger;

    private readonly Channel<string> _channel;

    protected GtkWebViewManager(IServiceProvider provider, Dispatcher dispatcher, Uri appBaseUri, IFileProvider fileProvider, JSComponentConfigurationStore jsComponents, string hostPageRelativePath) :
        base(provider, dispatcher, appBaseUri, fileProvider, jsComponents, hostPageRelativePath)
    {
        _appBaseUri = appBaseUri;
        _hostPageRelativePath = hostPageRelativePath;

        // https://github.com/DevToys-app/DevToys/issues/1194
        // Forked from https://github.com/tryphotino/photino.Blazor/issues/40
        //Create channel and start reader
        _channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });
        Task.Run(SendMessagePump);
    }

    delegate bool TryGetResponseContentHandler(string uri, bool allowFallbackOnHostPage, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers);

    static readonly Dictionary<IntPtr, (string _hostPageRelativePath, TryGetResponseContentHandler tryGetResponseContent)> UriSchemeRequestHandlers = new();

    static bool HandleUriSchemeRequestIsRegistered = false;

    /// <summary>
    /// RegisterUriScheme can only called once per scheme
    /// so it's needed to have a list of all WebViews registered
    /// </summary>
    /// <param name="request"></param>
    /// <exception cref="Exception"></exception>
    static void HandleUriSchemeRequest(URISchemeRequest request)
    {
        if (!UriSchemeRequestHandlers.TryGetValue(request.GetWebView().Handle, out var uriSchemeHandler))
        {
            throw new Exception($"Invalid scheme \"{request.GetScheme()}\"");
        }

        var intercept = InterceptCustomPathRequest(request);
        if (intercept)
        {
            return;
        }

        // fix https://github.com/jsuarezruiz/maui-linux/issues/100
        var uri = QueryStringHelper.RemovePossibleQueryString(request.GetUri());

        if (request.GetPath() == "/")
        {
            uri += uriSchemeHandler._hostPageRelativePath;
        }

        if (uriSchemeHandler.tryGetResponseContent(uri, false, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers))
        {
            var (inputStream, length) = InputStreamNewFromStream(content);

            var response = URISchemeResponse.New(inputStream, length);

            response.SetContentType(headers["Content-Type"]);
            response.SetStatus((uint)statusCode, statusMessage);

            var messageHeaders = MessageHeaders.New(MessageHeadersType.Response);
            messageHeaders.SetContentLength(length);
            // Disable local caching. This will prevent user scripts from executing correctly.
            messageHeaders.Append("Cache-Control", "no-cache, max-age=0, must-revalidate, no-store");
            response.SetHttpHeaders(messageHeaders);

            request.FinishWithResponse(response);

            inputStream?.Dispose();
        }
        else
        {
            throw new Exception($"Failed to serve \"{uri}\". {statusCode} - {statusMessage}");
        }
    }

    void RegisterUriSchemeRequestHandler()
    {
        if (WebView is not { })
            return;

        if (!UriSchemeRequestHandlers.TryGetValue(WebView.Handle, out var uriSchemeHandler))
        {
            UriSchemeRequestHandlers.Add(WebView.Handle, (_hostPageRelativePath, TryGetResponseContent));
        }
    }

    protected override void NavigateCore(Uri absoluteUri)
    {
        if (WebView is not { })
            return;

        Logger?.LogInformation($"Navigating to \"{absoluteUri}\"");
        var loadUri = absoluteUri.ToString();

        WebView.LoadUri(loadUri);
    }

    public string JsScript(string messageQueueId) =>
        """
		window.__receiveMessageCallbacks = [];

		window.__dispatchMessageCallback = function(message) {
		   window.__receiveMessageCallbacks.forEach(function(callback) { callback(message); });
		};

		window.external = {
		   sendMessage: function(message) {
		"""
        +
        $"""
		        window.webkit.messageHandlers.{MessageQueueId}.postMessage(message);
		 """
        +
        """
		   },
		   receiveMessage: function(callback) {
		       window.__receiveMessageCallbacks.push(callback);
		   }
		};
		""";

    protected virtual void Attach()
    {
        if (WebView is not { })
            throw new ArgumentException();

        if (!HandleUriSchemeRequestIsRegistered)
        {
            WebView.GetContext().RegisterUriScheme(AppHostScheme, HandleUriSchemeRequest);
            HandleUriSchemeRequestIsRegistered = true;
        }

        RegisterUriSchemeRequestHandler();

        var jsScript = JsScript(MessageQueueId);

        _script = UserScript.New(
            jsScript,
            UserContentInjectedFrames.AllFrames,
            UserScriptInjectionTime.Start,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        WebView.OnDestroy += (o, args) => Detach();

        WebView.UserContentManager.AddScript(_script);

        WebView.UserContentManager.OnScriptMessageReceived += (o, args) =>
        {
            var jsValue = args.Value;

            if (!jsValue.IsString())
                return;

            var s = jsValue.ToString();

            if (s is not null)
            {
                Logger?.LogDebug($"Received message `{s}`");

                try
                {
                    MessageReceived(_appBaseUri, s);
                }
                finally
                { }
            }
        };

        WebView.UserContentManager.RegisterScriptMessageHandler(MessageQueueId, null);
    }

    bool _detached = false;

    protected virtual void Detach()
    {
        if (WebView is not { })
            return;

        if (_detached)
            return;

        WebView.UserContentManager.UnregisterScriptMessageHandler(MessageQueueId, null);
        WebView.UserContentManager.RemoveScript(_script);
        UriSchemeRequestHandlers.Remove(WebView.Handle);

        _detached = true;
    }

    protected override void SendMessage(string message)
    {
        if (WebView is not { })
            return;

        Logger?.LogDebug($"Dispatching `{message}`");

        var script = $"__dispatchMessageCallback(\"{HttpUtility.JavaScriptStringEncode(message)}\")";

        // https://github.com/DevToys-app/DevToys/issues/1194
        // Forked from https://github.com/tryphotino/photino.Blazor/issues/40
        while (!_channel.Writer.TryWrite(script))
        {
            Thread.Sleep(200);
        }
    }

    private async Task SendMessagePump()
    {
        // https://github.com/DevToys-app/DevToys/issues/1194
        // Forked from https://github.com/tryphotino/photino.Blazor/issues/40
        ChannelReader<string> reader = _channel.Reader;
        try
        {
            while (true)
            {
                string script = await reader.ReadAsync();
                _ = WebView?.EvaluateJavascriptAsync(script);
            }
        }
        catch (ChannelClosedException) { }
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        Detach();
        await base.DisposeAsyncCore();
    }

    public static (Gio.InputStream inputStream, int length) InputStreamNewFromStream(Stream content)
    {
        using var memoryStream = new MemoryStream();
        var length = (int)content.Length;
        content.CopyTo(memoryStream, length);
        var buffer = memoryStream.GetBuffer();
        Array.Resize(ref buffer, length);
        var bytes = GLib.Bytes.New(buffer);
        var inputStream = Gio.MemoryInputStream.NewFromBytes(bytes);

        return (inputStream, length);
    }

    private static bool InterceptCustomPathRequest(URISchemeRequest request)
    {
        string uri = request.GetUri();
        if (!InterceptLocalFileRequest(uri, out string filePath))
        {
            return false;
        }

        using var contentStream = File.OpenRead(filePath);
        string contentType = StaticContentProvider.GetResponseContentTypeOrDefault(filePath);
        var length = contentStream.Length;
        int statusCode = 200;
        string statusMessage = "OK";
        long rangeStart = 0;
        long rangeEnd = length - 1;
        var messageHeaders = MessageHeaders.New(MessageHeadersType.Response);

        //适用于音频视频文件资源的响应
        string? rangeString = request.GetHttpHeaders().GetOne("Range");
        bool partial = !string.IsNullOrEmpty(rangeString);
        if (partial)
        {
            //206,可断点续传
            statusCode = 206;
            statusMessage = "Partial Content";

            ParseRange(rangeString, ref rangeStart, ref rangeEnd);
            messageHeaders.Append("Accept-Ranges", "bytes");
            messageHeaders.Append("Content-Range", $"bytes {rangeStart}-{rangeEnd}/{length}");
        }

        var bytes = ReadStreamRange(contentStream, rangeStart, rangeEnd);
        var inputStream = InputStreamNewFromBytes(bytes);
        var response = URISchemeResponse.New(inputStream, bytes.LongLength);

        response.SetStatus((uint)statusCode, statusMessage);
        messageHeaders.SetContentLength(bytes.LongLength);
        // Disable local caching. This will prevent user scripts from executing correctly.
        messageHeaders.Append("Cache-Control", "no-cache, max-age=0, must-revalidate, no-store");
        messageHeaders.Append("Content-Type", contentType);
        response.SetHttpHeaders(messageHeaders);

        request.FinishWithResponse(response);

        inputStream?.Dispose();
        return true;
    }

    private static bool InterceptLocalFileRequest(string uri, out string filePath)
    {
        if (!uri.StartsWith(AppOriginUri.ToString()))
        {
            filePath = string.Empty;
            return false;
        }

        var urlRelativePath = new Uri(uri).AbsolutePath.TrimStart('/');
        filePath = LocalFileWebAccessHelper.UrlRelativePathToFilePath(urlRelativePath);
        if (!File.Exists(filePath))
        {
            return false;
        }

        return true;
    }

    private static int ParseRange(string rangeString, ref long rangeStart, ref long rangeEnd)
    {
        var ranges = rangeString.Split('=');
        if (ranges.Length < 2 || string.IsNullOrEmpty(ranges[1]))
        {
            return 0;
        }

        string[] rangeDatas = ranges[1].Split("-");
        rangeStart = Convert.ToInt64(rangeDatas[0]);
        if (rangeDatas.Length > 1 && !string.IsNullOrEmpty(rangeDatas[1]))
        {
            rangeEnd = Convert.ToInt64(rangeDatas[1]);
            return 2;
        }
        else
        {
            return 1;
        }
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
        int bytesRead = contentStream.Read(byteArray, 0, (int)(numberOfBytesToRead));
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

    static Gio.MemoryInputStream InputStreamNewFromBytes(byte[] buffer)
    {
        var bytes = GLib.Bytes.New(buffer);
        return Gio.MemoryInputStream.NewFromBytes(bytes);
    }
}