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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocMovements
{
    public class DocMovementsController : ApiController
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

        int ListObjectID = 33;

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
            public int? DocMovementID;
        }
        // GET: api/DocMovements
        public async Task<IHttpActionResult> GetDocMovements(HttpRequestMessage request)
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
                _params.DocMovementID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocMovementID", true) == 0).Value);

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
                        from docMovements in db.DocMovements

                        join docMovementTabs1 in db.DocMovementTabs on docMovements.DocMovementID equals docMovementTabs1.DocMovementID into docMovementTabs2
                        from docMovementTabs in docMovementTabs2.DefaultIfEmpty()

                        //where docMovements.doc.DocDate >= _params.DateS && docMovements.doc.DocDate <= _params.DatePo

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

                            DocMovementID = docMovements.DocMovementID,
                            //DirContractorName = docMovements.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docMovements.doc.dirContractorOrg.DirContractorID, DirContractorNameOrg = docMovements.doc.dirContractorOrg.DirContractorName,

                            DirWarehouseIDFrom = docMovements.dirWarehouseFrom.DirWarehouseID, DirWarehouseNameFrom = docMovements.dirWarehouseFrom.DirWarehouseName,
                            DirWarehouseIDTo = docMovements.dirWarehouseTo.DirWarehouseID, DirWarehouseNameTo = docMovements.dirWarehouseTo.DirWarehouseName,

                            NumberInt = docMovements.doc.NumberInt,

                            DirEmployeeIDCourier = docMovements.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docMovements.dirEmployee_Courier.DirEmployeeName,
                            DirMovementStatusID = docMovements.DirMovementStatusID, //Курьер штрихнулся и забрал товар

                            DirEmployeeName = docMovements.doc.dirEmployee.DirEmployeeName,

                            DocDateHeld = docMovements.doc.DocDateHeld,
                            DocDatePayment = docMovements.doc.DocDatePayment,
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

                            DocMovementID = g.Key.DocMovementID,
                            //DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg, DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseIDFrom = g.Key.DirWarehouseIDFrom, DirWarehouseNameFrom = g.Key.DirWarehouseNameFrom,
                            DirWarehouseIDTo = g.Key.DirWarehouseIDTo, DirWarehouseNameTo = g.Key.DirWarehouseNameTo,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            DirEmployeeIDCourier = g.Key.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = g.Key.DirEmployeeNameCourier,
                            DirMovementStatusID = g.Key.DirMovementStatusID, //Курьер штрихнулся и забрал товар
                            DirEmployeeName = g.Key.DirEmployeeName,

                            DocDateHeld = g.Key.DocDateHeld,
                            DocDatePayment = g.Key.DocDatePayment,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Отобразить "Предыдущие ремнты"

                if (_params.DocMovementID != null && _params.DocMovementID > 0)
                {
                    query = query.Where(x => x.DocMovementID == _params.DocMovementID);
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
                    if(_params.DirMovementStatusID == 77777) query = query.Where(x => x.DirMovementStatusID > 1 && x.DirMovementStatusID != 4);
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
                        query = query.Where(x => x.DocMovementID == iNumber32);
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

                //query = query.OrderByDescending(x => x.DocMovementID).Skip(_params.Skip).Take(_params.limit);

                if (sysSetting.DocsSortField == 1)
                {
                    query = query.OrderByDescending(x => x.DocMovementID);
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
                    query = query.OrderByDescending(x => x.DocMovementID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

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
                    DocMovement = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocMovements/5
        [ResponseType(typeof(DocMovement))]
        public async Task<IHttpActionResult> GetDocMovement(int id, HttpRequestMessage request)
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

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docMovements in db.DocMovements

                        join docMovementTabs1 in db.DocMovementTabs on docMovements.DocMovementID equals docMovementTabs1.DocMovementID into docMovementTabs2
                        from docMovementTabs in docMovementTabs2.DefaultIfEmpty()

                        #endregion

                        where docMovements.DocMovementID == id

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

                            DocMovementID = docMovements.DocMovementID,
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

                            DocMovementID = g.Key.DocMovementID,
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
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //Сумма только НДС
                            SumVATCurrency =
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * (x.docMovementTabs.PriceCurrency - (x.docMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * (x.docMovementTabs.PriceCurrency - (x.docMovementTabs.PriceCurrency / (1 + (g.Key.DirVatValue) / 100)))), sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay = g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docMovementTabs.Quantity * x.docMovementTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum),

                            //Резерв
                            Reserve = g.Key.Reserve,
                            DescriptionMovement =  g.Key.DirMovementDescriptionName, //DirMovementDescriptionID = g.Key.DirMovementDescriptionID,

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

        // PUT: api/DocMovements/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocMovement(int id, DocMovement docMovement, HttpRequestMessage request)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovements"));
                //if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Возможно доступна Логистика
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovements"));
                if (iRight != 1)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovementsLogistics"));
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
                Models.Sklad.Doc.DocMovementTab[] docMovementTabCollection = null;
                if (!String.IsNullOrEmpty(docMovement.recordsDocMovementTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docMovementTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocMovementTab[]>(docMovement.recordsDocMovementTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docMovement.DocMovementID || docMovement.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //2. Если статус > 1, то редактировать может только Администратор
                if (docMovement.DirMovementStatusID > 1 && field.DirEmployeeID != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg122)); }

                //3. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docMovement.DirEmployeeIDCourier != null && docMovement.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docMovement.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }

                //4. Получаем "docMovement.DocID" из БД, если он отличается от пришедшего от клиента "docMovement.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocMovements
                        where x.DocMovementID == docMovement.DocMovementID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docMovement.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //5. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docMovement.DocID);
                //6.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //6.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docMovement.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docMovement = await Task.Run(() => mPutPostDocMovement(db, dbRead, UO_Action, docMovement, EntityState.Modified, docMovementTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docMovement.DocMovementID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docMovement.DocID,
                    DocMovementID = docMovement.DocMovementID,
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

        //Смена статуса
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocMovement(int id, int DirStatusID, HttpRequestMessage request)
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


                Models.Sklad.Doc.DocMovement docMovement = await db.DocMovements.FindAsync(id);
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docMovement.DocID);


                #region Проверки

                //1. Если документ проведён то выдать сообщение
                if (Convert.ToBoolean(doc.Held)) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3)); }

                //2. Нельзя перескакивать через статусы (статус всегда +/- 1)
                if (Math.Abs(DirStatusID - Convert.ToInt32(docMovement.DirMovementStatusID)) != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_4)); }

                //3. Если статус == 3, то проверяем введённый пароль и чистим его (что бы не писался в Лог)
                if (DirStatusID == 3)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(docMovement.DirEmployeeIDCourier);
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
                                if (docMovement.DirWarehouseIDTo == queryW[0].DirWarehouseID) { bFindWarehouse = true; break; }
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
                            //await mStatusChange(db, ts, docMovement, id, DirStatusID, sDiagnosticRresults, field);
                            await mStatusChange(db, dbRead, docMovement, id, DirStatusID, sDiagnosticRresults, field);

                            //docMovement = await Task.Run(() => mPutPostDocMovement(db, dbRead, UO_Action, docMovement, EntityState.Modified, docMovementTabCollection, field)); //sysSetting

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
                        DocID = docMovement.DocID,
                        DocMovementID = docMovement.DocMovementID
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


        // POST: api/DocMovements
        [ResponseType(typeof(DocMovement))]
        public async Task<IHttpActionResult> PostDocMovement(DocMovement docMovement, HttpRequestMessage request)
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
                string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
                if (UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
                UO_Action = UO_Action.ToLower();


                //Проверяме пароль
                string DirEmployeePswd = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeePswd", true) == 0).Value;
                Classes.Account.EncodeDecode encode = new Classes.Account.EncodeDecode();
                if (DirEmployeePswd != encode.UnionDecode(authCookie["CookieP"])) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_6));


                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocMovementTab[] docMovementTabCollection = null;
                if (!String.IsNullOrEmpty(docMovement.recordsDocMovementTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docMovementTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocMovementTab[]>(docMovement.recordsDocMovementTab);
                }

                #endregion

                #region Проверки

                //1. 
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);


                //2. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docMovement.DirEmployeeIDCourier != null && docMovement.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docMovement.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }


                //3. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docMovement.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docMovement = await Task.Run(() => mPutPostDocMovement(db, dbRead, UO_Action, docMovement, EntityState.Added, docMovementTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docMovement.DocMovementID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docMovement.DocID,
                    DocMovementID = docMovement.DocMovementID,
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

        // DELETE: api/DocMovements/5
        [ResponseType(typeof(DocMovement))]
        public async Task<IHttpActionResult> DeleteDocMovement(int id)
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

                #region Проверки

                try
                {
                    //Документ проведён!<BR>Перед удалением, нужно отменить проводку!
                    var queryHeld = await Task.Run(() =>
                        (
                            from x in dbRead.DocMovements
                            where x.DocMovementID == id
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
                //2. DocMovementTabs
                //3. DocMovements
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocMovement docMovement = await db.DocMovements.FindAsync(id);
                if (docMovement == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. RemPartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocMovements
                                where x.DocMovementID == id
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

                        //1.2. Удаляем "RemParties"
                        var queryRemParties = await
                            (
                                from x in db.RemParties
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryRemParties.Count(); i++)
                        {
                            Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(queryRemParties[i].RemPartyID);
                            db.RemParties.Remove(remParty);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 2. DocMovementTabs *** *** *** *** ***

                        var queryDocMovementTabs = await
                            (
                                from x in db.DocMovementTabs
                                where x.DocMovementID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocMovementTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocMovementTab docMovementTab = await db.DocMovementTabs.FindAsync(queryDocMovementTabs[i].DocMovementTabID);
                            db.DocMovementTabs.Remove(docMovementTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocMovements *** *** *** *** ***

                        var queryDocMovements = await
                            (
                                from x in db.DocMovements
                                where x.DocMovementID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocMovements.Count(); i++)
                        {
                            Models.Sklad.Doc.DocMovement docMovement1 = await db.DocMovements.FindAsync(queryDocMovements[i].DocMovementID);
                            db.DocMovements.Remove(docMovement1);
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

        private bool DocMovementExists(int id)
        {
            return db.DocMovements.Count(e => e.DocMovementID == id) > 0;
        }


        bool bFindWarehouse = false;
        internal async Task<DocMovement> mPutPostDocMovement(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocMovement docMovement,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocMovementTab[] docMovementTabCollection,

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
                        if (docMovement.DirWarehouseIDTo == queryW[i].DirWarehouseID) { bFindWarehouse = true; break; }
                    }
                    if (!bFindWarehouse) UO_Action = "save";
                }
            }
            else
            {
                bFindWarehouse = true;
            }

            #endregion


            if (UO_Action == "held") docMovement.Reserve = false;
            else docMovement.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docMovement.NumberInt;
                doc.NumberReal = docMovement.DocMovementID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = 1;
                doc.Payment = 0;
                doc.DirContractorID = docMovement.DirContractorIDOrg;
                doc.DirContractorIDOrg = docMovement.DirContractorIDOrg;
                doc.Discount = docMovement.Discount;
                doc.DirVatValue = docMovement.DirVatValue;
                doc.Base = docMovement.Base;
                doc.Description = docMovement.Description;
                doc.DocDate = docMovement.DocDate;
                //doc.DocDisc = docMovement.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docMovement.DocID;
                doc.DocIDBase = docMovement.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docMovement" со всем полями!
                docMovement.DocID = doc.DocID;

                #endregion

                #region 4. DescriptionMovement: пишем ID-шник в DocMovementTab и RemParty

                string DescriptionMovement = ""; if (docMovementTabCollection.Length > 0) DescriptionMovement = docMovement.DescriptionMovement;
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

                #region 2. DocMovement *** *** *** *** *** *** *** *** *** ***

                //Если ещё не переместили в Логистику
                if (docMovement.DirMovementStatusID==null || docMovement.DirMovementStatusID <= 1)
                {
                    if (docMovement.DirEmployeeIDCourier > 0) docMovement.DirMovementStatusID = 2;
                    else docMovement.DirMovementStatusID = 1;
                }
                //Если редактируем в Логистики
                else
                {

                }

                docMovement.DocID = doc.DocID;
                docMovement.DirMovementDescriptionID = DirMovementDescriptionID;

                db.Entry(docMovement).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docMovement.doc.NumberInt == null || docMovement.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docMovement.DocMovementID.ToString();
                    doc.NumberReal = docMovement.DocMovementID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docMovement.DocMovementID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocMovementTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocMovementID = new SQLiteParameter("@DocMovementID", System.Data.DbType.Int32) { Value = docMovement.DocMovementID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocMovementTabs WHERE DocMovementID=@DocMovementID;", parDocMovementID);
                }

                //2.2. Проставляем ID-шник "DocMovementID" для всех позиций спецификации
                for (int i = 0; i < docMovementTabCollection.Count(); i++)
                {
                    docMovementTabCollection[i].DocMovementTabID = null;
                    docMovementTabCollection[i].DocMovementID = Convert.ToInt32(docMovement.DocMovementID);
                    db.Entry(docMovementTabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion


                if (UO_Action == "held" || docMovement.Reserve)
                {
                    //Алгоритм:
                    //1. Сначало списываем товар
                    //2. Потом приходуем товар
                    //П.С. Как бы смесь Расхода + Приход



                    //1. === === === Списание === === ===

                    Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();


                    #region Удаляем все записи из таблицы "RemPartyMinuses"
                    //Удаляем все записи из таблицы "RemPartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (RemPartyMinuses)

                    DateTime? DocDatePurches = new DateTime();
                    //Models.Sklad.Rem.RemPartyMinus[] remPartyMinusCollection = new Models.Sklad.Rem.RemPartyMinus[docMovementTabCollection.Count()];
                    for (int i = 0; i < docMovementTabCollection.Count(); i++)
                    {
                        #region Проверки

                        //Переменные
                        int iRemPartyID = docMovementTabCollection[i].RemPartyID;
                        double dQuantity = docMovementTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.RemParty remParty1 = await db.RemParties.FindAsync(iRemPartyID);
                        db.Entry(remParty1).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (remParty1.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docMovementTabCollection[i].RemPartyID + "</td>" +                        //партия
                                "<td>" + docMovementTabCollection[i].DirNomenID + "</td>" +                        //Код товара
                                "<td>" + docMovementTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                                "<td>" + remParty1.Remnant + "</td>" +                                              //остаток партии
                                "<td>" + (docMovementTabCollection[i].Quantity - remParty1.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (remParty1.DirWarehouseID != docMovement.DirWarehouseIDFrom)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docMovement.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docMovement.DirWarehouseIDFrom);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docMovementTabCollection[i].RemPartyID + "</td>" +       //партия
                                "<td>" + docMovementTabCollection[i].DirNomenID + "</td>" +       //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + remParty1.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (remParty1.DirContractorIDOrg != docMovement.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docMovement.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docMovement.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docMovementTabCollection[i].RemPartyID + "</td>" +       //партия
                                "<td>" + docMovementTabCollection[i].DirNomenID + "</td>" +       //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                                "<td>" + remParty1.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg106_1
                            );
                        }
                        #endregion


                        //Если всё нормально, то меняем некоторые значения: DirWarehouseIDPurch и DirContractorID
                        docMovementTabCollection[i].DirContractorID = remParty1.DirContractorID;    //Поставщика от которого пришла партия первоначально
                        docMovementTabCollection[i].DirWarehouseIDPurch = remParty1.DirWarehouseID; //Склад на который пришла партия первоначально
                        DocDatePurches = remParty1.DocDatePurches;

                        #endregion


                        #region Сохранение

                        Models.Sklad.Rem.RemPartyMinus remPartyMinus = new Models.Sklad.Rem.RemPartyMinus();
                        remPartyMinus.RemPartyMinusID = null;
                        remPartyMinus.RemPartyID = docMovementTabCollection[i].RemPartyID;
                        remPartyMinus.DirNomenID = docMovementTabCollection[i].DirNomenID;
                        remPartyMinus.Quantity = docMovementTabCollection[i].Quantity;
                        remPartyMinus.DirCurrencyID = docMovementTabCollection[i].DirCurrencyID;
                        remPartyMinus.DirCurrencyMultiplicity = docMovementTabCollection[i].DirCurrencyMultiplicity;
                        remPartyMinus.DirCurrencyRate = docMovementTabCollection[i].DirCurrencyRate;
                        remPartyMinus.DirVatValue = docMovement.DirVatValue;
                        remPartyMinus.DirWarehouseID = docMovement.DirWarehouseIDFrom;
                        remPartyMinus.DirContractorIDOrg = docMovement.DirContractorIDOrg;
                        remPartyMinus.DirContractorID = docMovement.DirContractorIDOrg;
                        remPartyMinus.DocID = Convert.ToInt32(docMovement.DocID);
                        remPartyMinus.PriceCurrency = docMovementTabCollection[i].PriceCurrency;
                        remPartyMinus.PriceVAT = docMovementTabCollection[i].PriceVAT;
                        remPartyMinus.FieldID = Convert.ToInt32(docMovementTabCollection[i].DocMovementTabID);
                        remPartyMinus.Reserve = docMovement.Reserve;

                        remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        remPartyMinus.DocDate = doc.DocDate;

                        db.Entry(remPartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion
                    }

                    #endregion


                    //2. === === === Оприходование === === ===
                    if (UO_Action == "held")
                    {
                        #region RemParty - Партии

                        Models.Sklad.Rem.RemParty[] remPartyCollection = new Models.Sklad.Rem.RemParty[docMovementTabCollection.Count()];
                        for (int i = 0; i < docMovementTabCollection.Count(); i++)
                        {
                            Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();
                            remParty.RemPartyID = null;
                            remParty.DirNomenID = docMovementTabCollection[i].DirNomenID;
                            remParty.Quantity = docMovementTabCollection[i].Quantity;
                            remParty.Remnant = docMovementTabCollection[i].Quantity;
                            remParty.DirCurrencyID = docMovementTabCollection[i].DirCurrencyID;
                            //remParty.DirCurrencyMultiplicity = docMovementTabCollection[i].DirCurrencyMultiplicity;
                            //remParty.DirCurrencyRate = docMovementTabCollection[i].DirCurrencyRate;
                            remParty.DirVatValue = docMovement.DirVatValue;
                            remParty.DirWarehouseID = docMovement.DirWarehouseIDTo;
                            remParty.DirWarehouseIDDebit = docMovement.DirWarehouseIDFrom; //Склад с которого списали партию
                            remParty.DirWarehouseIDPurch = docMovementTabCollection[i].DirWarehouseIDPurch;     //Склад на который пришла партия первоначально (получили Выше)
                            remParty.DirContractorIDOrg = docMovement.DirContractorIDOrg;
                            remParty.DirContractorID = docMovementTabCollection[i].DirContractorID;        //Поставщика от которого пришла партия первоначально (получили Выше)

                            remParty.DocDatePurches = DocDatePurches;

                            remParty.DirCharColourID = docMovementTabCollection[i].DirCharColourID;
                            remParty.DirCharMaterialID = docMovementTabCollection[i].DirCharMaterialID;
                            remParty.DirCharNameID = docMovementTabCollection[i].DirCharNameID;
                            remParty.DirCharSeasonID = docMovementTabCollection[i].DirCharSeasonID;
                            remParty.DirCharSexID = docMovementTabCollection[i].DirCharSexID;
                            remParty.DirCharSizeID = docMovementTabCollection[i].DirCharSizeID;
                            remParty.DirCharStyleID = docMovementTabCollection[i].DirCharStyleID;
                            remParty.DirCharTextureID = docMovementTabCollection[i].DirCharTextureID;

                            remParty.SerialNumber = docMovementTabCollection[i].SerialNumber;
                            remParty.Barcode = docMovementTabCollection[i].Barcode;

                            remParty.DocID = Convert.ToInt32(docMovement.DocID);
                            remParty.PriceCurrency = docMovementTabCollection[i].PriceCurrency;
                            remParty.PriceVAT = docMovementTabCollection[i].PriceVAT;
                            remParty.FieldID = Convert.ToInt32(docMovementTabCollection[i].DocMovementTabID);

                            remParty.PriceRetailVAT = docMovementTabCollection[i].PriceRetailVAT;
                            remParty.PriceRetailCurrency = docMovementTabCollection[i].PriceRetailCurrency;
                            remParty.PriceWholesaleVAT = docMovementTabCollection[i].PriceWholesaleVAT;
                            remParty.PriceWholesaleCurrency = docMovementTabCollection[i].PriceWholesaleCurrency;
                            remParty.PriceIMVAT = docMovementTabCollection[i].PriceIMVAT;
                            remParty.PriceIMCurrency = docMovementTabCollection[i].PriceIMCurrency;

                            //DirNomenMinimumBalance
                            remParty.DirNomenMinimumBalance = sysSetting.DirNomenMinimumBalance;

                            remParty.DirEmployeeID = doc.DirEmployeeID;
                            remParty.DocDate = doc.DocDate;

                            remPartyCollection[i] = remParty;
                        }

                        Controllers.Sklad.Rem.RemPartiesController remPartys = new Rem.RemPartiesController();
                        await Task.Run(() => remPartys.Save(db, remPartyCollection)); //remPartys.Save(db, remPartyCollection);

                        #endregion
                    }
                }
            }
            else if (UO_Action == "held_cancel")
            {
                //Т.к. Мы и приходуем товар, то:
                #region 1. Проверка: Было ли списание с партий *** *** *** *** *** *** *** *** *** ***


                //int DocMovementID = Convert.ToInt32(docMovement.DocMovementID);


                //Алгоритм №1
                //SELECT DocID
                //FROM RemPartyMinuses 
                //WHERE RemPartyID in (SELECT RemPartyID FROM RemParties WHERE DocID=@DocID)


                #region Алгоритм №1 (OLD)

                /*
                //Получаем DocMovement из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocMovement _docMovement = db.DocMovements.Find(DocMovementID);
                int? iDocMovement_DocID = _docMovement.DocID;


                var queryRemPartyMinuses =
                    (
                        from remPartyMinuses in db.RemPartyMinuses

                        join remParties1 in db.RemParties on remPartyMinuses.RemPartyID equals remParties1.RemPartyID into remParties2
                        from remParties in remParties2.Where(x => x.DocID == iDocMovement_DocID) //.DefaultIfEmpty()

                        select new
                        {
                            DocID = remPartyMinuses.DocID,
                            ListObjectNameRu = remPartyMinuses.doc.listObject.ListObjectNameRu
                        }
                    ).Distinct().ToList(); // - убрать повторяющиеся

                //Есть списания!
                if (queryRemPartyMinuses.Count() > 0)
                {
                    //Поиск всех DocID
                    string arrDocID = "";
                    for (int i = 0; i < queryRemPartyMinuses.Count(); i++)
                    {
                        arrDocID += queryRemPartyMinuses[i].DocID + " (" + queryRemPartyMinuses[i].ListObjectNameRu + ")";
                        if (i != queryRemPartyMinuses.Count() - 1) arrDocID += "<br />";
                    }
                    //Сообщение клиенту
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg102 + arrDocID + Classes.Language.Sklad.Language.msg102_1);
                }
                */

                #endregion



                #region Алгоритм №2 (OLD)

                //Алгоритм №2
                //Пробегаемся по всем "RemParties.Remnant"
                //и есть оно отличается от "RemParties.Quantity"
                //то был списан товар


                /*

                //Получаем DocMovement из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocMovement _docMovement = db.DocMovements.Find(DocMovementID);
                int? iDocMovement_DocID = _docMovement.DocID;

                //Есть ли списанный товар с данного прихода
                var queryRemParties = await Task.Run(() =>
                    (
                        from x in db.RemParties
                        where x.DocID == iDocMovement_DocID && x.Quantity != x.Remnant
                        select x
                    ).ToListAsync());

                //Есть!
                if (queryRemParties.Count() > 0)
                {
                    //Смотрим, какие именно накладные списали товар.
                    var queryRemPartyMinuses = await Task.Run(() =>
                        (
                            from remPartyMinuses in db.RemPartyMinuses

                            join remParties1 in db.RemParties on remPartyMinuses.RemPartyID equals remParties1.RemPartyID into remParties2
                            from remParties in remParties2.Where(x => x.DocID == iDocMovement_DocID) //.DefaultIfEmpty()

                            select new
                            {
                                DocID = remPartyMinuses.DocID,
                                ListObjectNameRu = remPartyMinuses.doc.listObject.ListObjectNameRu
                            }
                        ).Distinct().ToListAsync()); // - убрать повторяющиеся

                    //Есть списания!
                    if (queryRemPartyMinuses.Count() > 0)
                    {
                        //Поиск всех DocID
                        string arrDocID = "";
                        for (int i = 0; i < queryRemPartyMinuses.Count(); i++)
                        {
                            arrDocID += queryRemPartyMinuses[i].DocID + " (" + queryRemPartyMinuses[i].ListObjectNameRu + ")";
                            if (i != queryRemPartyMinuses.Count() - 1) arrDocID += "<br />";
                        }
                        //Сообщение клиенту
                        throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg102 + arrDocID + Classes.Language.Sklad.Language.msg102_1);
                    }

                }

                */


                #endregion


                int DocMovementID = Convert.ToInt32(docMovement.DocMovementID);

                //Получаем DocPurch из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocMovement _docMovement = db.DocMovements.Find(DocMovementID);
                int? iDocMovement_DocID = _docMovement.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocMovement_DocID));

                #endregion


                #region 1. RemPartyMinuses и RemParties *** *** *** *** *** *** *** *** *** ***

                //Удаление записей в таблицах: RemPartyMinuses
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docMovement.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID;", parDocID); //DELETE FROM RemPartyMinuses WHERE DocID=@DocID;

                //Обновление записей: RemPartyMinuses
                SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                await db.Database.ExecuteSqlCommandAsync("UPDATE RemPartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion


                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docMovement.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion
            }


            #region 3. Лог - не используется


            #region Пишем в Лог о смене статуса и мастера, если такое было
            /*
            logLogistic.DocLogisticID = docMovement.DocMovementID;
            logLogistic.DirMovementLogTypeID = 1; //Смена статуса
            logLogistic.DirEmployeeID = field.DirEmployeeID;
            logLogistic.DirMovementStatusID = 1;
            //logLogistic.Msg = "Создание документа";

            await logLogisticsController.mPutPostLogLogistics(db, logLogistic, EntityState.Added);
            */
            #endregion


            #endregion



            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docMovement;
        }



        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            //System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocMovement docMovement,
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
                        where x.DocLogisticID == id && x.DirMovementStatusID != null
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

            docMovement.DirMovementStatusID = StatusID;
            db.Entry(docMovement).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 2. Если статус == 4, то проводим

            if (StatusID == 4)
            {

                #region 2.1. Doc

                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docMovement.DocID);
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
                        from x in db.DocMovementTabs
                        where x.DocMovementID == docMovement.DocMovementID
                        select x
                    ).ToListAsync();

                Models.Sklad.Doc.DocMovementTab[] docMovementTabCollection = new Models.Sklad.Doc.DocMovementTab[queryTabs.Count()];

                for (int i = 0; i < queryTabs.Count(); i++)
                {
                    Models.Sklad.Doc.DocMovementTab docMovementTab = await db.DocMovementTabs.FindAsync(queryTabs[0].DocMovementTabID);
                    docMovementTabCollection[0] = docMovementTab;
                }


                #endregion


                //Алгоритм:
                //1. Сначало списываем товар
                //2. Потом приходуем товар
                //П.С. Как бы смесь Расхода + Приход



                //1. === === === Списание === === ===

                Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();


                #region Удаляем все записи из таблицы "RemPartyMinuses"
                //Удаляем все записи из таблицы "RemPartyMinuses"
                //Что бы правильно Проверяло на Остаток.
                //А то, товар уже списан, а я проверяю на остаток!

                await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                #endregion


                #region Проверки и Списание с партий (RemPartyMinuses)

                DateTime? DocDatePurches = new DateTime();
                //Models.Sklad.Rem.RemPartyMinus[] remPartyMinusCollection = new Models.Sklad.Rem.RemPartyMinus[docMovementTabCollection.Count()];
                for (int i = 0; i < docMovementTabCollection.Count(); i++)
                {
                    #region Проверки

                    //Переменные
                    int iRemPartyID = docMovementTabCollection[i].RemPartyID;
                    double dQuantity = docMovementTabCollection[i].Quantity;
                    //Находим партию
                    Models.Sklad.Rem.RemParty remParty1 = await db.RemParties.FindAsync(iRemPartyID);
                    db.Entry(remParty1).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                    #region 1. Есть ли остаток в партии с которой списываем!
                    if (remParty1.Remnant < dQuantity)
                    {
                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg104 +

                            "<tr>" +
                            "<td>" + docMovementTabCollection[i].RemPartyID + "</td>" +                        //партия
                            "<td>" + docMovementTabCollection[i].DirNomenID + "</td>" +                        //Код товара
                            "<td>" + docMovementTabCollection[i].Quantity + "</td>" +                          //списуемое к-во
                            "<td>" + remParty1.Remnant + "</td>" +                                              //остаток партии
                            "<td>" + (docMovementTabCollection[i].Quantity - remParty1.Remnant).ToString() + "</td>" +  //недостающее к-во
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg104_1
                        );
                    }
                    #endregion

                    #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                    if (remParty1.DirWarehouseID != docMovement.DirWarehouseIDFrom)
                    {
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docMovement.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docMovement.DirWarehouseIDFrom);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg105 +

                            "<tr>" +
                            "<td>" + docMovementTabCollection[i].RemPartyID + "</td>" +       //партия
                            "<td>" + docMovementTabCollection[i].DirNomenID + "</td>" +       //Код товара
                            "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                            "<td>" + remParty1.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg105_1
                        );
                    }
                    #endregion

                    #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                    if (remParty1.DirContractorIDOrg != doc.DirContractorIDOrg)
                    {
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docMovement.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docMovement.DirContractorIDOrg);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg106 +

                            "<tr>" +
                            "<td>" + docMovementTabCollection[i].RemPartyID + "</td>" +       //партия
                            "<td>" + docMovementTabCollection[i].DirNomenID + "</td>" +       //Код товара
                            "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                            "<td>" + remParty1.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg106_1
                        );
                    }
                    #endregion


                    //Если всё нормально, то меняем некоторые значения: DirWarehouseIDPurch и DirContractorID
                    docMovementTabCollection[i].DirContractorID = remParty1.DirContractorID;    //Поставщика от которого пришла партия первоначально
                    docMovementTabCollection[i].DirWarehouseIDPurch = remParty1.DirWarehouseID; //Склад на который пришла партия первоначально
                    DocDatePurches = remParty1.DocDatePurches;

                    #endregion


                    #region Сохранение

                    Models.Sklad.Rem.RemPartyMinus remPartyMinus = new Models.Sklad.Rem.RemPartyMinus();
                    remPartyMinus.RemPartyMinusID = null;
                    remPartyMinus.RemPartyID = docMovementTabCollection[i].RemPartyID;
                    remPartyMinus.DirNomenID = docMovementTabCollection[i].DirNomenID;
                    remPartyMinus.Quantity = docMovementTabCollection[i].Quantity;
                    remPartyMinus.DirCurrencyID = docMovementTabCollection[i].DirCurrencyID;
                    remPartyMinus.DirCurrencyMultiplicity = docMovementTabCollection[i].DirCurrencyMultiplicity;
                    remPartyMinus.DirCurrencyRate = docMovementTabCollection[i].DirCurrencyRate;
                    remPartyMinus.DirVatValue = docMovement.DirVatValue;
                    remPartyMinus.DirWarehouseID = docMovement.DirWarehouseIDFrom;
                    remPartyMinus.DirContractorIDOrg = doc.DirContractorIDOrg;
                    remPartyMinus.DirContractorID = doc.DirContractorIDOrg;
                    remPartyMinus.DocID = Convert.ToInt32(doc.DocID);
                    remPartyMinus.PriceCurrency = docMovementTabCollection[i].PriceCurrency;
                    remPartyMinus.PriceVAT = docMovementTabCollection[i].PriceVAT;
                    remPartyMinus.FieldID = Convert.ToInt32(docMovementTabCollection[i].DocMovementTabID);
                    remPartyMinus.Reserve = docMovement.Reserve;

                    remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
                    remPartyMinus.DocDate = doc.DocDate;

                    db.Entry(remPartyMinus).State = EntityState.Added;
                    await db.SaveChangesAsync();

                    #endregion
                }

                #endregion


                #region RemParty - Партии

                Models.Sklad.Rem.RemParty[] remPartyCollection = new Models.Sklad.Rem.RemParty[docMovementTabCollection.Count()];
                for (int i = 0; i < docMovementTabCollection.Count(); i++)
                {
                    Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();
                    remParty.RemPartyID = null;
                    remParty.DirNomenID = docMovementTabCollection[i].DirNomenID;
                    remParty.Quantity = docMovementTabCollection[i].Quantity;
                    remParty.Remnant = docMovementTabCollection[i].Quantity;
                    remParty.DirCurrencyID = docMovementTabCollection[i].DirCurrencyID;
                    //remParty.DirCurrencyMultiplicity = docMovementTabCollection[i].DirCurrencyMultiplicity;
                    //remParty.DirCurrencyRate = docMovementTabCollection[i].DirCurrencyRate;
                    remParty.DirVatValue = docMovement.DirVatValue;
                    remParty.DirWarehouseID = docMovement.DirWarehouseIDTo;
                    remParty.DirWarehouseIDDebit = docMovement.DirWarehouseIDFrom; //Склад с которого списали партию
                    remParty.DirWarehouseIDPurch = docMovementTabCollection[i].DirWarehouseIDPurch;     //Склад на который пришла партия первоначально (получили Выше)
                    remParty.DirContractorIDOrg = doc.DirContractorIDOrg;
                    remParty.DirContractorID = docMovementTabCollection[i].DirContractorID;        //Поставщика от которого пришла партия первоначально (получили Выше)

                    remParty.DocDatePurches = DocDatePurches;

                    remParty.DirCharColourID = docMovementTabCollection[i].DirCharColourID;
                    remParty.DirCharMaterialID = docMovementTabCollection[i].DirCharMaterialID;
                    remParty.DirCharNameID = docMovementTabCollection[i].DirCharNameID;
                    remParty.DirCharSeasonID = docMovementTabCollection[i].DirCharSeasonID;
                    remParty.DirCharSexID = docMovementTabCollection[i].DirCharSexID;
                    remParty.DirCharSizeID = docMovementTabCollection[i].DirCharSizeID;
                    remParty.DirCharStyleID = docMovementTabCollection[i].DirCharStyleID;
                    remParty.DirCharTextureID = docMovementTabCollection[i].DirCharTextureID;

                    remParty.SerialNumber = docMovementTabCollection[i].SerialNumber;
                    remParty.Barcode = docMovementTabCollection[i].Barcode;

                    remParty.DocID = Convert.ToInt32(docMovement.DocID);
                    remParty.PriceCurrency = docMovementTabCollection[i].PriceCurrency;
                    remParty.PriceVAT = docMovementTabCollection[i].PriceVAT;
                    remParty.FieldID = Convert.ToInt32(docMovementTabCollection[i].DocMovementTabID);

                    remParty.PriceRetailVAT = docMovementTabCollection[i].PriceRetailVAT;
                    remParty.PriceRetailCurrency = docMovementTabCollection[i].PriceRetailCurrency;
                    remParty.PriceWholesaleVAT = docMovementTabCollection[i].PriceWholesaleVAT;
                    remParty.PriceWholesaleCurrency = docMovementTabCollection[i].PriceWholesaleCurrency;
                    remParty.PriceIMVAT = docMovementTabCollection[i].PriceIMVAT;
                    remParty.PriceIMCurrency = docMovementTabCollection[i].PriceIMCurrency;

                    remParty.DirEmployeeID = doc.DirEmployeeID;
                    remParty.DocDate = doc.DocDate;
                    remParty.DirNomenMinimumBalance = 1;

                    remPartyCollection[i] = remParty;
                }

                Controllers.Sklad.Rem.RemPartiesController remPartys = new Rem.RemPartiesController();
                await Task.Run(() => remPartys.Save(db, remPartyCollection)); //remPartys.Save(db, remPartyCollection);

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
            if(!String.IsNullOrEmpty(sDiagnosticRresults) && StatusID != 3) logLogistic.Msg = sDiagnosticRresults;
            else logLogistic.Msg = docMovement.dirEmployee_Courier.DirEmployeeName;

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
                "[DocMovements].[DocMovementID] AS [DocMovementID], " +
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
                "[DocMovements].[Reserve] AS [Reserve] " +

                "FROM [DocMovements] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocMovements].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "INNER JOIN [DirWarehouses] AS [DirWarehousesFrom] ON [DirWarehousesFrom].[DirWarehouseID] = [DocMovements].[DirWarehouseIDFrom] " +
                "INNER JOIN [DirWarehouses] AS [DirWarehousesTo] ON [DirWarehousesTo].[DirWarehouseID] = [DocMovements].[DirWarehouseIDTo] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion
    }
}