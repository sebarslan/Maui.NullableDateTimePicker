#nullable enable 
namespace Maui.NullableDateTimePicker;

public class NullableDateTimePickerOptions : INullableDateTimePickerOptions
{
    public DateTime? NullableDateTime { get; set; }
    public PickerModes Mode { get; set; } = PickerModes.Date;
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public string OkButtonText { get; set; } = "OK";
    public string CancelButtonText { get; set; } = "Cancel";
    public string ClearButtonText { get; set; } = "Clear";
    public Color? BodyBackgroundColor { get; set; }
    public Color? ForeColor { get; set; }
    public Color? HeaderForeColor { get; set; }
    public Color? HeaderBackgroundColor { get; set; }
    public Style? ToolButtonsStyle { get; set; }
    public Style? DayStyle { get; set; }
    public Style? DisabledDayStyle { get; set; }
    public Style? OtherMonthDayStyle { get; set; }
    public Style? SelectedDayStyle { get; set; }
    public Style? DayNamesStyle { get; set; }
    public bool ShowWeekNumbers { get; set; }
    public Style? WeekNumberStyle { get; set; }
    public bool ShowOtherMonthDays { get; set; } = true;
    public Color? ActivityIndicatorColor { get; set; }
    public bool ShowClearButton { get; set; } = true;
    public bool CloseOnOutsideClick { get; set; }
}
