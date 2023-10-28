namespace Maui.NullableDateTimePicker.Helpers;

internal class MainThreadHelper
{
    public static void SafeBeginInvokeOnMainThread(Action action)
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

    public static async Task SafeInvokeOnMainThreadAsync(Action action)
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

    public static async Task<T> SafeInvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
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
