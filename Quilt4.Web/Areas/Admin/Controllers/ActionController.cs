using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Quilt4.Interface;
using Quilt4.Web.Models;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    public class ActionController : Controller
    {
        private readonly IEventLogAgent _eventLogAgent;
        private readonly ISettingsBusiness _settingsBusiness;

        public ActionController(IEventLogAgent eventLogAgent, ISettingsBusiness settingsBusiness)
        {
            _eventLogAgent = eventLogAgent;
            _settingsBusiness = settingsBusiness;
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
                             EventLogCheckMessage = eventLogCheckMessage,
                         };
            return PartialView(vm);
        }

        public ActionResult _EventLogAlert()
        {
            var lastRead = _settingsBusiness.GetEventLogReadDate();

            var eventLogData = new List<EventLogItemModel>();
            try
            {
                var eventLogEntries = _eventLogAgent.GetEventLogData().Where(x => x.EntryType == EventLogEntryType.Error && x.TimeGenerated > lastRead);
                eventLogData = eventLogEntries.OrderByDescending(x => x.TimeGenerated).Select(x => new EventLogItemModel { EntryType = x.EntryType, Icon = EventLogController.GetIcon(x.EntryType), Message = x.Message, TimeGenerated = x.TimeGenerated, Source = x.Source }).ToList();
            }
            catch (Exception)
            {
            }

            var vm = new EventLogStatusViewModel
            {
                EventLogData = eventLogData
            };
            return PartialView(vm);            
        }
    }
}