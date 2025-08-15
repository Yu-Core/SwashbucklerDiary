using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TransferDialog : DialogComponentBase
    {
        private double percentage;

        private long transferredSize;

        private long totalSize;

        private double _maxPercentage = 100;

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public double MaxPercentage
        {
            get => _maxPercentage;
            set => _maxPercentage = Math.Clamp(value, 0, double.MaxValue);
        }

        [Parameter]
        public Func<long, string>? TotalSizeText { get; set; }

        [Parameter]
        public Func<long, string>? TransferredSizeText { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        public void SetProgress(long transferredSize, long totalSize)
        {
            InvokeAsync(() =>
            {
                this.percentage = totalSize == 0 ? 0 : (double)transferredSize / totalSize * 100;
                this.transferredSize = transferredSize;
                this.totalSize = totalSize;
                StateHasChanged();
            });
        }

        private double Percentage => Math.Min(percentage, MaxPercentage);

        private long TransferredSize => totalSize <= 0 ? 0 : Math.Min(transferredSize, (long)(Percentage / 100 * totalSize));

        private string InternalTransferredSizeText
        {
            get
            {
                if (TransferredSizeText is not null)
                {
                    return TransferredSizeText.Invoke(TransferredSize);
                }

                return $"{TransferredSize / 1024} KB";
            }
        }

        private string InternalTotalSizeText
        {
            get
            {
                if (TotalSizeText is not null)
                {
                    return TotalSizeText.Invoke(totalSize);
                }

                return $"{totalSize / 1024} KB";
            }
        }
    }
}
