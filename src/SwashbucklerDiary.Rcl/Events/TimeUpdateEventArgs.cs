namespace SwashbucklerDiary.Rcl.Events;

public class TimeUpdateEventArgs : EventArgs
{
    public double CurrentTime { get; set; }
}