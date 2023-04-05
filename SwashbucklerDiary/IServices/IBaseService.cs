using System.Linq.Expressions;

namespace SwashbucklerDiary.IServices
{
    public interface IBaseService<TEntity> where TEntity : class, new()
    {
        Task<bool> AddAsync(TEntity entity);
        Task<bool> AddAsync(List<TEntity> entities);
        Task<int> AddReturnIdAsync(TEntity entity);
        Task<TEntity> AddReturnEntityAsync(TEntity entity);
        Task<bool> DeleteAsync();
        Task<bool> DeleteAsync(TEntity entity);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> func);
        Task<bool> UpdateAsync(TEntity entity);
        Task<List<TEntity>> QueryAsync();
        Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func);
        Task<List<TEntity>> QueryTakeAsync(int count);
        Task<List<TEntity>> QueryTakeAsync(int count, Expression<Func<TEntity, bool>> func);
        Task<TEntity> FindAsync(Guid id);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> func);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> func);
    }
}
