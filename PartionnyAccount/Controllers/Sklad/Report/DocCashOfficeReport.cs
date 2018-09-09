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

namespace PartionnyAccount.Controllers.Sklad.Report
{
    public class DocCashOfficeReport
    {
        string pID = "";
        int pLanguage = 0, DirCashOfficeID = 0, DirCashOfficeSumTypeID = 0;
        DateTime DateS, DatePo;

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db)
        {
            #region Параметры

            pID = Request.Params["pID"];
            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);
            DateS = Convert.ToDateTime(Request.Params["DateS"]);
            DatePo = Convert.ToDateTime(Request.Params["DatePo"]);

            DirCashOfficeID = 0;
            bool bDirCashOfficeID = Int32.TryParse(Request.Params["DirCashOfficeID"], out DirCashOfficeID);

            DirCashOfficeSumTypeID = 0;
            bool bDirCashOfficeSumTypeID = Int32.TryParse(Request.Params["DirCashOfficeSumTypeID"], out DirCashOfficeSumTypeID);

            #endregion

            string ret = "<center>" + ReportHeader(pLanguage) + "</center>";

            #region Формируем отчет (ViewData["ReportHtml"])

            var queryTemp = 
                (
                    from x in db.DocCashOfficeSums

                    join dirEmployeesMoney1 in db.DirEmployees on x.DirEmployeeIDMoney equals dirEmployeesMoney1.DirEmployeeID into dirEmployeesMoney2
                    from dirEmployeesMoney in dirEmployeesMoney2.DefaultIfEmpty()

                    where x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo
                    select new
                    {
                        Base = x.Base,
                        DirEmployeeID = x.DirEmployeeID,
                        DirEmployeeName = x.dirEmployee.DirEmployeeName,
                        Description = x.Description,
                        DirCashOfficeID = x.DirCashOfficeID,
                        DirCashOfficeName = x.dirCashOffice.DirCashOfficeName,
                        DirCashOfficeSumTypeID = x.DirCashOfficeSumTypeID,
                        DirCashOfficeSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName,
                        DirCurrencyID = x.DirCurrencyID,
                        DirCurrencyName = x.dirCurrency.DirCurrencyName,
                        DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                        DirCurrencyRate = x.DirCurrencyRate,
                        DirEmployeeIDMoney = x.DirEmployeeIDMoney,
                        DirEmployeeNameMoney = dirEmployeesMoney.DirEmployeeName,
                        DocCashOfficeSumDate = x.DocCashOfficeSumDate,
                        DocCashOfficeSumID = x.DocCashOfficeSumID,
                        DocCashOfficeSumSum = x.DocCashOfficeSumSum,
                        DocID = x.DocID,
                        DocXID = x.DocXID,
                    }
                );

            //Касса
            if (DirCashOfficeID > 0)
            {
                queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID);
            }

            //Тип кассовой операции
            if (DirCashOfficeSumTypeID > 0)
            {
                queryTemp = queryTemp.Where(x => x.DirCashOfficeSumTypeID == DirCashOfficeSumTypeID);
            }

            //Получение списка
            var query = await Task.Run(() => queryTemp.ToListAsync());

            //Получение данных
            double dSum = 0;
            for (int i = 0; i < query.Count(); i++)
            {
                dSum += query[i].DocCashOfficeSumSum;

                ret +=
                    "<TR>" +
                    //1. Дата + Время
                    "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                    //2. Тип операции
                    "<TD>" + query[i].DirCashOfficeSumTypeName + "</TD> " +
                    //3. Сумма + Валюта
                    "<TD>" + query[i].DocCashOfficeSumSum.ToString() + "</TD> " +
                    "<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                    //4. От кого (кому)
                    "<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                    //5. Примечание
                    "<TD>" + query[i].Base + "</TD> " +
                    //6. Касса
                    "<TD>" + query[i].DirCashOfficeName + "</TD> " +
                    //7. Оператор, который создал запись
                    "<TD>" + query[i].DirEmployeeName + "</TD> " +
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
                "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                "<TD> </TD> " +
                //4.
                "<TD> </TD> " +
                //5.
                "<TD> </TD> " +
                //6.
                "<TD> </TD> " +
                //7.
                "<TD> </TD> " +
                "</TR>";

            ret += "</TABLE><br />";

            #endregion

            return ret;
        }

        //СТАНДАРТНАЯ табличная часть
        private string ReportHeader(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) + 
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата</TD> <TD class='table_header'>Операція</TD> <TD class='table_header'>Сума</TD> <TD class='table_header'>Валюта</TD> <TD class='table_header'>Від кого (кому)</TD> <TD class='table_header'>Примітка</TD> <TD class='table_header'>Каса</TD> <TD class='table_header'>Оператор</TD>" +
                    "</TR>";

                default:
                    return
                    Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата</TD> <TD class='table_header'>Операция</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Валюта</TD> <TD class='table_header'>От кого (кому)</TD> <TD class='table_header'>Примечание</TD> <TD class='table_header'>Касса</TD> <TD class='table_header'>Оператор</TD>" +
                    "</TR>";
            }
        }
    }
}