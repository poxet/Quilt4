using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quilt4.Web.Models
{
    public class IssueModel
    {
        public IEnumerable<string> ExceptionTypeName { get; set; }
        public string InitiativeId { get; set; }
        public string ApplicationName { get; set; }
        public string Version { get; set; }
    }
}