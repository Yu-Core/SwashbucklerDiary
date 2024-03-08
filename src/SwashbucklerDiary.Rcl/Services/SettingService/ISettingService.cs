using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ISettingService : IPreferences
    {
        Task InitializeAsync();

        T Get<T>(Setting setting);

        T Get<T>(Setting setting, T defaultValue);

        Task Set<T>(Setting setting, T value);

        Task Remove(Setting setting);
    }
}
