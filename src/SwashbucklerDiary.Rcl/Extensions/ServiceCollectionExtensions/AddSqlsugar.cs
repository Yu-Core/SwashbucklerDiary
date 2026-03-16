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
        public static IServiceCollection AddSqlSugarConfig(this IServiceCollection services,
            string connectionString,
            string privacyConnectionString,
            string logConnectionString,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            services.Add<ISqlSugarClient>(sp =>
            {
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
                var logConfigureExternalServices = new ConfigureExternalServices
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

                        column.IfTable<LogModel>()
                        .UpdateProperty(it => it.Timestamp, it =>
                        {
                            it.DataType = "TEXT";
                        })
                        .UpdateProperty(it => it.LevelName, it =>
                        {
                            it.DataType = "TEXT";
                        })
                        .UpdateProperty(it => it.Message, it =>
                        {
                            it.DataType = "TEXT";
                            it.IsNullable = true;
                        })
                        .UpdateProperty(it => it.MessageTemplate, it =>
                        {
                            it.DataType = "TEXT";
                            it.IsNullable = true;
                        })
                        .UpdateProperty(it => it.Exception, it =>
                        {
                            it.DataType = "TEXT";
                            it.IsNullable = true;
                        })
                        .UpdateProperty(it => it.Properties, it =>
                        {
                            it.DataType = "TEXT";
                            it.IsNullable = true;
                        })
                        .UpdateProperty(it => it.SourceContext, it =>
                        {
                            it.DataType = "TEXT";
                            it.IsNullable = true;
                        })
                        .UpdateProperty(it => it.MachineName, it =>
                        {
                            it.DataType = "TEXT";
                            it.IsNullable = true;
                        })
                        .UpdateProperty(it => it.ThreadId, it =>
                        {
                            it.DataType = "INTEGER";
                            it.IsNullable = true;
                        });
                    }
                };
                var configs = new List<ConnectionConfig>(){
                    new ConnectionConfig(){
                        ConfigId = SQLiteConstants.MainDatabaseFilename,
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
                        ConfigId = SQLiteConstants.PrivacyDatabaseFilename,
                        DbType = DbType.Sqlite,
                        ConnectionString = privacyConnectionString,
                        InitKeyType = InitKeyType.Attribute,
                        IsAutoCloseConnection = true,
                        ConfigureExternalServices =configureExternalServices,
                        MoreSettings=new ConnMoreSettings()
                        {
                            IsNoReadXmlDescription=true
                        }
                    },
                    new ConnectionConfig(){
                        ConfigId = SQLiteConstants.LogDatabaseFilename,
                        DbType = DbType.Sqlite,
                        ConnectionString = logConnectionString,
                        InitKeyType = InitKeyType.Attribute,
                        IsAutoCloseConnection = true,
                        ConfigureExternalServices = logConfigureExternalServices,
                        MoreSettings=new ConnMoreSettings()
                        {
                            IsNoReadXmlDescription=true
                        }
                    }
                };
                var dataConfigs = configs.Where(c => c.ConfigId.ToString() != SQLiteConstants.LogDatabaseFilename).ToList();
                void ConfigAction(SqlSugarClient db)
                {
                    ISettingService? settingService = sp.GetService<ISettingService>();
                    IAppFileSystem? appFileSystem = sp.GetService<IAppFileSystem>();

                    //单例参数配置，所有上下文生效
                    foreach (var config in dataConfigs)
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

                    string? currentConfigId = db.CurrentConnectionConfig.ConfigId.ToString();
                    if (currentConfigId == SQLiteConstants.LogDatabaseFilename)
                    {
                        return;
                    }

                    if (settingService is not null)
                    {
                        bool privacyMode = settingService.GetTemp(it => it.PrivacyMode);
                        string configId = privacyMode ? SQLiteConstants.PrivacyDatabaseFilename : SQLiteConstants.MainDatabaseFilename;

                        if (configId != currentConfigId)
                        {
                            db.ChangeDatabase(configId);
                        }
                    }
                }

                ISqlSugarClient sqlSugar = serviceLifetime == ServiceLifetime.Singleton
                ? new SqlSugarScope(configs, ConfigAction)
                : new SqlSugarClient(configs, ConfigAction);

                // 创建表
                Type[] types = {
                    typeof(DiaryModel),
                    typeof(TagModel),
                    typeof(DiaryTagModel),
                    typeof(UserAchievementModel),
                    typeof(UserStateModel),
                    typeof(LocationModel),
                    typeof(ResourceModel),
                    typeof(DiaryResourceModel),
                };

                foreach (var config in dataConfigs)
                {
                    sqlSugar.AsTenant().GetConnection(config.ConfigId).CodeFirst.InitTables(types);
                }

                sqlSugar.AsTenant().GetConnection(SQLiteConstants.LogDatabaseFilename).CodeFirst.InitTables<LogModel>();
                return sqlSugar;
            }, serviceLifetime);//这边是SqlSugarScope用AddSingleton
            return services;
        }

        private static void Add<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory, ServiceLifetime lifetime) where TService : class
        {
            services.Add(new ServiceDescriptor(typeof(TService), implementationFactory, lifetime));
        }
    }
}
