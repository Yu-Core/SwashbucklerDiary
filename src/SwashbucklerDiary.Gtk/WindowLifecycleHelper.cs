namespace SwashbucklerDiary.Gtk
{
    public class WindowLifecycleHelper
    {
        private bool _isActive;
        private bool _isFirstActiveChanged = true;
        private readonly global::Gtk.Window _window;

        public event EventHandler? Resumed;
        public event EventHandler? Stopped;

        public WindowLifecycleHelper(global::Gtk.Window window)
        {
            _window = window;
            _window.OnStateFlagsChanged += StateFlagsChanged;
        }

        private void StateFlagsChanged(global::Gtk.Widget sender, global::Gtk.Widget.StateFlagsChangedSignalArgs args)
        {
            var isActive = GetWindowActive(_window, args.Flags);
            if (_isActive != isActive)
            {
                _isActive = isActive;
                if (_isFirstActiveChanged)
                {
                    _isFirstActiveChanged = false;
                    return;
                }

                if (_isActive)
                {
                    Resumed?.Invoke(this, EventArgs.Empty);

                }
                else
                {
                    Stopped?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private static bool GetWindowActive(global::Gtk.Window window, global::Gtk.StateFlags stateFlags)
        {
            if (window.IsActive)
            {
                return true;
            }

            if (stateFlags.HasFlag(global::Gtk.StateFlags.FocusWithin) && !stateFlags.HasFlag(global::Gtk.StateFlags.Backdrop))
            {
                return true;
            }

            return false;
        }
    }
}