using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class AppLifecycle : Rcl.Essentials.AppLifecycle, IDisposable
    {
        private readonly AppLifecycleJSModule _jSModule;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(),
                new ObjectToInferredTypesConverter()
            }
        };
        public AppLifecycle(AppLifecycleJSModule jSModule)
        {
            _jSModule = jSModule;
            _jSModule.OnResumed += Resume;
            _jSModule.OnStopped += Stop;
        }

        public override async void QuitApp()
        {
            await _jSModule.Quit();
        }

        public async Task InitializedAsync()
        {
            string? jsonString = await _jSModule.Init();

            if (!string.IsNullOrEmpty(jsonString))
            {
                try
                {
                    ActivationArguments = JsonSerializer.Deserialize<ActivationArguments>(jsonString, jsonSerializerOptions);
                }
                catch (Exception)
                {
                }
            }
        }

        public void Dispose()
        {
            _jSModule.OnResumed -= Resume;
            _jSModule.OnStopped -= Stop;
            GC.SuppressFinalize(this);
        }
    }
}
