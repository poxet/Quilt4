﻿using System.ComponentModel.DataAnnotations;
using Quilt4.Interface;

namespace Quilt4.Web.Areas.Admin.Models
{
    public class InviteModel
    {
        [Display(Name = "Email")]
        public string InviteEmail { get; set; }

        public IInitiative Initiative { get; set; }

        [Display(Name = "Additional message")]
        public string Message { get; set; }

        //public string RoleName { get; set; }

        public bool IsAdministrator { get; set; }
    }
}