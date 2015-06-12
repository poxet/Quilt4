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
        private readonly IInitiativeBusiness _initiativeBusiness;

        public AdminController(SystemBusiness systemBusiness, IEmailBusiness emailBusiness, IInitiativeBusiness initiativeBusiness)
        {
            _systemBusiness = systemBusiness;
            _emailBusiness = emailBusiness;
            _initiativeBusiness = initiativeBusiness;
        }

        // GET: Admin
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

        public ActionResult SendTestEmail(SendEmailViewModel model)
        {
            var success = true;
            try
            {
                _emailBusiness.SendEmail(new List<string> { model.ToEmail }, "Test", "Testar");
            }
            catch (FormatException e)
            {

                ViewBag.ErrorMessage = "Fel format på E-Posten!";
                success = false;

            }
            catch (SmtpException e)
            {
                ViewBag.ErrorMessage = "Servern kan inte nås";
                success = false;
            }


            if (success)
            {
                return Redirect("Email");
            }

            return View("Email", model);
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