using Microsoft.AspNetCore.Components.WebView.Maui;
using NoDecentDiary.Extend;

namespace NoDecentDiary;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif
		builder.Services.AddCustomIOC();
        builder.Services.AddMasaBlazor();

        return builder.Build();
	}
}
