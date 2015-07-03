using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    public class EventLogController : Controller
    {
        private readonly IEventLogAgent _eventLogAgent;

        public EventLogController(IEventLogAgent eventLogAgent)
        {
            _eventLogAgent = eventLogAgent;
        }

        // GET: Admin/EventLog
        public ActionResult Index()
        {
            var model = new EventLogIndexViewModel
                            {
                                EventLogData = _eventLogAgent.GetEventLogData().OrderByDescending(x => x.TimeGenerated).Select(x => new EventLogItemModel{ EntryType = x.EntryType, Icon = GetIcon(x.EntryType), Message = x.Message, TimeGenerated = x.TimeGenerated, Source = x.Source}).ToList()
                            };
            return View(model);
        }

        public static string GetIcon(EventLogEntryType entryType)
        {
            switch (entryType)
            {
                case EventLogEntryType.Information:
                case EventLogEntryType.SuccessAudit:
                    return "fa-info";
                case EventLogEntryType.Error:
                case EventLogEntryType.FailureAudit:
                    return "fa-exclamation-circle";
                case EventLogEntryType.Warning:
                    return "fa-exclamation-triangle";
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown entry type {0}", entryType));
            }
        }

        public ActionResult Clear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Clear(FormCollection collection)
        {
            try
            {
                _eventLogAgent.ClearAll();
                return RedirectToAction("Index", "EventLog");
            }
            catch (Exception exception)
            {
                ViewBag.ErrorMessage = exception.Message;
                return View();
            }
        }
    }
}