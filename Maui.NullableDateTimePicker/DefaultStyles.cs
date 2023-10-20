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
                            new Setter { Property = Button.TextColorProperty, Value = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black },
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
                        BasedOn = DayStyle
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
                        BasedOn = DayStyle
                    };

                    _selectedDayStyle.Setters.Add(new Setter { Property = Button.CornerRadiusProperty, Value = DeviceInfo.Platform == DevicePlatform.iOS ? 15 : 50 });
                    _selectedDayStyle.Setters.Add(new Setter { Property = Button.TextColorProperty, Value = Colors.White });
                    _selectedDayStyle.Setters.Add(new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Blue });
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
                            new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Fill },
                            new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Fill},
                            new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
                            new Setter { Property = Label.VerticalTextAlignmentProperty, Value = TextAlignment.Center },
                            new Setter { Property = Label.FontSizeProperty, Value = 12 },
                            new Setter { Property = Label.TextColorProperty, Value =  Color.FromArgb("#512BD4") },
                            new Setter { Property = Label.FontAttributesProperty, Value = FontAttributes.Bold },
                            new Setter { Property = Label.MarginProperty, Value = 0 },
                            new Setter { Property = Label.PaddingProperty, Value = 0 },
                            new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.NoWrap }
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
                            new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Fill },
                            new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Fill },
                            new Setter { Property = Label.HorizontalTextAlignmentProperty, Value = TextAlignment.Center },
                            new Setter { Property = Label.VerticalTextAlignmentProperty, Value = TextAlignment.Center },
                            new Setter { Property = VisualElement.MinimumWidthRequestProperty, Value = 30 },
                            new Setter { Property = VisualElement.MinimumHeightRequestProperty, Value = 30 },
                            new Setter { Property = Label.FontSizeProperty, Value = 12 },
                            new Setter { Property = Label.TextColorProperty, Value = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black },
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
                            new Setter { Property = VisualElement.BackgroundColorProperty, Value = Colors.Transparent },
                            new Setter { Property = Button.BorderColorProperty, Value = Colors.Transparent },
                            new Setter { Property = View.HorizontalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = View.VerticalOptionsProperty, Value = LayoutOptions.Center },
                            new Setter { Property = View.MarginProperty, Value = new Thickness(10, 0, 10, 0) },
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
