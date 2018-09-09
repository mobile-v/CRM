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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocRetails
{
    public class DocRetailsController : ApiController
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

        int ListObjectID = 56;

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
        // GET: api/DocRetails
        public async Task<IHttpActionResult> GetDocRetails(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetails"));
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
                else _params.DateS= _params.DateS.Value.AddDays(-1);

                _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docRetails in db.DocRetails

                        join docRetailTabs1 in db.DocRetailTabs on docRetails.DocRetailID equals docRetailTabs1.DocRetailID into docRetailTabs2
                        from docRetailTabs in docRetailTabs2.DefaultIfEmpty()

                        where docRetails.doc.DocDate >= _params.DateS && docRetails.doc.DocDate <= _params.DatePo

                        group new { docRetailTabs }
                        by new
                        {
                            DocID = docRetails.DocID,
                            DocDate = docRetails.doc.DocDate,
                            Base = docRetails.doc.Base,
                            Held = docRetails.doc.Held,
                            Discount = docRetails.doc.Discount,
                            Del = docRetails.doc.Del,
                            Description = docRetails.doc.Description,
                            IsImport = docRetails.doc.IsImport,
                            DirVatValue = docRetails.doc.DirVatValue,

                            DocRetailID = docRetails.DocRetailID,
                            DirContractorID = docRetails.doc.dirContractor.DirContractorID,
                            DirContractorName = docRetails.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docRetails.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docRetails.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docRetails.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docRetails.dirWarehouse.DirWarehouseName,

                            NumberInt = docRetails.doc.NumberInt,

                            //Оплата
                            Payment = docRetails.doc.Payment,
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

                            DocRetailID = g.Key.DocRetailID,

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
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum)
                            */
                            g.Sum(x => x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment, sysSetting.FractionalPartInSum)
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
                        query = query.Where(x => x.DocRetailID == iNumber32);
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
                int dirCount = await Task.Run(() => db.DocRetails.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocRetail = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocRetails/5
        [ResponseType(typeof(DocRetail))]
        public async Task<IHttpActionResult> GetDocRetail(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetails"));
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
                        from docRetails in db.DocRetails
                        where docRetails.DocRetailID == id
                        select docRetails
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docRetails in db.DocRetails

                        join docRetailTabs1 in db.DocRetailTabs on docRetails.DocRetailID equals docRetailTabs1.DocRetailID into docRetailTabs2
                        from docRetailTabs in docRetailTabs2.DefaultIfEmpty()

                        #endregion

                        where docRetails.DocRetailID == id

                        #region group

                        group new { docRetailTabs }
                        by new
                        {
                            DocID = docRetails.DocID, //DocID = docRetails.doc.DocID,
                            DocIDBase = docRetails.doc.DocIDBase,
                            DocDate = docRetails.doc.DocDate,
                            DirPaymentTypeID = docRetails.doc.DirPaymentTypeID,
                            Base = docRetails.doc.Base,
                            Held = docRetails.doc.Held,
                            Discount = docRetails.doc.Discount,
                            Del = docRetails.doc.Del,
                            IsImport = docRetails.doc.IsImport,
                            Description = docRetails.doc.Description,
                            DirVatValue = docRetails.doc.DirVatValue,
                            //DirPaymentTypeID = docRetails.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docRetails.doc.dirPaymentType.DirPaymentTypeName,

                            DocRetailID = docRetails.DocRetailID,
                            DirContractorID = docRetails.doc.DirContractorID,
                            DirContractorName = docRetails.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docRetails.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docRetails.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docRetails.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docRetails.dirWarehouse.DirWarehouseName,
                            NumberInt = docRetails.doc.NumberInt,

                            //Оплата
                            Payment = docRetails.doc.Payment,

                            //Резерв
                            Reserve = docRetails.Reserve,
                            OnCredit = docRetails.OnCredit
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

                            DocRetailID = g.Key.DocRetailID,

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
                            g.Sum(x => x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docRetailTabs.Quantity * x.docRetailTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocRetails/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocRetail(int id, DocRetail docRetail, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetails"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Скидка: разрешена или нет
                if (docRetail.Discount > 0)
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
                Models.Sklad.Doc.DocRetailTab[] docRetailTabCollection = null;
                if (!String.IsNullOrEmpty(docRetail.recordsDocRetailTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docRetailTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocRetailTab[]>(docRetail.recordsDocRetailTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docRetail.DocRetailID || docRetail.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docRetail.DocID" из БД, если он отличается от пришедшего от клиента "docRetail.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocRetails
                        where x.DocRetailID == docRetail.DocRetailID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docRetail.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docRetail.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docRetail.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docRetail = await Task.Run(() => mPutPostDocRetail(db, dbRead, UO_Action, docRetail, EntityState.Modified, docRetailTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docRetail.DocRetailID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docRetail.DocID,
                    DocRetailID = docRetail.DocRetailID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        //После печати чека, если щабыли напечатать, сохранить ID-шнини чека
        //id == DocID
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocRetail(int id, int ServiceTypeRepair, int iTrash, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetails"));
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
                        //Models.Sklad.Doc.DocRetail docRetail = (Models.Sklad.Doc.DocRetail)db.DocRetails.Where(x => x.DocID == id);

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
                sysJourDisp.TableFieldID = docRetail.DocRetailID;
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


        // POST: api/DocRetails
        [ResponseType(typeof(DocRetail))]
        public async Task<IHttpActionResult> PostDocRetail(DocRetail docRetail, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetails"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Скидка: разрешена или нет
                if (docRetail.Discount > 0)
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
                Models.Sklad.Doc.DocRetailTab[] docRetailTabCollection = null;
                if (!String.IsNullOrEmpty(docRetail.recordsDocRetailTab) && UO_Action != "held_cancel")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docRetailTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocRetailTab[]>(docRetail.recordsDocRetailTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docRetail.DocID" из БД, если он отличается от пришедшего от клиента "docRetail.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocRetails
                        where x.DocRetailID == docRetail.DocRetailID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docRetail.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */


                //Проверка "Скидки"
                //1. Получаем сотурдника с правами

                

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docRetail.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docRetail = await Task.Run(() => mPutPostDocRetail(db, dbRead, UO_Action, docRetail, EntityState.Added, docRetailTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docRetail.DocRetailID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docRetail.DocID,
                    DocRetailID = docRetail.DocRetailID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocRetails/5
        [ResponseType(typeof(DocRetail))]
        public async Task<IHttpActionResult> DeleteDocRetail(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetails"));
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
                            from x in dbRead.DocRetails
                            where x.DocRetailID == id
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
                //2. DocRetailTabs
                //3. DocRetails
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocRetail docRetail = await db.DocRetails.FindAsync(id);
                if (docRetail == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Ищим DocID *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocRetails
                                where x.DocRetailID == id
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

                            if (queryDocReturnsCustomerTab.Count() > 0)
                            {
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


                        #region 2. DocRetailTabs *** *** *** *** ***

                        var queryDocRetailTabs = await
                            (
                                from x in db.DocRetailTabs
                                where x.DocRetailID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocRetailTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocRetailTab docRetailTab = await db.DocRetailTabs.FindAsync(queryDocRetailTabs[i].DocRetailTabID);
                            db.DocRetailTabs.Remove(docRetailTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocRetails *** *** *** *** ***

                        var queryDocRetails = await
                            (
                                from x in db.DocRetails
                                where x.DocRetailID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocRetails.Count(); i++)
                        {
                            Models.Sklad.Doc.DocRetail docRetail1 = await db.DocRetails.FindAsync(queryDocRetails[i].DocRetailID);
                            db.DocRetails.Remove(docRetail1);
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

        private bool DocRetailExists(int id)
        {
            return db.DocRetails.Count(e => e.DocRetailID == id) > 0;
        }


        internal async Task<DocRetail> mPutPostDocRetail(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocRetail docRetail,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocRetailTab[] docRetailTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "held") docRetail.Reserve = false;
            else docRetail.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                //Скидка: от суммы или от цены (если продали более 1 аппарата)
                //sysSetting.DiscountMarketType
                if (sysSetting.DiscountMarketType == 1)
                {
                    //от суммы
                    //...
                }
                else
                {
                    //от цены
                    if (docRetailTabCollection.Count() == 1)
                    {
                        double Quantity = docRetailTabCollection[0].Quantity;
                        docRetail.Discount = docRetail.Discount * Quantity;
                    }
                    else
                    {
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg129));
                    }
                }


                #region 1. Doc

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docRetail.NumberInt;
                doc.NumberReal = docRetail.DocRetailID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docRetail.DirPaymentTypeID;
                doc.Payment = docRetail.Payment;
                if(docRetail.DirContractorID != null) doc.DirContractorID = Convert.ToInt32(docRetail.DirContractorID); else doc.DirContractorID = docRetail.DirContractorIDOrg;
                doc.DirContractorIDOrg = docRetail.DirContractorIDOrg;
                doc.Discount = docRetail.Discount;
                doc.DirVatValue = docRetail.DirVatValue;
                doc.Base = docRetail.Base;
                doc.Description = docRetail.Description;
                doc.DocDate = DateTime.Now; //docRetail.DocDate;
                //doc.DocDisc = docRetail.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docRetail.DocID;
                doc.DocIDBase = docRetail.DocIDBase;
                doc.KKMSCheckNumber = docRetail.KKMSCheckNumber;
                doc.KKMSIdCommand = docRetail.KKMSIdCommand;
                doc.KKMSEMail = docRetail.KKMSEMail;
                doc.KKMSPhone = docRetail.KKMSPhone;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docRetail" со всем полями!
                docRetail.DocID = doc.DocID;

                #endregion

                #region 2. DocRetail

                docRetail.DocID = doc.DocID;

                db.Entry(docRetail).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT

                if (entityState == EntityState.Added && (docRetail.doc.NumberInt == null || docRetail.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docRetail.DocRetailID.ToString();
                    doc.NumberReal = docRetail.DocRetailID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docRetail.DocRetailID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 4. Description: пишем ID-шник в DocRetailReturnTab и RemParty

                string Description = ""; if (docRetailTabCollection.Length > 0) Description = docRetailTabCollection[0].Description;
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

                #region 3. DocRetailTab

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocRetailID = new SQLiteParameter("@DocRetailID", System.Data.DbType.Int32) { Value = docRetail.DocRetailID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocRetailTabs WHERE DocRetailID=@DocRetailID;", parDocRetailID);
                }

                //2.2. Проставляем ID-шник "DocRetailID" для всех позиций спецификации
                double dSumTab = 0;
                for (int i = 0; i < docRetailTabCollection.Count(); i++)
                {
                    docRetailTabCollection[i].DocRetailTabID = null;
                    docRetailTabCollection[i].DocRetailID = Convert.ToInt32(docRetail.DocRetailID);
                    docRetailTabCollection[i].DirDescriptionID = DirDescriptionID;
                    db.Entry(docRetailTabCollection[i]).State = EntityState.Added;

                    dSumTab += docRetailTabCollection[i].Quantity * docRetailTabCollection[i].PriceCurrency;
                }
                await db.SaveChangesAsync();
                dSumTab = dSumTab - doc.Discount;

                #endregion


                if (UO_Action == "held" || docRetail.Reserve)
                {
                    Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();

                    #region 1. Ищим в Возврате покупателя, если нет, то удаляем в RemPartyMinuses *** *** *** *** ***

                    //1.1. Удаляем "RemPartyMinuses"
                    var queryRemPartyMinuses = await
                        (
                            from x in db.RemPartyMinuses
                            where x.DocID == docRetail.DocID
                            select x
                        ).ToListAsync();

                    for (int i = 0; i < queryRemPartyMinuses.Count(); i++)
                    {
                        int iRemPartyMinusID = Convert.ToInt32(queryRemPartyMinuses[i].RemPartyMinusID);

                        var queryDocRetailReturnTab = await
                            (
                                from x in db.DocRetailReturnTabs
                                where x.RemPartyMinusID == iRemPartyMinusID
                                select x
                            ).ToListAsync();

                        if (queryDocRetailReturnTab.Count() > 0)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg117 +

                                "<tr>" +
                                "<td>" + queryDocRetailReturnTab[0].RemPartyMinusID + "</td>" +          //партия списания
                                "<td>" + queryDocRetailReturnTab[0].DocRetailReturnID + "</td>" +        //№ д-та
                                "<td>" + queryDocRetailReturnTab[0].DirNomenID + "</td>" +               //Код товара
                                "<td>" + queryDocRetailReturnTab[0].Quantity + "</td>" +                 //списуемое к-во
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

                    string NomenName = "";

                    for (int i = 0; i < docRetailTabCollection.Count(); i++)
                    {
                        #region Проверка

                        //Переменные
                        int iRemPartyID = Convert.ToInt32(docRetailTabCollection[i].RemPartyID);
                        double dQuantity = docRetailTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(iRemPartyID);
                        if (remParty == null)
                        {
                            throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg116_1 + iRemPartyID + Classes.Language.Sklad.Language.msg116_2);
                        }
                        db.Entry(remParty).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (remParty.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docRetailTabCollection[i].RemPartyID + "</td>" +                                //партия
                                "<td>" + docRetailTabCollection[i].DirNomenID + "</td>" +                                //Код товара
                                "<td>" + docRetailTabCollection[i].Quantity + "</td>" +                                  //списуемое к-во
                                "<td>" + remParty.Remnant + "</td>" +                                                  //остаток партии
                                "<td>" + (docRetailTabCollection[i].Quantity - remParty.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirWarehouseID != docRetail.DirWarehouseID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docRetail.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docRetail.DirWarehouseID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docRetailTabCollection[i].RemPartyID + "</td>" +           //партия
                                "<td>" + docRetailTabCollection[i].DirNomenID + "</td>" +           //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + remParty.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirContractorIDOrg != docRetail.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docRetail.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docRetail.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docRetailTabCollection[i].RemPartyID + "</td>" +           //партия
                                "<td>" + docRetailTabCollection[i].DirNomenID + "</td>" +           //Код товара
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
                        remPartyMinus.RemPartyID = Convert.ToInt32(docRetailTabCollection[i].RemPartyID);

                        remPartyMinus.DirNomenID = docRetailTabCollection[i].DirNomenID;
                        /*
                        remPartyMinus.DirCharColourID = remParty.DirCharColourID;
                        remPartyMinus.DirCharMaterialID = remParty.DirCharMaterialID;
                        remPartyMinus.DirCharNameID = remParty.DirCharNameID;
                        remPartyMinus.DirCharSeasonID = remParty.DirCharSeasonID;
                        remPartyMinus.DirCharSexID = remParty.DirCharSexID;
                        remPartyMinus.DirCharSizeID = remParty.DirCharSizeID;
                        remPartyMinus.DirCharStyleID = remParty.DirCharStyleID;
                        remPartyMinus.DirCharTextureID = remParty.DirCharTextureID;
                        remParty.SerialNumber = remParty.SerialNumber;
                        remParty.Barcode = remParty.Barcode;
                        */
                        remPartyMinus.Quantity = docRetailTabCollection[i].Quantity;
                        remPartyMinus.DirCurrencyID = docRetailTabCollection[i].DirCurrencyID;
                        remPartyMinus.DirCurrencyMultiplicity = docRetailTabCollection[i].DirCurrencyMultiplicity;
                        remPartyMinus.DirCurrencyRate = docRetailTabCollection[i].DirCurrencyRate;
                        remPartyMinus.DirVatValue = docRetail.DirVatValue;
                        remPartyMinus.DirWarehouseID = docRetail.DirWarehouseID;
                        remPartyMinus.DirContractorIDOrg = docRetail.DirContractorIDOrg;

                        //remPartyMinus.DirContractorID = docRetail.DirContractorID;
                        if (docRetail.DirContractorID != null) remPartyMinus.DirContractorID = Convert.ToInt32(docRetail.DirContractorID);
                        else remPartyMinus.DirContractorID = docRetail.DirContractorIDOrg;

                        remPartyMinus.DocID = Convert.ToInt32(docRetail.DocID);
                        remPartyMinus.PriceCurrency = docRetailTabCollection[i].PriceCurrency;
                        remPartyMinus.PriceVAT = docRetailTabCollection[i].PriceVAT;
                        remPartyMinus.FieldID = Convert.ToInt32(docRetailTabCollection[i].DocRetailTabID);
                        remPartyMinus.Reserve = docRetail.Reserve;

                        remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        remPartyMinus.DocDate = doc.DocDate;
                        remPartyMinus.DirDescriptionID = DirDescriptionID;

                        db.Entry(remPartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion

                        NomenName += docRetailTabCollection[i].DirNomenID + " ";
                    }

                    #endregion


                    #region Касса или Банк


                    #region 1. Получаем валюту из склада

                    int DirCurrencyID = 0, DirCurrencyMultiplicity = 0; //, DirCashOfficeID = 0, DirBankID = 0;;
                    double DirCurrencyRate = 0;

                    var query = await Task.Run(() =>
                        (
                            from x in db.DirWarehouses
                            where x.DirWarehouseID == docRetail.DirWarehouseID
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
                    pay.DocXID = docRetail.DocRetailID;
                    pay.DocXSumDate = doc.DocDate;
                    pay.DocXSumSum = dSumTab; // - получили при сохранении Спецификации (выше)
                    pay.Base = "Оплата за коды товаров: " + NomenName; // - получили при сохранении Спецификации (выше)
                    //pay.Description = "";
                    pay.KKMSCheckNumber = docRetail.KKMSCheckNumber;
                    pay.KKMSIdCommand = docRetail.KKMSIdCommand;
                    pay.KKMSEMail = docRetail.KKMSEMail;
                    pay.KKMSPhone = docRetail.KKMSPhone;
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

                //Если хоть одно поле для "DocRetails.DocID" будет "RemParties.RemPartyMinusID > 0", то возвращали товар - Эксепшен.
                var query = await
                    (
                        from x in db.RemParties
                        from y in db.RemPartyMinuses
                        where y.DocID == docRetail.DocID && x.RemPartyMinusID == y.RemPartyMinusID
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

                #region DocRetailXXXTabs

                //Проверим поле "RemPartyMinusID" в таблице "DocRetailReturnTabs"
                var queryX = await
                    (
                        from x in db.DocRetailReturnTabs
                        from y in db.RemPartyMinuses
                        where x.RemPartyMinusID == y.RemPartyMinusID && y.DocID== docRetail.DocID
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
                        "<td>" + queryX[i].x.docRetailReturn.doc.NumberReal + "</td>" +                     //Номер документа
                        "<td>" + queryX[i].x.docRetailReturn.doc.DocID + "</td>" +                          //Общий документа
                        "<td>" + queryX[i].x.docRetailReturn.doc.listObject.ListObjectNameRu + "</td>" +    //Тип документа

                        "<td>" + queryX[i].y.RemPartyMinusID + "</td>" +                    //списаная партия
                        "<td>" + queryX[i].y.DirNomenID + "</td>" +                         //Код товара
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

                #endregion

                #endregion

                //Удаление записей в таблицах: RemPartyMinuses
                #region 1. RemPartyMinuses *** *** *** *** *** *** *** *** *** ***
                /*
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docRetail.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemPartyMinuses WHERE DocID=@DocID; ", parDocID);
                */
                #endregion

                //Обновление записей: RemPartyMinuses
                #region 1. RemPartyMinuses и RemParties *** *** *** *** *** *** *** *** *** ***

                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docRetail.DocID };
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE RemPartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion

                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = await Task.Run(() => db.Docs.FindAsync(docRetail.DocID));
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


            return docRetail;
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
                "[DocRetails].[DocRetailID] AS [DocRetailID], " +
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
                "[DocRetails].[Reserve] AS [Reserve] " +

                "FROM [DocRetails] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocRetails].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocRetails].[DirWarehouseID] " +
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