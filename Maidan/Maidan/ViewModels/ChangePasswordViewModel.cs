using System.ComponentModel.DataAnnotations;

namespace Maidan.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string? Id { get; set; }
        [DataType(DataType.Password, ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        [Required]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password, ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        [Required]
        public string NewPassword { get; set; }
        [DataType(DataType.Password, ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        [Compare("NewPassword", ErrorMessage = "The Password and Confirm Password fields do not match!")]
        [Required]
        public string PasswordConfirm { get; set; }
    }
}
