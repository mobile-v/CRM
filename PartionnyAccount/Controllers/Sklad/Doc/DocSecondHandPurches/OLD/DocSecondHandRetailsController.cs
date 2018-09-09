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
    public class DocSecondHandRetailsController : ApiController
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

        int ListObjectID = 66;

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
        // GET: api/DocSecondHandRetails
        public async Task<IHttpActionResult> GetDocSecondHandRetails(HttpRequestMessage request)
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
                        from docSecondHandRetails in db.DocSecondHandRetails

                        join docSecondHandRetailTabs1 in db.DocSecondHandRetailTabs on docSecondHandRetails.DocSecondHandRetailID equals docSecondHandRetailTabs1.DocSecondHandRetailID into docSecondHandRetailTabs2
                        from docSecondHandRetailTabs in docSecondHandRetailTabs2.DefaultIfEmpty()

                        where docSecondHandRetails.doc.DocDate >= _params.DateS && docSecondHandRetails.doc.DocDate <= _params.DatePo

                        group new { docSecondHandRetailTabs }
                        by new
                        {
                            DocID = docSecondHandRetails.DocID,
                            DocDate = docSecondHandRetails.doc.DocDate,
                            Base = docSecondHandRetails.doc.Base,
                            Held = docSecondHandRetails.doc.Held,
                            Discount = docSecondHandRetails.doc.Discount,
                            Del = docSecondHandRetails.doc.Del,
                            Description = docSecondHandRetails.doc.Description,
                            IsImport = docSecondHandRetails.doc.IsImport,
                            DirVatValue = docSecondHandRetails.doc.DirVatValue,

                            DocSecondHandRetailID = docSecondHandRetails.DocSecondHandRetailID,
                            DirContractorID = docSecondHandRetails.doc.dirContractor.DirContractorID,
                            DirContractorName = docSecondHandRetails.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandRetails.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandRetails.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandRetails.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandRetails.dirWarehouse.DirWarehouseName,

                            NumberInt = docSecondHandRetails.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandRetails.doc.Payment,
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

                            DocSecondHandRetailID = g.Key.DocSecondHandRetailID,

                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum)
                            */
                            g.Sum(x => x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment, sysSetting.FractionalPartInSum)
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
                        query = query.Where(x => x.DocSecondHandRetailID == iNumber32);
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
                int dirCount = await Task.Run(() => db.DocSecondHandRetails.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSecondHandRetail = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandRetails/5
        [ResponseType(typeof(DocSecondHandRetail))]
        public async Task<IHttpActionResult> GetDocSecondHandRetail(int id, HttpRequestMessage request)
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
                        from docSecondHandRetails in db.DocSecondHandRetails
                        where docSecondHandRetails.DocSecondHandRetailID == id
                        select docSecondHandRetails
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docSecondHandRetails in db.DocSecondHandRetails

                        join docSecondHandRetailTabs1 in db.DocSecondHandRetailTabs on docSecondHandRetails.DocSecondHandRetailID equals docSecondHandRetailTabs1.DocSecondHandRetailID into docSecondHandRetailTabs2
                        from docSecondHandRetailTabs in docSecondHandRetailTabs2.DefaultIfEmpty()

                        #endregion

                        where docSecondHandRetails.DocSecondHandRetailID == id

                        #region group

                        group new { docSecondHandRetailTabs }
                        by new
                        {
                            DocID = docSecondHandRetails.DocID, //DocID = docSecondHandRetails.doc.DocID,
                            DocIDBase = docSecondHandRetails.doc.DocIDBase,
                            DocDate = docSecondHandRetails.doc.DocDate,
                            DirPaymentTypeID = docSecondHandRetails.doc.DirPaymentTypeID,
                            Base = docSecondHandRetails.doc.Base,
                            Held = docSecondHandRetails.doc.Held,
                            Discount = docSecondHandRetails.doc.Discount,
                            Del = docSecondHandRetails.doc.Del,
                            IsImport = docSecondHandRetails.doc.IsImport,
                            Description = docSecondHandRetails.doc.Description,
                            DirVatValue = docSecondHandRetails.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandRetails.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docSecondHandRetails.doc.dirPaymentType.DirPaymentTypeName,

                            DocSecondHandRetailID = docSecondHandRetails.DocSecondHandRetailID,
                            DirContractorID = docSecondHandRetails.doc.DirContractorID,
                            DirContractorName = docSecondHandRetails.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandRetails.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docSecondHandRetails.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandRetails.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandRetails.dirWarehouse.DirWarehouseName,
                            NumberInt = docSecondHandRetails.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandRetails.doc.Payment,

                            //Резерв
                            Reserve = docSecondHandRetails.Reserve,
                            OnCredit = docSecondHandRetails.OnCredit
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

                            DocSecondHandRetailID = g.Key.DocSecondHandRetailID,

                            DirContractorID = g.Key.DirContractorID == 1 ? 0
                            :
                            g.Key.DirContractorID,
                            //DirContractorName = g.Key.DirContractorName,

                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,

                            Payment = g.Key.Payment,
                            Reserve = g.Key.Reserve,
                            OnCredit = g.Key.OnCredit,

                            //Сумма с НДС
                            SumDocServicePurch1Tabs =
                            /*
                            g.Sum(x => x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandRetailTabs.Quantity * x.docSecondHandRetailTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocSecondHandRetails/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRetail(int id, DocSecondHandRetail docSecondHandRetail, HttpRequestMessage request)
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
                if (docSecondHandRetail.Discount > 0)
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
                Models.Sklad.Doc.DocSecondHandRetailTab[] docSecondHandRetailTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandRetail.recordsDocSecondHandRetailTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandRetailTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandRetailTab[]>(docSecondHandRetail.recordsDocSecondHandRetailTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docSecondHandRetail.DocSecondHandRetailID || docSecondHandRetail.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docSecondHandRetail.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandRetail.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandRetails
                        where x.DocSecondHandRetailID == docSecondHandRetail.DocSecondHandRetailID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandRetail.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docSecondHandRetail.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandRetail.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRetail = await Task.Run(() => mPutPostDocSecondHandRetail(db, dbRead, UO_Action, docSecondHandRetail, EntityState.Modified, docSecondHandRetailTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandRetail.DocSecondHandRetailID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandRetail.DocID,
                    DocSecondHandRetailID = docSecondHandRetail.DocSecondHandRetailID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        //После печати чека, если забыли напечатать, сохранить ID-шнини чека
        //id == DocID
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PuttDocSecondHandRetail(int id, int ServiceTypeRepair, int iTrash, HttpRequestMessage request)
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
                        //Models.Sklad.Doc.DocSecondHandRetail docSecondHandRetail = (Models.Sklad.Doc.DocSecondHandRetail)db.DocSecondHandRetails.Where(x => x.DocID == id);

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
                sysJourDisp.TableFieldID = docSecondHandRetail.DocSecondHandRetailID;
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


        // POST: api/DocSecondHandRetails
        [ResponseType(typeof(DocSecondHandRetail))]
        public async Task<IHttpActionResult> PostDocSecondHandRetail(DocSecondHandRetail docSecondHandRetail, HttpRequestMessage request)
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
                if (docSecondHandRetail.Discount > 0)
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
                Models.Sklad.Doc.DocSecondHandRetailTab[] docSecondHandRetailTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandRetail.recordsDocSecondHandRetailTab) && UO_Action != "held_cancel")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandRetailTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandRetailTab[]>(docSecondHandRetail.recordsDocSecondHandRetailTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docSecondHandRetail.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandRetail.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandRetails
                        where x.DocSecondHandRetailID == docSecondHandRetail.DocSecondHandRetailID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandRetail.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */


                //Проверка "Скидки"
                //1. Получаем сотурдника с правами



                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandRetail.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRetail = await Task.Run(() => mPutPostDocSecondHandRetail(db, dbRead, UO_Action, docSecondHandRetail, EntityState.Added, docSecondHandRetailTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandRetail.DocSecondHandRetailID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandRetail.DocID,
                    DocSecondHandRetailID = docSecondHandRetail.DocSecondHandRetailID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandRetails/5
        [ResponseType(typeof(DocSecondHandRetail))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRetail(int id)
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

                #region Проверки

                try
                {
                    //Документ проведён!<BR>Перед удалением, нужно отменить проводку!
                    var queryHeld = await Task.Run(() =>
                        (
                            from x in dbRead.DocSecondHandRetails
                            where x.DocSecondHandRetailID == id
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
                //2. DocSecondHandRetailTabs
                //3. DocSecondHandRetails
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocSecondHandRetail docSecondHandRetail = await db.DocSecondHandRetails.FindAsync(id);
                if (docSecondHandRetail == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Ищим DocID *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocSecondHandRetails
                                where x.DocSecondHandRetailID == id
                                select x
                            ).ToListAsync();
                        if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                        #endregion


                        #region 1. Ищим в Возврате покупателя, если нет, то удаляем в Rem2PartyMinuses *** *** *** *** ***

                        //1.1. Удаляем "Rem2PartyMinuses"
                        var queryRem2PartyMinuses = await
                            (
                                from x in db.Rem2PartyMinuses
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();

                        
                        for (int i = 0; i < queryRem2PartyMinuses.Count(); i++)
                        {
                            int iRem2PartyMinusID = Convert.ToInt32(queryRem2PartyMinuses[i].Rem2PartyMinusID);

                            /*
                            var queryDocReturnsCustomerTab = await
                                (
                                    from x in db.DocReturnsCustomerTabs
                                    where x.Rem2PartyMinusID == iRem2PartyMinusID
                                    select x
                                ).ToListAsync();

                            if (queryDocReturnsCustomerTab.Count() > 0)
                            {
                                throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg112 +

                                    "<tr>" +
                                    "<td>" + queryDocReturnsCustomerTab[0].Rem2PartyMinusID + "</td>" +            //партия списания
                                    "<td>" + queryDocReturnsCustomerTab[0].DocReturnsCustomerID + "</td>" +       //№ д-та
                                    "<td>" + queryDocReturnsCustomerTab[0].DirServiceNomenID + "</td>" +          //Код товара
                                    "<td>" + queryDocReturnsCustomerTab[0].Quantity + "</td>" +                   //списуемое к-во
                                    "</tr>" +
                                    "</table>" +

                                    Classes.Language.Sklad.Language.msg112_1
                                    );
                            }
                            */

                            Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = await db.Rem2PartyMinuses.FindAsync(iRem2PartyMinusID);
                            db.Rem2PartyMinuses.Remove(rem2PartyMinus);
                            await db.SaveChangesAsync();
                        }
                        

                        #endregion


                        #region 2. DocSecondHandRetailTabs *** *** *** *** ***

                        var queryDocSecondHandRetailTabs = await
                            (
                                from x in db.DocSecondHandRetailTabs
                                where x.DocSecondHandRetailID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandRetailTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandRetailTab docSecondHandRetailTab = await db.DocSecondHandRetailTabs.FindAsync(queryDocSecondHandRetailTabs[i].DocSecondHandRetailTabID);
                            db.DocSecondHandRetailTabs.Remove(docSecondHandRetailTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocSecondHandRetails *** *** *** *** ***

                        var queryDocSecondHandRetails = await
                            (
                                from x in db.DocSecondHandRetails
                                where x.DocSecondHandRetailID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandRetails.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandRetail docSecondHandRetail1 = await db.DocSecondHandRetails.FindAsync(queryDocSecondHandRetails[i].DocSecondHandRetailID);
                            db.DocSecondHandRetails.Remove(docSecondHandRetail1);
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

        private bool DocSecondHandRetailExists(int id)
        {
            return db.DocSecondHandRetails.Count(e => e.DocSecondHandRetailID == id) > 0;
        }


        internal async Task<DocSecondHandRetail> mPutPostDocSecondHandRetail(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandRetail docSecondHandRetail,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocSecondHandRetailTab[] docSecondHandRetailTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "held") docSecondHandRetail.Reserve = false;
            else docSecondHandRetail.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docSecondHandRetail.NumberInt;
                doc.NumberReal = docSecondHandRetail.DocSecondHandRetailID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docSecondHandRetail.DirPaymentTypeID;
                doc.Payment = docSecondHandRetail.Payment;
                if (docSecondHandRetail.DirContractorID != null) doc.DirContractorID = Convert.ToInt32(docSecondHandRetail.DirContractorID); else doc.DirContractorID = docSecondHandRetail.DirContractorIDOrg;
                doc.DirContractorIDOrg = docSecondHandRetail.DirContractorIDOrg;
                doc.Discount = docSecondHandRetail.Discount;
                doc.DirVatValue = docSecondHandRetail.DirVatValue;
                doc.Base = docSecondHandRetail.Base;
                doc.Description = docSecondHandRetail.Description;
                doc.DocDate = DateTime.Now; //docSecondHandRetail.DocDate;
                //doc.DocDisc = docSecondHandRetail.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docSecondHandRetail.DocID;
                doc.DocIDBase = docSecondHandRetail.DocIDBase;
                doc.KKMSCheckNumber = docSecondHandRetail.KKMSCheckNumber;
                doc.KKMSIdCommand = docSecondHandRetail.KKMSIdCommand;
                doc.KKMSEMail = docSecondHandRetail.KKMSEMail;
                doc.KKMSPhone = docSecondHandRetail.KKMSPhone;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docSecondHandRetail" со всем полями!
                docSecondHandRetail.DocID = doc.DocID;

                #endregion

                #region 2. DocSecondHandRetail

                docSecondHandRetail.DocID = doc.DocID;

                db.Entry(docSecondHandRetail).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT

                if (entityState == EntityState.Added && (docSecondHandRetail.doc.NumberInt == null || docSecondHandRetail.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docSecondHandRetail.DocSecondHandRetailID.ToString();
                    doc.NumberReal = docSecondHandRetail.DocSecondHandRetailID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docSecondHandRetail.DocSecondHandRetailID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocSecondHandRetailTab

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocSecondHandRetailID = new SQLiteParameter("@DocSecondHandRetailID", System.Data.DbType.Int32) { Value = docSecondHandRetail.DocSecondHandRetailID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocSecondHandRetailTabs WHERE DocSecondHandRetailID=@DocSecondHandRetailID;", parDocSecondHandRetailID);
                }

                //2.2. Проставляем ID-шник "DocSecondHandRetailID" для всех позиций спецификации
                double dSumTab = 0;
                string sDocSecondHandPurchID = "";
                for (int i = 0; i < docSecondHandRetailTabCollection.Count(); i++)
                {
                    docSecondHandRetailTabCollection[i].DocSecondHandRetailTabID = null;
                    docSecondHandRetailTabCollection[i].DocSecondHandRetailID = Convert.ToInt32(docSecondHandRetail.DocSecondHandRetailID);
                    db.Entry(docSecondHandRetailTabCollection[i]).State = EntityState.Added;

                    //Сумма
                    dSumTab += docSecondHandRetailTabCollection[i].Quantity * docSecondHandRetailTabCollection[i].PriceCurrency;
                    //Находим "DocSecondHandPurchID"
                    sDocSecondHandPurchID += docSecondHandRetailTabCollection[i].DocSecondHandPurchID + " ";

                    //Проставляем дату продажи и убираем дату возврата
                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandRetailTabCollection[i].DocSecondHandPurchID);
                    docSecondHandPurch.DateRetail = doc.DocDate;
                    docSecondHandPurch.DateReturn = null;
                    db.Entry(docSecondHandPurch).State = EntityState.Modified;

                    //Сохраняем
                    await db.SaveChangesAsync();
                }
                dSumTab = dSumTab - doc.Discount;

                #endregion


                if (UO_Action == "held" || docSecondHandRetail.Reserve)
                {
                    Controllers.Sklad.Rem.Rem2PartyMinusesController rem2PartyMinuses = new Rem.Rem2PartyMinusesController();

                    #region 1. Ищим в Возврате покупателя, если нет, то удаляем в Rem2PartyMinuses *** *** *** *** ***

                    //1.1. Удаляем "Rem2PartyMinuses"
                    var queryRem2PartyMinuses = await
                        (
                            from x in db.Rem2PartyMinuses
                            where x.DocID == docSecondHandRetail.DocID
                            select x
                        ).ToListAsync();

                    for (int i = 0; i < queryRem2PartyMinuses.Count(); i++)
                    {
                        int iRem2PartyMinusID = Convert.ToInt32(queryRem2PartyMinuses[i].Rem2PartyMinusID);

                        /*
                        var queryDocSecondHandRetailReturnTab = await
                            (
                                from x in db.DocSecondHandRetailReturnTabs
                                where x.Rem2PartyMinusID == iRem2PartyMinusID
                                select x
                            ).ToListAsync();

                        if (queryDocSecondHandRetailReturnTab.Count() > 0)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg117 +

                                "<tr>" +
                                "<td>" + queryDocSecondHandRetailReturnTab[0].Rem2PartyMinusID + "</td>" +          //партия списания
                                "<td>" + queryDocSecondHandRetailReturnTab[0].DocSecondHandRetailReturnID + "</td>" +        //№ д-та
                                "<td>" + queryDocSecondHandRetailReturnTab[0].DirServiceNomenID + "</td>" +               //Код товара
                                "<td>" + queryDocSecondHandRetailReturnTab[0].Quantity + "</td>" +                 //списуемое к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg117_1
                                );
                        }
                        */

                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = await db.Rem2PartyMinuses.FindAsync(iRem2PartyMinusID);
                        db.Rem2PartyMinuses.Remove(rem2PartyMinus);
                        await db.SaveChangesAsync();
                    }

                    #endregion

                    #region Удаляем все записи из таблицы "Rem2PartyMinuses"
                    //Удаляем все записи из таблицы "Rem2PartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => rem2PartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (Rem2PartyMinuses)

                    string NomenName = "";

                    for (int i = 0; i < docSecondHandRetailTabCollection.Count(); i++)
                    {
                        #region Проверка

                        //Переменные
                        int iRem2PartyID = Convert.ToInt32(docSecondHandRetailTabCollection[i].Rem2PartyID);
                        double dQuantity = docSecondHandRetailTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.Rem2Party rem2Party = await db.Rem2Parties.FindAsync(iRem2PartyID);
                        if (rem2Party == null)
                        {
                            throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg116_1 + iRem2PartyID + Classes.Language.Sklad.Language.msg116_2);
                        }
                        db.Entry(rem2Party).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (rem2Party.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docSecondHandRetailTabCollection[i].Rem2PartyID + "</td>" +                                //партия
                                "<td>" + docSecondHandRetailTabCollection[i].DirServiceNomenID + "</td>" +                                //Код товара
                                "<td>" + docSecondHandRetailTabCollection[i].Quantity + "</td>" +                                  //списуемое к-во
                                "<td>" + rem2Party.Remnant + "</td>" +                                                  //остаток партии
                                "<td>" + (docSecondHandRetailTabCollection[i].Quantity - rem2Party.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (rem2Party.DirWarehouseID != docSecondHandRetail.DirWarehouseID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandRetail.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandRetail.DirWarehouseID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docSecondHandRetailTabCollection[i].Rem2PartyID + "</td>" +           //партия
                                "<td>" + docSecondHandRetailTabCollection[i].DirServiceNomenID + "</td>" +           //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + rem2Party.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (rem2Party.DirContractorIDOrg != docSecondHandRetail.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandRetail.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docSecondHandRetail.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docSecondHandRetailTabCollection[i].Rem2PartyID + "</td>" +           //партия
                                "<td>" + docSecondHandRetailTabCollection[i].DirServiceNomenID + "</td>" +           //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                                "<td>" + rem2Party.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg106_1
                            );
                        }
                        #endregion

                        #region 4. Что бы продажная цена не была меньше закупочной
                        if (rem2Party.PriceCurrency >= docSecondHandRetailTabCollection[i].PriceCurrency - docSecondHandRetail.Discount)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg135 +
                                Classes.Language.Sklad.Language.msg135_1
                            );
                        }
                        #endregion

                        #endregion


                        #region Сохранение

                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                        rem2PartyMinus.Rem2PartyMinusID = null;
                        rem2PartyMinus.Rem2PartyID = Convert.ToInt32(docSecondHandRetailTabCollection[i].Rem2PartyID);

                        rem2PartyMinus.DirServiceNomenID = docSecondHandRetailTabCollection[i].DirServiceNomenID;
                        /*
                        rem2PartyMinus.DirCharColourID = rem2Party.DirCharColourID;
                        rem2PartyMinus.DirCharMaterialID = rem2Party.DirCharMaterialID;
                        rem2PartyMinus.DirCharNameID = rem2Party.DirCharNameID;
                        rem2PartyMinus.DirCharSeasonID = rem2Party.DirCharSeasonID;
                        rem2PartyMinus.DirCharSexID = rem2Party.DirCharSexID;
                        rem2PartyMinus.DirCharSizeID = rem2Party.DirCharSizeID;
                        rem2PartyMinus.DirCharStyleID = rem2Party.DirCharStyleID;
                        rem2PartyMinus.DirCharTextureID = rem2Party.DirCharTextureID;
                        rem2Party.SerialNumber = rem2Party.SerialNumber;
                        rem2Party.Barcode = rem2Party.Barcode;
                        */
                        rem2PartyMinus.Quantity = docSecondHandRetailTabCollection[i].Quantity;
                        rem2PartyMinus.DirCurrencyID = docSecondHandRetailTabCollection[i].DirCurrencyID;
                        rem2PartyMinus.DirCurrencyMultiplicity = docSecondHandRetailTabCollection[i].DirCurrencyMultiplicity;
                        rem2PartyMinus.DirCurrencyRate = docSecondHandRetailTabCollection[i].DirCurrencyRate;
                        rem2PartyMinus.DirVatValue = docSecondHandRetail.DirVatValue;
                        rem2PartyMinus.DirWarehouseID = docSecondHandRetail.DirWarehouseID;
                        rem2PartyMinus.DirContractorIDOrg = docSecondHandRetail.DirContractorIDOrg;


                        //!!!
                        /*
                        //rem2PartyMinus.DirServiceContractorID = docSecondHandRetail.DirServiceContractorID;
                        if (docSecondHandRetail.DirContractorID != null) rem2PartyMinus.DirContractorID = Convert.ToInt32(docSecondHandRetail.DirContractorID);
                        else rem2PartyMinus.DirContractorID = docSecondHandRetail.DirContractorIDOrg;
                        */
                        rem2PartyMinus.DirServiceContractorID = rem2Party.DirServiceContractorID;


                        rem2PartyMinus.DocID = Convert.ToInt32(docSecondHandRetail.DocID);
                        rem2PartyMinus.PriceCurrency = docSecondHandRetailTabCollection[i].PriceCurrency;
                        rem2PartyMinus.PriceVAT = docSecondHandRetailTabCollection[i].PriceVAT;
                        rem2PartyMinus.FieldID = Convert.ToInt32(docSecondHandRetailTabCollection[i].DocSecondHandRetailTabID);
                        rem2PartyMinus.Reserve = docSecondHandRetail.Reserve;

                        rem2PartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        rem2PartyMinus.DocDate = doc.DocDate;

                        db.Entry(rem2PartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion

                        NomenName += docSecondHandRetailTabCollection[i].DirServiceNomenID + " ";
                    }

                    #endregion


                    #region Касса или Банк


                    #region 1. Получаем валюту из склада

                    int DirCurrencyID = 0, DirCurrencyMultiplicity = 0; //, DirCashOfficeID = 0, DirBankID = 0;;
                    double DirCurrencyRate = 0;

                    var query = await Task.Run(() =>
                        (
                            from x in db.DirWarehouses
                            where x.DirWarehouseID == docSecondHandRetail.DirWarehouseID
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
                        else if(doc.DirPaymentTypeID == 2)
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
                    pay.DocXID = docSecondHandRetail.DocSecondHandRetailID;
                    pay.DocXSumDate = doc.DocDate;
                    pay.DocXSumSum = dSumTab; // - получили при сохранении Спецификации (выше)

                    //DocSecondHandPurchID - найти!!!
                    pay.Base = "Продажа документа №" + sDocSecondHandPurchID; //pay.Base = "Оплата за коды товаров: " + NomenName; // - получили при сохранении Спецификации (выше)

                    //pay.Description = "";
                    pay.KKMSCheckNumber = docSecondHandRetail.KKMSCheckNumber;
                    pay.KKMSIdCommand = docSecondHandRetail.KKMSIdCommand;
                    pay.KKMSEMail = docSecondHandRetail.KKMSEMail;
                    pay.KKMSPhone = docSecondHandRetail.KKMSPhone;

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
                #region Проверка не вернул ли товар покупатель

                #region RemXXX

                //Если хоть одно поле для "DocSecondHandRetails.DocID" будет "Rem2Parties.Rem2PartyMinusID > 0", то возвращали товар - Эксепшен.
                var query = await
                    (
                        from x in db.Rem2Parties
                        from y in db.Rem2PartyMinuses
                        where y.DocID == docSecondHandRetail.DocID && x.Rem2PartyMinusID == y.Rem2PartyMinusID
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

                        "<td>" + query[i].y.Rem2PartyMinusID + "</td>" +                    //списаная партия
                        "<td>" + query[i].y.DirServiceNomenID + "</td>" +                         //Код товара
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

                #region DocSecondHandRetailXXXTabs

                /*
                //Проверим поле "Rem2PartyMinusID" в таблице "DocSecondHandRetailReturnTabs"
                var queryX = await
                    (
                        from x in db.DocSecondHandRetailReturnTabs
                        from y in db.Rem2PartyMinuses
                        where x.Rem2PartyMinusID == y.Rem2PartyMinusID && y.DocID == docSecondHandRetail.DocID
                        select new
                        {
                            x,
                            y
                        }
                    ).ToListAsync();
                if (queryX.Count() > 0)
                {
                    string sData = "";
                    for (int i = 0; i < queryX.Count(); i++)
                    {
                        sData +=
                        "<tr>" +
                        "<td>" + queryX[i].x.docSecondHandRetailReturn.doc.NumberReal + "</td>" +                     //Номер документа
                        "<td>" + queryX[i].x.docSecondHandRetailReturn.doc.DocID + "</td>" +                          //Общий документа
                        "<td>" + queryX[i].x.docSecondHandRetailReturn.doc.listObject.ListObjectNameRu + "</td>" +    //Тип документа

                        "<td>" + queryX[i].y.Rem2PartyMinusID + "</td>" +                    //списаная партия
                        "<td>" + queryX[i].y.DirServiceNomenID + "</td>" +                         //Код товара
                        "<td>" + queryX[i].x.Quantity + "</td>" +                           //списуемое к-во
                        "</tr>";
                    }

                    throw new System.InvalidOperationException(
                        Classes.Language.Sklad.Language.msg110 +
                        sData +
                        "</table>" +
                        Classes.Language.Sklad.Language.msg110_1
                    );
                }
                */

                #endregion

                #endregion

                //Удаление записей в таблицах: Rem2PartyMinuses
                #region 1. Rem2PartyMinuses *** *** *** *** *** *** *** *** *** ***
                /*
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSecondHandRetail.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Rem2PartyMinuses WHERE DocID=@DocID; ", parDocID);
                */
                #endregion

                //Обновление записей: Rem2PartyMinuses
                #region 1. Rem2PartyMinuses и Rem2Parties *** *** *** *** *** *** *** *** *** ***

                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSecondHandRetail.DocID };
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE Rem2PartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion

                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = await Task.Run(() => db.Docs.FindAsync(docSecondHandRetail.DocID));
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
                else if (doc.DirPaymentTypeID == 2)
                {
                    var queryDocCashBankID = await Task.Run(() =>
                       (
                            from x in db.DocBankSums
                            where x.DocID == doc.DocID
                            select x
                        ).ToListAsync());

                    if (queryDocCashBankID.Count() > 0) DocCashBankID = queryDocCashBankID[0].DocBankSumID;
                }
                else
                {
                    throw new System.InvalidOperationException("Не выбран метод оплаты: Касса или Банк!");
                }

                PartionnyAccount.Controllers.Sklad.Pay.PayController payController = new Pay.PayController();
                int? iDocID = await Task.Run(() => payController.mDeletePay(db, Convert.ToInt32(doc.DirPaymentTypeID), Convert.ToInt32(DocCashBankID))); //sysSetting

                #endregion
            }


            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docSecondHandRetail;
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
                "[DocSecondHandRetails].[DocSecondHandRetailID] AS [DocSecondHandRetailID], " +
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
                "[DocSecondHandRetails].[Reserve] AS [Reserve], " +


                //Табличная часть

                "SUM(DocSecondHandRetailTabs.PriceCurrency) AS [PriceVATEstimated], " +
                "SUM(DocSecondHandRetailTabs.PriceCurrency) AS [SumTotal_InWords], " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "CASE " +
                "WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN [dirServiceNomensSubGroup].[DirServiceNomenName] " +

                "WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END ELSE " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN '' ELSE [DirServiceNomens].[DirServiceNomenName] END END AS [DirServiceNomenName], " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "Rem2Parties.SerialNumber AS DeviceSerialNumber " + 



                "FROM [DocSecondHandRetails] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandRetails].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocSecondHandRetails].[DirWarehouseID] " +
                //Банк для Контрагента
                "LEFT JOIN [DirBanks] AS [DirBanks] ON [DirBanks].[DirBankID] = [DirContractors].[DirBankID] " +
                //Банк для Организации
                "LEFT JOIN [DirBanks] AS [DirBanksOrg] ON [DirBanks].[DirBankID] = [DirContractorOrg].[DirBankID] " +


                //Табличная часть
                "LEFT JOIN [DocSecondHandRetailTabs] AS [DocSecondHandRetailTabs] ON DocSecondHandRetailTabs.DocSecondHandRetailID=DocSecondHandRetails.DocSecondHandRetailID " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocSecondHandRetailTabs].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [dirServiceNomensSubGroup].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "INNER JOIN [Rem2Parties] ON [Rem2Parties].[Rem2PartyID] = [DocSecondHandRetailTabs].[Rem2PartyID] " +



                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion

    }
}