namespace Maui.NullableDateTimePicker;

internal class MainThreadHelper
{
    internal static void SafeBeginInvokeOnMainThread(Action action)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            Application.Current.Dispatcher.Dispatch(action);
        }
        else
        {
            MainThread.BeginInvokeOnMainThread(action);
        }
    }

    internal static async Task SafeInvokeOnMainThreadAsync(Action action)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            await Application.Current.Dispatcher.DispatchAsync(action);
        }
        else
        {
            await MainThread.InvokeOnMainThreadAsync(action);
        }
    }

    internal static async Task<T> SafeInvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            return await Application.Current.Dispatcher.DispatchAsync(funcTask);
        }
        else
        {
            return await MainThread.InvokeOnMainThreadAsync(funcTask);
        }
    }
}
