using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.IServices
{
    public interface INavigateService
    {
        List<string> RootPaths { get; protected set; }

        /// <summary>
        /// 存储想要返回时触发的方法
        /// </summary>
        event Action Action;

        event Func<PushEventArgs, Task>? BeforePush;

        event Func<PopEventArgs, Task>? BeforePop;

        event Func<PopEventArgs, Task>? BeforePopToRoot;

        event Action<PushEventArgs>? Pushed;

        event Action<PopEventArgs>? Poped;

        event Action<PopEventArgs>? PopedToRoot;
        NavigationManager Navigation { get; protected set; }

        /// <summary>
        /// 存储URL历史记录
        /// </summary>
        List<string> HistoryURLs { get; protected set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="navigation"></param>
        void Initialize(NavigationManager navigation);

        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="url"></param>
        Task PushAsync(string url, bool isCachePrevious = true);

        /// <summary>
        /// 返回上一页
        /// </summary>
        Task PopAsync();

        /// <summary>
        /// 返回到根页面
        /// </summary>
        /// <returns></returns>
        Task PopToRootAsync(string url);

        /// <summary>
        /// 返回键处理
        /// </summary>
        /// <returns>true已处理事件，false为无事件可处理(便于做进一步处理，例如退出应用)</returns>
        bool OnBackButtonPressed();
    }
}
