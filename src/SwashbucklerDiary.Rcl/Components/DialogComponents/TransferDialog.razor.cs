using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TransferDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public int Ps { get; set; }

        [Parameter]
        public long TotalBytes { get; set; }

        [Parameter]
        public long Bytes { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }
    }
}
