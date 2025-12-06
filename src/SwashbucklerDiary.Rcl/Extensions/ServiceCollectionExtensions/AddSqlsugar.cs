using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Diagnostics;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlSugarConfig(this IServiceCollection services, string connectionString, string privacyConnectionString)
        {
            ISettingService? settingService = null;
            IAppFileSystem? appFileSystem = null;

            var configureExternalServices = new ConfigureExternalServices
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
                    })
                    .ManyToMany(it => it.Diaries, typeof(DiaryResourceModel), nameof(ResourceModel.ResourceUri), nameof(DiaryResourceModel.DiaryId));

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
            };
            var configs = new List<ConnectionConfig>(){
                    new ConnectionConfig(){
                        ConfigId="0",
                        DbType = DbType.Sqlite,
                        ConnectionString = connectionString,
                        InitKeyType = InitKeyType.Attribute,
                        IsAutoCloseConnection = true,
                        ConfigureExternalServices =configureExternalServices,
                        MoreSettings=new ConnMoreSettings()
                        {
                            IsNoReadXmlDescription=true
                        }
                    },
                    new ConnectionConfig(){
                        ConfigId="1",
                        DbType = DbType.Sqlite,
                        ConnectionString = privacyConnectionString,
                        InitKeyType = InitKeyType.Attribute,
                        IsAutoCloseConnection = true,
                        ConfigureExternalServices =configureExternalServices,
                        MoreSettings=new ConnMoreSettings()
                        {
                            IsNoReadXmlDescription=true
                        }
                    }
                };
            SqlSugarScope sqlSugar = new(configs
            ,
            db =>
            {
                //单例参数配置，所有上下文生效
                foreach (var config in configs)
                {
#if DEBUG
                    db.GetConnection(config.ConfigId).Aop.OnLogExecuting = (sql, pars) =>
                    {
                        //Debug.WriteLine(sql);//输出sql
                        Debug.WriteLine(UtilMethods.GetSqlString(DbType.Sqlite, sql, pars));//输出sql
                    };
#endif
                    if (OperatingSystem.IsBrowser())
                    {
                        db.GetConnection(config.ConfigId).Aop.OnLogExecuted = async (sql, pars) =>
                        {
                            if (!sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase)
                                && appFileSystem is not null)
                            {
                                await Task.Delay(500);
                                await appFileSystem.SyncFS();
                            }
                        };
                    }
                }

                if (settingService is not null)
                {
                    bool privacyMode = settingService.GetTemp(it => it.PrivacyMode);
                    string configId = privacyMode ? "1" : "0";
                    string? currentConfigId = db.CurrentConnectionConfig.ConfigId.ToString();
                    if (configId != currentConfigId)
                    {
                        db.ChangeDatabase(configId);
                    }
                }
            }

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
            foreach (var config in configs)
            {
                sqlSugar.GetConnection(config.ConfigId).CodeFirst.InitTables(types);
            }

            services.AddSingleton<ISqlSugarClient>(sp =>
            {
                settingService = sp.GetService<ISettingService>();
                appFileSystem = sp.GetService<IAppFileSystem>();
                return sqlSugar;
            });//这边是SqlSugarScope用AddSingleton
            return services;
        }
    }
}
