using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    public class ActionController : Controller
    {
        private readonly IEventLogAgent _eventLogAgent;

        public ActionController(IEventLogAgent eventLogAgent)
        {
            _eventLogAgent = eventLogAgent;
        }

        public ActionResult _EventLogStatus()
        {
            var response = _eventLogAgent.AssureEventLogSource();
            string eventLogCheckMessage = null;
            if (response != null)
            {
                eventLogCheckMessage = "The event log Quilt4 does not exist and cannot be created. If something goes wrong issues cannot be written there. Try to run this instance as administrator once, or create the event log " + Constants.EventLogName + " and source " + Constants.EventSourceName + " manually. (" + response.Message + ")";
            }

            var vm = new EventLogStatusViewModel
                         {
                             EventLogCheckMessage = eventLogCheckMessage
                         };
            return PartialView("_EventLogStatus", vm);
        }

        //private void CheckEventlogAccess()
        //{
        //    //_eventLogAgent.DeleteLog();

        //    var response = _eventLogAgent.AssureEventLogSource();
        //    //var response = ExceptionHandlingAttribute;
        //    if (response != null)
        //    {
        //        //TODO: Try to write to the event log and tell the administrator if something went wrong.
        //        ViewBag.EventLogCheckMessage = "The event log Quilt4 does not exist and cannot be created. If something goes wrong issues cannot be written there. Try to run this instance as administrator once, or create the event log " + Constants.EventLogName + " and source " + Constants.EventSourceName + " manually. (" + response.Message + ")";
        //    }

        //    //TODO: Check if there are new issues and show them on the dashboard.

        //    //TODO: Load issues related to Quilt4 and show them on the admin page
        //    //var entries = ExceptionHandlingAttribute.GetEventLogData().ToArray();
        //}
    }
}