// The MauiCommunityToolkit popup crashes on Windows when used in a Modal Page. (System.Runtime.InteropServices.COMException: 'Catastrophic failure XamlRoot must be explicitly set for unparented popup')
// The Mopup popup displays behind Modal on Android
// => if Windows Use Mopup else use MauiCommunityToolkit
#if WINDOWS
using LibraryPopup = Mopups.Pages.PopupPage;
#else
using LibraryPopup = CommunityToolkit.Maui.Views.Popup;
#endif

namespace Maui.NullableDateTimePicker
{
    internal class NullableDateTimePickerPopup : LibraryPopup, IDisposable
    {
        private readonly EventHandler<EventArgs> okButtonClickedHandler = null;
        private readonly EventHandler<EventArgs> clearButtonClickedHandler = null;
        private readonly EventHandler<EventArgs> cancelButtonClickedHandler = null;
        private NullableDateTimePickerContent _content = null;
        private bool _disposed = false;
        internal NullableDateTimePickerPopup(INullableDateTimePickerOptions options)
        {
            _content = new NullableDateTimePickerContent(options);

            if (options.AutomationId == null)
                options.AutomationId = "";

            this.AutomationId = options.AutomationId + "_DatetimePickerPopup";

            DisplayInfo displayMetrics = DeviceDisplay.MainDisplayInfo;
#if WINDOWS
            this.BackgroundColor = Colors.Transparent;
#else
            this.Color = Colors.Transparent;
#endif

            var popupWidth = Math.Max(Math.Min(displayMetrics.Width / displayMetrics.Density, 300), 100);
            var popupHeight = Math.Max(Math.Min(displayMetrics.Height / displayMetrics.Density, 450), 100);

#if WINDOWS
            if (options.CloseOnOutsideClick)
            {
                _content.WidthRequest = popupWidth;
                _content.HeightRequest = popupHeight;
            }
#else
            this.Size = new Size(popupWidth, popupHeight);
            this.CanBeDismissedByTappingOutsideOfPopup = options.CloseOnOutsideClick;
#endif


#if WINDOWS
            this.Appearing += _content.NullableDateTimePickerPopupAppearing;
#else
            this.Opened += _content.NullableDateTimePickerPopupOpened;
#endif


            okButtonClickedHandler = (s, e) =>
            {
                ClosePopup(PopupButtons.Ok);
            };
            _content.OkButtonClicked += okButtonClickedHandler;

            clearButtonClickedHandler = (s, e) =>
            {
                ClosePopup(PopupButtons.Clear);
            };
            _content.ClearButtonClicked += clearButtonClickedHandler;

            cancelButtonClickedHandler = (s, e) =>
            {
                ClosePopup(PopupButtons.Cancel);
            };
            _content.CancelButtonClicked += cancelButtonClickedHandler;

            Content = _content;
        }


#if WINDOWS
        private TaskCompletionSource<object?> _tcs = new TaskCompletionSource<object?>();
        public Task<object?> WaitForResultAsync()
        {
            return _tcs.Task;
        }
#endif

        internal void ClosePopup(PopupButtons buttonResult)
        {
            try
            {
                _content.OkButtonClicked -= okButtonClickedHandler;
                _content.ClearButtonClicked -= clearButtonClickedHandler;
                _content.CancelButtonClicked -= cancelButtonClickedHandler;

#if WINDOWS     
                _tcs.TrySetResult(new PopupResult(_content.SelectedDate, buttonResult));
#else
                Close(new PopupResult(_content.SelectedDate, buttonResult));
#endif

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Content = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~NullableDateTimePickerPopup()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _content = null;
                }
                _disposed = true;
            }
        }
    }
}