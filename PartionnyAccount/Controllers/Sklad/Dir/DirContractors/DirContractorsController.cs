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

namespace PartionnyAccount.Controllers.Sklad.Dir.DirContractors
{
    public class DirContractorsController : ApiController
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

        int ListObjectID = 19;

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
            public int DirContractor2TypeID1 = 0;
            public int DirContractor2TypeID2 = 0;
            public int? Discount = 0;
        }
        // GET: api/DirContractors
        public async Task<IHttpActionResult> GetDirContractors(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirContractors"));
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
                _params.DirContractor2TypeID1 = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractor2TypeID1", true) == 0).Value);
                _params.DirContractor2TypeID2 = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractor2TypeID2", true) == 0).Value);

                #endregion


                if (_params.type == "Grid")
                {
                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from x in db.DirContractors
                            select new
                            {
                                DirContractorID = x.DirContractorID,
                                Del = x.Del,
                                SysRecord = x.SysRecord,
                                DirContractorName = x.DirContractorName,
                                //DirContractorAddress = x.DirContractorAddress
                                DirContractor2TypeID = x.DirContractor2TypeID,

                                //Скидки
                                DirContractorDiscount = x.DirContractorDiscount,
                                DirDiscountID = x.DirDiscountID
                            }
                        );

                    #endregion


                    #region Условия (параметры) *** *** ***


                    #region DirContractor2TypeID

                    if (_params.DirContractor2TypeID1 > 0 || _params.DirContractor2TypeID2 > 0)
                    {
                        query = query.Where(x => x.DirContractor2TypeID == _params.DirContractor2TypeID1 || x.DirContractor2TypeID == _params.DirContractor2TypeID2);
                    }

                    #endregion


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
                            query = query.Where(x => x.DirContractorID == iNumber32 || x.DirContractorName.Contains(_params.parSearch));
                        }
                        else
                        {
                            query = query.Where(x => x.DirContractorName.Contains(_params.parSearch));
                        }
                    }

                    #endregion


                    #region OrderBy и Лимит

                    query = query.OrderBy(x => x.DirContractorName).Skip(_params.Skip).Take(_params.limit);

                    #endregion


                    #endregion


                    #region Отправка JSON

                    //К-во Номенклатуры
                    int dirCount = await Task.Run(() => db.DirContractors.Count());

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount2 = query.Count();
                    //if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirContractor = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    #region Отобразить только "Руты" *** *** ***

                    var query =
                        (
                         from x in db.DirContractors
                         select new
                         {
                             id = x.DirContractorID,
                             text = x.DirContractorName,
                             leaf = true,
                             Del = x.Del,

                             DirContractor2TypeID = x.DirContractor2TypeID,
                             DirContractor2TypeName = x.dirContractor2Type.DirContractor2TypeName,
                         }
                        );

                    if (_params.DirContractor2TypeID1 > 0 || _params.DirContractor2TypeID2 > 0)
                    {
                        query = query.Where(x => x.DirContractor2TypeID == _params.DirContractor2TypeID1 || x.DirContractor2TypeID == _params.DirContractor2TypeID2);
                    }

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

        // GET: api/DirContractors/5
        [ResponseType(typeof(DirContractor))]
        public async Task<IHttpActionResult> GetDirContractor(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirContractors"));
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
                //Пришли за "Градационной Скидкой"
                _params.Discount = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "Discount", true) == 0).Value);   //Номер страницы

                #endregion

                if (_params.Discount == 0)
                {
                    #region Отправка JSON

                    var query = await Task.Run(() =>
                        (

                            from x in db.DirContractors

                            join dirDiscounts1 in db.DirDiscounts on x.DirDiscountID equals dirDiscounts1.DirDiscountID into dirDiscounts2
                            from dirDiscounts in dirDiscounts2.DefaultIfEmpty()

                            join dirBanks1 in db.DirBanks on x.DirBankID equals dirBanks1.DirBankID into dirBanks2
                            from dirBanks in dirBanks2.DefaultIfEmpty()

                            where x.DirContractorID == id
                            select new
                            {
                                DirContractorID = x.DirContractorID,
                                Del = x.Del,
                                DirContractorName = x.DirContractorName,
                                NameLower = x.NameLower,
                                DirContractorAddress = x.DirContractorAddress,

                                DirContractor1TypeID = x.DirContractor1TypeID,
                                DirContractor1TypeName = x.dirContractor1Type.DirContractor1TypeName,

                                DirContractor2TypeID = x.DirContractor2TypeID,
                                DirContractor2TypeName = x.dirContractor2Type.DirContractor2TypeName,

                                DirContractorPhone = x.DirContractorPhone,
                                DirContractorFax = x.DirContractorFax,
                                DirContractorEmail = x.DirContractorEmail,
                                DirContractorWWW = x.DirContractorWWW,

                                DirContractorDiscount = x.DirContractorDiscount,

                                DirDiscountID = x.DirDiscountID,
                                DirDiscountName = dirDiscounts.DirDiscountName,

                                DirBankID = x.DirBankID,
                                DirBankName = dirBanks.DirBankName,

                                DirContractorDesc = x.DirContractorDesc,
                                ImageLink = x.ImageLink,
                                DirBankAccountName = x.DirBankAccountName
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
                else
                {
                    #region Отправка JSON

                    Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(id);
                    int iDirDiscountID = Convert.ToInt32(dirContractor.DirDiscountID);
                    double dSalesSum = dirContractor.SalesSum;

                    var query = await Task.Run(() =>
                        (
                            from x in db.DirDiscountTabs
                            where x.DirDiscountID == iDirDiscountID && dSalesSum < x.SumBegin
                            select new
                            {
                                Discount = x.Discount == null ? 0 : x.Discount
                            }
                        ).MaxAsync(x => x.Discount));


                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        DirContractorDiscount = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/DirContractors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirContractor(int id, DirContractor dirContractor)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirContractors"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirContractor.DirContractorID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            dirContractor.Substitute();

            #endregion


            #region Сохранение

            try
            {
                db.Entry(dirContractor).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirContractor.DirContractorID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirContractor.DirContractorID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/DirContractors
        [ResponseType(typeof(DirContractor))]
        public async Task<IHttpActionResult> PostDirContractor(DirContractor dirContractor)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirContractors"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            dirContractor.Substitute();

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                db.Entry(dirContractor).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirContractor.DirContractorID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirContractor.DirContractorID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DirContractors/5
        [ResponseType(typeof(DirContractor))]
        public async Task<IHttpActionResult> DeleteDirContractor(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirContractors"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DirContractor dirContractor = await db.DirContractors.FindAsync(id);
                if (dirContractor == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                db.DirContractors.Remove(dirContractor);
                await db.SaveChangesAsync();


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 5; //Удаление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirContractor.DirContractorID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirContractor.DirContractorID,
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

        private bool DirContractorExists(int id)
        {
            return db.DirContractors.Count(e => e.DirContractorID == id) > 0;
        }

        #endregion
    }
}