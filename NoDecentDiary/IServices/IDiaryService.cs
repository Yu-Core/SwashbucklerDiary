using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.IServices
{
    public interface IDiaryService : IBaseService<DiaryModel>
    {
        Task<List<TagModel>> GetTagsAsync(int id);
    }
}
