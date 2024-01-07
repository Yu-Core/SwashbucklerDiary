namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class ShowContentDialogComponentBase : DialogComponentBase
    {
        protected MDialogExtension mDialog = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                mDialog.BeforeShowContent = BeforeShowContent;
                mDialog.AfterShowContent = AfterShowContent;
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
