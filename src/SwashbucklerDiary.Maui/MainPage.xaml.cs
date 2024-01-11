using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage : ContentPage
    {
        private Color color;

        public MainPage(IThemeService themeService)
        {
            bool dark = themeService.RealTheme == Shared.Theme.Dark;
            color = dark ? TitleBarOrStatusBar.DarkColor : TitleBarOrStatusBar.LightColor;
            InitializeComponent();
            blazorWebView.BlazorWebViewInitializing += BlazorWebViewInitializing;
            blazorWebView.BlazorWebViewInitialized += BlazorWebViewInitialized;
            
            #if IOS
            RegisterForKeyboardNotifications();
            #endif
        }

        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e);

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e);
    }
}