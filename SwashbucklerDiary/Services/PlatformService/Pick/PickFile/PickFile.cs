namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        private static Task<string?> PickFileAsync(PickOptions options, string suffixName)
        {
            string[] suffixNames = { suffixName };
            return PickFileAsync(options, suffixNames);
        }

        private async static Task<string?> PickFileAsync(PickOptions options, string[] suffixNames)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    foreach (var suffixName in suffixNames)
                    {
                        if (result.FileName.EndsWith(suffixName, StringComparison.OrdinalIgnoreCase))
                        {
                            return result.FullPath;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }
    }
}
