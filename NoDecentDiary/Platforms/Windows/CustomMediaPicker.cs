using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.System;
using WinRT.Interop;

namespace NoDecentDiary;

public static class WindowsMediaPicker
{
    public static Task<FileResult?> CapturePhotoAsync()
        => CaptureAsync(false);

    public static Task<FileResult?> CaptureVideoAsync()
        => CaptureAsync(true);

    private static async Task<FileResult?> CaptureAsync(bool isVideo)
    {
        var captureUi = new CustomCameraCaptureUI();

        StorageFile? file = await captureUi.CaptureFileAsync(isVideo ? CameraCaptureUIMode.Video : CameraCaptureUIMode.Photo);

        if (file != null)
        {
            return new FileResult(file.Path, file.ContentType);
        }

        return null;
    }

    private class CustomCameraCaptureUI
    {
        private readonly LauncherOptions _launcherOptions;

        public CustomCameraCaptureUI()
        {
            var window = WindowStateManager.Default.GetActiveWindow();
            var handle = WindowNative.GetWindowHandle(window);

            _launcherOptions = new LauncherOptions();
            InitializeWithWindow.Initialize(_launcherOptions, handle);

            _launcherOptions.TreatAsUntrusted = false;
            _launcherOptions.DisplayApplicationPicker = false;
            _launcherOptions.TargetApplicationPackageFamilyName = "Microsoft.WindowsCamera_8wekyb3d8bbwe";
        }

        public async Task<StorageFile?> CaptureFileAsync(CameraCaptureUIMode mode)
        {
            var extension = mode == CameraCaptureUIMode.Photo ? ".jpg" : ".mp4";

            var currentAppData = ApplicationData.Current;
            var tempLocation = currentAppData.LocalCacheFolder;
            var tempFileName = $"capture{extension}";
            var tempFile = await tempLocation.CreateFileAsync(tempFileName, CreationCollisionOption.GenerateUniqueName);
            var token = Windows.ApplicationModel.DataTransfer.SharedStorageAccessManager.AddFile(tempFile);

            var set = new ValueSet();
            if (mode == CameraCaptureUIMode.Photo)
            {
                set.Add("MediaType", "photo");
                set.Add("PhotoFileToken", token);
            }
            else
            {
                set.Add("MediaType", "video");
                set.Add("VideoFileToken", token);
            }

            var uri = new Uri("microsoft.windows.camera.picker:");
            var result = await Windows.System.Launcher.LaunchUriForResultsAsync(uri, _launcherOptions, set);
            if (result.Status == LaunchUriStatus.Success && result.Result != null)
            {
                return tempFile;
            }

            return null;
        }
    }
}
