using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Script.Serialization;
using Quilt4.Interface;
using Quilt4.Web.Agents;

namespace Quilt4.Web
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        private readonly IEventLogAgent _eventLogAgent;

        public ExceptionHandlingAttribute()
        {
            _eventLogAgent = new EventLogAgent(); //TODO: Resolve
        }

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

        public void LogExceptionCore(Exception exception)
        {
            _eventLogAgent.WriteToEventLog(exception);
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