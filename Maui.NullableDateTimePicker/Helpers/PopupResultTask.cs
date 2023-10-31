namespace Maui.NullableDateTimePicker;

internal class PopupResultTask<T>
{
    internal Task<T> Result { get; }
    private readonly TaskCompletionSource<T> _tcs = new();

    internal PopupResultTask()
    {
        Result = _tcs.Task;
    }

    internal void SetResult(T result)
    {
        _tcs.SetResult(result);
    }

    internal void SetException(Exception exception)
    {
        _tcs.SetException(exception);
    }
}
