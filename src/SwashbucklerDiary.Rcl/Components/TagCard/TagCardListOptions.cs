namespace SwashbucklerDiary.Rcl.Components
{
    public class TagCardListOptions
    {
        public event Action? DiariesChanged;

        public void NotifyDiariesChanged()
        {
            DiariesChanged?.Invoke();
        }
    }
}
