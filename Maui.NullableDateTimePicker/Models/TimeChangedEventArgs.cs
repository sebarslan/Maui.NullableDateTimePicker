namespace Maui.NullableDateTimePicker;

public class TimeChangedEventArgs : EventArgs
{
    public TimeChangedEventArgs(TimeOnly oldTime, TimeOnly newTime)
    {
        NewTime = newTime;
        OldTime = oldTime;
    }

    public TimeOnly NewTime { get; }
    public TimeOnly OldTime { get; }
}
