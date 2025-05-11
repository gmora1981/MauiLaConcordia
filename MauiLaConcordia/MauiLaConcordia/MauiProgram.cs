using Microsoft.Extensions.Logging;
using MauiLaConcordia.Shared.Services;
using MauiLaConcordia.Services;
using MauiLaConcordia.Shared.Helpers;

namespace MauiLaConcordia;

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

        // Add device-specific services used by the MauiLaConcordia.Shared project
        builder.Services.AddSingleton<IFormFactor, FormFactor>();
        // Añadir en el método CreateMauiApp
        #if WINDOWS || ANDROID || IOS || MACCATALYST
             builder.Services.AddSingleton<ITokenStorage, NativeTokenStorage>();
        #else
             builder.Services.AddSingleton<ITokenStorage, BrowserTokenStorage>();
        #endif
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
