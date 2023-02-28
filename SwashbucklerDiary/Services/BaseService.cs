using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
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

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> func)
        {
            return _iBaseRepository.CountAsync(func);
        }

        public virtual Task<bool> DeleteAsync(TEntity entity)
        {
            return _iBaseRepository.DeleteAsync(entity);
        }

        public virtual Task<bool> DeleteAsync(Guid id)
        {
            return _iBaseRepository.DeleteByIdAsync(id);
        }

        public virtual Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> func)
        {
            return _iBaseRepository.DeleteAsync(func);
        }

        public virtual Task<TEntity> FindAsync(Guid id)
        {
            return _iBaseRepository.GetByIdAsync(id);
        }

        public virtual Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> func)
        {
            return _iBaseRepository.GetFirstAsync(func);
        }

        public virtual Task<List<TEntity>> QueryAsync()
        {
            return _iBaseRepository.GetListAsync();
        }

        public virtual Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func)
        {
            return _iBaseRepository.GetListAsync(func);
        }

        public virtual Task<bool> UpdateAsync(TEntity entity)
        {
            return _iBaseRepository.UpdateAsync(entity);
        }

        public Task<List<TEntity>> QueryTakeAsync(int count)
        {
            return _iBaseRepository.GetListTakeAsync(count);
        }

        public Task<List<TEntity>> QueryTakeAsync(int count, Expression<Func<TEntity, bool>> func)
        {
            return _iBaseRepository.GetListTakeAsync(count,func);
        }
    }
}
