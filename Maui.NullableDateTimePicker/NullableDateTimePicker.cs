using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Graphics.Platform;
using System.Reflection;

namespace Maui.NullableDateTimePicker;

// All the code in this file is included in all platforms.
public class NullableDateTimePicker : ContentView
{
    public event EventHandler<DateTimeChangedEventArgs> NullableDateTimeChanged;
    Microsoft.Maui.Controls.Grid _contentLayout;
    internal Entry _dateTimePickerEntry;
    internal ImageButton _dateTimePickerIcon;

    public NullableDateTimePicker()
    {
        BackgroundColor = Colors.Transparent;
        _dateTimePickerEntry = new Entry()
        {
            IsVisible = true,
            MinimumWidthRequest = 50,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            FontSize = 12,
            Margin = 0,
            IsReadOnly = true
        };

        _dateTimePickerIcon = new ImageButton
        {
            WidthRequest = 30,
            Aspect = Aspect.AspectFit,
            BackgroundColor = Color.FromArgb("#E1E1E1"),
            Padding = 2,
            Margin = 0
        };

        _dateTimePickerIcon.Clicked += OnDatePickerIconClicked;


        _contentLayout = new Microsoft.Maui.Controls.Grid
        {
            Padding = 0,
            ColumnSpacing = 0,
            RowSpacing = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
            }
        };
        _contentLayout.SetColumn(_dateTimePickerEntry, 0);
        _contentLayout.SetColumn(_dateTimePickerIcon, 1);
        _contentLayout.Add(_dateTimePickerEntry);
        _contentLayout.Add(_dateTimePickerIcon);

        Content = _contentLayout;
    }

    #region bindable properties
    public static readonly BindableProperty NullableDateTimeProperty =
    BindableProperty.Create(nameof(NullableDateTime),
        typeof(DateTime?),
        typeof(NullableDateTimePicker),
        null,
        defaultBindingMode: BindingMode.TwoWay,
        null,
        (b, o, n) =>
        {
            var self = (NullableDateTimePicker)b;
            var oldNullableDateTime = (DateTime?)o;
            var newNullableDateTime = (DateTime?)n;

            self._dateTimePickerEntry.Text = newNullableDateTime?.ToString(self.Format);

            //Date changed event
            bool isDateChanged = false;
            if (self.Mode == PickerMode.Date && oldNullableDateTime?.Date != newNullableDateTime?.Date)
            {
                isDateChanged = true;
            }
            else if (self.Mode == PickerMode.DateTime && (oldNullableDateTime?.Date != newNullableDateTime?.Date || oldNullableDateTime?.TimeOfDay != newNullableDateTime?.TimeOfDay))
            {
                isDateChanged = true;
            }
            else if (self.Mode == PickerMode.Time && oldNullableDateTime?.TimeOfDay != newNullableDateTime?.TimeOfDay)
            {
                isDateChanged = true;
            }

            if (isDateChanged)
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
                if (Mode == PickerMode.Time)
                    format = "t";
                else if (Mode == PickerMode.DateTime)
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

    public static readonly BindableProperty ToolButtonsStyleProperty =
BindableProperty.Create(nameof(ToolButtonsStyle), typeof(Style), typeof(Button), defaultValue: null, defaultBindingMode: BindingMode.OneWay,
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
        typeof(PickerMode),
        typeof(NullableDateTimePicker),
        PickerMode.Date,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (newValue is string strValue)
            {
                if (PickerMode.TryParse(strValue, out PickerMode pickerMode))
                {
                    newValue = pickerMode;
                }
            }
        });

    public PickerMode Mode
    {
        get { return (PickerMode)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }

    public static readonly BindableProperty MinDateProperty =
    BindableProperty.Create(nameof(MinDate), typeof(DateTime?), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, (view, value) =>
    {
        return (value != null && (DateTime)value >= DateTime.MinValue && (DateTime)value <= DateTime.MaxValue);
    },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (newValue is string strValue)
            {
                if (DateTime.TryParse(strValue, out DateTime minDate))
                {
                    newValue = minDate;
                }
            }
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
    BindableProperty.Create(nameof(MaxDate), typeof(DateTime?), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, (view, value) =>
    {
        return (value != null && (DateTime)value >= DateTime.MinValue && (DateTime)value <= DateTime.MaxValue);
    }, propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (newValue is string strValue)
        {
            if (DateTime.TryParse(strValue, out DateTime maxDate))
            {
                newValue = maxDate;
            }
        }
    });

    public DateTime? MaxDate
    {
        get { return (DateTime?)GetValue(MaxDateProperty); }
        set
        {
            SetValue(MaxDateProperty, value);
        }
    }

    public static readonly BindableProperty HeaderBackgroundColorProperty =
    BindableProperty.Create(nameof(HeaderBackgroundColor), typeof(Color), typeof(NullableDateTimePicker), null, defaultBindingMode: BindingMode.OneWay, (view, value) =>
    {
        return (value != null && (DateTime)value >= DateTime.MinValue && (DateTime)value <= DateTime.MaxValue);
    }, (b, o, n) =>
    {
    });

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

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
    nameof(Icon),
    typeof(ImageSource),
    typeof(NullableDateTimePicker),
    defaultValue: null,
    defaultBindingMode: BindingMode.OneWay,
    validateValue: null,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (newValue != null)
            ((NullableDateTimePicker)bindable)._dateTimePickerIcon.Source = (ImageSource)newValue;
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
        defaultValue: Color.FromArgb("#E1E1E1"),
        defaultBindingMode: BindingMode.OneWay,
        validateValue: null,
        propertyChanged: (b, o, n) =>
    {
        ((NullableDateTimePicker)b)._dateTimePickerIcon.BackgroundColor = (Color)o ?? Color.FromArgb("#E1E1E1");
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
    validateValue: null,
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
    defaultValue: null,
    defaultBindingMode: BindingMode.OneWay,
    validateValue: null,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (newValue != null)
            ((NullableDateTimePicker)bindable)._dateTimePickerEntry.FontSize = (double)newValue;
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
    defaultValue: null,
    defaultBindingMode: BindingMode.OneWay,
    validateValue: null,
    propertyChanged: (bindable, oldValue, newValue) =>
    {
        if (newValue != null)
            ((NullableDateTimePicker)bindable)._dateTimePickerEntry.TextColor = (Color)newValue;
    });

    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
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
    #endregion //bindable properties

    private void OnDatePickerIconClicked(object sender, EventArgs e)
    {
        OpenCalendarPopup();
    }

    bool isPopupOpen = false;

    private async void OpenCalendarPopup()
    {
        if (isPopupOpen)
            return;

        isPopupOpen = true;

        try
        {
            var options = new NullableDateTimePickerOptions
            {
                InitDateTimeValue = this.NullableDateTime,
                PickerMode = this.Mode,
                MinDate = this.MinDate,
                MaxDate = this.MaxDate,
                OkButtonText = this.OkButtonText,
                CancelButtonText = this.CancelButtonText,
                ClearButtonText = this.ClearButtonText,
                HeaderForeColor = this.HeaderForeColor,
                HeaderBackgroundColor = this.HeaderBackgroundColor,
                ToolButtonsStyle = this.ToolButtonsStyle,
                DayStyle = this.DayStyle,
                OtherMonthDayStyle = this.OtherMonthDayStyle,
                SelectedDayStyle = this.SelectedDayStyle,
                DayNamesStyle = this.DayNamesStyle,
                ShowWeekNumbers = this.ShowWeekNumbers,
                WeekNumberStyle = this.WeekNumberStyle,
                ShowOtherMonthDays = this.ShowOtherMonthDays,
                ActivityIndicatorColor = this.ActivityIndicatorColor,
                ShowClearButton = this.ShowClearButton
            };

            NullableDateTimePickerPopup popupControl = new NullableDateTimePickerPopup(options);

            await MainThreadHelper.SafeInvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var result = await popupControl.OpenPopupAsync();
                    if (result is PopupResult popupResult)
                    {
                        NullableDateTime = popupResult.DateTimeResult;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    popupControl = null;
                }
            });
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
                _dateTimePickerEntry.IsEnabled = base.IsEnabled;
                _dateTimePickerIcon.IsEnabled = base.IsEnabled;
                break;
        }
    }

    bool isLoaded = false;
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext != null && !isLoaded)
        {
            isLoaded = true;
            Padding = 0;
            Margin = 0;
            HorizontalOptions = LayoutOptions.Fill;
            VerticalOptions = LayoutOptions.Fill;

            SetCalendarIcon();
        }
    }

    private void SetCalendarIcon()
    {
        if (Icon == null && _dateTimePickerIcon.Source == null)
        {
            string imageName = "date_icon.png";

            if (Mode == PickerMode.DateTime)
                imageName = "date_time_icon.png";
            else if (Mode == PickerMode.Time)
                imageName = "time_icon.png";

            _dateTimePickerIcon.Source = ImageSource.FromResource("Maui.NullableDateTimePicker.Images." + imageName, typeof(NullableDateTimePicker).Assembly); 
        }
    }
}