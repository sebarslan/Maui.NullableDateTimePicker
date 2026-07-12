namespace Maui.NullableDateTimePicker.Samples
{

    public class MyPopup : CommunityToolkit.Maui.Views.Popup<PopupResult>, IDisposable
    {
        private bool _disposed = false;

        public MyPopup()
        {
            DisplayInfo displayMetrics = DeviceDisplay.MainDisplayInfo;

            WidthRequest = Math.Max(Math.Min(displayMetrics.Width / displayMetrics.Density, 300), 100);
            HeightRequest = Math.Max(Math.Min(displayMetrics.Height / displayMetrics.Density, 450), 100);

            Margin = 0;
            Padding = 0;
            BackgroundColor = Colors.Transparent;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        ~MyPopup()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {


                Content = null;
            }

            _disposed = true;
        }
    }
}
