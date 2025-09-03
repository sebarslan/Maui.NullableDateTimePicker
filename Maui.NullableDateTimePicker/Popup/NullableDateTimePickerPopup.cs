// The MauiCommunityToolkit popup crashes on Windows when used in a Modal Page. See: https://github.com/CommunityToolkit/Maui/issues/2459
// Fixed https://github.com/CommunityToolkit/Maui/pull/2476

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls.Shapes;


namespace Maui.NullableDateTimePicker;

internal class NullableDateTimePickerPopup : CommunityToolkit.Maui.Views.Popup<PopupResult>, IDisposable
{
    private readonly EventHandler<EventArgs> okButtonClickedHandler = null;
    private readonly EventHandler<EventArgs> clearButtonClickedHandler = null;
    private readonly EventHandler<EventArgs> cancelButtonClickedHandler = null;
    private NullableDateTimePickerContent _content = null;
    private bool _disposed = false;
    INullableDateTimePickerOptions _options;
    CancellationTokenSource _cancellationTokenSource;
    internal NullableDateTimePickerPopup(INullableDateTimePickerOptions options, CancellationTokenSource cancellationTokenSource = default)
    {

        _cancellationTokenSource = cancellationTokenSource;
        _options = options;
        _content = new NullableDateTimePickerContent(options);


        if (options.AutomationId == null)
            options.AutomationId = "";

        base.AutomationId = options.AutomationId + "_DatetimePickerPopup";

        DisplayInfo displayMetrics = DeviceDisplay.MainDisplayInfo;

        base.BackgroundColor = Colors.Transparent;

        WidthRequest = Math.Max(Math.Min(displayMetrics.Width / displayMetrics.Density, 300), 100);
        HeightRequest = Math.Max(Math.Min(displayMetrics.Height / displayMetrics.Density, 450), 100);
        Margin = 0;
        Padding = 0;

        base.Opened += _content.NullableDateTimePickerPopupOpened;

        okButtonClickedHandler = async (s, e) =>
        {
            await ClosePopupAsync(PopupButtons.Ok);
        };
        _content.OkButtonClicked += okButtonClickedHandler;

        clearButtonClickedHandler = async (s, e) =>
        {
            await ClosePopupAsync(PopupButtons.Clear);
        };
        _content.ClearButtonClicked += clearButtonClickedHandler;

        cancelButtonClickedHandler = async (s, e) =>
        {
            await ClosePopupAsync(PopupButtons.Cancel);
        };
        _content.CancelButtonClicked += cancelButtonClickedHandler;

        Content = _content;
    }

    Page _hostPage;
    internal async Task<PopupResult> OpenPopupAsync(Page page = null)
    {
        _hostPage = page;
        if (_hostPage == null)
            _hostPage = FindHostPage();

        if (_hostPage == null)
            throw new InvalidOperationException("Could not find a valid Page to show popup.");


        var result = await _hostPage.ShowPopupAsync<PopupResult>(this, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = _options.CloseOnOutsideClick,
            PageOverlayColor = _options.PopupPageOverlayColor,
            Shape = new RoundRectangle
            {
                CornerRadius = _options.PopupCornerRadius,
                Stroke = _options.PopupBorderColor,
                StrokeThickness = _options.PopupBorderWidth
            }
        }, _cancellationTokenSource.Token);

        if (result is IPopupResult<PopupResult> popupResult && !result.WasDismissedByTappingOutsideOfPopup)
        {
            return popupResult.Result;
        }

        return new PopupResult(null, PopupButtons.Cancel);
    }

    internal async Task ClosePopupAsync(PopupButtons buttonResult)
    {
        try
        {
            var popupResult = new PopupResult(_content.SelectedDateTime, buttonResult);
            await _hostPage.ClosePopupAsync(popupResult);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ClosePopupAsync Error: " + ex);
        }
    }

    public static Page? FindHostPage()
    {
        var rootPage = Application.Current?.Windows.FirstOrDefault()?.Page;

        //if (rootPage?.Navigation?.ModalStack?.Count > 0)
        //{
        //    var topModal = rootPage.Navigation.ModalStack.Last();

        //    if (topModal is NavigationPage nav)
        //        return nav.CurrentPage;

        //    return topModal;
        //}

        //if (rootPage is Shell shell)
        //{
        //    rootPage = (shell.CurrentItem?.CurrentItem as IShellSectionController)?
        //        .PresentedPage;
        //}

        //if (rootPage is NavigationPage mainNav)
        //    return mainNav.CurrentPage;

        return rootPage;
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
                _content.OkButtonClicked -= okButtonClickedHandler;
                _content.ClearButtonClicked -= clearButtonClickedHandler;
                _content.CancelButtonClicked -= cancelButtonClickedHandler;
                this.Content = null;
                _content = null;
            }
            _disposed = true;
        }
    }
}