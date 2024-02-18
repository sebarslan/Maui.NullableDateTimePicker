#nullable enable
namespace Maui.NullableDateTimePicker;

public interface INullableDateTimePickerOptions
{
    DateTime? NullableDateTime { get; set; }
    PickerModes Mode { get; set; }
    DateTime? MinDate { get; set; }
    DateTime? MaxDate { get; set; }
    string OkButtonText { get; set; }
    string CancelButtonText { get; set; }
    string ClearButtonText { get; set; }
    Color? BodyBackgroundColor { get; set; }
    Color? ForeColor { get; set; }
    Color? HeaderForeColor { get; set; }
    Color? HeaderBackgroundColor { get; set; }
    Style?    ToolButtonsStyle { get; set; }
    Style? DayStyle { get; set; }
    Style? DisabledDayStyle { get; set; }
    Style? OtherMonthDayStyle { get; set; }
    Style? SelectedDayStyle { get; set; }
    Style? DayNamesStyle { get; set; }
    bool ShowWeekNumbers { get; set; }
    Style? WeekNumberStyle { get; set; }
    bool ShowOtherMonthDays { get; set; }
    Color? ActivityIndicatorColor { get; set; }
    bool ShowClearButton { get; set; }
    bool CloseOnOutsideClick { get; set; }
    bool Is12HourClock { get; set; }
};
