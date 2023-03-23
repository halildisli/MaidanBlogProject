namespace Maidan.Models
{
    public class Tag
    {
        public Tag()
        {
            Articles = new List<Article>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<Article> Articles { get; set; }
    }
}
