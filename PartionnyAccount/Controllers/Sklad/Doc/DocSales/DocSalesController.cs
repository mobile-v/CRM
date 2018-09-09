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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSales
{
    public class DocSalesController : ApiController
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

        int ListObjectID = 32;

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
        // GET: api/DocSales
        public async Task<IHttpActionResult> GetDocSales(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSales"));
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
                        from docSales in db.DocSales

                        join docSaleTabs1 in db.DocSaleTabs on docSales.DocSaleID equals docSaleTabs1.DocSaleID into docSaleTabs2
                        from docSaleTabs in docSaleTabs2.DefaultIfEmpty()

                        where docSales.doc.DocDate >= _params.DateS && docSales.doc.DocDate <= _params.DatePo

                        group new { docSaleTabs }
                        by new
                        {
                            DocID = docSales.DocID,
                            DocDate = docSales.doc.DocDate,
                            Base = docSales.doc.Base,
                            Held = docSales.doc.Held,
                            Discount = docSales.doc.Discount,
                            Del = docSales.doc.Del,
                            Description = docSales.doc.Description,
                            IsImport = docSales.doc.IsImport,
                            DirVatValue = docSales.doc.DirVatValue,

                            DocSaleID = docSales.DocSaleID,
                            DirContractorID = docSales.doc.dirContractor.DirContractorID, DirContractorName = docSales.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSales.doc.dirContractorOrg.DirContractorID, DirContractorNameOrg = docSales.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSales.dirWarehouse.DirWarehouseID, DirWarehouseName = docSales.dirWarehouse.DirWarehouseName,

                            NumberInt = docSales.doc.NumberInt,
                            NumberTT = docSales.NumberTT,
                            NumberTax = docSales.NumberTax,

                            //Оплата
                            Payment = docSales.doc.Payment,

                            DocDateHeld = docSales.doc.DocDateHeld,
                            DocDatePayment = docSales.doc.DocDatePayment,
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

                            DocSaleID = g.Key.DocSaleID,

                            DirContractorID = g.Key.DirContractorID, DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg, DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID, DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            NumberTT = g.Key.NumberTT,
                            NumberTax = g.Key.NumberTax,
                            DirVatValue = g.Key.DirVatValue,

                            DocDateHeld = g.Key.DocDateHeld,
                            DocDatePayment = g.Key.DocDatePayment,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSaleTabs.Quantity * x.docSaleTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSaleTabs.Quantity * x.docSaleTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docSaleTabs.Quantity * x.docSaleTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSaleTabs.Quantity * x.docSaleTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum)
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
                        query = query.Where(x => x.DocSaleID == iNumber32);
                    }
                    //Если Дата
                    else if (bDateTime)
                    {
                        query = query.Where(x => x.DocDate == dDateTime);
                    }
                    //Иначе, только текстовые поля
                    else
                    {
                        query = query.Where(x => x.DirContractorName.Contains(_params.parSearch) || x.DirWarehouseName.Contains(_params.parSearch) || x.NumberInt.Contains(_params.parSearch) || x.NumberTT.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                //query = query.OrderByDescending(x => x.DocDate).Skip(_params.Skip).Take(_params.limit);

                if (sysSetting.DocsSortField == 1)
                {
                    query = query.OrderByDescending(x => x.DocSaleID);
                }
                else if (sysSetting.DocsSortField == 2)
                {
                    query = query.OrderByDescending(x => x.DocDate);
                }
                else if (sysSetting.DocsSortField == 3)
                {
                    query = query.OrderByDescending(x => x.DocDateHeld);
                }
                else if (sysSetting.DocsSortField == 4)
                {
                    query = query.OrderByDescending(x => x.DocDatePayment);
                }
                else
                {
                    query = query.OrderByDescending(x => x.DocSaleID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocSales.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSale = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSales/5
        [ResponseType(typeof(DocSale))]
        public async Task<IHttpActionResult> GetDocSale(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSales"));
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
                        from docSales in db.DocSales
                        where docSales.DocSaleID == id
                        select docSales
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docSales in db.DocSales

                        join docSaleTabs1 in db.DocSaleTabs on docSales.DocSaleID equals docSaleTabs1.DocSaleID into docSaleTabs2
                        from docSaleTabs in docSaleTabs2.DefaultIfEmpty()

                        #endregion

                        where docSales.DocSaleID == id

                        #region group

                        group new { docSaleTabs }
                        by new
                        {
                            DocID = docSales.DocID, //DocID = docSales.doc.DocID,
                            DocIDBase = docSales.doc.DocIDBase,
                            DocDate = docSales.doc.DocDate,
                            Base = docSales.doc.Base,
                            Held = docSales.doc.Held,
                            Discount = docSales.doc.Discount,
                            Del = docSales.doc.Del,
                            IsImport = docSales.doc.IsImport,
                            Description = docSales.doc.Description,
                            DirVatValue = docSales.doc.DirVatValue,
                            //DirPaymentTypeID = docSales.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docSales.doc.dirPaymentType.DirPaymentTypeName,

                            DocSaleID = docSales.DocSaleID,
                            DirContractorID = docSales.doc.DirContractorID,
                            DirContractorName = docSales.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSales.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docSales.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSales.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSales.dirWarehouse.DirWarehouseName,
                            NumberInt = docSales.doc.NumberInt,
                            NumberTT = docSales.NumberTT,
                            NumberTax = docSales.NumberTax,

                            //Оплата
                            Payment = docSales.doc.Payment,

                            //Резерв
                            Reserve = docSales.Reserve
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
                            Discount = g.Key.Discount,
                            Del = g.Key.Del,
                            Description = g.Key.Description,
                            IsImport = g.Key.IsImport,
                            DirVatValue = g.Key.DirVatValue,
                            //DirPaymentTypeID = g.Key.DirPaymentTypeID,
                            //DirPaymentTypeName = g.Key.DirPaymentTypeName,

                            DocSaleID = g.Key.DocSaleID,
                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,
                            NumberTT = g.Key.NumberTT,
                            NumberTax = g.Key.NumberTax,

                            Payment = g.Key.Payment,

                            //Сумма с НДС
                            SumDocServicePurch1Tabs =
                            g.Sum(x => x.docSaleTabs.Quantity * x.docSaleTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSaleTabs.Quantity * x.docSaleTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            
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

        // PUT: api/DocSales/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSale(int id, DocSale docSale, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSales"));
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
                Models.Sklad.Doc.DocSaleTab[] docSaleTabCollection = null;
                if (!String.IsNullOrEmpty(docSale.recordsDocSaleTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSaleTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSaleTab[]>(docSale.recordsDocSaleTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docSale.DocSaleID || docSale.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docSale.DocID" из БД, если он отличается от пришедшего от клиента "docSale.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSales
                        where x.DocSaleID == docSale.DocSaleID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSale.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docSale.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSale.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSale = await Task.Run(() => mPutPostDocSale(db, dbRead, UO_Action, docSale, EntityState.Modified, docSaleTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSale.DocSaleID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSale.DocID,
                    DocSaleID = docSale.DocSaleID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocSales
        [ResponseType(typeof(DocSale))]
        public async Task<IHttpActionResult> PostDocSale(DocSale docSale, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSales"));
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
                Models.Sklad.Doc.DocSaleTab[] docSaleTabCollection = null;
                if (!String.IsNullOrEmpty(docSale.recordsDocSaleTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSaleTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSaleTab[]>(docSale.recordsDocSaleTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docSale.DocID" из БД, если он отличается от пришедшего от клиента "docSale.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSales
                        where x.DocSaleID == docSale.DocSaleID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSale.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSale.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSale = await Task.Run(() => mPutPostDocSale(db, dbRead, UO_Action, docSale, EntityState.Added, docSaleTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSale.DocSaleID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSale.DocID,
                    DocSaleID = docSale.DocSaleID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSales/5
        [ResponseType(typeof(DocSale))]
        public async Task<IHttpActionResult> DeleteDocSale(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSales"));
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
                            from x in dbRead.DocSales
                            where x.DocSaleID == id
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
                //2. DocSaleTabs
                //3. DocSales
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocSale docSale = await db.DocSales.FindAsync(id);
                if (docSale == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Ищим DocID *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocSales
                                where x.DocSaleID == id
                                select x
                            ).ToListAsync();
                        if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                        #endregion


                        #region 1. Ищим в Возврате покупателя, если нет, то удаляем в RemPartyMinuses *** *** *** *** ***

                        //1.1. Удаляем "RemPartyMinuses"
                        var queryRemPartyMinuses = await
                            (
                                from x in db.RemPartyMinuses
                                where x.DocID == iDocID
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

                            if (queryDocReturnsCustomerTab.Count() > 0) {
                                throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg112 +

                                    "<tr>" +
                                    "<td>" + queryDocReturnsCustomerTab[0].RemPartyMinusID + "</td>" +                           //партия списания
                                    "<td>" + queryDocReturnsCustomerTab[0].DocReturnsCustomerID + "</td>" +                      //№ д-та
                                    "<td>" + queryDocReturnsCustomerTab[0].DirNomenID + "</td>" +                                //Код товара
                                    "<td>" + queryDocReturnsCustomerTab[0].Quantity + "</td>" +                                  //списуемое к-во
                                    "</tr>" +
                                    "</table>" +

                                    Classes.Language.Sklad.Language.msg112_1
                                    );
                            }

                            Models.Sklad.Rem.RemPartyMinus remPartyMinus = await db.RemPartyMinuses.FindAsync(iRemPartyMinusID);
                            db.RemPartyMinuses.Remove(remPartyMinus);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 2. DocSaleTabs *** *** *** *** ***

                        var queryDocSaleTabs = await
                            (
                                from x in db.DocSaleTabs
                                where x.DocSaleID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSaleTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSaleTab docSaleTab = await db.DocSaleTabs.FindAsync(queryDocSaleTabs[i].DocSaleTabID);
                            db.DocSaleTabs.Remove(docSaleTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocSales *** *** *** *** ***

                        var queryDocSales = await
                            (
                                from x in db.DocSales
                                where x.DocSaleID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSales.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSale docSale1 = await db.DocSales.FindAsync(queryDocSales[i].DocSaleID);
                            db.DocSales.Remove(docSale1);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 4. Doc *** *** *** *** ***

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

        private bool DocSaleExists(int id)
        {
            return db.DocSales.Count(e => e.DocSaleID == id) > 0;
        }


        internal async Task<DocSale> mPutPostDocSale(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSale docSale,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocSaleTab[] docSaleTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "held") docSale.Reserve = false;
            else docSale.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docSale.NumberInt;
                doc.NumberReal = docSale.DocSaleID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = docSale.DirPaymentTypeID;
                doc.Payment = docSale.Payment;
                doc.DirContractorID = docSale.DirContractorID;
                doc.DirContractorIDOrg = docSale.DirContractorIDOrg;
                doc.Discount = docSale.Discount;
                doc.DirVatValue = docSale.DirVatValue;
                doc.Base = docSale.Base;
                doc.Description = docSale.Description;
                doc.DocDate = docSale.DocDate;
                //doc.DocDisc = docSale.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docSale.DocID;
                doc.DocIDBase = docSale.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docSale" со всем полями!
                docSale.DocID = doc.DocID;

                #endregion

                #region 2. DocSale

                docSale.DocID = doc.DocID;

                db.Entry(docSale).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT

                if (entityState == EntityState.Added && (docSale.doc.NumberInt == null || docSale.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docSale.DocSaleID.ToString();
                    doc.NumberReal = docSale.DocSaleID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docSale.DocSaleID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocSaleTab

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocSaleID = new SQLiteParameter("@DocSaleID", System.Data.DbType.Int32) { Value = docSale.DocSaleID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocSaleTabs WHERE DocSaleID=@DocSaleID;", parDocSaleID);
                }

                //2.2. Проставляем ID-шник "DocSaleID" для всех позиций спецификации
                for (int i = 0; i < docSaleTabCollection.Count(); i++)
                {
                    docSaleTabCollection[i].DocSaleTabID = null;
                    docSaleTabCollection[i].DocSaleID = Convert.ToInt32(docSale.DocSaleID);
                    db.Entry(docSaleTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion
                

                if (UO_Action == "held" || docSale.Reserve)
                {
                    Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();

                    #region 1. Ищим в Возврате покупателя, если нет, то удаляем в RemPartyMinuses *** *** *** *** ***

                    //1.1. Удаляем "RemPartyMinuses"
                    var queryRemPartyMinuses = await
                        (
                            from x in db.RemPartyMinuses
                            where x.DocID == docSale.DocID
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

                        Models.Sklad.Rem.RemPartyMinus remPartyMinus = await db.RemPartyMinuses.FindAsync(iRemPartyMinusID);
                        db.RemPartyMinuses.Remove(remPartyMinus);
                        await db.SaveChangesAsync();
                    }

                    #endregion

                    #region Удаляем все записи из таблицы "RemPartyMinuses"
                    //Удаляем все записи из таблицы "RemPartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (RemPartyMinuses)

                    for (int i = 0; i < docSaleTabCollection.Count(); i++)
                    {
                        #region Проверка

                        //Переменные
                        int iRemPartyID = docSaleTabCollection[i].RemPartyID;
                        double dQuantity = docSaleTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(iRemPartyID);
                        db.Entry(remParty).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (remParty.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docSaleTabCollection[i].RemPartyID + "</td>" +                                //партия
                                "<td>" + docSaleTabCollection[i].DirNomenID + "</td>" +                                //Код товара
                                "<td>" + docSaleTabCollection[i].Quantity + "</td>" +                                  //списуемое к-во
                                "<td>" + remParty.Remnant + "</td>" +                                                  //остаток партии
                                "<td>" + (docSaleTabCollection[i].Quantity - remParty.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirWarehouseID != docSale.DirWarehouseID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSale.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSale.DirWarehouseID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docSaleTabCollection[i].RemPartyID + "</td>" +           //партия
                                "<td>" + docSaleTabCollection[i].DirNomenID + "</td>" +           //Код товара
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
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSale.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(doc.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docSaleTabCollection[i].RemPartyID + "</td>" +           //партия
                                "<td>" + docSaleTabCollection[i].DirNomenID + "</td>" +           //Код товара
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
                        remPartyMinus.RemPartyID = docSaleTabCollection[i].RemPartyID;
                        remPartyMinus.DirNomenID = docSaleTabCollection[i].DirNomenID;
                        remPartyMinus.Quantity = docSaleTabCollection[i].Quantity;
                        remPartyMinus.DirCurrencyID = docSaleTabCollection[i].DirCurrencyID;
                        remPartyMinus.DirCurrencyMultiplicity = docSaleTabCollection[i].DirCurrencyMultiplicity;
                        remPartyMinus.DirCurrencyRate = docSaleTabCollection[i].DirCurrencyRate;
                        remPartyMinus.DirVatValue = docSale.DirVatValue;
                        remPartyMinus.DirWarehouseID = docSale.DirWarehouseID;
                        remPartyMinus.DirContractorIDOrg = doc.DirContractorIDOrg;
                        remPartyMinus.DirContractorID = docSale.DirContractorID;
                        remPartyMinus.DocID = Convert.ToInt32(docSale.DocID);
                        remPartyMinus.PriceCurrency = docSaleTabCollection[i].PriceCurrency;
                        remPartyMinus.PriceVAT = docSaleTabCollection[i].PriceVAT;
                        remPartyMinus.FieldID = Convert.ToInt32(docSaleTabCollection[i].DocSaleTabID);
                        remPartyMinus.Reserve = docSale.Reserve;

                        remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        remPartyMinus.DocDate = doc.DocDate;

                        db.Entry(remPartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion
                    }
                    
                    #endregion


                    #region Касса или Банк

                    //Только, если сумма больше 0
                    /*
                    if (doc.Payment > 0)
                    {
                        //Касса
                        if (doc.DirPaymentTypeID == 1)
                        {
                            #region Касса

                            //1. По складу находим привязанную к нему Кассу
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docSale.DirWarehouseID);
                            int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                            //2. Заполняем модель "DocCashOfficeSum"
                            Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                            docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                            docCashOfficeSum.DirCashOfficeSumTypeID = 6; //Внесение в кассу на основании проведения продажи №
                            docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                            docCashOfficeSum.DocID = doc.DocID;
                            docCashOfficeSum.DocXID = docSale.DocSaleID;
                            docCashOfficeSum.DocCashOfficeSumSum = doc.Payment;
                            docCashOfficeSum.Description = "";
                            docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;

                            //3. Пишем в Кассу
                            Doc.DocCashOfficeSums.DocCashOfficeSumsController docCashOfficeSumsController = new Doc.DocCashOfficeSums.DocCashOfficeSumsController();
                            docCashOfficeSum = await Task.Run(() => docCashOfficeSumsController.mPutPostDocCashOfficeSum(db, docCashOfficeSum, EntityState.Added));

                            #endregion
                        }
                        //Банк
                        else if (doc.DirPaymentTypeID == 2)
                        {
                            #region Банк

                            //1. По складу находим привязанную к нему Кассу
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docSale.DirWarehouseID);
                            int iDirBankID = dirWarehouse.DirBankID;

                            //2. Заполняем модель "DocBankSum"
                            Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                            docBankSum.DirBankID = iDirBankID;
                            docBankSum.DirBankSumTypeID = 5; //Изъятие из кассы на основании проведения приходной накладной №
                            docBankSum.DocBankSumDate = DateTime.Now;
                            docBankSum.DocID = doc.DocID;
                            docBankSum.DocXID = docSale.DocSaleID;
                            docBankSum.DocBankSumSum = doc.Payment;
                            docBankSum.Description = "";
                            docBankSum.DirEmployeeID = field.DirEmployeeID;

                            //3. Пишем в Кассу
                            Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                            docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum(db, docBankSum, EntityState.Added));

                            #endregion
                        }
                    }
                    */

                    #endregion
                }
            }
            else if (UO_Action == "held_cancel")
            {
                #region Проверка не вернул ли товар покупатель

                //Если хоть одно поле для "DocSales.DocID" будет "RemParties.RemPartyMinusID > 0", то возвращали товар - Эксепшен.
                var query = await
                    (
                        from x in db.RemParties
                        from y in db.RemPartyMinuses
                        where y.DocID == docSale.DocID && x.RemPartyMinusID == y.RemPartyMinusID
                        select new
                        {
                            x,
                            y
                        }
                    ).ToListAsync();
                if (query.Count() > 0)
                {
                    string sData = "";
                    for (int i = 0; i < query.Count(); i++)
                    {
                        sData +=
                        "<tr>" +
                        "<td>" + query[i].x.doc.NumberReal + "</td>" +                     //Номер документа
                        "<td>" + query[i].x.doc.DocID + "</td>" +                          //Общий документа
                        "<td>" + query[i].x.doc.listObject.ListObjectNameRu + "</td>" +    //Тип документа

                        "<td>" + query[i].y.RemPartyMinusID + "</td>" +                    //списаная партия
                        "<td>" + query[i].y.DirNomenID + "</td>" +                         //Код товара
                        "<td>" + query[i].x.Quantity + "</td>" +                           //списуемое к-во
                        "</tr>";
                    }

                    throw new System.InvalidOperationException(
                        Classes.Language.Sklad.Language.msg110 +
                        sData + 
                        "</table>" +
                        Classes.Language.Sklad.Language.msg110_1
                    );
                }

                #endregion

                //Удаление записей в таблицах: RemPartyMinuses
                #region 1. RemPartyMinuses *** *** *** *** *** *** *** *** *** ***
                /*
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSale.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemPartyMinuses WHERE DocID=@DocID; ", parDocID);
                */
                #endregion

                //Обновление записей: RemPartyMinuses
                #region 1. RemPartyMinuses и RemParties *** *** *** *** *** *** *** *** *** ***

                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSale.DocID };
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE RemPartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion

                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docSale.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion

                #region 3. Касса или Банк

                //Только, если сумма больше 0
                /*
                if (doc.Payment > 0)
                {
                    //Касса
                    if (doc.DirPaymentTypeID == 1)
                    {
                        #region Касса

                        //1. По складу находим привязанную к нему Кассу
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docSale.DirWarehouseID);
                        int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                        //2. Заполняем модель "DocCashOfficeSum"
                        Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                        docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                        docCashOfficeSum.DirCashOfficeSumTypeID = 7; //Приход в кассу на основании отмены проведения приходной накладной №
                        docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                        docCashOfficeSum.DocID = doc.DocID;
                        docCashOfficeSum.DocXID = docSale.DocSaleID;
                        docCashOfficeSum.DocCashOfficeSumSum = doc.Payment;
                        docCashOfficeSum.Description = "";
                        docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;

                        //3. Пишем в Кассу
                        Doc.DocCashOfficeSums.DocCashOfficeSumsController docCashOfficeSumsController = new Doc.DocCashOfficeSums.DocCashOfficeSumsController();
                        docCashOfficeSum = await Task.Run(() => docCashOfficeSumsController.mPutPostDocCashOfficeSum(db, docCashOfficeSum, EntityState.Added));

                        #endregion
                    }
                    //Банк
                    else if (doc.DirPaymentTypeID == 2)
                    {
                        #region Банк

                        //1. По складу находим привязанную к нему Кассу
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docSale.DirWarehouseID);
                        int iDirBankID = dirWarehouse.DirBankID;

                        //2. Заполняем модель "DocBankSum"
                        Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                        docBankSum.DirBankID = iDirBankID;
                        docBankSum.DirBankSumTypeID = 6; //Приход в кассу на основании отмены проведения приходной накладной №
                        docBankSum.DocBankSumDate = DateTime.Now;
                        docBankSum.DocID = doc.DocID;
                        docBankSum.DocXID = docSale.DocSaleID;
                        docBankSum.DocBankSumSum = doc.Payment;
                        docBankSum.Description = "";
                        docBankSum.DirEmployeeID = field.DirEmployeeID;

                        //3. Пишем в Кассу
                        Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                        docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum(db, docBankSum, EntityState.Added));

                        #endregion
                    }
                }
                */
                #endregion
            }


            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docSale;
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
                "[DocSales].[DocSaleID] AS [DocSaleID], " +
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
                "[DocSales].[NumberTT] AS [NumberTT], " +
                "[DocSales].[NumberTax] AS [NumberTax], " +
                "[DocSales].[Reserve] AS [Reserve] " +

                "FROM [DocSales] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSales].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocSales].[DirWarehouseID] " +
                //Банк для Контрагента
                "LEFT JOIN [DirBanks] AS [DirBanks] ON [DirBanks].[DirBankID] = [DirContractors].[DirBankID] " +
                //Банк для Организации
                "LEFT JOIN [DirBanks] AS [DirBanksOrg] ON [DirBanks].[DirBankID] = [DirContractorOrg].[DirBankID] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion
    }
}