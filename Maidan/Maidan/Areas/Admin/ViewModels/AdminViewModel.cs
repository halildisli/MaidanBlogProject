using System.ComponentModel.DataAnnotations;

namespace Maidan.Areas.Admin.ViewModels
{
    public class AdminViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="Username is required!")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Email is required!")]
        [EmailAddress(ErrorMessage ="Invalid e-mail format. Please check this field.")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is required!")]
        [DataType(DataType.Password, ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        public string Password { get; set; }
    }
}
