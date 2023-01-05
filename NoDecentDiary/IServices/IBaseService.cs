using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.IServices
{
    public interface IBaseService<TEntity> where TEntity : class, new()
    {
        Task<bool> AddAsync(TEntity entity);
        Task<bool> AddAsync(List<TEntity> entities);
        Task<bool> DeleteAsync(TEntity entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> func);
        Task<bool> UpdateAsync(TEntity entity);
        Task<List<TEntity>> QueryAsync();
        Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func);
        Task<TEntity> FindAsync(int id);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> func);
        Task<int> GetLastInsertRowId();
    }
}
