namespace Maui.NullableDateTimePicker;

public class PopupResult
{
    internal PopupResult(DateTime? selectedDateTime, PopupButtons buttonResult)
    {
        SelectedDateTime = selectedDateTime;
        ButtonResult = buttonResult;
    }

    public PopupButtons ButtonResult { get; }
    public DateTime? SelectedDateTime { get; }
}
