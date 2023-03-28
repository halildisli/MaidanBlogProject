using System.ComponentModel.DataAnnotations;

namespace Maidan.Areas.Admin.ViewModels
{
    public class AdminSignInViewModel
    {
        [Required(ErrorMessage ="Username is required!")]
        public string Username { get; set; }
        [Required(ErrorMessage ="Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
