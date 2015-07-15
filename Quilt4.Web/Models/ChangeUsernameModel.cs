using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quilt4.Web.Models
{
    public class ChangeUsernameModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "New Username")]
        public string NewUsername { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}