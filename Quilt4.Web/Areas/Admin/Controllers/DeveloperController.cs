using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;

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
            var model = _accountRepository.GetUsers().Single(x => x.UserId == id);

            return View(model);
        }

        // POST: Admin/Developer/ConfirmDeveloperEMail
        [HttpPost]
        public ActionResult ConfirmDeveloperEMail(string id, FormCollection collection)
        {
            _accountRepository.ConfirmEmailAsync(id, null);
           
            return RedirectToAction("Index", "Developer");
        }

        // GET: Admin/Developer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
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
                return View();
            }
        }
    }
}