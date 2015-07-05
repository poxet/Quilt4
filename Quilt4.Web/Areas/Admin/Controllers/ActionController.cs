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
            string eventLogCheckMessage = null;
            if (User.IsInRole("Admin"))
            {
                var response = _eventLogAgent.AssureEventLogSource();
                if (response != null)
                {
                    eventLogCheckMessage = "The event log Quilt4 does not exist and cannot be created.";
                }
            }

            var vm = new EventLogStatusViewModel
                         {
                             EventLogCheckMessage = eventLogCheckMessage,
                         };
            return PartialView(vm);
        }

        public ActionResult _EventLogAlert()
        {
            var eventLogData = new List<EventLogItemModel>();
            if (User.IsInRole("Admin"))
            {
                var lastRead = _settingsBusiness.GetEventLogReadDate();
                try
                {
                    var eventLogEntries = _eventLogAgent.GetEventLogData().Where(x => x.EntryType == EventLogEntryType.Error && x.TimeGenerated > lastRead);
                    eventLogData = eventLogEntries.OrderByDescending(x => x.TimeGenerated).Select(x => new EventLogItemModel { EntryType = x.EntryType, Icon = EventLogController.GetIcon(x.EntryType), Message = x.Message, TimeGenerated = x.TimeGenerated, Source = x.Source }).ToList();
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
            }

            var vm = new EventLogStatusViewModel
            {
                EventLogData = eventLogData
            };
            return PartialView(vm);            
        }
    }
}