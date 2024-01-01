using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class AppLifecycle : IAppLifecycle
    {
        public event Action? Resumed;

        public event Action? Stopped;

        private readonly IJSInProcessRuntime _jS; 

        public AppLifecycle(IJSRuntime jSRuntime) 
        {
            _jS = (IJSInProcessRuntime)jSRuntime;
        }

        public void OnResume() => Resumed?.Invoke();

        public void OnStop() => Stopped?.Invoke();

        public void QuitApp()
        {
            _jS.InvokeVoid("quit");
        }
    }
}
