using SqlSugar;

namespace SwashbucklerDiary.IRepository
{
    public interface IBaseRepository<T> : ISimpleClient<T> where T : class, new()
    {
        Task<int> CountAsync();
        Task<List<T>> GetListTakeAsync(int count);
    }
}
