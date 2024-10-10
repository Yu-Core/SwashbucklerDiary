namespace SwashbucklerDiary.Maui.Essentials
{
    public class AppFileSystem : Rcl.Essentials.AppFileSystem
    {
        public override string AppDataDirectory => FileSystem.Current.AppDataDirectory;

        public override string CacheDirectory => FileSystem.Current.CacheDirectory;
    }
}
