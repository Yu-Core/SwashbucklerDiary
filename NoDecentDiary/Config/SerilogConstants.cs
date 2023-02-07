namespace NoDecentDiary.Config
{
    public static class SerilogConstants
    {
        readonly static string folderName = "Logs";
        readonly static string fileName = "serilog.log";
        readonly static string appPath = FileSystem.Current.AppDataDirectory; // Documents folder
        public readonly static string folderPath = Path.Combine(appPath, folderName);
        public readonly static string filePath = Path.Combine(appPath, folderName, fileName);

    }
}
