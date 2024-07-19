namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class AppFileManager : Rcl.Essentials.AppFileManager
    {
        public override string AppDataDirectory => FileSystem.AppDataDirectory;

        public override string CacheDirectory => FileSystem.CacheDirectory;
    }
}
