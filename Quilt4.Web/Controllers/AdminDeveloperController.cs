using System.Linq;
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

        //// GET: AdminDeveloper/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: AdminDeveloper/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
