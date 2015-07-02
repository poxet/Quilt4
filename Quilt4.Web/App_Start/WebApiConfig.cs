using System.Web.Http;

namespace Quilt4.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute(
            //    name: "ActionApi",
            //    routeTemplate: "api/{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //config.Filters.Add(new ExceptionHandlingAttribute());

            //config.Routes.MapHttpRoute(
            //    name: "MethodTwo",
            //    routeTemplate: "api/{controller}/methodtwo/{directory}/{report}",
            //    defaults: new { directory = RouteParameter.Optional, report = RouteParameter.Optional }
            //);

            config.Filters.Add(new ExceptionHandlingAttribute());
        }
    }
}
