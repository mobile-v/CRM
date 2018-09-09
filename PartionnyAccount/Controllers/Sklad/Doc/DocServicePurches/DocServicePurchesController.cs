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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocServicePurches
{
    public class DocServicePurchesController : ApiController
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
        Models.Sklad.Log.LogService logService = new Models.Sklad.Log.LogService(); Controllers.Sklad.Log.LogServicesController logServicesController = new Log.LogServicesController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 40;

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
            public int? FilterType; // == DirServiceStatusID
            public int? DirWarehouseID;
            public int? DirWarehouseIDOnly;
            public DateTime? DateS;
            public DateTime? DatePo;
            public int? DirServiceStatusIDS;
            public int? DirServiceStatusIDPo;
            public int iTypeService; //1 - Приёмка, 2 - Мастерская, 3 - Выдача
            public int? DirEmployeeID;

            //В Приёмке, для выборка списка уже принятых аппаратов!
            public int? DirServiceContractorID;
            public int? DirServiceNomenID;

            //Отобразить "Архив"
            public int? DocServicePurchID;
            public string collectionWrapper = "";
        }
        // GET: api/DocServicePurches
        public async Task<IHttpActionResult> GetDocServicePurches(HttpRequestMessage request)
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


                //1. Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //2. Получаем "RightDocServiceWorkshopsOnlyUsers"
                //bool bRightCheck = await Task.Run(() => accessRight.AccessCheck(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurchesWarehouseAllCheck"));
                //2.1. Админ точки?
                //... ниже, т.к. нам нужен склад ...



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
                _params.limit = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); if (_params.limit == 0) _params.limit = 999999999; //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value); if (_params.page == 0) _params.page = 1; //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.GroupID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pGroupID", true) == 0).Value); //Кликнули по группе
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск
                _params.FilterType = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "FilterType", true) == 0).Value);
                _params.DirServiceStatusIDS = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceStatusIDS", true) == 0).Value);
                _params.DirServiceStatusIDPo = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceStatusIDPo", true) == 0).Value);
                _params.DirEmployeeID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value);
                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);
                _params.DirWarehouseIDOnly = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseIDOnly", true) == 0).Value);
                _params.collectionWrapper = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "collectionWrapper", true) == 0).Value;

                //В Приёмке, для выборка списка уже принятых аппаратов!
                _params.DirServiceContractorID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceContractorID", true) == 0).Value);
                _params.DirServiceNomenID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceNomenID", true) == 0).Value);



                //*** *** ***
                //2.1. Админ точки?
                bool iRightCheck = await Task.Run(() => accessRight.AccessIsAdmin(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, _params.DirWarehouseID));
                //*** *** ***

                //Есть доступ для Выдачи аппарата (Статус 7 и 8 для вкладки Выдача)
                bool isExtradition2 = false;
                int iRight2 = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceOutputs"));
                if (iRight2 < 3 && _params.DirServiceStatusIDS == 7 && _params.DirServiceStatusIDPo == 8) { isExtradition2 = true; }

                bool isExtradition3 = false;
                int iRight3 = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurchesExtradition"));
                if (iRight3 < 3 && _params.DirServiceStatusIDS == 7 && _params.DirServiceStatusIDPo == 8) { isExtradition3 = true; }


                //Архив
                _params.DocServicePurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocServicePurchID", true) == 0).Value);

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


                //Находим все точки к которым есть доступ на просмотр ремонтов СЦ
                List<int?> lstWarID = new List<int?>();
                var queryDirEmployeeWarehouses = await
                    (
                        from x in db.DirEmployeeWarehouse
                        where x.DirEmployeeID == field.DirEmployeeID && x.WarehouseAll == true
                        select x
                    ).ToListAsync();
                for (int i = 0; i < queryDirEmployeeWarehouses.Count(); i++)
                {
                    lstWarID.Add(queryDirEmployeeWarehouses[i].DirWarehouseID);
                }

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.DocServicePurches

                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        join docSecondHandPurches1 in db.DocSecondHandPurches on x.DocID equals docSecondHandPurches1.DocIDService into docSecondHandPurches2
                        from docSecondHandPurches in docSecondHandPurches2.DefaultIfEmpty()

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

                            DocServicePurchID = x.DocServicePurchID,
                            DirContractorName = x.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = x.doc.dirContractorOrg.DirContractorID,
                            //DirContractorNameOrg = x.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = x.dirWarehouse.DirWarehouseID,
                            DirWarehouseIDPurches = x.DirWarehouseIDPurches,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,
                            ProblemClientWords = x.ProblemClientWords,
                            SerialNumber = x.SerialNumber,
                            DirServiceStatusID = x.DirServiceStatusID, Status = x.DirServiceStatusID,
                            DirServiceStatusName = x.dirServiceStatus.DirServiceStatusName,
                            DirServiceContractorName = x.DirServiceContractorName,
                            DirServiceContractorPhone = x.DirServiceContractorPhone,
                            UrgentRepairs = x.UrgentRepairs,
                            PriceVAT = x.PriceVAT, PriceCurrency = x.PriceVAT,
                            DateDone = x.DateDone.ToString(),
                            PrepaymentSum = x.PrepaymentSum == null ? 0 : x.PrepaymentSum,
                            Sums1 = x.Sums1 == null ? 0 : x.Sums1,
                            Sums2 = x.Sums2 == null ? 0 : x.Sums2,

                            //Оплата
                            Payment = x.doc.Payment,
                            //Мастер
                            DirEmployeeIDMaster = x.DirEmployeeIDMaster,
                            DirEmployeeNameMaster = x.dirEmployee.DirEmployeeName,
                            FromGuarantee = x.FromGuarantee,
                            ServiceTypeRepair = x.ServiceTypeRepair,

                            DirServiceContractorID = x.DirServiceContractorID,
                            //QuantityOk = x.dirServiceContractor.QuantityOk,
                            //QuantityFail = x.dirServiceContractor.QuantityFail,
                            //QuantityCount = x.dirServiceContractor.QuantityCount

                            //Дата, когда аппарат переместили на выдачу
                            IssuanceDate = x.IssuanceDate.ToString(),
                            Sums = x.Sums,


                            //!!! Дата выдачи !!!
                            DateVIDACHI = x.DirServiceStatusID == 9 ? x.DateStatusChange : 
                            null,


                            //Перемещён в БУ
                            InSecondHand = x.InSecondHand,
                            InSecondHandString = x.InSecondHand == true ? "В Б/У" : "",
                            DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,


                            Exist = 1,
                            ExistName = "Присутствует",

                        }
                        
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Отобразить "Предыдущие ремонты"

                if (_params.DocServicePurchID != null && _params.DocServicePurchID > 0)
                {
                    //1. Получаем "DirServiceContractorID" по "DocServicePurchID"
                    //2.Добавляем условие в запрос

                    Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(_params.DocServicePurchID);
                    int? DirServiceContractorID = docServicePurch.DirServiceContractorID;
                    if (DirServiceContractorID == null) { DirServiceContractorID = 0; }

                    query = query.Where(x => x.DirServiceContractorID == DirServiceContractorID && x.DocServicePurchID != _params.DocServicePurchID);
                }

                #endregion



                #region dirEmployee: dirEmployee.DirWarehouseID

                if (_params.DirWarehouseIDOnly > 0)
                {
                    if (field.DirEmployeeID != 1)
                    {
                        //Используется "lstWarID" - что бы не отобразить не доступную точку
                        query = query.Where(x => lstWarID.Contains(x.DirWarehouseID) && (x.DirWarehouseID == _params.DirWarehouseIDOnly || x.DirWarehouseIDPurches == _params.DirWarehouseIDOnly));
                    }
                    else
                    {
                        query = query.Where(x => x.DirWarehouseID == _params.DirWarehouseIDOnly || x.DirWarehouseIDPurches == _params.DirWarehouseIDOnly);
                    }
                }
                else 
                {
                    if (field.DirEmployeeID != 1)
                    {
                        if (queryDirEmployeeWarehouses.Count() > 0)
                        {
                            query = query.Where(x => lstWarID.Contains(x.DirWarehouseID) || x.DirWarehouseID == _params.DirWarehouseID);
                        }
                        else
                        {
                            if (_params.DirWarehouseID != null && _params.DirWarehouseID > 0)
                            {
                                query = query.Where(x => x.DirWarehouseID == _params.DirWarehouseID);
                            }
                        }
                    }
                }

                #endregion


                #region dirEmployee: dirEmployee.DirContractorIDOrg

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
                    query = query.Where(x => x.DirServiceStatusID == _params.FilterType);
                }

                #endregion

                #region Фильтр DirServiceStatusID S и Po

                if (_params.DirServiceStatusIDS != _params.DirServiceStatusIDPo)
                {
                    if (_params.DirServiceStatusIDS > 0 && _params.DirServiceStatusIDPo > 0) { query = query.Where(x => x.DirServiceStatusID >= _params.DirServiceStatusIDS && x.DirServiceStatusID <= _params.DirServiceStatusIDPo); }
                    else
                    {
                        if (_params.DirServiceStatusIDS > 0) query = query.Where(x => x.DirServiceStatusID >= _params.DirServiceStatusIDS);
                        if (_params.DirServiceStatusIDPo > 0) query = query.Where(x => x.DirServiceStatusID <= _params.DirServiceStatusIDPo);
                    }
                }
                else
                {
                    if (_params.DirServiceStatusIDS > 0) query = query.Where(x => x.DirServiceStatusID == _params.DirServiceStatusIDS);
                }

                if (_params.DirEmployeeID > 0)
                {
                    query = query.Where(x => x.DirEmployeeID <= _params.DirEmployeeID && x.DirServiceStatusID < 7);
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


                //!!! Новое !!!
                #region Видит только свои ремонты (Админ Точки видит все ремонты точки, мастер только свои)

                //Везде кроме Выдачи
                /*if (!iRightCheck && _params.DirServiceStatusIDPo != 9)
                {
                    if (!isExtradition2)
                    {
                        query = query.Where(x => x.DirEmployeeIDMaster == field.DirEmployeeID);
                    }
                }*/


                //Везде кроме Выдачи
                if (!iRightCheck && _params.DirServiceStatusIDPo != 8)
                {
                    query = query.Where(x => x.DirEmployeeIDMaster == field.DirEmployeeID);
                }


                //Выдача
                if (!iRightCheck && _params.DirServiceStatusIDPo == 8)
                {
                    //Выдача своих
                    if (!isExtradition2)
                    {
                        query = query.Where(x => x.DirEmployeeIDMaster == 0);
                    }
                    else if (!isExtradition3)
                    {
                        query = query.Where(x => x.DirEmployeeIDMaster == field.DirEmployeeID);
                    }
                    else //if (isExtradition3)
                    {
                        //...
                    }
                }


                #endregion


                #region Поиск

                if (!String.IsNullOrEmpty(_params.parSearch))
                {
                    //Проверяем число ли это
                    Int32 iNumber32;
                    bool bResult32 = Int32.TryParse(_params.parSearch, out iNumber32);
                    Int64 iNumber64;
                    bool bResult64 = Int64.TryParse(_params.parSearch, out iNumber64);


                    //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                    if (bResult32 && _params.parSearch.IndexOf("+") == -1)
                    {
                        query = query.Where(x => x.DocServicePurchID == iNumber32);
                    }
                    else
                    {
                        //Ищим в справочнике "DirServiceContractors"

                        if (bResult64)
                        {
                            //query = query.Where(x => x.DirServiceContractorPhone == _params.parSearch);

                            //Ищим в справочнике "DirServiceContractors"
                            int DirServiceContractorID = 0;
                            var queryDirServiceContractors = await
                                (
                                    from x in db.DirServiceContractors
                                    where x.DirServiceContractorPhone == _params.parSearch
                                    select new
                                    {
                                        x.DirServiceContractorID
                                    }
                                ).ToListAsync();
                            if (queryDirServiceContractors.Count() > 0)
                            {
                                DirServiceContractorID = Convert.ToInt32(queryDirServiceContractors[0].DirServiceContractorID);
                            }

                            query = query.Where(x => x.DirServiceContractorID == DirServiceContractorID);
                        }
                        //Иначе, только текстовые поля
                        else
                        {
                            _params.parSearch = _params.parSearch.Replace("+", "");
                            //query = query.Where(x => x.DirServiceContractorName.Contains(_params.parSearch));

                            //Ищим в справочнике "DirServiceContractors"
                            //int DirServiceContractorID = 0;
                            var queryDirServiceContractors = await
                                (
                                    from x in db.DirServiceContractors
                                    where x.DirServiceContractorName.Contains(_params.parSearch)
                                    select x.DirServiceContractorID
                                ).ToListAsync();
                            /*if (queryDirServiceContractors.Count() > 0)
                            {
                                DirServiceContractorID = Convert.ToInt32(queryDirServiceContractors[0].DirServiceContractorID);
                            }*/

                            query = query.Where(x => queryDirServiceContractors.Contains(x.DirServiceContractorID));
                        }
                    }
                    
                }

                #endregion


                #region DirServiceContractorID и DirServiceNomenID

                if (_params.DirServiceContractorID > 0 && _params.DirServiceNomenID > 0)
                {
                    query = query.Where(x => x.DirServiceContractorID == _params.DirServiceContractorID && x.DirServiceNomenID == _params.DirServiceNomenID);
                }
                else if (_params.DirServiceContractorID > 0)
                {
                    query = query.Where(x => x.DirServiceContractorID == _params.DirServiceContractorID);
                }
                else if (_params.DirServiceNomenID > 0)
                {
                    query = query.Where(x => x.DirServiceNomenID == _params.DirServiceNomenID);
                }

                #endregion


                #region OrderBy и Лимит

                //query = query.OrderByDescending(x => x.DocDate).Skip(_params.Skip).Take(_params.limit);
                query = query.OrderByDescending(x => x.DocServicePurchID); //.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                /*
                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.DocServicePurches.Where(x => x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo).Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DocServicePurch = query
                };
                return await Task.Run(() => Ok(collectionWrapper));
                */


                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount = query.Count();
                if (dirCount < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount;

                if (_params.collectionWrapper == "DocServiceInvTab")
                {
                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DocServiceInvTab = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));
                }
                else
                {
                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DocServicePurch = query
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

        // GET: api/DocServicePurches/5
        [ResponseType(typeof(DocServicePurch))]
        public async Task<IHttpActionResult> GetDocServicePurch(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
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
                        from docServicePurches in db.DocServicePurches
                        where docServicePurches.DocServicePurchID == id
                        select docServicePurches
                    ).ToListAsync());

                    if (queryDocID.Count() > 0) DocID = Convert.ToInt32(queryDocID[0].DocID);
                }

                #endregion


                #region Отправка JSON


                #region Полный путь Аппарата

                //1. Получаем Sub аппарата по "DocServicePurchID" (id)
                /*
                string DirServiceNomenPatchFull = null;
                var querySub = await Task.Run(() =>
                     (
                        from x in db.DocServicePurches
                        where x.DocServicePurchID == id
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
                        from x in db.DocServicePurches
                        where x.DocServicePurchID == id
                        select x.DirServiceNomenID
                    ).ToArrayAsync();

                if (queryDirServiceNomenID.Count() > 0)
                {
                    string[] ret = await Task.Run(() => mPatchFull(db, queryDirServiceNomenID[0]));
                    DirServiceNomenPatchFull = ret[0];

                    //Для поиска в списке товара (клгда нажимаем на кнопку "Склад", что бы сразу попасть на нужную группу)
                    string[] sID = ret[1].Split(',');
                    try { if (!String.IsNullOrEmpty(sID[0])) ID0 = sID[0].ToUpper(); if (ID0[ID0.Length - 1].ToString() == " ") ID0 = ID0.Remove(ID0.Length - 1); } catch { }
                    try { if (!String.IsNullOrEmpty(sID[1])) ID1 = sID[1].ToUpper(); if (ID1[ID1.Length - 1].ToString() == " ") ID1 = ID1.Remove(ID1.Length - 1); } catch { }
                    try { if (!String.IsNullOrEmpty(sID[2])) ID2 = sID[2].ToUpper(); if (ID2[ID2.Length - 1].ToString() == " ") ID2 = ID2.Remove(ID2.Length - 1); } catch { }
                }

                #endregion


                #region Суммы Услуг и Запчастей

               //double dSumDocServicePurch1Tabs = await db.DocServicePurch1Tabs.Where(x => x.DocServicePurchID == id).Select(x=>x.PriceCurrency).DefaultIfEmpty(0).SumAsync();

                //double dSumDocServicePurch2Tabs = await db.DocServicePurch2Tabs.Where(x => x.DocServicePurchID == id).Select(x => x.PriceCurrency).DefaultIfEmpty(0).SumAsync();

                #endregion



                #region QUERY


                var query = await Task.Run(() =>
                    (
                        #region from

                        from docServicePurches in db.DocServicePurches

                        join docServicePurch1Tabs1 in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs1.DocServicePurchID into docServicePurch1Tabs2
                        from docServicePurch1Tabs in docServicePurch1Tabs2.DefaultIfEmpty()

                        join docServicePurch2Tabs1 in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs1.DocServicePurchID into docServicePurch2Tabs2
                        from docServicePurch2Tabs in docServicePurch2Tabs2.DefaultIfEmpty()

                        #endregion

                        where docServicePurches.DocServicePurchID == id

                        #region select

                        select new
                        {
                            DocID = docServicePurches.DocID,
                            DocDate = docServicePurches.doc.DocDate,
                            Base = docServicePurches.doc.Base,
                            Held = docServicePurches.doc.Held,
                            Discount = docServicePurches.doc.Discount,
                            Del = docServicePurches.doc.Del,
                            Description = docServicePurches.doc.Description,
                            IsImport = docServicePurches.doc.IsImport,
                            DirVatValue = docServicePurches.doc.DirVatValue,
                            //DirPaymentTypeID = docServicePurches.doc.DirPaymentTypeID,

                            DirServiceNomenID = docServicePurches.DirServiceNomenID,

                            DirServiceNomenNameLittle = docServicePurches.dirServiceNomen.DirServiceNomenName,

                            DirServiceNomenName =
                            DirServiceNomenPatchFull == null ? docServicePurches.dirServiceNomen.DirServiceNomenName
                            :
                            DirServiceNomenPatchFull, // + docServicePurches.dirServiceNomen.DirServiceNomenName,

                            ID0 = ID0,
                            ID1 = ID1,
                            ID2 = ID2,

                            DocServicePurchID = docServicePurches.DocServicePurchID,
                            DirContractorName = docServicePurches.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docServicePurches.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docServicePurches.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docServicePurches.dirWarehouse.DirWarehouseID,
                            DirWarehouseIDPurches = docServicePurches.DirWarehouseIDPurches,
                            DirWarehouseName = docServicePurches.dirWarehouse.DirWarehouseName,

                            DirServiceStatusID = docServicePurches.DirServiceStatusID,
                            SerialNumberNo = docServicePurches.SerialNumberNo,
                            SerialNumber = docServicePurches.SerialNumber,
                            TypeRepair = docServicePurches.TypeRepair,
                            ComponentDevice = docServicePurches.ComponentDevice,

                            ComponentBattery = docServicePurches.ComponentBattery,
                            ComponentBatterySerial = docServicePurches.ComponentBatterySerial,
                            ComponentBackCover = docServicePurches.ComponentBackCover,
                            ComponentPasTextNo = docServicePurches.ComponentPasTextNo,
                            ComponentPasText = docServicePurches.ComponentPasText,
                            ComponentOtherText = docServicePurches.ComponentOtherText,
                            ProblemClientWords = docServicePurches.ProblemClientWords,
                            Note = docServicePurches.Note,
                            DirServiceContractorName = docServicePurches.DirServiceContractorName,
                            DirServiceContractorRegular = docServicePurches.DirServiceContractorRegular,
                            DirServiceContractorID = docServicePurches.DirServiceContractorID,
                            DirServiceContractorAddress = docServicePurches.DirServiceContractorAddress,
                            DirServiceContractorPhone = docServicePurches.DirServiceContractorPhone,
                            DirServiceContractorEmail = docServicePurches.DirServiceContractorEmail,

                            PriceVAT = docServicePurches.PriceVAT,
                            //PriceCurrency = docServicePurches.PriceCurrency,

                            DirCurrencyID = docServicePurches.DirCurrencyID,
                            DirCurrencyRate = docServicePurches.DirCurrencyRate,
                            DirCurrencyMultiplicity = docServicePurches.DirCurrencyMultiplicity,
                            DirCurrencyName = docServicePurches.dirCurrency.DirCurrencyName + " (" + docServicePurches.DirCurrencyRate + ", " + docServicePurches.DirCurrencyMultiplicity + ")",

                            DateDone = docServicePurches.DateDone,
                            UrgentRepairs = docServicePurches.UrgentRepairs,
                            Prepayment = docServicePurches.Prepayment,
                            PrepaymentSum = docServicePurches.PrepaymentSum == null ? 0 : docServicePurches.PrepaymentSum,

                            //Оплата
                            Payment = docServicePurches.doc.Payment,
                            //Мастер
                            DirEmployeeIDMaster = docServicePurches.DirEmployeeIDMaster,
                            DirEmployeeNameMaster = docServicePurches.dirEmployee.DirEmployeeName,

                            ServiceTypeRepair = docServicePurches.ServiceTypeRepair,

                            Phone = docServicePurches.dirWarehouse.Phone,
                            DirWarehouseAddress = docServicePurches.dirWarehouse.DirWarehouseAddress,

                            //К-во раз Клиент обращался в сервис
                            QuantityOk = docServicePurches.dirServiceContractor.QuantityOk,
                            QuantityFail = docServicePurches.dirServiceContractor.QuantityFail,
                            QuantityCount = docServicePurches.dirServiceContractor.QuantityCount,

                            DiscountX = docServicePurches.DiscountX,
                            DiscountY = docServicePurches.DiscountY,

                            //Перемещён в БУ
                            InSecondHand = docServicePurches.InSecondHand,
                            InSecondHandString = docServicePurches.InSecondHand == true ? "В Б/У" : "",


                            // *** СУММЫ *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                            //1. Подсчет табличной части Работы "SumDocServicePurch1Tabs"
                            SumDocServicePurch1Tabs = docServicePurches.Sums1, SumDocServicePurch1Tabs2 = docServicePurches.Sums1, //SumDocServicePurch1Tabs = dSumDocServicePurch1Tabs, SumDocServicePurch1Tabs2 = dSumDocServicePurch1Tabs,
                            //2. Подсчет табличной части Работы "SumDocServicePurch2Tabs"
                            SumDocServicePurch2Tabs = docServicePurches.Sums2, SumDocServicePurch2Tabs2 = docServicePurches.Sums2, //SumDocServicePurch2Tabs = dSumDocServicePurch2Tabs, SumDocServicePurch2Tabs2 = dSumDocServicePurch2Tabs,
                            //3. Сумма 1+2 "SumTotal"
                            SumTotal = docServicePurches.Sums1 + docServicePurches.Sums2, //SumTotal = dSumDocServicePurch1Tabs + dSumDocServicePurch2Tabs, SumTotal2 = dSumDocServicePurch1Tabs + dSumDocServicePurch2Tabs,
                            //5. 3 - 4 "SumTotal2"
                            SumTotal2a = docServicePurches.Sums1 + docServicePurches.Sums2 - docServicePurches.PrepaymentSum, //SumTotal2a = dSumDocServicePurch1Tabs + dSumDocServicePurch2Tabs - docServicePurches.PrepaymentSum,

                            Alerted = docServicePurches.AlertedCount == null ? "Не оповещён" : "Оповещён (" + docServicePurches.AlertedCount + ") " + docServicePurches.AlertedDateTxt,

                            //Для отправки СМС, что бы знать были ли предыдущие ремонты
                            PrepaymentSum_1 = docServicePurches.PrepaymentSum_1,
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
                    if (query[0].DirServiceStatusID == 1)
                    {
                        //Меняем статус + меняем мастера
                        Models.Sklad.Doc.DocServicePurch docServicePurch = await dbRead.DocServicePurches.FindAsync(id);
                        //docServicePurch.DirEmployeeIDMaster = field.DirEmployeeID;
                        docServicePurch.DirServiceStatusID = 2;
                        dbRead.Entry(docServicePurch).State = EntityState.Modified;
                        await dbRead.SaveChangesAsync();

                        //Пишем в Лог о смене статуса и мастера, если такое было
                        logService.DocServicePurchID = id;
                        logService.DirServiceLogTypeID = 1;
                        logService.DirEmployeeID = field.DirEmployeeID;
                        logService.DirServiceStatusID = docServicePurch.DirServiceStatusID;
                        //if (query[0].DirEmployeeIDMaster != field.DirEmployeeID) logService.Msg = "Смена мастера " + query[0].DirEmployeeNameMaster;

                        await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);
                    }


                    //??? Пишем в Лог о смене мастера, если такое было ???
                    /*
                    if (query[0].DirEmployeeIDMaster != field.DirEmployeeID)
                    {
                        Models.Sklad.Doc.DocServicePurch docServicePurch = await dbRead.DocServicePurches.FindAsync(id);
                        //docServicePurch.DirEmployeeIDMaster = field.DirEmployeeID;
                        //docServicePurch.DirServiceStatusID = 2;
                        dbRead.Entry(docServicePurch).State = EntityState.Modified;
                        await dbRead.SaveChangesAsync();
                    }
                    */

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

        // PUT: api/DocServicePurches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocServicePurch(int id, DocServicePurch docServicePurch, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Разрешение менять дату
                bool bRight = await Task.Run(() => accessRight.AccessCheck(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurchesDateDoneCheck"));
                if (!bRight) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

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
                    docServicePurch = await db.DocServicePurches.FindAsync(id);
                    DateTime DateDoneOLD = docServicePurch.DateDone;
                    docServicePurch.DateDone = DateDone;

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            db.Entry(docServicePurch).State = EntityState.Modified;
                            await db.SaveChangesAsync();


                            #region 4. Log

                            logService.DocServicePurchID = docServicePurch.DocServicePurchID;
                            logService.DirServiceLogTypeID = 7;
                            logService.DirEmployeeID = field.DirEmployeeID;
                            //logService.DirServiceStatusID = docServicePurch.DirServiceStatusID;
                            logService.Msg = "Смена даты готовности с " + DateDoneOLD.ToString("yyyy-MM-dd") + " на " + DateDone.ToString("yyyy-MM-dd");

                            await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

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
                    sysJourDisp.TableFieldID = docServicePurch.DocServicePurchID;
                    sysJourDisp.Description = "";
                    try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                    #endregion


                    dynamic collectionWrapper = new
                    {
                        DocID = docServicePurch.DocID,
                        DocServicePurchID = docServicePurch.DocServicePurchID
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
        public async Task<IHttpActionResult> PutDocServicePurch(int id, int DirStatusID, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
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

                double SumTotal2a = 0;
                string sSumTotal2a = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "SumTotal2a", true) == 0).Value;
                if (!String.IsNullOrEmpty(sSumTotal2a))
                {
                    sSumTotal2a = sSumTotal2a.Replace(".", ",");
                    SumTotal2a = Convert.ToDouble(sSumTotal2a);
                }

                string sDiscountX = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DiscountX", true) == 0).Value;
                double DiscountX = 0;
                if (!String.IsNullOrEmpty(sDiscountX))
                {
                    sDiscountX = sDiscountX.Replace(".", ",");
                    DiscountX = Convert.ToDouble(sDiscountX);
                }

                string sDiscountY = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DiscountY", true) == 0).Value;
                //double DiscountY = Convert.ToDouble(sDiscountY);
                double DiscountY = 0;
                if (!String.IsNullOrEmpty(sDiscountY))
                {
                    sDiscountY = sDiscountY.Replace(".", ",");
                    DiscountY = Convert.ToDouble(sDiscountY);
                }

                string sReturnRresults = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sReturnRresults", true) == 0).Value;


                int? KKMSCheckNumber = null;
                try
                {
                    KKMSCheckNumber = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "KKMSCheckNumber", true) == 0).Value.Replace(".", ","));
                }
                catch { }
                string KKMSIdCommand = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "KKMSIdCommand", true) == 0).Value;


                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);


                /*
                //2.1. Админ точки?
                bool iRightCheck = await Task.Run(() => accessRight.AccessIsAdmin(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, _params.DirWarehouseID));
                //Есть доступ для Выдачи аппарата (Статус 7 и 8 для вкладки Выдача)
                bool isExtradition = false;
                int iRight2 = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurchesExtradition"));
                if (iRight2 < 3) { isExtradition = true; }

                if (!iRightCheck && DirStatusID == 8 && !isExtradition)
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg128));
                }
                */

                #endregion

                #region Проверки
                //Если Статус "7", то проверить Таб часть-1
                // Если DirStatusID = 7 и нет ни одной выполненной работы, то не пускать сохранять и выдать эксепшн
                //if (iTypeService > 1 && docServicePurch.DirStatusID == 7 && docServicePurch1TabCollection.Length == 0) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg114); }

                var queryCount = await
                    (
                        from x in db.DocServicePurch1Tabs
                        where x.DocServicePurchID == id
                        select x
                    ).CountAsync();
                if (queryCount == 0 && DirStatusID == 7)
                {
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg114);
                }

                //Если аппарат перемещён в УСЦ (Статус == 6 или точка поступления отличается от текущей точки), то выдать исключение
                Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(id);
                if (docServicePurch.DirWarehouseIDPurches != docServicePurch.DirWarehouseID)
                {
                    throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg114_1);
                }

                #endregion


                #region Сохранение

                try
                {
                    //Models.Sklad.Doc.DocServicePurch docServicePurch = new DocServicePurch();

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            await mStatusChange(db, ts, docServicePurch, id, DirStatusID, DirPaymentTypeID, SumTotal2a, sReturnRresults, field, KKMSCheckNumber, KKMSIdCommand, DiscountX, DiscountY);

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
                        DocID = docServicePurch.DocID,
                        DocServicePurchID = docServicePurch.DocServicePurchID
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
        public async Task<IHttpActionResult> PutDocServicePurch(int id, int ServiceTypeRepair, int iTrash, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
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
                    Models.Sklad.Doc.DocServicePurch docServicePurch = new DocServicePurch();

                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            await mRepairChange(db, ts, docServicePurch, id, ServiceTypeRepair, field);

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
                        DocID = docServicePurch.DocID,
                        DocServicePurchID = docServicePurch.DocServicePurchID
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

        //Смена мастера
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocServicePurch(int id, int DirEmployeeID, int iTrash, int iTrash2, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //2. Получаем "RightDocServiceWorkshopsOnlyUsers"
                //bool iRightCheck = await Task.Run(() => accessRight.AccessCheck(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceWorkshopsOnlyUsersCheck"));
                //2.1. Админ точки?
                //... ниже, т.к. нам нужен склад ...


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

                //Надо получить склад "docServicePurch.DirWarehouseID"
                Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(id);


                //*** *** ***
                //2.1. Админ точки?
                bool iRightCheck = await Task.Run(() => accessRight.AccessIsAdmin(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, docServicePurch.DirWarehouseID));
                //*** *** ***


                //Только если Админ точки
                //Админ точки != RightDocServiceWorkshopsOnlyUsersCheck
                if (!iRightCheck) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                #endregion


                #region Сохранение

                try
                {
                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            await mDirEmployeeIDChange(db, ts, docServicePurch, id, DirEmployeeID, field);

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
                        DocID = docServicePurch.DocID,
                        DocServicePurchID = docServicePurch.DocServicePurchID
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

        //В БУ
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocServicePurch(int id, int iTrash0, int iTrash, int iTrash2, int iTrash3, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //2. Получаем "RightDocServiceWorkshopsOnlyUsers"
                //bool iRightCheck = await Task.Run(() => accessRight.AccessCheck(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceWorkshopsOnlyUsersCheck"));
                //2.1. Админ точки?
                //... ниже, т.к. нам нужен склад ...


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

                //Надо получить склад "docServicePurch.DirWarehouseID"
                Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(id);


                //*** *** ***
                //2.1. Админ точки?
                bool iRightCheck = await Task.Run(() => accessRight.AccessIsAdmin(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, docServicePurch.DirWarehouseID));
                //*** *** ***


                //Только если Админ точки
                //Админ точки != RightDocServiceWorkshopsOnlyUsersCheck
                if (!iRightCheck) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                #endregion


                #region Сохранение

                try
                {
                    using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;

                        try
                        {
                            //Перемещение в модуль БУ


                            #region 1. Получаем по "id" сам ремонт

                            //docServicePurch - уже нашли (выше)

                            #endregion


                            #region 2. Копируем модели


                            #region 2.1. Подсчёт сумм работ и запчастей для PriceVAT, PriceCurrency

                            double SumPriceVAT1 = 0, SumPriceCurrency1 = 0;
                            var query1 = await
                                (
                                    from x in db.DocServicePurch1Tabs
                                    where x.DocServicePurchID == id

                                    group x by 1 into g

                                    select new
                                    {
                                        SumPriceVAT1 = g.Sum(x => x.PriceVAT),
                                        SumPriceCurrency1 = g.Sum(x => x.PriceCurrency)
                                    }
                                ).ToListAsync();
                            if (query1.Count() > 0)
                            {
                                SumPriceVAT1 = query1[0].SumPriceVAT1;
                                SumPriceCurrency1 = query1[0].SumPriceCurrency1;
                            }


                            double SumPriceVAT2 = 0, SumPriceCurrency2 = 0;
                            var query2 = await
                                (
                                    from x in db.DocServicePurch2Tabs
                                    where x.DocServicePurchID == id

                                    group x by 1 into g

                                    select new
                                    {
                                        SumPriceVAT2 = g.Sum(x => x.PriceVAT),
                                        SumPriceCurrency2 = g.Sum(x => x.PriceCurrency)
                                    }
                                ).ToListAsync();
                            if (query2.Count() > 0)
                            {
                                SumPriceVAT2 = query2[0].SumPriceVAT2;
                                SumPriceCurrency2 = query2[0].SumPriceCurrency2;
                            }

                            #endregion


                            Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = new DocSecondHandPurch();

                            //docSecondHandPurch
                            docSecondHandPurch.DocSecondHandPurchID = null;
                            docSecondHandPurch.DocID = null;
                            docSecondHandPurch.DirWarehouseID = docServicePurch.DirWarehouseID;
                            docSecondHandPurch.DirServiceNomenID = docServicePurch.DirServiceNomenID;
                            docSecondHandPurch.DirSecondHandStatusID = 7;
                            docSecondHandPurch.DirSecondHandStatusID_789 = null;
                            docSecondHandPurch.SerialNumberNo = docServicePurch.SerialNumberNo;
                            docSecondHandPurch.SerialNumber = docServicePurch.SerialNumber;
                            docSecondHandPurch.ComponentPasTextNo = docServicePurch.ComponentPasTextNo;
                            docSecondHandPurch.ComponentPasText = docServicePurch.ComponentPasText;
                            docSecondHandPurch.ComponentOtherText = docServicePurch.ComponentOtherText;
                            docSecondHandPurch.ProblemClientWords = docServicePurch.ProblemClientWords;
                            docSecondHandPurch.Note = docServicePurch.Note;
                            docSecondHandPurch.DirServiceContractorName = docServicePurch.DirServiceContractorName;
                            docSecondHandPurch.DirServiceContractorRegular = docServicePurch.DirServiceContractorRegular;
                            docSecondHandPurch.DirServiceContractorID = docServicePurch.DirServiceContractorID;
                            docSecondHandPurch.DirServiceContractorAddress = docServicePurch.DirServiceContractorAddress;
                            docSecondHandPurch.DirServiceContractorPhone = docServicePurch.DirServiceContractorPhone;
                            docSecondHandPurch.DirServiceContractorEmail = docServicePurch.DirServiceContractorEmail;

                            docSecondHandPurch.PassportSeries = ""; //docServicePurch.PassportSeries;
                            docSecondHandPurch.PassportNumber = ""; //docServicePurch.PassportNumber;

                            //Цены
                            docSecondHandPurch.PriceVAT = 0; //SumPriceVAT1 + SumPriceVAT2; //docServicePurch.PriceVAT;
                            docSecondHandPurch.PriceCurrency = 0; //SumPriceCurrency1 + SumPriceCurrency2; //docServicePurch.PriceCurrency;

                            docSecondHandPurch.DirCurrencyID = docServicePurch.DirCurrencyID;
                            docSecondHandPurch.DirCurrencyRate = docServicePurch.DirCurrencyRate;
                            docSecondHandPurch.DirCurrencyMultiplicity = docServicePurch.DirCurrencyMultiplicity;

                            docSecondHandPurch.PriceRetailVAT = 0; //docServicePurch.PriceRetailVAT;
                            docSecondHandPurch.PriceRetailCurrency = 0; //docServicePurch.PriceRetailCurrency;
                            docSecondHandPurch.PriceWholesaleVAT = 0; //docServicePurch.PriceWholesaleVAT;
                            docSecondHandPurch.PriceWholesaleCurrency = 0; //docServicePurch.PriceWholesaleCurrency;
                            docSecondHandPurch.PriceIMVAT = 0; //docServicePurch.PriceIMVAT;
                            docSecondHandPurch.PriceIMCurrency = 0; //docServicePurch.PriceIMCurrency;


                            docSecondHandPurch.DateDone = docServicePurch.DateDone;
                            docSecondHandPurch.DirEmployeeIDMaster = docServicePurch.DirEmployeeIDMaster;
                            docSecondHandPurch.ServiceTypeRepair = docServicePurch.ServiceTypeRepair;
                            docSecondHandPurch.Summ_NotPre = docServicePurch.Summ_NotPre;
                            docSecondHandPurch.IssuanceDate = docServicePurch.IssuanceDate;
                            docSecondHandPurch.DateStatusChange = docServicePurch.DateStatusChange;
                            docSecondHandPurch.Sums = docServicePurch.Sums;
                            docSecondHandPurch.Sums1 = docServicePurch.Sums1;
                            docSecondHandPurch.Sums2 = docServicePurch.Sums2;
                            docSecondHandPurch.DocDate_First = docServicePurch.DocDate_First;
                            docSecondHandPurch.FromService = true;
                            docSecondHandPurch.DocIDService = docServicePurch.DocID;
                            docSecondHandPurch.Sums1Service = docServicePurch.Sums1; docSecondHandPurch.Sums2Service = docServicePurch.Sums2;


                            //Doc

                            Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docServicePurch.DocID);

                            docSecondHandPurch.Del = false;
                            docSecondHandPurch.ListObjectID = 65;
                            docSecondHandPurch.NumberInt = null;
                            docSecondHandPurch.DocDate = DateTime.Now;
                            //docSecondHandPurch.DocDateCreate = DateTime.Now;
                            docSecondHandPurch.Held = false;
                            docSecondHandPurch.Discount = 0;
                            docSecondHandPurch.DocIDBase = null;
                            docSecondHandPurch.Base = null;
                            docSecondHandPurch.DirContractorIDOrg = doc.DirContractorIDOrg;
                            docSecondHandPurch.DirContractorID = doc.DirContractorID;
                            docSecondHandPurch.DirEmployeeID = field.DirEmployeeID;
                            docSecondHandPurch.IsImport = false;
                            docSecondHandPurch.Description = null;
                            docSecondHandPurch.DirPaymentTypeID = 1;
                            docSecondHandPurch.Payment = 0;
                            docSecondHandPurch.DirVatValue = 0;

                            #endregion


                            #region 3. Созлаём документ БУ

                            Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurchesController docSecondHandPurchesController = new DocSecondHandPurches.DocSecondHandPurchesController();
                            docSecondHandPurch =
                                await Task.Run(() =>
                                    docSecondHandPurchesController.mPutPostDocSecondHandPurch
                                    (
                                        db,
                                        dbRead,
                                        "", //UO_Action, 
                                        docSecondHandPurch,
                                        null,
                                        null,
                                        EntityState.Added,
                                        1,
                                        field
                                    )
                                );

                            #endregion


                            #region 4. Помечаем ремонт, как перемещённый в БУ - запретить его редактирование

                            docServicePurch.InSecondHand = true;
                            docServicePurch.DirServiceStatusID = 9;
                            docServicePurch.DirServiceStatusID_789 = 7;
                            db.Entry(docServicePurch).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            #endregion





                            #region 3. Лог: СЦ + БУ


                            #region СЦ

                            logService.DocServicePurchID = docServicePurch.DocServicePurchID;
                            logService.DirServiceLogTypeID = 10;
                            logService.DirEmployeeID = field.DirEmployeeID;
                            logService.DirServiceStatusID = docServicePurch.DirServiceStatusID;
                            logService.Msg = "из СЦ №" + docServicePurch.DocServicePurchID + " в БУ №" + docSecondHandPurch.DocSecondHandPurchID;

                            await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

                            #endregion


                            #region БУ

                            Models.Sklad.Log.LogSecondHand logServiceSH = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesControllerSH = new Log.LogSecondHandsController();

                            logServiceSH.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
                            logServiceSH.DirSecondHandLogTypeID = 11; //Смена статуса
                            logServiceSH.DirEmployeeID = field.DirEmployeeID;
                            logServiceSH.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
                            logServiceSH.Msg = "из СЦ №" + docServicePurch.DocServicePurchID + " в БУ №" + docSecondHandPurch.DocSecondHandPurchID;

                            await logServicesControllerSH.mPutPostLogSecondHands(db, logServiceSH, EntityState.Added);

                            #endregion


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

                    dynamic collectionWrapper = new
                    {
                        DocID = docServicePurch.DocID,
                        DocServicePurchID = docServicePurch.DocServicePurchID
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



        // POST: api/DocServicePurches
        [ResponseType(typeof(DocServicePurch))]
        public async Task<IHttpActionResult> PostDocServicePurch(DocServicePurch docServicePurch, HttpRequestMessage request)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
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

            Models.Sklad.Doc.DocServicePurch1Tab[] docServicePurch1TabCollection = null;
            if (!String.IsNullOrEmpty(docServicePurch.recordsDocServicePurch1Tab))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                docServicePurch1TabCollection = serializer.Deserialize<Models.Sklad.Doc.DocServicePurch1Tab[]>(docServicePurch.recordsDocServicePurch1Tab);
            }

            Models.Sklad.Doc.DocServicePurch2Tab[] docServicePurch2TabCollection = null;
            if (!String.IsNullOrEmpty(docServicePurch.recordsDocServicePurch2Tab))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                docServicePurch2TabCollection = serializer.Deserialize<Models.Sklad.Doc.DocServicePurch2Tab[]>(docServicePurch.recordsDocServicePurch2Tab);
            }

            #endregion

            #region Проверки

            if (!ModelState.IsValid && _params.iTypeService != 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            docServicePurch.Substitute();

            #endregion


            #region Сохранение

            string res = "";

            try
            {
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docServicePurch = await Task.Run(() => mPutPostDocServicePurch(db, dbRead, UO_Action, docServicePurch, docServicePurch1TabCollection, docServicePurch2TabCollection, EntityState.Added, _params.iTypeService, field)); //sysSetting
                        ts.Commit(); //.Complete();


                        try
                        {
                            #region 5. Sms

                            if (
                                docServicePurch.DirServiceContractorPhone != null && docServicePurch.DirServiceContractorPhone.Length > 7
                                &&
                                sysSetting.DocServicePurchSmsAutoShow
                               )
                            {

                                //string res = "";

                                Models.Sklad.Dir.DirSmsTemplate dirSmsTemplate = await db.DirSmsTemplates.FindAsync(6);

                                string DirSmsTemplateMsg = await function.mSms_DocServicePurches(db, dirSmsTemplate, docServicePurch, sysSetting);
                                
                                PartionnyAccount.Controllers.Sklad.SMS.SmsController smsController = new SMS.SmsController();
                                res = await smsController.SenSms(
                                    //res,
                                    sysSetting,
                                    40,
                                    6,
                                    docServicePurch.DirServiceContractorPhone,
                                    DirSmsTemplateMsg, //dirSmsTemplate.DirSmsTemplateMsg,
                                    field,
                                    db,
                                    Convert.ToInt32(docServicePurch.DocServicePurchID)
                                    );
                                
                            }

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
                sysJourDisp.TableFieldID = docServicePurch.DocServicePurchID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docServicePurch.DocID,
                    DocServicePurchID = docServicePurch.DocServicePurchID,
                    Msg = res
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DocServicePurches/5
        [ResponseType(typeof(DocServicePurch))]
        public async Task<IHttpActionResult> DeleteDocServicePurch(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurches"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion


            #region Удаление

            //Алгоритм.
            //Удаляем по порядку:
            //1. RemParties
            //2. DocServicePurchTabs
            //3. DocServicePurches
            //4. Docs


            //Сотрудник
            Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(id);
            if (docServicePurch == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


            using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
            {
                try
                {
                    #region 1. Ищим DocID *** *** *** *** ***

                    //1.1. Ищим DocID
                    int iDocID = 0;
                    var queryDocs1 = await
                        (
                            from x in db.DocServicePurches
                            where x.DocServicePurchID == id
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


                    #region 2. DocServicePurch1Tabs *** *** *** *** ***

                    var queryDocServicePurch1Tabs = await
                        (
                            from x in db.DocServicePurch1Tabs
                            where x.DocServicePurchID == id
                            select x
                        ).ToListAsync();
                    for (int i = 0; i < queryDocServicePurch1Tabs.Count(); i++)
                    {
                        Models.Sklad.Doc.DocServicePurch1Tab docServicePurch1Tab = await db.DocServicePurch1Tabs.FindAsync(queryDocServicePurch1Tabs[i].DocServicePurch1TabID);
                        db.DocServicePurch1Tabs.Remove(docServicePurch1Tab);
                        await db.SaveChangesAsync();
                    }

                    #endregion

                    #region 2. DocServicePurch2Tabs *** *** *** *** ***

                    var queryDocServicePurch2Tabs = await
                        (
                            from x in db.DocServicePurch2Tabs
                            where x.DocServicePurchID == id
                            select x
                        ).ToListAsync();
                    for (int i = 0; i < queryDocServicePurch2Tabs.Count(); i++)
                    {
                        Models.Sklad.Doc.DocServicePurch2Tab docServicePurch2Tab = await db.DocServicePurch2Tabs.FindAsync(queryDocServicePurch2Tabs[i].DocServicePurch2TabID);
                        db.DocServicePurch2Tabs.Remove(docServicePurch2Tab);
                        await db.SaveChangesAsync();
                    }

                    #endregion


                    #region 3. DocServicePurches *** *** *** *** ***

                    var queryDocServicePurches = await
                        (
                            from x in db.DocServicePurches
                            where x.DocServicePurchID == id
                            select x
                        ).ToListAsync();
                    for (int i = 0; i < queryDocServicePurches.Count(); i++)
                    {
                        Models.Sklad.Doc.DocServicePurch docServicePurch1 = await db.DocServicePurches.FindAsync(queryDocServicePurches[i].DocServicePurchID);
                        db.DocServicePurches.Remove(docServicePurch1);
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

        private bool DocServicePurchExists(int id)
        {
            return db.DocServicePurches.Count(e => e.DocServicePurchID == id) > 0;
        }


        internal async Task<DocServicePurch> mPutPostDocServicePurch(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            DocServicePurch docServicePurch,
            Models.Sklad.Doc.DocServicePurch1Tab[] docServicePurch1TabCollection,
            Models.Sklad.Doc.DocServicePurch2Tab[] docServicePurch2TabCollection,
            EntityState entityState, //EntityState.Added, Modified
            int iTypeService,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            
            // Если DirServiceStatusID > 1, то не сохранять, а выводить сообщение!
            if (iTypeService == 1 && docServicePurch.DirServiceStatusID > 1) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg113); }
            // Если DirServiceStatusID = 7 и нет ни одной выполненной работы, то не пускать сохранять и выдать эксепшн
            if (iTypeService > 1 && docServicePurch.DirServiceStatusID == 7 && docServicePurch1TabCollection.Length == 0) { throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg114); }




            //Единственный рабочий вариант
            if (iTypeService == 1)
            {
                #region 1


                #region 0. Заполняем DirServiceContractors
                // - не находим - создаём новую
                // - находим - обновляем

                Models.Sklad.Dir.DirServiceContractor dirServiceContractor = new Models.Sklad.Dir.DirServiceContractor();
                if (!Convert.ToBoolean(docServicePurch.UrgentRepairs))
                {

                    string DirServiceContractorPhone = docServicePurch.DirServiceContractorPhone.Replace("+", "").ToLower();

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
                            dirServiceContractor.DirServiceContractorName = docServicePurch.DirServiceContractorName;
                            dirServiceContractor.QuantityOk = 0;
                            dirServiceContractor.QuantityFail = 0;
                            dirServiceContractor.QuantityCount = 0;

                            db.Entry(dirServiceContractor).State = EntityState.Added;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            dirServiceContractor = await db.DirServiceContractors.FindAsync(queryDirServiceContractors[0].DirServiceContractorID);
                            dirServiceContractor.DirServiceContractorName = docServicePurch.DirServiceContractorName;

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
                doc.NumberInt = docServicePurch.NumberInt;
                doc.NumberReal = docServicePurch.DocServicePurchID;
                doc.DirEmployeeID = field.DirEmployeeID;
                doc.DirPaymentTypeID = docServicePurch.DirPaymentTypeID;
                doc.Payment = docServicePurch.Payment;
                doc.DirContractorID = docServicePurch.DirContractorIDOrg;
                doc.DirContractorIDOrg = docServicePurch.DirContractorIDOrg;
                doc.Discount = docServicePurch.Discount;
                doc.DirVatValue = docServicePurch.DirVatValue;
                doc.Base = docServicePurch.Base;
                doc.Description = docServicePurch.Description;
                doc.DocDate = docServicePurch.DocDate;
                //doc.DocDisc = docServicePurch.DocDisc;
                doc.Held = false;  //if (UO_Action == "held") doc.Held = false; //else doc.Held = false;
                doc.DocID = docServicePurch.DocID;
                doc.DocIDBase = docServicePurch.DocIDBase;
                doc.KKMSCheckNumber = docServicePurch.KKMSCheckNumber;
                doc.KKMSIdCommand = docServicePurch.KKMSIdCommand;
                doc.KKMSEMail = docServicePurch.KKMSEMail;
                doc.KKMSPhone = docServicePurch.KKMSPhone;

                //Класс
                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                //doc = await docs.Save();
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docServicePurch" со всем полями!
                docServicePurch.DocID = doc.DocID;

                #endregion

                #region 2. DocServicePurch *** *** *** *** *** *** *** *** *** ***


                #region Если выбрана хоть одна типовая несисправность - статус "Согласован" (3)
                if (
                    Convert.ToBoolean(docServicePurch.DirServiceNomenTypicalFaultID1) || Convert.ToBoolean(docServicePurch.DirServiceNomenTypicalFaultID2) || Convert.ToBoolean(docServicePurch.DirServiceNomenTypicalFaultID3) ||
                    Convert.ToBoolean(docServicePurch.DirServiceNomenTypicalFaultID4) || Convert.ToBoolean(docServicePurch.DirServiceNomenTypicalFaultID5) || Convert.ToBoolean(docServicePurch.DirServiceNomenTypicalFaultID4) ||
                    Convert.ToBoolean(docServicePurch.DirServiceNomenTypicalFaultID7)
                  )
                {
                    docServicePurch.DirServiceStatusID = 4;
                }

                #endregion


                #region Сохранение

                docServicePurch.DocID = doc.DocID;
                docServicePurch.DirWarehouseIDPurches = docServicePurch.DirWarehouseID;
                //docServicePurch.DirEmployeeIDMaster = field.DirEmployeeID; //Это мастер, его пока нет в форме.
                docServicePurch.ServiceTypeRepair = sysSetting.ServiceTypeRepair;
                docServicePurch.DirServiceContractorID = dirServiceContractor.DirServiceContractorID;
                if (docServicePurch.Sums == null) docServicePurch.Sums = 0;
                if (docServicePurch.Sums1 == null) docServicePurch.Sums1 = 0;
                if (docServicePurch.Sums2 == null) docServicePurch.Sums2 = 0;

                db.Entry(docServicePurch).State = entityState;
                await db.SaveChangesAsync();

                #endregion


                #region UpdateNumberInt, если INSERT *** *** *** *** ***

                if (entityState == EntityState.Added && (docServicePurch.doc.NumberInt == null || docServicePurch.doc.NumberInt.Length == 0))
                {
                    doc.NumberInt = docServicePurch.DocServicePurchID.ToString();
                    doc.NumberReal = docServicePurch.DocServicePurchID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }
                else if (entityState == EntityState.Added)
                {
                    doc.NumberReal = docServicePurch.DocServicePurchID;
                    docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                    await Task.Run(() => docs.Save());
                }

                #endregion


                #endregion

                #region 3. Касса или Банк

                //Только, если сумма больше 0
                if (docServicePurch.PrepaymentSum > 0)  //if (doc.Payment > 0)
                {
                    //Получаем наименование аппарата
                    Models.Sklad.Dir.DirServiceNomen dirServiceNomen = await db.DirServiceNomens.FindAsync(docServicePurch.DirServiceNomenID);
                    Controllers.Sklad.Dir.DirServiceNomens.DirServiceNomensController dirServiceNomensController = new Dir.DirServiceNomens.DirServiceNomensController();
                    string nomen = await dirServiceNomensController.DirServiceNomenSubFind2(dbRead, docServicePurch.DirServiceNomenID);

                    //Касса
                    if (doc.DirPaymentTypeID == 1)
                    {
                        #region Касса

                        //1. По складу находим привязанную к нему Кассу
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docServicePurch.DirWarehouseID);
                        int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;

                        //2. Заполняем модель "DocCashOfficeSum"
                        Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                        docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                        docCashOfficeSum.DirCashOfficeSumTypeID = 14;
                        docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                        docCashOfficeSum.DocID = doc.DocID;
                        docCashOfficeSum.DocXID = docServicePurch.DocServicePurchID;
                        docCashOfficeSum.DocCashOfficeSumSum = docServicePurch.PrepaymentSum; //doc.Payment;
                        docCashOfficeSum.Description = "";
                        docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;
                        docCashOfficeSum.Base = "Предоплата за аппарат: " + nomen; //dirServiceNomen.DirServiceNomenName;
                        docCashOfficeSum.KKMSCheckNumber = docServicePurch.KKMSCheckNumber;
                        docCashOfficeSum.KKMSIdCommand = docServicePurch.KKMSIdCommand;
                        docCashOfficeSum.KKMSEMail = docServicePurch.KKMSEMail;
                        docCashOfficeSum.KKMSPhone = docServicePurch.KKMSPhone;
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
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docServicePurch.DirWarehouseID);
                        int iDirBankID = dirWarehouse.DirBankID;

                        //2. Заполняем модель "DocBankSum"
                        Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                        docBankSum.DirBankID = iDirBankID;
                        docBankSum.DirBankSumTypeID = 13; //Изъятие из кассы на основании проведения приходной накладной №
                        docBankSum.DocBankSumDate = DateTime.Now;
                        docBankSum.DocID = doc.DocID;
                        docBankSum.DocXID = docServicePurch.DocServicePurchID;
                        docBankSum.DocBankSumSum = docServicePurch.PrepaymentSum; //doc.Payment;
                        docBankSum.Description = "";
                        docBankSum.DirEmployeeID = field.DirEmployeeID;
                        docBankSum.Base = "Предоплата за аппарат: " + dirServiceNomen.DirServiceNomenName;
                        docBankSum.KKMSCheckNumber = docServicePurch.KKMSCheckNumber;
                        docBankSum.KKMSIdCommand = docServicePurch.KKMSIdCommand;
                        docBankSum.KKMSEMail = docServicePurch.KKMSEMail;
                        docBankSum.KKMSPhone = docServicePurch.KKMSPhone;
                        docBankSum.Discount = doc.Discount;

                        //3. Пишем в Банк
                        Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                        docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum(db, docBankSum, EntityState.Added));

                        #endregion
                    }
                }
                
                #endregion

                #region 4. Log

                logService.DocServicePurchID = docServicePurch.DocServicePurchID;
                logService.DirServiceLogTypeID = 1;
                logService.DirEmployeeID = field.DirEmployeeID;
                logService.DirServiceStatusID = docServicePurch.DirServiceStatusID;

                if (entityState == EntityState.Added) {

                    string DirWarehouseName = "";
                    if (docServicePurch.dirWarehouse != null)
                    {
                        DirWarehouseName = docServicePurch.dirWarehouse.DirWarehouseName;
                    }
                    else
                    {
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docServicePurch.DirWarehouseID);
                        DirWarehouseName = dirWarehouse.DirWarehouseName;
                    }

                    logService.Msg = "Аппарат принят на точку №" + DirWarehouseName;
                }

                await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

                #endregion

                #region 5. Если заполненно поле "ComponentOtherText", то ищим похожую запись в таблице "DirServiceComplects.DirServiceComplectName"
                // - не находим - создаём новую
                // - находим - ничего не делаем

                if (!String.IsNullOrEmpty(docServicePurch.ComponentOtherText))
                {
                    var queryDirServiceComplects = await
                        (
                            from x in db.DirServiceComplects
                            where x.DirServiceComplectName.ToLower() == docServicePurch.ComponentOtherText.ToLower()
                            select x
                        ).ToListAsync();
                    if (queryDirServiceComplects.Count() == 0)
                    {
                        Models.Sklad.Dir.DirServiceComplect dirServiceComplect = new Models.Sklad.Dir.DirServiceComplect();
                        dirServiceComplect.DirServiceComplectName = docServicePurch.ComponentOtherText;

                        db.Entry(dirServiceComplect).State = EntityState.Added;
                        await db.SaveChangesAsync();
                    }
                }

                #endregion

                #endregion
            }



            #region OLD - НЕ используется
            /*
            else if (iTypeService == 2)
            {
                #region 2 - НЕ используется

                //Сохраняем в Шапке, только Статус - не получилось, глючит: обнуляет значения для модели "Docs" (она содержится в моделе DocServicePurches)

                #region 1. DocServicePurch *** *** *** *** *** *** *** *** *** ***

                //Сохраняем
                db.Entry(docServicePurch).State = entityState;
                await db.SaveChangesAsync();

                #endregion


                //Спецификация

                #region 3. DocServicePurch1Tab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocServicePurchID = new SQLiteParameter("@DocServicePurchID", System.Data.DbType.Int32) { Value = docServicePurch.DocServicePurchID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocServicePurch1Tabs WHERE DocServicePurchID=@DocServicePurchID;", parDocServicePurchID);
                }

                //2.2. Проставляем ID-шник "DocServicePurchID" для всех позиций спецификации
                for (int i = 0; i < docServicePurch1TabCollection.Count(); i++)
                {
                    docServicePurch1TabCollection[i].DocServicePurch1TabID = null;
                    docServicePurch1TabCollection[i].DocServicePurchID = Convert.ToInt32(docServicePurch.DocServicePurchID);
                    db.Entry(docServicePurch1TabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion


                #region 4. DocServicePurch2Tab *** *** *** *** *** *** *** *** ***

                //2.1. Удаляем записи в БД, если UPDATE
                if (entityState == EntityState.Modified)
                {
                    SQLiteParameter parDocServicePurchID = new SQLiteParameter("@DocServicePurchID", System.Data.DbType.Int32) { Value = docServicePurch.DocServicePurchID };
                    db.Database.ExecuteSqlCommand("DELETE FROM DocServicePurch2Tabs WHERE DocServicePurchID=@DocServicePurchID;", parDocServicePurchID);
                }

                //2.2. Проставляем ID-шник "DocServicePurchID" для всех позиций спецификации
                for (int i = 0; i < docServicePurch2TabCollection.Count(); i++)
                {
                    docServicePurch2TabCollection[i].DocServicePurch2TabID = null;
                    docServicePurch2TabCollection[i].DocServicePurchID = Convert.ToInt32(docServicePurch.DocServicePurchID);
                    db.Entry(docServicePurch2TabCollection[i]).State = EntityState.Added;
                }
                await db.SaveChangesAsync();

                #endregion


                //Списание Партий

                Controllers.Sklad.Rem.RemPartyMinusesController remPartyMinuses = new Rem.RemPartyMinusesController();


                #region Удаляем все записи из таблицы "RemPartyMinuses"
                //Удаляем все записи из таблицы "RemPartyMinuses"
                //Что бы правильно Проверяло на Остаток.
                //А то, товар уже списан, а я проверяю на остаток!

                await Task.Run(() => remPartyMinuses.Delete(db, Convert.ToInt32(docServicePurch.DocID))); //doc.DocID

                #endregion


                #region Проверки и Списание с партий (RemPartyMinuses)

                for (int i = 0; i < docServicePurch2TabCollection.Count(); i++)
                {
                    #region Проверка

                    //Переменные
                    int iRemPartyID = docServicePurch2TabCollection[i].RemPartyID;
                    double dQuantity = 1; // docServicePurch2TabCollection[i].Quantity;
                    //Находим партию
                    Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(iRemPartyID);
                    db.Entry(remParty).Reload(); // - Это Важно! Триггер изменил значения, то они НЕ видны в проекте, надо обновить значения!!!

                    #region 1. Есть ли остаток в партии с которой списываем!
                    if (remParty.Remnant < dQuantity)
                    {
                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg104 +

                            "<tr>" +
                            "<td>" + docServicePurch2TabCollection[i].RemPartyID + "</td>" +    //Партия
                            "<td>" + docServicePurch2TabCollection[i].DirNomenID + "</td>" +    //Код товара
                            "<td>" + 1 + "</td>" +                                              //списуемое к-во (docServicePurch2TabCollection[i].Quantity)
                            "<td>" + remParty.Remnant + "</td>" +                               //остаток партии
                            "<td>" + (1 - remParty.Remnant).ToString() + "</td>" +              //недостающее к-во (docServicePurch2TabCollection[i].Quantity - ...)
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg104_1
                        );
                    }
                    #endregion

                    #region 2. Склад: склад документа должен соответствовать каждой списуемой партии!
                    if (remParty.DirWarehouseID != docServicePurch.DirWarehouseID)
                    {
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docServicePurch.dirWarehouse.DirWarehouseName"
                        Models.Sklad.Dir.DirWarehouse dirWarehouse = await db.DirWarehouses.FindAsync(docServicePurch.DirWarehouseID);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg105 +

                            "<tr>" +
                            "<td>" + docServicePurch2TabCollection[i].RemPartyID + "</td>" +           //партия
                            "<td>" + docServicePurch2TabCollection[i].DirNomenID + "</td>" +           //Код товара
                            "<td>" + dirWarehouse.DirWarehouseName + "</td>" +                //склад документа
                            "<td>" + remParty.dirWarehouse.DirWarehouseName + "</td>" +       //склад партии
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg105_1
                        );
                    }
                    #endregion

                    #region 3. Организация: организация документа должен соответствовать каждой списуемой партии!
                    if (remParty.DirContractorIDOrg != docServicePurch.DirContractorIDOrg)
                    {
                        //Это нужно, т.к. к нам от клиента не пришли все значения модели: "docServicePurch.dirWarehouse.DirWarehouseName"
                        int iDirContractorIDOrg = docServicePurch.DirContractorIDOrg;
                        int iDirCurrencyID = docServicePurch.DirCurrencyID;
                        Models.Sklad.Dir.DirContractor dirContractor = await db.DirContractors.FindAsync(iDirContractorIDOrg);

                        throw new System.InvalidOperationException(
                            Classes.Language.Sklad.Language.msg106 +

                            "<tr>" +
                            "<td>" + docServicePurch2TabCollection[i].RemPartyID + "</td>" +           //партия
                            "<td>" + docServicePurch2TabCollection[i].DirNomenID + "</td>" +           //Код товара
                            "<td>" + dirContractor.DirContractorName + "</td>" +              //организация спецификации
                            "<td>" + remParty.dirContractorOrg.DirContractorName + "</td>" +  //организация партии
                            "</tr>" +
                            "</table>" +

                            Classes.Language.Sklad.Language.msg106_1
                        );
                    }
                    #endregion

                    #endregion


                    #region Сохранение

                    Models.Sklad.Rem.RemPartyMinus remPartyMinus = new Models.Sklad.Rem.RemPartyMinus();
                    remPartyMinus.RemPartyMinusID = null;
                    remPartyMinus.RemPartyID = docServicePurch2TabCollection[i].RemPartyID;
                    remPartyMinus.DirNomenID = docServicePurch2TabCollection[i].DirNomenID;
                    remPartyMinus.Quantity = 1; // docServicePurch2TabCollection[i].Quantity;
                    remPartyMinus.DirCurrencyID = docServicePurch2TabCollection[i].DirCurrencyID;
                    remPartyMinus.DirCurrencyMultiplicity = docServicePurch2TabCollection[i].DirCurrencyMultiplicity;
                    remPartyMinus.DirCurrencyRate = docServicePurch2TabCollection[i].DirCurrencyRate;
                    remPartyMinus.DirVatValue = docServicePurch.DirVatValue;
                    remPartyMinus.DirWarehouseID = docServicePurch.DirWarehouseID;
                    remPartyMinus.DirContractorIDOrg = docServicePurch.DirContractorIDOrg;
                    if (docServicePurch.DirContractorID > 0) remPartyMinus.DirContractorID = docServicePurch.DirContractorID;
                    else remPartyMinus.DirContractorID = docServicePurch.DirContractorIDOrg;
                    remPartyMinus.DocID = Convert.ToInt32(docServicePurch.DocID);
                    remPartyMinus.PriceCurrency = docServicePurch2TabCollection[i].PriceCurrency;
                    remPartyMinus.PriceVAT = docServicePurch2TabCollection[i].PriceVAT;
                    remPartyMinus.FieldID = Convert.ToInt32(docServicePurch2TabCollection[i].DocServicePurch2TabID);
                    remPartyMinus.Reserve = false; // docServicePurch.Reserve;

                    db.Entry(remPartyMinus).State = EntityState.Added;
                    await db.SaveChangesAsync();

                    #endregion
                }

                #endregion

                #endregion
            }
            else if (iTypeService == 3)
            {
                #region 3 - НЕ используется

                //Сохраняем в Документе только поле Held = true

                var query = await
                    (
                        from x in db.DocServicePurches
                        where x.DocServicePurchID == docServicePurch.DocServicePurchID
                        select new
                        {
                            DirServiceStatusName = x.dirServiceStatus.DirServiceStatusName
                        }
                    ).ToListAsync();

                string DirServiceStatusName = "";
                if (query.Count() > 0) DirServiceStatusName = query[0].DirServiceStatusName;


                #region 2. DocServicePurch *** *** *** *** *** *** *** *** *** ***

                //Т.к. после сохранения это поле будет == 0
                double SumTotal2 = Convert.ToDouble(docServicePurch.SumTotal2);

                //Сохраняем
                docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch.DocServicePurchID);
                docServicePurch.DirServiceStatusID = 9;
                db.Entry(docServicePurch).State = entityState;
                //db.Entry(docServicePurch).Reload();
                await db.SaveChangesAsync();

                #endregion


                #region 1. Doc *** *** *** *** *** *** *** *** *** ***

                //Модель
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docServicePurch.DocID);
                doc.Held = true;
                doc.Payment = SumTotal2; // Convert.ToDouble(docServicePurch.SumTotal2);
                doc.Description = DirServiceStatusName;

                Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
                await Task.Run(() => docs.Save());

                //Нужно вернуть "docServicePurch" со всем полями!
                docServicePurch.DocID = doc.DocID;

                #endregion

                
                #endregion
            }
            else if (iTypeService == 4)
            {
                #region 4 - НЕ используется
                //На доработку

                #region 2. DocServicePurch *** *** *** *** *** *** *** *** *** ***

                //Сохраняем
                docServicePurch = await db.DocServicePurches.FindAsync(docServicePurch.DocServicePurchID);
                docServicePurch.DirServiceStatusID = 2;
                db.Entry(docServicePurch).State = entityState;
                await db.SaveChangesAsync();

                #endregion

                #endregion
            }
            */
            #endregion



            #region n. Подтверждение транзакции - НЕ используется

            //ts.Commit(); //.Complete();

            #endregion


            return docServicePurch;
        }


        internal async Task<bool> mStatusChange(
            DbConnectionSklad db,
            System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocServicePurch docServicePurch,
            int id,
            int DirServiceStatusID,
            int DirPaymentTypeID,
            double SumTotal2a,
            string sReturnRresults,

            Classes.Account.Login.Field field, //Дополнительные данные о сотруднике

            int? KKMSCheckNumber,
            string KKMSIdCommand,

            double DiscountX,
            double DiscountY
            )
        {

            DateTime dtNow = DateTime.Now;
            //docServicePurch = await db.DocServicePurches.FindAsync(id);

            #region Проверки

            //0. Проверяем статус Аппарата
            if (docServicePurch.DirServiceStatusID == 9 && DirServiceStatusID != 2)
            {
                throw new System.InvalidOperationException("Можно сменить статус 'Выдан' только на 'В диагностике'!");
            }


            //1. если "Перемещён в БУ"
            if (Convert.ToBoolean(docServicePurch.InSecondHand))
            {
                return false;
            }


            //2. === ЛОГ ( !!! Это Сука Блядь ЛОГ и статусы тут берутся с ЛОГА !!! ) === 
            //2.1. если предыдущий статус такой же на который меняем, то не писать в Лог
            //     исключение, т.к. если в Логе нет записей с сменой статуса получим Ошибку из-за "FirstAsync()"
            //2.2. если предыдущий статус 9 и новый <> 2, то тогда исключение!
            int? DirServiceStatusID_OLD_1 = 0;
            try
            {
                var query = //await
                    (
                        from x in db.LogServices
                        where x.DocServicePurchID == id && x.DirServiceStatusID != null
                        select new
                        {
                            LogServiceID = x.LogServiceID,
                            DirServiceStatusID = x.DirServiceStatusID
                        }
                    ).OrderByDescending(x => x.LogServiceID); //.FirstAsync();

                if (await query.CountAsync() > 0)
                {
                    var _query = await query.FirstAsync();

                    if (_query.DirServiceStatusID == DirServiceStatusID)
                    {
                        return false;
                    }

                    DirServiceStatusID_OLD_1 = _query.DirServiceStatusID;
                }
            }
            catch (Exception ex) { }

            if (DirServiceStatusID_OLD_1 == 9 && DirServiceStatusID != 2)
            {
                throw new System.InvalidOperationException("Можно сменить статус 'Выдан' только на 'В диагностике'!");
            }



            #region Заказы 
            //Если в списке запчастей есть не заказы, то выдать сообщение

            if (DirServiceStatusID > 6)
            {

                //1.Получаем Docs.DocIDBase
                int? _DocID = 0;
                var queryDocIDBase = await
                    (
                        from x in db.DocServicePurches
                        where x.DocServicePurchID == docServicePurch.DocServicePurchID
                        select x
                    ).ToListAsync();
                if (queryDocIDBase.Count() > 0)
                {
                    _DocID = queryDocIDBase[0].doc.DocID;
                }

                //2. Получаем к-во заказов для ремонта
                int queryDocServicePurch2Tabs = await
                    (
                        from x in db.DocOrderInts
                        where x.doc.DocIDBase == _DocID && x.DirOrderIntStatusID < 4
                        select x
                    ).CountAsync();


                if (queryDocServicePurch2Tabs > 0)
                {
                    throw new System.InvalidOperationException("Внимание!!!<br />В списке запчастей имеет заказ(ы)! Смена статуса не возможна!");
                }

            }

            #endregion


            #endregion


            #region 1. Сохранение статуса в БД

            //Сохраняем старый статус, ниже - нужен
            int? DirServiceStatusID_OLD_2 = docServicePurch.DirServiceStatusID;

            //Если Статус == 9 (Выдан), то менять "DateDone" на текущую
            if (DirServiceStatusID == 9)
            {
                docServicePurch.DateDone = dtNow;
                docServicePurch.FromGuaranteeCount = Convert.ToInt32(docServicePurch.FromGuaranteeCount) + 1;
                docServicePurch.Summ_NotPre = SumTotal2a;
                //docServicePurch.DirServiceStatusID_789 = DirServiceStatusID_OLD; //Статус аппарата сохранить: Готов или Отказ
            }

            //Сохранить статус аппарата: Готов (7) или Отказ (8)
            if (DirServiceStatusID == 7 || DirServiceStatusID == 8) // || DirServiceStatusID == 9
            {
                //Статус аппарата сохранить: Готов или Отказ
                docServicePurch.DirServiceStatusID_789 = DirServiceStatusID; 

                //Если Отказной, то обнулить гарантию
                if (DirServiceStatusID == 8) docServicePurch.ServiceTypeRepair = 0;
            }

            //Если был Статус == 9 (Выдан) и сменили на "В диагностике", то менять "DateDone" на текущую + 7 дней (из настроек)
            bool bDirServiceLogTypeID9 = false;
            if (docServicePurch.DirServiceStatusID == 9 && DirServiceStatusID == 2)
            {
                //1. Проверяем есть ли ещё Гарантия
                if (docServicePurch.DateDone.AddMonths(docServicePurch.ServiceTypeRepair) <= dtNow && sysSetting.WarrantyPeriodPassed)
                {
                    //Исключение
                    throw new System.InvalidOperationException("Срок гарантии прошёл (до " + docServicePurch.DateDone.AddMonths(docServicePurch.ServiceTypeRepair).ToString("yyyy-MM-dd") + ")!");
                }

                //2.1. Меняем дату "Готовности"
                docServicePurch.DateDone = dtNow.AddDays(sysSetting.ReadinessDay);

                //2.2 Запоминает первичную дату документа и меняем дату документа на текущую.
                //2.2.1.
                docServicePurch.DocDate_First = docServicePurch.doc.DocDate;
                //2.2.2.
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docServicePurch.DocID);
                doc.DocDate = dtNow;
                doc.Discount = DiscountX + DiscountY;
                db.Entry(doc).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());

                //3. Суммы:
                //   3.1. Переносим PrepaymentSum в PrepaymentSum_1 (2,3,4,5) в зависимости от поля "FromGuaranteeCount" (null-1, 1-2, 2-3, 3-4, 4-5)
                switch (docServicePurch.FromGuaranteeCount)
                {
                    case 1: docServicePurch.PrepaymentSum_1 = Convert.ToDouble(docServicePurch.PrepaymentSum); break;
                    case 2: docServicePurch.PrepaymentSum_2 = Convert.ToDouble(docServicePurch.PrepaymentSum); break;
                    case 3: docServicePurch.PrepaymentSum_3 = Convert.ToDouble(docServicePurch.PrepaymentSum); break;
                    case 4: docServicePurch.PrepaymentSum_4 = Convert.ToDouble(docServicePurch.PrepaymentSum); break;
                    case 5: docServicePurch.PrepaymentSum_5 = Convert.ToDouble(docServicePurch.PrepaymentSum); break;
                }
                //   3.2. меняем сумму пред-оплаты на сумму
                docServicePurch.PrepaymentSum = Convert.ToDouble(Convert.ToDouble(docServicePurch.PrepaymentSum) + Convert.ToDouble(docServicePurch.Summ_NotPre));

                //4. Сообщение для Лога: 
                //   
                bDirServiceLogTypeID9 = true;
            }
            else if(DirServiceStatusID == 9 && DiscountX + DiscountY > 0)
            {
                Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docServicePurch.DocID);
                doc.Discount = DiscountX + DiscountY;
                db.Entry(doc).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());
            }

            //Если статус: 7 или 8, то заполняем дату "IssuanceDate"
            //!!! СУКА !!! Сдесь ОШИБКА !!! БЛЯДЬ !!!
            //if (docServicePurch.DirServiceStatusID == 7 || docServicePurch.DirServiceStatusID == 8) docServicePurch.IssuanceDate = dtNow;
            if (DirServiceStatusID == 7 || DirServiceStatusID == 8) docServicePurch.IssuanceDate = dtNow;

            //Дата смены статуса
            docServicePurch.DateStatusChange = dtNow;


            docServicePurch.DirServiceStatusID = DirServiceStatusID;
            docServicePurch.DiscountX = DiscountX;
            docServicePurch.DiscountY = DiscountY;

            db.Entry(docServicePurch).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 2. Касса или Банк

            int? DocCashOfficeSumID = null, DocBankSumID = null;
            string sSumTotal2a = "";

            //Только, если сумма больше 0
            if (DirServiceStatusID == 9 && SumTotal2a != 0)  //if (doc.Payment > 0)
            {

                #region Подсчитываем реальную сумму. Если оная отличается от "SumTotal2a", то выдаём Эксепшин
                //Алгоритм
                //1. Получаем предоплату
                //2. Получаем сумму Работ и запчастей
                //3. Отнимаем 2-1
                //4. Если сумма отличается от "SumTotal2a", то исключение

                double PriceCurrency0 = await
                    (
                        from x in db.DocServicePurches
                        where x.DocServicePurchID == id //docServicePurch.DocServicePurchID
                        select x
                    ).SumAsync(x => (double?)x.PrepaymentSum) ?? 0;

                double PriceCurrency1 = await
                    (
                        from x in db.DocServicePurch1Tabs
                        where x.DocServicePurchID == id //docServicePurch.DocServicePurchID
                        select x
                    ).SumAsync(x => (double?)x.PriceCurrency) ?? 0;

                double PriceCurrency2 = await
                    (
                        from x in db.DocServicePurch2Tabs
                        where x.DocServicePurchID == id //docServicePurch.DocServicePurchID
                        select x
                    ).SumAsync(x => (double?) x.PriceCurrency) ?? 0;

                if (Math.Round(Math.Round(SumTotal2a, 2) - (Math.Round(PriceCurrency1, 2) + Math.Round(PriceCurrency2, 2) - Math.Round(PriceCurrency0, 2)), 2) != 0)
                {
                    throw new System.InvalidOperationException("Не совпадают суммы 'SumTotal2a = ("+ SumTotal2a + ")' и самма 'PriceCurrency1 ("+ PriceCurrency1 + ") + PriceCurrency2 (" + PriceCurrency2 + ")'!");
                }

                #endregion



                sSumTotal2a = " Сумма оплаты клиентом " + SumTotal2a.ToString();

                //Получаем наименование аппарата
                Models.Sklad.Dir.DirServiceNomen dirServiceNomen = await db.DirServiceNomens.FindAsync(docServicePurch.DirServiceNomenID);
                Controllers.Sklad.Dir.DirServiceNomens.DirServiceNomensController dirServiceNomensController = new Dir.DirServiceNomens.DirServiceNomensController();
                string nomen = await dirServiceNomensController.DirServiceNomenSubFind2(dbRead, docServicePurch.DirServiceNomenID);

                //Для скидки
                Models.Sklad.Doc.Doc doc = db.Docs.Find(docServicePurch.doc.DocID);

                //1. По складу находим привязанную к нему Кассу
                Models.Sklad.Dir.DirWarehouse dirWarehouse = db.DirWarehouses.Find(docServicePurch.DirWarehouseID);
                int iDirCashOfficeID = dirWarehouse.DirCashOfficeID;
                int iDirBankID = dirWarehouse.DirBankID;

                //Касса
                if (DirPaymentTypeID == 1)
                {
                    #region Касса

                    //2. Заполняем модель "DocCashOfficeSum"
                    Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum = new Models.Sklad.Doc.DocCashOfficeSum();
                    docCashOfficeSum.DirCashOfficeID = iDirCashOfficeID;
                    docCashOfficeSum.DirCashOfficeSumTypeID = 15;
                    docCashOfficeSum.DocCashOfficeSumDate = DateTime.Now;
                    docCashOfficeSum.DocID = docServicePurch.doc.DocID; //doc.DocID;
                    docCashOfficeSum.DocXID = docServicePurch.DocServicePurchID;
                    docCashOfficeSum.DocCashOfficeSumSum = SumTotal2a - DiscountX - DiscountY; //docServicePurch.PrepaymentSum; //doc.Payment;
                    docCashOfficeSum.Description = "";
                    docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;
                    docCashOfficeSum.Base = "Оплата за аппарат: " + nomen; //dirServiceNomen.DirServiceNomenName;
                    docCashOfficeSum.KKMSCheckNumber = KKMSCheckNumber;
                    docCashOfficeSum.KKMSIdCommand = KKMSIdCommand;
                    docCashOfficeSum.KKMSEMail = "";
                    docCashOfficeSum.KKMSPhone = "";
                    docCashOfficeSum.Discount = doc.Discount;

                    //3. Пишем в Кассу
                    Doc.DocCashOfficeSums.DocCashOfficeSumsController docCashOfficeSumsController = new Doc.DocCashOfficeSums.DocCashOfficeSumsController();
                    docCashOfficeSum = await Task.Run(() => docCashOfficeSumsController.mPutPostDocCashOfficeSum(db, docCashOfficeSum, EntityState.Added));
                    DocCashOfficeSumID = docCashOfficeSum.DocCashOfficeSumID;

                    #endregion
                }
                //Банк
                else if (DirPaymentTypeID == 2)
                {
                    #region Банк
                    
                    //2. Заполняем модель "DocBankSum"
                    Models.Sklad.Doc.DocBankSum docBankSum = new Models.Sklad.Doc.DocBankSum();
                    docBankSum.DirBankID = iDirBankID;
                    docBankSum.DirBankSumTypeID = 14; //Изъятие из кассы на основании проведения приходной накладной №
                    docBankSum.DocBankSumDate = DateTime.Now;
                    docBankSum.DocID = docServicePurch.doc.DocID; //doc.DocID;
                    docBankSum.DocXID = docServicePurch.DocServicePurchID;
                    docBankSum.DocBankSumSum = SumTotal2a - DiscountX - DiscountY; //docServicePurch.PrepaymentSum; //doc.Payment;
                    docBankSum.Description = "";
                    docBankSum.DirEmployeeID = field.DirEmployeeID;
                    docBankSum.Base = "Оплата за аппарат: " + dirServiceNomen.DirServiceNomenName;
                    docBankSum.KKMSCheckNumber = KKMSCheckNumber;
                    docBankSum.KKMSIdCommand = KKMSIdCommand;
                    docBankSum.KKMSEMail = "";
                    docBankSum.KKMSPhone = "";
                    docBankSum.Discount = doc.Discount;

                    //3. Пишем в Банк
                    Doc.DocBankSums.DocBankSumsController docBankSumsController = new Doc.DocBankSums.DocBankSumsController();
                    docBankSum = await Task.Run(() => docBankSumsController.mPutPostDocBankSum(db, docBankSum, EntityState.Added));
                    DocBankSumID = docBankSum.DocBankSumID;

                    #endregion
                }
                else
                {
                    throw new System.InvalidOperationException("Не выбран метод оплаты: Касса или Банк!");
                }

            }

            #endregion


            #region DirServiceStatusID == 9: DocServicePurch1Tab, DocServicePurch2Tab
            //Ну и надо Работы и запчасти пометить как оплоченные!
            //Но, только новые. То есть аппарат могут вернуть несколько раз надоработку.

            if (DirServiceStatusID == 9)
            {
                #region Находим максимум "RemontN" для каждой таблицы: DocServicePurch1Tab и DocServicePurch2Tab

                var query_DocServicePurch1Tab_RemontN_Max = await
                    (
                        from x in db.DocServicePurch1Tabs
                        where x.DocServicePurchID == id
                        select x
                    ).MaxAsync(x => x.RemontN);
                if (query_DocServicePurch1Tab_RemontN_Max == null) query_DocServicePurch1Tab_RemontN_Max = 0;
                 query_DocServicePurch1Tab_RemontN_Max++;


                var query_DocServicePurch2Tab_RemontN_Max = await
                    (
                        from x in db.DocServicePurch2Tabs
                        where x.DocServicePurchID == id
                        select x
                    ).MaxAsync(x => x.RemontN);
                if (query_DocServicePurch2Tab_RemontN_Max == null) query_DocServicePurch2Tab_RemontN_Max = 0;
                query_DocServicePurch2Tab_RemontN_Max++;


                //Берём максимальный!!!
                if (query_DocServicePurch1Tab_RemontN_Max > query_DocServicePurch2Tab_RemontN_Max) query_DocServicePurch2Tab_RemontN_Max = query_DocServicePurch1Tab_RemontN_Max;
                else query_DocServicePurch1Tab_RemontN_Max = query_DocServicePurch2Tab_RemontN_Max;

                #endregion


                    //DocServicePurch1Tab === === === === === === === === === === ===
                List<Models.Sklad.Doc.DocServicePurch1Tab> listDocServicePurch1Tab =
                    (
                        from x in db.DocServicePurch1Tabs
                        where x.DocServicePurchID == id && x.PayDate == null
                        select x
                    ).ToList();

                foreach (Models.Sklad.Doc.DocServicePurch1Tab docServicePurch1Tab in listDocServicePurch1Tab)
                {
                    docServicePurch1Tab.PayDate = dtNow;
                    docServicePurch1Tab.RemontN = query_DocServicePurch1Tab_RemontN_Max;
                    docServicePurch1Tab.DocCashOfficeSumID = DocCashOfficeSumID;
                    docServicePurch1Tab.DocBankSumID = DocBankSumID;
                    db.Entry(docServicePurch1Tab).State = EntityState.Modified;
                }

                //DocServicePurch2Tab === === === === === === === === === === ===
                List<Models.Sklad.Doc.DocServicePurch2Tab> listDocServicePurch2Tab =
                    (
                        from x in db.DocServicePurch2Tabs
                        where x.DocServicePurchID == id && x.PayDate == null
                        select x
                    ).ToList();

                foreach (Models.Sklad.Doc.DocServicePurch2Tab docServicePurch2Tab in listDocServicePurch2Tab)
                {
                    docServicePurch2Tab.PayDate = dtNow;
                    docServicePurch2Tab.RemontN = query_DocServicePurch2Tab_RemontN_Max;
                    docServicePurch2Tab.DocCashOfficeSumID = DocCashOfficeSumID;
                    docServicePurch2Tab.DocBankSumID = DocBankSumID;
                    db.Entry(docServicePurch2Tab).State = EntityState.Modified;
                }

                //Сохраняем
                await Task.Run(() => db.SaveChangesAsync());
            }

            #endregion


            #region 3. Лог


            #region Пишем в Лог о смене статуса и мастера, если такое было

            logService.DocServicePurchID = id;
            if(!bDirServiceLogTypeID9) logService.DirServiceLogTypeID = 1; //Смена статуса
            else logService.DirServiceLogTypeID = 9; //Возврат по гарантии
            logService.DirEmployeeID = field.DirEmployeeID;
            logService.DirServiceStatusID = DirServiceStatusID;
            if(!String.IsNullOrEmpty(sReturnRresults)) logService.Msg = sReturnRresults;
            if (DirServiceStatusID == 9 && DiscountX > 0 || DiscountY > 0) logService.Msg += " Скидки на работы: " + DiscountX + ", на запчасти: " + DiscountY;

             await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

            #endregion


            #endregion


            #region 4. Заполняем DirServiceContractors
            //Надо ввести доп.поле статуса в "DocServicePurches" с предыдущим статусом: "Готов" или "Отказ"
            //Когда нажимаем выдан, то заполнять это поле.
            //Нужно для:
            // - Справочника "DirServiceContractor" поля: QuantityOk и QuantityFail
            // - Для статистики сколько Готовых, сколько Отказных


            //Если в Логе НЕТ записей, что вернут на доработку
            var queryLogCount = await
                (
                    from x in db.LogServices
                    where x.DirServiceLogTypeID == 9 && x.DocServicePurchID == id
                    select x
                ).CountAsync();


            //1. Находим Клиента по 
            if (
                queryLogCount == 0 &&
                DirServiceStatusID == 9 && 
                docServicePurch.DirServiceContractorID != null && 
                docServicePurch.DirServiceContractorID > 0
               )
            {
                Models.Sklad.Dir.DirServiceContractor dirServiceContractor = await db.DirServiceContractors.FindAsync(docServicePurch.DirServiceContractorID);

                //2. К-во (3 шт)
                if (DirServiceStatusID_OLD_2 == 7)
                {
                    //Выдан
                    dirServiceContractor.QuantityOk = dirServiceContractor.QuantityOk + 1;
                }
                else if (DirServiceStatusID_OLD_2 == 8)
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
            Models.Sklad.Doc.DocServicePurch docServicePurch,
            int id,
            int ServiceTypeRepair,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            docServicePurch = await db.DocServicePurches.FindAsync(id);

            #region Проверка

            //1. если "Перемещён в БУ"
            if (Convert.ToBoolean(docServicePurch.InSecondHand))
            {
                return false;
            }

            #endregion

            #region 1. Сохранение статуса в БД

            if (docServicePurch.ServiceTypeRepair == ServiceTypeRepair) { return false; }
            else { logService.Msg = "Была: " + docServicePurch.ServiceTypeRepair + " поменяли на: " + ServiceTypeRepair; }

            docServicePurch.ServiceTypeRepair = ServiceTypeRepair;
            db.Entry(docServicePurch).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 3. Лог
            
            logService.DocServicePurchID = id;
            logService.DirServiceLogTypeID = 8;
            logService.DirEmployeeID = field.DirEmployeeID;
            //logService.Msg = "Была гарантия: "; //Выше изменили!!!

            await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);
            
            #endregion


            return true;
        }


        internal async Task<bool> mDirEmployeeIDChange(
            DbConnectionSklad db,
            System.Data.Entity.DbContextTransaction ts,
            Models.Sklad.Doc.DocServicePurch docServicePurch,
            int id,
            int DirEmployeeID,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            #region Проверка

            //1. если "Перемещён в БУ"
            if (Convert.ToBoolean(docServicePurch.InSecondHand))
            {
                return false;
            }

            #endregion

            #region 1. Сохранение статуса в БД

            //docServicePurch = await db.DocServicePurches.FindAsync(id);

            //Ищим наименование Сотрудников
            Models.Sklad.Dir.DirEmployee dirEmployee1 = await dbRead.DirEmployees.FindAsync(docServicePurch.DirEmployeeIDMaster);
            Models.Sklad.Dir.DirEmployee dirEmployee2 = await dbRead.DirEmployees.FindAsync(DirEmployeeID);

            if (docServicePurch.DirEmployeeIDMaster == DirEmployeeID) { return false; }
            else
            {
                //logService.Msg = "Был мастер №" + docServicePurch.DirEmployeeIDMaster + " поменяли на мастера №" + DirEmployeeID;
                logService.Msg = "Был мастер " + dirEmployee1.DirEmployeeName + " поменяли на мастера " + dirEmployee2.DirEmployeeName;
            }

            docServicePurch.DirEmployeeIDMaster = DirEmployeeID;
            db.Entry(docServicePurch).State = EntityState.Modified;
            await Task.Run(() => db.SaveChangesAsync());

            #endregion


            #region 3. Лог

            logService.DocServicePurchID = id;
            logService.DirServiceLogTypeID = 7;
            logService.DirEmployeeID = field.DirEmployeeID;
            //logService.Msg = "Была гарантия: "; //Выше изменили!!!

            await logServicesController.mPutPostLogServices(db, logService, EntityState.Added);

            #endregion


            return true;
        }


        internal async Task<string[]> mPatchFull(DbConnectionSklad db, int id)
        {
            /*
            //1. Получаем Sub аппарата по "DocServicePurchID" (id)
            string DirServiceNomenPatchFull = null;

            var querySub = await Task.Run(() =>
                 (
                    from x in db1.DocServicePurches
                    where x.DocServicePurchID == id
                    select new
                    {
                        Sub = x.dirServiceNomen.Sub
                    }
                ).ToArrayAsync());

            if (querySub.Count() > 0)
            {
                int? iSub = querySub[0].Sub;

                Controllers.Sklad.Dir.DirServiceNomens.DirServiceNomensController dirServiceNomensController = new Dir.DirServiceNomens.DirServiceNomensController();
                DirServiceNomenPatchFull = await Task.Run(() => dirServiceNomensController.DirServiceNomenSubFind2(db1, iSub));
            }

            return DirServiceNomenPatchFull;
            */

            ArrayList alNameSpase = new ArrayList();
            ArrayList alNameSpaseNo = new ArrayList();
            ArrayList alNameID = new ArrayList();

            int? Sub = id;

            while (Sub > 0)
            {
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
                }
                else
                {
                    Sub = null;
                }
            }

            string[] ret = new string[3];
            for (int i = alNameSpase.Count - 1; i >= 0; i--)
            {
                ret[0] += alNameSpase[i].ToString();
                ret[1] += alNameSpaseNo[i].ToString();
                ret[2] += alNameID[i].ToString();
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
            string SQL = "";

            SQL =
                
                "SELECT " +
                " [DocServicePurches].[DocServicePurchID] AS [DocServicePurchID], [Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], [Docs].[Base] AS [Base],  [Docs].[Held] AS [Held], [Docs].[Discount] AS [Discount], [Docs].[Description] AS [Description], [Docs].[DirVatValue] AS [DirVatValue],  [DocServicePurches].[SerialNumber] AS [DeviceSerialNumber], " +
                " CASE [DocServicePurches].[TypeRepair]  WHEN [TypeRepair] = 1 THEN 'Не гарантийный' ELSE 'Гарантийный' END AS [TypeRepair], " +
                " CASE [DocServicePurches].[ComponentDevice]  WHEN [ComponentDevice] = 1 THEN 'Аппарат' ELSE '-' END AS [ComponentDevice], " +
                " CASE [DocServicePurches].[ComponentBattery]  WHEN [ComponentBattery] = 1 THEN 'Аккумулятор' ELSE '-' END AS [ComponentBattery],[DocServicePurches].[ComponentBatterySerial] AS [ComponentBatterySerial], " +
                " CASE [DocServicePurches].[ComponentBackCover]  WHEN [ComponentBackCover] = 1 THEN 'Задняя крышка' ELSE '-' END AS [ComponentBackCover],[DocServicePurches].[ComponentPasTextNo] AS [ComponentPasTextNo], [DocServicePurches].[ComponentPasText] AS [ComponentPasText], [DocServicePurches].[ComponentOtherText] AS [ComponentOtherText], [DocServicePurches].[ProblemClientWords] AS [ProblemClientWords], [DocServicePurches].[Note] AS [Note], [DocServicePurches].[DirServiceContractorName] AS [DirContractorName], [DocServicePurches].[DirServiceContractorAddress] AS [DirContractorAddress], [DocServicePurches].[DirServiceContractorPhone] AS [DirContractorPhone], [DocServicePurches].[DirServiceContractorEmail] AS [DirContractorEmail], [DocServicePurches].[DirServiceContractorRegular] AS [DirServiceContractorRegular], [DocServicePurches].[PriceVAT] AS [PriceVATEstimated], [DocServicePurches].[DateDone] AS [DateDone], [DocServicePurches].[UrgentRepairs] AS [UrgentRepairs], [DocServicePurches].[Prepayment] AS [Prepayment], [DocServicePurches].[PrepaymentSum] AS [PrepaymentSum], " +
                " [DirServiceNomens].[DirServiceNomenID] AS [DirServiceNomenID], " +
                " [DocServicePurches].[ServiceTypeRepair], " +
                

                " CASE " +
                "   WHEN dirServiceNomensSubGroup.DirServiceNomenID IS NULL THEN " +

                "      CASE WHEN dirServiceNomensGroup.DirServiceNomenID IS NULL THEN " +

                "           CASE WHEN DirServiceNomens.DirServiceNomenID IS NULL THEN " +

                "                'Ошибка: нет аппарата (ID IS NULL)' " +
                //"                END " +

                "           ELSE " +
                "             DirServiceNomens.DirServiceNomenName " +
                //"             END " +
                "           END " +

                "       ELSE " +
                "           dirServiceNomensGroup.DirServiceNomenName || ' / ' || DirServiceNomens.DirServiceNomenName " +
                //"           END " +
                "       END " +

                "   ELSE " +
                "       dirServiceNomensSubGroup.DirServiceNomenName || ' / ' || dirServiceNomensGroup.DirServiceNomenName || ' / ' || DirServiceNomens.DirServiceNomenName " +
                "   END " +
                //" END  " +
                "AS [DirServiceNomenName], " +




                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                " [DirServiceNomens].[DirServiceNomenNameFull] AS [DirServiceNomenNameFull], [DirServiceNomens].[Description] AS [Description], [DirServiceNomens].[DescriptionFull] AS [DescriptionFull], " +

                " [DirServiceStatuses].[DirServiceStatusName] AS [DirServiceStatusName], [DirBanksOrg].[DirBankName] AS [DirBankNameOrg], [DirBanksOrg].[DirBankMFO] AS [DirBankMFOOrg], [DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], [DirContractorOrg].[DirContractorEmail] AS [DirContractorEmailOrg], [DirContractorOrg].[DirContractorWWW] AS [DirContractorWWWOrg], [DirContractorOrg].[DirContractorAddress] AS [DirContractorAddressOrg], [DirContractorOrg].[DirContractorLegalCertificateDate] AS [DirContractorLegalCertificateDateOrg], [DirContractorOrg].[DirContractorLegalTIN] AS [DirContractorLegalTINOrg], [DirContractorOrg].[DirContractorLegalCAT] AS [DirContractorLegalCATOrg], [DirContractorOrg].[DirContractorLegalCertificateNumber] AS [DirContractorLegalCertificateNumberOrg], [DirContractorOrg].[DirContractorLegalBIN] AS [DirContractorLegalBINOrg], [DirContractorOrg].[DirContractorLegalOGRNIP] AS [DirContractorLegalOGRNIPOrg], [DirContractorOrg].[DirContractorLegalRNNBO] AS [DirContractorLegalRNNBOOrg], [DirContractorOrg].[DirContractorDesc] AS [DirContractorDescOrg],  [DirContractorOrg].[DirContractorLegalPasIssued] AS [DirContractorLegalPasIssuedOrg],  [DirContractorOrg].[DirContractorLegalPasDate] AS [DirContractorLegalPasDateOrg],  [DirContractorOrg].[DirContractorLegalPasCode] AS [DirContractorLegalPasCodeOrg],  [DirContractorOrg].[DirContractorLegalPasNumber] AS [DirContractorLegalPasNumberOrg],  [DirContractorOrg].[DirContractorLegalPasSeries] AS [DirContractorLegalPasSeriesOrg],  [DirContractorOrg].[DirContractorDiscount] AS [DirContractorDiscountOrg],  [DirContractorOrg].[DirContractorPhone] AS [DirContractorPhoneOrg],  [DirContractorOrg].[DirContractorFax] AS [DirContractorFaxOrg],  [DirContractorOrg].[DirContractorLegalAddress] AS [DirContractorLegalAddressOrg],  [DirContractorOrg].[DirContractorLegalName] AS [DirContractorLegalNameOrg], [DirWarehouses].[DirWarehouseName] AS [DirWarehouseName],  [DirWarehouses].[DirWarehouseAddress] AS [DirWarehouseAddress],  [DirWarehouses].[DirWarehouseDesc] AS [DirWarehouseDesc],  [DirWarehouses].[Phone] AS [Phone], [DirEmployees].[DirEmployeeName] AS [DirEmployeeName], [Docs].[NumberInt] AS [NumberInt] ," +

                " [DocServicePurches].[DateStatusChange] AS [DateStatusChange], " +


                " CASE " +
                "   WHEN  [DocServicePurches].[DirServiceStatusID] IS NULL THEN NULL " +
                "   ELSE [DocServicePurches].[DateStatusChange] " +
                "   END " +
                " AS [DateVIDACHI] " + 


                "FROM [DocServicePurches]  " +
                "INNER JOIN [Docs] ON [Docs].[DocID] = [DocServicePurches].[DocID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocServicePurches].[DirServiceNomenID] " +    //Товар
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +            //Под-Группа
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [dirServiceNomensGroup].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " + //Группа
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "INNER JOIN [DirServiceStatuses] ON [DirServiceStatuses].[DirServiceStatusID] = [DocServicePurches].[DirServiceStatusID] INNER JOIN [DirPaymentTypes] ON [DirPaymentTypes].[DirPaymentTypeID] = [Docs].[DirPaymentTypeID] INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] INNER JOIN [DirWarehouses] ON [DirWarehouses].[DirWarehouseID] = [DocServicePurches].[DirWarehouseID] INNER JOIN [DirEmployees] ON [DirEmployees].[DirEmployeeID] = [Docs].[DirEmployeeID] LEFT JOIN [DirBanks] AS [DirBanksOrg] ON [DirBanksOrg].[DirBankID] = [DirContractorOrg].[DirBankID] " +
                "WHERE (Docs.DocID=@DocID) ";



            return SQL;
        }


        //Сумма документа
        internal string GenerateSQLSUM(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";

            SQL =

                "SELECT " +

                "[DocDate] AS [DocDate], [DocDate] AS [DocDate_InWords], " +
                "Discount AS Discount, " +

                " SUM(CountRecord1 + CountRecord2) AS CountRecord, " +
                " SUM(CountRecord1 + CountRecord2) AS CountRecord_NumInWords, " +

                " SUM(SumDocServicePurch1Tabs) AS SumDocServicePurch1Tabs, " +
                " SUM(SumDocServicePurch1Tabs) AS SumDocServicePurch1Tabs_InWords, " +

                " SUM(SumDocServicePurch2Tabs) AS SumDocServicePurch2Tabs, " +
                " SUM(SumDocServicePurch2Tabs) AS SumDocServicePurch2Tabs_InWords, " +

                " SUM(SumDocServicePurch1Tabs) + SUM(SumDocServicePurch2Tabs) AS SumTotal, " +
                " SUM(SumDocServicePurch1Tabs) + SUM(SumDocServicePurch2Tabs) AS SumTotal_InWords, " +

                " [PrepaymentSum] AS PrepaymentSum, " +

                " SUM(SumDocServicePurch1Tabs) + SUM(SumDocServicePurch2Tabs) - [PrepaymentSum] AS SumTotal2, " +

                "[DirContractorName] AS [DirContractorName], " +
                "[DirContractorAddress] AS [DirContractorAddress], " +
                "[DirContractorPhone] AS [DirContractorPhone], " +
                "[DirContractorEmail] AS [DirContractorEmail], " +
                "[DirServiceContractorRegular] AS [DirServiceContractorRegular], " + //Постоянный
                "[ServiceTypeRepair], " +

                "[DateVIDACHI] " + 


                "FROM " +
                "(" +

                "SELECT " +
                "[Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], " +
                "Docs.Discount AS Discount, " +

                "COUNT(*) CountRecord1, " +
                "0 CountRecord2, " +

                //1. Подсчет табличной части Работы "SumDocServicePurch1Tabs"
                "ROUND((SUM(DocServicePurch1Tabs.PriceCurrency)), " + sysSettings.FractionalPartInSum + ") AS SumDocServicePurch1Tabs, " +
                "0 AS SumDocServicePurch2Tabs, " +

                //4. Константа "PrepaymentSum"
                "[DocServicePurches].[PrepaymentSum] AS [PrepaymentSum], " +

                "[DocServicePurches].[DirServiceContractorName] AS [DirContractorName], " +
                "[DocServicePurches].[DirServiceContractorAddress] AS [DirContractorAddress], " +
                "[DocServicePurches].[DirServiceContractorPhone] AS [DirContractorPhone], " +
                "[DocServicePurches].[DirServiceContractorEmail] AS [DirContractorEmail], " +
                "[DocServicePurches].[DirServiceContractorRegular] AS [DirServiceContractorRegular], " + //Постоянный
                "CASE WHEN ([DocServicePurches].[ServiceTypeRepair] IS NULL) THEN 1 ELSE [DocServicePurches].[ServiceTypeRepair] END AS [ServiceTypeRepair], " +

                " CASE " +
                "   WHEN  [DocServicePurches].[DirServiceStatusID] IS NULL THEN NULL " +
                "   ELSE [DocServicePurches].[DateStatusChange] " +
                "   END " +
                " AS [DateVIDACHI] " +


                "FROM Docs, DocServicePurches " +
                " LEFT JOIN DocServicePurch1Tabs ON (DocServicePurch1Tabs.DocServicePurchID=DocServicePurches.DocServicePurchID) " + 

                "WHERE (Docs.DocID=DocServicePurches.DocID)and(Docs.DocID=@DocID) " +



                " UNION " +



                "SELECT " +
                "[Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], " +
                "Docs.Discount AS Discount, " +

                "0 CountRecord1, " +
                "COUNT(*) CountRecord2, " +

                //1. Подсчет табличной части Работы "SumDocServicePurch2Tabs"
                "0 AS SumDocServicePurch1Tabs, " +
                "ROUND((SUM(DocServicePurch2Tabs.PriceCurrency)), " + sysSettings.FractionalPartInSum + ") AS SumDocServicePurch2Tabs, " +

                //4. Константа "PrepaymentSum"
                "[DocServicePurches].[PrepaymentSum] AS [PrepaymentSum], " +

                "[DocServicePurches].[DirServiceContractorName] AS [DirContractorName], " +
                "[DocServicePurches].[DirServiceContractorAddress] AS [DirContractorAddress], " +
                "[DocServicePurches].[DirServiceContractorPhone] AS [DirContractorPhone], " +
                "[DocServicePurches].[DirServiceContractorEmail] AS [DirContractorEmail], " +
                "[DocServicePurches].[DirServiceContractorRegular] AS [DirServiceContractorRegular], " + //Постоянный
                "CASE WHEN ([DocServicePurches].[ServiceTypeRepair] IS NULL) THEN 1 ELSE [DocServicePurches].[ServiceTypeRepair] END AS [ServiceTypeRepair], " +

                " CASE " +
                "   WHEN  [DocServicePurches].[DirServiceStatusID] IS NULL THEN NULL " +
                "   ELSE [DocServicePurches].[DateStatusChange] " +
                "   END " +
                " AS [DateVIDACHI] " +


                "FROM Docs, DocServicePurches " +
                " LEFT JOIN DocServicePurch2Tabs ON (DocServicePurch2Tabs.DocServicePurchID=DocServicePurches.DocServicePurchID) " + 
                "WHERE (Docs.DocID=DocServicePurches.DocID)and(Docs.DocID=@DocID) " +

                ")";




            return SQL;
        }


        #endregion
    }
}