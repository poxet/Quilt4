using System;
using System.Web.Mvc;
using Quilt4.Interface;

namespace Quilt4.Web.Controllers
{
    //TODO: Roles
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class AdminDeveloperController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AdminDeveloperController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        // GET: AdminDeveloper
        public ActionResult Index()
        {
            var users = _accountRepository.GetUsers();
            return View(users);
        }

        //// GET: AdminDeveloper/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: AdminDeveloper/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: AdminDeveloper/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: AdminDeveloper/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: AdminDeveloper/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: AdminDeveloper/Delete/5
        public ActionResult Delete(string id)
        {
            var developer = _accountRepository.FindById(id);
            return View(developer);
        }

        // POST: AdminDeveloper/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                _accountRepository.DeleteUser(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ConfirmDeveloperEMail(string id)
        {
            var developer = _accountRepository.FindById(id);
            return View(developer);
        }

        [HttpPost]
        public ActionResult ConfirmDeveloperEMail(string id, FormCollection collection)
        {
            try
            {
                _accountRepository.ConfirmEmailAsync(id, null);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult MakeDeveloperAdmin(string id)
        {
            _accountRepository.AssignRole(id, "Admin");
            return RedirectToAction("Index");
        }
    }
}
