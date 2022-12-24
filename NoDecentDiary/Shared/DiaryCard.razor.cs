using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class DiaryCard
    {
        [Parameter]
        public DateTime Date { get; set; }
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public string? Text { get; set; }
    }
}
