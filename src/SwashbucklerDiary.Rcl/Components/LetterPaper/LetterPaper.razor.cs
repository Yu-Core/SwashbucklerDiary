using System.Net;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class LetterPaper
    {
        [Parameter]
        public LetterPaperTemplate Template { get; set; } = LetterPaperTemplate.ZhuLan;

        /// <summary>
        /// 日记 Markdown 原文
        /// </summary>
        [Parameter]
        public string? Content { get; set; }

        /// <summary>
        /// 日记创建时间
        /// </summary>
        [Parameter]
        public DateTime Date { get; set; }

        /// <summary>
        /// 日记标题（有标题时以首栏/首行呈现，用拙楷稍大字号）
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        /// <summary>
        /// DiaryModel.Weather（i18n 键，如 sunny）
        /// </summary>
        [Parameter]
        public string? Weather { get; set; }

        /// <summary>
        /// 自定义章文（1–2 个汉字），为空时使用模板默认章文
        /// </summary>
        [Parameter]
        public string? SealText { get; set; }

        private string? TitleDisplay => string.IsNullOrWhiteSpace(Title) ? null : Title!.Trim();

        private bool HasTitle => TitleDisplay is not null;

        /// <summary>
        /// 默认卡片预览正文（Markdown 转纯文本后按卡片容量截断）
        /// </summary>
        private string? CardBodyDisplay
            => LetterPaperTextHelper.Truncate(LetterPaperTextHelper.ToPlainText(Content), 420);

        private string BodySource
        {
            get
            {
                var body = LetterPaperTextHelper.ToPlainText(Content);
                return TitleDisplay is null ? body : TitleDisplay + "\n" + body;
            }
        }

        private string FittedText => LetterPaperTextHelper.FitToPaper(BodySource, Template);

        // 标题行内嵌 span 样式化（拙楷），正文已转义；无标题时整段转义输出
        private MarkupString BodyDisplay
        {
            get
            {
                var fitted = FittedText;
                if (!HasTitle)
                {
                    return new MarkupString(WebUtility.HtmlEncode(fitted));
                }

                var idx = fitted.IndexOf('\n');
                var titleLine = idx < 0 ? fitted : fitted[..idx];
                var rest = idx < 0 ? string.Empty : "\n" + fitted[(idx + 1)..];
                var html = $"<span class=\"letterpaper-title\">{WebUtility.HtmlEncode(titleLine)}</span>{WebUtility.HtmlEncode(rest)}";
                return new MarkupString(html);
            }
        }

        private string HanziDate => LetterPaperTextHelper.ToHanziDate(Date);

        private bool ShowWeather => !string.IsNullOrEmpty(Weather);

        private string WeatherDisplay => Template switch
        {
            LetterPaperTemplate.ZhuLan => $"{I18n.T("Weather")}：{I18n.T(Weather!)}",
            LetterPaperTemplate.ZhuYing => $"· {I18n.T(Weather!)}",
            _ => I18n.T(Weather!)
        };

        /// <summary>
        /// 章面文字：02 为单字章取首字，其余为二字章
        /// </summary>
        private string SealDisplay
        {
            get
            {
                var text = string.IsNullOrWhiteSpace(SealText) ? DefaultSealText : SealText!.Trim();
                return Template == LetterPaperTemplate.SuJian ? text[..1] : text;
            }
        }

        private string DefaultSealText => Template switch
        {
            LetterPaperTemplate.SuJian => "记",
            LetterPaperTemplate.ZhuYing => "清欢",
            _ => "素心"
        };

        private string Svg => Template switch
        {
            // 三套模板的印章 <g>（含 <text>）已内联在 SVG 中，此处仅注入章字；
            // <text> 采用 text-anchor="middle"，一字/二字均自动居中
            LetterPaperTemplate.SuJian => string.Format(LetterPaperTemplateSvg.SuJian,
                WebUtility.HtmlEncode(SealDisplay)),
            LetterPaperTemplate.ZhuYing => string.Format(LetterPaperTemplateSvg.ZhuYing,
                WebUtility.HtmlEncode(SealDisplay)),
            _ => string.Format(LetterPaperTemplateSvg.ZhuLan,
                WebUtility.HtmlEncode(SealDisplay))
        };
    }
}
