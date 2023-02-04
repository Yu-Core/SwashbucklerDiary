using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.IServices
{
    public interface ISystemService
    {
        Task SetClipboard(string text);
        Task ShareText(string title, string text);
        Task ShareFile(string title, string text);
        Task<string?> PickPhotoAsync();
        bool IsCaptureSupported();
        Task<string?> CapturePhotoAsync();
        bool CheckMail();
        Task SendEmail(List<string>? recipients);
        Task SendEmail(string? subject, string? body, List<string>? recipients);
        Task OpenBrowser(string url);
        Task FileCopy(string source, string target);
        Task<bool> CheckCameraPermission();
        Task<bool> CheckStorageWritePermission();
    }
}
