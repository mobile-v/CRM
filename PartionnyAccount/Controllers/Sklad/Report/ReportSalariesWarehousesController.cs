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
    public class ReportSalariesWarehousesController : ApiController
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

        int DirContractorIDOrg = 0, DirWarehouseID = 0;
        string DirContractorNameOrg, DirWarehouseName;
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightReportSalariesWarehouses"));
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

                DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateS", true) == 0).Value).ToString("yyyy-MM-dd 00:00:01"));
                if (DateS < Convert.ToDateTime("01.01.1800")) DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else
                {
                    //DateS = DateS.AddDays(-1);
                }

                DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (DatePo < Convert.ToDateTime("01.01.1800")) DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                DirContractorIDOrg = 0;
                bool bDirContractorIDOrg = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorIDOrg", true) == 0).Value, out DirContractorIDOrg);
                DirContractorNameOrg = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorNameOrg", true) == 0).Value;

                DirWarehouseID = 0;
                bool bDirWarehouseID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value, out DirWarehouseID);
                DirWarehouseName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseName", true) == 0).Value;

                #endregion



                #region  Доступ сотрудника к точкам
                //Для Админа - полный доступ
                //Для всех остальных - доступ только к точке на которой они являются Админами

                if (field.DirEmployeeID > 1 && DirWarehouseID > 0)
                {
                    //Если админ точки, то показать данные по ней
                    //иначе нет

                    var queryEW = await
                        (
                            from x in db.DirEmployeeWarehouse
                            where x.DirEmployeeID == field.DirEmployeeID && x.DirWarehouseID == DirWarehouseID
                            select x
                        ).ToListAsync();
                    if (queryEW.Count() > 0)
                    {
                        if (!queryEW[0].IsAdmin)
                        {
                            return Ok(returnServer.Return(false, "Вы не Администратор точки - у Вас нет доступа!"));
                        }
                    }
                    else
                    {
                        return Ok(returnServer.Return(false, "Вы не Администратор точки - у Вас нет доступа!"));
                    }
                }

                #endregion



                #region Для каждой точки подсчитываем Торговля (1), СЦ (2) и БУ(1)


                //Алгоритм
                //1. Получаем премии по точке

                //2. Получаем в ArrayList все промежуточные даты: [DateS, DatePo]
                //   Премии считаем по каждой дате отдельно

                //3. Формирование ЗП

                //3.1. Продажи за нал и безнал - 2-а поля: К-во * Расходная цена
                //3.2. СЦ за нал и безнал - 2-а поля: К-во * Расходная цена
                //3.3. БУ за нал и безнал - 2-а поля: К-во * Расходная цена
                //3.Х. Сумма: 3.1 + 3.2 + 3.3 (Касса)

                //4. Приход нал + безнал: К-во * Приходная цена

                //5. Закупки нал + безнал: К-во * Приходная цена

                // === === === === ===

                //6. ДР.РАСХОДЫ

                //7. БУ.[Списание аппаратов с ЗП]



                #region //1. Получаем премии по точке и Кассу + Банк

                int SalaryPercentTradeType = 0; double SalaryPercentTrade = 0;
                int SalaryPercentService1TabsType = 0; double SalaryPercentService1Tabs = 0;
                int SalaryPercentService2TabsType = 0; double SalaryPercentService2Tabs = 0;
                double 
                    SalaryPercentSecond = 0, SalaryPercent2Second = 0, SalaryPercent3Second = 0, SalaryPercent7Second = 0,
                    SalaryPercent4Second = 0, SalaryPercent5Second = 0, SalaryPercent6Second = 0, SalaryPercent8Second = 0;
                int DirCashOfficeID = 0, DirBankID = 0;


                var queryDirWarehouse = await
                    (
                        from x in db.DirWarehouses
                        where x.DirWarehouseID == DirWarehouseID
                        select x
                    ).ToListAsync();

                //Если выбран Сотрудник
                if (queryDirWarehouse.Count() > 0)
                {
                    DirCashOfficeID = queryDirWarehouse[0].DirCashOfficeID;
                    DirBankID = queryDirWarehouse[0].DirBankID;


                    SalaryPercentTradeType = queryDirWarehouse[0].SalaryPercentTradeType;
                    SalaryPercentTrade = queryDirWarehouse[0].SalaryPercentTrade;

                    SalaryPercentService1TabsType = queryDirWarehouse[0].SalaryPercentService1TabsType;
                    SalaryPercentService1Tabs = queryDirWarehouse[0].SalaryPercentService1Tabs;

                    SalaryPercentService2TabsType = queryDirWarehouse[0].SalaryPercentService2TabsType;
                    SalaryPercentService2Tabs = queryDirWarehouse[0].SalaryPercentService2Tabs;

                    //Точка продавшая аппарат === === ===
                    SalaryPercentSecond = queryDirWarehouse[0].SalaryPercentSecond;
                    SalaryPercent2Second = queryDirWarehouse[0].SalaryPercent2Second;
                    SalaryPercent3Second = queryDirWarehouse[0].SalaryPercent3Second;
                    SalaryPercent7Second = queryDirWarehouse[0].SalaryPercent7Second;
                    //Точка купившая аппарат === === ===
                    SalaryPercent4Second = queryDirWarehouse[0].SalaryPercent4Second;
                    SalaryPercent5Second = queryDirWarehouse[0].SalaryPercent5Second;
                    SalaryPercent6Second = queryDirWarehouse[0].SalaryPercent6Second;
                    SalaryPercent8Second = queryDirWarehouse[0].SalaryPercent8Second;

                }

                #endregion



                #region //2. Получаем в ArrayList все промежуточные даты: [DateS, DatePo]
                //           Премии считаем по каждой дате отдельно

                ArrayList alDate = new ArrayList();
                TimeSpan ts = DatePo - DateS;
                int differenceInDays = ts.Days;

                for (int i = 0; i < differenceInDays; i++)
                {
                    alDate.Add(DateS.AddDays(i));
                }
                alDate.Add(DatePo);

                #endregion



                #region //3. Формирование ЗП

                Models.Sklad.Doc.DocSalaryTabSQL2[] arrDocSalaryTabSQL = new Models.Sklad.Doc.DocSalaryTabSQL2[alDate.Count];

                if (DirWarehouseID > 0)
                {
                    for (int i = 0; i < alDate.Count; i++)
                    {

                        DateTime dtS = Convert.ToDateTime(((DateTime)alDate[i]).ToString("yyyy-MM-dd")); //ToString("yyyy-MM-dd 00:00:01"));
                        DateTime dtPo = Convert.ToDateTime(((DateTime)alDate[i]).ToString("yyyy-MM-dd 23:59:59"));


                        //1. Торговля
                        //Проблема обращения к таблицам "DocCashOfficeSums" и "DocBankSums": Мы не можем узнать приходные цены
                        //Решения проблемы:
                        //Получить приходную цену: 
                        //"DocCashOfficeSums.DocID => RemPartyMinuses.DocID => 
                        //RemPartyMinuses.RemPartyID => RemParties.RemPartyID => 
                        //RemParties.PriceCurrency"

                        //2. СЦ - !!! НЕ СДЕЛАЛ !!! Всё считает одной суммой!
                        //Надо определеить сумму запчасти или ремонт а в модуле финансы
                        //Запчасть считать отдельно
                        //Ремонт считать отдельно



                        #region //1.1. Продажи за нал и безнал - 2-а поля: К-во * Расходная цена

                        var queryTradeCash =
                            (
                                from x in db.DocCashOfficeSums
                                join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties
                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 //32-DocSales, 36-DocReturnsCustomers, 56-DocRetails, 57-DocRetailReturns
                                 (x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56) && //(x.doc.ListObjectID == 32 || x.doc.ListObjectID == 36 || x.doc.ListObjectID == 56 || x.doc.ListObjectID == 57) && 
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                //select x.DocCashOfficeSumSum
                                select yMinus.Quantity * yMinus.PriceCurrency - x.doc.Discount
                            ).DefaultIfEmpty(0).Sum();


                        var queryTradeBank =
                            (
                                from x in db.DocBankSums
                                join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 //32-DocSales, 36-DocReturnsCustomers, 56-DocRetails, 57-DocRetailReturns
                                 (x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56) && //(x.doc.ListObjectID == 32 || x.doc.ListObjectID == 36 || x.doc.ListObjectID == 56 || x.doc.ListObjectID == 57) &&
                                 (x.DirBankID == DirBankID)
                                //select x.DocBankSumSum
                                select yMinus.Quantity * yMinus.PriceCurrency - x.doc.Discount
                            ).DefaultIfEmpty(0).Sum();


                        #endregion


                        #region //1.2. СЦ за нал и безнал - 2-а поля: К-во * Расходная цена


                        var queryServiceCash =
                            (
                                from x in db.DocCashOfficeSums
                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select x.DocCashOfficeSumSum
                            ).DefaultIfEmpty(0).Sum();


                        var queryServiceBank =
                            (
                                from x in db.DocBankSums
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();


                        #endregion


                        #region //1.3. БУ за нал и безнал - 2-а поля: К-во * Расходная цена

                        var querySecondHandCash =
                            (
                                from x in db.DocCashOfficeSums
                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 66) && //(x.doc.ListObjectID == 65 || x.doc.ListObjectID == 66 || x.doc.ListObjectID == 67 || x.doc.ListObjectID == 68) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select x.DocCashOfficeSumSum
                            ).DefaultIfEmpty(0).Sum();


                        var querySecondHandBank =
                            (
                                from x in db.DocBankSums
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 66) && //(x.doc.ListObjectID == 65 || x.doc.ListObjectID == 66 || x.doc.ListObjectID == 67 || x.doc.ListObjectID == 68) &&
                                 (x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();


                        #endregion


                        #region //1.Х. Сумма: 3.1 + 3.2 + 3.3 (Касса)

                        //В "X. Data" суммируем

                        #endregion


                        #region //2. Приход нал + безнал: К-во * Приходная цена
                        //1. Торговля
                        //Проблема обращения к таблицам "DocCashOfficeSums" и "DocBankSums": Мы не можем узнать приходные цены
                        //Решения проблемы:
                        //Получить приходную цену: 
                        //"DocCashOfficeSums.DocID => RemPartyMinuses.DocID => 
                        //RemPartyMinuses.RemPartyID => RemParties.RemPartyID => 
                        //RemParties.PriceCurrency"


                        //Тут проблемка
                        //Товар кото



                        #region === RemParties === === === === ===


                        #region DocCashOfficeSums

                        //Все кроме СЦ
                        var queryPurchesSumCash11 =
                            (
                                from x in db.DocCashOfficeSums
                                join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56) && // || x.doc.ListObjectID == 67
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                //&& (x.DocID == yMinus.DocID && yMinus.RemPartyID == yPlus.RemPartyID)

                                select yMinus.Quantity * yPlus.PriceCurrency

                            ).DefaultIfEmpty(0).Sum();

                        //СЦ: 
                        var queryPurchesSumCash12 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                where
                                 (docServicePurches.DirServiceStatusID == 9) &&
                                 (x.DirCashOfficeSumTypeID == 15) &&
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                //&& (x.DocID == yMinus.DocID && yMinus.RemPartyID == yPlus.RemPartyID)

                                select yMinus.Quantity * yPlus.PriceCurrency

                            ).DefaultIfEmpty(0).Sum();


                        #endregion


                        #region DocBankSums

                        //Все кроме СЦ
                        var queryPurchesSumBank11 =
                            (
                                from x in db.DocBankSums
                                join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56) && // || x.doc.ListObjectID == 67
                                 (x.DirBankID == DirBankID)
                                 //&& (x.DocID == yMinus.DocID && yMinus.RemPartyID == yPlus.RemPartyID)

                                select yMinus.Quantity * yPlus.PriceCurrency

                            ).DefaultIfEmpty(0).Sum();


                        //СЦ: 
                        var queryPurchesSumBank12 =
                            (
                                from x in db.DocBankSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                where
                                 (docServicePurches.DirServiceStatusID == 9) &&
                                 (x.DirBankSumTypeID == 15) &&
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirBankID == DirBankID)
                                //&& (x.DocID == yMinus.DocID && yMinus.RemPartyID == yPlus.RemPartyID)

                                select yMinus.Quantity * yPlus.PriceCurrency

                            ).DefaultIfEmpty(0).Sum();

                        #endregion


                        #endregion




                        #region === Rem2Parties - 123456789 === === === === ===


                        #region DocCashOfficeSums - 123456789

                        /*
                        var queryPurchesSumCash2 =
                            (
                                from x in db.DocCashOfficeSums
                                join yMinus in db.Rem2PartyMinuses on x.DocID equals yMinus.DocID
                                join yPlus in db.Rem2Parties on yMinus.Rem2PartyID equals yPlus.Rem2PartyID

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.DirCashOfficeID == DirCashOfficeID) 

                                select yMinus.Quantity * yPlus.PriceCurrency

                            ).DefaultIfEmpty(0).Sum();
                        */
                        
                        var queryPurchesSumCash2 =
                            (
                                from x in db.DocCashOfficeSums
                                join y in db.DocSecondHandSales on x.DocID equals y.DocID
                                join z in db.DocSecondHandPurches on y.DocSecondHandPurchID equals z.DocSecondHandPurchID

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)

                                select z.PriceVAT + z.Sums
                            ).DefaultIfEmpty(0).Sum();

                        #endregion


                        #region DocBankSums - 123456789

                        /*
                        var queryPurchesSumBank2 =
                            (
                                from x in db.DocBankSums
                                join yMinus in db.Rem2PartyMinuses on x.DocID equals yMinus.DocID
                                join yPlus in db.Rem2Parties on yMinus.Rem2PartyID equals yPlus.Rem2PartyID

                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.DirBankID == DirBankID)

                                select yMinus.Quantity * yPlus.PriceCurrency

                            ).DefaultIfEmpty(0).Sum();
                        */

                        var queryPurchesSumBank2 =
                            (
                                from x in db.DocBankSums
                                join y in db.DocSecondHandSales on x.DocID equals y.DocID
                                join z in db.DocSecondHandPurches on y.DocSecondHandPurchID equals z.DocSecondHandPurchID

                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.DirBankID == DirBankID)

                                select z.PriceCurrency + z.Sums

                            ).DefaultIfEmpty(0).Sum();


                        #endregion


                        #endregion



                        #endregion


                        #region //3. Закупки нал + безнал: К-во * Приходная цена

                        var queryPurchesCash =
                            (
                                from x in db.DocCashOfficeSums
                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 6) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select x.DocCashOfficeSumSum
                            ).DefaultIfEmpty(0).Sum();


                        var queryPurchesBank =
                            (
                                from x in db.DocBankSums
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 6) &&
                                 (x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();

                        #endregion


                        #region //4. БУ за нал и безнал - 2-а поля: К-во * Расходная цена

                        var querySecondCashPurch =
                            (
                                from x in db.DocCashOfficeSums
                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 65 || x.doc.ListObjectID == 67 || x.doc.ListObjectID == 68) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select x.DocCashOfficeSumSum
                            ).DefaultIfEmpty(0).Sum();


                        var querySecondBankPurch =
                            (
                                from x in db.DocBankSums
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 65 || x.doc.ListObjectID == 67 || x.doc.ListObjectID == 68) &&
                                 (x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();


                        #endregion



                        //Хоз.расчёты

                        #region //5.1. Хоз.расчёты за нал и безнал (DirDomesticExpenses.DirDomesticExpenseType == 1)

                        var queryDomesticExpenseCash1 =
                            (
                                from x in db.DocCashOfficeSums
                                from y in db.DocDomesticExpenses
                                from z in db.DocDomesticExpenseTabs
                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 70) &&
                                 (x.doc.DocID == y.doc.DocID && y.DocDomesticExpenseID == z.DocDomesticExpenseID && z.dirDomesticExpense.DirDomesticExpenseType == 1) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select x.DocCashOfficeSumSum
                            ).DefaultIfEmpty(0).Sum();


                        var queryDomesticExpenseBank1 =
                            (
                                from x in db.DocBankSums
                                from y in db.DocDomesticExpenses
                                from z in db.DocDomesticExpenseTabs
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 70) &&
                                 (x.doc.DocID == y.doc.DocID && y.DocDomesticExpenseID == z.DocDomesticExpenseID && z.dirDomesticExpense.DirDomesticExpenseType == 1) &&
                                 (x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();

                        #endregion


                        #region //5.2. Хоз.расчёты за нал и безнал (DirDomesticExpenses.DirDomesticExpenseType == 2)

                        var queryDomesticExpenseCash2 =
                            (
                                from x in db.DocCashOfficeSums
                                from y in db.DocDomesticExpenses
                                from z in db.DocDomesticExpenseTabs
                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 70) &&
                                 (x.doc.DocID == y.doc.DocID && y.DocDomesticExpenseID == z.DocDomesticExpenseID && z.dirDomesticExpense.DirDomesticExpenseType == 2) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select x.DocCashOfficeSumSum
                            ).DefaultIfEmpty(0).Sum();


                        var queryDomesticExpenseBank2 =
                            (
                                from x in db.DocBankSums
                                from y in db.DocDomesticExpenses
                                from z in db.DocDomesticExpenseTabs
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 70) &&
                                 (x.doc.DocID == y.doc.DocID && y.DocDomesticExpenseID == z.DocDomesticExpenseID && z.dirDomesticExpense.DirDomesticExpenseType == 2) &&
                                 (x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();

                        #endregion


                        #region //5.3. Хоз.расчёты за нал и безнал (DirDomesticExpenses.DirDomesticExpenseType == 3)

                        var queryDomesticExpenseCash3 =
                            (
                                from x in db.DocCashOfficeSums
                                from y in db.DocDomesticExpenses
                                from z in db.DocDomesticExpenseTabs
                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 70) &&
                                 (x.doc.DocID == y.doc.DocID && y.DocDomesticExpenseID == z.DocDomesticExpenseID && z.dirDomesticExpense.DirDomesticExpenseType == 3) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select x.DocCashOfficeSumSum
                            ).DefaultIfEmpty(0).Sum();


                        var queryDomesticExpenseBank3 =
                            (
                                from x in db.DocBankSums
                                from y in db.DocDomesticExpenses
                                from z in db.DocDomesticExpenseTabs
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 70) &&
                                 (x.doc.DocID == y.doc.DocID && y.DocDomesticExpenseID == z.DocDomesticExpenseID && z.dirDomesticExpense.DirDomesticExpenseType == 3) &&
                                 (x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();

                        #endregion



                        //Инвентаризация

                        #region //6. Инвентаризация: Сумма цена розницы на списание - Сумма цены розницы на поступление

                        var queryInventorySum1 =
                            (
                                from docInventories in db.DocInventories

                                join docInventoryTabs1 in db.DocInventoryTabs on docInventories.DocInventoryID equals docInventoryTabs1.DocInventoryID into docInventoryTabs2
                                from docInventoryTabs in docInventoryTabs2.DefaultIfEmpty()

                                where
                                 (docInventories.DirWarehouseID == DirWarehouseID) &&
                                 (docInventories.doc.DocDate >= dtS && docInventories.doc.DocDate <= dtPo) &&
                                 (docInventories.doc.Held == true)

                                select new
                                {
                                    sum = (docInventoryTabs.Quantity_Purch * docInventoryTabs.PriceRetailCurrency) - (docInventoryTabs.Quantity_WriteOff * docInventoryTabs.PriceRetailCurrency)
                                }
                            ).ToList();

                        double dQueryInventorySum1 = 0;
                        if (queryInventorySum1.Count() > 0)
                        {
                            dQueryInventorySum1 = queryInventorySum1.Sum(x => x.sum);
                        }


                        #endregion



                        //Зарплата

                        #region 7. ЗП


                        #region //7.1. Торговля: SalaryPercentTrade

                        double sumSalaryPercentTrade = 0;

                        if (SalaryPercentTradeType == 1)
                        {
                            sumSalaryPercentTrade = ((queryTradeCash + queryTradeBank) * SalaryPercentTrade) / 100;
                        }
                        else if (SalaryPercentTradeType == 2)
                        {
                            //Все кроме СЦ
                            var queryPurchesSumCash11_1 =
                                (
                                    from x in db.DocCashOfficeSums
                                    join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                    join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                    where
                                     (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                     //32-DocSales, 36-DocReturnsCustomers, 56-DocRetails, 57-DocRetailReturns
                                     // || x.doc.ListObjectID == 56 || x.doc.ListObjectID == 57 - для Розницы возврат не работает
                                     (x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56) && //(x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56 || x.doc.ListObjectID == 67) &&
                                     (x.DirCashOfficeID == DirCashOfficeID)

                                    select yMinus.Quantity * (yMinus.PriceCurrency - yPlus.PriceCurrency) - x.doc.Discount

                                ).DefaultIfEmpty(0).Sum();

                            //Все кроме СЦ
                            var queryPurchesSumBank11_1 =
                                (
                                    from x in db.DocBankSums
                                    join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                    join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                    where
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                     //32-DocSales, 36-DocReturnsCustomers, 56-DocRetails, 57-DocRetailReturns
                                     // || x.doc.ListObjectID == 56 || x.doc.ListObjectID == 57 - для Розницы возврат не работает
                                     (x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56) && //(x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56 || x.doc.ListObjectID == 67) &&
                                     (x.DirBankID == DirBankID)

                                    select yMinus.Quantity * (yMinus.PriceCurrency - yPlus.PriceCurrency) - x.doc.Discount

                                ).DefaultIfEmpty(0).Sum();

                            sumSalaryPercentTrade = ((queryPurchesSumCash11_1 + queryPurchesSumBank11_1) * SalaryPercentTrade) / 100; 
                        }
                        else if (SalaryPercentTradeType == 3)
                        {
                            //Все кроме СЦ
                            var queryPurchesSumCash11_1 =
                                (
                                    from x in db.DocCashOfficeSums
                                    join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                    //join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                    where
                                         (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                         (x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56) && // || x.doc.ListObjectID == 67
                                         (x.DirCashOfficeID == DirCashOfficeID)

                                    select yMinus.RemPartyMinusID

                                ).Count();

                            //Все кроме СЦ
                            var queryPurchesSumBank11_1 =
                                (
                                    from x in db.DocBankSums
                                    join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                    //join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                    where
                                         (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                         (x.doc.ListObjectID == 32 || x.doc.ListObjectID == 56) && // || x.doc.ListObjectID == 67
                                         (x.DirBankID == DirBankID)

                                    select yMinus.RemPartyMinusID

                                ).Count();

                            sumSalaryPercentTrade = ((queryPurchesSumCash11_1 + queryPurchesSumBank11_1) * SalaryPercentTrade);
                        }

                        #endregion



                        #region //7.2. СЦ: SalaryPercentService1Tabs and SalaryPercentService2Tabs


                        #region Работы

                        double sumSalaryPercentService1Tabs = 0, sumSalaryPercentService1TabsCount = 0;

                        if (SalaryPercentService1TabsType == 1)
                        {
                            //Процент с суммы всех работ

                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select docServicePurch1Tabs.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();


                            var queryServiceBank_2 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                     (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirBankID == DirBankID)
                                    select docServicePurch1Tabs.PriceCurrency
                                ).DefaultIfEmpty(0).Sum();


                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs) / 100;
                        }
                        else if (SalaryPercentService1TabsType == 2)
                        {
                            //Фиксированная сумма за все работы в рамках одного ремонта
                            //Фиксированная сумма с каждого ремнта

                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) && (x.DirCashOfficeID == DirCashOfficeID) && (x.DirCashOfficeSumTypeID == 15) &&
                                 (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) 
                                select docServicePurches.DocServicePurchID
                            ).Distinct().Count();

                            //var queryServiceCash_2 = queryServiceCash_2_111.Distinct().Count();


                            var queryServiceBank_2 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) && (x.DirBankID == DirBankID) && (x.DirBankSumTypeID == 14) &&
                                     (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40)
                                    select docServicePurches.DocServicePurchID
                                ).Distinct().Count();

                            sumSalaryPercentService1TabsCount = queryServiceCash_2 + queryServiceBank_2;
                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs);
                        }
                        else if (SalaryPercentService1TabsType == 3)
                        {
                            //Фиксированная сумма за все работы в рамках одного ремонта
                            //Фиксированная сумма с каждого ремнта

                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirCashOfficeID == DirCashOfficeID)
                                select docServicePurches.DocServicePurchID
                            ).Count();


                            var queryServiceBank_2 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                     (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirBankID == DirBankID)
                                    select docServicePurches.DocServicePurchID
                                ).Count();

                            sumSalaryPercentService1TabsCount = queryServiceCash_2 + queryServiceBank_2;
                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs);
                        }

                        #endregion


                        #region Запчасти

                        double sumSalaryPercentService2Tabs = 0;

                        if (SalaryPercentService2TabsType == 1)
                        {
                            //Процент с продажи

                            var queryServiceCash_3 =
                                (
                                    from x in db.DocCashOfficeSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch2Tabs in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs.DocServicePurchID

                                    where
                                     (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                     (docServicePurch2Tabs.PayDate >= dtS && docServicePurch2Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirCashOfficeID == DirCashOfficeID)
                                    select docServicePurch2Tabs.PriceCurrency
                                ).DefaultIfEmpty(0).Sum();


                            var queryServiceBank_3 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch2Tabs in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                     (docServicePurch2Tabs.PayDate >= dtS && docServicePurch2Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirBankID == DirBankID)
                                    select docServicePurch2Tabs.PriceCurrency
                                ).DefaultIfEmpty(0).Sum();


                            sumSalaryPercentService2Tabs = ((queryServiceCash_3 + queryServiceBank_3) * SalaryPercentService2Tabs) / 100;
                        }
                        else if (SalaryPercentService2TabsType == 2)
                        {
                            //Процент с прибыли

                            //СЦ: 
                            var queryPurchesSumCash12_1 =
                                (
                                    from x in db.DocCashOfficeSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                    join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                    where
                                     (docServicePurches.DirServiceStatusID == 9) &&
                                     (x.DirCashOfficeSumTypeID == 15) &&
                                     (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirCashOfficeID == DirCashOfficeID)

                                    select yMinus.Quantity * (yMinus.PriceCurrency - yPlus.PriceCurrency)

                                ).DefaultIfEmpty(0).Sum();

                            var queryPurchesSumBank12_1 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join yMinus in db.RemPartyMinuses on x.DocID equals yMinus.DocID //from yMinus in db.RemPartyMinuses
                                    join yPlus in db.RemParties on yMinus.RemPartyID equals yPlus.RemPartyID //from yPlus in db.RemParties

                                    where
                                     (docServicePurches.DirServiceStatusID == 9) &&
                                     (x.DirBankSumTypeID == 15) &&
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirBankID == DirBankID)

                                    select yMinus.Quantity * (yMinus.PriceCurrency - yPlus.PriceCurrency)

                                ).DefaultIfEmpty(0).Sum();

                            sumSalaryPercentService2Tabs = ((queryPurchesSumCash12_1 + queryPurchesSumBank12_1) * SalaryPercentService2Tabs) / 100;
                        }
                        else if (SalaryPercentService2TabsType == 3)
                        {
                            //Фиксированная сумма за одну использованную к ремонту запчасти

                            var queryServiceCash_3 =
                                (
                                    from x in db.DocCashOfficeSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch2Tabs in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs.DocServicePurchID

                                    where
                                     (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                     (docServicePurch2Tabs.PayDate >= dtS && docServicePurch2Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirCashOfficeID == DirCashOfficeID)
                                    select docServicePurches.DocServicePurchID
                                ).Count();


                            var queryServiceBank_3 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch2Tabs in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                     (docServicePurch2Tabs.PayDate >= dtS && docServicePurch2Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirBankID == DirBankID)
                                    select docServicePurches.DocServicePurchID
                                ).Count();


                            sumSalaryPercentService2Tabs = ((queryServiceCash_3 + queryServiceBank_3) * SalaryPercentService2Tabs);
                        }

                        #endregion


                        #endregion



                        #region //7.3 БУ: 

                        double 
                            sumSalaryPercentSecond = 0, sumSalaryPercentSecond_Cash = 0, sumSalaryPercentSecond_Bank = 0,
                            sumSalaryPercent4Second = 0, sumSalaryPercent4Second_Cash = 0, sumSalaryPercent4Second_Bank = 0;
                        double 
                            sumSalaryPercent2Second = 0, sumSalaryPercent2Second_Cash = 0, sumSalaryPercent2Second_Bank = 0,
                            sumSalaryPercent5Second = 0, sumSalaryPercent5Second_Cash = 0, sumSalaryPercent5Second_Bank = 0;
                        double 
                            sumSalaryPercent3Second = 0,
                            sumSalaryPercent6Second = 0;
                        double
                            sumSalaryPercent7Second = 0, sumSalaryPercent7Second_Cash = 0, sumSalaryPercent7Second_Bank = 0,
                            sumSalaryPercent8Second = 0, sumSalaryPercent8Second_Cash = 0, sumSalaryPercent8Second_Bank = 0;

                        //Сумма продажи - Сумма покупки - Сумма работ - Сумма запчастей






                        //OLD === === === === === === === === === === === === === === === === === === === === === === === OLD

                        #region OLD

                        /*

                        #region Cash

                        if (SalaryPercentSecond > 0)
                        {

                            //Получение "DocSecondHandRetails.DocID"
                            var querySecondHandCash_X =
                                (
                                    from x in db.DocCashOfficeSums
                                    where
                                     (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                     (x.doc.ListObjectID == 66) && //(x.doc.ListObjectID == 65 || x.doc.ListObjectID == 66 || x.doc.ListObjectID == 67 || x.doc.ListObjectID == 68) &&
                                     (x.DirCashOfficeID == DirCashOfficeID) &&
                                     (x.DirCashOfficeSumTypeID == 23)
                                    select new
                                    {
                                        x.DocID,
                                        x.DocCashOfficeSumSum
                                    }
                                ).ToList();

                            //DocSecondHandRetails.DocID => DocSecondHandRetailTabs => RemParties => RemParties.DocID - этот "DocID" и будет линком на "DocSecondHandPurches"
                            //В "DocSecondHandPurches" содержатся все необходимые суммы
                            if (querySecondHandCash_X.Count > 0)
                            {
                                for (int j = 0; j < querySecondHandCash_X.Count; j++)
                                {
                                    int? DocID = querySecondHandCash_X[j].DocID;
                                    double DocCashOfficeSumSum = querySecondHandCash_X[j].DocCashOfficeSumSum;

                                    //Получаем "Цену продажи"
                                    var queryX1 =
                                        (
                                            from x in db.DocSecondHandSales
                                            join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                            where x.DocID == DocID
                                            select y
                                        ).ToList();

                                    if (queryX1.Count() > 0)
                                    {
                                        //sumSalaryPercentSecond_Cash += (DocCashOfficeSumSum - Convert.ToDouble(queryX1[0].PriceVAT)) * SalaryPercentSecond / 100;
                                        sumSalaryPercentSecond_Cash += (DocCashOfficeSumSum - Convert.ToDouble(queryX1[0].PriceVAT) - Convert.ToDouble(queryX1[0].Sums1) - Convert.ToDouble(queryX1[0].Sums2)) * SalaryPercentSecond / 100;
                                    }
                                }
                            }


                        }

                        //Фикс с каждого проданной единицы
                        if (SalaryPercent2Second > 0)
                        {
                            var querySecondCashPurch2 =
                                (
                                    from x in db.DocCashOfficeSums
                                    where
                                     (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                     (x.doc.ListObjectID == 66) &&
                                     (x.DirCashOfficeID == DirCashOfficeID)
                                    select x
                                ).Count();

                            if (querySecondCashPurch2 > 0)
                            {
                                sumSalaryPercent2Second_Cash += querySecondCashPurch2 * SalaryPercent2Second;
                            }
                        }

                        #endregion


                        #region Bank

                        //% с прибыли
                        if (SalaryPercentSecond > 0)
                        {
                            //Получение "DocSecondHandRetails.DocID"
                            var querySecondHandBank_X =
                            (
                                from x in db.DocBankSums
                                where
                                 (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                 (x.doc.ListObjectID == 66) && //(x.doc.ListObjectID == 65 || x.doc.ListObjectID == 66 || x.doc.ListObjectID == 67 || x.doc.ListObjectID == 68) &&
                                 (x.DirBankID == DirBankID) &&
                                 (x.DirBankSumTypeID == 21)
                                select new
                                {
                                    x.DocID,
                                    x.DocBankSumSum
                                }
                            ).ToList();

                            //DocSecondHandRetails.DocID => DocSecondHandRetailTabs => RemParties => RemParties.DocID - этот "DocID" и будет линком на "DocSecondHandPurches"
                            //В "DocSecondHandPurches" содержатся все необходимые суммы
                            if (querySecondHandBank_X.Count > 0)
                            {
                                for (int j = 0; j < querySecondHandBank_X.Count; j++)
                                {
                                    int? DocID = querySecondHandBank_X[j].DocID;
                                    double DocBankSumSum = querySecondHandBank_X[j].DocBankSumSum;

                                    //Получаем "docSecondHandPurches"
                                    var queryX1 =
                                        (
                                            from x in db.DocSecondHandSales
                                            join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                            where x.DocID == DocID
                                            select y
                                        ).ToList();

                                    if (queryX1.Count() > 0)
                                    {
                                        sumSalaryPercentSecond_Bank += (DocBankSumSum - Convert.ToDouble(queryX1[0].PriceVAT) - Convert.ToDouble(queryX1[0].Sums1) - Convert.ToDouble(queryX1[0].Sums2)) * SalaryPercentSecond / 100;
                                    }
                                }
                            }

                        }

                        //Фикс с каждого проданной единицы
                        if (SalaryPercent2Second > 0)
                        {
                            var querySecondBankPurch2 =
                                (
                                    from x in db.DocBankSums
                                    where
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) &&
                                     (x.doc.ListObjectID == 66) &&
                                     (x.DirBankID == DirBankID)
                                    select x
                                ).Count();

                            if (querySecondBankPurch2 > 0)
                            {
                                sumSalaryPercent2Second_Bank += querySecondBankPurch2 * SalaryPercent2Second;
                            }
                        }

                        #endregion

                        */

                        #endregion

                        //OLD === === === === === === === === === === === === === === === === === === === === === === === OLD






                        //NEW === === === === === === === === === === === === === === === === === === === === === === === NEW

                        #region 1. Точка продавшая аппарат

                        //1.1. % с прибыли
                        if (SalaryPercentSecond > 0)
                        {
                            var queryX1 =
                                (
                                    from x in db.DocSecondHandSales
                                    join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                    where (x.doc.DocDate >= dtS && x.doc.DocDate <= dtPo) && y.DirSecondHandStatusID == 10 && x.DirWarehouseID == DirWarehouseID
                                    select new
                                    {
                                        x,
                                        y
                                    }
                                ).ToList();

                            for (int i2 = 0; i2 < queryX1.Count(); i2++)
                            {
                                //Подсчёт чистой прибыли (процент от чистой прибыли)
                                sumSalaryPercentSecond_Cash += (Convert.ToDouble((queryX1[i2].x.PriceCurrency - queryX1[i2].x.doc.Discount) - queryX1[i2].y.PriceVAT - queryX1[i2].y.Sums1 - queryX1[i2].y.Sums2)) * SalaryPercentSecond / 100;
                            }
                        }

                        //1.2. Фикс с каждого проданной единицы
                        if (SalaryPercent2Second > 0)
                        {
                            var querySecondCashPurch2 =
                                (
                                    from x in db.DocSecondHandSales
                                    join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                    where (x.doc.DocDate >= dtS && x.doc.DocDate <= dtPo) && y.DirSecondHandStatusID == 10 && x.DirWarehouseID == DirWarehouseID
                                    select x
                                ).Count();

                            if (querySecondCashPurch2 > 0)
                            {
                                sumSalaryPercent2Second_Cash += querySecondCashPurch2 * SalaryPercent2Second;
                            }
                        }

                        //1.3. Фикс за отремонтированную единицу
                        if (SalaryPercent3Second > 0)
                        {
                            var querySecondCashPurch3 =
                                (
                                    from x in db.DocSecondHandPurches
                                    where
                                     (x.DateStatusChange >= dtS && x.DateStatusChange <= dtPo) &&
                                     (x.DirSecondHandStatusID == 9) && // && x.DirSecondHandStatusID == 7
                                     (x.doc.ListObjectID == 65) &&
                                     (x.DirWarehouseID == DirWarehouseID)
                                    select x
                                ).Count();

                            if (querySecondCashPurch3 > 0)
                            {
                                sumSalaryPercent3Second += querySecondCashPurch3 * SalaryPercent3Second;
                            }
                        }

                        //1.4. % от стоимости аппарата
                        if (SalaryPercent7Second > 0)
                        {
                            var queryX1 =
                                (
                                    from x in db.DocSecondHandSales
                                    join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                    where (x.doc.DocDate >= dtS && x.doc.DocDate <= dtPo) && y.DirSecondHandStatusID == 10 && x.DirWarehouseID == DirWarehouseID
                                    select new
                                    {
                                        x,
                                        y
                                    }
                                ).ToList();

                            for (int i2 = 0; i2 < queryX1.Count(); i2++)
                            {
                                //Подсчёт чистой прибыли (процент от чистой прибыли)
                                sumSalaryPercent7Second_Cash += (Convert.ToDouble(queryX1[i2].x.PriceCurrency - queryX1[i2].x.doc.Discount)) * SalaryPercent7Second / 100;
                            }
                        }

                        #endregion


                        #region 2. Точка купившая аппарат

                        //2.1. % с прибыли
                        if (SalaryPercent4Second > 0)
                        {
                            var queryX1 =
                                (
                                    from x in db.DocSecondHandSales
                                    join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                    where (x.doc.DocDate >= dtS && x.doc.DocDate <= dtPo) && y.DirSecondHandStatusID == 10 && y.DirWarehouseIDPurches == DirWarehouseID && y.DirWarehouseID != DirWarehouseID
                                    select new
                                    {
                                        x,
                                        y
                                    }
                                ).ToList();

                            for (int i2 = 0; i2 < queryX1.Count(); i2++)
                            {
                                //Подсчёт чистой прибыли (процент от чистой прибыли)
                                sumSalaryPercent4Second_Cash += (Convert.ToDouble((queryX1[i2].x.PriceCurrency - queryX1[i2].x.doc.Discount) - queryX1[i2].y.PriceVAT - queryX1[i2].y.Sums1 - queryX1[i2].y.Sums2)) * SalaryPercent4Second / 100;
                            }
                        }

                        //2.2. Фикс с каждого проданной единицы
                        if (SalaryPercent5Second > 0)
                        {
                            var querySecondCashPurch2 =
                                (
                                    from x in db.DocSecondHandSales
                                    join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                    where (x.doc.DocDate >= dtS && x.doc.DocDate <= dtPo) && y.DirSecondHandStatusID == 10 && y.DirWarehouseIDPurches == DirWarehouseID && y.DirWarehouseID != DirWarehouseID
                                    select x
                                ).Count();

                            if (querySecondCashPurch2 > 0)
                            {
                                sumSalaryPercent5Second_Cash += querySecondCashPurch2 * SalaryPercent5Second;
                            }
                        }

                        //2.3. Фикс за отремонтированную единицу
                        if (SalaryPercent6Second > 0)
                        {
                            var querySecondCashPurch3 =
                                (
                                    from x in db.DocSecondHandPurches
                                    where
                                     (x.DateStatusChange >= dtS && x.DateStatusChange <= dtPo) &&
                                     (x.DirSecondHandStatusID == 9) && // && x.DirSecondHandStatusID == 7
                                     (x.doc.ListObjectID == 65) &&
                                     (x.DirWarehouseIDPurches == DirWarehouseID && x.DirWarehouseID != DirWarehouseID)
                                    select x
                                ).Count();

                            if (querySecondCashPurch3 > 0)
                            {
                                sumSalaryPercent6Second += querySecondCashPurch3 * SalaryPercent6Second;
                            }
                        }

                        //2.4. % от стоимости аппарата
                        if (SalaryPercent8Second > 0)
                        {
                            var queryX1111 =
                                (
                                    from x in db.DocSecondHandSales
                                    join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                    where (x.doc.DocDate >= dtS && x.doc.DocDate <= dtPo) && y.DirSecondHandStatusID == 10 && y.DirWarehouseIDPurches == DirWarehouseID && y.DirWarehouseID != DirWarehouseID
                                    select new
                                    {
                                        x,
                                        y
                                    }
                                );

                            var queryX1 =
                                (
                                    from x in db.DocSecondHandSales
                                    join y in db.DocSecondHandPurches on x.DocSecondHandPurchID equals y.DocSecondHandPurchID
                                    where (x.doc.DocDate >= dtS && x.doc.DocDate <= dtPo) && y.DirSecondHandStatusID == 10 && y.DirWarehouseIDPurches == DirWarehouseID && y.DirWarehouseID != DirWarehouseID
                                    select new
                                    {
                                        x,
                                        y
                                    }
                                ).ToList();

                            for (int i2 = 0; i2 < queryX1.Count(); i2++)
                            {
                                //Подсчёт чистой прибыли (процент от чистой прибыли)
                                sumSalaryPercent8Second_Cash += (Convert.ToDouble(queryX1[i2].x.PriceCurrency - queryX1[i2].x.doc.Discount)) * SalaryPercent8Second / 100;
                            }
                        }

                        #endregion

                        //NEW === === === === === === === === === === === === === === === === === === === === === === === NEW






                        sumSalaryPercentSecond = sumSalaryPercentSecond_Cash + sumSalaryPercent4Second_Cash + sumSalaryPercent7Second_Cash; // + sumSalaryPercentSecond_Bank; 
                        sumSalaryPercent2Second = sumSalaryPercent2Second_Cash + sumSalaryPercent5Second_Cash + sumSalaryPercent8Second_Cash; // + sumSalaryPercent2Second_Bank;
                        sumSalaryPercent3Second = sumSalaryPercent3Second + sumSalaryPercent6Second;

                        #endregion



                        #region //7.4 БУ: Инвентаризация

                        double sumSecondHandInventory = 0;

                        //Получаем сумму (К-во*Цена аппарата) в инвентаризации за дату
                        var querySecondHandInventory = 
                            (
                                from x in db.DocSecondHandInvTabs
                                where 
                                 (x.docSecondHandInv.DirWarehouseID == DirWarehouseID) &&
                                 (x.docSecondHandInv.doc.DocDate >= dtS && x.docSecondHandInv.doc.DocDate <= dtPo) &&
                                 (x.docSecondHandInv.doc.Held == true) &&
                                 (x.Exist == 2) &&
                                 (x.docSecondHandInv.SpisatS == 1)
                                select x.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();

                        sumSecondHandInventory = querySecondHandInventory;

                        #endregion


                        #endregion





                        // === Отправка === === ===

                        #region X. Data

                        Models.Sklad.Doc.DocSalaryTabSQL2 docSalaryTab = new Models.Sklad.Doc.DocSalaryTabSQL2();

                        docSalaryTab.DocDate = (DateTime)alDate[i];

                        //Торговля *** *** ***
                        docSalaryTab.TradeCash = Math.Round(queryTradeCash, 2);
                        docSalaryTab.TradeBank = Math.Round(queryTradeBank, 2);
                        //СЦ *** *** ***
                        docSalaryTab.ServiceCash = Math.Round(queryServiceCash, 2);
                        docSalaryTab.ServiceBank = Math.Round(queryServiceBank, 2);
                        //БУ *** *** ***
                        docSalaryTab.SecondCash = Math.Round(querySecondHandCash, 2);
                        docSalaryTab.SecondBank = Math.Round(querySecondHandBank, 2);
                        //КАССА *** *** ***
                        docSalaryTab.TradeSumCashBank = Math.Round(queryTradeCash + queryTradeBank + queryServiceCash + queryServiceBank + querySecondHandCash + querySecondHandBank, 2);

                        //Себестоимость "Приходная цена" *** *** ***
                        docSalaryTab.PurchesSum = 
                            Math.Round(
                                Convert.ToDouble(
                                    queryPurchesSumCash11 + queryPurchesSumCash12 +
                                    queryPurchesSumBank11 + queryPurchesSumBank12 +

                                    queryPurchesSumCash2 + queryPurchesSumBank2
                                )
                            , 2);

                        //ЗАКУП (Приходная накладная на точку) *** *** ***
                        docSalaryTab.DocPurchesCashSum = queryPurchesCash;
                        docSalaryTab.DocPurchesBankSum = queryPurchesBank;

                        //Б/У закупки *** *** ***
                        docSalaryTab.SecondCashPurch = querySecondCashPurch;
                        docSalaryTab.SecondBankPurch = querySecondBankPurch;


                        //Хоз.расходы
                        docSalaryTab.DomesticExpenses = queryDomesticExpenseCash1 + queryDomesticExpenseBank1;
                        docSalaryTab.DomesticExpensesSalary = queryDomesticExpenseCash2 + queryDomesticExpenseBank2;
                        //Инкассация
                        docSalaryTab.Encashment = queryDomesticExpenseCash3 + queryDomesticExpenseBank3;

                        //Инвентаризация
                        docSalaryTab.InventorySum1 = dQueryInventorySum1;


                        //Зарплата
                        docSalaryTab.sumSalaryPercentTrade = Math.Round(sumSalaryPercentTrade, 2);
                        docSalaryTab.sumSalaryPercentService1Tabs = Math.Round(sumSalaryPercentService1Tabs, 2); docSalaryTab.sumSalaryPercentService1TabsCount = Math.Round(sumSalaryPercentService1TabsCount, 2);
                        docSalaryTab.sumSalaryPercentService2Tabs = Math.Round(sumSalaryPercentService2Tabs, 2);

                        docSalaryTab.sumSecondHandInventory = Math.Round(sumSecondHandInventory, 2);
                        docSalaryTab.sumSalaryPercent2Second = Math.Round(sumSalaryPercent2Second, 2);
                        docSalaryTab.sumSalaryPercent3Second = Math.Round(sumSalaryPercent3Second, 2);
                        docSalaryTab.sumSalaryPercentSecond = Math.Round(sumSalaryPercentSecond, 2);

                        docSalaryTab.SalatyProc = Math.Round(sumSalaryPercentTrade + sumSalaryPercentService1Tabs + sumSalaryPercentService2Tabs + sumSalaryPercentSecond + sumSalaryPercent2Second + sumSalaryPercent3Second - sumSecondHandInventory, 2);



                        arrDocSalaryTabSQL[i] = docSalaryTab;


                        #endregion
                    }
                }

                #endregion



                #endregion




                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = alDate.Count,
                    ReportSalariesWarehouses = arrDocSalaryTabSQL
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
