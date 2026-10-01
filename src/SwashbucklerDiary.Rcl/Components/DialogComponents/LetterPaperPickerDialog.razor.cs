using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class LetterPaperPickerDialog : DialogComponentBase
    {
        [Parameter]
        public LetterPaperTemplate Value { get; set; } = LetterPaperTemplate.DefaultCard;

        [Parameter]
        public EventCallback<LetterPaperTemplate> ValueChanged { get; set; }

        /// <summary>
        /// 日记 Markdown 原文（预览用）
        /// </summary>
        [Parameter]
        public string? Content { get; set; }

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public DateTime Date { get; set; }

        [Parameter]
        public string? Weather { get; set; }

        [Parameter]
        public string? SealText { get; set; }

        [Parameter]
        public EventCallback OnShare { get; set; }

        [Parameter]
        public EventCallback OnDownload { get; set; }

        private void HandleValueChanged(LetterPaperTemplate value)
        {
            Value = value;

            if (ValueChanged.HasDelegate)
            {
                ValueChanged.InvokeAsync(value);
            }
        }

        private async Task HandleShare()
        {
            await InternalVisibleChanged(false);
            await OnShare.InvokeAsync();
        }

        private async Task HandleDownload()
        {
            await InternalVisibleChanged(false);
            await OnDownload.InvokeAsync();
        }
    }
}
