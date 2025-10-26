using System.Reflection;

namespace Maui.NullableDateTimePicker.Helpers
{
    internal static class Utilities
    {
        public static ImageSource GetImageSource(string imageName)
        {
            var assembly = typeof(NullableDateTimePicker).GetTypeInfo().Assembly;
            var assemblyName = assembly.GetName().Name;
            var imagePath = $"{assemblyName}.Images.{imageName}";
            return ImageSource.FromResource($"{imagePath}");
        }


        internal static Page? FindParentPage(this VisualElement element)
        {
            var parent = element.Parent;
            while (parent != null && parent is not Page)
            {
                parent = parent.Parent;
            }
            return parent as Page;
        }

        internal static string GetTranslateText(IList<TranslateItem> translates, string key, string defaultText)
        {
            var item = translates?.FirstOrDefault(t => t.Key == key);
            return string.IsNullOrEmpty(item?.Key) || string.IsNullOrEmpty(item?.Value) ? defaultText : item.Value;
        }
    }
}
