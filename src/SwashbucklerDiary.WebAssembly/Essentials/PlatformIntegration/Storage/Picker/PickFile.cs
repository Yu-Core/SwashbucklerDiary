using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        private async Task<string?> PickFileAsync(string accept, string? suffix = null)
        {
            try
            {
                var module = await Module;
                var result = await module.InvokeAsync<string>("pickFileAsync", accept, suffix);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(PickFileAsync)} wrong");
            }

            return null;
        }
    }
}
