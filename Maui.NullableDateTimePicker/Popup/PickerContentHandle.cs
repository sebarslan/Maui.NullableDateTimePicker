namespace Maui.NullableDateTimePicker;

public class PickerContentHandle
{
    public View View { get; }
    public Task<PopupResult> ResultTask { get; }

    internal PickerContentHandle(View view, Task<PopupResult> resultTask)
    {
        View = view;
        ResultTask = resultTask;
    }
}
