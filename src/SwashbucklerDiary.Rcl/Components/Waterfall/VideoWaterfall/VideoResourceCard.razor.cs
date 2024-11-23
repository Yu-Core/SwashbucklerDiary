using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VideoResourceCard : CardComponentBase<ResourceModel>
    {
        private string? src;

        private ResourceModel? previousValue;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (previousValue != Value)
            {
                previousValue = Value;

                // Display the first frame
                if (!string.IsNullOrEmpty(Value?.ResourceUri) && !Value.ResourceUri.Contains('#'))
                {
                    src = Value.ResourceUri + "#t=0.1";
                }
            }
        }
    }
}