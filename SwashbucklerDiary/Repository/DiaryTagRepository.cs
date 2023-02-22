using SqlSugar;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Repository
{
    public class DiaryTagRepository : BaseRepository<DiaryTagModel>, IDiaryTagRepository
    {
        public DiaryTagRepository(ISqlSugarClient context) : base(context)
        {
        }
    }
}
