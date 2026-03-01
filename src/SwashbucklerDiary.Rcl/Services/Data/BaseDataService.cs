using SwashbucklerDiary.Rcl.Repository;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public class BaseDataService<TEntity> : IBaseDataService<TEntity> where TEntity : class, new()
    {
        protected IBaseRepository<TEntity> _iBaseRepository = default!;

        public virtual Task<bool> AddAsync(TEntity entity)
        {
            return _iBaseRepository.InsertAsync(entity);
        }

        public virtual Task<bool> AddAsync(List<TEntity> entities)
        {
            return _iBaseRepository.InsertRangeAsync(entities);
        }

        public virtual Task<int> AddReturnIdAsync(TEntity entity)
        {
            return _iBaseRepository.InsertReturnIdentityAsync(entity);
        }

        public virtual Task<TEntity> AddReturnEntityAsync(TEntity entity)
        {
            return _iBaseRepository.InsertReturnEntityAsync(entity);
        }

        public virtual Task<int> CountAsync()
        {
            return _iBaseRepository.CountAsync();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _iBaseRepository.CountAsync(expression);
        }

        public virtual Task<bool> DeleteAsync()
        {
            return _iBaseRepository.DeleteAsync();
        }

        public virtual Task<bool> DeleteAsync(TEntity entity)
        {
            return _iBaseRepository.DeleteAsync(entity);
        }

        public virtual Task<bool> DeleteAsync(Guid id)
        {
            return _iBaseRepository.DeleteByIdAsync(id);
        }

        public virtual Task<bool> DeleteAsync(List<TEntity> entities)
        {
            return _iBaseRepository.DeleteAsync(entities);
        }

        public virtual Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _iBaseRepository.DeleteAsync(expression);
        }

        public virtual Task<TEntity> FindAsync(Guid id)
        {
            return _iBaseRepository.GetByIdAsync(id);
        }

        public virtual Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _iBaseRepository.GetFirstAsync(expression);
        }

        public virtual Task<List<TEntity>> QueryAsync()
        {
            return _iBaseRepository.GetListAsync();
        }

        public virtual Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _iBaseRepository.GetListAsync(expression);
        }

        public virtual Task<bool> UpdateAsync(TEntity entity)
        {
            return _iBaseRepository.UpdateAsync(entity);
        }

        public Task<bool> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>> columns)
        {
            return _iBaseRepository.UpdateAsync(entity, columns);
        }

        public Task<bool> UpdateAsync(List<TEntity> updateObjs, Expression<Func<TEntity, object>> columns)
        {
            return _iBaseRepository.UpdateAsync(updateObjs, columns);
        }

        public Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression)
        {
            return _iBaseRepository.UpdateAsync(columns, whereExpression);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression)
        {
            return _iBaseRepository.IsAnyAsync(expression);
        }
    }
}
