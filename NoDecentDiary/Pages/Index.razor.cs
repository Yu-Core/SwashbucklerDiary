using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.Models;
using NoDecentDiary.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class Index
    {
        [Inject]
        public I18n? I18n { get; set; }
        [CascadingParameter]
        public Error? Error { get; set; }
        private List<DiaryModel> Diaries { get; set; } = new List<DiaryModel>();

        protected override Task OnInitializedAsync()
        {
            Diaries = new List<DiaryModel>()
            {
                new DiaryModel(){CreateTime=DateTime.Now, Title="伺机待发",Content="挖宝贷记卡悲剧啊开始播放借记卡"},
                new DiaryModel(){CreateTime=DateTime.Now, Title="dfkdsl",Content="挖宝贷记卡悲剧啊开始播放借记卡"},
                new DiaryModel(){CreateTime=DateTime.Now, Title="阿斯顿撒",Content="挖宝贷记卡悲剧啊开始播放借记卡"},
                new DiaryModel(){CreateTime=DateTime.Now, Title="我佛了",Content="挖宝贷记卡悲剧啊开始播放借记卡",Top=true}
            };
            //try
            //{
            //    throw new Exception();
            //}
            //catch (Exception ex)
            //{
            //    Error!.ProcessError(ex);
            //}
            return base.OnInitializedAsync();
        }
    }
}
