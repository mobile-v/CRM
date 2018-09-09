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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocReturnsCustomers
{
    public class DocReturnsCustomersController : ApiController
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

        int ListObjectID = 36;

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
        // GET: api/DocReturnsCustomers
        public async Task<IHttpActionResult> GetDocReturnsCustomers(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnsCustomers"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

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
                        from docReturnsCustomers in db.DocReturnsCustomers

                        join docReturnsCustomerTabs1 in db.DocReturnsCustomerTabs on docReturnsCustomers.DocReturnsCustomerID equals docReturnsCustomerTabs1.DocReturnsCustomerID into docReturnsCustomerTabs2
                        from docReturnsCustomerTabs in docReturnsCustomerTabs2.DefaultIfEmpty()

                        where docReturnsCustomers.doc.DocDate >= _params.DateS && docReturnsCustomers.doc.DocDate <= _params.DatePo

                        group new { docReturnsCustomerTabs }
                        by new
                        {
                            DocID = docReturnsCustomers.DocID,
                            DocDate = docReturnsCustomers.doc.DocDate,
                            Base = docReturnsCustomers.doc.Base,
                            Held = docReturnsCustomers.doc.Held,
                            Discount = docReturnsCustomers.doc.Discount,
                            Del = docReturnsCustomers.doc.Del,
                            Description = docReturnsCustomers.doc.Description,
                            IsImport = docReturnsCustomers.doc.IsImport,
                            DirVatValue = docReturnsCustomers.doc.DirVatValue,

                            DocReturnsCustomerID = docReturnsCustomers.DocReturnsCustomerID,
                            DirContractorName = docReturnsCustomers.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docReturnsCustomers.doc.dirContractorOrg.DirContractorID, DirContractorNameOrg = docReturnsCustomers.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docReturnsCustomers.dirWarehouse.DirWarehouseID, DirWarehouseName = docReturnsCustomers.dirWarehouse.DirWarehouseName,

                            NumberInt = docReturnsCustomers.doc.NumberInt,
                            NumberTT = docReturnsCustomers.NumberTT,
                            NumberTax = docReturnsCustomers.NumberTax,

                            //Оплата
                            Payment = docReturnsCustomers.doc.Payment,

                            DocDateHeld = docReturnsCustomers.doc.DocDateHeld,
                            DocDatePayment = docReturnsCustomers.doc.DocDatePayment,
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

                            DocReturnsCustomerID = g.Key.DocReturnsCustomerID,
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
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * x.docReturnsCustomerTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * x.docReturnsCustomerTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * x.docReturnsCustomerTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * x.docReturnsCustomerTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
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
                        query = query.Where(x => x.DocReturnsCustomerID == iNumber32);
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
                    query = query.OrderByDescending(x => x.DocReturnsCustomerID);
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
                    query = query.OrderByDescending(x => x.DocReturnsCustomerID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocReturnsCustomers.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocReturnsCustomer = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocReturnsCustomers/5
        [ResponseType(typeof(DocReturnsCustomer))]
        public async Task<IHttpActionResult> GetDocReturnsCustomer(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnsCustomers"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

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
                        from docReturnsCustomers in db.DocReturnsCustomers
                        where docReturnsCustomers.DocReturnsCustomerID == id
                        select docReturnsCustomers
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docReturnsCustomers in db.DocReturnsCustomers

                        join docReturnsCustomerTabs1 in db.DocReturnsCustomerTabs on docReturnsCustomers.DocReturnsCustomerID equals docReturnsCustomerTabs1.DocReturnsCustomerID into docReturnsCustomerTabs2
                        from docReturnsCustomerTabs in docReturnsCustomerTabs2.DefaultIfEmpty()

                        #endregion

                        where docReturnsCustomers.DocReturnsCustomerID == id

                        #region group

                        group new { docReturnsCustomerTabs }
                        by new
                        {
                            DocID = docReturnsCustomers.DocID, //DocID = docReturnsCustomers.doc.DocID,
                            DocSaleID = docReturnsCustomers.DocSaleID, DocSaleName = docReturnsCustomers.DocSaleID, //"№ " + docReturnsCustomers.DocSaleID + " за " + docReturnsCustomers.doc.DocDate,
                            DocIDBase = docReturnsCustomers.doc.DocIDBase,
                            DocDate = docReturnsCustomers.doc.DocDate,
                            Base = docReturnsCustomers.doc.Base,
                            Held = docReturnsCustomers.doc.Held,
                            Discount = docReturnsCustomers.doc.Discount,
                            Del = docReturnsCustomers.doc.Del,
                            IsImport = docReturnsCustomers.doc.IsImport,
                            Description = docReturnsCustomers.doc.Description,
                            DirVatValue = docReturnsCustomers.doc.DirVatValue,
                            //DirPaymentTypeID = docReturnsCustomers.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docReturnsCustomers.doc.dirPaymentType.DirPaymentTypeName,

                            DocReturnsCustomerID = docReturnsCustomers.DocReturnsCustomerID,
                            DirContractorID = docReturnsCustomers.doc.DirContractorID,
                            DirContractorName = docReturnsCustomers.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docReturnsCustomers.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docReturnsCustomers.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseID = docReturnsCustomers.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docReturnsCustomers.dirWarehouse.DirWarehouseName,
                            NumberInt = docReturnsCustomers.doc.NumberInt,
                            NumberTT = docReturnsCustomers.NumberTT,
                            NumberTax = docReturnsCustomers.NumberTax,

                            //Оплата
                            Payment = docReturnsCustomers.doc.Payment,
                        }
                        into g

                        #endregion

                        #region select

                        select new
                        {
                            DocID = g.Key.DocID,
                            DocSaleID = g.Key.DocSaleID, DocSaleName = g.Key.DocSaleName,
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

                            DocReturnsCustomerID = g.Key.DocReturnsCustomerID,
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
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * x.docReturnsCustomerTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * x.docReturnsCustomerTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * (x.docReturnsCustomerTabs.PriceCurrency - (x.docReturnsCustomerTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * (x.docReturnsCustomerTabs.PriceCurrency - (x.docReturnsCustomerTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * x.docReturnsCustomerTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docReturnsCustomerTabs.Quantity * x.docReturnsCustomerTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocReturnsCustomers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocReturnsCustomer(int id, DocReturnsCustomer docReturnsCustomer, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnsCustomers"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Скидка: разрешена или нет
                if (docReturnsCustomer.Discount > 0)
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
                Models.Sklad.Doc.DocReturnsCustomerTab[] docReturnsCustomerTabCollection = null;
                if (!String.IsNullOrEmpty(docReturnsCustomer.recordsDocReturnsCustomerTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docReturnsCustomerTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocReturnsCustomerTab[]>(docReturnsCustomer.recordsDocReturnsCustomerTab);
                }

                #endregion

                #region Проверки

                //1. 
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docReturnsCustomer.DocReturnsCustomerID || docReturnsCustomer.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docReturnsCustomer.DocID" из БД, если он отличается от пришедшего от клиента "docReturnsCustomer.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocReturnsCustomers
                        where x.DocReturnsCustomerID == docReturnsCustomer.DocReturnsCustomerID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docReturnsCustomer.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docReturnsCustomer.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docReturnsCustomer.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docReturnsCustomer = await Task.Run(() => mPutPostDocReturnsCustomer(db, dbRead, UO_Action, docReturnsCustomer, EntityState.Modified, docReturnsCustomerTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docReturnsCustomer.DocReturnsCustomerID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docReturnsCustomer.DocID,
                    DocReturnsCustomerID = docReturnsCustomer.DocReturnsCustomerID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocReturnsCustomers
        [ResponseType(typeof(DocReturnsCustomer))]
        public async Task<IHttpActionResult> PostDocReturnsCustomer(DocReturnsCustomer docReturnsCustomer, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnsCustomers"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Скидка: разрешена или нет
                if (docReturnsCustomer.Discount > 0)
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
                Models.Sklad.Doc.DocReturnsCustomerTab[] docReturnsCustomerTabCollection = null;
                if (!String.IsNullOrEmpty(docReturnsCustomer.recordsDocReturnsCustomerTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docReturnsCustomerTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocReturnsCustomerTab[]>(docReturnsCustomer.recordsDocReturnsCustomerTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Это нужно только для UPDATE
                /*
                try
                {
                    //Получаем "docReturnsCustomer.DocID" из БД, если он отличается от пришедшего от клиента "docReturnsCustomer.DocID" выдаём ошибку
                    //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                    var query = await Task.Run(() =>
                        (
                            from x in dbRead.DocReturnsCustomers
                            where x.DocReturnsCustomerID == docReturnsCustomer.DocReturnsCustomerID
                            select x
                        ).ToListAsync());

                    if (query.Count() > 0)
                        if (query[0].DocID != docReturnsCustomer.DocID)
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                    //dbRead.Database.Connection.Close();
                }
                catch (Exception ex)
                {
                    return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                }
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docReturnsCustomer.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docReturnsCustomer = await Task.Run(() => mPutPostDocReturnsCustomer(db, dbRead, UO_Action, docReturnsCustomer, EntityState.Added, docReturnsCustomerTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docReturnsCustomer.DocReturnsCustomerID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docReturnsCustomer.DocID,
                    DocReturnsCustomerID = docReturnsCustomer.DocReturnsCustomerID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocReturnsCustomers/5
        [ResponseType(typeof(DocReturnsCustomer))]
        public async Task<IHttpActionResult> DeleteDocReturnsCustomer(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnsCustomers"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Проверки

                try
                {
                    //Документ проведён!<BR>Перед удалением, нужно отменить проводку!
                    var queryHeld = await Task.Run(() =>
                        (
                            from x in dbRead.DocReturnsCustomers
                            where x.DocReturnsCustomerID == id
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
                //2. DocReturnsCustomerTabs
                //3. DocReturnsCustomers
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocReturnsCustomer docReturnsCustomer = await db.DocReturnsCustomers.FindAsync(id);
                if (docReturnsCustomer == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. RemParties *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocReturnsCustomers
                                where x.DocReturnsCustomerID == id
                                select x
                            ).ToListAsync();
                        if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

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


                        #region 2. DocReturnsCustomerTabs *** *** *** *** ***

                        var queryDocReturnsCustomerTabs = await
                            (
                                from x in db.DocReturnsCustomerTabs
                                where x.DocReturnsCustomerID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocReturnsCustomerTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocReturnsCustomerTab docReturnsCustomerTab = await db.DocReturnsCustomerTabs.FindAsync(queryDocReturnsCustomerTabs[i].DocReturnsCustomerTabID);
                            db.DocReturnsCustomerTabs.Remove(docReturnsCustomerTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocReturnsCustomers *** *** *** *** ***

                        var queryDocReturnsCustomers = await
                            (
                                from x in db.DocReturnsCustomers
                                where x.DocReturnsCustomerID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocReturnsCustomers.Count(); i++)
                        {
                            Models.Sklad.Doc.DocReturnsCustomer docReturnsCustomer1 = await db.DocReturnsCustomers.FindAsync(queryDocReturnsCustomers[i].DocReturnsCustomerID);
                            db.DocReturnsCustomers.Remove(docReturnsCustomer1);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 4. DocReturnsCustomers *** *** *** *** ***

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

        private bool DocReturnsCustomerExists(int id)
        {
            return db.DocReturnsCustomers.Count(e => e.DocReturnsCustomerID == id) > 0;
        }


        internal async Task<DocReturnsCustomer> mPutPostDocReturnsCustomer(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocReturnsCustomer docReturnsCustomer,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocReturnsCustomerTab[] docReturnsCustomerTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Получаем 

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docReturnsCustomer.NumberInt;
                doc.NumberReal = docReturnsCustomer.DocReturnsCustomerID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = docReturnsCustomer.DirPaymentTypeID;
                doc.Payment = docReturnsCustomer.Payment;
                doc.DirContractorID = docReturnsCustomer.DirContractorID;
                doc.DirContractorIDOrg = docReturnsCustomer.DirContractorIDOrg;
                doc.Discount = docReturnsCustomer.Discount;
                doc.DirVatValue = docReturnsCustomer.DirVatValue;
                doc.Base = docReturnsCustomer.Base;
                doc.Description = docReturnsCustomer.Description;
                doc.DocDate = docReturnsCustomer.DocDate;
                //doc.DocDisc = docReturnsCustomer.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docReturnsCustomer.DocID;
                doc.DocIDBase = docReturnsCustomer.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docReturnsCustomer" со всем полями!
                docReturnsCustomer.DocID = doc.DocID;

                #endregion

                #region 2. DocReturnsCustomer *** *** *** *** *** *** *** *** *** ***

                docReturnsCustomer.DocID = doc.DocID;

                db.Entry(docReturnsCustomer).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docReturnsCustomer.doc.NumberInt == null || docReturnsCustomer.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docReturnsCustomer.DocReturnsCustomerID.ToString();
                    doc.NumberReal = docReturnsCustomer.DocReturnsCustomerID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docReturnsCustomer.DocReturnsCustomerID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocReturnsCustomerTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocReturnsCustomerID = new SQLiteParameter("@DocReturnsCustomerID", System.Data.DbType.Int32) { Value = docReturnsCustomer.DocReturnsCustomerID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocReturnsCustomerTabs WHERE DocReturnsCustomerID=@DocReturnsCustomerID;", parDocReturnsCustomerID);
                }

                //2.2. Проставляем ID-шник "DocReturnsCustomerID" для всех позиций спецификации
                for (int i = 0; i < docReturnsCustomerTabCollection.Count(); i++)
                {
                    docReturnsCustomerTabCollection[i].DocReturnsCustomerTabID = null;
                    docReturnsCustomerTabCollection[i].DocReturnsCustomerID = Convert.ToInt32(docReturnsCustomer.DocReturnsCustomerID);
                    db.Entry(docReturnsCustomerTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion

                #region 4. DirNomenHistories *** *** *** *** *** *** *** *** ***

                /*
                //1. проверяем, если есть в Истории ничего не делаем
                for (int i = 0; i < docReturnsCustomerTabCollection.Count(); i++)
                {
                    Models.Sklad.Dir.DirNomenHistory dirNomenHistory = new Models.Sklad.Dir.DirNomenHistory();
                    dirNomenHistory.DirNomenHistoryID = null;


                    int DirNomenID = docReturnsCustomerTabCollection[i].DirNomenID;
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


                    dirNomenHistory.DirCurrencyID = docReturnsCustomerTabCollection[i].DirCurrencyID;
                    dirNomenHistory.DirNomenID = docReturnsCustomerTabCollection[i].DirNomenID;
                    dirNomenHistory.PriceVAT = docReturnsCustomerTabCollection[i].PriceVAT;

                    dirNomenHistory.HistoryDate = Convert.ToDateTime(docReturnsCustomer.DocDate);

                    db.Configuration.AutoDetectChangesEnabled = false;

                    db.Entry(dirNomenHistory).State = entityState2;
                    await db.SaveChangesAsync();
                }
                */

                #endregion


                //Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();
                if (UO_Action == "held" || docReturnsCustomer.Reserve)
                {
                    //Алгоритм:
                    //По ПартияМинус находим Партию
                    //С неё берём Характеристики и цены
                    //Создаём новую партию

                    //!!! Важно !!!
                    //Ещё должна быть проверка: например, покупатель уже возвращал товар с этой ПартияМинус
                    //То есть в Партии надо иметь Линк на Партию Минус.


                    #region RemParty - Партии

                    Models.Sklad.Rem.RemParty[] remPartyCollection = new Models.Sklad.Rem.RemParty[docReturnsCustomerTabCollection.Count()];
                    for (int i = 0; i < docReturnsCustomerTabCollection.Count(); i++)
                    {
                        Models.Sklad.Rem.RemPartyMinus remPartyMinus = await db.RemPartyMinuses.FindAsync(docReturnsCustomerTabCollection[i].RemPartyMinusID);


                        #region Проверка: 1 и 2

                        //1. Что бы клиент не вернул товара больше, чем купил
                        if (docReturnsCustomerTabCollection[i].Quantity > remPartyMinus.Quantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg108 +
                                //docReturnsCustomerTabCollection[i].DirNomenID + " (" + docReturnsCustomerTabCollection[i].RemPartyMinusID + ") - " + iRemPartyMinusID
                                "<tr>" +
                                "<td>" + docReturnsCustomerTabCollection[i].RemPartyMinusID + "</td>" +     //списаная партия
                                "<td>" + docReturnsCustomerTabCollection[i].DirNomenID + "</td>" +     //код товара
                                "<td>" + remPartyMinus.Quantity + "</td>" +                //к-во реальное
                                "</tr>" +
                                "</table>"
                                );
                        }
                        //2. Что бы клиент 2-ды не вернул один и тот же товар с продажи
                        int iRemPartyMinusID = docReturnsCustomerTabCollection[i].RemPartyMinusID;
                        double? dSumQuantity = db.RemParties.Where(x => x.RemPartyMinusID == iRemPartyMinusID).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();
                        if (dSumQuantity >= remPartyMinus.Quantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg109 +
                                "<tr>" +
                                "<td>" + docReturnsCustomerTabCollection[i].RemPartyMinusID + "</td>" +     //списаная партия
                                "<td>" + docReturnsCustomerTabCollection[i].DirNomenID + "</td>" +     //код товара
                                "</tr>" +
                                "</table>"
                                );
                        }

                        #endregion
                        

                        #region Партия

                        /*
                        remParty.RemPartyID = null;
                        remParty.DirNomenID = docReturnsCustomerTabCollection[i].DirNomenID;
                        remParty.Quantity = docReturnsCustomerTabCollection[i].Quantity;
                        remParty.Remnant = docReturnsCustomerTabCollection[i].Quantity;
                        remParty.DirCurrencyID = docReturnsCustomerTabCollection[i].DirCurrencyID;
                        //remParty.DirCurrencyMultiplicity = docReturnsCustomerTabCollection[i].DirCurrencyMultiplicity;
                        //remParty.DirCurrencyRate = docReturnsCustomerTabCollection[i].DirCurrencyRate;
                        remParty.DirVatValue = docReturnsCustomer.DirVatValue;
                        remParty.DirWarehouseID = docReturnsCustomer.DirWarehouseID;
                        remParty.DirWarehouseIDDebit = docReturnsCustomer.DirWarehouseID;
                        remParty.DirWarehouseIDPurch = docReturnsCustomer.DirWarehouseID;
                        remParty.DirContractorIDOrg = docReturnsCustomer.DirContractorIDOrg;
                        remParty.DirContractorID = remPartyMinus.remParty.DirContractorID;

                        remParty.DirCharColourID = remPartyMinus.remParty.DirCharColourID; //docReturnsCustomerTabCollection[i].DirCharColourID;
                        remParty.DirCharMaterialID = remPartyMinus.remParty.DirCharMaterialID;
                        remParty.DirCharNameID = remPartyMinus.remParty.DirCharNameID;
                        remParty.DirCharSeasonID = remPartyMinus.remParty.DirCharSeasonID;
                        remParty.DirCharSexID = remPartyMinus.remParty.DirCharSexID;
                        remParty.DirCharSizeID = remPartyMinus.remParty.DirCharSizeID;
                        remParty.DirCharStyleID = remPartyMinus.remParty.DirCharStyleID;
                        remParty.DirCharTextureID = remPartyMinus.remParty.DirCharTextureID;

                        remParty.SerialNumber = remPartyMinus.remParty.SerialNumber;
                        remParty.Barcode = remPartyMinus.remParty.Barcode;

                        remParty.DocID = Convert.ToInt32(docReturnsCustomer.DocID);
                        remParty.PriceCurrency = docReturnsCustomerTabCollection[i].PriceCurrency;
                        remParty.PriceVAT = docReturnsCustomerTabCollection[i].PriceVAT;
                        remParty.FieldID = Convert.ToInt32(docReturnsCustomerTabCollection[i].DocReturnsCustomerTabID);

                        remParty.PriceRetailVAT = remPartyMinus.remParty.PriceRetailVAT;
                        remParty.PriceRetailCurrency = remPartyMinus.remParty.PriceRetailCurrency;
                        remParty.PriceWholesaleVAT = remPartyMinus.remParty.PriceWholesaleVAT;
                        remParty.PriceWholesaleCurrency = remPartyMinus.remParty.PriceWholesaleCurrency;
                        remParty.PriceIMVAT = remPartyMinus.remParty.PriceIMVAT;
                        remParty.PriceIMCurrency = remPartyMinus.remParty.PriceIMCurrency;

                        remParty.RemPartyMinusID = docReturnsCustomerTabCollection[i].RemPartyMinusID;
                        */

                        Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();

                        remParty.RemPartyID = null;
                        remParty.DirNomenID = remPartyMinus.remParty.DirNomenID;
                        remParty.Quantity = docReturnsCustomerTabCollection[i].Quantity;
                        remParty.Remnant = docReturnsCustomerTabCollection[i].Quantity;
                        remParty.DirCurrencyID = remPartyMinus.remParty.DirCurrencyID;
                        //remParty.DirCurrencyMultiplicity = docReturnsCustomerTabCollection[i].DirCurrencyMultiplicity;
                        //remParty.DirCurrencyRate = docReturnsCustomerTabCollection[i].DirCurrencyRate;
                        remParty.DirVatValue = remPartyMinus.remParty.DirVatValue;
                        remParty.DirWarehouseID = remPartyMinus.remParty.DirWarehouseID;
                        remParty.DirWarehouseIDDebit = remPartyMinus.remParty.DirWarehouseIDDebit;
                        remParty.DirWarehouseIDPurch = remPartyMinus.remParty.DirWarehouseIDPurch;
                        remParty.DirContractorIDOrg = remPartyMinus.remParty.DirContractorIDOrg;
                        remParty.DirContractorID = remPartyMinus.remParty.DirContractorID;

                        //Дата Приёмки товара
                        remParty.DocDatePurches = remPartyMinus.remParty.DocDatePurches;

                        remParty.DirCharColourID = remPartyMinus.remParty.DirCharColourID; //docReturnsCustomerTabCollection[i].DirCharColourID;
                        remParty.DirCharMaterialID = remPartyMinus.remParty.DirCharMaterialID;
                        remParty.DirCharNameID = remPartyMinus.remParty.DirCharNameID;
                        remParty.DirCharSeasonID = remPartyMinus.remParty.DirCharSeasonID;
                        remParty.DirCharSexID = remPartyMinus.remParty.DirCharSexID;
                        remParty.DirCharSizeID = remPartyMinus.remParty.DirCharSizeID;
                        remParty.DirCharStyleID = remPartyMinus.remParty.DirCharStyleID;
                        remParty.DirCharTextureID = remPartyMinus.remParty.DirCharTextureID;

                        remParty.SerialNumber = remPartyMinus.remParty.SerialNumber;
                        remParty.Barcode = remPartyMinus.remParty.Barcode;

                        remParty.DocID = Convert.ToInt32(docReturnsCustomer.DocID);
                        remParty.PriceCurrency = remPartyMinus.remParty.PriceCurrency; //(!!!)
                        remParty.PriceVAT = remPartyMinus.remParty.PriceVAT;
                        remParty.FieldID = Convert.ToInt32(docReturnsCustomerTabCollection[i].DocReturnsCustomerTabID);

                        remParty.PriceRetailVAT = remPartyMinus.remParty.PriceRetailVAT;
                        remParty.PriceRetailCurrency = remPartyMinus.remParty.PriceRetailCurrency;
                        remParty.PriceWholesaleVAT = remPartyMinus.remParty.PriceWholesaleVAT;
                        remParty.PriceWholesaleCurrency = remPartyMinus.remParty.PriceWholesaleCurrency;
                        remParty.PriceIMVAT = remPartyMinus.remParty.PriceIMVAT;
                        remParty.PriceIMCurrency = remPartyMinus.remParty.PriceIMCurrency;

                        remParty.RemPartyMinusID = remPartyMinus.RemPartyMinusID; //docReturnsCustomerTabCollection[i].RemPartyMinusID;

                        //DirNomenMinimumBalance
                        remParty.DirNomenMinimumBalance = sysSetting.DirNomenMinimumBalance;

                        remParty.DirEmployeeID = doc.DirEmployeeID;
                        remParty.DocDate = doc.DocDate;

                        remPartyCollection[i] = remParty;

                        #endregion
                    }

                    Controllers.Sklad.Rem.RemPartiesController remPartys = new Rem.RemPartiesController();
                    await Task.Run(() => remPartys.Save(db, remPartyCollection)); //remPartys.Save(db, remPartyCollection);

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
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docReturnsCustomer.DirWarehouseID);
                            int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                            //2. Заполняем модель "DocCashOfficeSum"
                            Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                            docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                            docCashOfficeSum.DirCashOfficeSumTypeID = 10; //Изъятие из кассы на основании проведения возврата от покупателя №
                            docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                            docCashOfficeSum.DocID = doc.DocID;
                            docCashOfficeSum.DocXID = docReturnsCustomer.DocReturnsCustomerID;
                            docCashOfficeSum.DocCashOfficeSumSum = doc.Payment; //Минусуем, т.к. берём деньги из кассы за товар
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
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docReturnsCustomer.DirWarehouseID);
                            int iDirBankID = dirWarehouse.DirBankID;

                            //2. Заполняем модель "DocBankSum"
                            Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                            docBankSum.DirBankID = iDirBankID;
                            docBankSum.DirBankSumTypeID = 9; //Изъятие из банка на основании проведения возврата от покупателя №
                            docBankSum.DocBankSumDate = DateTime.Now;
                            docBankSum.DocID = doc.DocID;
                            docBankSum.DocXID = docReturnsCustomer.DocReturnsCustomerID;
                            docBankSum.DocBankSumSum = doc.Payment; //Минусуем, т.к. берём деньги из кассы за товар
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
                #region 1. Проверка: Было ли списание с партий *** *** *** *** *** *** *** *** *** ***


                //int DocReturnsCustomerID = Convert.ToInt32(docReturnsCustomer.DocReturnsCustomerID);


                //Алгоритм №1
                //SELECT DocID
                //FROM RemPartyMinuses 
                //WHERE RemPartyID in (SELECT RemPartyID FROM RemParties WHERE DocID=@DocID)



                #region Алгоритм №1 (OLD)

                /*
                //Получаем DocReturnsCustomer из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocReturnsCustomer _docReturnsCustomer = db.DocReturnsCustomers.Find(DocReturnsCustomerID);
                int? iDocReturnsCustomer_DocID = _docReturnsCustomer.DocID;


                var queryRemPartyMinuses =
                    (
                        from remPartyMinuses in db.RemPartyMinuses

                        join remParties1 in db.RemParties on remPartyMinuses.RemPartyID equals remParties1.RemPartyID into remParties2
                        from remParties in remParties2.Where(x => x.DocID == iDocReturnsCustomer_DocID) //.DefaultIfEmpty()

                        select new
                        {
                            DocID = remPartyMinuses.DocID,
                            ListObjectNameRu = remPartyMinuses.doc.listObject.ListObjectNameRu
                        }
                    ).Distinct().ToList(); // - убрать повторяющиеся

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
                */

                #endregion



                //Этот 2-й алгоритм глючит! Если поле "Remnant==Quantity", а списания были (глюкануло что-то), то выдаст сообщение, что "Связи между таблицами нарушены!"

                //Алгоритм №2
                //Пробегаемся по всем "RemParties.Remnant"
                //и есть оно отличается от "RemParties.Quantity"
                //то был списан товар

                /*

                //Получаем DocReturnsCustomer из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocReturnsCustomer _docReturnsCustomer = db.DocReturnsCustomers.Find(DocReturnsCustomerID);
                int? iDocReturnsCustomer_DocID = _docReturnsCustomer.DocID;

                //Есть ли списанный товар с данного прихода
                var queryRemParties = await Task.Run(() =>
                    (
                        from x in db.RemParties
                        where x.DocID == iDocReturnsCustomer_DocID && x.Quantity != x.Remnant
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
                            from remParties in remParties2.Where(x => x.DocID == iDocReturnsCustomer_DocID) //.DefaultIfEmpty()

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

                int DocReturnsCustomerID = Convert.ToInt32(docReturnsCustomer.DocReturnsCustomerID);

                Models.Sklad.Doc.DocReturnsCustomer _docReturnsCustomer = db.DocReturnsCustomers.Find(DocReturnsCustomerID);
                int? iDocReturnsCustomer_DocID = _docReturnsCustomer.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocReturnsCustomer_DocID));

                #endregion


                //Проверка и Удаление записей в таблицах: RemParties
                #region 1. RemParties - удаление *** *** *** *** *** *** *** *** *** ***

                //Проверяем если ли расходы (проведённый или НЕ проведенные)


                //Удаляем записи в таблице "RemParties"
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docReturnsCustomer.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID; ", parDocID);

                #endregion

                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docReturnsCustomer.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

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
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docReturnsCustomer.DirWarehouseID);
                        int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                        //2. Заполняем модель "DocCashOfficeSum"
                        Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                        docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                        docCashOfficeSum.DirCashOfficeSumTypeID = 11; //Внесение в кассу на основании отмены проведения возврата от покупателя №
                        docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                        docCashOfficeSum.DocID = doc.DocID;
                        docCashOfficeSum.DocXID = docReturnsCustomer.DocReturnsCustomerID;
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
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docReturnsCustomer.DirWarehouseID);
                        int iDirBankID = dirWarehouse.DirBankID;

                        //2. Заполняем модель "DocBankSum"
                        Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                        docBankSum.DirBankID = iDirBankID;
                        docBankSum.DirBankSumTypeID = 10; //Внесение в банка на основании отмены проведения возврата от покупателя №
                        docBankSum.DocBankSumDate = DateTime.Now;
                        docBankSum.DocID = doc.DocID;
                        docBankSum.DocXID = docReturnsCustomer.DocReturnsCustomerID;
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


            return docReturnsCustomer;
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
                "[DocReturnsCustomers].[DocReturnsCustomerID] AS [DocReturnsCustomerID], " +
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
                "[DocReturnsCustomers].[NumberTT] AS [NumberTT], " +
                "[DocReturnsCustomers].[NumberTax] AS [NumberTax] " +

                "FROM [DocReturnsCustomers] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocReturnsCustomers].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocReturnsCustomers].[DirWarehouseID] " +
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