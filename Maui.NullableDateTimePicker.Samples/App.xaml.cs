namespace Maui.NullableDateTimePicker.Samples
{
    public partial class App : Application
    {
        public App()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("tr-TR");
            InitializeComponent();

            MainPage = new AppShell();

            // Register a handler for unhandled XAML exceptions

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }


        // Handler for unhandled XAML exceptions
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            // Handle the exception here
            Exception exception = (Exception)args.ExceptionObject;
            Console.WriteLine($"Unhandled exception occurred: {exception}");
        }
    }
}