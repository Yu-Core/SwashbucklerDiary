using Microsoft.AspNetCore.Components.Web;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class DialogComponentBase : OpenCloseComponentBase
    {
        protected virtual async Task HandleOnCancel(MouseEventArgs _)
        {
            await InternalVisibleChanged(false);
        }
    }
}
