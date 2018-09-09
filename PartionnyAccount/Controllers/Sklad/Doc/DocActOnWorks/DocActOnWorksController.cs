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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocActOnWorks
{
    public class DocActOnWorksController : ApiController
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

        int ListObjectID = 37;

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
        // GET: api/DocActOnWorks
        public async Task<IHttpActionResult> GetDocActOnWorks(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActOnWorks"));
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
                        from docActOnWorks in db.DocActOnWorks

                        join docActOnWorkTabs1 in db.DocActOnWorkTabs on docActOnWorks.DocActOnWorkID equals docActOnWorkTabs1.DocActOnWorkID into docActOnWorkTabs2
                        from docActOnWorkTabs in docActOnWorkTabs2.DefaultIfEmpty()

                        where docActOnWorks.doc.DocDate >= _params.DateS && docActOnWorks.doc.DocDate <= _params.DatePo

                        group new { docActOnWorkTabs }
                        by new
                        {
                            DocID = docActOnWorks.DocID,
                            DocDate = docActOnWorks.doc.DocDate,
                            Base = docActOnWorks.doc.Base,
                            Held = docActOnWorks.doc.Held,
                            Discount = docActOnWorks.doc.Discount,
                            Del = docActOnWorks.doc.Del,
                            Description = docActOnWorks.doc.Description,
                            IsImport = docActOnWorks.doc.IsImport,
                            DirVatValue = docActOnWorks.doc.DirVatValue,

                            DocActOnWorkID = docActOnWorks.DocActOnWorkID,
                            DirContractorID = docActOnWorks.doc.dirContractor.DirContractorID,
                            DirContractorName = docActOnWorks.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docActOnWorks.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docActOnWorks.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docActOnWorks.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docActOnWorks.dirWarehouse.DirWarehouseName,

                            NumberInt = docActOnWorks.doc.NumberInt,

                            //Оплата
                            Payment = docActOnWorks.doc.Payment,

                            DocDateHeld = docActOnWorks.doc.DocDateHeld,
                            DocDatePayment = docActOnWorks.doc.DocDatePayment,
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

                            DocActOnWorkID = g.Key.DocActOnWorkID,

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
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * x.docActOnWorkTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * x.docActOnWorkTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * x.docActOnWorkTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * x.docActOnWorkTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum)
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
                        //case 1: query = query.Where(x => x.Held == true); break;
                        //case 2: query = query.Where(x => x.Held == false); break;
                        //case 3: query = query.Where(x => x.IsImport == true); break;

                        case 1: query = query.Where(x => x.HavePay == 0); break;
                        case 2: query = query.Where(x => x.HavePay > 0 && x.Payment > 0); break;
                        case 3: query = query.Where(x => x.Payment == 0); break;
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
                        query = query.Where(x => x.DocActOnWorkID == iNumber32);
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
                    query = query.OrderByDescending(x => x.DocActOnWorkID);
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
                    query = query.OrderByDescending(x => x.DocActOnWorkID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocActOnWorks.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocActOnWork = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocActOnWorks/5
        [ResponseType(typeof(DocActOnWork))]
        public async Task<IHttpActionResult> GetDocActOnWork(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActOnWorks"));
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
                        from docActOnWorks in db.DocActOnWorks
                        where docActOnWorks.DocActOnWorkID == id
                        select docActOnWorks
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docActOnWorks in db.DocActOnWorks

                        join docActOnWorkTabs1 in db.DocActOnWorkTabs on docActOnWorks.DocActOnWorkID equals docActOnWorkTabs1.DocActOnWorkID into docActOnWorkTabs2
                        from docActOnWorkTabs in docActOnWorkTabs2.DefaultIfEmpty()

                        #endregion

                        where docActOnWorks.DocActOnWorkID == id

                        #region group

                        group new { docActOnWorkTabs }
                        by new
                        {
                            DocID = docActOnWorks.DocID, //DocID = docActOnWorks.doc.DocID,
                            DocIDBase = docActOnWorks.doc.DocIDBase,
                            DocDate = docActOnWorks.doc.DocDate,
                            Base = docActOnWorks.doc.Base,
                            Held = docActOnWorks.doc.Held,
                            Discount = docActOnWorks.doc.Discount,
                            Del = docActOnWorks.doc.Del,
                            IsImport = docActOnWorks.doc.IsImport,
                            Description = docActOnWorks.doc.Description,
                            DirVatValue = docActOnWorks.doc.DirVatValue,
                            //DirPaymentTypeID = docActOnWorks.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docActOnWorks.doc.dirPaymentType.DirPaymentTypeName,

                            DocActOnWorkID = docActOnWorks.DocActOnWorkID,
                            DirContractorID = docActOnWorks.doc.DirContractorID,
                            DirContractorName = docActOnWorks.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docActOnWorks.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docActOnWorks.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docActOnWorks.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docActOnWorks.dirWarehouse.DirWarehouseName,
                            NumberInt = docActOnWorks.doc.NumberInt,
     
                            //Оплата
                            Payment = docActOnWorks.doc.Payment,
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

                            DocActOnWorkID = g.Key.DocActOnWorkID,
                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * x.docActOnWorkTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * x.docActOnWorkTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * (x.docActOnWorkTabs.PriceCurrency - (x.docActOnWorkTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * (x.docActOnWorkTabs.PriceCurrency - (x.docActOnWorkTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * x.docActOnWorkTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docActOnWorkTabs.Quantity * x.docActOnWorkTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocActOnWorks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocActOnWork(int id, DocActOnWork docActOnWork, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActOnWorks"));
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
                Models.Sklad.Doc.DocActOnWorkTab[] docActOnWorkTabCollection = null;
                if (!String.IsNullOrEmpty(docActOnWork.recordsDocActOnWorkTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docActOnWorkTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocActOnWorkTab[]>(docActOnWork.recordsDocActOnWorkTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docActOnWork.DocActOnWorkID || docActOnWork.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docActOnWork.DocID" из БД, если он отличается от пришедшего от клиента "docActOnWork.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocActOnWorks
                        where x.DocActOnWorkID == docActOnWork.DocActOnWorkID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docActOnWork.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                /*
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docActOnWork.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));
                */


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docActOnWork.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docActOnWork = await Task.Run(() => mPutPostDocActOnWork(db, dbRead, UO_Action, docActOnWork, EntityState.Modified, docActOnWorkTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docActOnWork.DocActOnWorkID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docActOnWork.DocID,
                    DocActOnWorkID = docActOnWork.DocActOnWorkID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocActOnWorks
        [ResponseType(typeof(DocActOnWork))]
        public async Task<IHttpActionResult> PostDocActOnWork(DocActOnWork docActOnWork, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActOnWorks"));
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
                Models.Sklad.Doc.DocActOnWorkTab[] docActOnWorkTabCollection = null;
                if (!String.IsNullOrEmpty(docActOnWork.recordsDocActOnWorkTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docActOnWorkTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocActOnWorkTab[]>(docActOnWork.recordsDocActOnWorkTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docActOnWork.DocID" из БД, если он отличается от пришедшего от клиента "docActOnWork.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocActOnWorks
                        where x.DocActOnWorkID == docActOnWork.DocActOnWorkID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docActOnWork.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docActOnWork.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docActOnWork = await Task.Run(() => mPutPostDocActOnWork(db, dbRead, UO_Action, docActOnWork, EntityState.Added, docActOnWorkTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docActOnWork.DocActOnWorkID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docActOnWork.DocID,
                    DocActOnWorkID = docActOnWork.DocActOnWorkID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocActOnWorks/5
        [ResponseType(typeof(DocActOnWork))]
        public async Task<IHttpActionResult> DeleteDocActOnWork(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActOnWorks"));
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
                            from x in dbRead.DocActOnWorks
                            where x.DocActOnWorkID == id
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
                //2. DocActOnWorkTabs
                //3. DocActOnWorks
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocActOnWork docActOnWork = await db.DocActOnWorks.FindAsync(id);
                if (docActOnWork == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. RemPartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocActOnWorks
                                where x.DocActOnWorkID == id
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


                        #region 2. DocActOnWorkTabs *** *** *** *** ***

                        var queryDocActOnWorkTabs = await
                            (
                                from x in db.DocActOnWorkTabs
                                where x.DocActOnWorkID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocActOnWorkTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocActOnWorkTab docActOnWorkTab = await db.DocActOnWorkTabs.FindAsync(queryDocActOnWorkTabs[i].DocActOnWorkTabID);
                            db.DocActOnWorkTabs.Remove(docActOnWorkTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocActOnWorks *** *** *** *** ***

                        var queryDocActOnWorks = await
                            (
                                from x in db.DocActOnWorks
                                where x.DocActOnWorkID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocActOnWorks.Count(); i++)
                        {
                            Models.Sklad.Doc.DocActOnWork docActOnWork1 = await db.DocActOnWorks.FindAsync(queryDocActOnWorks[i].DocActOnWorkID);
                            db.DocActOnWorks.Remove(docActOnWork1);
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

        private bool DocActOnWorkExists(int id)
        {
            return db.DocActOnWorks.Count(e => e.DocActOnWorkID == id) > 0;
        }


        internal async Task<DocActOnWork> mPutPostDocActOnWork(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocActOnWork docActOnWork,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocActOnWorkTab[] docActOnWorkTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region 1. Doc *** *** *** *** *** *** *** *** *** ***

            //Модель
            Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
            //Присваиваем значения
            doc.ListObjectID = ListObjectID;
            doc.IsImport = false;
            doc.NumberInt = docActOnWork.NumberInt;
            doc.NumberReal = docActOnWork.DocActOnWorkID;
            doc.DirEmployeeID = field.DirEmployeeID;
            //doc.DirPaymentTypeID = docActOnWork.DirPaymentTypeID;
            doc.Payment = docActOnWork.Payment;
            doc.DirContractorID = docActOnWork.DirContractorID;
            doc.DirContractorIDOrg = docActOnWork.DirContractorIDOrg;
            doc.Discount = docActOnWork.Discount;
            doc.DirVatValue = docActOnWork.DirVatValue;
            doc.Base = docActOnWork.Base;
            doc.Description = docActOnWork.Description;
            doc.DocDate = docActOnWork.DocDate;
            //doc.Discount = docActOnWork.Discount;
            if (UO_Action == "held") doc.Held = true;
            else doc.Held = false;
            doc.DocID = docActOnWork.DocID;
            doc.DocIDBase = docActOnWork.DocIDBase;

            //Класс
            Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
            //doc = await docs.Save();
            await Task.Run(() => docs.Save());

            //Нужно вернуть "docActOnWork" со всем полями!
            docActOnWork.DocID = doc.DocID;

            #endregion

            #region 2. DocActOnWork *** *** *** *** *** *** *** *** *** ***

            docActOnWork.DocID = doc.DocID;

            db.Entry(docActOnWork).State = entityState;
            await db.SaveChangesAsync();

            #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

            if (entityState == EntityState.Added && (docActOnWork.doc.NumberInt == null || docActOnWork.doc.NumberInt.Length == 0))
            {
                doc.NumberInt = docActOnWork.DocActOnWorkID.ToString();
                doc.NumberReal = docActOnWork.DocActOnWorkID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }
            else if (entityState == EntityState.Added)
            {
                doc.NumberReal = docActOnWork.DocActOnWorkID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }

            #endregion


            #endregion

            #region 3. DocActOnWorkTab *** *** *** *** *** *** *** *** ***

            //2.1. Удаляем записи в БД, если UPDATE
            if (entityState == EntityState.Modified)
            {
                SQLiteParameter parDocActOnWorkID = new SQLiteParameter("@DocActOnWorkID", System.Data.DbType.Int32) { Value = docActOnWork.DocActOnWorkID };
                db.Database.ExecuteSqlCommand("DELETE FROM DocActOnWorkTabs WHERE DocActOnWorkID=@DocActOnWorkID;", parDocActOnWorkID);
            }

            //2.2. Проставляем ID-шник "DocActOnWorkID" для всех позиций спецификации
            for (int i = 0; i < docActOnWorkTabCollection.Count(); i++)
            {
                docActOnWorkTabCollection[i].DocActOnWorkTabID = null;
                docActOnWorkTabCollection[i].DocActOnWorkID = Convert.ToInt32(docActOnWork.DocActOnWorkID);
                db.Entry(docActOnWorkTabCollection[i]).State = EntityState.Added;
            }
            await db.SaveChangesAsync();

            #endregion


            #region 4. Касса или Банк

            #region 4.1. Списываем с кассы/банка деньги
            //Получаем предыдущие данные. Т.к. клиент мог изменить, как сумму, так и тип оплаты.
            /*
            Models.Sklad.Doc.DocActOnWork _docActOnWork = await dbRead.DocActOnWorks.FindAsync(docActOnWork.DocActOnWorkID);
            if (entityState == EntityState.Modified && (_docActOnWork.doc.Payment != doc.Payment || _docActOnWork.doc.DirPaymentTypeID != doc.DirPaymentTypeID))
            {
                //Касса
                if (_docActOnWork.doc.DirPaymentTypeID == 1)
                {
                    #region Касса

                    //1. По складу находим привязанную к нему Кассу
                    Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docActOnWork.DirWarehouseID);
                    int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                    Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                    docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                    docCashOfficeSum.DirCashOfficeSumTypeID = 13; //Изъятие из кассы на основании пересохранения  акта выполненных работ №
                    docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                    docCashOfficeSum.DocID = _docActOnWork.doc.DocID;
                    docCashOfficeSum.DocXID = docActOnWork.DocActOnWorkID;
                    docCashOfficeSum.DocCashOfficeSumSum = _docActOnWork.doc.Payment;
                    docCashOfficeSum.Description = "";
                    docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;

                    Doc.DocCashOfficeSums.DocCashOfficeSumsController docCashOfficeSumsController = new Doc.DocCashOfficeSums.DocCashOfficeSumsController();

                    docCashOfficeSum.DirCashOfficeSumTypeID = 13; //Изъятие из кассы на основании пересохранения  акта выполненных работ №
                    docCashOfficeSum = await Task.Run(() => docCashOfficeSumsController.mPutPostDocCashOfficeSum(db, docCashOfficeSum, EntityState.Added));

                    #endregion
                }
                //Банк
                else if (_docActOnWork.doc.DirPaymentTypeID == 2)
                {
                    #region Банк

                    //1. По складу находим привязанную к нему Кассу
                    Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docActOnWork.DirWarehouseID);
                    int iDirBankID = dirWarehouse.DirBankID;

                    //2. Заполняем модель "DocBankSum"
                    Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                    docBankSum.DirBankID = iDirBankID;
                    docBankSum.DirBankSumTypeID = 12; //Изъятие из банка на основании отмены пересохранения акта выполенных работ №
                    docBankSum.DocBankSumDate = DateTime.Now;
                    docBankSum.DocID = _docActOnWork.doc.DocID;
                    docBankSum.DocXID = docActOnWork.DocActOnWorkID;
                    docBankSum.DocBankSumSum = _docActOnWork.doc.Payment;
                    docBankSum.Description = "";
                    docBankSum.DirEmployeeID = field.DirEmployeeID;

                    Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                    docBankSum.DirBankSumTypeID = 11; //Изъятие из банка на основании отмены пересохранения акта выполенных работ №
                    docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum(db, docBankSum, EntityState.Added));

                    #endregion
                }
            }
            */
            #endregion


            #region 4.2. Добавляем в кассу/банк деньги
            //Только, если сумма больше 0
            /*
            if (doc.Payment > 0 && (entityState == EntityState.Added || (_docActOnWork.doc.Payment != doc.Payment || _docActOnWork.doc.DirPaymentTypeID != doc.DirPaymentTypeID)))
            {
                //Касса
                if (doc.DirPaymentTypeID == 1)
                {
                    #region Касса

                    //1. По складу находим привязанную к нему Кассу
                    Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docActOnWork.DirWarehouseID);
                    int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                    //2. Заполняем модель "DocCashOfficeSum"
                    Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                    docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                    docCashOfficeSum.DirCashOfficeSumTypeID = 12; //Внесение в кассу на основании сохранения акта выполненных работ №
                    docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                    docCashOfficeSum.DocID = doc.DocID;
                    docCashOfficeSum.DocXID = docActOnWork.DocActOnWorkID;
                    docCashOfficeSum.DocCashOfficeSumSum = doc.Payment;
                    docCashOfficeSum.Description = "";
                    docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;

                    Doc.DocCashOfficeSums.DocCashOfficeSumsController docCashOfficeSumsController = new Doc.DocCashOfficeSums.DocCashOfficeSumsController();
                    docCashOfficeSum = await Task.Run(() => docCashOfficeSumsController.mPutPostDocCashOfficeSum(db, docCashOfficeSum, EntityState.Added));

                    #endregion
                }
                //Банк
                else if (doc.DirPaymentTypeID == 2)
                {
                    #region Банк

                    //1. По складу находим привязанную к нему Кассу
                    Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docActOnWork.DirWarehouseID);
                    int iDirBankID = dirWarehouse.DirBankID;

                    //2. Заполняем модель "DocBankSum"
                    Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                    docBankSum.DirBankID = iDirBankID;
                    docBankSum.DirBankSumTypeID = 11; //Внесение в банк на основании сохранения акта выполенных работ №
                    docBankSum.DocBankSumDate = DateTime.Now;
                    docBankSum.DocID = doc.DocID;
                    docBankSum.DocXID = docActOnWork.DocActOnWorkID;
                    docBankSum.DocBankSumSum = doc.Payment;
                    docBankSum.Description = "";
                    docBankSum.DirEmployeeID = field.DirEmployeeID;

                    Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                    docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum(db, docBankSum, EntityState.Added));

                    #endregion
                }
            }
            */

            #endregion

            #endregion


            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docActOnWork;
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
                "[DocActOnWorks].[DocActOnWorkID] AS [DocActOnWorkID], " +
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

                "[Docs].[NumberInt] AS [NumberInt] " +

                "FROM [DocActOnWorks] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocActOnWorks].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
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