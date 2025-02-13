using System.ComponentModel.DataAnnotations;

namespace SwashbucklerDiary.Rcl.Pages
{
    public class WebDavConfigForm
    {
        [Required(ErrorMessage = "Please input ServerAddress")]
        [Url(ErrorMessage = "Please input ServerAddress")]
        public string? ServerAddress { get; set; }

        [Required(ErrorMessage = "Please input Account")]
        public string? Account { get; set; }

        [Required(ErrorMessage = "Please input Password")]
        public string? Password { get; set; }
    }
}
