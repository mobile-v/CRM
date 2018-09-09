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
    public class DocSecondHandMovsController : ApiController
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
        Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 71;

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
            public int? DocSecondHandMovID;
        }
        // GET: api/DocSecondHandMovs
        public async Task<IHttpActionResult> GetDocSecondHandMovs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovementsLogistics"));
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
                _params.DocSecondHandMovID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandMovID", true) == 0).Value);

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
                        from docSecondHandMovs in db.DocSecondHandMovs

                        join docSecondHandMovTabs1 in db.DocSecondHandMovTabs on docSecondHandMovs.DocSecondHandMovID equals docSecondHandMovTabs1.DocSecondHandMovID into docSecondHandMovTabs2
                        from docSecondHandMovTabs in docSecondHandMovTabs2.DefaultIfEmpty()

                            //where docSecondHandMovs.doc.DocDate >= _params.DateS && docSecondHandMovs.doc.DocDate <= _params.DatePo

                        group new { docSecondHandMovTabs }
                        by new
                        {
                            DocID = docSecondHandMovs.DocID,
                            DocDate = docSecondHandMovs.doc.DocDate,
                            Base = docSecondHandMovs.doc.Base,
                            Held = docSecondHandMovs.doc.Held,
                            Discount = docSecondHandMovs.doc.Discount,
                            Del = docSecondHandMovs.doc.Del,
                            Description = docSecondHandMovs.doc.Description,
                            IsImport = docSecondHandMovs.doc.IsImport,
                            DirVatValue = docSecondHandMovs.doc.DirVatValue,

                            DocSecondHandMovID = docSecondHandMovs.DocSecondHandMovID,
                            //DirContractorName = docSecondHandMovs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandMovs.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandMovs.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docSecondHandMovs.dirWarehouseFrom.DirWarehouseID,
                            DirWarehouseNameFrom = docSecondHandMovs.dirWarehouseFrom.DirWarehouseName,
                            DirWarehouseIDTo = docSecondHandMovs.dirWarehouseTo.DirWarehouseID,
                            DirWarehouseNameTo = docSecondHandMovs.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docSecondHandMovs.doc.NumberInt,

                            DirEmployeeIDCourier = docSecondHandMovs.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docSecondHandMovs.dirEmployee_Courier.DirEmployeeName,
                            DirMovementStatusID = docSecondHandMovs.DirMovementStatusID, //Курьер штрихнулся и забрал товар

                            DirEmployeeName = docSecondHandMovs.doc.dirEmployee.DirEmployeeName,
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

                            DocSecondHandMovID = g.Key.DocSecondHandMovID,
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
                            g.Sum(x => x.docSecondHandMovTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandMovTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Отобразить "Предыдущие ремнты"

                if (_params.DocSecondHandMovID != null && _params.DocSecondHandMovID > 0)
                {
                    query = query.Where(x => x.DocSecondHandMovID == _params.DocSecondHandMovID);
                }

                #endregion


                #region dirEmployee: dirEmployee.DirWarehouseID and/or dirEmployee.DirContractorIDOrg

                /*if (_params.DirWarehouseID != null && _params.DirWarehouseID > 0)
                {
                    query = query.Where(x => x.DirWarehouseIDTo == _params.DirWarehouseID);
                }*/
                //Если привязка к сотруднику (для документа "DocSecondHandMovs" показать все склады)
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
                        query = query.Where(x => x.DocSecondHandMovID == iNumber32);
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

                query = query.OrderByDescending(x => x.DocSecondHandMovID).Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocSecondHandMovs.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSecondHandMov = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandMovs/5
        [ResponseType(typeof(DocSecondHandMov))]
        public async Task<IHttpActionResult> GetDocSecondHandMov(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovementsLogistics"));
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
                        from docSecondHandMovs in db.DocSecondHandMovs
                        where docSecondHandMovs.DocSecondHandMovID == id
                        select docSecondHandMovs
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docSecondHandMovs in db.DocSecondHandMovs

                        join docSecondHandMovTabs1 in db.DocSecondHandMovTabs on docSecondHandMovs.DocSecondHandMovID equals docSecondHandMovTabs1.DocSecondHandMovID into docSecondHandMovTabs2
                        from docSecondHandMovTabs in docSecondHandMovTabs2.DefaultIfEmpty()

                        #endregion

                        where docSecondHandMovs.DocSecondHandMovID == id

                        #region group

                        group new { docSecondHandMovTabs }
                        by new
                        {
                            DocID = docSecondHandMovs.DocID, //DocID = docSecondHandMovs.doc.DocID,
                            DocIDBase = docSecondHandMovs.doc.DocIDBase,
                            DocDate = docSecondHandMovs.doc.DocDate,
                            Base = docSecondHandMovs.doc.Base,
                            Held = docSecondHandMovs.doc.Held,
                            Discount = docSecondHandMovs.doc.Discount,
                            Del = docSecondHandMovs.doc.Del,
                            IsImport = docSecondHandMovs.doc.IsImport,
                            Description = docSecondHandMovs.doc.Description,
                            DirVatValue = docSecondHandMovs.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandMovs.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docSecondHandMovs.doc.dirPaymentType.DirPaymentTypeName,

                            DocSecondHandMovID = docSecondHandMovs.DocSecondHandMovID,
                            //DirContractorID = docSecondHandMovs.doc.DirContractorID,
                            //DirContractorName = docSecondHandMovs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandMovs.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docSecondHandMovs.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docSecondHandMovs.dirWarehouseFrom.DirWarehouseID,
                            DirWarehouseNameFrom = docSecondHandMovs.dirWarehouseFrom.DirWarehouseName,

                            DirWarehouseIDTo = docSecondHandMovs.dirWarehouseTo.DirWarehouseID,
                            DirWarehouseNameTo = docSecondHandMovs.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docSecondHandMovs.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandMovs.doc.Payment,

                            //Резерв
                            Reserve = docSecondHandMovs.Reserve,
                            DirMovementDescriptionName = docSecondHandMovs.dirMovementDescription.DirMovementDescriptionName,

                            DirMovementStatusID = docSecondHandMovs.DirMovementStatusID,
                            DirEmployeeIDCourier = docSecondHandMovs.DirEmployeeIDCourier,

                            LoadFrom = docSecondHandMovs.LoadFrom,
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

                            DocSecondHandMovID = g.Key.DocSecondHandMovID,
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
                            g.Sum(x => x.docSecondHandMovTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandMovTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Розница
                            SumRetail =
                            g.Sum(x => x.docSecondHandMovTabs.PriceRetailCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandMovTabs.PriceRetailCurrency), sysSetting.FractionalPartInSum),

                            /*
                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (x.docSecondHandMovTabs.PriceCurrency - (x.docSecondHandMovTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (x.docSecondHandMovTabs.PriceCurrency - (x.docSecondHandMovTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата
                            Payment = g.Key.Payment,
                            HavePay = g.Sum(x => x.docSecondHandMovTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x =>  x.docSecondHandMovTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),

                            //Резерв
                            Reserve = g.Key.Reserve,
                            */

                            DescriptionMovement = g.Key.DirMovementDescriptionName, //DirMovementDescriptionID = g.Key.DirMovementDescriptionID,

                            DirMovementStatusID = g.Key.DirMovementStatusID,
                            DirEmployeeIDCourier = g.Key.DirEmployeeIDCourier,

                            LoadFrom = g.Key.LoadFrom,
                            LoadXFrom = g.Key.LoadFrom,
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

        // PUT: api/DocSecondHandMovs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandMov(int id, DocSecondHandMov docSecondHandMov, HttpRequestMessage request)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
                //if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Возможно доступна Логистика
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
                if (iRight != 1)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovementsLogistics"));
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


                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocSecondHandMovTab[] docSecondHandMovTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandMov.recordsDocSecondHandMovTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandMovTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandMovTab[]>(docSecondHandMov.recordsDocSecondHandMovTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docSecondHandMov.DocSecondHandMovID || docSecondHandMov.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //2. Если статус > 1, то редактировать может только Администратор
                if (docSecondHandMov.DirMovementStatusID > 1 && field.DirEmployeeID != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg122)); }

                //3. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docSecondHandMov.DirEmployeeIDCourier != null && docSecondHandMov.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docSecondHandMov.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }

                //4. Получаем "docSecondHandMov.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandMov.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandMovs
                        where x.DocSecondHandMovID == docSecondHandMov.DocSecondHandMovID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandMov.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //5. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docSecondHandMov.DocID);
                //6.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //6.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandMov.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandMov = await Task.Run(() => mPutPostDocSecondHandMov(db, dbRead, UO_Action, docSecondHandMov, EntityState.Modified, docSecondHandMovTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandMov.DocSecondHandMovID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandMov.DocID,
                    DocSecondHandMovID = docSecondHandMov.DocSecondHandMovID,
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

        // PUT: api/DocSecondHandMovs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandMov(int id, int DirStatusID, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
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


                Models.Sklad.Doc.DocSecondHandMov docSecondHandMov = await db.DocSecondHandMovs.FindAsync(id);
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandMov.DocID);


                #region Проверки

                //1. Если документ проведён то выдать сообщение
                if (Convert.ToBoolean(doc.Held)) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3)); }

                //2. Нельзя перескакивать через статусы (статус всегда +/- 1)
                if (Math.Abs(DirStatusID - Convert.ToInt32(docSecondHandMov.DirMovementStatusID)) != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_4)); }

                //3. Если статус == 3, то проверяем введённый пароль и чистим его (что бы не писался в Лог)
                if (DirStatusID == 3)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(docSecondHandMov.DirEmployeeIDCourier);
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
                                if (docSecondHandMov.DirWarehouseIDTo == queryW[0].DirWarehouseID) { bFindWarehouse = true; break; }
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
                            //await mStatusChange(db, ts, docSecondHandMov, id, DirStatusID, sDiagnosticRresults, field);
                            await mStatusChange(db, dbRead, docSecondHandMov, id, DirStatusID, sDiagnosticRresults, field);

                            //docSecondHandMov = await Task.Run(() => mPutPostDocSecondHandMov(db, dbRead, UO_Action, docSecondHandMov, EntityState.Modified, docSecondHandMovTabCollection, field)); //sysSetting

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
                        DocID = docSecondHandMov.DocID,
                        DocSecondHandMovID = docSecondHandMov.DocSecondHandMovID
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


        // POST: api/DocSecondHandMovs
        [ResponseType(typeof(DocSecondHandMov))]
        public async Task<IHttpActionResult> PostDocSecondHandMov(DocSecondHandMov docSecondHandMov, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
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


                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocSecondHandMovTab[] docSecondHandMovTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandMov.recordsDocSecondHandMovTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandMovTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandMovTab[]>(docSecondHandMov.recordsDocSecondHandMovTab);
                }

                #endregion

                #region Проверки

                //1. 
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);


                //2. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docSecondHandMov.DirEmployeeIDCourier != null && docSecondHandMov.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docSecondHandMov.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }


                //3. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandMov.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandMov = await Task.Run(() => mPutPostDocSecondHandMov(db, dbRead, UO_Action, docSecondHandMov, EntityState.Added, docSecondHandMovTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandMov.DocSecondHandMovID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandMov.DocID,
                    DocSecondHandMovID = docSecondHandMov.DocSecondHandMovID,
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

        // DELETE: api/DocSecondHandMovs/5
        [ResponseType(typeof(DocSecondHandMov))]
        public async Task<IHttpActionResult> DeleteDocSecondHandMov(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
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
                            from x in dbRead.DocSecondHandMovs
                            where x.DocSecondHandMovID == id
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
                //2. DocSecondHandMovTabs
                //3. DocSecondHandMovs
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocSecondHandMov docSecondHandMov = await db.DocSecondHandMovs.FindAsync(id);
                if (docSecondHandMov == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Rem2PartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocSecondHandMovs
                                where x.DocSecondHandMovID == id
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


                        #region 2. DocSecondHandMovTabs *** *** *** *** ***

                        var queryDocSecondHandMovTabs = await
                            (
                                from x in db.DocSecondHandMovTabs
                                where x.DocSecondHandMovID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandMovTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandMovTab docSecondHandMovTab = await db.DocSecondHandMovTabs.FindAsync(queryDocSecondHandMovTabs[i].DocSecondHandMovTabID);
                            db.DocSecondHandMovTabs.Remove(docSecondHandMovTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocSecondHandMovs *** *** *** *** ***

                        var queryDocSecondHandMovs = await
                            (
                                from x in db.DocSecondHandMovs
                                where x.DocSecondHandMovID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandMovs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandMov docSecondHandMov1 = await db.DocSecondHandMovs.FindAsync(queryDocSecondHandMovs[i].DocSecondHandMovID);
                            db.DocSecondHandMovs.Remove(docSecondHandMov1);
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

        private bool DocSecondHandMovExists(int id)
        {
            return db.DocSecondHandMovs.Count(e => e.DocSecondHandMovID == id) > 0;
        }


        bool bFindWarehouse = false;
        internal async Task<DocSecondHandMov> mPutPostDocSecondHandMov(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandMov docSecondHandMov,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocSecondHandMovTab[] docSecondHandMovTabCollection,

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
                        if (docSecondHandMov.DirWarehouseIDTo == queryW[i].DirWarehouseID) { bFindWarehouse = true; break; }
                    }
                    if (!bFindWarehouse) UO_Action = "save";
                }
            }
            else
            {
                bFindWarehouse = true;
            }

            #endregion


            if (UO_Action == "held") docSecondHandMov.Reserve = false;
            else docSecondHandMov.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docSecondHandMov.NumberInt;
                doc.NumberReal = docSecondHandMov.DocSecondHandMovID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = 1;
                doc.Payment = 0;
                doc.DirContractorID = docSecondHandMov.DirContractorIDOrg;
                doc.DirContractorIDOrg = docSecondHandMov.DirContractorIDOrg;
                doc.Discount = docSecondHandMov.Discount;
                doc.DirVatValue = docSecondHandMov.DirVatValue;
                doc.Base = docSecondHandMov.Base;
                doc.Description = docSecondHandMov.Description;
                doc.DocDate = docSecondHandMov.DocDate;
                //doc.DocDisc = docSecondHandMov.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docSecondHandMov.DocID;
                doc.DocIDBase = docSecondHandMov.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docSecondHandMov" со всем полями!
                docSecondHandMov.DocID = doc.DocID;

                #endregion

                #region 4. DescriptionMovement: пишем ID-шник в DocSecondHandMovTab и Rem2Party

                string DescriptionMovement = ""; if (docSecondHandMovTabCollection.Length > 0) DescriptionMovement = docSecondHandMov.DescriptionMovement;
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

                #region 2. DocSecondHandMov *** *** *** *** *** *** *** *** *** ***

                //Если ещё не переместили в Логистику
                if (docSecondHandMov.DirMovementStatusID == null || docSecondHandMov.DirMovementStatusID <= 1)
                {
                    if (docSecondHandMov.DirEmployeeIDCourier > 0) docSecondHandMov.DirMovementStatusID = 2;
                    else docSecondHandMov.DirMovementStatusID = 1;
                }
                //Если редактируем в Логистики
                else
                {

                }

                docSecondHandMov.DocID = doc.DocID;
                docSecondHandMov.DirMovementDescriptionID = DirMovementDescriptionID;

                db.Entry(docSecondHandMov).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docSecondHandMov.doc.NumberInt == null || docSecondHandMov.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docSecondHandMov.DocSecondHandMovID.ToString();
                    doc.NumberReal = docSecondHandMov.DocSecondHandMovID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docSecondHandMov.DocSecondHandMovID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocSecondHandMovTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocSecondHandMovID = new SQLiteParameter("@DocSecondHandMovID", System.Data.DbType.Int32) { Value = docSecondHandMov.DocSecondHandMovID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocSecondHandMovTabs WHERE DocSecondHandMovID=@DocSecondHandMovID;", parDocSecondHandMovID);
                }

                //2.2. Проставляем ID-шник "DocSecondHandMovID" для всех позиций спецификации
                for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                {
                    docSecondHandMovTabCollection[i].DocSecondHandMovTabID = null;
                    docSecondHandMovTabCollection[i].DocSecondHandMovID = Convert.ToInt32(docSecondHandMov.DocSecondHandMovID);
                    db.Entry(docSecondHandMovTabCollection[i]).State = EntityState.Added;
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
                    //Models.Sklad.Rem.Rem2PartyMinus[] rem2PartyMinusCollection = new Models.Sklad.Rem.Rem2PartyMinus[docSecondHandMovTabCollection.Count()];
                    for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                    {
                        #region Проверки

                        //Переменные
                        int iRem2PartyID = docSecondHandMovTabCollection[i].Rem2PartyID;
                        double dQuantity = docSecondHandMovTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.Rem2Party rem2Party1 = await db.Rem2Parties.FindAsync(iRem2PartyID);
                        db.Entry(rem2Party1).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (rem2Party1.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docSecondHandMovTabCollection[i].Rem2PartyID + "</td>" +                        //партия
                                "<td>" + docSecondHandMovTabCollection[i].DirServiceNomenID + "</td>" +                        //Код товара
                                "<td>" + docSecondHandMovTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                                "<td>" + rem2Party1.Remnant + "</td>" +                                              //остаток партии
                                "<td>" + (docSecondHandMovTabCollection[i].Quantity - rem2Party1.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (rem2Party1.DirWarehouseID != docSecondHandMov.DirWarehouseIDFrom)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandMov.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandMov.DirWarehouseIDFrom);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docSecondHandMovTabCollection[i].Rem2PartyID + "</td>" +       //партия
                                "<td>" + docSecondHandMovTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + rem2Party1.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (rem2Party1.DirContractorIDOrg != docSecondHandMov.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandMov.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docSecondHandMov.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docSecondHandMovTabCollection[i].Rem2PartyID + "</td>" +       //партия
                                "<td>" + docSecondHandMovTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                                "<td>" + rem2Party1.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg106_1
                            );
                        }
                        #endregion


                        //Если всё нормально, то меняем некоторые значения: DirWarehouseIDPurch и DirContractorID
                        docSecondHandMovTabCollection[i].DirWarehouseIDPurch = rem2Party1.DirWarehouseID; //Склад на который пришла партия первоначально
                        DocDatePurches = rem2Party1.DocDatePurches;

                        #endregion


                        #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                        //Получаем "DocID" из списуемой партии "docSecondHandMovTabCollection[i].DocID" для "DocIDFirst"
                        Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandMovTabCollection[i].Rem2PartyID);

                        #endregion


                        #region Списание

                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                        rem2PartyMinus.Rem2PartyMinusID = null;
                        rem2PartyMinus.Rem2PartyID = docSecondHandMovTabCollection[i].Rem2PartyID;
                        rem2PartyMinus.DirServiceNomenID = docSecondHandMovTabCollection[i].DirServiceNomenID;
                        rem2PartyMinus.Quantity = docSecondHandMovTabCollection[i].Quantity;
                        rem2PartyMinus.DirCurrencyID = docSecondHandMovTabCollection[i].DirCurrencyID;
                        rem2PartyMinus.DirCurrencyMultiplicity = docSecondHandMovTabCollection[i].DirCurrencyMultiplicity;
                        rem2PartyMinus.DirCurrencyRate = docSecondHandMovTabCollection[i].DirCurrencyRate;
                        rem2PartyMinus.DirVatValue = docSecondHandMov.DirVatValue;
                        rem2PartyMinus.DirWarehouseID = docSecondHandMov.DirWarehouseIDFrom;
                        rem2PartyMinus.DirContractorIDOrg = docSecondHandMov.DirContractorIDOrg;
                        rem2PartyMinus.DirServiceContractorID = _Rem2Party.DirServiceContractorID; //rem2PartyMinus.DirServiceContractorID = null;
                        rem2PartyMinus.DocID = Convert.ToInt32(docSecondHandMov.DocID);
                        rem2PartyMinus.PriceCurrency = docSecondHandMovTabCollection[i].PriceCurrency;
                        rem2PartyMinus.PriceVAT = docSecondHandMovTabCollection[i].PriceVAT;
                        rem2PartyMinus.FieldID = Convert.ToInt32(docSecondHandMovTabCollection[i].DocSecondHandMovTabID);
                        rem2PartyMinus.Reserve = docSecondHandMov.Reserve;

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

                        Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[docSecondHandMovTabCollection.Count()];
                        for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                        {
                            #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                            //Получаем "DocID" из списуемой партии "docSecondHandMovTabCollection[i].DocID" для "DocIDFirst"
                            Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandMovTabCollection[i].Rem2PartyID);

                            #endregion


                            Models.Sklad.Rem.Rem2Party rem2Party = new Models.Sklad.Rem.Rem2Party();
                            rem2Party.Rem2PartyID = null;
                            rem2Party.DirServiceNomenID = docSecondHandMovTabCollection[i].DirServiceNomenID;
                            rem2Party.Quantity = docSecondHandMovTabCollection[i].Quantity;
                            rem2Party.Remnant = docSecondHandMovTabCollection[i].Quantity;
                            rem2Party.DirCurrencyID = docSecondHandMovTabCollection[i].DirCurrencyID;
                            //rem2Party.DirCurrencyMultiplicity = docSecondHandMovTabCollection[i].DirCurrencyMultiplicity;
                            //rem2Party.DirCurrencyRate = docSecondHandMovTabCollection[i].DirCurrencyRate;
                            rem2Party.DirVatValue = docSecondHandMov.DirVatValue;
                            rem2Party.DirWarehouseID = docSecondHandMov.DirWarehouseIDTo;
                            rem2Party.DirWarehouseIDDebit = docSecondHandMov.DirWarehouseIDFrom; //Склад с которого списали партию
                            rem2Party.DirWarehouseIDPurch = docSecondHandMovTabCollection[i].DirWarehouseIDPurch;     //Склад на который пришла партия первоначально (получили Выше)
                            rem2Party.DirContractorIDOrg = docSecondHandMov.DirContractorIDOrg;

                            rem2Party.DocDatePurches = DocDatePurches;

                            rem2Party.DocID = Convert.ToInt32(docSecondHandMov.DocID);
                            rem2Party.PriceCurrency = docSecondHandMovTabCollection[i].PriceCurrency;
                            rem2Party.PriceVAT = docSecondHandMovTabCollection[i].PriceVAT;
                            rem2Party.FieldID = Convert.ToInt32(docSecondHandMovTabCollection[i].DocSecondHandMovTabID);

                            rem2Party.PriceRetailVAT = docSecondHandMovTabCollection[i].PriceRetailVAT;
                            rem2Party.PriceRetailCurrency = docSecondHandMovTabCollection[i].PriceRetailCurrency;
                            rem2Party.PriceWholesaleVAT = docSecondHandMovTabCollection[i].PriceWholesaleVAT;
                            rem2Party.PriceWholesaleCurrency = docSecondHandMovTabCollection[i].PriceWholesaleCurrency;
                            rem2Party.PriceIMVAT = docSecondHandMovTabCollection[i].PriceIMVAT;
                            rem2Party.PriceIMCurrency = docSecondHandMovTabCollection[i].PriceIMCurrency;

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
                    //1. Менять склад аппарата в "DocSecondHandPurches"
                    //2. Писать в Лог о перемещении в "LogSecondHands" (внутри первого)

                    #region 1. Менять склад аппарата в "DocSecondHandPurches"

                    for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                    {
                        Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandMovTabCollection[i].DocSecondHandPurchID);
                        if (docSecondHandPurch.DirWarehouseID == docSecondHandMov.DirWarehouseIDFrom)
                        {
                            docSecondHandPurch.DirWarehouseID = docSecondHandMov.DirWarehouseIDTo;
                        }
                        else
                        {
                            throw new System.InvalidOperationException("Не найден на точке аппарат №" + docSecondHandMovTabCollection[i].DocSecondHandPurchID);
                        }
                        db.Entry(docSecondHandPurch).State = EntityState.Modified;
                        await db.SaveChangesAsync();


                        //2. Писать в Лог о перемещении в "LogSecondHands"
                        #region 4. Log

                        logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                        logService.DirSecondHandLogTypeID = 12;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                        logService.DirWarehouseIDFrom = docSecondHandMov.DirWarehouseIDFrom;
                        logService.DirWarehouseIDTo = docSecondHandMov.DirWarehouseIDTo;
                        //logService.Msg = "Аппарат принят на точку №" + docSecondHandPurch.DirWarehouseID;

                        await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                        #endregion
                    }

                    #endregion

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

                
                int DocSecondHandMovID = Convert.ToInt32(docSecondHandMov.DocSecondHandMovID);

                //Получаем DocPurch из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocSecondHandMov _docSecondHandMov = db.DocSecondHandMovs.Find(DocSecondHandMovID);
                int? iDocSecondHandMov_DocID = _docSecondHandMov.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocSecondHandMov_DocID));

                #endregion


                #region 1. Rem2PartyMinuses и Rem2Parties *** *** *** *** *** *** *** *** *** ***

                //Удаление записей в таблицах: Rem2PartyMinuses
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSecondHandMov.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Rem2Parties WHERE DocID=@DocID;", parDocID); //DELETE FROM Rem2PartyMinuses WHERE DocID=@DocID;

                //Обновление записей: Rem2PartyMinuses
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE Rem2PartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion

                */

                #endregion



                #region 1. Проверка: продали товар

                for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandMovTabCollection[i].DocSecondHandPurchID);
                    if (docSecondHandPurch.DirSecondHandStatusID > 9)
                    {
                        throw new System.InvalidOperationException("Аппарт уже продан или разобран №" + docSecondHandMovTabCollection[i].DocSecondHandPurchID);
                    }
                }

                #endregion


                #region 2. Менем точку

                for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandMovTabCollection[i].DocSecondHandPurchID);
                    docSecondHandPurch.DirWarehouseID = docSecondHandMov.DirWarehouseIDFrom;

                    db.Entry(docSecondHandPurch).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                #endregion


                //Doc.Held = false
                #region 3. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docSecondHandMov.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion
            }


            #region 3. Лог


            #region Пишем в Лог о смене статуса и мастера, если такое было

            logLogistic.DocLogisticID = docSecondHandMov.DocSecondHandMovID;
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


            return docSecondHandMov;
        }



        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            //System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocSecondHandMov docSecondHandMov,
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

            docSecondHandMov.DirMovementStatusID = StatusID;
            db.Entry(docSecondHandMov).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 2. Если статус == 4, то проводим

            if (StatusID == 4)
            {

                #region 2.1. Doc

                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandMov.DocID);
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
                        from x in db.DocSecondHandMovTabs
                        where x.DocSecondHandMovID == docSecondHandMov.DocSecondHandMovID
                        select x
                    ).ToListAsync();

                Models.Sklad.Doc.DocSecondHandMovTab[] docSecondHandMovTabCollection = new Models.Sklad.Doc.DocSecondHandMovTab[queryTabs.Count()];

                for (int i = 0; i < queryTabs.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandMovTab docSecondHandMovTab = await db.DocSecondHandMovTabs.FindAsync(queryTabs[0].DocSecondHandMovTabID);
                    docSecondHandMovTabCollection[0] = docSecondHandMovTab;
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
                //Models.Sklad.Rem.Rem2PartyMinus[] rem2PartyMinusCollection = new Models.Sklad.Rem.Rem2PartyMinus[docSecondHandMovTabCollection.Count()];
                for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                {
                    #region Проверки

                    //Переменные
                    int iRem2PartyID = docSecondHandMovTabCollection[i].Rem2PartyID;
                    double dQuantity = docSecondHandMovTabCollection[i].Quantity;
                    //Находим партию
                    Models.Sklad.Rem.Rem2Party rem2Party1 = await db.Rem2Parties.FindAsync(iRem2PartyID);
                    db.Entry(rem2Party1).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                    #region 1. Есть ли остаток в партии с которой списываем!
                    if (rem2Party1.Remnant < dQuantity)
                    {
                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg104 +

                            "<tr>" +
                            "<td>" + docSecondHandMovTabCollection[i].Rem2PartyID + "</td>" +                        //партия
                            "<td>" + docSecondHandMovTabCollection[i].DirServiceNomenID + "</td>" +                        //Код товара
                            "<td>" + docSecondHandMovTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                            "<td>" + rem2Party1.Remnant + "</td>" +                                              //остаток партии
                            "<td>" + (docSecondHandMovTabCollection[i].Quantity - rem2Party1.Remnant).ToString() + "</td>" +  //недостающее к-во
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg104_1
                        );
                    }
                    #endregion

                    #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                    if (rem2Party1.DirWarehouseID != docSecondHandMov.DirWarehouseIDFrom)
                    {
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandMov.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandMov.DirWarehouseIDFrom);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg105 +

                            "<tr>" +
                            "<td>" + docSecondHandMovTabCollection[i].Rem2PartyID + "</td>" +       //партия
                            "<td>" + docSecondHandMovTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
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
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandMov.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docSecondHandMov.DirContractorIDOrg);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg106 +

                            "<tr>" +
                            "<td>" + docSecondHandMovTabCollection[i].Rem2PartyID + "</td>" +       //партия
                            "<td>" + docSecondHandMovTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                            "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                            "<td>" + rem2Party1.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg106_1
                        );
                    }
                    #endregion


                    //Если всё нормально, то меняем некоторые значения: DirWarehouseIDPurch и DirContractorID
                    docSecondHandMovTabCollection[i].DirWarehouseIDPurch = rem2Party1.DirWarehouseID; //Склад на который пришла партия первоначально
                    DocDatePurches = rem2Party1.DocDatePurches;

                    #endregion


                    #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                    //Получаем "DocID" из списуемой партии "docSecondHandMovTabCollection[i].DocID" для "DocIDFirst"
                    Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandMovTabCollection[i].Rem2PartyID);

                    #endregion


                    #region Сохранение

                    Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                    rem2PartyMinus.Rem2PartyMinusID = null;
                    rem2PartyMinus.Rem2PartyID = docSecondHandMovTabCollection[i].Rem2PartyID;
                    rem2PartyMinus.DirServiceNomenID = docSecondHandMovTabCollection[i].DirServiceNomenID;
                    rem2PartyMinus.Quantity = docSecondHandMovTabCollection[i].Quantity;
                    rem2PartyMinus.DirCurrencyID = docSecondHandMovTabCollection[i].DirCurrencyID;
                    rem2PartyMinus.DirCurrencyMultiplicity = docSecondHandMovTabCollection[i].DirCurrencyMultiplicity;
                    rem2PartyMinus.DirCurrencyRate = docSecondHandMovTabCollection[i].DirCurrencyRate;
                    rem2PartyMinus.DirVatValue = docSecondHandMov.DirVatValue;
                    rem2PartyMinus.DirWarehouseID = docSecondHandMov.DirWarehouseIDFrom;
                    rem2PartyMinus.DirServiceContractorID = _Rem2Party.DirServiceContractorID; //rem2PartyMinus.DirServiceContractorID = null;
                    rem2PartyMinus.DirContractorIDOrg = doc.DirContractorIDOrg;
                    rem2PartyMinus.DocID = Convert.ToInt32(doc.DocID);
                    rem2PartyMinus.PriceCurrency = docSecondHandMovTabCollection[i].PriceCurrency;
                    rem2PartyMinus.PriceVAT = docSecondHandMovTabCollection[i].PriceVAT;
                    rem2PartyMinus.FieldID = Convert.ToInt32(docSecondHandMovTabCollection[i].DocSecondHandMovTabID);
                    rem2PartyMinus.Reserve = docSecondHandMov.Reserve;

                    rem2PartyMinus.DirEmployeeID = doc.DirEmployeeID;
                    rem2PartyMinus.DocDate = doc.DocDate;

                    db.Entry(rem2PartyMinus).State = EntityState.Added;
                    await db.SaveChangesAsync();

                    #endregion
                }

                #endregion


                #region Rem2Party - Партии

                Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[docSecondHandMovTabCollection.Count()];
                for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                {
                    #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                    //Получаем "DocID" из списуемой партии "docSecondHandMovTabCollection[i].DocID" для "DocIDFirst"
                    Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandMovTabCollection[i].Rem2PartyID);

                    #endregion


                    Models.Sklad.Rem.Rem2Party rem2Party = new Models.Sklad.Rem.Rem2Party();
                    rem2Party.Rem2PartyID = null;
                    rem2Party.DirServiceNomenID = docSecondHandMovTabCollection[i].DirServiceNomenID;
                    rem2Party.Quantity = docSecondHandMovTabCollection[i].Quantity;
                    rem2Party.Remnant = docSecondHandMovTabCollection[i].Quantity;
                    rem2Party.DirCurrencyID = docSecondHandMovTabCollection[i].DirCurrencyID;
                    //rem2Party.DirCurrencyMultiplicity = docSecondHandMovTabCollection[i].DirCurrencyMultiplicity;
                    //rem2Party.DirCurrencyRate = docSecondHandMovTabCollection[i].DirCurrencyRate;
                    rem2Party.DirVatValue = docSecondHandMov.DirVatValue;
                    rem2Party.DirWarehouseID = docSecondHandMov.DirWarehouseIDTo;
                    rem2Party.DirWarehouseIDDebit = docSecondHandMov.DirWarehouseIDFrom; //Склад с которого списали партию
                    rem2Party.DirWarehouseIDPurch = docSecondHandMovTabCollection[i].DirWarehouseIDPurch;     //Склад на который пришла партия первоначально (получили Выше)
                    rem2Party.DirContractorIDOrg = doc.DirContractorIDOrg;

                    rem2Party.DocDatePurches = DocDatePurches;

                    rem2Party.DocID = Convert.ToInt32(docSecondHandMov.DocID);
                    rem2Party.PriceCurrency = docSecondHandMovTabCollection[i].PriceCurrency;
                    rem2Party.PriceVAT = docSecondHandMovTabCollection[i].PriceVAT;
                    rem2Party.FieldID = Convert.ToInt32(docSecondHandMovTabCollection[i].DocSecondHandMovTabID);

                    rem2Party.PriceRetailVAT = docSecondHandMovTabCollection[i].PriceRetailVAT;
                    rem2Party.PriceRetailCurrency = docSecondHandMovTabCollection[i].PriceRetailCurrency;
                    rem2Party.PriceWholesaleVAT = docSecondHandMovTabCollection[i].PriceWholesaleVAT;
                    rem2Party.PriceWholesaleCurrency = docSecondHandMovTabCollection[i].PriceWholesaleCurrency;
                    rem2Party.PriceIMVAT = docSecondHandMovTabCollection[i].PriceIMVAT;
                    rem2Party.PriceIMCurrency = docSecondHandMovTabCollection[i].PriceIMCurrency;

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



                #region 1. Менять склад аппарата в "DocSecondHandPurches"

                for (int i = 0; i < docSecondHandMovTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandMovTabCollection[i].DocSecondHandPurchID);
                    if (docSecondHandPurch.DirWarehouseID == docSecondHandMov.DirWarehouseIDFrom)
                    {
                        docSecondHandPurch.DirWarehouseID = docSecondHandMov.DirWarehouseIDTo;
                    }
                    else
                    {
                        throw new System.InvalidOperationException("Не найден на точке аппарат №" + docSecondHandMovTabCollection[i].DocSecondHandPurchID);
                    }
                    db.Entry(docSecondHandMovTabCollection[i]).State = EntityState.Added;
                    await db.SaveChangesAsync();


                    //2. Писать в Лог о перемещении в "LogSecondHands"
                    #region 4. Log

                    logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                    logService.DirSecondHandLogTypeID = 15;
                    logService.DirEmployeeID = field.DirEmployeeID;
                    logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                    logService.DirWarehouseIDFrom = docSecondHandMov.DirWarehouseIDFrom;
                    logService.DirWarehouseIDTo = docSecondHandMov.DirWarehouseIDTo;
                    //logService.Msg = "Аппарат принят на точку №" + docSecondHandPurch.DirWarehouseID;

                    await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

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
            else logLogistic.Msg = docSecondHandMov.dirEmployee_Courier.DirEmployeeName;

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
                "[DocSecondHandMovs].[DocSecondHandMovID] AS [DocSecondHandMovementID], " +
                "[DocSecondHandMovs].[DocSecondHandMovID] AS [DocSecondHandMovID], " +
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
                "[DocSecondHandMovs].[Reserve] AS [Reserve] " +

                "FROM [DocSecondHandMovs] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandMovs].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "INNER JOIN [DirWarehouses] AS [DirWarehousesFrom] ON [DirWarehousesFrom].[DirWarehouseID] = [DocSecondHandMovs].[DirWarehouseIDFrom] " +
                "INNER JOIN [DirWarehouses] AS [DirWarehousesTo] ON [DirWarehousesTo].[DirWarehouseID] = [DocSecondHandMovs].[DirWarehouseIDTo] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion
        
    }
}