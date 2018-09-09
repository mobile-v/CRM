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
    public class AccountController : Controller
    {
        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Account.EncodeDecode encode = new Classes.Account.EncodeDecode();

        // GET: Account

        #region Index - используется для переадресации

        [HttpGet]
        public ActionResult Index()
        {
            //return View();
            return Redirect("~/account/login/");
        }

        #endregion


        #region Registration

        //Русский
        [HttpGet]
        public ActionResult Registration(int? id)
        {
            ViewData["Title"] = "Сервисный центр: регистрация";

            Models.Login.Dir.DirCustomerReg dirCustomerReg = new Models.Login.Dir.DirCustomerReg();
            dirCustomerReg.Refer = Convert.ToInt32(HttpContext.Request.QueryString["ref"]);

            return View(dirCustomerReg);
        }
        [HttpPost]
        public async Task<ActionResult> Registration(Models.Login.Dir.DirCustomerReg dirCustomerReg)
        {
            ViewData["Title"] = "Сервисный центр: регистрация";

            if (await Task.Run(() => mRegistration(dirCustomerReg, 1))) return View("RegistrationSuccess", dirCustomerReg);
            else return View(dirCustomerReg);
        }

        //Украинский
        [HttpGet]
        public ActionResult RegistrationUk(int? id)
        {
            ViewData["Title"] = "Сервисный центр: реєстрація";

            Models.Login.Dir.DirCustomerReg dirCustomerReg = new Models.Login.Dir.DirCustomerReg();
            dirCustomerReg.Refer = Convert.ToInt32(HttpContext.Request.QueryString["ref"]);

            return View(dirCustomerReg);
        }
        [HttpPost]
        public async Task<ActionResult> RegistrationUk(Models.Login.Dir.DirCustomerReg dirCustomerReg)
        {
            ViewData["Title"] = "Сервисный центр: реєстрація";

            if (await Task.Run(() => mRegistration(dirCustomerReg, 2))) return View("RegistrationSuccess", dirCustomerReg);
            else return View(dirCustomerReg);
        }

        //Английский
        [HttpGet]
        public ActionResult RegistrationEn(int? id)
        {
            ViewData["Title"] = "Сервисный центр: Registration";

            Models.Login.Dir.DirCustomerReg dirCustomerReg = new Models.Login.Dir.DirCustomerReg();
            dirCustomerReg.Refer = Convert.ToInt32(HttpContext.Request.QueryString["ref"]);

            return View(dirCustomerReg);
        }
        [HttpPost]
        public async Task<ActionResult> RegistrationEn(Models.Login.Dir.DirCustomerReg dirCustomerReg)
        {
            ViewData["Title"] = "Сервисный центр: Registration";

            if (await Task.Run(() => mRegistration(dirCustomerReg, 3))) return View("RegistrationSuccess", dirCustomerReg);
            else return View(dirCustomerReg);
        }


        private bool mRegistration(Models.Login.Dir.DirCustomerReg dirCustomerReg, int iLanguage)
        {
            //Для записи в БД, т.к. использовать "dirCustomerReg" нельзя!
            Models.Login.Dir.DirCustomer dirCustomer = new Models.Login.Dir.DirCustomer();

            if (dirCustomerReg != null && ModelState.IsValid)
            {
                try
                {
                    using (DbConnectionLogin con = new DbConnectionLogin("ConnStrMSSQL"))
                    {
                        #region Проверка
                        // на существования такого логина
                        int iCount =
                            (
                                (
                                    from dirCustomers in con.DirCustomers
                                    where dirCustomers.Login == dirCustomerReg.Login
                                    select dirCustomers.Login
                                ).Concat
                                (
                                    from dirLoginNot in con.DirLoginNot
                                    where dirLoginNot.Login == dirCustomerReg.Login
                                    select dirLoginNot.Login
                                )
                            ).Count();

                        if (iCount > 0) { con.Database.Connection.Close(); ViewData["Msg"] = Classes.Language.Login.Language.msg1(iLanguage); return false; }

                        // на существования такого реферала
                        if (dirCustomerReg.Refer != 0)
                        {
                            int iCountRefer =
                                (
                                    from DirCustomers in con.DirCustomers
                                    where dirCustomerReg.DirCustomersID == dirCustomerReg.Refer
                                    select dirCustomerReg.Login
                                ).Count();
                            if (iCountRefer == 0) dirCustomerReg.Refer = 0;
                        }
                        #endregion

                        #region Сохраняем
                        //Формирование пароля
                        Classes.Function.FunctionMSSQL.RandomSymbol randomSymbol = new Classes.Function.FunctionMSSQL.RandomSymbol();
                        dirCustomerReg.Pswd = randomSymbol.ReturnRandom(10);

                        //Переносим данные в "нужную" модель: dirCustomerReg => dirCustomer
                        dirCustomer.DirCustomersDate = DateTime.Now;
                        dirCustomer.Email = dirCustomerReg.Email;
                        dirCustomer.Login = dirCustomerReg.Login;
                        dirCustomer.DomainName = dirCustomerReg.Login;
                        dirCustomer.Pswd = dirCustomerReg.Pswd;
                        dirCustomer.Telefon = dirCustomerReg.Telefon;
                        dirCustomer.FIO = dirCustomerReg.FIO;
                        dirCustomer.DirLanguageID = dirCustomerReg.DirLanguageID;
                        dirCustomer.DirCountryID = dirCustomerReg.DirCountryID;
                        dirCustomer.Refer = dirCustomerReg.Refer;
                        dirCustomer.SendMail = false;
                        dirCustomer.Active = false;
                        dirCustomer.Pay = true;
                        dirCustomer.Confirmed = false;

                        con.Entry(dirCustomer).State = EntityState.Added;
                        try
                        {
                            con.SaveChanges();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();

                            foreach (var failure in ex.EntityValidationErrors)
                            {
                                //sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                                foreach (var error in failure.ValidationErrors)
                                {
                                    sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                                    sb.AppendLine();
                                }
                            }

                            throw new System.Data.Entity.Validation.DbEntityValidationException(
                                //"Entity Validation Failed - errors follow:\n" +
                                sb.ToString(), ex
                            ); // Add the original exception as the innerException
                        }
                        /*catch (Exception ex)
                        {
                            throw new System.InvalidOperationException(ex.Message);
                        }*/

                        #endregion
                    }

                    #region Отправка письма на E-Mail
                    try
                    {
                        //Текс на русском
                        string sSendTest = Classes.Language.Login.Language.txtRegistrationMail(dirCustomer, iLanguage);
                        Classes.Function.FunctionMSSQL.FunMailSend funMailSend = new Classes.Function.FunctionMSSQL.FunMailSend();
                        //Отправка клиенту
                        funMailSend.SendTo_OtherEMail(Classes.Language.Login.Language.msg2(iLanguage), sSendTest, dirCustomer.Email);
                        //Отправка себе
                        try { funMailSend.SendTo_ESklad24(Classes.Language.Login.Language.msg2(iLanguage), "E-Mail: " + dirCustomer.Email + ", Login: " + dirCustomer.Login + "<br />" + Classes.Language.Login.Language.msg2(iLanguage) + "<hr />" + sSendTest); } catch { }
                    }
                    catch (Exception ex)
                    {
                        ViewData["Msg"] = Classes.Language.Login.Language.msg4(iLanguage) + ex.Message + "";
                        ViewData["Title"] = Classes.Language.Login.Language.msg5(iLanguage);
                        return true;
                    }
                    #endregion

                    using (DbConnectionLogin con = new DbConnectionLogin("ConnStrMSSQL"))
                    {
                        #region Пишем в БД, что Мейл отправлен (SendMail=true)
                        dirCustomer.SendMail = true;
                        con.Entry(dirCustomer).State = EntityState.Modified;
                        con.SaveChanges();
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = exceptionEntry.Return(ex);
                    ViewData["Msg"] = Classes.Language.Login.Language.error(iLanguage) + ": <br />" + sMsg + "<br /> " + Classes.Language.Login.Language.msg6(iLanguage) + ": <a href='mailto:support@intradecloud.com' rel='nofollow'>support@intradecloud.com</a>"; return false;
                }


                ViewData["Title"] = Classes.Language.Login.Language.msg5(iLanguage);
                ViewData["Msg"] = Classes.Language.Login.Language.msg7(iLanguage);
                return true;
            }
            else { return false; }
        }

        #endregion


        #region RegConf

        [HttpGet]
        public async Task<ActionResult> RegConf(string Confirmed)
        {
            ViewData["Title"] = "Сервисный центр: подтверждение регистрации";

            if (Confirmed == null) return View();

            string[] mReg = await Task.Run(() => mRegConf(Confirmed));

            if (mReg[0] != null && mReg[0].Length > 0) Response.Redirect("/account/login/?username=admin@" + mReg[0] + "&password=" + mReg[1] + "&language=" + mReg[2] + "", true);
            else return View();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RegConf(string Confirmed, string Trash) //FormCollection formCollection
        {
            ViewData["Title"] = "Сервисный центр: подтверждение регистрации";

            if (Confirmed == "") return View();

            string[] mReg = await Task.Run(() => mRegConf(Confirmed));

            if (mReg[0] != null && mReg[0].Length > 0) Response.Redirect("/account/login/?username=admin@" + mReg[0] + "&password=" + mReg[1] + "&language=" + mReg[2] + "", true);
            else return View();

            return View();
        }

        private async Task<string[]> mRegConf(string Confirmed)
        {
            try
            {
                Classes.Account.RegConf regConf = new Classes.Account.RegConf();
                return await Task.Run(() => regConf.Confirmed(Confirmed));
            }
            catch (Exception ex)
            {
                string sMsg = exceptionEntry.Return(ex);

                ViewData["Msg"] = sMsg; // ex.Message;

                string[] ret = new string[3];
                return ret;
            }
        }

        #endregion


        #region Login

        [HttpGet]
        public async Task<ActionResult> Login() //HttpRequest request, string username
        {
            ViewData["Title"] = "Сервисный центр: вход в сервис";

            ViewData["Msg"] = Request.QueryString["Err"];
            //ViewData["Return"] = Request.QueryString["Return"];

            //0. Модель
            Models.Login.Dir.DirCustomerLogin dirCustomerLogin = new Models.Login.Dir.DirCustomerLogin();

            try
            {
                //1.1. Параметры
                //1.1.1. Логин
                dirCustomerLogin.Login = Request.QueryString["username"];
                dirCustomerLogin.Pswd = Request.QueryString["password"];
                dirCustomerLogin.DirLanguageID = Convert.ToInt32(Request.QueryString["language"]);
                dirCustomerLogin.DirThemeID = Convert.ToInt32(Request.QueryString["theme"]);
                dirCustomerLogin.DirInterfaceID = Convert.ToInt32(Request.QueryString["interface"]);
                //1.1.2. Сообщение о ошибке
                string Msg = Request.QueryString["Msg"]; if (!String.IsNullOrEmpty(Msg)) ViewData["Msg"] = Msg;
                //1.1.3. Для переадресации, например в Розницу
                string PageReturn = Request.QueryString["PageReturn"];


                //2. mLogin - Проверка Логина и Пароля
                if (await Task.Run(() => mLogin(dirCustomerLogin)))
                {
                    //2.1. Если переадресоваться, например в Розницу или интерфейс для Заказов покупателя
                    if (!String.IsNullOrEmpty(PageReturn)) { return Redirect("~/" + PageReturn + "/"); }

                    //Если доступ только в Розницу
                    //if (field.RetailOnly) return Redirect("~/Retail/");

                    //2.2. Переходим "Домой"
                    return Redirect("~/");
                }
                else
                {
                    return View(dirCustomerLogin);
                }
            }
            catch (Exception ex)
            {
                ViewData["Msg"] = exceptionEntry.Return(ex);
                return View(dirCustomerLogin);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Login(Models.Login.Dir.DirCustomerLogin dirCustomerLogin)
        {
            ViewData["Title"] = "Сервисный центр: вход в сервис";

            try
            {
                if (dirCustomerLogin != null && (ModelState.IsValid || dirCustomerLogin.Pswd2.Length > 0))
                {
                    if(!String.IsNullOrEmpty(dirCustomerLogin.Pswd2)) dirCustomerLogin.Pswd = dirCustomerLogin.Pswd2;

                    if (await Task.Run(() => mLogin(dirCustomerLogin)))
                    {
                        //Если доступ только в Розницу
                        //if (field.RetailOnly) return Redirect("~/Retail/");
                        //Иначе - в основной сервис
                        return Redirect("~/");
                    }
                    else
                    {
                        ViewData["Msg"] = Classes.Language.Sklad.Language.msg4;
                        return View(dirCustomerLogin);
                    }
                }
                else { return View(dirCustomerLogin); }
            }
            catch (Exception ex)
            {
                ViewData["Msg"] = exceptionEntry.Return(ex);
                return View(dirCustomerLogin);
            }
        }

        Classes.Account.Login.Field field = new Classes.Account.Login.Field();
        private bool mLogin(Models.Login.Dir.DirCustomerLogin dirCustomerLogin)
        {
            HttpCookie CookieIPOL = null;
            if (!String.IsNullOrEmpty(dirCustomerLogin.Login) && !String.IsNullOrEmpty(dirCustomerLogin.Pswd))
            {

                #region 1. Проверяем Логин и Пароль

                Classes.Account.Login login = new Classes.Account.Login();
                field = login.Return(dirCustomerLogin.Login, dirCustomerLogin.Pswd, false);
                if (!field.Access) { return false; }

                #endregion


                #region Если выбрали упращённый режим входа

                if (dirCustomerLogin.DirLanguageID == 0) dirCustomerLogin.DirLanguageID = 1;
                if (dirCustomerLogin.DirThemeID == 0) dirCustomerLogin.DirThemeID = 1;
                if (dirCustomerLogin.DirInterfaceID == 0) dirCustomerLogin.DirInterfaceID = 2;

                #endregion


                #region 2. Создаём Куку

                CookieIPOL = new HttpCookie("CookieIPOL");
                CookieIPOL["CookieU"] = encode.UnionEncode(dirCustomerLogin.Login);
                CookieIPOL["CookieP"] = encode.UnionEncode(dirCustomerLogin.Pswd);
                CookieIPOL["CookieL"] = dirCustomerLogin.DirLanguageID.ToString();
                CookieIPOL["CookieT"] = dirCustomerLogin.DirThemeID.ToString();
                CookieIPOL["CookieI"] = dirCustomerLogin.DirInterfaceID.ToString();
                Response.Cookies.Add(CookieIPOL);

                #endregion


                #region 3. Пишем в БД о посещении (только Comercial)

                Classes.Function.FunctionMSSQL.Jurn.JurnDispLogining jurnDispLogining = new Classes.Function.FunctionMSSQL.Jurn.JurnDispLogining();
                jurnDispLogining.Write(field.DirCustomersID, dirCustomerLogin.Login, dirCustomerLogin.Pswd, "Browser: " + Request.Browser.Browser + ". UserAgent: " + Request.UserAgent);

                #endregion


                #region 4. Обновление - это есть в Дефолте

                Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
                string ConStr = connectionString.Return(field.DirCustomersID, null, false);

                Classes.Update.Update update = new Classes.Update.Update();
                update.UpdatingOne(ConStr, field.DirCustomersID); //update.Start();

                #endregion


                return true;
            }

            return false;
        }

        #endregion


        #region PassRemind

        [HttpGet]
        public ActionResult PassRemind()
        {
            ViewData["Title"] = "Сервисный центр: напоминание пароля";
            ViewData["Visible"] = true;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PassRemind(string EMail) //FormCollection formCollection
        {
            ViewData["Title"] = "Сервисный центр: напоминание пароля";

            if (EMail == "") return View();

            Classes.Account.PassRemind passRemind = await Task.Run(() => mPassRemind(EMail));

            if (passRemind.CountRecord == 0) ViewData["Msg"] = "Такой Е-Майл адрес не найден в Базе Данных!<br /> Попробуйде ещё раз (у вас ещё 4 попытки)!";
            else
            {
                if (ViewData["Msg"] == null)
                {
                    string GMailCom = "";
                    if (EMail.IndexOf("@gmail.com") != -1) { GMailCom = "<br /><b style='color:Red'>Внимание! Почта <<< GMAIL.COM >>> по неизвестным причинам заносит письма в спам!</b><br />(проверьте, пожалуйста папку 'Спам')"; }

                    ViewData["Msg"] = "Данные отправлены Вам на Е-Майл!" + GMailCom;
                    ViewData["Visible"] = false;
                }
            }

            return View();
        }

        private Classes.Account.PassRemind mPassRemind(string EMail)
        {
            Classes.Account.PassRemind passRemind = new Classes.Account.PassRemind();
            try
            {
                passRemind.ConfirmAndSendMail(EMail);
            }
            catch (Exception ex)
            {
                ViewData["Msg"] = ex.Message;
            }

            return passRemind;
        }

        #endregion
    }
}