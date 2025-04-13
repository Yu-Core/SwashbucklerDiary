using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class Setting
    {
        public string Language { get; set; } = "zh-CN";
        public bool Title { get; set; }
        public bool Markdown { get; set; } = true;
        public string NickName { get; set; } = string.Empty;
        public string Sign { get; set; } = string.Empty;
        public string Avatar { get; set; } = "_content/SwashbucklerDiary.Rcl/logo/logo.jpg";
        public bool HidePrivacyModeEntrance { get; set; }
        public string PrivacyModeEntrancePassword { get; set; } = string.Empty;
        public string PrivacyModeFunctionSearchKey { get; set; } = string.Empty;
        public bool PrivacyModeDark { get; set; } = true;
        public bool SetPrivacyDiary { get; set; } = true;
        public int Theme { get; set; } = 0;
        public bool FirstSetLanguage { get; set; }
        public bool FirstAgree { get; set; }
        public string WebDavConfig { get; set; } = string.Empty;
        public bool WelcomeText { get; set; }
        public bool IndexDate { get; set; }
        public bool DiaryCardIcon { get; set; } = true;
        public int AlertTimeout { get; set; } = 2000;
        public bool WebDAVCopyResources { get; set; }
        public string DiaryCardTimeFormat { get; set; } = "MM/dd";
        public bool AchievementsAlert { get; set; } = true;
        public string LANDeviceName { get; set; } = string.Empty;
        public int LANScanPort { get; set; } = 5299;
        public int LANTransmissionPort { get; set; } = 52099;
        public bool StatisticsCard { get; set; } = true;
        public int EditAutoSave { get; set; } = 5;
        public string DiarySort { get; set; } = string.Empty;
        public string TagSort { get; set; } = string.Empty;
        public string LocationSort { get; set; } = string.Empty;
        public bool UpdateNotPrompt { get; set; }
        public bool ImageLazy { get; set; } = true;
        public bool FirstLineIndent { get; set; }
        public bool CodeLineNumber { get; set; }
        public bool TaskListLineThrough { get; set; }
        public bool DiaryCardTags { get; set; }
        public bool DiaryIconText { get; set; }
        public bool OtherInfo { get; set; } = true;
        public bool AutoPlay { get; set; }
        public bool Outline { get; set; }
        public bool RigthOutline { get; set; }
        public bool DiaryCardLocation { get; set; }
        public string UrlScheme { get; set; } = SchemeConstants.SwashbucklerDiary;
        public int FirstDayOfWeek { get; set; } = 0;
        public int UseTemplateMethod { get; set; } = 0;
        public bool SelectTemplateWhenCreate { get; set; }
        public bool DiaryCardTemplate { get; set; } = true;
    }
}
