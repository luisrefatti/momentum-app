using Microsoft.Extensions.Logging;
using CommunityToolkit;
using Plugin.LocalNotification;

namespace momentum_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Montserrat.ttf", "Montserrat");
                    fonts.AddFont("SpaceMono.ttf", "SpaceMono");
                    fonts.AddFont("MontserratBold.ttf", "MontserratBold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
