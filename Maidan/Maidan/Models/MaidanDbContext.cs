﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Maidan.ViewModels;
using Maidan.Areas.Admin.ViewModels;

namespace Maidan.Models
{
    public class MaidanDbContext : IdentityDbContext<IdentityUser>
    {
        public MaidanDbContext(DbContextOptions<MaidanDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //DummyRolesAdd(builder);
        }
        //private static void DummyRolesAdd(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<IdentityRole>().HasData(
        //        new IdentityRole() { Name = "admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1" },
        //        new IdentityRole() { Name = "member", NormalizedName = "MEMBER", ConcurrencyStamp = "10" }
        //        );
        //}
    }



}
