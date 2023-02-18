using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class TagService : BaseService<TagModel>, ITagService
    {
        public async Task<List<TagModel>> GetDiaryTagsAsync(int diaryId)
        {
            await Init();
            await Database!.CreateTableAsync<DiaryModel>();
            await Database.CreateTableAsync<DiaryTagModel>();

            var diaryTags = await Database
                .Table<DiaryTagModel>()
                .Where(it => it.DiaryId == diaryId)
                .ToListAsync();

            var tagIds = diaryTags.Select(it => it.TagId).ToList();

            var Tags = await QueryAsync(it => tagIds.Contains(it.Id));

            return Tags;
        }
    }
}
