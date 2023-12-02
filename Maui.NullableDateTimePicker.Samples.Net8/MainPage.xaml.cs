namespace Maui.NullableDateTimePicker.Samples.Net8
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
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
    }

}
