using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PartionnyAccount.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net.Http;

namespace PartionnyAccount.Controllers
{
    public class HomeController : Controller
    {
        //public ActionResult Index()
        public async Task<ActionResult> Index(HttpRequestMessage request, FormCollection formCollection)
        {
            //return View();
            try
            {
                #region Права

                HttpCookie CookieIPOL = Request.Cookies["CookieIPOL"];

                Classes.Account.Login login = new Classes.Account.Login();
                Classes.Account.Login.Field field = await Task.Run(() => login.Return(CookieIPOL, true)); //HttpCookie CookieIPOL = Request.Cookies["CookieIPOL"];
                if (!field.Access) { return Redirect("~/account/login/"); }
                //if (field.RetailOnly) { return Redirect("~/Retail/"); }

                #endregion


                #region Обновление - это есть в Дефолте
                
                Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
                string ConStr = connectionString.Return(field.DirCustomersID, null, false);

                Classes.Update.Update update = new Classes.Update.Update();
                string[] sResultawait = await update.UpdatingOne(ConStr, field.DirCustomersID); //update.Start();
                
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
                return Redirect("~/account/login?Err=" + exceptionEntry.Return(ex));
            }
        }
    }
}
