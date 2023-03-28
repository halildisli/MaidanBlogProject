using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maidan.Models
{
    public class Comment
    {
        public Comment()
        {
            PublishDate = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        [ForeignKey("Article")]
        public int? ArticleId { get; set; }
        public string? AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }

        public Author Author { get; set; }
        public Article Article { get; set; }
    }
}
