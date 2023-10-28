#nullable enable
using Maui.NullableDateTimePicker.Enums;

namespace Maui.NullableDateTimePicker.Interfaces;

public interface INullableDateTimePickerOptions
{
    DateTime? InitDateTimeValue { get; set; }
    PickerMode PickerMode { get; set; }
    DateTime? MinDate { get; set; }
    DateTime? MaxDate { get; set; }
    string OkButtonText { get; set; }
    string CancelButtonText { get; set; }
    string ClearButtonText { get; set; }
    Color? BackgroundColor { get; set; }
    Color? ForeColor { get; set; }
    Color? HeaderForeColor { get; set; }
    Color? HeaderBackgroundColor { get; set; }
    Style?    ToolButtonsStyle { get; set; }
    Style? DayStyle { get; set; }
    Style OtherMonthDayStyle { get; set; }
    Style? SelectedDayStyle { get; set; }
    Style? DayNamesStyle { get; set; }
    bool ShowWeekNumbers { get; set; }
    Style? WeekNumberStyle { get; set; }
    bool ShowOtherMonthDays { get; set; }
    Color? ActivityIndicatorColor { get; set; }
    bool ShowClearButton { get; set; }
    bool CloseOnOutsideClick { get; set; }
};
