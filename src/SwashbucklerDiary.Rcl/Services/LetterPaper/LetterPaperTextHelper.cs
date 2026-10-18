using System.Text.RegularExpressions;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Services
{
    /// <summary>
    /// 古风信笺纸正文处理：Markdown 转纯文本、按模板容量截断、汉字日期转换
    /// </summary>
    public static partial class LetterPaperTextHelper
    {
        private const string ellipsis = "……";

        /// <summary>
        /// Markdown 转纯文本，剥离标题记号、强调记号、链接、图片、代码围栏、HTML 标签等
        /// </summary>
        public static string ToPlainText(string? markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                return string.Empty;
            }

            var text = markdown.Replace("\r\n", "\n").Replace("\r", "\n");

            // 图片仅保留替代文字
            text = ImagePattern().Replace(text, static m => m.Groups[1].Value);

            // 链接仅保留文字
            text = LinkPattern().Replace(text, static m => m.Groups[1].Value);

            // 自动链接 <https://...>
            text = AutoLinkPattern().Replace(text, static m => m.Groups[1].Value);

            // HTML 标签
            text = HtmlTagPattern().Replace(text, string.Empty);

            // 代码围栏记号（保留其中内容）
            text = FencePattern().Replace(text, string.Empty);

            // 行内代码记号
            text = InlineCodePattern().Replace(text, static m => m.Groups[1].Value);

            // 标题记号
            text = HeadingPattern().Replace(text, string.Empty);

            // 引用记号
            text = BlockQuotePattern().Replace(text, string.Empty);

            // 分隔线
            text = HorizontalRulePattern().Replace(text, string.Empty);

            // 列表记号（有序列表保留编号）
            text = UnorderedListPattern().Replace(text, string.Empty);

            // 表格：竖线替换为空格，分隔行丢弃
            text = TableSeparatorPattern().Replace(text, string.Empty);
            text = text.Replace("|", " ");

            // 强调记号
            text = BoldPattern().Replace(text, static m => m.Groups[1].Value);
            text = AsteriskItalicPattern().Replace(text, static m => m.Groups[1].Value);
            text = UnderscoreItalicPattern().Replace(text, static m => m.Groups[1].Value);
            text = StrikethroughPattern().Replace(text, static m => m.Groups[1].Value);
            text = HighlightPattern().Replace(text, static m => m.Groups[1].Value);

            // 转义还原
            text = EscapePattern().Replace(text, static m => m.Groups[1].Value);

            // 段间空行收敛为单次换行：竖排模板中空行会额外消耗一列，导致正文溢出笺框
            text = BlankLinesPattern().Replace(text, "\n");

            return text.Trim();
        }

        /// <summary>
        /// 按容量截断，超出部分以省略号收尾
        /// </summary>
        public static string Truncate(string? text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || maxLength >= int.MaxValue || text.Length <= maxLength)
            {
                return text ?? string.Empty;
            }

            return text[..maxLength] + ellipsis;
        }

        /// <summary>
        /// 按信笺可用行数（竖排为栏数，横排为横线行数）适配正文：
        /// 段落换行会新起一行，仅按字数截断无法保证不溢出笺框，
        /// 故按 ceil(段长/每行字数) 累计行数，超出时截断并追加省略号。
        /// </summary>
        public static string FitToPaper(string? text, LetterPaperTemplate template)
        {
            var (maxRows, charsPerRow) = template switch
            {
                LetterPaperTemplate.ZhuLan => (8, 25),
                LetterPaperTemplate.SuJian => (16, 22),
                LetterPaperTemplate.ZhuYing => (6, 25),
                _ => (int.MaxValue, int.MaxValue)
            };

            if (string.IsNullOrEmpty(text) || maxRows == int.MaxValue)
            {
                return text ?? string.Empty;
            }

            var sb = new System.Text.StringBuilder();
            var usedRows = 0;

            var paragraphs = text.Split('\n');
            for (var i = 0; i < paragraphs.Length; i++)
            {
                var remaining = maxRows - usedRows;
                if (remaining <= 0)
                {
                    sb.Append(ellipsis);
                    return sb.ToString();
                }

                var para = paragraphs[i];
                var units = para.Length == 0 ? 1 : (para.Length + charsPerRow - 1) / charsPerRow;

                if (i > 0)
                {
                    sb.Append('\n');
                }

                if (units <= remaining)
                {
                    sb.Append(para);
                    usedRows += units;
                }
                else
                {
                    var take = Math.Min(para.Length, remaining * charsPerRow);
                    if (take > 0)
                    {
                        sb.Append(para, 0, take);
                    }

                    sb.Append(ellipsis);
                    return sb.ToString();
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 转为汉字日期，如 2026-09-14 → "二〇二六年九月十四日"
        /// </summary>
        public static string ToHanziDate(DateTime date)
        {
            string year = string.Concat(date.Year.ToString().Select(d => HanziDigits[d - '0']));
            return $"{year}年{ToHanziNumber(date.Month)}月{ToHanziNumber(date.Day)}日";
        }

        private static readonly string[] HanziDigits = ["〇", "一", "二", "三", "四", "五", "六", "七", "八", "九"];

        private static readonly string[] HanziTeen = ["", "一", "二", "三", "四", "五", "六", "七", "八", "九"];

        private static string ToHanziNumber(int number)
        {
            if (number <= 10)
            {
                return number == 10 ? "十" : HanziTeen[number];
            }

            if (number < 20)
            {
                return "十" + HanziTeen[number % 10];
            }

            return HanziTeen[number / 10] + "十" + (number % 10 == 0 ? "" : HanziTeen[number % 10]);
        }

        [GeneratedRegex(@"!\[([^\]]*)\]\([^)]*\)")]
        private static partial Regex ImagePattern();

        [GeneratedRegex(@"\[([^\]]*)\]\([^)]*\)")]
        private static partial Regex LinkPattern();

        [GeneratedRegex(@"<((?:https?|ftp)://[^>\s]+)>")]
        private static partial Regex AutoLinkPattern();

        [GeneratedRegex(@"<\/?[a-zA-Z][^>]*>")]
        private static partial Regex HtmlTagPattern();

        [GeneratedRegex(@"^ {0,3}(```|~~~).*\n?", RegexOptions.Multiline)]
        private static partial Regex FencePattern();

        [GeneratedRegex(@"`([^`]*)`")]
        private static partial Regex InlineCodePattern();

        [GeneratedRegex(@"^\s{0,3}#{1,6}\s+", RegexOptions.Multiline)]
        private static partial Regex HeadingPattern();

        [GeneratedRegex(@"^\s{0,3}>\s?", RegexOptions.Multiline)]
        private static partial Regex BlockQuotePattern();

        [GeneratedRegex(@"^\s{0,3}((\* *){3,}|(- *){3,}|(_ *){3,})\s*$\n?", RegexOptions.Multiline)]
        private static partial Regex HorizontalRulePattern();

        [GeneratedRegex(@"^\s*[-*+]\s+", RegexOptions.Multiline)]
        private static partial Regex UnorderedListPattern();

        [GeneratedRegex(@"^\s*\|?[\s:|-]*-[\s:|-]*\|[\s:|-]*\|?\s*$\n?", RegexOptions.Multiline)]
        private static partial Regex TableSeparatorPattern();

        [GeneratedRegex(@"\*\*([^*]+)\*\*")]
        private static partial Regex BoldPattern();

        [GeneratedRegex(@"(?<!\*)\*([^*\n]+)\*(?!\*)")]
        private static partial Regex AsteriskItalicPattern();

        [GeneratedRegex(@"(?<![A-Za-z0-9_\\])_([^_\n]+)_(?![A-Za-z0-9_])")]
        private static partial Regex UnderscoreItalicPattern();

        [GeneratedRegex(@"~~([^~]+)~~")]
        private static partial Regex StrikethroughPattern();

        [GeneratedRegex(@"==([^=]+)==")]
        private static partial Regex HighlightPattern();

        [GeneratedRegex(@"\\([\\`*_{}\[\]()#+.!~-])")]
        private static partial Regex EscapePattern();

        [GeneratedRegex(@"\n{2,}")]
        private static partial Regex BlankLinesPattern();
    }
}
