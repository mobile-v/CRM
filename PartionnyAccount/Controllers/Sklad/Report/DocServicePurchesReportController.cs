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
    public class DocServicePurchesReportController : ApiController
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
        bool TypeRepair = false;
        int pLanguage = 0, DirContractorIDOrg = 0, DirWarehouseID = 0, DirServiceStatusID = 0, DirEmployeeID = 0, DirEmployeeIDMaster = 0, DirServiceContractorID = 0, ReportType = 0;
        string DirContractorNameOrg, DirWarehouseName, DirServiceStatusName, DirEmployeeName, DirEmployeeNameMaster, DirServiceContractorName, ReportTypeName;
        DateTime DateS, DatePo;

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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServicePurchesReport"));
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
                

                pID = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pID", true) == 0).Value;

                pLanguage = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pLanguage", true) == 0).Value);

                DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (DateS < Convert.ToDateTime("01.01.1800")) DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else DateS = DateS.AddDays(-1);

                DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (DatePo < Convert.ToDateTime("01.01.1800")) DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                TypeRepair = false;
                bool bTypeRepair = Boolean.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "TypeRepair", true) == 0).Value, out TypeRepair); //TypeRepair

                DirContractorIDOrg = 0;
                bool bDirContractorIDOrg = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorIDOrg", true) == 0).Value, out DirContractorIDOrg); //DirContractorIDOrg
                DirContractorNameOrg = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorNameOrg", true) == 0).Value; //Request.Params["DirContractorNameOrg"];

                DirWarehouseID = 0;
                bool bDirWarehouseID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value, out DirWarehouseID); //DirWarehouseID
                DirWarehouseName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseName", true) == 0).Value; //Request.Params["DirWarehouseName"];

                DirServiceStatusID = 0;
                bool bDirServiceStatusID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceStatusID", true) == 0).Value, out DirServiceStatusID); //DirServiceStatusID
                DirServiceStatusName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceStatusName", true) == 0).Value; //Request.Params["DirServiceStatusName"];

                DirEmployeeID = 0;
                bool bDirEmployeeID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value, out DirEmployeeID); //DirEmployeeID
                DirEmployeeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeName", true) == 0).Value; //Request.Params["DirEmployeeName"];

                DirEmployeeIDMaster = 0;
                bool bDirEmployeeIDMaster = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeIDMaster", true) == 0).Value, out DirEmployeeIDMaster); //DirEmployeeIDMaster
                DirEmployeeNameMaster = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeNameMaster", true) == 0).Value; //Request.Params["DirEmployeeNameMaster"];

                DirServiceContractorID = 0;
                bool bDirServiceContractorID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceContractorID", true) == 0).Value, out DirServiceContractorID); //DirServiceContractorID
                DirServiceContractorName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceContractorName", true) == 0).Value; // Request.Params["DirServiceContractorName"];

                ReportType = 0;
                bool bReportType = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportType", true) == 0).Value, out ReportType); //ReportType
                ReportTypeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportTypeName", true) == 0).Value; // Request.Params["ReportTypeName"];

                #endregion



                #region queryTemp

                var queryTemp =
                    (
                        #region from

                        from docServicePurches in db.DocServicePurches


                        join dirServiceNomens11 in db.DirServiceNomens on docServicePurches.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        /*
                        join docServicePurch1Tabs1 in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs1.DocServicePurchID into docServicePurch1Tabs2
                        from docServicePurch1Tabs in docServicePurch1Tabs2.DefaultIfEmpty()
                        //where docServicePurch1Tabs.PriceCurrency > 0
                        */

                        /*
                        join docServicePurch2Tabs1 in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs1.DocServicePurchID into docServicePurch2Tabs2
                        from docServicePurch2Tabs in docServicePurch2Tabs2.DefaultIfEmpty()
                        //where docServicePurch2Tabs.PriceCurrency > 0
                        */

                        #endregion

                        where docServicePurches.doc.DirContractorIDOrg == DirContractorIDOrg //&& docServicePurches.doc.DocDate >= DateS && docServicePurches.doc.DocDate <= DatePo

                        #region group

                        /*
                        group new { docServicePurch1Tabs, docServicePurch2Tabs }
                        //group new { docServicePurches }
                        by new
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

                            //Принял
                            DirEmployeeID = docServicePurches.doc.DirEmployeeID,
                            DirEmployeeName = docServicePurches.doc.dirEmployee.DirEmployeeName,
                            //Мастер
                            DirEmployeeIDMaster = docServicePurches.DirEmployeeIDMaster,
                            DirEmployeeNameMaster = docServicePurches.dirEmployee.DirEmployeeName,

                            DirServiceNomenID = docServicePurches.DirServiceNomenID,
                            //DirServiceNomenName = docServicePurches.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                                dirServiceNomensSubGroup.DirServiceNomenName == null ? docServicePurches.dirServiceNomen.DirServiceNomenName :
                                dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docServicePurches.dirServiceNomen.DirServiceNomenName :
                                dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docServicePurches.dirServiceNomen.DirServiceNomenName,

                            DocServicePurchID = docServicePurches.DocServicePurchID,
                            DirContractorName = docServicePurches.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docServicePurches.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docServicePurches.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docServicePurches.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = docServicePurches.dirWarehouse.DirWarehouseName,

                            DirServiceStatusID = docServicePurches.DirServiceStatusID,
                            DirServiceStatusID_789 = docServicePurches.DirServiceStatusID_789,
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

                            IssuanceDate = docServicePurches.IssuanceDate,
                            DateStatusChange = docServicePurches.DateStatusChange,

                            //Оплата
                            Payment = docServicePurches.doc.Payment,

                            //Сумма Выполненный работ + Запчастей
                            Sums = docServicePurches.Sums,
                            Sums1 = docServicePurches.Sums1,
                            Sums2 = docServicePurches.Sums2,
                        }
                        into g
                        */

                        #endregion

                        #region select docServicePurches

                        select new
                        {
                            DocID = docServicePurches.DocID,
                            DocDate = docServicePurches.doc.DocDate.ToString(), DocDate1 = docServicePurches.doc.DocDate,
                            Base = docServicePurches.doc.Base,
                            Held = docServicePurches.doc.Held,
                            Del = docServicePurches.doc.Del,
                            Description = docServicePurches.doc.Description,
                            IsImport = docServicePurches.doc.IsImport,
                            DirVatValue = docServicePurches.doc.DirVatValue,
                            //DirPaymentTypeID = docServicePurches.DirPaymentTypeID,
                            //DirPaymentTypeName = docServicePurches.DirPaymentTypeName,

                            //Принял
                            DirEmployeeID = docServicePurches.doc.DirEmployeeID,
                            DirEmployeeName = docServicePurches.doc.dirEmployee.DirEmployeeName,
                            //Мастер
                            DirEmployeeIDMaster = docServicePurches.DirEmployeeIDMaster,
                            DirEmployeeNameMaster = docServicePurches.dirEmployee.DirEmployeeName,

                            DirServiceNomenID = docServicePurches.DirServiceNomenID,
                            //DirServiceNomenName = docServicePurches.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                                dirServiceNomensSubGroup.DirServiceNomenName == null ? docServicePurches.dirServiceNomen.DirServiceNomenName :
                                dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docServicePurches.dirServiceNomen.DirServiceNomenName :
                                dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docServicePurches.dirServiceNomen.DirServiceNomenName,

                            DocServicePurchID = docServicePurches.DocServicePurchID,
                            DirContractorName = docServicePurches.doc.dirContractor.DirContractorName,
                            DirContractorIDOrg = docServicePurches.doc.dirContractorOrg.DirContractorID,
                            DirContractorNameOrg = docServicePurches.doc.dirContractorOrg.DirContractorName,
                            DirWarehouseID = docServicePurches.DirWarehouseID,
                            DirWarehouseName = docServicePurches.dirWarehouse.DirWarehouseName,

                            DirServiceStatusID = docServicePurches.DirServiceStatusID,
                            DirServiceStatusID_789 = docServicePurches.DirServiceStatusID_789,
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

                            DateDone = docServicePurches.DateDone.ToString(), DateDone1 = docServicePurches.DateDone,
                            UrgentRepairs = docServicePurches.UrgentRepairs,
                            Prepayment = docServicePurches.Prepayment,
                            PrepaymentSum = docServicePurches.PrepaymentSum == null ? 0 : docServicePurches.PrepaymentSum,

                            IssuanceDate = docServicePurches.IssuanceDate.ToString(), IssuanceDate1 = docServicePurches.IssuanceDate,
                            DateStatusChange = docServicePurches.DateStatusChange.ToString(), DateStatusChange1 = docServicePurches.DateStatusChange,

                            //Оплата
                            Payment = docServicePurches.doc.Payment,


                            /*
                            //1. Подсчет табличной части Работы "SumDocServicePurch1Tabs"
                            SumDocServicePurch1Tabs = g.Sum(x => x.docServicePurch1Tabs.PriceCurrency) == null ? 0 : Math.Round(g.Sum(x => x.docServicePurch1Tabs.PriceCurrency), sysSetting.FractionalPartInSum),

                            //2. Подсчет табличной части Работы "SumDocServicePurch2Tabs"
                            SumDocServicePurch2Tabs = g.Sum(x => x.docServicePurch2Tabs.PriceCurrency) == null ? 0 : Math.Round(g.Sum(x => x.docServicePurch2Tabs.PriceCurrency), sysSetting.FractionalPartInSum),


                            //3. Сумма 1+2 "SumTotal"
                            SumTotal =
                             Math.Round(
                            (g.Sum(x => x.docServicePurch1Tabs.PriceCurrency) == null ? 0 : g.Sum(x => x.docServicePurch1Tabs.PriceCurrency)) +
                            (g.Sum(x => x.docServicePurch2Tabs.PriceCurrency) == null ? 0 : g.Sum(x => x.docServicePurch2Tabs.PriceCurrency))
                            , sysSetting.FractionalPartInSum),

                            //4. Константа "PrepaymentSum"
                            //...

                            //5. 3 - 4 "SumTotal2"
                            SumTotal2 = g.Key.Sums,
                            */

                            DiscountX = docServicePurches.DiscountX,
                            SumDocServicePurch1Tabs = docServicePurches.Sums1,

                            DiscountY = docServicePurches.DiscountY,
                            SumDocServicePurch2Tabs = docServicePurches.Sums2,

                            SumTotal = 
                            (docServicePurches.Sums1 - docServicePurches.DiscountX)
                            + 
                            (docServicePurches.Sums2 - docServicePurches.DiscountY),

                            SumTotal2 = docServicePurches.Sums - docServicePurches.DiscountX - docServicePurches.DiscountY,

                        }

                        #endregion

                    );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirServiceStatusID > 0) queryTemp = queryTemp.Where(z => z.DirServiceStatusID == DirServiceStatusID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);
                if (DirEmployeeIDMaster > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeIDMaster == DirEmployeeIDMaster);
                if (DirServiceContractorID > 0) queryTemp = queryTemp.Where(z => z.DirServiceContractorID == DirServiceContractorID);
                if (ReportType > 0)
                {
                    switch (ReportType)
                    {
                        //1. Выданные (все)
                        case 1:
                            {
                                queryTemp = queryTemp.Where(z => z.DirServiceStatusID == 9 && (z.DateStatusChange1 >= DateS && z.DateStatusChange1 <= DatePo));
                            }
                            break;
                        //2. Выданные (готовые)
                        case 2:
                            {
                                queryTemp = queryTemp.Where(z => z.DirServiceStatusID == 9 && z.DirServiceStatusID_789 == 7 && (z.DateStatusChange1 >= DateS && z.DateStatusChange1 <= DatePo));
                            }
                            break;
                        //3. Не отремонтированные все (Выданные (отказные))
                        case 3:
                            {
                                queryTemp = queryTemp.Where(z => z.DirServiceStatusID_789 == 8 && (z.IssuanceDate1 >= DateS && z.IssuanceDate1 <= DatePo)); //queryTemp = queryTemp.Where(z => z.DirServiceStatusID == 9 && z.DirServiceStatusID_789 == 8 && (z.DocDate >= DateS && z.DocDate <= DatePo));
                            }
                            break;
                        //4. Сделанные
                        case 4:
                            {
                                queryTemp = queryTemp.Where(z => z.DirServiceStatusID_789 == 7 && (z.IssuanceDate1 >= DateS && z.IssuanceDate1 <= DatePo)); //queryTemp = queryTemp.Where(z => z.DirServiceStatusID == 7 && (z.DocDate >= DateS && z.DocDate <= DatePo));
                            }
                            break;
                        //5. Принятые
                        case 5:
                            {
                                queryTemp = queryTemp.Where(z => (z.DocDate1 >= DateS && z.DocDate1 <= DatePo)); //queryTemp = queryTemp.Where(z => z.DirServiceStatusID == 1 && (z.DocDate >= DateS && z.DocDate <= DatePo));
                            }
                            break;

                        default:
                            {
                                queryTemp = queryTemp.Where(z => z.DirServiceStatusID == 9 && (z.DateStatusChange1 >= DateS && z.DateStatusChange1 <= DatePo));
                            }
                            break;
                    }
                }
                else
                {
                    queryTemp = queryTemp.Where(z => z.DirServiceStatusID == 9 && (z.DateStatusChange1 >= DateS && z.DateStatusChange1 <= DatePo));
                }

                #endregion


                #region Отправка JSON

                int dirCount = queryTemp.Count();

                dynamic collectionWrapper1 = new
                {
                    sucess = true,
                    total = dirCount,
                    DocServicePurchesReport = queryTemp
                };
                return await Task.Run(() => Ok(collectionWrapper1));

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
