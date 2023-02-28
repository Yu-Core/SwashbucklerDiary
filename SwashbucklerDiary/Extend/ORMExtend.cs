using SqlSugar;
using SwashbucklerDiary.Config;
using System.Reflection;

namespace SwashbucklerDiary.Extend
{
    public static class ORMExtend
    {
        public static void AddSqlsugar(this IServiceCollection services)
        {
            SqlSugarScope sqlSugar = new (new ConnectionConfig()
            {
                DbType = DbType.Sqlite,
                ConnectionString = SQLiteConstants.ConnectionString,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    //注意:  这儿AOP设置不能少
                    EntityService = (c, p) =>
                    {
                        /***高版C#写法***/
                        //支持string?和string  
                        if (p.IsPrimarykey == false && new NullabilityInfoContext()
                         .Create(c).WriteState is NullabilityState.Nullable)
                        {
                            p.IsNullable = true;
                        }
                    }
                }
            }
#if DEBUG
            ,
            db =>
            {
                //单例参数配置，所有上下文生效
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql);//输出sql
                };
            }
#endif
            );

            services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        }
    }
}
