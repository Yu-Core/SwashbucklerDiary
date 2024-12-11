using Microsoft.Windows.AppLifecycle;
using SwashbucklerDiary.Rcl.Essentials;
using System.Diagnostics;
using System.Reflection;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static partial class LaunchActivation
    {
        public static async Task OnLaunchedAsync()
        {
            var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            var mainInstance = AppInstance.FindOrRegisterForKey("main");

            if (!mainInstance.IsCurrent)
            {
                await mainInstance.RedirectActivationToAsync(activatedArgs);
                Process.GetCurrentProcess().Kill();
                return;
            }

            mainInstance.Activated += OnActivated;
            ActivationArguments = await ConvertActivationArguments(activatedArgs);
        }

        private static async void OnActivated(object? sender, AppActivationArguments args)
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
                ExtendedActivationKind.Launch => HandleLaunch((LaunchActivatedEventArgs)args.Data),
                _ => NewLaunchActivationArguments()
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

        private static ValueTask<ActivationArguments> HandleLaunch(LaunchActivatedEventArgs args)
        {
            var id = ArgumentsToId(args.Arguments);
            if (!string.IsNullOrEmpty(id))
            {
                return HandleAppActions(id);
            }

            return NewLaunchActivationArguments();
        }

        private static ValueTask<ActivationArguments> NewLaunchActivationArguments()
            => ValueTask.FromResult(new ActivationArguments() { Kind = LaunchActivationKind.Launch });

        private static async ValueTask<ActivationArguments> HandleAppActions(string id)
        {
            var activationArguments = await AppActionsHelper.ConvertActivationArguments(id);
            if (activationArguments is not null)
            {
                return activationArguments;
            }
            else
            {
                return await NewLaunchActivationArguments();
            }
        }

        private readonly static MethodInfo ArgumentsToIdMethod = typeof(AppActionsExtensions).GetMethod("ArgumentsToId", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new Exception("Method ArgumentsToId does not exist");
        static string? ArgumentsToId(string arguments)
        {
            return ArgumentsToIdMethod.Invoke(null, [arguments]) as string;
        }
    }
}
