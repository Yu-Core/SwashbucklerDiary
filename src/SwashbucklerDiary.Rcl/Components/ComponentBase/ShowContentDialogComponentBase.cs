namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class ShowContentDialogComponentBase : DialogComponentBase
    {
        protected MDialogExtension mDialogExtension = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                mDialogExtension.BeforeShowContent = BeforeShowContent;
                mDialogExtension.AfterShowContent = AfterShowContent;
            }
        }

        protected virtual Task AfterShowContent(bool isLazyContent)
        {
            return Task.CompletedTask;
        }

        protected virtual Task BeforeShowContent()
        {
            return Task.CompletedTask;
        }
    }
}
