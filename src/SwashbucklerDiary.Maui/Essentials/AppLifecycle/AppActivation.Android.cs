using Android.Content;
using SwashbucklerDiary.Maui.Extensions;
using SwashbucklerDiary.Rcl.Essentials;
using System.Reflection;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static partial class AppActivation
    {
        public static void Launch(Intent intent)
        {
            AppLifecycle.Default.ActivationArguments = CreateArgumentsFromIntent(intent);
        }

        public static void Activate(Intent intent)
        {
            var activationArguments = CreateArgumentsFromIntent(intent);
            AppLifecycle.Default.NotifyActivated(activationArguments);
        }

        private static ActivationArguments CreateArgumentsFromIntent(Intent intent)
        {
            return intent.Action switch
            {
                Intent.ActionView => HandleScheme(intent),
                Intent.ActionSend => HandleShare(intent),
                Intent.ActionSendMultiple => HandleShareMultiple(intent),
                _ => new ActivationArguments() { Kind = AppActivationKind.Launch }
            };
        }

        private static ActivationArguments HandleScheme(Intent intent)
        {
            var data = intent.Data;
            return new()
            {
                Kind = AppActivationKind.Scheme,
                Data = data?.ToString()
            };
        }

        private static ActivationArguments HandleShare(Intent intent)
        {
            ShareActivationArguments? shareActivationArguments = null;
            string? title = intent.GetStringExtra(Intent.ExtraTitle);
            var mimeType = intent.Type;
            if (mimeType == "text/plain")
            {
                string? text = intent.GetStringExtra(Intent.ExtraText);
                shareActivationArguments = new()
                {
                    Kind = ShareActivationKind.Text,
                    Data = text,
                    Title = title
                };
            }
            else
            {
                var uri = intent.GetParcelableExtra<Android.Net.Uri>(Intent.ExtraStream);
                if (uri is not null)
                {
                    var filePath = EnsurePhysicalPath(uri);
                    List<string> filePaths = [filePath];
                    shareActivationArguments = new()
                    {
                        Kind = ShareActivationKind.FilePaths,
                        Data = filePaths,
                        Title = title
                    };
                }
            }

            return CreateActivationArguments(shareActivationArguments);
        }

        private static ActivationArguments HandleShareMultiple(Intent intent)
        {
            ShareActivationArguments? shareActivationArguments = null;
            string? title = intent.GetStringExtra(Intent.ExtraTitle);

            var uri = intent.GetParcelableArrayListExtra<Android.Net.Uri>(Intent.ExtraStream);
            if (uri is not null)
            {
                var filePaths = uri.Select(EnsurePhysicalPath).ToList();
                shareActivationArguments = new()
                {
                    Kind = ShareActivationKind.FilePaths,
                    Data = filePaths,
                    Title = title
                };
            }

            return CreateActivationArguments(shareActivationArguments);
        }

        private static ActivationArguments CreateActivationArguments(ShareActivationArguments? shareActivationArguments)
        {
            ActivationArguments? activationArguments;
            if (shareActivationArguments is not null)
            {
                activationArguments = new()
                {
                    Kind = AppActivationKind.Share,
                    Data = shareActivationArguments
                };
            }
            else
            {
                activationArguments = new()
                {
                    Kind = AppActivationKind.Launch,
                };
            }

            return activationArguments;
        }

        //Reflection Microsoft.Maui.Storage.FileSystemUtils
        private static string? EnsurePhysicalPath(Android.Net.Uri uri)
        {
            Assembly assembly = typeof(FileSystem).Assembly;
            Type? fileSystemUtilsType = assembly.GetType("Microsoft.Maui.Storage.FileSystemUtils") ?? throw new Exception("FileSystemUtils type not found.");
            MethodInfo? ensurePhysicalPathMethod = fileSystemUtilsType.GetMethod("EnsurePhysicalPath", BindingFlags.Public | BindingFlags.Static) ?? throw new Exception("EnsurePhysicalPath method not found.");
            string? result = (string?)ensurePhysicalPathMethod.Invoke(null, [uri, true]);
            return result;
        }
    }
}
