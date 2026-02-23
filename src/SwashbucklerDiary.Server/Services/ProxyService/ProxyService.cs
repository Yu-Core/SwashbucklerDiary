using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Server.Services
{
    public class ProxyService : IProxyService
    {
        public string ProxyUrl { get; } = "api/proxy?url=";
    }
}
