using System.ComponentModel.DataAnnotations;

namespace Maidan.Models.ViewModels
{
    public class MyProfileViewModel
    {
        public string? Id { get; set; 
        }
        [MaxLength(30,ErrorMessage ="Username cannot be greater than 30 characters!")]
        [MinLength(5,ErrorMessage ="Username cannot be greater than 30 characters!")]
        [Required]
        public string? UserName { get; set; }
        [DataType(DataType.Password, ErrorMessage = "Your password should be minimum eight characters, at least one uppercase letter, one lowercase letter and one number!")]
        [Required]
        public string PasswordHash { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Required]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        [MaxLength(30,ErrorMessage ="First Name cannot be greater than 30 characters!")]
        public string? FirstName { get; set; }
        [MaxLength(30, ErrorMessage = "First Name cannot be greater than 30 characters!")]
        public string? LastName { get; set; }
        [MaxLength(300, ErrorMessage = "Your photo-name cannot be greater than 250 characters!")]
        public string? Photo { get; set; }
        [MaxLength(500, ErrorMessage = "Your bio cannot be greater than 500 characters!")]
        public string? Bio { get; set; }
        [MaxLength(100, ErrorMessage = "Your sub-domain cannot be greater than 100 characters!")]
        public string? SubDomain { get; set; }
        [MaxLength(100, ErrorMessage = "Your Github-address cannot be greater than 100 characters!")]
        public string? GithubUrl { get; set; }
        [MaxLength(100, ErrorMessage = "Your LinkedIn-address cannot be greater than 100 characters!")]
        public string? LinkedInUrl { get; set; }
        [MaxLength(100, ErrorMessage = "Your Twitter-address cannot be greater than 100 characters!")]
        public string? TwitterUrl { get; set; }
        [MaxLength(100, ErrorMessage = "Your Instagram-address cannot be greater than 100 characters!")]
        public string? InstagramUrl { get; set; }
        [MaxLength(100, ErrorMessage = "Your Website-address cannot be greater than 100 characters!")]
        public string? WebsiteUrl { get; set; }
    }
}
