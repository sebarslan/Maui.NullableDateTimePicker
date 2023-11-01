using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
namespace Maui.NullableDateTimePicker.Samples
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder = builder.UseMauiApp<App>();
            builder.ConfigureNullableDateTimePicker()
            .ConfigureFonts(fonts =>
        {
            fonts.AddFont("fa-solid-900.ttf", "FontAwesome");
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}