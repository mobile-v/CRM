using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PartionnyAccount
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;
            routes.AppendTrailingSlash = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            #region Registration

            //Русский
            routes.MapRoute(
               name: "Registration",
               url: "account/registratsiya/",
               defaults: new { controller = "Account", action = "Registration" }
            );
            //Украинский
            routes.MapRoute(
               name: "RegistrationUk",
               url: "uk/account/reyestratsiya/",
               defaults: new { controller = "Account", action = "RegistrationUk" }
            );
            //English
            routes.MapRoute(
               name: "RegistrationEn",
               url: "en/account/registration/",
               defaults: new { controller = "Account", action = "RegistrationEn" }
            );

            #endregion


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
