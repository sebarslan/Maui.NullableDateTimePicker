using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;

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

            // Remove Entry control underline, padding and background color
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NullableDateTimePickerCustomization", (handler, view) =>
            {
                if (view is Maui.NullableDateTimePicker.NullableDateTimePickerEntry)
                {
#if ANDROID
                    handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    handler.PlatformView.SetPadding(0,0,0,0);
#elif IOS || MACCATALYST
                    handler.PlatformView.BackgroundColor = Colors.Transparent.ToPlatform();
                    handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                    handler.PlatformView.Background = Colors.Transparent.ToPlatform();
                    handler.PlatformView.Padding = new Microsoft.UI.Xaml.Thickness(0);
                    handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness()
                    {
                        Bottom = 0,
                        Top = 0,
                        Left = 0,
                        Right = 0,
                    };
#endif
                }
            });

            return builder.Build();
        }
    }
}