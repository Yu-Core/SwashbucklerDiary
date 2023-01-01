using NoDecentDiary.IServices;
using NoDecentDiary.StaticData;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Services
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, new()
    {
        private SQLiteAsyncConnection? Database;

        private async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(SQLiteConstants.DatabasePath, SQLiteConstants.Flags);
            await Database.CreateTableAsync<TEntity>();
        }

        public virtual async Task<List<TEntity>> QueryAsync()
        {
            await Init();
            return await Database!.Table<TEntity>().ToListAsync();
        }

        public virtual async Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func)
        {
            await Init();
            return await Database!.Table<TEntity>().Where(func).ToListAsync();
        }

        public virtual async Task<TEntity> FindAsync(int id)
        {
            await Init();
            return await Database!.FindAsync<TEntity>(id);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> func)
        {
            await Init();
            return await Database!.FindAsync(func);
        }

        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            await Init();
            return await Database!.InsertAsync(entity) > 0;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
        {
            await Init();
            return await Database!.DeleteAsync(entity) > 0;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            await Init();
            return await Database!.DeleteAsync<TEntity>(id) > 0;
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            await Init();
            return await Database!.UpdateAsync(entity) > 0;
        }
    }
}
