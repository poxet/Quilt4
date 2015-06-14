using System;
using System.Web.Mvc;
using Quilt4.Interface;

namespace Quilt4.Web.Controllers.Admin
{
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class DeveloperController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public DeveloperController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // GET: Admin/Developer/Index
        public ActionResult Index()
        {
            var users = _accountRepository.GetUsers();
            return View("~/Views/Admin/Developer/Index.cshtml", users);
        }

        // GET: Admin/Developer/MakeDeveloperAdmin
        public ActionResult MakeDeveloperAdmin(string id)
        {
            _accountRepository.AssignRole(id, "Admin");
            return RedirectToAction("Index");
        }

        // GET: Admin/Developer/ConfirmDeveloperEMail
        public ActionResult ConfirmDeveloperEMail(string id)
        {
            throw new NotImplementedException();
            //var developer = _accountRepository.FindById(id);
            //return View("", developer);
        }

        // POST: Admin/Developer/ConfirmDeveloperEMail
        [HttpPost]
        public ActionResult ConfirmDeveloperEMail(string id, FormCollection collection)
        {
            throw new NotImplementedException();
            //try
            //{
            //    _accountRepository.ConfirmEmailAsync(id, null);
            //    return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }

        // GET: Admin/Developer/Delete/5
        public ActionResult Delete(int id)
        {
            return View("~/Views/Admin/Developer/Delete.cshtml");
        }

        // POST: Admin/Developer/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View("~/Views/Admin/Developer/Delete.cshtml");
            }
        }
    }
}
