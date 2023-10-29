namespace Maui.NullableDateTimePicker.Helpers;

internal class PopupResultTask<T>
{
    public Task<T> Result { get; }
    private readonly TaskCompletionSource<T> _tcs = new();

    public PopupResultTask()
    {
        Result = _tcs.Task;
    }

    public void SetResult(T result)
    {
        _tcs.SetResult(result);
    }

    public void SetException(Exception exception)
    {
        _tcs.SetException(exception);
    }
}
