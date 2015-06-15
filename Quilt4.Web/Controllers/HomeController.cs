using System.Reflection;
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
            ViewBag.Environment = Tharga.Quilt4Net.Information.Environment;
            ViewBag.Quilt4SessionStarter = Tharga.Quilt4Net.Session.ClientStartTime.ToLocalTime();
            ViewBag.Quilt4RegisteredOnServer = Tharga.Quilt4Net.Session.RegisteredOnServer;
            ViewBag.Quilt4HasClientToken = !string.IsNullOrEmpty(Tharga.Quilt4Net.Configuration.ClientToken);
            ViewBag.Quilt4IsEnabled = Tharga.Quilt4Net.Configuration.Enabled;

            return View();
        }
    }
}