using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Essentials
{
    public static class AppActionsHelper
    {
        public static async void HandleAppActions(AppAction action)
        {
            if (action.Id is null) return;

            var activationArguments = await ConvertActivationArguments(action.Id);
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

        public static async Task<ActivationArguments?> ConvertActivationArguments(string id)
        {
            var actions = await AppActions.Current.GetAsync();
            var appAction = actions.FirstOrDefault(a => a.Id == id);
            if (appAction is null) return null;

            return new ActivationArguments()
            {
                Kind = AppActivationKind.Scheme,
                Data = $"{SchemeConstants.SwashbucklerDiary}://{id}"
            };
        }

        public static void SetAppActions(II18nService i18n)
        {
            if (AppActions.Current.IsSupported)
            {
                AppActions.Current.SetAsync([
                   new("write",i18n.T("Share.Add"),icon:"shortcut_pencil"),
                   new("search",i18n.T("Share.Search"),icon:"shortcut_magnify"),
                ]);
            }
        }
    }
}
