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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandPurches
{
    public class DocSecondHandPurch2TabsController : ApiController
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
        // GET: api/DocSecondHandPurch2Tabs
        public async Task<IHttpActionResult> GetDocSecondHandPurch2Tabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurches"));
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


                //Получаем Docs.DocIDBase
                int? _DocID = 0;
                var queryDocIDBase = await
                    (
                        from x in db.DocSecondHandPurches
                        where x.DocSecondHandPurchID == _params.DocSecondHandPurchID
                        select x
                    ).ToListAsync();
                if (queryDocIDBase.Count() > 0)
                {
                    _DocID = queryDocIDBase[0].doc.DocID;
                }

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.DocSecondHandPurch2Tabs

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        where x.DocSecondHandPurchID == _params.DocSecondHandPurchID

                        #region select

                        select new
                        {

                            IsZakaz = false,

                            DocSecondHandPurch2TabID = x.DocSecondHandPurch2TabID,
                            DocSecondHandPurchID = x.DocSecondHandPurchID,

                            DirEmployeeID = x.DirEmployeeID,
                            DirEmployeeName = x.dirEmployee.DirEmployeeName,

                            DirNomenID = x.DirNomenID,

                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName
                            :
                            dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            RemPartyID = x.RemPartyID,
                            PriceVAT = x.PriceVAT,
                            PriceCurrency = x.PriceCurrency,

                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",

                            TabDate = x.TabDate,

                            PayDate = x.PayDate,
                        }

                        #endregion
                    );


                //Заказы !!!
                if (_DocID > 0)
                {
                    int? DocSecondHandPurch2TabID = null;
                    int DocSecondHandPurchID = _params.DocSecondHandPurchID;
                    DateTime? PayDate = null;

                    query = query.Concat
                        (
                            from x in db.DocOrderInts

                                //join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                                //from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                            where x.doc.DocIDBase == _DocID && x.DirOrderIntStatusID < 4

                            #region select

                            select new
                            {

                                IsZakaz = true,

                                DocSecondHandPurch2TabID = DocSecondHandPurch2TabID, //x.DocSecondHandPurch2TabID,
                                DocSecondHandPurchID = DocSecondHandPurchID,

                                DirEmployeeID = x.doc.DirEmployeeID,
                                DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,

                                DirNomenID = x.DocOrderIntID, //x.DirNomenID == null ? 0 : x.DirNomenID, //x.DirNomenCategoryID, //x.DirNomenID,  //===== !!! DirNomenCategoryID !!!

                                //DirNomenName = x.dirNomen.DirNomenName,
                                DirNomenName =
                                    x.dirNomen1.DirNomenName + " / " + x.dirNomen2.DirNomenName + " / " + x.dirNomenCategory.DirNomenCategoryName,

                                RemPartyID = 0, //x.RemPartyID,
                                PriceVAT = x.PriceVAT,
                                PriceCurrency = x.PriceCurrency,

                                DirCurrencyID = x.DirCurrencyID,
                                DirCurrencyRate = x.DirCurrencyRate,
                                DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",

                                TabDate = x.doc.DocDate, //x.TabDate,

                                PayDate = PayDate, //x.PayDate,
                            }

                            #endregion
                        );

                }

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSecondHandPurch2Tab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandPurch2Tabs/5
        [ResponseType(typeof(DocSecondHandPurch2Tab))]
        public async Task<IHttpActionResult> GetDocSecondHandPurch2Tab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandPurch2Tabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandPurch2Tab(int id, DocSecondHandPurch2Tab docSecondHandPurch2Tab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandPurch2Tabs
        [ResponseType(typeof(DocSecondHandPurch2Tab))]
        public async Task<IHttpActionResult> PostDocSecondHandPurch2Tab(DocSecondHandPurch2Tab docSecondHandPurch2Tab)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurch2Tabs"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocSecondHandPurch2Tab[] DocSecondHandPurch2TabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandPurch2Tab.recordsDataX))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    DocSecondHandPurch2TabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandPurch2Tab[]>("[" + docSecondHandPurch2Tab.recordsDataX + "]");
                }

                if (DocSecondHandPurch2TabCollection.Length > 0)
                {
                    docSecondHandPurch2Tab = DocSecondHandPurch2TabCollection[0];
                    docSecondHandPurch2Tab.PriceVAT = docSecondHandPurch2Tab.PriceCurrency;
                    //if (docSecondHandPurch2Tab.DirSecondHandJobNomenID == 0) docSecondHandPurch2Tab.DirSecondHandJobNomenID = null;
                    if (docSecondHandPurch2Tab.DocSecondHandPurch2TabID == null || docSecondHandPurch2Tab.DocSecondHandPurch2TabID < 1)
                    {
                        docSecondHandPurch2Tab.DocSecondHandPurch2TabID = null;
                        docSecondHandPurch2Tab.DirEmployeeID = field.DirEmployeeID;
                        docSecondHandPurch2Tab.DirCurrencyID = sysSetting.DirCurrencyID;
                        docSecondHandPurch2Tab.DirCurrencyRate = 1;
                        docSecondHandPurch2Tab.DirCurrencyMultiplicity = 1;
                    }
                }

                #endregion


                #region Сохранение


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {

                        docSecondHandPurch2Tab = await Task.Run(() => mPutPostDocSecondHandPurch2Tab(db, docSecondHandPurch2Tab, field));

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
                    DocSecondHandPurch2TabID = docSecondHandPurch2Tab.DocSecondHandPurch2TabID,
                    DirEmployeeID = docSecondHandPurch2Tab.DirEmployeeID,
                    DirCurrencyID = docSecondHandPurch2Tab.DirCurrencyID,
                    DirCurrencyRate = docSecondHandPurch2Tab.DirCurrencyRate,
                    DirCurrencyMultiplicity = docSecondHandPurch2Tab.DirCurrencyMultiplicity
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandPurch2Tabs/5
        [ResponseType(typeof(DocSecondHandPurch2Tab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandPurch2Tab(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurch2Tabs"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DocSecondHandPurch2Tab docSecondHandPurch2Tab = await db.DocSecondHandPurch2Tabs.FindAsync(id);
                if (docSecondHandPurch2Tab == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        #region Лог

                        //Пишем в Лог о смене статуса и мастера, если такое было
                        logSecondHand.DocSecondHandPurchID = docSecondHandPurch2Tab.DocSecondHandPurchID;
                        logSecondHand.DirSecondHandLogTypeID = 6;
                        logSecondHand.DirEmployeeID = field.DirEmployeeID;
                        logSecondHand.DirSecondHandStatusID = null;
                        logSecondHand.Msg = "Удаление записи " + docSecondHandPurch2Tab.DirNomenName + " на сумму " + docSecondHandPurch2Tab.PriceCurrency;

                        await logSecondHandsController.mPutPostLogSecondHands(db, logSecondHand, EntityState.Added);

                        #endregion

                        #region Save

                        db.DocSecondHandPurch2Tabs.Remove(docSecondHandPurch2Tab);
                        await db.SaveChangesAsync();

                        #endregion


                        #region Партии

                        Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandPurch2Tab.DocSecondHandPurchID);

                        //Models.Sklad.Rem.RemPartyMinus remPartyMinus = db.RemPartyMinuses.Where(x => x.DocID == docSecondHandPurch.DocID && x.FieldID == id).ToList().First();
                        var query = await db.RemPartyMinuses.Where(x => x.DocID == docSecondHandPurch.DocID && x.FieldID == id).ToListAsync();
                        if (query.Count() > 0)
                        {
                            Models.Sklad.Rem.RemPartyMinus remPartyMinus = query[0];
                            db.RemPartyMinuses.Remove(remPartyMinus);
                            await db.SaveChangesAsync();
                        }

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
                    ID = docSecondHandPurch2Tab.DocSecondHandPurch2TabID,
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

        private bool DocSecondHandPurch2TabExists(int id)
        {
            return db.DocSecondHandPurch2Tabs.Count(e => e.DocSecondHandPurch2TabID == id) > 0;
        }


        internal async Task<DocSecondHandPurch2Tab> mPutPostDocSecondHandPurch2Tab(
            DbConnectionSklad db,
            //Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch,
            DocSecondHandPurch2Tab docSecondHandPurch2Tab,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region Лог

            //Пишем в Лог о смене статуса и мастера, если такое было
            logSecondHand.DocSecondHandPurchID = docSecondHandPurch2Tab.DocSecondHandPurchID;
            logSecondHand.DirSecondHandLogTypeID = 6;
            logSecondHand.DirEmployeeID = field.DirEmployeeID;
            logSecondHand.DirSecondHandStatusID = null;
            if (docSecondHandPurch2Tab.DocSecondHandPurch2TabID == null) logSecondHand.Msg = "Создание записи " + docSecondHandPurch2Tab.DirNomenName + " на сумму " + docSecondHandPurch2Tab.PriceCurrency;
            else logSecondHand.Msg = "Изменение записи " + docSecondHandPurch2Tab.DirNomenName + " на сумму " + docSecondHandPurch2Tab.PriceCurrency;

            await logSecondHandsController.mPutPostLogSecondHands(db, logSecondHand, EntityState.Added);

            #endregion


            #region Save

            docSecondHandPurch2Tab.TabDate = DateTime.Now;

            if (docSecondHandPurch2Tab.DocSecondHandPurch2TabID > 0)
            {
                db.Entry(docSecondHandPurch2Tab).State = EntityState.Modified;
            }
            else
            {
                db.Entry(docSecondHandPurch2Tab).State = EntityState.Added;
            }
            await Task.Run(() => db.SaveChangesAsync());

            #endregion

            #region Партии *** *** *** *** *** *** *** ***


            Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();

            //Находим "DocSecondHandPurch" по "docSecondHandPurch2Tab.DocSecondHandPurchID"
            Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandPurch2Tab.DocSecondHandPurchID);
            Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandPurch.DocID);


            #region Ищим в Возврате покупателя, если нет, то удаляем в RemPartyMinuses *** *** *** *** ***

            //Ищим в Возврате покупателя
            var queryRemPartyMinuses = await
                (
                    from x in db.RemPartyMinuses
                    where x.DocID == docSecondHandPurch.DocID
                    select x
                ).ToListAsync();

            for (int i = 0; i < queryRemPartyMinuses.Count(); i++)
            {
                int iRemPartyMinusID = Convert.ToInt32(queryRemPartyMinuses[i].RemPartyMinusID);

                var queryDocReturnsCustomerTab = await
                    (
                        from x in db.DocReturnsCustomerTabs
                        where x.RemPartyMinusID == iRemPartyMinusID
                        select x
                    ).ToListAsync();

                if (queryDocReturnsCustomerTab.Count() > 0)
                {
                    throw new System.InvalidOperationException(
                        Classes.Language.Sklad.Language.msg117 +

                        "<tr>" +
                        "<td>" + queryDocReturnsCustomerTab[0].RemPartyMinusID + "</td>" +                           //партия списания
                        "<td>" + queryDocReturnsCustomerTab[0].DocReturnsCustomerID + "</td>" +                      //№ д-та
                        "<td>" + queryDocReturnsCustomerTab[0].DirNomenID + "</td>" +                                //Код товара
                        "<td>" + queryDocReturnsCustomerTab[0].Quantity + "</td>" +                                  //списуемое к-во
                        "</tr>" +
                        "</table>" +

                        Classes.Language.Sklad.Language.msg117_1
                        );
                }

                //1.1. Удаляем "RemPartyMinuses" - не удаляем!!!
                //Models.Sklad.Rem.RemPartyMinus _remPartyMinus = await db.RemPartyMinuses.FindAsync(iRemPartyMinusID);
                //db.RemPartyMinuses.Remove(_remPartyMinus);
                //await db.SaveChangesAsync();
            }

            #endregion

            #region Удаляем все записи из таблицы "RemPartyMinuses" - не удаляем!!!
            //Удаляем все записи из таблицы "RemPartyMinuses"
            //Что бы правильно Проверяло на Остаток.
            //А то, товар уже списан, а я проверяю на остаток!

            //await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(docSecondHandPurch.DocID)));  //remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

            #endregion


            #region Проверки и Списание с партий (RemPartyMinuses)


            #region Проверка

            //Переменные
            int iRemPartyID = docSecondHandPurch2Tab.RemPartyID;
            double dQuantity = 1; // docSecondHandPurch2Tab.Quantity;
                                  //Находим партию
            Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(iRemPartyID);
            db.Entry(remParty).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

            #region 1. Есть ли остаток в партии с которой списываем!
            if (remParty.Remnant < dQuantity)
            {
                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg104 +

                    "<tr>" +
                    "<td>" + docSecondHandPurch2Tab.RemPartyID + "</td>" +                                //партия
                    "<td>" + docSecondHandPurch2Tab.DirNomenID + "</td>" +                                //Код товара
                    "<td>1</td>" + //"<td>" + docSecondHandPurch2Tab.Quantity + "</td>"                                 //списуемое к-во
                    "<td>" + remParty.Remnant + "</td>" +                                                  //остаток партии
                    "<td>" + (1 - remParty.Remnant).ToString() + "</td>" +  //"<td>" + (docSecondHandPurch2Tab.Quantity - remParty.Remnant).ToString() + "</td>" //недостающее к-во
                    "</tr>" +
                    "</table>" +

                    Classes.Language.Sklad.Language.msg104_1
                );
            }
            #endregion

            #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
            if (remParty.DirWarehouseID != docSecondHandPurch.DirWarehouseID)
            {
                //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandPurch.dirWarehouse.DirWarehouseName"
                Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandPurch.DirWarehouseID);

                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg105 +

                    "<tr>" +
                    "<td>" + docSecondHandPurch2Tab.RemPartyID + "</td>" +           //партия
                    "<td>" + docSecondHandPurch2Tab.DirNomenID + "</td>" +           //Код товара
                    "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                    "<td>" + remParty.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                    "</tr>" +
                    "</table>" +

                    Classes.Language.Sklad.Language.msg105_1
                );
            }
            #endregion

            #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
            if (remParty.DirContractorIDOrg != doc.DirContractorIDOrg)
            {
                //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandPurch.dirWarehouse.DirWarehouseName"
                Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(doc.DirContractorIDOrg);

                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg106 +

                    "<tr>" +
                    "<td>" + docSecondHandPurch2Tab.RemPartyID + "</td>" +           //партия
                    "<td>" + docSecondHandPurch2Tab.DirNomenID + "</td>" +           //Код товара
                    "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                    "<td>" + remParty.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                    "</tr>" +
                    "</table>" +

                    Classes.Language.Sklad.Language.msg106_1
                );
            }
            #endregion

            #endregion


            #region Сохранение

            Models.Sklad.Rem.RemPartyMinus remPartyMinus = new Models.Sklad.Rem.RemPartyMinus();
            remPartyMinus.RemPartyMinusID = null;
            remPartyMinus.RemPartyID = docSecondHandPurch2Tab.RemPartyID;
            remPartyMinus.DirNomenID = Convert.ToInt32(docSecondHandPurch2Tab.DirNomenID);
            remPartyMinus.Quantity = 1; // docSecondHandPurch2Tab.Quantity;
            remPartyMinus.DirCurrencyID = docSecondHandPurch2Tab.DirCurrencyID;
            remPartyMinus.DirCurrencyMultiplicity = docSecondHandPurch2Tab.DirCurrencyMultiplicity;
            remPartyMinus.DirCurrencyRate = docSecondHandPurch2Tab.DirCurrencyRate;
            remPartyMinus.DirVatValue = docSecondHandPurch.DirVatValue;
            remPartyMinus.DirWarehouseID = docSecondHandPurch.DirWarehouseID;
            remPartyMinus.DirContractorIDOrg = doc.DirContractorIDOrg;
            remPartyMinus.DirContractorID = doc.DirContractorIDOrg;
            remPartyMinus.DocID = Convert.ToInt32(docSecondHandPurch.DocID);
            remPartyMinus.PriceCurrency = docSecondHandPurch2Tab.PriceCurrency;
            remPartyMinus.PriceVAT = docSecondHandPurch2Tab.PriceVAT;
            remPartyMinus.FieldID = Convert.ToInt32(docSecondHandPurch2Tab.DocSecondHandPurch2TabID);
            remPartyMinus.Reserve = false; //docSecondHandPurch.Reserve;

            remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
            remPartyMinus.DocDate = doc.DocDate;

            db.Entry(remPartyMinus).State = EntityState.Added;
            await db.SaveChangesAsync();

            #endregion


            #endregion


            #endregion


            return docSecondHandPurch2Tab;

        }

        #endregion



        #region SQL

        internal string GenerateSQLSelectCollection2(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";

            //Используем оператор "UNION ALL"

            SQL =

                //DocSecondHandPurch2Tabs *** *** ***

                "SELECT " +
                "'Работа' AS TabNum, " +
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                //"[DirNomens].[DirNomenName] AS [DirNomenName], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandPurch2Tabs].[DirCurrencyRate] || ', ' || [DocSecondHandPurch2Tabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocSecondHandPurch2Tabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandPurch2Tabs].[PriceCurrency] AS [PriceCurrency], " +
                "[DocSecondHandPurch2Tabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandPurch2Tabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirEmployees].[DirEmployeeName] AS [DirEmployeeName], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty, " +



                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "CASE " +
                "WHEN ([dirNomensGroup].[DirNomenName] IS NULL) THEN [dirNomensSubGroup].[DirNomenName] " +

                "WHEN ([DirNomens].[DirNomenName] IS NULL) THEN " +

                "CASE WHEN ([dirNomensSubGroup].[DirNomenName] IS NULL) THEN '' ELSE [dirNomensSubGroup].[DirNomenName] END || ' / ' || " +
                "CASE WHEN ([dirNomensGroup].[DirNomenName] IS NULL) THEN '' ELSE [dirNomensGroup].[DirNomenName] END ELSE " +

                "CASE WHEN ([dirNomensSubGroup].[DirNomenName] IS NULL) THEN '' ELSE [dirNomensSubGroup].[DirNomenName] END || ' / ' || " +
                "CASE WHEN ([dirNomensGroup].[DirNomenName] IS NULL) THEN '' ELSE [dirNomensGroup].[DirNomenName] END || ' / ' || " +
                "CASE WHEN ([DirNomens].[DirNomenName] IS NULL) THEN '' ELSE [DirNomens].[DirNomenName] END END AS [DirNomenName] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                "FROM [Docs], [DocSecondHandPurches] " +
                "INNER JOIN [DocSecondHandPurch2Tabs] AS [DocSecondHandPurch2Tabs] ON [DocSecondHandPurch2Tabs].[DocSecondHandPurchID] = [DocSecondHandPurches].[DocSecondHandPurchID] " +
                //"INNER JOIN [DirNomens] AS [DirNomens] ON [DocSecondHandPurch2Tabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocSecondHandPurch2Tabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirEmployees] ON [DocSecondHandPurch2Tabs].[DirEmployeeID] = [DirEmployees].[DirEmployeeID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocSecondHandPurch2Tabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "WHERE ([Docs].[DocID]=[DocSecondHandPurches].[DocID])and(Docs.DocID=@DocID) ";


            return SQL;
        }

        #endregion

    }
}