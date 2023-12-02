
namespace Maui.NullableDateTimePicker.Samples
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();

            // Create Datetimepicker Programmatically
            CreateDateTimePickerProgrammatically();
        }

        DateTime? myDateTime = DateTime.Now;
        public DateTime? MyDateTime
        {
            get => myDateTime;
            set
            {
                myDateTime = value;
                OnPropertyChanged(nameof(MyDateTime));
            }
        }

        readonly FontImageSource calendarIcon = new()
        {
            Glyph = IconFont.CalendarDay,
            FontAutoScalingEnabled = false,
            FontFamily = "FontAwesome",
            Color = Colors.Black
        };

        public FontImageSource CalendarIcon => calendarIcon;

        readonly FontImageSource calendarIconAppTheme = new()
        {
            Glyph = IconFont.CalendarDays,
            FontFamily = "FontAwesome",
            FontAutoScalingEnabled = false,
            Color = Application.Current.RequestedTheme == AppTheme.Dark ? Colors.White : Colors.Black
        };
        public FontImageSource CalendarIconAppTheme => calendarIconAppTheme;

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);

            await Navigation.PushModalAsync(new NewPage());
        }

        // Datepicker programmatically
        private void CreateDateTimePickerProgrammatically()
        {
            Maui.NullableDateTimePicker.NullableDateTimePicker datePicker = new()
            {
                Mode = PickerModes.Date,
                Format = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern,
                ShowWeekNumbers = true,
                ShowOtherMonthDays = true,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center
            };

            datePicker.SetAppThemeColor(Maui.NullableDateTimePicker.NullableDateTimePicker.ForeColorProperty, Microsoft.Maui.Graphics.Colors.Black, Microsoft.Maui.Graphics.Colors.White);
            datePicker.SetAppThemeColor(Maui.NullableDateTimePicker.NullableDateTimePicker.BodyBackgroundColorProperty, Microsoft.Maui.Graphics.Colors.White, Microsoft.Maui.Graphics.Colors.Black);


            // binding
            datePicker.BindingContext = this;
            datePicker.SetBinding(Maui.NullableDateTimePicker.NullableDateTimePicker.NullableDateTimeProperty, nameof(MyDateTime), BindingMode.TwoWay);

            DateTimePlaceStackLayout.Add(datePicker);
        }


        // Calling nullabledatetimepicker calendar popup directly with own entry and button
        private async void DateTimePicker_Clicked(object sender, EventArgs e)
        {
            INullableDateTimePickerOptions nullableDateTimePickerOptions = new NullableDateTimePickerOptions
            {
                NullableDateTime = MyDateTime,
                Mode = PickerModes.DateTime,
                ShowWeekNumbers = true
            };

            var result = await NullableDateTimePicker.OpenCalendarAsync(nullableDateTimePickerOptions);
            if (result is PopupResult popupResult && popupResult.ButtonResult != PopupButtons.Cancel)
            {
                MyDateTime = popupResult.NullableDateTime;
                //DateTimeEntry.Text = popupResult.NullableDateTime?.ToString("g"); //If you are not using ViewModel
            }
        }

        public DateTime? MyMinDate => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10);
        public DateTime? MyMaxDate => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 20);
    }

    static class IconFont
    {
        public const string Calendar = "\uf133";
        public const string CalendarDay = "\uf783";
        public const string CalendarDays = "\uf073";
    }
}