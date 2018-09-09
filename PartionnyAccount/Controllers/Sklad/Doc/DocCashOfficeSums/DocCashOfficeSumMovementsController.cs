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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocCashOfficeSums
{
    public class DocCashOfficeSumMovementsController : ApiController
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

        int ListObjectID = 79;

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
            public int? DocCashOfficeSumMovementID;
        }
        // GET: api/DocCashOfficeSumMovements
        public async Task<IHttpActionResult> GetDocCashOfficeSumMovements(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovementsLogistics"));
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
                _params.DocCashOfficeSumMovementID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocCashOfficeSumMovementID", true) == 0).Value);

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
                        from docCashOfficeSumMovements in db.DocCashOfficeSumMovements
                        select new
                        {
                            DocID = docCashOfficeSumMovements.DocID,
                            DocDate = docCashOfficeSumMovements.doc.DocDate,
                            Base = docCashOfficeSumMovements.doc.Base,
                            Held = docCashOfficeSumMovements.doc.Held,
                            Del = docCashOfficeSumMovements.doc.Del,
                            Description = docCashOfficeSumMovements.doc.Description,
                            IsImport = docCashOfficeSumMovements.doc.IsImport,

                            DocCashOfficeSumMovementID = docCashOfficeSumMovements.DocCashOfficeSumMovementID,
                            DirContractorIDOrg = docCashOfficeSumMovements.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docCashOfficeSumMovements.doc.dirContractorOrg.DirContractorName,


                            //Точка  + Касса
                            DirWarehouseIDFrom = docCashOfficeSumMovements.DirWarehouseIDFrom,
                            DirWarehouseNameFrom = docCashOfficeSumMovements.dirWarehouseFrom.DirWarehouseName,

                            DirCashOfficeIDFrom = docCashOfficeSumMovements.DirCashOfficeIDFrom,
                            DirCashOfficeNameFrom = docCashOfficeSumMovements.dirCashOfficeFrom.DirCashOfficeName,

                            //Точка  + Касса
                            DirWarehouseIDTo = docCashOfficeSumMovements.DirWarehouseIDTo,
                            DirWarehouseNameTo = docCashOfficeSumMovements.dirWarehouseTo.DirWarehouseName,

                            DirCashOfficeIDTo = docCashOfficeSumMovements.DirCashOfficeIDTo,
                            DirCashOfficeNameTo = docCashOfficeSumMovements.dirCashOfficeTo.DirCashOfficeName,



                            NumberInt = docCashOfficeSumMovements.doc.NumberInt,

                            DirEmployeeIDCourier = docCashOfficeSumMovements.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docCashOfficeSumMovements.dirEmployee_Courier.DirEmployeeName,
                            DirMovementStatusID = docCashOfficeSumMovements.DirMovementStatusID, //Курьер штрихнулся и забрал товар

                            DirEmployeeName = docCashOfficeSumMovements.doc.dirEmployee.DirEmployeeName,

                            DocDateHeld = docCashOfficeSumMovements.doc.DocDateHeld,
                            DocDatePayment = docCashOfficeSumMovements.doc.DocDatePayment,


                            Sums = docCashOfficeSumMovements.Sums,
                            SumsCurrency = docCashOfficeSumMovements.SumsCurrency,

                            DirCurrencyID = docCashOfficeSumMovements.DirCurrencyID,
                            DirCurrencyRate = docCashOfficeSumMovements.DirCurrencyRate,
                            DirCurrencyMultiplicity = docCashOfficeSumMovements.DirCurrencyMultiplicity,
                            DirCurrencyName = docCashOfficeSumMovements.dirCurrency.DirCurrencyName + " (" + docCashOfficeSumMovements.DirCurrencyRate + ", " + docCashOfficeSumMovements.DirCurrencyMultiplicity + ")",

                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Отобразить "Предыдущие ремнты"

                if (_params.DocCashOfficeSumMovementID != null && _params.DocCashOfficeSumMovementID > 0)
                {
                    query = query.Where(x => x.DocCashOfficeSumMovementID == _params.DocCashOfficeSumMovementID);
                }

                #endregion


                #region dirEmployee: dirEmployee.DirWarehouseID and/or dirEmployee.DirContractorIDOrg

                /*if (_params.DirWarehouseID != null && _params.DirWarehouseID > 0)
                {
                    query = query.Where(x => x.DirWarehouseIDTo == _params.DirWarehouseID);
                }*/
                //Если привязка к сотруднику (для документа "DocCashOfficeSumMovements" показать все склады)
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
                        query = query.Where(x => x.DocCashOfficeSumMovementID == iNumber32);
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

                //query = query.OrderByDescending(x => x.DocCashOfficeSumMovementID).Skip(_params.Skip).Take(_params.limit);

                if (sysSetting.DocsSortField == 1)
                {
                    query = query.OrderByDescending(x => x.DocCashOfficeSumMovementID);
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
                    query = query.OrderByDescending(x => x.DocCashOfficeSumMovementID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocCashOfficeSumMovements.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocCashOfficeSumMovement = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocCashOfficeSumMovements/5
        [ResponseType(typeof(DocCashOfficeSumMovement))]
        public async Task<IHttpActionResult> GetDocCashOfficeSumMovement(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovementsLogistics"));
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
                        from docCashOfficeSumMovements in db.DocCashOfficeSumMovements
                        where docCashOfficeSumMovements.DocCashOfficeSumMovementID == id
                        select docCashOfficeSumMovements
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        from docCashOfficeSumMovements in db.DocCashOfficeSumMovements
                        where docCashOfficeSumMovements.DocCashOfficeSumMovementID == id
                        select new
                        {
                            DocID = docCashOfficeSumMovements.DocID,
                            DocDate = docCashOfficeSumMovements.doc.DocDate,
                            Base = docCashOfficeSumMovements.doc.Base,
                            Held = docCashOfficeSumMovements.doc.Held,
                            Del = docCashOfficeSumMovements.doc.Del,
                            Description = docCashOfficeSumMovements.doc.Description,
                            IsImport = docCashOfficeSumMovements.doc.IsImport,

                            DocCashOfficeSumMovementID = docCashOfficeSumMovements.DocCashOfficeSumMovementID,
                            DirContractorIDOrg = docCashOfficeSumMovements.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docCashOfficeSumMovements.doc.dirContractorOrg.DirContractorName,



                            //Точка  + Касса
                            DirWarehouseIDFrom = docCashOfficeSumMovements.DirWarehouseIDFrom,
                            DirWarehouseNameFrom = docCashOfficeSumMovements.dirWarehouseFrom.DirWarehouseName,
                            DirCashOfficeSumFrom = docCashOfficeSumMovements.DirCashOfficeSumFrom,

                            DirCashOfficeIDFrom = docCashOfficeSumMovements.DirCashOfficeIDFrom,
                            DirCashOfficeNameFrom = docCashOfficeSumMovements.dirCashOfficeFrom.DirCashOfficeName,
                            DirCashOfficeSumTo = docCashOfficeSumMovements.DirCashOfficeSumTo,

                            //Точка  + Касса
                            DirWarehouseIDTo = docCashOfficeSumMovements.DirWarehouseIDTo,
                            DirWarehouseNameTo = docCashOfficeSumMovements.dirWarehouseTo.DirWarehouseName,

                            DirCashOfficeIDTo = docCashOfficeSumMovements.DirCashOfficeIDTo,
                            DirCashOfficeNameTo = docCashOfficeSumMovements.dirCashOfficeTo.DirCashOfficeName,



                            NumberInt = docCashOfficeSumMovements.doc.NumberInt,

                            DirEmployeeIDCourier = docCashOfficeSumMovements.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docCashOfficeSumMovements.dirEmployee_Courier.DirEmployeeName,
                            DirMovementStatusID = docCashOfficeSumMovements.DirMovementStatusID, //Курьер штрихнулся и забрал товар

                            DirEmployeeName = docCashOfficeSumMovements.doc.dirEmployee.DirEmployeeName,

                            DocDateHeld = docCashOfficeSumMovements.doc.DocDateHeld,
                            DocDatePayment = docCashOfficeSumMovements.doc.DocDatePayment,


                            Sums = docCashOfficeSumMovements.Sums,
                            SumsCurrency = docCashOfficeSumMovements.SumsCurrency,

                            DirCurrencyID = docCashOfficeSumMovements.DirCurrencyID,
                            DirCurrencyRate = docCashOfficeSumMovements.DirCurrencyRate,
                            DirCurrencyMultiplicity = docCashOfficeSumMovements.DirCurrencyMultiplicity,
                            DirCurrencyName = docCashOfficeSumMovements.dirCurrency.DirCurrencyName + " (" + docCashOfficeSumMovements.DirCurrencyRate + ", " + docCashOfficeSumMovements.DirCurrencyMultiplicity + ")",

                        }
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

        // PUT: api/DocCashOfficeSumMovements/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocCashOfficeSumMovement(int id, DocCashOfficeSumMovement docCashOfficeSumMovement, HttpRequestMessage request)
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
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovements"));
                //if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Возможно доступна Логистика
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovements"));
                if (iRight != 1)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovementsLogistics"));
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

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docCashOfficeSumMovement.DocCashOfficeSumMovementID || docCashOfficeSumMovement.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //2. Если статус > 1, то редактировать может только Администратор
                if (docCashOfficeSumMovement.DirMovementStatusID > 1 && field.DirEmployeeID != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg122)); }

                //3. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docCashOfficeSumMovement.DirEmployeeIDCourier != null && docCashOfficeSumMovement.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docCashOfficeSumMovement.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }

                //4. Получаем "docCashOfficeSumMovement.DocID" из БД, если он отличается от пришедшего от клиента "docCashOfficeSumMovement.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocCashOfficeSumMovements
                        where x.DocCashOfficeSumMovementID == docCashOfficeSumMovement.DocCashOfficeSumMovementID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docCashOfficeSumMovement.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //5. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docCashOfficeSumMovement.DocID);
                //6.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //6.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docCashOfficeSumMovement.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docCashOfficeSumMovement = await Task.Run(() => mPutPostDocCashOfficeSumMovement(db, dbRead, UO_Action, docCashOfficeSumMovement, EntityState.Modified, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docCashOfficeSumMovement.DocID,
                    DocCashOfficeSumMovementID = docCashOfficeSumMovement.DocCashOfficeSumMovementID,
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
        public async Task<IHttpActionResult> PutDocCashOfficeSumMovement(int id, int DirStatusID, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovements"));
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


                Models.Sklad.Doc.DocCashOfficeSumMovement docCashOfficeSumMovement = await db.DocCashOfficeSumMovements.FindAsync(id);
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docCashOfficeSumMovement.DocID);


                #region Проверки

                //1. Если документ проведён то выдать сообщение
                if (Convert.ToBoolean(doc.Held)) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3)); }

                //2. Нельзя перескакивать через статусы (статус всегда +/- 1)
                if (Math.Abs(DirStatusID - Convert.ToInt32(docCashOfficeSumMovement.DirMovementStatusID)) != 1) { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_4)); }

                //3. Если статус == 3, то проверяем введённый пароль и чистим его (что бы не писался в Лог)
                if (DirStatusID == 3)
                {
                    Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(docCashOfficeSumMovement.DirEmployeeIDCourier);
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
                                if (docCashOfficeSumMovement.DirWarehouseIDTo == queryW[0].DirWarehouseID) { bFindWarehouse = true; break; }
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
                            //await mStatusChange(db, ts, docCashOfficeSumMovement, id, DirStatusID, sDiagnosticRresults, field);
                            await mStatusChange(db, dbRead, docCashOfficeSumMovement, id, DirStatusID, sDiagnosticRresults, field);

                            //docCashOfficeSumMovement = await Task.Run(() => mPutPostDocCashOfficeSumMovement(db, dbRead, UO_Action, docCashOfficeSumMovement, EntityState.Modified, docCashOfficeSumMovementTabCollection, field)); //sysSetting

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
                        DocID = docCashOfficeSumMovement.DocID,
                        DocCashOfficeSumMovementID = docCashOfficeSumMovement.DocCashOfficeSumMovementID
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


        // POST: api/DocCashOfficeSumMovements
        [ResponseType(typeof(DocCashOfficeSumMovement))]
        public async Task<IHttpActionResult> PostDocCashOfficeSumMovement(DocCashOfficeSumMovement docCashOfficeSumMovement, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovements"));
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

                #endregion

                #region Проверки

                //1. 
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);


                //2. ПРОВЕРЯЕМ: Если выбран курьер и статус < 4, то не разрешать проводить. Ексепшн: Выбран курьер: передача пакета только через курьера в моделе Логистика!!!
                if (
                    docCashOfficeSumMovement.DirEmployeeIDCourier != null && docCashOfficeSumMovement.DirEmployeeIDCourier > 1 &&
                    UO_Action == "held" &&
                    docCashOfficeSumMovement.DirMovementStatusID != 4
                   )
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg123));
                }


                //3. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docCashOfficeSumMovement.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docCashOfficeSumMovement = await Task.Run(() => mPutPostDocCashOfficeSumMovement(db, dbRead, UO_Action, docCashOfficeSumMovement, EntityState.Added, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docCashOfficeSumMovement.DocID,
                    DocCashOfficeSumMovementID = docCashOfficeSumMovement.DocCashOfficeSumMovementID,
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

        // DELETE: api/DocCashOfficeSumMovements/5
        [ResponseType(typeof(DocCashOfficeSumMovement))]
        public async Task<IHttpActionResult> DeleteDocCashOfficeSumMovement(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSumMovements"));
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
                            from x in dbRead.DocCashOfficeSumMovements
                            where x.DocCashOfficeSumMovementID == id
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
                //3. DocCashOfficeSumMovements
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocCashOfficeSumMovement docCashOfficeSumMovement = await db.DocCashOfficeSumMovements.FindAsync(id);
                if (docCashOfficeSumMovement == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));
                int iDocID = Convert.ToInt32(docCashOfficeSumMovement.DocID);


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {

                        #region 0. Удалить записи в таблице "DocCashOfficeSums"

                        //Удалить надо 2-е записи (изъятие и внесение)
                        var queryDocCashOfficeSums = await
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocCashOfficeSums.Count(); i++)
                        {
                            int DocCashOfficeSumID = Convert.ToInt32(queryDocCashOfficeSums[i].DocCashOfficeSumID);
                            Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = await db.DocCashOfficeSums.FindAsync(DocCashOfficeSumID);

                            db.DocCashOfficeSums.Remove(docCashOfficeSum);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 1. DocCashOfficeSumMovements *** *** *** *** ***

                        db.DocCashOfficeSumMovements.Remove(docCashOfficeSumMovement);
                        await db.SaveChangesAsync();

                        #endregion


                        #region 2. Doc *** *** *** *** ***

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

        private bool DocCashOfficeSumMovementExists(int id)
        {
            return db.DocCashOfficeSumMovements.Count(e => e.DocCashOfficeSumMovementID == id) > 0;
        }


        bool bFindWarehouse = false;
        internal async Task<DocCashOfficeSumMovement> mPutPostDocCashOfficeSumMovement(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            DocCashOfficeSumMovement docCashOfficeSumMovement,
            EntityState entityState, //EntityState.Added, Modified

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
                        if (docCashOfficeSumMovement.DirWarehouseIDTo == queryW[i].DirWarehouseID) { bFindWarehouse = true; break; }
                    }
                    if (!bFindWarehouse) UO_Action = "save";
                }
            }
            else
            {
                bFindWarehouse = true;
            }

            #endregion


            //if (UO_Action == "held") docCashOfficeSumMovement.Reserve = false;
            //else docCashOfficeSumMovement.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docCashOfficeSumMovement.NumberInt;
                doc.NumberReal = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = 1;
                doc.Payment = 0;
                doc.DirContractorID = docCashOfficeSumMovement.DirContractorIDOrg;
                doc.DirContractorIDOrg = docCashOfficeSumMovement.DirContractorIDOrg;
                doc.Discount = docCashOfficeSumMovement.Discount;
                doc.DirVatValue = docCashOfficeSumMovement.DirVatValue;
                doc.Base = docCashOfficeSumMovement.Base;
                doc.Description = docCashOfficeSumMovement.Description;
                doc.DocDate = docCashOfficeSumMovement.DocDate;
                //doc.DocDisc = docCashOfficeSumMovement.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docCashOfficeSumMovement.DocID;
                doc.DocIDBase = docCashOfficeSumMovement.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docCashOfficeSumMovement" со всем полями!
                docCashOfficeSumMovement.DocID = doc.DocID;

                #endregion

                #region 2. DocCashOfficeSumMovement *** *** *** *** *** *** *** *** *** ***

                //Если ещё не переместили в Логистику
                if (docCashOfficeSumMovement.DirMovementStatusID == null || docCashOfficeSumMovement.DirMovementStatusID <= 1)
                {
                    if (docCashOfficeSumMovement.DirEmployeeIDCourier > 0) docCashOfficeSumMovement.DirMovementStatusID = 2;
                    else docCashOfficeSumMovement.DirMovementStatusID = 1;
                }
                //Если редактируем в Логистики
                else
                {

                }

                db.Entry(docCashOfficeSumMovement).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docCashOfficeSumMovement.doc.NumberInt == null || docCashOfficeSumMovement.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docCashOfficeSumMovement.DocCashOfficeSumMovementID.ToString();
                    doc.NumberReal = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion



                if (UO_Action == "held") // || docCashOfficeSumMovement.Reserve
                {

                    #region 1. Проверка, есть ли деньги в кассе по точке From

                    var querySum = await
                        (
                            from x in db.DirCashOffices
                            where x.DirCashOfficeID == docCashOfficeSumMovement.DirCashOfficeIDFrom
                            select x
                        ).SumAsync(x => x.DirCashOfficeSum);
                    if (querySum < docCashOfficeSumMovement.Sums)
                    {
                        throw new System.InvalidOperationException("В списсуемой кассе не достаточно средств!");
                    }

                    #endregion


                    #region 2. Создаём записи в таблице "DocCashOfficeSums"

                    DateTime DocDate = Convert.ToDateTime(docCashOfficeSumMovement.DocDate);

                    //Изымаем средства с Кассы X
                    Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSumFrom = new DocCashOfficeSum();
                    docCashOfficeSumFrom.DirCashOfficeID = docCashOfficeSumMovement.DirCashOfficeIDFrom;
                    docCashOfficeSumFrom.DirEmployeeID = field.DirEmployeeID;
                    docCashOfficeSumFrom.DirCashOfficeSumTypeID = 29;
                    docCashOfficeSumFrom.DocCashOfficeSumDate = DocDate;
                    docCashOfficeSumFrom.DateOnly = Convert.ToDateTime(DocDate.ToString("yyyy-MM-dd"));
                    docCashOfficeSumFrom.DocID = docCashOfficeSumMovement.DocID;
                    docCashOfficeSumFrom.DocXID = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                    docCashOfficeSumFrom.DocCashOfficeSumSum = -docCashOfficeSumMovement.Sums;
                    docCashOfficeSumFrom.DirCurrencyID = docCashOfficeSumMovement.DirCurrencyID;
                    docCashOfficeSumFrom.DirCurrencyRate = docCashOfficeSumMovement.DirCurrencyRate;
                    docCashOfficeSumFrom.DirCurrencyMultiplicity = docCashOfficeSumMovement.DirCurrencyMultiplicity;
                    //docCashOfficeSumFrom.DirEmployeeIDMoney = 0;

                    db.Entry(docCashOfficeSumFrom).State = EntityState.Added;
                    await db.SaveChangesAsync();


                    //Вносим средства в Кассу Y
                    Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSumTo = new DocCashOfficeSum();
                    docCashOfficeSumTo.DirCashOfficeID = docCashOfficeSumMovement.DirCashOfficeIDTo;
                    docCashOfficeSumTo.DirEmployeeID = field.DirEmployeeID;
                    docCashOfficeSumTo.DirCashOfficeSumTypeID = 30;
                    docCashOfficeSumTo.DocCashOfficeSumDate = DocDate;
                    docCashOfficeSumTo.DateOnly = Convert.ToDateTime(DocDate.ToString("yyyy-MM-dd"));
                    docCashOfficeSumTo.DocID = docCashOfficeSumMovement.DocID;
                    docCashOfficeSumTo.DocXID = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                    docCashOfficeSumTo.DocCashOfficeSumSum = docCashOfficeSumMovement.Sums;
                    docCashOfficeSumTo.DirCurrencyID = docCashOfficeSumMovement.DirCurrencyID;
                    docCashOfficeSumTo.DirCurrencyRate = docCashOfficeSumMovement.DirCurrencyRate;
                    docCashOfficeSumTo.DirCurrencyMultiplicity = docCashOfficeSumMovement.DirCurrencyMultiplicity;
                    //docCashOfficeSumTo.DirEmployeeIDMoney = 0;

                    db.Entry(docCashOfficeSumTo).State = EntityState.Added;
                    await db.SaveChangesAsync();

                    #endregion

                }
            }
            else if (UO_Action == "held_cancel")
            {
                //При смены проведении все выборки берём из БД
                docCashOfficeSumMovement = await db.DocCashOfficeSumMovements.FindAsync(docCashOfficeSumMovement.DocCashOfficeSumMovementID);


                #region 1. Проверка, есть ли деньги в кассе по точке To

                var querySum = await
                    (
                        from x in db.DirCashOffices
                        where x.DirCashOfficeID == docCashOfficeSumMovement.DirCashOfficeIDTo
                        select x
                    ).SumAsync(x => x.DirCashOfficeSum);
                if (querySum < docCashOfficeSumMovement.Sums)
                {
                    throw new System.InvalidOperationException("В списсуемой кассе не достаточно средств!");
                }

                #endregion


                #region 1. Удаляем записи в таблице "DocCashOfficeSums"
                
                //Удалить надо 2-е записи (изъятие и внесение)
                var queryDocCashOfficeSums = await
                    (
                        from x in db.DocCashOfficeSums
                        where x.DocID == docCashOfficeSumMovement.DocID
                        select x
                    ).ToListAsync();
                for (int i = 0; i < queryDocCashOfficeSums.Count(); i++)
                {
                    int DocCashOfficeSumID = Convert.ToInt32(queryDocCashOfficeSums[i].DocCashOfficeSumID);
                    Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = await db.DocCashOfficeSums.FindAsync(DocCashOfficeSumID);

                    db.DocCashOfficeSums.Remove(docCashOfficeSum);
                    await db.SaveChangesAsync();
                }

                #endregion


                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docCashOfficeSumMovement.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion
            }


            #region 3. Лог


            #region Пишем в Лог о смене статуса и мастера, если такое было
            /*
            logLogistic.DocLogisticID = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
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


            return docCashOfficeSumMovement;
        }



        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            //System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocCashOfficeSumMovement docCashOfficeSumMovement,
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

            docCashOfficeSumMovement.DirMovementStatusID = StatusID;
            db.Entry(docCashOfficeSumMovement).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 2. Если статус == 4, то проводим

            if (StatusID == 4)
            {

                #region 2.1. Doc

                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docCashOfficeSumMovement.DocID);
                doc.Held = true;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                #endregion


                #region 2. Создаём записи в таблице "DocCashOfficeSums"

                DateTime DocDate = Convert.ToDateTime(docCashOfficeSumMovement.DocDate);

                //Изымаем средства с Кассы X
                Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSumFrom = new DocCashOfficeSum();
                docCashOfficeSumFrom.DirCashOfficeID = docCashOfficeSumMovement.DirCashOfficeIDFrom;
                docCashOfficeSumFrom.DirEmployeeID = field.DirEmployeeID;
                docCashOfficeSumFrom.DirCashOfficeSumTypeID = 29;
                docCashOfficeSumFrom.DocCashOfficeSumDate = DocDate;
                docCashOfficeSumFrom.DateOnly = Convert.ToDateTime(DocDate.ToString("yyyy-MM-dd"));
                docCashOfficeSumFrom.DocID = docCashOfficeSumMovement.DocID;
                docCashOfficeSumFrom.DocXID = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                docCashOfficeSumFrom.DocCashOfficeSumSum = docCashOfficeSumMovement.Sums;
                docCashOfficeSumFrom.DirCurrencyID = docCashOfficeSumMovement.DirCurrencyID;
                docCashOfficeSumFrom.DirCurrencyRate = docCashOfficeSumMovement.DirCurrencyRate;
                docCashOfficeSumFrom.DirCurrencyMultiplicity = docCashOfficeSumMovement.DirCurrencyMultiplicity;
                //docCashOfficeSumFrom.DirEmployeeIDMoney = 0;

                db.Entry(docCashOfficeSumFrom).State = EntityState.Added;
                await db.SaveChangesAsync();


                //Вносим средства в Кассу Y
                Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSumTo = new DocCashOfficeSum();
                docCashOfficeSumTo.DirCashOfficeID = docCashOfficeSumMovement.DirCashOfficeIDTo;
                docCashOfficeSumTo.DirEmployeeID = field.DirEmployeeID;
                docCashOfficeSumTo.DirCashOfficeSumTypeID = 30;
                docCashOfficeSumTo.DocCashOfficeSumDate = DocDate;
                docCashOfficeSumTo.DateOnly = Convert.ToDateTime(DocDate.ToString("yyyy-MM-dd"));
                docCashOfficeSumTo.DocID = docCashOfficeSumMovement.DocID;
                docCashOfficeSumTo.DocXID = docCashOfficeSumMovement.DocCashOfficeSumMovementID;
                docCashOfficeSumTo.DocCashOfficeSumSum = docCashOfficeSumMovement.Sums;
                docCashOfficeSumTo.DirCurrencyID = docCashOfficeSumMovement.DirCurrencyID;
                docCashOfficeSumTo.DirCurrencyRate = docCashOfficeSumMovement.DirCurrencyRate;
                docCashOfficeSumTo.DirCurrencyMultiplicity = docCashOfficeSumMovement.DirCurrencyMultiplicity;
                //docCashOfficeSumTo.DirEmployeeIDMoney = 0;

                db.Entry(docCashOfficeSumTo).State = EntityState.Added;
                await db.SaveChangesAsync();

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
            else logLogistic.Msg = docCashOfficeSumMovement.dirEmployee_Courier.DirEmployeeName;

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
                "[DocCashOfficeSumMovements].[DocCashOfficeSumMovementID] AS [DocCashOfficeSumMovementID], " +
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
                "[DocCashOfficeSumMovements].[Reserve] AS [Reserve] " +

                "FROM [DocCashOfficeSumMovements] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocCashOfficeSumMovements].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "INNER JOIN [DirWarehouses] AS [DirWarehousesFrom] ON [DirWarehousesFrom].[DirWarehouseID] = [DocCashOfficeSumMovements].[DirWarehouseIDFrom] " +
                "INNER JOIN [DirWarehouses] AS [DirWarehousesTo] ON [DirWarehousesTo].[DirWarehouseID] = [DocCashOfficeSumMovements].[DirWarehouseIDTo] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }

        #endregion

    }
}