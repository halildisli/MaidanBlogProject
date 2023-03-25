using System.ComponentModel.DataAnnotations;

namespace Maidan.ViewModels
{
    public class SignUpViewModel
    {
        [MaxLength(30, ErrorMessage = "Username cannot be greater than 30 characters!")]
        [MinLength(5, ErrorMessage = "Username cannot be greater than 30 characters!")]
        [Required]
        public string Username { get; set; }
        //[RegularExpression(@"^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Invalid Email Address!")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password, ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        [Required]
        public string Password { get; set; }
        [DataType(DataType.Password, ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password fields do not match!")]
        [Required]
        public string PasswordConfirm { get; set; }
    }
}
