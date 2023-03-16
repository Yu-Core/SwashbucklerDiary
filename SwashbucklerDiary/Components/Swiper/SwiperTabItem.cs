namespace SwashbucklerDiary.Components
{
    public class SwiperTabItem : ScrollContainer
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Class = "swiper-slide "+ Class;
        }
    }
}
