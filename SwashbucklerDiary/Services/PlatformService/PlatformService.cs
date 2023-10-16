using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public partial class PlatformService : IPlatformService
    {
        private readonly IAlertService AlertService;

        private readonly II18nService I18n;

        public PlatformService(IAlertService alertService,II18nService i18n) 
        { 
            this.AlertService = alertService;
            this.I18n = i18n;
        }
    }
}
