using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using System.Reflection;

namespace Maui.NullableDateTimePicker;

// All the code in this file is included in all platforms.
public class NullableDateTimePicker : ContentView
{
    public event EventHandler<DateTimeChangedEventArgs> NullableDateTimeChanged;
    private Grid _dateTimePickerGrid;
    private Entry _dateTimePickerEntry;
    private Image _dateTimePickerIcon;
    private Border _dateTimePickerBorder;
    private bool isSetIconCalledForFirstTime = false;
    const double defaultHeightRequest = 40;
    static Page Page => Application.Current?.MainPage ?? throw new NullReferenceException();

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
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = this.VerticalTextAlignment
        };

        _dateTimePickerIcon = new Image
        {
            BackgroundColor = this.IconBackgroundColor,
            Aspect = Aspect.AspectFit,
            Margin = 0,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };


        _dateTimePickerGrid.SetColumn(_dateTimePickerEntry, 0);
        _dateTimePickerGrid.Add(_dateTimePickerEntry);

        _dateTimePickerGrid.SetColumn(_dateTimePickerIcon, 1);
        _dateTimePickerGrid.Add(_dateTimePickerIcon);

        TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer
        {
            NumberOfTapsRequired = 1
        };
        tapGestureRecognizer.Tapped += (s, e) =>
        {
            OnDatePickerClicked(s, e);
        };

        _dateTimePickerIcon.GestureRecognizers.Add(tapGestureRecognizer);

        var dateTimePickerStackLayout = new StackLayout
        {
            Margin = 0,
            Padding = 0,
            BackgroundColor = Colors.Transparent
        };
        dateTimePickerStackLayout.Add(_dateTimePickerGrid);

        _dateTimePickerIcon.SetBinding(Image.WidthRequestProperty, new Binding("Height", source: dateTimePickerStackLayout));

        _dateTimePickerIcon.SetBinding(Image.HeightRequestProperty, new Binding("Height", source: dateTimePickerStackLayout));

        _dateTimePickerEntry.SetBinding(Entry.HeightRequestProperty, new Binding("Height", source: dateTimePickerStackLayout));

        _dateTimePickerBorder = new Border
        {
            BackgroundColor = this.BackgroundColor,
            Stroke = this.BorderColor,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(0, 0, 0, 0)
            },
            StrokeThickness = this.BorderWidth,
            Content = dateTimePickerStackLayout,
            Margin = 0,
            Padding = this.Padding,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        _dateTimePickerBorder.GestureRecognizers.Add(tapGestureRecognizer);

        this.Loaded += (s, e) =>
        {
            if (!isSetIconCalledForFirstTime)
                SetCalendarIcon();
        };

        Content = _dateTimePickerBorder;
    }

    public static async Task<object> OpenCalendarAsync(INullableDateTimePickerOptions options)
    {
        PopupResultTask<PopupResult> popupResultTask = null;
        try
        {
            popupResultTask = new PopupResultTask<PopupResult>();

            using (var popupControl = new NullableDateTimePickerPopup(options))
            {
                var result = await CommunityToolkit.Maui.Views.PopupExtensions.ShowPopupAsync(Page, popupControl);

                if (result is PopupResult popupResult)
                {
                    popupResultTask.SetResult(popupResult);
                }
                else
                {
                    popupResultTask.SetResult(null);
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        return await popupResultTask?.Result;
    }

    #region bindable properties

    public static readonly BindableProperty NullableDateTimeProperty =
    BindableProperty.Create(nameof(NullableDateTime),
        typeof(DateTime?),
        typeof(NullableDateTimePicker),
        null,
        defaultBindingMode: BindingMode.TwoWay,
        null,
        (bindable, oldValue, newValue) =>
        {
            var self = (NullableDateTimePicker)bindable;
            var oldNullableDateTime = (DateTime?)oldValue;
            var newNullableDateTime = (DateTime?)newValue;

            self._dateTimePickerEntry.Text = newNullableDateTime?.ToString(self.Format);

            //Date changed event
            bool isDateTimeChanged = false;
            if (self.Mode == PickerModes.Date && oldNullableDateTime?.Date != newNullableDateTime?.Date)
            {
                isDateTimeChanged = true;
            }
            else if (self.Mode == PickerModes.DateTime && (oldNullableDateTime?.Date != newNullableDateTime?.Date || oldNullableDateTime?.TimeOfDay != newNullableDateTime?.TimeOfDay))
            {
                isDateTimeChanged = true;
            }
            else if (self.Mode == PickerModes.Time && oldNullableDateTime?.TimeOfDay != newNullableDateTime?.TimeOfDay)
            {
                isDateTimeChanged = true;
            }

            if (isDateTimeChanged)
                self.NullableDateTimeChanged?.Invoke(self, new DateTimeChangedEventArgs(oldNullableDateTime, newNullableDateTime));
        });

    public DateTime? NullableDateTime
    {
        get { return (DateTime?)GetValue(NullableDateTimeProperty); }
        set
        {
            SetValue(NullableDateTimeProperty, value);
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
                self._dateTimePickerEntry.Text = self.NullableDateTime?.ToString(self.Format);
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

    #endregion //bindable properties

    private async void OnDatePickerClicked(object sender, EventArgs e)
    {
        if (!base.IsEnabled)
            return;

        await OpenCalendarPopupAsync();
    }

    bool isPopupOpen = false;

    private async Task OpenCalendarPopupAsync()
    {
        try
        {
            if (isPopupOpen)
                return;

            isPopupOpen = true;

            INullableDateTimePickerOptions options = new NullableDateTimePickerOptions
            {
                NullableDateTime = this.NullableDateTime,
                Mode = this.Mode,
                MinDate = this.MinDate,
                MaxDate = this.MaxDate,
                OkButtonText = this.OkButtonText,
                CancelButtonText = this.CancelButtonText,
                ClearButtonText = this.ClearButtonText,
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
                Is12HourFormat = this.Is12HourFormat
            };

            var result = await NullableDateTimePicker.OpenCalendarAsync(options);
            if (result is PopupResult popupResult && popupResult.ButtonResult != PopupButtons.Cancel)
            {
                NullableDateTime = popupResult.NullableDateTime;
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

    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(base.IsEnabled):
                _dateTimePickerBorder.IsEnabled = base.IsEnabled;
                _dateTimePickerIcon.IsEnabled = base.IsEnabled;
                break;
        }
    }


    string imgName;
    ImageSource imgSource;

    private void SetCalendarIcon()
    {
        MainThreadHelper.SafeBeginInvokeOnMainThread(async () =>
        {
            isSetIconCalledForFirstTime = true;
            await Task.Delay(100);

            if (Icon != null)
            {
                if (imgSource != Icon)
                {
                    imgSource = Icon;

                    _dateTimePickerIcon.Source = Icon;
                }
            }
            else
            {
                string imageName = "date_icon.png";

                if (Mode == PickerModes.DateTime)
                    imageName = "date_time_icon.png";
                else if (Mode == PickerModes.Time)
                    imageName = "time_icon.png";

                if (imgName != imageName)
                {
                    imgName = imageName;

                    _dateTimePickerIcon.Source = await Task.Run(() => ImageSource.FromResource($"Maui.NullableDateTimePicker.Images.{imageName}", typeof(NullableDateTimePicker).GetTypeInfo().Assembly));
                }
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
}
