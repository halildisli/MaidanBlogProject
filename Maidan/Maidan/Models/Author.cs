using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maidan.Models
{
    public class Author:IdentityUser
    {
        public virtual List<Tag> Tags { get; set; }
        public Author()
        {
            //Articles = new List<Article>();
            //if (Tags==null)
            //{
            //    Tags = new List<Tag>();
            //}
            MembershipDate = DateTime.Now;
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
        public DateTime? MembershipDate { get; set; }
        public virtual List<Article> Articles { get; set; }
        
    }
}
