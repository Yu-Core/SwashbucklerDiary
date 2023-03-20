namespace SwashbucklerDiary.IServices
{
    //https://github.com/dotnet/maui/issues/2907
    public interface ILocalImageService
    {
        // 为本地路径的图片创建blob,并返回blob的url,若不是本地路径会直接返回
        Task<string> ToUrl(string path);
        // 调用js，释放图片的blob
        Task RevokeUrl(string path);
    }
}
