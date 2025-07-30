namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> SaveFileAsync(string sourceFilePath)
        {
            var fileName = Path.GetFileName(sourceFilePath);
            return SaveFileAsync(fileName, sourceFilePath);
        }

        public async Task<bool> SaveFileAsync(string name, string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                return false;
            }

            await _jsModule.SaveFileAsync(name, sourceFilePath);
            return true;
        }
    }
}
