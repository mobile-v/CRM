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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocRetailReturns
{
    public class DocRetailReturnsController : ApiController
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

        int ListObjectID = 57;

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
        // GET: api/DocRetailReturns
        public async Task<IHttpActionResult> GetDocRetailReturns(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailReturns"));
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
                        from docRetailReturns in db.DocRetailReturns

                        join docRetailReturnTabs1 in db.DocRetailReturnTabs on docRetailReturns.DocRetailReturnID equals docRetailReturnTabs1.DocRetailReturnID into docRetailReturnTabs2
                        from docRetailReturnTabs in docRetailReturnTabs2.DefaultIfEmpty()

                        where docRetailReturns.doc.DocDate >= _params.DateS && docRetailReturns.doc.DocDate <= _params.DatePo

                        group new { docRetailReturnTabs }
                        by new
                        {
                            DocID = docRetailReturns.DocID,
                            DocDate = docRetailReturns.doc.DocDate,
                            Base = docRetailReturns.doc.Base,
                            Held = docRetailReturns.doc.Held,
                            Discount = docRetailReturns.doc.Discount,
                            Del = docRetailReturns.doc.Del,
                            Description = docRetailReturns.doc.Description,
                            IsImport = docRetailReturns.doc.IsImport,
                            DirVatValue = docRetailReturns.doc.DirVatValue,

                            DocRetailReturnID = docRetailReturns.DocRetailReturnID,
                            DirContractorName = docRetailReturns.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docRetailReturns.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docRetailReturns.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docRetailReturns.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docRetailReturns.dirWarehouse.DirWarehouseName,

                            NumberInt = docRetailReturns.doc.NumberInt,

                            //Оплата
                            Payment = docRetailReturns.doc.Payment,
                            
                            DocDateHeld = docRetailReturns.doc.DocDateHeld,
                            DocDatePayment = docRetailReturns.doc.DocDatePayment,
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

                            DocRetailReturnID = g.Key.DocRetailReturnID,
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
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * x.docRetailReturnTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * x.docRetailReturnTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * x.docRetailReturnTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * x.docRetailReturnTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
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
                        query = query.Where(x => x.DocRetailReturnID == iNumber32);
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

                query = query.OrderByDescending(x => x.DocDate).Skip(_params.Skip).Take(_params.limit);

                /*
                if (sysSetting.DocsSortField == 1)
                {
                    query = query.OrderByDescending(x => x.DocRetailReturnID);
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
                    query = query.OrderByDescending(x => x.DocRetailReturnID);
                }
                */

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocRetailReturns.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocRetailReturn = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocRetailReturns/5
        [ResponseType(typeof(DocRetailReturn))]
        public async Task<IHttpActionResult> GetDocRetailReturn(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailReturns"));
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
                        from docRetailReturns in db.DocRetailReturns
                        where docRetailReturns.DocRetailReturnID == id
                        select docRetailReturns
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docRetailReturns in db.DocRetailReturns

                        join docRetailReturnTabs1 in db.DocRetailReturnTabs on docRetailReturns.DocRetailReturnID equals docRetailReturnTabs1.DocRetailReturnID into docRetailReturnTabs2
                        from docRetailReturnTabs in docRetailReturnTabs2.DefaultIfEmpty()

                        #endregion

                        where docRetailReturns.DocRetailReturnID == id

                        #region group

                        group new { docRetailReturnTabs }
                        by new
                        {
                            DocID = docRetailReturns.DocID, //DocID = docRetailReturns.doc.DocID,
                            DocRetailID = docRetailReturns.DocRetailID,
                            DocRetailName = docRetailReturns.DocRetailID, //"№ " + docRetailReturns.DocRetailID + " за " + docRetailReturns.doc.DocDate,
                            DocIDBase = docRetailReturns.doc.DocIDBase,
                            DocDate = docRetailReturns.doc.DocDate,
                            Base = docRetailReturns.doc.Base,
                            Held = docRetailReturns.doc.Held,
                            Discount = docRetailReturns.doc.Discount,
                            Del = docRetailReturns.doc.Del,
                            IsImport = docRetailReturns.doc.IsImport,
                            Description = docRetailReturns.doc.Description,
                            DirVatValue = docRetailReturns.doc.DirVatValue,
                            //DirPaymentTypeID = docRetailReturns.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docRetailReturns.doc.dirPaymentType.DirPaymentTypeName,

                            DocRetailReturnID = docRetailReturns.DocRetailReturnID,
                            DirContractorID = docRetailReturns.doc.DirContractorID,
                            DirContractorName = docRetailReturns.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docRetailReturns.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docRetailReturns.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseID = docRetailReturns.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docRetailReturns.dirWarehouse.DirWarehouseName,
                            NumberInt = docRetailReturns.doc.NumberInt,

                            //Оплата
                            Payment = docRetailReturns.doc.Payment,

                            //Резерв
                            Reserve = docRetailReturns.Reserve,
                            OnCredit = docRetailReturns.OnCredit
                        }
                        into g

                        #endregion

                        #region select

                        select new
                        {
                            DocID = g.Key.DocID,
                            DocRetailID = g.Key.DocRetailID,
                            DocRetailName = g.Key.DocRetailName,
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

                            DocRetailReturnID = g.Key.DocRetailReturnID,
                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,

                            Reserve = g.Key.Reserve,
                            OnCredit = g.Key.OnCredit,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * x.docRetailReturnTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * x.docRetailReturnTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * (x.docRetailReturnTabs.PriceCurrency - (x.docRetailReturnTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * (x.docRetailReturnTabs.PriceCurrency - (x.docRetailReturnTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * x.docRetailReturnTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailReturnTabs.Quantity * x.docRetailReturnTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocRetailReturns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocRetailReturn(int id, DocRetailReturn docRetailReturn, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailReturns"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

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

                //Ставим дату с ПК
                docRetailReturn.DocDate = DateTime.Now;

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocRetailReturnTab[] docRetailReturnTabCollection = null;
                if (!String.IsNullOrEmpty(docRetailReturn.recordsDocRetailReturnTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docRetailReturnTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocRetailReturnTab[]>(docRetailReturn.recordsDocRetailReturnTab);
                }

                #endregion

                #region Проверки

                //1. 
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docRetailReturn.DocRetailReturnID || docRetailReturn.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docRetailReturn.DocID" из БД, если он отличается от пришедшего от клиента "docRetailReturn.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocRetailReturns
                        where x.DocRetailReturnID == docRetailReturn.DocRetailReturnID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docRetailReturn.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docRetailReturn.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docRetailReturn.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docRetailReturn = await Task.Run(() => mPutPostDocRetailReturn(db, dbRead, UO_Action, docRetailReturn, EntityState.Modified, docRetailReturnTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docRetailReturn.DocRetailReturnID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docRetailReturn.DocID,
                    DocRetailReturnID = docRetailReturn.DocRetailReturnID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        //!!! НЕ ИСПОЛЬЗУЕТСЯ !!!
        //ИСПОЛЬЗУЕТСЯ В КОНТРОЛЛЕРЕ "DocRetailsController.PutDocRetail"
        //После печати чека, если щабыли напечатать, сохранить ID-шнини чека
        //id == DocID
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocRetailReturn(int id, int ServiceTypeRepair, int iTrash, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailReturns"));
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
                sysJourDisp.TableFieldID = docRetailReturn.DocRetailReturnID;
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


        // POST: api/DocRetailReturns
        [ResponseType(typeof(DocRetailReturn))]
        public async Task<IHttpActionResult> PostDocRetailReturn(DocRetailReturn docRetailReturn, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailReturns"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

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

                //Ставим дату с ПК
                docRetailReturn.DocDate = DateTime.Now;

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocRetailReturnTab[] docRetailReturnTabCollection = null;
                if (!String.IsNullOrEmpty(docRetailReturn.recordsDocRetailReturnTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docRetailReturnTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocRetailReturnTab[]>(docRetailReturn.recordsDocRetailReturnTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Это нужно только для UPDATE
                /*
                try
                {
                    //Получаем "docRetailReturn.DocID" из БД, если он отличается от пришедшего от клиента "docRetailReturn.DocID" выдаём ошибку
                    //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                    var query = await Task.Run(() =>
                        (
                            from x in dbRead.DocRetailReturns
                            where x.DocRetailReturnID == docRetailReturn.DocRetailReturnID
                            select x
                        ).ToListAsync());

                    if (query.Count() > 0)
                        if (query[0].DocID != docRetailReturn.DocID)
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                    //dbRead.Database.Connection.Close();
                }
                catch (Exception ex)
                {
                    return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                }
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docRetailReturn.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docRetailReturn = await Task.Run(() => mPutPostDocRetailReturn(db, dbRead, UO_Action, docRetailReturn, EntityState.Added, docRetailReturnTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docRetailReturn.DocRetailReturnID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docRetailReturn.DocID,
                    DocRetailReturnID = docRetailReturn.DocRetailReturnID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocRetailReturns/5
        [ResponseType(typeof(DocRetailReturn))]
        public async Task<IHttpActionResult> DeleteDocRetailReturn(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailReturns"));
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
                            from x in dbRead.DocRetailReturns
                            where x.DocRetailReturnID == id
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
                //2. DocRetailReturnTabs
                //3. DocRetailReturns
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocRetailReturn docRetailReturn = await db.DocRetailReturns.FindAsync(id);
                if (docRetailReturn == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. RemParties *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocRetailReturns
                                where x.DocRetailReturnID == id
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


                        #region 2. DocRetailReturnTabs *** *** *** *** ***

                        var queryDocRetailReturnTabs = await
                            (
                                from x in db.DocRetailReturnTabs
                                where x.DocRetailReturnID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocRetailReturnTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocRetailReturnTab docRetailReturnTab = await db.DocRetailReturnTabs.FindAsync(queryDocRetailReturnTabs[i].DocRetailReturnTabID);
                            db.DocRetailReturnTabs.Remove(docRetailReturnTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocRetailReturns *** *** *** *** ***

                        var queryDocRetailReturns = await
                            (
                                from x in db.DocRetailReturns
                                where x.DocRetailReturnID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocRetailReturns.Count(); i++)
                        {
                            Models.Sklad.Doc.DocRetailReturn docRetailReturn1 = await db.DocRetailReturns.FindAsync(queryDocRetailReturns[i].DocRetailReturnID);
                            db.DocRetailReturns.Remove(docRetailReturn1);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 4. DocRetailReturns *** *** *** *** ***

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

        private bool DocRetailReturnExists(int id)
        {
            return db.DocRetailReturns.Count(e => e.DocRetailReturnID == id) > 0;
        }


        internal async Task<DocRetailReturn> mPutPostDocRetailReturn(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocRetailReturn docRetailReturn,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocRetailReturnTab[] docRetailReturnTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docRetailReturn.NumberInt;
                doc.NumberReal = docRetailReturn.DocRetailReturnID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docRetailReturn.DirPaymentTypeID;
                doc.Payment = docRetailReturn.Payment;
                if(doc.DirContractorID > 0) doc.DirContractorID = docRetailReturn.DirContractorID;
                else doc.DirContractorID = docRetailReturn.DirContractorIDOrg;
                doc.DirContractorIDOrg = docRetailReturn.DirContractorIDOrg;
                doc.Discount = docRetailReturn.Discount;
                doc.DirVatValue = docRetailReturn.DirVatValue;
                doc.Base = docRetailReturn.Base;
                doc.Description = ""; // docRetailReturn.Description;
                doc.DocDate = DateTime.Now; //docRetailReturn.DocDate;
                //doc.DocDisc = docRetailReturn.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docRetailReturn.DocID;
                doc.DocIDBase = docRetailReturn.DocIDBase;
                doc.KKMSCheckNumber = docRetailReturn.KKMSCheckNumber;
                doc.KKMSIdCommand = docRetailReturn.KKMSIdCommand;
                doc.KKMSEMail = docRetailReturn.KKMSEMail;
                doc.KKMSPhone = docRetailReturn.KKMSPhone;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docRetailReturn" со всем полями!
                docRetailReturn.DocID = doc.DocID;

                #endregion

                #region 2. Находим "DocRetailID" (алгоритм работы поменялся, а поле осталось)

                int iRemPartyMinusID_ = Convert.ToInt32(docRetailReturnTabCollection[0].RemPartyMinusID);

                var queryDocRetailID = await
                    (
                        from x in db.RemPartyMinuses
                        where x.RemPartyMinusID == iRemPartyMinusID_
                        select new
                        {
                            NumberReal = x.doc.NumberReal
                        }
                    ).ToListAsync();

                if (queryDocRetailID.Count() > 0)
                {
                    docRetailReturn.DocRetailID = queryDocRetailID[0].NumberReal;
                }

                #endregion

                #region 3. DocRetailReturn *** *** *** *** *** *** *** *** *** ***

                docRetailReturn.DocID = doc.DocID;

                db.Entry(docRetailReturn).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docRetailReturn.doc.NumberInt == null || docRetailReturn.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docRetailReturn.DocRetailReturnID.ToString();
                    doc.NumberReal = docRetailReturn.DocRetailReturnID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docRetailReturn.DocRetailReturnID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 4. Description: пишем ID-шник в DocRetailReturnTab и RemParty

                string Description = ""; if (docRetailReturnTabCollection.Length > 0) Description = docRetailReturnTabCollection[0].Description;
                int? DirDescriptionID = null;
                if (!String.IsNullOrEmpty(Description))
                {
                    //Алгоритм:
                    //1. Проверяем, если уже такая запись есть (маленькими буквами), то не сохраняем
                    //2. Иначе сохраняем
                    var queryDirDescriptions = await
                        (
                            from x in dbRead.DirDescriptions
                            where x.DirDescriptionName.ToLower() == Description.ToLower()
                            select x
                        ).ToListAsync();

                    if (queryDirDescriptions.Count() == 0)
                    {
                        //Модель
                        Models.Sklad.Dir.DirDescription dirDescription = new Models.Sklad.Dir.DirDescription();
                        dirDescription.DirDescriptionName = Description;
                        db.Entry(dirDescription).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        DirDescriptionID = dirDescription.DirDescriptionID;
                    }
                    else
                    {
                        DirDescriptionID = queryDirDescriptions[0].DirDescriptionID;
                    }
                }

                #endregion

                #region 5. DocRetailReturnTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocRetailReturnID = new SQLiteParameter("@DocRetailReturnID", System.Data.DbType.Int32) { Value = docRetailReturn.DocRetailReturnID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocRetailReturnTabs WHERE DocRetailReturnID=@DocRetailReturnID;", parDocRetailReturnID);
                }

                //2.2. Проставляем ID-шник "DocRetailReturnID" для всех позиций спецификации
                string NomenName = "";
                double dSumTab = 0;
                for (int i = 0; i < docRetailReturnTabCollection.Count(); i++)
                {
                    docRetailReturnTabCollection[i].DocRetailReturnTabID = null;
                    docRetailReturnTabCollection[i].DocRetailReturnID = Convert.ToInt32(docRetailReturn.DocRetailReturnID);
                    docRetailReturnTabCollection[i].DirDescriptionID = DirDescriptionID;
                    db.Entry(docRetailReturnTabCollection[i]).State = EntityState.Added;

                    dSumTab += docRetailReturnTabCollection[i].Quantity * docRetailReturnTabCollection[i].PriceCurrency;

                    NomenName += docRetailReturnTabCollection[i].DirNomenID + " ";
                }
                await db.SaveChangesAsync();
                dSumTab = dSumTab - doc.Discount;

                #endregion


                //Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();
                if (UO_Action == "held" || docRetailReturn.Reserve)
                {
                    //Алгоритм:
                    //По ПартияМинус находим Партию
                    //С неё берём Характеристики и цены
                    //Создаём новую партию

                    //!!! Важно !!!
                    //Ещё должна быть проверка: например, покупатель уже возвращал товар с этой ПартияМинус
                    //То есть в Партии надо иметь Линк на Партию Минус.


                    #region RemParty - Партии

                    Models.Sklad.Rem.RemParty[] remPartyCollection = new Models.Sklad.Rem.RemParty[docRetailReturnTabCollection.Count()];
                    for (int i = 0; i < docRetailReturnTabCollection.Count(); i++)
                    {
                        Models.Sklad.Rem.RemPartyMinus remPartyMinus = await db.RemPartyMinuses.FindAsync(docRetailReturnTabCollection[i].RemPartyMinusID);


                        #region Проверка: 1 и 2

                        //1. Что бы клиент не вернул товара больше, чем купил
                        if (docRetailReturnTabCollection[i].Quantity > remPartyMinus.Quantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg108 +
                                //docRetailReturnTabCollection[i].DirNomenID + " (" + docRetailReturnTabCollection[i].RemPartyMinusID + ") - " + iRemPartyMinusID
                                "<tr>" +
                                "<td>" + docRetailReturnTabCollection[i].RemPartyMinusID + "</td>" +     //списаная партия
                                "<td>" + docRetailReturnTabCollection[i].DirNomenID + "</td>" +     //код товара
                                "<td>" + remPartyMinus.Quantity + "</td>" +                //к-во реальное
                                "</tr>" +
                                "</table>"
                                );
                        }
                        //2. Что бы клиент 2-ды не вернул один и тот же товар с продажи
                        int iRemPartyMinusID = Convert.ToInt32(docRetailReturnTabCollection[i].RemPartyMinusID);
                        double? dSumQuantity = db.RemParties.Where(x => x.RemPartyMinusID == iRemPartyMinusID).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();
                        if (dSumQuantity >= remPartyMinus.Quantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg109 +
                                "<tr>" +
                                "<td>" + docRetailReturnTabCollection[i].RemPartyMinusID + "</td>" +     //списаная партия
                                "<td>" + docRetailReturnTabCollection[i].DirNomenID + "</td>" +     //код товара
                                "</tr>" +
                                "</table>"
                                );
                        }

                        #endregion


                        #region Партия
                        
                        Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();

                        remParty.RemPartyID = null;
                        remParty.DirNomenID = remPartyMinus.remParty.DirNomenID;
                        remParty.Quantity = docRetailReturnTabCollection[i].Quantity;
                        remParty.Remnant = docRetailReturnTabCollection[i].Quantity;
                        remParty.DirCurrencyID = remPartyMinus.remParty.DirCurrencyID;
                        //remParty.DirCurrencyMultiplicity = docRetailReturnTabCollection[i].DirCurrencyMultiplicity;
                        //remParty.DirCurrencyRate = docRetailReturnTabCollection[i].DirCurrencyRate;
                        remParty.DirVatValue = remPartyMinus.remParty.DirVatValue;
                        remParty.DirWarehouseID = remPartyMinus.remParty.DirWarehouseID;
                        remParty.DirWarehouseIDDebit = remPartyMinus.remParty.DirWarehouseIDDebit;
                        remParty.DirWarehouseIDPurch = remPartyMinus.remParty.DirWarehouseIDPurch;
                        remParty.DirContractorIDOrg = remPartyMinus.remParty.DirContractorIDOrg;
                        remParty.DirContractorID = remPartyMinus.remParty.DirContractorID;

                        //Дата Приёмки товара
                        remParty.DocDatePurches = remPartyMinus.remParty.DocDatePurches;

                        remParty.DirCharColourID = remPartyMinus.remParty.DirCharColourID; //docRetailReturnTabCollection[i].DirCharColourID;
                        remParty.DirCharMaterialID = remPartyMinus.remParty.DirCharMaterialID;
                        remParty.DirCharNameID = remPartyMinus.remParty.DirCharNameID;
                        remParty.DirCharSeasonID = remPartyMinus.remParty.DirCharSeasonID;
                        remParty.DirCharSexID = remPartyMinus.remParty.DirCharSexID;
                        remParty.DirCharSizeID = remPartyMinus.remParty.DirCharSizeID;
                        remParty.DirCharStyleID = remPartyMinus.remParty.DirCharStyleID;
                        remParty.DirCharTextureID = remPartyMinus.remParty.DirCharTextureID;

                        remParty.SerialNumber = remPartyMinus.remParty.SerialNumber;
                        remParty.Barcode = remPartyMinus.remParty.Barcode;

                        remParty.DocID = Convert.ToInt32(docRetailReturn.DocID);
                        remParty.PriceCurrency = remPartyMinus.remParty.PriceCurrency;
                        remParty.PriceVAT = remPartyMinus.remParty.PriceVAT;
                        remParty.FieldID = Convert.ToInt32(docRetailReturnTabCollection[i].DocRetailReturnTabID);

                        remParty.PriceRetailVAT = remPartyMinus.remParty.PriceRetailVAT;
                        remParty.PriceRetailCurrency = remPartyMinus.remParty.PriceRetailCurrency;
                        remParty.PriceWholesaleVAT = remPartyMinus.remParty.PriceWholesaleVAT;
                        remParty.PriceWholesaleCurrency = remPartyMinus.remParty.PriceWholesaleCurrency;
                        remParty.PriceIMVAT = remPartyMinus.remParty.PriceIMVAT;
                        remParty.PriceIMCurrency = remPartyMinus.remParty.PriceIMCurrency;

                        remParty.RemPartyMinusID = remPartyMinus.RemPartyMinusID;

                        remParty.DirReturnTypeID = docRetailReturnTabCollection[i].DirReturnTypeID;
                        remParty.DirDescriptionID = DirDescriptionID;

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


                    #region 1. Получаем валюту из склада

                    int DirCurrencyID = 0, DirCurrencyMultiplicity = 0; //, DirCashOfficeID = 0, DirBankID = 0;;
                    double DirCurrencyRate = 0;

                    var query = await Task.Run(() =>
                        (
                            from x in db.DirWarehouses
                            where x.DirWarehouseID == docRetailReturn.DirWarehouseID
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

                    pay.DirEmployeeID = field.DirEmployeeID;
                    pay.DirPaymentTypeID = doc.DirPaymentTypeID;
                    //pay.DirXName = ""; //no
                    //pay.DirXSumTypeID = 0; //no
                    pay.DocCashBankID = null;
                    pay.DocID = doc.DocID;
                    pay.DocXID = docRetailReturn.DocRetailReturnID;
                    pay.DocXSumDate = doc.DocDate;
                    pay.DocXSumSum = dSumTab; // - получили при сохранении Спецификации (выше)
                    pay.Base = "Возврат за коды товаров: " + NomenName; // - получили при сохранении Спецификации (выше)
                    pay.KKMSCheckNumber = docRetailReturn.KKMSCheckNumber;
                    pay.KKMSIdCommand = docRetailReturn.KKMSIdCommand;
                    pay.KKMSEMail = docRetailReturn.KKMSEMail;
                    pay.KKMSPhone = docRetailReturn.KKMSPhone;
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
                #region 1. Проверка: Было ли списание с партий *** *** *** *** *** *** *** *** *** ***


                //int DocRetailReturnID = Convert.ToInt32(docRetailReturn.DocRetailReturnID);


                //Алгоритм №1
                //SELECT DocID
                //FROM RemPartyMinuses 
                //WHERE RemPartyID in (SELECT RemPartyID FROM RemParties WHERE DocID=@DocID)



                #region Алгоритм №1 (OLD)

                /*
                //Получаем DocRetailReturn из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocRetailReturn _docRetailReturn = db.DocRetailReturns.Find(DocRetailReturnID);
                int? iDocRetailReturn_DocID = _docRetailReturn.DocID;


                var queryRemPartyMinuses =
                    (
                        from remPartyMinuses in db.RemPartyMinuses

                        join remParties1 in db.RemParties on remPartyMinuses.RemPartyID equals remParties1.RemPartyID into remParties2
                        from remParties in remParties2.Where(x => x.DocID == iDocRetailReturn_DocID) //.DefaultIfEmpty()

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

                //Получаем DocRetailReturn из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocRetailReturn _docRetailReturn = db.DocRetailReturns.Find(DocRetailReturnID);
                int? iDocRetailReturn_DocID = _docRetailReturn.DocID;

                //Есть ли списанный товар с данного прихода
                var queryRemParties = await Task.Run(() =>
                    (
                        from x in db.RemParties
                        where x.DocID == iDocRetailReturn_DocID && x.Quantity != x.Remnant
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
                            from remParties in remParties2.Where(x => x.DocID == iDocRetailReturn_DocID) //.DefaultIfEmpty()

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

                int DocRetailReturnID = Convert.ToInt32(docRetailReturn.DocRetailReturnID);

                Models.Sklad.Doc.DocRetailReturn _docRetailReturn = db.DocRetailReturns.Find(DocRetailReturnID);
                int? iDocRetailReturn_DocID = _docRetailReturn.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocRetailReturn_DocID));

                #endregion


                //Проверка и Удаление записей в таблицах: RemParties
                #region 1. RemParties - удаление *** *** *** *** *** *** *** *** *** ***

                //Проверяем если ли расходы (проведённый или НЕ проведенные)


                //Удаляем записи в таблице "RemParties"
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docRetailReturn.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID; ", parDocID);

                #endregion

                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docRetailReturn.DocID);
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


            return docRetailReturn;
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
                "[DocRetailReturns].[DocRetailReturnID] AS [DocRetailReturnID], " +
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
                "[DocRetailReturns].[NumberTT] AS [NumberTT], " +
                "[DocRetailReturns].[NumberTax] AS [NumberTax] " +

                "FROM [DocRetailReturns] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocRetailReturns].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocRetailReturns].[DirWarehouseID] " +
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