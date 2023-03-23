using System.ComponentModel.DataAnnotations;

namespace Maidan.Models
{
    public class Author
    {
        public Author()
        {
            Articles = new List<Article>();
        }
        [Key]
        public int Id { get; set; }
        public string Authorname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public int Likes { get; set; }
        public decimal Tips { get; set; }
        public bool IsActive { get; set; }
        public bool IsPremium { get; set; }

        public List<Article> Articles { get; set; }


    }
}
