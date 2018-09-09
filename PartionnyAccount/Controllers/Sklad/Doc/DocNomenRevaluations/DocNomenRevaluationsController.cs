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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocNomenRevaluations
{
    public class DocNomenRevaluationsController : ApiController
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
        // GET: api/DocNomenRevaluations
        public async Task<IHttpActionResult> GetDocNomenRevaluations(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocNomenRevaluations"));
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
                        from docNomenRevaluations in db.DocNomenRevaluations

                        join docNomenRevaluationTabs1 in db.DocNomenRevaluationTabs on docNomenRevaluations.DocNomenRevaluationID equals docNomenRevaluationTabs1.DocNomenRevaluationID into docNomenRevaluationTabs2
                        from docNomenRevaluationTabs in docNomenRevaluationTabs2.DefaultIfEmpty()

                        where docNomenRevaluations.doc.DocDate >= _params.DateS && docNomenRevaluations.doc.DocDate <= _params.DatePo

                        group new { docNomenRevaluationTabs }
                        by new
                        {
                            DocID = docNomenRevaluations.DocID,
                            DocDate = docNomenRevaluations.doc.DocDate,
                            Base = docNomenRevaluations.doc.Base,
                            Held = docNomenRevaluations.doc.Held,
                            Discount = docNomenRevaluations.doc.Discount,
                            Del = docNomenRevaluations.doc.Del,
                            Description = docNomenRevaluations.doc.Description,
                            IsImport = docNomenRevaluations.doc.IsImport,
                            DirVatValue = docNomenRevaluations.doc.DirVatValue,

                            DocNomenRevaluationID = docNomenRevaluations.DocNomenRevaluationID,
                            DirContractorID = docNomenRevaluations.doc.dirContractor.DirContractorID,
                            DirContractorName = docNomenRevaluations.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docNomenRevaluations.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docNomenRevaluations.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docNomenRevaluations.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docNomenRevaluations.dirWarehouse.DirWarehouseName,

                            NumberInt = docNomenRevaluations.doc.NumberInt,

                            //Оплата
                            Payment = docNomenRevaluations.doc.Payment,

                            DocDateHeld = docNomenRevaluations.doc.DocDateHeld,
                            DocDatePayment = docNomenRevaluations.doc.DocDatePayment,
                        }
                        into g

                        select new
                        {
                            DocID = g.Key.DocID,
                            DocDate = g.Key.DocDate,
                            Held = g.Key.Held,
                            Base = g.Key.Base,

                            //Discount = g.Key.Discount,
                            Del = g.Key.Del,
                            Description = g.Key.Description,
                            //IsImport = g.Key.IsImport,

                            DocNomenRevaluationID = g.Key.DocNomenRevaluationID,

                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            //DirVatValue = g.Key.DirVatValue,

                            DocDateHeld = g.Key.DocDateHeld,
                            DocDatePayment = g.Key.DocDatePayment,

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
                        query = query.Where(x => x.DocNomenRevaluationID == iNumber32);
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
                    query = query.OrderByDescending(x => x.DocNomenRevaluationID);
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
                    query = query.OrderByDescending(x => x.DocNomenRevaluationID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocNomenRevaluations.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocNomenRevaluation = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocNomenRevaluations/5
        [ResponseType(typeof(DocNomenRevaluation))]
        public async Task<IHttpActionResult> GetDocNomenRevaluation(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocNomenRevaluations"));
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
                        from docNomenRevaluations in db.DocNomenRevaluations
                        where docNomenRevaluations.DocNomenRevaluationID == id
                        select docNomenRevaluations
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                #region from

                        from docNomenRevaluations in db.DocNomenRevaluations

                        join docNomenRevaluationTabs1 in db.DocNomenRevaluationTabs on docNomenRevaluations.DocNomenRevaluationID equals docNomenRevaluationTabs1.DocNomenRevaluationID into docNomenRevaluationTabs2
                        from docNomenRevaluationTabs in docNomenRevaluationTabs2.DefaultIfEmpty()

                            #endregion

                        where docNomenRevaluations.DocNomenRevaluationID == id

                        #region group

                        group new { docNomenRevaluationTabs }
                        by new
                        {
                            DocID = docNomenRevaluations.DocID, //DocID = docNomenRevaluations.doc.DocID,
                            DocIDBase = docNomenRevaluations.doc.DocIDBase,
                            DocDate = docNomenRevaluations.doc.DocDate,
                            Base = docNomenRevaluations.doc.Base,
                            Held = docNomenRevaluations.doc.Held,
                            Discount = docNomenRevaluations.doc.Discount,
                            Del = docNomenRevaluations.doc.Del,
                            IsImport = docNomenRevaluations.doc.IsImport,
                            Description = docNomenRevaluations.doc.Description,
                            DirVatValue = docNomenRevaluations.doc.DirVatValue,

                            DocNomenRevaluationID = docNomenRevaluations.DocNomenRevaluationID,
                            DirContractorID = docNomenRevaluations.doc.DirContractorID,
                            DirContractorName = docNomenRevaluations.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docNomenRevaluations.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docNomenRevaluations.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docNomenRevaluations.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docNomenRevaluations.dirWarehouse.DirWarehouseName,
                            NumberInt = docNomenRevaluations.doc.NumberInt,
                            //Оплата
                            Payment = docNomenRevaluations.doc.Payment,
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

                            DocNomenRevaluationID = g.Key.DocNomenRevaluationID,
                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,
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

        // PUT: api/DocNomenRevaluations/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocNomenRevaluation(int id, DocNomenRevaluation docNomenRevaluation, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocNomenRevaluations"));
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
                Models.Sklad.Doc.DocNomenRevaluationTab[] docNomenRevaluationTabCollection = null;
                if (!String.IsNullOrEmpty(docNomenRevaluation.recordsDocNomenRevaluationTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docNomenRevaluationTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocNomenRevaluationTab[]>(docNomenRevaluation.recordsDocNomenRevaluationTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docNomenRevaluation.DocNomenRevaluationID || docNomenRevaluation.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docNomenRevaluation.DocID" из БД, если он отличается от пришедшего от клиента "docNomenRevaluation.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocNomenRevaluations
                        where x.DocNomenRevaluationID == docNomenRevaluation.DocNomenRevaluationID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docNomenRevaluation.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docNomenRevaluation.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docNomenRevaluation.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docNomenRevaluation = await Task.Run(() => mPutPostDocNomenRevaluation(db, dbRead, UO_Action, docNomenRevaluation, EntityState.Modified, docNomenRevaluationTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docNomenRevaluation.DocNomenRevaluationID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docNomenRevaluation.DocID,
                    DocNomenRevaluationID = docNomenRevaluation.DocNomenRevaluationID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocNomenRevaluations
        [ResponseType(typeof(DocNomenRevaluation))]
        public async Task<IHttpActionResult> PostDocNomenRevaluation(DocNomenRevaluation docNomenRevaluation, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocNomenRevaluations"));
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
                Models.Sklad.Doc.DocNomenRevaluationTab[] docNomenRevaluationTabCollection = null;
                if (!String.IsNullOrEmpty(docNomenRevaluation.recordsDocNomenRevaluationTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docNomenRevaluationTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocNomenRevaluationTab[]>(docNomenRevaluation.recordsDocNomenRevaluationTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docNomenRevaluation.DocID" из БД, если он отличается от пришедшего от клиента "docNomenRevaluation.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocNomenRevaluations
                        where x.DocNomenRevaluationID == docNomenRevaluation.DocNomenRevaluationID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docNomenRevaluation.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docNomenRevaluation.Substitute();

                #endregion


                #region Сохранение

                //using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docNomenRevaluation = await Task.Run(() => mPutPostDocNomenRevaluation(db, dbRead, UO_Action, docNomenRevaluation, EntityState.Added, docNomenRevaluationTabCollection, field)); //sysSetting
                        //ts.Commit(); //.Complete();
                    }
                    catch (Exception ex)
                    {
                        //try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = docNomenRevaluation.DocNomenRevaluationID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docNomenRevaluation.DocID,
                    DocNomenRevaluationID = docNomenRevaluation.DocNomenRevaluationID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocNomenRevaluations/5
        [ResponseType(typeof(DocNomenRevaluation))]
        public async Task<IHttpActionResult> DeleteDocNomenRevaluation(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocNomenRevaluations"));
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
                            from x in dbRead.DocNomenRevaluations
                            where x.DocNomenRevaluationID == id
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
                //2. DocNomenRevaluationTabs
                //3. DocNomenRevaluations
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocNomenRevaluation docNomenRevaluation = await db.DocNomenRevaluations.FindAsync(id);
                if (docNomenRevaluation == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Ищим DocID *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocNomenRevaluations
                                where x.DocNomenRevaluationID == id
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


                        #region 2. DocNomenRevaluationTabs *** *** *** *** ***

                        var queryDocNomenRevaluationTabs = await
                            (
                                from x in db.DocNomenRevaluationTabs
                                where x.DocNomenRevaluationID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocNomenRevaluationTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocNomenRevaluationTab docNomenRevaluationTab = await db.DocNomenRevaluationTabs.FindAsync(queryDocNomenRevaluationTabs[i].DocNomenRevaluationTabID);
                            db.DocNomenRevaluationTabs.Remove(docNomenRevaluationTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocNomenRevaluations *** *** *** *** ***

                        var queryDocNomenRevaluations = await
                            (
                                from x in db.DocNomenRevaluations
                                where x.DocNomenRevaluationID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocNomenRevaluations.Count(); i++)
                        {
                            Models.Sklad.Doc.DocNomenRevaluation docNomenRevaluation1 = await db.DocNomenRevaluations.FindAsync(queryDocNomenRevaluations[i].DocNomenRevaluationID);
                            db.DocNomenRevaluations.Remove(docNomenRevaluation1);
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

        private bool DocNomenRevaluationExists(int id)
        {
            return db.DocNomenRevaluations.Count(e => e.DocNomenRevaluationID == id) > 0;
        }



        internal async Task<DocNomenRevaluation> mPutPostDocNomenRevaluation(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocNomenRevaluation docNomenRevaluation,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocNomenRevaluationTab[] docNomenRevaluationTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docNomenRevaluation.NumberInt;
                doc.NumberReal = docNomenRevaluation.DocNomenRevaluationID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = docNomenRevaluation.DirPaymentTypeID;
                doc.Payment = docNomenRevaluation.Payment;
                doc.DirContractorID = docNomenRevaluation.DirContractorIDOrg;
                doc.DirContractorIDOrg = docNomenRevaluation.DirContractorIDOrg;
                doc.Discount = docNomenRevaluation.Discount;
                doc.DirVatValue = docNomenRevaluation.DirVatValue;
                doc.Base = docNomenRevaluation.Base;
                doc.Description = docNomenRevaluation.Description;
                doc.DocDate = docNomenRevaluation.DocDate;
                //doc.DocDisc = docNomenRevaluation.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docNomenRevaluation.DocID;
                doc.DocIDBase = docNomenRevaluation.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docNomenRevaluation" со всем полями!
                docNomenRevaluation.DocID = doc.DocID;

                #endregion

                #region 2. DocNomenRevaluation

                docNomenRevaluation.DocID = doc.DocID;

                db.Entry(docNomenRevaluation).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT

                if (entityState == EntityState.Added && (docNomenRevaluation.doc.NumberInt == null || docNomenRevaluation.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docNomenRevaluation.DocNomenRevaluationID.ToString();
                    doc.NumberReal = docNomenRevaluation.DocNomenRevaluationID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docNomenRevaluation.DocNomenRevaluationID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocNomenRevaluationTab

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocNomenRevaluationID = new SQLiteParameter("@DocNomenRevaluationID", System.Data.DbType.Int32) { Value = docNomenRevaluation.DocNomenRevaluationID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocNomenRevaluationTabs WHERE DocNomenRevaluationID=@DocNomenRevaluationID;", parDocNomenRevaluationID);
                }

                //2.2. Проставляем ID-шник "DocNomenRevaluationID" для всех позиций спецификации
                for (int i = 0; i < docNomenRevaluationTabCollection.Count(); i++)
                {
                    docNomenRevaluationTabCollection[i].DocNomenRevaluationTabID = null;
                    docNomenRevaluationTabCollection[i].DocNomenRevaluationID = Convert.ToInt32(docNomenRevaluation.DocNomenRevaluationID);
                    //Цена прихода в текущей валюте
                    docNomenRevaluationTabCollection[i].PriceCurrency = docNomenRevaluationTabCollection[i].PriceVAT * docNomenRevaluationTabCollection[i].DirCurrencyRate / docNomenRevaluationTabCollection[i].DirCurrencyMultiplicity;
                    db.Entry(docNomenRevaluationTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion


                if (UO_Action == "held")
                {
                    #region Переаценка

                    for (int i = 0; i < docNomenRevaluationTabCollection.Count(); i++)
                    {
                        #region Сохранение

                        Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(docNomenRevaluationTabCollection[i].RemPartyID);
                        remParty.PriceRetailVAT = docNomenRevaluationTabCollection[i].PriceRetailVAT;
                        remParty.PriceRetailCurrency = docNomenRevaluationTabCollection[i].PriceRetailCurrency;
                        remParty.PriceWholesaleVAT = docNomenRevaluationTabCollection[i].PriceWholesaleVAT;
                        remParty.PriceWholesaleCurrency = docNomenRevaluationTabCollection[i].PriceWholesaleCurrency;
                        remParty.PriceIMVAT = docNomenRevaluationTabCollection[i].PriceIMVAT;
                        remParty.PriceIMCurrency = docNomenRevaluationTabCollection[i].PriceIMCurrency;

                        db.Entry(remParty).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        #endregion
                    }

                    #endregion
                }
            }
            else if (UO_Action == "held_cancel")
            {
                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docNomenRevaluation.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion

                #region Переаценка

                for (int i = 0; i < docNomenRevaluationTabCollection.Count(); i++)
                {
                    #region Сохранение

                    Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(docNomenRevaluationTabCollection[i].RemPartyID);
                    remParty.PriceRetailVAT = docNomenRevaluationTabCollection[i].PriceRetailVAT_OLD;
                    remParty.PriceRetailCurrency = docNomenRevaluationTabCollection[i].PriceRetailCurrency_OLD;
                    remParty.PriceWholesaleVAT = docNomenRevaluationTabCollection[i].PriceWholesaleVAT_OLD;
                    remParty.PriceWholesaleCurrency = docNomenRevaluationTabCollection[i].PriceWholesaleCurrency_OLD;
                    remParty.PriceIMVAT = docNomenRevaluationTabCollection[i].PriceIMVAT_OLD;
                    remParty.PriceIMCurrency = docNomenRevaluationTabCollection[i].PriceIMCurrency_OLD;

                    db.Entry(remParty).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    #endregion
                }

                #endregion
            }


            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docNomenRevaluation;
        }

        #endregion

    }
}