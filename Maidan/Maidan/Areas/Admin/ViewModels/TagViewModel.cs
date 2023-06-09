﻿using Microsoft.Build.Framework;

namespace Maidan.Areas.Admin.ViewModels
{
    public class TagViewModel
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
