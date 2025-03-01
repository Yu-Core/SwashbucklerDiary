using Microsoft.Maui.Dispatching;

namespace Microsoft.AspNetCore.Components.WebView.Gtk;


/// <summary>
/// DUMMY
/// </summary>
public class GtkDispatcher : Dispatcher
{
    readonly IDispatcher _dispatcher;

    public GtkDispatcher(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override bool CheckAccess()
    {
        return !_dispatcher.IsDispatchRequired;
    }

    public override Task InvokeAsync(Action workItem)
    {
        return _dispatcher.DispatchAsync(workItem);
    }

    public override Task InvokeAsync(Func<Task> workItem)
    {
        return _dispatcher.DispatchAsync(workItem);
    }

    public override Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem)
    {
        return _dispatcher.DispatchAsync(workItem);
    }

    public override Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
    {
        return _dispatcher.DispatchAsync(workItem);
    }
}