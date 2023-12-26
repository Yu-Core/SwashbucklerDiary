namespace SwashbucklerDiary.Rcl.Services
{
    public interface IStorageSpace
    {
        /// <summary>
        /// 清除缓存
        /// </summary>
        void ClearCache();

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns></returns>
        string GetCacheSize();
    }
}
