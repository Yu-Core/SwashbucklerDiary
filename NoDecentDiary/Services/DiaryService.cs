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
        public override async Task<List<DiaryModel>> QueryAsync()
        {
            var diaries = await base.QueryAsync();
            return diaries.OrderByDescending(it => it.Id).ToList();
        }
        public override async Task<List<DiaryModel>> QueryAsync(Expression<Func<DiaryModel, bool>> func)
        {
            var diaries = await base.QueryAsync(func);
            return diaries.OrderByDescending(it => it.Id).ToList();
        }
        public async Task<List<DiaryModel>> GetDiariesByTagAsync(int tagId)
        {
            await Init();
            await Database!.CreateTableAsync<TagModel>();
            await Database.CreateTableAsync<DiaryTagModel>();

            var diaryTag = await Database
                .Table<DiaryTagModel>()
                .Where(it => it.TagId == tagId)
                .ToListAsync();

            var diaryIds = diaryTag.Select(it => it.DiaryId).ToList();

            var Diaries = await QueryAsync(it => diaryIds.Contains(it.Id));

            return Diaries;
        }
    }
}
