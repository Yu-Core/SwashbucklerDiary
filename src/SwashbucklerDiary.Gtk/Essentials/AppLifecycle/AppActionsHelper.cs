using GLib.Internal;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public static class AppActionsHelper
    {
        public static string[] AppActionNames { get; } = ["search", "write"];

        public static void HandleAppActions(string appActionName)
        {
            var activationArguments = ConvertActivationArguments(appActionName);
            if (activationArguments is null) return;

            if (AppActivation.OnActivated is null)
            {
                AppActivation.Arguments = activationArguments;
            }
            else
            {
                AppActivation.OnActivated.Invoke(activationArguments);
            }
        }

        private static ActivationArguments? ConvertActivationArguments(string appActionName)
        {
            var appAction = AppActionNames.FirstOrDefault(it => it == appActionName);
            if (appAction is null) return null;

            return new ActivationArguments()
            {
                Kind = AppActivationKind.Scheme,
                Data = $"{SchemeConstants.SwashbucklerDiary}://{appActionName}"
            };
        }

        public static void AddMainOptionEntries(global::Gtk.Application application)
        {
            // Don`t change order
            List<GLib.OptionEntry> entries = [];

            foreach (var name in AppActionNames)
            {
                entries.Add(new()
                {
                    LongName = name,
                    ShortName = 0,
                    Flags = 0,
                    Arg = GLib.OptionArg.None,
                    Description = null,
                    ArgDescription = null
                });
            }

            entries.Add(new(){
                    LongName=GLib.Constants.OPTION_REMAINING,
                    ShortName=0,
                    Flags=0,
                    Arg = GLib.OptionArg.StringArray,
                    Description = null,
                    ArgDescription = null
                 });
            entries.Add(new(){});

            // Gio.Application Unencapsulated AddMainOptionEntries
            Gio.Internal.Application.AddMainOptionEntries(application.Handle.DangerousGetHandle(), OptionEntryArrayOwnedHandle.Create(entries.ToArray()));
        }
    }
}
