using Maidan.Models;
using System.ComponentModel.DataAnnotations;

namespace Maidan.ViewModels
{
    public class ArticleViewModel
    {
        public ArticleViewModel()
        {
            Tags = new List<Tag>();
        }
        public int? Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Article title cannot be greater than 50 characters!")]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
