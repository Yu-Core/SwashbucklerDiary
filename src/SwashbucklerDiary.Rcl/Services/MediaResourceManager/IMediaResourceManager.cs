using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IMediaResourceManager
    {
        string? MarkdownLinkBase { get; }

        string AssetsDirectoryPath { get; }

        Dictionary<MediaResource, string> MediaResourceDirectoryPaths { get; }

        /// <summary>
        /// 添加图片
        /// </summary>
        /// <returns></returns>
        Task<ResourceModel?> AddImageAsync();

        Task<IEnumerable<ResourceModel>?> AddMultipleImageAsync();

        /// <summary>
        /// 添加音频
        /// </summary>
        /// <returns></returns>
        Task<ResourceModel?> AddAudioAsync();

        Task<IEnumerable<ResourceModel>?> AddMultipleAudioAsync();

        /// <summary>
        /// 添加视频
        /// </summary>
        /// <returns></returns>
        Task<ResourceModel?> AddVideoAsync();

        Task<IEnumerable<ResourceModel>?> AddMultipleVideoAsync();

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

        Task<AudioFileInfo> GetAudioFileInfo(string uri);

        Task<IEnumerable<ResourceModel>?> AddMediaFilesAsync(IEnumerable<string?> filePaths);

        Task<string?> CreateMediaFilesInsertContentAsync(List<string?> filePaths);

        string? CreateMediaFilesInsertContent(IEnumerable<ResourceModel>? resources);

        string RelativeUrlToFilePath(string urlRelativePath);

        string FilePathToRelativeUrl(string filePath);

        MediaResourcePath? ToMediaResourcePath(NavigationManager navigationManager, string? url);

        Task<string?> ToFilePathAsync(MediaResourcePath? mediaResourcePath);

        string? ReplaceDisplayedUrlToRelativeUrl(string? content);
    }
}
