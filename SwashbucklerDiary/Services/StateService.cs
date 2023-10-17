using BlazorComponent;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class StateService : IStateService
    {
        public event Func<Task>? FirstLauch;

        public async Task NotifyFirstLauchChanged()
        {
            if(FirstLauch == null)
            {
                return;
            }

            await FirstLauch.Invoke();
        }
    }
}
