using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.NullableDateTimePicker
{
    internal static class DefaultStyles
    {
        private static Style _dayStyle;
        internal static Style DayStyle
        {
            get
            {
                if (_dayStyle == null)
                {
                    _dayStyle = new Style(typeof(Button))
                    {
                        Setters = {
                            new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = Button.TextColorProperty, Value = Colors.Black },
                            new Setter { Property = View.MarginProperty, Value = new Thickness(0) },
                            new Setter { Property = Button.PaddingProperty, Value = new Thickness(0) },
                            new Setter { Property = Button.FontSizeProperty, Value = 12 },
                            new Setter { Property = Button.BorderColorProperty, Value = Colors.Transparent },
                            new Setter { Property = VisualElement.WidthRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.HeightRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MaximumWidthRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MaximumHeightRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MinimumWidthRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MinimumHeightRequestProperty, Value = 30 }
                        }
                    };
                }
                return _dayStyle;
            }
        }

        private static Style _otherMonthDayStyle;
        internal static Style OtherMonthDayStyle
        {
            get
            {
                if (_otherMonthDayStyle == null)
                {
                    _otherMonthDayStyle = new Style(typeof(Button))
                    {
                        BasedOn = DefaultStyles.DayStyle
                    };
                    _otherMonthDayStyle.Setters.Add(new Setter { Property = Button.TextColorProperty, Value = Colors.Gray });
                }
                return _otherMonthDayStyle;
            }
        }

        private static Style _selectedDayStyle;
        internal static Style SelectedDayStyle
        {
            get
            {
                if (_selectedDayStyle == null)
                {
                    _selectedDayStyle = new Style(typeof(Button))
                    {
                        BasedOn = DefaultStyles.DayStyle
                    };
                    _selectedDayStyle.Setters.Add(new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Blue });
                    _selectedDayStyle.Setters.Add(new Setter { Property = Button.CornerRadiusProperty, Value = DeviceInfo.Platform == DevicePlatform.iOS ? 15 : 50 });
                    _selectedDayStyle.Setters.Add(new Setter { Property = Button.TextColorProperty, Value = Colors.White });
                }

                return _selectedDayStyle;
            }
        }

        private static Style _weekNumberStyle;
        internal static Style WeekNumberStyle
        {
            get
            {
                if (_weekNumberStyle == null)
                {
                    _weekNumberStyle = new Style(typeof(Label))
                    {
                        Setters = {
                            new Setter { Property = Label.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = Label.HorizontalOptionsProperty, Value = LayoutOptions.Fill },
                            new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.Fill },
                            new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
                            new Setter { Property = Label.VerticalTextAlignmentProperty, Value = TextAlignment.Center },
                            new Setter { Property = Label.FontSizeProperty, Value = 12 },
                            new Setter { Property = Label.TextColorProperty, Value = Color.FromArgb("#512BD4") },
                            new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold }
                        }
                    };
                }

                return _weekNumberStyle;
            }
        }

        private static Style _dayNamesStyle;
        internal static Style DayNamesStyle
        {
            get
            {
                if (_dayNamesStyle == null)
                {
                    _dayNamesStyle = new Style(typeof(Label))
                    {
                        Setters = {
                            new Setter { Property = Label.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = Label.HorizontalOptionsProperty, Value = LayoutOptions.Fill },
                            new Setter { Property = Label.VerticalOptionsProperty, Value = LayoutOptions.Fill },
                            new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
                            new Setter { Property = Label.VerticalTextAlignmentProperty, Value = TextAlignment.Center },
                            new Setter { Property = Label.MinimumWidthRequestProperty, Value = 20 },
                            new Setter { Property = Label.MinimumHeightRequestProperty, Value = 20 },
                            new Setter { Property = Label.FontSizeProperty, Value = 12 },
                            new Setter { Property = Label.TextColorProperty, Value = Colors.Black },
                        }
                    };
                }

                return _dayNamesStyle;
            }
        }

        private static Style _toolButtonsStyle;
        internal static Style ToolButtonsStyle
        {
            get
            {
                if (_toolButtonsStyle == null)
                {
                    _toolButtonsStyle = new Style(typeof(Button))
                    {
                        Setters = {
                            new Setter { Property = Button.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = Button.BorderColorProperty, Value = Colors.Transparent },
                            new Setter { Property = Button.HorizontalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = Button.VerticalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = Button.MarginProperty, Value = new Thickness(10, 0, 10, 0) },
                            new Setter { Property = Button.TextColorProperty, Value = Color.FromArgb("#7658dd") },
                            new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold }
                        }
                    };
                }

                return _toolButtonsStyle;
            }
        }
    }
}
