using CommunityToolkit.Maui;

namespace Maui.NullableDateTimePicker
{
    public static class AppBuilderExtensions
    {
        public static MauiAppBuilder ConfigureNullableDateTimePicker(this MauiAppBuilder builder)
        {
#if DEBUG
            builder.UseMauiCommunityToolkit();
#else
            builder.UseMauiCommunityToolkit(options =>
                 {
                     options.SetShouldSuppressExceptionsInConverters(true);
                     options.SetShouldSuppressExceptionsInBehaviors(true);
                     options.SetShouldSuppressExceptionsInAnimations(true);
                 });
#endif
           
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
            return builder;
        }
    }
}
