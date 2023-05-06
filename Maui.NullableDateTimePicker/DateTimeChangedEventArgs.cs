namespace Maui.NullableDateTimePicker
{
    public class DateTimeChangedEventArgs : EventArgs
    {
        public DateTimeChangedEventArgs(DateTime? oldDateTime, DateTime? newDateTime)
        {
            NewDateTime = newDateTime;
            OldDateTime = oldDateTime;
        }

        public DateTime? NewDateTime { get; }
        public DateTime? OldDateTime { get; }
    }
}
