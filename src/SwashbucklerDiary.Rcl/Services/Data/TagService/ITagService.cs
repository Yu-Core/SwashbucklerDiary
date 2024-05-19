using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ITagService : IBaseDataService<TagModel>
    {
        Task<TagModel> FindIncludesAsync(Guid id);

        Task<Dictionary<Guid, int>> TagsDiaryCount();
    }
}
