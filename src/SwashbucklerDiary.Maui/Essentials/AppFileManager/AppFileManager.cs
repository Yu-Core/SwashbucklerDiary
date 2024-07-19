namespace SwashbucklerDiary.Maui.Essentials
{
    public class AppFileManager : Rcl.Essentials.AppFileManager
    {
        public override string AppDataDirectory => FileSystem.Current.AppDataDirectory;

        public override string CacheDirectory => FileSystem.Current.CacheDirectory;
    }
}
