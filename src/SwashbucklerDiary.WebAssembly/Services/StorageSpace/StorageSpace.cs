using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class StorageSpace : Rcl.Services.StorageSpace
    {
        public StorageSpace(IAppFileManager appFileManager) : base(appFileManager) { }

        public override void ClearCache()
            => _appFileManager.ClearFolder(FileSystem.CacheDirectory);

        public override string GetCacheSize()
        {
            long fileSizeInBytes = _appFileManager.GetFolderSize(FileSystem.CacheDirectory);
            return ConvertBytesToReadable(fileSizeInBytes);
        }
    }
}
