using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Agents;

namespace Quilt4.Web.Controllers
{
    public class HelpController : Controller
    {
        private readonly IEventLogAgent _eventLogAgent;

        public HelpController(IEventLogAgent eventLogAgent)
        {
            _eventLogAgent = eventLogAgent;
        }

        // GET: Help/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EventLog()
        {
            @ViewBag.EventLogInitiatLentry = EventLogAgent.EventLogInitialMessage;

            if (_eventLogAgent.AssureEventLogSource() != null)
            {
                @ViewBag.EventLogSourceHelp = true;
            }

            return View();
        }
    }
}
