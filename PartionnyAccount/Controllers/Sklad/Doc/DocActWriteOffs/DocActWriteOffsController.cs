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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocActWriteOffs
{
    public class DocActWriteOffsController : ApiController
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

        int ListObjectID = 35;

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
        // GET: api/DocActWriteOffs
        public async Task<IHttpActionResult> GetDocActWriteOffs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActWriteOffs"));
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
                        from docActWriteOffs in db.DocActWriteOffs

                        join docActWriteOffTabs1 in db.DocActWriteOffTabs on docActWriteOffs.DocActWriteOffID equals docActWriteOffTabs1.DocActWriteOffID into docActWriteOffTabs2
                        from docActWriteOffTabs in docActWriteOffTabs2.DefaultIfEmpty()

                        where docActWriteOffs.doc.DocDate >= _params.DateS && docActWriteOffs.doc.DocDate <= _params.DatePo

                        group new { docActWriteOffTabs }
                        by new
                        {
                            DocID = docActWriteOffs.DocID,
                            DocDate = docActWriteOffs.doc.DocDate,
                            Base = docActWriteOffs.doc.Base,
                            Held = docActWriteOffs.doc.Held,
                            Discount = docActWriteOffs.doc.Discount,
                            Del = docActWriteOffs.doc.Del,
                            Description = docActWriteOffs.doc.Description,
                            IsImport = docActWriteOffs.doc.IsImport,
                            DirVatValue = docActWriteOffs.doc.DirVatValue,

                            DocActWriteOffID = docActWriteOffs.DocActWriteOffID,
                            //DirContractorName = docActWriteOffs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docActWriteOffs.doc.dirContractorOrg.DirContractorID, DirContractorNameOrg = docActWriteOffs.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docActWriteOffs.dirWarehouse.DirWarehouseID, DirWarehouseName = docActWriteOffs.dirWarehouse.DirWarehouseName,

                            NumberInt = docActWriteOffs.doc.NumberInt,

                            DocDateHeld = docActWriteOffs.doc.DocDateHeld,
                            DocDatePayment = docActWriteOffs.doc.DocDatePayment,
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

                            DocActWriteOffID = g.Key.DocActWriteOffID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg, DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID, DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            DocDateHeld = g.Key.DocDateHeld,
                            DocDatePayment = g.Key.DocDatePayment,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docActWriteOffTabs.Quantity * x.docActWriteOffTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActWriteOffTabs.Quantity * x.docActWriteOffTabs.PriceCurrency), sysSetting.FractionalPartInSum)
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
                        query = query.Where(x => x.DocActWriteOffID == iNumber32);
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
                    query = query.OrderByDescending(x => x.DocActWriteOffID);
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
                    query = query.OrderByDescending(x => x.DocActWriteOffID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocActWriteOffs.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocActWriteOff = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocActWriteOffs/5
        [ResponseType(typeof(DocActWriteOff))]
        public async Task<IHttpActionResult> GetDocActWriteOff(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActWriteOffs"));
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
                        from docActWriteOffs in db.DocActWriteOffs
                        where docActWriteOffs.DocActWriteOffID == id
                        select docActWriteOffs
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docActWriteOffs in db.DocActWriteOffs

                        join docActWriteOffTabs1 in db.DocActWriteOffTabs on docActWriteOffs.DocActWriteOffID equals docActWriteOffTabs1.DocActWriteOffID into docActWriteOffTabs2
                        from docActWriteOffTabs in docActWriteOffTabs2.DefaultIfEmpty()

                        #endregion

                        where docActWriteOffs.DocActWriteOffID == id

                        #region group

                        group new { docActWriteOffTabs }
                        by new
                        {
                            DocID = docActWriteOffs.DocID, //DocID = docActWriteOffs.doc.DocID,
                            DocIDBase = docActWriteOffs.doc.DocIDBase,
                            DocDate = docActWriteOffs.doc.DocDate,
                            Base = docActWriteOffs.doc.Base,
                            Held = docActWriteOffs.doc.Held,
                            Discount = docActWriteOffs.doc.Discount,
                            Del = docActWriteOffs.doc.Del,
                            IsImport = docActWriteOffs.doc.IsImport,
                            Description = docActWriteOffs.doc.Description,
                            DirVatValue = docActWriteOffs.doc.DirVatValue,
                            //DirPaymentTypeID = docActWriteOffs.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docActWriteOffs.doc.dirPaymentType.DirPaymentTypeName,

                            DocActWriteOffID = docActWriteOffs.DocActWriteOffID,
                            //DirContractorID = docActWriteOffs.doc.DirContractorID,
                            //DirContractorName = docActWriteOffs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docActWriteOffs.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docActWriteOffs.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docActWriteOffs.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docActWriteOffs.dirWarehouse.DirWarehouseName,
                            NumberInt = docActWriteOffs.doc.NumberInt,

                            //Оплата
                            Payment = docActWriteOffs.doc.Payment,

                            //Резерв
                            Reserve = docActWriteOffs.Reserve
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

                            DocActWriteOffID = g.Key.DocActWriteOffID,
                            //DirContractorID = g.Key.DirContractorID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docActWriteOffTabs.Quantity * x.docActWriteOffTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActWriteOffTabs.Quantity * x.docActWriteOffTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docActWriteOffTabs.Quantity * (x.docActWriteOffTabs.PriceCurrency - (x.docActWriteOffTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActWriteOffTabs.Quantity * (x.docActWriteOffTabs.PriceCurrency - (x.docActWriteOffTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docActWriteOffTabs.Quantity * x.docActWriteOffTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActWriteOffTabs.Quantity * x.docActWriteOffTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),

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

        // PUT: api/DocActWriteOffs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocActWriteOff(int id, DocActWriteOff docActWriteOff, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActWriteOffs"));
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
                Models.Sklad.Doc.DocActWriteOffTab[] docActWriteOffTabCollection = null;
                if (!String.IsNullOrEmpty(docActWriteOff.recordsDocActWriteOffTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docActWriteOffTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocActWriteOffTab[]>(docActWriteOff.recordsDocActWriteOffTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docActWriteOff.DocActWriteOffID || docActWriteOff.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docActWriteOff.DocID" из БД, если он отличается от пришедшего от клиента "docActWriteOff.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocActWriteOffs
                        where x.DocActWriteOffID == docActWriteOff.DocActWriteOffID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docActWriteOff.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docActWriteOff.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docActWriteOff.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docActWriteOff = await Task.Run(() => mPutPostDocActWriteOff(db, dbRead, UO_Action, docActWriteOff, EntityState.Modified, docActWriteOffTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docActWriteOff.DocActWriteOffID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docActWriteOff.DocID,
                    DocActWriteOffID = docActWriteOff.DocActWriteOffID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocActWriteOffs
        [ResponseType(typeof(DocActWriteOff))]
        public async Task<IHttpActionResult> PostDocActWriteOff(DocActWriteOff docActWriteOff, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActWriteOffs"));
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
                Models.Sklad.Doc.DocActWriteOffTab[] docActWriteOffTabCollection = null;
                if (!String.IsNullOrEmpty(docActWriteOff.recordsDocActWriteOffTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docActWriteOffTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocActWriteOffTab[]>(docActWriteOff.recordsDocActWriteOffTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docActWriteOff.DocID" из БД, если он отличается от пришедшего от клиента "docActWriteOff.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocActWriteOffs
                        where x.DocActWriteOffID == docActWriteOff.DocActWriteOffID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docActWriteOff.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docActWriteOff.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docActWriteOff = await Task.Run(() => mPutPostDocActWriteOff(db, dbRead, UO_Action, docActWriteOff, EntityState.Added, docActWriteOffTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docActWriteOff.DocActWriteOffID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docActWriteOff.DocID,
                    DocActWriteOffID = docActWriteOff.DocActWriteOffID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocActWriteOffs/5
        [ResponseType(typeof(DocActWriteOff))]
        public async Task<IHttpActionResult> DeleteDocActWriteOff(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActWriteOffs"));
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
                            from x in dbRead.DocActWriteOffs
                            where x.DocActWriteOffID == id
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
                //2. DocActWriteOffTabs
                //3. DocActWriteOffs
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocActWriteOff docActWriteOff = await db.DocActWriteOffs.FindAsync(id);
                if (docActWriteOff == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. RemPartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocActWriteOffs
                                where x.DocActWriteOffID == id
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


                        #region 2. DocActWriteOffTabs *** *** *** *** ***

                        var queryDocActWriteOffTabs = await
                            (
                                from x in db.DocActWriteOffTabs
                                where x.DocActWriteOffID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocActWriteOffTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocActWriteOffTab docActWriteOffTab = await db.DocActWriteOffTabs.FindAsync(queryDocActWriteOffTabs[i].DocActWriteOffTabID);
                            db.DocActWriteOffTabs.Remove(docActWriteOffTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocActWriteOffs *** *** *** *** ***

                        var queryDocActWriteOffs = await
                            (
                                from x in db.DocActWriteOffs
                                where x.DocActWriteOffID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocActWriteOffs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocActWriteOff docActWriteOff1 = await db.DocActWriteOffs.FindAsync(queryDocActWriteOffs[i].DocActWriteOffID);
                            db.DocActWriteOffs.Remove(docActWriteOff1);
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

        private bool DocActWriteOffExists(int id)
        {
            return db.DocActWriteOffs.Count(e => e.DocActWriteOffID == id) > 0;
        }


        internal async Task<DocActWriteOff> mPutPostDocActWriteOff(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocActWriteOff docActWriteOff,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocActWriteOffTab[] docActWriteOffTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "held") docActWriteOff.Reserve = false;
            else docActWriteOff.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docActWriteOff.NumberInt;
                doc.NumberReal = docActWriteOff.DocActWriteOffID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = 1;
                doc.Payment = 0;
                doc.DirContractorID = docActWriteOff.DirContractorIDOrg;
                doc.DirContractorIDOrg = docActWriteOff.DirContractorIDOrg;
                doc.Discount = docActWriteOff.Discount;
                doc.DirVatValue = docActWriteOff.DirVatValue;
                doc.Base = docActWriteOff.Base;
                doc.Description = docActWriteOff.Description;
                doc.DocDate = docActWriteOff.DocDate;
                //doc.DocDisc = docActWriteOff.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docActWriteOff.DocID;
                doc.DocIDBase = docActWriteOff.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docActWriteOff" со всем полями!
                docActWriteOff.DocID = doc.DocID;

                #endregion

                #region 2. DocActWriteOff *** *** *** *** *** *** *** *** *** ***

                docActWriteOff.DocID = doc.DocID;

                db.Entry(docActWriteOff).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docActWriteOff.doc.NumberInt == null || docActWriteOff.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docActWriteOff.DocActWriteOffID.ToString();
                    doc.NumberReal = docActWriteOff.DocActWriteOffID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docActWriteOff.DocActWriteOffID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocActWriteOffTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocActWriteOffID = new SQLiteParameter("@DocActWriteOffID", System.Data.DbType.Int32) { Value = docActWriteOff.DocActWriteOffID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocActWriteOffTabs WHERE DocActWriteOffID=@DocActWriteOffID;", parDocActWriteOffID);
                }

                //2.2. Проставляем ID-шник "DocActWriteOffID" для всех позиций спецификации
                for (int i = 0; i < docActWriteOffTabCollection.Count(); i++)
                {
                    docActWriteOffTabCollection[i].DocActWriteOffTabID = null;
                    docActWriteOffTabCollection[i].DocActWriteOffID = Convert.ToInt32(docActWriteOff.DocActWriteOffID);
                    db.Entry(docActWriteOffTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion

                #region 4. DirNomenHistories *** *** *** *** *** *** *** *** ***

                /*
                //1. проверяем, если есть в Истории ничего не делаем
                for (int i = 0; i < docActWriteOffTabCollection.Count(); i++)
                {
                    Models.Sklad.Dir.DirNomenHistory dirNomenHistory = new Models.Sklad.Dir.DirNomenHistory();
                    dirNomenHistory.DirNomenHistoryID = null;


                    int DirNomenID = docActWriteOffTabCollection[i].DirNomenID;
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

                    dirNomenHistory.DirCurrencyID = docActWriteOffTabCollection[i].DirCurrencyID;
                    dirNomenHistory.DirNomenID = docActWriteOffTabCollection[i].DirNomenID;
                    dirNomenHistory.PriceVAT = docActWriteOffTabCollection[i].PriceVAT;
                    dirNomenHistory.PriceRetailVAT = docActWriteOffTabCollection[i].PriceRetailVAT;
                    dirNomenHistory.MarkupRetail = docActWriteOffTabCollection[i].MarkupRetail;
                    dirNomenHistory.PriceWholeActWriteOffVAT = docActWriteOffTabCollection[i].PriceWholeActWriteOffVAT;
                    dirNomenHistory.MarkupWholeActWriteOff = docActWriteOffTabCollection[i].MarkupWholeActWriteOff;
                    dirNomenHistory.PriceIMVAT = docActWriteOffTabCollection[i].PriceIMVAT;
                    dirNomenHistory.MarkupIM = docActWriteOffTabCollection[i].MarkupIM;
                    dirNomenHistory.HistoryDate = Convert.ToDateTime(docActWriteOff.DocDate);

                    db.Configuration.AutoDetectChangesEnabled = false;

                    db.Entry(dirNomenHistory).State = entityState2;
                    await db.SaveChangesAsync();
                }
                */

                #endregion


                if (UO_Action == "held" || docActWriteOff.Reserve)
                {
                    Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();


                    #region Удаляем все записи из таблицы "RemPartyMinuses"
                    //Удаляем все записи из таблицы "RemPartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (RemPartyMinuses)

                    for (int i = 0; i < docActWriteOffTabCollection.Count(); i++)
                    {
                        #region Проверка

                        //Переменные
                        int iRemPartyID = docActWriteOffTabCollection[i].RemPartyID;
                        double dQuantity = docActWriteOffTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(iRemPartyID);
                        db.Entry(remParty).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (remParty.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docActWriteOffTabCollection[i].RemPartyID + "</td>" +                                //партия
                                "<td>" + docActWriteOffTabCollection[i].DirNomenID + "</td>" +                                //Код товара
                                "<td>" + docActWriteOffTabCollection[i].Quantity + "</td>" +                                  //списуемое к-во
                                "<td>" + remParty.Remnant + "</td>" +                                                  //остаток партии
                                "<td>" + (docActWriteOffTabCollection[i].Quantity - remParty.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirWarehouseID != docActWriteOff.DirWarehouseID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docActWriteOff.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docActWriteOff.DirWarehouseID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docActWriteOffTabCollection[i].RemPartyID + "</td>" +           //партия
                                "<td>" + docActWriteOffTabCollection[i].DirNomenID + "</td>" +           //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + remParty.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirContractorIDOrg != docActWriteOff.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docActWriteOff.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docActWriteOff.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docActWriteOffTabCollection[i].RemPartyID + "</td>" +           //партия
                                "<td>" + docActWriteOffTabCollection[i].DirNomenID + "</td>" +           //Код товара
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
                        remPartyMinus.RemPartyID = docActWriteOffTabCollection[i].RemPartyID;
                        remPartyMinus.DirNomenID = docActWriteOffTabCollection[i].DirNomenID;
                        remPartyMinus.Quantity = docActWriteOffTabCollection[i].Quantity;
                        remPartyMinus.DirCurrencyID = docActWriteOffTabCollection[i].DirCurrencyID;
                        remPartyMinus.DirCurrencyMultiplicity = docActWriteOffTabCollection[i].DirCurrencyMultiplicity;
                        remPartyMinus.DirCurrencyRate = docActWriteOffTabCollection[i].DirCurrencyRate;
                        remPartyMinus.DirVatValue = docActWriteOff.DirVatValue;
                        remPartyMinus.DirWarehouseID = docActWriteOff.DirWarehouseID;
                        remPartyMinus.DirContractorIDOrg = docActWriteOff.DirContractorIDOrg;
                        remPartyMinus.DirContractorID = docActWriteOff.DirContractorIDOrg;
                        remPartyMinus.DocID = Convert.ToInt32(docActWriteOff.DocID);
                        remPartyMinus.PriceCurrency = docActWriteOffTabCollection[i].PriceCurrency;
                        remPartyMinus.PriceVAT = docActWriteOffTabCollection[i].PriceVAT;
                        remPartyMinus.FieldID = Convert.ToInt32(docActWriteOffTabCollection[i].DocActWriteOffTabID);
                        remPartyMinus.Reserve = docActWriteOff.Reserve;
                        remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        remPartyMinus.DocDate = doc.DocDate;

                        db.Entry(remPartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion
                    }

                    #endregion
                }
            }
            else if (UO_Action == "held_cancel")
            {
                //Удаление записей в таблицах: RemPartyMinuses
                #region 1. RemPartyMinuses *** *** *** *** *** *** *** *** *** ***
                /*
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docActWriteOff.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemPartyMinuses WHERE DocID=@DocID; ", parDocID);
                */
                #endregion

                //Обновление записей: RemPartyMinuses
                #region 1. RemPartyMinuses и RemParties *** *** *** *** *** *** *** *** *** ***

                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docActWriteOff.DocID };
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE RemPartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion

                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docActWriteOff.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion
            }


            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docActWriteOff;
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
                "[DocActWriteOffs].[DocActWriteOffID] AS [DocActWriteOffID], " +
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
                "[DocActWriteOffs].[Reserve] AS [Reserve] " +

                "FROM [DocActWriteOffs] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocActWriteOffs].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "INNER JOIN [DirWarehouses] AS [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocActWriteOffs].[DirWarehouseID] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion
    }
}