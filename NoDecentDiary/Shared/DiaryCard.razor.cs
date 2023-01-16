using Microsoft.AspNetCore.Components;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Reflection.Expressions.IntelligentGeneration.Extensions;

namespace NoDecentDiary.Shared
{
    public partial class DiaryCard
    {
        [Parameter]
        [EditorRequired]
        public DiaryModel? Value { get; set; }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public EventCallback OnTopping { get; set; }
        [Parameter]
        public EventCallback OnDelete { get; set; }
        [Parameter]
        public EventCallback OnCopy { get; set; }
        [Parameter]
        public EventCallback OnTag { get; set; }
        [Parameter]
        public EventCallback OnClick { get; set; }

        private DateTime Date => Value!.CreateTime;
        private string? Title => Value!.Title;
        private string? Text => Value!.Content;
        private bool Top => Value!.Top;
        private bool showMenu = false;

        
    }
}
