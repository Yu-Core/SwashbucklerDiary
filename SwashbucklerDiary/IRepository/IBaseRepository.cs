using SqlSugar;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IRepository
{
    public interface IBaseRepository<T> : ISimpleClient<T> where T : class, new()
    {
        Task<int> CountAsync();
        Task<List<T>> GetListTakeAsync(int count);
        Task<List<T>> GetListTakeAsync(int count, Expression<Func<T, bool>> func);
    }
}
