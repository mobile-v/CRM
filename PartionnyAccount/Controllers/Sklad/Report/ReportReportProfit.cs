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
    public class ReportReportProfit
    {
        string pID = "";
        bool ProfitNomenAll = false;
        int pLanguage = 0, DirContractorIDOrg = 0, DirWarehouseID = 0, DirEmployeeID = 0;
        string DirContractorNameOrg, DirWarehouseName, DirEmployeeName;
        DateTime DateS, DatePo;

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db)
        {
            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);
            DateS = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DateS"]).ToString("yyyy-MM-dd 00:00:00"));
            DatePo = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DatePo"]).ToString("yyyy-MM-dd 23:59:59"));

            ProfitNomenAll = false;
            bool bProfitNomenAll = Boolean.TryParse(Request.Params["ProfitNomenAll"], out ProfitNomenAll);

            DirContractorIDOrg = 0;
            bool bDirContractorIDOrg = Int32.TryParse(Request.Params["DirContractorIDOrg"], out DirContractorIDOrg);
            DirContractorNameOrg = Request.Params["DirContractorNameOrg"];

            DirWarehouseID = 0;
            bool bDirWarehouseID = Int32.TryParse(Request.Params["DirWarehouseID"], out DirWarehouseID);
            DirWarehouseName = Request.Params["DirWarehouseName"];

            DirEmployeeID = 0;
            bool bDirEmployeeID = Int32.TryParse(Request.Params["DirEmployeeID"], out DirEmployeeID);
            DirEmployeeName = Request.Params["DirEmployeeName"];

            #endregion

            string ret =
                "<center>" +
                "<h2>Прибыль за период с " + DateS.ToString("yyyy-MM-dd") + " по " + DatePo.ToString("yyyy-MM-dd") + "</h2>";

            //Алгоритм:
            //Простая выборку с ПартииМинус и Партии
            //ПартииМинус - продажная цена
            //Партии - приходная цена
            //Разница - прибыль

            if (!ProfitNomenAll)
            {
                #region Прибыль одной суммой

                var query =
                (
                    from x in db.RemPartyMinuses
                    from y in db.RemParties

                    where x.RemPartyID == y.RemPartyID && x.DirContractorIDOrg == DirContractorIDOrg && x.doc.Held == true && x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo
                    select new
                    {
                        DirWarehouseID = x.DirWarehouseID,
                        DirEmployeeID = x.doc.DirEmployeeID,

                        Sale_Discount = x.doc.Discount,
                        Purch_PriceCurrency = y.PriceCurrency,
                        Sale_Quantity = x.Quantity,
                        Sale_PriceCurrency = x.PriceCurrency,
                    }
                );

                if (DirWarehouseID > 0) query = query.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) query = query.Where(z => z.DirEmployeeID == DirEmployeeID);

                var query2 = query.Sum(z => (double?)((1 - z.Sale_Discount / 100) * z.Sale_Quantity * (z.Sale_PriceCurrency - z.Purch_PriceCurrency))) ?? 0;

                ret +=
                    "по юр.лицу: " + DirContractorNameOrg + "<br />";
                if (DirWarehouseID > 0) ret += "по складу: " + DirWarehouseName + "<br />";
                if (DirEmployeeID > 0) ret += "по сотруднику: " + DirEmployeeName + "<br />";

                ret += "</center>" +

                    "Ваша прибыль =  " + query2;

                return ret;

                #endregion
            }
            else
            {
                #region Прибыль по каждой проданной позиции товара

                var queryTemp =
                    (
                        from x in db.RemPartyMinuses
                        from y in db.RemParties
                        where x.RemPartyID == y.RemPartyID && x.DirContractorIDOrg == DirContractorIDOrg && x.doc.Held == true && x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo

                        select new
                        {
                            DirWarehouseID = x.DirWarehouseID,
                            DirEmployeeID = x.doc.DirEmployeeID,

                            DocDate = x.doc.DocDate,

                            DirNomenID = x.DirNomenID,
                            DirNomenName = x.dirNomen.DirNomenName,

                            Sale_Discount = x.doc.Discount,
                            Purch_PriceCurrency = y.PriceCurrency,
                            Sale_Quantity = x.Quantity,
                            Sale_PriceCurrency = x.PriceCurrency,

                            SumProfit = (1 - x.doc.Discount / 100) * x.Quantity * (x.PriceCurrency - y.PriceCurrency),

                        }

                    );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);


                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());

                ret += ReportHeader(pLanguage) + "</center>";

                double dSum = 0;
                for (int i = 0; i < query.Count(); i++)
                {
                    if (query[i].SumProfit > 0)
                    {
                        dSum += query[i].SumProfit;

                        ret +=
                            "<TR>" +
                            //1. Дата продажи
                            "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                            //2. Код товара
                            "<TD>" + query[i].DirNomenID + "</TD> " +
                            //3. Наименование товара
                            "<TD>" + query[i].DirNomenName + "</TD> " +
                            //4. К-во продано
                            "<TD>" + query[i].Sale_Quantity + "</TD> " +
                            //5. Цена продано
                            "<TD>" + query[i].Sale_PriceCurrency + "</TD> " +
                            //6. Цена прихода
                            "<TD>" + query[i].Purch_PriceCurrency + "</TD> " +
                            //7. Цена прибыли
                            "<TD>" + query[i].SumProfit + "</TD> " +
                            "</TR>";
                    }
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
                    "<TD> </TD> " +
                    //6.
                    "<TD> </TD> " +
                    //7.
                    "<TD> </TD> " +
                    "</TR>" +

                    "<TR>" +
                    //1.
                    "<TD> <b>Итого</b> </TD> " +
                    //2.
                    "<TD> </TD> " +
                    //3.
                    "<TD> </TD> " +
                    //4.
                    "<TD> </TD> " +
                    //5.
                    "<TD> </TD> " +
                    //6.
                    "<TD> </TD> " +
                    //7.
                    "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                    "</TR>";
                    
                ret += "</TABLE><br />";


                return ret;


                #endregion
            }

        }

        //СТАНДАРТНАЯ табличная часть
        private string ReportHeader(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата продажу</TD> <TD class='table_header'>Код</TD> <TD class='table_header'>Найменування</TD> <TD class='table_header'>К-ть продано</TD> <TD class='table_header'>Ціна продано</TD> <TD class='table_header'>Ціна прихід</TD> <TD class='table_header'>Сума прибутку</TD>" +
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата продажи</TD> <TD class='table_header'>Код</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>К-во продано</TD> <TD class='table_header'>Цена продано</TD> <TD class='table_header'>Цена приход</TD> <TD class='table_header'>Сумма прибыли</TD>" +
                    "</TR>";
            }
        }

    }
}