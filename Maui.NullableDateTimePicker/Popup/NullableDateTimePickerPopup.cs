using CommunityToolkit.Maui.Views;
using static System.Net.Mime.MediaTypeNames;
using CommunityToolkitPopup = CommunityToolkit.Maui.Views.Popup;
namespace Maui.NullableDateTimePicker
{
    internal class NullableDateTimePickerPopup : CommunityToolkitPopup
    {
        private readonly EventHandler<EventArgs> okButtonClickedHandler = null;
        private readonly EventHandler<EventArgs> clearButtonClickedHandler = null;
        private readonly EventHandler<EventArgs> cancelButtonClickedHandler = null;
        private NullableDateTimePickerContent _content = null;
        
        internal NullableDateTimePickerPopup(INullableDateTimePickerOptions options)
        {
            _content = new NullableDateTimePickerContent(options);

            DisplayInfo displayMetrics = DeviceDisplay.MainDisplayInfo;
            Color = Colors.Transparent;
            var popupWidth = Math.Min(displayMetrics.Width / displayMetrics.Density, 300);
            var popupHeight = Math.Min(displayMetrics.Height / displayMetrics.Density, 450);
            Size = new Size(Math.Max(popupWidth, 100), Math.Max(popupHeight, 100));

            CanBeDismissedByTappingOutsideOfPopup = options.CloseOnOutsideClick;

            this.Opened += _content.NullableDateTimePickerPopupOpened;

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

        internal void ClosePopup(PopupButtons buttonResult)
        {
            try
            {
                _content.OkButtonClicked -= okButtonClickedHandler;
                _content.ClearButtonClicked -= clearButtonClickedHandler;
                _content.CancelButtonClicked -= cancelButtonClickedHandler;

                Close(new PopupResult(_content.SelectedDate, buttonResult));
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
    }
}