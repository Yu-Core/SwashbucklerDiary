namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAvatarService
    {
        Task<string> SetAvatar(string filePath);
    }
}
