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
using PartionnyAccount.Models.Sklad.Dir;
using System.Data.SQLite;

namespace PartionnyAccount.Controllers.Sklad.Dir.DirCurrencies
{
    public class DirCurrenciesController : ApiController
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

        int ListObjectID = 20;

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
            public DateTime? HistoryDate;  //public DateTime? HistoryDate;
            public string parSearch = "";
        }
        // GET: api/DirCurrencies
        public async Task<IHttpActionResult> GetDirCurrencies(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCurrencies"));
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
                //_params.HistoryDate = Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "HistoryDate", true) == 0).Value); if (_params.HistoryDate < Convert.ToDateTime("01.01.1800")) _params.HistoryDate = DateTime.Now;
                _params.HistoryDate = Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "HistoryDate", true) == 0).Value); if (_params.HistoryDate == null || _params.HistoryDate < Convert.ToDateTime("01.01.1800")) _params.HistoryDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")); _params.HistoryDate = function.ReturnHistorytDate(_params.HistoryDate); //Кликнули по "Поменять" (дату)
                //_params.GroupID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pGroupID", true) == 0).Value); //Кликнули по группе
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                if (_params.type == "Grid")
                {

                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from x in db.DirCurrencies
                            select new
                            {
                                DirCurrencyID = x.DirCurrencyID,
                                Del = x.Del,
                                SysRecord = x.SysRecord,
                                DirCurrencyName = x.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",
                                DirCurrencyNameShort = x.DirCurrencyNameShort,
                                DirCurrencyRate = x.DirCurrencyRate,
                                DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            }
                        );

                    #endregion


                    #region Условия (параметры) *** *** ***


                    #region Не показывать удалённые

                    if (!Convert.ToBoolean(sysSetting.DeletedRecordsShow))
                    {
                        query = query.Where(x => x.Del == sysSetting.DeletedRecordsShow);
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
                            query = query.Where(x => x.DirCurrencyID == iNumber32 || x.DirCurrencyName.Contains(_params.parSearch));
                        }
                        else
                        {
                            query = query.Where(x => x.DirCurrencyName.Contains(_params.parSearch));
                        }
                    }

                    #endregion


                    #region OrderBy и Лимит

                    //query = query.OrderBy(x => x.DirCurrencyName).Skip(_params.Skip).Take(_params.limit);

                    #endregion


                    #endregion


                    #region Отправка JSON

                    //К-во Номенклатуры
                    int dirCount = await Task.Run(() => db.DirCurrencies.Count());

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount2 = query.Count();
                    //if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirCurrency = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    #region Отобразить только "Руты" *** *** ***

                    var query =
                        (
                         from dirCurrencies in db.DirCurrencies
                         select new
                         {
                             id = dirCurrencies.DirCurrencyID,
                             sub = 0,
                             text = dirCurrencies.DirCurrencyName,
                             leaf = true,
                             Del = dirCurrencies.Del
                         }
                        );

                    #endregion


                    #region Условия (параметры) *** *** ***


                    #region Не показывать удалённые

                    if (!Convert.ToBoolean(sysSetting.DeletedRecordsShow))
                    {
                        query = query.Where(x => x.Del == sysSetting.DeletedRecordsShow);
                    }

                    #endregion


                    #region OrderBy и Лимит

                    //query = query.OrderBy(x => x.DirCurrencyName).Skip(_params.Skip).Take(_params.limit);

                    #endregion


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

        // GET: api/DirCurrencies/5
        [ResponseType(typeof(DirCurrency))]
        public async Task<IHttpActionResult> GetDirCurrency(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCurrencies"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                #endregion


                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.HistoryDate = Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "HistoryDate", true) == 0).Value); if (_params.HistoryDate == null || _params.HistoryDate < Convert.ToDateTime("01.01.1800")) _params.HistoryDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00")); _params.HistoryDate = function.ReturnHistorytDate(_params.HistoryDate); //Кликнули по "Поменять" (дату)
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                #region Основной запрос *** *** ***

                var query = await Task.Run(() =>
                    (

                        from x in db.DirCurrencies

                        where x.DirCurrencyID == id

                        select new
                        {
                            //1. Оснавные === === === === === === === === === === ===

                            DirCurrencyID = x.DirCurrencyID,
                            Del = x.Del,
                            SysRecord = x.SysRecord,
                            DirCurrencyCode = x.DirCurrencyCode,
                            DirCurrencyNameShort = x.DirCurrencyNameShort,
                            DirCurrencyName = x.DirCurrencyName,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity
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

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/DirCurrencies/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirCurrency(int id, DirCurrency dirCurrency)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCurrencies"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirCurrency.DirCurrencyID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //dirCurrency.Substitute();

            //Дополнительные проверки
            /*if (Convert.ToBoolean(dirCurrency.DirCurrencyActive))
            {
                //Уникальность Логина
                if (ExistLogin(dirCurrency)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg81));

                //Корректность Логина
                if (!CorrectLogin(dirCurrency)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg86));
            }*/

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                await Task.Run(() => mPutPostDirCurrency(dirCurrency, EntityState.Modified, field));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirCurrency.DirCurrencyID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirCurrency.DirCurrencyID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/DirCurrencies
        [ResponseType(typeof(DirCurrency))]
        public async Task<IHttpActionResult> PostDirCurrency(DirCurrency dirCurrency)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCurrencies"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //dirCurrency.Substitute();

            //Дополнительные проверки
            /*if (Convert.ToBoolean(dirCurrency.DirCurrencyActive))
            {
                //Уникальность Логина
                if (ExistLogin(dirCurrency)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg81));

                //Корректность Логина
                if (!CorrectLogin(dirCurrency)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg86));
            }*/

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                await Task.Run(() => mPutPostDirCurrency(dirCurrency, EntityState.Added, field));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirCurrency.DirCurrencyID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirCurrency.DirCurrencyID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DirCurrencies/5
        [ResponseType(typeof(DirCurrency))]
        public async Task<IHttpActionResult> DeleteDirCurrency(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCurrencies"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                //История Сотрудника
                var query = await
                    (
                        from x in db.DirCurrencyHistories
                        where x.DirCurrencyID == id
                        select x
                    ).ToListAsync();


                //Сотрудник
                Models.Sklad.Dir.DirCurrency dirCurrency = await db.DirCurrencies.FindAsync(id);
                if (dirCurrency == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));
                if (dirCurrency.SysRecord) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg98));

                if (!dirCurrency.Del)
                {
                    // === Удаляем === === === === === 


                    #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                    Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                    sysJourDisp.DirDispOperationID = 5; //Удаление записи
                    sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                    sysJourDisp.ListObjectID = ListObjectID;
                    sysJourDisp.TableFieldID = dirCurrency.DirCurrencyID;
                    sysJourDisp.Description = "";
                    try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                    #endregion


                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        //1. Сотрудника
                        try
                        {
                            //1. Историю Сотрудника
                            for (int i = 0; i < query.Count(); i++)
                            {
                                Models.Sklad.Dir.DirCurrencyHistory dirCurrencyHistory = await db.DirCurrencyHistories.FindAsync(query[i].DirCurrencyHistoryID);
                                db.DirCurrencyHistories.Remove(dirCurrencyHistory);
                                await db.SaveChangesAsync();
                            }

                            db.DirCurrencies.Remove(dirCurrency);
                            await db.SaveChangesAsync();

                            ts.Commit();

                            dynamic collectionWrapper = new
                            {
                                ID = dirCurrency.DirCurrencyID,
                                Msg = Classes.Language.Sklad.Language.msg19
                            };
                            return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
                        }
                        catch (Exception ex)
                        {
                            ts.Rollback();
                            if (function.ExceptionFkExist(ex))
                            {
                                //2. Исключение - пометка на удаление
                                dirCurrency.Del = true;

                                db.Entry(dirCurrency).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                                ts.Commit();

                                dynamic collectionWrapper = new
                                {
                                    ID = dirCurrency.DirCurrencyID,
                                    Msg = Classes.Language.Sklad.Language.msg96 //"Помечено на удаление, так как запись задействована в других объектах сервиса (напр. в документах)."
                                };
                                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
                            }
                            else
                            {
                                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                            }
                        } //catch

                    } //DbContextTransaction
                }
                else
                {
                    // === Снимаем пометку на удаление === === === === === 

                    dirCurrency.Del = false;

                    db.Entry(dirCurrency).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    dynamic collectionWrapper = new
                    {
                        ID = dirCurrency.DirCurrencyID,
                        Msg = Classes.Language.Sklad.Language.msg97 //"Пометка на удаление снята."
                    };
                    return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
                }
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

        private bool DirCurrencyExists(int id)
        {
            return db.DirCurrencies.Count(e => e.DirCurrencyID == id) > 0;
        }


        //Сохранение
        private void mPutPostDirCurrency(
            DirCurrency dirCurrency,
            EntityState entityState, //EntityState.Added, Modified

            Classes.Account.Login.Field field //Для "ExistPay"
            )
        {
            using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
            {
                #region 1. DirCurrency

                db.Entry(dirCurrency).State = entityState;
                db.SaveChanges();

                #endregion


                #region 2. DirCurrencyHistory

                //Получаем историю
                DirCurrencyHistory dirCurrencyHistory = new DirCurrencyHistory();
                dirCurrencyHistory.DirCurrencyID = Convert.ToInt32(dirCurrency.DirCurrencyID);
                dirCurrencyHistory.HistoryDate = DateTime.Now;
                dirCurrencyHistory.DirCurrencyID = dirCurrency.DirCurrencyID;
                dirCurrencyHistory.DirCurrencyRate = dirCurrency.DirCurrencyRate;
                dirCurrencyHistory.DirCurrencyMultiplicity = dirCurrency.DirCurrencyMultiplicity;


                //Алгоритм: 
                // - Если не найдена, то создаём сразу запись, с датой = "1800-01-01", т.к. это первая запис в истории


                //1. Ищим дату самую блискую снизу вверх (Максимум) к дате "dirCurrencyHistory.DirCurrencyHistoryDate"
                //   Возмоно 2-а варианта: "есть дата" и "нет даты"
                var queryMax =
                    db.DirCurrencyHistories.
                    Where(x => x.DirCurrencyID == dirCurrency.DirCurrencyID).
                    GroupBy(g => new { g.HistoryDate, g.DirCurrencyHistoryID, g.DirCurrencyID, g.DirCurrencyRate, g.DirCurrencyMultiplicity }).
                    Where(grp => grp.Max(m => m.HistoryDate) <= dirCurrencyHistory.HistoryDate).
                    Select(x => x.Key).OrderByDescending(o => o.HistoryDate).FirstOrDefault();

                //var queryMax = queryMax1.ToList();

                //if (queryMax.Count() == 0)
                if (queryMax == null)
                {
                    //2. Нет данных: "INSERT", создаем новую запись в Истории
                    dirCurrencyHistory.HistoryDate = Convert.ToDateTime("1800-01-01");
                    db.Entry(dirCurrencyHistory).State = EntityState.Added;
                }
                else
                {
                    //3.1. Если реквизиты совпадают, то ничего не делаем
                    //     - Если нет, то:
                    //       - Даты совпадают - UPDATE
                    //       - Даты не совпадают - INSERT

                    //Если не совпадают реквизиты
                    if (
                        queryMax.DirCurrencyID != dirCurrencyHistory.DirCurrencyID ||
                        queryMax.DirCurrencyRate != dirCurrencyHistory.DirCurrencyRate ||
                        queryMax.DirCurrencyMultiplicity != dirCurrencyHistory.DirCurrencyMultiplicity
                    )
                    {
                        if (Convert.ToDateTime(queryMax.HistoryDate).ToString("yyyy-MM-dd") == Convert.ToDateTime(dirCurrencyHistory.HistoryDate).ToString("yyyy-MM-dd"))
                        {
                            //"HistoryID" который надо обновить, т.к. обновляет по PK
                            dirCurrencyHistory.DirCurrencyHistoryID = queryMax.DirCurrencyHistoryID;

                            //  - Даты совпадают - UPDATE
                            db.Entry(dirCurrencyHistory).State = EntityState.Modified;
                        }
                        else
                        {
                            //  - Даты не совпадают - INSERT
                            db.Entry(dirCurrencyHistory).State = EntityState.Added;

                            //3.2. Если следуящая запись (вверх, до этого нижнюю смотрели) равна (Rate, Multy) вставляемой, то удалить её!
                            var queryMin =
                                db.DirCurrencyHistories.
                                Where(x => x.DirCurrencyID == dirCurrency.DirCurrencyID).
                                GroupBy(g => new { g.HistoryDate, g.DirCurrencyHistoryID, g.DirCurrencyID, g.DirCurrencyRate, g.DirCurrencyMultiplicity }).
                                Where(grp => grp.Min(m => m.HistoryDate) >= dirCurrencyHistory.HistoryDate).
                                Select(x => x.Key).
                                ToList();
                            if (
                                queryMin.Count() > 0 &&
                                queryMin[0].DirCurrencyID == dirCurrencyHistory.DirCurrencyID &&
                                queryMin[0].DirCurrencyRate == dirCurrencyHistory.DirCurrencyRate &&
                                queryMin[0].DirCurrencyMultiplicity == dirCurrencyHistory.DirCurrencyMultiplicity
                                )
                            {
                                DirCurrencyHistory dirCurrencyHistoryMin = db.DirCurrencyHistories.Find(queryMin[0].DirCurrencyHistoryID);
                                db.Entry(dirCurrencyHistoryMin).State = EntityState.Deleted;
                            }
                        }
                    }
                }

                db.SaveChanges();

                #endregion


                #region 3. Подтверждение транзакции

                ts.Commit(); //.Complete();

                #endregion
            }
        }

        #endregion
    }
}