using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Quilt4.Interface;
using Quilt4.Web.Agents;
using Tharga.Quilt4Net;

namespace Quilt4.Web
{
    public class MvcApplication : HttpApplication
    {
        private readonly static IWindsorContainer _container;
        private readonly IEventLogAgent _eventLogAgent;

        static MvcApplication()
        {
            _container = new WindsorContainer();
        }

        public MvcApplication()
        {
            _eventLogAgent = new EventLogAgent(); //TODO: Resolve
        }

        public static IWindsorContainer Container { get { return _container; } }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterWindsor();

            //GlobalConfiguration.Configuration.MessageHandlers.Add(new WebApiCallLogHandler(new SettingsBusiness())); //TODO: Resolve the SettingsBusiness instead

            RegisterQuilt4Session();
        }

        private void RegisterQuilt4Session()
        {
            var sb = _container.Resolve<ISettingsBusiness>();
            Configuration.ClientToken = sb.GetQuilt4ClientToken();
            Configuration.Target.Location = sb.GetQuilt4TargetLocation(Configuration.Target.Location);

            if (!string.IsNullOrEmpty(Configuration.ClientToken))
            {
                Tharga.Quilt4Net.Session.RegisterCompleteEvent += Session_RegisterCompleteEvent;
                Tharga.Quilt4Net.Session.BeginRegister(Assembly.GetAssembly(typeof(MvcApplication)));
            }
            else
            {
                Configuration.Enabled = false;
            }
        }

        void Session_RegisterCompleteEvent(object sender, Session.RegisterCompleteEventArgs e)
        {
            if (!e.Success)
            {
                LogException(e.Exception);
            }
        }

        private static void RegisterWindsor()
        {
            //_container = new WindsorContainer();
            _container.Install(new ApplicationCastleInstaller());
            //_container.Install(new WindsorCompositionRoot());

            // Create the Controller Factory
            var castleControllerFactory = new CastleControllerFactory(_container);

            // Add the Controller Factory into the MVC web request pipeline
            ControllerBuilder.Current.SetControllerFactory(castleControllerFactory);

            //TODO: Is this line needed for web api calls?
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(_container));
        }

        protected void Application_End()
        {
            Tharga.Quilt4Net.Session.End();
            //EventLog.WriteEntry(Constants.EventLogName, "Ending", EventLogEntryType.Information);
        }

        protected void Application_BeginRequest()
        {
            try
            {
                if (string.Compare(Request.Url.Host, "quilt4net.com", StringComparison.InvariantCultureIgnoreCase) == 0 || (string.Compare(Request.Url.Scheme, "http", StringComparison.InvariantCultureIgnoreCase) == 0 && (string.Compare(Request.Url.Host, "quilt4net.com", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(Request.Url.Host, "www.quilt4net.com", StringComparison.InvariantCultureIgnoreCase) == 0)))
                {
                    var targetUrl = "https://www.quilt4.com" + Request.Url.LocalPath;
                    Issue.BeginRegister(string.Format("Redirecting web page."), Issue.MessageIssueLevel.Information, data: new Dictionary<string, string> { { "SourceUrl", "Request.Url.Host" }, { "TargetUrl", targetUrl } });
                    Response.Redirect(targetUrl, true);
                }
            }
            catch (Exception exception)
            {
                LogException(exception);
                throw;
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            var lastError = Server.GetLastError();
            LogException(lastError);
        }

        private string GetUserHandle()
        {
            try
            {
                return User.Identity.Name;
            }
            catch
            {
                return null;
            }
        }

        private void LogException(Exception exception)
        {
            try
            {
                Issue.Register(exception, Issue.ExceptionIssueLevel.Error, true, GetUserHandle());
            }
            catch (Exception exp)
            {
                _eventLogAgent.WriteToEventLog(exception);
            }
        }
    }
}