using CommunityToolkit.Maui;

namespace Maui.NullableDateTimePicker
{
    internal static class DefaultStyles
    {
        private static Style _dayStyle;
        internal static Style DayStyle
        {
            get
            {
                _dayStyle ??= new Style(typeof(Button))
                {
                    Setters = {
                            new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = Button.TextColorProperty, Value = new AppThemeColor { Light = Colors.Black, Dark = Colors.White }.GetBinding() },
                            new Setter { Property = View.MarginProperty, Value = new Thickness(0) },
                            new Setter { Property = Button.PaddingProperty, Value = new Thickness(0) },
                            new Setter { Property = Button.FontSizeProperty, Value = 12 },
                            new Setter { Property = Button.BorderColorProperty, Value = Colors.Transparent },
                            new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = VisualElement.WidthRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.HeightRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MaximumWidthRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MaximumHeightRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MinimumWidthRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MinimumHeightRequestProperty, Value = 30 }
                        }
                };
                return _dayStyle;
            }
        }

        private static Style _disabledDayStyle;
        internal static Style DisabledDayStyle
        {
            get
            {
                _disabledDayStyle ??= new Style(typeof(Button))
                {
                    BasedOn = DayStyle,
                    Setters = {
                        new Setter { Property = Button.TextColorProperty, Value = Color.FromRgba("#919191")}
                    }
                };
                return _disabledDayStyle;
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
                        BasedOn = DayStyle
                    };
                    _otherMonthDayStyle.Setters.Add(new Setter
                    {
                        Property = Button.TextColorProperty,
                        Value = new AppThemeColor { Light = Colors.Gray, Dark = Color.FromRgba("#ACACAC") }.GetBinding()
                    });
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
                        BasedOn = DayStyle
                    };

                    _selectedDayStyle.Setters.Add(new Setter { Property = Button.CornerRadiusProperty, Value = DeviceInfo.Platform == DevicePlatform.iOS ? 15 : 50 });
                    _selectedDayStyle.Setters.Add(new Setter { Property = Button.TextColorProperty, Value = Colors.White });
                    _selectedDayStyle.Setters.Add(new Setter
                    {
                        Property = VisualElement.BackgroundColorProperty,
                        Value = new AppThemeColor { Light = Colors.Blue, Dark = Color.FromRgba("#6e5df2") }.GetBinding()
                    });
                }

                return _selectedDayStyle;
            }
        }

        private static Style _weekNumberStyle;
        internal static Style WeekNumberStyle
        {
            get
            {
                _weekNumberStyle ??= new Style(typeof(Label))
                {
                    Setters = {
                            new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center},
                            new Setter { Property = Label.FontSizeProperty, Value = 12 },
                            new Setter { Property = Label.TextColorProperty, Value =  Color.FromRgba("#6e5df2") },
                            new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
                            new Setter { Property = Label.MarginProperty, Value = 0 },
                            new Setter { Property = Label.PaddingProperty, Value = 0 },
                            new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.NoWrap }
                        }
                };

                return _weekNumberStyle;
            }
        }

        private static Style _dayNamesStyle;
        internal static Style DayNamesStyle
        {
            get
            {
                _dayNamesStyle ??= new Style(typeof(Label))
                {
                    Setters = {
                            new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = VisualElement.MinimumWidthRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MinimumHeightRequestProperty, Value = 30 },
                            new Setter { Property = Label.FontSizeProperty, Value = 12 },
                            new Setter { Property = Label.TextColorProperty, Value = new AppThemeColor { Light = Colors.Black, Dark = Colors.White }.GetBinding() },
                        }
                };

                return _dayNamesStyle;
            }
        }

        private static Style _toolButtonsStyle;
        internal static Style ToolButtonsStyle
        {
            get
            {
                _toolButtonsStyle ??= new Style(typeof(Button))
                {
                    Setters = {
                            new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = Button.BorderColorProperty, Value = Colors.Transparent },
                            new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = View.MarginProperty, Value = new Thickness(10, 0, 10, 0) },
                            new Setter { Property = Button.TextColorProperty, Value = Color.FromRgba("#6e5df2") },
                            new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold }
                        }
                };

                return _toolButtonsStyle;
            }
        }
    }
}
