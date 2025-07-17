using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using SwashbucklerDiary.WebAssembly.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class AppLifecycle : Rcl.Essentials.AppLifecycle
    {
        private readonly Lazy<ValueTask<IJSInProcessObjectReference>> _module;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(),
                new ObjectToInferredTypesConverter()
            }
        };

        public AppLifecycle(IJSRuntime jSRuntime)
        {
            _module = new(() => ((IJSInProcessRuntime)jSRuntime).ImportJsModule("js/appLifecycle.js"));
        }

        [JSInvokable]
        public override void Resume() => base.Resume();

        [JSInvokable]
        public override void Stop() => base.Stop();

        [JSInvokable]
        public void SetActivationArgumentsFromJson(string jsonString)
        {
            try
            {
                ActivationArguments = JsonSerializer.Deserialize<ActivationArguments>(jsonString, jsonSerializerOptions);
            }
            catch (Exception)
            {
            }
        }

        public override async void QuitApp()
        {
            var module = await _module.Value;
            module.InvokeVoid("quit");
        }

        public async Task InitializedAsync()
        {
            var module = await _module.Value;
            var dotNetObject = DotNetObjectReference.Create(this);
            module.InvokeVoid("init", dotNetObject);
        }
    }
}
