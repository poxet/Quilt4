using System.Web.Mvc;
using Castle.Core;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]

    //[Transient(Lifestyle = LifestyleType.PerWebRequest)]
    public class ActionController : Controller
    {
        public ActionResult _EventLogStatus()
        {
            //TODO: Check the status of the event log here!

            @ViewBag.EventLogCheckMessage = "Blah!";
            //var model = repository.GetThingByParameter(parameter1);
            //var partialViewModel = new PartialViewModel(model);
            //return PartialView(partialViewModel);
            var obj = new object();
            return PartialView("_EventLogStatus", obj);
        }
    }
}