namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAvatarService
    {
        Task<string> SetAvatarByPickPhotoAsync();

        Task<string> SetAvatarByCaptureAsync();
    }
}
