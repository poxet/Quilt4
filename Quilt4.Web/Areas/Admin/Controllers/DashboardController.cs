using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Business;
using Quilt4.Web.Models;
using System.Diagnostics;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Admin")]
    public class DashboardController : Controller
    {
        private readonly SystemBusiness _systemBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly ISettingsBusiness _settingsBusiness;

        public DashboardController(SystemBusiness systemBusiness, IInitiativeBusiness initiativeBusiness, ISettingsBusiness settingsBusiness)
        {
            _systemBusiness = systemBusiness;
            _initiativeBusiness = initiativeBusiness;
            _settingsBusiness = settingsBusiness;
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

            CheckEventlogAccess();

            return View(adminIndexViewModel);
        }

        private void CheckEventlogAccess()
        {
            //ExceptionHandlingAttribute.DeleteLog();

            var response = ExceptionHandlingAttribute.AssureEventLogSource();
            if (response != null)
            {
                //TODO: Try to write to the event log and tell the administrator if something went wrong.
                ViewBag.EventLogCheckMessage = "The event log Quilt4 does not exist and cannot be created. If something goes wrong issues cannot be written there. Try to run this instance as administrator once, or create the event log " + Constants.EventLogName + " and source " + Constants.EventSourceName + " manually. (" + response.Message + ")";
            }

            //TODO: Check if there are new issues and show them on the dashboard.

            //TODO: Load issues related to Quilt4 and show them on the admin page
            //var entries = ExceptionHandlingAttribute.GetEventLogData().ToArray();
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

            var emailSetting = _settingsBusiness.GetEmailSetting();
            adminViewModel.SupportEmailAdress = emailSetting.SupportEmailAddress;
            adminViewModel.SmtpServerAdress = emailSetting.SmtpServerAdress;
            adminViewModel.SmtpServerPort = emailSetting.SmtpServerPort.ToString();
            adminViewModel.SendEMailEnabled = emailSetting.SendEMailEnabled.ToString();
            adminViewModel.EMailConfirmationEnabled = emailSetting.EMailConfirmationEnabled.ToString();

            return View(adminViewModel);
        }
    }
}