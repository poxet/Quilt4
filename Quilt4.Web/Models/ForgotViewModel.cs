﻿using System.ComponentModel.DataAnnotations;

namespace Quilt4.Web.Models
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}