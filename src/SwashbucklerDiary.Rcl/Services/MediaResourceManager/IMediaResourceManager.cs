using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IMediaResourceManager
    {
        Dictionary<MediaResource, string> MediaResourceFolders { get; }

        /// <summary>
        /// 添加图片
        /// </summary>
        /// <returns></returns>
        Task<string?> AddImageAsync();

        /// <summary>
        /// 添加音频
        /// </summary>
        /// <returns></returns>
        Task<string?> AddAudioAsync();

        /// <summary>
        /// 添加视频
        /// </summary>
        /// <returns></returns>
        Task<string?> AddVideoAsync();

        /// <summary>
        /// 从日记中提取出所使用的媒体资源
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        List<ResourceModel> GetDiaryResources(string content);

        /// <summary>
        /// 通过uri获得媒体资源的类型
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        MediaResource GetResourceKind(string uri);

        Task<string?> CreateMediaResourceFileAsync(string targetDirectoryPath, string? sourceFilePath);

        /// <summary>
        /// 分享图片
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<bool> ShareImageAsync(string title, string url);

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<bool> SaveImageAsync(string url);

        Task<AudioFileInfo> GetAudioFileInfo(string uri);

        Task<List<ResourceModel>> ReceiveShareFilesAsync(List<string> filePaths);
    }
}
