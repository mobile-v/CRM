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
    public class DocSecondHandMovementsController : ApiController
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
            public int? DocSecondHandMovementID;
        }
        // GET: api/DocSecondHandMovements
        public async Task<IHttpActionResult> GetDocSecondHandMovements(HttpRequestMessage request)
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
                _params.DocSecondHandMovementID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandMovementID", true) == 0).Value);

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
                        from docSecondHandMovements in db.DocSecondHandMovements

                        join docSecondHandMovementTabs1 in db.DocSecondHandMovementTabs on docSecondHandMovements.DocSecondHandMovementID equals docSecondHandMovementTabs1.DocSecondHandMovementID into docSecondHandMovementTabs2
                        from docSecondHandMovementTabs in docSecondHandMovementTabs2.DefaultIfEmpty()

                            //where docSecondHandMovements.doc.DocDate >= _params.DateS && docSecondHandMovements.doc.DocDate <= _params.DatePo

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

                            DocSecondHandMovementID = docSecondHandMovements.DocSecondHandMovementID,
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

                            DocSecondHandMovementID = g.Key.DocSecondHandMovementID,
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
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Отобразить "Предыдущие ремнты"

                if (_params.DocSecondHandMovementID != null && _params.DocSecondHandMovementID > 0)
                {
                    query = query.Where(x => x.DocSecondHandMovementID == _params.DocSecondHandMovementID);
                }

                #endregion


                #region dirEmployee: dirEmployee.DirWarehouseID and/or dirEmployee.DirContractorIDOrg

                /*if (_params.DirWarehouseID != null && _params.DirWarehouseID > 0)
                {
                    query = query.Where(x => x.DirWarehouseIDTo == _params.DirWarehouseID);
                }*/
                //Если привязка к сотруднику (для документа "DocSecondHandMovements" показать все склады)
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
                        query = query.Where(x => x.DocSecondHandMovementID == iNumber32);
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

                query = query.OrderByDescending(x => x.DocSecondHandMovementID).Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocSecondHandMovements.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSecondHandMovement = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandMovements/5
        [ResponseType(typeof(DocSecondHandMovement))]
        public async Task<IHttpActionResult> GetDocSecondHandMovement(int id, HttpRequestMessage request)
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
                        from docSecondHandMovements in db.DocSecondHandMovements
                        where docSecondHandMovements.DocSecondHandMovementID == id
                        select docSecondHandMovements
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docSecondHandMovements in db.DocSecondHandMovements

                        join docSecondHandMovementTabs1 in db.DocSecondHandMovementTabs on docSecondHandMovements.DocSecondHandMovementID equals docSecondHandMovementTabs1.DocSecondHandMovementID into docSecondHandMovementTabs2
                        from docSecondHandMovementTabs in docSecondHandMovementTabs2.DefaultIfEmpty()

                        #endregion

                        where docSecondHandMovements.DocSecondHandMovementID == id

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

                            DocSecondHandMovementID = docSecondHandMovements.DocSecondHandMovementID,
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

                            DocSecondHandMovementID = g.Key.DocSecondHandMovementID,
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
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * (x.docSecondHandMovementTabs.PriceCurrency - (x.docSecondHandMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * (x.docSecondHandMovementTabs.PriceCurrency - (x.docSecondHandMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandMovementTabs.Quantity * x.docSecondHandMovementTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),

                            //Резерв
                            Reserve = g.Key.Reserve,
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

        // PUT: api/DocSecondHandMovements/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandMovement(int id, DocSecondHandMovement docSecondHandMovement, HttpRequestMessage request)
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
                Models.Sklad.Doc.DocSecondHandMovementTab[] docSecondHandMovementTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandMovement.recordsDocSecondHandMovementTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandMovementTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandMovementTab[]>(docSecondHandMovement.recordsDocSecondHandMovementTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docSecondHandMovement.DocSecondHandMovementID || docSecondHandMovement.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //2. Если статус > 1, то редактировать может только Администратор
                if (docSecondHandMovement.DirMovementStatusID > 1 && field.DirEmployeeID != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg122)); }

                //3. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docSecondHandMovement.DirEmployeeIDCourier != null && docSecondHandMovement.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docSecondHandMovement.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }

                //4. Получаем "docSecondHandMovement.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandMovement.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandMovements
                        where x.DocSecondHandMovementID == docSecondHandMovement.DocSecondHandMovementID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandMovement.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //5. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docSecondHandMovement.DocID);
                //6.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //6.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandMovement.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandMovement = await Task.Run(() => mPutPostDocSecondHandMovement(db, dbRead, UO_Action, docSecondHandMovement, EntityState.Modified, docSecondHandMovementTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandMovement.DocSecondHandMovementID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandMovement.DocID,
                    DocSecondHandMovementID = docSecondHandMovement.DocSecondHandMovementID,
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

        // PUT: api/DocSecondHandMovements/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandMovement(int id, int DirStatusID, HttpRequestMessage request)
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


                Models.Sklad.Doc.DocSecondHandMovement docSecondHandMovement = await db.DocSecondHandMovements.FindAsync(id);
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandMovement.DocID);


                #region Проверки

                //1. Если документ проведён то выдать сообщение
                if (Convert.ToBoolean(doc.Held)) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3)); }

                //2. Нельзя перескакивать через статусы (статус всегда +/- 1)
                if (Math.Abs(DirStatusID - Convert.ToInt32(docSecondHandMovement.DirMovementStatusID)) != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_4)); }

                //3. Если статус == 3, то проверяем введённый пароль и чистим его (что бы не писался в Лог)
                if (DirStatusID == 3)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(docSecondHandMovement.DirEmployeeIDCourier);
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
                                if (docSecondHandMovement.DirWarehouseIDTo == queryW[0].DirWarehouseID) { bFindWarehouse = true; break; }
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
                            //await mStatusChange(db, ts, docSecondHandMovement, id, DirStatusID, sDiagnosticRresults, field);
                            await mStatusChange(db, dbRead, docSecondHandMovement, id, DirStatusID, sDiagnosticRresults, field);

                            //docSecondHandMovement = await Task.Run(() => mPutPostDocSecondHandMovement(db, dbRead, UO_Action, docSecondHandMovement, EntityState.Modified, docSecondHandMovementTabCollection, field)); //sysSetting

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
                        DocID = docSecondHandMovement.DocID,
                        DocSecondHandMovementID = docSecondHandMovement.DocSecondHandMovementID
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


        // POST: api/DocSecondHandMovements
        [ResponseType(typeof(DocSecondHandMovement))]
        public async Task<IHttpActionResult> PostDocSecondHandMovement(DocSecondHandMovement docSecondHandMovement, HttpRequestMessage request)
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
                Models.Sklad.Doc.DocSecondHandMovementTab[] docSecondHandMovementTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandMovement.recordsDocSecondHandMovementTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandMovementTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandMovementTab[]>(docSecondHandMovement.recordsDocSecondHandMovementTab);
                }

                #endregion

                #region Проверки

                //1. 
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);


                //2. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docSecondHandMovement.DirEmployeeIDCourier != null && docSecondHandMovement.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docSecondHandMovement.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }


                //3. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandMovement.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandMovement = await Task.Run(() => mPutPostDocSecondHandMovement(db, dbRead, UO_Action, docSecondHandMovement, EntityState.Added, docSecondHandMovementTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandMovement.DocSecondHandMovementID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandMovement.DocID,
                    DocSecondHandMovementID = docSecondHandMovement.DocSecondHandMovementID,
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

        // DELETE: api/DocSecondHandMovements/5
        [ResponseType(typeof(DocSecondHandMovement))]
        public async Task<IHttpActionResult> DeleteDocSecondHandMovement(int id)
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
                            from x in dbRead.DocSecondHandMovements
                            where x.DocSecondHandMovementID == id
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
                //2. DocSecondHandMovementTabs
                //3. DocSecondHandMovements
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocSecondHandMovement docSecondHandMovement = await db.DocSecondHandMovements.FindAsync(id);
                if (docSecondHandMovement == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Rem2PartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocSecondHandMovements
                                where x.DocSecondHandMovementID == id
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


                        #region 2. DocSecondHandMovementTabs *** *** *** *** ***

                        var queryDocSecondHandMovementTabs = await
                            (
                                from x in db.DocSecondHandMovementTabs
                                where x.DocSecondHandMovementID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandMovementTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandMovementTab docSecondHandMovementTab = await db.DocSecondHandMovementTabs.FindAsync(queryDocSecondHandMovementTabs[i].DocSecondHandMovementTabID);
                            db.DocSecondHandMovementTabs.Remove(docSecondHandMovementTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocSecondHandMovements *** *** *** *** ***

                        var queryDocSecondHandMovements = await
                            (
                                from x in db.DocSecondHandMovements
                                where x.DocSecondHandMovementID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandMovements.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandMovement docSecondHandMovement1 = await db.DocSecondHandMovements.FindAsync(queryDocSecondHandMovements[i].DocSecondHandMovementID);
                            db.DocSecondHandMovements.Remove(docSecondHandMovement1);
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

        private bool DocSecondHandMovementExists(int id)
        {
            return db.DocSecondHandMovements.Count(e => e.DocSecondHandMovementID == id) > 0;
        }


        bool bFindWarehouse = false;
        internal async Task<DocSecondHandMovement> mPutPostDocSecondHandMovement(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandMovement docSecondHandMovement,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocSecondHandMovementTab[] docSecondHandMovementTabCollection,

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
                        if (docSecondHandMovement.DirWarehouseIDTo == queryW[i].DirWarehouseID) { bFindWarehouse = true; break; }
                    }
                    if (!bFindWarehouse) UO_Action = "save";
                }
            }
            else
            {
                bFindWarehouse = true;
            }

            #endregion


            if (UO_Action == "held") docSecondHandMovement.Reserve = false;
            else docSecondHandMovement.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docSecondHandMovement.NumberInt;
                doc.NumberReal = docSecondHandMovement.DocSecondHandMovementID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = 1;
                doc.Payment = 0;
                doc.DirContractorID = docSecondHandMovement.DirContractorIDOrg;
                doc.DirContractorIDOrg = docSecondHandMovement.DirContractorIDOrg;
                doc.Discount = docSecondHandMovement.Discount;
                doc.DirVatValue = docSecondHandMovement.DirVatValue;
                doc.Base = docSecondHandMovement.Base;
                doc.Description = docSecondHandMovement.Description;
                doc.DocDate = docSecondHandMovement.DocDate;
                //doc.DocDisc = docSecondHandMovement.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docSecondHandMovement.DocID;
                doc.DocIDBase = docSecondHandMovement.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docSecondHandMovement" со всем полями!
                docSecondHandMovement.DocID = doc.DocID;

                #endregion

                #region 4. DescriptionMovement: пишем ID-шник в DocSecondHandMovementTab и Rem2Party

                string DescriptionMovement = ""; if (docSecondHandMovementTabCollection.Length > 0) DescriptionMovement = docSecondHandMovement.DescriptionMovement;
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

                #region 2. DocSecondHandMovement *** *** *** *** *** *** *** *** *** ***

                //Если ещё не переместили в Логистику
                if (docSecondHandMovement.DirMovementStatusID == null || docSecondHandMovement.DirMovementStatusID <= 1)
                {
                    if (docSecondHandMovement.DirEmployeeIDCourier > 0) docSecondHandMovement.DirMovementStatusID = 2;
                    else docSecondHandMovement.DirMovementStatusID = 1;
                }
                //Если редактируем в Логистики
                else
                {

                }

                docSecondHandMovement.DocID = doc.DocID;
                docSecondHandMovement.DirMovementDescriptionID = DirMovementDescriptionID;

                db.Entry(docSecondHandMovement).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docSecondHandMovement.doc.NumberInt == null || docSecondHandMovement.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docSecondHandMovement.DocSecondHandMovementID.ToString();
                    doc.NumberReal = docSecondHandMovement.DocSecondHandMovementID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docSecondHandMovement.DocSecondHandMovementID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocSecondHandMovementTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocSecondHandMovementID = new SQLiteParameter("@DocSecondHandMovementID", System.Data.DbType.Int32) { Value = docSecondHandMovement.DocSecondHandMovementID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocSecondHandMovementTabs WHERE DocSecondHandMovementID=@DocSecondHandMovementID;", parDocSecondHandMovementID);
                }

                //2.2. Проставляем ID-шник "DocSecondHandMovementID" для всех позиций спецификации
                for (int i = 0; i < docSecondHandMovementTabCollection.Count(); i++)
                {
                    docSecondHandMovementTabCollection[i].DocSecondHandMovementTabID = null;
                    docSecondHandMovementTabCollection[i].DocSecondHandMovementID = Convert.ToInt32(docSecondHandMovement.DocSecondHandMovementID);
                    db.Entry(docSecondHandMovementTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion


                if (UO_Action == "held" || docSecondHandMovement.Reserve)
                {
                    //Алгоритм:
                    //1. Сначало списываем товар
                    //2. Потом приходуем товар
                    //П.С. Как бы смесь Расхода + Приход



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
                    //Models.Sklad.Rem.Rem2PartyMinus[] rem2PartyMinusCollection = new Models.Sklad.Rem.Rem2PartyMinus[docSecondHandMovementTabCollection.Count()];
                    for (int i = 0; i < docSecondHandMovementTabCollection.Count(); i++)
                    {
                        #region Проверки

                        //Переменные
                        int iRem2PartyID = docSecondHandMovementTabCollection[i].Rem2PartyID;
                        double dQuantity = docSecondHandMovementTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.Rem2Party rem2Party1 = await db.Rem2Parties.FindAsync(iRem2PartyID);
                        db.Entry(rem2Party1).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (rem2Party1.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docSecondHandMovementTabCollection[i].Rem2PartyID + "</td>" +                        //партия
                                "<td>" + docSecondHandMovementTabCollection[i].DirServiceNomenID + "</td>" +                        //Код товара
                                "<td>" + docSecondHandMovementTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                                "<td>" + rem2Party1.Remnant + "</td>" +                                              //остаток партии
                                "<td>" + (docSecondHandMovementTabCollection[i].Quantity - rem2Party1.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (rem2Party1.DirWarehouseID != docSecondHandMovement.DirWarehouseIDFrom)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandMovement.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandMovement.DirWarehouseIDFrom);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docSecondHandMovementTabCollection[i].Rem2PartyID + "</td>" +       //партия
                                "<td>" + docSecondHandMovementTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + rem2Party1.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (rem2Party1.DirContractorIDOrg != docSecondHandMovement.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandMovement.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docSecondHandMovement.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docSecondHandMovementTabCollection[i].Rem2PartyID + "</td>" +       //партия
                                "<td>" + docSecondHandMovementTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                                "<td>" + rem2Party1.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg106_1
                            );
                        }
                        #endregion


                        //Если всё нормально, то меняем некоторые значения: DirWarehouseIDPurch и DirContractorID
                        docSecondHandMovementTabCollection[i].DirWarehouseIDPurch = rem2Party1.DirWarehouseID; //Склад на который пришла партия первоначально
                        DocDatePurches = rem2Party1.DocDatePurches;

                        #endregion


                        #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                        //Получаем "DocID" из списуемой партии "docSecondHandMovementTabCollection[i].DocID" для "DocIDFirst"
                        Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandMovementTabCollection[i].Rem2PartyID);

                        #endregion


                        #region Списание

                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                        rem2PartyMinus.Rem2PartyMinusID = null;
                        rem2PartyMinus.Rem2PartyID = docSecondHandMovementTabCollection[i].Rem2PartyID;
                        rem2PartyMinus.DirServiceNomenID = docSecondHandMovementTabCollection[i].DirServiceNomenID;
                        rem2PartyMinus.Quantity = docSecondHandMovementTabCollection[i].Quantity;
                        rem2PartyMinus.DirCurrencyID = docSecondHandMovementTabCollection[i].DirCurrencyID;
                        rem2PartyMinus.DirCurrencyMultiplicity = docSecondHandMovementTabCollection[i].DirCurrencyMultiplicity;
                        rem2PartyMinus.DirCurrencyRate = docSecondHandMovementTabCollection[i].DirCurrencyRate;
                        rem2PartyMinus.DirVatValue = docSecondHandMovement.DirVatValue;
                        rem2PartyMinus.DirWarehouseID = docSecondHandMovement.DirWarehouseIDFrom;
                        rem2PartyMinus.DirContractorIDOrg = docSecondHandMovement.DirContractorIDOrg;
                        rem2PartyMinus.DirServiceContractorID = _Rem2Party.DirServiceContractorID; //rem2PartyMinus.DirServiceContractorID = null;
                        rem2PartyMinus.DocID = Convert.ToInt32(docSecondHandMovement.DocID);
                        rem2PartyMinus.PriceCurrency = docSecondHandMovementTabCollection[i].PriceCurrency;
                        rem2PartyMinus.PriceVAT = docSecondHandMovementTabCollection[i].PriceVAT;
                        rem2PartyMinus.FieldID = Convert.ToInt32(docSecondHandMovementTabCollection[i].DocSecondHandMovementTabID);
                        rem2PartyMinus.Reserve = docSecondHandMovement.Reserve;

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

                        Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[docSecondHandMovementTabCollection.Count()];
                        for (int i = 0; i < docSecondHandMovementTabCollection.Count(); i++)
                        {
                            #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                            //Получаем "DocID" из списуемой партии "docSecondHandMovementTabCollection[i].DocID" для "DocIDFirst"
                            Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandMovementTabCollection[i].Rem2PartyID);

                            #endregion


                            Models.Sklad.Rem.Rem2Party rem2Party = new Models.Sklad.Rem.Rem2Party();
                            rem2Party.Rem2PartyID = null;
                            rem2Party.DirServiceNomenID = docSecondHandMovementTabCollection[i].DirServiceNomenID;
                            rem2Party.Quantity = docSecondHandMovementTabCollection[i].Quantity;
                            rem2Party.Remnant = docSecondHandMovementTabCollection[i].Quantity;
                            rem2Party.DirCurrencyID = docSecondHandMovementTabCollection[i].DirCurrencyID;
                            //rem2Party.DirCurrencyMultiplicity = docSecondHandMovementTabCollection[i].DirCurrencyMultiplicity;
                            //rem2Party.DirCurrencyRate = docSecondHandMovementTabCollection[i].DirCurrencyRate;
                            rem2Party.DirVatValue = docSecondHandMovement.DirVatValue;
                            rem2Party.DirWarehouseID = docSecondHandMovement.DirWarehouseIDTo;
                            rem2Party.DirWarehouseIDDebit = docSecondHandMovement.DirWarehouseIDFrom; //Склад с которого списали партию
                            rem2Party.DirWarehouseIDPurch = docSecondHandMovementTabCollection[i].DirWarehouseIDPurch;     //Склад на который пришла партия первоначально (получили Выше)
                            rem2Party.DirContractorIDOrg = docSecondHandMovement.DirContractorIDOrg;

                            rem2Party.DocDatePurches = DocDatePurches;

                            rem2Party.DocID = Convert.ToInt32(docSecondHandMovement.DocID);
                            rem2Party.PriceCurrency = docSecondHandMovementTabCollection[i].PriceCurrency;
                            rem2Party.PriceVAT = docSecondHandMovementTabCollection[i].PriceVAT;
                            rem2Party.FieldID = Convert.ToInt32(docSecondHandMovementTabCollection[i].DocSecondHandMovementTabID);

                            rem2Party.PriceRetailVAT = docSecondHandMovementTabCollection[i].PriceRetailVAT;
                            rem2Party.PriceRetailCurrency = docSecondHandMovementTabCollection[i].PriceRetailCurrency;
                            rem2Party.PriceWholesaleVAT = docSecondHandMovementTabCollection[i].PriceWholesaleVAT;
                            rem2Party.PriceWholesaleCurrency = docSecondHandMovementTabCollection[i].PriceWholesaleCurrency;
                            rem2Party.PriceIMVAT = docSecondHandMovementTabCollection[i].PriceIMVAT;
                            rem2Party.PriceIMCurrency = docSecondHandMovementTabCollection[i].PriceIMCurrency;

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
                }
            }
            else if (UO_Action == "held_cancel")
            {
                //Т.к. Мы и приходуем товар, то:
                #region 1. Проверка: Было ли списание с партий *** *** *** *** *** *** *** *** *** ***


                //int DocSecondHandMovementID = Convert.ToInt32(docSecondHandMovement.DocSecondHandMovementID);


                //Алгоритм №1
                //SELECT DocID
                //FROM Rem2PartyMinuses 
                //WHERE Rem2PartyID in (SELECT Rem2PartyID FROM Rem2Parties WHERE DocID=@DocID)


                #region Алгоритм №1 (OLD)

                /*
                //Получаем DocSecondHandMovement из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocSecondHandMovement _docSecondHandMovement = db.DocSecondHandMovements.Find(DocSecondHandMovementID);
                int? iDocSecondHandMovement_DocID = _docSecondHandMovement.DocID;


                var queryRem2PartyMinuses =
                    (
                        from rem2PartyMinuses in db.Rem2PartyMinuses

                        join rem2Parties1 in db.Rem2Parties on rem2PartyMinuses.Rem2PartyID equals rem2Parties1.Rem2PartyID into rem2Parties2
                        from rem2Parties in rem2Parties2.Where(x => x.DocID == iDocSecondHandMovement_DocID) //.DefaultIfEmpty()

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



                #region Алгоритм №2 (OLD)

                //Алгоритм №2
                //Пробегаемся по всем "Rem2Parties.Remnant"
                //и есть оно отличается от "Rem2Parties.Quantity"
                //то был списан товар


                /*

                //Получаем DocSecondHandMovement из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocSecondHandMovement _docSecondHandMovement = db.DocSecondHandMovements.Find(DocSecondHandMovementID);
                int? iDocSecondHandMovement_DocID = _docSecondHandMovement.DocID;

                //Есть ли списанный товар с данного прихода
                var queryRem2Parties = await Task.Run(() =>
                    (
                        from x in db.Rem2Parties
                        where x.DocID == iDocSecondHandMovement_DocID && x.Quantity != x.Remnant
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
                            from rem2Parties in rem2Parties2.Where(x => x.DocID == iDocSecondHandMovement_DocID) //.DefaultIfEmpty()

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


                #endregion


                int DocSecondHandMovementID = Convert.ToInt32(docSecondHandMovement.DocSecondHandMovementID);

                //Получаем DocPurch из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocSecondHandMovement _docSecondHandMovement = db.DocSecondHandMovements.Find(DocSecondHandMovementID);
                int? iDocSecondHandMovement_DocID = _docSecondHandMovement.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocSecondHandMovement_DocID));

                #endregion


                #region 1. Rem2PartyMinuses и Rem2Parties *** *** *** *** *** *** *** *** *** ***

                //Удаление записей в таблицах: Rem2PartyMinuses
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSecondHandMovement.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Rem2Parties WHERE DocID=@DocID;", parDocID); //DELETE FROM Rem2PartyMinuses WHERE DocID=@DocID;

                //Обновление записей: Rem2PartyMinuses
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE Rem2PartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion


                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docSecondHandMovement.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion
            }


            #region 3. Лог


            #region Пишем в Лог о смене статуса и мастера, если такое было

            logLogistic.DocLogisticID = docSecondHandMovement.DocSecondHandMovementID;
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


            return docSecondHandMovement;
        }



        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            //System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocSecondHandMovement docSecondHandMovement,
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

            docSecondHandMovement.DirMovementStatusID = StatusID;
            db.Entry(docSecondHandMovement).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 2. Если статус == 4, то проводим

            if (StatusID == 4)
            {

                #region 2.1. Doc

                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandMovement.DocID);
                doc.Held = true;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                #endregion


                #region Party


                #region Получаем колеуцию табличной части

                //1. К-во записец
                var queryTabs = await
                    (
                        from x in db.DocSecondHandMovementTabs
                        where x.DocSecondHandMovementID == docSecondHandMovement.DocSecondHandMovementID
                        select x
                    ).ToListAsync();

                Models.Sklad.Doc.DocSecondHandMovementTab[] docSecondHandMovementTabCollection = new Models.Sklad.Doc.DocSecondHandMovementTab[queryTabs.Count()];

                for (int i = 0; i < queryTabs.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandMovementTab docSecondHandMovementTab = await db.DocSecondHandMovementTabs.FindAsync(queryTabs[0].DocSecondHandMovementTabID);
                    docSecondHandMovementTabCollection[0] = docSecondHandMovementTab;
                }


                #endregion


                //Алгоритм:
                //1. Сначало списываем товар
                //2. Потом приходуем товар
                //П.С. Как бы смесь Расхода + Приход



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
                //Models.Sklad.Rem.Rem2PartyMinus[] rem2PartyMinusCollection = new Models.Sklad.Rem.Rem2PartyMinus[docSecondHandMovementTabCollection.Count()];
                for (int i = 0; i < docSecondHandMovementTabCollection.Count(); i++)
                {
                    #region Проверки

                    //Переменные
                    int iRem2PartyID = docSecondHandMovementTabCollection[i].Rem2PartyID;
                    double dQuantity = docSecondHandMovementTabCollection[i].Quantity;
                    //Находим партию
                    Models.Sklad.Rem.Rem2Party rem2Party1 = await db.Rem2Parties.FindAsync(iRem2PartyID);
                    db.Entry(rem2Party1).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                    #region 1. Есть ли остаток в партии с которой списываем!
                    if (rem2Party1.Remnant < dQuantity)
                    {
                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg104 +

                            "<tr>" +
                            "<td>" + docSecondHandMovementTabCollection[i].Rem2PartyID + "</td>" +                        //партия
                            "<td>" + docSecondHandMovementTabCollection[i].DirServiceNomenID + "</td>" +                        //Код товара
                            "<td>" + docSecondHandMovementTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                            "<td>" + rem2Party1.Remnant + "</td>" +                                              //остаток партии
                            "<td>" + (docSecondHandMovementTabCollection[i].Quantity - rem2Party1.Remnant).ToString() + "</td>" +  //недостающее к-во
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg104_1
                        );
                    }
                    #endregion

                    #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                    if (rem2Party1.DirWarehouseID != docSecondHandMovement.DirWarehouseIDFrom)
                    {
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandMovement.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandMovement.DirWarehouseIDFrom);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg105 +

                            "<tr>" +
                            "<td>" + docSecondHandMovementTabCollection[i].Rem2PartyID + "</td>" +       //партия
                            "<td>" + docSecondHandMovementTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
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
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandMovement.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docSecondHandMovement.DirContractorIDOrg);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg106 +

                            "<tr>" +
                            "<td>" + docSecondHandMovementTabCollection[i].Rem2PartyID + "</td>" +       //партия
                            "<td>" + docSecondHandMovementTabCollection[i].DirServiceNomenID + "</td>" +       //Код товара
                            "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                            "<td>" + rem2Party1.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg106_1
                        );
                    }
                    #endregion


                    //Если всё нормально, то меняем некоторые значения: DirWarehouseIDPurch и DirContractorID
                    docSecondHandMovementTabCollection[i].DirWarehouseIDPurch = rem2Party1.DirWarehouseID; //Склад на который пришла партия первоначально
                    DocDatePurches = rem2Party1.DocDatePurches;

                    #endregion


                    #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                    //Получаем "DocID" из списуемой партии "docSecondHandMovementTabCollection[i].DocID" для "DocIDFirst"
                    Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandMovementTabCollection[i].Rem2PartyID);

                    #endregion


                    #region Сохранение

                    Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                    rem2PartyMinus.Rem2PartyMinusID = null;
                    rem2PartyMinus.Rem2PartyID = docSecondHandMovementTabCollection[i].Rem2PartyID;
                    rem2PartyMinus.DirServiceNomenID = docSecondHandMovementTabCollection[i].DirServiceNomenID;
                    rem2PartyMinus.Quantity = docSecondHandMovementTabCollection[i].Quantity;
                    rem2PartyMinus.DirCurrencyID = docSecondHandMovementTabCollection[i].DirCurrencyID;
                    rem2PartyMinus.DirCurrencyMultiplicity = docSecondHandMovementTabCollection[i].DirCurrencyMultiplicity;
                    rem2PartyMinus.DirCurrencyRate = docSecondHandMovementTabCollection[i].DirCurrencyRate;
                    rem2PartyMinus.DirVatValue = docSecondHandMovement.DirVatValue;
                    rem2PartyMinus.DirWarehouseID = docSecondHandMovement.DirWarehouseIDFrom;
                    rem2PartyMinus.DirServiceContractorID = _Rem2Party.DirServiceContractorID; //rem2PartyMinus.DirServiceContractorID = null;
                    rem2PartyMinus.DirContractorIDOrg = doc.DirContractorIDOrg;
                    rem2PartyMinus.DocID = Convert.ToInt32(doc.DocID);
                    rem2PartyMinus.PriceCurrency = docSecondHandMovementTabCollection[i].PriceCurrency;
                    rem2PartyMinus.PriceVAT = docSecondHandMovementTabCollection[i].PriceVAT;
                    rem2PartyMinus.FieldID = Convert.ToInt32(docSecondHandMovementTabCollection[i].DocSecondHandMovementTabID);
                    rem2PartyMinus.Reserve = docSecondHandMovement.Reserve;

                    rem2PartyMinus.DirEmployeeID = doc.DirEmployeeID;
                    rem2PartyMinus.DocDate = doc.DocDate;

                    db.Entry(rem2PartyMinus).State = EntityState.Added;
                    await db.SaveChangesAsync();

                    #endregion
                }

                #endregion


                #region Rem2Party - Партии

                Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[docSecondHandMovementTabCollection.Count()];
                for (int i = 0; i < docSecondHandMovementTabCollection.Count(); i++)
                {
                    #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                    //Получаем "DocID" из списуемой партии "docSecondHandMovementTabCollection[i].DocID" для "DocIDFirst"
                    Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandMovementTabCollection[i].Rem2PartyID);

                    #endregion


                    Models.Sklad.Rem.Rem2Party rem2Party = new Models.Sklad.Rem.Rem2Party();
                    rem2Party.Rem2PartyID = null;
                    rem2Party.DirServiceNomenID = docSecondHandMovementTabCollection[i].DirServiceNomenID;
                    rem2Party.Quantity = docSecondHandMovementTabCollection[i].Quantity;
                    rem2Party.Remnant = docSecondHandMovementTabCollection[i].Quantity;
                    rem2Party.DirCurrencyID = docSecondHandMovementTabCollection[i].DirCurrencyID;
                    //rem2Party.DirCurrencyMultiplicity = docSecondHandMovementTabCollection[i].DirCurrencyMultiplicity;
                    //rem2Party.DirCurrencyRate = docSecondHandMovementTabCollection[i].DirCurrencyRate;
                    rem2Party.DirVatValue = docSecondHandMovement.DirVatValue;
                    rem2Party.DirWarehouseID = docSecondHandMovement.DirWarehouseIDTo;
                    rem2Party.DirWarehouseIDDebit = docSecondHandMovement.DirWarehouseIDFrom; //Склад с которого списали партию
                    rem2Party.DirWarehouseIDPurch = docSecondHandMovementTabCollection[i].DirWarehouseIDPurch;     //Склад на который пришла партия первоначально (получили Выше)
                    rem2Party.DirContractorIDOrg = doc.DirContractorIDOrg;

                    rem2Party.DocDatePurches = DocDatePurches;

                    rem2Party.DocID = Convert.ToInt32(docSecondHandMovement.DocID);
                    rem2Party.PriceCurrency = docSecondHandMovementTabCollection[i].PriceCurrency;
                    rem2Party.PriceVAT = docSecondHandMovementTabCollection[i].PriceVAT;
                    rem2Party.FieldID = Convert.ToInt32(docSecondHandMovementTabCollection[i].DocSecondHandMovementTabID);

                    rem2Party.PriceRetailVAT = docSecondHandMovementTabCollection[i].PriceRetailVAT;
                    rem2Party.PriceRetailCurrency = docSecondHandMovementTabCollection[i].PriceRetailCurrency;
                    rem2Party.PriceWholesaleVAT = docSecondHandMovementTabCollection[i].PriceWholesaleVAT;
                    rem2Party.PriceWholesaleCurrency = docSecondHandMovementTabCollection[i].PriceWholesaleCurrency;
                    rem2Party.PriceIMVAT = docSecondHandMovementTabCollection[i].PriceIMVAT;
                    rem2Party.PriceIMCurrency = docSecondHandMovementTabCollection[i].PriceIMCurrency;

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
            else logLogistic.Msg = docSecondHandMovement.dirEmployee_Courier.DirEmployeeName;

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
                "[DocSecondHandMovements].[DocSecondHandMovementID] AS [DocSecondHandMovementID], " +
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
                "[DocSecondHandMovements].[Reserve] AS [Reserve] " +

                "FROM [DocSecondHandMovements] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandMovements].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "INNER JOIN [DirWarehouses] AS [DirWarehousesFrom] ON [DirWarehousesFrom].[DirWarehouseID] = [DocSecondHandMovements].[DirWarehouseIDFrom] " +
                "INNER JOIN [DirWarehouses] AS [DirWarehousesTo] ON [DirWarehousesTo].[DirWarehouseID] = [DocSecondHandMovements].[DirWarehouseIDTo] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion
    }
}