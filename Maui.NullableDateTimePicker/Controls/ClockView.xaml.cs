using Maui.NullableDateTimePicker.Helpers;
using Microsoft.Maui.Graphics;

namespace Maui.NullableDateTimePicker.Controls;

internal partial class ClockView : ContentView
{
    public event EventHandler<TimeChangedEventArgs> TimeChanged;
    private readonly ClockDrawable _drawable;

    INullableDateTimePickerOptions _options;
    public ClockView(INullableDateTimePickerOptions options)
    {
        _options = options;
        InitializeComponent();
        _drawable = (ClockDrawable)Resources["clockDrawable"];
        _drawable.Is12HourFormat = options.Is12HourFormat;

        graphicsView.AutomationId = options.AutomationId + "_ClockGraphicsView";
        ClockAmButton.AutomationId = options.AutomationId + "_ClockAmButton";
        ClockPmButton.AutomationId = options.AutomationId + "_ClockPmButton";
        ClockAmButton.Text = Utilities.GetTranslateText(_options.Translations, "AM", _options.Is12HourFormat ? "AM" : "00-11");
        ClockPmButton.Text = Utilities.GetTranslateText(_options.Translations, "PM", _options.Is12HourFormat ? "PM" : "12-23");

        ClockHourButton.AutomationId = options.AutomationId + "_ClockHourButton";
        ClockMinuteButton.AutomationId = options.AutomationId + "_ClockMinuteButton";
        ClockHourButton.Text = Utilities.GetTranslateText(_options.Translations, "Hour", "Hour");
        ClockMinuteButton.Text = Utilities.GetTranslateText(_options.Translations, "Minute", "Minute");

        SetButtonSelected(_drawable.IsAmMode, _drawable.IsHourMode);
    }

    public static readonly BindableProperty SelectedTimeProperty =
        BindableProperty.Create(nameof(SelectedTime), typeof(TimeOnly), typeof(ClockView), TimeOnly.FromDateTime(DateTime.Now), propertyChanged: OnSelectedTimeChanged);

    public TimeOnly SelectedTime
    {
        get => (TimeOnly)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    private static void OnSelectedTimeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ClockView clockView && newValue is TimeOnly newTime)
        {
            if (oldValue != newValue)
            {
                clockView._drawable.IsAmMode = newTime.Hour < 12;
                clockView._drawable.SetClockValueFromTime(newTime);
                clockView.RefreshGraphicsView();
                clockView.TimeChanged?.Invoke(clockView, new TimeChangedEventArgs((TimeOnly)oldValue, (TimeOnly)newValue));
            }
        }
    }

    public static readonly BindableProperty HourDotColorProperty =
       BindableProperty.Create(nameof(HourDotColor), typeof(Color), typeof(ClockView), Color.FromArgb("#4f20b0"), propertyChanged: OnHourDotColorChanged);

    public Color HourDotColor
    {
        get => (Color)GetValue(HourDotColorProperty);
        set => SetValue(HourDotColorProperty, value);
    }

    private static void OnHourDotColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ClockView clockView && newValue is Color newColor)
        {
            if (oldValue != newValue)
            {
                clockView._drawable.HourDotColor = newColor;
                clockView.RefreshGraphicsView();
            }
        }
    }

    public static readonly BindableProperty MinuteDotColorProperty =
       BindableProperty.Create(nameof(MinuteDotColor), typeof(Color), typeof(ClockView), Color.FromArgb("#4f20b0"), propertyChanged: OnMinuteDotColorChanged);

    public Color MinuteDotColor
    {
        get => (Color)GetValue(MinuteDotColorProperty);
        set => SetValue(MinuteDotColorProperty, value);
    }

    private static void OnMinuteDotColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ClockView clockView && newValue is Color newColor)
        {
            if (oldValue != newValue)
            {
                clockView._drawable.MinuteDotColor = newColor;
                clockView.RefreshGraphicsView();
            }
        }
    }
    public static readonly BindableProperty ClockModeButtonBackgroundColorProperty =
        BindableProperty.Create(nameof(ClockModeButtonBackgroundColor), typeof(Color), typeof(ClockView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is ClockView clockView && newValue is Color newColor)
            {
                if (oldValue != newValue)
                {
                    clockView.ClockAmButton.OuterBorderBackgroundColor = newColor;
                    clockView.ClockPmButton.OuterBorderBackgroundColor = newColor;
                    clockView.ClockHourButton.OuterBorderBackgroundColor = newColor;
                    clockView.ClockMinuteButton.OuterBorderBackgroundColor = newColor;
                }
            }
        });

    public Color ClockModeButtonBackgroundColor
    {
        get => (Color)GetValue(ClockModeButtonBackgroundColorProperty);
        set => SetValue(ClockModeButtonBackgroundColorProperty, value);
    }

    public static readonly BindableProperty ClockModeButtonSelectedBackgroundColorProperty =
        BindableProperty.Create(nameof(ClockModeButtonSelectedBackgroundColor), typeof(Color), typeof(ClockView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is ClockView clockView && newValue is Color newColor)
            {
                if (oldValue != newValue)
                {
                    clockView.ClockAmButton.SelectedOuterBorderBackgroundColor = newColor;
                    clockView.ClockPmButton.SelectedOuterBorderBackgroundColor = newColor;
                    clockView.ClockHourButton.SelectedOuterBorderBackgroundColor = newColor;
                    clockView.ClockMinuteButton.SelectedOuterBorderBackgroundColor = newColor;
                }
            }
        });

    public Color ClockModeButtonSelectedBackgroundColor
    {
        get => (Color)GetValue(ClockModeButtonSelectedBackgroundColorProperty);
        set => SetValue(ClockModeButtonSelectedBackgroundColorProperty, value);
    }

    public static readonly BindableProperty ClockModeButtonTextColorProperty =
        BindableProperty.Create(nameof(ClockModeButtonTextColor), typeof(Color), typeof(ClockView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is ClockView clockView && newValue is Color newColor)
            {
                if (oldValue != newValue)
                {
                    clockView.ClockAmButton.InnerLabelTextColor = newColor;
                    clockView.ClockPmButton.InnerLabelTextColor = newColor;
                    clockView.ClockHourButton.InnerLabelTextColor = newColor;
                    clockView.ClockMinuteButton.InnerLabelTextColor = newColor;
                }
            }
        });

    public Color ClockModeButtonTextColor
    {
        get => (Color)GetValue(ClockModeButtonTextColorProperty);
        set => SetValue(ClockModeButtonTextColorProperty, value);
    }

    public static readonly BindableProperty ClockModeButtonSelectedTextColorProperty =
        BindableProperty.Create(nameof(ClockModeButtonSelectedTextColor), typeof(Color), typeof(ClockView), null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            if (bindable is ClockView clockView && newValue is Color newColor)
            {
                if (oldValue != newValue)
                {
                    clockView.ClockAmButton.SelectedInnerLabelTextColor = newColor;
                    clockView.ClockPmButton.SelectedInnerLabelTextColor = newColor;
                    clockView.ClockHourButton.SelectedInnerLabelTextColor = newColor;
                    clockView.ClockMinuteButton.SelectedInnerLabelTextColor = newColor;
                }
            }
        });

    public Color ClockModeButtonSelectedTextColor
    {
        get => (Color)GetValue(ClockModeButtonSelectedTextColorProperty);
        set => SetValue(ClockModeButtonSelectedTextColorProperty, value);
    }

    private void OnGraphicsTapped(object sender, TappedEventArgs e)
    {
        if (e.GetPosition(graphicsView) is not Point touchPoint) return;

        var dirtyRect = graphicsView.Bounds;

        float centerX = (float)dirtyRect.Width / 2f;
        float centerY = (float)dirtyRect.Height / 2f;

        float scale = Math.Min((float)dirtyRect.Width / 200f, (float)dirtyRect.Height / 200f);

        _drawable.HandleTap((float)touchPoint.X, (float)touchPoint.Y, centerX, centerY, scale);

        SetTimeValueFromClock();

        //if (_drawable.LastTappedGroup == TappedGroup.Hour)
        //{
        //    Console.WriteLine($"Hour clicked: {_drawable.LastTappedValue}");
        //    _drawable.IsHourMode = false;
        //    SetTimeValueFromClock(_drawable.LastTappedValue, SelectedTime.Minute);
        //}
        //else if (_drawable.LastTappedGroup == TappedGroup.Minute)
        //{
        //    Console.WriteLine($"Minute clicked: {_drawable.LastTappedValue}");
        //    _drawable.IsHourMode = true;
        //    SetTimeValueFromClock(SelectedTime.Hour, _drawable.LastTappedValue);
        //}
    }


    private void SetTimeValueFromClock()
    {
        int hour12 = _drawable.Hour;
        int minute = _drawable.Minute;
        int second = _drawable.Second;

        int hour24;
        if (_options.Is12HourFormat)
        {
            // AM/PM format
            hour24 = _drawable.IsAmMode
                ? (hour12 == 12 ? 0 : hour12)        // 12 AM = 00
                : (hour12 == 12 ? 12 : hour12 + 12); // 12 PM = 12
        }
        else
        {
            if (_drawable.Is24Mode)
            {
                // Midday side of 24-hour mode (12–23)
                hour24 = hour12 % 12 + 12;  // 12 → 12, 1 → 13, ..., 11 → 23
            }
            else
            {
                // Night side of 24-hour mode (00–11)
                hour24 = hour12 % 12;       // 12 → 0, 1–11 → 1–11
            }
        }

        var selectedTime = new TimeOnly(hour24 % 24, minute, second);
        if (selectedTime == SelectedTime)
            RefreshGraphicsView();

        SelectedTime = selectedTime;
    }
    private void OnAmButtonClicked(object sender, EventArgs e)
    {
        if (_drawable.IsAmMode)
            return;
        _drawable.IsHourMode = true;
        _drawable.IsAmMode = true;
        SetTimeValueFromClock();
    }

    private void OnPmButtonClicked(object sender, EventArgs e)
    {
        if (!_drawable.IsAmMode)
            return;
        _drawable.IsHourMode = true;
        _drawable.IsAmMode = false;
        SetTimeValueFromClock();
    }

    private void OnHourButtonClicked(object sender, EventArgs e)
    {
        if (_drawable.IsHourMode)
            return;

        _drawable.IsHourMode = true;
        SetTimeValueFromClock();
    }
    private void OnMinuteButtonClicked(object sender, EventArgs e)
    {
        if (!_drawable.IsHourMode)
            return;
        _drawable.IsHourMode = false;
        SetTimeValueFromClock();
    }

    private void SetButtonSelected(bool isAmMode, bool isHourMode)
    {
        ClockHourButton.Selected = false;
        ClockMinuteButton.Selected = false;
        if (isHourMode)
        {
            ClockHourButton.Selected = true;
        }
        else
        {
            ClockMinuteButton.Selected = true;

        }
        ClockAmButton.Selected = false;
        ClockPmButton.Selected = false;

        if (isAmMode)
        {
            ClockAmButton.Selected = true;
        }
        else
        {
            ClockPmButton.Selected = true;
        }
    }

    private void RefreshGraphicsView()
    {
        SetButtonSelected(_drawable.IsAmMode, _drawable.IsHourMode);
        graphicsView.Dispatcher.Dispatch(() =>
        {
            graphicsView.Invalidate();
        });
    }
}