using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Services
{
    public class DiaryService : BaseService<DiaryModel>, IDiaryService
    {
        public async Task<List<TagModel>> GetTagsAsync(int id)
        {
            await Init();
            await Database!.CreateTableAsync<DiaryTagModel>();
            await Database.CreateTableAsync<TagModel>();
            var DiaryTags = await Database.Table<DiaryTagModel>().Where(it=>it.DiaryId == id).ToListAsync();
            var Tags = new List<TagModel>();
            foreach (var item in DiaryTags)
            {
                var tag = await Database.FindAsync<TagModel>(item.TagId);
                Tags.Add(tag);
            }
            return Tags;
        }
    }
}
