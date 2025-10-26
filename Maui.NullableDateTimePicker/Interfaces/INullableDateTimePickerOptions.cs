#nullable enable
using CommunityToolkit.Maui;

namespace Maui.NullableDateTimePicker;

public interface INullableDateTimePickerOptions
{
    DateTime? SelectedDateTime { get; set; }
    PickerModes Mode { get; set; }
    DateTime? MinDate { get; set; }
    DateTime? MaxDate { get; set; }
    string OkButtonText { get; set; }
    string CancelButtonText { get; set; }
    string ClearButtonText { get; set; }
    Color? PopupBorderColor { get; set; }
    double PopupBorderWidth { get; set; }
    CornerRadius PopupCornerRadius { get; set; }
    Color PopupPageOverlayColor { get; set; }
    Thickness PopupPadding { get; set; }
    Color? BodyBackgroundColor { get; set; }
    AppThemeColor? BodyBackgroundThemeColor { get; set; }
    Color? ForeColor { get; set; }
    AppThemeColor? ForeColorThemeColor { get; set; }
    Color? HeaderForeColor { get; set; }
    AppThemeColor? HeaderForeThemeColor { get; set; }
    Color? HeaderBackgroundColor { get; set; }
    AppThemeColor? HeaderBackgroundThemeColor { get; set; }
    Style? ToolButtonsStyle { get; set; }
    Style? DayStyle { get; set; }
    Style? DisabledDayStyle { get; set; }
    Style? OtherMonthDayStyle { get; set; }
    Style? SelectedDayStyle { get; set; }
    Style? DayNamesStyle { get; set; }
    bool ShowWeekNumbers { get; set; }
    Style? WeekNumberStyle { get; set; }
    bool ShowOtherMonthDays { get; set; }
    Color? ActivityIndicatorColor { get; set; }
    AppThemeColor? ActivityIndicatorThemeColor { get; set; }
    bool ShowClearButton { get; set; }
    bool CloseOnOutsideClick { get; set; }
    bool Is12HourFormat { get; set; }
    string AutomationId { get; set; }
    IList<TranslationItem> Translations { get; set; }
};
