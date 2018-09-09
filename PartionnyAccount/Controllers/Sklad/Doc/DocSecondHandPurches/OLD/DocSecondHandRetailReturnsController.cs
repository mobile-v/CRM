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
    public class DocSecondHandRetailReturnsController : ApiController
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

        int ListObjectID = 67;

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
        // GET: api/DocSecondHandRetailReturns
        public async Task<IHttpActionResult> GetDocSecondHandRetailReturns(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailReturns"));
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
                        from docSecondHandRetailReturns in db.DocSecondHandRetailReturns

                        join docSecondHandRetailReturnTabs1 in db.DocSecondHandRetailReturnTabs on docSecondHandRetailReturns.DocSecondHandRetailReturnID equals docSecondHandRetailReturnTabs1.DocSecondHandRetailReturnID into docSecondHandRetailReturnTabs2
                        from docSecondHandRetailReturnTabs in docSecondHandRetailReturnTabs2.DefaultIfEmpty()

                        where docSecondHandRetailReturns.doc.DocDate >= _params.DateS && docSecondHandRetailReturns.doc.DocDate <= _params.DatePo

                        group new { docSecondHandRetailReturnTabs }
                        by new
                        {
                            DocID = docSecondHandRetailReturns.DocID,
                            DocDate = docSecondHandRetailReturns.doc.DocDate,
                            Base = docSecondHandRetailReturns.doc.Base,
                            Held = docSecondHandRetailReturns.doc.Held,
                            Discount = docSecondHandRetailReturns.doc.Discount,
                            Del = docSecondHandRetailReturns.doc.Del,
                            Description = docSecondHandRetailReturns.doc.Description,
                            IsImport = docSecondHandRetailReturns.doc.IsImport,
                            DirVatValue = docSecondHandRetailReturns.doc.DirVatValue,

                            DocSecondHandRetailReturnID = docSecondHandRetailReturns.DocSecondHandRetailReturnID,
                            DirContractorName = docSecondHandRetailReturns.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandRetailReturns.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandRetailReturns.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandRetailReturns.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandRetailReturns.dirWarehouse.DirWarehouseName,

                            NumberInt = docSecondHandRetailReturns.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandRetailReturns.doc.Payment,

                            //Описание причины возврата

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

                            DocSecondHandRetailReturnID = g.Key.DocSecondHandRetailReturnID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * x.docSecondHandRetailReturnTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * x.docSecondHandRetailReturnTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * x.docSecondHandRetailReturnTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * x.docSecondHandRetailReturnTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
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
                        query = query.Where(x => x.DocSecondHandRetailReturnID == iNumber32);
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

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocSecondHandRetailReturns.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSecondHandRetailReturn = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandRetailReturns/5
        [ResponseType(typeof(DocSecondHandRetailReturn))]
        public async Task<IHttpActionResult> GetDocSecondHandRetailReturn(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailReturns"));
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
                        from docSecondHandRetailReturns in db.DocSecondHandRetailReturns
                        where docSecondHandRetailReturns.DocSecondHandRetailReturnID == id
                        select docSecondHandRetailReturns
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                #region from

                        from docSecondHandRetailReturns in db.DocSecondHandRetailReturns

                        join docSecondHandRetailReturnTabs1 in db.DocSecondHandRetailReturnTabs on docSecondHandRetailReturns.DocSecondHandRetailReturnID equals docSecondHandRetailReturnTabs1.DocSecondHandRetailReturnID into docSecondHandRetailReturnTabs2
                        from docSecondHandRetailReturnTabs in docSecondHandRetailReturnTabs2.DefaultIfEmpty()

                            #endregion

                        where docSecondHandRetailReturns.DocSecondHandRetailReturnID == id

                        #region group

                        group new { docSecondHandRetailReturnTabs }
                        by new
                        {
                            DocID = docSecondHandRetailReturns.DocID, //DocID = docSecondHandRetailReturns.doc.DocID,
                            DocSecondHandRetailID = docSecondHandRetailReturns.DocSecondHandRetailID,
                            DocSecondHandRetailName = docSecondHandRetailReturns.DocSecondHandRetailID, //"№ " + docSecondHandRetailReturns.DocSecondHandRetailID + " за " + docSecondHandRetailReturns.doc.DocDate,
                            DocIDBase = docSecondHandRetailReturns.doc.DocIDBase,
                            DocDate = docSecondHandRetailReturns.doc.DocDate,
                            Base = docSecondHandRetailReturns.doc.Base,
                            Held = docSecondHandRetailReturns.doc.Held,
                            Discount = docSecondHandRetailReturns.doc.Discount,
                            Del = docSecondHandRetailReturns.doc.Del,
                            IsImport = docSecondHandRetailReturns.doc.IsImport,
                            Description = docSecondHandRetailReturns.doc.Description,
                            DirVatValue = docSecondHandRetailReturns.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandRetailReturns.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docSecondHandRetailReturns.doc.dirPaymentType.DirPaymentTypeName,

                            DocSecondHandRetailReturnID = docSecondHandRetailReturns.DocSecondHandRetailReturnID,
                            DirContractorID = docSecondHandRetailReturns.doc.DirContractorID,
                            DirContractorName = docSecondHandRetailReturns.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandRetailReturns.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docSecondHandRetailReturns.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseID = docSecondHandRetailReturns.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandRetailReturns.dirWarehouse.DirWarehouseName,
                            NumberInt = docSecondHandRetailReturns.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandRetailReturns.doc.Payment,

                            //Резерв
                            Reserve = docSecondHandRetailReturns.Reserve,
                            OnCredit = docSecondHandRetailReturns.OnCredit
                        }
                        into g

                        #endregion

                        #region select

                        select new
                        {
                            DocID = g.Key.DocID,
                            DocSecondHandRetailID = g.Key.DocSecondHandRetailID,
                            DocSecondHandRetailName = g.Key.DocSecondHandRetailName,
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

                            DocSecondHandRetailReturnID = g.Key.DocSecondHandRetailReturnID,
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
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * x.docSecondHandRetailReturnTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * x.docSecondHandRetailReturnTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * (x.docSecondHandRetailReturnTabs.PriceCurrency - (x.docSecondHandRetailReturnTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * (x.docSecondHandRetailReturnTabs.PriceCurrency - (x.docSecondHandRetailReturnTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * x.docSecondHandRetailReturnTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailReturnTabs.Quantity * x.docSecondHandRetailReturnTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocSecondHandRetailReturns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRetailReturn(int id, DocSecondHandRetailReturn docSecondHandRetailReturn, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailReturns"));
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
                docSecondHandRetailReturn.DocDate = DateTime.Now;

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocSecondHandRetailReturnTab[] docSecondHandRetailReturnTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandRetailReturn.recordsDocSecondHandRetailReturnTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandRetailReturnTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandRetailReturnTab[]>(docSecondHandRetailReturn.recordsDocSecondHandRetailReturnTab);
                }

                #endregion

                #region Проверки

                //1. 
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docSecondHandRetailReturn.DocSecondHandRetailReturnID || docSecondHandRetailReturn.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docSecondHandRetailReturn.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandRetailReturn.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandRetailReturns
                        where x.DocSecondHandRetailReturnID == docSecondHandRetailReturn.DocSecondHandRetailReturnID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandRetailReturn.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docSecondHandRetailReturn.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandRetailReturn.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRetailReturn = await Task.Run(() => mPutPostDocSecondHandRetailReturn(db, dbRead, UO_Action, docSecondHandRetailReturn, EntityState.Modified, docSecondHandRetailReturnTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandRetailReturn.DocSecondHandRetailReturnID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandRetailReturn.DocID,
                    DocSecondHandRetailReturnID = docSecondHandRetailReturn.DocSecondHandRetailReturnID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocSecondHandRetailReturns
        [ResponseType(typeof(DocSecondHandRetailReturn))]
        public async Task<IHttpActionResult> PostDocSecondHandRetailReturn(DocSecondHandRetailReturn docSecondHandRetailReturn, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailReturns"));
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
                docSecondHandRetailReturn.DocDate = DateTime.Now;

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocSecondHandRetailReturnTab[] docSecondHandRetailReturnTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandRetailReturn.recordsDocSecondHandRetailReturnTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandRetailReturnTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandRetailReturnTab[]>(docSecondHandRetailReturn.recordsDocSecondHandRetailReturnTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Это нужно только для UPDATE
                /*
                try
                {
                    //Получаем "docSecondHandRetailReturn.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandRetailReturn.DocID" выдаём ошибку
                    //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                    var query = await Task.Run(() =>
                        (
                            from x in dbRead.DocSecondHandRetailReturns
                            where x.DocSecondHandRetailReturnID == docSecondHandRetailReturn.DocSecondHandRetailReturnID
                            select x
                        ).ToListAsync());

                    if (query.Count() > 0)
                        if (query[0].DocID != docSecondHandRetailReturn.DocID)
                            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                    //dbRead.Database.Connection.Close();
                }
                catch (Exception ex)
                {
                    return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                }
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandRetailReturn.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRetailReturn = await Task.Run(() => mPutPostDocSecondHandRetailReturn(db, dbRead, UO_Action, docSecondHandRetailReturn, EntityState.Added, docSecondHandRetailReturnTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandRetailReturn.DocSecondHandRetailReturnID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandRetailReturn.DocID,
                    DocSecondHandRetailReturnID = docSecondHandRetailReturn.DocSecondHandRetailReturnID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandRetailReturns/5
        [ResponseType(typeof(DocSecondHandRetailReturn))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRetailReturn(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailReturns"));
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
                            from x in dbRead.DocSecondHandRetailReturns
                            where x.DocSecondHandRetailReturnID == id
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
                //1. Rem2Parties
                //2. DocSecondHandRetailReturnTabs
                //3. DocSecondHandRetailReturns
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocSecondHandRetailReturn docSecondHandRetailReturn = await db.DocSecondHandRetailReturns.FindAsync(id);
                if (docSecondHandRetailReturn == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Rem2Parties *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocSecondHandRetailReturns
                                where x.DocSecondHandRetailReturnID == id
                                select x
                            ).ToListAsync();
                        if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                        //1.1. Удаляем "Rem2Parties"
                        var queryRem2Parties = await
                            (
                                from x in db.Rem2Parties
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryRem2Parties.Count(); i++)
                        {
                            Models.Sklad.Rem.Rem2Party rem2Party = await db.Rem2Parties.FindAsync(queryRem2Parties[i].Rem2PartyID);
                            db.Rem2Parties.Remove(rem2Party);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 2. DocSecondHandRetailReturnTabs *** *** *** *** ***

                        var queryDocSecondHandRetailReturnTabs = await
                            (
                                from x in db.DocSecondHandRetailReturnTabs
                                where x.DocSecondHandRetailReturnID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandRetailReturnTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandRetailReturnTab docSecondHandRetailReturnTab = await db.DocSecondHandRetailReturnTabs.FindAsync(queryDocSecondHandRetailReturnTabs[i].DocSecondHandRetailReturnTabID);
                            db.DocSecondHandRetailReturnTabs.Remove(docSecondHandRetailReturnTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocSecondHandRetailReturns *** *** *** *** ***

                        var queryDocSecondHandRetailReturns = await
                            (
                                from x in db.DocSecondHandRetailReturns
                                where x.DocSecondHandRetailReturnID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandRetailReturns.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandRetailReturn docSecondHandRetailReturn1 = await db.DocSecondHandRetailReturns.FindAsync(queryDocSecondHandRetailReturns[i].DocSecondHandRetailReturnID);
                            db.DocSecondHandRetailReturns.Remove(docSecondHandRetailReturn1);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 4. DocSecondHandRetailReturns *** *** *** *** ***

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

        private bool DocSecondHandRetailReturnExists(int id)
        {
            return db.DocSecondHandRetailReturns.Count(e => e.DocSecondHandRetailReturnID == id) > 0;
        }


        internal async Task<DocSecondHandRetailReturn> mPutPostDocSecondHandRetailReturn(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandRetailReturn docSecondHandRetailReturn,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocSecondHandRetailReturnTab[] docSecondHandRetailReturnTabCollection,

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
                doc.NumberInt = docSecondHandRetailReturn.NumberInt;
                doc.NumberReal = docSecondHandRetailReturn.DocSecondHandRetailReturnID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docSecondHandRetailReturn.DirPaymentTypeID;
                doc.Payment = docSecondHandRetailReturn.Payment;
                if (doc.DirContractorID > 0) doc.DirContractorID = docSecondHandRetailReturn.DirContractorID;
                else doc.DirContractorID = docSecondHandRetailReturn.DirContractorIDOrg;
                doc.DirContractorIDOrg = docSecondHandRetailReturn.DirContractorIDOrg;
                doc.Discount = docSecondHandRetailReturn.Discount;
                doc.DirVatValue = docSecondHandRetailReturn.DirVatValue;
                doc.Base = docSecondHandRetailReturn.Base;
                doc.Description = ""; // docSecondHandRetailReturn.Description;
                doc.DocDate = DateTime.Now; //docSecondHandRetailReturn.DocDate;
                //doc.DocDisc = docSecondHandRetailReturn.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docSecondHandRetailReturn.DocID;
                doc.DocIDBase = docSecondHandRetailReturn.DocIDBase;
                doc.KKMSCheckNumber = docSecondHandRetailReturn.KKMSCheckNumber;
                doc.KKMSIdCommand = docSecondHandRetailReturn.KKMSIdCommand;
                doc.KKMSEMail = docSecondHandRetailReturn.KKMSEMail;
                doc.KKMSPhone = docSecondHandRetailReturn.KKMSPhone;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docSecondHandRetailReturn" со всем полями!
                docSecondHandRetailReturn.DocID = doc.DocID;

                #endregion

                #region 2. Находим "DocSecondHandRetailID" (алгоритм работы поменялся, а поле осталось)

                int iRem2PartyMinusID_ = Convert.ToInt32(docSecondHandRetailReturnTabCollection[0].Rem2PartyMinusID);

                var queryDocSecondHandRetailID = await
                    (
                        from x in db.Rem2PartyMinuses
                        where x.Rem2PartyMinusID == iRem2PartyMinusID_
                        select new
                        {
                            NumberReal = x.doc.NumberReal
                        }
                    ).ToListAsync();

                if (queryDocSecondHandRetailID.Count() > 0)
                {
                    docSecondHandRetailReturn.DocSecondHandRetailID = queryDocSecondHandRetailID[0].NumberReal;
                }

                #endregion

                #region 3. DocSecondHandRetailReturn *** *** *** *** *** *** *** *** *** ***

                docSecondHandRetailReturn.DocID = doc.DocID;

                db.Entry(docSecondHandRetailReturn).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docSecondHandRetailReturn.doc.NumberInt == null || docSecondHandRetailReturn.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docSecondHandRetailReturn.DocSecondHandRetailReturnID.ToString();
                    doc.NumberReal = docSecondHandRetailReturn.DocSecondHandRetailReturnID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docSecondHandRetailReturn.DocSecondHandRetailReturnID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 4. Description: пишем ID-шник в DocSecondHandRetailReturnTab и Rem2Party

                string Description = ""; if (docSecondHandRetailReturnTabCollection.Length > 0) Description = docSecondHandRetailReturnTabCollection[0].Description;
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

                #region 5. DocSecondHandRetailReturnTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocSecondHandRetailReturnID = new SQLiteParameter("@DocSecondHandRetailReturnID", System.Data.DbType.Int32) { Value = docSecondHandRetailReturn.DocSecondHandRetailReturnID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocSecondHandRetailReturnTabs WHERE DocSecondHandRetailReturnID=@DocSecondHandRetailReturnID;", parDocSecondHandRetailReturnID);
                }

                //2.2. Проставляем ID-шник "DocSecondHandRetailReturnID" для всех позиций спецификации
                string NomenName = "";
                double dSumTab = 0;
                string sDocSecondHandPurchID = "";
                for (int i = 0; i < docSecondHandRetailReturnTabCollection.Count(); i++)
                {
                    docSecondHandRetailReturnTabCollection[i].DocSecondHandRetailReturnTabID = null;
                    docSecondHandRetailReturnTabCollection[i].DocSecondHandRetailReturnID = Convert.ToInt32(docSecondHandRetailReturn.DocSecondHandRetailReturnID);
                    docSecondHandRetailReturnTabCollection[i].DirDescriptionID = DirDescriptionID;
                    db.Entry(docSecondHandRetailReturnTabCollection[i]).State = EntityState.Added;

                    //Сумма
                    dSumTab += docSecondHandRetailReturnTabCollection[i].Quantity * docSecondHandRetailReturnTabCollection[i].PriceCurrency;
                    //Находим "DocSecondHandPurchID"
                    sDocSecondHandPurchID += docSecondHandRetailReturnTabCollection[i].DocSecondHandPurchID + " "; //NomenName += docSecondHandRetailReturnTabCollection[i].DirServiceNomenID + " ";

                    //Проставляем дату продажи и убираем дату возврата
                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandRetailReturnTabCollection[i].DocSecondHandPurchID);
                    docSecondHandPurch.DateRetail = null;
                    docSecondHandPurch.DateReturn = doc.DocDate; 
                    db.Entry(docSecondHandPurch).State = EntityState.Modified;

                    //Сохраняем
                    await db.SaveChangesAsync();
                }
                //await db.SaveChangesAsync();
                dSumTab = dSumTab - doc.Discount;

                #endregion


                //Models.Sklad.Rem.Rem2Party rem2Party = new Models.Sklad.Rem.Rem2Party();
                if (UO_Action == "held" || docSecondHandRetailReturn.Reserve)
                {
                    //Алгоритм:
                    //По ПартияМинус находим Партию
                    //С неё берём Характеристики и цены
                    //Создаём новую партию

                    //!!! Важно !!!
                    //Ещё должна быть проверка: например, покупатель уже возвращал товар с этой ПартияМинус
                    //То есть в Партии надо иметь Линк на Партию Минус.


                    #region Rem2Party - Партии

                    Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[docSecondHandRetailReturnTabCollection.Count()];
                    for (int i = 0; i < docSecondHandRetailReturnTabCollection.Count(); i++)
                    {
                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = await db.Rem2PartyMinuses.FindAsync(docSecondHandRetailReturnTabCollection[i].Rem2PartyMinusID);


                        #region Проверка: 1 и 2

                        //1. Что бы клиент не вернул товара больше, чем купил
                        if (docSecondHandRetailReturnTabCollection[i].Quantity > rem2PartyMinus.Quantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg108 +
                                //docSecondHandRetailReturnTabCollection[i].DirServiceNomenID + " (" + docSecondHandRetailReturnTabCollection[i].Rem2PartyMinusID + ") - " + iRem2PartyMinusID
                                "<tr>" +
                                "<td>" + docSecondHandRetailReturnTabCollection[i].Rem2PartyMinusID + "</td>" +     //списаная партия
                                "<td>" + docSecondHandRetailReturnTabCollection[i].DirServiceNomenID + "</td>" +     //код товара
                                "<td>" + rem2PartyMinus.Quantity + "</td>" +                //к-во реальное
                                "</tr>" +
                                "</table>"
                                );
                        }
                        //2. Что бы клиент 2-ды не вернул один и тот же товар с продажи
                        int iRem2PartyMinusID = Convert.ToInt32(docSecondHandRetailReturnTabCollection[i].Rem2PartyMinusID);
                        double? dSumQuantity = db.Rem2Parties.Where(x => x.Rem2PartyMinusID == iRem2PartyMinusID).Select(x => x.Quantity).DefaultIfEmpty(0).Sum();
                        if (dSumQuantity >= rem2PartyMinus.Quantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg109 +
                                "<tr>" +
                                "<td>" + docSecondHandRetailReturnTabCollection[i].Rem2PartyMinusID + "</td>" +     //списаная партия
                                "<td>" + docSecondHandRetailReturnTabCollection[i].DirServiceNomenID + "</td>" +     //код товара
                                "</tr>" +
                                "</table>"
                                );
                        }

                        #endregion


                        #region DocIDFirst

                        int? DocIDFirst = rem2PartyMinus.rem2Party.DocIDFirst;

                        #endregion


                        #region Партия

                        Models.Sklad.Rem.Rem2Party rem2Party = new Models.Sklad.Rem.Rem2Party();

                        rem2Party.Rem2PartyID = null;
                        rem2Party.DirServiceNomenID = rem2PartyMinus.rem2Party.DirServiceNomenID;
                        rem2Party.Quantity = docSecondHandRetailReturnTabCollection[i].Quantity;
                        rem2Party.Remnant = docSecondHandRetailReturnTabCollection[i].Quantity;
                        rem2Party.DirCurrencyID = rem2PartyMinus.rem2Party.DirCurrencyID;
                        //rem2Party.DirCurrencyMultiplicity = docSecondHandRetailReturnTabCollection[i].DirCurrencyMultiplicity;
                        //rem2Party.DirCurrencyRate = docSecondHandRetailReturnTabCollection[i].DirCurrencyRate;
                        rem2Party.DirVatValue = rem2PartyMinus.rem2Party.DirVatValue;
                        rem2Party.DirWarehouseID = rem2PartyMinus.rem2Party.DirWarehouseID;
                        rem2Party.DirWarehouseIDDebit = rem2PartyMinus.rem2Party.DirWarehouseIDDebit;
                        rem2Party.DirWarehouseIDPurch = rem2PartyMinus.rem2Party.DirWarehouseIDPurch;
                        rem2Party.DirContractorIDOrg = rem2PartyMinus.rem2Party.DirContractorIDOrg;
                        rem2Party.DirServiceContractorID = rem2PartyMinus.rem2Party.DirServiceContractorID;

                        //Дата Приёмки товара
                        rem2Party.DocDatePurches = rem2PartyMinus.rem2Party.DocDatePurches;

                        /*
                        rem2Party.DirCharColourID = rem2PartyMinus.rem2Party.DirCharColourID; //docSecondHandRetailReturnTabCollection[i].DirCharColourID;
                        rem2Party.DirCharMaterialID = rem2PartyMinus.rem2Party.DirCharMaterialID;
                        rem2Party.DirCharNameID = rem2PartyMinus.rem2Party.DirCharNameID;
                        rem2Party.DirCharSeasonID = rem2PartyMinus.rem2Party.DirCharSeasonID;
                        rem2Party.DirCharSexID = rem2PartyMinus.rem2Party.DirCharSexID;
                        rem2Party.DirCharSizeID = rem2PartyMinus.rem2Party.DirCharSizeID;
                        rem2Party.DirCharStyleID = rem2PartyMinus.rem2Party.DirCharStyleID;
                        rem2Party.DirCharTextureID = rem2PartyMinus.rem2Party.DirCharTextureID;
                        */

                        rem2Party.SerialNumber = rem2PartyMinus.rem2Party.SerialNumber;
                        rem2Party.Barcode = rem2PartyMinus.rem2Party.Barcode;

                        rem2Party.DocID = Convert.ToInt32(docSecondHandRetailReturn.DocID);
                        rem2Party.PriceCurrency = rem2PartyMinus.rem2Party.PriceCurrency;
                        rem2Party.PriceVAT = rem2PartyMinus.rem2Party.PriceVAT;
                        rem2Party.FieldID = Convert.ToInt32(docSecondHandRetailReturnTabCollection[i].DocSecondHandRetailReturnTabID);

                        rem2Party.PriceRetailVAT = rem2PartyMinus.rem2Party.PriceRetailVAT;
                        rem2Party.PriceRetailCurrency = rem2PartyMinus.rem2Party.PriceRetailCurrency;
                        rem2Party.PriceWholesaleVAT = rem2PartyMinus.rem2Party.PriceWholesaleVAT;
                        rem2Party.PriceWholesaleCurrency = rem2PartyMinus.rem2Party.PriceWholesaleCurrency;
                        rem2Party.PriceIMVAT = rem2PartyMinus.rem2Party.PriceIMVAT;
                        rem2Party.PriceIMCurrency = rem2PartyMinus.rem2Party.PriceIMCurrency;

                        rem2Party.Rem2PartyMinusID = rem2PartyMinus.Rem2PartyMinusID;

                        rem2Party.DirReturnTypeID = docSecondHandRetailReturnTabCollection[i].DirReturnTypeID;
                        rem2Party.DirDescriptionID = DirDescriptionID;

                        //DirServiceNomenMinimumBalance
                        rem2Party.DirNomenMinimumBalance = sysSetting.DirNomenMinimumBalance;

                        rem2Party.DirEmployeeID = doc.DirEmployeeID;
                        rem2Party.DocDate = doc.DocDate;


                        //Документ создания первой партии (создания документа)
                        //Нужен для правильно подсчёта партии
                        rem2Party.DocIDFirst = DocIDFirst;


                        rem2PartyCollection[i] = rem2Party;

                        #endregion
                    }

                    Controllers.Sklad.Rem.Rem2PartiesController rem2Partys = new Rem.Rem2PartiesController();
                    await Task.Run(() => rem2Partys.Save(db, rem2PartyCollection)); //rem2Partys.Save(db, rem2PartyCollection);

                    #endregion


                    #region Касса или Банк


                    #region 1. Получаем валюту из склада

                    int DirCurrencyID = 0, DirCurrencyMultiplicity = 0; //, DirCashOfficeID = 0, DirBankID = 0;;
                    double DirCurrencyRate = 0;

                    var query = await Task.Run(() =>
                        (
                            from x in db.DirWarehouses
                            where x.DirWarehouseID == docSecondHandRetailReturn.DirWarehouseID
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
                    pay.DocXID = docSecondHandRetailReturn.DocSecondHandRetailReturnID;
                    pay.DocXSumDate = doc.DocDate;
                    pay.DocXSumSum = dSumTab; // - получили при сохранении Спецификации (выше)
                    pay.Base = "Возврат документа №" + sDocSecondHandPurchID;  //"Возврат за коды товаров: " + NomenName; // - получили при сохранении Спецификации (выше)
                    pay.KKMSCheckNumber = docSecondHandRetailReturn.KKMSCheckNumber;
                    pay.KKMSIdCommand = docSecondHandRetailReturn.KKMSIdCommand;
                    pay.KKMSEMail = docSecondHandRetailReturn.KKMSEMail;
                    pay.KKMSPhone = docSecondHandRetailReturn.KKMSPhone;
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


                //int DocSecondHandRetailReturnID = Convert.ToInt32(docSecondHandRetailReturn.DocSecondHandRetailReturnID);


                //Алгоритм №1
                //SELECT DocID
                //FROM Rem2PartyMinuses 
                //WHERE Rem2PartyID in (SELECT Rem2PartyID FROM Rem2Parties WHERE DocID=@DocID)



                #region Алгоритм №1 (OLD)

                /*
                //Получаем DocSecondHandRetailReturn из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocSecondHandRetailReturn _docSecondHandRetailReturn = db.DocSecondHandRetailReturns.Find(DocSecondHandRetailReturnID);
                int? iDocSecondHandRetailReturn_DocID = _docSecondHandRetailReturn.DocID;


                var queryRem2PartyMinuses =
                    (
                        from rem2PartyMinuses in db.Rem2PartyMinuses

                        join rem2Parties1 in db.Rem2Parties on rem2PartyMinuses.Rem2PartyID equals rem2Parties1.Rem2PartyID into rem2Parties2
                        from rem2Parties in rem2Parties2.Where(x => x.DocID == iDocSecondHandRetailReturn_DocID) //.DefaultIfEmpty()

                        select new
                        {
                            DocID = rem2PartyMinuses.DocID,
                            ListObjectNameRu = rem2PartyMinuses.doc.listObject.ListObjectNameRu
                        }
                    ).Distinct().ToList(); // - убрать повторяющиеся

                //Есть списания!
                if (queryRem2PartyMinuses.Count() > 0)
                {
                    //Поиск всех DocID
                    string arrDocID = "";
                    for (int i = 0; i < queryRem2PartyMinuses.Count(); i++)
                    {
                        arrDocID += queryRem2PartyMinuses[i].DocID + " (" + queryRem2PartyMinuses[i].ListObjectNameRu + ")";
                        if (i != queryRem2PartyMinuses.Count() - 1) arrDocID += "<br />";
                    }
                    //Сообщение клиенту
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg102 + arrDocID + Classes.Language.Sklad.Language.msg102_1);
                }
                */

                #endregion



                //Этот 2-й алгоритм глючит! Если поле "Remnant==Quantity", а списания были (глюкануло что-то), то выдаст сообщение, что "Связи между таблицами нарушены!"

                //Алгоритм №2
                //Пробегаемся по всем "Rem2Parties.Remnant"
                //и есть оно отличается от "Rem2Parties.Quantity"
                //то был списан товар

                /*

                //Получаем DocSecondHandRetailReturn из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocSecondHandRetailReturn _docSecondHandRetailReturn = db.DocSecondHandRetailReturns.Find(DocSecondHandRetailReturnID);
                int? iDocSecondHandRetailReturn_DocID = _docSecondHandRetailReturn.DocID;

                //Есть ли списанный товар с данного прихода
                var queryRem2Parties = await Task.Run(() =>
                    (
                        from x in db.Rem2Parties
                        where x.DocID == iDocSecondHandRetailReturn_DocID && x.Quantity != x.Remnant
                        select x
                    ).ToListAsync());

                //Есть!
                if (queryRem2Parties.Count() > 0)
                {
                    //Смотрим, какие именно накладные списали товар.
                    var queryRem2PartyMinuses = await Task.Run(() =>
                        (
                            from rem2PartyMinuses in db.Rem2PartyMinuses

                            join rem2Parties1 in db.Rem2Parties on rem2PartyMinuses.Rem2PartyID equals rem2Parties1.Rem2PartyID into rem2Parties2
                            from rem2Parties in rem2Parties2.Where(x => x.DocID == iDocSecondHandRetailReturn_DocID) //.DefaultIfEmpty()

                            select new
                            {
                                DocID = rem2PartyMinuses.DocID,
                                ListObjectNameRu = rem2PartyMinuses.doc.listObject.ListObjectNameRu
                            }
                        ).Distinct().ToListAsync()); // - убрать повторяющиеся

                    //Есть списания!
                    if (queryRem2PartyMinuses.Count() > 0)
                    {
                        //Поиск всех DocID
                        string arrDocID = "";
                        for (int i = 0; i < queryRem2PartyMinuses.Count(); i++)
                        {
                            arrDocID += queryRem2PartyMinuses[i].DocID + " (" + queryRem2PartyMinuses[i].ListObjectNameRu + ")";
                            if (i != queryRem2PartyMinuses.Count() - 1) arrDocID += "<br />";
                        }
                        //Сообщение клиенту
                        throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg102 + arrDocID + Classes.Language.Sklad.Language.msg102_1);
                    }

                }

                */

                int DocSecondHandRetailReturnID = Convert.ToInt32(docSecondHandRetailReturn.DocSecondHandRetailReturnID);

                Models.Sklad.Doc.DocSecondHandRetailReturn _docSecondHandRetailReturn = db.DocSecondHandRetailReturns.Find(DocSecondHandRetailReturnID);
                int? iDocSecondHandRetailReturn_DocID = _docSecondHandRetailReturn.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocSecondHandRetailReturn_DocID));

                #endregion


                //Проверка и Удаление записей в таблицах: Rem2Parties
                #region 1. Rem2Parties - удаление *** *** *** *** *** *** *** *** *** ***

                //Проверяем если ли расходы (проведённый или НЕ проведенные)


                //Удаляем записи в таблице "Rem2Parties"
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSecondHandRetailReturn.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Rem2Parties WHERE DocID=@DocID; ", parDocID);

                #endregion

                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docSecondHandRetailReturn.DocID);
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


            return docSecondHandRetailReturn;
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
                "[DocSecondHandRetailReturns].[DocSecondHandRetailReturnID] AS [DocSecondHandRetailReturnID], " +
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
                "[DocSecondHandRetailReturns].[NumberTT] AS [NumberTT], " +
                "[DocSecondHandRetailReturns].[NumberTax] AS [NumberTax] " +

                "FROM [DocSecondHandRetailReturns] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandRetailReturns].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocSecondHandRetailReturns].[DirWarehouseID] " +
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