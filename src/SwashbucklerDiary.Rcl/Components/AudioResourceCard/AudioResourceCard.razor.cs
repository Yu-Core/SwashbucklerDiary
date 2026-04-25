using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Events;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class AudioResourceCard : MediaResourceComponentBase
    {
        private bool isPlaying;

        private double currentTime;

        private double? seekingTime;

        private double duration;

        private double playbackRate = 1;

        ElementReference audioElementRef;

        AudioFileInfo audioFileInfo = new();

        ResourceModel? previousValue;

        MediaResourcePath? albumCoverResourcePath;

        [Inject]
        private AudioInterop AudioInterop { get; set; } = default!;

        [Inject]
        private ILogger<AudioResourceCard> Logger { get; set; } = default!;

        private string InputStyle => StyleBuilder.Create()
            .Add("--current-width", (CurrentTime / duration * 100).ToString("0.00", CultureInfo.InvariantCulture) + "%")
            .Build();

        private double CurrentTime => seekingTime ?? currentTime;

        private string? AlbumCoverSrc
            => albumCoverResourcePath?.DisPlayedUrl ?? "_content/SwashbucklerDiary.Rcl/img/albumCover.jpeg";

        private string? Title => !string.IsNullOrEmpty(audioFileInfo.Title) ? audioFileInfo.Title : I18n.T("Unknown title");

        private string Artists => audioFileInfo.Artists.Length > 0 ? string.Join(" / ", audioFileInfo.Artists) : I18n.T("Unknown artist");

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousValue != Value)
            {
                previousValue = Value;
                if (Value.ResourceUri is not null)
                {
                    audioFileInfo = await MediaResourceManager.GetAudioFileInfo(Value.ResourceUri);
                    albumCoverResourcePath = MediaResourceManager.ToMediaResourcePath(NavigationManager, audioFileInfo.PictureUri);
                }
            }
        }

        private void HandleOnDurationChange(DurationChangeEventArgs args)
        {
            if (duration == args.Duration)
            {
                return;
            }

            duration = args.Duration;
        }

        private void HandleOnTimeUpdate(TimeUpdateEventArgs args)
        {
            if ((int)currentTime == (int)args.CurrentTime)
            {
                return;
            }

            currentTime = (int)args.CurrentTime;
        }

        private void HandleOnPlaybackRateChange(PlaybackRateChangeEventArgs args)
        {
            if (playbackRate == args.PlaybackRate)
            {
                return;
            }

            playbackRate = args.PlaybackRate;
        }

        private void HandleOnPlay(EventArgs args)
        {
            if (isPlaying)
            {
                return;
            }

            isPlaying = true;
        }

        private void HandleOnPause(EventArgs args)
        {
            if (!isPlaying)
            {
                return;
            }

            isPlaying = false;
        }

        private async Task PlayOrPause()
        {
            try
            {
                // Check if media source is available
                if (mediaResourcePath?.DisPlayedUrl == null)
                {
                    Logger.LogWarning("Audio source is not available");
                    return;
                }

                if (isPlaying)
                {
                    await AudioInterop.PauseAsync(audioElementRef);
                }
                else
                {
                    await AudioInterop.PlayAsync(audioElementRef);
                }
            }
            catch (Exception ex)
            {
                // If play fails (e.g., no valid source), reset the playing state
                isPlaying = false;
                // Log the error or handle it appropriately
                Logger.LogError(ex, "Audio playback error: {Message}", ex.Message);
            }
        }

        private void OnInputCurrentTime(ChangeEventArgs e)
        {
            if (e.Value is not null && int.TryParse(e.Value.ToString()!, out var time))
            {
                seekingTime = time;
            }
        }

        private async Task OnChangeCurrentTime(ChangeEventArgs e)
        {
            if (e.Value is not null && int.TryParse(e.Value.ToString()!, out var time))
            {
                await AudioInterop.SetCurrentTimeAsync(audioElementRef, time);
                currentTime = time;
            }

            seekingTime = null;
        }
    }
}
