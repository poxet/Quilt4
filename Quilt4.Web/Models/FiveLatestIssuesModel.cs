using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quilt4.Interface;

namespace Quilt4.Web.Models
{
    public class FiveLatestIssuesModel : ItemsInIssueModel
    {
        public IEnumerable<ItemsInIssueModel> ItemsInIssueModel { get; set; }
        //public IEnumerable<IIssueType> FiveLatestIssueTypes { get; set; }
        //public IEnumerable<IIssue> FiveLatestIssues { get; set; }
    }

    public class ItemsInIssueModel
    {
        public string IssueTypeName { get; set; }
        public string IssueTypeLevel { get; set; }
        public DateTime IssueTime { get; set; }
        public string IssueVisible { get; set; }
        public int IssueTypeTicket { get; set; }
        public int IssueTicket { get; set; }
        public string InitativeId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
    }
}