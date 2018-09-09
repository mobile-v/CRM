using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using PartionnyAccount.Models;
using PartionnyAccount.Models.Sklad.Service.IM;

namespace PartionnyAccount.Controllers.Sklad.Service.IM
{
    public class DirWebShopUOsController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 75;

        #endregion


        #region SELECT

        class Params
        {
            //Grid
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;

            //Tree
            public string node = "";
            public int? XGroupID_NotShow = 0;

            //Other
            public string type = "Grid";
            public string parSearch = "";
        }
        // GET: api/DirWebShopUOs
        public async Task<IHttpActionResult> GetDirWebShopUOs(HttpRequestMessage request)
        {
            try
            {
                #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

                //Получаем Куку
                System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

                // Проверяем Логин и Пароль
                Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
                if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

                //Изменяем строку соединения
                db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWebShopUOs"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.limit = 999999; // sysSetting.PageSizeDir; //Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.type = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "type", true) == 0).Value;
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                if (_params.type == "Grid")
                {
                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from dirWebShopUOs in db.DirWebShopUOs
                            select new
                            {
                                DirWebShopUOID = dirWebShopUOs.DirWebShopUOID,
                                DirWebShopUOName = dirWebShopUOs.DirWebShopUOName,
                            }
                        );

                    #endregion
                    
                    #region Отправка JSON

                    //К-во Номенклатуры
                    int dirCount = await Task.Run(() => db.DirWebShopUOs.Count());
                    
                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirWebShopUO = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    #region Отобразить только "Руты" *** *** ***

                    var query =
                        (
                         from x in db.DirWebShopUOs
                         select new
                         {
                             id = x.DirWebShopUOID,
                             text = x.DirWebShopUOName,
                             leaf = true,
                             Del = x.Del
                         }
                        );

                    #endregion


                    #region Отправка JSON

                    //return Ok(await Task.Run(() => query));

                    dynamic collectionWrapper = new
                    {
                        query
                    };
                    return Ok(await Task.Run(() => collectionWrapper));

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DirWebShopUOs/5
        [ResponseType(typeof(DirWebShopUO))]
        public async Task<IHttpActionResult> GetDirWebShopUO(int id, HttpRequestMessage request)
        {
            try
            {
                #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

                //Получаем Куку
                System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

                // Проверяем Логин и Пароль
                Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
                if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

                //Изменяем строку соединения
                db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права
                /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DirWebShopUO"));
                if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWebShopUOs"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                #region Отправка JSON

                string DomainName = await SelectDomainName(field);

                var query = await Task.Run(() =>
                    (

                        from x in db.DirWebShopUOs

                        where x.DirWebShopUOID == id

                        select
                        new
                        {
                            //1. Оснавные === === === === === === === === === === ===

                            DirWebShopUOID = x.DirWebShopUOID,
                            Del = x.Del,
                            DirWebShopUOName = x.DirWebShopUOName,

                            DomainName = DomainName,

                            Nomen_DirPriceTypeID = x.Nomen_DirPriceTypeID,
                            Nomen_Remains = x.Nomen_Remains,
                            DirCurrencyID = x.DirCurrencyID,
                            Orders_Doc_DirOrdersStateID = x.Orders_Doc_DirOrdersStateID,
                            Orders_Nomen_DirOrdersStateID = x.Orders_Nomen_DirOrdersStateID,
                            Orders_DirWarehouseID = x.Orders_DirWarehouseID,
                            Orders_DirContractorIDOrg = x.Orders_DirContractorIDOrg,
                            Orders_DirContractorID = x.Orders_DirContractorID,
                            Orders_Reserve = x.Orders_Reserve,


                            Slider_Quantity = x.Slider_Quantity,

                            //1.
                            Slider_DirNomen1ID = x.Slider_DirNomen1ID,
                            //Slider_DirNomen1ID_Img = x.Slider_DirNomen1ID_Img,
                            SysGen1ID = x.SysGen1ID,
                            SysGen1IDPatch = x.SysGen1ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen1ID + ".jpg",

                            Slider_DirNomen2ID = x.Slider_DirNomen2ID,
                            //Slider_DirNomen2ID_Img = x.Slider_DirNomen2ID_Img,
                            SysGen2ID = x.SysGen2ID,
                            SysGen2IDPatch = x.SysGen2ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen2ID + ".jpg",

                            Slider_DirNomen3ID = x.Slider_DirNomen3ID,
                            //Slider_DirNomen3ID_Img = x.Slider_DirNomen3ID_Img,
                            SysGen3ID = x.SysGen3ID,
                            SysGen3IDPatch = x.SysGen3ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen3ID + ".jpg",

                            Slider_DirNomen4ID = x.Slider_DirNomen4ID,
                            //Slider_DirNomen4ID_Img = x.Slider_DirNomen4ID_Img,
                            SysGen4ID = x.SysGen4ID,
                            SysGen4IDPatch = x.SysGen4ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen4ID + ".jpg",


                            Recommended_Quantity = x.Recommended_Quantity,
                            Recommended_DirNomen1ID = x.Recommended_DirNomen1ID,
                            Recommended_DirNomen2ID = x.Recommended_DirNomen2ID,
                            Recommended_DirNomen3ID = x.Recommended_DirNomen3ID,
                            Recommended_DirNomen4ID = x.Recommended_DirNomen4ID,
                            Recommended_DirNomen5ID = x.Recommended_DirNomen5ID,
                            Recommended_DirNomen6ID = x.Recommended_DirNomen6ID,
                            Recommended_DirNomen7ID = x.Recommended_DirNomen7ID,
                            Recommended_DirNomen8ID = x.Recommended_DirNomen8ID,
                            Recommended_DirNomen9ID = x.Recommended_DirNomen9ID,
                            Recommended_DirNomen10ID = x.Recommended_DirNomen10ID,
                            Recommended_DirNomen11ID = x.Recommended_DirNomen11ID,
                            Recommended_DirNomen12ID = x.Recommended_DirNomen12ID,
                            Recommended_DirNomen13ID = x.Recommended_DirNomen13ID,
                            Recommended_DirNomen15ID = x.Recommended_DirNomen15ID,
                            Recommended_DirNomen16ID = x.Recommended_DirNomen16ID,

                            //HTML
                            Payment = x.Payment,
                            AboutUs = x.AboutUs,
                            DeliveryInformation = x.DeliveryInformation,
                            PrivacyPolicy = x.PrivacyPolicy,
                            TermsConditions = x.TermsConditions,
                            ContactUs = x.ContactUs,
                            Returns = x.Returns,
                            SiteMap = x.SiteMap,
                            Affiliate = x.Affiliate,
                            Specials = x.Specials,

                            DirNomenGroup_Top = x.DirNomenGroup_Top,


                            x,

                        }

                    ).ToListAsync());


                if (query.Count() > 0)
                {
                    return Ok(returnServer.Return(true, query[0]));
                }
                else
                {
                    return Ok(returnServer.Return(false, DomainName));
                }

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }
        //Получаем DomainName из базы MS SQL
        private async Task<string> SelectDomainName(Classes.Account.Login.Field field)
        {
            string DomainName = "";
            using (Models.DbConnectionLogin dbLogin = new Models.DbConnectionLogin("ConnStrMSSQL"))
            {
                var query = await dbLogin.DirCustomers.Where(x => x.DirCustomersID == field.DirCustomersID && x.Active == true).ToListAsync();
                if (query.Count() > 0)
                {
                    DomainName = query[0].DomainName;
                }
                else
                {
                    DomainName = "Не найден клиент или не активный!";
                }
            }

            return DomainName;
        }

        #endregion


        #region UPDATE

        // PUT: api/DirWebShopUOs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirWebShopUO(int id, DirWebShopUO dirWebShopUO)
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));
            dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWebShopUOs"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirWebShopUO.DirWebShopUOID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //dirWebShopUO.Substitute();

            if (!dirWebShopUO.Nomen_Remains == null) { dirWebShopUO.Nomen_Remains = false; }
            if (!dirWebShopUO.Orders_Reserve == null) { dirWebShopUO.Orders_Reserve = false; }

            #endregion


            #region Сохранение

            try
            {
                db.Entry(dirWebShopUO).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());


                //Сохраняем Доменное имя в МС Скул
                using (DbConnectionLogin con = new DbConnectionLogin("ConnStrMSSQL"))
                {
                    Models.Login.Dir.DirCustomer dirCustomer = await con.DirCustomers.FindAsync(field.DirCustomersID);
                    dirCustomer.DomainName = dirWebShopUO.DomainName;
                    con.Entry(dirCustomer).State = EntityState.Modified;
                    con.SaveChanges();
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirWebShopUO.DirWebShopUOID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirWebShopUO.DirWebShopUOID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/DirWebShopUOs
        [ResponseType(typeof(DirWebShopUO))]
        public async Task<IHttpActionResult> PostDirWebShopUO(DirWebShopUO dirWebShopUO)
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWebShopUOs"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            if (!dirWebShopUO.Nomen_Remains == null) { dirWebShopUO.Nomen_Remains = false; }
            if (!dirWebShopUO.Orders_Reserve == null) { dirWebShopUO.Orders_Reserve = false; }


            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //dirWebShopUO.Substitute();

            #endregion


            #region Сохранение

            try
            {
                db.Entry(dirWebShopUO).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());


                //Сохраняем Доменное имя в МС Скул
                using (DbConnectionLogin con = new DbConnectionLogin("ConnStrMSSQL"))
                {
                    Models.Login.Dir.DirCustomer dirCustomer = await db.DirCustomers.FindAsync(field.DirCustomersID);
                    dirCustomer.DomainName = dirWebShopUO.DomainName;
                    con.Entry(dirCustomer).State = EntityState.Modified;
                    con.SaveChanges();
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirWebShopUO.DirWebShopUOID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirWebShopUO.DirWebShopUOID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DirWebShopUOs/5
        [ResponseType(typeof(DirWebShopUO))]
        public async Task<IHttpActionResult> DeleteDirWebShopUO(int id)
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWebShopUOs"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DirWebShopUO dirWebShopUO = await db.DirWebShopUOs.FindAsync(id);
                if (dirWebShopUO == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                db.DirWebShopUOs.Remove(dirWebShopUO);
                await db.SaveChangesAsync();


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 5; //Удаление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = id;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirWebShopUO.DirWebShopUOID,
                    Msg = Classes.Language.Sklad.Language.msg19
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, "")
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        #endregion


        #region Mthods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DirWebShopUOExists(int id)
        {
            return db.DirWebShopUOs.Count(e => e.DirWebShopUOID == id) > 0;
        }

        #endregion
    }
}