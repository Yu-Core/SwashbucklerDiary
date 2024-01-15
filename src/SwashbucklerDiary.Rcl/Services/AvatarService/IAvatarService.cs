namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAvatarService
    {
        Task<string> SetAvatarByPickPhoto();

        Task<string> SetAvatarByCapture();
    }
}
