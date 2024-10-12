using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using WSyncPro.Core.Managers;
using WSyncPro.Util.Services;
using WSyncPro.Util.Files;

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

            // Construct the file path using MAUI's FileSystem
            string jobListFilePath = Path.Combine(FileSystem.AppDataDirectory, "joblist.json");

            // Register StateManager as a singleton service
            builder.Services.AddSingleton<IStateManager>(provider => new StateManager(jobListFilePath));

            // Register IFileCopyMoveService
            builder.Services.AddSingleton<IFileCopyMoveService, FileCopyMoveService>();

            // Register IFileLoader
            builder.Services.AddSingleton<IFileLoader, FileLoader>();

            return builder.Build();
        }
    }
}
