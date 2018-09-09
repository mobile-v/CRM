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

namespace PartionnyAccount.Controllers.Sklad.Logistic
{
    public class LogisticsController : ApiController
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
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 74;

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
            public int? LogisticID;
        }
        // GET: api/Logistics
        public async Task<IHttpActionResult> GetLogistics(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovementsLogistics"));
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
                _params.LogisticID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "LogisticID", true) == 0).Value);

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

                        #region from

                        from docMovements in db.DocMovements

                        join docMovementTabs1 in db.DocMovementTabs on docMovements.DocMovementID equals docMovementTabs1.DocMovementID into docMovementTabs2
                        from docMovementTabs in docMovementTabs2.DefaultIfEmpty()

                        //where docMovements.doc.DocDate >= _params.DateS && docMovements.doc.DocDate <= _params.DatePo

                        #endregion


                        #region group

                        group new { docMovementTabs }
                        by new
                        {
                            DocID = docMovements.DocID,
                            DocDate = docMovements.doc.DocDate,
                            Base = docMovements.doc.Base,
                            Held = docMovements.doc.Held,
                            Discount = docMovements.doc.Discount,
                            Del = docMovements.doc.Del,
                            Description = docMovements.doc.Description,
                            IsImport = docMovements.doc.IsImport,
                            DirVatValue = docMovements.doc.DirVatValue,

                            LogisticID = docMovements.DocMovementID,
                            //DirContractorName = docMovements.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docMovements.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docMovements.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docMovements.dirWarehouseFrom.DirWarehouseID,
                            DirWarehouseNameFrom = docMovements.dirWarehouseFrom.DirWarehouseName,
                            DirWarehouseIDTo = docMovements.dirWarehouseTo.DirWarehouseID,
                            DirWarehouseNameTo = docMovements.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docMovements.doc.NumberInt,

                            DirEmployeeIDCourier = docMovements.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docMovements.dirEmployee_Courier.DirEmployeeName,
                            DirMovementStatusID = docMovements.DirMovementStatusID, //Курьер штрихнулся и забрал товар

                            DirEmployeeName = docMovements.doc.dirEmployee.DirEmployeeName,
                        }
                        into g

                        #endregion


                        #region select

                        select new
                        {
                            DocTypeXID = 1,
                            DocTypeXName = "Торговля",

                            DocID = g.Key.DocID,
                            DocDate = g.Key.DocDate,
                            Held = g.Key.Held,
                            Base = g.Key.Base,

                            Discount = g.Key.Discount,
                            Del = g.Key.Del,
                            Description = g.Key.Description,
                            IsImport = g.Key.IsImport,

                            LogisticID = g.Key.LogisticID,
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
                            //Не работает (((
                            /*
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency), sysSetting.FractionalPartInSum)
                            */
                            SumOfVATCurrency = g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency)
                            //SumOfVATCurrency = 0,
                        }

                        #endregion

                    )

                    .Union

                    (

                        #region from

                        from docSecondHandMovements in db.DocSecondHandMovements

                        join docSecondHandMovementTabs1 in db.DocSecondHandMovementTabs on docSecondHandMovements.DocSecondHandMovementID equals docSecondHandMovementTabs1.DocSecondHandMovementID into docSecondHandMovementTabs2
                        from docSecondHandMovementTabs in docSecondHandMovementTabs2.DefaultIfEmpty()

                        //where docSecondHandMovements.doc.DocDate >= _params.DateS && docSecondHandMovements.doc.DocDate <= _params.DatePo

                        #endregion


                        #region group

                        group new { docSecondHandMovementTabs }
                        by new
                        {
                            DocID = docSecondHandMovements.DocID,
                            DocDate = docSecondHandMovements.doc.DocDate,
                            Base = docSecondHandMovements.doc.Base,
                            Held = docSecondHandMovements.doc.Held,
                            Discount = docSecondHandMovements.doc.Discount,
                            Del = docSecondHandMovements.doc.Del,
                            Description = docSecondHandMovements.doc.Description,
                            IsImport = docSecondHandMovements.doc.IsImport,
                            DirVatValue = docSecondHandMovements.doc.DirVatValue,

                            LogisticID = docSecondHandMovements.DocSecondHandMovementID,
                            //DirContractorName = docSecondHandMovements.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandMovements.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandMovements.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docSecondHandMovements.dirWarehouseFrom.DirWarehouseID,
                            DirWarehouseNameFrom = docSecondHandMovements.dirWarehouseFrom.DirWarehouseName,
                            DirWarehouseIDTo = docSecondHandMovements.dirWarehouseTo.DirWarehouseID,
                            DirWarehouseNameTo = docSecondHandMovements.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docSecondHandMovements.doc.NumberInt,

                            DirEmployeeIDCourier = docSecondHandMovements.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docSecondHandMovements.dirEmployee_Courier.DirEmployeeName,
                            DirMovementStatusID = docSecondHandMovements.DirMovementStatusID, //Курьер штрихнулся и забрал товар

                            DirEmployeeName = docSecondHandMovements.doc.dirEmployee.DirEmployeeName,
                        }
                        into g

                        #endregion


                        #region select

                        select new
                        {
                            DocTypeXID = 2,
                            DocTypeXName = "БУ",

                            DocID = g.Key.DocID,
                            DocDate = g.Key.DocDate,
                            Held = g.Key.Held,
                            Base = g.Key.Base,

                            Discount = g.Key.Discount,
                            Del = g.Key.Del,
                            Description = g.Key.Description,
                            IsImport = g.Key.IsImport,

                            LogisticID = g.Key.LogisticID,
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
                            //Не работает (((
                            /*
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency), sysSetting.FractionalPartInSum)
                            */
                            SumOfVATCurrency = g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency)
                            //SumOfVATCurrency = 0,
                        }

                        #endregion

                    );
                

                #endregion


                #region Условия (параметры) *** *** ***


                #region Отобразить "Предыдущие ремнты"

                if (_params.LogisticID != null && _params.LogisticID > 0)
                {
                    query = query.Where(x => x.LogisticID == _params.LogisticID);
                }

                #endregion


                #region dirEmployee: dirEmployee.DirWarehouseID and/or dirEmployee.DirContractorIDOrg

                /*if (_params.DirWarehouseID != null && _params.DirWarehouseID > 0)
                {
                    query = query.Where(x => x.DirWarehouseIDTo == _params.DirWarehouseID);
                }*/
                //Если привязка к сотруднику (для документа "DocMovements" показать все склады)
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
                        query = query.Where(x => x.LogisticID == iNumber32);
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

                query = query.OrderByDescending(x => x.LogisticID).Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocMovements.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    Logistic = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/Logistics/5
        [ResponseType(typeof(DocMovement))]
        public async Task<IHttpActionResult> GetDocLogistic(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovementsLogistics"));
                    if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                }

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Параметры

                /*
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
                        from docMovements in db.DocMovements
                        where docMovements.DocMovementID == id
                        select docMovements
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }
                */

                //int DocID = id;

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                (

                    (
                        #region from

                        from docMovements in db.DocMovements

                        join docMovementTabs1 in db.DocMovementTabs on docMovements.DocMovementID equals docMovementTabs1.DocMovementID into docMovementTabs2
                        from docMovementTabs in docMovementTabs2.DefaultIfEmpty()

                        #endregion

                        where docMovements.doc.DocID == id

                        #region group

                        group new { docMovementTabs }
                        by new
                        {
                            DocID = docMovements.DocID, //DocID = docMovements.doc.DocID,
                            DocIDBase = docMovements.doc.DocIDBase,
                            DocDate = docMovements.doc.DocDate,
                            Base = docMovements.doc.Base,
                            Held = docMovements.doc.Held,
                            Discount = docMovements.doc.Discount,
                            Del = docMovements.doc.Del,
                            IsImport = docMovements.doc.IsImport,
                            Description = docMovements.doc.Description,
                            DirVatValue = docMovements.doc.DirVatValue,
                            //DirPaymentTypeID = docMovements.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docMovements.doc.dirPaymentType.DirPaymentTypeName,

                            LogisticID = docMovements.DocMovementID,
                            //DirContractorID = docMovements.doc.DirContractorID,
                            //DirContractorName = docMovements.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docMovements.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docMovements.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docMovements.dirWarehouseFrom.DirWarehouseID,
                            DirWarehouseNameFrom = docMovements.dirWarehouseFrom.DirWarehouseName,

                            DirWarehouseIDTo = docMovements.dirWarehouseTo.DirWarehouseID,
                            DirWarehouseNameTo = docMovements.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docMovements.doc.NumberInt,

                            //Оплата
                            Payment = docMovements.doc.Payment,

                            //Резерв
                            Reserve = docMovements.Reserve,
                            DirMovementDescriptionName = docMovements.dirMovementDescription.DirMovementDescriptionName,

                            DirMovementStatusID = docMovements.DirMovementStatusID,

                            DirEmployeeIDCourier = docMovements.DirEmployeeIDCourier,

                        }
                        into g

                        #endregion

                        #region select

                        select new
                        {
                            DocTypeXID = 1,
                            DocTypeXName = "Торговля",

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

                            LogisticID = g.Key.LogisticID,
                            //DirContractorID = g.Key.DirContractorID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,

                            DirWarehouseIDFrom = g.Key.DirWarehouseIDFrom,
                            DirWarehouseNameFrom = g.Key.DirWarehouseNameFrom,

                            DirWarehouseIDTo = g.Key.DirWarehouseIDTo,
                            DirWarehouseNameTo = g.Key.DirWarehouseNameTo,

                            NumberInt = g.Key.NumberInt,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency),

                            //Сумма только НДС
                            SumVATCurrency =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * (x.docMovementTabs.PriceCurrency - (x.docMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * (x.docMovementTabs.PriceCurrency - (x.docMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * (x.docMovementTabs.PriceCurrency - (x.docMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) - g.Key.Payment,

                            //Резерв
                            Reserve = g.Key.Reserve,
                            DescriptionMovement = g.Key.DirMovementDescriptionName, //DirMovementDescriptionID = g.Key.DirMovementDescriptionID,

                            DirMovementStatusID = g.Key.DirMovementStatusID,
                            DirEmployeeIDCourier = g.Key.DirEmployeeIDCourier,
                        }

                        #endregion
                    ).

                    Union

                    (
                        #region from

                        from docSecondHandMovements in db.DocSecondHandMovements

                        join docSecondHandMovementTabs1 in db.DocSecondHandMovementTabs on docSecondHandMovements.DocSecondHandMovementID equals docSecondHandMovementTabs1.DocSecondHandMovementID into docSecondHandMovementTabs2
                        from docSecondHandMovementTabs in docSecondHandMovementTabs2.DefaultIfEmpty()

                        #endregion

                        where docSecondHandMovements.doc.DocID == id

                        #region group

                        group new { docSecondHandMovementTabs }
                        by new
                        {
                            DocID = docSecondHandMovements.DocID, //DocID = docSecondHandMovements.doc.DocID,
                            DocIDBase = docSecondHandMovements.doc.DocIDBase,
                            DocDate = docSecondHandMovements.doc.DocDate,
                            Base = docSecondHandMovements.doc.Base,
                            Held = docSecondHandMovements.doc.Held,
                            Discount = docSecondHandMovements.doc.Discount,
                            Del = docSecondHandMovements.doc.Del,
                            IsImport = docSecondHandMovements.doc.IsImport,
                            Description = docSecondHandMovements.doc.Description,
                            DirVatValue = docSecondHandMovements.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandMovements.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docSecondHandMovements.doc.dirPaymentType.DirPaymentTypeName,

                            LogisticID = docSecondHandMovements.DocSecondHandMovementID,
                            //DirContractorID = docSecondHandMovements.doc.DirContractorID,
                            //DirContractorName = docSecondHandMovements.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandMovements.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docSecondHandMovements.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docSecondHandMovements.dirWarehouseFrom.DirWarehouseID,
                            DirWarehouseNameFrom = docSecondHandMovements.dirWarehouseFrom.DirWarehouseName,

                            DirWarehouseIDTo = docSecondHandMovements.dirWarehouseTo.DirWarehouseID,
                            DirWarehouseNameTo = docSecondHandMovements.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docSecondHandMovements.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandMovements.doc.Payment,

                            //Резерв
                            Reserve = docSecondHandMovements.Reserve,
                            DirMovementDescriptionName = docSecondHandMovements.dirMovementDescription.DirMovementDescriptionName,

                            DirMovementStatusID = docSecondHandMovements.DirMovementStatusID,

                            DirEmployeeIDCourier = docSecondHandMovements.DirEmployeeIDCourier,

                        }
                        into g

                        #endregion

                        #region select

                        select new
                        {
                            DocTypeXID = 2,
                            DocTypeXName = "БУ",

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

                            LogisticID = g.Key.LogisticID,
                            //DirContractorID = g.Key.DirContractorID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,

                            DirWarehouseIDFrom = g.Key.DirWarehouseIDFrom,
                            DirWarehouseNameFrom = g.Key.DirWarehouseNameFrom,

                            DirWarehouseIDTo = g.Key.DirWarehouseIDTo,
                            DirWarehouseNameTo = g.Key.DirWarehouseNameTo,

                            NumberInt = g.Key.NumberInt,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency),

                            //Сумма только НДС
                            SumVATCurrency =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * (x.docSecondHandMovementTabs.PriceCurrency - (x.docSecondHandMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * (x.docSecondHandMovementTabs.PriceCurrency - (x.docSecondHandMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * (x.docSecondHandMovementTabs.PriceCurrency - (x.docSecondHandMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))),

                            //Оплата

                            Payment = g.Key.Payment,
                            /*
                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),
                            */
                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) - g.Key.Payment,

                            //Резерв
                            Reserve = g.Key.Reserve,
                            DescriptionMovement = g.Key.DirMovementDescriptionName, //DirMovementDescriptionID = g.Key.DirMovementDescriptionID,

                            DirMovementStatusID = g.Key.DirMovementStatusID,
                            DirEmployeeIDCourier = g.Key.DirEmployeeIDCourier,
                        }

                        #endregion
                    )

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

        //Смена статуса
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLogistic(int id, int DirStatusID, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovements"));
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


                #region Проверки

                //1. Если документ проведён то выдать сообщение
                /*
                Models.Sklad.Doc.DocMovement docMovement = await db.DocMovements.FindAsync(id);
                Models.Sklad.Doc.Doc doc1 = await db.Docs.FindAsync(docMovement.DocID);

                Models.Sklad.Doc.DocSecondHandMovement docSecondHandMovement = await db.DocSecondHandMovements.FindAsync(id);
                Models.Sklad.Doc.Doc doc2 = await db.Docs.FindAsync(docSecondHandMovement.DocID);
                */
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(id);
                if (Convert.ToBoolean(doc.Held)) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3)); }


                Models.Sklad.Doc.DocMovement docMovement = new DocMovement();
                Models.Sklad.Doc.DocSecondHandMovement docSecondHandMovement = new DocSecondHandMovement();
                int DirMovementStatusID = 0, DirEmployeeIDCourier = 0, DirWarehouseIDTo = 0;
                if (doc.ListObjectID == 33)
                {
                    var quert = await
                       (
                            from x in db.DocMovements
                            where x.DocID == id
                            select x
                        ).ToListAsync();

                    if (quert.Count() > 0)
                    {
                        docMovement = await db.DocMovements.FindAsync(quert[0].DocMovementID);
                        if (docMovement.DirMovementStatusID == DirStatusID) { return Ok(returnServer.Return(false, "Изменения не сохранены! Т.к. статус не сменён!")); }

                        DirMovementStatusID = Convert.ToInt32(docMovement.DirMovementStatusID);
                        DirEmployeeIDCourier = Convert.ToInt32(docMovement.DirEmployeeIDCourier);
                        DirWarehouseIDTo = Convert.ToInt32(docMovement.DirWarehouseIDTo);
                    }
                }
                else if (doc.ListObjectID == 71)
                {
                    var quert = await
                        (
                            from x in db.DocSecondHandMovements
                            where x.DocID == id
                            select x
                        ).ToListAsync();

                    if (quert.Count() > 0)
                    {
                        docSecondHandMovement = await db.DocSecondHandMovements.FindAsync(quert[0].DocSecondHandMovementID);
                        if (docSecondHandMovement.DirMovementStatusID == DirStatusID) { return Ok(returnServer.Return(false, "Изменения не сохранены! Т.к. статус не сменён!")); }

                        DirMovementStatusID = Convert.ToInt32(docSecondHandMovement.DirMovementStatusID);
                        DirEmployeeIDCourier = Convert.ToInt32(docSecondHandMovement.DirEmployeeIDCourier);
                        DirWarehouseIDTo = Convert.ToInt32(docSecondHandMovement.DirWarehouseIDTo);
                    }
                }


                //2. Нельзя перескакивать через статусы (статус всегда +/- 1)
                if (Math.Abs(DirStatusID - Convert.ToInt32(DirMovementStatusID)) != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_4)); }

                //3. Если статус == 3, то проверяем введённый пароль и чистим его (что бы не писался в Лог)
                if (DirStatusID == 3)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(DirEmployeeIDCourier);
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
                                if (DirWarehouseIDTo == queryW[0].DirWarehouseID) { bFindWarehouse = true; break; }
                            }
                            if (!bFindWarehouse) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_2));
                        }
                    }
                }

                #endregion

                #region Сохранение

                int DocID = 0, DocMovementID = 0, DocSecondHandMovementID = 0, LogisticID = 0;

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //await mStatusChange(db, ts, docMovement, id, DirStatusID, sDiagnosticRresults, field);
                        //await mStatusChange(db, docMovement, docSecondHandMovement, id, DirStatusID, sDiagnosticRresults, field); //!!!

                        if (doc.ListObjectID == 33)
                        {
                            Controllers.Sklad.Doc.DocMovements.DocMovementsController docMovementsController = new Controllers.Sklad.Doc.DocMovements.DocMovementsController();
                            await docMovementsController.mStatusChange(db, dbRead, docMovement, id, DirStatusID, sDiagnosticRresults, field);

                            DocID = Convert.ToInt32(docMovement.DocID);
                            DocMovementID = Convert.ToInt32(docMovement.DocMovementID);
                            LogisticID = Convert.ToInt32(docMovement.DocMovementID);
                        }
                        else if (doc.ListObjectID == 71)
                        {
                            Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandMovementsController docSecondHandMovementsController = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandMovementsController();
                            await docSecondHandMovementsController.mStatusChange(db, dbRead, docSecondHandMovement, id, DirStatusID, sDiagnosticRresults, field);

                            DocID = Convert.ToInt32(docSecondHandMovement.DocID);
                            DocSecondHandMovementID = Convert.ToInt32(docSecondHandMovement.DocSecondHandMovementID);
                            LogisticID = Convert.ToInt32(docSecondHandMovement.DocSecondHandMovementID);
                        }

                        //docMovement = await Task.Run(() => mPutPostDocMovement(db, dbRead, UO_Action, docMovement, EntityState.Modified, docMovementTabCollection, field)); //sysSetting

                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }

                dynamic collectionWrapper = new
                {
                    DocID = DocID,
                    DocMovementID = DocMovementID,
                    DocSecondHandMovementID = DocSecondHandMovementID,
                    Logistic = LogisticID,
                };
                return Ok(returnServer.Return(true, collectionWrapper));

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

        private bool DocMovementExists(int id)
        {
            return db.DocMovements.Count(e => e.DocMovementID == id) > 0;
        }

        bool bFindWarehouse = false;


        #endregion


    }

}
