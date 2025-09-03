using SqlSugar;
using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
#pragma warning disable CS0472 // 由于此类型的值永不等于 "null"，该表达式的结果始终相同
    public class DiaryService : BaseDataService<DiaryModel>, IDiaryService
    {
        private readonly IDiaryRepository _iDiaryRepository;

        public DiaryService(IDiaryRepository iDiaryRepository)
        {
            base._iBaseRepository = iDiaryRepository;
            _iDiaryRepository = iDiaryRepository;
        }

        public override Task<List<DiaryModel>> QueryAsync()
        {
            return _iBaseRepository.GetListAsync();
        }

        public override Task<List<DiaryModel>> QueryAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            return _iBaseRepository.GetListAsync(expression);
        }

        public Task<List<TagModel>> GetTagsAsync(Guid id)
        {
            return _iDiaryRepository.GetTagsAsync(id);
        }

        public Task<bool> UpdateIncludesAsync(DiaryModel model)
        {
            return _iDiaryRepository.UpdateIncludesAsync(model);
        }

        public Task<bool> UpdateIncludesAsync(List<DiaryModel> models)
        {
            return _iDiaryRepository.UpdateIncludesAsync(models);
        }

        public Task<bool> UpdateTagsAsync(DiaryModel model)
        {
            return _iDiaryRepository.UpdateTagsAsync(model);
        }

        public Task<bool> ImportAsync(List<DiaryModel> diaries)
        {
            return _iDiaryRepository.ImportAsync(diaries);
        }

        public Task<bool> MovePrivacyDiaryAsync(DiaryModel diary, bool toPrivacyMode)
        {
            return _iDiaryRepository.MovePrivacyDiaryAsync(diary, toPrivacyMode);
        }

        public Task<bool> MovePrivacyDiariesAsync()
        {
            return _iDiaryRepository.MovePrivacyDiariesAsync();
        }

        public Task<List<DiaryModel>> QueryDiariesAsync()
        {
            return QueryAsync(it => it.Template == null || it.Template == false);
        }

        public Task<List<DiaryModel>> QueryDiariesAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            var expable = Expressionable.Create<DiaryModel>();
            expable.And(expression);
            expable.And(it => it.Template == null || it.Template == false);
            return QueryAsync(expable.ToExpression());
        }

        public Task<List<DiaryModel>> QueryTemplatesAsync()
        {
            return QueryAsync(it => it.Template);
        }

        public Task<List<DiaryModel>> QueryTemplatesAsync(Expression<Func<DiaryModel, bool>> expression)
        {
            var expable = Expressionable.Create<DiaryModel>();
            expable.And(expression);
            expable.And(it => it.Template == null || it.Template == false);
            return QueryAsync(expable.ToExpression());
        }
    }
}
