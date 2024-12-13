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
            builder.Services.AddSingleton<IAppCache, AppCache>();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
