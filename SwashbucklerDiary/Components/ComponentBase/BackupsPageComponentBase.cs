using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public class BackupsPageComponentBase : ImportantComponentBase
    {
        [Inject]
        protected IAppDataService AppDataService { get; set; } = default!;

        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;
    }
}
