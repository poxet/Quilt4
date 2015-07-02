using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Reflection;
using Microsoft.AspNet.Identity;
using Quilt4.Interface;

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
            if (User.Identity.IsAuthenticated)
            {
                var initiativeHeads = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name);
                var initiatives = initiativeHeads.Select(initiativeHead => _initiativeBusiness.GetInitiative(initiativeHead.Id)).ToList();

                var initiaivesWithInvites = new List<IInitiative>();

                foreach (var initiative in initiatives)
                {
                    if (initiative.DeveloperRoles.Any(x => x.DeveloperName.Equals(User.Identity.Name) && x.RoleName.Equals("Invited")))
                    {
                        initiaivesWithInvites.Add(initiative);
                    }
                }

                return View(initiaivesWithInvites);
            }
            return View(new List<IInitiative>());
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