using System.ComponentModel.DataAnnotations;

namespace SwashbucklerDiary.Models
{
    public class WebDavConfigForm
    {
        [Required(ErrorMessage = "Please input ServerAddress")]
        public string? ServerAddress { get; set; }
        [Required(ErrorMessage = "Please input Account")]
        public string? Account { get; set; }
        [Required(ErrorMessage = "Please input Password")]
        public string? Password { get; set; }
    }
}
