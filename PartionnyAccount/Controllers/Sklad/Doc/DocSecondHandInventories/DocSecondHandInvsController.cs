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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandInventories
{
    public class DocSecondHandInvsController : ApiController
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

        int ListObjectID = 76;

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
        // GET: api/DocSecondHandInvs
        public async Task<IHttpActionResult> GetDocSecondHandInvs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandInventories"));
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
                        from docSecondHandInvs in db.DocSecondHandInvs

                        join docSecondHandInvTabs1 in db.DocSecondHandInvTabs on docSecondHandInvs.DocSecondHandInvID equals docSecondHandInvTabs1.DocSecondHandInvID into docSecondHandInvTabs2
                        from docSecondHandInvTabs in docSecondHandInvTabs2.DefaultIfEmpty()

                        where docSecondHandInvs.doc.DocDate >= _params.DateS && docSecondHandInvs.doc.DocDate <= _params.DatePo

                        group new { docSecondHandInvTabs }
                        by new
                        {
                            DocID = docSecondHandInvs.DocID,
                            DocDate = docSecondHandInvs.doc.DocDate,
                            Base = docSecondHandInvs.doc.Base,
                            Held = docSecondHandInvs.doc.Held,
                            Discount = docSecondHandInvs.doc.Discount,
                            Del = docSecondHandInvs.doc.Del,
                            Description = docSecondHandInvs.doc.Description,
                            IsImport = docSecondHandInvs.doc.IsImport,
                            DirVatValue = docSecondHandInvs.doc.DirVatValue,

                            DocSecondHandInvID = docSecondHandInvs.DocSecondHandInvID,
                            DirContractorID = docSecondHandInvs.doc.dirContractor.DirContractorID,
                            DirContractorName = docSecondHandInvs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandInvs.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandInvs.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandInvs.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandInvs.dirWarehouse.DirWarehouseName,
                            //Reserve = docSecondHandInvs.Reserve,

                            NumberInt = docSecondHandInvs.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandInvs.doc.Payment,

                            DocDateHeld = docSecondHandInvs.doc.DocDateHeld,
                            DocDatePayment = docSecondHandInvs.doc.DocDatePayment,

                            SpisatS = docSecondHandInvs.SpisatS,
                            SpisatSDirEmployeeID = docSecondHandInvs.SpisatSDirEmployeeID,


                            LoadFrom = docSecondHandInvs.LoadFrom,

                            //Подписи
                            DirEmployee1ID = docSecondHandInvs.DirEmployee1ID,
                            DirEmployee1Podpis = docSecondHandInvs.DirEmployee1Podpis,
                            DirEmployee2ID = docSecondHandInvs.DirEmployee2ID,
                            DirEmployee2Podpis = docSecondHandInvs.DirEmployee2Podpis,
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

                            DocSecondHandInvID = g.Key.DocSecondHandInvID,

                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,

                            //Reserve = g.Key.Reserve,
                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            DocDateHeld = g.Key.DocDateHeld,
                            DocDatePayment = g.Key.DocDatePayment,

                            SpisatS = g.Key.SpisatS,
                            SpisatSDirEmployeeID = g.Key.SpisatSDirEmployeeID,

                            LoadFrom = g.Key.LoadFrom,


                            //Подписи
                            DirEmployee1ID = g.Key.DirEmployee1ID,
                            DirEmployee1Podpis = g.Key.DirEmployee1Podpis,
                            DirEmployee2ID = g.Key.DirEmployee2ID,
                            DirEmployee2Podpis = g.Key.DirEmployee2Podpis,


                            //Сумма

                            //Сумма аппаратов на Списание
                            SumOfVATCurrency1 =

                            Math.Round
                            (
                                g.Sum
                                (
                                    x =>
                                    x.docSecondHandInvTabs.Exist == 2 ? x.docSecondHandInvTabs.PriceCurrency
                                    :
                                    0
                                ),
                                sysSetting.FractionalPartInSum
                            ),

                            //Сумма аппаратов на Разбор
                            SumOfVATCurrency2 =

                            Math.Round
                            (
                                g.Sum
                                (
                                    x =>
                                    x.docSecondHandInvTabs.Exist == 3 ? x.docSecondHandInvTabs.PriceCurrency
                                    :
                                    0
                                ),
                                sysSetting.FractionalPartInSum
                            ),

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

                /*if (_params.FilterType > 0)
                {
                    switch (_params.FilterType)
                    {
                        //case 1: query = query.Where(x => x.Held == true); break;
                        //case 2: query = query.Where(x => x.Held == false); break;
                        //case 3: query = query.Where(x => x.IsImport == true); break;

                        case 1: query = query.Where(x => x.HavePay == 0); break;
                        case 2: query = query.Where(x => x.HavePay > 0 && x.Payment > 0); break;
                        case 3: query = query.Where(x => x.Payment == 0); break;
                        //case 4: query = query.Where(x => x.Reserve == true); break;
                    }
                }*/

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
                        query = query.Where(x => x.DocSecondHandInvID == iNumber32);
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
                    query = query.OrderByDescending(x => x.DocSecondHandInvID);
                }
                else if (sysSetting.DocsSortField == 2)
                {
                    query = query.OrderByDescending(x => x.DocDate);
                }
                else if (sysSetting.DocsSortField == 3)
                {
                    query = query.OrderByDescending(x => x.DocDateHeld);
                }
                else if (sysSetting.DocsSortField == 4)
                {
                    query = query.OrderByDescending(x => x.DocDatePayment);
                }
                else
                {
                    query = query.OrderByDescending(x => x.DocSecondHandInvID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocSecondHandInvs.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSecondHandInv = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandInvs/5
        [ResponseType(typeof(DocSecondHandInv))]
        public async Task<IHttpActionResult> GetDocSecondHandInv(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandInventories"));
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
                        from docSecondHandInvs in db.DocSecondHandInvs
                        where docSecondHandInvs.DocSecondHandInvID == id
                        select docSecondHandInvs
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docSecondHandInvs in db.DocSecondHandInvs

                        join docSecondHandInvTabs1 in db.DocSecondHandInvTabs on docSecondHandInvs.DocSecondHandInvID equals docSecondHandInvTabs1.DocSecondHandInvID into docSecondHandInvTabs2
                        from docSecondHandInvTabs in docSecondHandInvTabs2.DefaultIfEmpty()

                        #endregion

                        where docSecondHandInvs.DocSecondHandInvID == id

                        #region group

                        group new { docSecondHandInvTabs }
                        by new
                        {
                            DocID = docSecondHandInvs.DocID, //DocID = docSecondHandInvs.doc.DocID,
                            DocIDBase = docSecondHandInvs.doc.DocIDBase,
                            DocDate = docSecondHandInvs.doc.DocDate,
                            Base = docSecondHandInvs.doc.Base,
                            Held = docSecondHandInvs.doc.Held,
                            Discount = docSecondHandInvs.doc.Discount,
                            Del = docSecondHandInvs.doc.Del,
                            IsImport = docSecondHandInvs.doc.IsImport,
                            Description = docSecondHandInvs.doc.Description,
                            DirVatValue = docSecondHandInvs.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandInvs.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docSecondHandInvs.doc.dirPaymentType.DirPaymentTypeName,

                            DocSecondHandInvID = docSecondHandInvs.DocSecondHandInvID,
                            DirContractorID = docSecondHandInvs.doc.DirContractorID,
                            DirContractorName = docSecondHandInvs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandInvs.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docSecondHandInvs.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandInvs.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandInvs.dirWarehouse.DirWarehouseName,
                            NumberInt = docSecondHandInvs.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandInvs.doc.Payment,

                            //Резерв
                            //Reserve = docSecondHandInvs.Reserve

                            SpisatS = docSecondHandInvs.SpisatS,
                            SpisatSDirEmployeeID = docSecondHandInvs.SpisatSDirEmployeeID,

                            LoadFrom = docSecondHandInvs.LoadFrom,


                            //Подпись
                            DirEmployee1ID = docSecondHandInvs.DirEmployee1ID,
                            DirEmployee1Podpis = docSecondHandInvs.DirEmployee1Podpis,
                            DirEmployee2ID = docSecondHandInvs.DirEmployee2ID,
                            DirEmployee2Podpis = docSecondHandInvs.DirEmployee2Podpis,

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

                            DocSecondHandInvID = g.Key.DocSecondHandInvID,
                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,

                            SpisatS = g.Key.SpisatS,
                            SpisatSDirEmployeeID = g.Key.SpisatSDirEmployeeID,

                            LoadFrom = g.Key.LoadFrom,
                            LoadXFrom = g.Key.LoadFrom,

                            //Подпись
                            DirEmployee1ID = g.Key.DirEmployee1ID,
                            DirEmployee1Podpis = g.Key.DirEmployee1Podpis,
                            DirEmployee2ID = g.Key.DirEmployee2ID,
                            DirEmployee2Podpis = g.Key.DirEmployee2Podpis,


                            //Сумма аппаратов на Списание
                            SumOfVATCurrency1 =

                            Math.Round
                            (
                                g.Sum
                                (
                                    x =>
                                    x.docSecondHandInvTabs.Exist == 2 ? x.docSecondHandInvTabs.PriceCurrency
                                    :
                                    0
                                ),
                                sysSetting.FractionalPartInSum
                            ),

                            //Сумма аппаратов на Разбор
                            SumOfVATCurrency2 =

                            Math.Round
                            (
                                g.Sum
                                (
                                    x =>
                                    x.docSecondHandInvTabs.Exist == 3 ? x.docSecondHandInvTabs.PriceCurrency
                                    :
                                    0
                                ),
                                sysSetting.FractionalPartInSum
                            ),


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

        // PUT: api/DocSecondHandInvs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandInv(int id, DocSecondHandInv docSecondHandInv, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandInventories"));
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
                Models.Sklad.Doc.DocSecondHandInvTab[] docSecondHandInvTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandInv.recordsDocSecondHandInvTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandInvTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandInvTab[]>(docSecondHandInv.recordsDocSecondHandInvTab);
                }

                for (int i = 0; i < docSecondHandInvTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandInvTab docSecondHandInvTab = docSecondHandInvTabCollection[i];
                    /*
                    if (docSecondHandInvTab.ExistName.ToString().ToLower() == "присутствует") { docSecondHandInvTab.Exist = 1; }
                    else if (docSecondHandInvTab.ExistName.ToString().ToLower() == "списывается с зп") { docSecondHandInvTab.Exist = 2; }
                    else if (docSecondHandInvTab.ExistName.ToString().ToLower() == "отсутствует") { docSecondHandInvTab.Exist = 4; }
                    else docSecondHandInvTab.Exist = 3;
                    */
                    if (docSecondHandInvTab.ExistName.ToString().ToLower() == "присутствует") { docSecondHandInvTab.Exist = 1; }
                    else if (docSecondHandInvTab.ExistName.ToString().ToLower() == "отсутствует") { docSecondHandInvTab.Exist = 2; }
                    else throw new System.InvalidOperationException("Действие для аппарата не верное!");

                    docSecondHandInvTabCollection[i] = docSecondHandInvTab;
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docSecondHandInv.DocSecondHandInvID || docSecondHandInv.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docSecondHandInv.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandInv.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandInvs
                        where x.DocSecondHandInvID == docSecondHandInv.DocSecondHandInvID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandInv.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docSecondHandInv.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandInv.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandInv = await Task.Run(() => mPutPostDocSecondHandInv(db, dbRead, UO_Action, docSecondHandInv, EntityState.Modified, docSecondHandInvTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandInv.DocSecondHandInvID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandInv.DocID,
                    DocSecondHandInvID = docSecondHandInv.DocSecondHandInvID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        //Подпись
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandInv(int id, int ServiceTypeRepair, int iTrash, HttpRequestMessage request)
        {
            try
            {
                int DirEmployee1ID = ServiceTypeRepair;
                int DirEmployee2ID = iTrash;

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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandInventories"));
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
                int? iType = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "iType", true) == 0).Value);
                if (iType <= 0) return Ok(returnServer.Return(false, "Нет параметра iType!"));

                #endregion

                #region Проверки

                Models.Sklad.Doc.DocSecondHandInv docSecondHandInv = await db.DocSecondHandInvs.FindAsync(id);

                //Если док проведён
                if (Convert.ToBoolean(docSecondHandInv.doc.Held)) { throw new System.InvalidOperationException("Документ проведёт! Изменения не возможны!"); }

                //iType
                /*
                if (DirEmployee1ID != 0)
                {
                    if (docSecondHandInv.DirEmployee1ID != DirEmployee1ID)
                    {
                        throw new System.InvalidOperationException("Не совпадает товаровед!");
                    }
                    if (Convert.ToBoolean(docSecondHandInv.DirEmployee2Podpis))
                    {
                        throw new System.InvalidOperationException("Администратор точки уже подписал документ! Попросите его снять подпись!");
                    }
                }
                else if (DirEmployee2ID != 0)
                {
                    if (docSecondHandInv.DirEmployee2ID != DirEmployee2ID)
                    {
                        throw new System.InvalidOperationException("Не совпадает администратор точки!");
                    }
                    if (!Convert.ToBoolean(docSecondHandInv.DirEmployee1Podpis))
                    {
                        throw new System.InvalidOperationException("Товаровед ещё не подписал документ! Попросите его подписать документ!");
                    }
                }
                */

                if (iType == 1)
                {
                    if (DirEmployee1ID == 0) { throw new System.InvalidOperationException("Выберите товароведа!"); }
                    if (docSecondHandInv.DirEmployee1ID != DirEmployee1ID) { throw new System.InvalidOperationException("Не совпадает товаровед!"); }

                    //if (isNaN(DirEmployee2ID)) { Ext.Msg.alert(lanOrgName, "Выберите администратора точки!"); return; }
                    if (docSecondHandInv.DirEmployee2ID == null) { throw new System.InvalidOperationException("Выберите администратора точки!"); }

                    //if (varDirEmployeeID != 1 && DirEmployee1ID != varDirEmployeeID) { Ext.Msg.alert(lanOrgName, "Документ подписывать имеют право или Администратор или Товаровед создавший документ!"); return; }
                    if (field.DirEmployeeID != 1 && docSecondHandInv.DirEmployee1ID != field.DirEmployeeID) { throw new System.InvalidOperationException("Документ подписывать имеют право или Администратор или Товаровед создавший документ!"); }

                    //if (DirEmployee1Podpis) { Ext.Msg.alert(lanOrgName, "Документ уже подписан товароведом!"); return; }
                    if (Convert.ToBoolean(docSecondHandInv.DirEmployee1Podpis)) { throw new System.InvalidOperationException("Документ уже подписан товароведом!"); }

                    //if (DirEmployee2Podpis) { Ext.Msg.alert(lanOrgName, "Документ уже подписан администратором точки! Попросите его снять подпись!"); return; }
                    if (Convert.ToBoolean(docSecondHandInv.DirEmployee2Podpis)) { throw new System.InvalidOperationException("Документ уже подписан администратором точки! Попросите его снять подпись!"); }

                    //if (!DocSecondHandInvID || store.data.length == 0) { Ext.Msg.alert(lanOrgName, "Подпись документа станет доступной только после <span style='color: red'>сохранения документа и заполнения табличной части</span>!<br />И учтите, что после Вашей подписи администратор точки получит сообщение о том что данный документ доступен ему для подписи!<br />Поэтому, перед подписью, заполните табличную часть!"); return; }
                    if (docSecondHandInv.DocSecondHandInvID == null) { throw new System.InvalidOperationException("Подпись документа станет доступной только после <span style='color: red'>сохранения документа и заполнения табличной части</span>!<br />И учтите, что после Вашей подписи администратор точки получит сообщение о том что данный документ доступен ему для подписи!<br />Поэтому, перед подписью, заполните табличную часть!"); }

                }
                else if (iType == 11)
                {
                    if (DirEmployee1ID == 0) { throw new System.InvalidOperationException("Выберите товароведа!"); }
                    if (docSecondHandInv.DirEmployee1ID != DirEmployee1ID) { throw new System.InvalidOperationException("Не совпадает товаровед!"); }

                    //if (!DirEmployee1Podpis) { Ext.Msg.alert(lanOrgName, "Документ ещё не подписан Товароведом!"); return; }
                    if (!Convert.ToBoolean(docSecondHandInv.DirEmployee1Podpis)) { throw new System.InvalidOperationException("Документ ещё не подписан Товароведом!"); }

                    //if (DirEmployee2Podpis) { Ext.Msg.alert(lanOrgName, "Документ уже подписан администратором точки! Попросите его снять подпись!"); return; }
                    if (Convert.ToBoolean(docSecondHandInv.DirEmployee2Podpis)) { throw new System.InvalidOperationException("Документ уже подписан администратором точки! Попросите его снять подпись!"); }

                }
                else if (iType == 2)
                {
                    if (DirEmployee2ID == 0) { throw new System.InvalidOperationException("Выберите администратора точки!"); }
                    if (docSecondHandInv.DirEmployee2ID != DirEmployee2ID) { throw new System.InvalidOperationException("Не совпадает администратор точки!"); }

                    //if (!DirEmployee1Podpis) { Ext.Msg.alert(lanOrgName, "Документ ещё не подписан Товароведом!"); return; }
                    if (!Convert.ToBoolean(docSecondHandInv.DirEmployee1Podpis)) { throw new System.InvalidOperationException("Документ ещё не подписан Товароведом!"); }

                    //if (DirEmployee2Podpis) { Ext.Msg.alert(lanOrgName, "Документ уже подписан администратором точки!"); return; }
                    if (Convert.ToBoolean(docSecondHandInv.DirEmployee2Podpis)) { throw new System.InvalidOperationException("Документ уже подписан администратором точки!"); }

                }
                else if (iType == 21)
                {
                    if (DirEmployee2ID == 0) { throw new System.InvalidOperationException("Выберите администратора точки!"); }
                    if (docSecondHandInv.DirEmployee2ID != DirEmployee2ID) { throw new System.InvalidOperationException("Не совпадает администратор точки!"); }

                    //if (!DirEmployee2Podpis) { Ext.Msg.alert(lanOrgName, "Документ ещё не подписан администратором точки!"); return; }
                    if (!Convert.ToBoolean(docSecondHandInv.DirEmployee2Podpis)) { throw new System.InvalidOperationException("Документ ещё не подписан администратором точки!"); }
                }

                #endregion


                #region Сохранение

                try
                {
                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            if (iType==1 && DirEmployee1ID > 0 && DirEmployee2ID == 0)
                            {
                                docSecondHandInv.DirEmployee1Podpis = true;
                                docSecondHandInv.DirEmployee2Podpis = false;
                            }
                            if (iType == 11 && DirEmployee1ID > 0 && DirEmployee2ID == 0)
                            {
                                docSecondHandInv.DirEmployee1Podpis = false;
                                docSecondHandInv.DirEmployee2Podpis = false;
                            }
                            else if (iType == 2 && DirEmployee1ID == 0 && DirEmployee2ID > 0)
                            {
                                docSecondHandInv.DirEmployee2Podpis = true;
                            }
                            else if (iType == 21 && DirEmployee1ID == 0 && DirEmployee2ID > 0)
                            {
                                docSecondHandInv.DirEmployee2Podpis = false;
                            }

                            db.Entry(docSecondHandInv).State = EntityState.Modified;
                            await Task.Run(() => db.SaveChangesAsync());


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
                        DocID = docSecondHandInv.DocID,
                        DocSecondHandPurchID = docSecondHandInv.DocSecondHandInvID
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

        //Смена админа точки
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandPurch(int id, int DirStatusID, HttpRequestMessage request)
        {
            try
            {
                int DirEmployee2ID = DirStatusID;

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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandInventories"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

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
                //int? iType = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "iType", true) == 0).Value);
                //if (iType <= 0) return Ok(returnServer.Return(false, "Нет параметра iType!"));

                #endregion

                #region Проверки

                Models.Sklad.Doc.DocSecondHandInv docSecondHandInv = await db.DocSecondHandInvs.FindAsync(id);

                //Если док проведён
                if (Convert.ToBoolean(docSecondHandInv.doc.Held)) { throw new System.InvalidOperationException("Документ проведёт! Изменения не возможны!"); }

                //Если меняет сотрудника не товаровед или админ
                if (field.DirEmployeeID != 1 && docSecondHandInv.DirEmployee1ID != field.DirEmployeeID) { throw new System.InvalidOperationException("менять админа точки может только Админ сервиса или товаровед создавший документ! Изменения не возможны!"); }

                //Если меняется сотрудник, но предыдущий сотрудник уже подписал документ
                if (Convert.ToBoolean(docSecondHandInv.DirEmployee2Podpis)) { throw new System.InvalidOperationException("Документ уже подписан админом точки! Изменения не возможны!"); }

                #endregion


                #region Сохранение

                try
                {
                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            docSecondHandInv.DirEmployee2ID = DirEmployee2ID;
                            db.Entry(docSecondHandInv).State = EntityState.Modified;
                            await Task.Run(() => db.SaveChangesAsync());

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
                        DocID = docSecondHandInv.DocID,
                        DocSecondHandPurchID = docSecondHandInv.DocSecondHandInvID
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



        // POST: api/DocSecondHandInvs
        [ResponseType(typeof(DocSecondHandInv))]
        public async Task<IHttpActionResult> PostDocSecondHandInv(DocSecondHandInv docSecondHandInv, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandInventories"));
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
                Models.Sklad.Doc.DocSecondHandInvTab[] docSecondHandInvTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandInv.recordsDocSecondHandInvTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandInvTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandInvTab[]>(docSecondHandInv.recordsDocSecondHandInvTab);
                }

                for (int i = 0; i < docSecondHandInvTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandInvTab docSecondHandInvTab = docSecondHandInvTabCollection[i];
                    /*
                    if (docSecondHandInvTab.ExistName.ToString().ToLower() == "присутствует") { docSecondHandInvTab.Exist = 1; }
                    else if (docSecondHandInvTab.ExistName.ToString().ToLower() == "списывается с зп") { docSecondHandInvTab.Exist = 2; }
                    else if (docSecondHandInvTab.ExistName.ToString().ToLower() == "отсутствует") { docSecondHandInvTab.Exist = 4; }
                    else docSecondHandInvTab.Exist = 3;
                    */
                    if (docSecondHandInvTab.ExistName.ToString().ToLower() == "присутствует") { docSecondHandInvTab.Exist = 1; }
                    else if (docSecondHandInvTab.ExistName.ToString().ToLower() == "отсутствует") { docSecondHandInvTab.Exist = 2; }
                    else throw new System.InvalidOperationException("Действие для аппарата не верное!");

                    docSecondHandInvTabCollection[i] = docSecondHandInvTab;
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docSecondHandInv.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandInv.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandInvs
                        where x.DocSecondHandInvID == docSecondHandInv.DocSecondHandInvID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandInv.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandInv.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandInv = await Task.Run(() => mPutPostDocSecondHandInv(db, dbRead, UO_Action, docSecondHandInv, EntityState.Added, docSecondHandInvTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandInv.DocSecondHandInvID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandInv.DocID,
                    DocSecondHandInvID = docSecondHandInv.DocSecondHandInvID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandInvs/5
        [ResponseType(typeof(DocSecondHandInv))]
        public async Task<IHttpActionResult> DeleteDocSecondHandInv(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandInventories"));
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
                            from x in dbRead.DocSecondHandInvs
                            where x.DocSecondHandInvID == id
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
                //2. DocSecondHandInvTabs
                //3. DocSecondHandInvs
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocSecondHandInv docSecondHandInv = await db.DocSecondHandInvs.FindAsync(id);
                if (docSecondHandInv == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Rem2PartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocSecondHandInvs
                                where x.DocSecondHandInvID == id
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

                        #endregion


                        #region 2. DocSecondHandInvTabs *** *** *** *** ***

                        var queryDocSecondHandInvTabs = await
                            (
                                from x in db.DocSecondHandInvTabs
                                where x.DocSecondHandInvID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandInvTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandInvTab docSecondHandInvTab = await db.DocSecondHandInvTabs.FindAsync(queryDocSecondHandInvTabs[i].DocSecondHandInvTabID);
                            db.DocSecondHandInvTabs.Remove(docSecondHandInvTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocSecondHandInvs *** *** *** *** ***

                        var queryDocSecondHandInvs = await
                            (
                                from x in db.DocSecondHandInvs
                                where x.DocSecondHandInvID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandInvs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandInv docSecondHandInv1 = await db.DocSecondHandInvs.FindAsync(queryDocSecondHandInvs[i].DocSecondHandInvID);
                            db.DocSecondHandInvs.Remove(docSecondHandInv1);
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

        private bool DocSecondHandInvExists(int id)
        {
            return db.DocSecondHandInvs.Count(e => e.DocSecondHandInvID == id) > 0;
        }

        internal async Task<DocSecondHandInv> mPutPostDocSecondHandInv(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandInv docSecondHandInv,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocSecondHandInvTab[] docSecondHandInvTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {

                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docSecondHandInv.NumberInt;
                doc.NumberReal = docSecondHandInv.DocSecondHandInvID;
                doc.DirEmployeeID = field.DirEmployeeID;
                //doc.DirPaymentTypeID = 1; // docSecondHandInv.DirPaymentTypeID;
                doc.Payment = docSecondHandInv.Payment;
                doc.DirContractorID = docSecondHandInv.DirContractorIDOrg;
                doc.DirContractorIDOrg = docSecondHandInv.DirContractorIDOrg;
                doc.Discount = docSecondHandInv.Discount;
                doc.DirVatValue = docSecondHandInv.DirVatValue;
                doc.Base = docSecondHandInv.Base;
                doc.Description = docSecondHandInv.Description;
                doc.DocDate = docSecondHandInv.DocDate;
                //doc.Discount = docSecondHandInv.Discount;
                if (UO_Action == "held") doc.Held = true; 
                else doc.Held = false;
                doc.DocID = docSecondHandInv.DocID;
                doc.DocIDBase = docSecondHandInv.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docSecondHandInv" со всем полями!
                docSecondHandInv.DocID = doc.DocID;

                #endregion

                #region 2. DocSecondHandInv *** *** *** *** *** *** *** *** *** ***
                if (docSecondHandInv.DocSecondHandInvID != null)
                {
                    int? DocSecondHandInvID = docSecondHandInv.DocSecondHandInvID;
                    DocSecondHandInv docSecondHandInv2 = await dbRead.DocSecondHandInvs.FindAsync(DocSecondHandInvID);

                    //Проводится документ только если подписал Админ точки
                    if (UO_Action == "held" && !Convert.ToBoolean(docSecondHandInv2.DirEmployee2Podpis))
                    {
                        throw new System.InvalidOperationException("Проводить документ можно только после подписи администратором точки!");
                    }
                    //Сохранять/Проводить документ может только Админ или Товаровед создавший документ
                    if (field.DirEmployeeID != 1 && field.DirEmployeeID != docSecondHandInv2.DirEmployee1ID)
                    {
                        throw new System.InvalidOperationException("Сохранять/Проводить документ может только Админ или Товаровед создавший документ!");
                    }

                    docSecondHandInv.DirEmployee1Podpis = docSecondHandInv2.DirEmployee1Podpis;
                    docSecondHandInv.DirEmployee2Podpis = docSecondHandInv2.DirEmployee2Podpis;
                }

                docSecondHandInv.DocID = doc.DocID;
                db.Entry(docSecondHandInv).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. Update: NumberInt and NumberReal, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docSecondHandInv.doc.NumberInt == null || docSecondHandInv.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docSecondHandInv.DocSecondHandInvID.ToString();
                    doc.NumberReal = docSecondHandInv.DocSecondHandInvID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docSecondHandInv.DocSecondHandInvID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. DocSecondHandInvTab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocSecondHandInvID = new SQLiteParameter("@DocSecondHandInvID", System.Data.DbType.Int32) { Value = docSecondHandInv.DocSecondHandInvID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocSecondHandInvTabs WHERE DocSecondHandInvID=@DocSecondHandInvID;", parDocSecondHandInvID);
                }

                #endregion


                for (int i = 0; i < docSecondHandInvTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandInvTabCollection[i].DocSecondHandPurchID);


                    #region 3. DocSecondHandInvTab *** *** *** *** *** *** *** *** ***

                    //Ниже сохраняем
                    //2.2. Проставляем ID-шник "DocSecondHandInvID" для всех позиций спецификации
                    docSecondHandInvTabCollection[i].DocSecondHandInvTabID = null;
                    docSecondHandInvTabCollection[i].DocSecondHandInvID = Convert.ToInt32(docSecondHandInv.DocSecondHandInvID);
                    docSecondHandInvTabCollection[i].DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID; //Предыдущий статус, это важно, т.к. аппараты могут приходить с разными статусами!
                    db.Entry(docSecondHandInvTabCollection[i]).State = EntityState.Added;
                    await db.SaveChangesAsync();

                    #endregion


                    if (UO_Action == "held")
                    {
                        #region Проверяем склад и статус

                        if (docSecondHandInv.DirWarehouseID != docSecondHandPurch.DirWarehouseID)
                        {
                            throw new System.InvalidOperationException("Точка документа и точка инвентаризации не совпадают! Документ №" + docSecondHandPurch.DocSecondHandPurchID);
                        }

                        //if (!(docSecondHandPurch.DirSecondHandStatusID >=1 || docSecondHandPurch.DirSecondHandStatusID <= 9))
                        if (docSecondHandPurch.DirSecondHandStatusID >= 10)
                        {
                            throw new System.InvalidOperationException("Статус аппарата не корректный! Документ №<b style='color: red'>" + docSecondHandPurch.DocSecondHandPurchID + "</b> статус <b style='color: red'>" + docSecondHandPurch.dirSecondHandStatus.DirSecondHandStatusName + "</b>");
                        }

                        #endregion

                        #region Проверяем статус каждого аппарата, если он >= 10, то исключение в котором указать №Документа и статус!

                        

                        #endregion

                        int Exist = docSecondHandInvTabCollection[i].Exist;

                        if (Exist == 2)
                        {
                            //Списать с ЗП

                            docSecondHandPurch.DirSecondHandStatusID = 11;
                            db.Entry(docSecondHandPurch).State = EntityState.Modified;
                            await db.SaveChangesAsync();


                            #region Log: Писать в Лог о Списании с ЗП в "LogSecondHands"

                            Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();

                            logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                            logService.DirSecondHandLogTypeID = 16;
                            logService.DirEmployeeID = field.DirEmployeeID;
                            logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                            logService.DirWarehouseIDFrom = docSecondHandInv.DirWarehouseID;
                            //logService.DirWarehouseIDTo = docSecondHandMov.DirWarehouseIDTo;
                            //logService.Msg = "Списан с ЗП №" + docSecondHandPurch.DirWarehouseID;

                            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                            #endregion

                        }
                        if (Exist == 4)
                        {
                            //Списать с ЗП

                            docSecondHandPurch.DirSecondHandStatusID = 15;
                            db.Entry(docSecondHandPurch).State = EntityState.Modified;
                            await db.SaveChangesAsync();


                            #region Log: Писать в Лог о Списании с ЗП в "LogSecondHands"

                            Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();

                            logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                            logService.DirSecondHandLogTypeID = 22;
                            logService.DirEmployeeID = field.DirEmployeeID;
                            logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                            logService.DirWarehouseIDFrom = docSecondHandInv.DirWarehouseID;
                            //logService.DirWarehouseIDTo = docSecondHandMov.DirWarehouseIDTo;
                            //logService.Msg = "Списан с ЗП №" + docSecondHandPurch.DirWarehouseID;

                            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                            #endregion

                        }
                        if (Exist == 3)
                        {
                            //Разбор

                            docSecondHandPurch.DirSecondHandStatusID = 12;
                            db.Entry(docSecondHandPurch).State = EntityState.Modified;
                            await db.SaveChangesAsync();


                            #region Log: Писать в Лог о Списании с ЗП в "LogSecondHands"

                            Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();

                            logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                            logService.DirSecondHandLogTypeID = 17;
                            logService.DirEmployeeID = field.DirEmployeeID;
                            logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                            logService.DirWarehouseIDFrom = docSecondHandInv.DirWarehouseID;
                            //logService.DirWarehouseIDTo = docSecondHandMov.DirWarehouseIDTo;
                            //logService.Msg = "Списан с ЗП №" + docSecondHandPurch.DirWarehouseID;

                            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                            #endregion

                        }

                    }

                }

            }
            else if (UO_Action == "held_cancel")
            {
                //Doc.Held = false
                #region 2. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docSecondHandInv.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion


                var queryDocSecondHandInvTabs = await db.DocSecondHandInvTabs.Where(x=>x.DocSecondHandInvID == docSecondHandInv.DocSecondHandInvID).ToListAsync();

                for (int i = 0; i < queryDocSecondHandInvTabs.Count(); i++)
                {

                    int? DocSecondHandPurchID = queryDocSecondHandInvTabs[i].DocSecondHandPurchID;
                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(DocSecondHandPurchID);


                    #region Проверкаи

                    // - Заполнена табличная часть с запчастями
                    int iCount = await db.DocSecondHandRazbor2Tabs.Where(x=> x.DocSecondHandPurchID == DocSecondHandPurchID).CountAsync();
                    if (iCount > 0)
                    {
                        throw new System.InvalidOperationException( Classes.Language.Sklad.Language.msg130 + queryDocSecondHandInvTabs[i].DocSecondHandPurchID );
                    }
                    // - Статус ТОЛЬКО "На разборе (11)"
                    int? StatusID = docSecondHandPurch.DirSecondHandStatusID;
                    //if (docSecondHandPurch.DirSecondHandStatusID > 12)
                    if (StatusID == 13 || StatusID == 14)
                    {
                        throw new System.InvalidOperationException( Classes.Language.Sklad.Language.msg132 + queryDocSecondHandInvTabs[i].DocSecondHandPurchID );
                    }

                    #endregion


                    #region Меняем статус

                    docSecondHandPurch.DirSecondHandStatusID = queryDocSecondHandInvTabs[i].DirSecondHandStatusID;
                    db.Entry(docSecondHandPurch).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    #endregion


                    #region Log: Писать в Лог о Списании с ЗП в "LogSecondHands"

                    int Exist = queryDocSecondHandInvTabs[i].Exist;

                    if (Exist == 3)
                    {
                        Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();

                        logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                        logService.DirSecondHandLogTypeID = 20;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                        logService.DirWarehouseIDFrom = docSecondHandInv.DirWarehouseID;
                        //logService.DirWarehouseIDTo = docSecondHandMov.DirWarehouseIDTo;
                        //logService.Msg = "Списан с ЗП №" + docSecondHandPurch.DirWarehouseID;

                        await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                    }
                    else if (Exist == 2)
                    {

                        Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();

                        logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                        logService.DirSecondHandLogTypeID = 21;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                        logService.DirWarehouseIDFrom = docSecondHandInv.DirWarehouseID;
                        //logService.DirWarehouseIDTo = docSecondHandMov.DirWarehouseIDTo;
                        //logService.Msg = "Списан с ЗП №" + docSecondHandPurch.DirWarehouseID;

                        await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                    }
                    else if (Exist == 4)
                    {

                        Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();

                        logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                        logService.DirSecondHandLogTypeID = 23;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                        logService.DirWarehouseIDFrom = docSecondHandInv.DirWarehouseID;
                        //logService.DirWarehouseIDTo = docSecondHandMov.DirWarehouseIDTo;
                        //logService.Msg = "Списан с ЗП №" + docSecondHandPurch.DirWarehouseID;

                        await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                    }

                    #endregion

                }

            }


            return docSecondHandInv;
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
                "[DocSecondHandInvs].[DocSecondHandInvID] AS [DocSecondHandInventoryID], " +
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
                "[Docs].[NumberInt] AS [NumberInt], " +


                //Многие поля есть в БД, но нет в проекте.

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

                //"[DirBanksOrg].[DirBankName] AS [DirBankNameOrg], " +
                //"[DirBanksOrg].[DirBankMFO] AS [DirBankMFOOrg], " +


                "[DirWarehouses].[DirWarehouseID] AS [DirWarehouseID], " +
                "[DirWarehouses].[DirWarehouseName] AS [DirWarehouseName], " +
                "[DirWarehouses].[DirWarehouseAddress] AS [DirWarehouseAddress], " +
                "[DirWarehouses].[DirWarehouseDesc] AS [DirWarehouseDesc], " + //есть в БД, но нет в проекте


                //"[DocSecondHandInvs].[SpisatS] AS [SpisatS], " +
                " CASE " +
                "   WHEN [DocSecondHandInvs].[SpisatS] = 1 THEN 'С точки' " +
                "   WHEN [DocSecondHandInvs].[SpisatS] = 2 THEN 'С сотрудника' " +
                " END AS [SpisatS], " +
                "[DirEmployees].[DirEmployeeName] AS [SpisatSDirEmployeeName] " +


                "FROM [DocSecondHandInvs] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandInvs].[DocID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocSecondHandInvs].[DirWarehouseID] " +
                "LEFT OUTER JOIN [DirEmployees] ON [DirEmployees].[DirEmployeeID] = [DocSecondHandInvs].[SpisatSDirEmployeeID] " +
                //Банк для Организации
                //"LEFT JOIN [DirBanks] AS [DirBanksOrg] ON [DirBanks].[DirBankID] = [DirContractorOrg].[DirBankID] " +

                "WHERE (Docs.DocID=@DocID)";


            return SQL;
        }


        //Сумма документа
        internal string GenerateSQLSUM(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";

            SQL =

                "SELECT " +
                " [DocDate] AS [DocDate], " +
                " [DocDate] AS [DocDate_InWords], " +

                " Sum1.Counts AS CountRecord1, " +
                " Sum1.Counts AS CountRecord_NumInWords1, " +
                " Sum1.Sums AS Sums1, " +

                " Sum2.Counts AS CountRecord2, " +
                " Sum2.Counts AS CountRecord_NumInWords2, " +
                " Sum2.Sums AS Sums2, " +

                " Sum3.Counts AS CountRecord3, " +
                " Sum3.Counts AS CountRecord_NumInWords3, " +
                " Sum3.Sums AS Sums3, " +

                " SumX.Counts AS CountRecord, " +
                " SumX.Counts AS CountRecord_NumInWords, " +
                " SumX.Sums AS SumOfVATCurrency " +


               "FROM " +
               " DocSecondHandInvs, Docs, " +
               " (" +
               "   SELECT COUNT(*) Counts, IFNULL(SUM(DocSecondHandInvTabs.PriceCurrency), 0) AS Sums " +
               "   FROM DocSecondHandInvTabs, DocSecondHandInvs, Docs " +
               "   WHERE " +
               "    (Docs.DocID=DocSecondHandInvs.DocID)and(DocSecondHandInvs.DocSecondHandInvID=DocSecondHandInvTabs.DocSecondHandInvID)and(Docs.DocID=@DocID)and(DocSecondHandInvTabs.Exist=1) " +
               " ) AS Sum1, " +
               " (" +
               "   SELECT COUNT(*) Counts, IFNULL(SUM(DocSecondHandInvTabs.PriceCurrency), 0) AS Sums " +
               "   FROM DocSecondHandInvTabs, DocSecondHandInvs, Docs " +
               "   WHERE " +
               "    (Docs.DocID=DocSecondHandInvs.DocID)and(DocSecondHandInvs.DocSecondHandInvID=DocSecondHandInvTabs.DocSecondHandInvID)and(Docs.DocID=@DocID)and(DocSecondHandInvTabs.Exist=2) " +
               " ) AS Sum2, " +
               " (" +
               "   SELECT COUNT(*) Counts, IFNULL(SUM(DocSecondHandInvTabs.PriceCurrency), 0) AS Sums " +
               "   FROM DocSecondHandInvTabs, DocSecondHandInvs, Docs " +
               "   WHERE " +
               "    (Docs.DocID=DocSecondHandInvs.DocID)and(DocSecondHandInvs.DocSecondHandInvID=DocSecondHandInvTabs.DocSecondHandInvID)and(Docs.DocID=@DocID)and(DocSecondHandInvTabs.Exist=3) " +
               " ) AS Sum3, " +

               " (" +
               "   SELECT COUNT(*) Counts, IFNULL(SUM(DocSecondHandInvTabs.PriceCurrency), 0) AS Sums " +
               "   FROM DocSecondHandInvTabs, DocSecondHandInvs, Docs " +
               "   WHERE " +
               "    (Docs.DocID=DocSecondHandInvs.DocID)and(DocSecondHandInvs.DocSecondHandInvID=DocSecondHandInvTabs.DocSecondHandInvID)and(Docs.DocID=@DocID) " +
               " ) AS SumX " +

               "WHERE " +
               " Docs.DocID=DocSecondHandInvs.DocID ";


            return SQL;
        }


        #endregion

    }
}