﻿using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maidan.Models
{
    public class Article
    {
        public Article()
        {
            //Tags = new List<Tag>();
            //Comments = new List<Comment>();
            ReleaseDate = DateTime.Now;
            UpdateDate = ReleaseDate;
        }
        [Key]
        public int Id { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? Privacy { get; set; }
        public int? TotalReadTime { get; set; }
        public int NumberOfReads { get; set; }


        public virtual Author Author { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public virtual List<Comment> Comments { get; set; }

    }
}
