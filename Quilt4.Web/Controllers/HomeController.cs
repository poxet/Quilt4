using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
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

        public HomeController(IInitiativeBusiness initiativeBusiness, IApplicationVersionBusiness applicationVersionBusiness, IAccountRepository accountRepository, ISessionBusiness sessionBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
            _applicationVersionBusiness = applicationVersionBusiness;
            _accountRepository = accountRepository;
            _sessionBusiness = sessionBusiness;
        }

        public ActionResult Index()
        {
            return View();
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

            model.SearchResultRows = searchResultRows;

            

            //if (searchText.IsNullOrEmpty())
            //{
            //    return RedirectToAction("Search","Home");
            //}

            //var initiativeHeads = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name);
            //var initiatives = initiativeHeads.Select(initiativeHead => _initiativeBusiness.GetInitiative(initiativeHead.Id)).ToArray();

            //var applications = new List<IApplication>();
            //foreach (var initiative in initiatives)
            //{
            //    applications.AddRange(initiative.ApplicationGroups.SelectMany(x => x.Applications));
            //}

            //var applicationIds = new List<Guid>();
            //var ticketPrefixs = new List<string>();
            //foreach (var application in applications)
            //{
            //    applicationIds.Add(application.Id);
            //    ticketPrefixs.Add(application.TicketPrefix);
            //}

            //var versions = new List<IApplicationVersion>();
            //foreach (var applicationId in applicationIds)
            //{
            //    versions.AddRange(_applicationVersionBusiness.GetApplicationVersions(applicationId));
            //}

            //var allIssueTypes = versions.SelectMany(x => x.IssueTypes).ToArray();
            
            //var issueTypeResults = new List<IIssueType>();
            //foreach (var issueType in allIssueTypes)
            //{   
            //    int value;
            //    if (int.TryParse(model.SearchText, out value))
            //    {
            //        foreach (var issue in issueType.Issues)
            //        if (issueType.Ticket.ToString().Equals(model.SearchText) ||issue.Ticket.ToString().Equals(model.SearchText))
            //        {
            //            issueTypeResults.Add(issueType);
            //        }
                    
            //    }

            //    if (!issueType.ExceptionTypeName.IsNullOrEmpty())
            //    {
            //        if (issueType.ExceptionTypeName.Contains(model.SearchText))
            //        {
            //            issueTypeResults.Add(issueType);
            //        }
            //    }
            //}

            //model.IssueTypeResults = issueTypeResults;
            
            return View("SearchResults", model);
        }

        private ISession GetSession(Guid sessionId)
        {
            return _sessionBusiness.GetSession(sessionId);
        }
    }
}