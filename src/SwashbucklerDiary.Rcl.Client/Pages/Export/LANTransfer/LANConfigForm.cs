using SwashbucklerDiary.Shared;
using System.ComponentModel.DataAnnotations;

namespace SwashbucklerDiary.Rcl.Pages
{
    public class LANConfigForm
    {
        [Required(ErrorMessage = "Please enter device name")]
        public string? DeviceName { get; set; }

        [Required(ErrorMessage = "Please enter scan port")]
        [NotEqual(nameof(TransmissionPort), ErrorMessage = "Scan port and transmission port cannot be the same")]
        public int ScanPort { get; set; }

        [Required(ErrorMessage = "Please enter transmission port")]
        [NotEqual(nameof(ScanPort), ErrorMessage = "Scan port and transmission port cannot be the same")]
        public int TransmissionPort { get; set; }
    }
}
