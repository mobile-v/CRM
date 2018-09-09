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
    public class ReportSalaries
    {
        string pID = "";
        int pLanguage = 0, DirContractorIDOrg = 0, DirEmployeeID = 0, DirMovementStatusID = 0, DocOrTab = 1;
        string DirContractorNameOrg, DirEmployeeName, DirMovementStatusName;
        DateTime DateS, DatePo;

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db)
        {
            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);
            

            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);
            DateS = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DateS"]).ToString("yyyy-MM-dd 00:00:00"));
            DatePo = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DatePo"]).ToString("yyyy-MM-dd 23:59:59"));

            DirContractorIDOrg = 0;
            bool bDirContractorIDOrg = Int32.TryParse(Request.Params["DirContractorIDOrg"], out DirContractorIDOrg);
            DirContractorNameOrg = Request.Params["DirContractorNameOrg"];

            DirEmployeeID = 0;
            bool bDirEmployeeID = Int32.TryParse(Request.Params["DirEmployeeID"], out DirEmployeeID);
            DirEmployeeName = Request.Params["DirEmployeeName"];
            
            #endregion


            //!!! Не менял код !!! Всё как в контролере "ReportSalariesController"
            #region Для каждого сутрудника подсчитываем ЗП, Премия1 и Премия2

            //Алгортм:
            //Получаем каждого сотрудника и параметры ЗП: ЗП + Тип, Премия1 и Премия2
            //1. ЗП вычисляем по типу. Пробегаем по каждому дню, если есть хоть одна продажа/ремонт, то засчитываем ему этот день
            //1.1. Для продаж смотрим таблицу "RemPartyMinuses"

            //Блядь!!!
            //1.
            //В таблицу "RemPartyMinuses" добавить поле "DirEmployeeID" и перенести всех сотрудников, в зависимости от типа документа:
            // - Если СС, то смотрим "DirEmployeeID" в таблице "DocServicePurch2Tabs"
            // - Во всех остальных случаях смотрим талицу "Doc"
            //А также во всех формах где задействована таблица "RemPartyMinuses" переносить "DirEmployeeID"
            //2.
            //Аналогично сделать для таблиц "RemParty"


            #region Получаем сотрудников

            var queryDirEmployees1 =
                (
                    from x in db.DirEmployees
                        //where x.DirEmployeeID == DirEmployeeID
                        select x
                );

            //Если выбран Сотрудник
            if (DirEmployeeID > 0)
            {
                queryDirEmployees1 = queryDirEmployees1.Where(x => x.DirEmployeeID == DirEmployeeID);
            }

            var queryDirEmployees = await queryDirEmployees1.ToListAsync();

            Models.Sklad.Doc.DocSalaryTabSQL[] arrDocSalaryTabSQL = new Models.Sklad.Doc.DocSalaryTabSQL[queryDirEmployees.Count()];

            #endregion


            if (queryDirEmployees.Count() > 0)
            {
                for (int i = 0; i < queryDirEmployees.Count(); i++)
                {
                    int CountDay = 0;

                    double
                        SumSalary = 0,
                        DirBonusIDSalary = 0,
                        DirBonus2IDSalary = 0,
                        SalaryFixedSalesMount = 0,
                        SumSalaryFixedServiceOne = 0,
                        //Б/У
                        DirBonus4IDSalary = 0,
                        DirBonus3IDSalary = 0,
                        SumSalaryFixedSecondHandRetailOne = 0,
                        SumSalaryFixedSecondHandWorkshopOne = 0;

                    int?
                        DirEmployeeID = queryDirEmployees[i].DirEmployeeID,
                        DirBonusID = queryDirEmployees[i].DirBonusID,
                        DirBonus2ID = queryDirEmployees[i].DirBonus2ID,
                        //Б/У
                        DirBonus4ID = queryDirEmployees[i].DirBonus4ID,
                        DirBonus3ID = queryDirEmployees[i].DirBonus3ID;




                    #region 1. SumSalary: К-во выходов в месяц


                    #region Зарплата за 1 месяц: если установили даты больше или меньше месяца (Salary, SalaryFixedSalesMount)
                    //Проблема в том что:
                    //В каждом месяце ЗП за 1 день будет разная:
                    //1. 28 дней в месяце
                    //2. 30 дней в месяце
                    //3. 31 дней в месяце

                    double
                        iDayDiv_SumSalary = 0, //Convert.ToDouble(queryDirEmployees[i].Salary), 
                        iDayDiv_SalaryFixedSalesMount = 0; //Convert.ToDouble(queryDirEmployees[i].SalaryFixedSalesMount);

                    //За 1 месяц 1000 рублей
                    //Период: [2016-11-20] - [2017-03-10]
                    //1. [2016-11-20] - [2016-11-30] //11 дней // 367 руб
                    //2. [2016-12-01] - [2016-12-31] //31 дней // 1000 руб
                    //3. [2017-01-01] - [2016-01-31] //31 дней // 1000 руб
                    //4. [2017-02-01] - [2016-02-28] //28 дней // 1000 руб
                    //5. [2017-03-01] - [2016-03-10] //10 дней // 323 руб

                    //Будем менять (ниже)
                    DateTime DateS2 = DateS;

                    //Сколько дней в периоде
                    TimeSpan ts = DatePo - DateS;

                    //for (int d = 0; d < ts.Days + 1; d++)
                    int d = 0;
                    while (true)
                    {
                        //Что бы выйти при d = 1000;
                        d++;

                        //Получили к-во дней в месяце
                        int iDay1 = System.DateTime.DaysInMonth(DateS2.Year, DateS2.Month);

                        //Если начинается День НЕ с "1" (30 - 20 + 1 = 11)
                        int iDay2 = 0;
                        if (DateS2.Year != DatePo.Year || DateS2.Month != DatePo.Month) iDay2 = iDay1 - DateS2.Day + 1;
                        else iDay2 = DatePo.Day - DateS2.Day + 1;

                        //Переопределяем DateS2
                        DateS2 = DateS2.AddDays(iDay2);

                        //Высчитываем сумму ЗП по каждому месяцу
                        iDayDiv_SumSalary += (Convert.ToDouble(queryDirEmployees[i].Salary) / iDay1) * iDay2;
                        iDayDiv_SalaryFixedSalesMount += (Convert.ToDouble(queryDirEmployees[i].SalaryFixedSalesMount) / iDay1) * iDay2;

                        //Выход
                        if (DateS2 >= DatePo || d == 1000) break;
                    }

                    #endregion



                    if (queryDirEmployees[i].SalaryDayMonthly == 1)
                    {
                        #region Продажи + Ремонты

                        //Продажи: RemPartyMinuses
                        //Продажи: DocServicePurch1Tabs


                        string SQL =
                            "SELECT DateX, SUM(CountX) AS CountX " +
                            "FROM " +
                            "  ( " +
                            "  SELECT date(DocDate) AS DateX, COUNT(date(DocDate)) AS CountX " +
                            "  FROM RemPartyMinuses " +
                            "  WHERE DirEmployeeID=@DirEmployeeID and (DocDate BETWEEN @DateS and @DatePo) " +
                            "  GROUP BY date(RemPartyMinuses.DocDate) " +
                            "  UNION " +
                            "  SELECT date(TabDate) AS DateX, COUNT(date(TabDate)) AS CountX " +
                            "  FROM DocServicePurch1Tabs " +
                            "  WHERE DirEmployeeID=@DirEmployeeID and (TabDate BETWEEN @DateS and @DatePo) " +
                            "  GROUP BY date(DocServicePurch1Tabs.TabDate) " +
                            "  ) AS X1 " +
                            "GROUP BY date(X1.DateX) ";

                        SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                        SQLiteParameter parDateS = new SQLiteParameter("@DateS", System.Data.DbType.Date) { Value = DateS.ToString("yyyy-MM-dd 00:00:00") };
                        SQLiteParameter parDatePo = new SQLiteParameter("@DatePo", System.Data.DbType.Date) { Value = DatePo.ToString("yyyy-MM-dd 23:59:59") };

                        //Сам запрос с параметрами
                        var query = db.Database.SqlQuery<Models.Sklad.Rem.RemPartyMinusSQL>(SQL, parDirEmployeeID, parDateS, parDatePo);

                        if (query.Count() > 0)
                        {
                            CountDay = query.Count();
                            SumSalary = query.Count() * Convert.ToDouble(queryDirEmployees[i].Salary);
                        }

                        #endregion
                    }
                    else
                    {
                        SumSalary = iDayDiv_SumSalary; // Convert.ToDouble(queryDirEmployees[i].Salary);
                    }

                    //ЗП фиксированная за месяц
                    if (queryDirEmployees[i].SalaryFixedSalesMount > 0)
                    {
                        SalaryFixedSalesMount = iDayDiv_SalaryFixedSalesMount; // Convert.ToDouble(queryDirEmployees[i].SalaryFixedSalesMount);
                    }

                    #endregion




                    // === Магазин === === ===

                    #region 1. DirBonusIDSalary: Премия с продаж

                    //Алгоритм:
                    //Получаем [Сумму продаж]
                    //По градации смотрим в какой диапазон попадает и берём процент
                    //[Сумма продаж] * [Процент]

                    if (DirBonusID > 0)
                    {
                        //1. Получаем сумму 
                        double querySum =
                            (
                                from x in db.RemPartyMinuses
                                where x.DirEmployeeID == DirEmployeeID && (x.DocDate >= DateS && x.DocDate <= DatePo)
                                select x.Quantity * x.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();


                        //2. Получаем градацию и в цикле пробегаем её
                        var queryBonus =
                            (
                                from x in db.DirBonusTabs
                                where x.DirBonusID == DirBonusID
                                select new
                                {
                                    SumBegin = x.SumBegin,
                                    Bonus = x.Bonus
                                }
                            ).OrderByDescending(o => o.SumBegin).ToList();

                        for (int j = 0; j < queryBonus.Count(); j++)
                        {
                            if (queryBonus[j].SumBegin <= querySum)
                            {
                                DirBonusIDSalary = (querySum / 100) * queryBonus[j].Bonus;
                                break;
                            }
                        }

                    }


                    #endregion


                    #region 2. DirBonus2IDSalary: Премия с ремонта

                    //Алгоритм:
                    //Получаем [Сумму продаж]
                    //По градации смотрим в какой диапазон попадает и берём процент
                    //[Сумма продаж] * [Процент]

                    if (DirBonus2ID > 0)
                    {
                        //1. Получаем сумму 
                        double querySum =
                            (
                                from x in db.DocServicePurch1Tabs
                                where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo)
                                select x.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();


                        //2. Получаем градацию и в цикле пробегаем её
                        var queryBonus =
                            (
                                from x in db.DirBonusTabs
                                where x.DirBonusID == DirBonus2ID
                                select new
                                {
                                    SumBegin = x.SumBegin,
                                    Bonus = x.Bonus
                                }
                            ).OrderByDescending(o => o.SumBegin).ToList();

                        for (int j = 0; j < queryBonus.Count(); j++)
                        {
                            if (queryBonus[j].SumBegin <= querySum)
                            {
                                DirBonus2IDSalary = (querySum / 100) * queryBonus[j].Bonus;
                                break;
                            }
                        }

                    }


                    //Ремонт: фиксированной суммы с каждого ремонта, скажем 300 руб
                    //Алгоритм:
                    //Получаем к-во ремонтов с сумой > "SalaryFixedServiceOne"
                    double? SalaryFixedServiceOne = queryDirEmployees[i].SalaryFixedServiceOne;
                    if (SalaryFixedServiceOne > 0)
                    {
                        string SQL =
                            /*
                                "SELECT COUNT(*) AS CountX " +
                                "FROM " +
                                "( " +
                                "  SELECT COUNT(*) AS CountX " +
                                "  FROM Docs, DocServicePurches, DocServicePurch1Tabs " +
                                "  WHERE " +
                                "   Docs.DocID = DocServicePurches.DocID and" +
                                "   DocServicePurches.DocServicePurchID = DocServicePurch1Tabs.DocServicePurchID and " +
                                "   DocServicePurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                "   DocServicePurch1Tabs.TabDate BETWEEN @DateS and @DatePo " +
                                "  GROUP BY DocServicePurches.DocServicePurchID " +
                                ")";
                            */
                            "  SELECT COUNT(*) AS CountX " +
                            "  FROM Docs, DocServicePurches, DocServicePurch1Tabs " +
                            "  WHERE " +
                            "   Docs.DocID = DocServicePurches.DocID and" +
                            "   DocServicePurches.DocServicePurchID = DocServicePurch1Tabs.DocServicePurchID and " +
                            "   DocServicePurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                            "   DocServicePurch1Tabs.TabDate BETWEEN @DateS and @DatePo " +
                            "  GROUP BY DocServicePurches.DocServicePurchID ";


                        SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                        SQLiteParameter parDateS = new SQLiteParameter("@DateS", System.Data.DbType.Date) { Value = DateS.ToString("yyyy-MM-dd 00:00:00") };
                        SQLiteParameter parDatePo = new SQLiteParameter("@DatePo", System.Data.DbType.Date) { Value = DatePo.ToString("yyyy-MM-dd 23:59:59") };

                        //Сам запрос с параметрами
                        var query = await db.Database.SqlQuery<Models.Sklad.Doc.DocServicePurchSQL>(SQL, parDirEmployeeID, parDateS, parDatePo).ToListAsync();

                        if (query.Count() > 0)
                        {
                            //CountDay = query.Count();
                            SumSalaryFixedServiceOne = Convert.ToDouble(query[0].CountX) * Convert.ToDouble(queryDirEmployees[i].SalaryFixedServiceOne);
                        }
                    }


                    #endregion




                    // === Б/У === === === 

                    #region 3. DirBonus3IDSalary: Премия с ремонта

                    //Алгоритм:
                    //Получаем [Сумму продаж]
                    //По градации смотрим в какой диапазон попадает и берём процент
                    //[Сумма продаж] * [Процент]

                    if (DirBonus3ID > 0)
                    {
                        double querySum = 0;

                        //1. Получаем сумму 
                        if (!queryDirEmployees[i].SalarySecondHandWorkshopCheck)
                        {
                            querySum =
                            (
                                from x in db.DocSecondHandPurch1Tabs
                                where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo)
                                select x.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();
                        }
                        else
                        {
                            querySum =
                            (
                                from x in db.DocSecondHandPurch1Tabs
                                from r in db.Rem2Parties

                                where
                                    x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo) &&
                                    x.docSecondHandPurch.doc.DocID == r.DocID && r.Remnant == 0

                                select x.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();
                        }

                        //2. Получаем градацию и в цикле пробегаем её
                        var queryBonus =
                            (
                                from x in db.DirBonusTabs
                                where x.DirBonusID == DirBonus3ID
                                select new
                                {
                                    SumBegin = x.SumBegin,
                                    Bonus = x.Bonus
                                }
                            ).OrderByDescending(o => o.SumBegin).ToList();

                        for (int j = 0; j < queryBonus.Count(); j++)
                        {
                            if (queryBonus[j].SumBegin <= querySum)
                            {
                                DirBonus3IDSalary = (querySum / 100) * queryBonus[j].Bonus;
                                break;
                            }
                        }

                    }


                    //Ремонт: фиксированной суммы с каждого ремонта, скажем 300 руб
                    //Алгоритм:
                    //Получаем к-во ремонтов с сумой > "SalaryFixedSecondHandWorkshopOne"
                    double? SalaryFixedSecondHandWorkshopOne = queryDirEmployees[i].SalaryFixedSecondHandWorkshopOne;
                    if (SalaryFixedSecondHandWorkshopOne > 0)
                    {
                        string SQL = "";

                        if (!queryDirEmployees[i].SalarySecondHandWorkshopCheck)
                        {
                            SQL =
                                "SELECT COUNT(*) AS CountX " +
                                "FROM Docs, DocSecondHandPurches, DocSecondHandPurch1Tabs " +
                                "WHERE " +
                                " Docs.DocID = DocSecondHandPurches.DocID and" +
                                " DocSecondHandPurches.DocSecondHandPurchID = DocSecondHandPurch1Tabs.DocSecondHandPurchID and " +
                                " DocSecondHandPurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                " DocSecondHandPurch1Tabs.TabDate BETWEEN @DateS and @DatePo " +
                                "GROUP BY DocSecondHandPurches.DocSecondHandPurchID ";
                        }
                        else
                        {
                            SQL =
                                "SELECT COUNT(*) AS CountX " +
                                "FROM Docs, DocSecondHandPurches, DocSecondHandPurch1Tabs, Rem2Parties " +
                                "WHERE " +
                                " Docs.DocID = DocSecondHandPurches.DocID and" +
                                " DocSecondHandPurches.DocSecondHandPurchID = DocSecondHandPurch1Tabs.DocSecondHandPurchID and " +
                                " DocSecondHandPurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                " DocSecondHandPurch1Tabs.TabDate BETWEEN @DateS and @DatePo and " +
                                " Docs.DocID = Rem2Parties.DocID and Rem2Parties.Remnant = 0 " + // !!! //
                                "GROUP BY DocSecondHandPurches.DocSecondHandPurchID ";
                        }


                        SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                        SQLiteParameter parDateS = new SQLiteParameter("@DateS", System.Data.DbType.Date) { Value = DateS.ToString("yyyy-MM-dd 00:00:00") };
                        SQLiteParameter parDatePo = new SQLiteParameter("@DatePo", System.Data.DbType.Date) { Value = DatePo.ToString("yyyy-MM-dd 23:59:59") };

                        //Сам запрос с параметрами
                        var query = await db.Database.SqlQuery<Models.Sklad.Doc.DocSecondHandPurchSQL>(SQL, parDirEmployeeID, parDateS, parDatePo).ToListAsync();

                        if (query.Count() > 0)
                        {
                            //CountDay = query.Count();
                            SumSalaryFixedSecondHandWorkshopOne = Convert.ToDouble(query[0].CountX) * Convert.ToDouble(queryDirEmployees[i].SalaryFixedSecondHandWorkshopOne);
                        }
                    }


                    #endregion


                    #region 4. DirBonus4IDSalary: Премия с продаж

                    //Алгоритм:
                    //Получаем [Сумму продаж]
                    //По градации смотрим в какой диапазон попадает и берём процент
                    //[Сумма продаж] * [Процент]

                    if (DirBonus4ID > 0)
                    {
                        //1. Получаем сумму 
                        double querySum =
                            (
                                from x in db.Rem2PartyMinuses
                                where x.DirEmployeeID == DirEmployeeID && (x.DocDate >= DateS && x.DocDate <= DatePo)
                                select x.Quantity * x.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();


                        //2. Получаем градацию и в цикле пробегаем её
                        var queryBonus =
                            (
                                from x in db.DirBonusTabs
                                where x.DirBonusID == DirBonus4ID
                                select new
                                {
                                    SumBegin = x.SumBegin,
                                    Bonus = x.Bonus
                                }
                            ).OrderByDescending(o => o.SumBegin).ToList();

                        for (int j = 0; j < queryBonus.Count(); j++)
                        {
                            if (queryBonus[j].SumBegin <= querySum)
                            {
                                DirBonus4IDSalary = (querySum / 100) * queryBonus[j].Bonus;
                                break;
                            }
                        }

                    }


                    //Продажа: фиксированной суммы с каждой продажи, скажем 300 руб
                    //Алгоритм:
                    //Получаем к-во продаж с сумой > "SalaryFixedSecondHandRetailOne"
                    double? SalaryFixedSecondHandRetailOne = queryDirEmployees[i].SalaryFixedSecondHandRetailOne;
                    if (SalaryFixedSecondHandRetailOne > 0)
                    {
                        string SQL =
                            /*
                            "  SELECT COUNT(*) AS CountX " +
                            "  FROM Docs, DocSecondHandPurches, DocSecondHandPurch1Tabs " +
                            "  WHERE " +
                            "   Docs.DocID = DocSecondHandPurches.DocID and" +
                            "   DocSecondHandPurches.DocSecondHandPurchID = DocSecondHandPurch1Tabs.DocSecondHandPurchID and " +
                            "   DocSecondHandPurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                            "   DocSecondHandPurch1Tabs.TabDate BETWEEN @DateS and @DatePo " +
                            "  GROUP BY DocSecondHandPurches.DocSecondHandPurchID ";
                            */
                            "  SELECT COUNT(*) AS CountX " +
                            "  FROM Docs, Rem2PartyMinuses " +
                            "  WHERE " +
                            "   Docs.DocID = Rem2PartyMinuses.DocID and" +
                            "   Rem2PartyMinuses.DirEmployeeID = @DirEmployeeID and " +
                            "   Docs.DocDate BETWEEN @DateS and @DatePo ";


                        SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                        SQLiteParameter parDateS = new SQLiteParameter("@DateS", System.Data.DbType.Date) { Value = DateS.ToString("yyyy-MM-dd 00:00:00") };
                        SQLiteParameter parDatePo = new SQLiteParameter("@DatePo", System.Data.DbType.Date) { Value = DatePo.ToString("yyyy-MM-dd 23:59:59") };

                        //Сам запрос с параметрами
                        var query = await db.Database.SqlQuery<Models.Sklad.Doc.DocSecondHandPurchSQL>(SQL, parDirEmployeeID, parDateS, parDatePo).ToListAsync();

                        if (query.Count() > 0)
                        {
                            //CountDay = query.Count();
                            SumSalaryFixedSecondHandRetailOne = Convert.ToDouble(query[0].CountX) * Convert.ToDouble(queryDirEmployees[i].SalaryFixedSecondHandRetailOne);
                        }
                    }


                    #endregion





                    #region 4. Data

                    string SalaryDayMonthlyName = "За день";
                    if (queryDirEmployees[i].SalaryDayMonthly == 2) SalaryDayMonthlyName = "За месяц";

                    string DirBonusName = ""; if (queryDirEmployees[i].dirBonus != null) DirBonusName = queryDirEmployees[i].dirBonus.DirBonusName;
                    string DirBonus2Name = ""; if (queryDirEmployees[i].dirBonus2 != null) DirBonus2Name = queryDirEmployees[i].dirBonus2.DirBonusName;
                    string DirBonus3Name = ""; if (queryDirEmployees[i].dirBonus3 != null) DirBonus3Name = queryDirEmployees[i].dirBonus3.DirBonusName;
                    string DirBonus4Name = ""; if (queryDirEmployees[i].dirBonus4 != null) DirBonus4Name = queryDirEmployees[i].dirBonus4.DirBonusName;


                    Models.Sklad.Doc.DocSalaryTabSQL docSalaryTab = new Models.Sklad.Doc.DocSalaryTabSQL();
                    docSalaryTab.DirEmployeeID = Convert.ToInt32(DirEmployeeID);
                    docSalaryTab.DirEmployeeName = queryDirEmployees[i].DirEmployeeName;

                    if (queryDirEmployees[i].DirCurrencyID != null)
                    {
                        docSalaryTab.DirCurrencyID = Convert.ToInt32(queryDirEmployees[i].DirCurrencyID);
                        docSalaryTab.DirCurrencyName = queryDirEmployees[i].dirCurrency.DirCurrencyName;
                        docSalaryTab.DirCurrencyRate = queryDirEmployees[i].dirCurrency.DirCurrencyRate;
                        docSalaryTab.DirCurrencyMultiplicity = queryDirEmployees[i].dirCurrency.DirCurrencyMultiplicity;
                    }
                    else
                    {
                        docSalaryTab.DirCurrencyID = 1;
                        docSalaryTab.DirCurrencyName = "???";
                        docSalaryTab.DirCurrencyRate = 1;
                        docSalaryTab.DirCurrencyMultiplicity = 1;
                    }

                    docSalaryTab.Salary = Math.Round(Convert.ToDouble(queryDirEmployees[i].Salary), 2);
                    docSalaryTab.CountDay = CountDay;
                    docSalaryTab.SalaryDayMonthly = Convert.ToInt32(queryDirEmployees[i].SalaryDayMonthly);
                    docSalaryTab.SalaryDayMonthlyName = SalaryDayMonthlyName;
                    docSalaryTab.SumSalary = Math.Round(SumSalary, 2);
                    docSalaryTab.SalaryFixedSalesMount = Math.Round(SalaryFixedSalesMount, 2);

                    docSalaryTab.DirBonusID = queryDirEmployees[i].DirBonusID;
                    docSalaryTab.DirBonusName = DirBonusName;
                    docSalaryTab.DirBonusIDSalary = Math.Round(DirBonusIDSalary, 2);

                    docSalaryTab.DirBonus2ID = queryDirEmployees[i].DirBonus2ID;
                    docSalaryTab.DirBonus2Name = DirBonus2Name;
                    docSalaryTab.DirBonus2IDSalary = Math.Round(DirBonus2IDSalary, 2);
                    docSalaryTab.SumSalaryFixedServiceOne = Math.Round(SumSalaryFixedServiceOne, 2);


                    //Б/У
                    docSalaryTab.DirBonus4ID = queryDirEmployees[i].DirBonus4ID;
                    docSalaryTab.DirBonus4Name = DirBonus4Name;
                    docSalaryTab.DirBonus4IDSalary = Math.Round(DirBonus4IDSalary, 2);
                    docSalaryTab.SumSalaryFixedSecondHandWorkshopOne = Math.Round(SumSalaryFixedSecondHandWorkshopOne, 2);

                    docSalaryTab.DirBonus3ID = queryDirEmployees[i].DirBonus3ID;
                    docSalaryTab.DirBonus3Name = DirBonus3Name;
                    docSalaryTab.DirBonus3IDSalary = Math.Round(DirBonus3IDSalary, 2);
                    docSalaryTab.SumSalaryFixedSecondHandRetailOne = Math.Round(SumSalaryFixedSecondHandRetailOne, 2);



                    docSalaryTab.Sums = Math.Round(SumSalary + DirBonusIDSalary + DirBonus2IDSalary + DirBonus4IDSalary + SumSalaryFixedSecondHandWorkshopOne + DirBonus3IDSalary + SumSalaryFixedSecondHandRetailOne + SalaryFixedSalesMount + SumSalaryFixedServiceOne, 2);

                    arrDocSalaryTabSQL[i] = docSalaryTab;


                    #endregion
                }
            }

            #endregion





            #region arrDocSalaryTabSQL[i] => Models.Sklad.Doc.DocSalaryTabSQL docSalaryTab

            string
                ret =
                "<center>" +
                "<h2>" + "Зарплата сотрудников " + DirEmployeeName + " (" + DateS.ToString("yyyy-MM-dd") + " по " + DatePo.ToString("yyyy-MM-dd") + ")</h2>";

            ret += ReportHeaderTab(pLanguage) + "</center>";

            double 
                SalaryS = 0, SumSalaryS = 0, SalaryFixedSalesMountS = 0, DirBonusIDSalaryS = 0, DirBonus2IDSalaryS = 0, SumSalaryFixedServiceOneS = 0,
                DirBonus4IDSalaryS = 0, SumSalaryFixedSecondHandWorkshopOneS = 0,
                DirBonus3IDSalaryS = 0, SumSalaryFixedSecondHandRetailOneS = 0,
                SumsS = 0;
            int CountDayS = 0;
            for (int i = 0; i < arrDocSalaryTabSQL.Length; i++)
            {
                Models.Sklad.Doc.DocSalaryTabSQL docSalaryTab = (Models.Sklad.Doc.DocSalaryTabSQL)arrDocSalaryTabSQL[i];

                SalaryS += docSalaryTab.Salary;
                CountDayS += docSalaryTab.CountDay;
                SumSalaryS += docSalaryTab.SumSalary;
                SalaryFixedSalesMountS += docSalaryTab.SalaryFixedSalesMount;
                DirBonusIDSalaryS += docSalaryTab.DirBonusIDSalary;
                DirBonus2IDSalaryS += docSalaryTab.DirBonus2IDSalary;
                SumSalaryFixedServiceOneS += docSalaryTab.SumSalaryFixedServiceOne;

                //Б/У

                DirBonus4IDSalaryS += docSalaryTab.DirBonus4IDSalary;
                SumSalaryFixedSecondHandWorkshopOneS += Convert.ToDouble(docSalaryTab.SumSalaryFixedSecondHandWorkshopOne);

                DirBonus3IDSalaryS += docSalaryTab.DirBonus3IDSalary;
                SumSalaryFixedSecondHandRetailOneS += Convert.ToDouble(docSalaryTab.SumSalaryFixedSecondHandRetailOne);

                SumsS += docSalaryTab.Sums;

                ret +=
                     "<TR>" + 

                    //Сотрудник
                    "<TD>" + docSalaryTab.DirEmployeeName + "</TD> " +

                    "<TD> </TD> " +

                    //Зарплата
                    "<TD>" + docSalaryTab.Salary + "</TD> " +
                    //Тип
                    "<TD>" + docSalaryTab.SalaryDayMonthlyName + "</TD> " +
                    //Дней
                    "<TD>" + docSalaryTab.CountDay + "</TD> " +
                    //Сумма
                    "<TD>" + docSalaryTab.SumSalary + "</TD> " +
                    //Сумма фикс.
                    "<TD>" + docSalaryTab.SalaryFixedSalesMount + "</TD> " +

                    "<TD> </TD> " +

                    //Премия (продавца)
                    "<TD>" + docSalaryTab.DirBonusName + "</TD> " +
                    //Сумма
                    "<TD>" + docSalaryTab.DirBonusIDSalary + "</TD> " +

                    "<TD> </TD> " +

                    //Премия (мастера)
                    "<TD>" + docSalaryTab.DirBonus2Name + "</TD> " +
                    //Сумма
                    "<TD>" + docSalaryTab.DirBonus2IDSalary + "</TD> " +
                    //За ремонт
                    "<TD>" + docSalaryTab.SumSalaryFixedServiceOne + "</TD> " +

                    "<TD> </TD> " +



                    //Б/У

                    //Премия (мастера)
                    "<TD>" + docSalaryTab.DirBonus4Name + "</TD> " +
                    //Сумма
                    "<TD>" + docSalaryTab.DirBonus4IDSalary + "</TD> " +
                    //За ремонт
                    "<TD>" + docSalaryTab.SumSalaryFixedSecondHandWorkshopOne + "</TD> " +

                    "<TD> </TD> " +

                    //Премия (мастера)
                    "<TD>" + docSalaryTab.DirBonus3Name + "</TD> " +
                    //Сумма
                    "<TD>" + docSalaryTab.DirBonus3IDSalary + "</TD> " +
                    //За ремонт
                    "<TD>" + docSalaryTab.SumSalaryFixedSecondHandRetailOne + "</TD> " +

                    "<TD> </TD> " +




                    //Общая
                    "<TD>" + docSalaryTab.Sums + "</TD> " +

                    "</TR>";
            }


            ret +=
                 "<TR>" +

                //Сотрудник
                "<TD><b> Итого </b></TD> " +

                "<TD> </TD> " +

                //Зарплата
                "<TD><b>" + SalaryS + "</b></TD> " +
                //Тип
                "<TD></TD> " +
                //Дней
                "<TD><b>" + CountDayS + "</b></TD> " +
                //Сумма
                "<TD><b>" + SumSalaryS + "</b></TD> " +
                //Сумма фикс.
                "<TD><b>" + SalaryFixedSalesMountS + "</b></TD> " +

                "<TD> </TD> " +

                //Премия (продавца)
                "<TD></TD> " +
                //Сумма
                "<TD><b>" + DirBonusIDSalaryS + "</b></TD> " +

                "<TD> </TD> " +

                //Премия (мастера)
                "<TD></TD> " +
                //Сумма
                "<TD><b>" + DirBonus2IDSalaryS + "</b></TD> " +
                //За ремонт
                "<TD><b>" + SumSalaryFixedServiceOneS + "</b></TD> " +

                "<TD> </TD> " +


                //Б/У

                //Премия (мастера)
                "<TD></TD> " +
                //Сумма
                "<TD><b>" + DirBonus4IDSalaryS + "</b></TD> " +
                //За ремонт
                "<TD><b>" + SumSalaryFixedSecondHandWorkshopOneS + "</b></TD> " +

                "<TD> </TD> " +

                //Премия (мастера)
                "<TD></TD> " +
                //Сумма
                "<TD><b>" + DirBonus3IDSalaryS + "</b></TD> " +
                //За ремонт
                "<TD><b>" + SumSalaryFixedSecondHandRetailOneS + "</b></TD> " +




                "<TD> </TD> " +

                //Общая
                "<TD><b>" + SumsS + "</b></TD> " +

                "</TR>";


            ret += "</TABLE>"; //"К-во Суотрудников: " + arrDocSalaryTabSQL.Length;

            #endregion




            return ret;
        }



        //СТАНДАРТНАЯ табличная часть
        private string ReportHeaderTab(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Сотрудник</TD> <TD class='table_header'>X</TD> <TD class='table_header'>Зарплата</TD> <TD class='table_header'>Тип</TD> <TD class='table_header'>Дней</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Сумма фикс.</TD> <TD class='table_header'>X</TD> <TD class='table_header'>Премия (продавца)</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>X</TD> <TD class='table_header'>Премия (мастера)</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>За ремонт</TD> " +
                    "<TD class='table_header'>X</TD> <TD class='table_header'>Премия (маст)</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>За ремонт</TD> " +
                    "<TD class='table_header'>X</TD> <TD class='table_header'>Премия (прод)</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>За прод</TD> " +
                    " <TD class='table_header'>X</TD> <TD class='table_header'>Общая</TD> " +
                    "</TR>";

                default:
                    return
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Сотрудник</TD> <TD class='table_header'>X</TD> <TD class='table_header'>Зарплата</TD> <TD class='table_header'>Тип</TD> <TD class='table_header'>Дней</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Сумма фикс.</TD> <TD class='table_header'>X</TD> <TD class='table_header'>Премия (продавца)</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>X</TD> <TD class='table_header'>Премия (мастера)</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>За ремонт</TD> " +
                    "<TD class='table_header'>X</TD> <TD class='table_header'>Премия (маст)</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>За ремонт</TD> " +
                    "<TD class='table_header'>X</TD> <TD class='table_header'>Премия (прод)</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>За прод</TD> " +
                    "<TD class='table_header'>X</TD> <TD class='table_header'>Общая</TD> " +
                    "</TR>";
            }
        }


    }
}