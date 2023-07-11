using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class LANTransfer : PageComponentBase
    {
        private List<DynamicListItem> DynamicLists = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadView();
        }

        private void LoadView()
        {
            DynamicLists = new()
            {
                new(this,"Export.Send","mdi-send-outline",()=>To("lanSender")),
                new(this,"Export.Receive","mdi-printer-pos-outline",()=>To("lanReceiver")),
            };
        }
    }
}
