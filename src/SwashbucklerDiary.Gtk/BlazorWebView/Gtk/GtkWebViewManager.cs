using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Soup;
using SwashbucklerDiary.Gtk.BlazorWebView;
using System.Web;
using WebKit;

namespace Microsoft.AspNetCore.Components.WebView.Gtk;
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

public partial class GtkWebViewManager : Microsoft.AspNetCore.Components.WebView.WebViewManager
{
    protected const string AppHostAddress = "localhost";

    protected static readonly string AppHostScheme = "app";

    /// <summary>
    /// Gets the application's base URI. Defaults to <c>app://localhost/</c>
    /// </summary>
    protected static readonly string AppOrigin = $"{AppHostScheme}://{AppHostAddress}/";

    internal static readonly Uri AppOriginUri = new(AppOrigin);
    const string MessageQueueId = "webview";
    readonly string _contentRootRelativeToAppRoot;
    readonly string _hostPageRelativePath;
    UserScript _script = default!;
    private readonly WebKit.WebView _webview;
    private readonly ILogger _logger;
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

    private readonly Action<UrlLoadingEventArgs>? _urlLoading;
    private readonly Action<BlazorWebViewInitializingEventArgs>? _blazorWebViewInitializing;
    private readonly Action<BlazorWebViewInitializedEventArgs>? _blazorWebViewInitialized;
    private readonly BlazorWebViewDeveloperTools _developerTools;

    /// <summary>
    /// Constructs an instance of <see cref="GtkWebViewManager"/>.
    /// </summary>
    /// <param name="webview">A <see cref="WebKit.WebView"/> to access platform-specific WebView2 APIs.</param>
    /// <param name="services">A service provider containing services to be used by this class and also by application code.</param>
    /// <param name="dispatcher">A <see cref="Dispatcher"/> instance that can marshal calls to the required thread or sync context.</param>
    /// <param name="fileProvider">Provides static content to the webview.</param>
    /// <param name="jsComponents">Describes configuration for adding, removing, and updating root components from JavaScript code.</param>
    /// <param name="contentRootRelativeToAppRoot">Path to the app's content root relative to the application root directory.</param>
    /// <param name="hostPagePathWithinFileProvider">Path to the host page within the <paramref name="fileProvider"/>.</param>
    /// <param name="urlLoading">Callback invoked when a url is about to load.</param>
    /// <param name="blazorWebViewInitializing">Callback invoked before the webview is initialized.</param>
    /// <param name="blazorWebViewInitialized">Callback invoked after the webview is initialized.</param>
    internal GtkWebViewManager(
        WebKit.WebView webview,
        IServiceProvider services,
        Dispatcher dispatcher,
        IFileProvider fileProvider,
        JSComponentConfigurationStore jsComponents,
        string contentRootRelativeToAppRoot,
        string hostPagePathWithinFileProvider,
        Action<UrlLoadingEventArgs> urlLoading,
        Action<BlazorWebViewInitializingEventArgs> blazorWebViewInitializing,
        Action<BlazorWebViewInitializedEventArgs> blazorWebViewInitialized,
        ILogger logger)
        : base(services, dispatcher, AppOriginUri, fileProvider, jsComponents, hostPagePathWithinFileProvider)

    {
        ArgumentNullException.ThrowIfNull(webview);

        if (services.GetService<GtkBlazorMarkerService>() is null)
        {
            throw new InvalidOperationException(
                "Unable to find the required services. " +
                $"Please add all the required services by calling '{nameof(IServiceCollection)}.{nameof(BlazorWebViewServiceCollectionExtensions.AddGtkBlazorWebView)}' in the application startup code.");
        }

        _logger = logger;
        _webview = webview;
        _urlLoading = urlLoading;
        _blazorWebViewInitializing = blazorWebViewInitializing;
        _blazorWebViewInitialized = blazorWebViewInitialized;
        _developerTools = services.GetRequiredService<BlazorWebViewDeveloperTools>();
        _contentRootRelativeToAppRoot = contentRootRelativeToAppRoot;
        _hostPageRelativePath = hostPagePathWithinFileProvider;

        Attach();
    }

    protected override void NavigateCore(Uri absoluteUri)
    {
        _logger.NavigatingToUri(absoluteUri);

        _webview.LoadUri(absoluteUri.ToString());
    }

    protected override void SendMessage(string message)
    {
        var script = $"__dispatchMessageCallback(\"{HttpUtility.JavaScriptStringEncode(message)}\")";

        _webview.EvaluateJavascriptAsync(script);
    }


    private void Attach()
    {
        _blazorWebViewInitializing?.Invoke(new BlazorWebViewInitializingEventArgs { });
        ApplyDefaultWebViewSettings(_developerTools);
        _blazorWebViewInitialized?.Invoke(new BlazorWebViewInitializedEventArgs { WebView = _webview });

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

        _webview.OnDecidePolicy += HandleOnDecidePolicy;
        _webview.OnDestroy += (o, args) => Detach();

        var userContentManager = _webview.GetUserContentManager();
        userContentManager.AddScript(_script);

        userContentManager.OnScriptMessageReceived += (o, args) => MessageReceived(AppOriginUri, args.Value.ToString());

        userContentManager.RegisterScriptMessageHandler(MessageQueueId, null);
    }

    private void ApplyDefaultWebViewSettings(BlazorWebViewDeveloperTools devTools)
    {
        Settings settings = _webview.GetSettings();
        settings.EnableDeveloperExtras = devTools.Enabled;
        settings.EnablePageCache = false;
        settings.EnableOfflineWebApplicationCache = false;
        _webview.SetSettings(settings);
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
    void HandleUriSchemeRequest(URISchemeRequest request)
    {
        if (!UriSchemeRequestHandlers.TryGetValue(request.GetWebView().Handle.DangerousGetHandle(), out var uriSchemeHandler))
        {
            throw new Exception($"Invalid scheme \"{request.GetScheme()}\"");
        }

        var intercept = LocalFileWebAccessHelper.InterceptCustomPathRequest(request);
        if (intercept)
        {
            return;
        }

        var requestUri = QueryStringHelper.RemovePossibleQueryString(request.GetUri());
        var allowFallbackOnHostPage = AppOriginUri.IsBaseOfPage(requestUri);

        _logger.HandlingWebRequest(requestUri);

        if (uriSchemeHandler.tryGetResponseContent(requestUri, allowFallbackOnHostPage, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers))
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

            _logger.ResponseContentBeingSent(requestUri, statusCode);

            request.FinishWithResponse(response);

            inputStream?.Dispose();
        }
        else
        {
            _logger.ReponseContentNotFound(requestUri);
        }
    }

    void RegisterUriSchemeRequestHandler()
    {
        if (!UriSchemeRequestHandlers.TryGetValue(_webview.Handle.DangerousGetHandle(), out var uriSchemeHandler))
        {
            UriSchemeRequestHandlers.Add(_webview.Handle.DangerousGetHandle(), (_hostPageRelativePath, TryGetResponseContentInternal));
        }
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

    private bool TryGetResponseContentInternal(
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

    private bool HandleOnDecidePolicy(WebKit.WebView sender, WebKit.WebView.DecidePolicySignalArgs args)
    {
        if (args.Decision is NavigationPolicyDecision navigationPolicyDecision)
        {
            return args.DecisionType switch
            {
                PolicyDecisionType.NavigationAction => HandleNavigationAction(navigationPolicyDecision),
                PolicyDecisionType.NewWindowAction => HandleNewWindowAction(navigationPolicyDecision),
                _ => false
            };
        }
        else if (args.Decision is ResponsePolicyDecision responsePolicyDecision)
        {
            if (responsePolicyDecision.IsMainFrameMainResource())
            {
                var uriString = responsePolicyDecision.GetRequest().GetUri();
                if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
                {
                    if (!AppOriginUri.IsBaseOf(uri))
                    {
                        responsePolicyDecision.Ignore();
                        return true;
                    }
                }

            }
        }

        return false;
    }

    private bool HandleNavigationAction(NavigationPolicyDecision args)
    {
        var uriString = args.NavigationAction.GetRequest().Uri;
        if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
        {
            var callbackArgs = UrlLoadingEventArgs.CreateWithDefaultLoadingStrategy(uri, AppOriginUri);
            var navigationType = args.NavigationAction.GetNavigationType();

            // <iframe> tags should open in webview
            if (navigationType == NavigationType.Other)
            {
                callbackArgs.UrlLoadingStrategy = UrlLoadingStrategy.OpenInWebView;
            }

            _urlLoading?.Invoke(callbackArgs);

            _logger.NavigationEvent(uri, callbackArgs.UrlLoadingStrategy);

            if (callbackArgs.UrlLoadingStrategy == UrlLoadingStrategy.OpenExternally)
            {
                LaunchUriInExternalBrowser(uri);
            }

            return callbackArgs.UrlLoadingStrategy != UrlLoadingStrategy.OpenInWebView;
        }

        return false;
    }

    private bool HandleNewWindowAction(NavigationPolicyDecision args)
    {
        var uriString = args.NavigationAction.GetRequest().Uri;

        // Intercept _blank target <a> tags to always open in device browser.
        // The ExternalLinkCallback is not invoked.
        if (Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
        {
            LaunchUriInExternalBrowser(uri);
            return true;
        }

        return false;
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
        UriSchemeRequestHandlers.Remove(_webview.Handle.DangerousGetHandle());

        _detached = true;
    }

    private void LaunchUriInExternalBrowser(Uri uri)
    {
        _logger.LaunchExternalBrowser(uri);

        using var launchBrowser = new System.Diagnostics.Process();
        launchBrowser.StartInfo.UseShellExecute = true;
        launchBrowser.StartInfo.FileName = "xdg-open";
        launchBrowser.StartInfo.Arguments = uri.ToString();
        launchBrowser.Start();
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        Detach();
        await base.DisposeAsyncCore();
    }
}