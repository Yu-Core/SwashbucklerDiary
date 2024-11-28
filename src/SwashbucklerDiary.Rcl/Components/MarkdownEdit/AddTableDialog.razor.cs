using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class AddTableDialog
    {
        private int cols;

        private int rows;

        private readonly int defaultCols = 3;

        private readonly int defaultRows = 3;

        private readonly int minCols = 2;

        private readonly int minRows = 2;

        [Parameter]
        public EventCallback<string> OnOK { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        protected override async Task HandleOnCancel(MouseEventArgs _)
        {
            await Close();
            await base.HandleOnCancel(_);
        }

        private Task BeforeShowAddTable()
        {
            rows = defaultRows;
            cols = defaultCols;
            return Task.CompletedTask;
        }

        private async Task HandleOnOK()
        {
            await Close();
            await InternalVisibleChanged(false);
            var md = CreateTableMd(rows, cols);

            if (OnOK.HasDelegate)
            {
                await OnOK.InvokeAsync(md);
            }
        }

        private static string CreateTableMd(int rows, int cols)
        {
            // 生成表格
            StringBuilder markdownTable = new StringBuilder();

            // 添加表头
            markdownTable.Append($"| <wbr> ");
            for (int i = 1; i < cols; i++)
            {
                markdownTable.Append($"|  "); // 每个单元格的开头
            }
            markdownTable.Append("|\n");

            // 添加分隔行
            for (int i = 0; i < cols; i++)
            {
                markdownTable.Append("| - "); // 每个单元格的分隔符
            }
            markdownTable.Append('|');

            // 添加数据行
            for (int i = 1; i < rows; i++)
            {
                markdownTable.Append("\n|");
                for (int j = 0; j < cols; j++)
                {
                    markdownTable.Append("  |"); // 每个单元格的开头
                }
            }

            return $"\n{markdownTable}\n\n";
        }

        private async Task HandleOnOutsideClick()
        {
            await Close();
        }

        private async Task Close()
        {
            if (OnClose.HasDelegate)
            {
                await OnClose.InvokeAsync();
            }
        }
    }
}