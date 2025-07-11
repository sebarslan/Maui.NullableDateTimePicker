﻿using System.Windows.Input;

namespace Maui.NullableDateTimePicker.Samples;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        BindingContext = this;
        InitializeComponent();
        themeSwitch.IsToggled = Application.Current.RequestedTheme == AppTheme.Dark;

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

    public bool Is12HourFormat => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern.Contains("tt");

    private ICommand _OpenModalCommand;
    public ICommand OpenModalCommand => _OpenModalCommand ??= new Command(async () => await Shell.Current.Navigation.PushModalAsync(new NewPage()));

    // Datepicker programmatically
    private void CreateDateTimePickerProgrammatically()
    {
        NullableDateTimePicker datePicker = new()
        {
            Mode = PickerModes.Date,
            Format = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern,
            ShowWeekNumbers = true,
            ShowOtherMonthDays = true,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center
        };

        datePicker.SetAppThemeColor(NullableDateTimePicker.ForeColorProperty, Colors.Black, Colors.White);
        datePicker.SetAppThemeColor(NullableDateTimePicker.BodyBackgroundColorProperty, Colors.White, Colors.Black);


        // binding
        datePicker.BindingContext = this;
        datePicker.SetBinding(NullableDateTimePicker.NullableDateTimeProperty, nameof(MyDateTime), BindingMode.TwoWay);

        DateTimePlaceStackLayout.Add(datePicker);
    }


    // Calling nullabledatetimepicker calendar popup directly with own entry and button
    private async void DateTimePicker_Clicked(object sender, EventArgs e)
    {
        INullableDateTimePickerOptions nullableDateTimePickerOptions = new NullableDateTimePickerOptions
        {
            NullableDateTime = MyDateTime,
            Mode = PickerModes.DateTime,
            ShowWeekNumbers = true,
            CloseOnOutsideClick = true,
            PopupPageOverlayColor = Color.FromArgb("#505050").WithAlpha(0.5f),
            PopupBorderColor = Color.FromArgb("#505050"),
            PopupCornerRadius = 10,
            PopupBorderWidth = 1,
            PopupPadding = 5,
        };

        try
        {
            var result = await NullableDateTimePicker.OpenAsync(nullableDateTimePickerOptions);
            if (result is PopupResult popupResult && popupResult.ButtonResult != PopupButtons.Cancel)
            {
                MyDateTime = popupResult.NullableDateTime;
                //DateTimeEntry.Text = popupResult.NullableDateTime?.ToString("g"); //If you are not using ViewModel
            }
        }
        catch (Exception ex)
        {

        }
    }

    public DateTime? MyMinDate => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10);
    public DateTime? MyMaxDate => new DateTime(DateTime.Now.Year, DateTime.Now.Month, 20);

    private void OnThemeToggled(object sender, ToggledEventArgs e)
    {
        bool isDarkMode = e.Value;

        App.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
    }
}

static class IconFont
{
    public const string Calendar = "\uf133";
    public const string CalendarDay = "\uf783";
    public const string CalendarDays = "\uf073";
}