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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocServicePurches
{
    public class DocServiceMovsController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        Models.Sklad.Log.LogLogistic logLogistic = new Models.Sklad.Log.LogLogistic(); Controllers.Sklad.Log.LogLogisticsController logLogisticsController = new Log.LogLogisticsController();
        Models.Sklad.Log.LogService logService = new Models.Sklad.Log.LogService(); Controllers.Sklad.Log.LogServicesController logServicesController = new Log.LogServicesController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 78;

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
            public int? DirMovementStatusID;
            public DateTime? DateS;
            public DateTime? DatePo;

            //Отобразить "Архив"
            public int? DocServiceMovID;
        }
        // GET: api/DocServiceMovs
        public async Task<IHttpActionResult> GetDocServiceMovs(HttpRequestMessage request)
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
                //dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права (1 - Write, 2 - Read, 3 - No Access)
                //Возможно доступна Логистика
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovementsLogistics"));
                    if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                }

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
                _params.DirMovementStatusID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirMovementStatusID", true) == 0).Value);

                //Архив
                _params.DocServiceMovID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocServiceMovID", true) == 0).Value);

                string sDate = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateS", true) == 0).Value;
                if (!String.IsNullOrEmpty(sDate))
                {
                    _params.DateS = Convert.ToDateTime(Convert.ToDateTime(sDate).ToString("yyyy-MM-dd 00:00:01"));
                    if (_params.DateS < Convert.ToDateTime("01.01.1800")) _params.DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                    else _params.DateS = _params.DateS.Value.AddDays(-1);
                }

                sDate = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DatePo", true) == 0).Value;
                if (!String.IsNullOrEmpty(sDate))
                {
                    _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(sDate).ToString("yyyy-MM-dd 23:59:59"));
                    if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));
                }

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docServiceMovs in db.DocServiceMovs

                        join docServiceMovTabs1 in db.DocServiceMovTabs on docServiceMovs.DocServiceMovID equals docServiceMovTabs1.DocServiceMovID into docServiceMovTabs2
                        from docServiceMovTabs in docServiceMovTabs2.DefaultIfEmpty()

                            //where docServiceMovs.doc.DocDate >= _params.DateS && docServiceMovs.doc.DocDate <= _params.DatePo

                        group new { docServiceMovTabs }
                        by new
                        {
                            DocID = docServiceMovs.DocID,
                            DocDate = docServiceMovs.doc.DocDate,
                            Base = docServiceMovs.doc.Base,
                            Held = docServiceMovs.doc.Held,
                            Discount = docServiceMovs.doc.Discount,
                            Del = docServiceMovs.doc.Del,
                            Description = docServiceMovs.doc.Description,
                            IsImport = docServiceMovs.doc.IsImport,
                            DirVatValue = docServiceMovs.doc.DirVatValue,

                            DocServiceMovID = docServiceMovs.DocServiceMovID,
                            //DirContractorName = docServiceMovs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docServiceMovs.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docServiceMovs.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docServiceMovs.dirWarehouseFrom.DirWarehouseID,
                            DirWarehouseNameFrom = docServiceMovs.dirWarehouseFrom.DirWarehouseName,
                            DirWarehouseIDTo = docServiceMovs.dirWarehouseTo.DirWarehouseID,
                            DirWarehouseNameTo = docServiceMovs.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docServiceMovs.doc.NumberInt,

                            DirEmployeeIDCourier = docServiceMovs.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docServiceMovs.dirEmployee_Courier.DirEmployeeName,
                            DirMovementStatusID = docServiceMovs.DirMovementStatusID, //Курьер штрихнулся и забрал товар

                            DirEmployeeName = docServiceMovs.doc.dirEmployee.DirEmployeeName,
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

                            DocServiceMovID = g.Key.DocServiceMovID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseIDFrom = g.Key.DirWarehouseIDFrom,
                            DirWarehouseNameFrom = g.Key.DirWarehouseNameFrom,
                            DirWarehouseIDTo = g.Key.DirWarehouseIDTo,
                            DirWarehouseNameTo = g.Key.DirWarehouseNameTo,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            DirEmployeeIDCourier = g.Key.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = g.Key.DirEmployeeNameCourier,
                            DirMovementStatusID = g.Key.DirMovementStatusID, //Курьер штрихнулся и забрал товар
                            DirEmployeeName = g.Key.DirEmployeeName,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => x.docServiceMovTabs.docServicePurch.PrepaymentSum) == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docServiceMovTabs.docServicePurch.PrepaymentSum), sysSetting.FractionalPartInSum),

                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Отобразить "Предыдущие ремнты"

                if (_params.DocServiceMovID != null && _params.DocServiceMovID > 0)
                {
                    query = query.Where(x => x.DocServiceMovID == _params.DocServiceMovID);
                }

                #endregion


                #region dirEmployee: dirEmployee.DirWarehouseID and/or dirEmployee.DirContractorIDOrg

                /*if (_params.DirWarehouseID != null && _params.DirWarehouseID > 0)
                {
                    query = query.Where(x => x.DirWarehouseIDTo == _params.DirWarehouseID);
                }*/
                //Если привязка к сотруднику (для документа "DocServiceMovs" показать все склады)
                if (field.DirEmployeeID != 1)
                {
                    //1. Получаем все склады к которым у Сотрудника есть доступ
                    var queryW = await Task.Run(() =>
                        (
                            from x in db.DirEmployeeWarehouse
                            where x.DirEmployeeID == field.DirEmployeeID
                            select x.DirWarehouseID
                        ).ToListAsync());

                    if (queryW.Count() > 0)
                    {
                        query = query.Where(x => queryW.Contains(x.DirWarehouseIDFrom));
                    }
                }

                if (dirEmployee.DirContractorIDOrg != null && dirEmployee.DirContractorIDOrg > 0)
                {
                    query = query.Where(x => x.DirContractorIDOrg == dirEmployee.DirContractorIDOrg);
                }

                #endregion


                #region DirEmployeeIDCourier - Логистика

                if (_params.DirMovementStatusID > 1)
                {
                    //77777 - показать все статусы
                    if (_params.DirMovementStatusID == 77777) query = query.Where(x => x.DirMovementStatusID > 1 && x.DirMovementStatusID != 4);
                    //Показать конкретный статус
                    else query = query.Where(x => x.DirMovementStatusID == _params.DirMovementStatusID);
                }
                else
                {
                    query = query.Where(x => x.DirMovementStatusID == 1);
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

                #region Фильтр Date S и Po

                if (_params.DateS != null)
                {
                    query = query.Where(x => x.DocDate >= _params.DateS);
                }

                if (_params.DatePo != null)
                {
                    query = query.Where(x => x.DocDate <= _params.DatePo);
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
                        query = query.Where(x => x.DocServiceMovID == iNumber32);
                    }
                    //Если Дата
                    else if (bDateTime)
                    {
                        query = query.Where(x => x.DocDate == dDateTime);
                    }
                    //Иначе, только текстовые поля
                    else
                    {
                        query = query.Where(x => x.DirWarehouseNameFrom.Contains(_params.parSearch) || x.DirWarehouseNameTo.Contains(_params.parSearch) || x.NumberInt.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                query = query.OrderByDescending(x => x.DocServiceMovID).Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocServiceMovs.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocServiceMov = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocServiceMovs/5
        [ResponseType(typeof(DocServiceMov))]
        public async Task<IHttpActionResult> GetDocServiceMov(int id, HttpRequestMessage request)
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
                //dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права (1 - Write, 2 - Read, 3 - No Access)
                //Возможно доступна Логистика
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovementsLogistics"));
                    if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                }

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
                        from docServiceMovs in db.DocServiceMovs
                        where docServiceMovs.DocServiceMovID == id
                        select docServiceMovs
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docServiceMovs in db.DocServiceMovs

                        join docServiceMovTabs1 in db.DocServiceMovTabs on docServiceMovs.DocServiceMovID equals docServiceMovTabs1.DocServiceMovID into docServiceMovTabs2
                        from docServiceMovTabs in docServiceMovTabs2.DefaultIfEmpty()

                        #endregion

                        where docServiceMovs.DocServiceMovID == id

                        #region group

                        group new { docServiceMovTabs }
                        by new
                        {
                            DocID = docServiceMovs.DocID, //DocID = docServiceMovs.doc.DocID,
                            DocIDBase = docServiceMovs.doc.DocIDBase,
                            DocDate = docServiceMovs.doc.DocDate,
                            Base = docServiceMovs.doc.Base,
                            Held = docServiceMovs.doc.Held,
                            Discount = docServiceMovs.doc.Discount,
                            Del = docServiceMovs.doc.Del,
                            IsImport = docServiceMovs.doc.IsImport,
                            Description = docServiceMovs.doc.Description,
                            DirVatValue = docServiceMovs.doc.DirVatValue,
                            //DirPaymentTypeID = docServiceMovs.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docServiceMovs.doc.dirPaymentType.DirPaymentTypeName,

                            DocServiceMovID = docServiceMovs.DocServiceMovID,
                            //DirContractorID = docServiceMovs.doc.DirContractorID,
                            //DirContractorName = docServiceMovs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docServiceMovs.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docServiceMovs.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docServiceMovs.dirWarehouseFrom.DirWarehouseID,
                            DirWarehouseNameFrom = docServiceMovs.dirWarehouseFrom.DirWarehouseName,

                            DirWarehouseIDTo = docServiceMovs.dirWarehouseTo.DirWarehouseID,
                            DirWarehouseNameTo = docServiceMovs.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docServiceMovs.doc.NumberInt,

                            //Оплата
                            Payment = docServiceMovs.doc.Payment,

                            //Резерв
                            Reserve = docServiceMovs.Reserve,
                            DirMovementDescriptionName = docServiceMovs.dirMovementDescription.DirMovementDescriptionName,

                            DirMovementStatusID = docServiceMovs.DirMovementStatusID,

                            DirEmployeeIDCourier = docServiceMovs.DirEmployeeIDCourier,

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

                            DocServiceMovID = g.Key.DocServiceMovID,
                            //DirContractorID = g.Key.DirContractorID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,

                            DirWarehouseIDFrom = g.Key.DirWarehouseIDFrom,
                            DirWarehouseNameFrom = g.Key.DirWarehouseNameFrom,

                            DirWarehouseIDTo = g.Key.DirWarehouseIDTo,
                            DirWarehouseNameTo = g.Key.DirWarehouseNameTo,

                            NumberInt = g.Key.NumberInt,

                            //Поступление
                            SumPurch =
                            g.Sum(x => x.docServiceMovTabs.docServicePurch.PrepaymentSum) == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docServiceMovTabs.docServicePurch.PrepaymentSum), sysSetting.FractionalPartInSum),
                            

                            DescriptionMovement = g.Key.DirMovementDescriptionName, //DirMovementDescriptionID = g.Key.DirMovementDescriptionID,

                            DirMovementStatusID = g.Key.DirMovementStatusID,
                            DirEmployeeIDCourier = g.Key.DirEmployeeIDCourier,
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

        // PUT: api/DocServiceMovs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocServiceMov(int id, DocServiceMov docServiceMov, HttpRequestMessage request)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovements"));
                //if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Возможно доступна Логистика
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovements"));
                if (iRight != 1)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovementsLogistics"));
                    if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
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


                int? UOSms = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UOSms", true) == 0).Value);


                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocServiceMovTab[] docServiceMovTabCollection = null;
                if (!String.IsNullOrEmpty(docServiceMov.recordsDocServiceMovTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docServiceMovTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocServiceMovTab[]>(docServiceMov.recordsDocServiceMovTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docServiceMov.DocServiceMovID || docServiceMov.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //2. Если статус > 1, то редактировать может только Администратор
                if (docServiceMov.DirMovementStatusID > 1 && field.DirEmployeeID != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg122)); }

                //3. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docServiceMov.DirEmployeeIDCourier != null && docServiceMov.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docServiceMov.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }

                //4. Получаем "docServiceMov.DocID" из БД, если он отличается от пришедшего от клиента "docServiceMov.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocServiceMovs
                        where x.DocServiceMovID == docServiceMov.DocServiceMovID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docServiceMov.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //5. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docServiceMov.DocID);
                //6.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //6.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docServiceMov.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docServiceMov = await Task.Run(() => mPutPostDocServiceMov(db, dbRead, UO_Action, UOSms, docServiceMov, EntityState.Modified, docServiceMovTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docServiceMov.DocServiceMovID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docServiceMov.DocID,
                    DocServiceMovID = docServiceMov.DocServiceMovID,
                    bFindWarehouse = bFindWarehouse
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                try { db.Database.Connection.Close(); db.Dispose(); } catch { }
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // PUT: api/DocServiceMovs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocServiceMov(int id, int DirStatusID, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovements"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                string sDiagnosticRresults = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sDiagnosticRresults", true) == 0).Value;

                #endregion


                Models.Sklad.Doc.DocServiceMov docServiceMov = await db.DocServiceMovs.FindAsync(id);
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docServiceMov.DocID);


                #region Проверки

                //1. Если документ проведён то выдать сообщение
                if (Convert.ToBoolean(doc.Held)) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3)); }

                //2. Нельзя перескакивать через статусы (статус всегда +/- 1)
                if (Math.Abs(DirStatusID - Convert.ToInt32(docServiceMov.DirMovementStatusID)) != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_4)); }

                //3. Если статус == 3, то проверяем введённый пароль и чистим его (что бы не писался в Лог)
                if (DirStatusID == 3)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(docServiceMov.DirEmployeeIDCourier);
                    if (dirEmployee.DirEmployeePswd != sDiagnosticRresults)
                    {
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_1));
                    }
                }

                //4. Если статус == 4, то проверить если ли у сотрудника доступ к сладку To
                if (DirStatusID == 4)
                {
                    //Админу можно "ВСЁ"
                    if (field.DirEmployeeID != 1)
                    {
                        //1. Получаем все склады к которым у Сотрудника есть доступ
                        var queryW = await Task.Run(() =>
                            (
                                from x in db.DirEmployeeWarehouse
                                where x.DirEmployeeID == field.DirEmployeeID
                                select x
                            ).ToListAsync());

                        if (queryW.Count() > 0)
                        {
                            //Пробегаем все доступные сотруднику склады и если нет == DirWarehouseIDTo, то только Сохранение и Резерв
                            for (int i = 0; i < queryW.Count(); i++)
                            {
                                if (docServiceMov.DirWarehouseIDTo == queryW[0].DirWarehouseID) { bFindWarehouse = true; break; }
                            }
                            if (!bFindWarehouse) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_2));
                        }
                    }
                }

                #endregion

                #region Сохранение

                try
                {
                    //using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            //await mStatusChange(db, ts, docServiceMov, id, DirStatusID, sDiagnosticRresults, field);
                            await mStatusChange(db, dbRead, docServiceMov, id, DirStatusID, sDiagnosticRresults, field);

                            //docServiceMov = await Task.Run(() => mPutPostDocServiceMov(db, dbRead, UO_Action, docServiceMov, EntityState.Modified, docServiceMovTabCollection, field)); //sysSetting

                            //ts.Commit();
                        }
                        catch (Exception ex)
                        {
                            //try { ts.Rollback(); ts.Dispose(); } catch { }
                            try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                            return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                        }
                    }

                    dynamic collectionWrapper = new
                    {
                        DocID = docServiceMov.DocID,
                        DocServiceMovID = docServiceMov.DocServiceMovID
                    };
                    return Ok(returnServer.Return(true, collectionWrapper));
                }
                catch (Exception ex)
                {
                    return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                }

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }


        // POST: api/DocServiceMovs
        [ResponseType(typeof(DocServiceMov))]
        public async Task<IHttpActionResult> PostDocServiceMov(DocServiceMov docServiceMov, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovements"));
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


                //Проверяме пароль
                string DirEmployeePswd = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeePswd", true) == 0).Value;
                Classes.Account.EncodeDecode encode = new Classes.Account.EncodeDecode();
                if (DirEmployeePswd != encode.UnionDecode(authCookie["CookieP"])) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_6));


                int? UOSms = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UOSms", true) == 0).Value);


                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocServiceMovTab[] docServiceMovTabCollection = null;
                if (!String.IsNullOrEmpty(docServiceMov.recordsDocServiceMovTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docServiceMovTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocServiceMovTab[]>(docServiceMov.recordsDocServiceMovTab);
                }

                #endregion

                #region Проверки

                //1. 
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);


                //2. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docServiceMov.DirEmployeeIDCourier != null && docServiceMov.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docServiceMov.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }


                //3. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docServiceMov.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docServiceMov = await Task.Run(() => mPutPostDocServiceMov(db, dbRead, UO_Action, UOSms, docServiceMov, EntityState.Added, docServiceMovTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docServiceMov.DocServiceMovID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docServiceMov.DocID,
                    DocServiceMovID = docServiceMov.DocServiceMovID,
                    bFindWarehouse = bFindWarehouse
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                try { db.Database.Connection.Close(); db.Dispose(); } catch { }
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocServiceMovs/5
        [ResponseType(typeof(DocServiceMov))]
        public async Task<IHttpActionResult> DeleteDocServiceMov(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovements"));
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
                            from x in dbRead.DocServiceMovs
                            where x.DocServiceMovID == id
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
                //2. DocServiceMovTabs
                //3. DocServiceMovs
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocServiceMov docServiceMov = await db.DocServiceMovs.FindAsync(id);
                if (docServiceMov == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Rem2PartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocServiceMovs
                                where x.DocServiceMovID == id
                                select x
                            ).ToListAsync();
                        if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                        //1.1. Удаляем "Rem2PartyMinuses"
                        var queryRem2PartyMinuses = await
                            (
                                from x in db.Rem2PartyMinuses
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryRem2PartyMinuses.Count(); i++)
                        {
                            Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = await db.Rem2PartyMinuses.FindAsync(queryRem2PartyMinuses[i].Rem2PartyMinusID);
                            db.Rem2PartyMinuses.Remove(rem2PartyMinus);
                            await db.SaveChangesAsync();
                        }

                        //1.2. Удаляем "Rem2Parties"
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


                        #region 2. DocServiceMovTabs *** *** *** *** ***

                        var queryDocServiceMovTabs = await
                            (
                                from x in db.DocServiceMovTabs
                                where x.DocServiceMovID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocServiceMovTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocServiceMovTab docServiceMovTab = await db.DocServiceMovTabs.FindAsync(queryDocServiceMovTabs[i].DocServiceMovTabID);
                            db.DocServiceMovTabs.Remove(docServiceMovTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocServiceMovs *** *** *** *** ***

                        var queryDocServiceMovs = await
                            (
                                from x in db.DocServiceMovs
                                where x.DocServiceMovID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocServiceMovs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocServiceMov docServiceMov1 = await db.DocServiceMovs.FindAsync(queryDocServiceMovs[i].DocServiceMovID);
                            db.DocServiceMovs.Remove(docServiceMov1);
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
                try { db.Database.Connection.Close(); db.Dispose(); } catch { }
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

        private bool DocServiceMovExists(int id)
        {
            return db.DocServiceMovs.Count(e => e.DocServiceMovID == id) > 0;
        }


        bool bFindWarehouse = false;
        internal async Task<DocServiceMov> mPutPostDocServiceMov(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            int? UOSms,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocServiceMov docServiceMov,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocServiceMovTab[] docServiceMovTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region 1. ПРОВЕРЯЕМ: Есть ли доступ к складу на который перемещаем, иначе только сохранить и резервировать!!!

            //Админу можно "ВСЁ"
            if (field.DirEmployeeID != 1)
            {
                //1. Получаем все склады к которым у Сотрудника есть доступ
                var queryW = await Task.Run(() =>
                    (
                        from x in db.DirEmployeeWarehouse
                        where x.DirEmployeeID == field.DirEmployeeID
                        select x
                    ).ToListAsync());

                if (queryW.Count() > 0)
                {
                    //Пробегаем все доступные сотруднику склады и если нет == DirWarehouseIDTo, то только Сохранение и Резерв
                    for (int i = 0; i < queryW.Count(); i++)
                    {
                        if (docServiceMov.DirWarehouseIDTo == queryW[i].DirWarehouseID) { bFindWarehouse = true; break; }
                    }
                    if (!bFindWarehouse) UO_Action = "save";
                }
            }
            else
            {
                bFindWarehouse = true;
            }

            #endregion


            if (UO_Action == "held") docServiceMov.Reserve = false;
            else docServiceMov.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docServiceMov.NumberInt;
                doc.NumberReal = docServiceMov.DocServiceMovID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = 1;
                doc.Payment = 0;
                doc.DirContractorID = docServiceMov.DirContractorIDOrg;
                doc.DirContractorIDOrg = docServiceMov.DirContractorIDOrg;
                doc.Discount = docServiceMov.Discount;
                doc.DirVatValue = docServiceMov.DirVatValue;
                doc.Base = docServiceMov.Base;
                doc.Description = docServiceMov.Description;
                doc.DocDate = docServiceMov.DocDate;
                //doc.DocDisc = docServiceMov.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docServiceMov.DocID;
                doc.DocIDBase = docServiceMov.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docServiceMov" со всем полями!
                docServiceMov.DocID = doc.DocID;

                #endregion

                #region 4. DescriptionMovement: пишем ID-шник в DocServiceMovTab и Rem2Party

                string DescriptionMovement = ""; if (docServiceMovTabCollection.Length > 0) DescriptionMovement = docServiceMov.DescriptionMovement;
                int? DirMovementDescriptionID = null;
                if (!String.IsNullOrEmpty(DescriptionMovement))
                {
                    //Алгоритм:
                    //1. Проверяем, если уже такая запись есть (маленькими буквами), то не сохраняем
                    //2. Иначе сохраняем
                    var queryDirMovementDescriptions = await
                        (
                            from x in dbRead.DirMovementDescriptions
                            where x.DirMovementDescriptionName.ToLower() == DescriptionMovement.ToLower()
                            select x
                        ).ToListAsync();

                    if (queryDirMovementDescriptions.Count() == 0)
                    {
                        //Модель
                        Models.Sklad.Dir.DirMovementDescription dirMovementDescription = new Models.Sklad.Dir.DirMovementDescription();
                        dirMovementDescription.DirMovementDescriptionName = DescriptionMovement;
                        db.Entry(dirMovementDescription).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        DirMovementDescriptionID = dirMovementDescription.DirMovementDescriptionID;
                    }
                    else
                    {
                        DirMovementDescriptionID = queryDirMovementDescriptions[0].DirMovementDescriptionID;
                    }
                }

                #endregion

                #region 2. DocServiceMov *** *** *** *** *** *** *** *** *** ***

                //Если ещё не переместили в Логистику
                if (docServiceMov.DirMovementStatusID == null || docServiceMov.DirMovementStatusID <= 1)
                {
                    if (docServiceMov.DirEmployeeIDCourier > 0) docServiceMov.DirMovementStatusID = 2;
                    else docServiceMov.DirMovementStatusID = 1;
                }
                //Если редактируем в Логистики
                else
                {

                }

                docServiceMov.DocID = doc.DocID;
                docServiceMov.DirMovementDescriptionID = DirMovementDescriptionID;

                db.Entry(docServiceMov).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docServiceMov.doc.NumberInt == null || docServiceMov.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docServiceMov.DocServiceMovID.ToString();
                    doc.NumberReal = docServiceMov.DocServiceMovID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docServiceMov.DocServiceMovID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocServiceMovTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocServiceMovID = new SQLiteParameter("@DocServiceMovID", System.Data.DbType.Int32) { Value = docServiceMov.DocServiceMovID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocServiceMovTabs WHERE DocServiceMovID=@DocServiceMovID;", parDocServiceMovID);
                }

                //2.2. Проставляем ID-шник "DocServiceMovID" для всех позиций спецификации
                for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                {
                    docServiceMovTabCollection[i].DocServiceMovTabID = null;
                    docServiceMovTabCollection[i].DocServiceMovID = Convert.ToInt32(docServiceMov.DocServiceMovID);
                    db.Entry(docServiceMovTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion


                if (UO_Action == "held")
                {

                    //Алгоритм:
                    //1. Сначало списываем товар
                    //2. Потом приходуем товар
                    //П.С. Как бы смесь Расхода + Приход
                    #region OLD

                    /*

                    //1. === === === Списание === === ===

                    Controllers.Sklad.Rem.Rem2PartyMinusesController rem2PartyMinuses = new Rem.Rem2PartyMinusesController();


                    #region Удаляем все записи из таблицы "Rem2PartyMinuses"
                    //Удаляем все записи из таблицы "Rem2PartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => rem2PartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (Rem2PartyMinuses)

                    DateTime? DocDatePurches = new DateTime();
                    //Models.Sklad.Rem.Rem2PartyMinus[] rem2PartyMinusCollection = new Models.Sklad.Rem.Rem2PartyMinus[docServiceMovTabCollection.Count()];
                    for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                    {
                        #region Проверки

                        //Переменные
                        int iRem2PartyID = docServiceMovTabCollection[i].Rem2PartyID;
                        double dQuantity = docServiceMovTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.Rem2Party rem2Party1 = await db.Rem2Parties.FindAsync(iRem2PartyID);
                        db.Entry(rem2Party1).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (rem2Party1.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docServiceMovTabCollection[i].Rem2PartyID + "</td>" +                        //партия
                                "<td>" + docServiceMovTabCollection[i].DirServiceNomenID + "</td>" +                        //Код товара
                                "<td>" + docServiceMovTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                                "<td>" + rem2Party1.Remnant + "</td>" +                                              //остаток партии
                                "<td>" + (docServiceMovTabCollection[i].Quantity - rem2Party1.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (rem2Party1.DirWarehouseID != docServiceMov.DirWarehouseIDFrom)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docServiceMov.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docServiceMov.DirWarehouseIDFrom);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docServiceMovTabCollection[i].Rem2PartyID + "</td>" +       //партия
                                "<td>" + docServiceMovTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + rem2Party1.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (rem2Party1.DirContractorIDOrg != docServiceMov.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docServiceMov.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docServiceMov.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docServiceMovTabCollection[i].Rem2PartyID + "</td>" +       //партия
                                "<td>" + docServiceMovTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                                "<td>" + rem2Party1.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg106_1
                            );
                        }
                        #endregion


                        //Если всё нормально, то меняем некоторые значения: DirWarehouseIDPurch и DirContractorID
                        docServiceMovTabCollection[i].DirWarehouseIDPurch = rem2Party1.DirWarehouseID; //Склад на который пришла партия первоначально
                        DocDatePurches = rem2Party1.DocDatePurches;

                        #endregion


                        #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                        //Получаем "DocID" из списуемой партии "docServiceMovTabCollection[i].DocID" для "DocIDFirst"
                        Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docServiceMovTabCollection[i].Rem2PartyID);

                        #endregion


                        #region Списание

                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                        rem2PartyMinus.Rem2PartyMinusID = null;
                        rem2PartyMinus.Rem2PartyID = docServiceMovTabCollection[i].Rem2PartyID;
                        rem2PartyMinus.DirServiceNomenID = docServiceMovTabCollection[i].DirServiceNomenID;
                        rem2PartyMinus.Quantity = docServiceMovTabCollection[i].Quantity;
                        rem2PartyMinus.DirCurrencyID = docServiceMovTabCollection[i].DirCurrencyID;
                        rem2PartyMinus.DirCurrencyMultiplicity = docServiceMovTabCollection[i].DirCurrencyMultiplicity;
                        rem2PartyMinus.DirCurrencyRate = docServiceMovTabCollection[i].DirCurrencyRate;
                        rem2PartyMinus.DirVatValue = docServiceMov.DirVatValue;
                        rem2PartyMinus.DirWarehouseID = docServiceMov.DirWarehouseIDFrom;
                        rem2PartyMinus.DirContractorIDOrg = docServiceMov.DirContractorIDOrg;
                        rem2PartyMinus.DirServiceContractorID = _Rem2Party.DirServiceContractorID; //rem2PartyMinus.DirServiceContractorID = null;
                        rem2PartyMinus.DocID = Convert.ToInt32(docServiceMov.DocID);
                        rem2PartyMinus.PriceCurrency = docServiceMovTabCollection[i].PriceCurrency;
                        rem2PartyMinus.PriceVAT = docServiceMovTabCollection[i].PriceVAT;
                        rem2PartyMinus.FieldID = Convert.ToInt32(docServiceMovTabCollection[i].DocServiceMovTabID);
                        rem2PartyMinus.Reserve = docServiceMov.Reserve;

                        rem2PartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        rem2PartyMinus.DocDate = doc.DocDate;

                        db.Entry(rem2PartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion
                    }

                    #endregion


                    //2. === === === Оприходование === === ===
                    if (UO_Action == "held")
                    {
                        #region Rem2Party - Партии

                        Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[docServiceMovTabCollection.Count()];
                        for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                        {
                            #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                            //Получаем "DocID" из списуемой партии "docServiceMovTabCollection[i].DocID" для "DocIDFirst"
                            Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docServiceMovTabCollection[i].Rem2PartyID);

                            #endregion


                            Models.Sklad.Rem.Rem2Party rem2Party = new Models.Sklad.Rem.Rem2Party();
                            rem2Party.Rem2PartyID = null;
                            rem2Party.DirServiceNomenID = docServiceMovTabCollection[i].DirServiceNomenID;
                            rem2Party.Quantity = docServiceMovTabCollection[i].Quantity;
                            rem2Party.Remnant = docServiceMovTabCollection[i].Quantity;
                            rem2Party.DirCurrencyID = docServiceMovTabCollection[i].DirCurrencyID;
                            //rem2Party.DirCurrencyMultiplicity = docServiceMovTabCollection[i].DirCurrencyMultiplicity;
                            //rem2Party.DirCurrencyRate = docServiceMovTabCollection[i].DirCurrencyRate;
                            rem2Party.DirVatValue = docServiceMov.DirVatValue;
                            rem2Party.DirWarehouseID = docServiceMov.DirWarehouseIDTo;
                            rem2Party.DirWarehouseIDDebit = docServiceMov.DirWarehouseIDFrom; //Склад с которого списали партию
                            rem2Party.DirWarehouseIDPurch = docServiceMovTabCollection[i].DirWarehouseIDPurch;     //Склад на который пришла партия первоначально (получили Выше)
                            rem2Party.DirContractorIDOrg = docServiceMov.DirContractorIDOrg;

                            rem2Party.DocDatePurches = DocDatePurches;

                            rem2Party.DocID = Convert.ToInt32(docServiceMov.DocID);
                            rem2Party.PriceCurrency = docServiceMovTabCollection[i].PriceCurrency;
                            rem2Party.PriceVAT = docServiceMovTabCollection[i].PriceVAT;
                            rem2Party.FieldID = Convert.ToInt32(docServiceMovTabCollection[i].DocServiceMovTabID);

                            rem2Party.PriceRetailVAT = docServiceMovTabCollection[i].PriceRetailVAT;
                            rem2Party.PriceRetailCurrency = docServiceMovTabCollection[i].PriceRetailCurrency;
                            rem2Party.PriceWholesaleVAT = docServiceMovTabCollection[i].PriceWholesaleVAT;
                            rem2Party.PriceWholesaleCurrency = docServiceMovTabCollection[i].PriceWholesaleCurrency;
                            rem2Party.PriceIMVAT = docServiceMovTabCollection[i].PriceIMVAT;
                            rem2Party.PriceIMCurrency = docServiceMovTabCollection[i].PriceIMCurrency;

                            rem2Party.DirEmployeeID = doc.DirEmployeeID;
                            rem2Party.DocDate = doc.DocDate;
                            rem2Party.DirNomenMinimumBalance = 1;

                            //***
                            rem2Party.DocIDFirst = _Rem2Party.DocIDFirst;
                            rem2Party.DirServiceContractorID = _Rem2Party.DirServiceContractorID;
                            //***

                            rem2PartyCollection[i] = rem2Party;
                        }

                        Controllers.Sklad.Rem.Rem2PartiesController rem2Partys = new Rem.Rem2PartiesController();
                        await Task.Run(() => rem2Partys.Save(db, rem2PartyCollection)); //rem2Partys.Save(db, rem2PartyCollection);

                        #endregion
                    }

                    */

                    #endregion



                    //Алгоритм:
                    //1. Менять склад аппарата в "DocServicePurches"
                    //2. Писать в Лог о перемещении в "LogServices" (внутри первого)

                    #region 1. Менять склад аппарата в "DocServicePurches" + Статус

                    for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                    {
                        Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServiceMovTabCollection[i].DocServicePurchID);
                        if (docServicePurch.DirWarehouseID == docServiceMov.DirWarehouseIDFrom)
                        {
                            docServicePurch.DirWarehouseID = docServiceMov.DirWarehouseIDTo;
                            docServicePurch.DateStatusChange = DateTime.Now;
                            docServicePurch.DirServiceStatusID = 6;
                        }
                        else
                        {
                            throw new System.InvalidOperationException("Не найден на точке аппарат №" + docServiceMovTabCollection[i].DocServicePurchID);
                        }
                        db.Entry(docServicePurch).State = EntityState.Modified;
                        await db.SaveChangesAsync();


                        //2. Писать в Лог о перемещении в "LogServices"
                        #region 4. Log

                        Models.Sklad.Log.LogService logService2 = new Models.Sklad.Log.LogService();
                        Controllers.Sklad.Log.LogServicesController logServicesController2 = new Log.LogServicesController();

                        logService2.DocServicePurchID = docServicePurch.DocServicePurchID;
                        logService2.DirServiceLogTypeID = 12;
                        logService2.DirEmployeeID = field.DirEmployeeID;
                        logService2.DirServiceStatusID = docServicePurch.DirServiceStatusID;
                        logService2.DirWarehouseIDFrom = docServiceMov.DirWarehouseIDFrom;
                        logService2.DirWarehouseIDTo = docServiceMov.DirWarehouseIDTo;
                        logService2.Msg = " (так же сменён статус)";

                        await logServicesController2.mPutPostLogServices(db, logService2, EntityState.Added);

                        #endregion
                    }

                    #endregion


                    if (UOSms == 1)
                    {
                        int DirWarehouseIDFrom = docServiceMov.DirWarehouseIDFrom;
                        Models.Sklad.Dir.DirWarehouse dirWarehouseFrom = await db.DirWarehouses.FindAsync(DirWarehouseIDFrom);

                        //Находим Phone точки на которую перемещаем:
                        int DirWarehouseIDTo = docServiceMov.DirWarehouseIDTo;
                        Models.Sklad.Dir.DirWarehouse dirWarehouseTo = await db.DirWarehouses.FindAsync(DirWarehouseIDTo);
                        if (dirWarehouseTo.Phone != null && dirWarehouseTo.Phone.Length >= 5)
                        {
                            //Получаем DirSmsTemplateMsg из DirSmsTemplates
                            Models.Sklad.Dir.DirSmsTemplate dirSmsTemplate = await db.DirSmsTemplates.FindAsync(8);
                            dirSmsTemplate.DirSmsTemplateMsg = dirSmsTemplate.DirSmsTemplateMsg.Replace("[[[ДокументНомер]]]", docServiceMov.DocServiceMovID.ToString());
                            dirSmsTemplate.DirSmsTemplateMsg = dirSmsTemplate.DirSmsTemplateMsg.Replace("[[[ТочкаОт]]]", dirWarehouseFrom.DirWarehouseName);

                            PartionnyAccount.Classes.SMS.infobip_com infobip_com = new Classes.SMS.infobip_com();
                            string res = infobip_com.Send(sysSetting, dirWarehouseTo.Phone, dirSmsTemplate.DirSmsTemplateMsg);
                        }
                    }

                }
            }
            else if (UO_Action == "held_cancel")
            {
                #region OLD

                /*
                //Т.к. Мы и приходуем товар, то:
                #region 1. Проверка: Было ли списание с партий *** *** *** *** *** *** *** *** *** ***
                
                //Алгоритм №1
                //SELECT DocID
                //FROM Rem2PartyMinuses 
                //WHERE Rem2PartyID in (SELECT Rem2PartyID FROM Rem2Parties WHERE DocID=@DocID)

                
                int DocServiceMovID = Convert.ToInt32(docServiceMov.DocServiceMovID);

                //Получаем DocPurch из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocServiceMov _docServiceMov = db.DocServiceMovs.Find(DocServiceMovID);
                int? iDocServiceMov_DocID = _docServiceMov.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocServiceMov_DocID));

                #endregion


                #region 1. Rem2PartyMinuses и Rem2Parties *** *** *** *** *** *** *** *** *** ***

                //Удаление записей в таблицах: Rem2PartyMinuses
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docServiceMov.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Rem2Parties WHERE DocID=@DocID;", parDocID); //DELETE FROM Rem2PartyMinuses WHERE DocID=@DocID;

                //Обновление записей: Rem2PartyMinuses
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE Rem2PartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion

                */

                #endregion



                #region 1. Проверка: продали товар

                for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServiceMovTabCollection[i].DocServicePurchID);
                    if (docServicePurch.DirServiceStatusID > 9)
                    {
                        throw new System.InvalidOperationException("Аппарт уже продан или разобран №" + docServiceMovTabCollection[i].DocServicePurchID);
                    }
                }

                #endregion


                #region 2. Менем точку + Статус

                for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServiceMovTabCollection[i].DocServicePurchID);
                    docServicePurch.DirWarehouseID = docServiceMov.DirWarehouseIDFrom;
                    docServicePurch.DateStatusChange = DateTime.Now;
                    docServicePurch.DirServiceStatusID = docServiceMovTabCollection[i].DirServiceStatusID;

                    db.Entry(docServicePurch).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                #endregion


                //Doc.Held = false
                #region 3. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docServiceMov.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion
            }


            #region 3. Лог


            #region Пишем в Лог о смене статуса и мастера, если такое было

            logLogistic.DocLogisticID = docServiceMov.DocServiceMovID;
            logLogistic.DirMovementLogTypeID = 1; //Смена статуса
            logLogistic.DirEmployeeID = field.DirEmployeeID;
            logLogistic.DirMovementStatusID = 1;
            //logLogistic.Msg = "Создание документа";

            await logLogisticsController.mPutPostLogLogistics(db, logLogistic, EntityState.Added);

            #endregion


            #endregion



            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docServiceMov;
        }



        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            //System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocServiceMov docServiceMov,
            int id,
            int StatusID,
            string sDiagnosticRresults,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region Проверка, если предыдущий статус такой же на который меняем, то не писать в Лог

            //Исключение, т.к. если в Логе нет записей с сменой статуса получим Ошибку из-за "FirstAsync()"
            try
            {
                var query = await
                    (
                        from x in db.LogLogistics
                        where x.DocLogisticID == id && x.DocLogisticID != null
                        select new
                        {
                            LogLogisticID = x.LogLogisticID,
                            DirMovementStatusID = x.DirMovementStatusID
                        }
                    ).OrderByDescending(x => x.LogLogisticID).FirstAsync();

                if (query.DirMovementStatusID == StatusID)
                {
                    return false;
                }
            }
            catch (Exception ex) { }

            #endregion


            #region 1. Сохранение статуса в БД

            docServiceMov.DirMovementStatusID = StatusID;
            db.Entry(docServiceMov).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 2. Если статус == 4, то проводим

            if (StatusID == 4)
            {

                #region 2.1. Doc

                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docServiceMov.DocID);
                doc.Held = true;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                #endregion




                #region Получаем колеуцию табличной части

                //1. К-во записец
                var queryTabs = await
                    (
                        from x in db.DocServiceMovTabs
                        where x.DocServiceMovID == docServiceMov.DocServiceMovID
                        select x
                    ).ToListAsync();

                Models.Sklad.Doc.DocServiceMovTab[] docServiceMovTabCollection = new Models.Sklad.Doc.DocServiceMovTab[queryTabs.Count()];

                for (int i = 0; i < queryTabs.Count(); i++)
                {
                    Models.Sklad.Doc.DocServiceMovTab docServiceMovTab = await db.DocServiceMovTabs.FindAsync(queryTabs[0].DocServiceMovTabID);
                    docServiceMovTabCollection[0] = docServiceMovTab;
                }


                #endregion


                #region Party
                //Алгоритм:
                //1. Сначало списываем товар
                //2. Потом приходуем товар
                //П.С. Как бы смесь Расхода + Приход

                /*

                //1. === === === Списание === === ===

                Controllers.Sklad.Rem.Rem2PartyMinusesController rem2PartyMinuses = new Rem.Rem2PartyMinusesController();


                #region Удаляем все записи из таблицы "Rem2PartyMinuses"
                //Удаляем все записи из таблицы "Rem2PartyMinuses"
                //Что бы правильно Проверяло на Остаток.
                //А то, товар уже списан, а я проверяю на остаток!

                await Task.Run(() => rem2PartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                #endregion


                #region Проверки и Списание с партий (Rem2PartyMinuses)

                DateTime? DocDatePurches = new DateTime();
                //Models.Sklad.Rem.Rem2PartyMinus[] rem2PartyMinusCollection = new Models.Sklad.Rem.Rem2PartyMinus[docServiceMovTabCollection.Count()];
                for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                {
                    #region Проверки

                    //Переменные
                    int iRem2PartyID = docServiceMovTabCollection[i].Rem2PartyID;
                    double dQuantity = docServiceMovTabCollection[i].Quantity;
                    //Находим партию
                    Models.Sklad.Rem.Rem2Party rem2Party1 = await db.Rem2Parties.FindAsync(iRem2PartyID);
                    db.Entry(rem2Party1).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                    #region 1. Есть ли остаток в партии с которой списываем!
                    if (rem2Party1.Remnant < dQuantity)
                    {
                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg104 +

                            "<tr>" +
                            "<td>" + docServiceMovTabCollection[i].Rem2PartyID + "</td>" +                        //партия
                            "<td>" + docServiceMovTabCollection[i].DirServiceNomenID + "</td>" +                        //Код товара
                            "<td>" + docServiceMovTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                            "<td>" + rem2Party1.Remnant + "</td>" +                                              //остаток партии
                            "<td>" + (docServiceMovTabCollection[i].Quantity - rem2Party1.Remnant).ToString() + "</td>" +  //недостающее к-во
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg104_1
                        );
                    }
                    #endregion

                    #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                    if (rem2Party1.DirWarehouseID != docServiceMov.DirWarehouseIDFrom)
                    {
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docServiceMov.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docServiceMov.DirWarehouseIDFrom);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg105 +

                            "<tr>" +
                            "<td>" + docServiceMovTabCollection[i].Rem2PartyID + "</td>" +       //партия
                            "<td>" + docServiceMovTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                            "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                            "<td>" + rem2Party1.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg105_1
                        );
                    }
                    #endregion

                    #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                    if (rem2Party1.DirContractorIDOrg != doc.DirContractorIDOrg)
                    {
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docServiceMov.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docServiceMov.DirContractorIDOrg);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg106 +

                            "<tr>" +
                            "<td>" + docServiceMovTabCollection[i].Rem2PartyID + "</td>" +       //партия
                            "<td>" + docServiceMovTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                            "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                            "<td>" + rem2Party1.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg106_1
                        );
                    }
                    #endregion


                    //Если всё нормально, то меняем некоторые значения: DirWarehouseIDPurch и DirContractorID
                    docServiceMovTabCollection[i].DirWarehouseIDPurch = rem2Party1.DirWarehouseID; //Склад на который пришла партия первоначально
                    DocDatePurches = rem2Party1.DocDatePurches;

                    #endregion


                    #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                    //Получаем "DocID" из списуемой партии "docServiceMovTabCollection[i].DocID" для "DocIDFirst"
                    Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docServiceMovTabCollection[i].Rem2PartyID);

                    #endregion


                    #region Сохранение

                    Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                    rem2PartyMinus.Rem2PartyMinusID = null;
                    rem2PartyMinus.Rem2PartyID = docServiceMovTabCollection[i].Rem2PartyID;
                    rem2PartyMinus.DirServiceNomenID = docServiceMovTabCollection[i].DirServiceNomenID;
                    rem2PartyMinus.Quantity = docServiceMovTabCollection[i].Quantity;
                    rem2PartyMinus.DirCurrencyID = docServiceMovTabCollection[i].DirCurrencyID;
                    rem2PartyMinus.DirCurrencyMultiplicity = docServiceMovTabCollection[i].DirCurrencyMultiplicity;
                    rem2PartyMinus.DirCurrencyRate = docServiceMovTabCollection[i].DirCurrencyRate;
                    rem2PartyMinus.DirVatValue = docServiceMov.DirVatValue;
                    rem2PartyMinus.DirWarehouseID = docServiceMov.DirWarehouseIDFrom;
                    rem2PartyMinus.DirServiceContractorID = _Rem2Party.DirServiceContractorID; //rem2PartyMinus.DirServiceContractorID = null;
                    rem2PartyMinus.DirContractorIDOrg = doc.DirContractorIDOrg;
                    rem2PartyMinus.DocID = Convert.ToInt32(doc.DocID);
                    rem2PartyMinus.PriceCurrency = docServiceMovTabCollection[i].PriceCurrency;
                    rem2PartyMinus.PriceVAT = docServiceMovTabCollection[i].PriceVAT;
                    rem2PartyMinus.FieldID = Convert.ToInt32(docServiceMovTabCollection[i].DocServiceMovTabID);
                    rem2PartyMinus.Reserve = docServiceMov.Reserve;

                    rem2PartyMinus.DirEmployeeID = doc.DirEmployeeID;
                    rem2PartyMinus.DocDate = doc.DocDate;

                    db.Entry(rem2PartyMinus).State = EntityState.Added;
                    await db.SaveChangesAsync();

                    #endregion
                }

                #endregion


                #region Rem2Party - Партии

                Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[docServiceMovTabCollection.Count()];
                for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                {
                    #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                    //Получаем "DocID" из списуемой партии "docServiceMovTabCollection[i].DocID" для "DocIDFirst"
                    Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docServiceMovTabCollection[i].Rem2PartyID);

                    #endregion


                    Models.Sklad.Rem.Rem2Party rem2Party = new Models.Sklad.Rem.Rem2Party();
                    rem2Party.Rem2PartyID = null;
                    rem2Party.DirServiceNomenID = docServiceMovTabCollection[i].DirServiceNomenID;
                    rem2Party.Quantity = docServiceMovTabCollection[i].Quantity;
                    rem2Party.Remnant = docServiceMovTabCollection[i].Quantity;
                    rem2Party.DirCurrencyID = docServiceMovTabCollection[i].DirCurrencyID;
                    //rem2Party.DirCurrencyMultiplicity = docServiceMovTabCollection[i].DirCurrencyMultiplicity;
                    //rem2Party.DirCurrencyRate = docServiceMovTabCollection[i].DirCurrencyRate;
                    rem2Party.DirVatValue = docServiceMov.DirVatValue;
                    rem2Party.DirWarehouseID = docServiceMov.DirWarehouseIDTo;
                    rem2Party.DirWarehouseIDDebit = docServiceMov.DirWarehouseIDFrom; //Склад с которого списали партию
                    rem2Party.DirWarehouseIDPurch = docServiceMovTabCollection[i].DirWarehouseIDPurch;     //Склад на который пришла партия первоначально (получили Выше)
                    rem2Party.DirContractorIDOrg = doc.DirContractorIDOrg;

                    rem2Party.DocDatePurches = DocDatePurches;

                    rem2Party.DocID = Convert.ToInt32(docServiceMov.DocID);
                    rem2Party.PriceCurrency = docServiceMovTabCollection[i].PriceCurrency;
                    rem2Party.PriceVAT = docServiceMovTabCollection[i].PriceVAT;
                    rem2Party.FieldID = Convert.ToInt32(docServiceMovTabCollection[i].DocServiceMovTabID);

                    rem2Party.PriceRetailVAT = docServiceMovTabCollection[i].PriceRetailVAT;
                    rem2Party.PriceRetailCurrency = docServiceMovTabCollection[i].PriceRetailCurrency;
                    rem2Party.PriceWholesaleVAT = docServiceMovTabCollection[i].PriceWholesaleVAT;
                    rem2Party.PriceWholesaleCurrency = docServiceMovTabCollection[i].PriceWholesaleCurrency;
                    rem2Party.PriceIMVAT = docServiceMovTabCollection[i].PriceIMVAT;
                    rem2Party.PriceIMCurrency = docServiceMovTabCollection[i].PriceIMCurrency;

                    rem2Party.DirEmployeeID = doc.DirEmployeeID;
                    rem2Party.DocDate = doc.DocDate;
                    rem2Party.DirNomenMinimumBalance = 1;

                    //***
                    rem2Party.DocIDFirst = _Rem2Party.DocIDFirst;
                    rem2Party.DirServiceContractorID = _Rem2Party.DirServiceContractorID;
                    //***

                    rem2PartyCollection[i] = rem2Party;
                }

                Controllers.Sklad.Rem.Rem2PartiesController rem2Partys = new Rem.Rem2PartiesController();
                await Task.Run(() => rem2Partys.Save(db, rem2PartyCollection)); //rem2Partys.Save(db, rem2PartyCollection);

                #endregion

                */

                #endregion



                #region 1. Менять склад аппарата в "DocServicePurches"

                for (int i = 0; i < docServiceMovTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(docServiceMovTabCollection[i].DocServicePurchID);
                    if (docServicePurch.DirWarehouseID == docServiceMov.DirWarehouseIDFrom)
                    {
                        docServicePurch.DirWarehouseID = docServiceMov.DirWarehouseIDTo;
                    }
                    else
                    {
                        throw new System.InvalidOperationException("Не найден на точке аппарат №" + docServiceMovTabCollection[i].DocServicePurchID);
                    }
                    db.Entry(docServiceMovTabCollection[i]).State = EntityState.Added;
                    await db.SaveChangesAsync();


                    //2. Писать в Лог о перемещении в "LogServices"
                    #region 4. Log

                    logService.DocServicePurchID = docServicePurch.DocServicePurchID;
                    logService.DirServiceLogTypeID = 15;
                    logService.DirEmployeeID = field.DirEmployeeID;
                    logService.DirServiceStatusID = docServicePurch.DirServiceStatusID;
                    logService.DirWarehouseIDFrom = docServiceMov.DirWarehouseIDFrom;
                    logService.DirWarehouseIDTo = docServiceMov.DirWarehouseIDTo;
                    //logService.Msg = "Аппарат принят на точку №" + docServicePurch.DirWarehouseID;

                    await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

                    #endregion
                }

                #endregion

            }

            #endregion


            #region 3. Лог


            #region Пишем в Лог о смене статуса и мастера, если такое было

            logLogistic.DocLogisticID = id;
            logLogistic.DirMovementLogTypeID = 1; //Смена статуса
            logLogistic.DirEmployeeID = field.DirEmployeeID;
            logLogistic.DirMovementStatusID = StatusID;
            if (!String.IsNullOrEmpty(sDiagnosticRresults) && StatusID != 3) logLogistic.Msg = sDiagnosticRresults;
            else logLogistic.Msg = docServiceMov.dirEmployee_Courier.DirEmployeeName;

            await logLogisticsController.mPutPostLogLogistics(db, logLogistic, EntityState.Added);

            #endregion


            #endregion


            return true;
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
                "[DocServiceMovs].[DocServiceMovID] AS [DocServiceMovID], " +
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

                //"[DirWarehousesFrom].[DirWarehouseID] AS [DirWarehouseIDFrom], " +
                "[DirWarehousesFrom].[DirWarehouseName] AS [DirWarehouseNameFrom], " +
                "[DirWarehousesFrom].[DirWarehouseAddress] AS [DirWarehouseAddressFrom], " +
                "[DirWarehousesFrom].[DirWarehouseDesc] AS [DirWarehouseDescFrom], " +

                //"[DirWarehousesTo].[DirWarehouseID] AS [DirWarehouseIDTo], " +
                "[DirWarehousesTo].[DirWarehouseName] AS [DirWarehouseNameTo], " +
                "[DirWarehousesTo].[DirWarehouseAddress] AS [DirWarehouseAddressTo], " +
                "[DirWarehousesTo].[DirWarehouseDesc] AS [DirWarehouseDescTo], " +

                "[Docs].[NumberInt] AS [NumberInt], " +
                "[DocServiceMovs].[Reserve] AS [Reserve] " +

                "FROM [DocServiceMovs] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocServiceMovs].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "INNER JOIN [DirWarehouses] AS [DirWarehousesFrom] ON [DirWarehousesFrom].[DirWarehouseID] = [DocServiceMovs].[DirWarehouseIDFrom] " +
                "INNER JOIN [DirWarehouses] AS [DirWarehousesTo] ON [DirWarehousesTo].[DirWarehouseID] = [DocServiceMovs].[DirWarehouseIDTo] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion

    }
}