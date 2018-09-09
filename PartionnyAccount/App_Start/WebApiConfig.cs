using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PartionnyAccount
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            /*config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );*/


            // === Sklad === 

            //Main
            config.Routes.MapHttpRoute(
                name: "SkladMainApi",
                routeTemplate: "api/sklad/main/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );

            //Sys
            config.Routes.MapHttpRoute(
                name: "SkladSysApi",
                routeTemplate: "api/sklad/sys/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );

            //Dir
            config.Routes.MapHttpRoute(
                name: "SkladDirApi",
                routeTemplate: "api/sklad/dir/{controller}/{id}/",
                defaults: new { pSearch = RouteParameter.Optional, id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladDirSearchApi",
                routeTemplate: "api/sklad/dir/{controller}/{pSearch}/{iPriznak}/",
                defaults: new { pSearch = RouteParameter.Optional, iPriznak = RouteParameter.Optional }
            );

            //Doc
            config.Routes.MapHttpRoute(
                name: "SkladDocApi",
                routeTemplate: "api/sklad/doc/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladDoc2ParamApi",
                routeTemplate: "api/sklad/doc/{controller}/{id}/{DirStatusID}/",
                defaults: new { id = RouteParameter.Optional, DirStatusID = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladDoc3ParamApi",
                routeTemplate: "api/sklad/doc/{controller}/{id}/{ServiceTypeRepair}/{iTrash}/",
                defaults: new { id = RouteParameter.Optional, ServiceTypeRepair = RouteParameter.Optional, iTrash = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladDoc4ParamApi",
                routeTemplate: "api/sklad/doc/{controller}/{id}/{DirEmployeeID}/{iTrash}/{iTrash2}/",
                defaults: new { id = RouteParameter.Optional, DirEmployeeID = RouteParameter.Optional, iTrash = RouteParameter.Optional, iTrash2 = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladDoc5ParamApi",
                routeTemplate: "api/sklad/doc/{controller}/{id}/{iTrash0}/{iTrash}/{iTrash2}/{iTrash3}/",
                defaults: new { id = RouteParameter.Optional, iTrash0 = RouteParameter.Optional, iTrash = RouteParameter.Optional, iTrash2 = RouteParameter.Optional, iTrash3 = RouteParameter.Optional }
            );

            //Logistics
            config.Routes.MapHttpRoute(
                name: "SkladLogisticsApi",
                routeTemplate: "api/sklad/Logistic/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladLogistics2ParamApi",
                routeTemplate: "api/sklad/Logistic/{controller}/{id}/{DirStatusID}/",
                defaults: new { id = RouteParameter.Optional, DirStatusID = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladLogistics3ParamApi",
                routeTemplate: "api/sklad/Logistic/{controller}/{id}/{ServiceTypeRepair}/{iTrash}/",
                defaults: new { id = RouteParameter.Optional, ServiceTypeRepair = RouteParameter.Optional, iTrash = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladLogistics4ParamApi",
                routeTemplate: "api/sklad/Logistic/{controller}/{id}/{DirEmployeeID}/{iTrash}/{iTrash2}/",
                defaults: new { id = RouteParameter.Optional, DirEmployeeID = RouteParameter.Optional, iTrash = RouteParameter.Optional, iTrash2 = RouteParameter.Optional }
            );

            //Pay
            config.Routes.MapHttpRoute(
                name: "SkladPayApi",
                routeTemplate: "api/sklad/Pay/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );

            //Rem
            config.Routes.MapHttpRoute(
                name: "SkladRemApi",
                routeTemplate: "api/sklad/rem/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Report
            config.Routes.MapHttpRoute(
                name: "SkladReportApi",
                routeTemplate: "api/sklad/report/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );

            //List
            config.Routes.MapHttpRoute(
                name: "SkladListApi",
                routeTemplate: "api/sklad/list/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );
            //List - HTML и JSON. Используется для JSON
            config.Routes.MapHttpRoute(
                name: "SkladList2Api",
                routeTemplate: "api/sklad/list/{controller}/{Html1}/{Html2}/",
                defaults: new { Html1 = RouteParameter.Optional, Html2 = RouteParameter.Optional }
            );

            //Log
            config.Routes.MapHttpRoute(
                name: "SkladLogApi",
                routeTemplate: "api/sklad/log/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );

            //ExchangeData
            config.Routes.MapHttpRoute(
                name: "ExchangeDataApi",
                routeTemplate: "api/webapi/exchangedata/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //API
            config.Routes.MapHttpRoute(
                name: "APIApi",
                routeTemplate: "API/{controller}/{id}", //routeTemplate: "api/Sklad/Service/API/{controller}/{id}",  - можно и так указать, но тогда надо в Variables так же указать!
                defaults: new { id = RouteParameter.Optional }
            );
            //Timer
            config.Routes.MapHttpRoute(
                name: "Timer",
                routeTemplate: "Timer/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Sms
            config.Routes.MapHttpRoute(
                name: "SkladSmsApi",
                routeTemplate: "api/sklad/SMS/{controller}/{SmsTemplateID}/{DocServicePurchID}/",
                defaults: new { SmsTemplateID = RouteParameter.Optional, DocServicePurchID = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "SkladSmsApi2",
                routeTemplate: "api/sklad/SMS/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );


            // === Login === 

            //Dir
            config.Routes.MapHttpRoute(
                name: "LoginDirApi",
                routeTemplate: "api/login/dir/{controller}/{id}/",
                defaults: new { id = RouteParameter.Optional }
            );

            // Раскомментируйте следующую строку кода, чтобы включить поддержку запросов для действий с типом возвращаемого значения IQueryable или IQueryable<T>.
            // Чтобы избежать обработки неожиданных или вредоносных запросов, используйте параметры проверки в QueryableAttribute, чтобы проверять входящие запросы.
            // Дополнительные сведения см. по адресу http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // Чтобы отключить трассировку в приложении, закомментируйте или удалите следующую строку кода
            // Дополнительные сведения см. по адресу: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();

            /*var appJsonType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/json");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appJsonType);*/
        }
    }
}
