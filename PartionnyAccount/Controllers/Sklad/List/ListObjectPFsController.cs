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
using PartionnyAccount.Models.Sklad.List;
using System.Web.Script.Serialization;
using System.Data.SQLite;

namespace PartionnyAccount.Controllers.Sklad.List
{
    public class ListObjectPFsController : ApiController
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

        int ListObjectID = 31;

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
            public int ListObjectID = 0;
            public int? ListObjectPFID = 0;
        }
        // GET: api/ListObjectPFs
        public async Task<IHttpActionResult> GetListObjectPFs(HttpRequestMessage request)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDevelop"));
                //if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                

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
                _params.ListObjectID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectID", true) == 0).Value);
                _params.ListObjectPFID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectPFID", true) == 0).Value);

                #endregion


                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.ListObjectPFs
                        where x.ListObjectID == _params.ListObjectID
                        select new
                        {
                            ListObjectPFID = x.ListObjectPFID,
                            Del = x.Del,
                            ListLanguageID = x.ListLanguageID,
                            ListLanguageNameSmall = x.listLanguage.ListLanguageNameSmall,
                            ListLanguageName = x.listLanguage.ListLanguageName,
                            ListObjectPFName = x.ListObjectPFName,
                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region ListObjectPFID - ПФ в СС

                if (_params.ListObjectPFID > 0)
                {
                    query = query.Where(x => x.ListObjectPFID == _params.ListObjectPFID);
                }

                #endregion


                #region Поиск

                if (!String.IsNullOrEmpty(_params.parSearch))
                {
                    //Проверяем число ли это
                    Int32 iNumber32;
                    bool bResult32 = Int32.TryParse(_params.parSearch, out iNumber32);


                    //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                    if (bResult32)
                    {
                        query = query.Where(x => x.ListObjectPFID == iNumber32 || x.ListObjectPFName.Contains(_params.parSearch));
                    }
                    else
                    {
                        query = query.Where(x => x.ListObjectPFName.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                //query = query.OrderBy(x => x.ListObjectPFName).Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                /*
                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.ListObjectPFs.Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;
                */

                int dirCount = query.Count();

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    ListObjectPF = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/ListObjectPFs/5
        [ResponseType(typeof(ListObjectPF))]
        public async Task<IHttpActionResult> GetListObjectPF(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDevelop"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                //Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        from x in db.ListObjectPFs
                        where x.ListObjectPFID == id
                        select new
                        {
                            ListObjectPFID = x.ListObjectPFID,
                            ListObjectPFIDSys = x.ListObjectPFIDSys,
                            Del = x.Del,
                            SysRecord = x.SysRecord,
                            ListObjectID = x.ListObjectID,
                            ListLanguageID = x.ListLanguageID,

                            ListObjectPFHtmlCSSUse = x.ListObjectPFHtmlCSSUse,
                            ListObjectPFSys = x.ListObjectPFSys,
                            ListObjectPFName = x.ListObjectPFName,
                            ListObjectPFHtmlHeaderUse = x.ListObjectPFHtmlHeaderUse,
                            ListObjectPFHtmlDouble = x.ListObjectPFHtmlDouble,
                            ListObjectPFHtmlHeader = x.ListObjectPFHtmlHeader,
                            ListObjectPFHtmlTabUseCap = x.ListObjectPFHtmlTabUseCap,
                            ListObjectPFHtmlTabCap = x.ListObjectPFHtmlTabCap,
                            ListObjectPFHtmlTabUseTab = x.ListObjectPFHtmlTabUseTab,
                            ListObjectPFHtmlTabEnumerate = x.ListObjectPFHtmlTabEnumerate,
                            ListObjectPFHtmlTabFont = x.ListObjectPFHtmlTabFont,
                            ListObjectPFHtmlTabFontSize = x.ListObjectPFHtmlTabFontSize,
                            ListObjectPFHtmlTabUseFooter = x.ListObjectPFHtmlTabUseFooter,
                            ListObjectPFHtmlTabFooter = x.ListObjectPFHtmlTabFooter,
                            ListObjectPFHtmlTabUseText = x.ListObjectPFHtmlTabUseText,
                            ListObjectPFHtmlTabText = x.ListObjectPFHtmlTabText,
                            ListObjectPFHtmlFooterUse = x.ListObjectPFHtmlFooterUse,
                            ListObjectPFHtmlFooter = x.ListObjectPFHtmlFooter,
                            ListObjectPFDesc = x.ListObjectPFDesc,
                            //Отступы
                            MarginTop = x.MarginTop,        //верх
                            MarginBottom = x.MarginBottom,  //низ
                            MarginLeft = x.MarginLeft,      //слева
                            MarginRight = x.MarginRight,    //справа
                        }
                    ).ToListAsync());


                if (query.Count() > 0)
                {
                    return Ok(returnServer.Return(true, query[0]));
                }
                else
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));
                }

                //return Ok(returnServer.Return(false, Classes.Language.Language.msg89));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/ListObjectPFs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutListObjectPF(int id, ListObjectPF listObjectPF, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDevelop"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Если НЕ ЛокалХост и меньше 1 000 000 - Эксепшен
                //if (request.RequestUri.DnsSafeHost != "localhost" && listObjectPF.ListObjectPFID < 1000000) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg60));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                //Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
                if (UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
                UO_Action = UO_Action.ToLower();

                //Получаем колекцию "Спецификации"
                Models.Sklad.List.ListObjectPFTab[] listObjectPFTabCollection = null;
                if (!String.IsNullOrEmpty(listObjectPF.recordsListObjectPFTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    listObjectPFTabCollection = serializer.Deserialize<Models.Sklad.List.ListObjectPFTab[]>(listObjectPF.recordsListObjectPFTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != listObjectPF.ListObjectPFID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                listObjectPF.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    //Используем метод, что бы было всё в одном потоке
                    listObjectPF = await Task.Run(() => mPutPostListObjectPF(db, UO_Action, listObjectPF, EntityState.Modified, listObjectPFTabCollection, field, request)); //sysSetting

                    ts.Commit(); //.Complete();
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = listObjectPF.ListObjectPFID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ListObjectPFID = listObjectPF.ListObjectPFID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/ListObjectPFs
        [ResponseType(typeof(ListObjectPF))]
        public async Task<IHttpActionResult> PostListObjectPF(ListObjectPF listObjectPF, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDevelop"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
                if (UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
                UO_Action = UO_Action.ToLower();

                //Получаем колекцию "Спецификации"
                Models.Sklad.List.ListObjectPFTab[] listObjectPFTabCollection = null;
                if (!String.IsNullOrEmpty(listObjectPF.recordsListObjectPFTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    listObjectPFTabCollection = serializer.Deserialize<Models.Sklad.List.ListObjectPFTab[]>(listObjectPF.recordsListObjectPFTab);
                }

                #endregion

                #region Проверки

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                listObjectPF.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    //Используем метод, что бы было всё в одном потоке
                    listObjectPF = await Task.Run(() => mPutPostListObjectPF(db, UO_Action, listObjectPF, EntityState.Added, listObjectPFTabCollection, field, request)); //sysSetting

                    ts.Commit(); //.Complete();
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = listObjectPF.ListObjectPFID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ListObjectPFID = listObjectPF.ListObjectPFID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/ListObjectPFs/5
        [ResponseType(typeof(ListObjectPF))]
        public async Task<IHttpActionResult> DeleteListObjectPF(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDevelop"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Проверки

                if (id < 1000000) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg40)); }

                #endregion


                #region Удаление

                //Алгоритм.
                //Удаляем по порядку:
                //2. ListObjectPFTabs
                //3. ListObjectPFs


                //ПФ
                Models.Sklad.List.ListObjectPF listObjectPF = await db.ListObjectPFs.FindAsync(id);
                if (listObjectPF == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 2. ListObjectPFTabs *** *** *** *** ***

                        var queryListObjectPFTabs = await
                            (
                                from x in db.ListObjectPFTabs
                                where x.ListObjectPFID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryListObjectPFTabs.Count(); i++)
                        {
                            Models.Sklad.List.ListObjectPFTab listObjectPFTab = await db.ListObjectPFTabs.FindAsync(queryListObjectPFTabs[i].ListObjectPFTabID);
                            db.ListObjectPFTabs.Remove(listObjectPFTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. ListObjectPFs *** *** *** *** ***

                        /*
                        var queryListObjectPFs = await
                            (
                                from x in db.ListObjectPFs
                                where x.ListObjectPFID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryListObjectPFs.Count(); i++)
                        {
                            Models.Sklad.List.ListObjectPF listObjectPF1 = await db.ListObjectPFs.FindAsync(queryListObjectPFs[i].ListObjectPFID);
                            db.ListObjectPFs.Remove(listObjectPF1);
                            await db.SaveChangesAsync();
                        }
                        */

                        db.ListObjectPFs.Remove(listObjectPF);
                        await db.SaveChangesAsync();

                        #endregion


                        ts.Commit();


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
                            ID = id,
                            Msg = Classes.Language.Sklad.Language.msg19
                        };
                        return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
                    }
                    catch (Exception ex)
                    {
                        ts.Rollback();
                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    } //catch

                } //DbContextTransaction

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
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

        private bool ListObjectPFExists(int id)
        {
            return db.ListObjectPFs.Count(e => e.ListObjectPFID == id) > 0;
        }


        private async Task<bool> MoreMillion(DbConnectionSklad db)
        {
            var query = await Task.Run(() =>
                (
                    from x in db.ListObjectPFs
                    select x
                ).CountAsync());
            if (query >= 1000000) return true;

            return false;
        }

        internal async Task<ListObjectPF> mPutPostListObjectPF(
            DbConnectionSklad db,
            string UO_Action,
            ListObjectPF listObjectPF,
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.List.ListObjectPFTab[] listObjectPFTabCollection,

            Classes.Account.Login.Field field, //Дополнительные данные о сотруднике
            HttpRequestMessage request //Информация о хосте
            )
        {
            //Алгоритм
            //1. Если писали с ЛокалХоста, то не проверяем на 1 000 000 записей
            //   Если писали НЕ ЛокалХоста, то проверяем (см ДокументМенеджент)
            //2. Сохраняем Шапку
            //3. Сохраняем Таблицу, если не пустая


            //1. Если меньше Миллиона записей и не Я редактирую, то это первая запись ПФ созданная пользователем, присваеваем её = 1 000 000
            if (!await Task.Run(() => MoreMillion(db)) && request.RequestUri.DnsSafeHost != "localhost" && entityState == EntityState.Added)
            {
                //listObjectPF.ListObjectPFID = 1000000;

                db.Entry(listObjectPF).State = entityState;
                await db.SaveChangesAsync();

                SQLiteParameter parDocPurchID1 = new SQLiteParameter("@ListObjectPFID1", System.Data.DbType.Int32) { Value = 1000000 };
                SQLiteParameter parDocPurchID2 = new SQLiteParameter("@ListObjectPFID2", System.Data.DbType.Int32) { Value = listObjectPF.ListObjectPFID };
                db.Database.ExecuteSqlCommand("UPDATE ListObjectPFs SET ListObjectPFID=@ListObjectPFID1 WHERE ListObjectPFID=@ListObjectPFID2;", parDocPurchID1, parDocPurchID2);

                listObjectPF.ListObjectPFID = 1000000;
            }
            else
            {
                //2. ListObjectPF
                db.Entry(listObjectPF).State = entityState;
                await db.SaveChangesAsync();
            }

            //3. listObjectPFTabCollection
            //3.1. Удаляем записи в БД, если UPDATE
            if (entityState == EntityState.Modified)
            {
                SQLiteParameter parDocPurchID = new SQLiteParameter("@ListObjectPFID", System.Data.DbType.Int32) { Value = listObjectPF.ListObjectPFID };
                db.Database.ExecuteSqlCommand("DELETE FROM ListObjectPFTabs WHERE ListObjectPFID=@ListObjectPFID;", parDocPurchID);
            }
            //3.2. Проставляем ID-шник "ListObjectPFID" для всех позиций спецификации
            for (int i = 0; i < listObjectPFTabCollection.Count(); i++)
            {
                listObjectPFTabCollection[i].ListObjectPFTabID = null;
                listObjectPFTabCollection[i].ListObjectPFID = Convert.ToInt32(listObjectPF.ListObjectPFID);
                db.Entry(listObjectPFTabCollection[i]).State = EntityState.Added;
            }
            await db.SaveChangesAsync();

            return listObjectPF;
        }

        #endregion
    }
}