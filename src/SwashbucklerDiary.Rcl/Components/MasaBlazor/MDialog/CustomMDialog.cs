using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Components
{
    //The significance of extension is to enable dialog or similar components to support back button
    public class CustomMDialog : MDialog
    {
        [Inject]
        protected INavigateController NavigateController { get; set; } = default!;

        [Parameter]
        public bool MyValue
        {
            get => base.Value;
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<bool> MyValueChanged
        {
            get => base.ValueChanged;
            set => base.ValueChanged = value;
        }

        [Parameter]
        public EventCallback<bool> OnAfterShowContent { get; set; }

        [Parameter]
        public EventCallback OnBeforeShowContent { get; set; }

        [Parameter]
        public bool OnMousedownPreventDefault { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AfterShowContent = HandleOnAfterShowContent;
            BeforeShowContent = HandleOnBeforeShowContent;
        }

        protected override ValueTask DisposeAsyncCore()
        {
            if (Value)
            {
                NavigateController.RemoveHistoryAction(Close);
            }

            return base.DisposeAsyncCore();
        }

        protected override async Task OnActiveUpdatedAsync(bool firstActive, bool active)
        {
            if (firstActive && OnMousedownPreventDefault)
            {
                ElementReference? overlayRef = overlayRefProperty.GetValue(this) as ElementReference?;

                List<object> args = [ContentRef, overlayRef];
                await Js.InvokeVoidAsync("preventDefaultOnmousedown", args);
            }

            await base.OnActiveUpdatedAsync(firstActive, active);
        }

        private static readonly PropertyInfo overlayRefProperty = typeof(MDialog).GetProperty("OverlayRef", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?? throw new Exception("Property OverlayRef does not exist");

        private void SetValue(bool value)
        {
            if (base.Value == value)
            {
                return;
            }

            base.Value = value;
            if (value)
            {
                NavigateController.AddHistoryAction(Close, isDialog: true);
            }
            else
            {
                NavigateController.RemoveHistoryAction(Close);
            }
        }

        private void Close()
        {
            InvokeAsync(async () =>
            {
                MyValue = false;
                if (MyValueChanged.HasDelegate)
                {
                    await MyValueChanged.InvokeAsync(false);
                }
            });
        }

        private async Task HandleOnAfterShowContent(bool value)
        {
            if (OnAfterShowContent.HasDelegate)
            {
                await OnAfterShowContent.InvokeAsync(value);
            }
        }

        private async Task HandleOnBeforeShowContent()
        {
            if (OnBeforeShowContent.HasDelegate)
            {
                await OnBeforeShowContent.InvokeAsync();
            }
        }
    }
}
