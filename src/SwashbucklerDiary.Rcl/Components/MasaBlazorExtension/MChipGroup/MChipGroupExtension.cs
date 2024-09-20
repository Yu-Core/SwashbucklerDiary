using Masa.Blazor;
using Masa.Blazor.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MChipGroupExtension : MChipGroup
    {
        protected override void RegisterWatchers(PropertyWatcher watcher)
        {
            watcher.Watch<double>(nameof(ScrollOffset), OnScrollOffsetChanged);

            watcher.Watch<bool>(nameof(Column), (val) =>
            {
                if (val)
                {
                    ScrollOffset = 0;
                }

                NextTick(OnResize);
            }, immediate: true);
        }

        private async void OnScrollOffsetChanged(double val)
        {
            if (RTL)
            {
                val = -val;
            }

            var scroll = val <= 0
                ? Bias(-val)
                : val > ContentWidth - WrapperWidth
                    ? -(ContentWidth - WrapperWidth) + Bias(ContentWidth - WrapperWidth - val)
                    : -val;

            if (RTL)
            {
                scroll = -scroll;
            }

            if (WrapperRef.Context != null)
            {
                await Js.ScrollTo(WrapperRef, 0, -scroll);
            }
        }

        private static double Bias(double val)
        {
            var c = 0.501;
            var x = Math.Abs(val);
            return Math.Sign(val) * (x / ((1 / c - 2) * (1 - x) + 1));
        }
    }
}
