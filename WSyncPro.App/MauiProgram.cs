using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using WSyncPro.Util.Services;
using WSyncPro.Util.Files;
using WSyncPro.Core.Services;

namespace WSyncPro.App
{
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
            builder.Logging.AddDebug();
#endif

            // Register all necessary services for SyncService
            builder.Services.AddSingleton<IFileCopyMoveService, FileCopyMoveService>();
            builder.Services.AddSingleton<IFileLoader, FileLoader>();
            builder.Services.AddSingleton<IDirectoryScannerService, DirectoryScannerService>();
            builder.Services.AddSingleton<ISyncService, SyncService>();

            return builder.Build();
        }
    }
}
