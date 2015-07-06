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
        private readonly ISystemBusiness _systemBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly ISettingsBusiness _settingsBusiness;
        private readonly IEventLogAgent _eventLogAgent;
        private readonly IInfluxDbAgent _influxDbAgent;

        public DashboardController(ISystemBusiness systemBusiness, IInitiativeBusiness initiativeBusiness, ISettingsBusiness settingsBusiness, IEventLogAgent eventLogAgent)
        {
            _systemBusiness = systemBusiness;
            _initiativeBusiness = initiativeBusiness;
            _settingsBusiness = settingsBusiness;
            _eventLogAgent = eventLogAgent;
            _influxDbAgent = influxDbAgent;
        }

        // GET: Admin/Dashboard/Index
        public ActionResult Index()
        {
            //TODO: Make this faster (Or use InfluxDB to get the data)
            //var initiativeCount = _initiativeBusiness.GetInitiativeCount();
            //var applicationCount = _initiativeBusiness.GetApplicationCount();
            //var issueTypeCount = _initiativeBusiness.GetIssueTypeCount();
            //var issueCount = _initiativeBusiness.GetIssueCount();

            var adminIndexViewModel = new AdminIndexViewModel
            {
                InitiativeCount = -1, //initiativeCount,
                ApplicationCount = -1, //applicationCount,
                IssueTypeCount = -1, //issueTypeCount,
                IssueCount = -1, //issueCount
            };

            return View(adminIndexViewModel);
        }

        // GET: Admin/Dashboard/System
        public ActionResult System()
        {
            var adminViewModel = new AdminViewModels();

            var dBType = ConfigurationManager.AppSettings["Repository"];
            adminViewModel.DbType = dBType;

            var dbinfo = _systemBusiness.GetDataBaseStatus();
            adminViewModel.DbOnline = dbinfo.Online;
            adminViewModel.DbName = dbinfo.Name;
            adminViewModel.DbServer = dbinfo.Server;

            var emailSetting = _settingsBusiness.GetEmailSetting();
            adminViewModel.SupportEmailAdress = emailSetting.SupportEmailAddress;
            adminViewModel.SmtpServerAdress = emailSetting.SmtpServerAdress;
            adminViewModel.SmtpServerPort = emailSetting.SmtpServerPort.ToString();
            adminViewModel.SendEMailEnabled = emailSetting.SendEMailEnabled.ToString();
            adminViewModel.EMailConfirmationEnabled = emailSetting.EMailConfirmationEnabled.ToString();

            adminViewModel.InfluxDbEnabled = _influxDbAgent.IsEnabled;
            adminViewModel.InfluxDbOnline = _influxDbAgent.CanConnect();
            adminViewModel.InfluxDbUrl = _influxDbAgent.GetSetting().Url;
            adminViewModel.InfluxDbName = _influxDbAgent.GetSetting().DatabaseName;
            adminViewModel.InfluxDbVersion = _influxDbAgent.GetDatabaseVersion();

            return View(adminViewModel);
        }
    }
}