using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Controllers
{
    public class MachineController : Controller
    {
        private readonly IMachineBusiness _machineBusiness;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IInitiativeBusiness _initiativeBusiness;
        private readonly IUserBusiness _userBusiness;

        public MachineController(IMachineBusiness machineBusiness, ISessionBusiness sessionBusiness, IInitiativeBusiness initiativeBusiness, IUserBusiness userBusiness)
        {
            _machineBusiness = machineBusiness;
            _sessionBusiness = sessionBusiness;
            _initiativeBusiness = initiativeBusiness;
            _userBusiness = userBusiness;
        }

        // GET: Machine
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(string initiativeId, string machineId)
        {
            var initiative = _initiativeBusiness.GetInitiative(Guid.Parse(initiativeId));

            var sessions = _sessionBusiness.GetSessionsForMachine(machineId).ToArray();
            
            var applicationIds = sessions.GroupBy(x => x.ApplicationId).Select(x => x.First()).Select(x => x.ApplicationId).ToArray();
            var userIds = sessions.GroupBy(x => x.UserFingerprint).Select(x => x.First()).Select(x => x.UserFingerprint).ToArray();

            var applications = new List<IApplication>();
            foreach (var applicationId in applicationIds)
            {
                applications.Add(initiative.ApplicationGroups.SelectMany(x => x.Applications).SingleOrDefault(x => x.Id == applicationId));
            }

            var users = new List<IUser>();
            foreach (var userId in userIds)
            {
                users.Add(_userBusiness.GetUser(userId));
            }

            var machines = new List<IMachine>();
            foreach (var session in sessions)
            {
                machines.Add(_machineBusiness.GetMachine(session.MachineFingerprint));
            }

            var model = new MachineDetailsModel()
            {
                Sessions = sessions,
                Applications = applications,
                Users = users,
                Machines = machines,
                MachineName = _machineBusiness.GetMachine(machineId).Name
            };


            //var applications = _initiativeBusiness
            //var sessions = _sessionBusiness.
            return View(model);
        }
    }
}