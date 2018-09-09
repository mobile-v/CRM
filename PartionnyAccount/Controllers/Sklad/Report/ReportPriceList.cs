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
    public class ReportPriceList
    {
        #region Classes

        Models.DbConnectionLogin db = new Models.DbConnectionLogin("ConnStrMSSQL");
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Function.FunctionMSSQL.Jurn.JurnDispError jurnDispError = new Classes.Function.FunctionMSSQL.Jurn.JurnDispError();
        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();

        #endregion

        string pID = "";
        bool PriceGreater0 = false;
        int pLanguage = 0, DirPriceTypeID = 0, DirNomenID = 0;

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db, int DirCustomersID)
        {
            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);

            PriceGreater0 = false;
            bool bPriceGreater0 = Boolean.TryParse(Request.Params["PriceGreater0"], out PriceGreater0);

            DirPriceTypeID = 0;
            bool bDirPriceTypeID = Int32.TryParse(Request.Params["DirPriceTypeID"], out DirPriceTypeID);

            DirNomenID = 0;
            bool bDirNomenID = Int32.TryParse(Request.Params["DirNomenID"], out DirNomenID);

            #endregion


            #region Формируем отчет (ViewData["ReportHtml"])

            string ret = "<center>" + ReportHeader(pLanguage) + "</center>";

            string SQL =

                "SELECT " +
                " n.DirNomenID, n.DirNomenArticle, n.DirNomenName, " +

                " h.PriceRetailVAT, " + 
                " h.PriceWholesaleVAT, " + 
                " h.PriceIMVAT, " +

                " h.PriceRetailVAT*c.DirCurrencyRate/c.DirCurrencyMultiplicity AS PriceRetailCurrency, " +
                " h.PriceWholesaleVAT*c.DirCurrencyRate/c.DirCurrencyMultiplicity AS PriceWholesaleCurrency, " +
                " h.PriceIMVAT*c.DirCurrencyRate/c.DirCurrencyMultiplicity AS PriceIMCurrency " +

                "FROM DirNomens AS n " +

                "INNER JOIN DirNomenHistories AS h ON " +
                " ( " +
                "  h.DirNomenHistoryID in  " +
                "   ( " +
                "    SELECT DirNomenHistoryID " +
                "    FROM DirNomenHistories INDEXED BY IDX_DirNomenHistories_ID_Date " +
                "    WHERE " +
                "     (DirNomenID = n.DirNomenID)and " +
                "     ( " +
                "       HistoryDate in  " +
                "        (" +
                "         SELECT Max(HistoryDate) From DirNomenHistories  INDEXED BY IDX_DirNomenHistories_ID_Date WHERE (DirNomenID = n.DirNomenID) " +  //(DirNomenHistoriesDate <= @DirNomenHistoriesDate)and
                "        ) " +
                "     ) " +
                "   ) " +
                " ) " +

                "INNER JOIN DirCurrencies AS c ON c.DirCurrencyID = h.DirCurrencyID ";


            string ConStr = connectionString.Return(DirCustomersID, null, true);
            using (SQLiteConnection con = new SQLiteConnection(ConStr))
            {
                con.Open();

                //Если выбрана группа товара
                if (DirNomenID > 0)
                {
                    SQL += "WHERE ";

                    ArrayList alGrouID = function.SelectAllSubGroups(con, "DirNomens", "DirNomen", DirNomenID);
                    for (int i = 0; i < alGrouID.Count; i++)
                    {
                        Classes.Function.Function.AllSubGroups _allSubGroups = (Classes.Function.Function.AllSubGroups)alGrouID[i];
                        //_allSubGroups.ID

                        SQL += "(n.DirNomenID=" + _allSubGroups.ID + ")";
                        if (i != alGrouID.Count - 1) SQL += "or";
                    }
                }

                using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
                {
                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        bool bRecord = true;
                        while (dr.Read())
                        {
                            //Пишем только если цены больше неля
                            if (PriceGreater0)
                            {
                                if (DirPriceTypeID == 1 && Convert.ToDouble(dr["PriceRetailCurrency"].ToString()) <= 0) bRecord = false;
                                if (DirPriceTypeID == 2 && Convert.ToDouble(dr["PriceWholesaleCurrency"].ToString()) <= 0) bRecord = false;
                                if (DirPriceTypeID == 3 && Convert.ToDouble(dr["PriceIMCurrency"].ToString()) <= 0) bRecord = false;
                            }

                            if (bRecord)
                            {
                                ret +=
                                "<TR>" +
                                //Товар
                                "<TD>" + dr["DirNomenID"].ToString() + "</TD> " +
                                "<TD>" + dr["DirNomenArticle"].ToString() + "</TD> " +
                                "<TD>" + dr["DirNomenName"].ToString() + "</TD> ";

                                //Цены
                                if (DirPriceTypeID == 1) ret += "<TD>" + dr["PriceRetailCurrency"].ToString() + "</TD> ";
                                if (DirPriceTypeID == 2) ret += "<TD>" + dr["PriceWholesaleCurrency"].ToString() + "</TD> ";
                                if (DirPriceTypeID == 3) ret += "<TD>" + dr["PriceIMCurrency"].ToString() + "</TD> ";

                                ret += "</TR>";
                            }
                        }
                    }
                }

                con.Close(); con.Dispose();
            }

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
                    Classes.Language.Sklad.Language.msg115(pLanguage) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Артикул</TD> <TD class='table_header'>Найменування</TD> <TD class='table_header'>Ціна</TD>" +
                    "</TR>";

                default:
                    return
                    Classes.Language.Sklad.Language.msg115(pLanguage) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Артикул</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Цена</TD>" +
                    "</TR>";
            }
        }

    }
}