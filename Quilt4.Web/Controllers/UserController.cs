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

            var i = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name).Where(x => x.Name == initiativeidentifier).ToArray();
            var initiativeId = Guid.Empty;

            if (i.Count() == 1)//Name is unique
            {
                initiativeId = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name).Single(x => x.Name == initiativeidentifier).Id;
            }
            else//go with id
            {
                initiativeId = _initiativeBusiness.GetInitiativesByDeveloper(User.Identity.Name).Single(x => x.Id == Guid.Parse(initiativeidentifier)).Id;
            }

            if (initiativeId == Guid.Empty)
            {
                throw new NullReferenceException("No initiative found for the specified uid.");
            }

            var initiative = _initiativeBusiness.GetInitiative(initiativeId);
            var initiativeName = initiative.Name;
            var sessions = _sessionBusiness.GetSessionsForUser(userId).ToArray();

            var applicationIds = sessions.GroupBy(x => x.ApplicationId).Select(x => x.First().ApplicationId).ToArray();
            var applicationNames = new List<string>();
            foreach (var applicationId in applicationIds)
            {
                var singleOrDefault = initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Id == applicationId);
                if (singleOrDefault != null) applicationNames.Add(singleOrDefault.Name);
            }

            var machineIds = sessions.GroupBy(x => x.MachineFingerprint).Select(x => x.First().MachineFingerprint).ToArray();
            var machineNames = new List<string>();
            foreach (var machineId in machineIds)
            {
                machineNames.Add(_machineBusiness.GetMachine(machineId).Name);
            }

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
                Users = users,
                ApplicationNames = applicationNames,
                Machines = machines,
                MachineNames = machineNames,
                InitiativeName = initiativeName,
                InitiativeUniqueIdentifier = initiativeidentifier,
            };

            return View(model);
        }
    }
}