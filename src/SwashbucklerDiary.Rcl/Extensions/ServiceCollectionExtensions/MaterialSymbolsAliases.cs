using Masa.Blazor;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public class MaterialSymbolsAliases : IconAliases
    {
        private static readonly string[] RtlIconFilters = ["left", "right", "arrow", "format", "before", "next", "sort", "toc", "list"];

        public MaterialSymbolsAliases()
        {
            Complete = "check";
            Cancel = "cancel-fill";
            Close = "close";
            Delete = "cancel-fill";
            Clear = "cancel-fill";
            Success = "check_circle";
            Info = "info";
            Warning = "priority_high";
            Error = "warning";
            Prev = "chevron_left";
            Next = "chevron_right";
            CheckboxOn = "check_box";
            CheckboxOff = "check_box_outline_blank";
            CheckboxIndeterminate = "indeterminate_check_box";
            Delimiter = "fiber_manual_record";
            Sort = "arrow_upward";
            Expand = "keyboard_arrow_down";
            Menu = "menu";
            Subgroup = "arrow_drop_down";
            Dropdown = "arrow_drop_down";
            RadioOn = "radio_button_checked";
            RadioOff = "radio_button_unchecked";
            Edit = "edit";
            RatingEmpty = "star_border";
            RatingFull = "star";
            RatingHalf = "star_half";
            Loading = "cached";
            First = "first_page";
            Last = "last_page";
            Unfold = "unfold_more";
            File = "attach_file";
            Plus = "add";
            Minus = "remove";
            Increase = "keyboard_arrow_up";
            Decrease = "keyboard_arrow_down";
            Copy = "content_copy";
            GoBack = "arrow_back";
            Search = "search";
            FilterOn = "keyboard_arrow_down";
            FilterOff = "keyboard_arrow_up";
            Retry = "refresh";
            CssFormatter = (icon) =>
            {
                List<string> classes = [];
                classes.Add("material-symbols-rounded");

                if (RtlIconFilters.Any(it => icon.Contains(it)))
                {
                    classes.Add("material-symbols-mirror");
                }

                string[] splits = icon.Split('-');
                if (splits.Length > 1 && splits[1] == "fill")
                {
                    classes.Add("material-symbols-fill");
                }

                return string.Join(" ", classes);
            };
            ContentFormatter = icon =>
            {
                string[] splits = icon.Split('-');
                return splits[0];
            };
        }
    }
}
