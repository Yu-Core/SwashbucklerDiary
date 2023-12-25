using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Wpf.Extensions;
using System.Windows;

namespace SwashbucklerDiary.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            serviceCollection.AddMasaBlazor();
            serviceCollection.AddDependencyInjection();

            Resources.Add("services", serviceCollection.BuildServiceProvider());
        }
    }
}