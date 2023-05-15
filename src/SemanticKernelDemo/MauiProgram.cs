using Microsoft.Extensions.Logging;
using SemanticKernelDemo.Data;
using BlazorBootstrap;
using SemanticKernelDemo.Services;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui;

namespace SemanticKernelDemo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
        builder.Services.AddBlazorBootstrap();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

	
		builder.Services.AddSingleton<SummaryService>();
		builder.Services.AddSingleton<ArtisticImageService>();
		builder.Services.AddSingleton<QnAService>();
		builder.Services.AddSingleton<SentimentService>();
		builder.Services.AddSingleton<TranslateProgramService>();
		builder.Services.AddSingleton<TranslatorService>();
		builder.Services.AddSingleton<GrammarCorrectionService>();
		builder.Services.AddSingleton<QuizCreatorService>();
			

        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
        return builder.Build();
	}
}
