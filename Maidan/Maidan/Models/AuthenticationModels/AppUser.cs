using System.ComponentModel.DataAnnotations;

namespace Maidan.Models.ViewModels
{
    public class AppUser
    {
        [MinLength(8,ErrorMessage ="Username should be at least 8 characters!")]
        public string Username { get; set; }
        [Display(Name ="Email Address :")]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [MinLength(8,ErrorMessage ="Password should be at least 8 characters!")]
        public string Password { get; set; }
    }
}
