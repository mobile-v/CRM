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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocInventories
{
    public class DocInventoriesController : ApiController
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
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 39;

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;
            public int GroupID = 0;
            public string parSearch = "";
            public int FilterType;
            public int? DirWarehouseID;
            public DateTime? DateS;
            public DateTime? DatePo;
        }
        // GET: api/DocInventories
        public async Task<IHttpActionResult> GetDocInventories(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocInventories"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                //Получаем сотрудника: если к нему привязан Склад и/или Организация, то выбираем документы только по этим характеристикам
                Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(field.DirEmployeeID);

                #endregion


                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.limit = sysSetting.PageSizeJurn; //Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.GroupID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pGroupID", true) == 0).Value); //Кликнули по группе
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск
                _params.FilterType = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "FilterType", true) == 0).Value);
                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);

                _params.DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DateS < Convert.ToDateTime("01.01.1800")) _params.DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else _params.DateS = _params.DateS.Value.AddDays(-1);

                _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docInventories in db.DocInventories

                        join docInventoryTabs1 in db.DocInventoryTabs on docInventories.DocInventoryID equals docInventoryTabs1.DocInventoryID into docInventoryTabs2
                        from docInventoryTabs in docInventoryTabs2.DefaultIfEmpty()

                        where docInventories.doc.DocDate >= _params.DateS && docInventories.doc.DocDate <= _params.DatePo

                        group new { docInventoryTabs }
                        by new
                        {
                            DocID = docInventories.DocID,
                            DocDate = docInventories.doc.DocDate,
                            Base = docInventories.doc.Base,
                            Held = docInventories.doc.Held,
                            Discount = docInventories.doc.Discount,
                            Del = docInventories.doc.Del,
                            Description = docInventories.doc.Description,
                            IsImport = docInventories.doc.IsImport,
                            DirVatValue = docInventories.doc.DirVatValue,

                            DocInventoryID = docInventories.DocInventoryID,
                            //DirContractorName = docInventories.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docInventories.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docInventories.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docInventories.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docInventories.dirWarehouse.DirWarehouseName,

                            NumberInt = docInventories.doc.NumberInt,
                            DirEmployeeName = docInventories.doc.dirEmployee.DirEmployeeName,

                            DocDateHeld = docInventories.doc.DocDateHeld,
                            DocDatePayment = docInventories.doc.DocDatePayment,
                        }
                        into g

                        select new
                        {
                            DocID = g.Key.DocID,
                            DocDate = g.Key.DocDate,
                            Held = g.Key.Held,
                            Base = g.Key.Base,

                            Discount = g.Key.Discount,
                            Del = g.Key.Del,
                            Description = g.Key.Description,
                            IsImport = g.Key.IsImport,

                            DocInventoryID = g.Key.DocInventoryID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,
                            DirEmployeeName = g.Key.DirEmployeeName,

                            DocDateHeld = g.Key.DocDateHeld,
                            DocDatePayment = g.Key.DocDatePayment,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docInventoryTabs.Quantity_WriteOff * x.docInventoryTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docInventoryTabs.Quantity_WriteOff * x.docInventoryTabs.PriceCurrency), sysSetting.FractionalPartInSum)
                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region dirEmployee: dirEmployee.DirWarehouseID and/or dirEmployee.DirContractorIDOrg

                if (_params.DirWarehouseID != null && _params.DirWarehouseID > 0)
                {
                    query = query.Where(x => x.DirWarehouseID == _params.DirWarehouseID);
                }

                if (dirEmployee.DirContractorIDOrg != null && dirEmployee.DirContractorIDOrg > 0)
                {
                    query = query.Where(x => x.DirContractorIDOrg == dirEmployee.DirContractorIDOrg);
                }

                #endregion


                #region Не показывать удалённые

                if (!Convert.ToBoolean(sysSetting.DeletedRecordsShow))
                {
                    query = query.Where(x => x.Del == sysSetting.DeletedRecordsShow);
                }

                #endregion


                #region Фильтр

                if (_params.FilterType > 0)
                {
                    switch (_params.FilterType)
                    {
                        case 1: query = query.Where(x => x.Held == true); break;
                        case 2: query = query.Where(x => x.Held == false); break;
                        case 3: query = query.Where(x => x.IsImport == true); break;

                            //case 4: query = query.Where(x => x.HavePay == 0); break;
                            //case 5: query = query.Where(x => x.HavePay > 0); break;
                    }
                }

                #endregion


                #region Поиск

                if (!String.IsNullOrEmpty(_params.parSearch))
                {
                    //Проверяем число ли это
                    Int32 iNumber32;
                    bool bResult32 = Int32.TryParse(_params.parSearch, out iNumber32);
                    DateTime dDateTime;
                    bool bDateTime = DateTime.TryParse(_params.parSearch, out dDateTime);


                    //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                    if (bResult32)
                    {
                        query = query.Where(x => x.DocInventoryID == iNumber32);
                    }
                    //Если Дата
                    else if (bDateTime)
                    {
                        query = query.Where(x => x.DocDate == dDateTime);
                    }
                    //Иначе, только текстовые поля
                    else
                    {
                        query = query.Where(x => x.DirWarehouseName.Contains(_params.parSearch) || x.NumberInt.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                //query = query.OrderByDescending(x => x.DocDate).Skip(_params.Skip).Take(_params.limit);

                if (sysSetting.DocsSortField == 1)
                {
                    query = query.OrderByDescending(x => x.DocInventoryID);
                }
                else if (sysSetting.DocsSortField == 2)
                {
                    query = query.OrderByDescending(x => x.DocDate);
                }
                else if (sysSetting.DocsSortField == 3)
                {
                    query = query.OrderByDescending(x => x.DocDateHeld);
                }
                /*else if (sysSetting.DocsSortField == 4)
                {
                    query = query.OrderByDescending(x => x.DocDatePayment);
                }*/
                else
                {
                    query = query.OrderByDescending(x => x.DocInventoryID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocInventories.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocInventory = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocInventories/5
        [ResponseType(typeof(DocInventory))]
        public async Task<IHttpActionResult> GetDocInventory(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocInventories"));
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
                int DocID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocID", true) == 0).Value); //Кликнули по группе

                //Если не пришёл параметр "DocID", то получаем его из БД (что бы SQlServer не перебирал все оплаты)
                if (DocID == 0)
                {
                    var queryDocID = await Task.Run(() =>
                    (
                        from docInventories in db.DocInventories
                        where docInventories.DocInventoryID == id
                        select docInventories
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docInventories in db.DocInventories

                        join docInventoryTabs1 in db.DocInventoryTabs on docInventories.DocInventoryID equals docInventoryTabs1.DocInventoryID into docInventoryTabs2
                        from docInventoryTabs in docInventoryTabs2.DefaultIfEmpty()

                        #endregion

                        where docInventories.DocInventoryID == id

                        #region group

                        group new { docInventoryTabs }
                        by new
                        {
                            DocID = docInventories.DocID, //DocID = docInventories.doc.DocID,
                            DocIDBase = docInventories.doc.DocIDBase,
                            DocDate = docInventories.doc.DocDate,
                            Base = docInventories.doc.Base,
                            Held = docInventories.doc.Held,
                            //Discount = docInventories.doc.Discount,
                            Del = docInventories.doc.Del,
                            IsImport = docInventories.doc.IsImport,
                            Description = docInventories.doc.Description,
                            DirVatValue = docInventories.doc.DirVatValue,
                            //DirPaymentTypeID = docInventories.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docInventories.doc.dirPaymentType.DirPaymentTypeName,

                            DocInventoryID = docInventories.DocInventoryID,
                            //DirContractorID = docInventories.doc.DirContractorID,
                            //DirContractorName = docInventories.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docInventories.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docInventories.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docInventories.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docInventories.dirWarehouse.DirWarehouseName,
                            NumberInt = docInventories.doc.NumberInt,

                            //Оплата
                            Payment = docInventories.doc.Payment,

                            //Резерв
                            Reserve = docInventories.Reserve
                        }
                        into g

                        #endregion

                        #region select

                        select new
                        {
                            DocID = g.Key.DocID,
                            DocIDBase = g.Key.DocIDBase,
                            DocDate = g.Key.DocDate,
                            Base = g.Key.Base,
                            Held = g.Key.Held,
                            //Discount = g.Key.Discount,
                            Del = g.Key.Del,
                            Description = g.Key.Description,
                            IsImport = g.Key.IsImport,
                            DirVatValue = g.Key.DirVatValue,
                            //DirPaymentTypeID = g.Key.DirPaymentTypeID,
                            //DirPaymentTypeName = g.Key.DirPaymentTypeName,

                            DocInventoryID = g.Key.DocInventoryID,
                            //DirContractorID = g.Key.DirContractorID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,
                            //Резерв
                            Reserve = g.Key.Reserve,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => x.docInventoryTabs.Quantity_WriteOff * x.docInventoryTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docInventoryTabs.Quantity_WriteOff * x.docInventoryTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма разницы цен розницы
                            SumMinus_PriceRetailCurrency =
                            g.Sum(x => x.docInventoryTabs.Quantity_Purch * x.docInventoryTabs.PriceRetailCurrency) - g.Sum(x => x.docInventoryTabs.Quantity_WriteOff * x.docInventoryTabs.PriceRetailCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docInventoryTabs.Quantity_Purch * x.docInventoryTabs.PriceRetailCurrency) - g.Sum(x => x.docInventoryTabs.Quantity_WriteOff * x.docInventoryTabs.PriceRetailCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            /*
                            SumVATCurrency =
                            g.Sum(x => x.docInventoryTabs.Quantity_WriteOff * (x.docInventoryTabs.PriceCurrency - (x.docInventoryTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docInventoryTabs.Quantity_WriteOff * (x.docInventoryTabs.PriceCurrency - (x.docInventoryTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),
                            */

                            //Оплата
                            /*
                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => x.docInventoryTabs.Quantity_WriteOff * x.docInventoryTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docInventoryTabs.Quantity_WriteOff * x.docInventoryTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
                            */

                        }

                        #endregion

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

        // PUT: api/DocInventories/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocInventory(int id, DocInventory docInventory, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocInventories"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                //Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
                if (UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
                UO_Action = UO_Action.ToLower();

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocInventoryTab[] docInventoryTabCollection = null;
                if (!String.IsNullOrEmpty(docInventory.recordsDocInventoryTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docInventoryTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocInventoryTab[]>(docInventory.recordsDocInventoryTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docInventory.DocInventoryID || docInventory.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docInventory.DocID" из БД, если он отличается от пришедшего от клиента "docInventory.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocInventories
                        where x.DocInventoryID == docInventory.DocInventoryID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docInventory.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                /*
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docInventory.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));
                */


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docInventory.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docInventory = await Task.Run(() => mPutPostDocInventory(db, dbRead, UO_Action, docInventory, EntityState.Modified, docInventoryTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docInventory.DocInventoryID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docInventory.DocID,
                    DocInventoryID = docInventory.DocInventoryID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocInventories
        [ResponseType(typeof(DocInventory))]
        public async Task<IHttpActionResult> PostDocInventory(DocInventory docInventory, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocInventories"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
                if (UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
                UO_Action = UO_Action.ToLower();

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocInventoryTab[] docInventoryTabCollection = null;
                if (!String.IsNullOrEmpty(docInventory.recordsDocInventoryTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docInventoryTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocInventoryTab[]>(docInventory.recordsDocInventoryTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docInventory.DocID" из БД, если он отличается от пришедшего от клиента "docInventory.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocInventories
                        where x.DocInventoryID == docInventory.DocInventoryID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docInventory.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docInventory.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docInventory = await Task.Run(() => mPutPostDocInventory(db, dbRead, UO_Action, docInventory, EntityState.Added, docInventoryTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docInventory.DocInventoryID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docInventory.DocID,
                    DocInventoryID = docInventory.DocInventoryID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocInventories/5
        [ResponseType(typeof(DocInventory))]
        public async Task<IHttpActionResult> DeleteDocInventory(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocInventories"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Проверки

                try
                {
                    //Документ проведён!<BR>Перед удалением, нужно отменить проводку!
                    var queryHeld = await Task.Run(() =>
                        (
                            from x in dbRead.DocInventories
                            where x.DocInventoryID == id
                            select x
                        ).ToListAsync());

                    if (queryHeld.Count() > 0)
                        if (Convert.ToBoolean(queryHeld[0].doc.Held))
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg18)); //return BadRequest();
                }
                catch (Exception ex)
                {
                    return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                }

                #endregion


                #region Удаление

                //Алгоритм.
                //Удаляем по порядку:
                //1. RemParties
                //2. DocInventoryTabs
                //3. DocInventories
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocInventory docInventory = await db.DocInventories.FindAsync(id);
                if (docInventory == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. RemPartyMinuses

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocInventories
                                where x.DocInventoryID == id
                                select x
                            ).ToListAsync();
                        if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                        //1.1. Удаляем "RemPartyMinuses"
                        var queryRemPartyMinuses = await
                            (
                                from x in db.RemPartyMinuses
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryRemPartyMinuses.Count(); i++)
                        {
                            Models.Sklad.Rem.RemPartyMinus remPartyMinus = await db.RemPartyMinuses.FindAsync(queryRemPartyMinuses[i].RemPartyMinusID);
                            db.RemPartyMinuses.Remove(remPartyMinus);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 1. RemParties

                        //1.1. Ищим DocID - нашли выше
                        /*
                        int iDocID = 0;
                        var queryDocs2 = await
                            (
                                from x in db.DocInventoryes
                                where x.DocInventoryID == id
                                select x
                            ).ToListAsync();
                        if (queryDocs2.Count() > 0) iDocID = Convert.ToInt32(queryDocs2[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));
                        */

                        //1.1. Удаляем "RemParties"
                        var queryRemParties = await
                            (
                                from x in db.RemParties
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryRemParties.Count(); i++)
                        {
                            Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(queryRemParties[i].RemPartyID);
                            db.RemParties.Remove(remParty);
                            await db.SaveChangesAsync();
                        }

                        #endregion



                        #region 2. DocInventoryTabs

                        var queryDocInventoryTabs = await
                            (
                                from x in db.DocInventoryTabs
                                where x.DocInventoryID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocInventoryTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocInventoryTab docInventoryTab = await db.DocInventoryTabs.FindAsync(queryDocInventoryTabs[i].DocInventoryTabID);
                            db.DocInventoryTabs.Remove(docInventoryTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocInventories

                        var queryDocInventories = await
                            (
                                from x in db.DocInventories
                                where x.DocInventoryID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocInventories.Count(); i++)
                        {
                            Models.Sklad.Doc.DocInventory docInventory1 = await db.DocInventories.FindAsync(queryDocInventories[i].DocInventoryID);
                            db.DocInventories.Remove(docInventory1);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 4. Doc

                        var queryDocs2 = await
                            (
                                from x in db.Docs
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocs2.Count(); i++)
                        {
                            Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(queryDocs2[i].DocID);
                            db.Docs.Remove(doc);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        ts.Commit();


                        #region 6. JourDisp

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
                            ID = id,
                            Msg = Classes.Language.Sklad.Language.msg19
                        };
                        return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    } //catch

                } //DbContextTransaction

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
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

        private bool DocInventoryExists(int id)
        {
            return db.DocInventories.Count(e => e.DocInventoryID == id) > 0;
        }



        //Алгоритм:
        //Пробегаемся по спецификации
        // - Если есть партия (RemPartyID != 0), то это списание и списываем товар
        // - Если нет партии (RemPartyID == 0), то это приход и приходуем товар, только если проводим документ

        int i300 = 0;
        internal async Task<DocInventory> mPutPostDocInventory(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocInventory docInventory,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocInventoryTab[] docInventoryTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "held") docInventory.Reserve = false;
            else docInventory.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docInventory.NumberInt;
                doc.NumberReal = docInventory.DocInventoryID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = 1;
                doc.Payment = 0;
                doc.DirContractorID = docInventory.DirContractorIDOrg;
                doc.DirContractorIDOrg = docInventory.DirContractorIDOrg;
                doc.Discount = docInventory.Discount;
                doc.DirVatValue = docInventory.DirVatValue;
                doc.Base = docInventory.Base;
                doc.Description = docInventory.Description;
                doc.DocDate = docInventory.DocDate;
                //doc.DocDisc = docInventory.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docInventory.DocID;
                doc.DocIDBase = docInventory.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                docs.DirWarehouseID = docInventory.DirWarehouseID;
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docInventory" со всем полями!
                docInventory.DocID = doc.DocID;

                #endregion

                #region 2. DocInventory

                docInventory.DocID = doc.DocID;

                db.Entry(docInventory).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docInventory.doc.NumberInt == null || docInventory.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docInventory.DocInventoryID.ToString();
                    doc.NumberReal = docInventory.DocInventoryID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docInventory.DocInventoryID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocInventoryTab

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocInventoryID = new SQLiteParameter("@DocInventoryID", System.Data.DbType.Int32) { Value = docInventory.DocInventoryID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocInventoryTabs WHERE DocInventoryID=@DocInventoryID;", parDocInventoryID);
                }

                //2.2. Проставляем ID-шник "DocInventoryID" для всех позиций спецификации
                for (int i = 0; i < docInventoryTabCollection.Count(); i++)
                {
                    if (docInventoryTabCollection[i].RemPartyID == 0) docInventoryTabCollection[i].RemPartyID = null;
                    docInventoryTabCollection[i].DocInventoryTabID = null;
                    docInventoryTabCollection[i].DocInventoryID = Convert.ToInt32(docInventory.DocInventoryID);
                    db.Entry(docInventoryTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion


                if (UO_Action == "held" || docInventory.Reserve)
                {

                    #region Списание

                    Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();


                    #region Удаляем все записи из таблицы "RemPartyMinuses"
                    //Удаляем все записи из таблицы "RemPartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (RemPartyMinuses)

                    for (int i = 0; i < docInventoryTabCollection.Count(); i++)
                    {
                        if (docInventoryTabCollection[i].Quantity_WriteOff > 0)
                        {
                            #region Проверка

                            //Переменные
                            int iRemPartyID = Convert.ToInt32(docInventoryTabCollection[i].RemPartyID);
                            double dQuantity = docInventoryTabCollection[i].Quantity_WriteOff;
                            //Находим партию
                            Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(iRemPartyID);
                            db.Entry(remParty).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                            #region 1. Есть ли остаток в партии с которой списываем!
                            if (remParty.Remnant < dQuantity)
                            {
                                throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg104 +

                                    "<tr>" +
                                    "<td>" + docInventoryTabCollection[i].RemPartyID + "</td>" +                                //партия
                                    "<td>" + docInventoryTabCollection[i].DirNomenID + "</td>" +                                //Код товара
                                    "<td>" + docInventoryTabCollection[i].Quantity_WriteOff + "</td>" +                                  //списуемое к-во
                                    "<td>" + remParty.Remnant + "</td>" +                                                  //остаток партии
                                    "<td>" + (docInventoryTabCollection[i].Quantity_WriteOff - remParty.Remnant).ToString() + "</td>" +  //недостающее к-во
                                    "</tr>" +
                                    "</table>" +

                                    Classes.Language.Sklad.Language.msg104_1
                                );
                            }
                            #endregion

                            #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                            if (remParty.DirWarehouseID != docInventory.DirWarehouseID)
                            {
                                //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docInventory.dirWarehouse.DirWarehouseName"
                                Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docInventory.DirWarehouseID);

                                throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg105 +

                                    "<tr>" +
                                    "<td>" + docInventoryTabCollection[i].RemPartyID + "</td>" +           //партия
                                    "<td>" + docInventoryTabCollection[i].DirNomenID + "</td>" +           //Код товара
                                    "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                    "<td>" + remParty.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                    "</tr>" +
                                    "</table>" +

                                    Classes.Language.Sklad.Language.msg105_1
                                );
                            }
                            #endregion

                            #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                            if (remParty.DirContractorIDOrg != docInventory.DirContractorIDOrg)
                            {
                                //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docInventory.dirWarehouse.DirWarehouseName"
                                Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docInventory.DirContractorIDOrg);

                                throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg106 +

                                    "<tr>" +
                                    "<td>" + docInventoryTabCollection[i].RemPartyID + "</td>" +           //партия
                                    "<td>" + docInventoryTabCollection[i].DirNomenID + "</td>" +           //Код товара
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
                            remPartyMinus.RemPartyID = docInventoryTabCollection[i].RemPartyID;
                            remPartyMinus.DirNomenID = docInventoryTabCollection[i].DirNomenID;
                            remPartyMinus.Quantity = docInventoryTabCollection[i].Quantity_WriteOff;
                            remPartyMinus.DirCurrencyID = docInventoryTabCollection[i].DirCurrencyID;
                            remPartyMinus.DirCurrencyMultiplicity = docInventoryTabCollection[i].DirCurrencyMultiplicity;
                            remPartyMinus.DirCurrencyRate = docInventoryTabCollection[i].DirCurrencyRate;
                            remPartyMinus.DirVatValue = docInventory.DirVatValue;
                            remPartyMinus.DirWarehouseID = docInventory.DirWarehouseID;
                            remPartyMinus.DirContractorIDOrg = docInventory.DirContractorIDOrg;
                            remPartyMinus.DirContractorID = docInventory.DirContractorIDOrg;
                            remPartyMinus.DocID = Convert.ToInt32(docInventory.DocID);
                            remPartyMinus.PriceCurrency = docInventoryTabCollection[i].PriceCurrency;
                            remPartyMinus.PriceVAT = docInventoryTabCollection[i].PriceVAT;
                            remPartyMinus.FieldID = Convert.ToInt32(docInventoryTabCollection[i].DocInventoryTabID);
                            remPartyMinus.Reserve = docInventory.Reserve;
                            remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
                            remPartyMinus.DocDate = doc.DocDate;

                            db.Entry(remPartyMinus).State = EntityState.Added;
                            await db.SaveChangesAsync();

                            #endregion

                        }
                    }

                    #endregion

                    #endregion



                    //Приходуем только при Проведении
                    if (UO_Action == "held")
                    {
                        #region OLD

                        //Есть только приход
                        //1. Списываем остаток с партии "Remnant" одним запросом
                        //2. Приходуем приход


                        /*
                        SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docInventory.DocID };
                        SQLiteParameter parDirContractorIDOrg = new SQLiteParameter("@DirContractorIDOrg", System.Data.DbType.Int32) { Value = doc.DirContractorIDOrg };
                        SQLiteParameter parDirWarehouseID = new SQLiteParameter("@DirWarehouseID", System.Data.DbType.Int32) { Value = docInventory.DirWarehouseID };
                        SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = docInventory.Reserve };
                        SQLiteParameter parDocDate = new SQLiteParameter("@DocDate", System.Data.DbType.Date) { Value = doc.DocDate };
                        */


                        #region 1. Помечаем все документы как "Инвентаризированные" - !!! НЕ ИСПОЛЬЗУЕТСЯ !!!
                        //Кроме:
                        //Инвентаризация с датой >= DocDate

                        /*
                        string SQL =
                            "UPDATE Docs " +
                            "SET IsInv=@IsInv, InvDocID=@DocID " +
                            "WHERE " + //"(ListObjectID<>39 or DocDate<@DocDate)and(InvDocID is null)and" +
                            "(InvDocID is null)and" + 
                            "(" +
                            "DocID IN " +
                            " (" +
                            "  SELECT par.DocID " +
                            "  FROM RemParties AS par " +
                            "  WHERE par.DirContractorIDOrg=@DirContractorIDOrg and par.DirWarehouseID=@DirWarehouseID and par.Remnant>0 " +
                            "  UNION ALL " +
                            "  SELECT parMin.DocID " +
                            "  FROM RemPartyMinuses AS parMin " +
                            "  WHERE parMin.DirContractorIDOrg=@DirContractorIDOrg and parMin.DirWarehouseID=@DirWarehouseID " +
                            " )" +
                            ")";

                        parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docInventory.DocID };
                        SQLiteParameter parIsInv = new SQLiteParameter("@IsInv", System.Data.DbType.Boolean) { Value = true };
                        //SQLiteParameter parDocDate = new SQLiteParameter("@DocDate", System.Data.DbType.Date) { Value = doc.DocDate };
                        db.Database.ExecuteSqlCommand(SQL, parDocID, parIsInv, parDirContractorIDOrg, parDirWarehouseID);
                        */

                        #endregion


                        #region 2. Списываем остаток с партии "Remnant" одним запросом (RemPartyMinuses - Списание партий) - !!! НЕ ИСПОЛЬЗУЕТСЯ !!!

                        /*
                        string SQL =
                            "INSERT INTO RemPartyMinuses " +
                            "(RemPartyID, DocID, DirNomenID, DirWarehouseID, DirContractorIDOrg, DirContractorID, Quantity, PriceVAT, DirVatValue, PriceCurrency, DirCurrencyID, DirCurrencyRate, DirCurrencyMultiplicity, Reserve, FieldID, DirEmployeeID, DocDate) " +
                            "SELECT par.RemPartyID, @DocID, par.DirNomenID, par.DirWarehouseID, @DirContractorIDOrg, @DirContractorIDOrg, par.Remnant, par.PriceVAT, par.DirVatValue, par.PriceCurrency, par.DirCurrencyID, cur.DirCurrencyRate, cur.DirCurrencyMultiplicity, @Reserve, 0, d.DirEmployeeID, d.DocDate " +
                            "FROM Docs AS d, RemParties AS par, DirCurrencies AS cur " +
                            "WHERE " +
                            " d.DocID=par.DocID and d.DocDate<=@DocDate and " +
                            " par.DirCurrencyID=cur.DirCurrencyID and " +
                            " par.DirContractorIDOrg=@DirContractorIDOrg and par.DirWarehouseID=@DirWarehouseID and " +
                            " par.Remnant>0 ";

                        db.Database.ExecuteSqlCommand(SQL, parDocID, parDocDate, parDirContractorIDOrg, parDirWarehouseID, parReserve);
                        */

                        #endregion


                        #region 2. Приходуем приход (RemParty - Партии) - !!! НЕ ИСПОЛЬЗУЕТСЯ !!!

                        /*
                        Models.Sklad.Rem.RemParty[] remPartyCollection = new Models.Sklad.Rem.RemParty[docInventoryTabCollection.Count()];
                        for (int i = 0; i < docInventoryTabCollection.Count(); i++)
                        {
                            Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();
                            remParty.RemPartyID = null;
                            remParty.DirNomenID = docInventoryTabCollection[i].DirNomenID;
                            remParty.Quantity = docInventoryTabCollection[i].Quantity;
                            remParty.Remnant = docInventoryTabCollection[i].Quantity;
                            remParty.DirCurrencyID = docInventoryTabCollection[i].DirCurrencyID;
                            //remParty.DirCurrencyMultiplicity = docInventoryTabCollection[i].DirCurrencyMultiplicity;
                            //remParty.DirCurrencyRate = docInventoryTabCollection[i].DirCurrencyRate;
                            remParty.DirVatValue = docInventory.DirVatValue;
                            remParty.DirWarehouseID = docInventory.DirWarehouseID;
                            remParty.DirWarehouseIDDebit = docInventory.DirWarehouseID;
                            remParty.DirWarehouseIDPurch = docInventory.DirWarehouseID;
                            remParty.DirContractorIDOrg = docInventory.DirContractorIDOrg;
                            remParty.DirContractorID = docInventory.DirContractorIDOrg;

                            //Дата Приёмки товара
                            remParty.DocDatePurches = docInventory.doc.DocDate;

                            remParty.DirCharColourID = docInventoryTabCollection[i].DirCharColourID;
                            remParty.DirCharMaterialID = docInventoryTabCollection[i].DirCharMaterialID;
                            remParty.DirCharNameID = docInventoryTabCollection[i].DirCharNameID;
                            remParty.DirCharSeasonID = docInventoryTabCollection[i].DirCharSeasonID;
                            remParty.DirCharSexID = docInventoryTabCollection[i].DirCharSexID;
                            remParty.DirCharSizeID = docInventoryTabCollection[i].DirCharSizeID;
                            remParty.DirCharStyleID = docInventoryTabCollection[i].DirCharStyleID;
                            remParty.DirCharTextureID = docInventoryTabCollection[i].DirCharTextureID;

                            remParty.SerialNumber = docInventoryTabCollection[i].SerialNumber;
                            remParty.Barcode = docInventoryTabCollection[i].Barcode;

                            remParty.DocID = Convert.ToInt32(docInventory.DocID);
                            remParty.PriceCurrency = docInventoryTabCollection[i].PriceCurrency;
                            remParty.PriceVAT = docInventoryTabCollection[i].PriceVAT;
                            remParty.FieldID = Convert.ToInt32(docInventoryTabCollection[i].DocInventoryTabID);

                            remParty.PriceRetailVAT = docInventoryTabCollection[i].PriceRetailVAT;
                            remParty.PriceRetailCurrency = docInventoryTabCollection[i].PriceRetailCurrency;
                            remParty.PriceWholesaleVAT = docInventoryTabCollection[i].PriceWholesaleVAT;
                            remParty.PriceWholesaleCurrency = docInventoryTabCollection[i].PriceWholesaleCurrency;
                            remParty.PriceIMVAT = docInventoryTabCollection[i].PriceIMVAT;
                            remParty.PriceIMCurrency = docInventoryTabCollection[i].PriceIMCurrency;

                            remParty.DirEmployeeID = doc.DirEmployeeID;
                            remParty.DocDate = doc.DocDate;

                            remPartyCollection[i] = remParty;
                        }

                        Controllers.Sklad.Rem.RemPartiesController remPartys = new Rem.RemPartiesController();
                        await Task.Run(() => remPartys.Save(db, remPartyCollection)); //remPartys.Save(db, remPartyCollection);
                        */

                        #endregion

                        #endregion


                        #region Оприходование


                        //Получаем к-во для "remPartyCollection" (ниже)
                        int docInventoryTabCollection_Count = 0, i_docInventoryTabCollection_Count = -1;
                        for (int i = 0; i < docInventoryTabCollection.Count(); i++)
                        {
                            if (docInventoryTabCollection[i].Quantity_Purch > 0)
                            {
                                docInventoryTabCollection_Count++;
                            }
                        }


                        Models.Sklad.Rem.RemParty[] remPartyCollection = new Models.Sklad.Rem.RemParty[docInventoryTabCollection_Count];
                        for (int i = 0; i < docInventoryTabCollection.Count(); i++)
                        {
                            if (docInventoryTabCollection[i].Quantity_Purch > 0)
                            {
                                Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();
                                remParty.RemPartyID = null;
                                remParty.DirNomenID = docInventoryTabCollection[i].DirNomenID;
                                remParty.Quantity = docInventoryTabCollection[i].Quantity_Purch;
                                remParty.Remnant = docInventoryTabCollection[i].Quantity_Purch;
                                remParty.DirCurrencyID = docInventoryTabCollection[i].DirCurrencyID;
                                //remParty.DirCurrencyMultiplicity = docInventoryTabCollection[i].DirCurrencyMultiplicity;
                                //remParty.DirCurrencyRate = docInventoryTabCollection[i].DirCurrencyRate;
                                remParty.DirVatValue = docInventory.DirVatValue;
                                remParty.DirWarehouseID = docInventory.DirWarehouseID;
                                remParty.DirWarehouseIDDebit = docInventory.DirWarehouseID;
                                remParty.DirWarehouseIDPurch = docInventory.DirWarehouseID;
                                remParty.DirContractorIDOrg = docInventory.DirContractorIDOrg;

                                if (docInventoryTabCollection[i].DirContractorID > 0) remParty.DirContractorID = Convert.ToInt32(docInventoryTabCollection[i].DirContractorID);
                                else remParty.DirContractorID = docInventory.DirContractorIDOrg;

                                //Дата Приёмки товара
                                remParty.DocDatePurches = docInventory.doc.DocDate;

                                remParty.DirNomenMinimumBalance = docInventoryTabCollection[i].DirNomenMinimumBalance;

                                remParty.DirCharColourID = docInventoryTabCollection[i].DirCharColourID;
                                remParty.DirCharMaterialID = docInventoryTabCollection[i].DirCharMaterialID;
                                remParty.DirCharNameID = docInventoryTabCollection[i].DirCharNameID;
                                remParty.DirCharSeasonID = docInventoryTabCollection[i].DirCharSeasonID;
                                remParty.DirCharSexID = docInventoryTabCollection[i].DirCharSexID;
                                remParty.DirCharSizeID = docInventoryTabCollection[i].DirCharSizeID;
                                remParty.DirCharStyleID = docInventoryTabCollection[i].DirCharStyleID;
                                remParty.DirCharTextureID = docInventoryTabCollection[i].DirCharTextureID;

                                remParty.SerialNumber = docInventoryTabCollection[i].SerialNumber;
                                remParty.Barcode = docInventoryTabCollection[i].Barcode;

                                remParty.DocID = Convert.ToInt32(docInventory.DocID);
                                remParty.PriceCurrency = docInventoryTabCollection[i].PriceVAT;
                                remParty.PriceVAT = docInventoryTabCollection[i].PriceVAT;
                                remParty.FieldID = Convert.ToInt32(docInventoryTabCollection[i].DocInventoryTabID);

                                remParty.PriceRetailVAT = docInventoryTabCollection[i].PriceRetailVAT;
                                remParty.PriceRetailCurrency = docInventoryTabCollection[i].PriceRetailCurrency;
                                remParty.PriceWholesaleVAT = docInventoryTabCollection[i].PriceWholesaleVAT;
                                remParty.PriceWholesaleCurrency = docInventoryTabCollection[i].PriceWholesaleCurrency;
                                remParty.PriceIMVAT = docInventoryTabCollection[i].PriceIMVAT;
                                remParty.PriceIMCurrency = docInventoryTabCollection[i].PriceIMCurrency;

                                remParty.DirEmployeeID = doc.DirEmployeeID;
                                remParty.DocDate = doc.DocDate;

                                i_docInventoryTabCollection_Count++;
                                remPartyCollection[i_docInventoryTabCollection_Count] = remParty;
                            }
                        }

                        Controllers.Sklad.Rem.RemPartiesController remPartys = new Rem.RemPartiesController();
                        await Task.Run(() => remPartys.Save(db, remPartyCollection)); //remPartys.Save(db, remPartyCollection);

                        #endregion

                    }

                }
            }
            else if (UO_Action == "held_cancel")
            {
                //Doc.Held = false
                #region 1. Doc

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docInventory.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion


                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docInventory.DocID };
                SQLiteParameter parDocIDNUll = new SQLiteParameter("@DocIDNUll", System.Data.DbType.Int32) { Value = null };
                SQLiteParameter parIsInv = new SQLiteParameter("@IsInv", System.Data.DbType.Boolean) { Value = null };
                SQLiteParameter parDocDate = new SQLiteParameter("@DocDate", System.Data.DbType.Date) { Value = doc.DocDate };


                #region 2. Помечаем все документы как "Инвентаризированные"
                //Кроме:
                //Инвентаризация с датой >= DocDate

                /*
                string SQL =
                    "UPDATE Docs " +
                    "SET IsInv=@IsInv, InvDocID=@DocIDNUll " +
                    "WHERE InvDocID=@DocID"; //"WHERE (ListObjectID<>39 or DocDate<@DocDate)and(InvDocID=@DocID)";

                db.Database.ExecuteSqlCommand(SQL, parDocID, parDocIDNUll, parIsInv, parDocDate);
                */

                #endregion


                //Для Списания (Мы же обнуляем остатки товара) === === === === === === === === === === === === === === === ===
                #region 3. RemPartyMinuses и RemParties - Не используется
                /*
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docInventory.DocID };
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE RemPartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);
                */
                #endregion


                //Для Прихода === === === === === === === === === === === === === === === ===
                #region 4. Проверка: Было ли списание с партий


                //int DocInventoryID = Convert.ToInt32(docInventory.DocInventoryID);


                //Этот 2-й алгоритм глючит! Если поле "Remnant==Quantity", а списания были (глюкануло что-то), то выдаст сообщение, что "Связи между таблицами нарушены!"

                //Алгоритм №2
                //Пробегаемся по всем "RemParties.Remnant"
                //и есть оно отличается от "RemParties.Quantity"
                //то был списан товар

                /*

                //Получаем DocInventory из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocInventory _docInventory = db.DocInventories.Find(DocInventoryID);
                int? iDocInventory_DocID = _docInventory.DocID;

                //Есть ли списанный товар с данного прихода
                var queryRemParties = await Task.Run(() =>
                    (
                        from x in db.RemParties
                        where x.DocID == iDocInventory_DocID && x.Quantity != x.Remnant
                        select x
                    ).ToListAsync());

                //Есть!
                if (queryRemParties.Count() > 0)
                {
                    //Смотрим, какие именно накладные списали товар.
                    var queryRemPartyMinuses = await Task.Run(() =>
                        (
                            from remPartyMinuses in db.RemPartyMinuses

                            join remParties1 in db.RemParties on remPartyMinuses.RemPartyID equals remParties1.RemPartyID into remParties2
                            from remParties in remParties2.Where(x => x.DocID == iDocInventory_DocID) //.DefaultIfEmpty()

                            select new
                            {
                                DocID = remPartyMinuses.DocID,
                                ListObjectNameRu = remPartyMinuses.doc.listObject.ListObjectNameRu
                            }
                        ).Distinct().ToListAsync()); // - убрать повторяющиеся

                    //Есть списания!
                    if (queryRemPartyMinuses.Count() > 0)
                    {
                        //Поиск всех DocID
                        string arrDocID = "";
                        for (int i = 0; i < queryRemPartyMinuses.Count(); i++)
                        {
                            arrDocID += queryRemPartyMinuses[i].DocID + " (" + queryRemPartyMinuses[i].ListObjectNameRu + ")";
                            if (i != queryRemPartyMinuses.Count() - 1) arrDocID += "<br />";
                        }
                        //Сообщение клиенту
                        throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg102 + arrDocID + Classes.Language.Sklad.Language.msg102_1);
                    }

                }

                */

                int DocInventoryID = Convert.ToInt32(docInventory.DocInventoryID);

                Models.Sklad.Doc.DocInventory _docInventory = db.DocInventories.Find(DocInventoryID);
                int? iDocInventory_DocID = _docInventory.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocInventory_DocID));

                #endregion


                //Удаление записей в таблицах: RemParties
                #region 5. RemParties - удаление

                //Проверяем если ли расходы (проведённый или НЕ проведенные)


                //Удаляем записи в таблице "RemParties"
                //SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docInventory.DocID };
                //await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID; ", parDocID);
                //await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemPartyMinuses WHERE DocID=@DocID; ", parDocID);

                //await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID; DELETE FROM RemPartyMinuses WHERE DocID=@DocID; ", parDocID);
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID; ", parDocID);

                #endregion

            }

            #region n. Подтверждение транзакции

            //ts.Commit(); //.Complete();

            #endregion

            return docInventory;
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
                "[DocInventories].[DocInventoryID] AS [DocInventoryID], " +
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
                "[DirPaymentTypes].[DirPaymentTypeName] AS [DirPaymentTypeName], " +

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

                //"[DirWarehouses].[DirWarehouseID] AS [DirWarehouseID], " +
                "[DirWarehouses].[DirWarehouseName] AS [DirWarehouseName], " +
                "[DirWarehouses].[DirWarehouseAddress] AS [DirWarehouseAddress], " +
                "[DirWarehouses].[DirWarehouseDesc] AS [DirWarehouseDesc], " +

                "[Docs].[NumberInt] AS [NumberInt], " +
                "[DocInventories].[Reserve] AS [Reserve] " +

                "FROM [DocInventories] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocInventories].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "INNER JOIN [DirWarehouses] AS [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocInventories].[DirWarehouseID] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion
    }
}