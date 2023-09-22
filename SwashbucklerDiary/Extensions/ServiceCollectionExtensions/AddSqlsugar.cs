using SqlSugar;
using SwashbucklerDiary.Config;
using SwashbucklerDiary.Models;
using System.Diagnostics;
using System.Reflection;

namespace SwashbucklerDiary.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlsugarConfig(this IServiceCollection services)
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
                    //Debug.WriteLine(sql);//输出sql
                    Debug.WriteLine(UtilMethods.GetSqlString(DbType.Sqlite, sql, pars));//输出sql
                };
            }
#endif
            );

            // 创建表
            Type[] types = {
                typeof(DiaryModel),
                typeof(TagModel),
                typeof(DiaryTagModel),
                typeof(UserAchievementModel),
                typeof(UserStateModel),
                typeof(LocationModel),
                typeof(LogModel),
                typeof(ResourceModel),
                typeof(DiaryResourceModel),
            };
            sqlSugar.CodeFirst.InitTables(types);

            services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
            return services;
        }
    }
}
