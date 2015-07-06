using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
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

        public ActionResult Details(string initiativeidentifier, string userId)
        {
            if (initiativeidentifier == null) throw new ArgumentNullException("initiativeidentifier", "InitiativeId was not provided.");

            var i = _initiativeBusiness.GetInitiatives().Where(x => x.Name == initiativeidentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Name == initiativeidentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiatives().Single(x => x.Id == Guid.Parse(initiativeidentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(initiativeId);
            //var applicationId = initiative.ApplicationGroups.SelectMany(x => x.Applications).Select(x => x.Id);

            var sessions = _sessionBusiness.GetSessionsForUser(userId).ToArray();
            var applicationIds = sessions.GroupBy(x => x.ApplicationId).Select(x => x.First().ApplicationId);

            var applications = _initiativeBusiness.GetApplicationByApplicationIds(applicationIds)
            


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