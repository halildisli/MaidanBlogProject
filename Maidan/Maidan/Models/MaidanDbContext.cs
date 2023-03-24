﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Maidan.Models
{
    public class MaidanDbContext : IdentityDbContext<IdentityUser>
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

            DummyRolesAdd(builder);
        }
        private static void DummyRolesAdd(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1" },
                new IdentityRole() { Name = "user", NormalizedName = "USER", ConcurrencyStamp = "10" }
                );
        }
    }
    
    
    
}
