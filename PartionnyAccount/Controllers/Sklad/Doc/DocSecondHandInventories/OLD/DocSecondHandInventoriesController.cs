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
    public class DocSecondHandInventoriesController : ApiController
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
        // GET: api/DocSecondHandInventories
        public async Task<IHttpActionResult> GetDocSecondHandInventories(HttpRequestMessage request)
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
                        from docSecondHandInventories in db.DocSecondHandInventories

                        join docSecondHandInventoryTabs1 in db.DocSecondHandInventoryTabs on docSecondHandInventories.DocSecondHandInventoryID equals docSecondHandInventoryTabs1.DocSecondHandInventoryID into docSecondHandInventoryTabs2
                        from docSecondHandInventoryTabs in docSecondHandInventoryTabs2.DefaultIfEmpty()

                        where docSecondHandInventories.doc.DocDate >= _params.DateS && docSecondHandInventories.doc.DocDate <= _params.DatePo

                        group new { docSecondHandInventoryTabs }
                        by new
                        {
                            DocID = docSecondHandInventories.DocID,
                            DocDate = docSecondHandInventories.doc.DocDate,
                            Base = docSecondHandInventories.doc.Base,
                            Held = docSecondHandInventories.doc.Held,
                            Discount = docSecondHandInventories.doc.Discount,
                            Del = docSecondHandInventories.doc.Del,
                            Description = docSecondHandInventories.doc.Description,
                            IsImport = docSecondHandInventories.doc.IsImport,
                            DirVatValue = docSecondHandInventories.doc.DirVatValue,

                            DocSecondHandInventoryID = docSecondHandInventories.DocSecondHandInventoryID,
                            DirContractorID = docSecondHandInventories.doc.dirContractor.DirContractorID,
                            DirContractorName = docSecondHandInventories.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandInventories.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandInventories.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandInventories.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandInventories.dirWarehouse.DirWarehouseName,
                            //Reserve = docSecondHandInventories.Reserve,

                            NumberInt = docSecondHandInventories.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandInventories.doc.Payment,

                            DocDateHeld = docSecondHandInventories.doc.DocDateHeld,
                            DocDatePayment = docSecondHandInventories.doc.DocDatePayment,

                            SpisatS = docSecondHandInventories.SpisatS,
                            SpisatSDirEmployeeID = docSecondHandInventories.SpisatSDirEmployeeID,
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

                            DocSecondHandInventoryID = g.Key.DocSecondHandInventoryID,

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


                            //Сумма

                            //Сумма аппаратов на Списание
                            SumOfVATCurrency1 =
                           
                            Math.Round
                            (
                                g.Sum
                                (
                                    x =>
                                    x.docSecondHandInventoryTabs.Exist == 2 ? (1 - g.Key.Discount / 100) * x.docSecondHandInventoryTabs.Quantity * x.docSecondHandInventoryTabs.PriceCurrency
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
                                    x.docSecondHandInventoryTabs.Exist == 3 ? (1 - g.Key.Discount / 100) * x.docSecondHandInventoryTabs.Quantity * x.docSecondHandInventoryTabs.PriceCurrency
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
                        query = query.Where(x => x.DocSecondHandInventoryID == iNumber32);
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
                    query = query.OrderByDescending(x => x.DocSecondHandInventoryID);
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
                    query = query.OrderByDescending(x => x.DocSecondHandInventoryID);
                }


                query = query.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocSecondHandInventories.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSecondHandInventory = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandInventories/5
        [ResponseType(typeof(DocSecondHandInventory))]
        public async Task<IHttpActionResult> GetDocSecondHandInventory(int id, HttpRequestMessage request)
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
                        from docSecondHandInventories in db.DocSecondHandInventories
                        where docSecondHandInventories.DocSecondHandInventoryID == id
                        select docSecondHandInventories
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docSecondHandInventories in db.DocSecondHandInventories

                        join docSecondHandInventoryTabs1 in db.DocSecondHandInventoryTabs on docSecondHandInventories.DocSecondHandInventoryID equals docSecondHandInventoryTabs1.DocSecondHandInventoryID into docSecondHandInventoryTabs2
                        from docSecondHandInventoryTabs in docSecondHandInventoryTabs2.DefaultIfEmpty()

                        #endregion

                        where docSecondHandInventories.DocSecondHandInventoryID == id

                        #region group

                        group new { docSecondHandInventoryTabs }
                        by new
                        {
                            DocID = docSecondHandInventories.DocID, //DocID = docSecondHandInventories.doc.DocID,
                            DocIDBase = docSecondHandInventories.doc.DocIDBase,
                            DocDate = docSecondHandInventories.doc.DocDate,
                            Base = docSecondHandInventories.doc.Base,
                            Held = docSecondHandInventories.doc.Held,
                            Discount = docSecondHandInventories.doc.Discount,
                            Del = docSecondHandInventories.doc.Del,
                            IsImport = docSecondHandInventories.doc.IsImport,
                            Description = docSecondHandInventories.doc.Description,
                            DirVatValue = docSecondHandInventories.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandInventories.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docSecondHandInventories.doc.dirPaymentType.DirPaymentTypeName,

                            DocSecondHandInventoryID = docSecondHandInventories.DocSecondHandInventoryID,
                            DirContractorID = docSecondHandInventories.doc.DirContractorID,
                            DirContractorName = docSecondHandInventories.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandInventories.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docSecondHandInventories.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandInventories.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandInventories.dirWarehouse.DirWarehouseName,
                            NumberInt = docSecondHandInventories.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandInventories.doc.Payment,

                            //Резерв
                            //Reserve = docSecondHandInventories.Reserve

                            SpisatS = docSecondHandInventories.SpisatS,
                            SpisatSDirEmployeeID = docSecondHandInventories.SpisatSDirEmployeeID,
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

                            DocSecondHandInventoryID = g.Key.DocSecondHandInventoryID,
                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,
                            NumberInt = g.Key.NumberInt,

                            SpisatS = g.Key.SpisatS,
                            SpisatSDirEmployeeID = g.Key.SpisatSDirEmployeeID,


                            //Сумма аппаратов на Списание
                            SumOfVATCurrency1 =

                            Math.Round
                            (
                                g.Sum
                                (
                                    x =>
                                    x.docSecondHandInventoryTabs.Exist == 2 ? x.docSecondHandInventoryTabs.Quantity * x.docSecondHandInventoryTabs.PriceCurrency
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
                                    x.docSecondHandInventoryTabs.Exist == 3 ? x.docSecondHandInventoryTabs.Quantity * x.docSecondHandInventoryTabs.PriceCurrency
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

        // PUT: api/DocSecondHandInventories/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandInventory(int id, DocSecondHandInventory docSecondHandInventory, HttpRequestMessage request)
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
                Models.Sklad.Doc.DocSecondHandInventoryTab[] docSecondHandInventoryTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandInventory.recordsDocSecondHandInventoryTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandInventoryTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandInventoryTab[]>(docSecondHandInventory.recordsDocSecondHandInventoryTab);
                }

                for (int i = 0; i < docSecondHandInventoryTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandInventoryTab docSecondHandInventoryTab = docSecondHandInventoryTabCollection[i];
                    if (docSecondHandInventoryTab.ExistName.ToString().ToLower() == "присутствует") { docSecondHandInventoryTab.Exist = 1; }
                    else if (docSecondHandInventoryTab.ExistName.ToString().ToLower() == "списывается с зп") { docSecondHandInventoryTab.Exist = 2; }
                    else docSecondHandInventoryTab.Exist = 3;

                    docSecondHandInventoryTabCollection[i] = docSecondHandInventoryTab;
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docSecondHandInventory.DocSecondHandInventoryID || docSecondHandInventory.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docSecondHandInventory.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandInventory.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandInventories
                        where x.DocSecondHandInventoryID == docSecondHandInventory.DocSecondHandInventoryID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandInventory.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docSecondHandInventory.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandInventory.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandInventory = await Task.Run(() => mPutPostDocSecondHandInventory(db, dbRead, UO_Action, docSecondHandInventory, EntityState.Modified, docSecondHandInventoryTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandInventory.DocSecondHandInventoryID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandInventory.DocID,
                    DocSecondHandInventoryID = docSecondHandInventory.DocSecondHandInventoryID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocSecondHandInventories
        [ResponseType(typeof(DocSecondHandInventory))]
        public async Task<IHttpActionResult> PostDocSecondHandInventory(DocSecondHandInventory docSecondHandInventory, HttpRequestMessage request)
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
                Models.Sklad.Doc.DocSecondHandInventoryTab[] docSecondHandInventoryTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandInventory.recordsDocSecondHandInventoryTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandInventoryTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandInventoryTab[]>(docSecondHandInventory.recordsDocSecondHandInventoryTab);
                }

                for (int i = 0; i < docSecondHandInventoryTabCollection.Count(); i++)
                {
                    Models.Sklad.Doc.DocSecondHandInventoryTab docSecondHandInventoryTab = docSecondHandInventoryTabCollection[i];
                    if (docSecondHandInventoryTab.ExistName.ToString().ToLower() == "присутствует") { docSecondHandInventoryTab.Exist = 1; }
                    else if (docSecondHandInventoryTab.ExistName.ToString().ToLower() == "списывается с зп") { docSecondHandInventoryTab.Exist = 2; }
                    else docSecondHandInventoryTab.Exist = 3;

                    docSecondHandInventoryTabCollection[i] = docSecondHandInventoryTab;
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docSecondHandInventory.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandInventory.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandInventories
                        where x.DocSecondHandInventoryID == docSecondHandInventory.DocSecondHandInventoryID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandInventory.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandInventory.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandInventory = await Task.Run(() => mPutPostDocSecondHandInventory(db, dbRead, UO_Action, docSecondHandInventory, EntityState.Added, docSecondHandInventoryTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandInventory.DocSecondHandInventoryID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandInventory.DocID,
                    DocSecondHandInventoryID = docSecondHandInventory.DocSecondHandInventoryID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandInventories/5
        [ResponseType(typeof(DocSecondHandInventory))]
        public async Task<IHttpActionResult> DeleteDocSecondHandInventory(int id)
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
                            from x in dbRead.DocSecondHandInventories
                            where x.DocSecondHandInventoryID == id
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
                //2. DocSecondHandInventoryTabs
                //3. DocSecondHandInventories
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocSecondHandInventory docSecondHandInventory = await db.DocSecondHandInventories.FindAsync(id);
                if (docSecondHandInventory == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Rem2PartyMinuses *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocSecondHandInventories
                                where x.DocSecondHandInventoryID == id
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


                        #region 2. DocSecondHandInventoryTabs *** *** *** *** ***

                        var queryDocSecondHandInventoryTabs = await
                            (
                                from x in db.DocSecondHandInventoryTabs
                                where x.DocSecondHandInventoryID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandInventoryTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandInventoryTab docSecondHandInventoryTab = await db.DocSecondHandInventoryTabs.FindAsync(queryDocSecondHandInventoryTabs[i].DocSecondHandInventoryTabID);
                            db.DocSecondHandInventoryTabs.Remove(docSecondHandInventoryTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocSecondHandInventories *** *** *** *** ***

                        var queryDocSecondHandInventories = await
                            (
                                from x in db.DocSecondHandInventories
                                where x.DocSecondHandInventoryID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandInventories.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandInventory docSecondHandInventory1 = await db.DocSecondHandInventories.FindAsync(queryDocSecondHandInventories[i].DocSecondHandInventoryID);
                            db.DocSecondHandInventories.Remove(docSecondHandInventory1);
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

        internal async Task<DocSecondHandInventory> mPutPostDocSecondHandInventory(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandInventory docSecondHandInventory,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocSecondHandInventoryTab[] docSecondHandInventoryTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region 1. Doc *** *** *** *** *** *** *** *** *** ***

            //Модель
            Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
            //Присваиваем значения
            doc.ListObjectID = ListObjectID;
            doc.IsImport = false;
            doc.NumberInt = docSecondHandInventory.NumberInt;
            doc.NumberReal = docSecondHandInventory.DocSecondHandInventoryID;
            doc.DirEmployeeID = field.DirEmployeeID;
            //doc.DirPaymentTypeID = 1; // docSecondHandInventory.DirPaymentTypeID;
            doc.Payment = docSecondHandInventory.Payment;
            doc.DirContractorID = docSecondHandInventory.DirContractorIDOrg;
            doc.DirContractorIDOrg = docSecondHandInventory.DirContractorIDOrg;
            doc.Discount = docSecondHandInventory.Discount;
            doc.DirVatValue = docSecondHandInventory.DirVatValue;
            doc.Base = docSecondHandInventory.Base;
            doc.Description = docSecondHandInventory.Description;
            doc.DocDate = docSecondHandInventory.DocDate;
            //doc.Discount = docSecondHandInventory.Discount;
            if (UO_Action == "held") doc.Held = true;
            else doc.Held = false;
            doc.DocID = docSecondHandInventory.DocID;
            doc.DocIDBase = docSecondHandInventory.DocIDBase;

            //Класс
            Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
            //doc = await docs.Save();
            await Task.Run(() => docs.Save());

            //Нужно вернуть "docSecondHandInventory" со всем полями!
            docSecondHandInventory.DocID = doc.DocID;

            #endregion

            #region 2. DocSecondHandInventory *** *** *** *** *** *** *** *** *** ***

            docSecondHandInventory.DocID = doc.DocID;

            db.Entry(docSecondHandInventory).State = entityState;
            await db.SaveChangesAsync();

            #region 2.1. Update: NumberInt and NumberReal, если INSERT *** *** *** *** ***

            if (entityState == EntityState.Added && (docSecondHandInventory.doc.NumberInt == null || docSecondHandInventory.doc.NumberInt.Length == 0))
            {
                doc.NumberInt = docSecondHandInventory.DocSecondHandInventoryID.ToString();
                doc.NumberReal = docSecondHandInventory.DocSecondHandInventoryID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }
            else if (entityState == EntityState.Added)
            {
                doc.NumberReal = docSecondHandInventory.DocSecondHandInventoryID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }

            #endregion


            #endregion

            #region 3. DocSecondHandInventoryTab *** *** *** *** *** *** *** *** ***

            //2.1. Удаляем записи в БД, если UPDATE
            if (entityState == EntityState.Modified)
            {
                SQLiteParameter parDocSecondHandInventoryID = new SQLiteParameter("@DocSecondHandInventoryID", System.Data.DbType.Int32) { Value = docSecondHandInventory.DocSecondHandInventoryID };
                db.Database.ExecuteSqlCommand("DELETE FROM DocSecondHandInventoryTabs WHERE DocSecondHandInventoryID=@DocSecondHandInventoryID;", parDocSecondHandInventoryID);
            }

            //2.2. Проставляем ID-шник "DocSecondHandInventoryID" для всех позиций спецификации
            for (int i = 0; i < docSecondHandInventoryTabCollection.Count(); i++)
            {
                docSecondHandInventoryTabCollection[i].DocSecondHandInventoryTabID = null;
                docSecondHandInventoryTabCollection[i].DocSecondHandInventoryID = Convert.ToInt32(docSecondHandInventory.DocSecondHandInventoryID);
                db.Entry(docSecondHandInventoryTabCollection[i]).State = EntityState.Added;
            }
            await db.SaveChangesAsync();

            #endregion


            Controllers.Sklad.Rem.Rem2PartyMinusesController rem2PartyMinuses = new Rem.Rem2PartyMinusesController();

            #region Удаляем все записи из таблицы "Rem2PartyMinuses"
            //Удаляем все записи из таблицы "Rem2PartyMinuses"
            //Что бы правильно Проверяло на Остаток.
            //А то, товар уже списан, а я проверяю на остаток!

            await Task.Run(() => rem2PartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

            #endregion

            if (UO_Action == "held")
            {
                #region Проверки и Списание с партий (Rem2PartyMinuses)


                //1. Находим по Складу "ХХХ" - "ХХХ БУ.Разбор"
                //2. На него перемещаем аппарат:
                //   Создаём новый документ "Разборка" на складе "ХХХ БУ.Разбор"
                //   Табличная часть содержит все аппараты со статусом "Exist == 3"


                #region 1. Находим по Складу "ХХХ" - "ХХХ БУ.Разбор"

                int?
                    iDirWarehouseID = docSecondHandInventory.DirWarehouseID,
                    iDirWarehouseIDSub = 0;
                var queryWarehouseSum = await
                    (
                        from x in db.DirWarehouses
                        where x.Sub == iDirWarehouseID && x.DirWarehouseLoc == 5
                        select x
                    ).ToListAsync();
                if (queryWarehouseSum.Count() > 0)
                {
                    iDirWarehouseIDSub = queryWarehouseSum[0].DirWarehouseID;
                }

                #endregion


                for (int i = 0; i < docSecondHandInventoryTabCollection.Count(); i++)
                {

                    #region DocIDFirst: Нужен для правильно подсчёта партии + DirServiceContractorID

                    //Получаем "DocID" из списуемой партии "docSecondHandInventoryTabCollection[i].DocID" для "DocIDFirst"
                    Models.Sklad.Rem.Rem2Party _Rem2Party = await db.Rem2Parties.FindAsync(docSecondHandInventoryTabCollection[i].Rem2PartyID);

                    #endregion


                    int Exist = docSecondHandInventoryTabCollection[i].Exist;

                    if (Exist > 1)
                    {

                        #region Проверка

                        //Переменные
                        int iRem2PartyID = Convert.ToInt32(docSecondHandInventoryTabCollection[i].Rem2PartyID);
                        double dQuantity = docSecondHandInventoryTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.Rem2Party rem2Party = await db.Rem2Parties.FindAsync(iRem2PartyID);
                        db.Entry(rem2Party).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region Найдена ЛИ Партий? Т.к. у нас нет вторичного ключа с Партией!
                        if (rem2Party == null)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg111 +

                                "<tr>" +
                                "<td>" + docSecondHandInventoryTabCollection[i].Rem2PartyID + "</td>" +                               //Партия
                                "<td>" + docSecondHandInventoryTabCollection[i].DirServiceNomenID + "</td>" +                         //Код товара
                                "<td>" + docSecondHandInventoryTabCollection[i].Quantity + "</td>" +                                  //списуемое к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg111_1
                            );
                        }
                        #endregion

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (rem2Party.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docSecondHandInventoryTabCollection[i].Rem2PartyID + "</td>" +                                //партия
                                "<td>" + docSecondHandInventoryTabCollection[i].DirServiceNomenID + "</td>" +                                //Код товара
                                "<td>" + docSecondHandInventoryTabCollection[i].Quantity + "</td>" +                                  //списуемое к-во
                                "<td>" + rem2Party.Remnant + "</td>" +                                                  //остаток партии
                                "<td>" + (docSecondHandInventoryTabCollection[i].Quantity - rem2Party.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (rem2Party.DirWarehouseID != docSecondHandInventory.DirWarehouseID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandInventory.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandInventory.DirWarehouseID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docSecondHandInventoryTabCollection[i].Rem2PartyID + "</td>" +           //партия
                                "<td>" + docSecondHandInventoryTabCollection[i].DirServiceNomenID + "</td>" +           //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + rem2Party.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (rem2Party.DirContractorIDOrg != docSecondHandInventory.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandInventory.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docSecondHandInventory.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docSecondHandInventoryTabCollection[i].Rem2PartyID + "</td>" +           //партия
                                "<td>" + docSecondHandInventoryTabCollection[i].DirServiceNomenID + "</td>" +           //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                                "<td>" + rem2Party.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg106_1
                            );
                        }
                        #endregion

                        #endregion


                        #region Списываем и учтём в ЗП

                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                        rem2PartyMinus.Rem2PartyMinusID = null;
                        rem2PartyMinus.Rem2PartyID = Convert.ToInt32(docSecondHandInventoryTabCollection[i].Rem2PartyID);
                        rem2PartyMinus.DirServiceNomenID = docSecondHandInventoryTabCollection[i].DirServiceNomenID;
                        rem2PartyMinus.Quantity = docSecondHandInventoryTabCollection[i].Quantity;
                        rem2PartyMinus.DirCurrencyID = docSecondHandInventoryTabCollection[i].DirCurrencyID;
                        rem2PartyMinus.DirCurrencyMultiplicity = docSecondHandInventoryTabCollection[i].DirCurrencyMultiplicity;
                        rem2PartyMinus.DirCurrencyRate = docSecondHandInventoryTabCollection[i].DirCurrencyRate;
                        rem2PartyMinus.DirVatValue = docSecondHandInventory.DirVatValue;
                        rem2PartyMinus.DirWarehouseID = docSecondHandInventory.DirWarehouseID;
                        rem2PartyMinus.DirContractorIDOrg = docSecondHandInventory.DirContractorIDOrg;
                        rem2PartyMinus.DirServiceContractorID = _Rem2Party.DirServiceContractorID; //rem2PartyMinus.DirServiceContractorID = null;
                        rem2PartyMinus.DocID = Convert.ToInt32(docSecondHandInventory.DocID);
                        rem2PartyMinus.PriceCurrency = docSecondHandInventoryTabCollection[i].PriceCurrency;
                        rem2PartyMinus.PriceVAT = docSecondHandInventoryTabCollection[i].PriceVAT;
                        rem2PartyMinus.FieldID = Convert.ToInt32(docSecondHandInventoryTabCollection[i].DocSecondHandInventoryTabID);
                        //rem2PartyMinus.Reserve = docSecondHandInventory.Reserve;
                        rem2PartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        rem2PartyMinus.DocDate = doc.DocDate;
                        rem2PartyMinus.Reserve = false;

                        db.Entry(rem2PartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion


                        if (Exist == 3)
                        {

                            //Перемещаем на склад: БУ.Разбор

                            //2. На него перемещаем аппарат:
                            //   2.1. Создаём новый документ "Разборка" на складе "ХХХ БУ.Разбор"
                            //   2.2. Табличная часть содержит все аппараты со статусом "Exist == 3"
                            //   2.3. Партии:
                            //        2.3.2 Оприходываем Rem2Parties

                            #region Doc

                            //Модель
                            Models.Sklad.Doc.Doc doc2 = new Models.Sklad.Doc.Doc();
                            //Присваиваем значения
                            doc2.ListObjectID = 77;
                            doc2.IsImport = false;
                            doc2.NumberInt = "0"; //docSecondHandInventory.NumberInt;
                            doc2.NumberReal = 0; //docSecondHandInventory.DocSecondHandInventoryID;
                            doc2.DirEmployeeID = field.DirEmployeeID;
                            //doc2.DirPaymentTypeID = 1; // docSecondHandInventory.DirPaymentTypeID;
                            doc2.Payment = 0;
                            doc2.DirContractorID = docSecondHandInventory.DirContractorIDOrg;
                            doc2.DirContractorIDOrg = docSecondHandInventory.DirContractorIDOrg;
                            doc2.Discount = 0;
                            doc2.DirVatValue = docSecondHandInventory.DirVatValue;
                            doc2.Base = "Создано на основании Инвентаризации №" + docSecondHandInventory.DocSecondHandInventoryID;
                            doc2.Description = "";
                            doc2.DocDate = DateTime.Now;
                            //doc2.Discount = docSecondHandInventory.Discount;
                            /*if (UO_Action == "held") doc2.Held = true;
                            else doc2.Held = false;*/
                            doc2.Held = false;
                            doc2.DocID = null; //docSecondHandInventory.DocID;
                            doc2.DocIDBase = docSecondHandInventory.DocID; //docSecondHandInventory.DocIDBase;

                            //Класс
                            Docs.Docs docs2 = new Docs.Docs(db, dbRead, doc2, EntityState.Added);
                            //doc = await docs2.Save();
                            await Task.Run(() => docs2.Save());

                            //Нужно вернуть "docSecondHandInventory" со всем полями!
                            int? iDocID2 = doc2.DocID;

                            #endregion

                            #region DocSecondHandRazbor

                            Models.Sklad.Doc.DocSecondHandRazbor docSecondHandRazbor = new DocSecondHandRazbor();
                            docSecondHandRazbor.DocSecondHandRazborID = null;
                            docSecondHandRazbor.DocID = iDocID2;
                            docSecondHandRazbor.DocIDFrom = docSecondHandInventory.DocID;

                            //ListObjects.ListObjectID - тип документа: БУ(65, 76) или СЦ(40)
                            docSecondHandRazbor.ListObjectIDFromType = 76;
                            //Docs.DocID - по DocID можно вычислить ID-шник документа 'Docs.NumberReal'
                            docSecondHandRazbor.DocIDFromType = _Rem2Party.DocIDFirst; //Найти из Партии2 DocIDFirst !!!

                            docSecondHandRazbor.DirWarehouseID = Convert.ToInt32(iDirWarehouseIDSub);
                            docSecondHandRazbor.DirServiceNomenID = docSecondHandInventoryTabCollection[i].DirServiceNomenID;
                            docSecondHandRazbor.DirSecondHandStatusID = 2;
                            docSecondHandRazbor.DirSecondHandStatusID_789 = null;

                            docSecondHandRazbor.PriceVAT = docSecondHandInventoryTabCollection[i].PriceVAT;
                            docSecondHandRazbor.PriceCurrency = docSecondHandInventoryTabCollection[i].PriceCurrency;
                            docSecondHandRazbor.DirCurrencyID = docSecondHandInventoryTabCollection[i].DirCurrencyID;
                            docSecondHandRazbor.DirCurrencyRate = docSecondHandInventoryTabCollection[i].DirCurrencyRate;
                            docSecondHandRazbor.DirCurrencyMultiplicity = docSecondHandInventoryTabCollection[i].DirCurrencyMultiplicity;
                            docSecondHandRazbor.DirEmployeeIDMaster = field.DirEmployeeID;
                            docSecondHandRazbor.DateStatusChange = null;

                            docSecondHandRazbor.Rem2PartyID = docSecondHandInventoryTabCollection[i].Rem2PartyID;


                            db.Entry(docSecondHandRazbor).State = EntityState.Added;
                            await db.SaveChangesAsync();


                            #region 2.1. Update: NumberInt and NumberReal, если INSERT *** *** *** *** ***

                            doc2.NumberInt = docSecondHandRazbor.DocSecondHandRazborID.ToString();
                            doc2.NumberReal = docSecondHandRazbor.DocSecondHandRazborID;
                            docs2 = new Docs.Docs(db, dbRead, doc2, EntityState.Modified);
                            await Task.Run(() => docs2.Save());

                            #endregion

                            #endregion


                            #region 2.3.2 Оприходываем Rem2Parties

                            //docSecondHandInventoryTabCollection[i].Rem2PartyID

                            Models.Sklad.Rem.Rem2Party rem2Party2 = new Models.Sklad.Rem.Rem2Party();
                            rem2Party2.Rem2PartyID = null;
                            rem2Party2.DirServiceNomenID = docSecondHandInventoryTabCollection[i].DirServiceNomenID;
                            rem2Party2.Quantity = 1; // docSecondHandInventoryTabCollection[i].Quantity;
                            rem2Party2.Remnant = 1; // docSecondHandInventoryTabCollection[i].Quantity;
                            rem2Party2.DirCurrencyID = docSecondHandInventoryTabCollection[i].DirCurrencyID;
                            //rem2Party2.DirCurrencyMultiplicity = docSecondHandInventoryTabCollection[i].DirCurrencyMultiplicity;
                            //rem2Party2.DirCurrencyRate = docSecondHandInventoryTabCollection[i].DirCurrencyRate;
                            rem2Party2.DirVatValue = docSecondHandInventory.DirVatValue;
                            rem2Party2.DirWarehouseID = Convert.ToInt32(iDirWarehouseIDSub);
                            rem2Party2.DirWarehouseIDDebit = Convert.ToInt32(iDirWarehouseID);
                            rem2Party2.DirWarehouseIDPurch = _Rem2Party.DirWarehouseIDPurch;
                            rem2Party2.DirContractorIDOrg = doc2.DirContractorIDOrg;

                            //!!! Важно !!!
                            rem2Party2.DirServiceContractorID = _Rem2Party.DirServiceContractorID;
                            //!!! Важно !!!

                            //Дата Приёмки товара
                            rem2Party2.DocDatePurches = _Rem2Party.DocDate;

                            rem2Party2.SerialNumber = ""; // docSecondHandInventoryTabCollection[i].SerialNumber;
                            rem2Party2.Barcode = ""; //docSecondHandInventoryTabCollection[i].Barcode;

                            rem2Party2.DocID = Convert.ToInt32(doc2.DocID);

                            //!!! Не правильно !!! 
                            //Надо подсчитать со всех таблиц суммы
                            rem2Party2.PriceVAT = docSecondHandInventoryTabCollection[i].PriceVAT;
                            rem2Party2.PriceCurrency = docSecondHandInventoryTabCollection[i].PriceCurrency;


                            rem2Party2.FieldID = Convert.ToInt32(docSecondHandRazbor.DocSecondHandRazborID);

                            rem2Party2.PriceRetailVAT = docSecondHandInventoryTabCollection[i].PriceVAT;
                            rem2Party2.PriceRetailCurrency = docSecondHandInventoryTabCollection[i].PriceCurrency;
                            rem2Party2.PriceWholesaleVAT = docSecondHandInventoryTabCollection[i].PriceVAT;
                            rem2Party2.PriceWholesaleCurrency = docSecondHandInventoryTabCollection[i].PriceCurrency;
                            rem2Party2.PriceIMVAT = docSecondHandInventoryTabCollection[i].PriceVAT;
                            rem2Party2.PriceIMCurrency = docSecondHandInventoryTabCollection[i].PriceCurrency;

                            //DirNomenMinimumBalance
                            rem2Party2.DirNomenMinimumBalance = 0;

                            rem2Party2.DirEmployeeID = doc.DirEmployeeID;
                            rem2Party2.DocDate = doc.DocDate;

                            //Документ создания первой партии (создания документа)
                            //Нужен для правильно подсчёта партии
                            rem2Party2.DocIDFirst = _Rem2Party.DocIDFirst;

                            Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[1];
                            rem2PartyCollection[0] = rem2Party2;


                            Controllers.Sklad.Rem.Rem2PartiesController rem2Partys = new Rem.Rem2PartiesController();
                            await Task.Run(() => rem2Partys.Save(db, rem2PartyCollection)); //rem2Partys.Save(db, rem2PartyCollection);

                            #endregion

                        }

                    }

                }

                #endregion
            }
            else if (UO_Action == "held_cancel")
            {
                #region  Документ БУ.Инвентаризация

                //Т.к. Мы и приходуем товар, то:
                #region 1. Проверка: Было ли списание с партий *** *** *** *** *** *** *** *** *** ***

                int DocSecondHandInventoryID = Convert.ToInt32(docSecondHandInventory.DocSecondHandInventoryID);

                //Получаем DocPurch из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocSecondHandInventory _docSecondHandInventory = db.DocSecondHandInventories.Find(DocSecondHandInventoryID);
                int? iDocSecondHandInventory_DocID = _docSecondHandInventory.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocSecondHandInventory_DocID));

                #endregion


                #region 2. RemPartyMinuses и RemParties *** *** *** *** *** *** *** *** *** ***

                //Удаление записей в таблицах: RemPartyMinuses
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = iDocSecondHandInventory_DocID }; //docSecondHandInventory.DocID
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID; DELETE FROM RemPartyMinuses WHERE DocID=@DocID;", parDocID); 
                //DELETE FROM RemPartyMinuses WHERE DocID=@DocID;

                //Обновление записей: RemPartyMinuses
                //SQLiteParameter parReserve = new SQLiteParameter("@Reserve", System.Data.DbType.Boolean) { Value = true };
                //await db.Database.ExecuteSqlCommandAsync("UPDATE RemPartyMinuses SET Reserve=@Reserve WHERE DocID=@DocID;", parDocID, parReserve);

                #endregion


                //Doc.Held = false
                #region 3. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc3 = db.Docs.Find(iDocSecondHandInventory_DocID); //docSecondHandInventory.DocID
                doc3.Held = false;

                Docs.Docs docs3 = new Docs.Docs(db, dbRead, doc3, EntityState.Modified);
                await Task.Run(() => docs3.Save()); //docs.Save();

                #endregion

                #endregion


                #region Разбор (на основании БУ.Инв - создаётся Разбор)

                //Получаем все документы (1 аппарат - 1 документ) Разборки
                var queryDocSecondHandRazbor = await
                    (
                        from x in db.DocSecondHandRazbors
                        where x.doc.DocIDBase == iDocSecondHandInventory_DocID //docSecondHandInventory.DocID
                        select x
                    ).ToListAsync();


                #region Проверки

                //!!! Важно: DocSecondHandRazbors !!!
                //Проверки:
                //1. DocSecondHandRazborTabs: если есть хоть одна запись - выдать сообщение об этом
                //2. Лог LogSecondHandRazbors.DocSecondHandRazborID = DocSecondHandRazborID
                //3. Если статус DocSecondHandRazbors.DirSecondHandStatusID != 2

                for (int i = 0; i < queryDocSecondHandRazbor.Count(); i++)
                {
                    int? DocSecondHandRazborID = queryDocSecondHandRazbor[i].DocSecondHandRazborID;

                    //1. DocSecondHandRazborTabs: если есть хоть одна запись - выдать сообщение об этом
                    var queryDocSecondHandRazborTabs = await db.DocSecondHandRazborTabs.Where(x => x.docSecondHandRazbor.DocSecondHandRazborID == DocSecondHandRazborID).CountAsync();
                    if (queryDocSecondHandRazborTabs > 0)
                    {
                        throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg130 + DocSecondHandRazborID + "." + queryDocSecondHandRazbor[i].dirServiceNomen.DirServiceNomenName
                                );
                    }

                    //2. Лог LogSecondHandRazbors.DocSecondHandRazborID = DocSecondHandRazborID
                    var queryLogSecondHandRazbors = await db.LogSecondHandRazbors.Where(x => x.DocSecondHandRazborID == DocSecondHandRazborID).CountAsync();
                    if (queryLogSecondHandRazbors > 0)
                    {
                        throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg131 + DocSecondHandRazborID + "." + queryDocSecondHandRazbor[i].dirServiceNomen.DirServiceNomenName
                                );
                    }

                    //3. Если статус DocSecondHandRazbors.DirSecondHandStatusID != 2
                    if (queryDocSecondHandRazbor[i].DirSecondHandStatusID != 2)
                    {
                        throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg132 + DocSecondHandRazborID + "." + queryDocSecondHandRazbor[i].dirServiceNomen.DirServiceNomenName
                                );
                    }
                }

                #endregion


                #region Сохранение (вернее удаление)

                //Удаление всех аппаратов
                for (int i = 0; i < queryDocSecondHandRazbor.Count(); i++)
                {
                    //1. Rem2Parties
                    int? DocID = queryDocSecondHandRazbor[i].DocID;
                    SQLiteParameter paDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = DocID };
                    await db.Database.ExecuteSqlCommandAsync("DELETE FROM Rem2Parties WHERE DocID=@DocID;", paDocID);

                    //2. DocSecondHandRazbors
                    int? DocSecondHandRazborID = queryDocSecondHandRazbor[i].DocSecondHandRazborID;
                    SQLiteParameter paDocSecondHandRazborID = new SQLiteParameter("@DocSecondHandRazborID", System.Data.DbType.Int32) { Value = DocSecondHandRazborID };
                    await db.Database.ExecuteSqlCommandAsync("DELETE FROM DocSecondHandRazbors WHERE DocSecondHandRazborID=@DocSecondHandRazborID;", paDocSecondHandRazborID);

                    //3. Docs
                    await db.Database.ExecuteSqlCommandAsync("DELETE FROM Docs WHERE DocID=@DocID;", paDocID);
                }

                #endregion


                #endregion
            }



            #region 3. Лог

            //...

            #endregion



            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docSecondHandInventory;
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
                "[DocSecondHandInventories].[DocSecondHandInventoryID] AS [DocSecondHandInventoryID], " +
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


                //"[DocSecondHandInventories].[SpisatS] AS [SpisatS], " +
                " CASE " +
                "   WHEN [DocSecondHandInventories].[SpisatS] = 1 THEN 'С точки' " +
                "   WHEN [DocSecondHandInventories].[SpisatS] = 2 THEN 'С сотрудника' " +
                " END AS [SpisatS], " +
                "[DirEmployees].[DirEmployeeName] AS [SpisatSDirEmployeeName] " + 


                "FROM [DocSecondHandInventories] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandInventories].[DocID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocSecondHandInventories].[DirWarehouseID] " +
                "LEFT OUTER JOIN [DirEmployees] ON [DirEmployees].[DirEmployeeID] = [DocSecondHandInventories].[SpisatSDirEmployeeID] " +
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
               " DocSecondHandInventories, Docs, " +
               " (" +
               "   SELECT COUNT(*) Counts, IFNULL(SUM(DocSecondHandInventoryTabs.Quantity * DocSecondHandInventoryTabs.PriceCurrency), 0) AS Sums " +
               "   FROM DocSecondHandInventoryTabs, DocSecondHandInventories, Docs " +
               "   WHERE " + 
               "    (Docs.DocID=DocSecondHandInventories.DocID)and(DocSecondHandInventories.DocSecondHandInventoryID=DocSecondHandInventoryTabs.DocSecondHandInventoryID)and(Docs.DocID=@DocID)and(DocSecondHandInventoryTabs.Exist=1) " +
               " ) AS Sum1, " +
               " (" +
               "   SELECT COUNT(*) Counts, IFNULL(SUM(DocSecondHandInventoryTabs.Quantity * DocSecondHandInventoryTabs.PriceCurrency), 0) AS Sums " +
               "   FROM DocSecondHandInventoryTabs, DocSecondHandInventories, Docs " +
               "   WHERE " +
               "    (Docs.DocID=DocSecondHandInventories.DocID)and(DocSecondHandInventories.DocSecondHandInventoryID=DocSecondHandInventoryTabs.DocSecondHandInventoryID)and(Docs.DocID=@DocID)and(DocSecondHandInventoryTabs.Exist=2) " +
               " ) AS Sum2, " +
               " (" +
               "   SELECT COUNT(*) Counts, IFNULL(SUM(DocSecondHandInventoryTabs.Quantity * DocSecondHandInventoryTabs.PriceCurrency), 0) AS Sums " +
               "   FROM DocSecondHandInventoryTabs, DocSecondHandInventories, Docs " +
               "   WHERE " +
               "    (Docs.DocID=DocSecondHandInventories.DocID)and(DocSecondHandInventories.DocSecondHandInventoryID=DocSecondHandInventoryTabs.DocSecondHandInventoryID)and(Docs.DocID=@DocID)and(DocSecondHandInventoryTabs.Exist=3) " +
               " ) AS Sum3, " +

               " (" +
               "   SELECT COUNT(*) Counts, IFNULL(SUM(DocSecondHandInventoryTabs.Quantity * DocSecondHandInventoryTabs.PriceCurrency), 0) AS Sums " +
               "   FROM DocSecondHandInventoryTabs, DocSecondHandInventories, Docs " +
               "   WHERE " +
               "    (Docs.DocID=DocSecondHandInventories.DocID)and(DocSecondHandInventories.DocSecondHandInventoryID=DocSecondHandInventoryTabs.DocSecondHandInventoryID)and(Docs.DocID=@DocID) " +
               " ) AS SumX " +

               "WHERE " +
               " Docs.DocID=DocSecondHandInventories.DocID ";


            return SQL;
        }


        #endregion
    }
}