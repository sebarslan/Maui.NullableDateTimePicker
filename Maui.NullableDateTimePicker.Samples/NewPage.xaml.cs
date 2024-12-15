namespace Maui.NullableDateTimePicker.Samples;

public partial class NewPage : ContentPage
{
    public NewPage()
    {
        InitializeComponent();
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}