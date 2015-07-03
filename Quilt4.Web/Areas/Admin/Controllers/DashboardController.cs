using System.Configuration;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Business;
using Quilt4.Web.Models;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Admin")]
    public class DashboardController : Controller
    {
        private readonly SystemBusiness _systemBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly ISettingsBusiness _settingsBusiness;
        private readonly IEventLogAgent _eventLogAgent;

        public DashboardController(SystemBusiness systemBusiness, IInitiativeBusiness initiativeBusiness, ISettingsBusiness settingsBusiness, IEventLogAgent eventLogAgent)
        {
            _systemBusiness = systemBusiness;
            _initiativeBusiness = initiativeBusiness;
            _settingsBusiness = settingsBusiness;
            _eventLogAgent = eventLogAgent;
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