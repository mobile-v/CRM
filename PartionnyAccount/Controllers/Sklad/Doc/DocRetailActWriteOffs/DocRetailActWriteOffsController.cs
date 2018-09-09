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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocRetailActWriteOffs
{
    public class DocRetailActWriteOffsController : ApiController
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

        int ListObjectID = 60;

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
        // GET: api/DocRetailActWriteOffs
        public async Task<IHttpActionResult> GetDocRetailActWriteOffs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailActWriteOffs"));
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
                        from docRetailActWriteOffs in db.DocRetailActWriteOffs

                        join docRetailActWriteOffTabs1 in db.DocRetailActWriteOffTabs on docRetailActWriteOffs.DocRetailActWriteOffID equals docRetailActWriteOffTabs1.DocRetailActWriteOffID into docRetailActWriteOffTabs2
                        from docRetailActWriteOffTabs in docRetailActWriteOffTabs2.DefaultIfEmpty()

                        where docRetailActWriteOffs.doc.DocDate >= _params.DateS && docRetailActWriteOffs.doc.DocDate <= _params.DatePo

                        group new { docRetailActWriteOffTabs }
                        by new
                        {
                            DocID = docRetailActWriteOffs.DocID,
                            DocDate = docRetailActWriteOffs.doc.DocDate,
                            Base = docRetailActWriteOffs.doc.Base,
                            Held = docRetailActWriteOffs.doc.Held,
                            Discount = docRetailActWriteOffs.doc.Discount,
                            Del = docRetailActWriteOffs.doc.Del,
                            Description = docRetailActWriteOffs.doc.Description,
                            IsImport = docRetailActWriteOffs.doc.IsImport,
                            DirVatValue = docRetailActWriteOffs.doc.DirVatValue,

                            DocRetailActWriteOffID = docRetailActWriteOffs.DocRetailActWriteOffID,
                            DirContractorID = docRetailActWriteOffs.doc.dirContractor.DirContractorID,
                            DirContractorName = docRetailActWriteOffs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docRetailActWriteOffs.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docRetailActWriteOffs.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docRetailActWriteOffs.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docRetailActWriteOffs.dirWarehouse.DirWarehouseName,

                            NumberInt = docRetailActWriteOffs.doc.NumberInt,

                            //Оплата
                            Payment = docRetailActWriteOffs.doc.Payment,

                            DocDateHeld = docRetailActWriteOffs.doc.DocDateHeld,
                            DocDatePayment = docRetailActWriteOffs.doc.DocDatePayment,
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

                            DocRetailActWriteOffID = g.Key.DocRetailActWriteOffID,

                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            DocDateHeld = g.Key.DocDateHeld,
                            DocDatePayment = g.Key.DocDatePayment,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum)
                            */
                            g.Sum(x => x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment, sysSetting.FractionalPartInSum)
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
                        query = query.Where(x => x.DocRetailActWriteOffID == iNumber32);
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

                /*
                if (sysSetting.DocsSortField == 1)
                {
                    query = query.OrderByDescending(x => x.DocRetailActWriteOffID);
                }
                else if (sysSetting.DocsSortField == 2)
                {
                    query = query.OrderByDescending(x => x.DocDate);
                }
                else if (sysSetting.DocsSortField == 3)
                {
                    query = query.OrderByDescending(x => x.DocDateHeld);
                }
                //else if (sysSetting.DocsSortField == 4)
                //{
                    //query = query.OrderByDescending(x => x.DocDatePayment);
                //}
                else
                {
                    query = query.OrderByDescending(x => x.DocRetailActWriteOffID);
                }
                */

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocRetailActWriteOffs.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocRetailActWriteOff = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocRetailActWriteOffs/5
        [ResponseType(typeof(DocRetailActWriteOff))]
        public async Task<IHttpActionResult> GetDocRetailActWriteOff(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailActWriteOffs"));
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
                        from docRetailActWriteOffs in db.DocRetailActWriteOffs
                        where docRetailActWriteOffs.DocRetailActWriteOffID == id
                        select docRetailActWriteOffs
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docRetailActWriteOffs in db.DocRetailActWriteOffs

                        join docRetailActWriteOffTabs1 in db.DocRetailActWriteOffTabs on docRetailActWriteOffs.DocRetailActWriteOffID equals docRetailActWriteOffTabs1.DocRetailActWriteOffID into docRetailActWriteOffTabs2
                        from docRetailActWriteOffTabs in docRetailActWriteOffTabs2.DefaultIfEmpty()

                        #endregion

                        where docRetailActWriteOffs.DocRetailActWriteOffID == id

                        #region group

                        group new { docRetailActWriteOffTabs }
                        by new
                        {
                            DocID = docRetailActWriteOffs.DocID, //DocID = docRetailActWriteOffs.doc.DocID,
                            DocIDBase = docRetailActWriteOffs.doc.DocIDBase,
                            DocDate = docRetailActWriteOffs.doc.DocDate,
                            DirPaymentTypeID = docRetailActWriteOffs.doc.DirPaymentTypeID,
                            Base = docRetailActWriteOffs.doc.Base,
                            Held = docRetailActWriteOffs.doc.Held,
                            Discount = docRetailActWriteOffs.doc.Discount,
                            Del = docRetailActWriteOffs.doc.Del,
                            IsImport = docRetailActWriteOffs.doc.IsImport,
                            Description = docRetailActWriteOffs.doc.Description,
                            DirVatValue = docRetailActWriteOffs.doc.DirVatValue,
                            //DirPaymentTypeID = docRetailActWriteOffs.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docRetailActWriteOffs.doc.dirPaymentType.DirPaymentTypeName,

                            DocRetailActWriteOffID = docRetailActWriteOffs.DocRetailActWriteOffID,
                            DirContractorID = docRetailActWriteOffs.doc.DirContractorID,
                            DirContractorName = docRetailActWriteOffs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docRetailActWriteOffs.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docRetailActWriteOffs.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docRetailActWriteOffs.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docRetailActWriteOffs.dirWarehouse.DirWarehouseName,
                            NumberInt = docRetailActWriteOffs.doc.NumberInt,

                            //Оплата
                            Payment = docRetailActWriteOffs.doc.Payment,

                            //Резерв
                            Reserve = docRetailActWriteOffs.Reserve,
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

                            DocRetailActWriteOffID = g.Key.DocRetailActWriteOffID,

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

                            //Сумма с НДС
                            SumDocServicePurch1Tabs =
                            /*
                            g.Sum(x => x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docRetailActWriteOffTabs.Quantity * x.docRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocRetailActWriteOffs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocRetailActWriteOff(int id, DocRetailActWriteOff docRetailActWriteOff, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailActWriteOffs"));
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

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocRetailActWriteOffTab[] docRetailActWriteOffTabCollection = null;
                if (!String.IsNullOrEmpty(docRetailActWriteOff.recordsDocRetailActWriteOffTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docRetailActWriteOffTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocRetailActWriteOffTab[]>(docRetailActWriteOff.recordsDocRetailActWriteOffTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docRetailActWriteOff.DocRetailActWriteOffID || docRetailActWriteOff.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docRetailActWriteOff.DocID" из БД, если он отличается от пришедшего от клиента "docRetailActWriteOff.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocRetailActWriteOffs
                        where x.DocRetailActWriteOffID == docRetailActWriteOff.DocRetailActWriteOffID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docRetailActWriteOff.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docRetailActWriteOff.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docRetailActWriteOff.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docRetailActWriteOff = await Task.Run(() => mPutPostDocRetailActWriteOff(db, dbRead, UO_Action, docRetailActWriteOff, EntityState.Modified, docRetailActWriteOffTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docRetailActWriteOff.DocRetailActWriteOffID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docRetailActWriteOff.DocID,
                    DocRetailActWriteOffID = docRetailActWriteOff.DocRetailActWriteOffID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocRetailActWriteOffs
        [ResponseType(typeof(DocRetailActWriteOff))]
        public async Task<IHttpActionResult> PostDocRetailActWriteOff(DocRetailActWriteOff docRetailActWriteOff, HttpRequestMessage request)
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

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
                if (UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
                UO_Action = UO_Action.ToLower();

                //Получаем колекцию "Спецификации"
                Models.Sklad.Doc.DocRetailActWriteOffTab[] docRetailActWriteOffTabCollection = null;
                if (!String.IsNullOrEmpty(docRetailActWriteOff.recordsDocRetailActWriteOffTab) && UO_Action != "held_cancel")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docRetailActWriteOffTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocRetailActWriteOffTab[]>(docRetailActWriteOff.recordsDocRetailActWriteOffTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docRetailActWriteOff.DocID" из БД, если он отличается от пришедшего от клиента "docRetailActWriteOff.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocRetailActWriteOffActWriteOffs
                        where x.DocRetailActWriteOffID == docRetailActWriteOff.DocRetailActWriteOffID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docRetailActWriteOff.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docRetailActWriteOff.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docRetailActWriteOff = await Task.Run(() => mPutPostDocRetailActWriteOff(db, dbRead, UO_Action, docRetailActWriteOff, EntityState.Added, docRetailActWriteOffTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docRetailActWriteOff.DocRetailActWriteOffID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docRetailActWriteOff.DocID,
                    DocRetailActWriteOffID = docRetailActWriteOff.DocRetailActWriteOffID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocRetailActWriteOffs/5
        [ResponseType(typeof(DocRetailActWriteOff))]
        public async Task<IHttpActionResult> DeleteDocRetailActWriteOff(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailActWriteOffs"));
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
                            from x in dbRead.DocRetailActWriteOffs
                            where x.DocRetailActWriteOffID == id
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
                //2. DocRetailActWriteOffTabs
                //3. DocRetailActWriteOffs
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocRetailActWriteOff docRetailActWriteOff = await db.DocRetailActWriteOffs.FindAsync(id);
                if (docRetailActWriteOff == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Ищим DocID *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocRetailActWriteOffs
                                where x.DocRetailActWriteOffID == id
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


                        #region 2. DocRetailActWriteOffTabs *** *** *** *** ***

                        var queryDocRetailActWriteOffTabs = await
                            (
                                from x in db.DocRetailActWriteOffTabs
                                where x.DocRetailActWriteOffID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocRetailActWriteOffTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocRetailActWriteOffTab docRetailActWriteOffTab = await db.DocRetailActWriteOffTabs.FindAsync(queryDocRetailActWriteOffTabs[i].DocRetailActWriteOffTabID);
                            db.DocRetailActWriteOffTabs.Remove(docRetailActWriteOffTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocRetailActWriteOffs *** *** *** *** ***

                        var queryDocRetailActWriteOffs = await
                            (
                                from x in db.DocRetailActWriteOffs
                                where x.DocRetailActWriteOffID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocRetailActWriteOffs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocRetailActWriteOff docRetailActWriteOff1 = await db.DocRetailActWriteOffs.FindAsync(queryDocRetailActWriteOffs[i].DocRetailActWriteOffID);
                            db.DocRetailActWriteOffs.Remove(docRetailActWriteOff1);
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

        private bool DocRetailActWriteOffExists(int id)
        {
            return db.DocRetailActWriteOffs.Count(e => e.DocRetailActWriteOffID == id) > 0;
        }


        internal async Task<DocRetailActWriteOff> mPutPostDocRetailActWriteOff(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            DocRetailActWriteOff docRetailActWriteOff,
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocRetailActWriteOffTab[] docRetailActWriteOffTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "held") docRetailActWriteOff.Reserve = false;
            else docRetailActWriteOff.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docRetailActWriteOff.NumberInt;
                doc.NumberReal = docRetailActWriteOff.DocRetailActWriteOffID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docRetailActWriteOff.DirPaymentTypeID;
                doc.Payment = docRetailActWriteOff.Payment;
                if (docRetailActWriteOff.DirContractorID != null) doc.DirContractorID = Convert.ToInt32(docRetailActWriteOff.DirContractorID); else doc.DirContractorID = docRetailActWriteOff.DirContractorIDOrg;
                doc.DirContractorIDOrg = docRetailActWriteOff.DirContractorIDOrg;
                doc.Discount = docRetailActWriteOff.Discount;
                doc.DirVatValue = docRetailActWriteOff.DirVatValue;
                doc.Base = docRetailActWriteOff.Base;
                doc.Description = docRetailActWriteOff.Description;
                doc.DocDate = DateTime.Now; //docRetailActWriteOff.DocDate;
                //doc.DocDisc = docRetailActWriteOff.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docRetailActWriteOff.DocID;
                doc.DocIDBase = docRetailActWriteOff.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docRetailActWriteOff" со всем полями!
                docRetailActWriteOff.DocID = doc.DocID;

                #endregion

                #region 2. DocRetailActWriteOff

                docRetailActWriteOff.DocID = doc.DocID;

                db.Entry(docRetailActWriteOff).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT

                if (entityState == EntityState.Added && (docRetailActWriteOff.doc.NumberInt == null || docRetailActWriteOff.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docRetailActWriteOff.DocRetailActWriteOffID.ToString();
                    doc.NumberReal = docRetailActWriteOff.DocRetailActWriteOffID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docRetailActWriteOff.DocRetailActWriteOffID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 4. Description: пишем ID-шник в DocRetailActWriteOffTab и RemParty

                string Description = ""; if (docRetailActWriteOffTabCollection.Length > 0) Description = docRetailActWriteOffTabCollection[0].Description;
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

                #region 3. DocRetailActWriteOffTab

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocRetailActWriteOffID = new SQLiteParameter("@DocRetailActWriteOffID", System.Data.DbType.Int32) { Value = docRetailActWriteOff.DocRetailActWriteOffID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocRetailActWriteOffTabs WHERE DocRetailActWriteOffID=@DocRetailActWriteOffID;", parDocRetailActWriteOffID);
                }

                //2.2. Проставляем ID-шник "DocRetailActWriteOffID" для всех позиций спецификации
                double dSumTab = 0;
                for (int i = 0; i < docRetailActWriteOffTabCollection.Count(); i++)
                {
                    docRetailActWriteOffTabCollection[i].DocRetailActWriteOffTabID = null;
                    docRetailActWriteOffTabCollection[i].DocRetailActWriteOffID = Convert.ToInt32(docRetailActWriteOff.DocRetailActWriteOffID);
                    docRetailActWriteOffTabCollection[i].DirDescriptionID = DirDescriptionID;
                    db.Entry(docRetailActWriteOffTabCollection[i]).State = EntityState.Added;

                    dSumTab += docRetailActWriteOffTabCollection[i].Quantity * docRetailActWriteOffTabCollection[i].PriceCurrency;
                }
                await db.SaveChangesAsync();
                dSumTab = dSumTab - doc.Discount;

                #endregion


                if (UO_Action == "held" || docRetailActWriteOff.Reserve)
                {
                    Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();

                    #region 1. Ищим в Возврате покупателя, если нет, то удаляем в RemPartyMinuses *** *** *** *** ***

                    //1.1. Удаляем "RemPartyMinuses"
                    var queryRemPartyMinuses = await
                        (
                            from x in db.RemPartyMinuses
                            where x.DocID == docRetailActWriteOff.DocID
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

                    #region Удаляем все записи из таблицы "RemPartyMinuses" - !!! НЕ НАДО !!!
                    //Удаляем все записи из таблицы "RemPartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (RemPartyMinuses)

                    for (int i = 0; i < docRetailActWriteOffTabCollection.Count(); i++)
                    {
                        #region Проверка

                        //Переменные
                        int iRemPartyID = Convert.ToInt32(docRetailActWriteOffTabCollection[i].RemPartyID);
                        double dQuantity = docRetailActWriteOffTabCollection[i].Quantity;
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
                                "<td>" + docRetailActWriteOffTabCollection[i].RemPartyID + "</td>" +                                //партия
                                "<td>" + docRetailActWriteOffTabCollection[i].DirNomenID + "</td>" +                                //Код товара
                                "<td>" + docRetailActWriteOffTabCollection[i].Quantity + "</td>" +                                  //списуемое к-во
                                "<td>" + remParty.Remnant + "</td>" +                                                  //остаток партии
                                "<td>" + (docRetailActWriteOffTabCollection[i].Quantity - remParty.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirWarehouseID != docRetailActWriteOff.DirWarehouseID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docRetailActWriteOff.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docRetailActWriteOff.DirWarehouseID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docRetailActWriteOffTabCollection[i].RemPartyID + "</td>" +           //партия
                                "<td>" + docRetailActWriteOffTabCollection[i].DirNomenID + "</td>" +           //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + remParty.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (remParty.DirContractorIDOrg != docRetailActWriteOff.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docRetailActWriteOff.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docRetailActWriteOff.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docRetailActWriteOffTabCollection[i].RemPartyID + "</td>" +           //партия
                                "<td>" + docRetailActWriteOffTabCollection[i].DirNomenID + "</td>" +           //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                                "<td>" + remParty.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg106_1
                            );
                        }
                        #endregion

                        #endregion


                        #region Сохранение - просто меняем причину в партии
                        //1. Списываем к-во 
                        //2. Приходуем к-во




                        #region 1. Списываем к-во

                        Models.Sklad.Rem.RemPartyMinus remPartyMinus = new Models.Sklad.Rem.RemPartyMinus();
                        remPartyMinus.RemPartyMinusID = null;
                        remPartyMinus.RemPartyID = Convert.ToInt32(docRetailActWriteOffTabCollection[i].RemPartyID);

                        remPartyMinus.DirNomenID = docRetailActWriteOffTabCollection[i].DirNomenID;
                        remPartyMinus.Quantity = docRetailActWriteOffTabCollection[i].Quantity;
                        remPartyMinus.DirCurrencyID = docRetailActWriteOffTabCollection[i].DirCurrencyID;
                        remPartyMinus.DirCurrencyMultiplicity = docRetailActWriteOffTabCollection[i].DirCurrencyMultiplicity;
                        remPartyMinus.DirCurrencyRate = docRetailActWriteOffTabCollection[i].DirCurrencyRate;
                        remPartyMinus.DirVatValue = docRetailActWriteOff.DirVatValue;
                        remPartyMinus.DirWarehouseID = docRetailActWriteOff.DirWarehouseID;
                        remPartyMinus.DirContractorIDOrg = docRetailActWriteOff.DirContractorIDOrg;

                        //remPartyMinus.DirContractorID = docRetailActWriteOff.DirContractorID;
                        if (docRetailActWriteOff.DirContractorID != null) remPartyMinus.DirContractorID = Convert.ToInt32(docRetailActWriteOff.DirContractorID);
                        else remPartyMinus.DirContractorID = docRetailActWriteOff.DirContractorIDOrg;

                        remPartyMinus.DocID = Convert.ToInt32(docRetailActWriteOff.DocID);
                        remPartyMinus.PriceCurrency = docRetailActWriteOffTabCollection[i].PriceCurrency;
                        remPartyMinus.PriceVAT = docRetailActWriteOffTabCollection[i].PriceVAT;
                        remPartyMinus.FieldID = Convert.ToInt32(docRetailActWriteOffTabCollection[i].DocRetailActWriteOffTabID);
                        remPartyMinus.Reserve = docRetailActWriteOff.Reserve;

                        remPartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        remPartyMinus.DocDate = doc.DocDate;

                        db.Entry(remPartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion





                        #region 2. Приходуем к-во *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                        Models.Sklad.Rem.RemParty _remParty = new Models.Sklad.Rem.RemParty();

                        _remParty.RemPartyID = null;
                        _remParty.DirNomenID = remPartyMinus.remParty.DirNomenID;
                        _remParty.Quantity = docRetailActWriteOffTabCollection[i].Quantity;
                        _remParty.Remnant = docRetailActWriteOffTabCollection[i].Quantity;
                        _remParty.DirCurrencyID = remPartyMinus.remParty.DirCurrencyID;
                        // _remParty.DirCurrencyMultiplicity = docRetailActWriteOffTabCollection[i].DirCurrencyMultiplicity;
                        // _remParty.DirCurrencyRate = docRetailActWriteOffTabCollection[i].DirCurrencyRate;
                        _remParty.DirVatValue = remPartyMinus.remParty.DirVatValue;
                        _remParty.DirWarehouseID = remPartyMinus.remParty.DirWarehouseID;
                        _remParty.DirWarehouseIDDebit = remPartyMinus.remParty.DirWarehouseIDDebit;
                        _remParty.DirWarehouseIDPurch = remPartyMinus.remParty.DirWarehouseIDPurch;
                        _remParty.DirContractorIDOrg = remPartyMinus.remParty.DirContractorIDOrg;
                        if (docRetailActWriteOff.DirContractorID != null) _remParty.DirContractorID = remPartyMinus.remParty.DirContractorID;
                        else _remParty.DirContractorID = docRetailActWriteOff.DirContractorIDOrg;

                        //Дата Приёмки товара
                        _remParty.DocDatePurches = remPartyMinus.remParty.DocDatePurches;

                        _remParty.DirCharColourID = remPartyMinus.remParty.DirCharColourID; //docRetailActWriteOffTabCollection[i].DirCharColourID;
                        _remParty.DirCharMaterialID = remPartyMinus.remParty.DirCharMaterialID;
                        _remParty.DirCharNameID = remPartyMinus.remParty.DirCharNameID;
                        _remParty.DirCharSeasonID = remPartyMinus.remParty.DirCharSeasonID;
                        _remParty.DirCharSexID = remPartyMinus.remParty.DirCharSexID;
                        _remParty.DirCharSizeID = remPartyMinus.remParty.DirCharSizeID;
                        _remParty.DirCharStyleID = remPartyMinus.remParty.DirCharStyleID;
                        _remParty.DirCharTextureID = remPartyMinus.remParty.DirCharTextureID;

                        _remParty.SerialNumber = remPartyMinus.remParty.SerialNumber;
                        _remParty.Barcode = remPartyMinus.remParty.Barcode;

                        _remParty.DocID = Convert.ToInt32(docRetailActWriteOff.DocID);
                        _remParty.PriceCurrency = remPartyMinus.remParty.PriceCurrency;
                        _remParty.PriceVAT = remPartyMinus.remParty.PriceVAT;
                        _remParty.FieldID = Convert.ToInt32(docRetailActWriteOffTabCollection[i].DocRetailActWriteOffTabID);

                        _remParty.PriceRetailVAT = remPartyMinus.remParty.PriceRetailVAT;
                        _remParty.PriceRetailCurrency = remPartyMinus.remParty.PriceRetailCurrency;
                        _remParty.PriceWholesaleVAT = remPartyMinus.remParty.PriceWholesaleVAT;
                        _remParty.PriceWholesaleCurrency = remPartyMinus.remParty.PriceWholesaleCurrency;
                        _remParty.PriceIMVAT = remPartyMinus.remParty.PriceIMVAT;
                        _remParty.PriceIMCurrency = remPartyMinus.remParty.PriceIMCurrency;

                        _remParty.RemPartyMinusID = remPartyMinus.RemPartyMinusID;

                        _remParty.DirReturnTypeID = docRetailActWriteOffTabCollection[i].DirReturnTypeID;
                        _remParty.DirDescriptionID = DirDescriptionID;

                        _remParty.DirEmployeeID = doc.DirEmployeeID;
                        _remParty.DocDate = doc.DocDate;
                        _remParty.DirNomenMinimumBalance = sysSetting.DirNomenMinimumBalance;

                        //Models.Sklad.Rem.RemParty[] remPartyCollection = new Models.Sklad.Rem.RemParty[docRetailActWriteOffTabCollection.Count()];
                        //remPartyCollection[i] = remParty;

                        //Controllers.Sklad.Rem.RemPartiesController remPartys = new Rem.RemPartiesController();
                        //await Task.Run(() => remPartys.Save(db, remPartyCollection)); //remPartys.Save(db, remPartyCollection);

                        db.Entry(_remParty).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion





                        //remParty - уже нашли (выше).
                        /*
                        remParty.DirDescriptionID = DirDescriptionID; // docRetailActWriteOffTabCollection[i].DirDescriptionID;
                        remParty.DirReturnTypeID = docRetailActWriteOffTabCollection[i].DirReturnTypeID;
                        db.Entry(remParty).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        */

                        #endregion
                    }

                    #endregion

                }
            }
            else if (UO_Action == "held_cancel")
            {

            }


            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docRetailActWriteOff;
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
                "[DocRetailActWriteOffs].[DocRetailID] AS [DocRetailID], " +
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
                "[DocRetailActWriteOffs].[Reserve] AS [Reserve] " +

                "FROM [DocRetailActWriteOffs] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocRetailActWriteOffs].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocRetailActWriteOffs].[DirWarehouseID] " +
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