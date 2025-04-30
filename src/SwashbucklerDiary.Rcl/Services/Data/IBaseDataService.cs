using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IBaseDataService<TEntity> where TEntity : class, new()
    {
        Task<bool> AddAsync(TEntity entity);

        Task<bool> AddAsync(List<TEntity> entities);

        Task<int> AddReturnIdAsync(TEntity entity);

        Task<TEntity> AddReturnEntityAsync(TEntity entity);

        Task<bool> DeleteAsync();

        Task<bool> DeleteAsync(TEntity entity);

        Task<bool> DeleteAsync(Guid id);

        Task<bool> DeleteAsync(List<TEntity> entities);

        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>> columns);

        Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression);

        Task<List<TEntity>> QueryAsync();

        Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> FindAsync(Guid id);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression);

        Task<int> CountAsync();

        Task<int> CountAsync(Expression<Func<TEntity, bool>> expression);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression);
    }
}
