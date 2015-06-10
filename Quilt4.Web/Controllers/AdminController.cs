using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Quilt4.Web.Models;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Quilt4.Interface;
using Quilt4.Web.Business;

namespace Quilt4.Web.Controllers
{
    //TODO: Roles
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly SystemBusiness _systemBusiness;
        private readonly IEmailBusiness _emailBusiness;

        public AdminController(SystemBusiness systemBusiness, IEmailBusiness emailBusiness)
        {
            _systemBusiness = systemBusiness;
            _emailBusiness = emailBusiness;
        }

        // GET: Admin
        public ActionResult Index()
        {


            return View();
        }

        public ActionResult EmailHistory()
        {
            var emails = _emailBusiness.GetLastHundredEmails();

            return View(emails);
        }


        public ActionResult Email()
        {
            var model = new SendEmailViewModel();

            model.EmailEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["SendEMailEnabled"]);

            return View(model);
        }

        public ActionResult SendEmail(SendEmailViewModel model)
        {
            _emailBusiness.SendEmail(new List<string>{model.ToEmail}, "Test", "Testar");

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