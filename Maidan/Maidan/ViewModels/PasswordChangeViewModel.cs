using System.ComponentModel.DataAnnotations;

namespace Maidan.ViewModels
{
    public class PasswordChangeViewModel
    {
        public string Id { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        [Required]
        public string? Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password fields do not match.")]
        public string? PasswordConfirm { get; set; }
    }
}
