using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Services
{
    public class DiaryTagService : BaseService<DiaryTagModel>, IDiaryTagService
    {
        public async Task<bool> AddTagsAsync(int diaryId, List<TagModel> tagModels)
        {
            var DiaryTags = new List<DiaryTagModel>();
            foreach (var item in tagModels)
            {
                var diaryTag = new DiaryTagModel()
                {
                    DiaryId = diaryId,
                    TagId = item.Id
                };
                DiaryTags.Add(diaryTag);
            }
            return await AddAsync(DiaryTags);
        }
    }
}
