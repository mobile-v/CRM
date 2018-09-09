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
using System.Collections;
using System.Data.SQLite;
using System.Web.Script.Serialization;

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandPurches
{
    public class DocSecondHandPurchesController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();
        Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 65;

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;
            public int? GroupID = 0;
            public string parSearch = "";
            public int? FilterType; // == DirSecondHandStatusID
            public int? DirWarehouseID;
            public DateTime? DateS;
            public DateTime? DatePo;
            public int? DirSecondHandStatusIDS; public int? DirSecondHandStatusIDPo;
            public int? DirSecondHandStatusID_789;
            public int iTypeService; //1 - Приёмка, 2 - Мастерская, 3 - Выдача
            public int? DirEmployeeID;

            //Отобразить "Архив"
            public int? DocSecondHandPurchID;
            public string collectionWrapper = "";
        }
        // GET: api/DocSecondHandPurches
        public async Task<IHttpActionResult> GetDocSecondHandPurches(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurches"));
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
                _params.limit = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.GroupID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pGroupID", true) == 0).Value); //Кликнули по группе
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск
                _params.FilterType = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "FilterType", true) == 0).Value);
                _params.DirSecondHandStatusIDS = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirSecondHandStatusIDS", true) == 0).Value);
                _params.DirSecondHandStatusIDPo = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirSecondHandStatusIDPo", true) == 0).Value);
                _params.DirSecondHandStatusID_789 = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirSecondHandStatusID_789", true) == 0).Value);
                _params.DirEmployeeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value);
                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);
                _params.collectionWrapper = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "collectionWrapper", true) == 0).Value;

                //Архив
                _params.DocSecondHandPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandPurchID", true) == 0).Value);

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
                        from x in db.DocSecondHandPurches

                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        join docServicePurches1 in db.DocServicePurches on x.DocIDService equals docServicePurches1.DocID into docServicePurches2
                        from docServicePurches in docServicePurches2.DefaultIfEmpty()

                        select new
                        {
                            DirServiceNomenID = x.DirServiceNomenID,

                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName,

                            DocID = x.DocID,
                            DocDate = x.doc.DocDate,
                            Base = x.doc.Base,
                            Held = x.doc.Held,
                            Discount = x.doc.Discount,
                            Del = x.doc.Del,
                            Description = x.doc.Description,
                            IsImport = x.doc.IsImport,
                            DirVatValue = x.doc.DirVatValue,

                            DirEmployeeID = x.doc.DirEmployeeID,

                            DocSecondHandPurchID = x.DocSecondHandPurchID,
                            DirContractorName = x.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = x.doc.dirContractorOrg.DirContractorID,
                            //DirContractorNameOrg = x.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = x.dirWarehouse.DirWarehouseID,
                            DirWarehouseIDPurches = x.DirWarehouseIDPurches,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,
                            ProblemClientWords = x.ProblemClientWords,
                            SerialNumber = x.SerialNumber,
                            DirSecondHandStatusID = x.DirSecondHandStatusID,
                            Status = x.DirSecondHandStatusID,
                            DirSecondHandStatusID_789 = x.DirSecondHandStatusID_789,
                            DirSecondHandStatusName = x.dirSecondHandStatus.DirSecondHandStatusName,
                            DirServiceContractorName = x.DirServiceContractorName,
                            DirServiceContractorPhone = x.DirServiceContractorPhone,
                            //UrgentRepairs = x.UrgentRepairs,

                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",

                            PriceVAT = x.PriceVAT + x.Sums, PriceCurrency = x.PriceVAT + x.Sums,
                            PriceRetailVAT = x.PriceRetailVAT, PriceRetailCurrency = x.PriceRetailCurrency,
                            PriceWholesaleVAT = x.PriceWholesaleVAT, PriceWholesaleCurrency = x.PriceWholesaleCurrency,
                            PriceIMVAT = x.PriceIMVAT, PriceIMCurrency = x.PriceIMCurrency,

                            DateDone = x.DateDone.ToString(),
                            //PrepaymentSum = x.PrepaymentSum == null ? 0 : x.PrepaymentSum,
                            //Оплата
                            Payment = x.doc.Payment,
                            //Мастер
                            DirEmployeeIDMaster = x.DirEmployeeIDMaster,
                            //FromGuarantee = x.FromGuarantee,
                            ServiceTypeRepair = x.ServiceTypeRepair,

                            DirServiceContractorID = x.DirServiceContractorID,
                            //QuantityOk = x.dirServiceContractor.QuantityOk,
                            //QuantityFail = x.dirServiceContractor.QuantityFail,
                            //QuantityCount = x.dirServiceContractor.QuantityCount

                            IssuanceDate = x.IssuanceDate.ToString(),
                            Sums = x.Sums,

                            //Перемещён в БУ
                            InSecondHand = x.FromService,
                            InSecondHandString = x.FromService == true ? "В Б/У" : "",
                            DocIDService = x.DocIDService,
                            DocServicePurchID = docServicePurches.DocServicePurchID,

                            //Дата продажи
                            DateRetail = x.DateRetail,
                            DateReturn = x.DateReturn,

                            x.DirReturnTypeID,
                            x.dirReturnType.DirReturnTypeName,
                            x.dirDescription.DirDescriptionName,

                            Exist = 1,
                            ExistName = "Присутствует",
                        }

                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Отобразить "Предыдущие ремонты"

                if (_params.DocSecondHandPurchID != null && _params.DocSecondHandPurchID > 0)
                {
                    //1. Получаем "DirServiceContractorID" по "DocSecondHandPurchID"
                    //2.Добавляем условие в запрос

                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(_params.DocSecondHandPurchID);
                    int? DirServiceContractorID = docSecondHandPurch.DirServiceContractorID;
                    if (DirServiceContractorID == null) { DirServiceContractorID = 0; }

                    query = query.Where(x => x.DirServiceContractorID == DirServiceContractorID && x.DocSecondHandPurchID != _params.DocSecondHandPurchID);
                }

                #endregion



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


                #region Фильтр FilterType

                if (_params.FilterType > 0)
                {
                    query = query.Where(x => x.DirSecondHandStatusID == _params.FilterType);
                }

                #endregion

                #region Фильтр DirSecondHandStatusID S и Po

                //DirSecondHandStatusIDS && DirSecondHandStatusIDPo
                if (_params.DirSecondHandStatusIDS != _params.DirSecondHandStatusIDPo)
                {
                    if (_params.DirSecondHandStatusIDS > 0 && _params.DirSecondHandStatusIDPo > 0) { query = query.Where(x => x.DirSecondHandStatusID >= _params.DirSecondHandStatusIDS && x.DirSecondHandStatusID <= _params.DirSecondHandStatusIDPo); }
                    else
                    {
                        if (_params.DirSecondHandStatusIDS > 0) query = query.Where(x => x.DirSecondHandStatusID >= _params.DirSecondHandStatusIDS);
                        if (_params.DirSecondHandStatusIDPo > 0) query = query.Where(x => x.DirSecondHandStatusID <= _params.DirSecondHandStatusIDPo);
                    }
                }
                else
                {
                    if (_params.DirSecondHandStatusIDS > 0) query = query.Where(x => x.DirSecondHandStatusID == _params.DirSecondHandStatusIDS);
                }

                //DirSecondHandStatusID_789
                if (_params.DirSecondHandStatusID_789 > 0 && _params.DirSecondHandStatusIDPo > 0)
                {
                    query = query.Where(x => x.DirSecondHandStatusID_789 == _params.DirSecondHandStatusID_789);
                }

                //DirEmployeeID
                if (_params.DirEmployeeID > 0)
                {
                    query = query.Where(x => x.DirEmployeeID <= _params.DirEmployeeID && x.DirSecondHandStatusID < 7);
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
                    //DateTime dDateTime;
                    //bool bDateTime = DateTime.TryParse(_params.parSearch, out dDateTime);


                    //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                    if (bResult32 && _params.parSearch.IndexOf("+") == -1)
                    {
                        query = query.Where(x => x.DocSecondHandPurchID == iNumber32);
                    }
                    //Если Дата
                    /*else if (bDateTime)
                    {
                        query = query.Where(x => x.DocDate == dDateTime);
                    }*/
                    //Иначе, только текстовые поля
                    else
                    {
                        _params.parSearch = _params.parSearch.Replace("+", "");
                        query = query.Where(x => x.DirServiceContractorPhone.Contains(_params.parSearch));
                    }

                }

                #endregion


                #region OrderBy и Лимит

                //query = query.OrderByDescending(x => x.DocDate).Skip(_params.Skip).Take(_params.limit);
                query = query.OrderByDescending(x => x.DocSecondHandPurchID); //.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во
                /*
                int dirCount = await Task.Run(() => db.DocSecondHandPurches.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());
                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;
                */

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount = query.Count();
                if (dirCount < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount;

                if (_params.collectionWrapper == "DocSecondHandInvTab")
                {
                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DocSecondHandInvTab = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));
                }
                else
                {
                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DocSecondHandPurch = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));
                }

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandPurches/5
        [ResponseType(typeof(DocSecondHandPurch))]
        public async Task<IHttpActionResult> GetDocSecondHandPurch(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurches"));
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
                        from docSecondHandPurches in db.DocSecondHandPurches
                        where docSecondHandPurches.DocSecondHandPurchID == id
                        select docSecondHandPurches
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON


                #region Полный путь Аппарата
                
                string DirServiceNomenPatchFull = "",
                       ID0 = "", ID1 = "", ID2 = "",
                       ID01 = "", ID11 = "", ID21 = "";

                var queryDirServiceNomenID = await
                    (
                        from x in db.DocSecondHandPurches
                        where x.DocSecondHandPurchID == id
                        select x.DirServiceNomenID
                    ).ToArrayAsync();

                if (queryDirServiceNomenID.Count() > 0)
                {
                    string[] ret1 = await Task.Run(() => mPatchFull(db, queryDirServiceNomenID[0]));
                    DirServiceNomenPatchFull = ret1[0];

                    //Для поиска в списке товара (когда нажимаем на кнопку "Склад", что бы сразу попасть на нужную группу)
                    string[] sID = ret1[1].Split(',');
                    try { if (!String.IsNullOrEmpty(sID[0])) ID0 = sID[0].ToUpper(); if (ID0[ID0.Length - 1].ToString() == " ") ID0 = ID0.Remove(ID0.Length - 1); } catch { }
                    try { if (!String.IsNullOrEmpty(sID[1])) ID1 = sID[1].ToUpper(); if (ID1[ID1.Length - 1].ToString() == " ") ID1 = ID1.Remove(ID1.Length - 1); } catch { }
                    try { if (!String.IsNullOrEmpty(sID[2])) ID2 = sID[2].ToUpper(); if (ID2[ID2.Length - 1].ToString() == " ") ID2 = ID2.Remove(ID2.Length - 1); } catch { }

                    /*
                    string[] sID_DirNomen = ret[3].Split(',');
                    try { if (!String.IsNullOrEmpty(sID_DirNomen[0])) ID01 = sID_DirNomen[0].ToUpper(); if (ID01[ID01.Length - 1].ToString() == " ") ID01 = ID0.Remove(ID0.Length - 1); } catch { }
                    try { if (!String.IsNullOrEmpty(sID_DirNomen[1])) ID11 = sID_DirNomen[1].ToUpper(); if (ID11[ID11.Length - 1].ToString() == " ") ID11 = ID1.Remove(ID1.Length - 1); } catch { }
                    try { if (!String.IsNullOrEmpty(sID_DirNomen[2])) ID21 = sID_DirNomen[2].ToUpper(); if (ID21[ID21.Length - 1].ToString() == " ") ID21 = ID2.Remove(ID2.Length - 1); } catch { }
                    */

                    int[] ret2 = await Task.Run(() => mDirNomenID(db, ID0, ID1, ID2));
                    ID01 = ret2[0].ToString();
                    ID11 = ret2[1].ToString();
                    ID21 = ret2[2].ToString();
                }

                #endregion


                #region QUERY

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docSecondHandPurches in db.DocSecondHandPurches

                            /*
                            join docSecondHandPurch1Tabs1 in db.DocSecondHandPurch1Tabs on docSecondHandPurches.DocSecondHandPurchID equals docSecondHandPurch1Tabs1.DocSecondHandPurchID into docSecondHandPurch1Tabs2
                            from docSecondHandPurch1Tabs in docSecondHandPurch1Tabs2.DefaultIfEmpty()

                            join docSecondHandPurch2Tabs1 in db.DocSecondHandPurch2Tabs on docSecondHandPurches.DocSecondHandPurchID equals docSecondHandPurch2Tabs1.DocSecondHandPurchID into docSecondHandPurch2Tabs2
                            from docSecondHandPurch2Tabs in docSecondHandPurch2Tabs2.DefaultIfEmpty()
                            */

                        join docServicePurches1 in db.DocServicePurches on docSecondHandPurches.DocIDService equals docServicePurches1.doc.DocID into docServicePurches2
                        from docServicePurches in docServicePurches2.DefaultIfEmpty()


                        #endregion

                        where docSecondHandPurches.DocSecondHandPurchID == id

                        #region select

                        select new
                        {
                            DocID = docSecondHandPurches.DocID,
                            DocDate = docSecondHandPurches.doc.DocDate,
                            Base = docSecondHandPurches.doc.Base,
                            Held = docSecondHandPurches.doc.Held,
                            Discount = docSecondHandPurches.doc.Discount,
                            Del = docSecondHandPurches.doc.Del,
                            Description = docSecondHandPurches.doc.Description,
                            IsImport = docSecondHandPurches.doc.IsImport,
                            DirVatValue = docSecondHandPurches.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandPurches.doc.DirPaymentTypeID,

                            DirServiceNomenID = docSecondHandPurches.DirServiceNomenID,

                            DirServiceNomenNameLittle = docSecondHandPurches.dirServiceNomen.DirServiceNomenName,

                            DirServiceNomenName =
                            DirServiceNomenPatchFull == null ? docSecondHandPurches.dirServiceNomen.DirServiceNomenName
                            :
                            DirServiceNomenPatchFull, // + docSecondHandPurches.dirServiceNomen.DirServiceNomenName,

                            ID0 = ID0,
                            ID1 = ID1,
                            ID2 = ID2,

                            ID01 = ID01,
                            ID11 = ID11,
                            ID21 = ID21,

                            DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,
                            DirContractorName = docSecondHandPurches.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandPurches.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandPurches.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandPurches.dirWarehouse.DirWarehouseID,
                            DirWarehouseIDPurches = docSecondHandPurches.DirWarehouseIDPurches,
                            DirWarehouseName = docSecondHandPurches.dirWarehouse.DirWarehouseName,

                            DirSecondHandStatusID = docSecondHandPurches.DirSecondHandStatusID,
                            SerialNumberNo = docSecondHandPurches.SerialNumberNo,
                            SerialNumber = docSecondHandPurches.SerialNumber,
                            //TypeRepair = docSecondHandPurches.TypeRepair,
                            //ComponentDevice = docSecondHandPurches.ComponentDevice,

                            //ComponentBattery = docSecondHandPurches.ComponentBattery,
                            //ComponentBatterySerial = docSecondHandPurches.ComponentBatterySerial,
                            //ComponentBackCover = docSecondHandPurches.ComponentBackCover,
                            ComponentPasTextNo = docSecondHandPurches.ComponentPasTextNo,
                            //ComponentPasText = docSecondHandPurches.ComponentPasText,
                            ComponentOtherText = docSecondHandPurches.ComponentOtherText,
                            ProblemClientWords = docSecondHandPurches.ProblemClientWords,
                            Note = docSecondHandPurches.Note,
                            DirServiceContractorName = docSecondHandPurches.DirServiceContractorName,
                            DirServiceContractorRegular = docSecondHandPurches.DirServiceContractorRegular,
                            DirServiceContractorID = docSecondHandPurches.DirServiceContractorID,
                            DirServiceContractorAddress = docSecondHandPurches.DirServiceContractorAddress,
                            DirServiceContractorPhone = docSecondHandPurches.DirServiceContractorPhone,
                            DirServiceContractorEmail = docSecondHandPurches.DirServiceContractorEmail,

                            PriceVAT = docSecondHandPurches.PriceVAT, PriceCurrency = docSecondHandPurches.PriceCurrency,
                            PriceRetailVAT = docSecondHandPurches.PriceRetailVAT, PriceRetailCurrency = docSecondHandPurches.PriceRetailCurrency,
                            PriceWholesaleVAT = docSecondHandPurches.PriceWholesaleVAT, PriceWholesaleCurrency = docSecondHandPurches.PriceWholesaleCurrency,
                            PriceIMVAT = docSecondHandPurches.PriceIMVAT, PriceIMCurrency = docSecondHandPurches.PriceIMCurrency,

                            DirCurrencyID = docSecondHandPurches.DirCurrencyID,
                            DirCurrencyRate = docSecondHandPurches.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSecondHandPurches.DirCurrencyMultiplicity,
                            DirCurrencyName = docSecondHandPurches.dirCurrency.DirCurrencyName + " (" + docSecondHandPurches.DirCurrencyRate + ", " + docSecondHandPurches.DirCurrencyMultiplicity + ")",

                            DateDone = docSecondHandPurches.DateDone,
                            //UrgentRepairs = docSecondHandPurches.UrgentRepairs,
                            //Prepayment = docSecondHandPurches.Prepayment,
                            //PrepaymentSum = docSecondHandPurches.PrepaymentSum == null ? 0 : docSecondHandPurches.PrepaymentSum,

                            //Оплата
                            Payment = docSecondHandPurches.doc.Payment,
                            //Мастер
                            DirEmployeeIDMaster = docSecondHandPurches.DirEmployeeIDMaster,
                            DirEmployeeNameMaster = docSecondHandPurches.dirEmployee.DirEmployeeName,

                            ServiceTypeRepair = docSecondHandPurches.ServiceTypeRepair,

                            //К-во раз Клиент обращался в сервис
                            QuantityOk = docSecondHandPurches.dirServiceContractor.QuantityOk,
                            QuantityFail = docSecondHandPurches.dirServiceContractor.QuantityFail,
                            QuantityCount = docSecondHandPurches.dirServiceContractor.QuantityCount,


                            // *** СУММЫ *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                            //1. Подсчет табличной части Работы "SumDocSecondHandPurch1Tabs"
                            SumDocSecondHandPurch1Tabs = docSecondHandPurches.Sums1, //dSumDocSecondHandPurch1Tabs,
                            SumDocSecondHandPurch1Tabs2 = docSecondHandPurches.Sums1, //dSumDocSecondHandPurch1Tabs,
                            //2. Подсчет табличной части Работы "SumDocSecondHandPurch2Tabs"
                            SumDocSecondHandPurch2Tabs = docSecondHandPurches.Sums2, //dSumDocSecondHandPurch2Tabs,
                            SumDocSecondHandPurch2Tabs2 = docSecondHandPurches.Sums2, //dSumDocSecondHandPurch2Tabs,
                            //3. Сумма 1+2 "SumTotal"
                            SumTotal = docSecondHandPurches.Sums1 + docSecondHandPurches.Sums2, //dSumDocSecondHandPurch1Tabs + dSumDocSecondHandPurch2Tabs,
                            SumTotal2 = docSecondHandPurches.Sums1 + docSecondHandPurches.Sums2, //dSumDocSecondHandPurch1Tabs + dSumDocSecondHandPurch2Tabs,
                            //5. 3 - 4 "SumTotal2"
                            SumTotal2a = docSecondHandPurches.Sums1 + docSecondHandPurches.Sums2, //dSumDocSecondHandPurch1Tabs + dSumDocSecondHandPurch2Tabs, // - docSecondHandPurches.PrepaymentSum,
                            PriceVATSums = docSecondHandPurches.Sums1 + docSecondHandPurches.Sums2 + docSecondHandPurches.PriceVAT, //dSumDocSecondHandPurch1Tabs + dSumDocSecondHandPurch2Tabs + docSecondHandPurches.PriceVAT, // - docSecondHandPurches.PrepaymentSum,

                            //Alerted = docSecondHandPurches.AlertedCount == null ? "Не оповещён" : "Оповещён (" + docSecondHandPurches.AlertedCount + ") " + docSecondHandPurches.AlertedDateTxt

                            //Перемещён в БУ
                            FromService = docSecondHandPurches.FromService,
                            FromServiceString = docSecondHandPurches.FromService == true ? "В Б/У" : "",
                            DocIDService = docSecondHandPurches.DocIDService, DocServicePurchID = docServicePurches.DocServicePurchID,

                        }

                        #endregion

                    ).ToListAsync());

                #endregion



                if (query.Count() > 0)
                {

                    #region Смена статуса и запис в Лог
                    //1. Изменять статус, если бы "Принят" на "В диагностике" + писать в Лог изменение статуса
                    //2. Изменять инженера, который открыл + писать в Лог изменение инженера

                    //1. Проверяем статус, если == 1, то меняем на 2
                    if (query[0].DirSecondHandStatusID == 1)
                    {
                        //Меняем статус + меняем мастера
                        Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await dbRead.DocSecondHandPurches.FindAsync(id);
                        docSecondHandPurch.DirEmployeeIDMaster = field.DirEmployeeID;
                        docSecondHandPurch.DirSecondHandStatusID = 2;
                        dbRead.Entry(docSecondHandPurch).State = EntityState.Modified;
                        await dbRead.SaveChangesAsync();

                        //Пишем в Лог о смене статуса и мастера, если такое было
                        logService.DocSecondHandPurchID = id;
                        logService.DirSecondHandLogTypeID = 1;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                        //if (query[0].DirEmployeeIDMaster != field.DirEmployeeID) logService.Msg = "Смена мастера " + query[0].DirEmployeeNameMaster;

                        await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);
                    }

                    //Пишем в Лог о смене мастера, если такое было
                    if (query[0].DirEmployeeIDMaster != field.DirEmployeeID)
                    {
                        Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await dbRead.DocSecondHandPurches.FindAsync(id);
                        docSecondHandPurch.DirEmployeeIDMaster = field.DirEmployeeID;
                        //docSecondHandPurch.DirSecondHandStatusID = 2;
                        dbRead.Entry(docSecondHandPurch).State = EntityState.Modified;
                        await dbRead.SaveChangesAsync();
                    }

                    #endregion

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

        // PUT: api/DocSecondHandPurches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandPurch(int id, DocSecondHandPurch docSecondHandPurch, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurches"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                var paramList = request.GetQueryNameValuePairs();

                //1 - Изменили "Дату Готовности"
                //Params _params = new Params();
                //_params.iTypeService = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "iTypeService", true) == 0).Value);

                DateTime DateDone = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateDone", true) == 0).Value).ToString("yyyy-MM-dd 00:00:00"));
                if (DateDone < Convert.ToDateTime("01.01.1800"))
                {
                    //...
                }


                #endregion


                #region Сохранение

                try
                {
                    //Находим Аппарат и меняем дату готовности
                    docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(id);
                    DateTime DateDoneOLD = docSecondHandPurch.DateDone;
                    docSecondHandPurch.DateDone = DateDone;

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            db.Entry(docSecondHandPurch).State = EntityState.Modified;
                            await db.SaveChangesAsync();


                            #region 4. Log

                            logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                            logService.DirSecondHandLogTypeID = 7;
                            logService.DirEmployeeID = field.DirEmployeeID;
                            //logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                            logService.Msg = "Смена даты готовности с " + DateDoneOLD.ToString("yyyy-MM-dd") + " на " + DateDone.ToString("yyyy-MM-dd");

                            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                            #endregion


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
                    sysJourDisp.TableFieldID = docSecondHandPurch.DocSecondHandPurchID;
                    sysJourDisp.Description = "";
                    try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                    #endregion


                    dynamic collectionWrapper = new
                    {
                        DocID = docSecondHandPurch.DocID,
                        DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID
                    };
                    return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
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

        //Смена статуса
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandPurch(int id, int DirStatusID, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurches"));
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
                int DirPaymentTypeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPaymentTypeID", true) == 0).Value);
                double SumTotal2a = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "SumTotal2a", true) == 0).Value.Replace(".", ","));
                string sReturnRresults = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sReturnRresults", true) == 0).Value;

                double PriceRetailVAT = 0; if (paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceRetailVAT", true) == 0).Value != null) PriceRetailVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceRetailVAT", true) == 0).Value.Replace(".", ",")); else PriceRetailVAT = 0;
                double PriceWholesaleVAT = 0; if (paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceWholesaleVAT", true) == 0).Value != null) PriceWholesaleVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceWholesaleVAT", true) == 0).Value.Replace(".", ",")); else PriceWholesaleVAT = 0;
                double PriceIMVAT = 0; if (paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceIMVAT", true) == 0).Value != null) PriceIMVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceIMVAT", true) == 0).Value.Replace(".", ",")); else PriceIMVAT = 0;

                int? KKMSCheckNumber = null;
                try
                {
                    KKMSCheckNumber = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "KKMSCheckNumber", true) == 0).Value.Replace(".", ","));
                }
                catch { }
                string KKMSIdCommand = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "KKMSIdCommand", true) == 0).Value;


                //!!! Важно !!!
                //Сергей предложил: не перекидывать в вкладки, а сразу переносить в продажу или на разбор
                int locDirSecondHandStatusI_OLD = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "locDirSecondHandStatusI_OLD", true) == 0).Value);


                //Если "В торговлю"
                DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(id);

                //Важно
                //Сергей предложил: не перекидывать в вкладки, а сразу переносить в продажу или на разбор
                if (locDirSecondHandStatusI_OLD != 0)
                {
                    docSecondHandPurch.DirSecondHandStatusID = locDirSecondHandStatusI_OLD;
                    docSecondHandPurch.DirSecondHandStatusID_789 = locDirSecondHandStatusI_OLD;
                    //db.Entry(docSecondHandPurch).State = EntityState.Modified;
                    //await Task.Run(() => db.SaveChangesAsync());
                }

                if (
                    docSecondHandPurch.DirSecondHandStatusID == 7 && DirStatusID == 9 &&
                    PriceRetailVAT > 0 && PriceWholesaleVAT > 0 && PriceIMVAT > 0
                   )
                {
                    docSecondHandPurch.PriceRetailVAT = PriceRetailVAT; docSecondHandPurch.PriceRetailCurrency = PriceRetailVAT;
                    docSecondHandPurch.PriceWholesaleVAT = PriceWholesaleVAT; docSecondHandPurch.PriceWholesaleCurrency = PriceWholesaleVAT;
                    docSecondHandPurch.PriceIMVAT = PriceIMVAT; docSecondHandPurch.PriceIMCurrency = PriceIMVAT;
                }

                #endregion

                #region Проверки
                //Если Статус "7", то проверить Таб часть-1
                // Если DirStatusID = 7 и нет ни одной выполненной работы, то не пускать сохранять и выдать эксепшн
                //if (iTypeService > 1 && docSecondHandPurch.DirStatusID == 7 && docSecondHandPurch1TabCollection.Length == 0) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg114); }

                var queryCount = await
                    (
                        from x in db.DocSecondHandPurch1Tabs
                        where x.DocSecondHandPurchID == id
                        select x
                    ).CountAsync();
                if (queryCount == 0 && DirStatusID == 7)
                {
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg114);
                }

                #endregion


                #region Сохранение

                try
                {
                    //Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = new DocSecondHandPurch();

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            await mStatusChange(db, docSecondHandPurch, DirStatusID, DirPaymentTypeID, SumTotal2a, sReturnRresults, locDirSecondHandStatusI_OLD, field, KKMSCheckNumber, KKMSIdCommand); //, ts, id 

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
                        DocID = docSecondHandPurch.DocID,
                        DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID
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

        //Смена гарантии
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandPurch(int id, int ServiceTypeRepair, int iTrash, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurches"));
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
                //int DirPaymentTypeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPaymentTypeID", true) == 0).Value);
                //double SumTotal2a = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "SumTotal2a", true) == 0).Value.Replace(".", ","));

                #endregion

                #region Проверки

                //...

                #endregion


                #region Сохранение

                try
                {
                    Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = new DocSecondHandPurch();

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            await mRepairChange(db, ts, docSecondHandPurch, id, ServiceTypeRepair, field);

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
                        DocID = docSecondHandPurch.DocID,
                        DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID
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



        // POST: api/DocSecondHandPurches
        [ResponseType(typeof(DocSecondHandPurch))]
        public async Task<IHttpActionResult> PostDocSecondHandPurch(DocSecondHandPurch docSecondHandPurch, HttpRequestMessage request)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurches"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            //save, save_close, held, held_cancel
            var paramList = request.GetQueryNameValuePairs();

            //1 - Приёмка, 2 - Мастерская, 3 - Выдача
            Params _params = new Params();
            _params.iTypeService = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "iTypeService", true) == 0).Value); //Записей на страницу

            string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
            if (_params.iTypeService == 3 && UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
            UO_Action = UO_Action.ToLower();

            //Получаем колекцию "Спецификации"

            Models.Sklad.Doc.DocSecondHandPurch1Tab[] docSecondHandPurch1TabCollection = null;
            if (!String.IsNullOrEmpty(docSecondHandPurch.recordsDocSecondHandPurch1Tab))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                docSecondHandPurch1TabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandPurch1Tab[]>(docSecondHandPurch.recordsDocSecondHandPurch1Tab);
            }

            Models.Sklad.Doc.DocSecondHandPurch2Tab[] docSecondHandPurch2TabCollection = null;
            if (!String.IsNullOrEmpty(docSecondHandPurch.recordsDocSecondHandPurch2Tab))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                docSecondHandPurch2TabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandPurch2Tab[]>(docSecondHandPurch.recordsDocSecondHandPurch2Tab);
            }

            #endregion

            #region Проверки

            if (!ModelState.IsValid && _params.iTypeService != 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            docSecondHandPurch.Substitute();

            #endregion


            #region Сохранение

            try
            {
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandPurch = await Task.Run(() => mPutPostDocSecondHandPurch(db, dbRead, UO_Action, docSecondHandPurch, docSecondHandPurch1TabCollection, docSecondHandPurch2TabCollection, EntityState.Added, _params.iTypeService, field)); //sysSetting
                        ts.Commit(); //.Complete();


                        try
                        {
                            #region 5. Sms

                            /*
                            if (
                                docSecondHandPurch.DirServiceContractorPhone != null && docSecondHandPurch.DirServiceContractorPhone.Length > 7
                                &&
                                sysSetting.DocSecondHandPurchSmsAutoShow
                               )
                            {

                                string res = "";

                                Models.Sklad.Dir.DirSmsTemplate dirSmsTemplate = await db.DirSmsTemplates.FindAsync(6);

                                PartionnyAccount.Controllers.Sklad.SMS.SmsController smsController = new SMS.SmsController();
                                await smsController.SenSms(
                                    res,
                                    sysSetting,
                                    40,
                                    6,
                                    docSecondHandPurch.DirServiceContractorPhone,
                                    dirSmsTemplate,
                                    field,
                                    db
                                    );

                            }
                            */

                            #endregion
                        }
                        catch (Exception ex7) { }
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
                sysJourDisp.TableFieldID = docSecondHandPurch.DocSecondHandPurchID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandPurch.DocID,
                    DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DocSecondHandPurches/5
        [ResponseType(typeof(DocSecondHandPurch))]
        public async Task<IHttpActionResult> DeleteDocSecondHandPurch(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandPurches"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion


            #region Удаление

            //Алгоритм.
            //Удаляем по порядку:
            //1. Rem2Parties
            //2. DocSecondHandPurchTabs
            //3. DocSecondHandPurches
            //4. Docs


            //Сотрудник
            Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(id);
            if (docSecondHandPurch == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


            using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
            {
                try
                {
                    #region 1. Ищим DocID *** *** *** *** ***

                    //1.1. Ищим DocID
                    int iDocID = 0;
                    var queryDocs1 = await
                        (
                            from x in db.DocSecondHandPurches
                            where x.DocSecondHandPurchID == id
                            select x
                        ).ToListAsync();
                    if (queryDocs1.Count() > 0) iDocID = Convert.ToInt32(queryDocs1[0].DocID);
                    else return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                    #endregion


                    #region 1. Rem2PartyMinuses *** *** *** *** ***

                    /*
                    //1.1. Удаляем "Rem2PartyMinuses"
                    var queryRem2PartyMinuses = await
                        (
                            from x in db.Rem2PartyMinuses
                            where x.DocID == iDocID
                            select x
                        ).ToListAsync();

                    if (queryRem2PartyMinuses.Count() > 0)
                    {
                        throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg126);
                    }
                    */

                    Classes.Function.WriteOffGoodsWithParty2 writeOffGoodsWithParty2 = new Classes.Function.WriteOffGoodsWithParty2();
                    bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty2.Exist(db, id));

                    #endregion


                    #region 2. DocSecondHandPurch1Tabs *** *** *** *** ***

                    var queryDocSecondHandPurch1Tabs = await
                        (
                            from x in db.DocSecondHandPurch1Tabs
                            where x.DocSecondHandPurchID == id
                            select x
                        ).ToListAsync();
                    for (int i = 0; i < queryDocSecondHandPurch1Tabs.Count(); i++)
                    {
                        Models.Sklad.Doc.DocSecondHandPurch1Tab docSecondHandPurch1Tab = await db.DocSecondHandPurch1Tabs.FindAsync(queryDocSecondHandPurch1Tabs[i].DocSecondHandPurch1TabID);
                        db.DocSecondHandPurch1Tabs.Remove(docSecondHandPurch1Tab);
                        await db.SaveChangesAsync();
                    }

                    #endregion

                    #region 2. DocSecondHandPurch2Tabs *** *** *** *** ***

                    var queryDocSecondHandPurch2Tabs = await
                        (
                            from x in db.DocSecondHandPurch2Tabs
                            where x.DocSecondHandPurchID == id
                            select x
                        ).ToListAsync();
                    for (int i = 0; i < queryDocSecondHandPurch2Tabs.Count(); i++)
                    {
                        Models.Sklad.Doc.DocSecondHandPurch2Tab docSecondHandPurch2Tab = await db.DocSecondHandPurch2Tabs.FindAsync(queryDocSecondHandPurch2Tabs[i].DocSecondHandPurch2TabID);
                        db.DocSecondHandPurch2Tabs.Remove(docSecondHandPurch2Tab);
                        await db.SaveChangesAsync();
                    }

                    #endregion


                    #region 3. DocSecondHandPurches *** *** *** *** ***

                    var queryDocSecondHandPurches = await
                        (
                            from x in db.DocSecondHandPurches
                            where x.DocSecondHandPurchID == id
                            select x
                        ).ToListAsync();
                    for (int i = 0; i < queryDocSecondHandPurches.Count(); i++)
                    {
                        Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch1 = await db.DocSecondHandPurches.FindAsync(queryDocSecondHandPurches[i].DocSecondHandPurchID);
                        db.DocSecondHandPurches.Remove(docSecondHandPurch1);
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

        private bool DocSecondHandPurchExists(int id)
        {
            return db.DocSecondHandPurches.Count(e => e.DocSecondHandPurchID == id) > 0;
        }


        internal async Task<DocSecondHandPurch> mPutPostDocSecondHandPurch(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            DocSecondHandPurch docSecondHandPurch,
            Models.Sklad.Doc.DocSecondHandPurch1Tab[] docSecondHandPurch1TabCollection,
            Models.Sklad.Doc.DocSecondHandPurch2Tab[] docSecondHandPurch2TabCollection,
            EntityState entityState, //EntityState.Added, Modified
            int iTypeService,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            //Если БУ, то упустить проверки
            if (!Convert.ToBoolean(docSecondHandPurch.FromService))
            {
                // Если DirSecondHandStatusID > 1, то не сохранять, а выводить сообщение!
                if (iTypeService == 1 && docSecondHandPurch.DirSecondHandStatusID > 1) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg113); }
                // Если DirSecondHandStatusID = 7 и нет ни одной выполненной работы, то не пускать сохранять и выдать эксепшн
                if (iTypeService > 1 && docSecondHandPurch.DirSecondHandStatusID == 7 && docSecondHandPurch1TabCollection.Length == 0) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg114); }
            }




            //Единственный рабочий вариант
            if (iTypeService == 1)
            {
                #region 1


                #region 0. Заполняем DirServiceContractors
                // - не находим - создаём новую
                // - находим - обновляем

                Models.Sklad.Dir.DirServiceContractor dirServiceContractor = new Models.Sklad.Dir.DirServiceContractor();
                //if (!Convert.ToBoolean(docSecondHandPurch.UrgentRepairs))
                {

                    string DirServiceContractorPhone = docSecondHandPurch.DirServiceContractorPhone.Replace("+", "").ToLower();

                    if (!String.IsNullOrEmpty(DirServiceContractorPhone))
                    {
                        var queryDirServiceContractors = await
                            (
                                from x in db.DirServiceContractors
                                where x.DirServiceContractorPhone == DirServiceContractorPhone
                                select x
                            ).ToListAsync();
                        if (queryDirServiceContractors.Count() == 0)
                        {
                            dirServiceContractor = new Models.Sklad.Dir.DirServiceContractor();
                            dirServiceContractor.DirServiceContractorPhone = DirServiceContractorPhone;
                            dirServiceContractor.DirServiceContractorName = docSecondHandPurch.DirServiceContractorName;
                            dirServiceContractor.QuantityOk = 0;
                            dirServiceContractor.QuantityFail = 0;
                            dirServiceContractor.QuantityCount = 0;
                            dirServiceContractor.PassportSeries = docSecondHandPurch.PassportSeries;
                            dirServiceContractor.PassportNumber = docSecondHandPurch.PassportNumber;

                            db.Entry(dirServiceContractor).State = EntityState.Added;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            dirServiceContractor = await db.DirServiceContractors.FindAsync(queryDirServiceContractors[0].DirServiceContractorID);
                            dirServiceContractor.DirServiceContractorName = docSecondHandPurch.DirServiceContractorName;

                            db.Entry(dirServiceContractor).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                    }
                }

                #endregion


                //Сохраняем Шапку, только, если это Приёмка

                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
                //Присваиваем значения
                doc.ListObjectID = ListObjectID;
                doc.IsImport = false;
                doc.NumberInt = docSecondHandPurch.NumberInt;
                doc.NumberReal = docSecondHandPurch.DocSecondHandPurchID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docSecondHandPurch.DirPaymentTypeID;
                doc.Payment = docSecondHandPurch.Payment;
                doc.DirContractorID = docSecondHandPurch.DirContractorIDOrg;
                doc.DirContractorIDOrg = docSecondHandPurch.DirContractorIDOrg;
                doc.Discount = docSecondHandPurch.Discount;
                doc.DirVatValue = docSecondHandPurch.DirVatValue;
                doc.Base = docSecondHandPurch.Base;
                doc.Description = docSecondHandPurch.Description;
                doc.DocDate = docSecondHandPurch.DocDate;
                //doc.DocDisc = docSecondHandPurch.DocDisc;
                doc.Held = false;  //if (UO_Action == "held") doc.Held = false; //else doc.Held = false;
                doc.DocID = docSecondHandPurch.DocID;
                doc.DocIDBase = docSecondHandPurch.DocIDBase;
                doc.KKMSCheckNumber = docSecondHandPurch.KKMSCheckNumber;
                doc.KKMSIdCommand = docSecondHandPurch.KKMSIdCommand;
                doc.KKMSEMail = docSecondHandPurch.KKMSEMail;
                doc.KKMSPhone = docSecondHandPurch.KKMSPhone;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docSecondHandPurch" со всем полями!
                docSecondHandPurch.DocID = doc.DocID;

                #endregion

                #region 2. DocSecondHandPurch *** *** *** *** *** *** *** *** *** ***


                #region Если выбрана хоть одна типовая несисправность - статус "Согласован" (3)

                /*
                if (
                    Convert.ToBoolean(docSecondHandPurch.DirServiceNomenTypicalFaultID1) || Convert.ToBoolean(docSecondHandPurch.DirServiceNomenTypicalFaultID2) || Convert.ToBoolean(docSecondHandPurch.DirServiceNomenTypicalFaultID3) ||
                    Convert.ToBoolean(docSecondHandPurch.DirServiceNomenTypicalFaultID4) || Convert.ToBoolean(docSecondHandPurch.DirServiceNomenTypicalFaultID5) || Convert.ToBoolean(docSecondHandPurch.DirServiceNomenTypicalFaultID4) ||
                    Convert.ToBoolean(docSecondHandPurch.DirServiceNomenTypicalFaultID7)
                  )
                {
                    docSecondHandPurch.DirSecondHandStatusID = 4;
                }
                */

                #endregion


                #region Сохранение

                docSecondHandPurch.DocID = doc.DocID;
                docSecondHandPurch.DirWarehouseIDPurches = docSecondHandPurch.DirWarehouseID;
                //docSecondHandPurch.DirEmployeeIDMaster = field.DirEmployeeID; //Это мастер, его пок анет в форме.
                docSecondHandPurch.ServiceTypeRepair = sysSetting.ServiceTypeRepair;
                docSecondHandPurch.DirServiceContractorID = dirServiceContractor.DirServiceContractorID;
                //Суммы
                if (docSecondHandPurch.Sums == null) docSecondHandPurch.Sums = 0;
                if (docSecondHandPurch.Sums1 == null) docSecondHandPurch.Sums1 = 0;
                if (docSecondHandPurch.Sums2 == null) docSecondHandPurch.Sums2 = 0;
                if (docSecondHandPurch.Sums1Service == null) docSecondHandPurch.Sums1Service = 0;
                if (docSecondHandPurch.Sums2Service == null) docSecondHandPurch.Sums2Service = 0;
                //Цены
                if (docSecondHandPurch.PriceCurrency == null) docSecondHandPurch.PriceCurrency = 0;
                if (docSecondHandPurch.PriceRetailVAT == null) docSecondHandPurch.PriceRetailVAT = 0;
                if (docSecondHandPurch.PriceRetailCurrency == null) docSecondHandPurch.PriceRetailCurrency = 0;
                if (docSecondHandPurch.PriceWholesaleVAT == null) docSecondHandPurch.PriceWholesaleVAT = 0;
                if (docSecondHandPurch.PriceWholesaleCurrency == null) docSecondHandPurch.PriceWholesaleCurrency = 0;
                if (docSecondHandPurch.PriceIMVAT == null) docSecondHandPurch.PriceIMVAT = 0;
                if (docSecondHandPurch.PriceIMCurrency == null) docSecondHandPurch.PriceIMCurrency = 0;


                db.Entry(docSecondHandPurch).State = entityState;
                await db.SaveChangesAsync();

                #endregion


                #region UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docSecondHandPurch.doc.NumberInt == null || docSecondHandPurch.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docSecondHandPurch.DocSecondHandPurchID.ToString();
                    doc.NumberReal = docSecondHandPurch.DocSecondHandPurchID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docSecondHandPurch.DocSecondHandPurchID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. Касса или Банк

                //Только, если сумма больше 0
                if (docSecondHandPurch.PriceVAT > 0 && !Convert.ToBoolean(docSecondHandPurch.FromService))  //if (doc.Payment > 0)
                {
                    //Получаем наименование аппарата
                    Models.Sklad.Dir.DirServiceNomen dirServiceNomen = await db.DirServiceNomens.FindAsync(docSecondHandPurch.DirServiceNomenID);
                    Controllers.Sklad.Dir.DirServiceNomens.DirServiceNomensController dirServiceNomensController = new Dir.DirServiceNomens.DirServiceNomensController();
                    string nomen = await dirServiceNomensController.DirServiceNomenSubFind2(dbRead, docSecondHandPurch.DirServiceNomenID);

                    //Касса
                    if (doc.DirPaymentTypeID == 1)
                    {
                        #region Касса

                        //1. По складу находим привязанную к нему Кассу
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docSecondHandPurch.DirWarehouseID);
                        int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                        //2. Заполняем модель "DocCashOfficeSum"
                        Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                        docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                        docCashOfficeSum.DirCashOfficeSumTypeID = 21;
                        docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                        docCashOfficeSum.DocID = doc.DocID;
                        docCashOfficeSum.DocXID = docSecondHandPurch.DocSecondHandPurchID;
                        docCashOfficeSum.DocCashOfficeSumSum = docSecondHandPurch.PriceVAT; //doc.Payment;
                        docCashOfficeSum.Description = "";
                        docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;
                        docCashOfficeSum.Base = "Покупка Б/У аппарата: " + nomen; //dirServiceNomen.DirServiceNomenName;
                        docCashOfficeSum.KKMSCheckNumber = docSecondHandPurch.KKMSCheckNumber;
                        docCashOfficeSum.KKMSIdCommand = docSecondHandPurch.KKMSIdCommand;
                        docCashOfficeSum.KKMSEMail = docSecondHandPurch.KKMSEMail;
                        docCashOfficeSum.KKMSPhone = docSecondHandPurch.KKMSPhone;
                        docCashOfficeSum.Discount = doc.Discount;

                        //3. Пишем в Кассу
                        Doc.DocCashOfficeSums.DocCashOfficeSumsController docCashOfficeSumsController = new Doc.DocCashOfficeSums.DocCashOfficeSumsController();
                        docCashOfficeSum = await Task.Run(() => docCashOfficeSumsController.mPutPostDocCashOfficeSum(db, docCashOfficeSum, EntityState.Added));

                        #endregion
                    }
                    //Банк
                    else if (doc.DirPaymentTypeID == 2)
                    {
                        #region Банк

                        //1. По складу находим привязанную к нему Кассу
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docSecondHandPurch.DirWarehouseID);
                        int iDirBankID = dirWarehouse.DirBankID;

                        //2. Заполняем модель "DocBankSum"
                        Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                        docBankSum.DirBankID = iDirBankID;
                        docBankSum.DirBankSumTypeID = 19; //Изъятие из кассы на основании проведения приходной накладной №
                        docBankSum.DocBankSumDate = DateTime.Now;
                        docBankSum.DocID = doc.DocID;
                        docBankSum.DocXID = docSecondHandPurch.DocSecondHandPurchID;
                        docBankSum.DocBankSumSum = docSecondHandPurch.PriceVAT; //doc.Payment;
                        docBankSum.Description = "";
                        docBankSum.DirEmployeeID = field.DirEmployeeID;
                        docBankSum.Base = "Покупка Б/У аппарата: " + dirServiceNomen.DirServiceNomenName;
                        docBankSum.KKMSCheckNumber = docSecondHandPurch.KKMSCheckNumber;
                        docBankSum.KKMSIdCommand = docSecondHandPurch.KKMSIdCommand;
                        docBankSum.KKMSEMail = docSecondHandPurch.KKMSEMail;
                        docBankSum.KKMSPhone = docSecondHandPurch.KKMSPhone;
                        docBankSum.Discount = doc.Discount;

                        //3. Пишем в Банк
                        Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                        docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum(db, docBankSum, EntityState.Added));

                        #endregion
                    }
                    else
                    {
                        throw new System.InvalidOperationException("Не выбран метод оплаты: Касса или Банк!");
                    }
                }

                #endregion

                #region 4. Log

                logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                logService.DirSecondHandLogTypeID = 1;
                logService.DirEmployeeID = field.DirEmployeeID;
                logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                logService.DirWarehouseIDFrom = docSecondHandPurch.DirWarehouseID;

                //if (entityState == EntityState.Added) { logService.Msg = "Аппарат принят на точку №" + docSecondHandPurch.dirWarehouse.DirWarehouseName; }
                if (entityState == EntityState.Added)
                {

                    string DirWarehouseName = "";
                    if (docSecondHandPurch.dirWarehouse != null)
                    {
                        DirWarehouseName = docSecondHandPurch.dirWarehouse.DirWarehouseName;
                    }
                    else
                    {
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandPurch.DirWarehouseID);
                        DirWarehouseName = dirWarehouse.DirWarehouseName;
                    }

                    logService.Msg = "Аппарат принят на точку №" + DirWarehouseName;
                }

                await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                #endregion

                #region 5. Если заполненно поле "ComponentOtherText", то ищим похожую запись в таблице "DirServiceComplects.DirServiceComplectName"
                // - не находим - создаём новую
                // - находим - ничего не делаем

                if (!String.IsNullOrEmpty(docSecondHandPurch.ComponentOtherText))
                {
                    var queryDirServiceComplects = await
                        (
                            from x in db.DirServiceComplects
                            where x.DirServiceComplectName.ToLower() == docSecondHandPurch.ComponentOtherText.ToLower()
                            select x
                        ).ToListAsync();
                    if (queryDirServiceComplects.Count() == 0)
                    {
                        Models.Sklad.Dir.DirServiceComplect dirServiceComplect = new Models.Sklad.Dir.DirServiceComplect();
                        dirServiceComplect.DirServiceComplectName = docSecondHandPurch.ComponentOtherText;

                        db.Entry(dirServiceComplect).State = EntityState.Added;
                        await db.SaveChangesAsync();
                    }
                }

                #endregion

                #endregion
            }



            #region n. Подтверждение транзакции - НЕ используется

            //ts.Commit(); //.Complete();

            #endregion


            return docSecondHandPurch;
        }


        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            //System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch,
            //int id,
            int DirSecondHandStatusID,
            int DirPaymentTypeID,
            double SumTotal2a,
            string sReturnRresults,
            int locDirSecondHandStatusI_OLD,

            Classes.Account.Login.Field field, //Дополнительные данные о сотруднике

            //Не нужно
            int? KKMSCheckNumber,
            string KKMSIdCommand
            )
        {
            //Не трогать!!!
            //docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(id);
            int DocSecondHandPurchID = Convert.ToInt32(docSecondHandPurch.DocSecondHandPurchID);

            DateTime dtNow = DateTime.Now;

            #region 0. Проверка, если предыдущий статус такой же на который меняем, то не писать в Лог

            //1. Если статус Продан(10) и Разобран(11), то відать исключение
            if (docSecondHandPurch.DirSecondHandStatusID == 10)
            {
                throw new System.InvalidOperationException("Аппарат уже продан!");
            }
            else if (docSecondHandPurch.DirSecondHandStatusID == 11)
            {
                throw new System.InvalidOperationException("Аппарат уже разобран!");
            }


            //2. Исключение, т.к. если в Логе нет записей с сменой статуса получим Ошибку из-за "FirstAsync()"
            try
            {
                var query = await
                    (
                        from x in db.LogSecondHands
                        where x.DocSecondHandPurchID == DocSecondHandPurchID && x.DirSecondHandStatusID != null
                        select new
                        {
                            LogSecondHandID = x.LogSecondHandID,
                            DirSecondHandStatusID = x.DirSecondHandStatusID
                        }
                    ).OrderByDescending(x => x.LogSecondHandID).FirstAsync();

                if (
                    docSecondHandPurch.DirSecondHandStatusID == DirSecondHandStatusID && 
                    query.DirSecondHandStatusID == DirSecondHandStatusID
                   )
                {
                    return false;
                }
            }
            catch (Exception ex) { }



            #region Заказы 
            //Если в списке запчастей есть не заказы, то выдать сообщение

            if (DirSecondHandStatusID > 6)
            {

                //1.Получаем Docs.DocIDBase
                int? _DocID = 0;
                var queryDocIDBase = await
                    (
                        from x in db.DocSecondHandPurches
                        where x.DocSecondHandPurchID == docSecondHandPurch.DocSecondHandPurchID
                        select x
                    ).ToListAsync();
                if (queryDocIDBase.Count() > 0)
                {
                    _DocID = queryDocIDBase[0].doc.DocID;
                }

                //2. Получаем к-во заказов для ремонта
                int queryDocSecondHandPurch2Tabs = await
                    (
                        from x in db.DocOrderInts
                        where x.doc.DocIDBase == _DocID && x.DirOrderIntStatusID < 4
                        select x
                    ).CountAsync();


                if (queryDocSecondHandPurch2Tabs > 0)
                {
                    throw new System.InvalidOperationException("Внимание!!!<br />В списке запчастей имеет заказ(ы)! Смена статуса не возможна!");
                }

            }

            #endregion


            #endregion


            #region 1. Сохранение статуса в БД

            //Сохраняем старый статус, ниже - нужен
            int? DirSecondHandStatusID_OLD = docSecondHandPurch.DirSecondHandStatusID;

            //Если Статус == 9 (Выдан), то менять "DateDone" на текущую
            if (DirSecondHandStatusID == 9)
            {
                docSecondHandPurch.DateDone = dtNow;
                docSecondHandPurch.Summ_NotPre = SumTotal2a;
            }

            //Сохранить статус аппарата: Готов (7) или Отказ (8)
            if (DirSecondHandStatusID == 7 || DirSecondHandStatusID == 8) // || DirSecondHandStatusID == 9
            {
                docSecondHandPurch.DirSecondHandStatusID_789 = DirSecondHandStatusID; //Статус аппарата сохранить: Готов или Отказ
            }

            //Если был Статус == 9 (Выдан) и сменили на "В диагностике", то менять "DateDone" на текущую + 7 дней (из настроек)
            bool bDirSecondHandLogTypeID9 = false;
            if (docSecondHandPurch.DirSecondHandStatusID == 9 && DirSecondHandStatusID == 2)
            {
                //1. Проверяем есть ли ещё Гарантия
                if (docSecondHandPurch.DateDone.AddMonths(docSecondHandPurch.ServiceTypeRepair) <= dtNow && sysSetting.WarrantyPeriodPassed)
                {
                    //Исключение
                    throw new System.InvalidOperationException("Срок гарантии прошёл (до " + docSecondHandPurch.DateDone.AddMonths(docSecondHandPurch.ServiceTypeRepair).ToString("yyyy-MM-dd") + ")!");
                }

                //2.1. Меняем дату "Готовности"
                docSecondHandPurch.DateDone = dtNow.AddDays(sysSetting.ReadinessDay);

                //2.2 Запоминает первичную дату документа и меняем дату документа на текущую.
                //2.2.1.
                docSecondHandPurch.DocDate_First = docSecondHandPurch.doc.DocDate;
                //2.2.2.
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandPurch.DocID);
                doc.DocDate = dtNow;
                db.Entry(doc).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());

                //4. Сообщение для Лога: 
                bDirSecondHandLogTypeID9 = true;
            }

            //Если статус: 7 или 8, то заполняем дату "IssuanceDate"
            //!!! СУКА !!! Сдесь ОШИБКА !!! БЛЯДЬ !!!
            //if (docSecondHandPurch.DirSecondHandStatusID == 7 || docSecondHandPurch.DirSecondHandStatusID == 8) docSecondHandPurch.IssuanceDate = dtNow;
            if (DirSecondHandStatusID == 7 || DirSecondHandStatusID == 8) docSecondHandPurch.IssuanceDate = dtNow;


            //!!! ВАЖНО !!!
            //Цены аппарата
            if (
                DirSecondHandStatusID_OLD == 7 && docSecondHandPurch.DirSecondHandStatusID == 9 &&
                docSecondHandPurch.PriceRetailVAT != null && docSecondHandPurch.PriceWholesaleVAT != null && docSecondHandPurch.PriceIMVAT != null
              )
            {
                docSecondHandPurch.PriceRetailCurrency = docSecondHandPurch.PriceRetailVAT;
                docSecondHandPurch.PriceWholesaleVAT = docSecondHandPurch.PriceWholesaleCurrency;
                docSecondHandPurch.PriceIMVAT = docSecondHandPurch.PriceIMCurrency;
            }


            //Дата смены статуса
            docSecondHandPurch.DateStatusChange = dtNow;


            //!!! Важно !!!
            //Если перемещаем аппарат "На разбор", то сменить статус на "12"
            if (DirSecondHandStatusID_OLD == 8 && DirSecondHandStatusID == 9)
            {
                DirSecondHandStatusID = 12;
            }


            docSecondHandPurch.DirSecondHandStatusID = DirSecondHandStatusID;

            db.Entry(docSecondHandPurch).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 2. DirSecondHandStatusID == 9: DocSecondHandPurch1Tab, DocSecondHandPurch2Tab
            //Ну и надо Работы и запчасти пометить как оплоченные!
            //Но, только новые. То есть аппарат могут вернуть несколько раз надоработку.

            if (DirSecondHandStatusID == 9)
            {
                //DocSecondHandPurch1Tab === === === === === === === === === === ===
                List<Models.Sklad.Doc.DocSecondHandPurch1Tab> listDocSecondHandPurch1Tab =
                    (
                        from x in db.DocSecondHandPurch1Tabs
                        where x.DocSecondHandPurchID == DocSecondHandPurchID && x.PayDate == null
                        select x
                    ).ToList();

                foreach (Models.Sklad.Doc.DocSecondHandPurch1Tab docSecondHandPurch1Tab in listDocSecondHandPurch1Tab)
                {
                    docSecondHandPurch1Tab.PayDate = dtNow;
                    db.Entry(docSecondHandPurch1Tab).State = EntityState.Modified;
                }

                //DocSecondHandPurch2Tab === === === === === === === === === === ===
                List<Models.Sklad.Doc.DocSecondHandPurch2Tab> listDocSecondHandPurch2Tab =
                    (
                        from x in db.DocSecondHandPurch2Tabs
                        where x.DocSecondHandPurchID == DocSecondHandPurchID && x.PayDate == null
                        select x
                    ).ToList();

                foreach (Models.Sklad.Doc.DocSecondHandPurch2Tab docSecondHandPurch2Tab in listDocSecondHandPurch2Tab)
                {
                    docSecondHandPurch2Tab.PayDate = dtNow;
                    db.Entry(docSecondHandPurch2Tab).State = EntityState.Modified;
                }

                //Сохраняем
                await Task.Run(() => db.SaveChangesAsync());
            }

            #endregion


            #region 3. Лог: Пишем в Лог о смене статуса и мастера, если такое было
            
            logService.DocSecondHandPurchID = DocSecondHandPurchID;
            if (!bDirSecondHandLogTypeID9) logService.DirSecondHandLogTypeID = 1; //Смена статуса
            else if(DirSecondHandStatusID == 12) logService.DirSecondHandLogTypeID = 17; //Перемещён "На разбор"
            else logService.DirSecondHandLogTypeID = 9; //Возврат по гарантии
            logService.DirEmployeeID = field.DirEmployeeID;
            logService.DirSecondHandStatusID = DirSecondHandStatusID;
            if (!String.IsNullOrEmpty(sReturnRresults)) logService.Msg = sReturnRresults;

            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);
            
            #endregion


            #region 4. Заполняем DirServiceContractors
            //Надо ввести доп.поле статуса в "DocSecondHandPurches" с предыдущим статусом: "Готов" или "Отказ"
            //Когда нажимаем выдан, то заполнять это поле.
            //Нужно для:
            // - Справочника "DirServiceContractor" поля: QuantityOk и QuantityFail
            // - Для статистики сколько Готовых, сколько Отказных


            //Если в Логе НЕТ записей, что вернут на доработку
            var queryLogCount = await
                (
                    from x in db.LogSecondHands
                    where x.DirSecondHandLogTypeID == 9 && x.DocSecondHandPurchID == DocSecondHandPurchID
                    select x
                ).CountAsync();


            //1. Находим Клиента по 
            if (
                queryLogCount == 0 &&
                DirSecondHandStatusID == 9 &&
                docSecondHandPurch.DirServiceContractorID != null &&
                docSecondHandPurch.DirServiceContractorID > 0
               )
            {
                Models.Sklad.Dir.DirServiceContractor dirServiceContractor = await db.DirServiceContractors.FindAsync(docSecondHandPurch.DirServiceContractorID);

                //2. К-во (3 шт)
                if (DirSecondHandStatusID_OLD == 7)
                {
                    //Выдан
                    dirServiceContractor.QuantityOk = dirServiceContractor.QuantityOk + 1;
                }
                else if (DirSecondHandStatusID_OLD == 8)
                {
                    //Отказ
                    dirServiceContractor.QuantityFail = dirServiceContractor.QuantityFail + 1;
                }
                dirServiceContractor.QuantityCount = dirServiceContractor.QuantityCount + 1;

                //3. Сохранение
                db.Entry(dirServiceContractor).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());
            }

            #endregion


            return true;
        }


        internal async Task<bool> mRepairChange(
            DbConnectionSklad db,
            System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch,
            int id,
            int ServiceTypeRepair,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region 1. Сохранение статуса в БД

            docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(id);

            if (docSecondHandPurch.ServiceTypeRepair == ServiceTypeRepair) { return false; }
            else { logService.Msg = "Была: " + docSecondHandPurch.ServiceTypeRepair + " поменяли на: " + ServiceTypeRepair; }

            docSecondHandPurch.ServiceTypeRepair = ServiceTypeRepair;
            db.Entry(docSecondHandPurch).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 3. Лог

            logService.DocSecondHandPurchID = id;
            logService.DirSecondHandLogTypeID = 8;
            logService.DirEmployeeID = field.DirEmployeeID;
            //logService.Msg = "Была гарантия: "; //Выше изменили!!!

            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

            #endregion


            return true;
        }


        internal async Task<string[]> mPatchFull(DbConnectionSklad db, int id)
        {
            ArrayList alNameSpase = new ArrayList();
            ArrayList alNameSpaseNo = new ArrayList();
            ArrayList alNameID = new ArrayList();
            //ArrayList alNameID_DirNomen = new ArrayList();

            int iCount = 0;
            int? Sub = id;
            while (Sub > 0)
            {
                iCount++;
                var query = await Task.Run(() =>
                     (
                         from x in db.DirServiceNomens
                         where x.DirServiceNomenID == Sub
                         select new
                         {
                             id = x.DirServiceNomenID,
                             sub = x.Sub,
                             text = x.DirServiceNomenName, // + " (" + x.DirServiceNomenName + ")",
                             leaf =
                             (
                              from y in db.DirServiceNomens
                              where y.Sub == x.DirServiceNomenID
                              select y
                             ).Count() == 0 ? 1 : 0,

                             Del = x.Del,
                             Sub = x.Sub,

                             //Полный путь от группы к выбраному элементу
                             DirServiceNomenPatchFull = x.DirServiceNomenName // + " (" + x.DirServiceNomenName + ")"
                         }
                    ).ToListAsync());

                if (query.Count() > 0)
                {
                    id = Convert.ToInt32(query[0].id);
                    Sub = query[0].Sub;
                    alNameSpase.Add(query[0].text + " / ");
                    alNameSpaseNo.Add(query[0].text + ",");
                    alNameID.Add(query[0].id + ",");

                    /*
                    //Получаем данные из справочника "DirNomens"
                    string text = query[0].text;
                    var queryDirNomen =
                    (
                        from x in db.DirNomens
                        where x.DirNomenName.Contains(text) //&& x.Sub == 0
                        select x
                    );

                    //queryDirNomen = queryDirNomen.Where(x => x.Sub == 0);

                    var queryDirNomen1 = await queryDirNomen.ToListAsync();
                    if (queryDirNomen1.Count() > 0)
                    {
                        alNameID_DirNomen.Add(queryDirNomen1[0].DirNomenID + ",");
                    }
                    else
                    {
                        alNameID_DirNomen.Add("");
                    }
                    */
                }
                else
                {
                    Sub = null;
                }
            }

            string[] ret = new string[4];
            for (int i = alNameSpase.Count - 1; i >= 0; i--)
            {
                ret[0] += alNameSpase[i].ToString();
                ret[1] += alNameSpaseNo[i].ToString();
                ret[2] += alNameID[i].ToString();
                //ret[3] += alNameID_DirNomen[i].ToString();
            }

            return ret;

        }

        internal async Task<int[]> mDirNomenID(DbConnectionSklad db, string ID0, string ID1, string ID2)
        {
            int[] ret = new int[3];

            //Получаем данные из справочника "DirNomens"
            //1. === === ===
            var queryDirNomen1 = await
            (
                from x in db.DirNomens
                where x.DirNomenName.Contains(ID1) && x.Sub == null
                select x
            ).ToListAsync();
            if (queryDirNomen1.Count() > 0)
            {
                ret[0] = Convert.ToInt32(queryDirNomen1[0].DirNomenID);
            }
            //2. === === ===
            int Sub = ret[0];
            var queryDirNomen2 = await
            (
                from x in db.DirNomens
                where x.DirNomenName.Contains(ID2) && x.Sub == Sub
                select x
            ).ToListAsync();
            if (queryDirNomen2.Count() > 0)
            {
                ret[1] = Convert.ToInt32(queryDirNomen2[0].DirNomenID);
            }

            return ret;
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
                " [DocSecondHandPurches].[DocSecondHandPurchID] AS [DocSecondHandPurchID], [Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], [Docs].[Base] AS [Base],  [Docs].[Held] AS [Held], [Docs].[Discount] AS [Discount], [Docs].[Description] AS [Description], [Docs].[DirVatValue] AS [DirVatValue],  [DocSecondHandPurches].[SerialNumber] AS [DeviceSerialNumber], " +
                " CASE [DocSecondHandPurches].[ServiceTypeRepair]  WHEN [ServiceTypeRepair] = 1 THEN 'Не гарантийный' ELSE 'Гарантийный' END AS [ServiceTypeRepair], " +
                //" CASE [DocSecondHandPurches].[ComponentDevice]  WHEN [ComponentDevice] = 1 THEN 'Аппарат' ELSE '-' END AS [ComponentDevice], " +
                //" CASE [DocSecondHandPurches].[ComponentBattery]  WHEN [ComponentBattery] = 1 THEN 'Аккумулятор' ELSE '-' END AS [ComponentBattery],[DocSecondHandPurches].[ComponentBatterySerial] AS [ComponentBatterySerial], " +
                //" CASE [DocSecondHandPurches].[ComponentBackCover]  WHEN [ComponentBackCover] = 1 THEN 'Задняя крышка' ELSE '-' END AS [ComponentBackCover], " + 
                " [DocSecondHandPurches].[ComponentPasTextNo] AS [ComponentPasTextNo], [DocSecondHandPurches].[ComponentPasText] AS [ComponentPasText], [DocSecondHandPurches].[ComponentOtherText] AS [ComponentOtherText], [DocSecondHandPurches].[ProblemClientWords] AS [ProblemClientWords], [DocSecondHandPurches].[Note] AS [Note], [DocSecondHandPurches].[DirServiceContractorName] AS [DirContractorName], [DocSecondHandPurches].[DirServiceContractorAddress] AS [DirContractorAddress], [DocSecondHandPurches].[DirServiceContractorPhone] AS [DirContractorPhone], [DocSecondHandPurches].[DirServiceContractorEmail] AS [DirContractorEmail], [DocSecondHandPurches].[DirServiceContractorRegular] AS [DirServiceContractorRegular], [DocSecondHandPurches].[PassportSeries] AS [PassportSeries], [DocSecondHandPurches].[PassportNumber] AS [PassportNumber], [DocSecondHandPurches].[PriceVAT] AS [PriceVATEstimated], [DocSecondHandPurches].[PriceVAT] AS [SumTotal_InWords], [DocSecondHandPurches].[DateDone] AS [DateDone], " + 
                //" [DocSecondHandPurches].[UrgentRepairs] AS [UrgentRepairs], [DocSecondHandPurches].[Prepayment] AS [Prepayment], [DocSecondHandPurches].[PrepaymentSum] AS [PrepaymentSum], " +
                " [DirServiceNomens].[DirServiceNomenID] AS [DirServiceNomenID], " +
                " [DocSecondHandPurches].[ServiceTypeRepair], " +



                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "CASE " +
                "WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN [dirServiceNomensSubGroup].[DirServiceNomenName] " +

                "WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END ELSE " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN '' ELSE [DirServiceNomens].[DirServiceNomenName] END END AS [DirServiceNomenName], " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                " [DirServiceNomens].[DirServiceNomenNameFull] AS [DirServiceNomenNameFull], [DirServiceNomens].[Description] AS [Description], [DirServiceNomens].[DescriptionFull] AS [DescriptionFull], " +

                " [DirSecondHandStatuses].[DirSecondHandStatusName] AS [DirSecondHandStatusName], [DirBanksOrg].[DirBankName] AS [DirBankNameOrg], [DirBanksOrg].[DirBankMFO] AS [DirBankMFOOrg], [DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], [DirContractorOrg].[DirContractorEmail] AS [DirContractorEmailOrg], [DirContractorOrg].[DirContractorWWW] AS [DirContractorWWWOrg], [DirContractorOrg].[DirContractorAddress] AS [DirContractorAddressOrg], [DirContractorOrg].[DirContractorLegalCertificateDate] AS [DirContractorLegalCertificateDateOrg], [DirContractorOrg].[DirContractorLegalTIN] AS [DirContractorLegalTINOrg], [DirContractorOrg].[DirContractorLegalCAT] AS [DirContractorLegalCATOrg], [DirContractorOrg].[DirContractorLegalCertificateNumber] AS [DirContractorLegalCertificateNumberOrg], [DirContractorOrg].[DirContractorLegalBIN] AS [DirContractorLegalBINOrg], [DirContractorOrg].[DirContractorLegalOGRNIP] AS [DirContractorLegalOGRNIPOrg], [DirContractorOrg].[DirContractorLegalRNNBO] AS [DirContractorLegalRNNBOOrg], [DirContractorOrg].[DirContractorDesc] AS [DirContractorDescOrg],  [DirContractorOrg].[DirContractorLegalPasIssued] AS [DirContractorLegalPasIssuedOrg],  [DirContractorOrg].[DirContractorLegalPasDate] AS [DirContractorLegalPasDateOrg],  [DirContractorOrg].[DirContractorLegalPasCode] AS [DirContractorLegalPasCodeOrg],  [DirContractorOrg].[DirContractorLegalPasNumber] AS [DirContractorLegalPasNumberOrg],  [DirContractorOrg].[DirContractorLegalPasSeries] AS [DirContractorLegalPasSeriesOrg],  [DirContractorOrg].[DirContractorDiscount] AS [DirContractorDiscountOrg],  [DirContractorOrg].[DirContractorPhone] AS [DirContractorPhoneOrg],  [DirContractorOrg].[DirContractorFax] AS [DirContractorFaxOrg],  [DirContractorOrg].[DirContractorLegalAddress] AS [DirContractorLegalAddressOrg],  [DirContractorOrg].[DirContractorLegalName] AS [DirContractorLegalNameOrg], [DirWarehouses].[DirWarehouseName] AS [DirWarehouseName],  [DirWarehouses].[DirWarehouseAddress] AS [DirWarehouseAddress],  [DirWarehouses].[DirWarehouseDesc] AS [DirWarehouseDesc], [DirEmployees].[DirEmployeeName] AS [DirEmployeeName], [Docs].[NumberInt] AS [NumberInt] " +
                "FROM [DocSecondHandPurches]  " +
                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocSecondHandPurches].[DocID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocSecondHandPurches].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [dirServiceNomensSubGroup].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "INNER JOIN [DirSecondHandStatuses] ON [DirSecondHandStatuses].[DirSecondHandStatusID] = [DocSecondHandPurches].[DirSecondHandStatusID] INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocSecondHandPurches].[DirWarehouseID] INNER JOIN [DirEmployees] ON [DirEmployees].[DirEmployeeID] = [Docs].[DirEmployeeID] LEFT JOIN [DirBanks] AS [DirBanksOrg] ON [DirBanksOrg].[DirBankID] = [DirContractorOrg].[DirBankID] " +
                "WHERE (Docs.DocID=@DocID) ";



            return SQL;
        }


        //Сумма документа
        internal string GenerateSQLSUM(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL =

                "SELECT " +

                "[DocDate] AS [DocDate], [DocDate] AS [DocDate_InWords], " +
                "Discount AS Discount, " +

                " SUM(CountRecord1 + CountRecord2) AS CountRecord, " +
                " SUM(CountRecord1 + CountRecord2) AS CountRecord_NumInWords, " +

                " SUM(SumDocSecondHandPurch1Tabs) AS SumDocSecondHandPurch1Tabs, " +
                " SUM(SumDocSecondHandPurch1Tabs) AS SumDocSecondHandPurch1Tabs_InWords, " +

                " SUM(SumDocSecondHandPurch2Tabs) AS SumDocSecondHandPurch2Tabs, " +
                " SUM(SumDocSecondHandPurch2Tabs) AS SumDocSecondHandPurch2Tabs_InWords, " +

                " SUM(SumDocSecondHandPurch1Tabs) + SUM(SumDocSecondHandPurch2Tabs) AS SumTotal, " +
                " SUM(SumDocSecondHandPurch1Tabs) + SUM(SumDocSecondHandPurch2Tabs) AS SumTotal_InWords, " +

                " [PrepaymentSum] AS PrepaymentSum, " +

                " SUM(SumDocSecondHandPurch1Tabs) + SUM(SumDocSecondHandPurch2Tabs) - [PrepaymentSum] AS SumTotal2, " +

                "[DirContractorName] AS [DirContractorName], " +
                "[DirContractorAddress] AS [DirContractorAddress], " +
                "[DirContractorPhone] AS [DirContractorPhone], " +
                "[DirContractorEmail] AS [DirContractorEmail], " +
                "[DirServiceContractorRegular] AS [DirServiceContractorRegular], " + //Постоянный
                "[PassportSeries] AS [PassportSeries], [PassportNumber] AS [PassportNumber], " + 
                "[ServiceTypeRepair] " +


                "FROM " +
                "(" +

                "SELECT " +
                "[Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], " +
                "Docs.Discount AS Discount, " +

                "COUNT(*) CountRecord1, " +
                "0 CountRecord2, " +

                //1. Подсчет табличной части Работы "SumDocSecondHandPurch1Tabs"
                "ROUND((SUM(DocSecondHandPurch1Tabs.PriceCurrency)), " + sysSettings.FractionalPartInSum + ") AS SumDocSecondHandPurch1Tabs, " +
                "0 AS SumDocSecondHandPurch2Tabs, " +

                //4. Константа "PrepaymentSum"
                "[DocSecondHandPurches].[PrepaymentSum] AS [PrepaymentSum], " +

                "[DocSecondHandPurches].[DirServiceContractorName] AS [DirContractorName], " +
                "[DocSecondHandPurches].[DirServiceContractorAddress] AS [DirContractorAddress], " +
                "[DocSecondHandPurches].[DirServiceContractorPhone] AS [DirContractorPhone], " +
                "[DocSecondHandPurches].[DirServiceContractorEmail] AS [DirContractorEmail], " +
                "[DocSecondHandPurches].[DirServiceContractorRegular] AS [DirServiceContractorRegular], " + //Постоянный
                "[DocSecondHandPurches].[PassportSeries] AS [PassportSeries], [DocSecondHandPurches].[PassportNumber] AS [PassportNumber], " + 
                "CASE WHEN ([DocSecondHandPurches].[ServiceTypeRepair] IS NULL) THEN 1 ELSE [DocSecondHandPurches].[ServiceTypeRepair] END AS [ServiceTypeRepair] " +


                "FROM Docs, DocSecondHandPurches " +
                " LEFT JOIN DocSecondHandPurch1Tabs ON (DocSecondHandPurch1Tabs.DocSecondHandPurchID=DocSecondHandPurches.DocSecondHandPurchID) " +

                "WHERE (Docs.DocID=DocSecondHandPurches.DocID)and(Docs.DocID=@DocID) " +



                " UNION " +



                "SELECT " +
                "[Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], " +
                "Docs.Discount AS Discount, " +

                "0 CountRecord1, " +
                "COUNT(*) CountRecord2, " +

                //1. Подсчет табличной части Работы "SumDocSecondHandPurch2Tabs"
                "0 AS SumDocSecondHandPurch1Tabs, " +
                "ROUND((SUM(DocSecondHandPurch2Tabs.PriceCurrency)), " + sysSettings.FractionalPartInSum + ") AS SumDocSecondHandPurch2Tabs, " +

                //4. Константа "PrepaymentSum"
                "[DocSecondHandPurches].[PrepaymentSum] AS [PrepaymentSum], " +

                "[DocSecondHandPurches].[DirServiceContractorName] AS [DirContractorName], " +
                "[DocSecondHandPurches].[DirServiceContractorAddress] AS [DirContractorAddress], " +
                "[DocSecondHandPurches].[DirServiceContractorPhone] AS [DirContractorPhone], " +
                "[DocSecondHandPurches].[DirServiceContractorEmail] AS [DirContractorEmail], " +
                "[DocSecondHandPurches].[DirServiceContractorRegular] AS [DirServiceContractorRegular], " + //Постоянный
                "[DocSecondHandPurches].[PassportSeries] AS [PassportSeries], [DocSecondHandPurches].[PassportNumber] AS [PassportNumber], " + 
                "CASE WHEN ([DocSecondHandPurches].[ServiceTypeRepair] IS NULL) THEN 1 ELSE [DocSecondHandPurches].[ServiceTypeRepair] END AS [ServiceTypeRepair] " +


                "FROM Docs, DocSecondHandPurches " +
                " LEFT JOIN DocSecondHandPurch2Tabs ON (DocSecondHandPurch2Tabs.DocSecondHandPurchID=DocSecondHandPurches.DocSecondHandPurchID) " +
                "WHERE (Docs.DocID=DocSecondHandPurches.DocID)and(Docs.DocID=@DocID) " +

                ")";




            return SQL;
        }


        #endregion
    }
}