using BemIt;
using Masa.Blazor.Components.ItemGroup;
using Masa.Blazor.Mixins;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SwiperTabItem : MGroupable<MItemGroupBase>
    {
        public SwiperTabItem() : base(GroupType.ItemGroup)
        {
        }

        [CascadingParameter(Name = "SwiperValue")]
        public string? SwiperValue { get; set; }

        [Parameter] public RenderFragment<SwiperTabItemContext>? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Id ??= $"swiper-slide-{Guid.NewGuid():N}";
        }

        private static Block _block = new("swiper-slide");
        private ModifierBuilder _modifierBuilder = _block.CreateModifierBuilder();

        protected override IEnumerable<string?> BuildComponentClass()
        {
            yield return _modifierBuilder.AddClass(ComputedActiveClass, InternalIsActive).Build();
        }

        protected RenderFragment? ComputedChildContent
            => ChildContent?.Invoke(new SwiperTabItemContext(Id));
    }
}
