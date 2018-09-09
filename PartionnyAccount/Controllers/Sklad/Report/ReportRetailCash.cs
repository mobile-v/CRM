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
    public class ReportRetailCash
    {
        #region Classes

        Models.DbConnectionLogin dbMs = new Models.DbConnectionLogin("ConnStrMSSQL");
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Function.FunctionMSSQL.Jurn.JurnDispError jurnDispError = new Classes.Function.FunctionMSSQL.Jurn.JurnDispError();
        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();

        #endregion

        string pID = "";
        bool OnlySum = false;
        int pLanguage = 0;
        DateTime DateS, DatePo;

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db, int DirCustomersID, Classes.Account.Login.Field field)
        {
            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);
            DateS = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DateS"]).ToString("yyyy-MM-dd 00:00:00"));
            DatePo = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DatePo"]).ToString("yyyy-MM-dd 23:59:59"));

            OnlySum = false;
            bool bOnlySum = Boolean.TryParse(Request.Params["OnlySum"], out OnlySum);

            #endregion


            string ret = "";

            if (OnlySum) ret = await mOnlySum(db, field);
            else ret = await mAllReport(db, field);

            return ret;
        }

        private async Task<string> mOnlySum(DbConnectionSklad db, Classes.Account.Login.Field field)
        {
            string ret = "";

            double 
                Prifit_Warehouses_Bank = 0, Profit_Warehouses_Cash = 0,
                Prifit_DirEmployees_Bank = 0, Profit_DirEmployees_Cash = 0;


            double
                Касса = 0, Банк = 0,

                Продажи_Магазин_Касса = 0, Продажи_Магазин_Банк = 0,
                Продажи_Сотрудник_Касса = 0, Продажи_Сотрудник_Банк = 0,

                Возвраты_Магазину_Касса = 0, Возвраты_Магазину_Банк = 0,
                Возвраты_Сотруднику_Касса = 0, Возвраты_Сотруднику_Банк = 0,

                Выручка_Склад_Касса = 0, Выручка_Склад_Банк = 0,

                Выручка_Сотрудник_Касса = 0, Выручка_Сотрудник_Касса_Банк = 0;


            //Денег в Кассе и Банке

            #region 1. Получаем по сотруднику Банк и Кассу к которой он привязан
            //   Что бы по этой кассу и банку получить суммы: 
            //   DirCashOffices.DirCashOfficeSum
            //   and 
            //   DirBanks.DirBankSum

            int DirWarehouseID = 0, DirBankID = 0, DirCashOfficeID = 0;

            /*
            var query = await Task.Run(() =>
                (
                    from x in db.DirEmployees

                    join dirWarehouses1 in db.DirWarehouses on x.DirWarehouseID equals dirWarehouses1.DirWarehouseID into dirWarehouses2
                    from dirWarehouses in dirWarehouses2.DefaultIfEmpty()

                    where x.DirEmployeeID == field.DirEmployeeID
                    select new
                    {
                        DirBankID = dirWarehouses.DirBankID,
                        DirCashOfficeID = dirWarehouses.DirCashOfficeID,
                        DirWarehouseID = x.DirWarehouseID
                    }
                ).ToListAsync());

            if (query.Count() > 0)
            {
                DirBankID = query[0].DirBankID;
                DirCashOfficeID = query[0].DirCashOfficeID;
                DirWarehouseID = Convert.ToInt32(query[0].DirWarehouseID);
            }
            */

            #endregion


            #region 1.1. Сумма по кассе

            double DirBankSum = 0;

            var queryDirBankSum = await Task.Run(() =>
                (
                    from x in db.DirBanks
                    where x.DirBankID == DirBankID
                    select new
                    {
                        DirBankSum = x.DirBankSum
                    }
                 ).ToListAsync());

            if (queryDirBankSum.Count() > 0)
            {
                DirBankSum = queryDirBankSum[0].DirBankSum;
            }

            ret += "<b>Сумма по Банку:</b> " + DirBankSum + "<br />";
            Банк = DirBankSum;

            #endregion


            #region 1.2. Сумма по банку

            double DirCashOfficeSum = 0;

            var queryDirCashOffices = await Task.Run(() =>
                (
                    from x in db.DirCashOffices
                    where x.DirCashOfficeID == DirCashOfficeID
                    select new
                    {
                        DirCashOfficeSum = x.DirCashOfficeSum
                    }
                 ).ToListAsync());

            if (queryDirCashOffices.Count() > 0)
            {
                DirCashOfficeSum = queryDirCashOffices[0].DirCashOfficeSum;
            }

            ret += "<b>Сумма по Кассе:</b> " + DirCashOfficeSum + "<br />";
            Касса = DirCashOfficeSum;

            #endregion


            ret += "<br />";


            //Продажи

            #region 2.1. Продажи по Складу модуля Розницы
            /*
            var query_DocRetails_DirWarehouses = await Task.Run(() =>
                (
                    from x in db.DocRetails
                    where x.DirWarehouseID == DirWarehouseID && x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo
                    group x by new { x.doc.DirPaymentTypeID } into grp
                    select new
                    {
                        DirPaymentTypeID = grp.Key.DirPaymentTypeID,
                        PaymentSum = grp.Sum(z => z.doc.Payment)
                    }
                ).ToListAsync());


            foreach (var group in query_DocRetails_DirWarehouses)
            {
                if (group.DirPaymentTypeID == 1) { ret += "<b>Продажи по Магазину за наличные средства:</b> " + group.PaymentSum + "<br />"; Profit_Warehouses_Cash += group.PaymentSum; Продажи_Магазин_Касса = Profit_Warehouses_Cash; }
                else { ret += "<b>Продажи по Магазину за безналичные средства:</b> " + group.PaymentSum + "<br />"; Prifit_Warehouses_Bank += group.PaymentSum; Продажи_Магазин_Банк = Prifit_Warehouses_Bank; }
            }
            */
            #endregion


            #region 2.2. Продажи по Сотруднику моудял Розницы
            /*
            var query_DocRetails_DirEmployees = await Task.Run(() =>
                (
                    from x in db.DocRetails
                    where x.DirWarehouseID == DirWarehouseID && x.doc.DirEmployeeID == field.DirEmployeeID && x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo
                    group x by new { x.doc.DirPaymentTypeID } into grp
                    select new
                    {
                        DirPaymentTypeID = grp.Key.DirPaymentTypeID,
                        PaymentSum = grp.Sum(z => z.doc.Payment)
                    }
                ).ToListAsync());


            foreach (var group in query_DocRetails_DirEmployees)
            {
                if (group.DirPaymentTypeID == 1) { ret += "<b>Продажи по Сотруднику за наличные средства:</b> " + group.PaymentSum + "<br />"; Profit_DirEmployees_Cash += group.PaymentSum; Продажи_Сотрудник_Касса = Profit_DirEmployees_Cash; }
                else { ret += "<b>Продажи по Сотруднику за безналичные средства:</b> " + group.PaymentSum + "<br />"; Prifit_DirEmployees_Bank += group.PaymentSum; Продажи_Сотрудник_Банк = Prifit_DirEmployees_Bank; }
            }
            */
            #endregion


            ret += "<br />";


            //Возвраты

            #region 3.1. Возвраты по Складу модуля Розницы
            /*
            var query_DocRetailReturns_DirWarehouses = await Task.Run(() =>
                (
                    from x in db.DocRetailReturns
                    where x.DirWarehouseID == DirWarehouseID && x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo
                    group x by new { x.doc.DirPaymentTypeID } into grp
                    select new
                    {
                        DirPaymentTypeID = grp.Key.DirPaymentTypeID,
                        PaymentSum = grp.Sum(z => z.doc.Payment)
                    }
                ).ToListAsync());


            foreach (var group in query_DocRetailReturns_DirWarehouses)
            {
                if (group.DirPaymentTypeID == 1) { ret += "<b>Возвраты по Магазину за наличные средства:</b> " + group.PaymentSum + "<br />"; Profit_Warehouses_Cash -= group.PaymentSum; Возвраты_Магазину_Касса = Profit_Warehouses_Cash; }
                else { ret += "<b>Возвраты по Магазину за безналичные средства:</b> " + group.PaymentSum + "<br />"; Prifit_Warehouses_Bank -= group.PaymentSum; Возвраты_Магазину_Банк = Prifit_Warehouses_Bank; }
            }
            */
            #endregion


            #region 3.2. Возвраты по Сотруднику модуля Розницы
            /*
            var query_DocRetailReturns_DirEmployeeID = await Task.Run(() =>
                (
                    from x in db.DocRetailReturns
                    where x.DirWarehouseID == DirWarehouseID && x.doc.DirEmployeeID == field.DirEmployeeID && x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo
                    group x by new { x.doc.DirPaymentTypeID } into grp
                    select new
                    {
                        DirPaymentTypeID = grp.Key.DirPaymentTypeID,
                        PaymentSum = grp.Sum(z => z.doc.Payment)
                    }
                ).ToListAsync());


            foreach (var group in query_DocRetailReturns_DirEmployeeID)
            {
                if (group.DirPaymentTypeID == 1) { ret += "<b>Возвраты по Сотруднику за наличные средства:</b> " + group.PaymentSum + "<br />"; Profit_DirEmployees_Cash -= group.PaymentSum; Возвраты_Сотруднику_Касса = Profit_DirEmployees_Cash; }
                else { ret += "<b>Возвраты по Сотруднику за безналичные средства:</b> " + group.PaymentSum + "<br />"; Prifit_DirEmployees_Bank -= group.PaymentSum; Возвраты_Сотруднику_Банк = Prifit_DirEmployees_Bank; }
            }
            */
            #endregion


            ret += "<br />";


            //Выручка

            ret += "<b>Выручка по складу за наличные средства:</b> " + Profit_Warehouses_Cash + "<br />"; Выручка_Склад_Касса = Profit_Warehouses_Cash;
            ret += "<b>Выручка по складу за безналичные средства:</b> " + Prifit_Warehouses_Bank + "<br /><br />"; Выручка_Склад_Банк = Prifit_Warehouses_Bank;

            ret += "<b>Выручка по сотруднику за наличные средства:</b> " + Profit_DirEmployees_Cash + "<br />"; Выручка_Сотрудник_Касса = Profit_DirEmployees_Cash;
            ret += "<b>Выручка по сотруднику за безналичные средства:</b> " + Prifit_DirEmployees_Bank + "<br />"; Выручка_Сотрудник_Касса_Банк = Prifit_DirEmployees_Bank;

            //ret - не используем
            //ret2 - используем


            string ret2 =

                "<center>" +

                //Касса *** *** *** *** ***

                "<table border=1>" +

                //Меню ***

                " <tr>" +
                "  <th colspan='7'>Касса</th>" +
                " </tr>" +

                " <tr>" +
                "  <th rowspan='2'>Сумма</th>" +
                "  <th colspan='2'>Продажа</th>" +
                "  <th colspan='2'>Возврат</th>" +
                "  <th colspan='2'>Выручка</th>" +
                " </tr>" +

                " <tr>" +
                "  <th>Магазин</th>" +
                "  <th>Сотрудник</th>" +

                "  <th>Магазин</th>" +
                "  <th>Сотрудник</th>" +

                "  <th>Магазин</th>" +
                "  <th>Сотрудник</th>" +
                " </tr>" +

                //Данные ***

                " <tr>" +
                "  <td>" + Касса + "</td>" +
                "  <td>" + Продажи_Магазин_Касса + "</td>" +
                "  <td>" + Продажи_Сотрудник_Касса + "</td>" +
                "  <td>" + Возвраты_Магазину_Касса + "</td>" +
                "  <td>" + Возвраты_Сотруднику_Касса + "</td>" +
                "  <td>" + Выручка_Склад_Касса + "</td>" +
                "  <td>" + Выручка_Сотрудник_Касса + "</td>" +
                " </tr>" +

                "</table>" +

                "<br /><br />" +

                //Банк *** *** *** *** ***

                "<table border=1>" +

                //Меню ***

                " <tr>" +
                "  <th colspan='7'>Банк</th>" +
                " </tr>" +

                " <tr>" +
                "  <th rowspan='2'>Сумма</th>" +
                "  <th colspan='2'>Продажа</th>" +
                "  <th colspan='2'>Возврат</th>" +
                "  <th colspan='2'>Выручка</th>" +
                " </tr>" +

                " <tr>" +
                "  <th>Магазин</th>" +
                "  <th>Сотрудник</th>" +

                "  <th>Магазин</th>" +
                "  <th>Сотрудник</th>" +

                "  <th>Магазин</th>" +
                "  <th>Сотрудник</th>" +
                " </tr>" +

                //Данные ***

                " <tr>" +
                "  <td>" + Банк + "</td>" +
                "  <td>" + Продажи_Магазин_Банк + "</td>" +
                "  <td>" + Продажи_Сотрудник_Банк + "</td>" +
                "  <td>" + Возвраты_Магазину_Банк + "</td>" +
                "  <td>" + Возвраты_Сотруднику_Банк + "</td>" +
                "  <td>" + Выручка_Склад_Банк + "</td>" +
                "  <td>" + Выручка_Сотрудник_Касса_Банк + "</td>" +
                " </tr>" +

                "</table>" +

                "</center>";




            return ret2;
        }

        private async Task<string> mAllReport(DbConnectionSklad db, Classes.Account.Login.Field field)
        {
            string ret = "";


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