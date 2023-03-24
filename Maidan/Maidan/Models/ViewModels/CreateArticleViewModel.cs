using System.ComponentModel.DataAnnotations;

namespace Maidan.Models.ViewModels
{
    public class CreateArticleViewModel
    {
        [Required]
        [StringLength(50,ErrorMessage ="Article title cannot be greater than 50 characters!")]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string? Image { get; set; }
    }
}
