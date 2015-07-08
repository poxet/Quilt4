using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class SearchModel
    {
        public string SearchText { get; set; }
        public IEnumerable<IIssueType> IssueTypeResults { get; set; }
        public bool IsConfirmed { get; set; }
    }
}