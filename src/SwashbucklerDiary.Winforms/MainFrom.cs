using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Winforms.Extensions;

namespace SwashbucklerDiary.Winforms
{
    public partial class MainFrom : Form
    {
        public MainFrom()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            services.AddWindowsFormsBlazorWebView();
            services.AddMasaBlazor();
            services.AddDependencyInjection();

            blazorWebView1.HostPage = "wwwroot/index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<App>("#app");
        }

        //Fix resource not released after closing app
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Environment.Exit(0);
        }
    }
}