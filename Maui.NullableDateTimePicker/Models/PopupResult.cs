using Maui.NullableDateTimePicker.Enums;

namespace Maui.NullableDateTimePicker.Modes
{
    public class PopupResult
    {
        internal PopupResult(DateTime? nullableDateTime, PopupButtons buttonResult)
        {
            NullableDateTime = nullableDateTime;
            ButtonResult = buttonResult;
        }

        public PopupButtons ButtonResult { get; }
        public DateTime? NullableDateTime { get; }
    }
}
