using System.Web.Mvc;
using Quilt4.Web.Models;
using System.Configuration;
using Quilt4.Web.Business;

namespace Quilt4.Web.Controllers
{
    //TODO: Roles
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private SystemBusiness _systemBusiness;

        public AdminController(SystemBusiness systemBusiness)
        {
            _systemBusiness = systemBusiness;
        }

        // GET: Admin
        public ActionResult Index()
        {


            return View();
        }

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