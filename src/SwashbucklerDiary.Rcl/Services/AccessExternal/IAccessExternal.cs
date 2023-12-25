namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAccessExternal
    {
        /// <summary>
        /// 打开应用商店App详情页
        /// </summary>
        /// <returns></returns>
        Task<bool> OpenAppStoreAppDetails();
        
        /// <summary>
        /// 加入qq群
        /// </summary>
        /// <returns></returns>
        Task<bool> JoinQQGroup();
    }
}
