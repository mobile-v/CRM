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
    public class DocSecondHandSalesController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();

        int ListObjectID = 66;

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            //public int DocID;
            public int? DocSecondHandSaleID;
            public DateTime? DateS, DatePo;
            public int DirWarehouseID;
        }
        // GET: api/DocSecondHandSales
        public async Task<IHttpActionResult> GetDocSecondHandSales(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetails"));
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
                _params.DocSecondHandSaleID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandSaleID", true) == 0).Value);
                //_params.DocDate = Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDate", true) == 0).Value);

                _params.DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DateS < Convert.ToDateTime("01.01.1800")) _params.DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else _params.DateS = _params.DateS.Value.AddDays(-1);

                _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                int? DocSecondHandSaleID = null, DocSecondHandSaleReturnID = null, Rem2PartyID = null, Rem2PartyMinusID = null, DirReturnTypeID = null, DirDescriptionID = null;
                string DirReturnTypeName = null, DirDescriptionName = null;
                double Quantity = 1;

                var query =
                    (

                        #region 1. Продажа

                        from x in db.DocSecondHandSales

                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where
                            x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo &&
                            x.DirWarehouseID == _params.DirWarehouseID

                        select new
                        {
                            //x.doc.DocDateCreate,

                            DocID = x.DocID,
                            DocSecondHandPurchID = x.DocSecondHandPurchID,
                            
                            KKMSCheckNumber = x.doc.KKMSCheckNumber,
                            KKMSIdCommand = x.doc.KKMSIdCommand,
                            DocDate = x.doc.DocDate,
                            Held = x.doc.Held,
                            Discount = x.doc.Discount,
                            
                            DocSecondHandSaleID = x.DocSecondHandSaleID,
                            DocSecondHandReturnID = x.DocSecondHandSaleID,

                            DirWarehouseID = x.DirWarehouseID,
                            ListObjectID = x.doc.listObject.ListObjectID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,

                            DirServiceNomenID = x.DirServiceNomenID,

                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName,
                            
                            PriceVAT = x.PriceVAT,
                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",
                            
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                                                        
                            //Приходная цена
                            PriceCurrencyPurch = x.docSecondHandPurch.PriceVAT + x.docSecondHandPurch.Sums, 

                            //Цена в т.в.
                            PriceCurrency = x.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = x.PriceCurrency - x.doc.Discount == null ? 0
                            : Math.Round(x.PriceCurrency - x.doc.Discount, sysSetting.FractionalPartInSum),


                            //Причина возврата
                            DirReturnTypeID = x.DirReturnTypeID,
                            DirReturnTypeName = "",

                            DirDescriptionID = x.DirDescriptionID,
                            DirDescriptionName = "",

                        }

                        #endregion

                    ).Union
                    (

                        #region 2. Возврат

                        from x in db.DocSecondHandReturns

                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where
                            x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo &&
                            x.DirWarehouseID == _params.DirWarehouseID

                        select new
                        {
                            //x.doc.DocDateCreate,

                            DocID = x.DocID,
                            DocSecondHandPurchID = x.DocSecondHandPurchID,
                            
                            KKMSCheckNumber = x.doc.KKMSCheckNumber,
                            KKMSIdCommand = x.doc.KKMSIdCommand,
                            DocDate = x.doc.DocDate,
                            Held = x.doc.Held,
                            Discount = x.doc.Discount,
                            
                            DocSecondHandSaleID = x.DocSecondHandSaleID,
                            DocSecondHandReturnID = x.DocSecondHandReturnID,

                            DirWarehouseID = x.DirWarehouseID,
                            ListObjectID = x.doc.listObject.ListObjectID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,

                            DirServiceNomenID = x.DirServiceNomenID,

                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName,
                            
                            PriceVAT = -x.PriceVAT,
                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",
                                                     
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                             
                            //Приходная цена
                            PriceCurrencyPurch = x.docSecondHandPurch.PriceVAT + x.docSecondHandPurch.Sums,

                            //Цена в т.в.
                            PriceCurrency = -x.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = x.PriceCurrency - x.doc.Discount == null ? 0
                            : -Math.Round(x.PriceCurrency - x.doc.Discount, sysSetting.FractionalPartInSum),

                            
                            //Причина возврата
                            DirReturnTypeID = x.DirReturnTypeID,
                            DirReturnTypeName = x.dirReturnType.DirReturnTypeName,

                            DirDescriptionID = x.DirDescriptionID,
                            DirDescriptionName = x.dirDescription.DirDescriptionName,
                            
                        }

                        #endregion

                    ).Union
                    (

                        #region 3. Поступление

                        from x in db.DocSecondHandPurches

                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where
                            x.doc.DocDateCreate >= _params.DateS && x.doc.DocDateCreate <= _params.DatePo &&
                            x.DirWarehouseIDPurches == _params.DirWarehouseID

                        #region select

                        select new
                        {
                            //x.doc.DocDateCreate,

                            DocID = x.DocID,
                            DocSecondHandPurchID = x.DocSecondHandPurchID,
                            
                            KKMSCheckNumber = x.doc.KKMSCheckNumber,
                            KKMSIdCommand = x.doc.KKMSIdCommand,

                            DocDate = x.doc.DocDate,
                            Held = x.doc.Held,
                            Discount = x.doc.Discount,
                            
                            DocSecondHandSaleID = DocSecondHandSaleID,
                            DocSecondHandReturnID = DocSecondHandSaleReturnID,

                            DirWarehouseID = x.DirWarehouseID,
                            ListObjectID = x.doc.listObject.ListObjectID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,

                            DirServiceNomenID = x.DirServiceNomenID,

                            //DirServiceNomenName = x.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName,
                            
                            PriceVAT = -x.PriceVAT,
                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",
                            
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            
                            //Rem2Party
                            //Barcode = 0,
                            //SerialNumber = 0,

                            //Приходная цена
                            PriceCurrencyPurch = -x.PriceVAT + x.Sums, //x.remParty.PriceCurrency,

                            //Цена в т.в.
                            PriceCurrency = -x.PriceVAT,
                            //Себестоимость
                            SUMSalePriceVATCurrency = -x.PriceVAT,

                            
                            //Причина возврата
                            DirReturnTypeID = DirReturnTypeID,
                            DirReturnTypeName = DirReturnTypeName,

                            DirDescriptionID = DirDescriptionID,
                            DirDescriptionName = DirDescriptionName,
                            
                        }

                        #endregion

                        #endregion

                    );


                #endregion


                #region Сортировка

                query = query.OrderByDescending(x => x.DocDate);

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSecondHandSale = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandSales/5
        [ResponseType(typeof(DocSecondHandSale))]
        public async Task<IHttpActionResult> GetDocSecondHandSale(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandSales/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandSale(int id, DocSecondHandSale docSecondHandSale, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        //После печати чека, если забыли напечатать, сохранить ID-шнини чека
        //id == DocID
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PuttDocSecondHandSale(int id, int ServiceTypeRepair, int iTrash, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetails"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                var paramList = request.GetQueryNameValuePairs();
                string KKMSCheckNumber = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "KKMSCheckNumber", true) == 0).Value;
                string KKMSIdCommand = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "KKMSIdCommand", true) == 0).Value;

                #endregion

                #region Проверки

                //...

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Получаем Чек по "DocID" (id)
                        Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(id);
                        //Models.Sklad.Doc.DocSecondHandSale docSecondHandSale = (Models.Sklad.Doc.DocSecondHandSale)db.DocSecondHandSales.Where(x => x.DocID == id);

                        //Сохраняем данные в Doc
                        doc.KKMSCheckNumber = Convert.ToInt32(KKMSCheckNumber);
                        doc.KKMSIdCommand = KKMSIdCommand;
                        db.Entry(doc).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        //Сохраняем данные в DocCashOfficeSums или DocBankSums
                        if (doc.DirPaymentTypeID == 1)
                        {
                            //Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = (Models.Sklad.Doc.DocCashOfficeSum)db.DocCashOfficeSums.Where(x => x.DocID == id);
                            var query = await db.DocCashOfficeSums.Where(x => x.DocID == id).ToListAsync();
                            if (query.Count() > 0)
                            {
                                int DocCashOfficeSumID = Convert.ToInt32(query[0].DocCashOfficeSumID);
                                Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = await db.DocCashOfficeSums.FindAsync(DocCashOfficeSumID);
                                if (docCashOfficeSum != null)
                                {
                                    docCashOfficeSum.KKMSCheckNumber = Convert.ToInt32(KKMSCheckNumber);
                                    docCashOfficeSum.KKMSIdCommand = KKMSIdCommand;
                                    db.Entry(docCashOfficeSum).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        else if (doc.DirPaymentTypeID == 2)
                        {
                            var query = await db.DocBankSums.Where(x => x.DocID == id).ToListAsync();
                            if (query.Count() > 0)
                            {
                                int DocBankSumID = Convert.ToInt32(query[0].DocBankSumID);
                                Models.Sklad.Doc.DocBankSum docBankSum = await db.DocBankSums.FindAsync(DocBankSumID);
                                if (docBankSum != null)
                                {
                                    docBankSum.KKMSCheckNumber = Convert.ToInt32(KKMSCheckNumber);
                                    docBankSum.KKMSIdCommand = KKMSIdCommand;
                                    db.Entry(docBankSum).State = EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }

                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *
                /*
                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = docSecondHandSale.DocSecondHandSaleID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }
                */
                #endregion


                dynamic collectionWrapper = new
                {
                    Status = true
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }


        // POST: api/DocSecondHandSales
        [ResponseType(typeof(DocSecondHandSale))]
        public async Task<IHttpActionResult> PostDocSecondHandSale(DocSecondHandSale docSecondHandSale, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetails"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Скидка: разрешена или нет
                if (docSecondHandSale.Discount > 0)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocDescription"));
                    if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_5));
                }

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
                if (UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
                UO_Action = UO_Action.ToLower();

                //Получаем колекцию "Спецификации"
                /*Models.Sklad.Doc.DocSecondHandSaleTab[] docSecondHandSaleTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandSale.recordsDocSecondHandSaleTab) && UO_Action != "held_cancel")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandSaleTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandSaleTab[]>(docSecondHandSale.recordsDocSecondHandSaleTab);
                }*/

                #endregion

                #region Проверки

                docSecondHandSale.ServiceTypeRepair = sysSetting.ServiceTypeRepair;

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docSecondHandSale.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandSale.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandSales
                        where x.DocSecondHandSaleID == docSecondHandSale.DocSecondHandSaleID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandSale.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */


                //Проверка "Скидки"
                //1. Получаем сотурдника с правами



                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandSale.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandSale = await Task.Run(() => mPutPostDocSecondHandSale(db, dbRead, UO_Action, docSecondHandSale, EntityState.Added, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandSale.DocSecondHandSaleID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandSale.DocID,
                    DocSecondHandSaleID = docSecondHandSale.DocSecondHandSaleID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandSales/5
        [ResponseType(typeof(DocSecondHandSale))]
        public async Task<IHttpActionResult> DeleteDocSecondHandSale(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
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

        private bool DocSecondHandSaleExists(int id)
        {
            return db.DocSecondHandSales.Count(e => e.DocSecondHandSaleID == id) > 0;
        }


        //Алгоритм:
        //1. INSERT в "DocSecondHandSales"
        //2. UPDATE Status в "DocSecondHandPurches"

        internal async Task<DocSecondHandSale> mPutPostDocSecondHandSale(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandSale docSecondHandSale,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            //Models.Sklad.Doc.DocSecondHandSaleTab[] docSecondHandSaleTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            #region Проверка:

            //1. Статуса аппарата, только если:
            //DirSecondHandStatusID == 9 || DirSecondHandStatusID_789 == 7

            Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandSale.DocSecondHandPurchID);
            if (docSecondHandPurch.DirSecondHandStatusID != 9 || docSecondHandPurch.DirSecondHandStatusID_789 != 7)
            {
                throw new System.InvalidOperationException("Аппарат не готов к продаже или уже продан!");
            }

            //2. Комиссионная торговля: аппараты купленные на одной точке, а продаваеммые другой точке.
            //Запретить для таких аппаратов скидку
            if (!sysSetting.DocSecondHandSalesDiscount && (docSecondHandPurch.DirWarehouseID != docSecondHandPurch.DirWarehouseIDPurches && docSecondHandSale.Discount > 0))
            {
                throw new System.InvalidOperationException("Скидка для комиссионного аппарата (купленного на другой точке) запрещёна!");
            }

            #endregion

            #region Меняем статус аппарата

            docSecondHandPurch.DirSecondHandStatusID = 10;

            db.Entry(docSecondHandPurch).State = EntityState.Modified;
            await db.SaveChangesAsync();

            #endregion


            #region 1. Doc

            //Модель
            Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
            //Присваиваем значения
            doc.ListObjectID = ListObjectID;
            doc.IsImport = false;
            doc.NumberInt = docSecondHandSale.NumberInt;
            doc.NumberReal = docSecondHandSale.DocSecondHandSaleID;
            doc.DirEmployeeID = field.DirEmployeeID;
            doc.DirPaymentTypeID = docSecondHandSale.DirPaymentTypeID;
            doc.Payment = docSecondHandSale.Payment;
            if (docSecondHandSale.DirContractorID != null) doc.DirContractorID = Convert.ToInt32(docSecondHandSale.DirContractorID); else doc.DirContractorID = docSecondHandSale.DirContractorIDOrg;
            doc.DirContractorIDOrg = docSecondHandSale.DirContractorIDOrg;
            doc.Discount = docSecondHandSale.Discount;
            doc.DirVatValue = docSecondHandSale.DirVatValue;
            doc.Base = docSecondHandSale.Base;
            doc.Description = docSecondHandSale.Description;
            doc.DocDate = DateTime.Now; //docSecondHandSale.DocDate;
                                        //doc.DocDisc = docSecondHandSale.DocDisc;
            if (UO_Action == "held") doc.Held = true;
            else doc.Held = false;
            doc.DocID = docSecondHandSale.DocID;
            doc.DocIDBase = docSecondHandSale.DocIDBase;
            doc.KKMSCheckNumber = docSecondHandSale.KKMSCheckNumber;
            doc.KKMSIdCommand = docSecondHandSale.KKMSIdCommand;
            doc.KKMSEMail = docSecondHandSale.KKMSEMail;
            doc.KKMSPhone = docSecondHandSale.KKMSPhone;

            //Класс
            Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
            //doc = await docs.Save();
            await Task.Run(() => docs.Save());

            //Нужно вернуть "docSecondHandSale" со всем полями!
            docSecondHandSale.DocID = doc.DocID;

            #endregion

            #region 2. DocSecondHandSale

            docSecondHandSale.DocID = doc.DocID;

            db.Entry(docSecondHandSale).State = entityState;
            await db.SaveChangesAsync();

            #region 2.1. UpdateNumberInt, если INSERT

            if (entityState == EntityState.Added && (docSecondHandSale.doc.NumberInt == null || docSecondHandSale.doc.NumberInt.Length == 0))
            {
                doc.NumberInt = docSecondHandSale.DocSecondHandSaleID.ToString();
                doc.NumberReal = docSecondHandSale.DocSecondHandSaleID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }
            else if (entityState == EntityState.Added)
            {
                doc.NumberReal = docSecondHandSale.DocSecondHandSaleID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }

            #endregion


            #endregion


            #region Касса или Банк


            #region 1. Получаем валюту из склада

            int DirCurrencyID = 0, DirCurrencyMultiplicity = 0; //, DirCashOfficeID = 0, DirBankID = 0;;
            double DirCurrencyRate = 0;

            var query = await Task.Run(() =>
                (
                    from x in db.DirWarehouses
                    where x.DirWarehouseID == docSecondHandSale.DirWarehouseID
                    select new
                    {
                            //DirCashOfficeID= x.dirCashOffice.DirCashOfficeID,
                            DirCurrencyID_Bank = x.dirBank.DirCurrencyID,
                        DirCurrencyRate_Bank = x.dirBank.dirCurrency.DirCurrencyRate,
                        DirCurrencyMultiplicity_Bank = x.dirBank.dirCurrency.DirCurrencyMultiplicity,

                            //DirBankID = x.dirBank.DirBankID,
                            DirCurrencyID_Cash = x.dirCashOffice.DirCurrencyID,
                        DirCurrencyRate_Cash = x.dirCashOffice.dirCurrency.DirCurrencyRate,
                        DirCurrencyMultiplicity_Cash = x.dirCashOffice.dirCurrency.DirCurrencyMultiplicity,
                    }
                ).ToListAsync());

            if (query.Count() > 0)
            {
                if (doc.DirPaymentTypeID == 1)
                {
                    //DirCashOfficeID = Convert.ToInt32(query[0].DirCashOfficeID);
                    DirCurrencyID = query[0].DirCurrencyID_Cash;
                    DirCurrencyRate = query[0].DirCurrencyRate_Cash;
                    DirCurrencyMultiplicity = query[0].DirCurrencyMultiplicity_Cash;
                }
                else if (doc.DirPaymentTypeID == 2)
                {
                    //DirBankID = Convert.ToInt32(query[0].DirBankID);
                    DirCurrencyID = query[0].DirCurrencyID_Bank;
                    DirCurrencyRate = query[0].DirCurrencyRate_Bank;
                    DirCurrencyMultiplicity = query[0].DirCurrencyMultiplicity_Bank;
                }
                else
                {
                    throw new System.InvalidOperationException("Не выбран метод оплаты: Касса или Банк!");
                }
            }

            #endregion


            #region 2. Заполняем Модель

            Models.Sklad.Pay.Pay pay = new Models.Sklad.Pay.Pay();
            //pay.DirCashOfficeID = Convert.ToInt32(DirCashOfficeID);
            //pay.DirBankID = Convert.ToInt32(DirBankID);
            //Валюта
            pay.DirCurrencyID = DirCurrencyID;
            pay.DirCurrencyRate = DirCurrencyRate;
            pay.DirCurrencyMultiplicity = DirCurrencyMultiplicity;

            pay.DirEmployeeID = field.DirEmployeeID;
            pay.DirPaymentTypeID = doc.DirPaymentTypeID;
            //pay.DirXName = ""; //no
            //pay.DirXSumTypeID = 0; //no
            pay.DocCashBankID = null;
            pay.DocID = doc.DocID;
            pay.DocXID = docSecondHandSale.DocSecondHandSaleID;
            pay.DocXSumDate = doc.DocDate;
            pay.DocXSumSum = docSecondHandSale.PriceCurrency - doc.Discount; // - получили при сохранении Спецификации (выше) docSecondHandSale.PriceVAT

            //DocSecondHandPurchID - найти!!!
            pay.Base = "Продажа документа №" + docSecondHandPurch.DocSecondHandPurchID; //pay.Base = "Оплата за коды товаров: " + NomenName; // - получили при сохранении Спецификации (выше)

            //pay.Description = "";
            pay.KKMSCheckNumber = docSecondHandSale.KKMSCheckNumber;
            pay.KKMSIdCommand = docSecondHandSale.KKMSIdCommand;
            pay.KKMSEMail = docSecondHandSale.KKMSEMail;
            pay.KKMSPhone = docSecondHandSale.KKMSPhone;

            pay.Discount = doc.Discount;

            #endregion


            #region 3. Сохраняем

            PartionnyAccount.Controllers.Sklad.Pay.PayController payController = new Pay.PayController();
            doc = await Task.Run(() => payController.mPutPostPay(db, pay, EntityState.Modified, field)); //sysSetting

            #endregion


            #endregion


            #region 4. Log

            logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
            logService.DirSecondHandLogTypeID = 13;
            logService.DirEmployeeID = field.DirEmployeeID;
            logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
            logService.DirWarehouseIDFrom = docSecondHandPurch.DirWarehouseID;
            logService.DirWarehouseIDTo = docSecondHandPurch.DirWarehouseID;
            //logService.Msg = "Аппарат принят на точку №" + docSecondHandPurch.DirWarehouseID;

            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

            #endregion


            return docSecondHandSale;
        }

        #endregion


        #region SQL

        /// <summary>
        /// </summary>
        /// <param name="bTresh">Не работает без этого параметра. Идёт конфликт с методами UPDATE</param>
        /// <returns></returns>
        public string GenerateSQLSelect(bool bTresh)
        {
            string SQL =
                "SELECT " +
                "[DocSecondHandSales].[DocSecondHandSaleID] AS [DocSecondHandRetailID], " +   
                "[DocSecondHandSales].[DocSecondHandSaleID] AS [DocSecondHandSaleID], " +
                "[Docs].[DocID] AS [DocID], " +
                "[Docs].[DocIDBase] AS [DocIDBase], " +
                "[Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], " +
                "[Docs].[Base] AS [Base], " +
                "[Docs].[Held] AS [Held], " +
                "[Docs].[Discount] AS [Discount], " +
                "[Docs].[Del] AS [Del], " +
                "[Docs].[Description] AS [Description], " +
                "[Docs].[IsImport] AS [IsImport], " +
                "[Docs].[DirVatValue] AS [DirVatValue], " +
                "[Docs].[DirPaymentTypeID] AS [DirPaymentTypeID], " +
                "[DirEmployees].[DirEmployeeName] AS [DirEmployeeName], " +
                "[DirPaymentTypes].[DirPaymentTypeName] AS [DirPaymentTypeName], " +


                //DocSecondHandSales
                "[DocSecondHandSales].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandSales].[PriceCurrency] AS [PriceCurrency], " +
                "[DocSecondHandSales].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandSales].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DocSecondHandSales].[ServiceTypeRepair] AS [ServiceTypeRepair], " +


                //DocSecondHandPurches
                "[DocSecondHandPurches].[ComponentPasTextNo] AS [ComponentPasTextNo], [DocSecondHandPurches].[ComponentPasText] AS [ComponentPasText], [DocSecondHandPurches].[ComponentOtherText] AS [ComponentOtherText], [DocSecondHandPurches].[ProblemClientWords] AS [ProblemClientWords], [DocSecondHandPurches].[Note] AS [Note], [DocSecondHandPurches].[DirServiceContractorName] AS [DirContractorName], [DocSecondHandPurches].[DirServiceContractorAddress] AS [DirContractorAddress], [DocSecondHandPurches].[DirServiceContractorPhone] AS [DirContractorPhone], [DocSecondHandPurches].[DirServiceContractorEmail] AS [DirContractorEmail], [DocSecondHandPurches].[DirServiceContractorRegular] AS [DirServiceContractorRegular], [DocSecondHandPurches].[PassportSeries] AS [PassportSeries], [DocSecondHandPurches].[PassportNumber] AS [PassportNumber], [DocSecondHandPurches].[PriceVAT] AS [PriceVATEstimated], [DocSecondHandPurches].[PriceVAT] AS [SumTotal_InWords], [DocSecondHandPurches].[DateDone] AS [DateDone], " +
                "[DocSecondHandPurches].[SerialNumber] AS [SerialNumber], " +

                "[DirSecondHandStatuses].[DirSecondHandStatusName] AS [DirSecondHandStatusName], " +



                //Многие поля есть в БД, но нет в проекте.

                //Контрагент
                "[DirContractors].[DirContractorID] AS [DirContractorID], " +
                "[DirContractors].[DirContractorName] AS [DirContractorName], " +
                "[DirContractors].[DirContractorEmail] AS [DirContractorEmail], " +
                "[DirContractors].[DirContractorWWW] AS [DirContractorWWW], " +
                "[DirContractors].[DirContractorAddress] AS [DirContractorAddress], " +
                "[DirContractors].[DirContractorLegalCertificateDate] AS [DirContractorLegalCertificateDate], " +
                "[DirContractors].[DirContractorLegalTIN] AS [DirContractorLegalTIN], " +
                "[DirContractors].[DirContractorLegalCAT] AS [DirContractorLegalCAT], " +
                "[DirContractors].[DirContractorLegalCertificateNumber] AS [DirContractorLegalCertificateNumber], " +
                "[DirContractors].[DirContractorLegalBIN] AS [DirContractorLegalBIN], " +
                "[DirContractors].[DirContractorLegalOGRNIP] AS [DirContractorLegalOGRNIP], " +
                "[DirContractors].[DirContractorLegalRNNBO] AS [DirContractorLegalRNNBO], " +
                "[DirContractors].[DirContractorDesc] AS [DirContractorDesc], " +
                "[DirContractors].[DirContractorLegalPasIssued] AS [DirContractorLegalPasIssued], " +
                "[DirContractors].[DirContractorLegalPasDate] AS [DirContractorLegalPasDate], " +
                "[DirContractors].[DirContractorLegalPasCode] AS [DirContractorLegalPasCode], " +
                "[DirContractors].[DirContractorLegalPasNumber] AS [DirContractorLegalPasNumber], " +
                "[DirContractors].[DirContractorLegalPasSeries] AS [DirContractorLegalPasSeries], " +
                "[DirContractors].[DirContractorDiscount] AS [DirContractorDiscount], " +
                "[DirContractors].[DirContractorPhone] AS [DirContractorPhone], " +
                "[DirContractors].[DirContractorFax] AS [DirContractorFax], " +
                "[DirContractors].[DirContractorLegalAddress] AS [DirContractorLegalAddress], " +
                "[DirContractors].[DirContractorLegalName] AS [DirContractorLegalName], " +
                "[DirContractors].[DirBankAccountName] AS [DirBankAccountName], " +

                "[DirBanks].[DirBankName] AS [DirBankName], " +
                "[DirBanks].[DirBankMFO] AS [DirBankMFO], " +

                //Организация
                //"[DirContractorOrg].[DirContractorID] AS [DirContractorIDOrg], " +
                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +
                "[DirContractorOrg].[DirContractorEmail] AS [DirContractorEmailOrg], " +
                "[DirContractorOrg].[DirContractorWWW] AS [DirContractorWWWOrg], " +
                "[DirContractorOrg].[DirContractorAddress] AS [DirContractorAddressOrg], " +
                "[DirContractorOrg].[DirContractorLegalCertificateDate] AS [DirContractorLegalCertificateDateOrg], " +
                "[DirContractorOrg].[DirContractorLegalTIN] AS [DirContractorLegalTINOrg], " +
                "[DirContractorOrg].[DirContractorLegalCAT] AS [DirContractorLegalCATOrg], " +
                "[DirContractorOrg].[DirContractorLegalCertificateNumber] AS [DirContractorLegalCertificateNumberOrg], " +
                "[DirContractorOrg].[DirContractorLegalBIN] AS [DirContractorLegalBINOrg], " +
                "[DirContractorOrg].[DirContractorLegalOGRNIP] AS [DirContractorLegalOGRNIPOrg], " +
                "[DirContractorOrg].[DirContractorLegalRNNBO] AS [DirContractorLegalRNNBOOrg], " +
                "[DirContractorOrg].[DirContractorDesc] AS [DirContractorDescOrg], " +
                "[DirContractorOrg].[DirContractorLegalPasIssued] AS [DirContractorLegalPasIssuedOrg], " +
                "[DirContractorOrg].[DirContractorLegalPasDate] AS [DirContractorLegalPasDateOrg], " +
                "[DirContractorOrg].[DirContractorLegalPasCode] AS [DirContractorLegalPasCodeOrg], " +
                "[DirContractorOrg].[DirContractorLegalPasNumber] AS [DirContractorLegalPasNumberOrg], " +
                "[DirContractorOrg].[DirContractorLegalPasSeries] AS [DirContractorLegalPasSeriesOrg], " +
                "[DirContractorOrg].[DirContractorDiscount] AS [DirContractorDiscountOrg], " +
                "[DirContractorOrg].[DirContractorPhone] AS [DirContractorPhoneOrg], " +
                "[DirContractorOrg].[DirContractorFax] AS [DirContractorFaxOrg], " +
                "[DirContractorOrg].[DirContractorLegalAddress] AS [DirContractorLegalAddressOrg], " +
                "[DirContractorOrg].[DirContractorLegalName] AS [DirContractorLegalNameOrg], " +
                "[DirContractorOrg].[DirBankAccountName] AS [DirBankAccountNameOrg], " +

                "[DirBanksOrg].[DirBankName] AS [DirBankNameOrg], " +
                "[DirBanksOrg].[DirBankMFO] AS [DirBankMFOOrg], " +


                "[DirWarehouses].[DirWarehouseID] AS [DirWarehouseID], " +
                "[DirWarehouses].[DirWarehouseName] AS [DirWarehouseName], " +
                "[DirWarehouses].[DirWarehouseAddress] AS [DirWarehouseAddress], " +
                "[DirWarehouses].[DirWarehouseDesc] AS [DirWarehouseDesc], " + //есть в БД, но нет в проекте

                "[Docs].[NumberInt] AS [NumberInt], " +
                //"[DocSecondHandSales].[Reserve] AS [Reserve], " +



                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "CASE " +
                "WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN [dirServiceNomensSubGroup].[DirServiceNomenName] " +

                "WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END ELSE " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN '' ELSE [DirServiceNomens].[DirServiceNomenName] END END AS [DirServiceNomenName] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                "FROM [DocSecondHandSales] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandSales].[DocID] " +
                "INNER JOIN [DirEmployees] ON [Docs].[DirEmployeeID] = [DirEmployees].[DirEmployeeID] " +
                "INNER JOIN [DocSecondHandPurches] ON [DocSecondHandPurches].[DocSecondHandPurchID] = [DocSecondHandSales].[DocSecondHandPurchID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocSecondHandSales].[DirWarehouseID] " +
                //Банк для Контрагента
                "LEFT JOIN [DirBanks] AS [DirBanks] ON [DirBanks].[DirBankID] = [DirContractors].[DirBankID] " +
                //Банк для Организации
                "LEFT JOIN [DirBanks] AS [DirBanksOrg] ON [DirBanks].[DirBankID] = [DirContractorOrg].[DirBankID] " +
                //Статус
                "INNER JOIN [DirSecondHandStatuses] ON [DirSecondHandStatuses].[DirSecondHandStatusID] = [DocSecondHandPurches].[DirSecondHandStatusID] " + 


                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocSecondHandSales].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [dirServiceNomensSubGroup].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion

    }
}