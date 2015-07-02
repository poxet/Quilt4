using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class MachineDetailsModel
    {
        public IEnumerable<IApplication> Applications { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
        public IEnumerable<IUser> Users { get; set; }
        public IEnumerable<IMachine> Machines { get; set; }
        public string MachineName { get; set; }
    }
}