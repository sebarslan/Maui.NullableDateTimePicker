using System.Windows.Input;

namespace Maui.NullableDateTimePicker.Samples;

public partial class NewPage : ContentPage
{
    public NewPage()
    {
        BindingContext = this;
        InitializeComponent();
    }

    private ICommand _CloseModalCommand;
    public ICommand CloseModalCommand => _CloseModalCommand ??= new Command(async () => await Navigation.PopModalAsync());

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var result = await NullableDateTimePicker.OpenCalendarAsync(new NullableDateTimePickerOptions
        {   
            Mode = PickerModes.Date,
            CloseOnOutsideClick = true,
            ShowClearButton = true,          
            PopupBorderCornerRadius = 10,
            PopupBorderThemeColor = new CommunityToolkit.Maui.AppThemeColor
            {
                Light = Colors.Black,
                Dark = Colors.White,
            },
            PopupBorderWidth = 1,
            PopupPadding = 5,
        });

        if (result is PopupResult popupResult && popupResult.ButtonResult != PopupButtons.Cancel)
        {
            if (sender is Button button)
            {
                button.Text = popupResult.NullableDateTime.HasValue ? popupResult.NullableDateTime.Value.ToString("D") : "NULL";
            }            
        }
    }
}