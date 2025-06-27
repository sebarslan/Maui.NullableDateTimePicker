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
    internal NullableDateTimePickerPopup(INullableDateTimePickerOptions options)
    {
        _cancellationTokenSource = new CancellationTokenSource();
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

        okButtonClickedHandler = (s, e) =>
        {
            ClosePopupAsync(PopupButtons.Ok);
        };
        _content.OkButtonClicked += okButtonClickedHandler;

        clearButtonClickedHandler = (s, e) =>
        {
            ClosePopupAsync(PopupButtons.Clear);
        };
        _content.ClearButtonClicked += clearButtonClickedHandler;

        cancelButtonClickedHandler = (s, e) =>
        {
            ClosePopupAsync(PopupButtons.Cancel);
        };
        _content.CancelButtonClicked += cancelButtonClickedHandler;

        Content = _content;
    }

    static Page MainPage => Shell.Current;
    internal async Task<Maui.NullableDateTimePicker.PopupResult> OpenPopupAsync()
    {
        //var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
        //if (mainPage == null)
        //    return null;

        if (MainPage == null)
            return null;

        var result = await MainPage.ShowPopupAsync<PopupResult>(this, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = _options.CloseOnOutsideClick,
            PageOverlayColor = _options.PopupPageOverlayColor,
            Shape = new RoundRectangle
            {
                CornerRadius = _options.PopupBorderCornerRadius,
                Stroke = _options.PopupBorderColor,
                StrokeThickness = _options.PopupBorderWidth
            }
        }, _cancellationTokenSource.Token);

        if (result is IPopupResult<PopupResult> popupResult && !result.WasDismissedByTappingOutsideOfPopup)
        {
            return popupResult.Result;
        }
        return null;
    }


    internal async void ClosePopupAsync(PopupButtons buttonResult)
    {
        try
        {
            await base.CloseAsync(new PopupResult(_content.SelectedDate, buttonResult));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
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