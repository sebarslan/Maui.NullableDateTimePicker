using System.Globalization;

namespace Maui.NullableDateTimePicker.Samples
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Register a handler for unhandled XAML exceptions
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = new Window(new AppShell());
            return window;
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