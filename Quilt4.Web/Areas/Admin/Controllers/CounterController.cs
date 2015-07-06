using System.Web.Mvc;
using Quilt4.Interface;

namespace Quilt4.Web.Areas.Admin.Controllers
{
    public class CounterController : Controller
    {
        private readonly ISessionBusiness _sessionBusiness;
        private readonly ICounterBusiness _counterBusiness;

        public CounterController(ISessionBusiness sessionBusiness, ICounterBusiness counterBusiness)
        {
            _sessionBusiness = sessionBusiness;
            _counterBusiness = counterBusiness;
        }

        // GET: Admin/Counter
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Counter/Reset
        public ActionResult Reset()
        {
            return View();
        }

        // POST: Admin/Counter/Reset
        [HttpPost]
        public ActionResult Reset(FormCollection collection)
        {
            _counterBusiness.ClearSessionCounters();

            var sessions = _sessionBusiness.GetSessions();
            _counterBusiness.UpdateSessionCounters(sessions);

            return View();
        }
    }
}