namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAvatarService
    {
        public string AvatarDirectoryPath { get; }

        Task<string> SetAvatarByPickPhotoAsync();

        Task<string> SetAvatarByCaptureAsync();
    }
}
