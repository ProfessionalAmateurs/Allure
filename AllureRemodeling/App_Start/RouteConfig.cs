using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AllureRemodeling
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Material",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Manager", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "MaterialCreate",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Manager", action = "Create", IdItem = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "MaterialEdit",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Material", action = "Edit", IdItem = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "MaterialDelete",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Material", action = "Delete", IdItem = UrlParameter.Optional }
            );
        }
    }
   
}

