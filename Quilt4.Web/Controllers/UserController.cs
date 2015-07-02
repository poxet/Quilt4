using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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


            var model = new UserModel()
            {
                Sessions = sessions,
            };

            return View(model);
        }
    }
}