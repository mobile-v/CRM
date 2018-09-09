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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocReturnVendors
{
    public class DocReturnVendorsController : ApiController
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

        int ListObjectID = 34;

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
        // GET: api/DocReturnVendors
        public async Task<IHttpActionResult> GetDocReturnVendors(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnVendors"));
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
                        from docReturnVendors in db.DocReturnVendors

                        join docReturnVendorTabs1 in db.DocReturnVendorTabs on docReturnVendors.DocReturnVendorID equals docReturnVendorTabs1.DocReturnVendorID into docReturnVendorTabs2
                        from docReturnVendorTabs in docReturnVendorTabs2.DefaultIfEmpty()

                        where docReturnVendors.doc.DocDate >= _params.DateS && docReturnVendors.doc.DocDate <= _params.DatePo

                        group new { docReturnVendorTabs }
                        by new
                        {
                            DocID = docReturnVendors.DocID,
                            DocDate = docReturnVendors.doc.DocDate,
                            Base = docReturnVendors.doc.Base,
                            Held = docReturnVendors.doc.Held,
                            Discount = docReturnVendors.doc.Discount,
                            Del = docReturnVendors.doc.Del,
                            Description = docReturnVendors.doc.Description,
                            IsImport = docReturnVendors.doc.IsImport,
                            DirVatValue = docReturnVendors.doc.DirVatValue,

                            DocReturnVendorID = docReturnVendors.DocReturnVendorID,
                            DirContractorName = docReturnVendors.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docReturnVendors.doc.dirContractorOrg.DirContractorID, DirContractorNameOrg = docReturnVendors.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docReturnVendors.dirWarehouse.DirWarehouseID, DirWarehouseName = docReturnVendors.dirWarehouse.DirWarehouseName,

                            NumberInt = docReturnVendors.doc.NumberInt,
                            NumberTT = docReturnVendors.NumberTT,
                            NumberTax = docReturnVendors.NumberTax,

                            //Оплата
                            Payment = docReturnVendors.doc.Payment,

                            DocDateHeld = docReturnVendors.doc.DocDateHeld,
                            DocDatePayment = docReturnVendors.doc.DocDatePayment,
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

                            DocReturnVendorID = g.Key.DocReturnVendorID,
                            DirContractorName = g.Key.DirContractorName,
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
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * x.docReturnVendorTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * x.docReturnVendorTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * x.docReturnVendorTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * x.docReturnVendorTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum)
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
                        query = query.Where(x => x.DocReturnVendorID == iNumber32);
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
                    query = query.OrderByDescending(x => x.DocReturnVendorID);
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
                    query = query.OrderByDescending(x => x.DocReturnVendorID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocReturnVendors.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocReturnVendor = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocReturnVendors/5
        [ResponseType(typeof(DocReturnVendor))]
        public async Task<IHttpActionResult> GetDocReturnVendor(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnVendors"));
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
                        from docReturnVendors in db.DocReturnVendors
                        where docReturnVendors.DocReturnVendorID == id
                        select docReturnVendors
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docReturnVendors in db.DocReturnVendors

                        join docReturnVendorTabs1 in db.DocReturnVendorTabs on docReturnVendors.DocReturnVendorID equals docReturnVendorTabs1.DocReturnVendorID into docReturnVendorTabs2
                        from docReturnVendorTabs in docReturnVendorTabs2.DefaultIfEmpty()

                        #endregion

                        where docReturnVendors.DocReturnVendorID == id

                        #region group

                        group new { docReturnVendorTabs }
                        by new
                        {
                            DocID = docReturnVendors.DocID, //DocID = docReturnVendors.doc.DocID,
                            DocIDBase = docReturnVendors.doc.DocIDBase,
                            DocDate = docReturnVendors.doc.DocDate,
                            Base = docReturnVendors.doc.Base,
                            Held = docReturnVendors.doc.Held,
                            Discount = docReturnVendors.doc.Discount,
                            Del = docReturnVendors.doc.Del,
                            IsImport = docReturnVendors.doc.IsImport,
                            Description = docReturnVendors.doc.Description,
                            DirVatValue = docReturnVendors.doc.DirVatValue,
                            //DirPaymentTypeID = docReturnVendors.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docReturnVendors.doc.dirPaymentType.DirPaymentTypeName,

                            DocReturnVendorID = docReturnVendors.DocReturnVendorID,
                            DirContractorID = docReturnVendors.doc.DirContractorID,
                            DirContractorName = docReturnVendors.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docReturnVendors.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docReturnVendors.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docReturnVendors.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docReturnVendors.dirWarehouse.DirWarehouseName,
                            NumberInt = docReturnVendors.doc.NumberInt,
                            NumberTT = docReturnVendors.NumberTT,
                            NumberTax = docReturnVendors.NumberTax,

                            //Оплата
                            Payment = docReturnVendors.doc.Payment,

                            //Резерв
                            Reserve = docReturnVendors.Reserve
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

                            DocReturnVendorID = g.Key.DocReturnVendorID,
                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,
                            NumberTT = g.Key.NumberTT,
                            NumberTax = g.Key.NumberTax,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * x.docReturnVendorTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * x.docReturnVendorTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * (x.docReturnVendorTabs.PriceCurrency - (x.docReturnVendorTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * (x.docReturnVendorTabs.PriceCurrency - (x.docReturnVendorTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * x.docReturnVendorTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnVendorTabs.Quantity * x.docReturnVendorTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),

                            //Резерв
                            Reserve = g.Key.Reserve
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

        // PUT: api/DocReturnVendors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocReturnVendor(int id, DocReturnVendor docReturnVendor, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnVendors"));
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
                Models.Sklad.Doc.DocReturnVendorTab[] docReturnVendorTabCollection = null;
                if (!String.IsNullOrEmpty(docReturnVendor.recordsDocReturnVendorTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docReturnVendorTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocReturnVendorTab[]>(docReturnVendor.recordsDocReturnVendorTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docReturnVendor.DocReturnVendorID || docReturnVendor.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docReturnVendor.DocID" из БД, если он отличается от пришедшего от клиента "docReturnVendor.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocReturnVendors
                        where x.DocReturnVendorID == docReturnVendor.DocReturnVendorID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docReturnVendor.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docReturnVendor.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docReturnVendor.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docReturnVendor = await Task.Run(() => mPutPostDocReturnVendor(db, dbRead, UO_Action, docReturnVendor, EntityState.Modified, docReturnVendorTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docReturnVendor.DocReturnVendorID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docReturnVendor.DocID,
                    DocReturnVendorID = docReturnVendor.DocReturnVendorID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocReturnVendors
        [ResponseType(typeof(DocReturnVendor))]
        public async Task<IHttpActionResult> PostDocReturnVendor(DocReturnVendor docReturnVendor, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnVendors"));
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
                Models.Sklad.Doc.DocReturnVendorTab[] docReturnVendorTabCollection = null;
                if (!String.IsNullOrEmpty(docReturnVendor.recordsDocReturnVendorTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docReturnVendorTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocReturnVendorTab[]>(docReturnVendor.recordsDocReturnVendorTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docReturnVendor.DocID" из БД, если он отличается от пришедшего от клиента "docReturnVendor.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocReturnVendors
                        where x.DocReturnVendorID == docReturnVendor.DocReturnVendorID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docReturnVendor.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docReturnVendor.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docReturnVendor = await Task.Run(() => mPutPostDocReturnVendor(db, dbRead, UO_Action, docReturnVendor, EntityState.Added, docReturnVendorTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docReturnVendor.DocReturnVendorID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docReturnVendor.DocID,
                    DocReturnVendorID = docReturnVendor.DocReturnVendorID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocReturnVendors/5
        [ResponseType(typeof(DocReturnVendor))]
        public async Task<IHttpActionResult> DeleteDocReturnVendor(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnVendors"));
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
                            from x in dbRead.DocReturnVendors
                            where x.DocReturnVendorID == id
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
                //2. DocReturnVendorTabs
                //3. DocReturnVendors
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocReturnVendor docReturnVendor = await db.DocReturnVendors.FindAsync(id);
                if (docReturnVendor == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. RemPartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocReturnVendors
                                where x.DocReturnVendorID == id
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


                        #region 2. DocReturnVendorTabs *** *** *** *** ***

                        var queryDocReturnVendorTabs = await
                            (
                                from x in db.DocReturnVendorTabs
                                where x.DocReturnVendorID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocReturnVendorTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocReturnVendorTab docReturnVendorTab = await db.DocReturnVendorTabs.FindAsync(queryDocReturnVendorTabs[i].DocReturnVendorTabID);
                            db.DocReturnVendorTabs.Remove(docReturnVendorTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocReturnVendors *** *** *** *** ***

                        var queryDocReturnVendors = await
                            (
                                from x in db.DocReturnVendors
                                where x.DocReturnVendorID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocReturnVendors.Count(); i++)
                        {
                            Models.Sklad.Doc.DocReturnVendor docReturnVendor1 = await db.DocReturnVendors.FindAsync(queryDocReturnVendors[i].DocReturnVendorID);
                            db.DocReturnVendors.Remove(docReturnVendor1);
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

        private bool DocReturnVendorExists(int id)
        {
            return db.DocReturnVendors.Count(e => e.DocReturnVendorID == id) > 0;
        }


        internal async Task<DocReturnVendor> mPutPostDocReturnVendor(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocReturnVendor docReturnVendor,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocReturnVendorTab[] docReturnVendorTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "held") docReturnVendor.Reserve = false;
            else docReturnVendor.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docReturnVendor.NumberInt;
                doc.NumberReal = docReturnVendor.DocReturnVendorID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = docReturnVendor.DirPaymentTypeID;
                doc.Payment = docReturnVendor.Payment;
                doc.DirContractorID = docReturnVendor.DirContractorID;
                doc.DirContractorIDOrg = docReturnVendor.DirContractorIDOrg;
                doc.Discount = docReturnVendor.Discount;
                doc.DirVatValue = docReturnVendor.DirVatValue;
                doc.Base = docReturnVendor.Base;
                doc.Description = docReturnVendor.Description;
                doc.DocDate = docReturnVendor.DocDate;
                //doc.DocDisc = docReturnVendor.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docReturnVendor.DocID;
                doc.DocIDBase = docReturnVendor.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docReturnVendor" со всем полями!
                docReturnVendor.DocID = doc.DocID;

                #endregion

                #region 2. DocReturnVendor *** *** *** *** *** *** *** *** *** ***

                docReturnVendor.DocID = doc.DocID;

                db.Entry(docReturnVendor).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docReturnVendor.doc.NumberInt == null || docReturnVendor.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docReturnVendor.DocReturnVendorID.ToString();
                    doc.NumberReal = docReturnVendor.DocReturnVendorID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docReturnVendor.DocReturnVendorID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocReturnVendorTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocReturnVendorID = new SQLiteParameter("@DocReturnVendorID", System.Data.DbType.Int32) { Value = docReturnVendor.DocReturnVendorID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocReturnVendorTabs WHERE DocReturnVendorID=@DocReturnVendorID;", parDocReturnVendorID);
                }

                //2.2. Проставляем ID-шник "DocReturnVendorID" для всех позиций спецификации
                for (int i = 0; i < docReturnVendorTabCollection.Count(); i++)
                {
                    docReturnVendorTabCollection[i].DocReturnVendorTabID = null;
                    docReturnVendorTabCollection[i].DocReturnVendorID = Convert.ToInt32(docReturnVendor.DocReturnVendorID);
                    db.Entry(docReturnVendorTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion

                #region 4. DirNomenHistories *** *** *** *** *** *** *** *** ***

                /*
                //1. проверяем, если есть в Истории ничего не делаем
                for (int i = 0; i < docReturnVendorTabCollection.Count(); i++)
                {
                    Models.Sklad.Dir.DirNomenHistory dirNomenHistory = new Models.Sklad.Dir.DirNomenHistory();
                    dirNomenHistory.DirNomenHistoryID = null;


                    int DirNomenID = docReturnVendorTabCollection[i].DirNomenID;
                    var query =
                    (
                        from x in db.DirNomenHistories
                        where x.DirNomenID == DirNomenID
                        select x
                    );

                    EntityState entityState2 = EntityState.Added;
                    if (query.Count() > 0)
                    {
                        entityState2 = EntityState.Modified;
                        //dirNomenHistory.DirNomenHistoryID = query.ToList()[0].DirNomenHistoryID;
                        dirNomenHistory = db.DirNomenHistories.Find(query.ToList()[0].DirNomenHistoryID);
                    }

                    dirNomenHistory.DirCurrencyID = docReturnVendorTabCollection[i].DirCurrencyID;
                    dirNomenHistory.DirNomenID = docReturnVendorTabCollection[i].DirNomenID;
                    dirNomenHistory.PriceVAT = docReturnVendorTabCollection[i].PriceVAT;
                    dirNomenHistory.PriceRetailVAT = docReturnVendorTabCollection[i].PriceRetailVAT;
                    dirNomenHistory.MarkupRetail = docReturnVendorTabCollection[i].MarkupRetail;
                    dirNomenHistory.PriceWholesaleVAT = docReturnVendorTabCollection[i].PriceWholesaleVAT;
                    dirNomenHistory.MarkupWholesale = docReturnVendorTabCollection[i].MarkupWholesale;
                    dirNomenHistory.PriceIMVAT = docReturnVendorTabCollection[i].PriceIMVAT;
                    dirNomenHistory.MarkupIM = docReturnVendorTabCollection[i].MarkupIM;
                    dirNomenHistory.HistoryDate = Convert.ToDateTime(docReturnVendor.DocDate);

                    db.Configuration.AutoDetectChangesEnabled = false;

                    db.Entry(dirNomenHistory).State = entityState2;
                    await db.SaveChangesAsync();
                }
                */

                #endregion


                if (UO_Action == "held" || docReturnVendor.Reserve)
                {
                    Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();


                    #region Удаляем все записи из таблицы "RemPartyMinuses"
                    //Удаляем все записи из таблицы "RemPartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (RemPartyMinuses)

                    for (int i = 0; i < docReturnVendorTabCollection.Count(); i++)
                    {
                        #region Проверка

                        //Переменные
                        int iRemPartyID = docReturnVendorTabCollection[i].RemPartyID;
                        double dQuantity = docReturnVendorTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(iRemPartyID);
                        db.Entry(remParty).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (remParty.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docReturnVendorTabCollection[i].RemPartyID + "</td>" +                        //партия
                                "<td>" + docReturnVendorTabCollection[i].DirNomenID + "</td>" +                        //Код товара
                                "<td>" + docReturnVendorTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                                "<td>" + remParty.Remnant + "</td>" +                                                  //остаток партии
                                "<td>" + (docReturnVendorTabCollection[i].Quantity - remParty.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirWarehouseID != docReturnVendor.DirWarehouseID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docReturnVendor.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docReturnVendor.DirWarehouseID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docReturnVendorTabCollection[i].DirNomenID + "</td>" +                //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                     //склад документа
                                "<td>" + remParty.dirWarehouse.DirWarehouseName + "</td>" +            //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirContractorIDOrg != docReturnVendor.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docReturnVendor.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractorOrg = await db.DirContractors.FindAsync(docReturnVendor.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docReturnVendorTabCollection[i].RemPartyID + "</td>" +        //партия
                                "<td>" + docReturnVendorTabCollection[i].DirNomenID + "</td>" +        //Код товара
                                "<td>" + dirContractorOrg.DirContractorName + "</td>" +                //организация спецификации
                                "<td>" + remParty.dirContractorOrg.DirContractorName + "</td>" +       //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg106_1
                            );
                        }
                        #endregion

                        #region 4. Контрагент: контрагент документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirContractorID != docReturnVendor.DirContractorID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docReturnVendor.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docReturnVendor.DirContractorID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg107 +

                                "<tr>" +
                                "<td>" + docReturnVendorTabCollection[i].RemPartyID + "</td>" +     //партия
                                "<td>" + docReturnVendorTabCollection[i].DirNomenID + "</td>" +     //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +                //организация спецификации
                                "<td>" + remParty.dirContractor.DirContractorName + "</td>" +       //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg107_1
                            );
                        }
                        #endregion

                        #endregion


                        #region Сохранение

                        Models.Sklad.Rem.RemPartyMinus remPartyMinus = new Models.Sklad.Rem.RemPartyMinus();
                        remPartyMinus.RemPartyMinusID = null;
                        remPartyMinus.RemPartyID = docReturnVendorTabCollection[i].RemPartyID;
                        remPartyMinus.DirNomenID = docReturnVendorTabCollection[i].DirNomenID;
                        remPartyMinus.Quantity = docReturnVendorTabCollection[i].Quantity;
                        remPartyMinus.DirCurrencyID = docReturnVendorTabCollection[i].DirCurrencyID;
                        remPartyMinus.DirCurrencyMultiplicity = docReturnVendorTabCollection[i].DirCurrencyMultiplicity;
                        remPartyMinus.DirCurrencyRate = docReturnVendorTabCollection[i].DirCurrencyRate;
                        remPartyMinus.DirVatValue = docReturnVendor.DirVatValue;
                        remPartyMinus.DirWarehouseID = docReturnVendor.DirWarehouseID;
                        remPartyMinus.DirContractorIDOrg = docReturnVendor.DirContractorIDOrg;
                        remPartyMinus.DirContractorID = docReturnVendor.DirContractorID;
                        remPartyMinus.DocID = Convert.ToInt32(docReturnVendor.DocID);
                        remPartyMinus.PriceCurrency = docReturnVendorTabCollection[i].PriceCurrency;
                        remPartyMinus.PriceVAT = docReturnVendorTabCollection[i].PriceVAT;
                        remPartyMinus.FieldID = Convert.ToInt32(docReturnVendorTabCollection[i].DocReturnVendorTabID);
                        remPartyMinus.Reserve = docReturnVendor.Reserve;

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
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docReturnVendor.DirWarehouseID);
                            int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                            //2. Заполняем модель "DocCashOfficeSum"
                            Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                            docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                            docCashOfficeSum.DirCashOfficeSumTypeID = 8; //Внесение в кассу на основании проведения возврата поставщику №
                            docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                            docCashOfficeSum.DocID = doc.DocID;
                            docCashOfficeSum.DocXID = docReturnVendor.DocReturnVendorID;
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
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docReturnVendor.DirWarehouseID);
                            int iDirBankID = dirWarehouse.DirBankID;

                            //2. Заполняем модель "DocBankSum"
                            Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                            docBankSum.DirBankID = iDirBankID;
                            docBankSum.DirBankSumTypeID = 7; //Внесение в банк на основании проведения возврата поставщику №
                            docBankSum.DocBankSumDate = DateTime.Now;
                            docBankSum.DocID = doc.DocID;
                            docBankSum.DocXID = docReturnVendor.DocReturnVendorID;
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
                //Удаление записей в таблицах: RemPartyMinuses
                #region 1. RemPartyMinuses *** *** *** *** *** *** *** *** *** ***
                /*
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docReturnVendor.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemPartyMinuses WHERE DocID=@DocID; ", parDocID);
                */
                #endregion

                //Обновление записей: RemPartyMinuses
                #region 1. RemPartyMinuses и RemParties *** *** *** *** *** *** *** *** *** ***

                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docReturnVendor.DocID };
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE RemPartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion

                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docReturnVendor.DocID);
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
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docReturnVendor.DirWarehouseID);
                        int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                        //2. Заполняем модель "DocCashOfficeSum"
                        Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                        docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                        docCashOfficeSum.DirCashOfficeSumTypeID = 9; //Изъятие из кассы на основании отмены проведения возврата поставщику №
                        docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                        docCashOfficeSum.DocID = doc.DocID;
                        docCashOfficeSum.DocXID = docReturnVendor.DocReturnVendorID;
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
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docReturnVendor.DirWarehouseID);
                        int iDirBankID = dirWarehouse.DirBankID;

                        //2. Заполняем модель "DocBankSum"
                        Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                        docBankSum.DirBankID = iDirBankID;
                        docBankSum.DirBankSumTypeID = 8; //Изъятие из банка на основании отмены проведения возврата поставщику №
                        docBankSum.DocBankSumDate = DateTime.Now;
                        docBankSum.DocID = doc.DocID;
                        docBankSum.DocXID = docReturnVendor.DocReturnVendorID;
                        docBankSum.DocBankSumSum = doc.Payment;
                        docBankSum.Description = "";
                        docBankSum.DirEmployeeID = field.DirEmployeeID;

                        //3. Пишем в Банк
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


            return docReturnVendor;
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
                "[DocReturnVendors].[DocReturnVendorID] AS [DocReturnVendorID], " +
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
                "[DocReturnVendors].[NumberTT] AS [NumberTT], " +
                "[DocReturnVendors].[NumberTax] AS [NumberTax], " +
                "[DocReturnVendors].[Reserve] AS [Reserve] " +

                "FROM [DocReturnVendors] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocReturnVendors].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocReturnVendors].[DirWarehouseID] " +
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