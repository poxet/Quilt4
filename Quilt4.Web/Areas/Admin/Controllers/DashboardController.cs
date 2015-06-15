using System.Configuration;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Business;
using Quilt4.Web.Models;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    [RouteArea("Admin")]
    public class DashboardController : Controller
    {
        private readonly SystemBusiness _systemBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;

        public DashboardController(SystemBusiness systemBusiness, IInitiativeBusiness initiativeBusiness)
        {
            _systemBusiness = systemBusiness;
            _initiativeBusiness = initiativeBusiness;
        }

        // GET: Admin/Dashboard/Index
        public ActionResult Index()
        {
            var initiativeCount = _initiativeBusiness.GetInitiativeCount();
            var applicationCount = _initiativeBusiness.GetApplicationCount();
            var issueTypeCount = _initiativeBusiness.GetIssueTypeCount();
            var issueCount = _initiativeBusiness.GetIssueCount();

            var adminIndexViewModel = new AdminIndexViewModel
            {
                InitiativeCount = initiativeCount,
                ApplicationCount = applicationCount,
                IssueTypeCount = issueTypeCount,
                IssueCount = issueCount
            };

            return View(adminIndexViewModel);
        }

        // GET: Admin/Dashboard/System
        public ActionResult System()
        {
            var adminViewModel = new AdminViewModels();

            var dBType = ConfigurationManager.AppSettings["Repository"];
            adminViewModel.DBType = dBType;

            var dbinfo = _systemBusiness.GetDataBaseStatus();
            adminViewModel.DBOnline = dbinfo.Online;
            adminViewModel.DBName = dbinfo.Name;
            adminViewModel.DBServer = dbinfo.Server;

            var supportEmailAdress = ConfigurationManager.AppSettings["SupportEmailAddress"];
            var smtpServerAdress = ConfigurationManager.AppSettings["SmtpServerAddress"];
            var smtpServerPort = ConfigurationManager.AppSettings["SmtpServerPort"];
            var sendEMailEnabled = ConfigurationManager.AppSettings["SendEMailEnabled"];
            var eMailConfirmationEnabled = ConfigurationManager.AppSettings["EMailConfirmationEnabled"];
            adminViewModel.SupportEmailAdress = supportEmailAdress;
            adminViewModel.SmtpServerAdress = smtpServerAdress;
            adminViewModel.SmtpServerPort = smtpServerPort;
            adminViewModel.SendEMailEnabled = sendEMailEnabled;
            adminViewModel.EMailConfirmationEnabled = eMailConfirmationEnabled;

            return View(adminViewModel);
        }
    }
}