using Gtk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MauiDispatcher = Microsoft.Maui.Dispatching.Dispatcher;

namespace Microsoft.AspNetCore.Components.WebView.Gtk
{
    /// <summary>
    /// A Gtk Widget for hosting Razor components locally in Windows desktop applications.
    /// </summary>
    public class BlazorWebView : ScrolledWindow, IAsyncDisposable
    {
        private WebKit.WebView _webview;
        private GtkWebViewManager? _webviewManager;
        private string? _hostPage;
        private IServiceProvider? _services;
        private bool _isDisposed;

        /// <summary>
        /// Creates a new instance of <see cref="BlazorWebView"/>.
        /// </summary>
        public BlazorWebView()
        {
            WebKit.Module.Initialize();

            ComponentsDispatcher = new GtkDispatcher(MauiDispatcher.GetForCurrentThread());

            RootComponents.CollectionChanged += HandleRootComponentsCollectionChanged;

            _webview = new WebKit.WebView();
            _webview.OnMap += HandleOnMap;

            this.Child = _webview;
        }

        /// <summary>
        /// Returns the inner <see cref="WebView"/> used by this control.
        /// </summary>
        /// <remarks>
        /// Directly using some functionality of the inner web view can cause unexpected results because its behavior
        /// is controlled by the <see cref="BlazorWebView"/> that is hosting it.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WebKit.WebView WebView => _webview;

        private Dispatcher ComponentsDispatcher { get; }

        bool Created { get; set; }

        /// <inheritdoc />
        private void HandleOnMap(Widget widget, EventArgs args)
        {
            Created = true;

            StartWebViewCoreIfPossible();
        }

        /// <summary>
        /// Path to the host page within the application's static files. For example, <code>wwwroot\index.html</code>.
        /// This property must be set to a valid value for the Razor components to start.
        /// </summary>
        [Category("Behavior")]
        [Description(@"Path to the host page within the application's static files. Example: wwwroot\index.html.")]
        public string? HostPage
        {
            get => _hostPage;
            set
            {
                _hostPage = value;
                OnHostPagePropertyChanged();
            }
        }

        /// <summary>
        /// Path for initial Blazor navigation when the Blazor component is finished loading.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("/")]
        [Description(@"Path for initial Blazor navigation when the Blazor component is finished loading.")]
        public string StartPath { get; set; } = "/";

        // Learn more about these methods here: https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/defining-default-values-with-the-shouldserialize-and-reset-methods?view=netframeworkdesktop-4.8
        private void ResetHostPage() => HostPage = null;

        private bool ShouldSerializeHostPage() => !string.IsNullOrEmpty(HostPage);

        /// <summary>
        /// A collection of <see cref="RootComponent"/> instances that specify the Blazor <see cref="IComponent"/> types
        /// to be used directly in the specified <see cref="HostPage"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RootComponentsCollection RootComponents { get; } = new();

        /// <summary>
        /// Gets or sets an <see cref="IServiceProvider"/> containing services to be used by this control and also by application code.
        /// This property must be set to a valid value for the Razor components to start.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DisallowNull]
        public IServiceProvider Services
        {
            get => _services!;
            set
            {
                _services = value;
                OnServicesPropertyChanged();
            }
        }

        /// <summary>
        /// Allows customizing how links are opened.
        /// By default, opens internal links in the webview and external links in an external app.
        /// </summary>
        [Category("Action")]
        [Description("Allows customizing how links are opened. By default, opens internal links in the webview and external links in an external app.")]
        public EventHandler<UrlLoadingEventArgs>? UrlLoading;

        /// <summary>
        /// Allows customizing the web view before it is created.
        /// </summary>
        [Category("Action")]
        [Description("Allows customizing the web view before it is created.")]
        public EventHandler<BlazorWebViewInitializingEventArgs>? BlazorWebViewInitializing;

        /// <summary>
        /// Allows customizing the web view after it is created.
        /// </summary>
        [Category("Action")]
        [Description("Allows customizing the web view after it is created.")]
        public EventHandler<BlazorWebViewInitializedEventArgs>? BlazorWebViewInitialized;

        private void OnHostPagePropertyChanged() => StartWebViewCoreIfPossible();

        private void OnServicesPropertyChanged() => StartWebViewCoreIfPossible();

        private bool RequiredStartupPropertiesSet =>
            Created &&
            _webview != null &&
            HostPage != null &&
            Services != null;

        private void StartWebViewCoreIfPossible()
        {
            CheckDisposed();

            if (!RequiredStartupPropertiesSet || _webviewManager != null)
            {
                return;
            }

            var logger = Services.GetService<ILogger<BlazorWebView>>() ?? NullLogger<BlazorWebView>.Instance;

            // We assume the host page is always in the root of the content directory, because it's
            // unclear there's any other use case. We can add more options later if so.
            string appRootDir;
#pragma warning disable IL3000 // 'System.Reflection.Assembly.Location.get' always returns an empty string for assemblies embedded in a single-file app. If the path to the app directory is needed, consider calling 'System.AppContext.BaseDirectory'.
            var entryAssemblyLocation = Assembly.GetEntryAssembly()?.Location;
#pragma warning restore IL3000
            if (!string.IsNullOrEmpty(entryAssemblyLocation))
            {
                appRootDir = Path.GetDirectoryName(entryAssemblyLocation)!;
            }
            else
            {
                appRootDir = AppContext.BaseDirectory;
            }
            var hostPageFullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(appRootDir, HostPage!)); // HostPage is nonnull because RequiredStartupPropertiesSet is checked above
            var contentRootDirFullPath = System.IO.Path.GetDirectoryName(hostPageFullPath)!;
            var contentRootRelativePath = System.IO.Path.GetRelativePath(appRootDir, contentRootDirFullPath);
            var hostPageRelativePath = System.IO.Path.GetRelativePath(contentRootDirFullPath, hostPageFullPath);

            logger.CreatingFileProvider(contentRootDirFullPath, hostPageRelativePath);
            var fileProvider = CreateFileProvider(contentRootDirFullPath);

            _webviewManager = new GtkWebViewManager(
                _webview,
                Services,
                ComponentsDispatcher,
                fileProvider,
                RootComponents.JSComponents,
                contentRootRelativePath,
                hostPageRelativePath,
                (args) => UrlLoading?.Invoke(this, args),
                (args) => BlazorWebViewInitializing?.Invoke(this, args),
                (args) => BlazorWebViewInitialized?.Invoke(this, args),
                logger);
            
            StaticContentHotReloadManager.AttachToWebViewManagerIfEnabled(_webviewManager);

            foreach (var rootComponent in RootComponents)
            {
                logger.AddingRootComponent(rootComponent.ComponentType.FullName ?? string.Empty, rootComponent.Selector, rootComponent.Parameters?.Count ?? 0);

                // Since the page isn't loaded yet, this will always complete synchronously
                _ = rootComponent.AddToWebViewManagerAsync(_webviewManager);
            }

            logger.StartingInitialNavigation(StartPath);
            _webviewManager.Navigate(StartPath);
        }

        private void HandleRootComponentsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            CheckDisposed();

            // If we haven't initialized yet, this is a no-op
            if (_webviewManager != null)
            {
                // Dispatch because this is going to be async, and we want to catch any errors
                _ = ComponentsDispatcher.InvokeAsync(async () =>
                {
                    var newItems = (eventArgs.NewItems ?? Array.Empty<RootComponent>()).Cast<RootComponent>();
                    var oldItems = (eventArgs.OldItems ?? Array.Empty<RootComponent>()).Cast<RootComponent>();

                    foreach (var item in newItems.Except(oldItems))
                    {
                        await item.AddToWebViewManagerAsync(_webviewManager);
                    }

                    foreach (var item in oldItems.Except(newItems))
                    {
                        await item.RemoveFromWebViewManagerAsync(_webviewManager);
                    }
                });
            }
        }

        /// <summary>
        /// Creates a file provider for static assets used in the <see cref="BlazorWebView"/>. The default implementation
        /// serves files from disk. Override this method to return a custom <see cref="IFileProvider"/> to serve assets such
        /// as <c>wwwroot/index.html</c>. Call the base method and combine its return value with a <see cref="CompositeFileProvider"/>
        /// to use both custom assets and default assets.
        /// </summary>
        /// <param name="contentRootDir">The base directory to use for all requested assets, such as <c>wwwroot</c>.</param>
        /// <returns>Returns a <see cref="IFileProvider"/> for static assets.</returns>
        public virtual IFileProvider CreateFileProvider(string contentRootDir)
        {
            if (Directory.Exists(contentRootDir))
            {
                // Typical case after publishing, or if you're copying content to the bin dir in development for some nonstandard reason
                return new PhysicalFileProvider(contentRootDir);
            }
            else
            {
                // Typical case in development, as the files come from Microsoft.AspNetCore.Components.WebView.StaticContentProvider
                // instead and aren't copied to the bin dir
                return new NullFileProvider();
            }
        }

        /// <summary>
        /// Calls the specified <paramref name="workItem"/> asynchronously and passes in the scoped services available to Razor components.
        /// </summary>
        /// <param name="workItem">The action to call.</param>
        /// <returns>Returns a <see cref="Task"/> representing <c>true</c> if the <paramref name="workItem"/> was called, or <c>false</c> if it was not called because Blazor is not currently running.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="workItem"/> is <c>null</c>.</exception>
        public virtual async Task<bool> TryDispatchAsync(Action<IServiceProvider> workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);
            if (_webviewManager is null)
            {
                return false;
            }

            return await _webviewManager.TryDispatchAsync(workItem);
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Allows asynchronous disposal of the <see cref="BlazorWebView" />.
        /// </summary>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            // Dispose this component's contents that user-written disposal logic and Razor component disposal logic will
            // complete first. Then dispose the WebView2 control. This order is critical because once the WebView2 is
            // disposed it will prevent and Razor component code from working because it requires the WebView to exist.
            if (_webviewManager != null)
            {
                await _webviewManager.DisposeAsync()
                    .ConfigureAwait(false);
                _webviewManager = null;
            }

            _webview.OnMap -= HandleOnMap;
            _webview?.Dispose();
            _webview = null;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            // Perform async cleanup.
            await DisposeAsyncCore();

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            // Suppress finalization.
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }
    }
}