using CommunityToolkit.Maui;
namespace Maui.NullableDateTimePicker
{
    public static class AppBuilderExtensions
    {
        public static MauiAppBuilder ConfigureNullableDateTimePicker(this MauiAppBuilder builder)
        {
#if DEBUG
            return builder.UseMauiCommunityToolkit();
#else
            return builder.UseMauiCommunityToolkit(options =>
                 {
                     options.SetShouldSuppressExceptionsInConverters(true);
                     options.SetShouldSuppressExceptionsInBehaviors(true);
                     options.SetShouldSuppressExceptionsInAnimations(true);
                 });
#endif
        }
    }
}
