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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocOrderInts
{
    public class DocOrderIntsController : ApiController
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
        Models.Sklad.Log.LogOrderInt logOrderInt = new Models.Sklad.Log.LogOrderInt(); Controllers.Sklad.Log.LogOrderIntsController logOrderIntsController = new Log.LogOrderIntsController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 59;

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
            public int FilterType; // == DirOrderIntStatusID
            public int? DirWarehouseID;

            public DateTime? DateS;
            public DateTime? DatePo;

            public int DirOrderIntStatusIDS;
            public int DirOrderIntStatusIDPo;

            public int DocOrderIntTypeS;
            public int DocOrderIntTypePo;

            public int DirEmployeeID;
        }
        // GET: api/DocOrderInts
        public async Task<IHttpActionResult> GetDocOrderInts(HttpRequestMessage request)
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

                //Права (1 - Write, 2 - Read, 3 - No Access)
                /*
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocOrderInts"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                */

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
                _params.DirOrderIntStatusIDS = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirOrderIntStatusIDS", true) == 0).Value);
                _params.DirOrderIntStatusIDPo = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirOrderIntStatusIDPo", true) == 0).Value);
                _params.DocOrderIntTypeS = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocOrderIntTypeS", true) == 0).Value);
                _params.DocOrderIntTypePo = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocOrderIntTypePo", true) == 0).Value);
                _params.DirEmployeeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value);
                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);

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
                        from x in db.DocOrderInts

                        select new
                        {
                            DocOrderIntID = x.DocOrderIntID,
                            DocDate = x.doc.DocDate,
                            DirContractorIDOrg = x.doc.DirContractorIDOrg,

                            DirEmployeeID = x.doc.DirEmployeeID,
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,

                            DirOrderIntStatusID = x.DirOrderIntStatusID,
                            DirOrderIntStatusName = x.dirOrderIntStatus.DirOrderIntStatusName,

                            DirOrderIntTypeID = x.DirOrderIntTypeID,
                            DirOrderIntTypeName = x.dirOrderIntType.DirOrderIntTypeName,

                            DirWarehouseID = x.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,

                            Description = x.doc.Description,

                            DirServiceNomenID = x.DirServiceNomenID,
                            DirServiceNomenName = x.dirServiceNomen.DirServiceNomenName,

                            //Новый товар или существующий
                            NomenExist = x.DirNomenID == null ? "<b>Новый</b>"
                            :
                            "Существующий",

                            DirNomenXName6 = x.dirNomen1.DirNomenName + " / " + x.dirNomen2.DirNomenName + "/" + x.dirNomenCategory.DirNomenCategoryName,

                            DirNomen1Name = x.dirNomen1.DirNomenName,
                            DirNomen2Name = x.dirNomen2.DirNomenName,
                            DirNomenName = x.DirNomenID == null ? "<b>" + x.DirNomenName + "</b>"
                            :
                            x.dirNomen.DirNomenName,
                            //DirNomenCategoryName = x.dirNomenCategory.DirNomenCategoryName,


                            //Клиент
                            DirOrderIntContractorPhone = x.DirOrderIntContractorPhone,
                            DirOrderIntContractorName = x.DirOrderIntContractorName,


                            PriceVAT = x.PriceVAT,
                            PriceCurrency = x.PriceCurrency,
                            PrepaymentSum = x.PrepaymentSum,

                            //Дата.исполнения
                            DateDone = x.DateDone,

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
                    //query = query.Where(x => x.Del == sysSetting.DeletedRecordsShow);
                }

                #endregion


                #region Фильтр "FilterType"

                if (_params.FilterType > 0)
                {
                    query = query.Where(x => x.DirOrderIntStatusID == _params.FilterType);
                }

                #endregion

                #region Фильтр "DocOrderIntType"

                //DocOrderIntType
                if (_params.DocOrderIntTypeS != _params.DocOrderIntTypePo)
                {
                    if (_params.DocOrderIntTypeS > 0) query = query.Where(x => x.DirOrderIntTypeID >= _params.DocOrderIntTypeS);
                    if (_params.DocOrderIntTypePo > 0) query = query.Where(x => x.DirOrderIntTypeID <= _params.DocOrderIntTypePo);
                }
                else
                {
                    if (_params.DocOrderIntTypeS > 0) query = query.Where(x => x.DirOrderIntTypeID == _params.DocOrderIntTypeS);
                }

                //DirOrderIntStatusID
                if (_params.DirOrderIntStatusIDS != _params.DirOrderIntStatusIDPo)
                {
                    if (_params.DirOrderIntStatusIDS > 0) query = query.Where(x => x.DirOrderIntStatusID >= _params.DirOrderIntStatusIDS);
                    if (_params.DirOrderIntStatusIDPo > 0) query = query.Where(x => x.DirOrderIntStatusID <= _params.DirOrderIntStatusIDPo);
                }
                else
                {
                    if (_params.DirOrderIntStatusIDS > 0) query = query.Where(x => x.DirOrderIntStatusID == _params.DirOrderIntStatusIDS);

                    //Кроме Архива
                    if (_params.DirOrderIntStatusIDS <= 0 && _params.DirOrderIntStatusIDPo <= 0)
                    {
                        query = query.Where(x => x.DirOrderIntStatusID != 40 && x.DirOrderIntStatusID != 50);
                    }
                }

                //DirEmployeeID
                if (_params.DirEmployeeID > 0)
                {
                    query = query.Where(x => x.DirEmployeeID <= _params.DirEmployeeID); // && x.DirOrderIntStatusID < 7
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
                        query = query.Where(x => x.DocOrderIntID == iNumber32 || x.DirWarehouseName.Contains(_params.parSearch));
                    }
                    //Если Дата
                    else if (bDateTime)
                    {
                        query = query.Where(x => x.DocDate == dDateTime);
                    }
                    //Иначе, только текстовые поля
                    else
                    {
                        query = query.Where(x => x.DirWarehouseName.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                //query = query.OrderByDescending(x => x.DocDate).Skip(_params.Skip).Take(_params.limit);
                query = query.OrderByDescending(x => x.DocOrderIntID); //.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                //int dirCount = await Task.Run(() => db.DocOrderInts.Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount = query.Count();
                //if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocOrderInt = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocOrderInts/5
        [ResponseType(typeof(DocOrderInt))]
        public async Task<IHttpActionResult> GetDocOrderInt(int id, HttpRequestMessage request)
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

                //Права
                /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DocOrderInt"));
                if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocOrderInts"));
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
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (

                        from x in db.DocOrderInts
                        where x.DocOrderIntID == id
                        select new
                        {
                            DocOrderIntID = x.DocOrderIntID,
                            DocDate = x.doc.DocDate,

                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            DirOrderIntStatusName = x.dirOrderIntStatus.DirOrderIntStatusName,
                            Description = x.doc.Description,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,


                            DocID = x.DocID,
                            DirContractorID = x.doc.DirContractorID,
                            DirOrderIntTypeID = x.DirOrderIntTypeID,

                            //Документ который создал этот Заказ
                            DocID2 = x.DocID2,

                            DirWarehouseID = x.DirWarehouseID,
                            DirOrderIntStatusID = x.DirOrderIntStatusID,


                            DirServiceNomenID = x.DirServiceNomenID,
                            DirServiceNomenName = x.dirServiceNomen.DirServiceNomenName,


                            //Товар
                            DirNomenID = x.DirNomenID,

                            //Группы товара и чсам товар
                            DirNomen1ID = x.DirNomen1ID,
                            DirNomen2ID = x.DirNomen2ID,
                            DirNomenCategoryID = x.DirNomenCategoryID,
                            //DirNomen3Name = x.DirNomen3Name,
                            //DirNomen4ID = x.DirNomen4ID,
                            //DirNomen4Name = x.DirNomen4Name,
                            //DirNomen5ID = x.DirNomen5ID,
                            //DirNomen5Name = x.DirNomen5Name,
                            //DirNomen6ID = x.DirNomen6ID,
                            //DirNomen6Name = x.DirNomen6Name,

                            PriceVAT = x.PriceVAT,
                            PriceCurrency = x.PriceCurrency,
                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            Quantity = x.Quantity,

                            //Характеристики
                            DirCharColourID = x.DirCharColourID,
                            DirCharMaterialID = x.DirCharMaterialID,
                            DirCharNameID = x.DirCharNameID,
                            DirCharSeasonID = x.DirCharSeasonID,
                            DirCharSexID = x.DirCharSexID,
                            DirCharSizeID = x.DirCharSizeID,
                            DirCharStyleID = x.DirCharStyleID,
                            DirCharTextureID = x.DirCharTextureID,


                            //Полное наименование товара/запчасти
                            /*
                            DirNomenXName6 =
                            x.DirNomen6Name == null ?
                            (
                                x.DirNomen5Name == null ?
                                (
                                    x.DirNomen4Name == null ?
                                    (
                                        x.DirNomen3Name == null ?
                                        (
                                            x.DirNomen2Name == null ? x.DirNomen1Name
                                            :
                                            x.DirNomen1Name + " / " + x.DirNomen2Name
                                        )
                                        :
                                        x.DirNomen1Name + " / " + x.DirNomen2Name + " / " + x.DirNomen3Name
                                    )
                                    :
                                    x.DirNomen1Name + " / " + x.DirNomen2Name + " / " + x.DirNomen3Name + " / " + x.DirNomen4Name
                                )
                                :
                                x.DirNomen1Name + " / " + x.DirNomen2Name + " / " + x.DirNomen3Name + " / " + x.DirNomen4Name + " / " + x.DirNomen5Name
                            )
                            :
                            x.DirNomen1Name + " / " + x.DirNomen2Name + " / " + x.DirNomen3Name + " / " + x.DirNomen4Name + " / " + x.DirNomen5Name + " / " + x.DirNomen6Name,
                            */

                            DirNomenXName6 = x.dirNomen1.DirNomenName + " / " + x.dirNomen1.DirNomenName + "/" + x.dirNomenCategory.DirNomenCategoryName,


                            //Клиент
                            DirOrderIntContractorPhone = x.DirOrderIntContractorPhone,
                            DirOrderIntContractorName = x.DirOrderIntContractorName,

                            //PriceVAT = x.PriceVAT,
                            //PriceCurrency = x.PriceCurrency,
                            PrepaymentSum = x.PrepaymentSum,

                            //Дата.исполнения
                            DateDone = x.DateDone,

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

                //return Ok(returnServer.Return(false, Classes.Language.Language.msg89));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/DocOrderInts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocOrderInt(int id, DocOrderInt docOrderInt, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocOrderInts"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                var paramList = request.GetQueryNameValuePairs();


                docOrderInt.DocIDBase = docOrderInt.DocID2;


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
                            docOrderInt = await Task.Run(() => mPutPostdocOrderInt(db, dbRead, docOrderInt, EntityState.Modified, field)); //sysSetting
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
                    sysJourDisp.TableFieldID = docOrderInt.DocOrderIntID;
                    sysJourDisp.Description = "";
                    try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                    #endregion


                    dynamic collectionWrapper = new
                    {
                        DocID = docOrderInt.DocID,
                        DocOrderIntID = docOrderInt.DocOrderIntID
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
        double?
            PriceCurrency = 0, PriceVAT = 0,
            MarkupRetail = 0, PriceRetailVAT = 0, PriceRetailCurrency = 0, 
            MarkupWholesale = 0, PriceWholesaleVAT = 0, PriceWholesaleCurrency = 0, 
            MarkupIM = 0, PriceIMVAT = 0, PriceIMCurrency = 0;
        int DirContractorID = 0;
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocOrderInt(int id, int DirStatusID, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocOrderInts"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));


                //Разные Функции
                //function.NumberDecimalSeparator();

                //System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
                //nfi.NumberDecimalSeparator = ".";

                //string sep = ".";
                //System.Globalization.NumberFormatInfo nfi1 = (System.Globalization.NumberFormatInfo)System.Globalization.NumberFormatInfo.CurrentInfo.Clone();
                //nfi1.NumberDecimalSeparator = sep;

                var currentCulture = System.Globalization.CultureInfo.InstalledUICulture;
                var numberFormat = (System.Globalization.NumberFormatInfo)currentCulture.NumberFormat.Clone();
                numberFormat.NumberDecimalSeparator = ".";


                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                int DirPaymentTypeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPaymentTypeID", true) == 0).Value);

                PriceCurrency = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceCurrency", true) == 0).Value, numberFormat);
                PriceVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceVAT", true) == 0).Value, numberFormat);
                MarkupRetail = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "MarkupRetail", true) == 0).Value, numberFormat);
                PriceRetailVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceRetailVAT", true) == 0).Value, numberFormat);
                PriceRetailCurrency = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceRetailCurrency", true) == 0).Value, numberFormat);
                MarkupWholesale = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "MarkupWholesale", true) == 0).Value, numberFormat);
                PriceWholesaleVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceWholesaleVAT", true) == 0).Value, numberFormat);
                PriceWholesaleCurrency = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceWholesaleCurrency", true) == 0).Value, numberFormat);
                MarkupIM = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "MarkupIM", true) == 0).Value, numberFormat);
                PriceIMVAT = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceIMVAT", true) == 0).Value, numberFormat);
                PriceIMCurrency = Convert.ToDouble(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "PriceIMCurrency", true) == 0).Value, numberFormat);
                DirContractorID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorID", true) == 0).Value, numberFormat);

                #endregion

                #region Проверки

                //...

                #endregion


                #region Сохранение

                try
                {
                    Models.Sklad.Doc.DocOrderInt docOrderInt = new DocOrderInt();

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            await mStatusChange(db, ts, docOrderInt, id, DirStatusID, DirPaymentTypeID, field);

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
                        DocID = docOrderInt.DocID,
                        DocOrderIntID = docOrderInt.DocOrderIntID
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


        // POST: api/DocOrderInts
        [ResponseType(typeof(DocOrderInt))]
        public async Task<IHttpActionResult> PostDocOrderInt(DocOrderInt docOrderInt, HttpRequestMessage request)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocOrderInts"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            //var paramList = request.GetQueryNameValuePairs();

            //Docs.DocIDBase = DocOrderInt.DocID2
            docOrderInt.DocIDBase = docOrderInt.DocID2;
            docOrderInt.DirOrderIntStatusID = 10;

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            if (docOrderInt.DirNomenID == null && (docOrderInt.DirNomenName == null || docOrderInt.DirNomenName.Length == 0))
            {
                throw new System.InvalidOperationException("Не заполненно поле Товар!<br />Если товар существует - выбирите его из выпадающего списка.<br />Если товар новый - введите буквенное наименование товара.");
            }

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            docOrderInt.Substitute();

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
                        docOrderInt = await Task.Run(() => mPutPostdocOrderInt(db, dbRead, docOrderInt, EntityState.Added, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docOrderInt.DocOrderIntID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docOrderInt.DocID,
                    DocOrderIntID = docOrderInt.DocOrderIntID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DocOrderInts/5
        [ResponseType(typeof(DocOrderInt))]
        public async Task<IHttpActionResult> DeleteDocOrderInt(int id)
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права
            /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DocOrderInt"));
            if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocOrderInts"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DocOrderInt docOrderInt = await db.DocOrderInts.FindAsync(id);
                if (docOrderInt == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                db.DocOrderInts.Remove(docOrderInt);
                await db.SaveChangesAsync();


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
                    ID = docOrderInt.DocOrderIntID,
                    Msg = Classes.Language.Sklad.Language.msg19
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, "")
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

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

        private bool DocOrderIntExists(int id)
        {
            return db.DocOrderInts.Count(e => e.DocOrderIntID == id) > 0;
        }


        internal async Task<DocOrderInt> mPutPostdocOrderInt(
            DbConnectionSklad db,
            //System.Data.Entity.DbContextTransaction ts,
            DbConnectionSklad dbRead,
            DocOrderInt docOrderInt,
            EntityState entityState, //EntityState.Added, Modified

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region 0. Заполняем DirOrderIntContractors
            // - не находим - создаём новую
            // - находим - обновляем

            Models.Sklad.Dir.DirOrderIntContractor dirOrderIntContractor = new Models.Sklad.Dir.DirOrderIntContractor();

            string DirOrderIntContractorPhone = docOrderInt.DirOrderIntContractorPhone.Replace("+", "").ToLower();
            if (docOrderInt.DirOrderIntContractorName != null)
            {
                if (!String.IsNullOrEmpty(DirOrderIntContractorPhone))
                {
                    var queryDirOrderIntContractors = await
                        (
                            from x in db.DirOrderIntContractors
                            where x.DirOrderIntContractorPhone == DirOrderIntContractorPhone
                            select x
                        ).ToListAsync();
                    if (queryDirOrderIntContractors.Count() == 0)
                    {
                        dirOrderIntContractor = new Models.Sklad.Dir.DirOrderIntContractor();
                        dirOrderIntContractor.DirOrderIntContractorPhone = DirOrderIntContractorPhone;
                        dirOrderIntContractor.DirOrderIntContractorName = docOrderInt.DirOrderIntContractorName;
                        dirOrderIntContractor.QuantityOk = 0;
                        dirOrderIntContractor.QuantityFail = 0;
                        dirOrderIntContractor.QuantityCount = 0;

                        db.Entry(dirOrderIntContractor).State = EntityState.Added;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        dirOrderIntContractor = await db.DirOrderIntContractors.FindAsync(queryDirOrderIntContractors[0].DirOrderIntContractorID);
                        dirOrderIntContractor.DirOrderIntContractorName = docOrderInt.DirOrderIntContractorName;

                        db.Entry(dirOrderIntContractor).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
            }

            #endregion


            #region 1. Doc

            //Модель
            Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
            //Присваиваем значения
            doc.ListObjectID = ListObjectID;
            doc.IsImport = false;
            doc.NumberInt = docOrderInt.NumberInt;
            doc.NumberReal = docOrderInt.DocOrderIntID;
            doc.DirEmployeeID = field.DirEmployeeID;
            if (docOrderInt.PrepaymentSum > 0) doc.DirPaymentTypeID = docOrderInt.DirPaymentTypeID;
            else doc.DirPaymentTypeID = 1;
            doc.Payment = docOrderInt.Payment;

            //if (docOrderInt.DirContractorID == null || docOrderInt.DirContractorID < 0) { doc.DirContractorID = docOrderInt.DirContractorIDOrg; }  //docOrderInt.DirContractorID;
            //else { doc.DirContractorID = docOrderInt.DirContractorID; }
            doc.DirContractorID = docOrderInt.DirContractorIDOrg;

            doc.DirContractorIDOrg = docOrderInt.DirContractorIDOrg;
            doc.Discount = docOrderInt.Discount;
            doc.DirVatValue = docOrderInt.DirVatValue;
            doc.Base = docOrderInt.Base;
            doc.Description = docOrderInt.Description;
            doc.DocDate = docOrderInt.DocDate;
            //doc.DocDisc = docOrderInt.DocDisc;
            doc.Held = false;
            doc.DocID = docOrderInt.DocID;
            doc.DocIDBase = docOrderInt.DocIDBase;

            //Класс
            Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
            //doc = await docs.Save();
            await Task.Run(() => docs.Save());

            //Нужно вернуть "docOrderInt" со всем полями!
            docOrderInt.DocID = doc.DocID;

            #endregion

            #region 2. docOrderInt


            #region  Если Новый товар. Проверить наименование, если отличается, то: docOrderInt.DirNomenID = null;

            /*
            var queryDirNomenCat = await db.DirNomens.Where(x => x.Sub == docOrderInt.DirNomen2ID && x.DirNomenCategoryID == docOrderInt.DirNomenCategoryID).ToListAsync();
            if (queryDirNomenCat.Count() > 0) { docOrderInt.DirNomenID = queryDirNomenCat[0].DirNomenID; }
            else { docOrderInt.DirNomenID = null; }
            */

            if (docOrderInt.DirNomenID == null)
            {
                //Добавим товар при проведении документа поступление (при смене статуса на "Исполнен")
            }

            #endregion


            docOrderInt.DocID = doc.DocID;
            docOrderInt.DirOrderIntContractorID = dirOrderIntContractor.DirOrderIntContractorID;

            db.Entry(docOrderInt).State = entityState;
            await db.SaveChangesAsync();

            #region 2.1. UpdateNumberInt, если INSERT

            if (entityState == EntityState.Added && (docOrderInt.doc.NumberInt == null || docOrderInt.doc.NumberInt.Length == 0))
            {
                doc.NumberInt = docOrderInt.DocOrderIntID.ToString();
                doc.NumberReal = docOrderInt.DocOrderIntID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }
            else if (entityState == EntityState.Added)
            {
                doc.NumberReal = docOrderInt.DocOrderIntID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }

            #endregion


            #endregion

            #region 3. Касса или Банк

            //Только, если сумма больше 0
            if (docOrderInt.PrepaymentSum > 0)  //if (doc.Payment > 0)
            {
                //Касса
                if (doc.DirPaymentTypeID == 1)
                {
                    #region Касса

                    //1. По складу находим привязанную к нему Кассу
                    Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docOrderInt.DirWarehouseID);
                    int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                    //2. Заполняем модель "DocCashOfficeSum"
                    Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                    docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                    docCashOfficeSum.DirCashOfficeSumTypeID = 14;
                    docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                    docCashOfficeSum.DocID = doc.DocID;
                    docCashOfficeSum.DocXID = docOrderInt.DocOrderIntID;
                    docCashOfficeSum.DocCashOfficeSumSum = docOrderInt.PrepaymentSum; //doc.Payment;
                    docCashOfficeSum.Description = "";
                    docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;
                    docCashOfficeSum.Base = "Предоплата";
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
                    Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docOrderInt.DirWarehouseID);
                    int iDirBankID = dirWarehouse.DirBankID;

                    //2. Заполняем модель "DocBankSum"
                    Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                    docBankSum.DirBankID = iDirBankID;
                    docBankSum.DirBankSumTypeID = 13; //Изъятие из кассы на основании проведения приходной накладной №
                    docBankSum.DocBankSumDate = DateTime.Now;
                    docBankSum.DocID = doc.DocID;
                    docBankSum.DocXID = docOrderInt.DocOrderIntID;
                    docBankSum.DocBankSumSum = docOrderInt.PrepaymentSum; //doc.Payment;
                    docBankSum.Description = "";
                    docBankSum.DirEmployeeID = field.DirEmployeeID;
                    docBankSum.Base = "Предоплата";
                    docBankSum.Discount = doc.Discount;

                    //3. Пишем в Банк
                    Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                    docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum(db, docBankSum, EntityState.Added));

                    #endregion
                }
            }

            #endregion

            #region 4. SMS

            string Msg = "";

            if (docOrderInt.DirOrderIntTypeID == 1 && sysSetting.SmsOrderInt5)
            {
                //Находим Phone точки на которую перемещаем:
                int DirWarehouseID = docOrderInt.DirWarehouseID;
                Models.Sklad.Dir.DirWarehouse dirWarehouseTo = await db.DirWarehouses.FindAsync(DirWarehouseID);
                if (dirWarehouseTo.Phone != null && dirWarehouseTo.Phone.Length >= 5)
                {
                    //Находим "DirNomenNameFull"
                    Models.Sklad.Dir.DirServiceNomen dirServiceNomen = await db.DirServiceNomens.FindAsync(docOrderInt.DirServiceNomenID);
                    Models.Sklad.Dir.DirNomen dirNomen1 = await db.DirNomens.FindAsync(docOrderInt.DirNomen1ID);
                    Models.Sklad.Dir.DirNomen dirNomen2 = await db.DirNomens.FindAsync(docOrderInt.DirNomen2ID);
                    string DirNomenName = docOrderInt.DirNomenName;
                    if (docOrderInt.DirNomenID != null)
                    {
                        Models.Sklad.Dir.DirNomen dirNomen = await db.DirNomens.FindAsync(docOrderInt.DirNomenID);
                        DirNomenName = dirNomen.DirNomenName;
                    }

                    string DirNomenNameFull = dirServiceNomen.DirServiceNomenName + "-" + dirNomen1.DirNomenName + "-" + dirNomen2.DirNomenName + "-" + DirNomenName;

                    //Получаем DirSmsTemplateMsg из DirSmsTemplates
                    Models.Sklad.Dir.DirSmsTemplate dirSmsTemplate = await db.DirSmsTemplates.FindAsync(5);
                    dirSmsTemplate.DirSmsTemplateMsg = dirSmsTemplate.DirSmsTemplateMsg.Replace("[[[ТоварНаименование]]]", DirNomenNameFull);

                    PartionnyAccount.Classes.SMS.infobip_com infobip_com = new Classes.SMS.infobip_com();
                    string res = infobip_com.Send(sysSetting, dirWarehouseTo.Phone, dirSmsTemplate.DirSmsTemplateMsg);

                    Msg = "Так же отправлено SMS. Результат: " + res;
                }
            }

            #endregion

            #region 5. Log

            logOrderInt.DocOrderIntID = docOrderInt.DocOrderIntID;
            logOrderInt.DirOrderIntLogTypeID = 1;
            logOrderInt.DirEmployeeID = field.DirEmployeeID;
            logOrderInt.DirOrderIntStatusID = docOrderInt.DirOrderIntStatusID;
            logOrderInt.Msg = Msg;

            await logOrderIntsController.mPutPostLogOrderInts(db, logOrderInt, EntityState.Added);

            #endregion


            #region n. Подтверждение транзакции *** *** *** *** *** *

            //ts.Commit(); //.Complete();

            #endregion


            return docOrderInt;
        }



        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocOrderInt docOrderInt,
            int id,
            int DirOrderIntStatusID,
            int DirPaymentTypeID,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            string sLogMsg = "";
            docOrderInt = await db.DocOrderInts.FindAsync(id);
            int? DirOrderIntStatusID_OLD = docOrderInt.DirOrderIntStatusID;


            #region Проверка, если предыдущий статус такой же на который меняем, то не писать в Лог

            //Исключение, т.к. если в Логе нет записей с сменой статуса получим Ошибку из-за "FirstAsync()"
            try
            {
                var query = await
                    (
                        from x in db.LogOrderInts
                        where x.DocOrderIntID == id && x.DirOrderIntStatusID != null
                        select new
                        {
                            LogOrderIntID = x.LogOrderIntID,
                            DirOrderIntStatusID = x.DirOrderIntStatusID
                        }
                    ).OrderByDescending(x => x.LogOrderIntID).FirstAsync();

                if (query.DirOrderIntStatusID == DirOrderIntStatusID)
                {
                    return false;
                }
            }
            catch (Exception ex) { }

            #endregion


            #region 1. Сохранение статуса в БД

            if (DirOrderIntStatusID_OLD < 40 && DirOrderIntStatusID == 40)
            {
                docOrderInt.PriceVAT = Convert.ToDouble(PriceRetailVAT);
                docOrderInt.PriceCurrency = Convert.ToDouble(PriceRetailCurrency);
            }

            docOrderInt.DirOrderIntStatusID = DirOrderIntStatusID;

            db.Entry(docOrderInt).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            //Готов к выдаче
            if (docOrderInt.DirOrderIntTypeID == 1 && docOrderInt.DirOrderIntStatusID == 35)
            {

                #region 4. SMS

                //string Msg = "";

                if (sysSetting.SmsOrderInt9)
                {
                    //Находим Phone точки на которую перемещаем:
                    int DirWarehouseID = docOrderInt.DirWarehouseID;
                    Models.Sklad.Dir.DirWarehouse dirWarehouseTo = await db.DirWarehouses.FindAsync(DirWarehouseID);
                    if (dirWarehouseTo.Phone != null && dirWarehouseTo.Phone.Length >= 5)
                    {
                        //Находим "DirNomenNameFull"
                        Models.Sklad.Dir.DirServiceNomen dirServiceNomen = await db.DirServiceNomens.FindAsync(docOrderInt.DirServiceNomenID);
                        Models.Sklad.Dir.DirNomen dirNomen1 = await db.DirNomens.FindAsync(docOrderInt.DirNomen1ID);
                        Models.Sklad.Dir.DirNomen dirNomen2 = await db.DirNomens.FindAsync(docOrderInt.DirNomen2ID);
                        string DirNomenName = docOrderInt.DirNomenName;
                        if (docOrderInt.DirNomenID != null)
                        {
                            Models.Sklad.Dir.DirNomen dirNomen = await db.DirNomens.FindAsync(docOrderInt.DirNomenID);
                            DirNomenName = dirNomen.DirNomenName;
                        }

                        string DirNomenNameFull = dirServiceNomen.DirServiceNomenName + "-" + dirNomen1.DirNomenName + "-" + dirNomen2.DirNomenName + "-" + DirNomenName;

                        //Получаем DirSmsTemplateMsg из DirSmsTemplates
                        Models.Sklad.Dir.DirSmsTemplate dirSmsTemplate = await db.DirSmsTemplates.FindAsync(9);
                        dirSmsTemplate.DirSmsTemplateMsg = dirSmsTemplate.DirSmsTemplateMsg.Replace("[[[ТоварНаименование]]]", DirNomenNameFull);

                        PartionnyAccount.Classes.SMS.infobip_com infobip_com = new Classes.SMS.infobip_com();
                        string res = infobip_com.Send(sysSetting, dirWarehouseTo.Phone, dirSmsTemplate.DirSmsTemplateMsg);

                        sLogMsg = "Так же отправлено SMS. Результат: " + res;
                    }
                }

                #endregion

            }


            if (DirOrderIntStatusID_OLD < 40 && DirOrderIntStatusID == 40)
            {
                //Алгоритм:
                //1. Новый товара
                //2. Создание Поступления
                //3. Запчасти

                #region 2.1. Создание документа поступление

                //Алгоритм
                //1. Для 1 и 2 просто приходуем на склад
                //2. Для 3 и 4: "1." + создаём запчасть в табличной части запчастей аппарата



                #region DirNomenID - создаём товар если новый!

                if (docOrderInt.DirNomenID == null)
                {
                    /*
                    //Наименование берём из Категории
                    int? DirNomenCategoryID = docOrderInt.DirNomenCategoryID;
                    Models.Sklad.Dir.DirNomenCategory dirNomenCategory = await db.DirNomenCategories.FindAsync(DirNomenCategoryID);

                    Models.Sklad.Dir.DirNomen dirNomen = new Models.Sklad.Dir.DirNomen();
                    dirNomen.DirNomenCategoryID = docOrderInt.DirNomenCategoryID;
                    dirNomen.DirNomenName = dirNomenCategory.DirNomenCategoryName;
                    dirNomen.DirNomenNameFull = dirNomenCategory.DirNomenCategoryName;
                    dirNomen.DirNomenTypeID = 1;
                    dirNomen.Sub = docOrderInt.DirNomen2ID;

                    db.Entry(dirNomen).State = EntityState.Added;
                    await db.SaveChangesAsync();


                    //Сохраняем полученный 
                    docOrderInt.DirNomenID = dirNomen.DirNomenID;
                    */


                    Models.Sklad.Dir.DirNomen dirNomen = new Models.Sklad.Dir.DirNomen();
                    //dirNomen.DirNomenCategoryID = docOrderInt.DirNomenCategoryID;
                    dirNomen.DirNomenName = docOrderInt.DirNomenName;
                    dirNomen.DirNomenNameFull = docOrderInt.DirNomenName;
                    dirNomen.DirNomenTypeID = 1;
                    dirNomen.Sub = docOrderInt.DirNomen2ID;

                    db.Entry(dirNomen).State = EntityState.Added;
                    await db.SaveChangesAsync();


                    //Сохраняем полученный 
                    docOrderInt.DirNomenID = dirNomen.DirNomenID;

                }

                #endregion


                #region Находим все нужные документы (DocServicePurch, DocSecondHandPurch)

                Models.Sklad.Doc.DocServicePurch docServicePurch = new DocServicePurch();
                Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = new DocSecondHandPurch();

                int? DocIDBase = docOrderInt.doc.DocIDBase;
                if (docOrderInt.DirOrderIntTypeID == 3)
                {
                    var query1 = await
                        (
                            from x in db.DocServicePurches
                            where x.DocID == DocIDBase
                            select x
                        ).ToListAsync();
                    if (query1.Count() > 0)
                    {
                        int? DocServicePurchID = query1[0].DocServicePurchID;
                        docServicePurch = await db.DocServicePurches.FindAsync(DocServicePurchID);
                    }
                }
                else if (docOrderInt.DirOrderIntTypeID == 4)
                {
                    var query1 = await
                        (
                            from x in db.DocSecondHandPurches
                            where x.DocID == DocIDBase
                            select x
                        ).ToListAsync();
                    if (query1.Count() > 0)
                    {
                        int? DocSecondHandPurchID = query1[0].DocSecondHandPurchID;
                        docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(DocSecondHandPurchID);
                    }
                }

                #endregion


                #region DocPurches and DocPurchTab


                #region DocPurches

                Models.Sklad.Doc.DocPurch docPurch = new DocPurch();

                docPurch.Base = "Создано на основании заказа №" + docOrderInt.DocOrderIntID + " для модуля ";
                if (docOrderInt.DirOrderIntTypeID == 3)
                {
                    docPurch.Base += "СЦ аппарат №" + docServicePurch.DocServicePurchID;
                }
                else if (docOrderInt.DirOrderIntTypeID == 4)
                {
                    docPurch.Base += "БУ аппарат №" + docSecondHandPurch.DocSecondHandPurchID;
                }
                docPurch.DirContractorID = docOrderInt.doc.DirContractorID;
                docPurch.DirContractorIDOrg = docOrderInt.doc.DirContractorIDOrg;
                docPurch.DirEmployeeID = field.DirEmployeeID;
                docPurch.DirPaymentTypeID = docOrderInt.DirPaymentTypeID;
                docPurch.DirVatValue = 0;
                docPurch.DirWarehouseID = docOrderInt.DirWarehouseID;
                docPurch.Discount = 0;
                docPurch.DocDate = DateTime.Now;
                docPurch.DocIDBase = docOrderInt.doc.DocID;
                docPurch.Held = true;
                docPurch.IsImport = false;
                docPurch.Payment = 0;

                #endregion


                #region DocPurchTab

                Models.Sklad.Doc.DocPurchTab docPurchTab = new DocPurchTab();

                docPurchTab.DirCharColourID = docOrderInt.DirCharColourID;
                docPurchTab.DirCharMaterialID = docOrderInt.DirCharMaterialID;
                docPurchTab.DirCharNameID = docOrderInt.DirCharNameID;
                docPurchTab.DirCharSeasonID = docOrderInt.DirCharSeasonID;
                docPurchTab.DirCharSexID = docOrderInt.DirCharSexID;
                docPurchTab.DirCharSizeID = docOrderInt.DirCharSizeID;
                docPurchTab.DirCharStyleID = docOrderInt.DirCharStyleID;
                docPurchTab.DirCharTextureID = docOrderInt.DirCharTextureID;
                docPurchTab.DirContractorID = Convert.ToInt32(DirContractorID); //docOrderInt.DirContractorID;

                docPurchTab.DirCurrencyID = docOrderInt.DirCurrencyID;
                docPurchTab.DirCurrencyMultiplicity = docOrderInt.DirCurrencyMultiplicity;
                docPurchTab.DirCurrencyRate = docOrderInt.DirCurrencyRate;
                docPurchTab.DirNomenID = Convert.ToInt32(docOrderInt.DirNomenID);

                docPurchTab.PriceCurrency = Convert.ToDouble(PriceCurrency);
                docPurchTab.PriceVAT = Convert.ToDouble(PriceVAT);

                docPurchTab.MarkupRetail = Convert.ToDouble(MarkupRetail);
                docPurchTab.PriceRetailVAT = Convert.ToDouble(PriceRetailVAT);
                docPurchTab.PriceRetailCurrency = Convert.ToDouble(PriceRetailCurrency);

                docPurchTab.MarkupWholesale = Convert.ToDouble(MarkupWholesale);
                docPurchTab.PriceWholesaleVAT = Convert.ToDouble(PriceWholesaleVAT);
                docPurchTab.PriceWholesaleCurrency = Convert.ToDouble(PriceWholesaleCurrency);

                docPurchTab.MarkupIM = Convert.ToDouble(MarkupIM);
                docPurchTab.PriceIMVAT = Convert.ToDouble(PriceIMVAT);
                docPurchTab.PriceIMCurrency = Convert.ToDouble(PriceIMCurrency);

                docPurchTab.Quantity = docOrderInt.Quantity;

                Models.Sklad.Doc.DocPurchTab[] docPurchTabCollection = new DocPurchTab[1];
                docPurchTabCollection[0] = docPurchTab;


                Controllers.Sklad.Doc.DocPurches.DocPurchesController docPurchesController = new DocPurches.DocPurchesController();
                docPurch = await Task.Run(() =>
                    docPurchesController.mPutPostDocPurch(
                        db,
                        dbRead,
                        "held", //UO_Action
                        sysSetting,
                        docPurch,
                        EntityState.Added,
                        docPurchTabCollection,

                        field
                    )
                );

                #endregion


                #endregion



                if (docOrderInt.DirOrderIntTypeID >= 3)
                {

                    #region RemParty - находим только что созданную партию (партия всего одна)

                    int? DocPurch_DocID = docPurch.DocID;

                    var queryRemParty = await
                        (
                            from x in db.RemParties
                            where x.DocID == DocPurch_DocID
                            select x
                        ).ToListAsync();

                    if (queryRemParty.Count() == 0)
                    {
                        throw new System.InvalidOperationException("Ошибка выборки только-что созданой партии! Сообщите разработчику проекта!");
                    }

                    #endregion


                    #region Запчасти

                    if (docOrderInt.DirOrderIntTypeID == 3)
                    {
                        Models.Sklad.Doc.DocServicePurch2Tab docServicePurch2Tab = new DocServicePurch2Tab();
                        docServicePurch2Tab.DirCurrencyID = docOrderInt.DirCurrencyID;
                        docServicePurch2Tab.DirCurrencyMultiplicity = docOrderInt.DirCurrencyMultiplicity;
                        docServicePurch2Tab.DirCurrencyRate = docOrderInt.DirCurrencyRate;
                        docServicePurch2Tab.DirEmployeeID = docOrderInt.doc.DirEmployeeID;
                        docServicePurch2Tab.DirNomenID = docOrderInt.DirNomenID;
                        docServicePurch2Tab.DirNomenName = docOrderInt.DirNomenName;
                        docServicePurch2Tab.DocServicePurchID = Convert.ToInt32(docServicePurch.DocServicePurchID);
                        docServicePurch2Tab.PriceVAT = Convert.ToDouble(PriceRetailVAT);
                        docServicePurch2Tab.PriceCurrency = Convert.ToDouble(PriceRetailCurrency);
                        docServicePurch2Tab.RemPartyID = Convert.ToInt32(queryRemParty[0].RemPartyID);
                        docServicePurch2Tab.TabDate = DateTime.Now;


                        Controllers.Sklad.Doc.DocServicePurches.DocServicePurch2TabsController docServicePurch2TabsController = new DocServicePurches.DocServicePurch2TabsController();
                        docServicePurch2Tab = await Task.Run(() => docServicePurch2TabsController.mPutPostDocServicePurch2Tab(db, docServicePurch, docServicePurch2Tab, field));

                    }
                    else if (docOrderInt.DirOrderIntTypeID == 4)
                    {
                        Models.Sklad.Doc.DocSecondHandPurch2Tab docSecondHandPurch2Tab = new DocSecondHandPurch2Tab();
                        docSecondHandPurch2Tab.DirCurrencyID = docOrderInt.DirCurrencyID;
                        docSecondHandPurch2Tab.DirCurrencyMultiplicity = docOrderInt.DirCurrencyMultiplicity;
                        docSecondHandPurch2Tab.DirCurrencyRate = docOrderInt.DirCurrencyRate;
                        docSecondHandPurch2Tab.DirEmployeeID = docOrderInt.doc.DirEmployeeID;
                        docSecondHandPurch2Tab.DirNomenID = docOrderInt.DirNomenID;
                        docSecondHandPurch2Tab.DirNomenName = docOrderInt.DirNomenName;
                        docSecondHandPurch2Tab.DocSecondHandPurchID = Convert.ToInt32(docSecondHandPurch.DocSecondHandPurchID);
                        docSecondHandPurch2Tab.PriceCurrency = docOrderInt.PriceCurrency;
                        docSecondHandPurch2Tab.PriceVAT = docOrderInt.PriceVAT;
                        docSecondHandPurch2Tab.RemPartyID = Convert.ToInt32(queryRemParty[0].RemPartyID);
                        docSecondHandPurch2Tab.TabDate = DateTime.Now;


                        Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurch2TabsController docSecondHandPurch2TabsController = new DocSecondHandPurches.DocSecondHandPurch2TabsController();
                        docSecondHandPurch2Tab = await Task.Run(() => docSecondHandPurch2TabsController.mPutPostDocSecondHandPurch2Tab(db, docSecondHandPurch2Tab, field));
                    }

                    #endregion

                }

                #endregion

            }
            else if (DirOrderIntStatusID_OLD == 40 && DirOrderIntStatusID != 40)
            {

                #region Возврат на доработку - НЕ доделал!!!


                #region Находим все нужные документы (DocServicePurch, DocSecondHandPurch)

                Models.Sklad.Doc.DocServicePurch docServicePurch = new DocServicePurch();
                Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = new DocSecondHandPurch();

                int? DocIDBase = docOrderInt.doc.DocIDBase;
                if (docOrderInt.DirOrderIntTypeID == 3)
                {
                    var query1 = await
                        (
                            from x in db.DocServicePurches
                            where x.DocID == DocIDBase
                            select x
                        ).ToListAsync();
                    if (query1.Count() > 0)
                    {
                        int? DocServicePurchID = query1[0].DocServicePurchID;
                        docServicePurch = await db.DocServicePurches.FindAsync(DocServicePurchID);
                    }
                }
                else if (docOrderInt.DirOrderIntTypeID == 4)
                {
                    var query1 = await
                        (
                            from x in db.DocSecondHandPurches
                            where x.DocID == DocIDBase
                            select x
                        ).ToListAsync();
                    if (query1.Count() > 0)
                    {
                        int? DocSecondHandPurchID = query1[0].DocSecondHandPurchID;
                        docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(DocSecondHandPurchID);
                    }
                }

                #endregion


                //Алгоритм:

                //1. Проверки
                //   1.1 Если "docOrderInt.DirOrderIntTypeID > 3", то проверяем статус аппарата в модулях СЦ и БУ
                //       - если статус != 5 (Ожидает запчастей), то выдать Эксепшин
                //   1.2. Если повторный ремонт, то установлена дата платы запчасти "PayDate is NOT NULL"
                //        то выдать Эксепшин

                //2. Выдаём сообщение, что возвращать Заказ на доработку нельзя! Можно:
                //   - Удалить вручную запчасть (партия вернётся на склад)
                //   - Поступление: снять вручную проводку и удалить документ



                #region 1.1 Если "docOrderInt.DirOrderIntTypeID > 3", то проверяем статус аппарата в модулях СЦ и БУ
                //       - если статус != 5 (Ожидает запчастей), то выдать Эксепшин
                if (docOrderInt.DirOrderIntTypeID == 3)
                {
                    if (docServicePurch.DirServiceStatusID != 5)
                    {
                        throw new System.InvalidOperationException(
                            "Сменить статус не возможно! Статус аппарата №" + docServicePurch.DocServicePurchID.ToString() +
                            " в моделуе СЦ должен быть 'Ожидает запчастей!' Текущий статус '" + docServicePurch.dirServiceStatus.DirServiceStatusName + "'"
                        );
                    }
                }
                else if (docOrderInt.DirOrderIntTypeID == 4)
                {
                    if (docSecondHandPurch.DirSecondHandStatusID != 5)
                    {
                        throw new System.InvalidOperationException(
                            "Сменить статус не возможно! Статус аппарата №" + docSecondHandPurch.DocSecondHandPurchID +
                            " в моделуе СЦ должен быть 'Ожидает запчастей!' Текущий статус '" + docSecondHandPurch.dirSecondHandStatus.DirSecondHandStatusName + "'"
                        );
                    }
                }

                #endregion



                #region 1.2. Если повторный ремонт, то установлена дата платы запчасти "PayDate is NOT NULL"
                //        то выдать Эксепшин
                //(Найти партию и по партии найти запчасть и проверить её PayDate)
                //docPurch.DocIDBase = docOrderInt.doc.DocID;

                int?
                    DocIDBase2 = docOrderInt.doc.DocID,
                    DocID = 0, DocPurchID = 0,
                    RemPartyID = 0;

                #region Поступление === === === === === === ===

                var queryDocPurch = await
                    (
                        from x in db.DocPurches
                        where x.doc.DocIDBase == DocIDBase2
                        select x
                    ).ToListAsync();

                if (queryDocPurch.Count() > 0)
                {
                    DocID = queryDocPurch[0].DocID;
                    DocPurchID = queryDocPurch[0].DocPurchID;

                    if (docOrderInt.DirOrderIntTypeID >= 3)
                    {
                        #region Партия === === === === === === ===

                        var queryRemParties = await
                            (
                                from x in db.RemParties
                                where x.DocID == DocID
                                select x
                            ).ToListAsync();
                        if (queryRemParties.Count() > 0)
                        {
                            RemPartyID = queryRemParties[0].RemPartyID;

                            #region Запчасть === === === === === === ===

                            if (docOrderInt.DirOrderIntTypeID == 3)
                            {

                                #region 3 === === === === === === ===

                                int? DocServicePurchID = docServicePurch.DocServicePurchID;
                                var queryDocServicePurch2Tabs = await
                                (
                                    from x in db.DocServicePurch2Tabs
                                    where x.RemPartyID == RemPartyID && x.DocServicePurchID == DocServicePurchID
                                    select x
                                ).ToListAsync();
                                if (queryDocServicePurch2Tabs.Count() > 0)
                                {
                                    if (queryDocServicePurch2Tabs[0].PayDate != null)
                                    {
                                        //!!! !!! !!! !!! !!! !!! !!!
                                        throw new System.InvalidOperationException(
                                            "Сменить статус не возможно! Для аппарата №" + docServicePurch.DocServicePurchID.ToString() +
                                            " в моделуе СЦ запчасть уже оплачена! Скорее всего аппарат уже выдан или на повторном ремонте!"
                                        );
                                        //!!! !!! !!! !!! !!! !!! !!!
                                    }
                                }
                                else { throw new System.InvalidOperationException("Ошибка! Запчасть не найдена (возможно была возвращена на склад)! Обратитесь к разработчику!"); }

                                #endregion

                            }
                            else if (docOrderInt.DirOrderIntTypeID == 4)
                            {

                                #region 4 === === === === === === ===

                                int? DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                                var queryDocSecondHandPurch2Tabs = await
                                (
                                    from x in db.DocSecondHandPurch2Tabs
                                    where x.RemPartyID == RemPartyID && x.DocSecondHandPurchID == DocSecondHandPurchID
                                    select x
                                ).ToListAsync();
                                if (queryDocSecondHandPurch2Tabs.Count() > 0)
                                {
                                    if (queryDocSecondHandPurch2Tabs[0].PayDate != null)
                                    {
                                        //!!! !!! !!! !!! !!! !!! !!!
                                        throw new System.InvalidOperationException(
                                            "Сменить статус не возможно! Для аппарата №" + docSecondHandPurch.DocSecondHandPurchID.ToString() +
                                            " в моделуе СЦ запчасть уже оплачена! Скорее всего аппарат уже выдан или на повторном ремонте!"
                                        );
                                        //!!! !!! !!! !!! !!! !!! !!!
                                    }
                                }
                                else { throw new System.InvalidOperationException("Ошибка! Запчасть не найдена (возможно была возвращена на склад)! Обратитесь к разработчику!"); }

                                #endregion

                            }

                            #endregion

                        }
                        else { throw new System.InvalidOperationException("Ошибка! Партия не найдена! Обратитесь к разработчику!"); }

                        #endregion
                    }
                }
                else { throw new System.InvalidOperationException( "Ошибка! Документ Поступление не найден! Обратитесь к разработчику!" ); }

                #endregion


                #endregion



                #region 2. Выдаём сообщение, что возвращать Заказ на доработку нельзя! Можно:
                //   - Удалить вручную запчасть (партия вернётся на склад)
                //   - Поступление: снять вручную проводку и удалить документ


                if (docOrderInt.DirOrderIntTypeID == 3)
                {
                    throw new System.InvalidOperationException(
                        "На данный момент смена статуса заказа, после 'Исполнен' не возможно! Рекомендуем:<br />" +
                        "1) Найти в модуле СЦ ремонт №" + docServicePurch.DocServicePurchID + " и удалить вручную запчасть<br />" +
                        "2) В поступлении №" + DocPurchID + " снять вручную проводку и удалить документ"
                    );
                }
                else if (docOrderInt.DirOrderIntTypeID == 4)
                {
                    throw new System.InvalidOperationException(
                        "На данный момент смена статуса заказа, после 'Исполнен' не возможно! Рекомендуем:<br />" +
                        "1) Найти в модуле БУ ремонт №" + docSecondHandPurch.DocSecondHandPurchID + " и удалить вручную запчасть<br />" +
                        "2) В поступлении №" + DocPurchID + " снять вручную проводку и удалить документ"
                    );
                }
                else
                {
                    throw new System.InvalidOperationException(
                        "На данный момент смена статуса заказа, после 'Исполнен' не возможно! Рекомендуем:<br />" +
                        "1) В поступлении №"+ DocPurchID + " снять вручную проводку и удалить документ"
                    );
                }

                #endregion


                #endregion

            }



            #region 3. Лог


            #region Пишем в Лог о смене статуса и мастера, если такое было

            logOrderInt.DocOrderIntID = id;
            logOrderInt.DirOrderIntLogTypeID = 1; //Смена статуса
            //if (!bDirOrderIntLogTypeID9) logOrderInt.DirOrderIntLogTypeID = 1; //Смена статуса
            //else logOrderInt.DirOrderIntLogTypeID = 9; //Возврат по гарантии
            logOrderInt.DirEmployeeID = field.DirEmployeeID;
            logOrderInt.DirOrderIntStatusID = DirOrderIntStatusID;
            logOrderInt.Msg = sLogMsg;

            await logOrderIntsController.mPutPostLogOrderInts(db, logOrderInt, EntityState.Added);

            #endregion


            #endregion



            return true;
        }



        #endregion
    }
}