using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Quilt4.Web
{
    public static class MenuExtensions
    {
        public static MvcHtmlString MenuItem(
            this HtmlHelper<dynamic> htmlHelper,
            string text,
            string iconClass,
            string action,
            string controller
        )
        {
            var active = false;

            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var a = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase))
            {
                active = true;
            }
            
            a.InnerHtml = "<a href=\"" + urlHelper.Action(action, controller) + "\" "+ (active ? "class=\"active\"" : "") +"><i class=\"" + iconClass + "\"></i> " + text + "</a>";
            return MvcHtmlString.Create(a.ToString());
        }
    }
}