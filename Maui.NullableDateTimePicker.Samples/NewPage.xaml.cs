using System.Windows.Input;

namespace Maui.NullableDateTimePicker.Samples;

public partial class NewPage : ContentPage
{
    public NewPage()
    {
        BindingContext = this;
        InitializeComponent();
        DateTimePickerInModal.ParentPage = this;
    }

    private ICommand _CloseModalCommand;
    public ICommand CloseModalCommand => _CloseModalCommand ??= new Command(async () => await Navigation.PopModalAsync());

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var result = await NullableDateTimePicker.OpenAsync(new NullableDateTimePickerOptions
        {
            Mode = PickerModes.Date,
            CloseOnOutsideClick = true,
            ShowClearButton = true,
            PopupCornerRadius = 10,
            PopupBorderColor = Colors.Black,
            PopupBorderWidth = 1,
            PopupPadding = 5,
        }, this);

        if (result is PopupResult popupResult && popupResult.ButtonResult != PopupButtons.Cancel)
        {
            if (sender is Button button)
            {
                button.Text = popupResult.NullableDateTime.HasValue ? popupResult.NullableDateTime.Value.ToString("D") : "NULL";
            }
        }
    }
}