using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class Index
    {
        private List<DiaryModel> Diaries { get; set; } = new List<DiaryModel>();

        protected override Task OnInitializedAsync()
        {
            Diaries = new List<DiaryModel>()
            {
                new DiaryModel(){CreateTime=DateTime.Now, Title="伺机待发",Content="挖宝贷记卡悲剧啊开始播放借记卡"}
            };
            return base.OnInitializedAsync();
        }
    }
}
