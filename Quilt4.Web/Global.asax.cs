using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Castle.Windsor;
using Quilt4.Interface;
using Quilt4.Web.Models;
using Tharga.Quilt4Net;

namespace Quilt4.Web
{
    public class MvcApplication : HttpApplication
    {
        private readonly static IWindsorContainer _container;

        static MvcApplication()
        {
            _container = new WindsorContainer();
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
            EventLog.WriteEntry(Models.Constants.EventLogName, "Ending", EventLogEntryType.Information);
            Tharga.Quilt4Net.Session.End();
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
                ExceptionHandlingAttribute.LogExceptionCore(exp);
            }
        }
    }

    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Exception exp;
            try
            {
                LogExceptionCore(context.Exception);
                var errorData = ExceptionToJson(context.Exception);
                var httpResponseMessage = new HttpResponseMessage(GetStatusCode(context.Exception))
                {
                    ReasonPhrase = errorData.Replace(Environment.NewLine, " "),
                    RequestMessage = context.Request,
                };
                exp = new HttpResponseException(httpResponseMessage);
            }
            catch (Exception exception)
            {
                LogExceptionCore(exception);
                throw;
            }
            throw exp;
        }

        public static void LogExceptionCore(Exception exception)
        {
            //1. !!!! This logger should never use quilt4net, since it could result in infinite recursive calls

            //2. Log directly using business objects
            //TODO:

            //3. Log to event log
            WriteToEventLog(exception);
        }

        private static void WriteToEventLog(Exception exception)
        {
            var message = GetMessageFromException(exception);
            WriteToEventLog(message, EventLogEntryType.Error);
        }

        private static string GetMessageFromException(Exception exception, bool appendStackTrace = true)
        {
            if (exception == null)
                return null;

            var sb = new StringBuilder();

            sb.AppendFormat("{0} ", exception.Message);
            if (exception.Data.Count > 0)
            {
                sb.Append("[");
                var sb2 = new StringBuilder();
                foreach (DictionaryEntry data in exception.Data)
                {
                    sb2.Append(data.Key + ": " + data.Value + ", ");
                }
                sb.Append(sb2.ToString().TrimEnd(',', ' '));
                sb.Append("]");
            }

            var subMessage = GetMessageFromException(exception.InnerException, false);
            if (!string.IsNullOrEmpty(subMessage))
            {
                sb.AppendFormat(" / {0}", subMessage);
            }

            if (appendStackTrace)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("@ {0}", exception.StackTrace);
            }

            return sb.ToString();
        }

        public static void DeleteLog()
        {
            if (EventLog.SourceExists(Constants.EventSourceName))
            {
                EventLog.DeleteEventSource(Constants.EventSourceName);
            }

            if (EventLog.GetEventLogs().Any(x => x.Log == Constants.EventLogName))
            {
                EventLog.Delete(Constants.EventLogName);
            }
        }

        public static IEnumerable<string> GetEventLogData()
        {
            var myLog = new EventLog(Constants.EventLogName);
            foreach (EventLogEntry entry in myLog.Entries)
            {
                yield return entry.Message;
            }
        }

        public static Exception AssureEventLogSource()
        {
            try
            {
                if (!EventLog.SourceExists(Constants.EventSourceName))
                {
                    EventLog.CreateEventSource(new EventSourceCreationData(Constants.EventSourceName, Constants.EventLogName));
                }
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        public static void WriteToEventLog(string message, EventLogEntryType eventLogEntryType)
        {
            AssureEventLogSource();

            var myLog = new EventLog(Constants.EventLogName);
            myLog.Source = Constants.EventSourceName;
            myLog.WriteEntry(message, eventLogEntryType);
        }

        class Error
        {
            public string Message { get; set; }
            public Dictionary<string, string> Data { get; set; }
        }

        private string ExceptionToJson(Exception exception)
        {
            var serializer = new JavaScriptSerializer();
            var message = exception != null ? exception.Message : "Unknown error.";
            //There can be no json in the response. Remove data if it is of json type.
            var dictionary = exception != null ? exception.Data.Cast<DictionaryEntry>().Where(x => x.Value != null).ToDictionary(item => item.Key.ToString(), item => item.Value.ToString()) : null;
            var a = new Error
            {
                Message = message,
                //TODO: What data is sent is very very sensitive. Invalid data will make breate the entire message and give an internal 500 error as a response.
                //Before enabling this, make sure that only valid data is sent.
                //Data = dictionary
            };
            var d = serializer.Serialize(a);

            return d;
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            if (exception == null)
                return HttpStatusCode.InternalServerError;

            switch (exception.GetType().Name)
            {
                case "InvalidOperationException":
                    return HttpStatusCode.InternalServerError;
                case "NotImplementedException":
                    return HttpStatusCode.NotImplemented;
                case "AuthenticationException":
                    return HttpStatusCode.Forbidden;
                case "ArgumentException":
                case "ArgumentNullException":
                    return HttpStatusCode.BadRequest;
                //return HttpStatusCode.Created
                //return HttpStatusCode.MethodNotAllowed
                default:
                    //return HttpStatusCode.BadRequest;
                    return HttpStatusCode.InternalServerError;
            }
        }
    }

}