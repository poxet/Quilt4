using System.Web.Mvc;
using Quilt4.Web.Models;
using System.Configuration;

namespace Quilt4.Web.Controllers
{
    //TODO: Roles
    //[Authorize(Roles = "Admin")]
    [Authorize]
    public class AdminController : Controller
    {
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
            return View(adminViewModel);
        }
    }
}