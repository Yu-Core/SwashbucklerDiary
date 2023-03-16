using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.IServices
{
    public interface INavigateService
    {
        event Func<string> CurrentUrl;
        /// <summary>
        /// 存储想要返回时触发的方法
        /// </summary>
        event Action Action;
        NavigationManager Navigation { get;protected set; }
        /// <summary>
        /// 存储URL历史记录
        /// </summary>
        List<string> HistoryUrl { get; protected set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="navigation"></param>
        void Initialize(NavigationManager navigation);
        /// <summary>
        /// 导航
        /// </summary>
        /// <param name="url"></param>
        void NavigateTo(string url);
        /// <summary>
        /// 返回上一页
        /// </summary>
        void NavigateToBack();
        /// <summary>
        /// 返回键处理
        /// </summary>
        /// <returns>true已处理事件，false为无事件可处理(便于做进一步处理，例如退出应用)</returns>
        bool OnBackButtonPressed();
    }
}
