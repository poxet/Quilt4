using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;
using Quilt4.Web.Models;
using Tharga.Quilt4Net;

namespace Quilt4.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IApplicationVersionBusiness _applicationVersionBusiness;
        private readonly IAccountRepository _accountRepository;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IIssueBusiness _issueBusiness;

        public HomeController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness, IAccountRepository accountRepository, ISessionBusiness sessionBusiness, IIssueBusiness issueBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
            _accountRepository = accountRepository;
            _sessionBusiness = sessionBusiness;
            _issueBusiness = issueBusiness;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated) { 
                var userEmail = User.Identity.Name;
                var issueTypes = _issueBusiness.GetIssueTypesForDeveloper(userEmail).ToArray();
                var fiveIssues = _issueBusiness.GetFiveLatestErrorIssusByIssueTypes(issueTypes).ToArray();
                var selectedIssueTypes = fiveIssues.SelectMany(x => issueTypes.Where(y => y.Issues.Contains(x))).Distinct().ToArray();
                var model = new FiveLatestIssuesModel();

                IApplicationVersion version;
                IApplication application;

                var list = new List<ItemsInIssueModel>();

                foreach (var issueType in selectedIssueTypes)
                {
                    foreach (var issue in fiveIssues)
                    {
                        if (issueType.Issues.Any(x => x.Id == issue.Id))
                        {
                            version = _applicationVersionBusiness.GetApplicationVersionByIssue(issue.Id);
                            application = _initiativeBusiness.GetApplicationByVersion(version.Id);
                            var itemInModel = new ItemsInIssueModel
                            {
                                IssueTypeName = issueType.ExceptionTypeName,
                                IssueTypeLevel = issueType.IssueLevel.ToString(),
                                IssueTime = issue.ServerTime,
                                IssueVisible = issue.VisibleToUser.ToString(),
                                IssueTypeTicket = issueType.Ticket,
                                IssueTicket = issue.Ticket,
                                ApplicationVersion = version.Id,
                                ApplicationName = application.Name,
                                InitativeId = _initiativeBusiness.GetInitiativeByApplication(application.Id).Id.ToString(),
                                
                            };
                            list.Add(itemInModel);
                        }
                    }
                }

                model.ItemsInIssueModel = list.OrderByDescending(x => x.IssueTime).ToArray();
                return View(model);

            }

            return View(new FiveLatestIssuesModel());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult System()
        {
            ViewBag.Version = Assembly.GetAssembly(typeof(HomeController)).GetName().Version.ToString();
            ViewBag.Environment = Information.Environment;
            ViewBag.Quilt4SessionStarter = Tharga.Quilt4Net.Session.ClientStartTime.ToLocalTime();
            ViewBag.Quilt4RegisteredOnServer = Tharga.Quilt4Net.Session.RegisteredOnServer;
            ViewBag.Quilt4HasClientToken = !string.IsNullOrEmpty(Configuration.ClientToken);
            ViewBag.Quilt4IsEnabled = Configuration.Enabled;

            return View();
        }

        [Authorize]
        public ActionResult Search()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult SearchResults(string searchText)
        {
            var model = new SearchModel()
            {
                IsConfirmed = _accountRepository.GetUser(User.Identity.Name).EMailConfirmed,
                SearchText = searchText,
            };

            if (!model.IsConfirmed)
            {
                return View("SearchResults", model);
            }

            var searchResultRows = new List<SearchResultRowModel>();

            var initiativeHeads = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name);
            var initiatives = _initiativeBusiness.GetInitiatives().ToArray();
            var userInitiatives = initiativeHeads.Select(initiativeHead => initiatives.Single(x => x.Id == initiativeHead.Id)).ToArray();

            foreach (var initiative in userInitiatives)
            {
                var applications = initiative.ApplicationGroups.SelectMany(x => x.Applications).ToArray();
                var initiativeUniqueIdentifier = initiative.GetUniqueIdentifier(initiatives.Select(x => x.Name));

                var versions = new List<IApplicationVersion>();
                foreach (var application in applications)
                {
                    versions.AddRange(_applicationVersionBusiness.GetApplicationVersions(application.Id));
                }

                foreach (var version in versions)
                {
                    foreach (var issueType in version.IssueTypes)
                    {
                        if (issueType.Ticket.ToString().Equals(searchText))
                        {
                            foreach (var issue in issueType.Issues)
                            {
                                searchResultRows.Add(new SearchResultRowModel()
                                {
                                    InitiativeName = initiative.Name,
                                    InitiativeUniqueIdentifier = initiativeUniqueIdentifier,
                                    ApplicationName = applications.Single(x => x.Id == version.ApplicationId).Name,
                                    Version = version.Version,
                                    VersionUniqueIdentifier = version.GetUniqueIdentifier(versions.Select(x => x.Version)),
                                    IssueType = issueType,
                                    Issue = issue,
                                    Environment = GetSession(issue.SessionId).Environment,
                                });
                            }
                        }
                        else
                        {
                            foreach (var issue in issueType.Issues)
                            {
                                if (issue.Ticket.ToString().Equals(searchText))
                                {
                                    searchResultRows.Add(new SearchResultRowModel()
                                    {
                                        InitiativeName = initiative.Name,
                                        InitiativeUniqueIdentifier = initiativeUniqueIdentifier,
                                        ApplicationName = applications.Single(x => x.Id == version.ApplicationId).Name,
                                        Version = version.Version,
                                        VersionUniqueIdentifier = version.GetUniqueIdentifier(versions.Select(x => x.Version)),
                                        IssueType = issueType,
                                        Issue = issue,
                                        Environment = GetSession(issue.SessionId).Environment,
                                    });
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(issueType.ExceptionTypeName))
                                    {
                                        if (issueType.ExceptionTypeName.Contains(searchText))
                                        {
                                            searchResultRows.Add(new SearchResultRowModel()
                                            {
                                                InitiativeName = initiative.Name,
                                                InitiativeUniqueIdentifier = initiativeUniqueIdentifier,
                                                ApplicationName = applications.Single(x => x.Id == version.ApplicationId).Name,
                                                Version = version.Version,
                                                VersionUniqueIdentifier = version.GetUniqueIdentifier(versions.Select(x => x.Version)),
                                                IssueType = issueType,
                                                Issue = issue,
                                                Environment = GetSession(issue.SessionId).Environment,
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var environments = _initiativeBusiness.GetEnvironmentColors(User.Identity.GetUserName());

            model.SearchResultRows = searchResultRows;
            model.Environments = environments.Select(x => new EnvironmentViewModel()
            {
                Name = x.Key,
                Color = x.Value,
            }).ToList();
            
            return View("SearchResults", model);
        }

        private ISession GetSession(Guid sessionId)
        {
            return _sessionBusiness.GetSession(sessionId);
        }
    }
}