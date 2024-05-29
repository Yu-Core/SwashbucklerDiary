using Microsoft.Windows.AppLifecycle;
using SwashbucklerDiary.Rcl.Essentials;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static partial class LaunchActivation
    {
        public static async Task HandleOnLaunchedAsync()
        {
            var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            var mainInstance = AppInstance.FindOrRegisterForKey("main");

            if (!mainInstance.IsCurrent)
            {
                await mainInstance.RedirectActivationToAsync(activatedArgs);
                Process.GetCurrentProcess().Kill();
                return;
            }

            ActivationArguments = await ConvertActivationArguments(activatedArgs);
            AppInstance.GetCurrent().Activated += OnApplicationActivated;
        }

        private static async void OnApplicationActivated(object? sender, AppActivationArguments args)
        {
            var activationArguments = await ConvertActivationArguments(args);
            Activated?.Invoke(activationArguments);
        }

        private static ValueTask<ActivationArguments> ConvertActivationArguments(AppActivationArguments args)
        {
            return args.Kind switch
            {
                ExtendedActivationKind.Protocol => HandleScheme((ProtocolActivatedEventArgs)args.Data),
                ExtendedActivationKind.ShareTarget => HandleShare((ShareTargetActivatedEventArgs)args.Data),
                _ => ValueTask.FromResult(new ActivationArguments() { Kind = LaunchActivationKind.Launch })
            };
        }

        private static ValueTask<ActivationArguments> HandleScheme(ProtocolActivatedEventArgs args)
        {
            var activationArguments = new ActivationArguments()
            {
                Kind = LaunchActivationKind.Scheme,
                Data = args.Uri.ToString()
            };
            return ValueTask.FromResult(activationArguments);
        }

        private static async ValueTask<ActivationArguments> HandleShare(ShareTargetActivatedEventArgs args)
        {
            ShareActivationArguments? shareActivationArguments = null;
            args.ShareOperation.ReportStarted();

            var title = args.ShareOperation.Data.Properties.Title;
            if (args.ShareOperation.Data.Contains(StandardDataFormats.StorageItems))
            {
                var storageItems = await args.ShareOperation.Data.GetStorageItemsAsync();
                var shareFilePaths = storageItems.Select(item => item.Path).ToList();

                shareActivationArguments = new()
                {
                    Title = title,
                    Kind = ShareKind.FilePaths,
                    Data = shareFilePaths
                };
            }
            else if (args.ShareOperation.Data.Contains(StandardDataFormats.Text))
            {
                var text = await args.ShareOperation.Data.GetTextAsync();
                shareActivationArguments = new()
                {
                    Title = title,
                    Kind = ShareKind.Text,
                    Data = text
                };
            }
            else if (args.ShareOperation.Data.Contains(StandardDataFormats.Uri))
            {
                var uri = await args.ShareOperation.Data.GetUriAsync();
                shareActivationArguments = new()
                {
                    Title = title,
                    Kind = ShareKind.Text,
                    Data = uri.ToString()
                };
            }

            // Once we have received the shared data from the ShareOperation, call ReportCompleted()
            args.ShareOperation.ReportCompleted();

            ActivationArguments? activationArguments = null;
            if (shareActivationArguments is not null)
            {
                activationArguments = new()
                {
                    Kind = LaunchActivationKind.Share,
                    Data = shareActivationArguments
                };
            }
            else
            {
                activationArguments = new()
                {
                    Kind = LaunchActivationKind.Launch,
                };
            }

            return activationArguments;
        }
    }
}
