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
using System.Collections;

namespace PartionnyAccount.Controllers.Sklad.Doc.DocServicePurches
{
    public class DocServicePurch1TabsController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Log.LogService logService = new Models.Sklad.Log.LogService(); Controllers.Sklad.Log.LogServicesController logServicesController = new Log.LogServicesController();
        private DbConnectionSklad db = new DbConnectionSklad();

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            public int DocServicePurchID;
            public int DocID;
        }
        // GET: api/DocServicePurch1Tabs
        public async Task<IHttpActionResult> GetDocServicePurch1Tabs(HttpRequestMessage request)
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
                /*
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurchs"));
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
                _params.DocServicePurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocServicePurchID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.DocServicePurch1Tabs

                        /*
                        join dirServiceJobNomens1 in db.DirServiceJobNomens on x.DirServiceJobNomenID equals dirServiceJobNomens1.DirServiceJobNomenID into dirServiceJobNomens2
                        from dirServiceJobNomens in dirServiceJobNomens2.DefaultIfEmpty()
                        */

                        join dirServiceJobNomens11 in db.DirServiceJobNomens on x.dirServiceJobNomen.Sub equals dirServiceJobNomens11.DirServiceJobNomenID into dirServiceJobNomens12
                        from dirServiceJobNomensSubGroup in dirServiceJobNomens12.DefaultIfEmpty()

                        join dirServiceJobNomens112 in db.DirServiceJobNomens on dirServiceJobNomensSubGroup.Sub equals dirServiceJobNomens112.DirServiceJobNomenID into dirServiceJobNomens122
                        from dirServiceJobNomensSubGroup2 in dirServiceJobNomens122.DefaultIfEmpty()

                        where x.DocServicePurchID == _params.DocServicePurchID

                        #region select

                        select new
                        {
                            DocServicePurch1TabID = x.DocServicePurch1TabID,
                            DocServicePurchID = x.DocServicePurchID,

                            DirEmployeeID = x.DirEmployeeID,
                            DirEmployeeName = x.dirEmployee.DirEmployeeName,

                            //Выполненная работа
                            DirServiceJobNomenID = x.DirServiceJobNomenID,

                            DirServiceJobNomenName =
                            x.DirServiceJobNomenID == null ? x.DirServiceJobNomenName
                            :
                            //dirServiceJobNomens.DirServiceJobNomenName,
                            (
                                dirServiceJobNomensSubGroup2.DirServiceJobNomenName == null ? 
                                (
                                    dirServiceJobNomensSubGroup.DirServiceJobNomenName == null ? x.dirServiceJobNomen.DirServiceJobNomenName
                                    :
                                    dirServiceJobNomensSubGroup.DirServiceJobNomenName + " / " + x.dirServiceJobNomen.DirServiceJobNomenName 
                                ) 
                                :
                                (
                                    dirServiceJobNomensSubGroup2.DirServiceJobNomenName + " / " + dirServiceJobNomensSubGroup.DirServiceJobNomenName + " / " + x.dirServiceJobNomen.DirServiceJobNomenName
                                )
                            ),


                            PriceVAT = x.PriceVAT,
                            PriceCurrency = x.PriceCurrency,

                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",

                            DiagnosticRresults = x.DiagnosticRresults,

                            TabDate = x.TabDate,
                            DirServiceStatusID = x.DirServiceStatusID,

                            PayDate = x.PayDate,
                            RemontN = x.RemontN,
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocServicePurch1Tab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocServicePurch1Tabs/5
        [ResponseType(typeof(DocServicePurch1Tab))]
        public async Task<IHttpActionResult> GetDocServicePurch1Tab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocServicePurch1Tabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocServicePurch1Tab(int id, DocServicePurch1Tab docServicePurch1Tab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocServicePurch1Tabs
        [ResponseType(typeof(DocServicePurch1Tab))]
        public async Task<IHttpActionResult> PostDocServicePurch1Tab(DocServicePurch1Tab docServicePurch1Tab, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurch1Tabs"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));


                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                var paramList = request.GetQueryNameValuePairs();
                int iDirServiceStatusID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceStatusID", true) == 0).Value);
                string sDiagnosticRresults = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sDiagnosticRresults", true) == 0).Value;

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocServicePurch1Tab[] DocServicePurch1TabCollection = null;
                if (!String.IsNullOrEmpty(docServicePurch1Tab.recordsDataX))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    DocServicePurch1TabCollection = serializer.Deserialize<Models.Sklad.Doc.DocServicePurch1Tab[]>("[" + docServicePurch1Tab.recordsDataX + "]");
                }

                if (DocServicePurch1TabCollection.Length > 0)
                {
                    docServicePurch1Tab = DocServicePurch1TabCollection[0];
                    docServicePurch1Tab.PriceVAT = docServicePurch1Tab.PriceCurrency;
                    if (docServicePurch1Tab.DirServiceJobNomenID == 0) docServicePurch1Tab.DirServiceJobNomenID = null;
                    if (docServicePurch1Tab.DocServicePurch1TabID == null || docServicePurch1Tab.DocServicePurch1TabID < 1)
                    {
                        docServicePurch1Tab.DocServicePurch1TabID = null;
                        docServicePurch1Tab.DirEmployeeID = field.DirEmployeeID;
                        docServicePurch1Tab.DirCurrencyID = sysSetting.DirCurrencyID;
                        docServicePurch1Tab.DirCurrencyRate = 1;
                        docServicePurch1Tab.DirCurrencyMultiplicity = 1;

                        if (docServicePurch1Tab.DirServiceJobNomenID == null)
                        {
                            bool bRight = await Task.Run(() => accessRight.AccessCheck(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceWorkshopsTab1AddCheck"));
                            if (!bRight) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                        }
                    }
                }

                //Проверка точки === === ===
                Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch1Tab.DocServicePurchID);
                if (field.DirEmployeeID != 1)
                {
                    int? DirWarehouseID = docServicePurch.DirWarehouseID;
                    var query = await
                        (
                            from x in db.DirEmployeeWarehouse
                            where x.DirEmployeeID == field.DirEmployeeID && x.DirWarehouseID == DirWarehouseID
                            select x
                        ).ToListAsync();
                    if (query.Count() == 0)
                    {
                        return Ok(returnServer.Return(false, "У Вас нет доступа к этой точке!"));
                    }
                }

                #endregion


                #region Сохранение


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        #region Save docServicePurch

                        //Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch1Tab.DocServicePurchID);
                        int? DirServiceStatusIDOLD = docServicePurch.DirServiceStatusID;
                        docServicePurch.DirServiceStatusID = iDirServiceStatusID;
                        db.Entry(docServicePurch).State = EntityState.Modified;


                        #region Лог - если поменялся статус

                        if (DirServiceStatusIDOLD != iDirServiceStatusID)
                        {
                            //Пишем в Лог о смене статуса и мастера, если такое было
                            logService.DocServicePurchID = docServicePurch1Tab.DocServicePurchID;
                            logService.DirServiceLogTypeID = 1;
                            logService.DirEmployeeID = field.DirEmployeeID;
                            logService.DirServiceStatusID = iDirServiceStatusID;
                            if(!String.IsNullOrEmpty(sDiagnosticRresults)) logService.Msg = sDiagnosticRresults;

                            await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);
                        }

                        #endregion

                        #endregion


                        #region Save docServicePurch1Tab


                        #region Лог

                        //Пишем в Лог о смене статуса и мастера, если такое было
                        logService.DocServicePurchID = docServicePurch1Tab.DocServicePurchID;
                        logService.DirServiceLogTypeID = 5;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirServiceStatusID = iDirServiceStatusID;

                        if (docServicePurch1Tab.DocServicePurch1TabID == null) logService.Msg = "Создание записи " + docServicePurch1Tab.DirServiceJobNomenName + " на сумму " + docServicePurch1Tab.PriceCurrency;
                        else logService.Msg = "Изменение записи " + docServicePurch1Tab.DirServiceJobNomenName + " на сумму " + docServicePurch1Tab.PriceCurrency;
                        if (!String.IsNullOrEmpty(sDiagnosticRresults)) logService.Msg += "<br /> Результат Диагностики: " +  sDiagnosticRresults;

                        await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

                        #endregion

                        docServicePurch1Tab.DirServiceStatusID = iDirServiceStatusID;

                        if (docServicePurch1Tab.DocServicePurch1TabID > 0)
                        {
                            db.Entry(docServicePurch1Tab).State = EntityState.Modified;
                        }
                        else
                        {
                            docServicePurch1Tab.DiagnosticRresults = sDiagnosticRresults;
                            docServicePurch1Tab.TabDate = DateTime.Now;

                            db.Entry(docServicePurch1Tab).State = EntityState.Added;
                        }
                        await Task.Run(() => db.SaveChangesAsync());

                        #endregion


                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }







                dynamic collectionWrapper = new
                {
                    DocServicePurch1TabID = docServicePurch1Tab.DocServicePurch1TabID,
                    DirEmployeeID = docServicePurch1Tab.DirEmployeeID,
                    DirCurrencyID = docServicePurch1Tab.DirCurrencyID,
                    DirCurrencyRate = docServicePurch1Tab.DirCurrencyRate,
                    DirCurrencyMultiplicity = docServicePurch1Tab.DirCurrencyMultiplicity
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocServicePurch1Tabs/5
        [ResponseType(typeof(DocServicePurch1Tab))]
        public async Task<IHttpActionResult> DeleteDocServicePurch1Tab(int id, HttpRequestMessage request)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurch1Tabs"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            var paramList = request.GetQueryNameValuePairs();
            int iDirServiceStatusID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceStatusID", true) == 0).Value);
            string sDiagnosticRresults = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sDiagnosticRresults", true) == 0).Value;

            #endregion


            #region Удаление

            try
            {
                DocServicePurch1Tab docServicePurch1Tab = await db.DocServicePurch1Tabs.FindAsync(id);
                if (docServicePurch1Tab == null)
                {
                    //return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                    dynamic collectionWrapper2 = new
                    {
                        ID = 0  ,
                        Msg = Classes.Language.Sklad.Language.msg99
                    };
                    return Ok(returnServer.Return(true, collectionWrapper2));
                }

                //Проверка точки === === ===
                Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch1Tab.DocServicePurchID);
                if (field.DirEmployeeID != 1)
                {
                    int? DirWarehouseID = docServicePurch.DirWarehouseID;
                    var query = await
                        (
                            from x in db.DirEmployeeWarehouse
                            where x.DirEmployeeID == field.DirEmployeeID && x.DirWarehouseID == DirWarehouseID
                            select x
                        ).ToListAsync();
                    if (query.Count() == 0)
                    {
                        return Ok(returnServer.Return(false, "У Вас нет доступа к этой точке!"));
                    }
                }


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        #region Лог

                        //Пишем в Лог о смене статуса и мастера, если такое было
                        logService.DocServicePurchID = docServicePurch1Tab.DocServicePurchID;
                        logService.DirServiceLogTypeID = 5;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirServiceStatusID = null;
                        logService.Msg = "Удаление записи " + docServicePurch1Tab.DirServiceJobNomenName + " на сумму " + docServicePurch1Tab.PriceCurrency;
                        logService.Msg += "<br />Причина удаление: " + sDiagnosticRresults;

                        await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

                        #endregion

                        #region Save

                        db.DocServicePurch1Tabs.Remove(docServicePurch1Tab);
                        await db.SaveChangesAsync();

                        #endregion

                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }


                dynamic collectionWrapper = new
                {
                    ID = docServicePurch1Tab.DocServicePurch1TabID,
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

        private bool DocServicePurch1TabExists(int id)
        {
            return db.DocServicePurch1Tabs.Count(e => e.DocServicePurch1TabID == id) > 0;
        }

        #endregion



        #region SQL

        //Спецификация - Не Используется!!!
        internal string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";

            //Используем оператор "UNION ALL"

            SQL =

                //DocServicePurch1Tabs *** *** ***

                "SELECT " +
                "'Работа' AS TabNum, " + 
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                "[DocServicePurch1Tabs].[DirServiceJobNomenName] || ''' || [DocServicePurch1Tabs].[DiagnosticRresults] || ''' || AS [DirNomenName], " + 
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocServicePurch1Tabs].[DirCurrencyRate] || ', ' || [DocServicePurch1Tabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocServicePurch1Tabs].[PriceVAT] AS [PriceVAT], " +
                "[DocServicePurch1Tabs].[PriceCurrency] AS [PriceCurrency], " +
                "[DocServicePurch1Tabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocServicePurch1Tabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                //"[DirEmployees].[DirEmployeeName] AS [DirEmployeeName], " + 

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocServicePurches] " +
                "INNER JOIN [DocServicePurch1Tabs] AS [DocServicePurch1Tabs] ON [DocServicePurch1Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID] " +
                "INNER JOIN [DirServiceJobNomens] AS [DirServiceJobNomens] ON [DirServiceJobNomens].[DirServiceJobNomenID] = [DocServicePurch1Tabs].[DirServiceJobNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocServicePurch1Tabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                //"INNER JOIN [DirEmployees] ON [DocServicePurches].[DirEmployeeID] = [DirEmployees].[DirEmployeeID] " +

                "WHERE ([Docs].[DocID]=[DocServicePurches].[DocID])and(Docs.DocID=@DocID) " +


                "UNION ALL " +


                //DocServicePurch2Tabs *** *** ***

                "SELECT " +
                "'Запчасть' AS TabNum, " +
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                "[DirNomens].[DirNomenName] AS [DirNomenName], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocServicePurch2Tabs].[DirCurrencyRate] || ', ' || [DocServicePurch2Tabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocServicePurch2Tabs].[PriceVAT] AS [PriceVAT], " +
                "[DocServicePurch2Tabs].[PriceCurrency] AS [PriceCurrency], " +
                "[DocServicePurch2Tabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocServicePurch2Tabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocServicePurches] " +
                "INNER JOIN [DocServicePurch2Tabs] AS [DocServicePurch2Tabs] ON [DocServicePurch2Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID] " +
                "INNER JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocServicePurch2Tabs].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocServicePurch2Tabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " + 

                "WHERE ([Docs].[DocID]=[DocServicePurches].[DocID])and(Docs.DocID=@DocID) ";

            return SQL;
        }

        internal string GenerateSQLSelectCollection1(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";

            //Используем оператор "UNION ALL"

            SQL =

                //DocServicePurch1Tabs *** *** ***

                "SELECT " +
                "'Работа' AS TabNum, " +
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                //"[DocServicePurch1Tabs].[DirServiceJobNomenName] || ' (' || [DocServicePurch1Tabs].[DiagnosticRresults] || ') ' AS [DirNomenName], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocServicePurch1Tabs].[DirCurrencyRate] || ', ' || [DocServicePurch1Tabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocServicePurch1Tabs].[PriceVAT] AS [PriceVAT], " +
                "[DocServicePurch1Tabs].[PriceCurrency] AS [PriceCurrency], " +
                "[DocServicePurch1Tabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocServicePurch1Tabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirEmployees].[DirEmployeeName] AS [DirEmployeeName], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty, " +



                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                " CASE " +
                "   WHEN dirServiceJobNomensSubGroup.DirServiceJobNomenID IS NULL THEN " +

                "      CASE WHEN dirServiceJobNomensGroup.DirServiceJobNomenID IS NULL THEN " +

                "           CASE WHEN DirServiceJobNomens.DirServiceJobNomenID IS NULL THEN " +

                "                CASE WHEN [DocServicePurch1Tabs].[DiagnosticRresults] IS NULL THEN[DocServicePurch1Tabs].[DirServiceJobNomenName] " +
                "                ELSE [DocServicePurch1Tabs].[DirServiceJobNomenName]  || ' (' || [DocServicePurch1Tabs].[DiagnosticRresults] || ') ' " +
                "                END " +

                "           ELSE " +
                "             CASE WHEN[DocServicePurch1Tabs].[DiagnosticRresults] IS NULL THEN DirServiceJobNomens.DirServiceJobNomenName " +
                "             ELSE DirServiceJobNomens.DirServiceJobNomenName  || ' (' || [DocServicePurch1Tabs].[DiagnosticRresults] || ') ' " +
                "             END " +
                "           END " +

                "       ELSE " +
                "           CASE WHEN[DocServicePurch1Tabs].[DiagnosticRresults] IS NULL THEN dirServiceJobNomensGroup.DirServiceJobNomenName || ' / ' || DirServiceJobNomens.DirServiceJobNomenName " +
                "           ELSE dirServiceJobNomensGroup.DirServiceJobNomenName || ' / ' || DirServiceJobNomens.DirServiceJobNomenName  || ' (' || [DocServicePurch1Tabs].[DiagnosticRresults] || ') ' " +
                "           END " +
                "       END " +

                "   ELSE " +
                "       CASE WHEN[DocServicePurch1Tabs].[DiagnosticRresults] IS NULL THEN dirServiceJobNomensSubGroup.DirServiceJobNomenName || ' / ' || dirServiceJobNomensGroup.DirServiceJobNomenName || ' / ' || DirServiceJobNomens.DirServiceJobNomenName " +
                "       ELSE dirServiceJobNomensSubGroup.DirServiceJobNomenName || ' / ' || dirServiceJobNomensGroup.DirServiceJobNomenName || ' / ' || DirServiceJobNomens.DirServiceJobNomenName  || ' (' || [DocServicePurch1Tabs].[DiagnosticRresults] || ') ' " +
                "   END " +
                " END  " +
                "AS [DirNomenName] " +



                "FROM [Docs], [DocServicePurches] " +
                "INNER JOIN [DocServicePurch1Tabs] AS [DocServicePurch1Tabs] ON [DocServicePurch1Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID]  and [DocServicePurch1Tabs].RemontN=(SELECT MAX(RemontN) FROM [DocServicePurch1Tabs] WHERE [DocServicePurch1Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID]) " + // and [DocServicePurch1Tabs].PayDate>=(SELECT MAX(PayDate) FROM [DocServicePurch1Tabs] WHERE [DocServicePurch1Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID])  
                "INNER JOIN [DirCurrencies] ON [DocServicePurch1Tabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirEmployees] ON [DocServicePurch1Tabs].[DirEmployeeID] = [DirEmployees].[DirEmployeeID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceJobNomens] AS [DirServiceJobNomens] ON [DirServiceJobNomens].[DirServiceJobNomenID] = [DocServicePurch1Tabs].[DirServiceJobNomenID] " +    //Товар
                "LEFT JOIN [DirServiceJobNomens] AS [dirServiceJobNomensGroup] ON [DirServiceJobNomens].[Sub] = [dirServiceJobNomensGroup].[DirServiceJobNomenID] " +            //Под-Группа
                "LEFT JOIN [DirServiceJobNomens] AS [dirServiceJobNomensSubGroup] ON [dirServiceJobNomensGroup].[Sub] = [dirServiceJobNomensSubGroup].[DirServiceJobNomenID] " + //Группа
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "WHERE ([Docs].[DocID]=[DocServicePurches].[DocID])and(Docs.DocID=@DocID) ";
                

            return SQL;
        }

        #endregion
    }
}