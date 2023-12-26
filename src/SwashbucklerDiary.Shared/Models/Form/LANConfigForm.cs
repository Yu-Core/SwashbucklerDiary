using System.ComponentModel.DataAnnotations;

namespace SwashbucklerDiary.Shared
{
    public class LANConfigForm
    {
        [Required(ErrorMessage = "Please input DeviceName")]
        public string? DeviceName { get; set; }

        [Required(ErrorMessage = "Please input ScanPort")]
        [NotEqual(nameof(TransmissionPort), ErrorMessage = "ScanPort and TransmissionPort cannot be the same")]
        public int ScanPort { get; set; }

        [Required(ErrorMessage = "Please input TransmissionPort")]
        [NotEqual(nameof(ScanPort), ErrorMessage = "ScanPort and TransmissionPort cannot be the same")]
        public int TransmissionPort { get; set; }
    }
}
