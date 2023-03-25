using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maidan.Models
{
    public class Author:IdentityUser
    {
        public Author()
        {
            Articles = new List<Article>();
            Tags = new List<Tag>();
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Photo { get; set; }
        public string? Bio { get; set; }
        public string? SubDomain { get; set; }
        public string? GithubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public List<Article> Articles { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
