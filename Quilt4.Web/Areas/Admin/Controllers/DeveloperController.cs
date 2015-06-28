using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Areas.Admin.Models;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Admin")]
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
            return View(users);
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
            var developer = _accountRepository.FindById(id);
            
            return View(developer);
        }

        // POST: Admin/Developer/ConfirmDeveloperEMail
        [HttpPost]
        public ActionResult ConfirmDeveloperEMail(string id, FormCollection collection)
        {
            _accountRepository.ConfirmEmailAsync(id, null);
           
            return RedirectToAction("Index", "Developer");
        }

        // GET: Admin/Developer/Delete/5
        public ActionResult Delete(string id)
        {
            var developer = _accountRepository.GetUsers().Single(x => x.UserId == id);
            
            return View(developer);
        }

        // POST: Admin/Developer/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                _accountRepository.DeleteUser(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}