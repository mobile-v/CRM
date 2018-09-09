using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PartionnyAccount.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Collections;
using System.Data.SQLite;
using System.Text;
using System.IO;

namespace PartionnyAccount.Controllers
{
    public class ReportController : Controller
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Controllers.Sklad.Sys.SysJourDispsController();
        private DbConnectionSklad db = new DbConnectionSklad();

        #endregion

        // GET: Report

        public ActionResult Index()
        {
            return View();
        }

        //Отчеты
        [HttpGet]
        public async Task<ActionResult> Report()
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access)
            {
                //return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));
                ViewData["Title"] = Classes.Language.Sklad.Language.msg10;
                return View();
            }

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            #endregion


            #region Параметр

            string pID = Request.Params["pID"];

            #endregion


            //Права (1 - Write, 2 - Read, 3 - No Access)
            string pID2 = pID;
            if (pID2 == "DocPurcheTabsPrintCode") pID2 = "DocPurchesPrintCode";
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "Right" + pID2)); //"RightDocCashOfficeReport"
            if (iRight == 3)
            {
                ViewData["Error"] = Classes.Language.Sklad.Language.msg57(0);
                return View();
            }


            switch (pID)
            {
                case "ReportPriceList":
                    {
                        Controllers.Sklad.Report.ReportPriceList _class = new Controllers.Sklad.Report.ReportPriceList();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db, field.DirCustomersID));
                    }
                    break;

                case "ReportProfit":
                    {
                        Controllers.Sklad.Report.ReportReportProfit _class = new Controllers.Sklad.Report.ReportReportProfit();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db));
                    }
                    break;

                case "ReportRemnants":
                    {
                        Controllers.Sklad.Report.ReportRemnants _class = new Controllers.Sklad.Report.ReportRemnants();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db));
                    }
                    break;

                case "DocServicePurchesReport":
                    {
                        Controllers.Sklad.Report.DocServicePurchesReport _class = new Controllers.Sklad.Report.DocServicePurchesReport();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db));
                    }
                    break;

                case "ReportRetailCash":
                    {
                        Controllers.Sklad.Report.ReportRetailCash _class = new Controllers.Sklad.Report.ReportRetailCash();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db, field.DirCustomersID, field));
                    }
                    break;


                    //!!! Печать кодов !!!
                case "DocPurchesPrintCode":
                    {

                        int DocID = Convert.ToInt32(Request.Params["DocID"]);
                        string UO_Action = Request.Params["UO_Action"];

                        MapPath = System.Web.HttpContext.Current.Server.MapPath("~/");
                        iUsersID = field.DirCustomersID;


                        //Загружаем настройки
                        sysSettings = await db.SysSettings.FindAsync(1);

                        barSettings.EncodeType = sysSettings.LabelEncodeType; //21; // "Code 128"

                        barSettings.Width = sysSettings.LabelWidth; //100;
                        barSettings.Height = sysSettings.LabelHeight; //30;

                        barSettings_Qr.Width = sysSettings.LabelWidth; //100;
                        barSettings_Qr.Height = sysSettings.LabelHeight; //30;


                        string ret =
                            "<center>\n" +
                            "<table style='border:1px solid black; width: " + (barSettings.Width + 1).ToString() + "px; height: " + (barSettings.Height + 1).ToString() + "px;'>\n" +
                            "<tbody>\n";

                        //Получам весь товар по DocID
                        var queryDocPurchTabs = await
                            (
                                from x in db.DocPurchTabs
                                where x.docPurch.DocID == DocID
                                select x
                            ).ToListAsync();
                        if (queryDocPurchTabs.Count() == 0)
                        {
                            ViewData["ReportHtml"] = "<center> </center>";
                            return View();
                        }

                        for (int c = 0; c < queryDocPurchTabs.Count(); c++)
                        {
                            string Value = "";
                            for (int i = 0; i < Convert.ToInt32(queryDocPurchTabs[c].Quantity); i++)
                            {
                                if (UO_Action.IndexOf("barcode") > -1)
                                {
                                    //Bar
                                    Value = dirBarCodeImage.GetBarCodeImageLink(barSettings, queryDocPurchTabs[c].DirNomenID.ToString(), iUsersID.ToString(), LocalGenID++, MapPath);
                                    //Qr
                                    //Value = dirQrCodeImage.GetBarCodeImageLink(barSettings_Qr, queryDocPurchTabs[c].DirNomenID.ToString(), iUsersID.ToString(), LocalGenID++, MapPath);


                                    ret +=
                                        " <tr>\n" +
                                        "  <td>\n" +
                                        "   <span style='font-size: 14px;'>\n" +
                                        "   <center>\n";

                                    if (UO_Action == "barcode_name") { ret += queryDocPurchTabs[c].dirNomen.DirNomenName + "<br />\n"; }
                                    else if (UO_Action == "barcode_price") { ret += queryDocPurchTabs[c].PriceRetailCurrency + " руб<br />\n"; }

                                    ret +=
                                        "   </center>\n" +
                                        "   </span>\n" +
                                        "   " + Value + "\n" +

                                        //"<br />" +
                                        //"<center>" + queryDocPurchTabs[c].DirNomenID.ToString() + "</center>" +

                                        "  </td>\n" +
                                        " </tr>\n";
                                }
                                else if (UO_Action.IndexOf("Qr") > -1)
                                {
                                    //Bar
                                    //Value = dirBarCodeImage.GetBarCodeImageLink(barSettings, queryDocPurchTabs[c].DirNomenID.ToString(), iUsersID.ToString(), LocalGenID++, MapPath);
                                    //Qr
                                    Value = dirQrCodeImage.GetBarCodeImageLink(barSettings_Qr, queryDocPurchTabs[c].DirNomenID.ToString(), iUsersID.ToString(), LocalGenID++, MapPath);


                                    ret +=
                                        " <tr>\n" +
                                        "  <td>\n" +
                                        "   <span style='font-size: 14px;'>\n" +
                                        "   <center>\n";

                                    if (UO_Action == "barcode_name") { ret += queryDocPurchTabs[c].dirNomen.DirNomenName + "<br />\n"; }
                                    else if (UO_Action == "barcode_price") { ret += queryDocPurchTabs[c].PriceRetailCurrency + " руб<br />\n"; }

                                    ret +=
                                        "   </center>\n" +
                                        "   </span>\n" +
                                        "   " + Value + "\n" +

                                        "<br />" +
                                        "<center>" + queryDocPurchTabs[c].DirNomenID.ToString() + "</center>" +

                                        "  </td>\n" +
                                        " </tr>\n";
                                }
                            }

                        }


                        ret +=
                            "</tbody>\n" +
                            "</table>\n" +
                            "</center>";

                        ViewData["ReportHtml"] = ret;
                    }
                    break;
                case "DocPurcheTabsPrintCode":
                    {

                        string Quantity = Request.Params["Quantity"];
                        string DirNomenID = Request.Params["DirNomenID"];
                        string DirNomenName = Request.Params["DirNomenName"];
                        string PriceRetailCurrency = Request.Params["PriceRetailCurrency"];
                        string UO_Action = Request.Params["UO_Action"];

                        MapPath = System.Web.HttpContext.Current.Server.MapPath("~/");
                        iUsersID = field.DirCustomersID;


                        //Загружаем настройки
                        sysSettings = await db.SysSettings.FindAsync(1);

                        barSettings.EncodeType = sysSettings.LabelEncodeType; //21; // "Code 128"

                        barSettings.Width = sysSettings.LabelWidth; //100;
                        barSettings.Height = sysSettings.LabelHeight; //30;

                        barSettings_Qr.Width = sysSettings.LabelWidth; //100;
                        barSettings_Qr.Height = sysSettings.LabelHeight; //30;


                        string ret =
                            "<center>\n" +
                            "<table style='border:1px solid black; width: " + (barSettings.Width + 1).ToString() + "px; height: " + (barSettings.Height + 1).ToString() + "px;'>\n" +
                            "<tbody>\n";


                        string Value = "";
                        for (int i = 0; i < Convert.ToInt32(Quantity); i++)
                        {
                            Value = dirBarCodeImage.GetBarCodeImageLink(barSettings, DirNomenID, iUsersID.ToString(), LocalGenID++, MapPath);


                            ret +=
                                " <tr>\n" +
                                "  <td>\n" +
                                "   <span style='font-size: 14px;'>\n" +
                                "   <center>\n";

                            if (UO_Action == "barcode_name") { ret += DirNomenName + "<br />\n"; }
                            else if (UO_Action == "barcode_price") { ret += PriceRetailCurrency + " руб<br />\n"; }

                            ret +=
                                "   </center>\n" +
                                "   </span>\n" +
                                "   " + Value + "\n" +
                                "  </td>\n" +
                                " </tr>\n";
                        }


                        ret +=
                            "</tbody>\n" +
                            "</table>\n" +
                            "</center>";

                        ViewData["ReportHtml"] = ret;
                    }
                    break;

                case "ReportTotalTrade":
                    {
                        Controllers.Sklad.Report.ReportTotalTrade _class = new Controllers.Sklad.Report.ReportTotalTrade();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db));
                    }
                    break;

                case "ReportBanksCashOffices":
                    {
                        Controllers.Sklad.Report.ReportBanksCashOffices _class = new Controllers.Sklad.Report.ReportBanksCashOffices();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db));
                    }
                    break;

                case "ReportMovementNomen":
                    {
                        Controllers.Sklad.Report.ReportMovementNomen _class = new Controllers.Sklad.Report.ReportMovementNomen();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db));
                    }
                    break;

                case "ReportLogistics":
                    {
                        Controllers.Sklad.Report.ReportLogistics _class = new Controllers.Sklad.Report.ReportLogistics();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db));
                    }
                    break;

                case "DocSalaries":
                    {
                        Controllers.Sklad.Report.ReportSalaries _class = new Controllers.Sklad.Report.ReportSalaries();
                        ViewData["ReportHtml"] = await Task.Run(() => _class.Generate(Request, db));
                    }
                    break;


                default: ViewData["ReportHtml"] = "Отчета "+ pID + " не найдено!"; break;
            }

            #region Выводим отчет

            return View();

            #endregion
        }



        //Печатные формы документов
        #region var

        string
            //pUsersID, CookieL, CookieText = "Ru",
            pID, DocID, ListObjectPFID, DocDate,
            HTTP_HOST, HtmlExcel = "html",
            sHtmlHeader = "", sHtmlTab = "", sHtmlTabText = "", sHtmlFooter = "",
            SQLHeader = "", SQLTab = "", SQLFooter = "",
            SQLTab1 = "", SQLTab2 = "", SQLTab3 = "", SQLTab4 = "", //Для Сервисного центра (УчетОблак: Акт пересортицы)
            ListObjectID = "0",

            ListObjectPFHtmlHeader = "",
            ListObjectPFHtmlTabCap = "",
            ListObjectPFHtmlTabText = "",
            ListObjectPFHtmlTabFooter = "",
            ListObjectPFHtmlFooter = "",
            ListLanguageID = "1", //Язык, для ПКО "НДС" (1-Рус, 2-Укр, ...)
            MapPath = "";

        ArrayList alTab = new ArrayList();
        int iUsersID = 0, pLanguage = 0, MarginTop = 0, MarginBottom = 0, MarginLeft = 0, MarginRight = 0;
        bool ListObjectPFHtmlCSSUse = false,
            ListObjectPFHtmlHeaderUse = false,
            ListObjectPFHtmlDouble = false,
            ListObjectPFHtmlTabUseTab = false, ListObjectPFHtmlTabUseCap = false, ListObjectPFHtmlTabEnumerate = false, ListObjectPFHtmlTabFont = false,
            ListObjectPFHtmlTabUseText = false,
            ListObjectPFHtmlTabUseFooter = false,
            ListObjectPFHtmlFooterUse = false;

        string pathFileStock = ""; // AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\UsersTemp\FileStock\";

        //Classes.Settings.Variables _var = new Class.Settings.Variables();
        Classes.Function.Variables.FileFolder fileFolder = new Classes.Function.Variables.FileFolder();
        Classes.Function.Variables.ConnectionString _var = new Classes.Function.Variables.ConnectionString();

        //Class.Functions.SQLiteDate sQLiteDate = new Class.Functions.SQLiteDate();
        Classes.Function.FunctionSQLite.SQLiteDate sQLiteDate = new Classes.Function.FunctionSQLite.SQLiteDate();

        //Class.Sys.SysSettings sysSettings = new Class.Sys.SysSettings();
        Models.Sklad.Sys.SysSetting sysSettings = new Models.Sklad.Sys.SysSetting();

        #endregion

        [HttpGet]
        public async Task<ActionResult> ReportPF(HttpRequestMessage request)
        {
            try
            {
                #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

                //Получаем Куку
                System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

                // Проверяем Логин и Пароль
                Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
                if (!field.Access)
                {
                    //return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));
                    ViewData["Title"] = Classes.Language.Sklad.Language.msg10;
                    return View();
                }

                //Изменяем строку соединения
                db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                iUsersID = field.DirCustomersID;

                //Получам настройки
                sysSettings = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметр

                pID = Request.Params["pID"];
                DocID = Request.Params["DocID"];
                ListObjectPFID = Request.Params["ListObjectPFID"];
                HtmlExcel = Request.Params["HtmlExcel"];
                HTTP_HOST = Request.Params["HTTP_HOST"];

                pathFileStock = fileFolder.Return(iUsersID, "FileStock");

                #endregion


                //Права (1 - Write, 2 - Read, 3 - No Access)
                /*int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, pID));
                if (iRight == 3)
                {
                    ViewData["Error"] = Classes.Language.Sklad.Language.msg57(0);
                    return View();
                }*/


                MapPath = System.Web.HttpContext.Current.Server.MapPath("~/");
                await Task.Run(() => ReportShow());
                //ReportShow();
            }
            catch (Exception ex) { ViewData["Error"] = "<center><h1>Приносим извинения!</h1><br> Вышла вот такая ошибочка:<br />" + ex.Message + "</center>"; }

            return View();
        }




        string ExcellHeader = "";
        //private void ReportShow()
        //private async Task<bool> ReportShow()
        private async Task<bool> ReportShow()
        {
            //Получаем номер пользователя из MS SQL по "pUsersID"
            //iUsersID = _var.VerifyUser_MSSQL_ID(pUsersID);

            //1.Получаем из ListObjectPF Булевые и Текст
            //Class.Functions.InitSQLiteSearch.initSQLiteSearch();
            using (SQLiteConnection con = new SQLiteConnection(_var.GetSQLiteBasicConnStr_DirCustomersID(iUsersID)))
            {
                con.Open();

                using (SQLiteConnection conHistory = new SQLiteConnection(_var.GetSQLiteBasicConnStr_DirCustomersID(iUsersID)))
                {
                    conHistory.Open();

                    string SQL = "";

                    #region 0.Получаем дату Документа по DocID, для Констант.

                    SQL = "SELECT DocDate FROM Docs WHERE DocID=@DocID";
                    using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
                    {
                        SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = DocID }; cmd.Parameters.Add(parDocID);
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                DocDate = Convert.ToDateTime(dr["DocDate"].ToString()).ToString("yyyy-MM-dd");
                            }
                        }
                    }

                    #endregion


                    //1.1.Получаем из ListObjectPF Булевые и Текст
                    #region Выборка Текста

                    SQL = "SELECT ListObjectID, ListObjectPFHtmlCSSUse, MarginTop, MarginBottom, MarginLeft, MarginRight, " +
                        "ListObjectPFHtmlHeaderUse, ListObjectPFHtmlDouble, ListObjectPFHtmlHeader, " +
                        "ListObjectPFHtmlTabUseTab, ListObjectPFHtmlTabUseCap, ListObjectPFHtmlTabCap, ListObjectPFHtmlTabEnumerate, ListObjectPFHtmlTabFont, " +
                        "ListObjectPFHtmlTabUseFooter, ListObjectPFHtmlTabFooter, " +
                        "ListObjectPFHtmlTabUseText, ListObjectPFHtmlTabText, " +
                        "ListObjectPFHtmlFooterUse, ListObjectPFHtmlFooter, " +
                        "ListLanguageID " +
                        "FROM ListObjectPFs " +
                        "WHERE ListObjectPFID=@ListObjectPFID";
                    using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
                    {
                        SQLiteParameter parListObjectPFID = new SQLiteParameter("@ListObjectPFID", System.Data.DbType.Int32) { Value = ListObjectPFID }; cmd.Parameters.Add(parListObjectPFID);
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                ListObjectID = dr["ListObjectID"].ToString();
                                ListObjectPFHtmlCSSUse = Convert.ToBoolean(dr["ListObjectPFHtmlCSSUse"].ToString());
                                MarginTop = Convert.ToInt32(dr["MarginTop"].ToString());
                                MarginBottom = Convert.ToInt32(dr["MarginBottom"].ToString());
                                MarginLeft = Convert.ToInt32(dr["MarginLeft"].ToString());
                                MarginRight = Convert.ToInt32(dr["MarginRight"].ToString());

                                ListObjectPFHtmlHeaderUse = Convert.ToBoolean(dr["ListObjectPFHtmlHeaderUse"].ToString());
                                ListObjectPFHtmlDouble = Convert.ToBoolean(dr["ListObjectPFHtmlDouble"].ToString());
                                ListObjectPFHtmlHeader = dr["ListObjectPFHtmlHeader"].ToString();

                                ListObjectPFHtmlTabUseTab = Convert.ToBoolean(dr["ListObjectPFHtmlTabUseTab"].ToString());
                                ListObjectPFHtmlTabUseCap = Convert.ToBoolean(dr["ListObjectPFHtmlTabUseCap"].ToString());
                                ListObjectPFHtmlTabCap = dr["ListObjectPFHtmlTabCap"].ToString();
                                ListObjectPFHtmlTabEnumerate = Convert.ToBoolean(dr["ListObjectPFHtmlTabEnumerate"].ToString());
                                ListObjectPFHtmlTabFont = Convert.ToBoolean(dr["ListObjectPFHtmlTabFont"].ToString());

                                ListObjectPFHtmlTabUseFooter = Convert.ToBoolean(dr["ListObjectPFHtmlTabUseFooter"].ToString());
                                ListObjectPFHtmlTabFooter = dr["ListObjectPFHtmlTabFooter"].ToString();

                                ListObjectPFHtmlTabUseText = Convert.ToBoolean(dr["ListObjectPFHtmlTabUseText"].ToString());
                                ListObjectPFHtmlTabText = dr["ListObjectPFHtmlTabText"].ToString();

                                ListObjectPFHtmlFooterUse = Convert.ToBoolean(dr["ListObjectPFHtmlFooterUse"].ToString());
                                ListObjectPFHtmlFooter = dr["ListObjectPFHtmlFooter"].ToString();

                                ListLanguageID = dr["ListLanguageID"].ToString(); //Язык, для ПКО "НДС"
                            }
                        }
                    }

                    //Выборка Табличной части
                    SQL =
                    "SELECT " +
                    " ListObjectPFTabs.ListObjectPFTabName, " +
                    " ListObjectPFTabs.ListObjectFieldNameID, " +
                    " ListObjectFieldNames.ListObjectFieldNameReal, " +
                    " ListObjectPFTabs.PositionID, ListObjectPFTabs.TabNum, ListObjectPFTabs.Width " +
                    "FROM ListObjectPFTabs, ListObjectFieldNames " +
                    "WHERE " +
                    " (ListObjectPFTabs.ListObjectFieldNameID = ListObjectFieldNames.ListObjectFieldNameID)and " +
                    " (ListObjectPFID = @ListObjectPFID) ";
                    using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
                    {
                        SQLiteParameter parListObjectPFID = new SQLiteParameter("@ListObjectPFID", System.Data.DbType.Int32) { Value = ListObjectPFID }; cmd.Parameters.Add(parListObjectPFID);
                        using (SQLiteDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string[] sArr = new string[5];
                                sArr[0] = dr["ListObjectPFTabName"].ToString();
                                sArr[1] = dr["ListObjectFieldNameReal"].ToString();
                                sArr[2] = dr["PositionID"].ToString();
                                sArr[3] = dr["TabNum"].ToString();
                                sArr[4] = dr["Width"].ToString();
                                alTab.Add(sArr);
                            }
                        }
                    }

                    #endregion


                    #region CSS Font: "ListObjectPFHtmlTabFont" (LiteralReport)

                    if (ListObjectPFHtmlCSSUse)
                    {
                        if (ListObjectPFHtmlTabFont)
                        {
                            if (MarginTop > 0 || MarginBottom > 0 || MarginLeft > 0 || MarginRight > 0)
                                ViewData["Style1"] =
                                "<style type='text/css'>" +
                                "body { margin: " + MarginTop + "px " + MarginRight + "px " + MarginBottom + "px " + MarginLeft + "px; } " +
                                "</style>";
                            ViewData["Style1"] += "<link rel='stylesheet' href='../../Scripts/sklad/css/Report/style1.css' />";

                            //Для EXCEL
                            ExcellHeader =
                                "<style type='text/css'>" +
                                "body { margin: " + MarginTop + "px " + MarginRight + "px " + MarginBottom + "px " + MarginLeft + "px; } " + 
                                "table { width: 100%; border: 1px solid black; border-collapse: collapse; }" +
                                ".table_header { text-align: left; background: #ccc; padding: 5px; border: 1px solid black; font-size: 12px; }" +
                                "td { padding: 5px; border: 1px solid black; font-size: 12px; }" +
                                "</style>";
                        }
                        else
                        {
                            if (MarginTop > 0 || MarginBottom > 0 || MarginLeft > 0 || MarginRight > 0)
                                ViewData["Style1"] =
                                "<style type='text/css'>" +
                                "body { margin: " + MarginTop + "px " + MarginRight + "px " + MarginBottom + "px " + MarginLeft + "px; } " +
                                "</style>";
                            ViewData["Style1"] += "<link rel='stylesheet' href='../../Scripts/sklad/css/Report/style0.css' />";

                            //Для EXCEL
                            ExcellHeader =
                                "<style type='text/css'>" +
                                "body { margin: " + MarginTop + "px " + MarginRight + "px " + MarginBottom + "px " + MarginLeft + "px; } " +
                                "table { width: 100%; border: 1px solid black; border-collapse: collapse; }" +
                                ".table_header { text-align: left; background: #ccc; padding: 5px; border: 1px solid black; font-weight: 600; }" +
                                "td { padding: 5px; border: 1px solid black; }" +
                                "</style>";
                        }
                    }
                    else
                    {
                        if (MarginTop > 0 || MarginBottom > 0 || MarginLeft > 0 || MarginRight > 0)
                            ViewData["Style1"] =
                            "<style type='text/css'>" +
                            "body { margin: " + MarginTop + "px " + MarginRight + "px " + MarginBottom + "px " + MarginLeft + "px; } " +
                            "</style>";
                        ViewData["Style1"] += "<link rel='stylesheet' href='../../Scripts/sklad/css/Report/style2.css' />";

                        //Для EXCEL
                        ExcellHeader =
                            "<style type='text/css'>" +
                            "body { margin: " + MarginTop + "px " + MarginRight + "px " + MarginBottom + "px " + MarginLeft + "px; } " +
                            ".table_1 { width: 100%; border: 1px solid black; border-collapse: collapse; }" +
                            ".table_header { text-align: left; background: #ccc; padding: 5px; border: 1px solid black; font-weight: 600; font-size: 12px; }" +
                            ".td_1 { padding: 5px; border: 1px solid black; font-size: 12px; }" +
                            "</style>";
                    }

                    #endregion


                    //1.2.Если Шапка использется, текст в "ListObjectPFHtmlHeader"
                    #region switch (ListObjectID) //pID

                    switch (ListObjectID) //pID
                    {
                        case "6":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocPurches.DocPurchesController _class = new Controllers.Sklad.Doc.DocPurches.DocPurchesController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocPurches.DocPurchTabsController _classTab = new Controllers.Sklad.Doc.DocPurches.DocPurchTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings, "");

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings, "");
                            }
                            break;
                        case "32":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocSales.DocSalesController _class = new Controllers.Sklad.Doc.DocSales.DocSalesController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocSales.DocSaleTabsController _classTab = new Controllers.Sklad.Doc.DocSales.DocSaleTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "33":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocMovements.DocMovementsController _class = new Controllers.Sklad.Doc.DocMovements.DocMovementsController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocMovements.DocMovementTabsController _classTab = new Controllers.Sklad.Doc.DocMovements.DocMovementTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "34":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocReturnVendors.DocReturnVendorsController _class = new Controllers.Sklad.Doc.DocReturnVendors.DocReturnVendorsController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocReturnVendors.DocReturnVendorTabsController _classTab = new Controllers.Sklad.Doc.DocReturnVendors.DocReturnVendorTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "35":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocActWriteOffs.DocActWriteOffsController _class = new Controllers.Sklad.Doc.DocActWriteOffs.DocActWriteOffsController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocActWriteOffs.DocActWriteOffTabsController _classTab = new Controllers.Sklad.Doc.DocActWriteOffs.DocActWriteOffTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "36":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocReturnsCustomers.DocReturnsCustomersController _class = new Controllers.Sklad.Doc.DocReturnsCustomers.DocReturnsCustomersController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocReturnsCustomers.DocReturnsCustomerTabsController _classTab = new Controllers.Sklad.Doc.DocReturnsCustomers.DocReturnsCustomerTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "37":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocActOnWorks.DocActOnWorksController _class = new Controllers.Sklad.Doc.DocActOnWorks.DocActOnWorksController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocActOnWorks.DocActOnWorkTabsController _classTab = new Controllers.Sklad.Doc.DocActOnWorks.DocActOnWorkTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "38":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocAccounts.DocAccountsController _class = new Controllers.Sklad.Doc.DocAccounts.DocAccountsController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocAccounts.DocAccountTabsController _classTab = new Controllers.Sklad.Doc.DocAccounts.DocAccountTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "39":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocInventories.DocInventoriesController _class = new Controllers.Sklad.Doc.DocInventories.DocInventoriesController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocInventories.DocInventoryTabsController _classTab = new Controllers.Sklad.Doc.DocInventories.DocInventoryTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "40":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocServicePurches.DocServicePurchesController _class = new Controllers.Sklad.Doc.DocServicePurches.DocServicePurchesController();
                                SQLHeader = _class.GenerateSQLSelect(true);
                                SQLFooter += _class.GenerateSQLSUM(sysSettings);

                                //Табличные части
                                //1
                                Controllers.Sklad.Doc.DocServicePurches.DocServicePurch1TabsController _classTab1 = new Controllers.Sklad.Doc.DocServicePurches.DocServicePurch1TabsController();
                                SQLTab1 = _classTab1.GenerateSQLSelectCollection1(sysSettings);
                                //1
                                Controllers.Sklad.Doc.DocServicePurches.DocServicePurch2TabsController _classTab2 = new Controllers.Sklad.Doc.DocServicePurches.DocServicePurch2TabsController();
                                SQLTab2 = _classTab2.GenerateSQLSelectCollection2(sysSettings);
                            }
                            break;
                        case "56":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocRetails.DocRetailsController _class = new Controllers.Sklad.Doc.DocRetails.DocRetailsController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocRetails.DocRetailTabsController _classTab = new Controllers.Sklad.Doc.DocRetails.DocRetailTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "65":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurchesController _class = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurchesController();
                                SQLHeader = _class.GenerateSQLSelect(true);
                                SQLFooter += _class.GenerateSQLSUM(sysSettings);

                                //Табличные части
                                //1
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurch1TabsController _classTab1 = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurch1TabsController();
                                SQLTab1 = _classTab1.GenerateSQLSelectCollection1(sysSettings);
                                //1
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurch2TabsController _classTab2 = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandPurch2TabsController();
                                SQLTab2 = _classTab2.GenerateSQLSelectCollection2(sysSettings);
                            }
                            break;
                        case "66":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandSalesController _class = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandSalesController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                //Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailTabsController _classTab = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailTabsController();
                                //SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                //SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "67":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailReturnsController _class = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailReturnsController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailReturnTabsController _classTab = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailReturnTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "68":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailActWriteOffsController _class = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailActWriteOffsController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                //Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailActWriteOffTabsController _classTab = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandRetailActWriteOffTabsController();
                                //SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                //SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;
                        case "76":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocSecondHandInventories.DocSecondHandInvsController _class = new Controllers.Sklad.Doc.DocSecondHandInventories.DocSecondHandInvsController();
                                SQLHeader = _class.GenerateSQLSelect(true);
                                SQLFooter += _class.GenerateSQLSUM(sysSettings);

                                //Табличные части
                                //1
                                Controllers.Sklad.Doc.DocSecondHandInvs.DocSecondHandInvTabsController _classTab = new Controllers.Sklad.Doc.DocSecondHandInvs.DocSecondHandInvTabsController();
                                SQLTab1 = _classTab.GenerateSQLSelectCollection(sysSettings, 1);
                                SQLTab2 = _classTab.GenerateSQLSelectCollection(sysSettings, 2);
                                SQLTab4 = _classTab.GenerateSQLSelectCollection(sysSettings, 4);
                                SQLTab3 = _classTab.GenerateSQLSelectCollection(sysSettings, 3);
                            }
                            break;
                        case "71":
                            {
                                //Шапка
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandMovsController _class = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandMovsController();
                                SQLHeader = _class.GenerateSQLSelect(true);

                                //Сумма
                                Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandMovTabsController _classTab = new Controllers.Sklad.Doc.DocSecondHandPurches.DocSecondHandMovTabsController();
                                SQLFooter += _classTab.GenerateSQLSUM(sysSettings);

                                //Табличная часть
                                SQLTab = _classTab.GenerateSQLSelectCollection(sysSettings);
                            }
                            break;

                        default: ViewData["ReportHtml"] = ListObjectID + Classes.Language.Sklad.Language.msg34; return false;
                    }

                    #endregion


                    #region ListObjectPFHtmlHeaderUse || ListObjectPFHtmlTabUseText || ListObjectPFHtmlFooterUse

                    if (ListObjectPFHtmlHeaderUse)
                    {
                        sHtmlHeader = ListObjectPFHtml(con, conHistory, ListObjectPFHtmlHeader, SQLHeader);
                    }
                    if (ListObjectPFHtmlTabUseTab)
                    {
                        //if (pID != "PFrDocActRegrading")
                        if(ListObjectID != "40" && ListObjectID != "76")
                        {
                            sHtmlTab = ListObjectPFTab(con, conHistory, SQLTab, "1");
                        }
                        else if (ListObjectID == "40")
                        {
                            sHtmlTab += "";
                            sHtmlTab += ListObjectPFTab(con, conHistory, SQLTab1, "1");
                            sHtmlTab += "<BR>";
                            sHtmlTab += ListObjectPFTab(con, conHistory, SQLTab2, "2");
                        }
                        else if (ListObjectID == "76")
                        {
                            sHtmlTab += "<BR>";
                            sHtmlTab += "Присутствуют: <br />";
                            sHtmlTab += ListObjectPFTab(con, conHistory, SQLTab1, "1");
                            sHtmlTab += "<BR>";
                            sHtmlTab += "Списать с ЗП: <br />";
                            sHtmlTab += ListObjectPFTab(con, conHistory, SQLTab2, "2");
                            sHtmlTab += "<BR>";
                            sHtmlTab += "Отсутствует: <br />";
                            sHtmlTab += ListObjectPFTab(con, conHistory, SQLTab4, "2");
                            sHtmlTab += "<BR>";
                            sHtmlTab += "На Разбор: <br />";
                            sHtmlTab += ListObjectPFTab(con, conHistory, SQLTab3, "3");
                        }
                    }
                    if (ListObjectPFHtmlTabUseText)
                    {
                        sHtmlTabText = ListObjectPFTabText(con, conHistory, ListObjectPFHtmlTabText);
                    }
                    if (ListObjectPFHtmlFooterUse)
                    {
                        sHtmlFooter = ListObjectPFHtml(con, conHistory, ListObjectPFHtmlFooter, SQLFooter);
                    }

                    #endregion

                    conHistory.Close();

                } //SQLiteConnection History

                con.Close();
            } //SQLiteConnection


            ViewData["ReportHtml"] = "";

            #region HtmlExcel == "excel"

            if (HtmlExcel == "excel")
            {
                Classes.Function.GenGenerate genGenerate = new Classes.Function.GenGenerate();
                string GenIDX = await Task.Run(() => genGenerate.ReturnGenID(db));
                string FileName = "PF_" + genGenerate.ReturnTrash() + "_" + GenIDX + genGenerate.ReturnTrash() + ".xls";
                string _Path = MapPath + "UsersTemp/FileStock/" + FileName;
                System.IO.File.Delete(_Path);

                using (FileStream fs = new FileStream(_Path, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                    {
                        sw.Write(
                            "<table>" +
                            "<head>" +
                            "<title>Печатная форма</title>" +
                            ExcellHeader +
                            " <meta http-equiv='content-type' content='text/html; charset=windows-1251' /> " +
                            "</head>" +
                            "<body>" +
                                sHtmlHeader + sHtmlTab + sHtmlTabText + sHtmlFooter +
                            "<br />" +
                            "</body>" +
                            "</table>"
                            );
                    }
                }

                ViewData["ReportHtml"] = "<br /><br /><br /><center><H1><a href='http://" + HTTP_HOST + "/UsersTemp/FileStock/" + FileName + "'>СКАЧАТЬ Печатную форму в MS Excel</a></H1></center>";
            }
            else
            {
                //var sText = (sHtmlHeader + sHtmlTab + sHtmlTabText + sHtmlFooter).Replace("{{{NotUseBeforeSecondCopy}}}", "").Replace("{{{NotUseOnSecondCopy}}}", "");


                #region 1 Часть *** *** ***

                //Удаляем "{{{UseSecondCopyBegin}}}" и "{{{UseSecondCopyEnd}}}" и текст между ними
                string sHtml = sHtmlHeader + sHtmlTab + sHtmlTabText + sHtmlFooter;

                int p1 = sHtml.IndexOf("{{{UseSecondCopyBegin}}}");
                int p2 = sHtml.IndexOf("{{{UseSecondCopyEnd}}}");
                if (p1 > -1 && p2 > -1) sHtml = sHtml.Remove(p1, p2 - p1);

                //Пишем первую часть
                //ViewData["ReportHtml"] += (sHtmlHeader + sHtmlTab + sHtmlTabText + sHtmlFooter).Replace("{{{NotUseBeforeSecondCopy}}}", "").Replace("{{{NotUseOnSecondCopy}}}", "");
                ViewData["ReportHtml"] += sHtml.Replace("{{{NotUseBeforeSecondCopy}}}", "").Replace("{{{NotUseOnSecondCopy}}}", "").Replace("{{{UseSecondCopyBegin}}}", "").Replace("{{{UseSecondCopyEnd}}}", "");

                #endregion


                #region 2. Часть *** *** ***

                if (ListObjectPFHtmlDouble)
                {
                    //ViewData["ReportHtml"] += "<br /><br />" + sHtmlHeader + sHtmlTab + sHtmlTabText + sHtmlFooter;

                    //Находим координаты "{{{NotUseBeforeSecondCopy}}}" и удаляем весь текст до этой записи.
                    //Находим координаты "{{{NotUseOnSecondCopy}}}" и удаляем весь текст от этой записи.


                    int i1 = sHtmlHeader.IndexOf("{{{NotUseBeforeSecondCopy}}}");
                    if (i1 > -1) sHtmlHeader = sHtmlHeader.Remove(0, i1 + 28);

                    i1 = sHtmlHeader.IndexOf("{{{NotUseOnSecondCopy}}}");
                    if (i1 > -1) sHtmlHeader = sHtmlHeader.Remove(i1, sHtmlHeader.Length - i1);


                    i1 = sHtmlTabText.IndexOf("{{{NotUseBeforeSecondCopy}}}");
                    if (i1 > -1) sHtmlTabText = sHtmlTabText.Remove(0, i1 + 28);

                    i1 = sHtmlTabText.IndexOf("{{{NotUseOnSecondCopy}}}");
                    if (i1 > -1) sHtmlTabText = sHtmlTabText.Remove(i1, sHtmlTabText.Length - i1);


                    i1 = sHtmlFooter.IndexOf("{{{NotUseBeforeSecondCopy}}}");
                    if (i1 > -1) sHtmlFooter = sHtmlFooter.Remove(0, i1 + 28);

                    i1 = sHtmlFooter.IndexOf("{{{NotUseOnSecondCopy}}}");
                    if (i1 > -1) sHtmlFooter = sHtmlFooter.Remove(i1, sHtmlFooter.Length - i1);


                    ViewData["ReportHtml"] += "<br /><br />" + (sHtmlHeader + sHtmlTab + sHtmlTabText + sHtmlFooter).Replace("{{{UseSecondCopyBegin}}}", "").Replace("{{{UseSecondCopyEnd}}}", "");
                }

                #endregion

            }

            #endregion

            return true;
        }

        #region Обработка текста

        private string ListObjectPFHtml(SQLiteConnection con, SQLiteConnection conHistory, string pListObjectPFHtml, string SQL)
        {
            if (SQL.Length < 5) return "";

            //Алгоритм:
            //Для Документов Справочников

            //Переменные.
            string sHtml = "";

            int i1 = 0, i2 = 0, i3 = 0;
            StringBuilder sb = new StringBuilder(pListObjectPFHtml);
            char[] b = new char[pListObjectPFHtml.Length];
            while (true)
            {
                //Получаем интервал в котором находится "Значение Справочника": [i2, i3].
                i2 = sb.ToString().IndexOf("[[[", i1);
                i3 = sb.ToString().IndexOf("]]]", i1);

                if ((i2 >= 0 && i3 > 0) && (i3 > i2))
                {
                    sHtml += sb.ToString().Substring(i1, i2 - i1);
                    i2 += 3;

                    string Field = sb.ToString().Substring(i2, i3 - i2);
                    string pValue = ReturnListObjectFieldNameReal(con, ListObjectID, Field);
                    string Value = "";
                    if (pValue != "FieldEmpty")
                    {
                        Value = ReturnValue(con, SQL, pValue); //SQLHeader, SQLFooter, SQLTab, 
                    }
                    Value = ConverterInWords(pValue, Value);


                    //ПКО или Платежные поручения=== === === === === === === === === === === === === ===
                    //Сдесь сделать проверку, если: (pID == "ПКО")and(pValue == SumVat или SumVATCurrency), то выполнить действия для ПКО.
                    if ((pID == "PFrDocPaymentOrder" || pID == "PFrDocCashOrdersPurch") && (pValue == "SumVat" || pValue == "SumVATCurrency"))
                    {
                        bool VatExist = Convert.ToBoolean(ReturnValue(con, SQL, "VatExist"));
                        bool VatNot = Convert.ToBoolean(ReturnValue(con, SQL, "VatNot"));
                        if (VatExist)
                        {
                            sHtml += Classes.Language.Sklad.Language.lanVat(pLanguage) + " " + ReturnValue(con, SQL, "DirVatValue") + "% ";
                        }
                    }
                    //=== === === === === === === === === === === === === === === === === === === === === 


                    sHtml += Value; // *** sHTML ***
                }
                else { sHtml += ErrorSymbolField(i1, i2, i3, sb); break; }

                //Переопределяем переменные.
                i3 += 3;
                i1 = i3; i2 = i3;
            }
            
            return sHtml;
        }

        //TabNum - Табличная часть (null, 0 или 1 - выборка с первой табличной части, 2 - второй)
        private string ListObjectPFTab(SQLiteConnection con, SQLiteConnection conHistory, string SQLTab, string TabNum)
        {
            if (SQLTab.Length < 5) return "";
            string HeaderTable = "<table class='table_1'>";
            string PositionID = "1";

            //Формирование Шапки таблицы
            if (!ListObjectPFHtmlTabUseCap)
            {
                HeaderTable += "<tr>";
                if (ListObjectPFHtmlTabEnumerate) { HeaderTable += "<td class='table_header' width='25'>№</td>"; }
                for (int i = 0; i < alTab.Count; i++)
                {
                    string[] sArr = (string[])alTab[i];

                    //TabNum - Табличная часть (null, 0 или 1 - выборка с первой табличной части, 2 - второй)
                    if (sArr[3] == "" || Convert.ToInt32(sArr[3]) < 2) sArr[3] = "1";
                    if (sArr[3] == TabNum)
                    {
                        //Фиксированная ширина
                        string Width = "";
                        if (sArr[4] != "" && Convert.ToInt32(sArr[4]) > 0) Width = " width='100' ";

                        PositionID = sArr[2];
                        HeaderTable += "<td class='table_header'"+ Width + ">" + sArr[0] + "</td>";
                    }
                }
                HeaderTable += "</tr>";
            }
            else
            {
                HeaderTable += ListObjectPFHtmlTabCap.Replace("<td>", "<td class='table_header'>").Replace("<td ", "<td class='table_header' ").Replace("< td ", "<td class='table_header' ").Replace("<  td ", "<td class='table_header' ");
            }

            string sHtml = "";
            //HeaderTable = sHtml; sHtml = "";

            //Формирование таблицы
            using (SQLiteCommand cmd = new SQLiteCommand(SQLTab, con))
            {
                SQLiteParameter patDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = DocID }; cmd.Parameters.Add(patDocID);
                SQLiteParameter parDirNomenHistoryDate = new SQLiteParameter("@DirNomenHistoryDate", System.Data.DbType.Date) { Value = sQLiteDate.Return((DateTime.Now).ToString()) }; cmd.Parameters.Add(parDirNomenHistoryDate); //.DirNomenHistoryDate;

                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    int Num = 0;
                    while (dr.Read())
                    {
                        sHtml += "<tr>";

                        //Параметры === === === === ===
                        string td_class = "";
                        if (!ListObjectPFHtmlTabUseCap) td_class = "class='td_1'";
                        //=== === === === === === === ===

                        if (ListObjectPFHtmlTabEnumerate) { Num++; sHtml += "<td " + td_class + ">" + Num.ToString() + "</td>"; }
                        for (int i = 0; i < alTab.Count; i++)
                        {
                            string[] sArr = (string[])alTab[i];

                            //TabNum - Табличная часть (null, 0 или 1 - выборка с первой табличной части, 2 - второй)
                            if (Convert.ToInt32(sArr[3]) < 2) sArr[3] = "1";
                            if (sArr[3] == TabNum)
                            {
                                //Фиксированная ширина
                                string Width = "";
                                if (sArr[4] != "" && Convert.ToInt32(sArr[4]) > 0) Width = " width='100' ";

                                try
                                {
                                    if (sArr[1] != "FieldEmpty")
                                    {
                                        PositionID = sArr[2];
                                        string Value = dr[sArr[1]].ToString();
                                        Value = ConverterInWords(sArr[1], Value);

                                        string align = "align='left'";
                                        if (PositionID == "2") align = "align='center'";
                                        else if (PositionID == "3") align = "align='right'";


                                        sHtml += "<td " + align + " " + td_class + ""+ Width + ">" + Value + "</td>";
                                    }
                                    else { sHtml += "<td " + td_class + "" + Width + "></td>"; }
                                }
                                catch { sHtml += "<td" + Width + ">" + sArr[1] + "</td>"; }
                            }
                        }

                        sHtml += "</tr>";
                    }
                }
            }


            if (ListObjectPFHtmlTabUseCap)
            {
                sHtml = sHtml.Replace("<td>", "<td class='td_1'>").Replace("<td ", "<td class='td_1' ").Replace("< td ", "<td class='td_1' ").Replace("<  td ", "<td class='td_1' ");
            }
            sHtml = HeaderTable + sHtml;


            //Если в конец таблицы надо добавить Итого.
            if (ListObjectPFHtmlTabUseFooter)
            {
                sHtml += ListObjectPFHtml(con, conHistory, ListObjectPFHtmlTabFooter, SQLFooter);
            }

            sHtml += "</table>";
            return sHtml;
        }

        private string ListObjectPFTabText(SQLiteConnection con, SQLiteConnection conHistory, string pListObjectPFHtml)
        {
            if (SQLTab.Length < 5) return "";

            string sHtml = "";
            using (SQLiteCommand cmd = new SQLiteCommand(SQLTab, con))
            {
                SQLiteParameter patDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = DocID }; cmd.Parameters.Add(patDocID);
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int iCount = Convert.ToInt32(dr["Quantity"].ToString()); //К-во ШК одной номенклатры
                        if (iCount == 0 || pID != "PFrDocBarCode") iCount = 1; //Если это не Документ "DocBarCode", то только один раз - одну Номенклатру
                        for (int c = 0; c < iCount; c++)
                        {
                            sHtml += "<div style='float: left'>";

                            int i1 = 0, i2 = 0, i3 = 0;
                            StringBuilder sb = new StringBuilder(pListObjectPFHtml);
                            char[] b = new char[pListObjectPFHtml.Length];
                            while (true)
                            {
                                //Получаем интервал в котором находится "Значение Справочника": [i2, i3].
                                i2 = sb.ToString().IndexOf("[[[", i1);
                                i3 = sb.ToString().IndexOf("]]]", i1);

                                if ((i2 >= 0 && i3 > 0) && (i3 > i2))
                                {
                                    sHtml += sb.ToString().Substring(i1, i2 - i1);
                                    i2 += 3;

                                    //Получаем наименование поле кирилицей из текста
                                    string Field = sb.ToString().Substring(i2, i3 - i2);
                                    //Получаем наименование поле на латиницей из БД
                                    string pValue = ReturnListObjectFieldNameReal(con, ListObjectID, Field);
                                    //Значения по полю.
                                    string Value = pValue; try { Value = dr[pValue].ToString(); }
                                    catch { }
                                    //Проверяем: надо ли урезать часть текст (есть ли в поле pValue тег Remove)
                                    Value = FieldRemove(pValue, Value);
                                    //Если поле "BarcodeImage", то вставить Штрих-Код
                                    Value = BarcodeImage(pValue, Value);

                                    Value = ConverterInWords(pValue, Value);
                                    sHtml += Value; // *** sHTML ***
                                }
                                else { sHtml += ErrorSymbolField(i1, i2, i3, sb); break; }

                                //Переопределяем переменные.
                                i3 += 3;
                                i1 = i3; i2 = i3;
                            }

                            /*
                            //Для Констант
                            i1 = 0; i2 = 0; i3 = 0;
                            sb = new StringBuilder(sHtml);
                            sHtml = "";
                            while (true)
                            {
                                //Получаем интервал в котором находится "Значение Справочника": [i2, i3].
                                i2 = sb.ToString().IndexOf("{{{", i1);
                                i3 = sb.ToString().IndexOf("}}}", i1);

                                if ((i2 >= 0 && i3 > 0) && (i3 > i2))
                                {
                                    sHtml += sb.ToString().Substring(i1, i2 - i1);
                                    i2 += 3;

                                    string Field = sb.ToString().Substring(i2, i3 - i2);
                                    string Value = ReturnSysDirConstantsValue(con, conHistory, Field, DocDate);
                                    sHtml += Value; // *** sHTML ***
                                }
                                else { sHtml += ErrorSymbolConst(i1, i2, i3, sb); break; }

                                //Переопределяем переменные.
                                i3 += 3;
                                i1 = i3; i2 = i3;
                            }
                            */

                            sHtml += "</div>";
                            sHtml += "<div style='float: left'>&nbsp;&nbsp;&nbsp;</div>";
                        }

                    } //while (dr.Read())
                }
            }

            sHtml += "";
            return sHtml;
        }
        private string FieldRemove(string pValue, string Value)
        {
            if (pValue.Length > 6 && Value.Length > 57)
            {
                string spValue = (pValue[pValue.Length - 6].ToString() + pValue[pValue.Length - 5].ToString() + pValue[pValue.Length - 4].ToString() + pValue[pValue.Length - 3].ToString() + pValue[pValue.Length - 2].ToString() + pValue[pValue.Length - 1].ToString()).ToLower();
                if (spValue == "remove")
                {
                    Value = Value.Remove(57);
                }
            }

            return Value;
        }
        int LocalGenID = 0;


        //Bar
        Classes.Function.DirBarCodeImageNew dirBarCodeImage = new Classes.Function.DirBarCodeImageNew();
        Classes.Function.DirBarCodeImageNew.BarSettings barSettings = new Classes.Function.DirBarCodeImageNew.BarSettings();
        private string BarcodeImage(string pValue, string Value)
        {
            if (pValue == "BarcodeImage")
            {
                if (Value != "") /*Value = "1230";*/
                {
                    Value = dirBarCodeImage.GetBarCodeImageLink(barSettings, Value, iUsersID.ToString(), LocalGenID++, MapPath);
                }
            }

            return Value;
        }

        //Qr
        Classes.Function.DirQrCodeImage dirQrCodeImage = new Classes.Function.DirQrCodeImage();
        Classes.Function.DirQrCodeImage.BarSettings barSettings_Qr = new Classes.Function.DirQrCodeImage.BarSettings();
        private string QrcodeImage(string pValue, string Value)
        {
            if (pValue == "BarcodeImage")
            {
                if (Value != "") /*Value = "1230";*/
                {
                    Value = dirQrCodeImage.GetBarCodeImageLink(barSettings_Qr, Value, iUsersID.ToString(), LocalGenID++, MapPath);
                }
            }

            return Value;
        }


        private string ReturnListObjectFieldNameReal(SQLiteConnection con, string ListObjectID, string ListObjectFieldName)
        {
            string ret = ListObjectFieldName;

            ListObjectFieldName = RemoveHtmlTag(ListObjectFieldName);
            string SQL = "SELECT ListObjectFieldNameReal " +
                "FROM ListObjectFields, ListObjectFieldNames " +
                "WHERE " +
                "(ListObjectFields.[ListObjectFieldNameID]=ListObjectFieldNames.[ListObjectFieldNameID])and" + 
                "(" +
                "(ListObjectFieldNameReal LIKE @ListObjectFieldName)or" +
                "(ListObjectFieldNameRu LIKE @ListObjectFieldName)or" +
                "(ListObjectFieldNameUa LIKE @ListObjectFieldName)or" +
                "(ListObjectFieldNameUs LIKE @ListObjectFieldName)" +
                ")";

            using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
            {
                //SQLiteParameter patListObjectID = new SQLiteParameter("@ListObjectID", System.Data.DbType.Int32) { Value = ListObjectID }; cmd.Parameters.Add(patListObjectID);
                SQLiteParameter patListObjectFieldName = new SQLiteParameter("@ListObjectFieldName", System.Data.DbType.String) { Value = ListObjectFieldName }; cmd.Parameters.Add(patListObjectFieldName);
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        ret = dr["ListObjectFieldNameReal"].ToString();
                    }
                }
            }

            return ret;
        }

        private string ReturnValue(SQLiteConnection con, string SQL, string pValue) //string SQLHeader, string SQLFooter, string SQLTab, 
        {
            string ret = pValue;

            //1-й SQL-запрос "SQLHeader"
            using (SQLiteCommand cmd = new SQLiteCommand(SQL, con))
            {
                SQLiteParameter patDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = DocID }; cmd.Parameters.Add(patDocID);
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        try
                        {
                            ret = dr[pValue].ToString();
                            dr.Close(); dr.Dispose();
                            return ret;
                        }
                        catch { }
                    }
                }
            }

            //2-й SQL-запрос "SQLFooter"
            /*using (SQLiteCommand cmd = new SQLiteCommand(SQLFooter, con))
            {
                SQLiteParameter patDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = DocID }; cmd.Parameters.Add(patDocID);
                using (SQLiteDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        try
                        {
                            ret = dr[pValue].ToString();
                            dr.Close(); dr.Dispose();
                            return ret;
                        }
                        catch { }
                    }
                }
            }*/

            return ret;
        }

        private string RemoveHtmlTag(string ListObjectFieldName)
        {
            string ret = "";

            ListObjectFieldName = ListObjectFieldName.Replace(" ", ""); ListObjectFieldName = ListObjectFieldName.Replace("&nbsp;", ""); ListObjectFieldName = ListObjectFieldName.Replace("&nbsp", "");

            StringBuilder sb = new StringBuilder(ListObjectFieldName);
            bool write = true;
            for (int i = 0; i < ListObjectFieldName.Length; i++)
            {
                //Начало HTML-тега
                if (sb[i].ToString() == "<") write = false;
                else if (sb[i].ToString() == ">") write = true;
                else if (write) ret += sb[i].ToString();
            }

            return ret;
        }

        private string ConverterInWords(string pValue, string Value)
        {
            try
            {
                if (pValue.IndexOf("Date") > -1 || pValue.IndexOf("Period") > -1) //Field.IndexOf(".Дата") > 0 || 
                {
                    if (pValue.IndexOf("_InWords") > -1)
                    {
                        //Value = Classes.Function.SumInWords.Ru.RuDateAndMoneyConverter.DateToTextLong(Convert.ToDateTime(Value));

                        if (ListLanguageID == "1") Value = Classes.Function.SumInWords.Ru.RuDateAndMoneyConverter.DateToTextLong(Convert.ToDateTime(Value));
                        else if (ListLanguageID == "2") Value = Classes.Function.SumInWords.Ua.RuDateAndMoneyConverter.DateToTextLong(Convert.ToDateTime(Value));
                    }
                    else Value = Convert.ToDateTime(Value).ToString(sysSettings.DateFormatStr); //"yyyy.MM.dd"
                }
                if (pValue.IndexOf("Time") > -1) //Field.IndexOf(".Время") > 0 || 
                {
                    Value = Convert.ToDateTime(Value).ToString("HH:mm:ss");
                }
                else if (pValue.IndexOf("_InWords") > -1)
                {
                    if (ListLanguageID == "1") Value = Classes.Function.SumInWords.Ru.RuDateAndMoneyConverter.CurrencyToTxt(Convert.ToDouble(Value), false);
                    else if (ListLanguageID == "2") Value = Classes.Function.SumInWords.Ua.RuDateAndMoneyConverter.CurrencyToTxt(Convert.ToDouble(Value), false);
                }
                else if (pValue.IndexOf("_NumInWords") > -1)
                {
                    if (ListLanguageID == "1") Value = Classes.Function.SumInWords.Ru.RuDateAndMoneyConverter.NumeralsToTxt((long)Convert.ToDouble(Value), Classes.Function.SumInWords.Ru.TextCase.Accusative, false, false);
                    else if (ListLanguageID == "2") Value = Classes.Function.SumInWords.Ua.RuDateAndMoneyConverter.NumeralsToTxt((long)Convert.ToDouble(Value), Classes.Function.SumInWords.Ua.TextCase.Accusative, false, false);
                }

                //Для Цены и Суммы может быть разное к-во знаков после точки
                else if (pValue.IndexOf("Sum") > -1 || pValue.IndexOf("SUM") > -1)
                {
                    double dSum = Math.Round(Convert.ToDouble(Value), sysSettings.FractionalPartInSum, MidpointRounding.AwayFromZero);
                    Value = sQLiteDate.Return_DoobleToString(dSum.ToString());
                }
                else if (pValue.IndexOf("Price") > -1)
                {
                    //Value = String.Format("{0:0.00}", Convert.ToDouble(Value)).Replace(",", ".");
                    double dPrice = Math.Round(Convert.ToDouble(Value), sysSettings.FractionalPartInPrice, MidpointRounding.AwayFromZero);
                    Value = sQLiteDate.Return_DoobleToString(dPrice.ToString());

                }
            }
            catch { }

            return Value;
        }

        private string ErrorSymbolField(int i1, int i2, int i3, StringBuilder sb)
        {
            string sHtml = "";

            if ((i2 > 0 && i3 <= 0) || (i2 <= 0 && i3 > 0)) sHtml += "<BR><b>ОШИБКА!!!<BR>Порядковый номер в тексте для '[[['=" + i2.ToString() + " и для ']]]'=" + i3.ToString() + " не найдены!" + "</b>";
            else if ((i3 != -1 && i2 != -1) && (i3 <= i2)) sHtml += "<BR><b>ОШИБКА!!!<BR>Индекс для '[[['=" + i2.ToString() + "']]]' и для ']]]' " + i3.ToString() + "! </b>";
            else sHtml += sb.ToString().Substring(i1, sb.Length - i1);

            return sHtml;
        }
        private string ErrorSymbolConst(int i1, int i2, int i3, StringBuilder sb)
        {
            string sHtml = "";

            if ((i2 > 0 && i3 <= 0) || (i2 <= 0 && i3 > 0)) sHtml += "<BR><b>ОШИБКА!!!<BR>Порядковый номер в тексте для '{{{'=" + i2.ToString() + " и для '}}}'=" + i3.ToString() + " не найдены!" + "</b>";
            else if ((i3 != -1 && i2 != -1) && (i3 <= i2)) sHtml += "<BR><b>ОШИБКА!!!<BR>Индекс для '{{{'=" + i2.ToString() + "'}}}' и для '}}}' " + i3.ToString() + "! </b>";
            else sHtml += sb.ToString().Substring(i1, sb.Length - i1);

            return sHtml;
        }

        #endregion

    }
}