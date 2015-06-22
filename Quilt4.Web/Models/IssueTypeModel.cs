using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class IssueTypeModel
    {
        public IIssueType IssueType { get; set; }
        public IEnumerable<ISession> Sessions { get; set; }
    }
}