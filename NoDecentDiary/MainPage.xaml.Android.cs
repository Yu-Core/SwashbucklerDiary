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
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.VerticalScrollBarEnabled = false;
        }
    }
}
