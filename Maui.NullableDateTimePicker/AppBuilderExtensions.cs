using CommunityToolkit.Maui;
#if WINDOWS
using Mopups.Hosting;
#endif

namespace Maui.NullableDateTimePicker
{
    public static class AppBuilderExtensions
    {
        public static MauiAppBuilder ConfigureNullableDateTimePicker(this MauiAppBuilder builder)
        {
#if WINDOWS          
            builder.ConfigureMopups();
#endif

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
            return builder;
        }
    }
}
