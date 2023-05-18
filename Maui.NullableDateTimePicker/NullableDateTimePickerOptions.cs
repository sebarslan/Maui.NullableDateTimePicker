using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.NullableDateTimePicker
{
    internal class NullableDateTimePickerOptions
    {
        public DateTime? InitDateTimeValue { get; set; }
        public PickerMode PickerMode { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public string OkButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public string ClearButtonText { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForeColor { get; set; }
        public Color HeaderForeColor { get; set; }
        public Color HeaderBackgroundColor { get; set; }
        public Style ToolButtonsStyle { get; set; }
        public Style DayStyle { get; set; }
        public Style OtherMonthDayStyle { get; set; }
        public Style SelectedDayStyle { get; set; }
        public Style DayNamesStyle { get; set; }
        public bool ShowWeekNumbers { get; set; }
        public Style WeekNumberStyle { get; set; }
        public bool ShowOtherMonthDays { get; set; }
        public Color ActivityIndicatorColor { get; set; }
        public bool ShowClearButton { get; set; }
        public bool CloseOnOutsideClick { get; set; } = true;
    }
}
