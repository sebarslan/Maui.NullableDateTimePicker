using CommunityToolkit.Maui;

namespace Maui.NullableDateTimePicker.Helpers
{
    internal static class StyleHelper
    {
        internal  static Style Merge(Style defaultStyle, Style? overrideStyle)
        {
            if (defaultStyle == null)
                throw new ArgumentNullException(nameof(defaultStyle));

            var targetType = overrideStyle?.TargetType ?? defaultStyle.TargetType;
            var merged = new Style(targetType);

            // Default style chain
            AddSettersRecursive(merged, defaultStyle);

            // Override style chain
            if (overrideStyle != null)
                AddSettersRecursive(merged, overrideStyle);

            return merged;
        }

        internal static Color? GetColorFromStyleOrDefault(
    Style style,
    BindableProperty property,
    string lightHex,
    string darkHex)
        {
            var color = GetColorFromStyle(style, property);

            if (IsTransparent(color))
                return GetColor(lightHex, darkHex);

            return color;
        }

        internal static Color? GetColorFromStyle(
    Style style,
    BindableProperty property,
    Color? defaultColor = null)
        {
            while (style != null)
            {
                var setter = style.Setters
                    .OfType<Setter>()
                    .FirstOrDefault(s => s.Property == property);

                if (setter != null)
                {
                    var value = setter.Value;

                    // Direct Color
                    if (value is Color color)
                        return color;

                    // Brush
                    if (value is SolidColorBrush brush)
                        return brush.Color;

                    // AppThemeColor
                    if (value is AppThemeColor themeColor)
                    {
                        var theme = Application.Current?.RequestedTheme ?? AppTheme.Light;
                        return theme == AppTheme.Dark
                            ? themeColor.Dark
                            : themeColor.Light;
                    }

                    // fallback: try reflection (covers AppThemeBinding)
                    var colorProp = value?.GetType().GetProperty("Color");
                    if (colorProp?.GetValue(value) is Color reflectedColor)
                        return reflectedColor;
                }

                style = style.BasedOn;
            }

            return defaultColor;
        }

        internal static bool IsTransparent(Color? c)
        {
            return c == null || c.Alpha == 0;
        }

        private static void AddSettersRecursive(Style target, Style style)
        {
            if (style.BasedOn != null)
                AddSettersRecursive(target, style.BasedOn);

            foreach (Setter setter in style.Setters.OfType<Setter>())
            {
                var existing = target.Setters
                    .OfType<Setter>()
                    .FirstOrDefault(s => s.Property == setter.Property);

                if (existing != null)
                    target.Setters.Remove(existing);

                target.Setters.Add(new Setter
                {
                    Property = setter.Property,
                    Value = setter.Value
                });
            }
        }

        internal static Color GetColor(string lightHex, string darkHex)
        {
            var theme = Application.Current?.RequestedTheme ?? AppTheme.Light;

            return theme == AppTheme.Dark
                ? Color.FromArgb(darkHex)
                : Color.FromArgb(lightHex);
        }
    }
}
