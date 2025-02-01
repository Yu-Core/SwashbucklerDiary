using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Soup;
using SwashbucklerDiary.Gtk.BlazorWebView;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using System.Web;
using WebKit;

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
    protected static readonly string AppOrigin = $"{AppHostScheme}://{AppHostAddress}/";

    internal static readonly Uri AppOriginUri = new(AppOrigin);
    protected const string MessageQueueId = "webview";
    string _contentRootRelativeToAppRoot;
    readonly string _hostPageRelativePath;
    UserScript? _script;
    protected WebView? _webview;
    private readonly ILogger _logger;
    private readonly Channel<string> _channel;
    private const string BlazorInitScript
        = $$"""
            window.__receiveMessageCallbacks = [];
            window.__dispatchMessageCallback = function(message) {
                window.__receiveMessageCallbacks.forEach(
                    function(callback)
                    {
                        try
                        {
                            callback(message);
                        }
                        catch { }
                    });
            };
            window.external = {
                sendMessage: function(message) {
                    window.webkit.messageHandlers.{{MessageQueueId}}.postMessage(message);
                },
                receiveMessage: function(callback) {
                    window.__receiveMessageCallbacks.push(callback);
                }
            };
            """;

    protected GtkWebViewManager(
        IServiceProvider provider,
        Dispatcher dispatcher,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        string contentRootRelativeToAppRoot,
        string hostPageRelativePath,
        ILogger logger
    )
        : base(provider, dispatcher, AppOriginUri, fileProvider, jsComponents, hostPageRelativePath)
    {
        _contentRootRelativeToAppRoot = contentRootRelativeToAppRoot;
        _hostPageRelativePath = hostPageRelativePath;
        _logger = logger;

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

        var intercept = LocalFileWebAccessHelper.InterceptCustomPathRequest(request);
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
        if (_webview is not { })
            return;

        if (!UriSchemeRequestHandlers.TryGetValue(_webview.Handle, out var uriSchemeHandler))
        {
            UriSchemeRequestHandlers.Add(_webview.Handle, (_hostPageRelativePath, TryGetResponseContentInternal));
        }
    }

    protected override void NavigateCore(Uri absoluteUri)
    {
        if (_webview is not { })
            return;

        _logger.NavigatingToUri(absoluteUri);

        _webview.LoadUri(absoluteUri.ToString());
    }

    protected virtual void Attach()
    {
        if (_webview is not { })
            throw new ArgumentException();

        if (!HandleUriSchemeRequestIsRegistered)
        {
            _webview.GetContext().RegisterUriScheme(AppHostScheme, HandleUriSchemeRequest);
            HandleUriSchemeRequestIsRegistered = true;
        }

        RegisterUriSchemeRequestHandler();

        _script = UserScript.New(
            BlazorInitScript,
            UserContentInjectedFrames.AllFrames,
            UserScriptInjectionTime.Start,
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        _webview.OnDestroy += (o, args) => Detach();

        var userContentManager = _webview.GetUserContentManager();
        userContentManager.AddScript(_script);

        userContentManager.OnScriptMessageReceived += (o, args) => MessageReceived(AppOriginUri, args.Value.ToString());

        userContentManager.RegisterScriptMessageHandler(MessageQueueId, null);
    }

    bool _detached = false;

    protected virtual void Detach()
    {
        if (_webview is not { })
            return;

        if (_detached)
            return;

        var userContentManager = _webview.GetUserContentManager();
        userContentManager.UnregisterScriptMessageHandler(MessageQueueId, null);
        userContentManager.RemoveScript(_script);
        UriSchemeRequestHandlers.Remove(_webview.Handle);

        _detached = true;
    }

    protected override void SendMessage(string message)
    {
        if (_webview is not { })
            return;

        _logger.LogDebug($"Dispatching `{message}`");

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
                _ = _webview?.EvaluateJavascriptAsync(script);
            }
        }
        catch (ChannelClosedException) { }
    }

    internal bool TryGetResponseContentInternal(
        string uri,
        bool allowFallbackOnHostPage,
        out int statusCode,
        out string statusMessage,
        out Stream content,
        out IDictionary<string, string> headers)
    {
        bool defaultResult
            = TryGetResponseContent(
                uri,
                allowFallbackOnHostPage,
                out statusCode,
                out statusMessage,
                out content,
                out headers);
        bool hotReloadedResult
            = StaticContentHotReloadManager.TryReplaceResponseContent(
                _contentRootRelativeToAppRoot,
                uri,
                ref statusCode,
                ref content,
                headers);
        return defaultResult || hotReloadedResult;
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        Detach();
        await base.DisposeAsyncCore();
    }

    private static (Gio.InputStream inputStream, int length) InputStreamNewFromStream(Stream content)
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
}