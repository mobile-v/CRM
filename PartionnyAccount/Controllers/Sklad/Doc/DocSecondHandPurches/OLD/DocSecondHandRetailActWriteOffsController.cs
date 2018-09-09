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
    public class DocSecondHandRetailActWriteOffsController : ApiController
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

        int ListObjectID = 68;

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
        // GET: api/DocSecondHandRetailActWriteOffs
        public async Task<IHttpActionResult> GetDocSecondHandRetailActWriteOffs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailActWriteOffs"));
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
                        from docSecondHandRetailActWriteOffs in db.DocSecondHandRetailActWriteOffs

                        join docSecondHandRetailActWriteOffTabs1 in db.DocSecondHandRetailActWriteOffTabs on docSecondHandRetailActWriteOffs.DocSecondHandRetailActWriteOffID equals docSecondHandRetailActWriteOffTabs1.DocSecondHandRetailActWriteOffID into docSecondHandRetailActWriteOffTabs2
                        from docSecondHandRetailActWriteOffTabs in docSecondHandRetailActWriteOffTabs2.DefaultIfEmpty()

                        where docSecondHandRetailActWriteOffs.doc.DocDate >= _params.DateS && docSecondHandRetailActWriteOffs.doc.DocDate <= _params.DatePo

                        group new { docSecondHandRetailActWriteOffTabs }
                        by new
                        {
                            DocID = docSecondHandRetailActWriteOffs.DocID,
                            DocDate = docSecondHandRetailActWriteOffs.doc.DocDate,
                            Base = docSecondHandRetailActWriteOffs.doc.Base,
                            Held = docSecondHandRetailActWriteOffs.doc.Held,
                            Discount = docSecondHandRetailActWriteOffs.doc.Discount,
                            Del = docSecondHandRetailActWriteOffs.doc.Del,
                            Description = docSecondHandRetailActWriteOffs.doc.Description,
                            IsImport = docSecondHandRetailActWriteOffs.doc.IsImport,
                            DirVatValue = docSecondHandRetailActWriteOffs.doc.DirVatValue,

                            DocSecondHandRetailActWriteOffID = docSecondHandRetailActWriteOffs.DocSecondHandRetailActWriteOffID,
                            DirContractorID = docSecondHandRetailActWriteOffs.doc.dirContractor.DirContractorID,
                            DirContractorName = docSecondHandRetailActWriteOffs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandRetailActWriteOffs.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandRetailActWriteOffs.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandRetailActWriteOffs.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandRetailActWriteOffs.dirWarehouse.DirWarehouseName,

                            NumberInt = docSecondHandRetailActWriteOffs.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandRetailActWriteOffs.doc.Payment,
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

                            DocSecondHandRetailActWriteOffID = g.Key.DocSecondHandRetailActWriteOffID,

                            DirContractorID = g.Key.DirContractorID,
                            DirContractorName = g.Key.DirContractorName,
                            DirContractorIDOrg = g.Key.DirContractorIDOrg,
                            DirContractorNameOrg = g.Key.DirContractorNameOrg,
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,

                            NumberInt = g.Key.NumberInt,
                            DirVatValue = g.Key.DirVatValue,

                            //Сумма с НДС
                            SumOfVATCurrency =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),

                            //Оплата

                            Payment = g.Key.Payment,

                            HavePay =
                            /*
                            g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) - g.Key.Payment, sysSetting.FractionalPartInSum)
                            */
                            g.Sum(x => x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount - g.Key.Payment, sysSetting.FractionalPartInSum)
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
                        query = query.Where(x => x.DocSecondHandRetailActWriteOffID == iNumber32);
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

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocSecondHandRetailActWriteOffs.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSecondHandRetailActWriteOff = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandRetailActWriteOffs/5
        [ResponseType(typeof(DocSecondHandRetailActWriteOff))]
        public async Task<IHttpActionResult> GetDocSecondHandRetailActWriteOff(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailActWriteOffs"));
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
                        from docSecondHandRetailActWriteOffs in db.DocSecondHandRetailActWriteOffs
                        where docSecondHandRetailActWriteOffs.DocSecondHandRetailActWriteOffID == id
                        select docSecondHandRetailActWriteOffs
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                #region from

                        from docSecondHandRetailActWriteOffs in db.DocSecondHandRetailActWriteOffs

                        join docSecondHandRetailActWriteOffTabs1 in db.DocSecondHandRetailActWriteOffTabs on docSecondHandRetailActWriteOffs.DocSecondHandRetailActWriteOffID equals docSecondHandRetailActWriteOffTabs1.DocSecondHandRetailActWriteOffID into docSecondHandRetailActWriteOffTabs2
                        from docSecondHandRetailActWriteOffTabs in docSecondHandRetailActWriteOffTabs2.DefaultIfEmpty()

                            #endregion

                        where docSecondHandRetailActWriteOffs.DocSecondHandRetailActWriteOffID == id

                        #region group

                        group new { docSecondHandRetailActWriteOffTabs }
                        by new
                        {
                            DocID = docSecondHandRetailActWriteOffs.DocID, //DocID = docSecondHandRetailActWriteOffs.doc.DocID,
                            DocIDBase = docSecondHandRetailActWriteOffs.doc.DocIDBase,
                            DocDate = docSecondHandRetailActWriteOffs.doc.DocDate,
                            DirPaymentTypeID = docSecondHandRetailActWriteOffs.doc.DirPaymentTypeID,
                            Base = docSecondHandRetailActWriteOffs.doc.Base,
                            Held = docSecondHandRetailActWriteOffs.doc.Held,
                            Discount = docSecondHandRetailActWriteOffs.doc.Discount,
                            Del = docSecondHandRetailActWriteOffs.doc.Del,
                            IsImport = docSecondHandRetailActWriteOffs.doc.IsImport,
                            Description = docSecondHandRetailActWriteOffs.doc.Description,
                            DirVatValue = docSecondHandRetailActWriteOffs.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandRetailActWriteOffs.doc.DirPaymentTypeID,
                            //DirPaymentTypeName = docSecondHandRetailActWriteOffs.doc.dirPaymentType.DirPaymentTypeName,

                            DocSecondHandRetailActWriteOffID = docSecondHandRetailActWriteOffs.DocSecondHandRetailActWriteOffID,
                            DirContractorID = docSecondHandRetailActWriteOffs.doc.DirContractorID,
                            DirContractorName = docSecondHandRetailActWriteOffs.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandRetailActWriteOffs.doc.DirContractorIDOrg,
                            DirContractorNameOrg = docSecondHandRetailActWriteOffs.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandRetailActWriteOffs.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandRetailActWriteOffs.dirWarehouse.DirWarehouseName,
                            NumberInt = docSecondHandRetailActWriteOffs.doc.NumberInt,

                            //Оплата
                            Payment = docSecondHandRetailActWriteOffs.doc.Payment,

                            //Резерв
                            Reserve = docSecondHandRetailActWriteOffs.Reserve,
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

                            DocSecondHandRetailActWriteOffID = g.Key.DocSecondHandRetailActWriteOffID,

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
                            g.Sum(x => x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) == null ? 0
                            :
                            Math.Round(g.Sum(x => (1 - g.Key.Discount / 100) * x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency), sysSetting.FractionalPartInSum),
                            */
                            g.Sum(x => x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount == null ? 0
                            :
                            Math.Round(g.Sum(x => x.docSecondHandRetailActWriteOffTabs.Quantity * x.docSecondHandRetailActWriteOffTabs.PriceCurrency) - g.Key.Discount, sysSetting.FractionalPartInSum),
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

        // PUT: api/DocSecondHandRetailActWriteOffs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRetailActWriteOff(int id, DocSecondHandRetailActWriteOff docSecondHandRetailActWriteOff, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailActWriteOffs"));
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
                Models.Sklad.Doc.DocSecondHandRetailActWriteOffTab[] docSecondHandRetailActWriteOffTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandRetailActWriteOff.recordsDocSecondHandRetailActWriteOffTab))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandRetailActWriteOffTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandRetailActWriteOffTab[]>(docSecondHandRetailActWriteOff.recordsDocSecondHandRetailActWriteOffTab);
                }

                #endregion

                #region Проверки

                //1.
                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                if (id < 1 || id != docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID || docSecondHandRetailActWriteOff.DocID < 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //2. Получаем "docSecondHandRetailActWriteOff.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandRetailActWriteOff.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandRetailActWriteOffs
                        where x.DocSecondHandRetailActWriteOffID == docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandRetailActWriteOff.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();


                //3. Проверяем, если документ 
                Models.Sklad.Doc.Doc docTemp = await dbRead.Docs.FindAsync(docSecondHandRetailActWriteOff.DocID);
                //3.1. проведён и мы пытаемся его сохранить или провести - выдать Эксепшн
                if (Convert.ToBoolean(docTemp.Held) && (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_1));
                //3.2. НЕ проведён и мы пытаемся Снять Проводку - выдать Эксепшн
                if (!Convert.ToBoolean(docTemp.Held) && UO_Action == "held_cancel") return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg12_2));


                //4. Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandRetailActWriteOff.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRetailActWriteOff = await Task.Run(() => mPutPostDocSecondHandRetailActWriteOff(db, dbRead, UO_Action, docSecondHandRetailActWriteOff, EntityState.Modified, docSecondHandRetailActWriteOffTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandRetailActWriteOff.DocID,
                    DocSecondHandRetailActWriteOffID = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocSecondHandRetailActWriteOffs
        [ResponseType(typeof(DocSecondHandRetailActWriteOff))]
        public async Task<IHttpActionResult> PostDocSecondHandRetailActWriteOff(DocSecondHandRetailActWriteOff docSecondHandRetailActWriteOff, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetails"));
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
                Models.Sklad.Doc.DocSecondHandRetailActWriteOffTab[] docSecondHandRetailActWriteOffTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandRetailActWriteOff.recordsDocSecondHandRetailActWriteOffTab) && UO_Action != "held_cancel")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandRetailActWriteOffTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandRetailActWriteOffTab[]>(docSecondHandRetailActWriteOff.recordsDocSecondHandRetailActWriteOffTab);
                }

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docSecondHandRetailActWriteOff.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandRetailActWriteOff.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandRetailActWriteOffActWriteOffs
                        where x.DocSecondHandRetailActWriteOffID == docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandRetailActWriteOff.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandRetailActWriteOff.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRetailActWriteOff = await Task.Run(() => mPutPostDocSecondHandRetailActWriteOff(db, dbRead, UO_Action, docSecondHandRetailActWriteOff, EntityState.Added, docSecondHandRetailActWriteOffTabCollection, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandRetailActWriteOff.DocID,
                    DocSecondHandRetailActWriteOffID = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandRetailActWriteOffs/5
        [ResponseType(typeof(DocSecondHandRetailActWriteOff))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRetailActWriteOff(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailActWriteOffs"));
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
                            from x in dbRead.DocSecondHandRetailActWriteOffs
                            where x.DocSecondHandRetailActWriteOffID == id
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
                //2. DocSecondHandRetailActWriteOffTabs
                //3. DocSecondHandRetailActWriteOffs
                //4. Docs


                //Сотрудник
                Models.Sklad.Doc.DocSecondHandRetailActWriteOff docSecondHandRetailActWriteOff = await db.DocSecondHandRetailActWriteOffs.FindAsync(id);
                if (docSecondHandRetailActWriteOff == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region 1. Ищим DocID *** *** *** *** ***

                        //1.1. Ищим DocID
                        int iDocID = 0;
                        var queryDocs1 = await
                            (
                                from x in db.DocSecondHandRetailActWriteOffs
                                where x.DocSecondHandRetailActWriteOffID == id
                                select x
                            ).ToListAsync();
                        if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                        else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                        #endregion


                        #region 1. Ищим в Возврате покупателя, если нет, то удаляем в Rem2PartyMinuses *** *** *** *** ***

                        //1.1. Удаляем "Rem2PartyMinuses"
                        var queryRem2PartyMinuses = await
                            (
                                from x in db.Rem2PartyMinuses
                                where x.DocID == iDocID
                                select x
                            ).ToListAsync();

                        for (int i = 0; i < queryRem2PartyMinuses.Count(); i++)
                        {
                            int iRem2PartyMinusID = Convert.ToInt32(queryRem2PartyMinuses[i].Rem2PartyMinusID);

                            /*
                            var queryDocReturnsCustomerTab = await
                                (
                                    from x in db.DocReturnsCustomerTabs
                                    where x.Rem2PartyMinusID == iRem2PartyMinusID
                                    select x
                                ).ToListAsync();

                            if (queryDocReturnsCustomerTab.Count() > 0)
                            {
                                throw new System.InvalidOperationException(
                                    Classes.Language.Sklad.Language.msg112 +

                                    "<tr>" +
                                    "<td>" + queryDocReturnsCustomerTab[0].Rem2PartyMinusID + "</td>" +                           //партия списания
                                    "<td>" + queryDocReturnsCustomerTab[0].DocReturnsCustomerID + "</td>" +                      //№ д-та
                                    "<td>" + queryDocReturnsCustomerTab[0].DirServiceNomenID + "</td>" +                                //Код товара
                                    "<td>" + queryDocReturnsCustomerTab[0].Quantity + "</td>" +                                  //списуемое к-во
                                    "</tr>" +
                                    "</table>" +

                                    Classes.Language.Sklad.Language.msg112_1
                                    );
                            }
                            */

                            Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = await db.Rem2PartyMinuses.FindAsync(iRem2PartyMinusID);
                            db.Rem2PartyMinuses.Remove(rem2PartyMinus);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 2. DocSecondHandRetailActWriteOffTabs *** *** *** *** ***

                        var queryDocSecondHandRetailActWriteOffTabs = await
                            (
                                from x in db.DocSecondHandRetailActWriteOffTabs
                                where x.DocSecondHandRetailActWriteOffID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandRetailActWriteOffTabs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandRetailActWriteOffTab docSecondHandRetailActWriteOffTab = await db.DocSecondHandRetailActWriteOffTabs.FindAsync(queryDocSecondHandRetailActWriteOffTabs[i].DocSecondHandRetailActWriteOffTabID);
                            db.DocSecondHandRetailActWriteOffTabs.Remove(docSecondHandRetailActWriteOffTab);
                            await db.SaveChangesAsync();
                        }

                        #endregion


                        #region 3. DocSecondHandRetailActWriteOffs *** *** *** *** ***

                        var queryDocSecondHandRetailActWriteOffs = await
                            (
                                from x in db.DocSecondHandRetailActWriteOffs
                                where x.DocSecondHandRetailActWriteOffID == id
                                select x
                            ).ToListAsync();
                        for (int i = 0; i < queryDocSecondHandRetailActWriteOffs.Count(); i++)
                        {
                            Models.Sklad.Doc.DocSecondHandRetailActWriteOff docSecondHandRetailActWriteOff1 = await db.DocSecondHandRetailActWriteOffs.FindAsync(queryDocSecondHandRetailActWriteOffs[i].DocSecondHandRetailActWriteOffID);
                            db.DocSecondHandRetailActWriteOffs.Remove(docSecondHandRetailActWriteOff1);
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

        private bool DocSecondHandRetailActWriteOffExists(int id)
        {
            return db.DocSecondHandRetailActWriteOffs.Count(e => e.DocSecondHandRetailActWriteOffID == id) > 0;
        }


        internal async Task<DocSecondHandRetailActWriteOff> mPutPostDocSecondHandRetailActWriteOff(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            DocSecondHandRetailActWriteOff docSecondHandRetailActWriteOff,
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Doc.DocSecondHandRetailActWriteOffTab[] docSecondHandRetailActWriteOffTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            if (UO_Action == "held") docSecondHandRetailActWriteOff.Reserve = false;
            else docSecondHandRetailActWriteOff.Reserve = true;

            if (UO_Action == "save" || UO_Action == "save_close" || UO_Action == "held")
            {
                #region 1. Doc

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docSecondHandRetailActWriteOff.NumberInt;
                doc.NumberReal = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docSecondHandRetailActWriteOff.DirPaymentTypeID;
                doc.Payment = docSecondHandRetailActWriteOff.Payment;
                if (docSecondHandRetailActWriteOff.DirContractorID != null) doc.DirContractorID = Convert.ToInt32(docSecondHandRetailActWriteOff.DirContractorID); else doc.DirContractorID = docSecondHandRetailActWriteOff.DirContractorIDOrg;
                doc.DirContractorIDOrg = docSecondHandRetailActWriteOff.DirContractorIDOrg;
                doc.Discount = docSecondHandRetailActWriteOff.Discount;
                doc.DirVatValue = docSecondHandRetailActWriteOff.DirVatValue;
                doc.Base = docSecondHandRetailActWriteOff.Base;
                doc.Description = docSecondHandRetailActWriteOff.Description;
                doc.DocDate = DateTime.Now; //docSecondHandRetailActWriteOff.DocDate;
                //doc.DocDisc = docSecondHandRetailActWriteOff.DocDisc;
                if (UO_Action == "held") doc.Held = true;
                else doc.Held = false;
                doc.DocID = docSecondHandRetailActWriteOff.DocID;
                doc.DocIDBase = docSecondHandRetailActWriteOff.DocIDBase;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docSecondHandRetailActWriteOff" со всем полями!
                docSecondHandRetailActWriteOff.DocID = doc.DocID;

                #endregion

                #region 2. DocSecondHandRetailActWriteOff

                docSecondHandRetailActWriteOff.DocID = doc.DocID;

                db.Entry(docSecondHandRetailActWriteOff).State = entityState;
                await db.SaveChangesAsync();

                #region 2.1. UpdateNumberInt, если INSERT

                if (entityState == EntityState.Added && (docSecondHandRetailActWriteOff.doc.NumberInt == null || docSecondHandRetailActWriteOff.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID.ToString();
                    doc.NumberReal = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 4. Description: пишем ID-шник в DocSecondHandRetailActWriteOffTab и Rem2Party

                string Description = ""; if (docSecondHandRetailActWriteOffTabCollection.Length > 0) Description = docSecondHandRetailActWriteOffTabCollection[0].Description;
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

                #region 3. DocSecondHandRetailActWriteOffTab

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocSecondHandRetailActWriteOffID = new SQLiteParameter("@DocSecondHandRetailActWriteOffID", System.Data.DbType.Int32) { Value = docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocSecondHandRetailActWriteOffTabs WHERE DocSecondHandRetailActWriteOffID=@DocSecondHandRetailActWriteOffID;", parDocSecondHandRetailActWriteOffID);
                }

                //2.2. Проставляем ID-шник "DocSecondHandRetailActWriteOffID" для всех позиций спецификации
                double dSumTab = 0;
                for (int i = 0; i < docSecondHandRetailActWriteOffTabCollection.Count(); i++)
                {
                    docSecondHandRetailActWriteOffTabCollection[i].DocSecondHandRetailActWriteOffTabID = null;
                    docSecondHandRetailActWriteOffTabCollection[i].DocSecondHandRetailActWriteOffID = Convert.ToInt32(docSecondHandRetailActWriteOff.DocSecondHandRetailActWriteOffID);
                    docSecondHandRetailActWriteOffTabCollection[i].DirDescriptionID = DirDescriptionID;
                    db.Entry(docSecondHandRetailActWriteOffTabCollection[i]).State = EntityState.Added;

                    dSumTab += docSecondHandRetailActWriteOffTabCollection[i].Quantity * docSecondHandRetailActWriteOffTabCollection[i].PriceCurrency;
                }
                await db.SaveChangesAsync();
                dSumTab = dSumTab - doc.Discount;

                #endregion


                if (UO_Action == "held" || docSecondHandRetailActWriteOff.Reserve)
                {
                    Controllers.Sklad.Rem.Rem2PartyMinusesController rem2PartyMinuses = new Rem.Rem2PartyMinusesController();

                    #region 1. Ищим в Возврате покупателя, если нет, то удаляем в Rem2PartyMinuses *** *** *** *** ***

                    //1.1. Удаляем "Rem2PartyMinuses"
                    var queryRem2PartyMinuses = await
                        (
                            from x in db.Rem2PartyMinuses
                            where x.DocID == docSecondHandRetailActWriteOff.DocID
                            select x
                        ).ToListAsync();

                    for (int i = 0; i < queryRem2PartyMinuses.Count(); i++)
                    {
                        int iRem2PartyMinusID = Convert.ToInt32(queryRem2PartyMinuses[i].Rem2PartyMinusID);

                        var queryDocSecondHandRetailReturnTab = await
                            (
                                from x in db.DocSecondHandRetailReturnTabs
                                where x.Rem2PartyMinusID == iRem2PartyMinusID
                                select x
                            ).ToListAsync();

                        if (queryDocSecondHandRetailReturnTab.Count() > 0)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg117 +

                                "<tr>" +
                                "<td>" + queryDocSecondHandRetailReturnTab[0].Rem2PartyMinusID + "</td>" +          //партия списания
                                "<td>" + queryDocSecondHandRetailReturnTab[0].DocSecondHandRetailReturnID + "</td>" +        //№ д-та
                                "<td>" + queryDocSecondHandRetailReturnTab[0].DirServiceNomenID + "</td>" +               //Код товара
                                "<td>" + queryDocSecondHandRetailReturnTab[0].Quantity + "</td>" +                 //списуемое к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg117_1
                                );
                        }

                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = await db.Rem2PartyMinuses.FindAsync(iRem2PartyMinusID);
                        db.Rem2PartyMinuses.Remove(rem2PartyMinus);
                        await db.SaveChangesAsync();
                    }

                    #endregion

                    #region Удаляем все записи из таблицы "Rem2PartyMinuses" - !!! НЕ НАДО !!!
                    //Удаляем все записи из таблицы "Rem2PartyMinuses"
                    //Что бы правильно Проверяло на Остаток.
                    //А то, товар уже списан, а я проверяю на остаток!

                    await Task.Run(() => rem2PartyMinuses.Delete(db, Convert.ToInt32(doc.DocID)));

                    #endregion


                    #region Проверки и Списание с партий (Rem2PartyMinuses)

                    for (int i = 0; i < docSecondHandRetailActWriteOffTabCollection.Count(); i++)
                    {
                        #region Проверка

                        //Переменные
                        int iRem2PartyID = Convert.ToInt32(docSecondHandRetailActWriteOffTabCollection[i].Rem2PartyID);
                        double dQuantity = docSecondHandRetailActWriteOffTabCollection[i].Quantity;
                        //Находим партию
                        Models.Sklad.Rem.Rem2Party rem2Party = await db.Rem2Parties.FindAsync(iRem2PartyID);
                        if (rem2Party == null)
                        {
                            throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg116_1 + iRem2PartyID + Classes.Language.Sklad.Language.msg116_2);
                        }
                        db.Entry(rem2Party).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                        #region 1. Есть ли остаток в партии с которой списываем!
                        if (rem2Party.Remnant < dQuantity)
                        {
                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg104 +

                                "<tr>" +
                                "<td>" + docSecondHandRetailActWriteOffTabCollection[i].Rem2PartyID + "</td>" +                                //партия
                                "<td>" + docSecondHandRetailActWriteOffTabCollection[i].DirServiceNomenID + "</td>" +                                //Код товара
                                "<td>" + docSecondHandRetailActWriteOffTabCollection[i].Quantity + "</td>" +                                  //списуемое к-во
                                "<td>" + rem2Party.Remnant + "</td>" +                                                  //остаток партии
                                "<td>" + (docSecondHandRetailActWriteOffTabCollection[i].Quantity - rem2Party.Remnant).ToString() + "</td>" +  //недостающее к-во
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg104_1
                            );
                        }
                        #endregion

                        #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                        if (rem2Party.DirWarehouseID != docSecondHandRetailActWriteOff.DirWarehouseID)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandRetailActWriteOff.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandRetailActWriteOff.DirWarehouseID);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg105 +

                                "<tr>" +
                                "<td>" + docSecondHandRetailActWriteOffTabCollection[i].Rem2PartyID + "</td>" +           //партия
                                "<td>" + docSecondHandRetailActWriteOffTabCollection[i].DirServiceNomenID + "</td>" +           //Код товара
                                "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                                "<td>" + rem2Party.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                                "</tr>" +
                                "</table>" +

                                Classes.Language.Sklad.Language.msg105_1
                            );
                        }
                        #endregion

                        #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                        if (rem2Party.DirContractorIDOrg != docSecondHandRetailActWriteOff.DirContractorIDOrg)
                        {
                            //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandRetailActWriteOff.dirWarehouse.DirWarehouseName"
                            Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docSecondHandRetailActWriteOff.DirContractorIDOrg);

                            throw new System.InvalidOperationException(
                                Classes.Language.Sklad.Language.msg106 +

                                "<tr>" +
                                "<td>" + docSecondHandRetailActWriteOffTabCollection[i].Rem2PartyID + "</td>" +           //партия
                                "<td>" + docSecondHandRetailActWriteOffTabCollection[i].DirServiceNomenID + "</td>" +           //Код товара
                                "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                                "<td>" + rem2Party.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
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

                        Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                        rem2PartyMinus.Rem2PartyMinusID = null;
                        rem2PartyMinus.Rem2PartyID = Convert.ToInt32(docSecondHandRetailActWriteOffTabCollection[i].Rem2PartyID);

                        rem2PartyMinus.DirServiceNomenID = docSecondHandRetailActWriteOffTabCollection[i].DirServiceNomenID;
                        rem2PartyMinus.Quantity = docSecondHandRetailActWriteOffTabCollection[i].Quantity;
                        rem2PartyMinus.DirCurrencyID = docSecondHandRetailActWriteOffTabCollection[i].DirCurrencyID;
                        rem2PartyMinus.DirCurrencyMultiplicity = docSecondHandRetailActWriteOffTabCollection[i].DirCurrencyMultiplicity;
                        rem2PartyMinus.DirCurrencyRate = docSecondHandRetailActWriteOffTabCollection[i].DirCurrencyRate;
                        rem2PartyMinus.DirVatValue = docSecondHandRetailActWriteOff.DirVatValue;
                        rem2PartyMinus.DirWarehouseID = docSecondHandRetailActWriteOff.DirWarehouseID;
                        rem2PartyMinus.DirContractorIDOrg = docSecondHandRetailActWriteOff.DirContractorIDOrg;


                        //!!!
                        /*
                        //rem2PartyMinus.DirContractorID = docSecondHandRetailActWriteOff.DirContractorID;
                        if (docSecondHandRetailActWriteOff.DirContractorID != null) rem2PartyMinus.DirContractorID = Convert.ToInt32(docSecondHandRetailActWriteOff.DirContractorID);
                        else rem2PartyMinus.DirContractorID = docSecondHandRetailActWriteOff.DirContractorIDOrg;
                        */
                        rem2PartyMinus.DirServiceContractorID = rem2Party.DirServiceContractorID;


                        rem2PartyMinus.DocID = Convert.ToInt32(docSecondHandRetailActWriteOff.DocID);
                        rem2PartyMinus.PriceCurrency = docSecondHandRetailActWriteOffTabCollection[i].PriceCurrency;
                        rem2PartyMinus.PriceVAT = docSecondHandRetailActWriteOffTabCollection[i].PriceVAT;
                        rem2PartyMinus.FieldID = Convert.ToInt32(docSecondHandRetailActWriteOffTabCollection[i].DocSecondHandRetailActWriteOffTabID);
                        rem2PartyMinus.Reserve = docSecondHandRetailActWriteOff.Reserve;

                        rem2PartyMinus.DirEmployeeID = doc.DirEmployeeID;
                        rem2PartyMinus.DocDate = doc.DocDate;

                        db.Entry(rem2PartyMinus).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion





                        #region 2. Приходуем к-во *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                        Models.Sklad.Rem.Rem2Party _rem2Party = new Models.Sklad.Rem.Rem2Party();

                        _rem2Party.Rem2PartyID = null;
                        _rem2Party.DirServiceNomenID = rem2PartyMinus.rem2Party.DirServiceNomenID;
                        _rem2Party.Quantity = docSecondHandRetailActWriteOffTabCollection[i].Quantity;
                        _rem2Party.Remnant = docSecondHandRetailActWriteOffTabCollection[i].Quantity;
                        _rem2Party.DirCurrencyID = rem2PartyMinus.rem2Party.DirCurrencyID;
                        // _rem2Party.DirCurrencyMultiplicity = docSecondHandRetailActWriteOffTabCollection[i].DirCurrencyMultiplicity;
                        // _rem2Party.DirCurrencyRate = docSecondHandRetailActWriteOffTabCollection[i].DirCurrencyRate;
                        _rem2Party.DirVatValue = rem2PartyMinus.rem2Party.DirVatValue;
                        _rem2Party.DirWarehouseID = rem2PartyMinus.rem2Party.DirWarehouseID;
                        _rem2Party.DirWarehouseIDDebit = rem2PartyMinus.rem2Party.DirWarehouseIDDebit;
                        _rem2Party.DirWarehouseIDPurch = rem2PartyMinus.rem2Party.DirWarehouseIDPurch;
                        _rem2Party.DirContractorIDOrg = rem2PartyMinus.rem2Party.DirContractorIDOrg;


                        //!!!
                        /*
                        if (docSecondHandRetailActWriteOff.DirContractorID != null) _rem2Party.DirContractorID = rem2PartyMinus.rem2Party.DirContractorID;
                        else _rem2Party.DirContractorID = docSecondHandRetailActWriteOff.DirContractorIDOrg;
                        */
                        _rem2Party.DirServiceContractorID = rem2PartyMinus.rem2Party.DirServiceContractorID;


                        //Дата Приёмки товара
                        _rem2Party.DocDatePurches = rem2PartyMinus.rem2Party.DocDatePurches;

                        /*
                        _rem2Party.DirCharColourID = rem2PartyMinus.rem2Party.DirCharColourID; //docSecondHandRetailActWriteOffTabCollection[i].DirCharColourID;
                        _rem2Party.DirCharMaterialID = rem2PartyMinus.rem2Party.DirCharMaterialID;
                        _rem2Party.DirCharNameID = rem2PartyMinus.rem2Party.DirCharNameID;
                        _rem2Party.DirCharSeasonID = rem2PartyMinus.rem2Party.DirCharSeasonID;
                        _rem2Party.DirCharSexID = rem2PartyMinus.rem2Party.DirCharSexID;
                        _rem2Party.DirCharSizeID = rem2PartyMinus.rem2Party.DirCharSizeID;
                        _rem2Party.DirCharStyleID = rem2PartyMinus.rem2Party.DirCharStyleID;
                        _rem2Party.DirCharTextureID = rem2PartyMinus.rem2Party.DirCharTextureID;
                        */

                        _rem2Party.SerialNumber = rem2PartyMinus.rem2Party.SerialNumber;
                        _rem2Party.Barcode = rem2PartyMinus.rem2Party.Barcode;

                        _rem2Party.DocID = Convert.ToInt32(docSecondHandRetailActWriteOff.DocID);
                        _rem2Party.PriceCurrency = rem2PartyMinus.rem2Party.PriceCurrency;
                        _rem2Party.PriceVAT = rem2PartyMinus.rem2Party.PriceVAT;
                        _rem2Party.FieldID = Convert.ToInt32(docSecondHandRetailActWriteOffTabCollection[i].DocSecondHandRetailActWriteOffTabID);

                        _rem2Party.PriceRetailVAT = rem2PartyMinus.rem2Party.PriceRetailVAT;
                        _rem2Party.PriceRetailCurrency = rem2PartyMinus.rem2Party.PriceRetailCurrency;
                        _rem2Party.PriceWholesaleVAT = rem2PartyMinus.rem2Party.PriceWholesaleVAT;
                        _rem2Party.PriceWholesaleCurrency = rem2PartyMinus.rem2Party.PriceWholesaleCurrency;
                        _rem2Party.PriceIMVAT = rem2PartyMinus.rem2Party.PriceIMVAT;
                        _rem2Party.PriceIMCurrency = rem2PartyMinus.rem2Party.PriceIMCurrency;

                        _rem2Party.Rem2PartyMinusID = rem2PartyMinus.Rem2PartyMinusID;

                        _rem2Party.DirReturnTypeID = docSecondHandRetailActWriteOffTabCollection[i].DirReturnTypeID;
                        _rem2Party.DirDescriptionID = DirDescriptionID;

                        _rem2Party.DirEmployeeID = doc.DirEmployeeID;
                        _rem2Party.DocDate = doc.DocDate;
                        _rem2Party.DirNomenMinimumBalance = sysSetting.DirNomenMinimumBalance;

                        //Models.Sklad.Rem.Rem2Party[] rem2PartyCollection = new Models.Sklad.Rem.Rem2Party[docSecondHandRetailActWriteOffTabCollection.Count()];
                        //rem2PartyCollection[i] = rem2Party;

                        //Controllers.Sklad.Rem.Rem2PartiesController rem2Partys = new Rem.Rem2PartiesController();
                        //await Task.Run(() => rem2Partys.Save(db, rem2PartyCollection)); //rem2Partys.Save(db, rem2PartyCollection);

                        
                        //Документ создания первой партии (создания документа)
                        //Нужен для правильно подсчёта партии
                        _rem2Party.DocIDFirst = rem2Party.DocIDFirst;


                        db.Entry(_rem2Party).State = EntityState.Added;
                        await db.SaveChangesAsync();

                        #endregion





                        //rem2Party - уже нашли (выше).
                        /*
                        rem2Party.DirDescriptionID = DirDescriptionID; // docSecondHandRetailActWriteOffTabCollection[i].DirDescriptionID;
                        rem2Party.DirReturnTypeID = docSecondHandRetailActWriteOffTabCollection[i].DirReturnTypeID;
                        db.Entry(rem2Party).State = EntityState.Modified;
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


            return docSecondHandRetailActWriteOff;
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
                "[DocSecondHandRetailActWriteOffs].[DocSecondHandRetailID] AS [DocSecondHandRetailID], " +
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
                "[DocSecondHandRetailActWriteOffs].[Reserve] AS [Reserve] " +

                "FROM [DocSecondHandRetailActWriteOffs] " +

                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandRetailActWriteOffs].[DocID] " +
                "INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] " +
                "INNER JOIN [DirContractors] AS [DirContractors] ON [Docs].[DirContractorID] = [DirContractors].[DirContractorID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocSecondHandRetailActWriteOffs].[DirWarehouseID] " +
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