using Microsoft.Maui.Graphics;

namespace Maui.NullableDateTimePicker.Controls;

public partial class NullableDateTimePickerClockView : ContentView
{
    public event EventHandler<TimeChangedEventArgs> TimeChanged;
    private readonly ClockDrawable _drawable;

    INullableDateTimePickerOptions _options;
    public NullableDateTimePickerClockView(INullableDateTimePickerOptions options)
    {
        _options = options;
        InitializeComponent();
        _drawable = (ClockDrawable)Resources["clockDrawable"];
        _drawable.Is12HourFormat = options.Is12HourFormat;

        graphicsView.AutomationId = options.AutomationId + "_ClockGraphicsView";
        AmPmToggleButton.AutomationId = options.AutomationId + "_ClockAmPmToggleButton";
        HourMinuteToggleButton.AutomationId = options.AutomationId + "_ClockModeToggleButton";

        SetButtonSelected(_drawable.IsAmMode, _drawable.IsHourMode);
    }


    public static readonly BindableProperty SelectedTimeProperty =
        BindableProperty.Create(nameof(SelectedTime), typeof(TimeOnly), typeof(NullableDateTimePickerClockView), TimeOnly.FromDateTime(DateTime.Now), propertyChanged: OnSelectedTimeChanged);

    public TimeOnly SelectedTime
    {
        get => (TimeOnly)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    private static void OnSelectedTimeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NullableDateTimePickerClockView clockView && newValue is TimeOnly newTime)
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
    private void OnAmPmToggleButtonClicked(object sender, EventArgs e)
    {
        AmPmToggleButton.VisualState = "Selected";
        _drawable.IsHourMode = true;
        _drawable.IsAmMode = !_drawable.IsAmMode;
        SetTimeValueFromClock();
        Task.Run(async () => { await Task.Delay(300); AmPmToggleButton.VisualState = "Normal"; });
    }

    private void OnHourMinuteToggleButtonClicked(object sender, EventArgs e)
    {
        HourMinuteToggleButton.VisualState = "Selected";
        _drawable.IsHourMode = !_drawable.IsHourMode;
        SetTimeValueFromClock();
        Task.Run(async () => { await Task.Delay(300); HourMinuteToggleButton.VisualState = "Normal"; });
    }

    private void SetButtonSelected(bool isAmMode, bool isHourMode)
    {
        AmPmToggleButton.Text = _options.Is12HourFormat ? _drawable.IsAmMode ? "PM" : "AM" : _drawable.Is24Mode ? "00-11" : "12-23";
        HourMinuteToggleButton.Text = _drawable.IsHourMode ? "Minute" : "Hour";
    }

    private void RefreshGraphicsView()
    {
        SetButtonSelected(_drawable.IsAmMode, _drawable.IsHourMode);
        graphicsView.Invalidate();
    }
}