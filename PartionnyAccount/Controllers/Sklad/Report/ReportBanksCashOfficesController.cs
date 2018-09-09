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
using System.Collections;

namespace PartionnyAccount.Controllers.Sklad.Report
{
    public class ReportBanksCashOfficesController : ApiController
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

        int ListObjectID = 32;

        #endregion


        #region SELECT

        string pID = "";
        bool ProfitNomenAll = false;
        int pLanguage = 0, CasheBank = 0, DirWarehouseID = 0, DirCashOfficeID = 0, DirBankID = 0, DirEmployeeID = 0, ReportType = 0, ReportGroup = 0, DocXID = 0;
        bool CasheAndBank = false, Cashe = false, Bank = false;
        string DirWarehouseName, DirEmployeeName, ReportTypeName, ReportGroupName;
        DateTime DateS, DatePo;
        ArrayList alDirNomenPatchFull = new ArrayList();

        // GET: api/DocSales
        public async Task<IHttpActionResult> GetDocSales(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightReportBanksCashOffices"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                //Получаем сотрудника: если к нему привязан Склад и/или Организация, то выбираем документы только по этим характеристикам
                Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(field.DirEmployeeID);

                #endregion


                #region Параметры

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();

                int limit = sysSetting.PageSizeJurn;
                int page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);
                int Skip = limit * (page - 1);

                CasheAndBank = Convert.ToBoolean(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "CasheAndBank", true) == 0).Value);
                Cashe = Convert.ToBoolean(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "Cashe", true) == 0).Value);
                Bank = Convert.ToBoolean(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "Bank", true) == 0).Value);

                DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (DateS < Convert.ToDateTime("01.01.1800")) DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else DateS = DateS.AddDays(-1);

                DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (DatePo < Convert.ToDateTime("01.01.1800")) DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                ProfitNomenAll = false;
                bool bProfitNomenAll = Boolean.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ProfitNomenAll", true) == 0).Value, out ProfitNomenAll);

                //DirContractorIDOrg = 0;
                //bool bDirContractorIDOrg = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorIDOrg", true) == 0).Value, out DirContractorIDOrg);
                //DirContractorNameOrg = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorNameOrg", true) == 0).Value;

                DirCashOfficeID = 0;
                bool bDirCashOfficeID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirCashOfficeID", true) == 0).Value, out DirCashOfficeID);
                DirWarehouseName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirCashOfficeName", true) == 0).Value;

                DirBankID = 0;
                bool bDirBankID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirBankID", true) == 0).Value, out DirBankID);
                DirWarehouseName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirCashOfficeName", true) == 0).Value;

                if (DirCashOfficeID == 0 && DirBankID == 0)
                {
                    DirWarehouseID = 0;
                    bool bDirWarehouseID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value, out DirWarehouseID);
                    DirWarehouseName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseName", true) == 0).Value;
                }

                DirEmployeeID = 0;
                bool bDirEmployeeID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value, out DirEmployeeID);
                DirEmployeeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeName", true) == 0).Value;

                //DirPriceTypeID = 0;
                //bool bDirPriceTypeID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPriceTypeID", true) == 0).Value, out DirPriceTypeID);
                //DirPriceTypeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPriceTypeName", true) == 0).Value;

                ReportType = 0;
                bool bReportType = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportType", true) == 0).Value, out ReportType);
                ReportTypeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportTypeName", true) == 0).Value;

                ReportGroup = 0;
                bool bReportGroup = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportGroup", true) == 0).Value, out ReportGroup);
                ReportGroupName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportGroupName", true) == 0).Value;

                DocXID = 0;
                bool bDocXID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocXID", true) == 0).Value, out DocXID);

                #endregion


                #region По складу определяем кассу и банк

                //int DirCashOfficeID = 0, DirBankID = 0;
                string DirCashOfficeName = "", DirBankName = "";
                if ((DirCashOfficeID == 0 || DirBankID == 0) && DirWarehouseID > 0)
                {
                    var queryCashBank = await Task.Run(() =>
                        (
                            from x in db.DirWarehouses
                            where x.DirWarehouseID == DirWarehouseID
                            select x
                        ).ToListAsync());

                    if (queryCashBank.Count() > 0)
                    {
                        DirCashOfficeID = queryCashBank[0].DirCashOfficeID; DirCashOfficeName = queryCashBank[0].DirWarehouseName;
                        DirBankID = queryCashBank[0].DirBankID; DirBankName = queryCashBank[0].DirWarehouseName;
                    }
                }
                else
                {
                    //...
                }

                #endregion



                if (ReportType == 1)
                {
                    #region Все операции

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        if (ReportGroup == 0|| ReportGroup == 1)
                        {
                            #region Полный отчет

                            #region queryTemp

                            var queryTemp =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID, // == null ? 0 : x.DocID,
                                    DocXID = x.DocXID, // == null ? 0 : x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                                                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber, // == null ? 0 : x.Discount,
                                }
                            ).Union
                                (
                                    from x in db.DocBankSums
                                    where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                    select new
                                    {

                                        Base = x.Base,
                                        DirEmployeeID = x.DirEmployeeID,
                                        DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                        Description = x.Description,
                                        DirCashOfficeID = 0,
                                        DirBankID = x.DirBankID,
                                        DirCashOfficeBankName = x.dirBank.DirBankName,
                                        DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                        DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                        DirCurrencyID = x.DirCurrencyID,
                                        DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                        DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                        DirCurrencyRate = x.DirCurrencyRate,
                                        DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                        DocCashOfficeBankSumID = x.DocBankSumID,
                                        DocCashOfficeBankSumSum = x.DocBankSumSum,
                                        DocID = x.DocID,
                                        DocXID = x.DocXID,
                                        DirWarehouseName = DirBankName,
                                        Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                                                                        //KKMS
                                        KKMSCheckNumber = x.KKMSCheckNumber,
                                    }
                                );


                            if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }

                            if (DocXID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                            }

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 2)
                        {
                            #region Группировать по дням

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,

                                    DirEmployeeID = x.DirEmployeeID,

                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                }

                            )
                            .Concat
                            (
                                from x in db.DocBankSums
                                where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,

                                    DirEmployeeID = x.DirEmployeeID,

                                    DirCashOfficeID = 0,
                                    DirBankID = x.DirBankID,
                                }
                            );



                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp = 
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                ); 
                            
                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 3)
                        {
                            #region Группировать по дням и точке

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,

                                    DirEmployeeID = x.DirEmployeeID,

                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                }

                            )
                            .Concat
                            (
                                from x in db.DocBankSums
                                where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,

                                    DirEmployeeID = x.DirEmployeeID,

                                    DirCashOfficeID = 0,
                                    DirBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                }
                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        //DirCashOfficeID = x.DirCashOfficeID,
                                        //DirBankID = x.DirBankID,
                                        DirCashOfficeBankName = x.DirCashOfficeBankName
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        //DirCashOfficeID = g.Key.DirCashOfficeID,
                                        //DirBankID = g.Key.DirBankID,
                                        DirCashOfficeBankName = g.Key.DirCashOfficeBankName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 4)
                        {
                            #region Группировать по дням и сотруднику

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,

                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,

                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                }

                            )
                            .Concat
                            (
                                from x in db.DocBankSums
                                where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,

                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,

                                    DirCashOfficeID = 0,
                                    DirBankID = x.DirBankID,
                                }
                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        //DirEmployeeID = x.DirEmployeeID,
                                        DirEmployeeName = x.DirEmployeeName,
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        //DirEmployeeID = g.Key.DirEmployeeID,
                                        DirEmployeeName = g.Key.DirEmployeeName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirEmployeeID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 5)
                        {
                            #region Группировать по дням, точке и сотруднику

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,

                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,

                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                }

                            )
                            .Concat
                            (
                                from x in db.DocBankSums
                                where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,

                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,

                                    DirCashOfficeID = 0,
                                    DirBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                }
                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        DirEmployeeName = x.DirEmployeeName,
                                        DirCashOfficeBankName = x.DirCashOfficeBankName
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        DirEmployeeName = g.Key.DirEmployeeName,
                                        DirCashOfficeBankName = g.Key.DirCashOfficeBankName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        if (ReportGroup == 0 || ReportGroup == 1)
                        {
                            #region Полный отчет

                            #region queryTemp

                            var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                            }
                            else if (DirCashOfficeID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }

                            if (DocXID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                            }

                            #endregion


                            #region Параметры

                            if ((DirCashOfficeID > 0 || DirBankID > 0) && DirWarehouseID == 0)
                            {
                                int dirCount = queryTemp.Count();

                                queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate).Skip(Skip).Take(limit);

                                //К-во Номенклатуры
                                //int dirCount = await Task.Run(() => db.DocAccounts.Where(x => x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo).Count());

                                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                                int dirCount2 = queryTemp.Count();
                                if (dirCount2 < limit) dirCount = limit * (page - 1) + dirCount2;

                                dynamic collectionWrapper1 = new
                                {
                                    sucess = true,
                                    total = dirCount,
                                    ReportBanksCashOffices = queryTemp
                                };
                                return await Task.Run(() => Ok(collectionWrapper1));
                            }

                            else
                            {
                                queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                                int dirCount = queryTemp.Count();

                                dynamic collectionWrapper1 = new
                                {
                                    sucess = true,
                                    total = dirCount,
                                    ReportBanksCashOffices = queryTemp
                                };
                                return await Task.Run(() => Ok(collectionWrapper1));
                            }

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 2)
                        {
                            #region Группировать по дням

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,

                                    DirEmployeeID = x.DirEmployeeID,

                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                }

                            );



                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 3)
                        {
                            #region Группировать по дням и точке

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,

                                    DirEmployeeID = x.DirEmployeeID,

                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                }

                            );


                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        //DirCashOfficeID = x.DirCashOfficeID,
                                        //DirBankID = x.DirBankID,
                                        DirCashOfficeBankName = x.DirCashOfficeBankName
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        //DirCashOfficeID = g.Key.DirCashOfficeID,
                                        //DirBankID = g.Key.DirBankID,
                                        DirCashOfficeBankName = g.Key.DirCashOfficeBankName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 4)
                        {
                            #region Группировать по дням и сотруднику

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,

                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,

                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                }

                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        //DirEmployeeID = x.DirEmployeeID,
                                        DirEmployeeName = x.DirEmployeeName,
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        //DirEmployeeID = g.Key.DirEmployeeID,
                                        DirEmployeeName = g.Key.DirEmployeeName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirEmployeeID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 5)
                        {
                            #region Группировать по дням, точке и сотруднику

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocCashOfficeSums
                                where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,

                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,

                                    DirCashOfficeID = x.DirCashOfficeID,
                                    DirBankID = 0,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                }

                            );


                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        DirEmployeeName = x.DirEmployeeName,
                                        DirCashOfficeBankName = x.DirCashOfficeBankName
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        DirEmployeeName = g.Key.DirEmployeeName,
                                        DirCashOfficeBankName = g.Key.DirCashOfficeBankName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }

                        #endregion

                    }
                    else if (Bank)
                    {
                        #region Bank

                        if (ReportGroup == 0 || ReportGroup == 1)
                        {
                            #region Полный отчет

                            #region queryTemp

                            var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                            }
                            else if (DirBankID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }

                            if (DocXID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                            }

                            #endregion


                            #region Параметры

                            if ((DirCashOfficeID > 0 || DirBankID > 0) && DirWarehouseID == 0)
                            {
                                int dirCount = queryTemp.Count();

                                queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate).Skip(Skip).Take(limit);

                                //К-во Номенклатуры
                                //int dirCount = await Task.Run(() => db.DocAccounts.Where(x => x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo).Count());

                                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                                int dirCount2 = queryTemp.Count();
                                if (dirCount2 < limit) dirCount = limit * (page - 1) + dirCount2;

                                dynamic collectionWrapper1 = new
                                {
                                    sucess = true,
                                    total = dirCount,
                                    ReportBanksCashOffices = queryTemp
                                };
                                return await Task.Run(() => Ok(collectionWrapper1));
                            }

                            else
                            {
                                int dirCount = queryTemp.Count();

                                queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                                dynamic collectionWrapper1 = new
                                {
                                    sucess = true,
                                    total = dirCount,
                                    ReportBanksCashOffices = queryTemp
                                };
                                return await Task.Run(() => Ok(collectionWrapper1));
                            }

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 2)
                        {
                            #region Группировать по дням

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocBankSums
                                where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,

                                    DirEmployeeID = x.DirEmployeeID,

                                    DirCashOfficeID = 0,
                                    DirBankID = x.DirBankID,
                                }
                            );



                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 3)
                        {
                            #region Группировать по дням и точке

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocBankSums
                                where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,

                                    DirEmployeeID = x.DirEmployeeID,

                                    DirCashOfficeID = 0,
                                    DirBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                }
                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        //DirCashOfficeID = x.DirCashOfficeID,
                                        //DirBankID = x.DirBankID,
                                        DirCashOfficeBankName = x.DirCashOfficeBankName
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        //DirCashOfficeID = g.Key.DirCashOfficeID,
                                        //DirBankID = g.Key.DirBankID,
                                        DirCashOfficeBankName = g.Key.DirCashOfficeBankName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 4)
                        {
                            #region Группировать по дням и сотруднику

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocBankSums
                                where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,

                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,

                                    DirCashOfficeID = 0,
                                    DirBankID = x.DirBankID,
                                }
                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        //DirEmployeeID = x.DirEmployeeID,
                                        DirEmployeeName = x.DirEmployeeName,
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        //DirEmployeeID = g.Key.DirEmployeeID,
                                        DirEmployeeName = g.Key.DirEmployeeName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirEmployeeID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }
                        else if (ReportGroup == 5)
                        {
                            #region Группировать по дням, точке и сотруднику

                            #region queryTemp

                            var queryTemp_ =
                            (
                                from x in db.DocBankSums
                                where x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo

                                select new
                                {
                                    DocCashOfficeBankSumDate = x.DateOnly,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,

                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,

                                    DirCashOfficeID = 0,
                                    DirBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                }
                            );

                            if (DirWarehouseID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }

                            if (DirEmployeeID > 0)
                            {
                                queryTemp_ = queryTemp_.Where(x => x.DirEmployeeID == DirEmployeeID);
                            }


                            var queryTemp =
                                (
                                    from x in queryTemp_

                                    group x by new
                                    {
                                        DateOnly = x.DocCashOfficeBankSumDate.ToString(),
                                        DirEmployeeName = x.DirEmployeeName,
                                        DirCashOfficeBankName = x.DirCashOfficeBankName
                                    }
                                    into g
                                    select new
                                    {
                                        DocCashOfficeBankSumDate = g.Key.DateOnly,
                                        DirEmployeeName = g.Key.DirEmployeeName,
                                        DirCashOfficeBankName = g.Key.DirCashOfficeBankName,
                                        DocCashOfficeBankSumSum = g.Sum(t => t.DocCashOfficeBankSumSum)
                                    }

                                );



                            /*if (DirWarehouseID > 0)
                            {
                                queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID || x.DirBankID == DirBankID);
                            }*/

                            #endregion


                            #region Параметры

                            queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                            #endregion


                            #region Отправка JSON

                            int dirCount = queryTemp.Count();

                            dynamic collectionWrapper1 = new
                            {
                                sucess = true,
                                total = dirCount,
                                ReportBanksCashOffices = queryTemp
                            };
                            return await Task.Run(() => Ok(collectionWrapper1));

                            #endregion

                            #endregion
                        }

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 2)
                {
                    #region Продажи

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 6 || x.DirCashOfficeSumTypeID == 16)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 5 || x.DirBankSumTypeID == 15)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 6 || x.DirCashOfficeSumTypeID == 16)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 5 || x.DirBankSumTypeID == 15)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 7)
                {
                    #region Ремонты

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 14 || x.DirCashOfficeSumTypeID == 15)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 13 || x.DirBankSumTypeID == 14)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 14 || x.DirCashOfficeSumTypeID == 15)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 13 || x.DirBankSumTypeID == 14)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 8)
                {
                    #region Магаз.продажи + СЦ.Ремонты + БУ.продажи

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 6 || x.DirCashOfficeSumTypeID == 16 || x.DirCashOfficeSumTypeID == 14 || x.DirCashOfficeSumTypeID == 15 || x.DirCashOfficeSumTypeID == 23)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 13 || x.DirBankSumTypeID == 14 || x.DirBankSumTypeID == 5 || x.DirBankSumTypeID == 15 || x.DirBankSumTypeID == 21)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 6 || x.DirCashOfficeSumTypeID == 16 || x.DirCashOfficeSumTypeID == 14 || x.DirCashOfficeSumTypeID == 15 || x.DirCashOfficeSumTypeID == 23)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 13 || x.DirBankSumTypeID == 14 || x.DirBankSumTypeID == 5 || x.DirBankSumTypeID == 15 || x.DirBankSumTypeID == 21)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 3)
                {
                    #region Возвраты

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 8 || x.DirCashOfficeSumTypeID == 10 || x.DirCashOfficeSumTypeID == 18)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 7 || x.DirBankSumTypeID == 9 || x.DirBankSumTypeID == 17)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 8 || x.DirCashOfficeSumTypeID == 10 || x.DirCashOfficeSumTypeID == 18)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 7 || x.DirBankSumTypeID == 9 || x.DirBankSumTypeID == 17)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 4)
                {
                    #region Финансы - Внесения

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 1)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 1)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 1)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 1)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 5)
                {
                    #region Финансы - Выплаты

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 2)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 2)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 2)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 2)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 6)
                {
                    #region Z-отчет

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 3)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 3)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region Отправка JSON

                        int dirCount = 0;

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = "Отчет 'Z-отчет' доступен только для Кассы!"
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 9)
                {
                    #region Скидки

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.Discount > 0) //(x.doc.Discount > 0)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.Discount > 0) //(x.doc.Discount > 0)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.Discount > 0) //(x.doc.Discount > 0)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.Discount > 0) //(x.doc.Discount > 0)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 10)
                {
                    #region Расходы на покупку товара (накладные поступление)

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 4)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 3)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 4)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 3)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }
                else if (ReportType == 11)
                {
                    #region Хоз.расходы - Выплаты

                    if (CasheAndBank)
                    {
                        #region CasheAndBank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 27)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        queryTemp = queryTemp.Union
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 25)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );


                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID || x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Cashe)
                    {
                        #region Cashe

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocCashOfficeSums

                                where

                                x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                                (x.DirCashOfficeSumTypeID == 27)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirCashOfficeID,
                                    DirCashOfficeBankName = x.dirCashOffice.DirCashOfficeName,
                                    DirCashOfficeBankSumTypeID = x.DirCashOfficeSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                    DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirCashOfficeName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirCashOfficeID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }
                    else if (Bank)
                    {
                        #region Bank

                        #region queryTemp

                        var queryTemp =
                            (
                                from x in db.DocBankSums

                                where

                                x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                                (x.DirBankSumTypeID == 25)

                                select new
                                {
                                    Base = x.Base,
                                    DirEmployeeID = x.DirEmployeeID,
                                    DirEmployeeName = x.dirEmployee.DirEmployeeName,
                                    Description = x.Description,
                                    DirCashOfficeBankID = x.DirBankID,
                                    DirCashOfficeBankName = x.dirBank.DirBankName,
                                    DirCashOfficeBankSumTypeID = x.DirBankSumTypeID,
                                    DirCashOfficeBankSumTypeName = x.dirBankSumType.DirBankSumTypeName + x.DocXID,
                                    DirCurrencyID = x.DirCurrencyID,
                                    DirCurrencyName = x.dirCurrency.DirCurrencyName,
                                    DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                                    DirCurrencyRate = x.DirCurrencyRate,
                                    DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                    DocCashOfficeBankSumID = x.DocBankSumID,
                                    DocCashOfficeBankSumSum = x.DocBankSumSum,
                                    DocID = x.DocID,
                                    DocXID = x.DocXID,
                                    DirWarehouseName = DirBankName,
                                    Discount = x.Discount == null ? 0 : x.Discount, //Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                                    //KKMS
                                    KKMSCheckNumber = x.KKMSCheckNumber,
                                }
                            );

                        if (DirWarehouseID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirCashOfficeBankID == DirBankID);
                        }

                        if (DirEmployeeID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                        }

                        if (DocXID > 0)
                        {
                            queryTemp = queryTemp.Where(x => x.DocXID == DocXID);
                        }

                        #endregion

                        queryTemp = queryTemp.OrderByDescending(x => x.DocCashOfficeBankSumDate);

                        #region Отправка JSON

                        int dirCount = queryTemp.Count();

                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = dirCount,
                            ReportBanksCashOffices = queryTemp
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));

                        #endregion

                        #endregion
                    }

                    #endregion
                }



                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = 0,
                    ReportBanksCashOffices = ""
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion


            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion
    }
}
