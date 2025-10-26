#nullable enable 
using CommunityToolkit.Maui;

namespace Maui.NullableDateTimePicker;

public class NullableDateTimePickerOptions : INullableDateTimePickerOptions
{
    public DateTime? SelectedDateTime { get; set; }
    public PickerModes Mode { get; set; } = PickerModes.Date;
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public string OkButtonText { get; set; } = "OK";
    public string CancelButtonText { get; set; } = "Cancel";
    public string ClearButtonText { get; set; } = "Clear";
    public Color? PopupBorderColor { get; set; } = Colors.Transparent;
    public double PopupBorderWidth { get; set; } = 0.0d;
    public CornerRadius PopupCornerRadius { get; set; } = new CornerRadius(0);
    public Thickness PopupPadding { get; set; } = new Thickness(0);

    public Color PopupPageOverlayColor { get; set; } = Colors.Black.WithAlpha(0.5f);

    public Color? BodyBackgroundColor { get; set; }
    public AppThemeColor? BodyBackgroundThemeColor { get; set; } = new AppThemeColor { Light = Colors.White, Dark = Color.FromRgba("#434343") };
    public Color? ForeColor { get; set; }
    public AppThemeColor? ForeColorThemeColor { get; set; } = new AppThemeColor { Light = Colors.Black, Dark = Colors.White };
    public Color? HeaderForeColor { get; set; }
    public AppThemeColor? HeaderForeThemeColor { get; set; } = new AppThemeColor { Light = Colors.White, Dark = Colors.White };
    public Color? HeaderBackgroundColor { get; set; }
    public AppThemeColor? HeaderBackgroundThemeColor { get; set; } = new AppThemeColor { Light = Color.FromRgba("#2b0b98"), Dark = Color.FromRgba("#252626") };
    public Style? ToolButtonsStyle { get; set; }
    public Style? DayStyle { get; set; }
    public Style? DisabledDayStyle { get; set; }
    public Style? OtherMonthDayStyle { get; set; }
    public Style? SelectedDayStyle { get; set; }
    public Style? DayNamesStyle { get; set; }
    public bool ShowWeekNumbers { get; set; }
    public Style? WeekNumberStyle { get; set; }
    public bool ShowOtherMonthDays { get; set; } = true;
    public bool ShowClearButton { get; set; } = true;
    public Color? ActivityIndicatorColor { get; set; }
    public AppThemeColor? ActivityIndicatorThemeColor { get; set; }
    public bool CloseOnOutsideClick { get; set; }
    public bool Is12HourFormat { get; set; }
    public string AutomationId { get; set; }
    public IList<TranslationItem> Translations { get; set; }
}
