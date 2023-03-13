namespace SwashbucklerDiary.IServices
{
    public interface ILocalImageService
    {
        /// <summary>
        /// 为本地图片地址添加标识，方便判断是本地图片
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<string> AddFlag(string uri);
        /// <summary>
        /// 为具有标识的本地图片地址创建url地址
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<string> ToUrl(string uri);
        /// <summary>
        /// 释放图片
        /// </summary>
        /// <param name="uri"></param>
        Task RevokeUrl(string uri);
    }
}
