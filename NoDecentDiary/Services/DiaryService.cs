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

            var Diaries = await Database
                .Table<DiaryModel>()
                .Where(it => diaryIds.Contains(it.Id))
                .ToListAsync();

            return Diaries;
        }
    }
}
