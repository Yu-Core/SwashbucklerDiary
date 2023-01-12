using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Services
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

            var Tags = await Database
                .Table<TagModel>()
                .Where(it => tagIds.Contains(it.Id))
                .ToListAsync();

            return Tags;
        }
    }
}
