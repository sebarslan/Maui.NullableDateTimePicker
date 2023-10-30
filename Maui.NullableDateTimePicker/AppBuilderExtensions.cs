using CommunityToolkit.Maui;

namespace Maui.NullableDateTimePicker
{
    public static class AppBuilderExtensions
    {
        public static MauiAppBuilder ConfigureNullableDateTimePicker(this MauiAppBuilder builder)
        {
            builder.UseMauiCommunityToolkit();
            return builder;
        }
    }
}
