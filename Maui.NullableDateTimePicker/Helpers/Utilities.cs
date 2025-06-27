using System.Reflection;

namespace Maui.NullableDateTimePicker.Helpers
{
    internal class Utilities
    {
        public static ImageSource GetImageSource(string imageName)
        {
            var assembly = typeof(NullableDateTimePicker).GetTypeInfo().Assembly;
            var assemblyName = assembly.GetName().Name;
            var imagePath = $"{assemblyName}.Images.{imageName}";
            return ImageSource.FromResource($"{imagePath}");
        }
    }
}
