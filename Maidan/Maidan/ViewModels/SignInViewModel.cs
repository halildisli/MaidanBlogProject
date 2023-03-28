

using System.ComponentModel.DataAnnotations;

namespace Maidan.ViewModels
{
    public class SignInViewModel
    {
        [Required(ErrorMessage ="Email is required!")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Invalid e-mail address!")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is required!")]
        [DataType(DataType.Password, ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
