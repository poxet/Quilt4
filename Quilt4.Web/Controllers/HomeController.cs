using System.Reflection;
using System.Web.Mvc;
using Quilt4.Interface;
using Tharga.Quilt4Net;

namespace Quilt4.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInitiativeBusiness _initiativeBusiness;

        public HomeController(IInitiativeBusiness initiativeBusiness)
        {
            _initiativeBusiness = initiativeBusiness;
        }

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
            ViewBag.Environment = Information.Environment;
            ViewBag.Quilt4SessionStarter = Tharga.Quilt4Net.Session.ClientStartTime.ToLocalTime();
            ViewBag.Quilt4RegisteredOnServer = Tharga.Quilt4Net.Session.RegisteredOnServer;
            ViewBag.Quilt4HasClientToken = !string.IsNullOrEmpty(Configuration.ClientToken);
            ViewBag.Quilt4IsEnabled = Configuration.Enabled;

            return View();
        }
    }
}