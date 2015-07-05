using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Business;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserBusiness _userBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IMachineBusiness _machineBusiness;

        public UserController(IUserBusiness userBusiness, IInitiativeBusiness initiativeBusiness, ISessionBusiness sessionBusiness, IMachineBusiness machineBusiness)
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

        public ActionResult Details(string applicationVersionId, string userId)
        {
            
            //var applications = _initiativeBusiness.GetApplicationsByUser(userId);
            var sessions = _sessionBusiness.GetSessionsForUser(userId).ToArray();

            var machines= new List<IMachine>();
            var users = new List<IUser>();
            foreach (var session in sessions)
            {
                machines.Add(_machineBusiness.GetMachine(session.MachineFingerprint));
                users.Add(_userBusiness.GetUser(session.UserFingerprint));
            }


            var model = new UserViewModel()
            {
                Sessions = sessions,
                Machines = machines,
                Users = users
            };

            return View(model);
        }
    }
}