﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SigesoftWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               "General_login", "ingresar/",
               new { controller = "Generals", action = "Login" },
               new[] { "SigesoftWeb.Controllers" }
           );

            routes.MapRoute(
               "General_logout", "salir/",
               new { controller = "Generals", action = "Logout" },
               new[] { "SigesoftWeb.Controllers" }
           );

            routes.MapRoute(
              "General_notauthorized",
              "notauthorized/",
              new { controller = "Generals", action = "Notauthorized" },
              new[] { "SigesoftWeb.Controllers" }
            );

            routes.MapRoute(
             "General_sessionexpired",
             "adios/",
             new { controller = "Generals", action = "SessionExpired" },
             new[] { "SigesoftWeb.Controllers" }
            );


            routes.MapRoute(
                 "SigesoftWeb", "SigesoftWeb/",
                 new { controller = "Generals", action = "Home" },
                 new[] { "SigesoftWeb.Controllers" }
             );


            routes.MapRoute(
               "404",
               "404/",
               new { controller = "Generals", action = "NoFound" },
               new[] { "SigesoftWeb.Controllers" }
            );

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Generals", action = "Index", id = UrlParameter.Optional }
          );

        }
    }
}
