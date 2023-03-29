namespace Maidan.Models
{
    public class Tag
    {
        public Tag()
        {
            //Articles = new List<Article>();
            //Authors = new List<Author>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public virtual List<Article> Articles { get; set; }
        public virtual List<Author> Authors { get; set; }
    }
}
