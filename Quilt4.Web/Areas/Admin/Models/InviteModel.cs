﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quilt4.Web.Areas.Admin.Models
{
    public class InviteModel
    {
        public string InviteEmail { get; set; }
        public Quilt4.Interface.IInitiative Initiative { get; set; }

    }
}