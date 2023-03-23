using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Maidan.Models
{
    public class MaidanDbContext : DbContext
    {
        public MaidanDbContext(DbContextOptions<MaidanDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<AuthorDetail> AuthorDetails { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder
        //        .Entity<Comment>()
        //        .HasOne(e => e.Author)
        //        .WithOne(e => e.Articles)
        //        .OnDelete(DeleteBehavior.ClientCascade);
        //}
    }
    
}
