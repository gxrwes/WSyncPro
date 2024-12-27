using Microsoft.Extensions.Logging;
using MudBlazor.Services;
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
            builder.Services.AddMudServices();

            // Register services
            builder.Services.AddSingleton<IAppLocalDb>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<AppLocalDb>>();
                string dbFilePath = Path.Combine(FileSystem.AppDataDirectory, "appdb.json");
                return new AppLocalDb(dbFilePath, logger);
            });

            builder.Services.AddSingleton<IAppCache>(provider =>
            {
                var localDb = provider.GetRequiredService<IAppLocalDb>();
                var logger = provider.GetRequiredService<ILogger<AppCache>>();
                return new AppCache(localDb, logger);
            });

            builder.Services.AddSingleton<IFileVersioning, FileVersioning>();

            builder.Services.AddSingleton<ICopyService, CopyService>();

            builder.Services.AddSingleton<IImportService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ImportService>>();
                var copyService = provider.GetRequiredService<ICopyService>();
                return new ImportService(logger, copyService);
            });

            builder.Services.AddSingleton<ISyncService>(provider =>
            {
                var cache = provider.GetRequiredService<IAppCache>();
                var copyService = provider.GetRequiredService<ICopyService>();
                var fileVersioning = provider.GetRequiredService<IFileVersioning>();
                var logger = provider.GetRequiredService<ILogger<SyncService>>();
                return new SyncService(cache, copyService, fileVersioning, logger);
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
