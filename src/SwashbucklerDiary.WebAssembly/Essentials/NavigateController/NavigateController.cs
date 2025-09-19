using SwashbucklerDiary.Rcl.Essentials;
using System.Reflection;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        public NavigateController(IAppLifecycle appLifecycle) : base(appLifecycle)
        {
        }

        protected override IEnumerable<Assembly> Assemblies => App.Assemblies;
    }
}
