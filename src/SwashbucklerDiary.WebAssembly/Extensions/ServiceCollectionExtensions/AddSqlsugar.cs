using SqlSugar;
using SwashbucklerDiary.Shared;
using System.Diagnostics;
using System.Reflection;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlsugarConfig(this IServiceCollection services)
        {
            SqlSugarScope sqlSugar = new(new ConnectionConfig()
            {
                DbType = DbType.Sqlite,
                ConnectionString = $"Data Source={SQLiteConstants.DatabasePath}",
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    //注意:  这儿AOP设置不能少
                    EntityService = (p, column) =>
                    {
                        if (column.PropertyName.ToLower() == "id") //是id的设为主键
                        {
                            column.IsPrimarykey = true;
                            if (column.PropertyInfo.PropertyType == typeof(int)) //是id并且是int的是自增
                            {
                                column.IsIdentity = true;
                            }
                        }

                        column.IfTable<DiaryModel>()
                        .ManyToMany(it => it.Tags, typeof(DiaryTagModel), nameof(DiaryTagModel.DiaryId), nameof(DiaryTagModel.TagId))
                        .ManyToMany(it => it.Resources, typeof(DiaryResourceModel), nameof(DiaryResourceModel.DiaryId), nameof(DiaryResourceModel.ResourceUri));

                        column.IfTable<LogModel>()
                        .UpdateProperty(it => it.Level, it =>
                        {
                            it.DataType = "varchar(10)";
                        });

                        column.IfTable<ResourceModel>()
                        .UpdateProperty(it => it.ResourceUri, it =>
                        {
                            it.IsPrimarykey = true;
                        });

                        column.IfTable<TagModel>()
                        .ManyToMany(it => it.Diaries, typeof(DiaryTagModel), nameof(DiaryTagModel.TagId), nameof(DiaryTagModel.DiaryId));

                        /***高版C#写法***/
                        //支持string?和string  
                        if (column.IsPrimarykey == false && new NullabilityInfoContext()
                         .Create(p).WriteState is NullabilityState.Nullable)
                        {
                            column.IsNullable = true;
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

            //创建表
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
