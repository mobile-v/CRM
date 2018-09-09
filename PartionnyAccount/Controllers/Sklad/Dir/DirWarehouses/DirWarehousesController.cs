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

namespace PartionnyAccount.Controllers.Sklad.Dir.DirWarehouses
{
    public class DirWarehousesController : ApiController
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
        //Локации (под-склады)
        string SubWar1 = " Списание", SubWar2 = " Возвраты", SubWar3 = " Заказы для ремонтов", SubWar4 = " Предзаказы", SubWar5 = " БУ.Разбор";

        int ListObjectID = 28;

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
            public int? ListObjectID;
            public int? Sub;
            public bool? WarehouseAll;
        }
        // GET: api/DirWarehouses
        public async Task<IHttpActionResult> GetDirWarehouses(HttpRequestMessage request)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWarehouses"));
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
                _params.Sub = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "Sub", true) == 0).Value); if (_params.Sub == 0) _params.Sub = null;
                //_params.WarehouseAll = Convert.ToBoolean(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "WarehouseAll", true) == 0).Value);

                //для документа "DocMovements" показать все склады
                _params.ListObjectID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectID", true) == 0).Value);   //Склад привязанный к сотруднику

                #endregion


                if (_params.type == "Grid")
                {

                    //Если привязка к сотруднику (для документа "DocMovements" показать все склады)
                    if (field.DirEmployeeID != 1 && _params.ListObjectID != 33)
                    {
                        #region Основной запрос *** *** ***

                        var query =
                        (
                            from dirWarehouses in db.DirWarehouses
                            from x in db.DirEmployeeWarehouse

                            where dirWarehouses.Sub == _params.Sub && x.DirWarehouseID == dirWarehouses.DirWarehouseID && x.DirEmployeeID == field.DirEmployeeID
                            select new
                            {
                                DirWarehouseID = dirWarehouses.DirWarehouseID,
                                Sub = dirWarehouses.Sub,
                                Del = dirWarehouses.Del,
                                SysRecord = dirWarehouses.SysRecord,
                                DirWarehouseName = dirWarehouses.DirWarehouseName,
                                DirWarehouseAddress = dirWarehouses.DirWarehouseAddress,
                                Phone = dirWarehouses.Phone,

                                DirCashOfficeID = dirWarehouses.dirCashOffice.DirCashOfficeID,
                                DirCashOfficeName = dirWarehouses.dirCashOffice.DirCashOfficeName,
                                DirCurrencyID = dirWarehouses.dirCashOffice.DirCurrencyID,
                                DirCurrencyRate = dirWarehouses.dirCashOffice.dirCurrency.DirCurrencyRate,
                                DirCurrencyMultiplicity = dirWarehouses.dirCashOffice.dirCurrency.DirCurrencyMultiplicity,
                                DirCashOfficeSum = dirWarehouses.dirCashOffice.DirCashOfficeSum,

                                //DirWarehouseLocName = dirWarehouses.DirWarehouseLoc
                                DirWarehouseLocName =
                                dirWarehouses.DirWarehouseLoc == 1 ? SubWar1 :
                                dirWarehouses.DirWarehouseLoc == 2 ? SubWar2 :
                                dirWarehouses.DirWarehouseLoc == 3 ? SubWar3 :
                                dirWarehouses.DirWarehouseLoc == 4 ? SubWar4 :
                                dirWarehouses.DirWarehouseLoc == 5 ? SubWar5 :
                                "Ошибка!",

                                IsAdmin = x.IsAdmin,

                                //SalaryPercentTrade = dirWarehouses.SalaryPercentTrade,
                                //SalaryPercentService1Tabs = dirWarehouses.SalaryPercentService1Tabs,
                                //SalaryPercentService2Tabs = dirWarehouses.SalaryPercentService2Tabs,
                                //SalaryPercentSecond = dirWarehouses.SalaryPercentSecond,

                                //ККМ
                                KKMSActive = dirWarehouses.KKMSActive,

                                //Автоматическое закрытие смены
                                SmenaClose = dirWarehouses.SmenaClose,
                                SmenaCloseTime = dirWarehouses.SmenaCloseTime,
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
                                query = query.Where(x => x.DirWarehouseID == iNumber32 || x.DirWarehouseName.Contains(_params.parSearch) || x.DirWarehouseAddress.Contains(_params.parSearch));
                            }
                            else
                            {
                                query = query.Where(x => x.DirWarehouseName.Contains(_params.parSearch) || x.DirWarehouseAddress.Contains(_params.parSearch));
                            }
                        }

                        #endregion


                        //Если привязка к сотруднику (для документа "DocMovements" показать все склады)
                        /*
                        if (field.DirEmployeeID != 1 && _params.ListObjectID != 33)
                        {
                            //1. Получаем все склады к которым у Сотрудника есть доступ
                            var queryW = await Task.Run(() =>
                                (
                                    from x in db.DirEmployeeWarehouse
                                    where x.DirEmployeeID == field.DirEmployeeID
                                    select x.DirWarehouseID
                                ).ToListAsync());

                            if (queryW.Count() > 0)
                            {
                                query = query.Where(x => queryW.Contains(x.DirWarehouseID));
                            }
                        }
                        */


                        #region OrderBy и Лимит

                        query = query.OrderBy(x => x.DirWarehouseName).Skip(_params.Skip).Take(_params.limit);

                        #endregion


                        #endregion


                        #region Отправка JSON

                        //К-во Номенклатуры
                        int dirCount = await Task.Run(() => db.DirWarehouses.CountAsync());

                        //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                        int dirCount2 = query.Count();
                        if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                        dynamic collectionWrapper = new
                        {
                            sucess = true,
                            total = dirCount,
                            DirWarehouse = query
                        };
                        return await Task.Run(() => Ok(collectionWrapper));

                        #endregion
                    }
                    else
                    {
                        #region Основной запрос *** *** ***

                        var query =
                        (
                            from dirWarehouses in db.DirWarehouses
                            where dirWarehouses.Sub == _params.Sub
                            select new
                            {
                                DirWarehouseID = dirWarehouses.DirWarehouseID,
                                Sub = dirWarehouses.Sub,
                                Del = dirWarehouses.Del,
                                SysRecord = dirWarehouses.SysRecord,
                                DirWarehouseName = dirWarehouses.DirWarehouseName,
                                DirWarehouseAddress = dirWarehouses.DirWarehouseAddress,
                                Phone = dirWarehouses.Phone,

                                DirCashOfficeID = dirWarehouses.dirCashOffice.DirCashOfficeID,
                                DirCashOfficeName = dirWarehouses.dirCashOffice.DirCashOfficeName,
                                DirCurrencyID = dirWarehouses.dirCashOffice.DirCurrencyID,
                                DirCurrencyRate = dirWarehouses.dirCashOffice.dirCurrency.DirCurrencyRate,
                                DirCurrencyMultiplicity = dirWarehouses.dirCashOffice.dirCurrency.DirCurrencyMultiplicity,
                                DirCashOfficeSum = dirWarehouses.dirCashOffice.DirCashOfficeSum,

                                //DirWarehouseLocName = dirWarehouses.DirWarehouseLoc
                                DirWarehouseLocName =
                                dirWarehouses.DirWarehouseLoc == 1 ? SubWar1 :
                                dirWarehouses.DirWarehouseLoc == 2 ? SubWar2 :
                                dirWarehouses.DirWarehouseLoc == 3 ? SubWar3 :
                                dirWarehouses.DirWarehouseLoc == 4 ? SubWar4 :
                                dirWarehouses.DirWarehouseLoc == 4 ? SubWar5 :
                                "Ошибка!",

                                //Админ - всегда Админ!
                                //IsAdmin = x.IsAdmin,

                                //ККМ
                                KKMSActive = dirWarehouses.KKMSActive,

                                //Автоматическое закрытие смены
                                SmenaClose = dirWarehouses.SmenaClose,
                                SmenaCloseTime = dirWarehouses.SmenaCloseTime,

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
                                query = query.Where(x => x.DirWarehouseID == iNumber32 || x.DirWarehouseName.Contains(_params.parSearch) || x.DirWarehouseAddress.Contains(_params.parSearch));
                            }
                            else
                            {
                                query = query.Where(x => x.DirWarehouseName.Contains(_params.parSearch) || x.DirWarehouseAddress.Contains(_params.parSearch));
                            }
                        }

                        #endregion


                        #region OrderBy и Лимит

                        query = query.OrderBy(x => x.DirWarehouseName).Skip(_params.Skip).Take(_params.limit);

                        #endregion


                        #endregion


                        #region Отправка JSON

                        //К-во Номенклатуры
                        int dirCount = await Task.Run(() => db.DirWarehouses.CountAsync());

                        //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                        int dirCount2 = query.Count();
                        if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                        dynamic collectionWrapper = new
                        {
                            sucess = true,
                            total = dirCount,
                            DirWarehouse = query
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
                         from x in db.DirWarehouses
                         where x.Sub == null && x.DirWarehouseID != _params.XGroupID_NotShow
                         select new
                         {
                             id = x.DirWarehouseID,
                             text = x.DirWarehouseName,
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

        // GET: api/DirWarehouses/5
        [ResponseType(typeof(DirWarehouse))]
        public async Task<IHttpActionResult> GetDirWarehouse(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWarehouses"));
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

                var query = await Task.Run(() =>
                    (
                        from x in db.DirWarehouses
                        where x.DirWarehouseID == id
                        select x
                        /*new
                        {
                            DirWarehouseID = x.DirWarehouseID,
                            Del = x.Del,
                            DirWarehouseName = x.DirWarehouseName,
                            DirWarehouseAddress = x.DirWarehouseAddress,
                            Phone = x.Phone,

                            DirCashOfficeID = x.DirCashOfficeID,
                            DirCashOfficeName = x.dirCashOffice.DirCashOfficeName,

                            DirBankID = x.DirBankID,
                            DirBankName = x.dirBank.DirBankName,


                            SalaryPercentTrade = x.SalaryPercentTrade,
                            SalaryPercentService1Tabs = x.SalaryPercentService1Tabs,
                            SalaryPercentService2Tabs = x.SalaryPercentService2Tabs,
                            SalaryPercentSecond = x.SalaryPercentSecond,
                        }*/
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

        // PUT: api/DirWarehouses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirWarehouse(int id, DirWarehouse dirWarehouse)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWarehouses"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirWarehouse.DirWarehouseID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //dirWarehouse.Substitute();

            #endregion


            #region Сохранение

            try
            {
                //db.Entry(dirWarehouse).State = EntityState.Modified;
                //await Task.Run(() => db.SaveChangesAsync());

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        dirWarehouse = await Task.Run(() => mPutPostDirWarehouse(db, dirWarehouse, EntityState.Modified, field));
                        ts.Commit(); //.Complete();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }



                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirWarehouse.DirWarehouseID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirWarehouse.DirWarehouseID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/DirWarehouses
        [ResponseType(typeof(DirWarehouse))]
        public async Task<IHttpActionResult> PostDirWarehouse(DirWarehouse dirWarehouse)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWarehouses"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //dirWarehouse.Substitute();

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                //db.Entry(dirWarehouse).State = EntityState.Added;
                //await Task.Run(() => db.SaveChangesAsync());

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        dirWarehouse = await Task.Run(() => mPutPostDirWarehouse(db, dirWarehouse, EntityState.Added, field));
                        ts.Commit(); //.Complete();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }



                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirWarehouse.DirWarehouseID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirWarehouse.DirWarehouseID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DirWarehouses/5
        [ResponseType(typeof(DirWarehouse))]
        public async Task<IHttpActionResult> DeleteDirWarehouse(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirWarehouses"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(id);
                if (dirWarehouse == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                db.DirWarehouses.Remove(dirWarehouse);
                await db.SaveChangesAsync();


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 5; //Удаление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirWarehouse.DirWarehouseID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirWarehouse.DirWarehouseID,
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

        private bool DirWarehouseExists(int id)
        {
            return db.DirWarehouses.Count(e => e.DirWarehouseID == id) > 0;
        }


        internal async Task<DirWarehouse> mPutPostDirWarehouse(
           DbConnectionSklad db,
           DirWarehouse dirWarehouse,
           EntityState entityState, //EntityState.Added, Modified

           Classes.Account.Login.Field field //Дополнительные данные о сотруднике
           )
        {
            if (dirWarehouse.SmenaClose && (dirWarehouse.SmenaCloseTime==null || dirWarehouse.SmenaCloseTime.Length != 5))
            {
                throw new System.InvalidOperationException("Не верное времмя закрытия смены ККМ!");
            }


            db.Entry(dirWarehouse).State = entityState;
            await Task.Run(() => db.SaveChangesAsync());


            #region Sub (Локации)

            //Если создаём новый склад, то создаём новые под-склады
            if (entityState == EntityState.Added)
            {
                //1.1. Списание
                Models.Sklad.Dir.DirWarehouse dirWarehouse1 = new DirWarehouse();
                dirWarehouse1.Sub = dirWarehouse.DirWarehouseID;
                dirWarehouse1.DirBankID = dirWarehouse.DirBankID;
                dirWarehouse1.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                dirWarehouse1.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                dirWarehouse1.Phone = dirWarehouse.Phone;
                dirWarehouse1.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar1;
                dirWarehouse1.DirWarehouseLoc = 1;

                db.Entry(dirWarehouse1).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());


                //1.2. Возвраты
                Models.Sklad.Dir.DirWarehouse dirWarehouse2 = new DirWarehouse();
                dirWarehouse2.Sub = dirWarehouse.DirWarehouseID;
                dirWarehouse2.DirBankID = dirWarehouse.DirBankID;
                dirWarehouse2.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                dirWarehouse2.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                dirWarehouse2.Phone = dirWarehouse.Phone;
                dirWarehouse2.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar2;
                dirWarehouse2.DirWarehouseLoc = 2;

                db.Entry(dirWarehouse2).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());


                //1.3. Заказы для ремонтов
                Models.Sklad.Dir.DirWarehouse dirWarehouse3 = new DirWarehouse();
                dirWarehouse3.Sub = dirWarehouse.DirWarehouseID;
                dirWarehouse3.DirBankID = dirWarehouse.DirBankID;
                dirWarehouse3.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                dirWarehouse3.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                dirWarehouse3.Phone = dirWarehouse.Phone;
                dirWarehouse3.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar3;
                dirWarehouse3.DirWarehouseLoc = 3;

                db.Entry(dirWarehouse3).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());


                //1.4. Предзаказы
                Models.Sklad.Dir.DirWarehouse dirWarehouse4 = new DirWarehouse();
                dirWarehouse4.Sub = dirWarehouse.DirWarehouseID;
                dirWarehouse4.DirBankID = dirWarehouse.DirBankID;
                dirWarehouse4.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                dirWarehouse4.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                dirWarehouse4.Phone = dirWarehouse.Phone;
                dirWarehouse4.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar4;
                dirWarehouse4.DirWarehouseLoc = 4;

                db.Entry(dirWarehouse4).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());


                //1.5. БУ.Разбор
                Models.Sklad.Dir.DirWarehouse dirWarehouse5 = new DirWarehouse();
                dirWarehouse5.Sub = dirWarehouse.DirWarehouseID;
                dirWarehouse5.DirBankID = dirWarehouse.DirBankID;
                dirWarehouse5.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                dirWarehouse5.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                dirWarehouse5.Phone = dirWarehouse.Phone;
                dirWarehouse5.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar5;
                dirWarehouse5.DirWarehouseLoc = 5;

                db.Entry(dirWarehouse5).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());
            }
            else
            {
                //1.1. Списание
                var query1 = await db.DirWarehouses.Where(x => x.Sub == dirWarehouse.DirWarehouseID && x.DirWarehouseLoc == 1).ToListAsync();
                if (query1.Count() > 0)
                {
                    Models.Sklad.Dir.DirWarehouse dirWarehouse1 = query1[0];
                    dirWarehouse1.DirBankID = dirWarehouse.DirBankID;
                    dirWarehouse1.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                    dirWarehouse1.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                    dirWarehouse1.Phone = dirWarehouse.Phone;
                    dirWarehouse1.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar1;
                    dirWarehouse1.DirWarehouseLoc = 1;

                    db.Entry(dirWarehouse1).State = EntityState.Modified;
                    await Task.Run(() => db.SaveChangesAsync());
                }


                //1.2. Возвраты
                var query2 = await db.DirWarehouses.Where(x => x.Sub == dirWarehouse.DirWarehouseID && x.DirWarehouseLoc == 2).ToListAsync();
                if (query2.Count() > 0)
                {
                    Models.Sklad.Dir.DirWarehouse dirWarehouse1 = query2[0];
                    dirWarehouse1.DirBankID = dirWarehouse.DirBankID;
                    dirWarehouse1.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                    dirWarehouse1.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                    dirWarehouse1.Phone = dirWarehouse.Phone;
                    dirWarehouse1.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar2;
                    dirWarehouse1.DirWarehouseLoc = 2;

                    db.Entry(dirWarehouse1).State = EntityState.Modified;
                    await Task.Run(() => db.SaveChangesAsync());
                }


                //1.3. Заказы для ремонтов
                var query3 = await db.DirWarehouses.Where(x => x.Sub == dirWarehouse.DirWarehouseID && x.DirWarehouseLoc == 3).ToListAsync();
                if (query3.Count() > 0)
                {
                    Models.Sklad.Dir.DirWarehouse dirWarehouse1 = query3[0];
                    dirWarehouse1.DirBankID = dirWarehouse.DirBankID;
                    dirWarehouse1.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                    dirWarehouse1.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                    dirWarehouse1.Phone = dirWarehouse.Phone;
                    dirWarehouse1.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar3;
                    dirWarehouse1.DirWarehouseLoc = 3;

                    db.Entry(dirWarehouse1).State = EntityState.Modified;
                    await Task.Run(() => db.SaveChangesAsync());
                }


                //1.4. Предзаказы
                var query4 = await db.DirWarehouses.Where(x => x.Sub == dirWarehouse.DirWarehouseID && x.DirWarehouseLoc == 4).ToListAsync();
                if (query4.Count() > 0)
                {
                    Models.Sklad.Dir.DirWarehouse dirWarehouse1 = query4[0];
                    dirWarehouse1.DirBankID = dirWarehouse.DirBankID;
                    dirWarehouse1.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                    dirWarehouse1.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                    dirWarehouse1.Phone = dirWarehouse.Phone;
                    dirWarehouse1.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar4;
                    dirWarehouse1.DirWarehouseLoc = 4;

                    db.Entry(dirWarehouse1).State = EntityState.Modified;
                    await Task.Run(() => db.SaveChangesAsync());
                }


                //1.5. БУ.Разбор
                var query5 = await db.DirWarehouses.Where(x => x.Sub == dirWarehouse.DirWarehouseID && x.DirWarehouseLoc == 5).ToListAsync();
                if (query5.Count() > 0)
                {
                    Models.Sklad.Dir.DirWarehouse dirWarehouse1 = query5[0];
                    dirWarehouse1.DirBankID = dirWarehouse.DirBankID;
                    dirWarehouse1.DirCashOfficeID = dirWarehouse.DirCashOfficeID;
                    dirWarehouse1.DirWarehouseAddress = dirWarehouse.DirWarehouseAddress;
                    dirWarehouse1.Phone = dirWarehouse.Phone;
                    dirWarehouse1.DirWarehouseName = dirWarehouse.DirWarehouseName + SubWar5;
                    dirWarehouse1.DirWarehouseLoc = 5;

                    db.Entry(dirWarehouse1).State = EntityState.Modified;
                    await Task.Run(() => db.SaveChangesAsync());
                }

            }


            #endregion


            return dirWarehouse;
        }

        #endregion
    }
}