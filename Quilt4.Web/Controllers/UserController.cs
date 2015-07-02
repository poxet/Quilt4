using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Business;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly UserBusiness _userBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IMachineBusiness _machineBusiness;

        public UserController(UserBusiness userBusiness, IInitiativeBusiness initiativeBusiness, ISessionBusiness sessionBusiness, IMachineBusiness machineBusiness)
        {
            _userBusiness = userBusiness;
            _initiativeBusiness = initiativeBusiness;
            _sessionBusiness = sessionBusiness;
            _machineBusiness = machineBusiness;
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string applicationVersionId, string userName)
        {
            var userId = _userBusiness.GetUsersByApplicationVersion(applicationVersionId).Single(x => x.UserName == userName).Id;
            var sessions = _sessionBusiness.GetSessionsForUser(userId);
            //var machines = _machineBusiness.
            //_initiativeBusiness.
            

            var model = new UserModel()
            {
                Sessions = sessions,
            };

            return View(model);
        }
    }
}