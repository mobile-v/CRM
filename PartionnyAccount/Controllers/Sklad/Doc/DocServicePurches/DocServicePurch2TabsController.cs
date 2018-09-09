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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocServicePurches
{
    public class DocServicePurch2TabsController : ApiController
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
        // GET: api/DocServicePurch2Tabs
        public async Task<IHttpActionResult> GetDocServicePurch2Tabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
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


                //Получаем Docs.DocIDBase
                int? _DocID = 0;
                var queryDocIDBase = await
                    (
                        from x in db.DocServicePurches
                        where x.DocServicePurchID == _params.DocServicePurchID
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
                        from x in db.DocServicePurch2Tabs

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        where x.DocServicePurchID == _params.DocServicePurchID

                        #region select

                        select new
                        {

                            IsZakaz = false,

                            DocServicePurch2TabID = x.DocServicePurch2TabID,
                            DocServicePurchID = x.DocServicePurchID,

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
                            RemontN = x.RemontN,
                        }

                        #endregion
                    );
                


                //Заказы !!!
                if (_DocID > 0)
                {
                    int? DocServicePurch2TabID = null, RemontN = null;
                    int DocServicePurchID = _params.DocServicePurchID;
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

                                DocServicePurch2TabID = DocServicePurch2TabID, //x.DocServicePurch2TabID,
                                DocServicePurchID = DocServicePurchID,

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
                                RemontN = RemontN, 
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
                    DocServicePurch2Tab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocServicePurch2Tabs/5
        [ResponseType(typeof(DocServicePurch2Tab))]
        public async Task<IHttpActionResult> GetDocServicePurch2Tab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocServicePurch2Tabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocServicePurch2Tab(int id, DocServicePurch2Tab docServicePurch2Tab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocServicePurch2Tabs
        [ResponseType(typeof(DocServicePurch2Tab))]
        public async Task<IHttpActionResult> PostDocServicePurch2Tab(DocServicePurch2Tab docServicePurch2Tab)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurch2Tabs"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocServicePurch2Tab[] DocServicePurch2TabCollection = null;
                if (!String.IsNullOrEmpty(docServicePurch2Tab.recordsDataX))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    DocServicePurch2TabCollection = serializer.Deserialize<Models.Sklad.Doc.DocServicePurch2Tab[]>("[" + docServicePurch2Tab.recordsDataX + "]");
                }

                if (DocServicePurch2TabCollection.Length > 0)
                {
                    docServicePurch2Tab = DocServicePurch2TabCollection[0];
                    docServicePurch2Tab.PriceVAT = docServicePurch2Tab.PriceCurrency;
                    //if (docServicePurch2Tab.DirServiceJobNomenID == 0) docServicePurch2Tab.DirServiceJobNomenID = null;
                    if (docServicePurch2Tab.DocServicePurch2TabID == null || docServicePurch2Tab.DocServicePurch2TabID < 1)
                    {
                        docServicePurch2Tab.DocServicePurch2TabID = null;
                        docServicePurch2Tab.DirEmployeeID = field.DirEmployeeID;
                        docServicePurch2Tab.DirCurrencyID = sysSetting.DirCurrencyID;
                        docServicePurch2Tab.DirCurrencyRate = 1;
                        docServicePurch2Tab.DirCurrencyMultiplicity = 1;
                    }
                }

                //Проверка точки === === ===
                Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch2Tab.DocServicePurchID);
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

                        docServicePurch2Tab = await Task.Run(() => mPutPostDocServicePurch2Tab(db, docServicePurch, docServicePurch2Tab, field));

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
                    DocServicePurch2TabID = docServicePurch2Tab.DocServicePurch2TabID,
                    DirEmployeeID = docServicePurch2Tab.DirEmployeeID,
                    DirCurrencyID = docServicePurch2Tab.DirCurrencyID,
                    DirCurrencyRate = docServicePurch2Tab.DirCurrencyRate,
                    DirCurrencyMultiplicity = docServicePurch2Tab.DirCurrencyMultiplicity
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocServicePurch2Tabs/5
        [ResponseType(typeof(DocServicePurch2Tab))]
        public async Task<IHttpActionResult> DeleteDocServicePurch2Tab(int id)
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
            bool bRight = await Task.Run(() => accessRight.AccessCheck(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceWorkshopsTab2ReturnCheck"));
            if (!bRight) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DocServicePurch2Tab docServicePurch2Tab = await db.DocServicePurch2Tabs.FindAsync(id);
                if (docServicePurch2Tab == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                //Проверка точки === === ===
                Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch2Tab.DocServicePurchID);
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
                        logService.DocServicePurchID = docServicePurch2Tab.DocServicePurchID;
                        logService.DirServiceLogTypeID = 6;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirServiceStatusID = null;
                        logService.Msg = "Удаление записи " + docServicePurch2Tab.DirNomenName + " на сумму " + docServicePurch2Tab.PriceCurrency;

                        await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

                        #endregion

                        #region Save

                        db.DocServicePurch2Tabs.Remove(docServicePurch2Tab);
                        await db.SaveChangesAsync();

                        #endregion


                        #region Партии

                        //Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch2Tab.DocServicePurchID);

                        //Models.Sklad.Rem.RemPartyMinus remPartyMinus = db.RemPartyMinuses.Where(x => x.DocID == docServicePurch.DocID && x.FieldID == id).ToList().First();
                        var query2 = await db.RemPartyMinuses.Where(x => x.DocID == docServicePurch.DocID && x.FieldID == id).ToListAsync();
                        if (query2.Count() > 0)
                        {
                            Models.Sklad.Rem.RemPartyMinus remPartyMinus = query2[0];
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
                    ID = docServicePurch2Tab.DocServicePurch2TabID,
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

        private bool DocServicePurch2TabExists(int id)
        {
            return db.DocServicePurch2Tabs.Count(e => e.DocServicePurch2TabID == id) > 0;
        }


        internal async Task<DocServicePurch2Tab> mPutPostDocServicePurch2Tab(
            DbConnectionSklad db,
            Models.Sklad.Doc.DocServicePurch docServicePurch,
            DocServicePurch2Tab docServicePurch2Tab,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region Лог

            //Пишем в Лог о смене статуса и мастера, если такое было
            logService.DocServicePurchID = docServicePurch2Tab.DocServicePurchID;
            logService.DirServiceLogTypeID = 6;
            logService.DirEmployeeID = field.DirEmployeeID;
            logService.DirServiceStatusID = null;
            if (docServicePurch2Tab.DocServicePurch2TabID == null) logService.Msg = "Создание записи " + docServicePurch2Tab.DirNomenName + " на сумму " + docServicePurch2Tab.PriceCurrency;
            else logService.Msg = "Изменение записи " + docServicePurch2Tab.DirNomenName + " на сумму " + docServicePurch2Tab.PriceCurrency;

            await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

            #endregion


            #region Save

            docServicePurch2Tab.TabDate = DateTime.Now;

            if (docServicePurch2Tab.DocServicePurch2TabID > 0)
            {
                db.Entry(docServicePurch2Tab).State = EntityState.Modified;
            }
            else
            {
                db.Entry(docServicePurch2Tab).State = EntityState.Added;
            }
            await Task.Run(() => db.SaveChangesAsync());

            #endregion

            #region Партии *** *** *** *** *** *** *** ***


            Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();

            //Находим "DocServicePurch" по "docServicePurch2Tab.DocServicePurchID"
            //Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch2Tab.DocServicePurchID);
            Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docServicePurch.DocID);


            #region Ищим в Возврате покупателя, если нет, то удаляем в RemPartyMinuses *** *** *** *** ***

            //Ищим в Возврате покупателя
            var queryRemPartyMinuses = await
                (
                    from x in db.RemPartyMinuses
                    where x.DocID == docServicePurch.DocID
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

            //await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(docServicePurch.DocID)));  //remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

            #endregion


            #region Проверки и Списание с партий (RemPartyMinuses)


            #region Проверка

            //Переменные
            int iRemPartyID = docServicePurch2Tab.RemPartyID;
            double dQuantity = 1; // docServicePurch2Tab.Quantity;
                                  //Находим партию
            Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(iRemPartyID);
            db.Entry(remParty).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

            #region 1. Есть ли остаток в партии с которой списываем!
            if (remParty.Remnant < dQuantity)
            {
                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg104 +

                    "<tr>" +
                    "<td>" + docServicePurch2Tab.RemPartyID + "</td>" +                                //партия
                    "<td>" + docServicePurch2Tab.DirNomenID + "</td>" +                                //Код товара
                    "<td>1</td>" + //"<td>" + docServicePurch2Tab.Quantity + "</td>"                                 //списуемое к-во
                    "<td>" + remParty.Remnant + "</td>" +                                                  //остаток партии
                    "<td>" + (1 - remParty.Remnant).ToString() + "</td>" +  //"<td>" + (docServicePurch2Tab.Quantity - remParty.Remnant).ToString() + "</td>" //недостающее к-во
                    "</tr>" +
                    "</table>" +

                    Classes.Language.Sklad.Language.msg104_1
                );
            }
            #endregion

            #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
            if (remParty.DirWarehouseID != docServicePurch.DirWarehouseID)
            {
                //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docServicePurch.dirWarehouse.DirWarehouseName"
                Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docServicePurch.DirWarehouseID);

                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg105 +

                    "<tr>" +
                    "<td>" + docServicePurch2Tab.RemPartyID + "</td>" +           //партия
                    "<td>" + docServicePurch2Tab.DirNomenID + "</td>" +           //Код товара
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
                //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docServicePurch.dirWarehouse.DirWarehouseName"
                Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(doc.DirContractorIDOrg);

                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg106 +

                    "<tr>" +
                    "<td>" + docServicePurch2Tab.RemPartyID + "</td>" +           //партия
                    "<td>" + docServicePurch2Tab.DirNomenID + "</td>" +           //Код товара
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
            remPartyMinus.RemPartyID = docServicePurch2Tab.RemPartyID;
            remPartyMinus.DirNomenID = Convert.ToInt32(docServicePurch2Tab.DirNomenID);
            remPartyMinus.Quantity = 1; // docServicePurch2Tab.Quantity;
            remPartyMinus.DirCurrencyID = docServicePurch2Tab.DirCurrencyID;
            remPartyMinus.DirCurrencyMultiplicity = docServicePurch2Tab.DirCurrencyMultiplicity;
            remPartyMinus.DirCurrencyRate = docServicePurch2Tab.DirCurrencyRate;
            remPartyMinus.DirVatValue = docServicePurch.DirVatValue;
            remPartyMinus.DirWarehouseID = docServicePurch.DirWarehouseID;
            remPartyMinus.DirContractorIDOrg = doc.DirContractorIDOrg;
            remPartyMinus.DirContractorID = doc.DirContractorIDOrg;
            remPartyMinus.DocID = Convert.ToInt32(docServicePurch.DocID);
            remPartyMinus.PriceCurrency = docServicePurch2Tab.PriceCurrency;
            remPartyMinus.PriceVAT = docServicePurch2Tab.PriceVAT;
            remPartyMinus.FieldID = Convert.ToInt32(docServicePurch2Tab.DocServicePurch2TabID);
            remPartyMinus.Reserve = false; //docServicePurch.Reserve;

            remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
            remPartyMinus.DocDate = doc.DocDate;

            db.Entry(remPartyMinus).State = EntityState.Added;
            await db.SaveChangesAsync();

            #endregion


            #endregion


            #endregion


            return docServicePurch2Tab;

        }

        #endregion



        #region SQL

        internal string GenerateSQLSelectCollection2(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";

            //Используем оператор "UNION ALL"

            SQL =

                //DocServicePurch2Tabs *** *** ***

                "SELECT " +
                "'Работа' AS TabNum, " +
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                //"[DirNomens].[DirNomenName] AS [DirNomenName], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocServicePurch2Tabs].[DirCurrencyRate] || ', ' || [DocServicePurch2Tabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocServicePurch2Tabs].[PriceVAT] AS [PriceVAT], " +
                "[DocServicePurch2Tabs].[PriceCurrency] AS [PriceCurrency], " +
                "[DocServicePurch2Tabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocServicePurch2Tabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
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



                "FROM [Docs], [DocServicePurches] " +
                "INNER JOIN [DocServicePurch2Tabs] AS [DocServicePurch2Tabs] ON [DocServicePurch2Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID]  and [DocServicePurch2Tabs].RemontN=(SELECT MAX(RemontN) FROM [DocServicePurch2Tabs] WHERE [DocServicePurch2Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID]) " + // and [DocServicePurch2Tabs].PayDate>=(SELECT MAX(PayDate) FROM [DocServicePurch2Tabs] WHERE [DocServicePurch2Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID])
                "INNER JOIN [DirCurrencies] ON [DocServicePurch2Tabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirEmployees] ON [DocServicePurch2Tabs].[DirEmployeeID] = [DirEmployees].[DirEmployeeID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocServicePurch2Tabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "WHERE " +
                "([Docs].[DocID]=[DocServicePurches].[DocID])and" +
                "(Docs.DocID=@DocID)";
                //"( [DocServicePurch2Tabs].PayDate=(SELECT MAX(PayDate) FROM [DocServicePurch2Tabs] WHERE [DocServicePurch2Tabs].[DocServicePurchID] = [DocServicePurches].[DocServicePurchID]) )";


            return SQL;
        }

        #endregion

    }
}