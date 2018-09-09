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
    public class ReportBanksCashOffices
    {
        string pID = "";
        bool ProfitNomenAll = false;
        int pLanguage = 0, CasheBank = 0, DirWarehouseID = 0, DirEmployeeID = 0, ReportType = 0;
        bool CasheAndBank = false, Cashe = false, Bank = false;
        string DirWarehouseName, DirEmployeeName, ReportTypeName;
        DateTime DateS, DatePo;
        ArrayList alDirNomenPatchFull = new ArrayList();

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db)
        {
            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);

            CasheAndBank = Convert.ToBoolean(Request.Params["CasheAndBank"]);
            Cashe = Convert.ToBoolean(Request.Params["Cashe"]);
            Bank = Convert.ToBoolean(Request.Params["Bank"]);

            DateS = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DateS"]).ToString("yyyy-MM-dd 00:00:00"));
            DatePo = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DatePo"]).ToString("yyyy-MM-dd 23:59:59"));

            ProfitNomenAll = false;
            bool bProfitNomenAll = Boolean.TryParse(Request.Params["ProfitNomenAll"], out ProfitNomenAll);
            
            DirWarehouseID = 0;
            bool bDirWarehouseID = Int32.TryParse(Request.Params["DirWarehouseID"], out DirWarehouseID);
            DirWarehouseName = Request.Params["DirWarehouseName"];

            DirEmployeeID = 0;
            bool bDirEmployeeID = Int32.TryParse(Request.Params["DirEmployeeID"], out DirEmployeeID);
            DirEmployeeName = Request.Params["DirEmployeeName"];

            ReportType = 0;
            bool bReportType = Int32.TryParse(Request.Params["ReportType"], out ReportType);
            ReportTypeName = Request.Params["ReportTypeName"];

            #endregion

            string
                ret =
                "<center>" +
                "<h2>" + ReportTypeName + " (" + DateS.ToString("yyyy-MM-dd") + " по " + DatePo.ToString("yyyy-MM-dd") + ")</h2>";
            double dSum = 0;
            string sColorTD = "";

            #region По складу определяем кассу и банк

            int DirCashOfficeID = 0, DirBankID = 0;
            if (DirWarehouseID > 0)
            {
                var queryCashBank = await Task.Run(() =>
                    (
                        from x in db.DirWarehouses
                        where x.DirWarehouseID == DirWarehouseID
                        select x
                    ).ToListAsync());

                if (queryCashBank.Count() > 0)
                {
                    DirCashOfficeID = queryCashBank[0].DirCashOfficeID;
                    DirBankID = queryCashBank[0].DirBankID;
                }
            }

            #endregion


            if (ReportType == 1)
            {
                #region Все операции

                if (CasheAndBank)
                {
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
                            //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                            DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                            DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                            DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                            DocID = x.DocID,
                            DocXID = x.DocXID,
                            //DirWarehouseName = DirCashOfficeName,

                            Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                        }
                    );

                    queryTemp = queryTemp.Union
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if(Cashe)
                {
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
                            //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                            DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                            DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                            DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                            DocID = x.DocID,
                            DocXID = x.DocXID,
                            //DirWarehouseName = DirCashOfficeName,

                            Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Bank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderBank(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Банк
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }

                return ret;

                #endregion
            }
            else if (ReportType == 2)
            {
                #region Продажи

                if (CasheAndBank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Cashe)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Bank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderBank(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Банк
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }

                return ret;

                #endregion
            }
            else if (ReportType == 7)
            {
                #region Ремонты

                if (CasheAndBank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Cashe)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Bank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderBank(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Банк
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }

                return ret;

                #endregion
            }
            else if (ReportType == 8)
            {
                #region Продажи + Ремонты

                if (CasheAndBank)
                {
                    #region queryTemp

                    var queryTemp =
                        (
                            from x in db.DocCashOfficeSums

                            where

                            x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                            (x.DirCashOfficeSumTypeID == 6 || x.DirCashOfficeSumTypeID == 16 || x.DirCashOfficeSumTypeID == 14 || x.DirCashOfficeSumTypeID == 15)

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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
                            }
                        );

                    queryTemp = queryTemp.Union
                        (
                            from x in db.DocBankSums

                            where

                            x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                            (x.DirBankSumTypeID == 13 || x.DirBankSumTypeID == 14 || x.DirBankSumTypeID == 5 || x.DirBankSumTypeID == 15)

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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Cashe)
                {
                    #region queryTemp

                    var queryTemp =
                        (
                            from x in db.DocCashOfficeSums

                            where

                            x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo &&
                            (x.DirCashOfficeSumTypeID == 6 || x.DirCashOfficeSumTypeID == 16 || x.DirCashOfficeSumTypeID == 14 || x.DirCashOfficeSumTypeID == 15)

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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Bank)
                {
                    #region queryTemp

                    var queryTemp =
                        (
                            from x in db.DocBankSums

                            where

                            x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo &&
                            (x.DirBankSumTypeID == 13 || x.DirBankSumTypeID == 14 || x.DirBankSumTypeID == 5 || x.DirBankSumTypeID == 15)

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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderBank(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Банк
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }

                return ret;

                #endregion
            }
            else if (ReportType == 3)
            {
                #region Возвраты

                if (CasheAndBank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Cashe)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Bank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderBank(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Банк
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }

                return ret;

                #endregion
            }
            else if (ReportType == 4)
            {
                #region Внесения

                if (CasheAndBank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Cashe)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName,DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Bank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderBank(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Банк
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }

                return ret;

                #endregion
            }
            else if (ReportType == 5)
            {
                #region Выплаты

                if (CasheAndBank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Cashe)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Bank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocBankSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocBankSumID,
                                DocCashOfficeBankSumSum = x.DocBankSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirBankName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderBank(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Банк
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }

                return ret;

                #endregion
            }
            else if (ReportType == 6)
            {
                #region Z-отчет

                if (CasheAndBank)
                {
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
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeBankSumDate = x.DocCashOfficeSumDate.ToString(),
                                DocCashOfficeBankSumID = x.DocCashOfficeSumID,
                                DocCashOfficeBankSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                                //DirWarehouseName = DirCashOfficeName,

                                Discount = x.doc.Discount == null ? 0 : x.doc.Discount,
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


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeBankSumSum;
                        if (query[i].DocCashOfficeBankSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeBankSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeBankSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeBankSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
                            //5. Примечание
                            "<TD>" + query[i].Base + "</TD> " +
                            //6. Касса
                            "<TD>" + query[i].DirCashOfficeBankName + "</TD> " +
                            //7. Оператор, который создал запись
                            "<TD>" + query[i].DirEmployeeName + "</TD> " +
                            "</TR>";
                    }

                    ret +=
                        "<TR>" +
                        //1.
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Cashe)
                {
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
                                DirCashOfficeID = x.DirCashOfficeID,
                                DirCashOfficeName = x.dirCashOffice.DirCashOfficeName,
                                DirCashOfficeSumTypeID = x.DirCashOfficeSumTypeID,
                                DirCashOfficeSumTypeName = x.dirCashOfficeSumType.DirCashOfficeSumTypeName,
                                DirCurrencyID = x.DirCurrencyID,
                                //DirCurrencyName = x.dirCurrency.DirCurrencyName, DirCurrencyMultiplicity = x.DirCurrencyMultiplicity, DirCurrencyRate = x.DirCurrencyRate,
                                DocCashOfficeSumDate = x.DocCashOfficeSumDate,
                                DocCashOfficeSumID = x.DocCashOfficeSumID,
                                DocCashOfficeSumSum = x.DocCashOfficeSumSum,
                                DocID = x.DocID,
                                DocXID = x.DocXID,
                            }
                        );

                    if (DirWarehouseID > 0)
                    {
                        queryTemp = queryTemp.Where(x => x.DirCashOfficeID == DirCashOfficeID);
                    }

                    if (DirEmployeeID > 0)
                    {
                        queryTemp = queryTemp.Where(x => x.DirEmployeeID == DirEmployeeID);
                    }

                    #endregion


                    #region DataReader

                    ret += ReportHeaderCash(pLanguage) + "</center>";

                    //Получение списка
                    var query = await Task.Run(() => queryTemp.ToListAsync());

                    //Получение данных
                    for (int i = 0; i < query.Count(); i++)
                    {
                        dSum += query[i].DocCashOfficeSumSum;
                        if (query[i].DocCashOfficeSumSum < 0) { sColorTD = "bgcolor='#FF0000'"; }
                        else { sColorTD = ""; }

                        ret +=
                            "<TR>" +
                            //1. Дата + Время
                            "<TD>" + Convert.ToDateTime(query[i].DocCashOfficeSumDate).ToString("yyyy-MM-dd HH:mm") + "</TD> " +
                            //2. Тип операции
                            "<TD>" + query[i].DirCashOfficeSumTypeName + query[i].DocXID + "</TD> " +
                            //3. Сумма + Валюта
                            "<TD " + sColorTD + ">" + query[i].DocCashOfficeSumSum.ToString() + "</TD> " +
                            //"<TD>" + query[i].DirCurrencyName + " (" + query[i].DirCurrencyRate + ", " + query[i].DirCurrencyMultiplicity + ")" + "</TD> " +
                            //4. От кого (кому)
                            //"<TD>" + query[i].DirEmployeeNameMoney + "</TD> " +
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
                        "<TD> <b>Итого</b> </TD> " +
                        //2.
                        "<TD> </TD> " +
                        //3.
                        "<TD> <b>" + dSum.ToString() + "</b> </TD> " +
                        "<TD> </TD> " +
                        //4.
                        //"<TD> </TD> " +
                        //5.
                        "<TD> </TD> " +
                        //6.
                        "<TD> </TD> " +
                        //7.
                        //"<TD> </TD> " +
                        "</TR>";

                    ret += "</TABLE><br />";

                    #endregion
                }
                else if (Bank)
                {
                    ret = "Отчет 'Z-отчет' доступен только для Кассы!";
                }

                return ret;

                #endregion
            }
            

            return ret;

        }


        //СТАНДАРТНАЯ табличная часть
        private string ReportHeaderCash(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата</TD> <TD class='table_header'>Операція</TD> <TD class='table_header'>Сума</TD> <TD class='table_header'>Примітка</TD> <TD class='table_header'>Каса</TD> <TD class='table_header'>Оператор</TD>" +
                    "</TR>";

                default:
                    return
                    Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата</TD> <TD class='table_header'>Операция</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Примечание</TD> <TD class='table_header'>Касса</TD> <TD class='table_header'>Оператор</TD>" +
                    "</TR>";
            }
        }

        //СТАНДАРТНАЯ табличная часть
        private string ReportHeaderBank(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата</TD> <TD class='table_header'>Операція</TD> <TD class='table_header'>Сума</TD> <TD class='table_header'>Примітка</TD> <TD class='table_header'>Каса</TD> <TD class='table_header'>Оператор</TD>" +
                    "</TR>";

                default:
                    return
                    Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата</TD> <TD class='table_header'>Операция</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Примечание</TD> <TD class='table_header'>Касса</TD> <TD class='table_header'>Оператор</TD>" +
                    "</TR>";
            }
        }

    }
}