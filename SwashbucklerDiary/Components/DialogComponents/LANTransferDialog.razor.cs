using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class LANTransferDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public int Ps { get; set; }

        [Parameter]
        public long TotalBytesToReceive { get; set; }

        [Parameter]
        public long BytesReceived { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }
    }
}
