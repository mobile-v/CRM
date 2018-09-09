using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PartionnyAccount.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;
using System.Net.Http;

namespace PartionnyAccount.Controllers
{
    public class DevelopController : Controller
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        //Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        private DbConnectionSklad db = new DbConnectionSklad();

        #endregion


        // GET: Develop
        public async Task<ActionResult> Index(HttpRequestMessage request, FormCollection formCollection)
        {
            //return View();
            try
            {
                #region Логин

                HttpCookie CookieIPOL = Request.Cookies["CookieIPOL"];

                Classes.Account.Login login = new Classes.Account.Login();
                Classes.Account.Login.Field field = await Task.Run(() => login.Return(CookieIPOL, true)); //HttpCookie CookieIPOL = Request.Cookies["CookieIPOL"];
                if (!field.Access) { return Redirect("~/account/login/"); }

                #endregion

                #region Доступ в Разработчик

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDevelop"));
                if (iRight != 1)
                {
                    return Redirect("~/develop/noaccess/");
                }

                #endregion

                #region Тема, Язык, Интерфейс

                //Тема
                ViewData["CookieT"] = CookieIPOL["CookieT"]; if (ViewData["CookieT"] == null) ViewData["CookieT"] = 1;
                //Язык
                ViewData["CookieL"] = CookieIPOL["CookieL"]; if (ViewData["CookieL"] == null) ViewData["CookieL"] = 1;
                //Интерфейс
                ViewData["CookieI"] = CookieIPOL["CookieI"]; if (ViewData["CookieI"] == null) ViewData["CookieI"] = 1;

                #endregion


                return View();
            }
            catch (Exception ex)
            {
                Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
                return Redirect("~/account/login/?Err=" + exceptionEntry.Return(ex));
            }
        }

        public async Task<ActionResult> NoAccess(HttpRequestMessage request, FormCollection formCollection)
        {
            //return View();
            try
            {
                //Получаем параметр "Err"
                ViewData["Title"] = "Нет доступа в Разработчки печатных форма";

                return View();
            }
            catch (Exception ex)
            {
                Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
                return Redirect("~/account/login/?Err=" + exceptionEntry.Return(ex));
            }
        }

    }
}