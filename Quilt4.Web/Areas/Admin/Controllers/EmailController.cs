using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    [RouteArea("Admin")]
    public class EmailController : Controller
    {
        private readonly IEmailBusiness _emailBusiness;
        
        public EmailController(IEmailBusiness emailBusiness)
        {
            _emailBusiness = emailBusiness;    
        }

        // GET: Admin/Email/EmailHistory
        public ActionResult EmailHistory()
        {
            var emails = _emailBusiness.GetLastHundredEmails();

            return View(emails);
        }

        // GET: Admin/Email/SendTestEmail
        public ActionResult SendTestEmail()
        {
            var model = new SendEmailViewModel();

            model.EmailEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["SendEMailEnabled"]);

            return View(model);
        }

        // POST: Admin/Email/SendTestEmail
        [HttpPost]
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
                return Redirect("SendTestEmail");
            }

            return View(model);
        }
    }
}
