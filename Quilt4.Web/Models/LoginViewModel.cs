﻿using System.ComponentModel.DataAnnotations;

namespace Quilt4.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email/Username")]
        public string User { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}