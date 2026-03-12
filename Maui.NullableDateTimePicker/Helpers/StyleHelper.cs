using CommunityToolkit.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    if (setter.Value is Color color)
                        return color;

                    if (setter.Value is AppThemeColor themeColor)
                        return Application.Current?.RequestedTheme == AppTheme.Dark
                            ? themeColor.Dark
                            : themeColor.Light;
                }

                style = style.BasedOn;
            }

            return defaultColor;
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

    }

}
