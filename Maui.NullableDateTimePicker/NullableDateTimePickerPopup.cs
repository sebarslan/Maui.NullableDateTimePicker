using CommunityToolkit.Maui.Views;
namespace Maui.NullableDateTimePicker
{
    internal class NullableDateTimePickerPopup : Popup
    {
        static Page Page => Application.Current?.MainPage;
        readonly EventHandler<EventArgs> okButtonClickedHandler = null;
        readonly EventHandler<EventArgs> clearButtonClickedHandler = null;
        readonly EventHandler<EventArgs> cancelButtonClickedHandler = null;
        NullableDateTimePickerContent _content = null;

        internal NullableDateTimePickerPopup(NullableDateTimePickerOptions options)
        {
            _content = new NullableDateTimePickerContent(options); ;

            DisplayInfo displayMetrics = DeviceDisplay.MainDisplayInfo;

            var popupWidth = Math.Min(displayMetrics.Width, 300);
            var popupHeight = Math.Min(displayMetrics.Height, 450);
            Size = new Size(popupWidth, popupHeight);
            CanBeDismissedByTappingOutsideOfPopup = options.CloseOnOutsideClick;

            this.Opened += _content.NullableDateTimePickerPopupOpened; ;

            okButtonClickedHandler = (s, e) =>
            {
                ClosePopup(PopupButtonResult.Ok);
            };
            _content.OkButtonClicked += okButtonClickedHandler;

            clearButtonClickedHandler = (s, e) =>
            {
                ClosePopup(PopupButtonResult.Clear);
            };
            _content.ClearButtonClicked += clearButtonClickedHandler;

            cancelButtonClickedHandler = (s, e) =>
            {
                ClosePopup(PopupButtonResult.Cancel);
            };
            _content.CancelButtonClicked += cancelButtonClickedHandler;

            Content = _content;
        }

        internal void ClosePopup(PopupButtonResult buttonResult)
        {
            try
            {
                _content.OkButtonClicked -= okButtonClickedHandler;
                _content.ClearButtonClicked -= clearButtonClickedHandler;
                _content.CancelButtonClicked -= cancelButtonClickedHandler;

                Close(new PopupResult { DateTimeResult = _content.SelectedDate, ButtonResult = buttonResult });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _content = null;
            }
        }

        internal async Task<object> OpenPopupAsync()
        {
            return await Page?.ShowPopupAsync(this);
        }
    }
}