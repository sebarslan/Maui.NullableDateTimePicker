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

        internal static string GetTranslateText(IList<TranslationItem> translations, string key, string defaultText)
        {
            var item = translations?.FirstOrDefault(t => t.Key == key);
            return string.IsNullOrEmpty(item?.Key) || string.IsNullOrEmpty(item?.Value) ? defaultText : item.Value;
        }

        internal static string[] GetTranslateList(
    IList<TranslationItem>? translations,
    string key,
    string[] defaultValues)
        {
            var item = translations?.FirstOrDefault(t => t.Key == key);

            if (string.IsNullOrEmpty(item?.Value))
                return defaultValues;

            var values = item.Value.Split(';');

            return values.Length == defaultValues.Length
                ? values
                : defaultValues;
        }
    }
}
