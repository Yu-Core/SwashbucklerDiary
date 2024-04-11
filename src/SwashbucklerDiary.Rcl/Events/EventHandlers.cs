using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Events;

[EventHandler("oncustomdurationchange", typeof(DurationChangeEventArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("oncustomtimeupdate", typeof(TimeUpdateEventArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
[EventHandler("oncustomplaybackratechange", typeof(PlaybackRateChangeEventArgs),
    enableStopPropagation: true, enablePreventDefault: true)]
public static class EventHandlers
{
}
