using SqlSugar;
using System.Linq.Expressions;

namespace SwashbucklerDiary.IRepository
{
    //此处偷懒了，请勿学习，接口方法应自行定义
    public interface IBaseRepository<T> : ISimpleClient<T> where T : class, new()
    {
        Task<int> CountAsync();
        Task<List<T>> GetListTakeAsync(int count);
        Task<List<T>> GetListTakeAsync(int count, Expression<Func<T, bool>> func);
    }
}
