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

        public virtual async Task<int> GetLastInsertRowId()
        {
            await Init();
            return (int)SQLite3.LastInsertRowid(Database!.GetConnection().Handle);
        }

        public async Task<bool> AddAsync(List<TEntity> entities)
        {
            await Init();
            if(entities.Count == 0)
            {
                return false;
            }
            var oldCount = await Database!.Table<TEntity>().CountAsync();
            await Database!.RunInTransactionAsync(tran => {
                foreach (var item in entities)
                {
                    tran.Insert(item);
                }
                tran.Commit();
            });
            var newCount = await Database!.Table<TEntity>().CountAsync();
            return newCount > oldCount;
        }

        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> func)
        {
            await Init();
            var entities = await Database!.Table<TEntity>().Where(func).ToListAsync();
            if(entities.Count == 0)
            {
                return false;
            }

            await Database.RunInTransactionAsync(tran => {
                foreach (var item in entities)
                {
                    tran.Delete(item);
                }
                tran.Commit();
            });
            var entityCount = await Database!.Table<TEntity>().Where(func).CountAsync();
            return entityCount < entities.Count;
        }
    }
}
