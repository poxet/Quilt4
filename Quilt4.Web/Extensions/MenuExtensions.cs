using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Quilt4.Web
{
    public class Link
    {
        public string Text { get; private set; }
        public string Action { get; private set; }
        public string Controller { get; private set; }

        public Link(string text, string action, string controller)
        {
            Text = text;
            Action = action;
            Controller = controller;
        }
    }

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

            a.InnerHtml = "<a href=\"" + urlHelper.Action(action, controller) + "\" " + (active ? "class=\"active\"" : "") + "><i class=\"" + iconClass + "\"></i> " + text + "</a>";
            return MvcHtmlString.Create(a.ToString());
        }

        

        public static MvcHtmlString MenuItem(
            this HtmlHelper<dynamic> htmlHelper,
            string text,
            string iconClass,
            IEnumerable<Link> links
        )
        {

            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            var a = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;
            
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");

            var htmlText = "<a href=\"#\"><i class=\"" + iconClass + "\"></i> Configuration<span class=\"fa arrow\"></span></a><ul class=\"nav nav-second-level\">";

            var anyActive = false;

            foreach (var link in links)
            {
                var active = false;

                if (string.Equals(currentAction, link.Action, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(currentController, link.Controller, StringComparison.OrdinalIgnoreCase))
                {
                    active = true;
                    anyActive = true;
                }

                htmlText += "<li><a href=\"" + urlHelper.Action(link.Action, link.Controller) +"\"" + (active ? "class=\"active\"" : "") + ">" + link.Text + "</a></li>";
            }

            if(anyActive)
                a.AddCssClass("active");

            htmlText += "</ul>";

            a.InnerHtml = htmlText;

            return MvcHtmlString.Create(a.ToString());
        }

        

    }
}