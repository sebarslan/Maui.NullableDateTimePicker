namespace Maui.NullableDateTimePicker;

public class DateTimeChangedEventArgs : EventArgs
{
    internal DateTimeChangedEventArgs(DateTime? oldDateTime, DateTime? newDateTime)
    {
        NewDateTime = newDateTime;
        OldDateTime = oldDateTime;
    }

    public DateTime? NewDateTime { get; }
    public DateTime? OldDateTime { get; }
}
