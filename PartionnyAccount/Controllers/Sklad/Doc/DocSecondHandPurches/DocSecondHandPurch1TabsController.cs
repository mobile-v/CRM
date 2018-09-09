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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandPurches
{
    public class DocSecondHandPurch1TabsController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Log.LogSecondHand logSecondHand = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logSecondHandsController = new Log.LogSecondHandsController();
        private DbConnectionSklad db = new DbConnectionSklad();

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            public int DocSecondHandPurchID;
            public int DocID;
        }
        // GET: api/DocSecondHandPurch1Tabs
        public async Task<IHttpActionResult> GetDocSecondHandPurch1Tabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurchs"));
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
                _params.DocSecondHandPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandPurchID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.DocSecondHandPurch1Tabs

                            /*
                            join dirServiceJobNomens1 in db.DirServiceJobNomens on x.DirServiceJobNomenID equals dirServiceJobNomens1.DirServiceJobNomenID into dirServiceJobNomens2
                            from dirServiceJobNomens in dirServiceJobNomens2.DefaultIfEmpty()
                            */

                        join dirServiceJobNomens11 in db.DirServiceJobNomens on x.dirServiceJobNomen.Sub equals dirServiceJobNomens11.DirServiceJobNomenID into dirServiceJobNomens12
                        from dirServiceJobNomensSubGroup in dirServiceJobNomens12.DefaultIfEmpty()

                        join dirServiceJobNomens112 in db.DirServiceJobNomens on dirServiceJobNomensSubGroup.Sub equals dirServiceJobNomens112.DirServiceJobNomenID into dirServiceJobNomens122
                        from dirServiceJobNomensSubGroup2 in dirServiceJobNomens122.DefaultIfEmpty()

                        where x.DocSecondHandPurchID == _params.DocSecondHandPurchID

                        #region select

                        select new
                        {
                            DocSecondHandPurch1TabID = x.DocSecondHandPurch1TabID,
                            DocSecondHandPurchID = x.DocSecondHandPurchID,

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
                            DirSecondHandStatusID = x.DirSecondHandStatusID,

                            PayDate = x.PayDate,
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSecondHandPurch1Tab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandPurch1Tabs/5
        [ResponseType(typeof(DocSecondHandPurch1Tab))]
        public async Task<IHttpActionResult> GetDocSecondHandPurch1Tab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandPurch1Tabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandPurch1Tab(int id, DocSecondHandPurch1Tab docSecondHandPurch1Tab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandPurch1Tabs
        [ResponseType(typeof(DocSecondHandPurch1Tab))]
        public async Task<IHttpActionResult> PostDocSecondHandPurch1Tab(DocSecondHandPurch1Tab docSecondHandPurch1Tab, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurch1Tabs"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                var paramList = request.GetQueryNameValuePairs();
                int iDirSecondHandStatusID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirSecondHandStatusID", true) == 0).Value);
                string sDiagnosticRresults = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sDiagnosticRresults", true) == 0).Value;

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocSecondHandPurch1Tab[] DocSecondHandPurch1TabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandPurch1Tab.recordsDataX))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    DocSecondHandPurch1TabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandPurch1Tab[]>("[" + docSecondHandPurch1Tab.recordsDataX + "]");
                }

                if (DocSecondHandPurch1TabCollection.Length > 0)
                {
                    docSecondHandPurch1Tab = DocSecondHandPurch1TabCollection[0];
                    docSecondHandPurch1Tab.PriceVAT = docSecondHandPurch1Tab.PriceCurrency;
                    if (docSecondHandPurch1Tab.DirServiceJobNomenID == 0) docSecondHandPurch1Tab.DirServiceJobNomenID = null;
                    if (docSecondHandPurch1Tab.DocSecondHandPurch1TabID == null || docSecondHandPurch1Tab.DocSecondHandPurch1TabID < 1)
                    {
                        docSecondHandPurch1Tab.DocSecondHandPurch1TabID = null;
                        docSecondHandPurch1Tab.DirEmployeeID = field.DirEmployeeID;
                        docSecondHandPurch1Tab.DirCurrencyID = sysSetting.DirCurrencyID;
                        docSecondHandPurch1Tab.DirCurrencyRate = 1;
                        docSecondHandPurch1Tab.DirCurrencyMultiplicity = 1;
                    }
                }

                #endregion


                #region Сохранение


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        #region Save docSecondHandPurch

                        Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandPurch1Tab.DocSecondHandPurchID);
                        int? DirSecondHandStatusIDOLD = docSecondHandPurch.DirSecondHandStatusID;
                        docSecondHandPurch.DirSecondHandStatusID = iDirSecondHandStatusID;
                        db.Entry(docSecondHandPurch).State = EntityState.Modified;


                        #region Лог - если поменялся статус

                        if (DirSecondHandStatusIDOLD != iDirSecondHandStatusID)
                        {
                            //Пишем в Лог о смене статуса и мастера, если такое было
                            logSecondHand.DocSecondHandPurchID = docSecondHandPurch1Tab.DocSecondHandPurchID;
                            logSecondHand.DirSecondHandLogTypeID = 1;
                            logSecondHand.DirEmployeeID = field.DirEmployeeID;
                            logSecondHand.DirSecondHandStatusID = iDirSecondHandStatusID;
                            if (!String.IsNullOrEmpty(sDiagnosticRresults)) logSecondHand.Msg = sDiagnosticRresults;

                            await logSecondHandsController.mPutPostLogSecondHands(db, logSecondHand, EntityState.Added);
                        }

                        #endregion

                        #endregion


                        #region Save docSecondHandPurch1Tab


                        #region Лог

                        //Пишем в Лог о смене статуса и мастера, если такое было
                        logSecondHand.DocSecondHandPurchID = docSecondHandPurch1Tab.DocSecondHandPurchID;
                        logSecondHand.DirSecondHandLogTypeID = 5;
                        logSecondHand.DirEmployeeID = field.DirEmployeeID;
                        logSecondHand.DirSecondHandStatusID = iDirSecondHandStatusID;

                        if (docSecondHandPurch1Tab.DocSecondHandPurch1TabID == null) logSecondHand.Msg = "Создание записи " + docSecondHandPurch1Tab.DirServiceJobNomenName + " на сумму " + docSecondHandPurch1Tab.PriceCurrency;
                        else logSecondHand.Msg = "Изменение записи " + docSecondHandPurch1Tab.DirServiceJobNomenName + " на сумму " + docSecondHandPurch1Tab.PriceCurrency;
                        if (!String.IsNullOrEmpty(sDiagnosticRresults)) logSecondHand.Msg += "<br /> Результат Диагностики: " + sDiagnosticRresults;

                        await logSecondHandsController.mPutPostLogSecondHands(db, logSecondHand, EntityState.Added);

                        #endregion

                        docSecondHandPurch1Tab.DirSecondHandStatusID = iDirSecondHandStatusID;

                        if (docSecondHandPurch1Tab.DocSecondHandPurch1TabID > 0)
                        {
                            db.Entry(docSecondHandPurch1Tab).State = EntityState.Modified;
                        }
                        else
                        {
                            docSecondHandPurch1Tab.DiagnosticRresults = sDiagnosticRresults;
                            docSecondHandPurch1Tab.TabDate = DateTime.Now;

                            db.Entry(docSecondHandPurch1Tab).State = EntityState.Added;
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
                    DocSecondHandPurch1TabID = docSecondHandPurch1Tab.DocSecondHandPurch1TabID,
                    DirEmployeeID = docSecondHandPurch1Tab.DirEmployeeID,
                    DirCurrencyID = docSecondHandPurch1Tab.DirCurrencyID,
                    DirCurrencyRate = docSecondHandPurch1Tab.DirCurrencyRate,
                    DirCurrencyMultiplicity = docSecondHandPurch1Tab.DirCurrencyMultiplicity
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandPurch1Tabs/5
        [ResponseType(typeof(DocSecondHandPurch1Tab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandPurch1Tab(int id, HttpRequestMessage request)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurch1Tabs"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            var paramList = request.GetQueryNameValuePairs();
            int iDirSecondHandStatusID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirSecondHandStatusID", true) == 0).Value);
            string sDiagnosticRresults = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sDiagnosticRresults", true) == 0).Value;

            #endregion


            #region Удаление

            try
            {
                DocSecondHandPurch1Tab docSecondHandPurch1Tab = await db.DocSecondHandPurch1Tabs.FindAsync(id);
                if (docSecondHandPurch1Tab == null)
                {
                    //return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                    dynamic collectionWrapper2 = new
                    {
                        ID = 0,
                        Msg = Classes.Language.Sklad.Language.msg99
                    };
                    return Ok(returnServer.Return(true, collectionWrapper2));
                }


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        #region Лог

                        //Пишем в Лог о смене статуса и мастера, если такое было
                        logSecondHand.DocSecondHandPurchID = docSecondHandPurch1Tab.DocSecondHandPurchID;
                        logSecondHand.DirSecondHandLogTypeID = 5;
                        logSecondHand.DirEmployeeID = field.DirEmployeeID;
                        logSecondHand.DirSecondHandStatusID = null;
                        logSecondHand.Msg = "Удаление записи " + docSecondHandPurch1Tab.DirServiceJobNomenName + " на сумму " + docSecondHandPurch1Tab.PriceCurrency;
                        logSecondHand.Msg += "<br />Причина удаление: " + sDiagnosticRresults;

                        await logSecondHandsController.mPutPostLogSecondHands(db, logSecondHand, EntityState.Added);

                        #endregion

                        #region Save

                        db.DocSecondHandPurch1Tabs.Remove(docSecondHandPurch1Tab);
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
                    ID = docSecondHandPurch1Tab.DocSecondHandPurch1TabID,
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

        private bool DocSecondHandPurch1TabExists(int id)
        {
            return db.DocSecondHandPurch1Tabs.Count(e => e.DocSecondHandPurch1TabID == id) > 0;
        }

        #endregion



        #region SQL

        //Спецификация - Не Используется!!!
        internal string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";

            //Используем оператор "UNION ALL"

            SQL =

                //DocSecondHandPurch1Tabs *** *** ***

                "SELECT " +
                "'Работа' AS TabNum, " +
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                "[DocSecondHandPurch1Tabs].[DirServiceJobNomenName] || ''' || [DocSecondHandPurch1Tabs].[DiagnosticRresults] || ''' || AS [DirNomenName], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandPurch1Tabs].[DirCurrencyRate] || ', ' || [DocSecondHandPurch1Tabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocSecondHandPurch1Tabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandPurch1Tabs].[PriceCurrency] AS [PriceCurrency], " +
                "[DocSecondHandPurch1Tabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandPurch1Tabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                //"[DirEmployees].[DirEmployeeName] AS [DirEmployeeName], " + 

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocSecondHandPurches] " +
                "INNER JOIN [DocSecondHandPurch1Tabs] AS [DocSecondHandPurch1Tabs] ON [DocSecondHandPurch1Tabs].[DocSecondHandPurchID] = [DocSecondHandPurches].[DocSecondHandPurchID] " +
                "INNER JOIN [DirServiceJobNomens] AS [DirServiceJobNomens] ON [DirServiceJobNomens].[DirServiceJobNomenID] = [DocSecondHandPurch1Tabs].[DirServiceJobNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocSecondHandPurch1Tabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                //"INNER JOIN [DirEmployees] ON [DocSecondHandPurches].[DirEmployeeID] = [DirEmployees].[DirEmployeeID] " +

                "WHERE ([Docs].[DocID]=[DocSecondHandPurches].[DocID])and(Docs.DocID=@DocID) " +


                "UNION ALL " +


                //DocSecondHandPurch2Tabs *** *** ***

                "SELECT " +
                "'Запчасть' AS TabNum, " +
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                "[DirNomens].[DirNomenName] AS [DirNomenName], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandPurch2Tabs].[DirCurrencyRate] || ', ' || [DocSecondHandPurch2Tabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocSecondHandPurch2Tabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandPurch2Tabs].[PriceCurrency] AS [PriceCurrency], " +
                "[DocSecondHandPurch2Tabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandPurch2Tabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocSecondHandPurches] " +
                "INNER JOIN [DocSecondHandPurch2Tabs] AS [DocSecondHandPurch2Tabs] ON [DocSecondHandPurch2Tabs].[DocSecondHandPurchID] = [DocSecondHandPurches].[DocSecondHandPurchID] " +
                "INNER JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocSecondHandPurch2Tabs].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocSecondHandPurch2Tabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +

                "WHERE ([Docs].[DocID]=[DocSecondHandPurches].[DocID])and(Docs.DocID=@DocID) ";

            return SQL;
        }


        internal string GenerateSQLSelectCollection1(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";

            //Используем оператор "UNION ALL"

            SQL =

                //DocSecondHandPurch1Tabs *** *** ***

                "SELECT " +
                "'Работа' AS TabNum, " +
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                //"[DocSecondHandPurch1Tabs].[DirServiceJobNomenName] || ' (' || [DocSecondHandPurch1Tabs].[DiagnosticRresults] || ') ' AS [DirNomenName], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandPurch1Tabs].[DirCurrencyRate] || ', ' || [DocSecondHandPurch1Tabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocSecondHandPurch1Tabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandPurch1Tabs].[PriceCurrency] AS [PriceCurrency], " +
                "[DocSecondHandPurch1Tabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandPurch1Tabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirEmployees].[DirEmployeeName] AS [DirEmployeeName], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty, " +



                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                " CASE " +
                " WHEN dirServiceJobNomensSubGroup.DirServiceJobNomenID IS NULL THEN " +

                "      CASE WHEN dirServiceJobNomensGroup.DirServiceJobNomenID IS NULL THEN " +

                "           CASE WHEN DirServiceJobNomens.DirServiceJobNomenID IS NULL THEN " +

                "                CASE WHEN DirServiceJobNomens.DirServiceJobNomenID IS NULL THEN[DocSecondHandPurch1Tabs].[DirServiceJobNomenName]  || ' (' || [DocSecondHandPurch1Tabs].[DiagnosticRresults] || ') ' " +
                "                END " +

                "           ELSE " +
                "             CASE WHEN[DocSecondHandPurch1Tabs].[DiagnosticRresults] IS '' THEN DirServiceJobNomens.DirServiceJobNomenName " +
                "             ELSE DirServiceJobNomens.DirServiceJobNomenName  || ' (' || [DocSecondHandPurch1Tabs].[DiagnosticRresults] || ') ' " +
                "             END " +
                "           END " +

                "       ELSE " +
                "           CASE WHEN[DocSecondHandPurch1Tabs].[DiagnosticRresults] IS '' THEN dirServiceJobNomensGroup.DirServiceJobNomenName || ' / ' || DirServiceJobNomens.DirServiceJobNomenName " +
                "           ELSE dirServiceJobNomensGroup.DirServiceJobNomenName || ' / ' || DirServiceJobNomens.DirServiceJobNomenName  || ' (' || [DocSecondHandPurch1Tabs].[DiagnosticRresults] || ') ' " +
                "           END " +
                "       END " +

                "   ELSE " +
                "       CASE WHEN[DocSecondHandPurch1Tabs].[DiagnosticRresults] IS '' THEN dirServiceJobNomensSubGroup.DirServiceJobNomenName || ' / ' || dirServiceJobNomensGroup.DirServiceJobNomenName || ' / ' || DirServiceJobNomens.DirServiceJobNomenName " +
                "       ELSE dirServiceJobNomensSubGroup.DirServiceJobNomenName || ' / ' || dirServiceJobNomensGroup.DirServiceJobNomenName || ' / ' || DirServiceJobNomens.DirServiceJobNomenName  || ' (' || [DocSecondHandPurch1Tabs].[DiagnosticRresults] || ') ' " +
                "   END " +
                "END  " +
                "AS [DirNomenName] " +



                "FROM [Docs], [DocSecondHandPurches] " +
                "INNER JOIN [DocSecondHandPurch1Tabs] AS [DocSecondHandPurch1Tabs] ON [DocSecondHandPurch1Tabs].[DocSecondHandPurchID] = [DocSecondHandPurches].[DocSecondHandPurchID] " +
                //"INNER JOIN [DirServiceJobNomens] AS [DirServiceJobNomens] ON [DirServiceJobNomens].[DirServiceJobNomenID] = [DocSecondHandPurch1Tabs].[DirServiceJobNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocSecondHandPurch1Tabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirEmployees] ON [DocSecondHandPurch1Tabs].[DirEmployeeID] = [DirEmployees].[DirEmployeeID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceJobNomens] AS [DirServiceJobNomens] ON [DirServiceJobNomens].[DirServiceJobNomenID] = [DocSecondHandPurch1Tabs].[DirServiceJobNomenID] " +    //Товар
                "LEFT JOIN [DirServiceJobNomens] AS [dirServiceJobNomensGroup] ON [DirServiceJobNomens].[Sub] = [dirServiceJobNomensGroup].[DirServiceJobNomenID] " +            //Под-Группа
                "LEFT JOIN [DirServiceJobNomens] AS [dirServiceJobNomensSubGroup] ON [dirServiceJobNomensGroup].[Sub] = [dirServiceJobNomensSubGroup].[DirServiceJobNomenID] " + //Группа
                                                                                                                                                                                 //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "WHERE ([Docs].[DocID]=[DocSecondHandPurches].[DocID])and(Docs.DocID=@DocID) ";


            return SQL;
        }


        #endregion
    }
}