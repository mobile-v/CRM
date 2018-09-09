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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocDomesticExpenses
{
    public class DocDomesticExpensesController : ApiController
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
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();

        int ListObjectID = 70;

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
        // GET: api/DocDomesticExpenses
        public async Task<IHttpActionResult> GetDocDomesticExpenses(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocDomesticExpenses"));
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
                        from docDomesticExpenses in db.DocDomesticExpenses

                        join docDomesticExpenseTabs1 in db.DocDomesticExpenseTabs on docDomesticExpenses.DocDomesticExpenseID equals docDomesticExpenseTabs1.DocDomesticExpenseID into docDomesticExpenseTabs2
                        from docDomesticExpenseTabs in docDomesticExpenseTabs2.DefaultIfEmpty()

                        where docDomesticExpenses.doc.DocDate >= _params.DateS && docDomesticExpenses.doc.DocDate <= _params.DatePo

                        group new { docDomesticExpenseTabs }
                        by new
                        {
                            DocID = docDomesticExpenses.DocID,
                            DocDate = docDomesticExpenses.doc.DocDate,
                            Base = docDomesticExpenses.doc.Base,
                            Held = docDomesticExpenses.doc.Held,
                            Discount = docDomesticExpenses.doc.Discount,
                            Del = docDomesticExpenses.doc.Del,
                            Description = docDomesticExpenses.doc.Description,
                            IsImport = docDomesticExpenses.doc.IsImport,
                            DirVatValue = docDomesticExpenses.doc.DirVatValue,

                            DocDomesticExpenseID = docDomesticExpenses.DocDomesticExpenseID,
                            DirContractorID = docDomesticExpenses.doc.dirContractor.DirContractorID,
                            DirContractorName = docDomesticExpenses.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docDomesticExpenses.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docDomesticExpenses.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docDomesticExpenses.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docDomesticExpenses.dirWarehouse.DirWarehouseName,

                            NumberInt = docDomesticExpenses.doc.NumberInt,

                            //Оплата
                            //Payment = docDomesticExpenses.doc.Payment,

                            DocDateHeld = docDomesticExpenses.doc.DocDateHeld,
                            DocDatePayment = docDomesticExpenses.doc.DocDatePayment,
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

                            DocDomesticExpenseID = g.Key.DocDomesticExpenseID,

                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            DocDateHeld = g.Key.DocDateHeld,
                            DocDatePayment = g.Key.DocDatePayment,

                            //Сумма с НДС
                            /*
                            SumOfVATCurrency =
                            g.Sum(x => x.docDomesticExpenseTabs.Quantity * x.docDomesticExpenseTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docDomesticExpenseTabs.Quantity * x.docDomesticExpenseTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),
                            */

                            //Оплата
                            /*
                            Payment = g.Key.Payment,

                            HavePay = 
                            g.Sum(x => x.docDomesticExpenseTabs.Quantity * x.docDomesticExpenseTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docDomesticExpenseTabs.Quantity * x.docDomesticExpenseTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment, sysSetting.FractionalPartInSum)
                            */
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
                        query = query.Where(x => x.DocDomesticExpenseID == iNumber32);
                    }
                    //Если Дата
                    else if (bDateTime)
                    {
                        query = query.Where(x => x.DocDate == dDateTime);
                    }
                    //Иначе, только текстовые поля
                    else
                    {
                        query = query.Where(x => x.DirContractorName.Contains(_params.parSearch) || x.DirWarehouseName.Contains(_params.parSearch) || x.NumberInt.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                //query = query.OrderByDescending(x => x.DocDate).Skip(_params.Skip).Take(_params.limit);

                if (sysSetting.DocsSortField == 1)
                {
                    query = query.OrderByDescending(x => x.DocDomesticExpenseID);
                }
                else if (sysSetting.DocsSortField == 2)
                {
                    query = query.OrderByDescending(x => x.DocDate);
                }
                /*else if (sysSetting.DocsSortField == 3)
                {
                    query = query.OrderByDescending(x => x.DocDateHeld);
                }
                else if (sysSetting.DocsSortField == 4)
                {
                    query = query.OrderByDescending(x => x.DocDatePayment);
                }*/
                else
                {
                    query = query.OrderByDescending(x => x.DocDomesticExpenseID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocDomesticExpenses.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocDomesticExpense = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocDomesticExpenses/5
        [ResponseType(typeof(DocDomesticExpense))]
        public async Task<IHttpActionResult> GetDocDomesticExpense(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocDomesticExpenses"));
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
                        from docDomesticExpenses in db.DocDomesticExpenses
                        where docDomesticExpenses.DocDomesticExpenseID == id
                        select docDomesticExpenses
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docDomesticExpenses in db.DocDomesticExpenses

                        join docDomesticExpenseTabs1 in db.DocDomesticExpenseTabs on docDomesticExpenses.DocDomesticExpenseID equals docDomesticExpenseTabs1.DocDomesticExpenseID into docDomesticExpenseTabs2
                        from docDomesticExpenseTabs in docDomesticExpenseTabs2.DefaultIfEmpty()

                        #endregion

                        where docDomesticExpenses.DocDomesticExpenseID == id

                        #region group

                        group new { docDomesticExpenseTabs }
                        by new
                        {
                            DocID = docDomesticExpenses.DocID, //DocID = docDomesticExpenses.doc.DocID,
                            DocIDBase = docDomesticExpenses.doc.DocIDBase,
                            DocDate = docDomesticExpenses.doc.DocDate,
                            DirPaymentTypeID = docDomesticExpenses.doc.DirPaymentTypeID,
                            Base = docDomesticExpenses.doc.Base,
                            Held = docDomesticExpenses.doc.Held,
                            Discount = docDomesticExpenses.doc.Discount,
                            Del = docDomesticExpenses.doc.Del,
                            IsImport = docDomesticExpenses.doc.IsImport,
                            Description = docDomesticExpenses.doc.Description,
                            DirVatValue = docDomesticExpenses.doc.DirVatValue,
                            //DirPaymentTypeID = docDomesticExpenses.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docDomesticExpenses.doc.dirPaymentType.DirPaymentTypeName,

                            DocDomesticExpenseID = docDomesticExpenses.DocDomesticExpenseID,
                            DirContractorID = docDomesticExpenses.doc.DirContractorID,
                            DirContractorName = docDomesticExpenses.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docDomesticExpenses.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docDomesticExpenses.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docDomesticExpenses.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docDomesticExpenses.dirWarehouse.DirWarehouseName,
                            NumberInt = docDomesticExpenses.doc.NumberInt,

                            //Оплата
                            //Payment = docDomesticExpenses.doc.Payment,
                            
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

                            DocDomesticExpenseID = g.Key.DocDomesticExpenseID,

                            DirContractorID = g.Key.DirContractorID == 1 ? 0
                            :
                            g.Key.DirContractorID,
                            //DirContractorName = g.Key.DirContractorName,

                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,

                            //Payment = g.Key.Payment,
                            //Reserve = g.Key.Reserve,
                            //OnCredit = g.Key.OnCredit,

                            //Сумма с НДС
                            /*
                            SumDocServicePurch1Tabs =
                            g.Sum(x => x.docDomesticExpenseTabs.Quantity * x.docDomesticExpenseTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docDomesticExpenseTabs.Quantity * x.docDomesticExpenseTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocDomesticExpenses/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocDomesticExpense(int id, DocDomesticExpense docDomesticExpense, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocDomesticExpenses"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Скидка: разрешена или нет
                if (docDomesticExpense.Discount > 0)
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
                Models.Sklad.Doc.DocDomesticExpenseTab[] docDomesticExpenseTabCollection = null;
                if (!String.IsNullOrEmpty(docDomesticExpense.recordsDocDomesticExpenseTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docDomesticExpenseTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocDomesticExpenseTab[]>(docDomesticExpense.recordsDocDomesticExpenseTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docDomesticExpense.DocDomesticExpenseID || docDomesticExpense.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docDomesticExpense.DocID" из БД, если он отличается от пришедшего от клиента "docDomesticExpense.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocDomesticExpenses
                        where x.DocDomesticExpenseID == docDomesticExpense.DocDomesticExpenseID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docDomesticExpense.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docDomesticExpense.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docDomesticExpense.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docDomesticExpense = await Task.Run(() => mPutPostDocDomesticExpense(db, dbRead, UO_Action, docDomesticExpense, EntityState.Modified, docDomesticExpenseTabCollection, field)); //sysSetting
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

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = docDomesticExpense.DocDomesticExpenseID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docDomesticExpense.DocID,
                    DocDomesticExpenseID = docDomesticExpense.DocDomesticExpenseID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocDomesticExpenses
        [ResponseType(typeof(DocDomesticExpense))]
        public async Task<IHttpActionResult> PostDocDomesticExpense(DocDomesticExpense docDomesticExpense, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocDomesticExpenses"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Скидка: разрешена или нет
                if (docDomesticExpense.Discount > 0)
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


                //Проверяме пароль
                string DirEmployeePswd = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeePswd", true) == 0).Value;
                Classes.Account.EncodeDecode encode = new Classes.Account.EncodeDecode();
                if (DirEmployeePswd != encode.UnionDecode(authCookie["CookieP"])) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_6));


                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocDomesticExpenseTab[] docDomesticExpenseTabCollection = null;
                if (!String.IsNullOrEmpty(docDomesticExpense.recordsDocDomesticExpenseTab) && UO_Action != "held_cancel")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docDomesticExpenseTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocDomesticExpenseTab[]>(docDomesticExpense.recordsDocDomesticExpenseTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docDomesticExpense.DocID" из БД, если он отличается от пришедшего от клиента "docDomesticExpense.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocDomesticExpenses
                        where x.DocDomesticExpenseID == docDomesticExpense.DocDomesticExpenseID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docDomesticExpense.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */


                //Проверка "Скидки"
                //1. Получаем сотурдника с правами



                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docDomesticExpense.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docDomesticExpense = await Task.Run(() => mPutPostDocDomesticExpense(db, dbRead, UO_Action, docDomesticExpense, EntityState.Added, docDomesticExpenseTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docDomesticExpense.DocDomesticExpenseID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docDomesticExpense.DocID,
                    DocDomesticExpenseID = docDomesticExpense.DocDomesticExpenseID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }




        // !!! НЕ СДЕЛНО !!!
        //Надо определять касса или банк и так удалять!
        //А как определить?!

        // DELETE: api/DocDomesticExpenses/5
        [ResponseType(typeof(DocDomesticExpense))]
        public async Task<IHttpActionResult> DeleteDocDomesticExpense(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocDomesticExpenses"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Проверки

                //Документ проведён!<BR>Перед удалением, нужно отменить проводку!
                /*try
                {
                    var queryHeld = await Task.Run(() =>
                        (
                            from x in dbRead.DocDomesticExpenses
                            where x.DocDomesticExpenseID == id
                            select x
                        ).ToListAsync());

                    if (queryHeld.Count() > 0)
                        if (Convert.ToBoolean(queryHeld[0].doc.Held))
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg18)); //return BadRequest();
                }
                catch (Exception ex)
                {
                    return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                }*/

                #endregion


                #region Удаление

                //Алгоритм.
                //Удаляем по порядку:
                //1. RemParties
                //2. DocDomesticExpenseTabs
                //3. DocDomesticExpenses
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocDomesticExpense docDomesticExpense = await db.DocDomesticExpenses.FindAsync(id);
                if (docDomesticExpense == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Ищим DocID *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocDomesticExpenses
                                where x.DocDomesticExpenseID == id
                                select x.doc
                            ).ToListAsync();
                        if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                        #endregion


                        #region Касса/Банк

                        //Касса
                        if (queryDocs1[0].DirPaymentTypeID == 1)
                        {
                            var queryDocCashOfficeSums = await
                                (
                                    from x in db.DocCashOfficeSums
                                    where x.DocID == iDocID
                                    select x
                                ).ToListAsync();
                            for (int i = 0; i < queryDocCashOfficeSums.Count(); i++)
                            {
                                Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = await db.DocCashOfficeSums.FindAsync(queryDocCashOfficeSums[i].DocCashOfficeSumID);
                                db.DocCashOfficeSums.Remove(docCashOfficeSum);
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            var queryDocBankSums = await
                                (
                                    from x in db.DocBankSums
                                    where x.DocID == iDocID
                                    select x
                                ).ToListAsync();
                            for (int i = 0; i < queryDocBankSums.Count(); i++)
                            {
                                Models.Sklad.Doc.DocBankSum docBankSum = await db.DocBankSums.FindAsync(queryDocBankSums[i].DocBankSumID);
                                db.DocBankSums.Remove(docBankSum);
                                await db.SaveChangesAsync();
                            }
                        }

                        #endregion


                        #region 2. DocDomesticExpenseTabs *** *** *** *** ***

                        var queryDocDomesticExpenseTabs = await
                            (
                                from x in db.DocDomesticExpenseTabs
                                where x.DocDomesticExpenseID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocDomesticExpenseTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocDomesticExpenseTab docDomesticExpenseTab = await db.DocDomesticExpenseTabs.FindAsync(queryDocDomesticExpenseTabs[i].DocDomesticExpenseTabID);
                            db.DocDomesticExpenseTabs.Remove(docDomesticExpenseTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocDomesticExpenses *** *** *** *** ***

                        var queryDocDomesticExpenses = await
                            (
                                from x in db.DocDomesticExpenses
                                where x.DocDomesticExpenseID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocDomesticExpenses.Count(); i++)
                        {
                            Models.Sklad.Doc.DocDomesticExpense docDomesticExpense1 = await db.DocDomesticExpenses.FindAsync(queryDocDomesticExpenses[i].DocDomesticExpenseID);
                            db.DocDomesticExpenses.Remove(docDomesticExpense1);
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

        private bool DocDomesticExpenseExists(int id)
        {
            return db.DocDomesticExpenses.Count(e => e.DocDomesticExpenseID == id) > 0;
        }


        internal async Task<DocDomesticExpense> mPutPostDocDomesticExpense(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocDomesticExpense docDomesticExpense,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocDomesticExpenseTab[] docDomesticExpenseTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            //if (UO_Action == "held") docDomesticExpense.Reserve = false;
            //else docDomesticExpense.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docDomesticExpense.NumberInt;
                doc.NumberReal = docDomesticExpense.DocDomesticExpenseID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docDomesticExpense.DirPaymentTypeID;
                doc.Payment = docDomesticExpense.Payment;
                if (docDomesticExpense.DirContractorID != null) doc.DirContractorID = Convert.ToInt32(docDomesticExpense.DirContractorID); else doc.DirContractorID = docDomesticExpense.DirContractorIDOrg;
                doc.DirContractorIDOrg = docDomesticExpense.DirContractorIDOrg;
                doc.Discount = docDomesticExpense.Discount;
                doc.DirVatValue = docDomesticExpense.DirVatValue;
                doc.Base = docDomesticExpense.Base;
                doc.Description = docDomesticExpense.Description;
                doc.DocDate = DateTime.Now; //docDomesticExpense.DocDate;
                //doc.DocDisc = docDomesticExpense.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docDomesticExpense.DocID;
                doc.DocIDBase = docDomesticExpense.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docDomesticExpense" со всем полями!
                docDomesticExpense.DocID = doc.DocID;

                #endregion

                #region 2. DocDomesticExpense

                docDomesticExpense.DocID = doc.DocID;

                db.Entry(docDomesticExpense).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT

                if (entityState == EntityState.Added && (docDomesticExpense.doc.NumberInt == null || docDomesticExpense.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docDomesticExpense.DocDomesticExpenseID.ToString();
                    doc.NumberReal = docDomesticExpense.DocDomesticExpenseID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docDomesticExpense.DocDomesticExpenseID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocDomesticExpenseTab

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocDomesticExpenseID = new SQLiteParameter("@DocDomesticExpenseID", System.Data.DbType.Int32) { Value = docDomesticExpense.DocDomesticExpenseID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocDomesticExpenseTabs WHERE DocDomesticExpenseID=@DocDomesticExpenseID;", parDocDomesticExpenseID);
                }

                //2.2. Проставляем ID-шник "DocDomesticExpenseID" для всех позиций спецификации
                double dSumTab = 0;
                string DirDomesticExpenseName = "";
                for (int i = 0; i < docDomesticExpenseTabCollection.Count(); i++)
                {
                    docDomesticExpenseTabCollection[i].DocDomesticExpenseTabID = null;
                    docDomesticExpenseTabCollection[i].DocDomesticExpenseID = Convert.ToInt32(docDomesticExpense.DocDomesticExpenseID);
                    db.Entry(docDomesticExpenseTabCollection[i]).State = EntityState.Added;

                    dSumTab += docDomesticExpenseTabCollection[i].PriceCurrency;

                    Models.Sklad.Dir.DirDomesticExpense dirDomesticExpense = await db.DirDomesticExpenses.FindAsync(docDomesticExpenseTabCollection[i].DirDomesticExpenseID);
                    DirDomesticExpenseName += dirDomesticExpense.DirDomesticExpenseName;
                }
                await db.SaveChangesAsync();
                //dSumTab = dSumTab - doc.Discount;

                #endregion


                if (UO_Action == "held")
                {
                    #region Касса или Банк


                    #region 1. Получаем валюту из склада

                    int DirCurrencyID = 0, DirCurrencyMultiplicity = 0; //, DirCashOfficeID = 0, DirBankID = 0;;
                    double DirCurrencyRate = 0;

                    var query = await Task.Run(() =>
                        (
                            from x in db.DirWarehouses
                            where x.DirWarehouseID == docDomesticExpense.DirWarehouseID
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
                        else
                        {
                            //DirBankID = Convert.ToInt32(query[0].DirBankID);
                            DirCurrencyID = query[0].DirCurrencyID_Bank;
                            DirCurrencyRate = query[0].DirCurrencyRate_Bank;
                            DirCurrencyMultiplicity = query[0].DirCurrencyMultiplicity_Bank;
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

                    pay.DirEmployeeID = docDomesticExpense.DirEmployeeIDSpisat; //field.DirEmployeeID;
                    pay.DirPaymentTypeID = doc.DirPaymentTypeID;
                    //pay.DirXName = ""; //no
                    //pay.DirXSumTypeID = 0; //no
                    pay.DocCashBankID = null;
                    pay.DocID = doc.DocID;
                    pay.DocXID = docDomesticExpense.DocDomesticExpenseID;
                    pay.DocXSumDate = doc.DocDate;
                    pay.DocXSumSum = dSumTab; // - получили при сохранении Спецификации (выше)
                    pay.Base = "Оплата за: " + DirDomesticExpenseName; // - получили при сохранении Спецификации (выше)
                    //pay.Description = "";
                    pay.Discount = doc.Discount;

                    #endregion


                    #region 3. Сохраняем

                    PartionnyAccount.Controllers.Sklad.Pay.PayController payController = new Pay.PayController();
                    doc = await Task.Run(() => payController.mPutPostPay(db, pay, EntityState.Modified, field)); //sysSetting

                    #endregion


                    #endregion
                }
            }
            else if (UO_Action == "held_cancel")
            {
                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = await Task.Run(() => db.Docs.FindAsync(docDomesticExpense.DocID));
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion

                #region 3. Касса или Банк

                //По "doc.DocID" и "doc.DirPaymentTypeID" получаем ID оплаты
                int? DocCashBankID = 0;
                if (doc.DirPaymentTypeID == 1)
                {
                    var queryDocCashBankID = await Task.Run(() =>
                       (
                            from x in db.DocCashOfficeSums
                            where x.DocID == doc.DocID
                            select x
                        ).ToListAsync());

                    if (queryDocCashBankID.Count() > 0) DocCashBankID = queryDocCashBankID[0].DocCashOfficeSumID;
                }
                else
                {
                    var queryDocCashBankID = await Task.Run(() =>
                       (
                            from x in db.DocBankSums
                            where x.DocID == doc.DocID
                            select x
                        ).ToListAsync());

                    if (queryDocCashBankID.Count() > 0) DocCashBankID = queryDocCashBankID[0].DocBankSumID;
                }

                PartionnyAccount.Controllers.Sklad.Pay.PayController payController = new Pay.PayController();
                int? iDocID = await Task.Run(() => payController.mDeletePay(db, Convert.ToInt32(doc.DirPaymentTypeID), Convert.ToInt32(DocCashBankID))); //sysSetting

                #endregion
            }


            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docDomesticExpense;
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
                "[DocDomesticExpenses].[DocDomesticExpenseID] AS [DocDomesticExpenseID], " +
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
                "[DocDomesticExpenses].[Reserve] AS [Reserve] " +

                "FROM [DocDomesticExpenses] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocDomesticExpenses].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocDomesticExpenses].[DirWarehouseID] " +
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