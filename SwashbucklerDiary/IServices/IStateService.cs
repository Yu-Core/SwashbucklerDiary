using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IStateService
    {
        event Func<Task>? FirstLauch;
        Task NotifyFirstLauchChanged();
    }
}
