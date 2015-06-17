using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quilt4.Web.Areas.Admin.Models
{
    public class InviteModel
    {
        [Display(Name = "Email")]
        public string InviteEmail { get; set; }

        public Quilt4.Interface.IInitiative Initiative { get; set; }

        [Display(Name = "Additional message")]
        public string Message { get; set; }

    }
}