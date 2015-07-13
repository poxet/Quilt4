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
        public bool IsConfirmed { get; set; }
        public IEnumerable<SearchResultRowModel> SearchResultRows { get; set; }
    }

    public class SearchResultRowModel
    {
        public string InitiativeName { get; set; }
        public string InitiativeUniqueIdentifier { get; set; }
        public string ApplicationName { get; set; }
        public string Version { get; set; }
        public string VersionUniqueIdentifier { get; set; }
        public IIssueType IssueType { get; set; }
        public IIssue Issue { get; set; }
        public string Environment { get; set; }
        //public List<EnvironmentViewModel> Environments { get; set; }
    }

}