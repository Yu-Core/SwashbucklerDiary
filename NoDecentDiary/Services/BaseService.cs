using NoDecentDiary.IServices;
using NoDecentDiary.StaticData;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public virtual async Task<int> AddAsync(TEntity entity)
        {
            await Init();
            return await Database!.InsertAsync(entity);
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            await Init();
            return await Database!.DeleteAsync(entity);
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            await Init();
            return await Database!.Table<TEntity>().ToListAsync();
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            await Init();
            return await Database!.UpdateAsync(entity);
        }
    }
}
