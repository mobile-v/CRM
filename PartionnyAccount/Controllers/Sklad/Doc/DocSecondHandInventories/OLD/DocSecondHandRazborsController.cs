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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandInventories
{
    public class DocSecondHandRazborsController : ApiController
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
        Models.Sklad.Log.LogSecondHandRazbor logService = new Models.Sklad.Log.LogSecondHandRazbor(); Controllers.Sklad.Log.LogSecondHandRazborsController logServicesController = new Log.LogSecondHandRazborsController();
        PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurchesController docSecondHandPurchesController = new DocSecondHandPurches.DocSecondHandPurchesController();
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
            public int? DirSecondHandStatusIDS;
            public int? DirSecondHandStatusIDPo;
            public int iTypeService; //1 - Приёмка, 2 - Мастерская, 3 - Выдача
            public int? DirEmployeeID;

            //Отобразить "Архив"
            public int? DocSecondHandRazborID;
        }
        // GET: api/DocSecondHandRazbors
        public async Task<IHttpActionResult> GetDocSecondHandRazbors(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRazbors"));
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
                _params.DirEmployeeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value);

                //Склад - надо найди Ыги-склад (Разбор)
                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);
                var queryDirWarehouseID = await
                    (
                        from x in db.DirWarehouses
                        where x.DirWarehouseLoc == 5 && x.Sub == _params.DirWarehouseID
                        select x.DirWarehouseID
                    ).ToListAsync();
                if (queryDirWarehouseID.Count() > 0)
                {
                    _params.DirWarehouseID = queryDirWarehouseID[0].Value;
                }


                //Архив
                _params.DocSecondHandRazborID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandRazborID", true) == 0).Value);

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
                        from x in db.DocSecondHandRazbors

                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        //x.rem2Party.DocIDFirst,
                        //join docSecondHandPurches1 in db.DocSecondHandPurches on x.rem2Party.DocIDFirst equals docSecondHandPurches1.DocID into docSecondHandPurches2
                        //from docSecondHandPurches in docSecondHandPurches2 //.DefaultIfEmpty()

                        select new
                        {
                            //DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,

                            //ListObjects.ListObjectID - тип документа: БУ(65) или СЦ(40)
                            //ListObjectNameRu = x.listObjectIDFromType.ListObjectNameRu,

                            //Docs.DocID - по DocID можно вычислить ID-шник документа 'Docs.NumberReal'
                            //DocSecondHandPurchID = x.docIDFromType.NumberReal,


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

                            DocSecondHandRazborID = x.DocSecondHandRazborID,
                            DirContractorName = x.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = x.doc.dirContractorOrg.DirContractorID,
                            //DirContractorNameOrg = x.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = x.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,
                            DirSecondHandStatusID = x.DirSecondHandStatusID,
                            Status = x.DirSecondHandStatusID,
                            DirSecondHandStatusName = x.dirSecondHandStatus.DirSecondHandStatusName,

                            PriceVAT = x.PriceVAT,
                            PriceCurrency = x.PriceCurrency,

                            //Мастер
                            DirEmployeeIDMaster = x.DirEmployeeIDMaster,

                            //Этой суммы нет, но может её включить ...
                            //Sums = x.Sums,

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


                #region Фильтр FilterType

                if (_params.FilterType > 0)
                {
                    query = query.Where(x => x.DirSecondHandStatusID == _params.FilterType);
                }

                #endregion

                #region Фильтр DirSecondHandStatusID S и Po

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
                        //query = query.Where(x => x.DocSecondHandPurchID == iNumber32);
                    }
                }

                #endregion


                #region OrderBy и Лимит

                //query = query.OrderByDescending(x => x.DocDate).Skip(_params.Skip).Take(_params.limit);
                query = query.OrderByDescending(x => x.DocSecondHandRazborID); //.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocSecondHandRazbors.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocSecondHandRazbor = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandRazbors/5
        [ResponseType(typeof(DocSecondHandRazbor))]
        public async Task<IHttpActionResult> GetDocSecondHandRazbor(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRazbors"));
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
                        from docSecondHandRazbors in db.DocSecondHandRazbors
                        where docSecondHandRazbors.DocSecondHandRazborID == id
                        select docSecondHandRazbors
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON


                #region Полный путь Аппарата

                //1. Получаем Sub аппарата по "DocSecondHandRazborID" (id)
                /*
                string DirServiceNomenPatchFull = null;
                var querySub = await Task.Run(() =>
                     (
                        from x in db.DocSecondHandRazbors
                        where x.DocSecondHandRazborID == id
                        select new
                        {
                            Sub = x.dirServiceNomen.Sub
                        }
                    ).ToArrayAsync());

                if (querySub.Count() > 0)
                {
                    int? iSub = querySub[0].Sub;

                    Controllers.Sklad.Dir.DirServiceNomens.DirServiceNomensController dirServiceNomensController = new Dir.DirServiceNomens.DirServiceNomensController();
                    DirServiceNomenPatchFull = await Task.Run(() => dirServiceNomensController.DirServiceNomenSubFind2(db, iSub));
                }
                */

                string DirServiceNomenPatchFull = "",
                       ID0 = "", ID1 = "", ID2 = "";

                var queryDirServiceNomenID = await
                    (
                        from x in db.DocSecondHandRazbors
                        where x.DocSecondHandRazborID == id
                        select x.DirServiceNomenID
                    ).ToArrayAsync();

                if (queryDirServiceNomenID.Count() > 0)
                {
                    string[] ret = await Task.Run(() => docSecondHandPurchesController.mPatchFull(db, queryDirServiceNomenID[0]));
                    DirServiceNomenPatchFull = ret[0];

                    //Для поиска в списке товара (клгда нажимаем на кнопку "Склад", что бы сразу попасть на нужную группу)
                    string[] sID = ret[1].Split(',');
                    try { if (!String.IsNullOrEmpty(sID[0])) ID0 = sID[0].ToUpper(); if (ID0[ID0.Length - 1].ToString() == " ") ID0 = ID0.Remove(ID0.Length - 1); } catch { }
                    try { if (!String.IsNullOrEmpty(sID[1])) ID1 = sID[1].ToUpper(); if (ID1[ID1.Length - 1].ToString() == " ") ID1 = ID1.Remove(ID1.Length - 1); } catch { }
                    try { if (!String.IsNullOrEmpty(sID[2])) ID2 = sID[2].ToUpper(); if (ID2[ID2.Length - 1].ToString() == " ") ID2 = ID2.Remove(ID2.Length - 1); } catch { }
                }

                #endregion


                #region Суммы Услуг и Запчастей
                /*
                double dSumDocSecondHandRazborTabs = await db.DocSecondHandRazborTabs.Where(x => x.DocSecondHandRazborID == id).Select(x => x.PriceCurrency).DefaultIfEmpty(0).SumAsync();

                double dSumDocSecondHandRazborTabs = await db.DocSecondHandRazborTabs.Where(x => x.DocSecondHandRazborID == id).Select(x => x.PriceCurrency).DefaultIfEmpty(0).SumAsync();
                */
                #endregion


                #region QUERY

                var query = await Task.Run(() =>
                    (
                        #region from

                        from docSecondHandRazbors in db.DocSecondHandRazbors

                        join dirServiceNomens11 in db.DirServiceNomens on docSecondHandRazbors.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                            /*
                            join docSecondHandRazbor1Tabs1 in db.DocSecondHandRazborTabs on docSecondHandRazbors.DocSecondHandRazborID equals docSecondHandRazbor1Tabs1.DocSecondHandRazborID into docSecondHandRazbor1Tabs2
                            from docSecondHandRazbor1Tabs in docSecondHandRazbor1Tabs2.DefaultIfEmpty()

                            join docSecondHandRazborTabs1 in db.DocSecondHandRazborTabs on docSecondHandRazbors.DocSecondHandRazborID equals docSecondHandRazborTabs1.DocSecondHandRazborID into docSecondHandRazborTabs2
                            from docSecondHandRazborTabs in docSecondHandRazborTabs2.DefaultIfEmpty()
                            */

                            //join docServicePurches1 in db.DocServicePurches on docSecondHandRazbors.DocIDService equals docServicePurches1.doc.DocID into docServicePurches2
                            //from docServicePurches in docServicePurches2.DefaultIfEmpty()

                        #endregion

                        where docSecondHandRazbors.DocSecondHandRazborID == id

                        #region select

                        select new
                        {
                            DocID = docSecondHandRazbors.DocID,
                            DocDate = docSecondHandRazbors.doc.DocDate,
                            Base = docSecondHandRazbors.doc.Base,
                            Held = docSecondHandRazbors.doc.Held,
                            Discount = docSecondHandRazbors.doc.Discount,
                            Del = docSecondHandRazbors.doc.Del,
                            Description = docSecondHandRazbors.doc.Description,
                            IsImport = docSecondHandRazbors.doc.IsImport,
                            DirVatValue = docSecondHandRazbors.doc.DirVatValue,
                            //DirPaymentTypeID = docSecondHandRazbors.doc.DirPaymentTypeID,

                            DirServiceNomenID = docSecondHandRazbors.DirServiceNomenID,

                            DirServiceNomenNameLittle = docSecondHandRazbors.dirServiceNomen.DirServiceNomenName,

                            DirServiceNomenName =
                            DirServiceNomenPatchFull == null ? docSecondHandRazbors.dirServiceNomen.DirServiceNomenName
                            :
                            DirServiceNomenPatchFull, // + docSecondHandRazbors.dirServiceNomen.DirServiceNomenName,

                            /*
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName,
                            */

                            ID0 = dirServiceNomensGroup.DirServiceNomenID,
                            ID1 = dirServiceNomensSubGroup.DirServiceNomenID,
                            ID2 = docSecondHandRazbors.dirServiceNomen.DirServiceNomenID,



                            DocSecondHandRazborID = docSecondHandRazbors.DocSecondHandRazborID,
                            DirContractorName = docSecondHandRazbors.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docSecondHandRazbors.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docSecondHandRazbors.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docSecondHandRazbors.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docSecondHandRazbors.dirWarehouse.DirWarehouseName,

                            DirSecondHandStatusID = docSecondHandRazbors.DirSecondHandStatusID,

                            PriceVAT = docSecondHandRazbors.PriceVAT,
                            PriceCurrency = docSecondHandRazbors.PriceCurrency,

                            DirCurrencyID = docSecondHandRazbors.DirCurrencyID,
                            DirCurrencyRate = docSecondHandRazbors.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSecondHandRazbors.DirCurrencyMultiplicity,
                            DirCurrencyName = docSecondHandRazbors.dirCurrency.DirCurrencyName + " (" + docSecondHandRazbors.DirCurrencyRate + ", " + docSecondHandRazbors.DirCurrencyMultiplicity + ")",

                            //Оплата
                            Payment = docSecondHandRazbors.doc.Payment,
                            //Мастер
                            DirEmployeeIDMaster = docSecondHandRazbors.DirEmployeeIDMaster,
                            DirEmployeeNameMaster = docSecondHandRazbors.dirEmployee.DirEmployeeName,



                            // *** СУММЫ *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                            SumsDirNomen = docSecondHandRazbors.SumsDirNomen,
                            SumOfVATCurrency = docSecondHandRazbors.SumsDirNomen,

                            /*
                            //1. Подсчет табличной части Работы "SumDocSecondHandRazborTabs"
                            SumDocSecondHandRazborTabs = docSecondHandRazbors.Sums1, //dSumDocSecondHandRazborTabs,
                            SumDocSecondHandRazborTabs2 = docSecondHandRazbors.Sums1, //dSumDocSecondHandRazborTabs,
                            //2. Подсчет табличной части Работы "SumDocSecondHandRazborTabs"
                            SumDocSecondHandRazborTabs = docSecondHandRazbors.Sums2, //dSumDocSecondHandRazborTabs,
                            SumDocSecondHandRazborTabs2 = docSecondHandRazbors.Sums2, //dSumDocSecondHandRazborTabs,
                            //3. Сумма 1+2 "SumTotal"
                            SumTotal = docSecondHandRazbors.Sums1 + docSecondHandRazbors.Sums2, //dSumDocSecondHandRazborTabs + dSumDocSecondHandRazborTabs,
                            SumTotal2 = docSecondHandRazbors.Sums1 + docSecondHandRazbors.Sums2, //dSumDocSecondHandRazborTabs + dSumDocSecondHandRazborTabs,
                            //5. 3 - 4 "SumTotal2"
                            SumTotal2a = docSecondHandRazbors.Sums1 + docSecondHandRazbors.Sums2, //dSumDocSecondHandRazborTabs + dSumDocSecondHandRazborTabs, // - docSecondHandRazbors.PrepaymentSum,
                            PriceVATSums = docSecondHandRazbors.Sums1 + docSecondHandRazbors.Sums2 + docSecondHandRazbors.PriceVAT, //dSumDocSecondHandRazborTabs + dSumDocSecondHandRazborTabs + docSecondHandRazbors.PriceVAT, // - docSecondHandRazbors.PrepaymentSum,

                            //Alerted = docSecondHandRazbors.AlertedCount == null ? "Не оповещён" : "Оповещён (" + docSecondHandRazbors.AlertedCount + ") " + docSecondHandRazbors.AlertedDateTxt

                            //Перемещён в БУ
                            FromService = docSecondHandRazbors.FromService,
                            FromServiceString = docSecondHandRazbors.FromService == true ? "В Б/У" : "",
                            DocIDService = docSecondHandRazbors.DocIDService,
                            DocServicePurchID = docServicePurches.DocServicePurchID,
                            */

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
                        Models.Sklad.Doc.DocSecondHandRazbor docSecondHandRazbor = await dbRead.DocSecondHandRazbors.FindAsync(id);
                        docSecondHandRazbor.DirEmployeeIDMaster = field.DirEmployeeID;
                        docSecondHandRazbor.DirSecondHandStatusID = 2;
                        dbRead.Entry(docSecondHandRazbor).State = EntityState.Modified;
                        await dbRead.SaveChangesAsync();

                        //Пишем в Лог о смене статуса и мастера, если такое было
                        logService.DocSecondHandRazborID = id;
                        logService.DirSecondHandLogTypeID = 1;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirSecondHandStatusID = docSecondHandRazbor.DirSecondHandStatusID;
                        //if (query[0].DirEmployeeIDMaster != field.DirEmployeeID) logService.Msg = "Смена мастера " + query[0].DirEmployeeNameMaster;

                        await logServicesController.mPutPostLogSecondHandRazbors(db, logService, EntityState.Added);
                    }

                    //Пишем в Лог о смене мастера, если такое было
                    if (query[0].DirEmployeeIDMaster != field.DirEmployeeID)
                    {
                        Models.Sklad.Doc.DocSecondHandRazbor docSecondHandRazbor = await dbRead.DocSecondHandRazbors.FindAsync(id);
                        docSecondHandRazbor.DirEmployeeIDMaster = field.DirEmployeeID;
                        //docSecondHandRazbor.DirSecondHandStatusID = 2;
                        dbRead.Entry(docSecondHandRazbor).State = EntityState.Modified;
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

        // PUT: api/DocSecondHandRazbors/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRazbor(int id, DocSecondHandRazbor docSecondHandRazbor, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        //Смена статуса
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRazbor(int id, int DirStatusID, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRazbors"));
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
                double SumOfVATCurrency = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "SumOfVATCurrency", true) == 0).Value.Replace(".", ","));
                string sReturnRresults = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sReturnRresults", true) == 0).Value;

                //double PriceRetailVAT = 0; if (paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceRetailVAT", true) == 0).Value != null) PriceRetailVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceRetailVAT", true) == 0).Value.Replace(".", ",")); else PriceRetailVAT = 0;
                //double PriceWholesaleVAT = 0; if (paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceWholesaleVAT", true) == 0).Value != null) PriceWholesaleVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceWholesaleVAT", true) == 0).Value.Replace(".", ",")); else PriceWholesaleVAT = 0;
                //double PriceIMVAT = 0; if (paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceIMVAT", true) == 0).Value != null) PriceIMVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceIMVAT", true) == 0).Value.Replace(".", ",")); else PriceIMVAT = 0;

                //Если "В торговлю"
                DocSecondHandRazbor docSecondHandRazbor = await db.DocSecondHandRazbors.FindAsync(id);
                /*if (
                    docSecondHandRazbor.DirSecondHandStatusID == 7 && DirStatusID == 9 &&
                    PriceRetailVAT > 0 && PriceWholesaleVAT > 0 && PriceIMVAT > 0
                   )
                {
                    docSecondHandRazbor.PriceRetailVAT = PriceRetailVAT; docSecondHandRazbor.PriceRetailCurrency = PriceRetailVAT;
                    docSecondHandRazbor.PriceWholesaleVAT = PriceWholesaleVAT; docSecondHandRazbor.PriceWholesaleCurrency = PriceWholesaleVAT;
                    docSecondHandRazbor.PriceIMVAT = PriceIMVAT; docSecondHandRazbor.PriceIMCurrency = PriceIMVAT;
                }*/

                #endregion

                #region Проверки
                //Если Статус "7", то проверить Таб часть-1
                // Если DirStatusID = 7 и нет ни одной выполненной работы, то не пускать сохранять и выдать эксепшн
                //if (iTypeService > 1 && docSecondHandRazbor.DirStatusID == 7 && docSecondHandRazbor1TabCollection.Length == 0) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg114); }

                var queryCount = await
                    (
                        from x in db.DocSecondHandRazborTabs
                        where x.DocSecondHandRazborID == id
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
                    //Models.Sklad.Doc.DocSecondHandRazbor docSecondHandRazbor = new DocSecondHandRazbor();

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            await mStatusChange(db, docSecondHandRazbor, DirStatusID, DirPaymentTypeID, SumOfVATCurrency, sReturnRresults, field); //, KKMSCheckNumber, KKMSIdCommand, ts, id 

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
                        DocID = docSecondHandRazbor.DocID,
                        DocSecondHandRazborID = docSecondHandRazbor.DocSecondHandRazborID
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



        // POST: api/DocSecondHandRazbors
        [ResponseType(typeof(DocSecondHandRazbor))]
        public async Task<IHttpActionResult> PostDocSecondHandRazbor(DocSecondHandRazbor docSecondHandRazbor, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSecondHandRazbors/5
        [ResponseType(typeof(DocSecondHandRazbor))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRazbor(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
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

        private bool DocSecondHandRazborExists(int id)
        {
            return db.DocSecondHandRazbors.Count(e => e.DocSecondHandRazborID == id) > 0;
        }


        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            //System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocSecondHandRazbor docSecondHandRazbor,
            //int id,
            int DirSecondHandStatusID,
            int DirPaymentTypeID,
            double SumOfVATCurrency,
            string sReturnRresults,

            Classes.Account.Login.Field field//, //Дополнительные данные о сотруднике

            //Не нужно
            //int? KKMSCheckNumber,
            //string KKMSIdCommand
            )
        {


            return false;


            //Не трогать!!!
            //docSecondHandRazbor = await db.DocSecondHandRazbors.FindAsync(id);
            int DocSecondHandRazborID = Convert.ToInt32(docSecondHandRazbor.DocSecondHandRazborID);

            DateTime dtNow = DateTime.Now;

            #region Проверка, если предыдущий статус такой же на который меняем, то не писать в Лог

            //Исключение, т.к. если в Логе нет записей с сменой статуса получим Ошибку из-за "FirstAsync()"
            try
            {
                var query = await
                    (
                        from x in db.LogSecondHandRazbors
                        where x.DocSecondHandRazborID == DocSecondHandRazborID && x.DirSecondHandStatusID != null
                        select new
                        {
                            LogSecondHandRazborID = x.LogSecondHandRazborID,
                            DirSecondHandStatusID = x.DirSecondHandStatusID
                        }
                    ).OrderByDescending(x => x.LogSecondHandRazborID).FirstAsync();

                if (
                    docSecondHandRazbor.DirSecondHandStatusID == DirSecondHandStatusID &&
                    query.DirSecondHandStatusID == DirSecondHandStatusID
                   )
                {
                    return false;
                }
            }
            catch (Exception ex) { }

            #endregion


            #region 1. Сохранение статуса в БД

            //Сохраняем старый статус, ниже - нужен
            int? DirSecondHandStatusID_OLD = docSecondHandRazbor.DirSecondHandStatusID;
            
            //Сохранить статус аппарата: Готов (7) или Отказ (8)
            if (DirSecondHandStatusID == 7 || DirSecondHandStatusID == 8) // || DirSecondHandStatusID == 9
            {
                docSecondHandRazbor.DirSecondHandStatusID_789 = DirSecondHandStatusID; //Статус аппарата сохранить: Готов или Отказ
            }

            //Если был Статус == 9 (Выдан) и сменили на "В диагностике", то менять "DateDone" на текущую + 7 дней (из настроек)
            bool bDirSecondHandLogTypeID9 = false;
            if (docSecondHandRazbor.DirSecondHandStatusID == 9 && DirSecondHandStatusID == 2)
            {
                //4. Сообщение для Лога: 
                bDirSecondHandLogTypeID9 = true;
            }
            
            //Дата смены статуса
            docSecondHandRazbor.DateStatusChange = dtNow;
            docSecondHandRazbor.DirSecondHandStatusID = DirSecondHandStatusID;

            db.Entry(docSecondHandRazbor).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion



            if (DirSecondHandStatusID_OLD == 7 && docSecondHandRazbor.DirSecondHandStatusID == 9)
            {

                #region В Продажу (Пратии)

                #region 1. Проверки: ...
                //1. Проверки:
                //   Если статус DirSecondHandStatusID_OLD == 7 && docSecondHandRazbor.DirSecondHandStatusID == 9
                //   тогда проверяем, если есть в таб части запчасти и есть продажные цены, то проводим документы
                //2. Проведение документа: 
                //   сам аппарат списываем на Склад Списания (найти его по Sub-складу)
                //   запчасти приходуем на Основй Склад (найти его по Sub-складу)

                //   Если статус DirSecondHandStatusID_OLD == 7 && docSecondHandRazbor.DirSecondHandStatusID == 9
                //   тогда проверяем, если есть в таб части запчасти и есть продажные цены, то проводим документы

                var queryDocSecondHandRazborTabs = await
                    (
                        from x in db.DocSecondHandRazborTabs
                        where x.DocSecondHandRazborID == docSecondHandRazbor.DocSecondHandRazborID
                        select x
                    ).ToListAsync();
                if (queryDocSecondHandRazborTabs.Count() > 0)
                {
                    //Проверяем цены
                    for (int i = 0; i < queryDocSecondHandRazborTabs.Count(); i++)
                    {
                        var qX = queryDocSecondHandRazborTabs[i];

                        //1. К-во
                        if (qX.Quantity <= 0) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg133_1); }

                        //1. Приходная цена > Продажных
                        if (qX.PriceVAT < 0 || qX.PriceVAT > qX.PriceRetailVAT || qX.PriceVAT > qX.PriceWholesaleVAT || qX.PriceVAT > qX.PriceIMVAT) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg133_2); }
                    }
                }
                else { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg133_3); }

                #endregion


                #region 2. Проведение документа: 

                #region Doc

                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandRazbor.DocID);
                doc.Held = true;
                db.Entry(doc).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());

                #endregion


                #region Находим нужные Точки: Куда приходовать запчасти и Куда списать аппарат

                //2.1. Находим Основной склад по Sub-складу
                //   запчасти приходуем на Основй Склад (найти его по Sub-складу)
                int Warehouse1 = 0;
                var queryWarehouse1 = await
                    (
                        from x in db.DirWarehouses
                        where x.DirWarehouseID == docSecondHandRazbor.DirWarehouseID
                        select x.Sub
                    ).ToListAsync();
                if (queryWarehouse1.Count() > 0)
                {
                    Warehouse1 = queryWarehouse1[0].Value;
                }

                //2.2. Склад списания основной склада
                //   сам аппарат списываем на Склад Списания (найти его по Sub-складу)
                int Warehouse2 = 0;
                var queryWarehouse2 = await
                    (
                        from x in db.DirWarehouses
                        where x.Sub == Warehouse1 && x.DirWarehouseLoc == 1
                        select x.DirWarehouseID
                    ).ToListAsync();
                if (queryWarehouse2.Count() > 0)
                {
                    Warehouse2 = queryWarehouse1[0].Value;
                }

                #endregion


                //Получаем партию аппарата
                //int iRem2PartyID = Convert.ToInt32(docSecondHandRazbor.Rem2PartyID);
                var queryRem2Party = await db.Rem2Parties.Where(x => x.DocID == docSecondHandRazbor.DocID).ToListAsync();
                if (queryRem2Party.Count() == 0) { throw new System.InvalidOperationException("Партия не найдена!!! № партии-2:" + queryRem2Party[0].Rem2PartyID); }
                //Находим партию
                Models.Sklad.Rem.Rem2Party _rem2Party = await db.Rem2Parties.FindAsync(queryRem2Party[0].Rem2PartyID);

                //Models.Sklad.Rem.Rem2Party rem2Party = await db.Rem2Parties.FindAsync(docSecondHandRazbor.Rem2PartyID);


                #region Приходуем запчасти: RemParty - Партии

                //Находим табличную часть (запчасти)
                var docSecondHandRazborTabCollection = await db.DocSecondHandRazborTabs.Where(x => x.DocSecondHandRazborID == docSecondHandRazbor.DocSecondHandRazborID).ToListAsync();

                Models.Sklad.Rem.RemParty[] remPartyCollection = new Models.Sklad.Rem.RemParty[docSecondHandRazborTabCollection.Count()];
                for (int i = 0; i < docSecondHandRazborTabCollection.Count(); i++)
                {
                    Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();
                    remParty.RemPartyID = null;
                    remParty.DirNomenID = docSecondHandRazborTabCollection[i].DirNomenID;
                    remParty.Quantity = docSecondHandRazborTabCollection[i].Quantity;
                    remParty.Remnant = docSecondHandRazborTabCollection[i].Quantity;
                    remParty.DirCurrencyID = docSecondHandRazborTabCollection[i].DirCurrencyID;
                    //remParty.DirCurrencyMultiplicity = docSecondHandRazborTabCollection[i].DirCurrencyMultiplicity;
                    //remParty.DirCurrencyRate = docSecondHandRazborTabCollection[i].DirCurrencyRate;
                    remParty.DirVatValue = 0; // docPurch.DirVatValue;
                    remParty.DirWarehouseID = Warehouse1; // docPurch.DirWarehouseID;
                    remParty.DirWarehouseIDDebit = Warehouse1; // docPurch.DirWarehouseID;
                    remParty.DirWarehouseIDPurch = _rem2Party.DirWarehouseIDPurch; // docPurch.DirWarehouseID;
                    remParty.DirContractorIDOrg = docSecondHandRazbor.doc.DirContractorIDOrg;

                    //!!! Важно !!!
                    //if (docSecondHandRazborTabCollection[i].DirContractorID != null) remParty.DirContractorID = Convert.ToInt32(docSecondHandRazborTabCollection[i].DirContractorID);
                    //else remParty.DirContractorID = docSecondHandRazbor.DirContractorID;
                    remParty.DirContractorID = docSecondHandRazbor.doc.DirContractorIDOrg;
                    //!!! Важно !!!

                    //Дата Приёмки товара
                    remParty.DocDatePurches = docSecondHandRazbor.doc.DocDate;

                    remParty.DirCharColourID = docSecondHandRazborTabCollection[i].DirCharColourID;
                    remParty.DirCharMaterialID = docSecondHandRazborTabCollection[i].DirCharMaterialID;
                    remParty.DirCharNameID = docSecondHandRazborTabCollection[i].DirCharNameID;
                    remParty.DirCharSeasonID = docSecondHandRazborTabCollection[i].DirCharSeasonID;
                    remParty.DirCharSexID = docSecondHandRazborTabCollection[i].DirCharSexID;
                    remParty.DirCharSizeID = docSecondHandRazborTabCollection[i].DirCharSizeID;
                    remParty.DirCharStyleID = docSecondHandRazborTabCollection[i].DirCharStyleID;
                    remParty.DirCharTextureID = docSecondHandRazborTabCollection[i].DirCharTextureID;

                    remParty.SerialNumber = docSecondHandRazborTabCollection[i].SerialNumber;
                    remParty.Barcode = docSecondHandRazborTabCollection[i].Barcode;

                    remParty.DocID = Convert.ToInt32(docSecondHandRazbor.DocID);
                    remParty.PriceCurrency = docSecondHandRazborTabCollection[i].PriceCurrency;
                    remParty.PriceVAT = docSecondHandRazborTabCollection[i].PriceVAT;
                    remParty.FieldID = Convert.ToInt32(docSecondHandRazborTabCollection[i].DocSecondHandRazborTabID);

                    remParty.PriceRetailVAT = docSecondHandRazborTabCollection[i].PriceRetailVAT;
                    remParty.PriceRetailCurrency = docSecondHandRazborTabCollection[i].PriceRetailCurrency;
                    remParty.PriceWholesaleVAT = docSecondHandRazborTabCollection[i].PriceWholesaleVAT;
                    remParty.PriceWholesaleCurrency = docSecondHandRazborTabCollection[i].PriceWholesaleCurrency;
                    remParty.PriceIMVAT = docSecondHandRazborTabCollection[i].PriceIMVAT;
                    remParty.PriceIMCurrency = docSecondHandRazborTabCollection[i].PriceIMCurrency;

                    //DirNomenMinimumBalance
                    remParty.DirNomenMinimumBalance = sysSetting.DirNomenMinimumBalance;

                    remParty.DirEmployeeID = doc.DirEmployeeID;
                    remParty.DocDate = doc.DocDate;

                    remPartyCollection[i] = remParty;
                }

                Controllers.Sklad.Rem.RemPartiesController remPartys = new Rem.RemPartiesController();
                await Task.Run(() => remPartys.Save(db, remPartyCollection)); //remPartys.Save(db, remPartyCollection);

                #endregion


                #region Списываем Аппарат

                // !!! Тут Важно !!!
                //По идее надо списівать на склад "Списание"
                //Но, тогда будет не видно в Архиве! Т.к. тут отображаются аппараті со склада "Разбор", а не "Списание".
                //Так что я не сделал перемещение аппарата на склад "Списание".


                #region Проверка


                //Переменные
                double dQuantity = 1;

                if (_rem2Party == null)
                {
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg116_1 + _rem2Party.Rem2PartyID + Classes.Language.Sklad.Language.msg116_2);
                }
                db.Entry(_rem2Party).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                #region 1. Есть ли остаток в партии с которой списываем!
                if (_rem2Party.Remnant < dQuantity)
                {
                    throw new System.InvalidOperationException(
                        Classes.Language.Sklad.Language.msg104 +

                        "<tr>" +
                        "<td>" + _rem2Party.Rem2PartyID + "</td>" +                                //партия
                        "<td>" + _rem2Party.DirServiceNomenID + "</td>" +                                //Код товара
                        "<td>" + _rem2Party.Quantity + "</td>" +                                  //списуемое к-во
                        "<td>" + _rem2Party.Remnant + "</td>" +                                                  //остаток партии
                        "<td>" + (1 - _rem2Party.Remnant).ToString() + "</td>" +  //недостающее к-во
                        "</tr>" +
                        "</table>" +

                        Classes.Language.Sklad.Language.msg104_1
                    );
                }
                #endregion

                #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                if (_rem2Party.DirWarehouseID != docSecondHandRazbor.DirWarehouseID)
                {
                    //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandRazbor.dirWarehouse.DirWarehouseName"
                    Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docSecondHandRazbor.DirWarehouseID);

                    throw new System.InvalidOperationException(
                        Classes.Language.Sklad.Language.msg105 +

                        "<tr>" +
                        "<td>" + _rem2Party.Rem2PartyID + "</td>" +           //партия
                        "<td>" + docSecondHandRazbor.DirServiceNomenID + "</td>" +           //Код товара
                        "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                        "<td>" + _rem2Party.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                        "</tr>" +
                        "</table>" +

                        Classes.Language.Sklad.Language.msg105_1
                    );
                }
                #endregion

                #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                if (_rem2Party.DirContractorIDOrg != docSecondHandRazbor.doc.DirContractorIDOrg)
                {
                    //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docSecondHandRazbor.dirWarehouse.DirWarehouseName"
                    Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(docSecondHandRazbor.DirContractorIDOrg);

                    throw new System.InvalidOperationException(
                        Classes.Language.Sklad.Language.msg106 +

                        "<tr>" +
                        "<td>" + _rem2Party.Rem2PartyID + "</td>" +           //партия
                        "<td>" + docSecondHandRazbor.DirServiceNomenID + "</td>" +           //Код товара
                        "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                        "<td>" + _rem2Party.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                        "</tr>" +
                        "</table>" +

                        Classes.Language.Sklad.Language.msg106_1
                    );
                }
                #endregion

                #endregion

                #region Сохранение

                Models.Sklad.Rem.Rem2PartyMinus rem2PartyMinus = new Models.Sklad.Rem.Rem2PartyMinus();
                rem2PartyMinus.Rem2PartyMinusID = null;
                rem2PartyMinus.Rem2PartyID = Convert.ToInt32(_rem2Party.Rem2PartyID);

                rem2PartyMinus.DirServiceNomenID = docSecondHandRazbor.DirServiceNomenID;

                rem2PartyMinus.Quantity = 1; // docSecondHandRazbor.Quantity;
                rem2PartyMinus.DirCurrencyID = docSecondHandRazbor.DirCurrencyID;
                rem2PartyMinus.DirCurrencyMultiplicity = docSecondHandRazbor.DirCurrencyMultiplicity;
                rem2PartyMinus.DirCurrencyRate = docSecondHandRazbor.DirCurrencyRate;
                rem2PartyMinus.DirVatValue = docSecondHandRazbor.DirVatValue;
                rem2PartyMinus.DirWarehouseID = docSecondHandRazbor.DirWarehouseID;
                rem2PartyMinus.DirContractorIDOrg = docSecondHandRazbor.doc.DirContractorIDOrg;

                rem2PartyMinus.DirServiceContractorID = _rem2Party.DirServiceContractorID;

                rem2PartyMinus.DocID = Convert.ToInt32(docSecondHandRazbor.DocID);
                rem2PartyMinus.PriceCurrency = Convert.ToDouble(docSecondHandRazbor.PriceCurrency);
                rem2PartyMinus.PriceVAT = docSecondHandRazbor.PriceVAT;
                rem2PartyMinus.FieldID = Convert.ToInt32(docSecondHandRazbor.DocSecondHandRazborID);
                rem2PartyMinus.Reserve = false; // docSecondHandRazbor.Reserve;

                rem2PartyMinus.DirEmployeeID = doc.DirEmployeeID;
                rem2PartyMinus.DocDate = doc.DocDate;

                db.Entry(rem2PartyMinus).State = EntityState.Added;
                await db.SaveChangesAsync();

                #endregion

                #endregion


                #endregion

                #endregion

            }
            else if (DirSecondHandStatusID_OLD == 9 && docSecondHandRazbor.DirSecondHandStatusID == 2)
            {

                #region В диагностику


                #region 1. Товар (RemParty - удаляем с партий)

                #region 1.1. Проверка: Было ли списание с партий *** *** *** *** *** *** *** *** *** ***
                

                //1.1.1. 
                //Получаем DocSecondHandRazbor из БД, потому, что могли изменить у Клиента
                Models.Sklad.Doc.DocSecondHandRazbor _docSecondHandRazbor = db.DocSecondHandRazbors.Find(DocSecondHandRazborID);
                int? iDocSecondHandRazbor_DocID = _docSecondHandRazbor.DocID;

                Classes.Function.WriteOffGoodsWithParty writeOffGoodsWithParty = new Classes.Function.WriteOffGoodsWithParty();
                bool bWriteOffGoodsWithParty = await Task.Run(() => writeOffGoodsWithParty.Exist(db, iDocSecondHandRazbor_DocID));
                if (bWriteOffGoodsWithParty)
                {
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg134);
                }

                //1.1.2. 
                var querySpisano = await
                    (
                        from x in db.RemParties
                        where x.DocID == docSecondHandRazbor.DocID
                        select x
                    ).ToListAsync();
                for (int i = 0; i < querySpisano.Count(); i++)
                {
                    if (querySpisano[i].Remnant != querySpisano[i].Quantity)
                    {
                        throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg134);
                    }
                }


                #endregion


                //Удаление записей в таблицах: RemParties
                #region 1.2. RemParties - удаление *** *** *** *** *** *** *** *** *** ***

                //Проверяем если ли расходы (проведённый или НЕ проведенные)


                //Удаляем записи в таблице "RemParties"
                SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSecondHandRazbor.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID; ", parDocID);

                #endregion

                //Doc.Held = false
                #region 1.3. Doc *** *** *** *** *** *** *** *** *** ***

                Models.Sklad.Doc.Doc doc = db.Docs.Find(docSecondHandRazbor.DocID);
                doc.Held = false;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save()); //docs.Save();

                #endregion

                #endregion


                #region 2. Аппарат (Rem2PartyMinuses - удаляем с МинусПартий)

                //Удаляем записи в таблице "RemParties"
                //SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docSecondHandRazbor.DocID };
                await db.Database.ExecuteSqlCommandAsync("DELETE FROM Rem2PartyMinuses WHERE DocID=@DocID; ", parDocID);

                #endregion


                #endregion

            }



            #region 3. Лог


            #region Пишем в Лог о смене статуса и мастера, если такое было

            logService.DocSecondHandRazborID = DocSecondHandRazborID;
            if (!bDirSecondHandLogTypeID9) logService.DirSecondHandLogTypeID = 1; //Смена статуса
            else logService.DirSecondHandLogTypeID = 9; //Возврат по гарантии
            logService.DirEmployeeID = field.DirEmployeeID;
            logService.DirSecondHandStatusID = DirSecondHandStatusID;
            if (!String.IsNullOrEmpty(sReturnRresults)) logService.Msg = sReturnRresults;

            await logServicesController.mPutPostLogSecondHandRazbors(db, logService, EntityState.Added);

            #endregion


            #endregion

            
            return true;
        }
        
        #endregion
    }
}