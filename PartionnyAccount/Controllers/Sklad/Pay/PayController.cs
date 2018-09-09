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
using PartionnyAccount.Models.Sklad.Doc;
using System.Data.SQLite;
using System.Web.Script.Serialization;

namespace PartionnyAccount.Controllers.Sklad.Pay
{
    public class PayController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 59;

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;

            public int DocID = 0;
            public int DocCashBankID = 0;
            public int DirPaymentTypeID = 0;
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetPay(HttpRequestMessage request)
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
                dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права (1 - Write, 2 - Read, 3 - No Access)
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocPurches"));
                //if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                //Получаем сотрудника: если к нему привязан Склад и/или Организация, то выбираем документы только по этим характеристикам
                Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(field.DirEmployeeID);

                #endregion


                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.limit = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.DocID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocID", true) == 0).Value); //Номер документа

                #endregion



                #region Основной запрос *** *** ***

                var queryDocCashOfficeSums =
                    (
                        from x in db.DocCashOfficeSums
                        where x.DocID == _params.DocID
                        select new
                        {
                            DocID = x.DocID,
                            DocXID = x.DocXID,
                            DocCashBankID = x.DocCashOfficeSumID,

                            DirPaymentTypeID = 1,
                            DirXName = x.dirCashOffice.DirCashOfficeName,

                            DirEmployeeName = x.dirEmployee.DirEmployeeName,
                            DirXSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName,
                            DocXSumDate = x.DocCashOfficeSumDate,

                            //DocXSumSum = x.DocCashOfficeSumSum,
                            DocXSumSum =
                                x.DocCashOfficeSumSum < 0 ? -x.DocCashOfficeSumSum
                                :
                                x.DocCashOfficeSumSum,

                            DirCurrencyName = x.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity
                        }
                    );

                var queryDocBankSums =
                    (
                        from x in db.DocBankSums
                        where x.DocID == _params.DocID
                        select new
                        {
                            DocID = x.DocID,
                            DocXID = x.DocXID,
                            DocCashBankID = x.DocBankSumID,

                            DirPaymentTypeID = 2,
                            DirXName = x.dirBank.DirBankName,

                            DirEmployeeName = x.dirEmployee.DirEmployeeName,
                            DirXSumTypeName = x.dirBankSumType.DirBankSumTypeName,
                            DocXSumDate = x.DocBankSumDate,

                            //DocXSumSum = x.DocBankSumSum,
                            DocXSumSum =
                                x.DocBankSumSum < 0 ? -x.DocBankSumSum
                                :
                                x.DocBankSumSum,

                            DirCurrencyName = x.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity
                        }
                    );

                var query = queryDocCashOfficeSums.Concat(queryDocBankSums);

                #endregion


                #region Отправка JSON

                int iCount = await query.CountAsync();

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = iCount,
                    Pay = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/Pay/5
        //[ResponseType(typeof(Pay))]
        [HttpGet]
        public async Task<IHttpActionResult> GetPay(int id, HttpRequestMessage request)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirBanks"));
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
                //_params.DocCashBankID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocCashBankID", true) == 0).Value); //Номер документа
                _params.DirPaymentTypeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPaymentTypeID", true) == 0).Value); //Тип документа, что бы знать где делать выборку: в Кассе или в Банке

                #endregion


                #region Отправка JSON

                if (_params.DirPaymentTypeID == 1)
                {
                    #region Касса
                    
                    var queryDocCashOfficeSums = await Task.Run(() =>
                        (
                            from x in db.DocCashOfficeSums
                            where x.DocCashOfficeSumID == id
                            select new
                            {
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                DocCashBankID = x.DocCashOfficeSumID,

                                DirPaymentTypeID = 1,
                                DirXName = x.dirCashOffice.DirCashOfficeName,

                                DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                DirXSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName,
                                DocXSumDate = x.DocCashOfficeSumDate.ToString(),
                                
                                DocXSumSum = 
                                x.DocCashOfficeSumSum < 0 ? -x.DocCashOfficeSumSum
                                :
                                x.DocCashOfficeSumSum,
                                
                                DirCurrencyID = x.dirCurrency.DirCurrencyID,
                                DirCurrencyRate = x.DirCurrencyRate,
                                DirCurrencyMultiplicity = x.DirCurrencyMultiplicity
                            }
                        ).ToListAsync());

                    if (queryDocCashOfficeSums.Count() > 0)
                    {
                        return Ok(returnServer.Return(true, queryDocCashOfficeSums[0]));
                    }
                    else
                    {
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));
                    }

                    #endregion
                }
                else if (_params.DirPaymentTypeID == 2)
                {
                    #region Банк
                    
                    var queryDocBankSums = await Task.Run(() =>
                        (
                            from x in db.DocBankSums
                            where x.DocBankSumID == id
                            select new
                            {
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                DocCashBankID = x.DocBankSumID,

                                DirPaymentTypeID = 2,
                                DirXName = x.dirBank.DirBankName,

                                DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                DirXSumTypeName = x.dirBankSumType.DirBankSumTypeName,
                                DocXSumDate = x.DocBankSumDate,
                                
                                DocXSumSum =
                                x.DocBankSumSum < 0 ? -x.DocBankSumSum
                                :
                                x.DocBankSumSum,

                                DirCurrencyID = x.dirCurrency.DirCurrencyID,
                                DirCurrencyRate = x.DirCurrencyRate,
                                DirCurrencyMultiplicity = x.DirCurrencyMultiplicity
                            }
                        ).ToListAsync());


                    if (queryDocBankSums.Count() > 0)
                    {
                        return Ok(returnServer.Return(true, queryDocBankSums[0]));
                    }
                    else
                    {
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));
                    }

                    #endregion
                }
                else
                {
                    throw new System.InvalidOperationException("Не выбран метод оплаты: Касса или Банк!");
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

        // PUT: api/DirPay/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirPay(int id, Models.Sklad.Pay.Pay pay)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirPay"));
                //if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                                                                                                                       //if (id != pay.DirPayID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //Подстановки - некоторые поля надо заполнить, если они не заполены
                //pay.Substitute();

                #endregion


                #region Сохранение

                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        doc = await Task.Run(() => mPutPostPay(db, pay, EntityState.Modified, field)); //sysSetting
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
                sysJourDisp.TableFieldID = pay.DocCashBankID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    Payment = doc.Payment
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

        }

        // POST: api/DirPay
        [ResponseType(typeof(Models.Sklad.Pay.Pay))]
        public async Task<IHttpActionResult> PostDirPay(Models.Sklad.Pay.Pay pay)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirPay"));
                //if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Подстановки - некоторые поля надо заполнить, если они не заполены
                //pay.Substitute();

                #endregion


                #region Сохранение

                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        doc = await Task.Run(() => mPutPostPay(db, pay, EntityState.Modified, field)); //sysSetting
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
                sysJourDisp.TableFieldID = pay.DocCashBankID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    Payment = doc.Payment
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DirPay/5
        [ResponseType(typeof(Models.Sklad.Pay.Pay))]
        public async Task<IHttpActionResult> DeleteDirPay(int id, HttpRequestMessage request)
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
            //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirPay"));
            //if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

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
            //_params.DocCashBankID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocCashBankID", true) == 0).Value); //Номер документа
            _params.DirPaymentTypeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPaymentTypeID", true) == 0).Value); //Тип документа, что бы знать где делать выборку: в Кассе или в Банке

            #endregion


            #region Удаление

            try
            {
                int? iDocID = 0;
                

                iDocID = await Task.Run(() => mDeletePay(db, _params.DirPaymentTypeID, id)); //sysSetting


                #region 3. Сколько реально оплатили (Docs.Payment)

                Models.Sklad.Doc.Doc doc = await Task.Run(() => db.Docs.FindAsync(iDocID));

                #endregion


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
                    Payment = doc.Payment,
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

        internal async Task<Models.Sklad.Doc.Doc> mPutPostPay(
            DbConnectionSklad db,
            Models.Sklad.Pay.Pay pay,
            EntityState entityState, //EntityState.Added, Modified
            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            #region 1. По DocID получить: Тип документа, Кассу, Банк, ...

            int
                DocXID = 0, //реальный ID-ник документа
                ListObjectID = 0, //Тип документа
                DirCashOfficeSumTypeID = 0, DirBankSumTypeID = 0, //Тип операции (вносится в коде программы)
                DirCashOfficeID = 0, DirBankID = 0; //Касса и Банк

            var query = await Task.Run(() =>
                (
                    from x in db.Docs
                    where x.DocID == pay.DocID
                    select new
                    {
                        ListObjectID = x.listObject.ListObjectID,
                    }
                ).ToListAsync());

            if (query.Count() > 0)
            {
                //PaySign = query[0].PaySign;
                ListObjectID = query[0].ListObjectID;

                switch (ListObjectID)
                {
                    case 6:

                        #region DocPurches

                        DirCashOfficeSumTypeID = 4;
                        DirBankSumTypeID = 3;

                        //Получаем Кассу и Банк
                        var query6 = await Task.Run(() =>
                            (
                                from x in db.DocPurches
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocPurchID
                                }
                            ).ToListAsync());

                        if (query6.Count() > 0)
                        {
                            DirCashOfficeID= Convert.ToInt32(query6[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query6[0].DirBankID);
                            DocXID = Convert.ToInt32(query6[0].DocXID);
                        }

                        #endregion

                        break;

                    case 32:

                        #region DocSales

                        DirCashOfficeSumTypeID = 6;
                        DirBankSumTypeID = 5;

                        //Получаем Кассу и Банк
                        var query32 = await Task.Run(() =>
                            (
                                from x in db.DocSales
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocSaleID
                                }
                            ).ToListAsync());

                        if (query32.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query32[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query32[0].DirBankID);
                            DocXID = Convert.ToInt32(query32[0].DocXID);
                        }

                        #endregion

                        break;

                    case 34:

                        #region DocReturnVendors

                        DirCashOfficeSumTypeID = 8;
                        DirBankSumTypeID = 7;

                        //Получаем Кассу и Банк
                        var query34 = await Task.Run(() =>
                            (
                                from x in db.DocReturnVendors
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocReturnVendorID
                                }
                            ).ToListAsync());

                        if (query34.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query34[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query34[0].DirBankID);
                            DocXID = Convert.ToInt32(query34[0].DocXID);
                        }

                        #endregion

                        break;

                    case 36:

                        #region DocReturnsCustomers

                        DirCashOfficeSumTypeID = 10;
                        DirBankSumTypeID = 9;

                        //Получаем Кассу и Банк
                        var query36 = await Task.Run(() =>
                            (
                                from x in db.DocReturnsCustomers
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocReturnsCustomerID
                                }
                            ).ToListAsync());

                        if (query36.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query36[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query36[0].DirBankID);
                            DocXID = Convert.ToInt32(query36[0].DocXID);
                        }

                        #endregion

                        break;

                    case 37:

                        #region DocActOnWorks

                        DirCashOfficeSumTypeID = 12;
                        DirBankSumTypeID = 11;

                        //Получаем Кассу и Банк
                        var query37 = await Task.Run(() =>
                            (
                                from x in db.DocActOnWorks
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocActOnWorkID
                                }
                            ).ToListAsync());

                        if (query37.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query37[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query37[0].DirBankID);
                            DocXID = Convert.ToInt32(query37[0].DocXID);
                        }

                        #endregion

                        break;

                        /*
                    case 38:

                        #region DocAccounts

                        //Если Счет оплачиваем в Кассу выдать исключение. Т.к. по счету только безналичный расчет.
                        if (pay.DirPaymentTypeID == 1)
                        {
                            throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg120);
                        }

                        DirCashOfficeSumTypeID = 0;
                        DirBankSumTypeID = 3;

                        //Получаем Кассу и Банк
                        var query38 = await Task.Run(() =>
                            (
                                from x in db.DocAccounts
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocAccountID
                                }
                            ).ToListAsync());

                        if (query38.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query38[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query38[0].DirBankID);
                            DocXID = Convert.ToInt32(query38[0].DocXID);
                        }

                        #endregion

                        break;
                        */

                    case 40:

                        #region DocServicePurches

                        DirCashOfficeSumTypeID = 14;
                        DirBankSumTypeID = 13;

                        //Получаем Кассу и Банк
                        var query40 = await Task.Run(() =>
                            (
                                from x in db.DocServicePurches
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocServicePurchID
                                }
                            ).ToListAsync());

                        if (query40.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query40[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query40[0].DirBankID);
                            DocXID = Convert.ToInt32(query40[0].DocXID);
                        }

                        #endregion

                        break;

                    case 56:

                        #region DocRetails

                        DirCashOfficeSumTypeID = 16;
                        DirBankSumTypeID = 15;

                        //Получаем Кассу и Банк
                        var query56 = await Task.Run(() =>
                            (
                                from x in db.DocRetails
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocRetailID
                                }
                            ).ToListAsync());

                        if (query56.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query56[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query56[0].DirBankID);
                            DocXID = Convert.ToInt32(query56[0].DocXID);
                        }
                        
                        /*
                        DirCashOfficeID = Convert.ToInt32(pay.DirCashOfficeID);
                        DirBankID = Convert.ToInt32(pay.DirBankID);
                        DocXID = Convert.ToInt32(pay.DocXID);
                        */
                        #endregion

                        break;

                    case 57:

                        #region DocRetailReturns

                        DirCashOfficeSumTypeID = 18;
                        DirBankSumTypeID = 17;

                        //Получаем Кассу и Банк
                        var query57 = await Task.Run(() =>
                            (
                                from x in db.DocRetailReturns
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocRetailReturnID
                                }
                            ).ToListAsync());

                        if (query57.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query57[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query57[0].DirBankID);
                            DocXID = Convert.ToInt32(query57[0].DocXID);
                        }

                        #endregion

                        break;

                    case 66:

                        #region DocSecondHandRetails

                        DirCashOfficeSumTypeID = 23; //20;
                        DirBankSumTypeID = 21; // 19;

                        //Получаем Кассу и Банк
                        /*
                        var query66 = await Task.Run(() =>
                            (
                                from x in db.DocSecondHandRetails
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocSecondHandRetailID
                                }
                            ).ToListAsync());
                        */
                        var query66 = await Task.Run(() =>
                            (
                                from x in db.DocSecondHandSales
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocSecondHandSaleID
                                }
                            ).ToListAsync());

                        if (query66.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query66[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query66[0].DirBankID);
                            DocXID = Convert.ToInt32(query66[0].DocXID);
                        }

                        #endregion

                        break;

                    case 67:

                        #region DocSecondHandRetailReturns

                        DirCashOfficeSumTypeID = 25; //22;
                        DirBankSumTypeID = 23; //21;

                        //Получаем Кассу и Банк
                        /*
                        var query67 = await Task.Run(() =>
                            (
                                from x in db.DocSecondHandRetailReturns
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocSecondHandRetailReturnID
                                }
                            ).ToListAsync());
                        */
                        var query67 = await Task.Run(() =>
                            (
                                from x in db.DocSecondHandReturns
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocSecondHandReturnID
                                }
                            ).ToListAsync());
                        if (query67.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query67[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query67[0].DirBankID);
                            DocXID = Convert.ToInt32(query67[0].DocXID);
                        }

                        #endregion

                        break;

                    case 70:

                        #region DocDomesticExpenses

                        DirCashOfficeSumTypeID = 27;
                        DirBankSumTypeID = 25;

                        //Получаем Кассу и Банк
                        var query70 = await Task.Run(() =>
                            (
                                from x in db.DocDomesticExpenses
                                where x.DocID == pay.DocID
                                select new
                                {
                                    DirCashOfficeID = x.dirWarehouse.dirCashOffice.DirCashOfficeID,
                                    DirBankID = x.dirWarehouse.dirBank.DirBankID,
                                    DocXID = x.DocDomesticExpenseID
                                }
                            ).ToListAsync());

                        if (query70.Count() > 0)
                        {
                            DirCashOfficeID = Convert.ToInt32(query70[0].DirCashOfficeID);
                            DirBankID = Convert.ToInt32(query70[0].DirBankID);
                            DocXID = Convert.ToInt32(query70[0].DocXID);
                        }

                        #endregion

                        break;


                    default: throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg118 + pay.DocID + " (ListObjectID=" + ListObjectID + ") " + Classes.Language.Sklad.Language.msg118_1);
                }
            }
            else
            {
                throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg118 + pay.DocID + Classes.Language.Sklad.Language.msg118_1);
            }

            #endregion


            #region 2. Сохраняем в Кассу или Банк

            if (pay.DirPaymentTypeID == 1)
            {
                #region Касса

                //Заполняем Модель
                Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                docCashOfficeSum.DocCashOfficeSumID = null; //pay.DocCashBankID;

                docCashOfficeSum.DirCashOfficeID = DirCashOfficeID; //iDirCashOfficeID;
                docCashOfficeSum.DirCashOfficeSumTypeID = DirCashOfficeSumTypeID; //Тип операции

                DateTime DocXSumDate = Convert.ToDateTime(pay.DocXSumDate);
                docCashOfficeSum.DocCashOfficeSumDate = DocXSumDate;
                docCashOfficeSum.DateOnly = Convert.ToDateTime(DocXSumDate.ToString("yyyy-MM-dd"));

                docCashOfficeSum.DocID = pay.DocID;
                docCashOfficeSum.DocXID = DocXID; // pay.DocXID;
                docCashOfficeSum.DocCashOfficeSumSum = Convert.ToDouble(pay.DocXSumSum); //Минусуем, т.к. берём деньги из кассы за товар
                docCashOfficeSum.Description = "";
                docCashOfficeSum.DirEmployeeID = field.DirEmployeeID; //pay.DirEmployeeID; 
                docCashOfficeSum.DirCurrencyID = pay.DirCurrencyID;
                docCashOfficeSum.DirCurrencyRate = pay.DirCurrencyRate;
                docCashOfficeSum.DirCurrencyMultiplicity = pay.DirCurrencyMultiplicity;
                docCashOfficeSum.Base = pay.Base;
                docCashOfficeSum.KKMSCheckNumber = pay.KKMSCheckNumber;
                docCashOfficeSum.KKMSIdCommand = pay.KKMSIdCommand;
                docCashOfficeSum.KKMSEMail = pay.KKMSEMail;
                docCashOfficeSum.KKMSPhone = pay.KKMSPhone;

                //Пишем в Кассу
                Doc.DocCashOfficeSums.DocCashOfficeSumsController docCashOfficeSumsController = new Doc.DocCashOfficeSums.DocCashOfficeSumsController();
                docCashOfficeSum = await Task.Run(() => docCashOfficeSumsController.mPutPostDocCashOfficeSum_2(db, docCashOfficeSum, EntityState.Added));

                #endregion
            }
            else if (pay.DirPaymentTypeID == 2)
            {
                #region Банк
                
                //1. Заполняем модель "DocBankSum"
                Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                docBankSum.DocBankSumID = null; //pay.DocCashBankID;

                docBankSum.DirBankID = DirBankID; //iDirCashOfficeID;
                docBankSum.DirBankSumTypeID = DirBankSumTypeID; //Тип операции

                DateTime DocXSumDate = Convert.ToDateTime(pay.DocXSumDate);
                docBankSum.DocBankSumDate = DocXSumDate;
                docBankSum.DateOnly = Convert.ToDateTime(DocXSumDate.ToString("yyyy-MM-dd"));

                docBankSum.DocID = pay.DocID;
                docBankSum.DocXID = DocXID; // pay.DocXID;
                docBankSum.DocBankSumSum = Convert.ToDouble(pay.DocXSumSum); //Минусуем, т.к. берём деньги из банка за товар
                docBankSum.Description = "";
                docBankSum.DirEmployeeID = pay.DirEmployeeID; //field.DirEmployeeID;
                docBankSum.DirCurrencyID = pay.DirCurrencyID;
                docBankSum.DirCurrencyRate = pay.DirCurrencyRate;
                docBankSum.DirCurrencyMultiplicity = pay.DirCurrencyMultiplicity;
                docBankSum.Base = pay.Base;
                docBankSum.KKMSCheckNumber = pay.KKMSCheckNumber;
                docBankSum.KKMSIdCommand = pay.KKMSIdCommand;
                docBankSum.KKMSEMail = pay.KKMSEMail;
                docBankSum.KKMSPhone = pay.KKMSPhone;

                //2. Пишем в Кассу
                Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum_2(db, docBankSum, EntityState.Added));

                #endregion
            }
            else
            {
                throw new System.InvalidOperationException("Не выбран метод оплаты: Касса или Банк!");
            }

            #endregion


            #region 3. Сколько реально оплатили (Docs.Payment)

            Models.Sklad.Doc.Doc doc = await Task.Run(() => db.Docs.FindAsync(pay.DocID));

            #endregion


            return doc;
        }

        internal async Task<int?> mDeletePay(
            DbConnectionSklad db,
            int DirPaymentTypeID,
            int id
            )
        {
            int? iDocID = 0;

            if (DirPaymentTypeID == 1)
            {
                //Касса
                Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = await Task.Run(() => db.DocCashOfficeSums.FindAsync(id));
                iDocID = docCashOfficeSum.DocID;
                db.DocCashOfficeSums.Remove(docCashOfficeSum);
                await db.SaveChangesAsync();
            }
            else if (DirPaymentTypeID == 2)
            {
                //Банк
                Models.Sklad.Doc.DocBankSum docBankSum = await Task.Run(() => db.DocBankSums.FindAsync(id));
                iDocID = docBankSum.DocID;
                db.DocBankSums.Remove(docBankSum);
                await db.SaveChangesAsync();
            }
            else
            {
                throw new System.InvalidOperationException("Не выбран метод оплаты: Касса или Банк!");
            }


            //Есть оплаты или нет
            Models.Sklad.Doc.Doc doc = await Task.Run(() => db.Docs.FindAsync(iDocID));
            if (doc.Payment == 0)
            {
                //Если нет оплат, то установить: DocDatePayment = Docs.DocDate
                doc.DocDatePayment = doc.DocDate;

                db.Entry(doc).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            else
            {
                //Если есть оплата(ы), то получить дату последней оплаты
                var queryUnion = 
                    (
                        from x in db.DocCashOfficeSums
                        where x.DocID == iDocID
                        select new
                        {
                            XDate = x.DocCashOfficeSumDate
                        }
                    ).Union
                    (
                        from x in db.DocBankSums
                        where x.DocID == iDocID
                        select new
                        {
                            XDate = x.DocBankSumDate
                        }
                    );

                var queryMax = await queryUnion.MaxAsync(y => y.XDate);
                if (queryMax.Value != null)
                {
                    doc.DocDatePayment = queryMax.Value;
                }
                else
                {
                    doc.DocDatePayment = doc.DocDate;
                }

                db.Entry(doc).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }


            return iDocID;
        }

        #endregion
    }
}
