﻿using System.ComponentModel.DataAnnotations;

namespace Maidan.Models.ViewModels
{
    public class ArticleViewModel
    {
        public int? Id { get; set; }
        [Required]
        [StringLength(50,ErrorMessage ="Article title cannot be greater than 50 characters!")]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
