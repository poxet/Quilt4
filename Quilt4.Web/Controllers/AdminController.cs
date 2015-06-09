using System.Web.Mvc;
using Quilt4.Web.Models;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Quilt4.Web.Business;

namespace Quilt4.Web.Controllers
{
    //TODO: Roles
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly SystemBusiness _systemBusiness;

        public AdminController(SystemBusiness systemBusiness)
        {
            _systemBusiness = systemBusiness;
        }

        // GET: Admin
        public ActionResult Index()
        {


            return View();
        }

        public ActionResult Email()
        {
            var model = new SendEmailViewModel();

            return View(model);
        }

        public ActionResult SendEmail(SendEmailViewModel model)
        {
            var smtpServerAdress = ConfigurationManager.AppSettings["SmtpServerAddress"];
            var smtpServerPort = ConfigurationManager.AppSettings["SmtpServerPort"];
            var mailFrom = ConfigurationManager.AppSettings["SupportEmailAddress"];

            int portNumber;
            int.TryParse(smtpServerPort, out portNumber);

            var smtpClient = new SmtpClient(smtpServerAdress, portNumber);
            var mailMessage = new MailMessage(mailFrom, model.ToEmail, "Test", "Test");

            smtpClient.Credentials = new NetworkCredential("daniel.bohlin@quilt4net.com", "All4One!");
            smtpClient.Send(mailMessage);

            return Redirect("Email");
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