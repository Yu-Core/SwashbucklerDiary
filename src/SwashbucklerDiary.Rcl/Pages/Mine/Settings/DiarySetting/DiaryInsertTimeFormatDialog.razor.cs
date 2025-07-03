using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiaryInsertTimeFormatDialog : DialogComponentBase
    {
        private string? internalValue;

        private bool showMore;

        private CustomMTextField<string>? textField;

        private static readonly Dictionary<string, string> timeMap = new()
        {
            { "Year", "yyyy" },
            { "Month", "MM" },
            { "Day", "dd" },
            { "Hour", "HH" },
            { "Minute", "mm" },
            { "Second", "ss" },
            { "Week", "dddd" },
        };

        [Inject]
        private InputJSModule InputJSModule { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string?> ValueChanged { get; set; }

        [Parameter]
        public EventCallback OnReset { get; set; }

        string? CurrentTimeFormat
        {
            get
            {
                internalValue ??= string.Empty;
                if (IsValidDateTimeFormat(internalValue))
                {
                    return internalValue;
                }
                else
                {
                    return Value;
                }
            }
        }

        private async Task HandleOnEnter()
        {
            if (!Visible)
            {
                return;
            }

            await HandleOnOK();
        }

        private async Task HandleOnOK()
        {
            await InternalVisibleChanged(false);

            internalValue ??= string.Empty;
            if (internalValue == Value || !IsValidDateTimeFormat(internalValue))
            {
                return;
            }

            Value = internalValue;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
        }

        private void HandleOnBeforeShowContent()
        {
            internalValue = Value;
        }

        static bool IsValidDateTimeFormat(string format)
        {
            try
            {
                DateTime.Now.ToString(format);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private async Task AddTimeAsync(string value)
        {
            if (textField is null)
            {
                return;
            }

            internalValue = await InputJSModule.InsertText(textField.InputElement, value);
        }
    }
}