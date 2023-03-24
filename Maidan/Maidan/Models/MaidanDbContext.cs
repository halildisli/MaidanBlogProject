using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Maidan.Models
{
    public class MaidanDbContext : IdentityDbContext
    {
        public MaidanDbContext(DbContextOptions<MaidanDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<AuthorDetail> AuthorDetails { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
    
    
}
