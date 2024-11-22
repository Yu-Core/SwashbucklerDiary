using DeepCloner.Core;
using Masa.Blazor;

namespace SwashbucklerDiary.Rcl.Services
{
    public class MasaBlazorHelper
    {
        readonly MasaBlazor _masaBlazor;

        Breakpoint _previousBreakpoint = default!;
        Breakpoints? _previousBreakpointName;

        public Breakpoint Breakpoint => _masaBlazor.Breakpoint;

        public event EventHandler<MyBreakpointChangedEventArgs>? BreakpointChanged;

        public MasaBlazorHelper(MasaBlazor masaBlazor)
        {
            _masaBlazor = masaBlazor;
            _masaBlazor.BreakpointChanged += MasaBlazorBreakpointChanged;
            UpdatePreviousBreakpoint();
        }

        private void MasaBlazorBreakpointChanged(object? sender, BreakpointChangedEventArgs e)
        {
            if (_previousBreakpointName == Breakpoint.Name)
            {
                return;
            }

            _previousBreakpointName = Breakpoint.Name;
            var eventArgs = new MyBreakpointChangedEventArgs()
            {
                XsChanged = _previousBreakpoint.Xs != Breakpoint.Xs,
                SmChanged = _previousBreakpoint.Sm != Breakpoint.Sm,
                MdChanged = _previousBreakpoint.Md != Breakpoint.Md,
                LgChanged = _previousBreakpoint.Lg != Breakpoint.Lg,
                XlChanged = _previousBreakpoint.Xl != Breakpoint.Xl,
                SmAndDownChanged = _previousBreakpoint.SmAndDown != Breakpoint.SmAndDown,
                SmAndUpChanged = _previousBreakpoint.SmAndUp != Breakpoint.SmAndUp,
                MdAndDownChanged = _previousBreakpoint.MdAndDown != Breakpoint.MdAndDown,
                MdAndUpChanged = _previousBreakpoint.MdAndUp != Breakpoint.MdAndUp,
                LgAndDownChanged = _previousBreakpoint.LgAndDown != Breakpoint.LgAndDown,
                LgAndUpChanged = _previousBreakpoint.LgAndUp != Breakpoint.LgAndUp,
            };

            UpdatePreviousBreakpoint();
            BreakpointChanged?.Invoke(this, eventArgs);
        }

        private void UpdatePreviousBreakpoint()
        {
            _previousBreakpoint = Breakpoint.ShallowClone();
        }
    }
}
