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
    public class DocServicePurchesReport
    {
        string pID = "";
        bool TypeRepair = false;
        int pLanguage = 0, DirContractorIDOrg = 0, DirWarehouseID = 0, DirServiceStatusID = 0, DirEmployeeID = 0, DirEmployeeIDMaster = 0, DirServiceContractorID = 0, ReportType = 0;
        string DirContractorNameOrg, DirWarehouseName, DirServiceStatusName, DirEmployeeName, DirEmployeeNameMaster, DirServiceContractorName, ReportTypeName;
        DateTime DateS, DatePo;

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db)
        {
            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);
            DateS = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DateS"]).ToString("yyyy-MM-dd 00:00:00"));
            DatePo = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DatePo"]).ToString("yyyy-MM-dd 23:59:59"));

            TypeRepair = false;
            bool bTypeRepair = Boolean.TryParse(Request.Params["TypeRepair"], out TypeRepair);

            DirContractorIDOrg = 0;
            bool bDirContractorIDOrg = Int32.TryParse(Request.Params["DirContractorIDOrg"], out DirContractorIDOrg);
            DirContractorNameOrg = Request.Params["DirContractorNameOrg"];

            DirWarehouseID = 0;
            bool bDirWarehouseID = Int32.TryParse(Request.Params["DirWarehouseID"], out DirWarehouseID);
            DirWarehouseName = Request.Params["DirWarehouseName"];

            DirServiceStatusID = 0;
            bool bDirServiceStatusID = Int32.TryParse(Request.Params["DirServiceStatusID"], out DirServiceStatusID);
            DirServiceStatusName = Request.Params["DirServiceStatusName"];

            DirEmployeeID = 0;
            bool bDirEmployeeID = Int32.TryParse(Request.Params["DirEmployeeID"], out DirEmployeeID);
            DirEmployeeName = Request.Params["DirEmployeeName"];

            DirEmployeeIDMaster = 0;
            bool bDirEmployeeIDMaster = Int32.TryParse(Request.Params["DirEmployeeIDMaster"], out DirEmployeeIDMaster);
            DirEmployeeNameMaster = Request.Params["DirEmployeeNameMaster"];

            DirServiceContractorID = 0;
            bool bDirServiceContractorID = Int32.TryParse(Request.Params["DirServiceContractorID"], out DirServiceContractorID);
            DirServiceContractorName = Request.Params["DirServiceContractorName"];

            ReportType = 0;
            bool bReportType = Int32.TryParse(Request.Params["ReportType"], out ReportType);
            ReportTypeName = Request.Params["ReportTypeName"];

            #endregion


            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);


            string ret = "";


            #region queryTemp

            var queryTemp =
                (
                    #region from

                    from docServicePurches in db.DocServicePurches


                    join dirServiceNomens11 in db.DirServiceNomens on docServicePurches.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                    from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                    join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                    from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()


                    join docServicePurch1Tabs1 in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs1.DocServicePurchID into docServicePurch1Tabs2
                    from docServicePurch1Tabs in docServicePurch1Tabs2.DefaultIfEmpty()

                    join docServicePurch2Tabs1 in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs1.DocServicePurchID into docServicePurch2Tabs2
                    from docServicePurch2Tabs in docServicePurch2Tabs2.DefaultIfEmpty()

                    #endregion

                    where docServicePurches.doc.DirContractorIDOrg == DirContractorIDOrg //&& docServicePurches.doc.DocDate >= DateS && docServicePurches.doc.DocDate <= DatePo

                    #region group

                    group new { docServicePurch1Tabs, docServicePurch2Tabs }

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
                    }
                    into g

                        #endregion

                    #region select

                    select new
                    {
                        DocID = g.Key.DocID,
                        DocDate = g.Key.DocDate.ToString(),
                        DocDate1 = g.Key.DocDate,
                        Base = g.Key.Base,
                        Held = g.Key.Held,
                        Del = g.Key.Del,
                        Description = g.Key.Description,
                        IsImport = g.Key.IsImport,
                        DirVatValue = g.Key.DirVatValue,
                        //DirPaymentTypeID = g.Key.DirPaymentTypeID,
                        //DirPaymentTypeName = g.Key.DirPaymentTypeName,

                        //Принял
                        DirEmployeeID = g.Key.DirEmployeeID,
                        DirEmployeeName = g.Key.DirEmployeeName,
                        //Мастер
                        DirEmployeeIDMaster = g.Key.DirEmployeeIDMaster,
                        DirEmployeeNameMaster = g.Key.DirEmployeeNameMaster,

                        DirServiceNomenID = g.Key.DirServiceNomenID,
                        DirServiceNomenName = g.Key.DirServiceNomenName,

                        DocServicePurchID = g.Key.DocServicePurchID,
                        DirContractorName = g.Key.DirContractorName,
                        DirContractorIDOrg = g.Key.DirContractorIDOrg,
                        DirContractorNameOrg = g.Key.DirContractorNameOrg,
                        DirWarehouseID = g.Key.DirWarehouseID,
                        DirWarehouseName = g.Key.DirWarehouseName,

                        DirServiceStatusID = g.Key.DirServiceStatusID,
                        DirServiceStatusID_789 = g.Key.DirServiceStatusID_789,
                        SerialNumber = g.Key.SerialNumber,
                        TypeRepair = g.Key.TypeRepair,
                        ComponentDevice = g.Key.ComponentDevice,

                        ComponentBattery = g.Key.ComponentBattery,
                        ComponentBatterySerial = g.Key.ComponentBatterySerial,
                        ComponentBackCover = g.Key.ComponentBackCover,
                        ComponentPasTextNo = g.Key.ComponentPasTextNo,
                        ComponentPasText = g.Key.ComponentPasText,
                        ComponentOtherText = g.Key.ComponentOtherText,
                        ProblemClientWords = g.Key.ProblemClientWords,
                        Note = g.Key.Note,
                        DirServiceContractorName = g.Key.DirServiceContractorName,
                        DirServiceContractorRegular = g.Key.DirServiceContractorRegular,
                        DirServiceContractorID = g.Key.DirServiceContractorID,
                        DirServiceContractorAddress = g.Key.DirServiceContractorAddress,
                        DirServiceContractorPhone = g.Key.DirServiceContractorPhone,
                        DirServiceContractorEmail = g.Key.DirServiceContractorEmail,

                        PriceVAT = g.Key.PriceVAT,
                        //PriceCurrency = g.Key.PriceCurrency,

                        DirCurrencyID = g.Key.DirCurrencyID,
                        DirCurrencyRate = g.Key.DirCurrencyRate,
                        DirCurrencyMultiplicity = g.Key.DirCurrencyMultiplicity,
                        DirCurrencyName = g.Key.DirCurrencyName + " (" + g.Key.DirCurrencyRate + ", " + g.Key.DirCurrencyMultiplicity + ")",

                        DateDone = g.Key.DateDone.ToString(),
                        DateDone1 = g.Key.DateDone,
                        UrgentRepairs = g.Key.UrgentRepairs,
                        Prepayment = g.Key.Prepayment,
                        PrepaymentSum = g.Key.PrepaymentSum == null ? 0 : g.Key.PrepaymentSum,

                        IssuanceDate = g.Key.IssuanceDate.ToString(),
                        IssuanceDate1 = g.Key.IssuanceDate,
                        DateStatusChange = g.Key.DateStatusChange.ToString(),
                        DateStatusChange1 = g.Key.DateStatusChange,

                        //Оплата
                        Payment = g.Key.Payment,


                        //1. Подсчет табличной части Работы "SumDocServicePurch1Tabs"
                        SumDocServicePurch1Tabs =
                        g.Sum(x => x.docServicePurch1Tabs.PriceCurrency) == null ? 0 : Math.Round(g.Sum(x => x.docServicePurch1Tabs.PriceCurrency), sysSetting.FractionalPartInSum),

                        //2. Подсчет табличной части Работы "SumDocServicePurch2Tabs"
                        SumDocServicePurch2Tabs =
                        g.Sum(x => x.docServicePurch2Tabs.PriceCurrency) == null ? 0 : Math.Round(g.Sum(x => x.docServicePurch2Tabs.PriceCurrency), sysSetting.FractionalPartInSum),

                        //3. Сумма 1+2 "SumTotal"
                        SumTotal =
                         Math.Round(
                        (g.Sum(x => x.docServicePurch1Tabs.PriceCurrency) == null ? 0 : g.Sum(x => x.docServicePurch1Tabs.PriceCurrency)) +
                        (g.Sum(x => x.docServicePurch2Tabs.PriceCurrency) == null ? 0 : g.Sum(x => x.docServicePurch2Tabs.PriceCurrency))
                        , sysSetting.FractionalPartInSum),

                        //4. Константа "PrepaymentSum"
                        //...

                        //5. 3 - 4 "SumTotal2"
                        SumTotal2 =
                         Math.Round(
                        (g.Sum(x => x.docServicePurch1Tabs.PriceCurrency) == null ? 0 : g.Sum(x => x.docServicePurch1Tabs.PriceCurrency)) +
                        (g.Sum(x => x.docServicePurch2Tabs.PriceCurrency) == null ? 0 : g.Sum(x => x.docServicePurch2Tabs.PriceCurrency)) -
                        g.Key.PrepaymentSum
                        , sysSetting.FractionalPartInSum),
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
                            queryTemp = queryTemp.Where(z => z.DirServiceStatusID == 9 && z.DirServiceStatusID_789 == 7 && (z.IssuanceDate1 >= DateS && z.DateStatusChange1 <= DatePo));
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


            #region HTML

            //Получение списка
            var query = await Task.Run(() => queryTemp.ToListAsync());


            double? dSum1 = 0, dSum2 = 0, SumTotal2 = 0;
            int Count_DocDate = 0, Count_IssuanceDate = 0, Count_DateStatusChange = 0;
            string DocDate = "", IssuanceDate = "", DateStatusChange = "";
            for (int i = 0; i < query.Count(); i++)
            {
                dSum1 += query[i].SumDocServicePurch1Tabs;
                dSum2 += query[i].SumDocServicePurch2Tabs;
                SumTotal2 += query[i].SumTotal2;

                if (!String.IsNullOrEmpty(query[i].DocDate.ToString())) DocDate = Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd");
                if (!String.IsNullOrEmpty(query[i].IssuanceDate.ToString())) IssuanceDate = Convert.ToDateTime(query[i].IssuanceDate).ToString("yyyy-MM-dd");
                if (!String.IsNullOrEmpty(query[i].DateStatusChange.ToString())) DateStatusChange = Convert.ToDateTime(query[i].DateStatusChange).ToString("yyyy-MM-dd");

                if (query[i].DocDate.ToString() != "") Count_DocDate++;
                if (query[i].IssuanceDate.ToString() != "") Count_IssuanceDate++;
                if (query[i].DateStatusChange.ToString() != "") Count_DateStatusChange++;

                ret +=
                    "<TR>" +
                    //1. Квитанция
                    "<TD>" + query[i].DocServicePurchID + "</TD> " +
                    //2. Аппарат
                    "<TD>" + query[i].DirServiceNomenName + "</TD> " +
                    //3. Сер.номер
                    "<TD>" + query[i].SerialNumber + "</TD> " +
                    //4. Клиент
                    "<TD>" + query[i].DirServiceContractorName + "</TD> " +

                    //5.1. Дата приёмки
                    "<TD>" + DocDate + "</TD> " +
                    //5.2. IssuanceDate
                    "<TD>" + IssuanceDate + "</TD> " +
                    //5.3. DateStatusChange
                    "<TD>" + DateStatusChange + "</TD> " +

                    //6.1. Принял
                    "<TD>" + query[i].DirEmployeeName + "</TD> " +
                    //6.2. Мастер
                    "<TD>" + query[i].DirEmployeeNameMaster + "</TD> " +

                    //7. Предоплата
                    "<TD>" + query[i].PrepaymentSum + "</TD> " +

                    //8. Работа
                    "<TD>" + query[i].SumDocServicePurch1Tabs + "</TD> " +
                    //9. Запчасти
                    "<TD>" + query[i].SumDocServicePurch2Tabs + "</TD> " +
                    //10. Работа + Запчасти
                    "<TD>" + query[i].SumTotal + "</TD> " +
                    //11. Итого
                    "<TD>" + query[i].SumTotal2 + "</TD> " +

                    "</TR>";
            }

            ret +=
                "<TR>" +
                //1.
                "<TD> </TD> " +
                //2.
                "<TD> </TD> " +
                //3.
                "<TD> </TD> " +
                //4.
                "<TD> </TD> " +
                //5.
                "<TD>" + Count_DocDate + "</TD> " +
                "<TD>" + Count_IssuanceDate + "</TD> " +
                "<TD>" + Count_DateStatusChange + "</TD> " +
                //6.1.
                "<TD> </TD> " +
                //6.2.
                "<TD> </TD> " +
                //7.
                "<TD> </TD> " +
                //8.
                "<TD> "+ dSum1 + " </TD> " +
                //9.
                "<TD> "+ dSum2 + " </TD> " +
                //10.
                "<TD> </TD> " +
                //11.
                "<TD> "+ SumTotal2 + " </TD> " +
                "</TR>";

            ret += "</TABLE><br />";

            ret = ReportHeader(pLanguage, Count_DocDate, Count_IssuanceDate, Count_DateStatusChange) + "</center>" + ret;

            #endregion


            return ret;
        }


        //СТАНДАРТНАЯ табличная часть
        private string ReportHeader(int pLanguage, int Count_DocDate, int Count_IssuanceDate, int Count_DateStatusChange)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Квитанція</TD> <TD class='table_header'>Апарат</TD> <TD class='table_header'>Сер.номер</TD> <TD class='table_header'>Клієнт</TD> <TD class='table_header'>Надходження (<b style='color:red'>"+ Count_DocDate + "</b>)</TD> <TD class='table_header'>Готов/Отказ (<b style='color:red'>" + Count_IssuanceDate + "</b>)</TD> <TD class='table_header'>Видача (<b style='color:red'>" + Count_DateStatusChange + "</b>)</TD> <TD class='table_header'>Прийняв</TD> <TD class='table_header'>Майстер</TD> <TD class='table_header'>Передоплата</TD> <TD class='table_header'>Робота</TD> <TD class='table_header'>Запчастини</TD> <TD class='table_header'>Робота + Запчастини</TD> <TD class='table_header'>Разом</TD>" +
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Квитанция</TD> <TD class='table_header'>Аппарат</TD> <TD class='table_header'>Сер.номер</TD> <TD class='table_header'>Клиент</TD> <TD class='table_header'>Приёмка (<b style='color:red'>" + Count_DocDate + "</b>)</TD> <TD class='table_header'>Готов/Отказ (<b style='color:red'>" + Count_IssuanceDate + "</b>)</TD> <TD class='table_header'>Видача (<b style='color:red'>" + Count_DateStatusChange + "</b>)</TD> <TD class='table_header'>Принял</TD> <TD class='table_header'>Мастер</TD> <TD class='table_header'>Предоплата</TD> <TD class='table_header'>Работа</TD> <TD class='table_header'>Запчасти</TD> <TD class='table_header'>Работа + Запчасти</TD> <TD class='table_header'>Итого</TD>" +
                    "</TR>";
            }
        }

    }
}