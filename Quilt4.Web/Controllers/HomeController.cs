using System.Web.Mvc;
using System.Reflection;

namespace Quilt4.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult System()
        {
            
            ViewBag.Version = Assembly.GetAssembly(typeof(HomeController)).GetName().Version.ToString();

            return View();
        }
    }
}