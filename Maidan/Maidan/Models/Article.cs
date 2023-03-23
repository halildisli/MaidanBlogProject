using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maidan.Models
{
    public class Article
    {
        public Article()
        {
            Tags = new List<Tag>();
        }
        [Key]
        public int Id { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Privacy { get; set; }
        public int TotalReadTime { get; set; }


        public Author Author { get; set; }
        public List<Tag> Tags { get; set; }

    }
}
