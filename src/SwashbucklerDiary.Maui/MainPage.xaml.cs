using Microsoft.AspNetCore.Components.WebView;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            blazorWebView.BlazorWebViewInitializing += BlazorWebViewInitializing;
            blazorWebView.BlazorWebViewInitialized += BlazorWebViewInitialized;
        }

        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e);
        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e);
    }
}