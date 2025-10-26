using Maui.NullableDateTimePicker.Controls;
using Maui.NullableDateTimePicker.Helpers;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;

namespace Maui.NullableDateTimePicker;

// All the code in this file is included in all platforms.
public class NullableDateTimePicker : ContentView
{
    public event EventHandler<DateTimeChangedEventArgs> SelectedDateTimeChanged;
    private Grid _dateTimePickerGrid;
    private Entry _dateTimePickerEntry;
    private Image _dateTimePickerIcon;
    private Border _dateTimePickerBorder;
    const double defaultHeightRequest = 40;
    public Page? ParentPage { get; set; }

    #region bindable properties

    public static readonly BindableProperty SelectedDateTimeProperty =
    BindableProperty.Create(nameof(SelectedDateTime),
        typeof(DateTime?),
        typeof(NullableDateTimePicker),
        null,
        defaultBindingMode: BindingMode.TwoWay,
        null,
        (bindable, oldValue, newValue) =>
        {
            var self = (NullableDateTimePicker)bindable;
            var oldSelectedDateTime = (DateTime?)oldValue;
            var newSelectedDateTime = (DateTime?)newValue;

            // Reset the text, so that the text color will be updated
            // https://github.com/dotnet/maui/issues/17843
            self._dateTimePickerEntry.Text = "";
            self._dateTimePickerEntry.Text = newSelectedDateTime?.ToString(self.Format);

            //Date changed event
            bool isDateTimeChanged = false;
            if (self.Mode == PickerModes.Date && oldSelectedDateTime?.Date != newSelectedDateTime?.Date)
            {
                isDateTimeChanged = true;
            }
            else if (self.Mode == PickerModes.DateTime && (oldSelectedDateTime?.Date != newSelectedDateTime?.Date || oldSelectedDateTime?.TimeOfDay != newSelectedDateTime?.TimeOfDay))
            {
                isDateTimeChanged = true;
            }
            else if (self.Mode == PickerModes.Time && oldSelectedDateTime?.TimeOfDay != newSelectedDateTime?.TimeOfDay)
            {
                isDateTimeChanged = true;
            }

            if (isDateTimeChanged)
                self.SelectedDateTimeChanged?.Invoke(self, new DateTimeChangedEventArgs(oldSelectedDateTime, newSelectedDateTime));
        });

    public DateTime? SelectedDateTime
    {
        get { return (DateTime?)GetValue(SelectedDateTimeProperty); }
        set
        {
            SetValue(SelectedDateTimeProperty, value);
        }
    }

    public static readonly BindableProperty ToolButtonsStyleProperty =
BindableProperty.Create(nameof(ToolButtonsStyle), typeof(Style), typeof(NullableDateTimePicker), defaultValue: null, defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
    });

    public Style ToolButtonsStyle
    {
        get { return (Style)GetValue(ToolButtonsStyleProperty); }
        set { SetValue(ToolButtonsStyleProperty, value); }
    }

    public static readonly BindableProperty ModeProperty =
    BindableProperty.Create(
        nameof(Mode),
        typeof(PickerModes),
        typeof(NullableDateTimePicker),
        PickerModes.Date,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (newValue is string strValue)
            {
                if (PickerModes.TryParse(strValue, out PickerModes pickerMode))
                {
                    newValue = pickerMode;
                }
            }
            ((NullableDateTimePicker)bindable).SetCalendarIcon();
        });

    public PickerModes Mode
    {
        get { return (PickerModes)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }

    public static readonly BindableProperty MinDateProperty =
    BindableProperty.Create(nameof(MinDate), typeof(DateTime?), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, null,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            newValue = ParseDateTime(newValue);
        });

    public DateTime? MinDate
    {
        get { return (DateTime?)GetValue(MinDateProperty); }
        set
        {
            SetValue(MinDateProperty, value);
        }
    }

    public static readonly BindableProperty MaxDateProperty =
    BindableProperty.Create(nameof(MaxDate), typeof(DateTime?), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, null, propertyChanged: (bindable, oldValue, newValue) =>
    {
        newValue = ParseDateTime(newValue);
    });

    public DateTime? MaxDate
    {
        get { return (DateTime?)GetValue(MaxDateProperty); }
        set
        {
            SetValue(MaxDateProperty, value);
        }
    }

    public static readonly BindableProperty BodyBackgroundColorProperty =
    BindableProperty.Create(nameof(BodyBackgroundColor),
        typeof(Color),
        typeof(NullableDateTimePicker),
        null,
        defaultBindingMode: BindingMode.OneWay);

    public Color BodyBackgroundColor
    {
        get { return (Color)GetValue(BodyBackgroundColorProperty); }
        set
        {
            SetValue(BodyBackgroundColorProperty, value);
        }
    }

    public static readonly BindableProperty HeaderBackgroundColorProperty =
    BindableProperty.Create(nameof(HeaderBackgroundColor),
        typeof(Color),
        typeof(NullableDateTimePicker),
        null,
        defaultBindingMode: BindingMode.OneWay);

    public Color HeaderBackgroundColor
    {
        get { return (Color)GetValue(HeaderBackgroundColorProperty); }
        set
        {
            SetValue(HeaderBackgroundColorProperty, value);
        }
    }

    public static readonly BindableProperty HeaderForeColorProperty =
    BindableProperty.Create(nameof(HeaderForeColor), typeof(Color), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, null, (b, o, n) =>
    {
    });

    public Color HeaderForeColor
    {
        get { return (Color)GetValue(HeaderForeColorProperty); }
        set
        {
            SetValue(HeaderForeColorProperty, value);
        }
    }

    public static readonly BindableProperty DayStyleProperty =
    BindableProperty.Create(nameof(DayStyle), typeof(Style), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, null, (b, o, n) =>
    {
    });

    public Style DayStyle
    {
        get { return (Style)GetValue(DayStyleProperty); }
        set
        {
            SetValue(DayStyleProperty, value);
        }
    }

    public static readonly BindableProperty DisabledDayStyleProperty =
    BindableProperty.Create(nameof(DisabledDayStyle), typeof(Style), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, null, (b, o, n) =>
    {
    });

    public Style DisabledDayStyle
    {
        get { return (Style)GetValue(DisabledDayStyleProperty); }
        set
        {
            SetValue(DisabledDayStyleProperty, value);
        }
    }

    public static readonly BindableProperty OtherMonthDayStyleProperty =
    BindableProperty.Create(nameof(OtherMonthDayStyle), typeof(Style), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, null, (b, o, n) =>
    {
    });

    public Style OtherMonthDayStyle
    {
        get { return (Style)GetValue(OtherMonthDayStyleProperty); }
        set
        {
            SetValue(OtherMonthDayStyleProperty, value);
        }
    }

    public static readonly BindableProperty SelectedDayStyleProperty =
    BindableProperty.Create(nameof(SelectedDayStyle), typeof(Style), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, null);

    public Style SelectedDayStyle
    {
        get { return (Style)GetValue(SelectedDayStyleProperty); }
        set
        {
            SetValue(SelectedDayStyleProperty, value);
        }
    }

    public static readonly BindableProperty DayNamesStyleProperty =
    BindableProperty.Create(
        nameof(DayNamesStyle),
        typeof(Style),
        typeof(NullableDateTimePicker),
        null,
        defaultBindingMode: BindingMode.OneWay);

    public Style DayNamesStyle
    {
        get { return (Style)GetValue(DayNamesStyleProperty); }
        set
        {
            SetValue(DayNamesStyleProperty, value);
        }
    }

    public static readonly BindableProperty WeekNumberStyleProperty =
   BindableProperty.Create(nameof(WeekNumberStyle), typeof(Style), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, null, (b, o, n) =>
   {
   });

    public Style WeekNumberStyle
    {
        get { return (Style)GetValue(WeekNumberStyleProperty); }
        set
        {
            SetValue(WeekNumberStyleProperty, value);
        }
    }

    public static readonly BindableProperty OkButtonTextProperty =
    BindableProperty.Create(
        nameof(OkButtonText),
        typeof(string),
        typeof(NullableDateTimePicker),
        "OK",
        defaultBindingMode: BindingMode.OneWay, null, (b, o, n) =>
    {
    });

    public string OkButtonText
    {
        get { return (string)GetValue(OkButtonTextProperty); }
        set
        {
            SetValue(OkButtonTextProperty, value);
        }
    }


    public static readonly BindableProperty ClearButtonTextProperty =
    BindableProperty.Create(nameof(ClearButtonText), typeof(string), typeof(NullableDateTimePicker), "Clear", defaultBindingMode: BindingMode.OneWay, null, (b, o, n) =>
    {
    });

    public string ClearButtonText
    {
        get { return (string)GetValue(ClearButtonTextProperty); }
        set
        {
            SetValue(ClearButtonTextProperty, value);
        }
    }

    public static readonly BindableProperty CancelButtonTextProperty =
    BindableProperty.Create(
        nameof(CancelButtonText),
        typeof(string),
        typeof(NullableDateTimePicker),
        "Cancel",
        defaultBindingMode: BindingMode.OneWay, null, (b, o, n) =>
    {
    });

    public string CancelButtonText
    {
        get { return (string)GetValue(CancelButtonTextProperty); }
        set
        {
            SetValue(CancelButtonTextProperty, value);
        }
    }

    public static readonly BindableProperty ShowWeekNumbersProperty = BindableProperty.Create(
    nameof(ShowWeekNumbers),
    typeof(bool),
    typeof(NullableDateTimePicker),
    defaultValue: false,
    defaultBindingMode: BindingMode.OneWay);

    public bool ShowWeekNumbers
    {
        get { return (bool)GetValue(ShowWeekNumbersProperty); }
        set { SetValue(ShowWeekNumbersProperty, value); }
    }

    public static readonly BindableProperty ShowOtherMonthDaysProperty = BindableProperty.Create(
    nameof(ShowOtherMonthDays),
    typeof(bool),
    typeof(NullableDateTimePicker),
    defaultValue: true,
    defaultBindingMode: BindingMode.OneWay);

    public bool ShowOtherMonthDays
    {
        get { return (bool)GetValue(ShowOtherMonthDaysProperty); }
        set { SetValue(ShowOtherMonthDaysProperty, value); }
    }

    public static readonly BindableProperty PopupBorderColorProperty = BindableProperty.Create(
    nameof(PopupBorderColor),
    typeof(Color),
    typeof(NullableDateTimePicker),
    defaultValue: Colors.Transparent,
    defaultBindingMode: BindingMode.OneWay);
    public Color PopupBorderColor
    {
        get { return (Color)GetValue(PopupBorderColorProperty); }
        set { SetValue(PopupBorderColorProperty, value); }
    }

    public static readonly BindableProperty PopupBorderWidthProperty = BindableProperty.Create(
    nameof(PopupBorderWidth),
    typeof(double),
    typeof(NullableDateTimePicker),
    defaultValue: 0.0d,
    defaultBindingMode: BindingMode.OneWay);
    public double PopupBorderWidth
    {
        get { return (double)GetValue(PopupBorderWidthProperty); }
        set { SetValue(PopupBorderWidthProperty, value); }
    }

    public static readonly BindableProperty PopupCornerRadiusProperty = BindableProperty.Create(
    nameof(PopupCornerRadius),
    typeof(CornerRadius),
    typeof(NullableDateTimePicker),
    defaultValue: new CornerRadius(0),
    defaultBindingMode: BindingMode.OneWay);

    public CornerRadius PopupCornerRadius
    {
        get { return (CornerRadius)GetValue(PopupCornerRadiusProperty); }
        set { SetValue(PopupCornerRadiusProperty, value); }
    }

    public static readonly BindableProperty PopupPaddingProperty = BindableProperty.Create(
    nameof(PopupPadding),
    typeof(Thickness),
    typeof(NullableDateTimePicker),
    defaultValue: new Thickness(0),
    defaultBindingMode: BindingMode.OneWay);

    public Thickness PopupPadding
    {
        get { return (Thickness)GetValue(PopupPaddingProperty); }
        set { SetValue(PopupPaddingProperty, value); }
    }

    public static readonly BindableProperty PopupPageOverlayColorProperty = BindableProperty.Create(
    nameof(PopupPageOverlayColor),
    typeof(Color),
    typeof(NullableDateTimePicker),
    defaultValue: Colors.Black.WithAlpha(0.5f),
    defaultBindingMode: BindingMode.OneWay);
    public Color PopupPageOverlayColor
    {
        get { return (Color)GetValue(PopupPageOverlayColorProperty); }
        set { SetValue(PopupPageOverlayColorProperty, value); }
    }

    public static readonly BindableProperty ForeColorProperty = BindableProperty.Create(
    nameof(ForeColor),
    typeof(Color),
    typeof(NullableDateTimePicker),
    defaultValue: null,
    defaultBindingMode: BindingMode.OneWay);

    public Color ForeColor
    {
        get { return (Color)GetValue(ForeColorProperty); }
        set { SetValue(ForeColorProperty, value); }
    }


    public static readonly BindableProperty ActivityIndicatorColorProperty = BindableProperty.Create(
    nameof(ActivityIndicatorColor),
    typeof(Color),
    typeof(NullableDateTimePicker),
    defaultValue: null,
    defaultBindingMode: BindingMode.OneWay);

    public Color ActivityIndicatorColor
    {
        get { return (Color)GetValue(ActivityIndicatorColorProperty); }
        set { SetValue(ActivityIndicatorColorProperty, value); }
    }

    public static readonly BindableProperty ShowClearButtonProperty = BindableProperty.Create(
    nameof(ShowClearButton),
    typeof(bool),
    typeof(NullableDateTimePicker),
    defaultValue: true,
    defaultBindingMode: BindingMode.OneWay);

    public bool ShowClearButton
    {
        get { return (bool)GetValue(ShowClearButtonProperty); }
        set { SetValue(ShowClearButtonProperty, value); }
    }

    public static readonly BindableProperty CloseOnOutsideClickProperty = BindableProperty.Create(
    nameof(CloseOnOutsideClick),
    typeof(bool),
    typeof(NullableDateTimePicker),
    defaultValue: false,
    defaultBindingMode: BindingMode.OneWay);

    public bool CloseOnOutsideClick
    {
        get { return (bool)GetValue(CloseOnOutsideClickProperty); }
        set { SetValue(CloseOnOutsideClickProperty, value); }
    }


    public static readonly BindableProperty FormatProperty =
    BindableProperty.Create(nameof(Format), typeof(string), typeof(NullableDateTimePicker), null,
        defaultBindingMode: BindingMode.OneWay,
        null, (b, o, n) =>
        {
            var self = (NullableDateTimePicker)b;
            var oldValue = (string)o;
            var newValue = (string)n;
            if (oldValue != newValue)
            {
                self._dateTimePickerEntry.Text = self.SelectedDateTime?.ToString(self.Format);
            }
        });

    public string Format
    {
        get
        {
            var format = (string)GetValue(FormatProperty);
            if (string.IsNullOrEmpty(format))
            {
                if (Mode == PickerModes.Time)
                    format = "t";
                else if (Mode == PickerModes.DateTime)
                    format = "g";
                else
                    format = "d";
            }
            return format;
        }
        set
        {
            SetValue(FormatProperty, value);
        }
    }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
    nameof(Icon),
    typeof(ImageSource),
    typeof(NullableDateTimePicker),
    defaultValue: null,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        ((NullableDateTimePicker)bindable).SetCalendarIcon();
    });

    public ImageSource Icon
    {
        get { return (ImageSource)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly BindableProperty IconBackgroundColorProperty =
    BindableProperty.Create(
        nameof(IconBackgroundColor),
        typeof(Color),
        typeof(NullableDateTimePicker),
        defaultValue: Colors.Transparent,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (newValue != null)
            ((NullableDateTimePicker)bindable)._dateTimePickerIcon.BackgroundColor = (Color)newValue;
    });

    public Color IconBackgroundColor
    {
        get { return (Color)GetValue(IconBackgroundColorProperty); }
        set
        {
            SetValue(IconBackgroundColorProperty, value);
        }
    }


    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
    nameof(Placeholder),
    typeof(string),
    typeof(NullableDateTimePicker),
    defaultValue: string.Empty,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        ((NullableDateTimePicker)bindable)._dateTimePickerEntry.Placeholder = (string)newValue ?? string.Empty;
    });

    public string Placeholder
    {
        get { return (string)GetValue(PlaceholderProperty); }
        set { SetValue(PlaceholderProperty, value); }
    }

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
    nameof(PlaceholderColor),
    typeof(Color),
    typeof(NullableDateTimePicker),
    defaultValue: Colors.Gray,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is Color placeholderColor)
        {
            nullableDateTimePickerBindable._dateTimePickerEntry.PlaceholderColor = placeholderColor;
        }
    });
    public Color PlaceholderColor
    {
        get { return (Color)GetValue(PlaceholderColorProperty); }
        set { SetValue(PlaceholderColorProperty, value); }
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
    nameof(FontSize),
    typeof(double),
    typeof(NullableDateTimePicker),
    defaultValue: 14d,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        double fontSize = 14d;
        if (newValue is double)
            fontSize = (double)newValue;
        else if (newValue is string)
            double.TryParse((string)newValue, out fontSize);

        ((NullableDateTimePicker)bindable)._dateTimePickerEntry.FontSize = fontSize;
    });

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
    nameof(TextColor),
    typeof(Color),
    typeof(NullableDateTimePicker),
    defaultValue: Colors.Black,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is Color textColor)
        {
            nullableDateTimePickerBindable._dateTimePickerEntry.TextColor = textColor;
        }
    });

    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(NullableDateTimePicker),
        defaultValue: "OpenSansRegular",
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is string fontFamily)
            {
                nullableDateTimePickerBindable._dateTimePickerEntry.FontFamily = fontFamily;
            }
        });

    public string FontFamily
    {
        get { return (string)GetValue(FontFamilyProperty); }
        set { SetValue(FontFamilyProperty, value); }
    }

    public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
    nameof(BackgroundColor),
    typeof(Color),
    typeof(NullableDateTimePicker),
    defaultValue: Colors.White,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is Color backgroundColor)
        {
            nullableDateTimePickerBindable._dateTimePickerBorder.BackgroundColor = backgroundColor;
        }
    });

    public new Color BackgroundColor
    {
        get { return (Color)GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
    nameof(BorderColor),
    typeof(Color),
    typeof(NullableDateTimePicker),
    defaultValue: Colors.Transparent,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is Color borderColor)
        {
            nullableDateTimePickerBindable._dateTimePickerBorder.Stroke = borderColor;
        }
    });
    public Color BorderColor
    {
        get { return (Color)GetValue(BorderColorProperty); }
        set { SetValue(BorderColorProperty, value); }
    }

    public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
    nameof(BorderWidth),
    typeof(double),
    typeof(NullableDateTimePicker),
    defaultValue: 0.0d,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (newValue is double)
        {
            ((NullableDateTimePicker)bindable)._dateTimePickerBorder.StrokeThickness = (double)newValue;
        }

    });
    public double BorderWidth
    {
        get { return (double)GetValue(BorderWidthProperty); }
        set { SetValue(BorderWidthProperty, value); }
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
    nameof(CornerRadius),
    typeof(Thickness),
    typeof(NullableDateTimePicker),
    defaultValue: new Thickness(0),
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is Thickness cornerRadius)
        {
            nullableDateTimePickerBindable._dateTimePickerBorder.StrokeShape = new RoundRectangle
            {
                CornerRadius = new Microsoft.Maui.CornerRadius(cornerRadius.Left, cornerRadius.Top, cornerRadius.Right, cornerRadius.Bottom)
            };
        }
    });

    public Thickness CornerRadius
    {
        get { return (Thickness)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    public new static readonly BindableProperty PaddingProperty = BindableProperty.Create(
    nameof(Padding),
    typeof(Thickness),
    typeof(NullableDateTimePicker),
    defaultValue: new Thickness(0),
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is Thickness padding)
        {
            nullableDateTimePickerBindable._dateTimePickerBorder.Padding = new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom);
        }
    });
    public new Thickness Padding
    {
        get { return (Thickness)GetValue(PaddingProperty); }
        set { SetValue(PaddingProperty, value); }
    }


    public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(
    nameof(VerticalTextAlignment),
    typeof(TextAlignment),
    typeof(NullableDateTimePicker),
    defaultValue: TextAlignment.Center,
    defaultBindingMode: BindingMode.OneWay,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is TextAlignment verticalTextAlignment)
        {
            nullableDateTimePickerBindable._dateTimePickerEntry.VerticalTextAlignment = verticalTextAlignment;
        }
    });
    public TextAlignment VerticalTextAlignment
    {
        get { return (TextAlignment)GetValue(VerticalTextAlignmentProperty); }
        set { SetValue(VerticalTextAlignmentProperty, value); }
    }


    public static readonly BindableProperty Is12HourFormatProperty = BindableProperty.Create(
       nameof(Is12HourFormat),
       typeof(bool),
       typeof(NullableDateTimePicker),
       defaultValue: false,
       defaultBindingMode: BindingMode.OneWay);

    public bool Is12HourFormat
    {
        get { return (bool)GetValue(Is12HourFormatProperty); }
        set { SetValue(Is12HourFormatProperty, value); }
    }

    public static readonly BindableProperty HideIconProperty = BindableProperty.Create(
       nameof(HideIcon),
       typeof(bool),
       typeof(NullableDateTimePicker),
       defaultValue: false,
       defaultBindingMode: BindingMode.OneWay,
       propertyChanged: (bindable, oldValue, newValue) =>
       {
           if (bindable is NullableDateTimePicker nullableDateTimePickerBindable && newValue is bool hideIcon)
           {
               nullableDateTimePickerBindable._dateTimePickerGrid.ColumnDefinitions[1].Width = hideIcon ? new GridLength(0) : GridLength.Auto;
               nullableDateTimePickerBindable._dateTimePickerIcon.IsVisible = !hideIcon;
           }
       });

    public bool HideIcon
    {
        get { return (bool)GetValue(HideIconProperty); }
        set { SetValue(HideIconProperty, value); }
    }

    public static readonly BindableProperty TranslationsProperty =
        BindableProperty.Create(
            nameof(Translations),
            typeof(IList<TranslationItem>),
            typeof(ClockView),
            new List<TranslationItem>());

    public IList<TranslationItem> Translations
    {
        get => (IList<TranslationItem>)GetValue(TranslationsProperty);
        set => SetValue(TranslationsProperty, value);
    }


    #endregion //bindable properties

    #region constructor
    public NullableDateTimePicker()
    {
        base.Padding = 0;
        base.Margin = 0;
        base.BackgroundColor = Colors.Transparent;
        base.HeightRequest = defaultHeightRequest;

        _dateTimePickerGrid = new Microsoft.Maui.Controls.Grid
        {
            Margin = 0,
            Padding = 0,
            RowSpacing = 0,
            ColumnSpacing = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor = Colors.Transparent,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
            }
        };

        _dateTimePickerEntry = new NullableDateTimePickerEntry()
        {
            IsReadOnly = true,
            Margin = 0,
            BackgroundColor = Colors.Transparent,
            FontSize = this.FontSize,
            TextColor = this.TextColor,
            FontFamily = FontFamily,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = this.VerticalTextAlignment,
            PlaceholderColor = this.PlaceholderColor
        };

        _dateTimePickerGrid.SetColumn(_dateTimePickerEntry, 0);
        _dateTimePickerGrid.Add(_dateTimePickerEntry);


        _dateTimePickerIcon = new Image
        {
            BackgroundColor = this.IconBackgroundColor,
            Aspect = Aspect.AspectFit,
            Margin = 0,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Fill
        };
        _dateTimePickerGrid.SetColumn(_dateTimePickerIcon, 1);
        _dateTimePickerGrid.Add(_dateTimePickerIcon);

        var clickableView = new BoxView { Color = Colors.Transparent, Background = Colors.Transparent, BackgroundColor = Colors.Transparent, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        _dateTimePickerGrid.SetColumn(clickableView, 0);

        _dateTimePickerGrid.SetColumnSpan(clickableView, 2);
        _dateTimePickerGrid.Add(clickableView);

        TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer
        {
            NumberOfTapsRequired = 1
        };
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            OnDatePickerClicked(s, e);
        };

        clickableView.GestureRecognizers.Add(tapGestureRecognizer);


        _dateTimePickerIcon.SetBinding(Image.WidthRequestProperty, new Binding("Height", source: clickableView));
        _dateTimePickerIcon.SetBinding(Image.MaximumWidthRequestProperty, new Binding("Height", source: clickableView));
        _dateTimePickerIcon.SetBinding(Image.HeightRequestProperty, new Binding("Height", source: clickableView));
        _dateTimePickerIcon.SetBinding(Image.MaximumHeightRequestProperty, new Binding("Height", source: clickableView));
        _dateTimePickerEntry.SetBinding(Entry.HeightRequestProperty, new Binding("Height", source: clickableView));

        _dateTimePickerBorder = new Border
        {
            BackgroundColor = this.BackgroundColor,
            Stroke = this.BorderColor,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(0, 0, 0, 0)
            },
            StrokeThickness = this.BorderWidth,
            Content = _dateTimePickerGrid,
            Margin = 0,
            Padding = this.Padding,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        this.Loaded += (s, e) =>
        {
            SetCalendarIcon();
        };

        Content = _dateTimePickerBorder;
    }
    #endregion //consturctor


    #region public methods
    public static async Task<Maui.NullableDateTimePicker.PopupResult> OpenAsync(INullableDateTimePickerOptions options, Page page = null)
    {
        using (var popupControl = new NullableDateTimePickerPopup(options, new CancellationTokenSource()))
        {
            return await popupControl.OpenPopupAsync(page);
        }
    }


    #endregion //public metlods

    #region private methods
    private async void OnDatePickerClicked(object sender, EventArgs e)
    {
        await OpenNullableDateTimePickerPopupAsync();
    }

    bool isPopupOpen = false;
    private async Task OpenNullableDateTimePickerPopupAsync()
    {
        if (!base.IsEnabled || isPopupOpen)
            return;

        isPopupOpen = true;

        try
        {
            INullableDateTimePickerOptions options = new NullableDateTimePickerOptions
            {
                SelectedDateTime = this.SelectedDateTime,
                Mode = this.Mode,
                MinDate = this.MinDate,
                MaxDate = this.MaxDate,
                OkButtonText = this.OkButtonText,
                CancelButtonText = this.CancelButtonText,
                ClearButtonText = this.ClearButtonText,
                PopupBorderColor = this.PopupBorderColor,
                PopupBorderWidth = this.PopupBorderWidth,
                PopupCornerRadius = this.PopupCornerRadius,
                PopupPadding = this.PopupPadding,
                PopupPageOverlayColor = this.PopupPageOverlayColor,
                ForeColor = this.ForeColor,
                BodyBackgroundColor = this.BodyBackgroundColor,
                HeaderForeColor = this.HeaderForeColor,
                HeaderBackgroundColor = this.HeaderBackgroundColor,
                ToolButtonsStyle = this.ToolButtonsStyle,
                DayStyle = this.DayStyle,
                DisabledDayStyle = this.DisabledDayStyle,
                OtherMonthDayStyle = this.OtherMonthDayStyle,
                SelectedDayStyle = this.SelectedDayStyle,
                DayNamesStyle = this.DayNamesStyle,
                ShowWeekNumbers = this.ShowWeekNumbers,
                WeekNumberStyle = this.WeekNumberStyle,
                ShowOtherMonthDays = this.ShowOtherMonthDays,
                ActivityIndicatorColor = this.ActivityIndicatorColor,
                ShowClearButton = this.ShowClearButton,
                CloseOnOutsideClick = this.CloseOnOutsideClick,
                Is12HourFormat = this.Is12HourFormat,
                AutomationId = base.AutomationId,
                Translations = this.Translations?.ToList()
            };

            var result = await NullableDateTimePicker.OpenAsync(options);
            if (result is PopupResult popupResult && popupResult.ButtonResult != PopupButtons.Cancel)
            {
                SelectedDateTime = popupResult.SelectedDateTime;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            isPopupOpen = false;
        }
    }

    private void SetCalendarIcon()
    {
        if (HideIcon)
            return;

        MainThreadHelper.SafeBeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(100);
            if (Icon != null)
            {
                _dateTimePickerIcon.Source = Icon;
            }
            else
            {
                string imageName = Mode switch
                {
                    PickerModes.DateTime => "date_time_icon.png",
                    PickerModes.Time => "time_icon.png",
                    _ => "date_icon.png"
                };
                try
                {
                    _dateTimePickerIcon.Source = Utilities.GetImageSource(imageName);
                }
                catch { }
            }
        });
    }

    private static DateTime? ParseDateTime(object objectValue)
    {
        DateTime? dateValue = null;
        if (objectValue is DateTime)
        {
            dateValue = (DateTime?)objectValue;
        }
        else if (objectValue is string strValue)
        {
            if (DateTime.TryParse(strValue, out DateTime outputDate))
            {
                dateValue = outputDate;
            }
        }
        return dateValue;
    }
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(base.IsEnabled):
                _dateTimePickerBorder.IsEnabled = base.IsEnabled;
                _dateTimePickerIcon.IsEnabled = base.IsEnabled;
                break;
            case nameof(base.AutomationId):
                _dateTimePickerEntry.AutomationId = base.AutomationId + "_DatetimePickerEntry";
                _dateTimePickerIcon.AutomationId = base.AutomationId + "_DatetimePickerIcon";
                break;
        }
    }
    #endregion //private methods
}