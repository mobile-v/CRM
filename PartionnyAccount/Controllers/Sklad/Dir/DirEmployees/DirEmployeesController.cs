﻿using System;
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
using System.Web.Script.Serialization;
using System.Transactions;
using System.Data.SQLite;

namespace PartionnyAccount.Controllers.Sklad.Dir.DirEmployees
{
    public class DirEmployeesController : ApiController
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

        int ListObjectID = 5;

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

            public int DirWarehouseID = 0;
            public bool DeletedRecordsShow = true;
        }
        // GET: api/DirEmployees
        public async Task<IHttpActionResult> GetDirEmployees(HttpRequestMessage request)
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


                // !!! НИЖЕ - нужно получить склад !!!
                //Права (1 - Write, 2 - Read, 3 - No Access)
                //Или есть доступ к справонику или Админ точки
                /*
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirEmployees"));
                bool bAdmin = await Task.Run(() => accessRight.AccessIsAdmin(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirEmployees"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                */

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

                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);   //Номер страницы
                _params.DeletedRecordsShow = Convert.ToBoolean(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DeletedRecordsShow", true) == 0).Value);   //Номер страницы


                //Права (1 - Write, 2 - Read, 3 - No Access)
                //Или есть доступ к справонику или Админ точки
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirEmployees"));
                bool bAdmin = await Task.Run(() => accessRight.AccessIsAdmin(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, _params.DirWarehouseID));
                if (iRight == 3 && !bAdmin) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                #endregion


                if (_params.type == "Grid")
                {

                    if (_params.DirWarehouseID < 1)
                    {

                        #region Основной запрос *** *** ***

                        var query =
                            (
                                from dirEmployees in db.DirEmployees
                                select new
                                {
                                    DirEmployeeID = dirEmployees.DirEmployeeID,
                                    Del = dirEmployees.Del,
                                    SysRecord = dirEmployees.SysRecord,
                                    DirEmployeeName = dirEmployees.DirEmployeeName,
                                    DirEmployeeActive = dirEmployees.DirEmployeeActive
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
                                query = query.Where(x => x.DirEmployeeID == iNumber32 || x.DirEmployeeName.Contains(_params.parSearch));
                            }
                            else
                            {
                                query = query.Where(x => x.DirEmployeeName.Contains(_params.parSearch));
                            }
                        }

                        #endregion


                        #region OrderBy и Лимит

                        query = query.OrderBy(x => x.DirEmployeeName).Skip(_params.Skip).Take(_params.limit);

                        #endregion


                        #endregion


                        #region Отправка JSON

                        //К-во Номенклатуры
                        int dirCount = await Task.Run(() => db.DirEmployees.Count());

                        //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                        int dirCount2 = query.Count();
                        if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                        dynamic collectionWrapper = new
                        {
                            sucess = true,
                            total = dirCount,
                            DirEmployee = query
                        };
                        return await Task.Run(() => Ok(collectionWrapper));

                        #endregion

                    }
                    else
                    {

                        #region Основной запрос *** *** ***

                        var query =
                            (
                                from dirEmployees in db.DirEmployees
                                from dirEmployeeWarehouse in db.DirEmployeeWarehouse
                                where dirEmployees.DirEmployeeID == dirEmployeeWarehouse.DirEmployeeID && dirEmployeeWarehouse.DirWarehouseID == _params.DirWarehouseID &&
                                      dirEmployees.DirEmployeeActive == true //Не показывать "Не активных"
                                select new
                                {
                                    DirEmployeeID = dirEmployees.DirEmployeeID,
                                    Del = dirEmployees.Del,
                                    SysRecord = dirEmployees.SysRecord,
                                    DirEmployeeName = dirEmployees.DirEmployeeName,
                                    DirEmployeeActive = dirEmployees.DirEmployeeActive
                                }
                            ).
                            Union
                            (
                                from dirEmployees in db.DirEmployees
                                from dirEmployeeWarehouse in db.DirEmployeeWarehouse
                                where dirEmployees.DirEmployeeID == 1
                                select new
                                {
                                    DirEmployeeID = dirEmployees.DirEmployeeID,
                                    Del = dirEmployees.Del,
                                    SysRecord = dirEmployees.SysRecord,
                                    DirEmployeeName = dirEmployees.DirEmployeeName,
                                    DirEmployeeActive = dirEmployees.DirEmployeeActive
                                }
                            );

                        #endregion


                        #region Условия (параметры) *** *** ***


                        #region Не показывать удалённые

                        if (!_params.DeletedRecordsShow)
                        {
                            query = query.Where(x => x.Del == false);
                        }
                        else
                        {
                            if (!Convert.ToBoolean(sysSetting.DeletedRecordsShow))
                            {
                                query = query.Where(x => x.Del == sysSetting.DeletedRecordsShow);
                            }
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
                                query = query.Where(x => x.DirEmployeeID == iNumber32 || x.DirEmployeeName.Contains(_params.parSearch));
                            }
                            else
                            {
                                query = query.Where(x => x.DirEmployeeName.Contains(_params.parSearch));
                            }
                        }

                        #endregion


                        #region OrderBy и Лимит

                        query = query.OrderBy(x => x.DirEmployeeName).Skip(_params.Skip).Take(_params.limit);

                        #endregion


                        #endregion


                        #region Отправка JSON

                        //К-во Номенклатуры
                        int dirCount = await Task.Run(() => db.DirEmployees.Count());

                        //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                        int dirCount2 = query.Count();
                        if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                        dynamic collectionWrapper = new
                        {
                            sucess = true,
                            total = dirCount,
                            DirEmployee = query
                        };
                        return await Task.Run(() => Ok(collectionWrapper));

                        #endregion

                    }

                }
                else //Tree
                {
                    #region Отобразить только "Руты" *** *** ***

                    var query =
                        (
                         from x in db.DirEmployees
                         select new
                         {
                             id = x.DirEmployeeID,
                             text = x.DirEmployeeName,
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

        // GET: api/DirEmployees/5
        [ResponseType(typeof(DirEmployee))]
        public async Task<IHttpActionResult> GetDirEmployee(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirEmployees"));
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



                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        from x in db.DirEmployees

                        join dirBonuses1 in db.DirBonuses on x.DirBonusID equals dirBonuses1.DirBonusID into dirBonuses2
                        from dirBonuses in dirBonuses2.DefaultIfEmpty()

                        join dirCurrencies1 in db.DirCurrencies on x.DirCurrencyID equals dirCurrencies1.DirCurrencyID into dirCurrencies2
                        from dirCurrencies in dirCurrencies2.DefaultIfEmpty()

                        //join dirWarehouses1 in db.DirWarehouses on x.DirWarehouseID equals dirWarehouses1.DirWarehouseID into dirWarehouses2
                        //from dirWarehouses in dirWarehouses2.DefaultIfEmpty()

                        join dirContractors1Org in db.DirContractors on x.DirContractorIDOrg equals dirContractors1Org.DirContractorID into dirContractors2Org
                        from dirContractorsOrg in dirContractors2Org.DefaultIfEmpty()

                        where x.DirEmployeeID == id
                        select x

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

        // PUT: api/DirEmployees/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirEmployee(int id, DirEmployee dirEmployee)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirEmployees"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            //Получаем колекцию "Табличную часть"
            Models.Sklad.Dir.DirEmployeeWarehouses[] dirEmployeeWarehousesCollection = null;
            if (!String.IsNullOrEmpty(dirEmployee.recordsDirEmployeeWarehouses))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dirEmployeeWarehousesCollection = serializer.Deserialize<Models.Sklad.Dir.DirEmployeeWarehouses[]>(dirEmployee.recordsDirEmployeeWarehouses);
            }

            for (int i = 0; i < dirEmployeeWarehousesCollection.Count(); i++)
            {
                Models.Sklad.Dir.DirEmployeeWarehouses dirEmployeeWarehouses = dirEmployeeWarehousesCollection[i];

                if (dirEmployeeWarehouses.IsAdminNameRu.ToString().ToLower() == "администратор") dirEmployeeWarehouses.IsAdmin = true;
                else dirEmployeeWarehouses.IsAdmin = false;

                //if (dirEmployeeWarehouses.WarehouseAllNameRu.ToString().ToLower() == "виден" && Convert.ToBoolean(dirEmployee.RightDocServicePurchesWarehouseAllCheck)) dirEmployeeWarehouses.WarehouseAll = true;
                if (dirEmployeeWarehouses.WarehouseAllNameRu.ToString().ToLower() == "виден") dirEmployeeWarehouses.WarehouseAll = true;
                else dirEmployeeWarehouses.WarehouseAll = false;

                dirEmployeeWarehousesCollection[i] = dirEmployeeWarehouses;
            }

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirEmployee.DirEmployeeID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            dirEmployee.Substitute();

            //Дополнительные проверки
            if (Convert.ToBoolean(dirEmployee.DirEmployeeActive))
            {
                //Уникальность Логина
                if (ExistLogin(dirEmployee)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg81));

                //Корректность Логина
                if (!CorrectLogin(dirEmployee)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg86));
            }

            //ЗП: Если есть фиксированный оклад "SalaryFixedSalesMount>0", то тоогда или "Salary=0" или "Salary>0 and SalaryDayMonthly=1"
            if (
                dirEmployee.SalaryFixedSalesMount > 0 &&
                dirEmployee.Salary > 0 &&
                dirEmployee.SalaryDayMonthly == 2
               )
            {
                return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg125));
            }

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                await Task.Run(() => mPutPostDirEmployee(dirEmployee, EntityState.Modified, dirEmployeeWarehousesCollection, field));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirEmployee.DirEmployeeID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirEmployee.DirEmployeeID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/DirEmployees
        [ResponseType(typeof(DirEmployee))]
        public async Task<IHttpActionResult> PostDirEmployee(DirEmployee dirEmployee)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirEmployees"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            //Получаем колекцию "Табличную часть"
            Models.Sklad.Dir.DirEmployeeWarehouses[] dirEmployeeWarehousesCollection = null;
            if (!String.IsNullOrEmpty(dirEmployee.recordsDirEmployeeWarehouses))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dirEmployeeWarehousesCollection = serializer.Deserialize<Models.Sklad.Dir.DirEmployeeWarehouses[]>(dirEmployee.recordsDirEmployeeWarehouses);
            }

            for (int i = 0; i < dirEmployeeWarehousesCollection.Count(); i++)
            {
                Models.Sklad.Dir.DirEmployeeWarehouses dirEmployeeWarehouses = dirEmployeeWarehousesCollection[i];

                if (dirEmployeeWarehouses.IsAdminNameRu.ToString().ToLower() == "администратор") dirEmployeeWarehouses.IsAdmin = true;
                else dirEmployeeWarehouses.IsAdmin = false;

                if (dirEmployeeWarehouses.WarehouseAllNameRu.ToString().ToLower() == "виден" && Convert.ToBoolean(dirEmployee.RightDocServicePurchesWarehouseAllCheck)) dirEmployeeWarehouses.WarehouseAll = true;
                else dirEmployeeWarehouses.WarehouseAll = false;

                dirEmployeeWarehousesCollection[i] = dirEmployeeWarehouses;
            }

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            dirEmployee.Substitute();

            //Дополнительные проверки
            if (Convert.ToBoolean(dirEmployee.DirEmployeeActive))
            {
                //Уникальность Логина
                if (ExistLogin(dirEmployee)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg81));

                //Корректность Логина
                if (!CorrectLogin(dirEmployee)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg86));
            }

            //ЗП: Если есть фиксированный оклад "SalaryFixedSalesMount>0", то тоогда или "Salary=0" или "Salary>0 and SalaryDayMonthly=1"
            if (
                dirEmployee.SalaryFixedSalesMount > 0 &&
                dirEmployee.Salary > 0 &&
                dirEmployee.SalaryDayMonthly == 2
               )
            {
                return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg125));
            }

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                await Task.Run(() => mPutPostDirEmployee(dirEmployee, EntityState.Added, dirEmployeeWarehousesCollection, field));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirEmployee.DirEmployeeID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirEmployee.DirEmployeeID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DirEmployees/5
        [ResponseType(typeof(DirEmployee))]
        public async Task<IHttpActionResult> DeleteDirEmployee(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirEmployees"));
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
                        from x in db.DirEmployeeHistories
                        where x.DirEmployeeID == id
                        select x
                    ).ToListAsync();


                //Сотрудник
                Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(id);
                if (dirEmployee == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));
                if (dirEmployee.SysRecord) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg98));

                if (!dirEmployee.Del)
                {
                    // === Удаляем === === === === === 


                    #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                    Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                    sysJourDisp.DirDispOperationID = 5; //Удаление записи
                    sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                    sysJourDisp.ListObjectID = ListObjectID;
                    sysJourDisp.TableFieldID = dirEmployee.DirEmployeeID;
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
                                Models.Sklad.Dir.DirEmployeeHistory dirEmployeeHistory = await db.DirEmployeeHistories.FindAsync(query[i].DirEmployeeHistoryID);
                                db.DirEmployeeHistories.Remove(dirEmployeeHistory);
                                await db.SaveChangesAsync();
                            }

                            db.DirEmployees.Remove(dirEmployee);
                            await db.SaveChangesAsync();

                            ts.Commit();

                            dynamic collectionWrapper = new
                            {
                                ID = dirEmployee.DirEmployeeID,
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
                                dirEmployee.Del = true;

                                db.Entry(dirEmployee).State = EntityState.Modified;
                                await db.SaveChangesAsync();

                                dynamic collectionWrapper = new
                                {
                                    ID = dirEmployee.DirEmployeeID,
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

                    dirEmployee.Del = false;

                    db.Entry(dirEmployee).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    dynamic collectionWrapper = new
                    {
                        ID = dirEmployee.DirEmployeeID,
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

        private bool DirEmployeeExists(int id)
        {
            return db.DirEmployees.Count(e => e.DirEmployeeID == id) > 0;
        }



        //Уникальный Логин
        private bool ExistLogin(
            DirEmployee dirEmployee
            )
        {
            int iCount =
                (
                    from x in db.DirEmployees
                    where x.DirEmployeeLogin == dirEmployee.DirEmployeeLogin && x.DirEmployeeID != dirEmployee.DirEmployeeID
                    select x
                ).Count();

            if (iCount > 0) return true;
            else return false;
        }

        //Спецсимволы Логина
        private bool CorrectLogin(DirEmployee dirEmployee)
        {
            string sLg = dirEmployee.DirEmployeeLogin;
            if
                (
                  sLg.IndexOf("@") != -1 || sLg.IndexOf("!") != -1 || sLg.IndexOf("#") != -1 || sLg.IndexOf("$") != -1 || sLg.IndexOf("%") != -1 || sLg.IndexOf("^") != -1 || sLg.IndexOf("&") != -1 || sLg.IndexOf("*") != -1 || sLg.IndexOf("(") != -1 || sLg.IndexOf(")") != -1 || sLg.IndexOf("_") != -1 || sLg.IndexOf("+") != -1 || sLg.IndexOf("=") != -1 ||
                   sLg.IndexOf("{") != -1 || sLg.IndexOf("}") != -1 || sLg.IndexOf(";") != -1 || sLg.IndexOf(":") != -1 || sLg.IndexOf(@"\") != -1 || sLg.IndexOf("|") != -1 ||
                    sLg.IndexOf("<") != -1 || sLg.IndexOf(">") != -1 || sLg.IndexOf("?") != -1 || sLg.IndexOf("/") != -1 ||
                  sLg.IndexOf("'") != -1 || sLg.IndexOf(@"""") != -1
                )
                return false;

            return true;
        }


        //Сохранение
        private void mPutPostDirEmployee(
            DirEmployee dirEmployee,
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Dir.DirEmployeeWarehouses[] dirEmployeeWarehousesCollection,

            Classes.Account.Login.Field field //Для "ExistPay"
            )
        {
            using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
            {
                #region 1. DirEmployee

                db.Entry(dirEmployee).State = entityState;
                db.SaveChanges();

                #endregion

                #region 2. dirEmployeeWarehouses *** *** *** *** *** *** *** *** *** ***


                //3.2. Удаляем все кроме существующих
                //3.2.1. Формируем запрос
                var query2 =
                    (
                    from dirEmployeeWarehouse in db.DirEmployeeWarehouse
                    where dirEmployeeWarehouse.DirEmployeeID == dirEmployee.DirEmployeeID // == id
                    select dirEmployeeWarehouse
                    ).ToList();
                //3.2.2. Удаляем каждую запись отдельно
                foreach (var dirDiscountTab in query2)
                {
                    try
                    {
                        db.Entry(dirDiscountTab).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    catch (Exception ex) { ts.Rollback(); db.Database.Connection.Close(); throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg93 + ex.Message); }
                }


                //3.3. Проверяем, что бы склады не повторялись:
                for (int i = 0; i < dirEmployeeWarehousesCollection.Length; i++)
                {
                    for (int j = i + 1; j < dirEmployeeWarehousesCollection.Length; j++)
                    {
                        //Суммы равны
                        if (dirEmployeeWarehousesCollection[i].DirWarehouseID == dirEmployeeWarehousesCollection[j].DirWarehouseID)
                        {
                            ts.Rollback(); db.Database.Connection.Close();
                            throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg121);
                        }
                    }
                }


                //3.4 Сохраняем
                for (int i = 0; i < dirEmployeeWarehousesCollection.Count(); i++)
                {
                    dirEmployeeWarehousesCollection[i].DirEmployeeID = Convert.ToInt32(dirEmployee.DirEmployeeID);
                    db.Entry(dirEmployeeWarehousesCollection[i]).State = EntityState.Added;
                    db.SaveChanges();
                }
                //db.SaveChanges();

                #endregion

                #region 2. DirEmployeeHistory

                //Получаем историю
                DirEmployeeHistory dirEmployeeHistory = new DirEmployeeHistory();
                dirEmployeeHistory.DirEmployeeID = Convert.ToInt32(dirEmployee.DirEmployeeID);
                dirEmployeeHistory.HistoryDate = DateTime.Now;
                dirEmployeeHistory.DirCurrencyID = dirEmployee.DirCurrencyID;
                dirEmployeeHistory.Salary = dirEmployee.Salary;
                dirEmployeeHistory.SalaryDayMonthly = dirEmployee.SalaryDayMonthly; 
                dirEmployeeHistory.DirBonusID = dirEmployee.DirBonusID;


                //Алгоритм: 
                // - Если не найдена, то создаём сразу запись, с датой = "1800-01-01", т.к. это первая запис в истории


                //1. Ищим дату самую блискую снизу вверх (Максимум) к дате "dirEmployeeHistory.DirEmployeeHistoryDate"
                //   Возмоно 2-а варианта: "есть дата" и "нет даты"
                var queryMax =
                    db.DirEmployeeHistories.
                    Where(x => x.DirEmployeeID == dirEmployee.DirEmployeeID).
                    GroupBy(g => new { g.HistoryDate, g.DirEmployeeHistoryID, g.DirCurrencyID, g.Salary, g.DirBonusID, g.SalaryDayMonthly }).
                    Where(grp => grp.Max(m => m.HistoryDate) <= dirEmployeeHistory.HistoryDate).
                    Select(x => x.Key).OrderByDescending(o => o.HistoryDate).FirstOrDefault();

                //var queryMax = queryMax1.ToList();

                //if (queryMax.Count() == 0)
                if (queryMax == null)
                {
                    //2. Нет данных: "INSERT", создаем новую запись в Истории
                    dirEmployeeHistory.HistoryDate = Convert.ToDateTime("1800-01-01");
                    db.Entry(dirEmployeeHistory).State = EntityState.Added;
                }
                else
                {
                    //3.1. Если реквизиты совпадают, то ничего не делаем
                    //     - Если нет, то:
                    //       - Даты совпадают - UPDATE
                    //       - Даты не совпадают - INSERT

                    //Если не совпадают реквизиты
                    if (
                        queryMax.DirCurrencyID != dirEmployeeHistory.DirCurrencyID ||
                        queryMax.Salary != dirEmployeeHistory.Salary ||
                        queryMax.DirBonusID != dirEmployeeHistory.DirBonusID ||
                        queryMax.SalaryDayMonthly != dirEmployeeHistory.SalaryDayMonthly
                    )
                    {
                        if (Convert.ToDateTime(queryMax.HistoryDate).ToString("yyyy-MM-dd") == Convert.ToDateTime(dirEmployeeHistory.HistoryDate).ToString("yyyy-MM-dd") )
                        {
                            //"HistoryID" который надо обновить, т.к. обновляет по PK
                            dirEmployeeHistory.DirEmployeeHistoryID = queryMax.DirEmployeeHistoryID;

                            //  - Даты совпадают - UPDATE
                            db.Entry(dirEmployeeHistory).State = EntityState.Modified;
                        }
                        else
                        {
                            //  - Даты не совпадают - INSERT
                            db.Entry(dirEmployeeHistory).State = EntityState.Added;

                            //3.2. Если следуящая запись (вверх, до этого нижнюю смотрели) равна (Rate, Multy) вставляемой, то удалить её!
                            var queryMin =
                                db.DirEmployeeHistories.
                                Where(x => x.DirEmployeeID == dirEmployee.DirEmployeeID).
                                GroupBy(g => new { g.HistoryDate, g.DirEmployeeHistoryID, g.DirCurrencyID, g.Salary, g.DirBonusID, g.SalaryDayMonthly }).
                                Where(grp => grp.Min(m => m.HistoryDate) >= dirEmployeeHistory.HistoryDate).
                                Select(x => x.Key).
                                ToList();
                            if (
                                queryMin.Count() > 0 &&
                                queryMin[0].DirCurrencyID == dirEmployeeHistory.DirCurrencyID &&
                                queryMin[0].Salary == dirEmployeeHistory.Salary &&
                                queryMin[0].DirBonusID == dirEmployeeHistory.DirBonusID &&
                                queryMin[0].SalaryDayMonthly == dirEmployeeHistory.SalaryDayMonthly
                                )
                            {
                                DirEmployeeHistory dirEmployeeHistoryMin = db.DirEmployeeHistories.Find(queryMin[0].DirEmployeeHistoryID);
                                db.Entry(dirEmployeeHistoryMin).State = EntityState.Deleted;
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