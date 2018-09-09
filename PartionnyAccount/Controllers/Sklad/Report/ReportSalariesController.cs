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
    public class ReportSalariesController : ApiController
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

        int DirContractorIDOrg = 0, DirEmployeeID = 0, ReportType = 1;
        string DirContractorNameOrg, DirEmployeeName;
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightReportSalaries"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                bool bRight = await Task.Run(() => accessRight.AccessCheck(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightReportSalariesEmplCheck"));

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
                
                DirEmployeeID = 0;
                bool bDirEmployeeID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value, out DirEmployeeID);
                DirEmployeeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeName", true) == 0).Value;
                if (!bRight) { DirEmployeeID = field.DirEmployeeID; }

                ReportType = 0;
                bool bReportType = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportType", true) == 0).Value, out ReportType);
                if (ReportType == 0) ReportType = 1;
                string DocServicePurch1Tabs_XDate = "TabDate";
                if(ReportType==2) DocServicePurch1Tabs_XDate = "PayDate";


                #endregion




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


                //!!! ПРАВА !!!
                //Варианты:
                //1) Администратор (field.DirEmployeeID == 1) - всё видит
                //2) Администратор точки - видит только сотрудников точки, которой он Админ
                //3) Юзер - видит только себя
                if (field.DirEmployeeID > 1)
                {
                    //Получаем точки на которых сотрудник Админ
                    var query_WarIsAdminList =
                        (
                            from o1 in db.DirEmployeeWarehouse
                            where o1.DirEmployeeID == field.DirEmployeeID && o1.IsAdmin == true
                            select o1.DirWarehouseID
                        );

                    //Получаем всех сотрудников на точках queryIsAdmin
                    var query_EmpList =
                        (
                            from o2 in db.DirEmployeeWarehouse
                            where query_WarIsAdminList.Contains(o2.DirWarehouseID)
                            select o2.DirEmployeeID
                        );

                    queryDirEmployees1 = queryDirEmployees1.Where(x => query_EmpList.Contains(x.DirEmployeeID));
                }


                //Если выбран Сотрудник
                if (DirEmployeeID > 0)
                {
                    queryDirEmployees1 = queryDirEmployees1.Where(x => x.DirEmployeeID == DirEmployeeID);
                }


                var queryDirEmployees = await queryDirEmployees1.ToListAsync();

                //Не админ ни одной точки
                if (queryDirEmployees.Count() == 0)
                {
                    queryDirEmployees = await db.DirEmployees.Where(x => x.DirEmployeeID == field.DirEmployeeID).ToListAsync();
                }


                //Массив отправляется в грид клиенту
                //Models.Sklad.Doc.DocSalaryTabSQL[] arrDocSalaryTabSQL = new Models.Sklad.Doc.DocSalaryTabSQL[queryDirEmployees.Count()];
                ArrayList alDate = new ArrayList();
                Models.Sklad.Doc.DocSalaryTabSQL[] arrDocSalaryTabSQL = null;
                if (queryDirEmployees.Count() > 1)
                {
                    arrDocSalaryTabSQL = new Models.Sklad.Doc.DocSalaryTabSQL[queryDirEmployees.Count()];
                }
                else if (queryDirEmployees.Count() == 1)
                {
                    #region Получаем в ArrayList все промежуточные даты: [DateS, DatePo]
                    //      Премии считаем по каждой дате отдельно

                    TimeSpan ts = DatePo - DateS;
                    int differenceInDays = ts.Days;

                    for (int i = 0; i < differenceInDays; i++)
                    {
                        alDate.Add(DateS.AddDays(i));
                    }
                    alDate.Add(DatePo);

                    #endregion

                    arrDocSalaryTabSQL = new Models.Sklad.Doc.DocSalaryTabSQL[alDate.Count];
                }

                #endregion


                if (queryDirEmployees.Count() > 1)
                {

                    #region Более 1 сотрудника

                    
                    for (int i = 0; i < queryDirEmployees.Count(); i++)
                    {

                        #region var

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


                        //NEW
                        int SalaryPercentService1TabsType = Convert.ToInt32(queryDirEmployees[i].SalaryPercentService1TabsType); double SalaryPercentService1Tabs = Convert.ToInt32(queryDirEmployees[i].SalaryPercentService1Tabs);
                        int SalaryPercentService2TabsType = Convert.ToInt32(queryDirEmployees[i].SalaryPercentService2TabsType); double SalaryPercentService2Tabs = Convert.ToInt32(queryDirEmployees[i].SalaryPercentService2Tabs);

                        #endregion


                        // === Salary === === ===

                        #region 0. SumSalary: К-во выходов в месяц


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
                                "  SELECT date(RemPartyMinuses.DocDate) AS DateX, COUNT(date(RemPartyMinuses.DocDate)) AS CountX " +
                                "  FROM RemPartyMinuses, Docs " +
                                "  WHERE RemPartyMinuses.DocID=Docs.DocID and RemPartyMinuses.DirEmployeeID=@DirEmployeeID and (Docs.ListObjectID == 56 or Docs.ListObjectID == 32) and (RemPartyMinuses.DocDate BETWEEN @DateS and @DatePo) " + //"  WHERE DirEmployeeID=@DirEmployeeID and (DocDate BETWEEN @DateS and @DatePo) " +
                                "  GROUP BY date(RemPartyMinuses.DocDate) " +
                                "  UNION " +
                                "  SELECT date(" + DocServicePurch1Tabs_XDate + ") AS DateX, COUNT(date(" + DocServicePurch1Tabs_XDate + ")) AS CountX " +
                                "  FROM DocServicePurch1Tabs " +
                                "  WHERE DocServicePurch1Tabs.DirEmployeeID=@DirEmployeeID and (" + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo) " +
                                "  GROUP BY date(DocServicePurch1Tabs." + DocServicePurch1Tabs_XDate + ") " +
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
                                    where x.DirEmployeeID == DirEmployeeID && (x.doc.ListObjectID == 56 || x.doc.ListObjectID == 32) && (x.DocDate >= DateS && x.DocDate <= DatePo)
                                    select x.Quantity * x.PriceCurrency - x.doc.Discount
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


                        // === СЦ === === ===

                        #region 2. DirBonus2IDSalary: Премия с ремонта

                        //Алгоритм:
                        //Получаем [Сумму продаж]
                        //По градации смотрим в какой диапазон попадает и берём процент
                        //[Сумма продаж] * [Процент]
                        /*
                        if (DirBonus2ID > 0)
                        {
                            //1. Получаем сумму 
                            double querySum = 0;

                            if (ReportType == 1)
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo)
                                        select x.PriceCurrency
                                    );
                                querySum = querySumX.DefaultIfEmpty(0).Sum();
                            }
                            else
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= DateS && x.PayDate <= DatePo)
                                        select x.PriceCurrency
                                    );
                                querySum = querySumX.DefaultIfEmpty(0).Sum();
                            }


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
                                "SELECT COUNT(*)  AS CountX " +
                                "FROM Docs, DocServicePurches, DocServicePurch1Tabs " +
                                "WHERE " +
                                " Docs.DocID = DocServicePurches.DocID and" +
                                " DocServicePurches.DocServicePurchID = DocServicePurch1Tabs.DocServicePurchID and " +
                                " DocServicePurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                " DocServicePurch1Tabs." + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo ";
                            //"GROUP BY DocServicePurches.DocServicePurchID ";


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
                        */

                        #endregion


                        //!!! Новый алгоритм !!!
                        #region //7.2. СЦ: SalaryPercentService1Tabs and SalaryPercentService2Tabs


                        #region Работы

                        double sumSalaryPercentService1Tabs = 0, sumSalaryPercentService1TabsCount = 0;

                        if (SalaryPercentService1TabsType == 1)
                        {
                            //Процент с суммы всех работ
                            /*
                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo) &&
                                 (docServicePurch1Tabs.PayDate >= DateS && docServicePurch1Tabs.PayDate <= DatePo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
                                select docServicePurch1Tabs.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();


                            var queryServiceBank_2 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo) &&
                                     (docServicePurch1Tabs.PayDate >= DateS && docServicePurch1Tabs.PayDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)
                                    select docServicePurch1Tabs.PriceCurrency
                                ).DefaultIfEmpty(0).Sum();


                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs) / 100;
                            */

                            // *** *** ***


                            double querySum = 0;
                            if (ReportType == 1)
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo)
                                        select x.PriceCurrency
                                    );
                                querySum = querySumX.DefaultIfEmpty(0).Sum();
                            }
                            else
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= DateS && x.PayDate <= DatePo)
                                        select x.PriceCurrency
                                    );
                                querySum = querySumX.DefaultIfEmpty(0).Sum();
                            }

                            sumSalaryPercentService1Tabs = (querySum * SalaryPercentService1Tabs) / 100;

                        }
                        else if (SalaryPercentService1TabsType == 2)
                        {
                            //Фиксированная сумма за все работы в рамках одного ремонта
                            /*
                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo) && (x.DirCashOfficeSumTypeID == 15) && (x.DirEmployeeID == DirEmployeeID) && // && (x.DirCashOfficeID == DirCashOfficeID)
                                 (docServicePurch1Tabs.PayDate >= DateS && docServicePurch1Tabs.PayDate <= DatePo) &&
                                 (x.doc.ListObjectID == 40)
                                select docServicePurches.DocServicePurchID
                            ).Distinct().Count();


                            var queryServiceBank_2 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo) && (x.DirBankSumTypeID == 14) && (x.DirEmployeeID == DirEmployeeID) && // && (x.DirBankID == DirBankID)
                                     (docServicePurch1Tabs.PayDate >= DateS && docServicePurch1Tabs.PayDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40)
                                    select docServicePurches.DocServicePurchID
                                ).Distinct().Count();

                            sumSalaryPercentService1TabsCount = queryServiceCash_2 + queryServiceBank_2;
                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs);
                            */

                            // *** *** ***


                            double queryCount = 0;
                            if (ReportType == 1)
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo)
                                        select x.DocServicePurchID
                                    );
                                queryCount = querySumX.Distinct().Count();
                            }
                            else
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= DateS && x.PayDate <= DatePo)
                                        select x.DocServicePurchID
                                    );
                                queryCount = querySumX.Distinct().Count();
                            }

                            sumSalaryPercentService1TabsCount = queryCount;
                            sumSalaryPercentService1Tabs = (queryCount * SalaryPercentService1Tabs);

                        }
                        else if (SalaryPercentService1TabsType == 3)
                        {
                            //Фиксированная сумма с каждого ремонта
                            /*
                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo) &&
                                 (docServicePurch1Tabs.PayDate >= DateS && docServicePurch1Tabs.PayDate <= DatePo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
                                select docServicePurches.DocServicePurchID
                            ).Count();


                            var queryServiceBank_2 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo) &&
                                     (docServicePurch1Tabs.PayDate >= DateS && docServicePurch1Tabs.PayDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)
                                    select docServicePurches.DocServicePurchID
                                ).Count();

                            sumSalaryPercentService1TabsCount = queryServiceCash_2 + queryServiceBank_2;
                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs);
                            */

                            // *** *** ***


                            double queryCount = 0;
                            if (ReportType == 1)
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo)
                                        select x.DocServicePurchID
                                    );
                                queryCount = querySumX.Count();
                            }
                            else
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= DateS && x.PayDate <= DatePo)
                                        select x.DocServicePurchID
                                    );
                                queryCount = querySumX.Count();
                            }

                            sumSalaryPercentService1TabsCount = queryCount;
                            sumSalaryPercentService1Tabs = (queryCount * SalaryPercentService1Tabs);
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
                                     (x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo) &&
                                     (docServicePurch2Tabs.PayDate >= DateS && docServicePurch2Tabs.PayDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
                                    select docServicePurch2Tabs.PriceCurrency
                                ).DefaultIfEmpty(0).Sum();


                            var queryServiceBank_3 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch2Tabs in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo) &&
                                     (docServicePurch2Tabs.PayDate >= DateS && docServicePurch2Tabs.PayDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)
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
                                     (x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)

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
                                     (x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)

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
                                     (x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo) &&
                                     (docServicePurch2Tabs.PayDate >= DateS && docServicePurch2Tabs.PayDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
                                    select docServicePurches.DocServicePurchID
                                ).Count();


                            var queryServiceBank_3 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch2Tabs in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo) &&
                                     (docServicePurch2Tabs.PayDate >= DateS && docServicePurch2Tabs.PayDate <= DatePo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)
                                    select docServicePurches.DocServicePurchID
                                ).Count();


                            sumSalaryPercentService2Tabs = ((queryServiceCash_3 + queryServiceBank_3) * SalaryPercentService2Tabs);
                        }

                        #endregion


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
                                if (ReportType == 1)
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
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= DateS && x.PayDate <= DatePo)
                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();
                                }
                            }
                            else
                            {
                                if (ReportType == 1)
                                {
                                    /*
                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        from r in db.Rem2Parties

                                        where
                                            x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo) &&
                                            x.docSecondHandPurch.doc.DocID == r.DocID && r.Remnant == 0

                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();
                                    */

                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        where
                                            x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DateS && x.TabDate <= DatePo) &&
                                            x.docSecondHandPurch.DirSecondHandStatusID == 10
                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();

                                }
                                else
                                {
                                    /*
                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        from r in db.Rem2Parties

                                        where
                                            x.DirEmployeeID == DirEmployeeID && (x.PayDate >= DateS && x.PayDate <= DatePo) &&
                                            x.docSecondHandPurch.doc.DocID == r.DocID && r.Remnant == 0

                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();
                                    */

                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        where
                                            x.DirEmployeeID == DirEmployeeID && (x.PayDate >= DateS && x.PayDate <= DatePo) &&
                                            x.docSecondHandPurch.DirSecondHandStatusID == 10
                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();

                                }
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
                                    " DocSecondHandPurch1Tabs." + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo ";
                                //"GROUP BY DocSecondHandPurches.DocSecondHandPurchID ";
                            }
                            else
                            {
                                /*
                                SQL =
                                    "SELECT COUNT(*) AS CountX " +
                                    "FROM Docs, DocSecondHandPurches, DocSecondHandPurch1Tabs, Rem2Parties " +
                                    "WHERE " +
                                    " Docs.DocID = DocSecondHandPurches.DocID and" +
                                    " DocSecondHandPurches.DocSecondHandPurchID = DocSecondHandPurch1Tabs.DocSecondHandPurchID and " +
                                    " DocSecondHandPurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                    " DocSecondHandPurch1Tabs." + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo and " +
                                    " Docs.DocID = Rem2Parties.DocID and Rem2Parties.Remnant = 0 "; // !!! //
                                    //" GROUP BY DocSecondHandPurches.DocSecondHandPurchID ";
                                */

                                SQL =
                                    "SELECT COUNT(*) AS CountX " +
                                    "FROM Docs, DocSecondHandPurches, DocSecondHandPurch1Tabs " +
                                    "WHERE " +
                                    " Docs.DocID = DocSecondHandPurches.DocID and" +
                                    " DocSecondHandPurches.DocSecondHandPurchID = DocSecondHandPurch1Tabs.DocSecondHandPurchID and " +
                                    " DocSecondHandPurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                    " DocSecondHandPurch1Tabs." + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo and " +
                                    " DocSecondHandPurches.DirSecondHandStatusID=10 "; // !!! //
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
                            /*
                            double querySum =
                                (
                                    from x in db.Rem2PartyMinuses
                                    where x.DirEmployeeID == DirEmployeeID && (x.DocDate >= DateS && x.DocDate <= DatePo)
                                    select x.Quantity * x.PriceCurrency
                                ).DefaultIfEmpty(0).Sum();
                            */
                            double querySum =
                                (
                                    from x in db.DocSecondHandSales
                                    where x.doc.DirEmployeeID == DirEmployeeID && (x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo)
                                    select x.PriceCurrency
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
                            /*
                            string SQL =
                                "  SELECT COUNT(*) AS CountX " +
                                "  FROM Docs, Rem2PartyMinuses " +
                                "  WHERE " +
                                "   Docs.DocID = Rem2PartyMinuses.DocID and" +
                                "   Rem2PartyMinuses.DirEmployeeID = @DirEmployeeID and " +
                                "   Docs.DocDate BETWEEN @DateS and @DatePo ";
                            */

                            string SQL =
                                "  SELECT COUNT(*) AS CountX " +
                                "  FROM Docs, DocSecondHandSales " +
                                "  WHERE " +
                                "   Docs.DocID = DocSecondHandSales.DocID and" +
                                "   Docs.DirEmployeeID = @DirEmployeeID and " +
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



                        // === Хоз.расчёты === === === 

                        #region //5.2. Хоз.расчёты за нал и безнал (DirDomesticExpenses.DirDomesticExpenseType == 2)

                        var queryDomesticExpenseCash2 =
                            (
                                from x in db.DocCashOfficeSums
                                from y in db.DocDomesticExpenses
                                from z in db.DocDomesticExpenseTabs
                                where
                                 (x.DocCashOfficeSumDate >= DateS && x.DocCashOfficeSumDate <= DatePo) &&
                                 (x.doc.ListObjectID == 70) &&
                                 (x.doc.DocID == y.doc.DocID && y.DocDomesticExpenseID == z.DocDomesticExpenseID && z.dirDomesticExpense.DirDomesticExpenseType == 2) &&
                                 (y.DirEmployeeIDSpisat == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)

                                select x.DocCashOfficeSumSum
                            ).DefaultIfEmpty(0).Sum();


                        var queryDomesticExpenseBank2 =
                            (
                                from x in db.DocBankSums
                                from y in db.DocDomesticExpenses
                                from z in db.DocDomesticExpenseTabs
                                where
                                 (x.DocBankSumDate >= DateS && x.DocBankSumDate <= DatePo) &&
                                 (x.doc.ListObjectID == 70) &&
                                 (x.doc.DocID == y.doc.DocID && y.DocDomesticExpenseID == z.DocDomesticExpenseID && z.dirDomesticExpense.DirDomesticExpenseType == 2) &&
                                 (y.DirEmployeeIDSpisat == DirEmployeeID) //(x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();

                        #endregion


                        // === Инвентаризация === === === 

                        #region //6 БУ: Инвентаризация

                        double sumSecondHandInventory = 0;

                        //Получаем сумму (К-во*Цена аппарата) в инвентаризации за дату
                        var querySecondHandInventory =
                            (
                                from x in db.DocSecondHandInvTabs
                                where
                                 //(x.docSecondHandInv.DirWarehouseID == DirWarehouseID) &&
                                 (x.docSecondHandInv.doc.DocDate >= DateS && x.docSecondHandInv.doc.DocDate <= DatePo) &&
                                 (x.docSecondHandInv.doc.Held == true) &&
                                 (x.Exist == 2) &&
                                 (x.docSecondHandInv.SpisatS == 2 && x.docSecondHandInv.SpisatSDirEmployeeID == DirEmployeeID)
                                select x.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();

                        sumSecondHandInventory = querySecondHandInventory;

                        #endregion



                        // === Отправка === === ===

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

                        //СЦ
                        /*
                        docSalaryTab.DirBonus2ID = queryDirEmployees[i].DirBonus2ID;
                        docSalaryTab.DirBonus2Name = DirBonus2Name;
                        docSalaryTab.DirBonus2IDSalary = Math.Round(DirBonus2IDSalary, 2);
                        docSalaryTab.SumSalaryFixedServiceOne = Math.Round(SumSalaryFixedServiceOne, 2);
                        */
                        docSalaryTab.sumSalaryPercentService1Tabs = Math.Round(sumSalaryPercentService1Tabs, 2); docSalaryTab.sumSalaryPercentService1TabsCount = Math.Round(sumSalaryPercentService1TabsCount, 2);
                        docSalaryTab.sumSalaryPercentService2Tabs = Math.Round(sumSalaryPercentService2Tabs, 2);


                        //Б/У
                        docSalaryTab.DirBonus4ID = queryDirEmployees[i].DirBonus4ID;
                        docSalaryTab.DirBonus4Name = DirBonus4Name;
                        docSalaryTab.DirBonus4IDSalary = Math.Round(DirBonus4IDSalary, 2);
                        docSalaryTab.SumSalaryFixedSecondHandWorkshopOne = Math.Round(SumSalaryFixedSecondHandWorkshopOne, 2);

                        docSalaryTab.DirBonus3ID = queryDirEmployees[i].DirBonus3ID;
                        docSalaryTab.DirBonus3Name = DirBonus3Name;
                        docSalaryTab.DirBonus3IDSalary = Math.Round(DirBonus3IDSalary, 2);
                        docSalaryTab.SumSalaryFixedSecondHandRetailOne = Math.Round(SumSalaryFixedSecondHandRetailOne, 2);


                        //Хоз.расходы (выплаченые ЗП сотруднику)
                        docSalaryTab.DomesticExpensesSalary = queryDomesticExpenseCash2 + queryDomesticExpenseBank2;
                        docSalaryTab.sumSecondHandInventory = sumSecondHandInventory; 


                        //docSalaryTab.Sums = Math.Round(SumSalary + DirBonusIDSalary + DirBonus2IDSalary + DirBonus4IDSalary + SumSalaryFixedSecondHandWorkshopOne + DirBonus3IDSalary + SumSalaryFixedSecondHandRetailOne + SalaryFixedSalesMount + SumSalaryFixedServiceOne, 2);
                        docSalaryTab.Sums = Math.Round(SumSalary + DirBonusIDSalary + DirBonus4IDSalary + SumSalaryFixedSecondHandWorkshopOne + DirBonus3IDSalary + SumSalaryFixedSecondHandRetailOne + SalaryFixedSalesMount + sumSalaryPercentService1Tabs + sumSalaryPercentService2Tabs + docSalaryTab.DomesticExpensesSalary - sumSecondHandInventory, 2);


                        arrDocSalaryTabSQL[i] = docSalaryTab;


                        #endregion
                    }

                    #endregion

                }
                else if (queryDirEmployees.Count() == 1)
                {

                    #region 1 сотрудник



                    #region var

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
                        DirEmployeeID = Convert.ToInt32(queryDirEmployees[0].DirEmployeeID),
                        DirBonusID = queryDirEmployees[0].DirBonusID,
                        DirBonus2ID = queryDirEmployees[0].DirBonus2ID,
                        //Б/У
                        DirBonus4ID = queryDirEmployees[0].DirBonus4ID,
                        DirBonus3ID = queryDirEmployees[0].DirBonus3ID;


                    //NEW
                    int SalaryPercentService1TabsType = Convert.ToInt32(queryDirEmployees[0].SalaryPercentService1TabsType); double SalaryPercentService1Tabs = Convert.ToInt32(queryDirEmployees[0].SalaryPercentService1Tabs);
                    int SalaryPercentService2TabsType = Convert.ToInt32(queryDirEmployees[0].SalaryPercentService2TabsType); double SalaryPercentService2Tabs = Convert.ToInt32(queryDirEmployees[0].SalaryPercentService2Tabs);

                    #endregion



                    for (int i = 0; i < alDate.Count; i++)
                    {

                        DateTime dtS = Convert.ToDateTime(((DateTime)alDate[i]).ToString("yyyy-MM-dd")); //ToString("yyyy-MM-dd 00:00:01"));
                        DateTime dtPo = Convert.ToDateTime(((DateTime)alDate[i]).ToString("yyyy-MM-dd 23:59:59"));


                        // === Salary === === ===

                        #region 0. SumSalary: К-во выходов в месяц


                        #region Зарплата за 1 месяц: если установили даты больше или меньше месяца (Salary, SalaryFixedSalesMount)
                        //Проблема в том что:
                        //В каждом месяце ЗП за 1 день будет разная:
                        //1. 28 дней в месяце
                        //2. 30 дней в месяце
                        //3. 31 дней в месяце

                        double
                            iDayDiv_SumSalary = 0, //Convert.ToDouble(queryDirEmployees[0].Salary), 
                            iDayDiv_SalaryFixedSalesMount = 0; //Convert.ToDouble(queryDirEmployees[0].SalaryFixedSalesMount);

                        //За 1 месяц 1000 рублей
                        //Период: [2016-11-20] - [2017-03-10]
                        //1. [2016-11-20] - [2016-11-30] //11 дней // 367 руб
                        //2. [2016-12-01] - [2016-12-31] //31 дней // 1000 руб
                        //3. [2017-01-01] - [2016-01-31] //31 дней // 1000 руб
                        //4. [2017-02-01] - [2016-02-28] //28 дней // 1000 руб
                        //5. [2017-03-01] - [2016-03-10] //10 дней // 323 руб

                        //Будем менять (ниже)
                        DateTime DateS2 = dtS;

                        //Сколько дней в периоде
                        TimeSpan ts = dtPo - dtS;

                        //for (int d = 0; d < ts.Days + 1; d++)
                        /*
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

                            iDayDiv_SumSalary += (Convert.ToDouble(queryDirEmployees[0].Salary) / iDay1) * iDay2;
                            iDayDiv_SalaryFixedSalesMount += (Convert.ToDouble(queryDirEmployees[0].SalaryFixedSalesMount) / iDay1) * iDay2;

                            //Выход
                            if (DateS2 >= DatePo || d == 1000) break;
                        }
                        */

                        //Получили к-во дней в месяце
                        int iDay1 = System.DateTime.DaysInMonth(DateS2.Year, DateS2.Month);
                        //...
                        iDayDiv_SumSalary += (Convert.ToDouble(queryDirEmployees[0].Salary) / iDay1);
                        iDayDiv_SalaryFixedSalesMount += (Convert.ToDouble(queryDirEmployees[0].SalaryFixedSalesMount) / iDay1);

                        #endregion




                        if (queryDirEmployees[0].SalaryDayMonthly == 1)
                        {
                            #region Продажи + Ремонты

                            //Продажи: RemPartyMinuses
                            //Продажи: DocServicePurch1Tabs


                            string SQL =
                                "SELECT DateX, SUM(CountX) AS CountX " +
                                "FROM " +
                                "  ( " +
                                "  SELECT date(RemPartyMinuses.DocDate) AS DateX, COUNT(date(RemPartyMinuses.DocDate)) AS CountX    " +
                                "  FROM RemPartyMinuses, Docs " +
                                "  WHERE RemPartyMinuses.DocID=Docs.DocID and RemPartyMinuses.DirEmployeeID=@DirEmployeeID and (Docs.ListObjectID == 56 or Docs.ListObjectID == 32) and (RemPartyMinuses.DocDate BETWEEN @DateS and @DatePo) " +
                                "  GROUP BY date(RemPartyMinuses.DocDate) " +
                                "  UNION " +
                                "  SELECT date(" + DocServicePurch1Tabs_XDate + ") AS DateX, COUNT(date(" + DocServicePurch1Tabs_XDate + ")) AS CountX " +
                                "  FROM DocServicePurch1Tabs " +
                                "  WHERE DirEmployeeID=@DirEmployeeID and (" + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo) " +
                                "  GROUP BY date(DocServicePurch1Tabs." + DocServicePurch1Tabs_XDate + ") " +
                                "  ) AS X1 " +
                                "GROUP BY date(X1.DateX) ";

                            SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                            SQLiteParameter parDateS = new SQLiteParameter("@DateS", System.Data.DbType.Date) { Value = dtS.ToString("yyyy-MM-dd 00:00:00") };
                            SQLiteParameter parDatePo = new SQLiteParameter("@DatePo", System.Data.DbType.Date) { Value = dtPo.ToString("yyyy-MM-dd 23:59:59") };

                            //Сам запрос с параметрами
                            var query = db.Database.SqlQuery<Models.Sklad.Rem.RemPartyMinusSQL>(SQL, parDirEmployeeID, parDateS, parDatePo);

                            CountDay = 0; SumSalary = 0;
                            if (query.Count() > 0)
                            {
                                CountDay = query.Count();
                                SumSalary = query.Count() * Convert.ToDouble(queryDirEmployees[0].Salary);
                            }

                            #endregion
                        }
                        else
                        {
                            SumSalary = iDayDiv_SumSalary; // Convert.ToDouble(queryDirEmployees[0].Salary);
                        }

                        //ЗП фиксированная за месяц
                        if (queryDirEmployees[0].SalaryFixedSalesMount > 0)
                        {
                            SalaryFixedSalesMount = iDayDiv_SalaryFixedSalesMount; // Convert.ToDouble(queryDirEmployees[0].SalaryFixedSalesMount);
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
                                    where x.DirEmployeeID == DirEmployeeID && (x.doc.ListObjectID == 56 || x.doc.ListObjectID == 32) && (x.DocDate >= dtS && x.DocDate <= dtPo)
                                    select x.Quantity * x.PriceCurrency - x.doc.Discount
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



                        // === СЦ === === ===

                        //!!! Старый алгоритм !!!
                        #region 2. DirBonus2IDSalary: Премия с ремонта

                        //Алгоритм:
                        //Получаем [Сумму продаж]
                        //По градации смотрим в какой диапазон попадает и берём процент
                        //[Сумма продаж] * [Процент]


                        /*
                        if (DirBonus2ID > 0)
                        {
                            //1. Получаем сумму 
                            double querySum = 0;
                            if (ReportType == 1)
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch2Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= dtS && x.TabDate <= dtPo)
                                        select x.PriceCurrency
                                    );
                                querySum = querySumX.DefaultIfEmpty(0).Sum();
                            }
                            else
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch2Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= dtS && x.PayDate <= dtPo)
                                        select x.PriceCurrency
                                    );
                                querySum = querySumX.DefaultIfEmpty(0).Sum();
                            }


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
                        double? SalaryFixedServiceOne = queryDirEmployees[0].SalaryFixedServiceOne;
                        if (SalaryFixedServiceOne > 0)
                        {
                            string SQL =
                                "SELECT COUNT(*)  AS CountX " +  // "SELECT DocServicePurches.DocServicePurchID  AS CountX " +  //"SELECT COUNT(*)  AS CountX " +
                                "FROM Docs, DocServicePurches, DocServicePurch1Tabs " +
                                "WHERE " +
                                " Docs.DocID = DocServicePurches.DocID and" +
                                " DocServicePurches.DocServicePurchID = DocServicePurch1Tabs.DocServicePurchID and " +
                                " DocServicePurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                " DocServicePurch1Tabs." + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo " + 
                                "GROUP BY DocServicePurches.DocServicePurchID ";


                            SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                            SQLiteParameter parDateS = new SQLiteParameter("@DateS", System.Data.DbType.Date) { Value = dtS.ToString("yyyy-MM-dd 00:00:00") };
                            SQLiteParameter parDatePo = new SQLiteParameter("@DatePo", System.Data.DbType.Date) { Value = dtPo.ToString("yyyy-MM-dd 23:59:59") };

                            //Сам запрос с параметрами
                            var query = await db.Database.SqlQuery<Models.Sklad.Doc.DocServicePurchSQL>(SQL, parDirEmployeeID, parDateS, parDatePo).ToListAsync();

                            if (query.Count() > 0)
                            {
                                //CountDay = query.Count();
                                //SumSalaryFixedServiceOne = Convert.ToDouble(query[0].CountX) * Convert.ToDouble(queryDirEmployees[0].SalaryFixedServiceOne);
                                SumSalaryFixedServiceOne = Convert.ToDouble(query.Count()) * Convert.ToDouble(queryDirEmployees[0].SalaryFixedServiceOne);
                            }
                        }
                        */


                        #endregion


                        //!!! Новый алгоритм !!!
                        #region //7.2. СЦ: SalaryPercentService1Tabs and SalaryPercentService2Tabs


                        #region Работы

                        double sumSalaryPercentService1Tabs = 0, sumSalaryPercentService1TabsCount = 0;

                        if (SalaryPercentService1TabsType == 1)
                        {
                            //Процент с суммы всех работ
                            /*
                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
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
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)
                                    select docServicePurch1Tabs.PriceCurrency
                                ).DefaultIfEmpty(0).Sum();


                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs) / 100;
                            */

                            // *** *** ***


                            double querySum = 0;
                            if (ReportType == 1)
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= dtS && x.TabDate <= dtPo)
                                        select x.PriceCurrency
                                    );
                                querySum = querySumX.DefaultIfEmpty(0).Sum();
                            }
                            else
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= dtS && x.PayDate <= dtPo)
                                        select x.PriceCurrency
                                    );
                                querySum = querySumX.DefaultIfEmpty(0).Sum();
                            }

                            sumSalaryPercentService1Tabs = (querySum * SalaryPercentService1Tabs) / 100;

                        }
                        else if (SalaryPercentService1TabsType == 2)
                        {
                            //Фиксированная сумма за все работы в рамках одного ремонта
                            /*
                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) && (x.DirCashOfficeSumTypeID == 15) && (x.DirEmployeeID == DirEmployeeID) && // && (x.DirCashOfficeID == DirCashOfficeID)
                                 (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40)
                                select docServicePurches.DocServicePurchID
                            ).Distinct().Count();


                            var queryServiceBank_2 =
                                (
                                    from x in db.DocBankSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                    where
                                     (x.DocBankSumDate >= dtS && x.DocBankSumDate <= dtPo) && (x.DirBankSumTypeID == 14) && (x.DirEmployeeID == DirEmployeeID) && // && (x.DirBankID == DirBankID)
                                     (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40)
                                    select docServicePurches.DocServicePurchID
                                ).Distinct().Count();

                            sumSalaryPercentService1TabsCount = queryServiceCash_2 + queryServiceBank_2;
                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs);
                            */

                            // *** *** ***


                            double queryCount = 0;
                            if (ReportType == 1)
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= dtS && x.TabDate <= dtPo)
                                        select x.DocServicePurchID
                                    );
                                queryCount = querySumX.Distinct().Count();
                            }
                            else
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= dtS && x.PayDate <= dtPo)
                                        select x.DocServicePurchID
                                    );
                                queryCount = querySumX.Distinct().Count();
                            }

                            sumSalaryPercentService1TabsCount = queryCount;
                            sumSalaryPercentService1Tabs = (queryCount * SalaryPercentService1Tabs);

                        }
                        else if (SalaryPercentService1TabsType == 3)
                        {
                            //Фиксированная сумма с каждого ремонта
                            /*
                            var queryServiceCash_2 =
                            (
                                from x in db.DocCashOfficeSums
                                join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                join docServicePurch1Tabs in db.DocServicePurch1Tabs on docServicePurches.DocServicePurchID equals docServicePurch1Tabs.DocServicePurchID

                                where
                                 (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                 (docServicePurch1Tabs.PayDate >= dtS && docServicePurch1Tabs.PayDate <= dtPo) &&
                                 (x.doc.ListObjectID == 40) &&
                                 (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
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
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)
                                    select docServicePurches.DocServicePurchID
                                ).Count();

                            sumSalaryPercentService1TabsCount = queryServiceCash_2 + queryServiceBank_2;
                            sumSalaryPercentService1Tabs = ((queryServiceCash_2 + queryServiceBank_2) * SalaryPercentService1Tabs);
                            */

                            // *** *** ***


                            double queryCount = 0;
                            if (ReportType == 1)
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= dtS && x.TabDate <= dtPo)
                                        select x.DocServicePurchID
                                    );
                                queryCount = querySumX.Count();
                            }
                            else
                            {
                                var querySumX =
                                    (
                                        from x in db.DocServicePurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= dtS && x.PayDate <= dtPo)
                                        select x.DocServicePurchID
                                    );
                                queryCount = querySumX.Count();
                            }

                            sumSalaryPercentService1TabsCount = queryCount;
                            sumSalaryPercentService1Tabs = (queryCount * SalaryPercentService1Tabs);
                        }

                        #endregion


                        #region Запчасти

                        double sumSalaryPercentService2Tabs = 0;

                        if (SalaryPercentService2TabsType == 1)
                        {
                            //Процент с продажи
                            var queryServiceCash_31111 =
                                (
                                    from x in db.DocCashOfficeSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch2Tabs in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs.DocServicePurchID

                                    where
                                     (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                     (docServicePurch2Tabs.PayDate >= dtS && docServicePurch2Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
                                    select docServicePurch2Tabs.PriceCurrency
                                );

                            var queryServiceCash_3 =
                                (
                                    from x in db.DocCashOfficeSums
                                    join docServicePurches in db.DocServicePurches on x.DocID equals docServicePurches.DocID
                                    join docServicePurch2Tabs in db.DocServicePurch2Tabs on docServicePurches.DocServicePurchID equals docServicePurch2Tabs.DocServicePurchID

                                    where
                                     (x.DocCashOfficeSumDate >= dtS && x.DocCashOfficeSumDate <= dtPo) &&
                                     (docServicePurch2Tabs.PayDate >= dtS && docServicePurch2Tabs.PayDate <= dtPo) &&
                                     (x.doc.ListObjectID == 40) &&
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
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
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)
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
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)

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
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)

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
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)
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
                                     (x.DirEmployeeID == DirEmployeeID) //(x.DirBankID == DirBankID)
                                    select docServicePurches.DocServicePurchID
                                ).Count();


                            sumSalaryPercentService2Tabs = ((queryServiceCash_3 + queryServiceBank_3) * SalaryPercentService2Tabs);
                        }

                        #endregion


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
                            if (!queryDirEmployees[0].SalarySecondHandWorkshopCheck)
                            {
                                if (ReportType == 1)
                                {
                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= dtS && x.TabDate <= dtPo)
                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();
                                }
                                else
                                {
                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        where x.DirEmployeeID == DirEmployeeID && (x.PayDate >= dtS && x.PayDate <= dtPo)
                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();
                                }
                            }
                            else
                            {
                                if (ReportType == 1)
                                {
                                    /*
                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        from r in db.Rem2Parties

                                        where
                                            x.DirEmployeeID == DirEmployeeID && (x.TabDate >= dtS && x.TabDate <= dtPo) &&
                                            x.docSecondHandPurch.doc.DocID == r.DocID && r.Remnant == 0

                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();
                                    */

                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        where
                                            x.DirEmployeeID == DirEmployeeID && (x.TabDate >= dtS && x.TabDate <= dtPo) &&
                                            x.docSecondHandPurch.DirSecondHandStatusID == 10
                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();

                                }
                                else
                                {
                                    /*
                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        from r in db.Rem2Parties

                                        where
                                            x.DirEmployeeID == DirEmployeeID && (x.PayDate >= dtS && x.PayDate <= dtPo) &&
                                            x.docSecondHandPurch.doc.DocID == r.DocID && r.Remnant == 0

                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();
                                    */

                                    querySum =
                                    (
                                        from x in db.DocSecondHandPurch1Tabs
                                        where
                                            x.DirEmployeeID == DirEmployeeID && (x.PayDate >= dtS && x.PayDate <= dtPo) &&
                                            x.docSecondHandPurch.DirSecondHandStatusID == 10

                                        select x.PriceCurrency
                                    ).DefaultIfEmpty(0).Sum();
                                }
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
                        double? SalaryFixedSecondHandWorkshopOne = queryDirEmployees[0].SalaryFixedSecondHandWorkshopOne;
                        if (SalaryFixedSecondHandWorkshopOne > 0)
                        {
                            string SQL = "";

                            if (!queryDirEmployees[0].SalarySecondHandWorkshopCheck)
                            {
                                SQL =
                                    "SELECT COUNT(*) AS CountX " +
                                    "FROM Docs, DocSecondHandPurches, DocSecondHandPurch1Tabs " +
                                    "WHERE " +
                                    " Docs.DocID = DocSecondHandPurches.DocID and" +
                                    " DocSecondHandPurches.DocSecondHandPurchID = DocSecondHandPurch1Tabs.DocSecondHandPurchID and " +
                                    " DocSecondHandPurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                    " DocSecondHandPurch1Tabs." + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo ";
                                //"GROUP BY DocSecondHandPurches.DocSecondHandPurchID ";
                            }
                            else
                            {
                                /*
                                SQL =
                                    "SELECT COUNT(*) AS CountX " +
                                    "FROM Docs, DocSecondHandPurches, DocSecondHandPurch1Tabs, Rem2Parties " +
                                    "WHERE " +
                                    " Docs.DocID = DocSecondHandPurches.DocID and" +
                                    " DocSecondHandPurches.DocSecondHandPurchID = DocSecondHandPurch1Tabs.DocSecondHandPurchID and " +
                                    " DocSecondHandPurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                    " DocSecondHandPurch1Tabs." + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo and " +
                                    " Docs.DocID = Rem2Parties.DocID and Rem2Parties.Remnant = 0 "; // !!! //
                                    //"GROUP BY DocSecondHandPurches.DocSecondHandPurchID ";
                                */

                                SQL =
                                    "SELECT COUNT(*) AS CountX " +
                                    "FROM Docs, DocSecondHandPurches, DocSecondHandPurch1Tabs " +
                                    "WHERE " +
                                    " Docs.DocID = DocSecondHandPurches.DocID and" +
                                    " DocSecondHandPurches.DocSecondHandPurchID = DocSecondHandPurch1Tabs.DocSecondHandPurchID and " +
                                    " DocSecondHandPurch1Tabs.DirEmployeeID = @DirEmployeeID and " +
                                    " DocSecondHandPurch1Tabs." + DocServicePurch1Tabs_XDate + " BETWEEN @DateS and @DatePo and " +
                                    " DocSecondHandPurches.DirSecondHandStatusID=10"; // !!! //
                            }


                            SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                            SQLiteParameter parDateS = new SQLiteParameter("@DateS", System.Data.DbType.Date) { Value = dtS.ToString("yyyy-MM-dd 00:00:00") };
                            SQLiteParameter parDatePo = new SQLiteParameter("@DatePo", System.Data.DbType.Date) { Value = dtPo.ToString("yyyy-MM-dd 23:59:59") };

                            //Сам запрос с параметрами
                            var query = await db.Database.SqlQuery<Models.Sklad.Doc.DocSecondHandPurchSQL>(SQL, parDirEmployeeID, parDateS, parDatePo).ToListAsync();

                            if (query.Count() > 0)
                            {
                                //CountDay = query.Count();
                                SumSalaryFixedSecondHandWorkshopOne = Convert.ToDouble(query[0].CountX) * Convert.ToDouble(queryDirEmployees[0].SalaryFixedSecondHandWorkshopOne);
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
                            /*
                            double querySum =
                                (
                                    from x in db.Rem2PartyMinuses
                                    where x.DirEmployeeID == DirEmployeeID && (x.DocDate >= dtS && x.DocDate <= dtPo)
                                    select x.Quantity * x.PriceCurrency
                                ).DefaultIfEmpty(0).Sum();
                            */
                            double querySum =
                                (
                                    from x in db.DocSecondHandSales
                                    where x.doc.DirEmployeeID == DirEmployeeID && (x.doc.DocDate >= dtS && x.doc.DocDate <= dtPo)
                                    select x.PriceCurrency
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
                        double? SalaryFixedSecondHandRetailOne = queryDirEmployees[0].SalaryFixedSecondHandRetailOne;
                        if (SalaryFixedSecondHandRetailOne > 0)
                        {
                            /*
                            string SQL =
                                "  SELECT COUNT(*) AS CountX " +
                                "  FROM Docs, Rem2PartyMinuses " +
                                "  WHERE " +
                                "   Docs.DocID = Rem2PartyMinuses.DocID and" +
                                "   Rem2PartyMinuses.DirEmployeeID = @DirEmployeeID and " +
                                "   Docs.DocDate BETWEEN @DateS and @DatePo ";
                            */

                            string SQL =
                                "  SELECT COUNT(*) AS CountX " +
                                "  FROM Docs, DocSecondHandSales " +
                                "  WHERE " +
                                "   Docs.DocID = DocSecondHandSales.DocID and" +
                                "   Docs.DirEmployeeID = @DirEmployeeID and " +
                                "   Docs.DocDate BETWEEN @DateS and @DatePo ";


                            SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                            SQLiteParameter parDateS = new SQLiteParameter("@DateS", System.Data.DbType.Date) { Value = dtS.ToString("yyyy-MM-dd 00:00:00") };
                            SQLiteParameter parDatePo = new SQLiteParameter("@DatePo", System.Data.DbType.Date) { Value = dtPo.ToString("yyyy-MM-dd 23:59:59") };

                            //Сам запрос с параметрами
                            var query = await db.Database.SqlQuery<Models.Sklad.Doc.DocSecondHandPurchSQL>(SQL, parDirEmployeeID, parDateS, parDatePo).ToListAsync();

                            if (query.Count() > 0)
                            {
                                //CountDay = query.Count();
                                SumSalaryFixedSecondHandRetailOne = Convert.ToDouble(query[0].CountX) * Convert.ToDouble(queryDirEmployees[0].SalaryFixedSecondHandRetailOne);
                            }
                        }


                        #endregion



                        // === Хоз.расчёты === === === 

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
                                 (y.DirEmployeeIDSpisat == DirEmployeeID) //(x.DirCashOfficeID == DirCashOfficeID)

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
                                 (y.DirEmployeeIDSpisat == DirEmployeeID) //(x.DirBankID == DirBankID)
                                select x.DocBankSumSum
                            ).DefaultIfEmpty(0).Sum();

                        #endregion


                        // === Инвентаризация === === === 

                        #region //6 БУ: Инвентаризация

                        double sumSecondHandInventory = 0;

                        //Получаем сумму (К-во*Цена аппарата) в инвентаризации за дату
                        var querySecondHandInventory =
                            (
                                from x in db.DocSecondHandInvTabs
                                where
                                 //(x.docSecondHandInv.DirWarehouseID == DirWarehouseID) &&
                                 (x.docSecondHandInv.doc.DocDate >= dtS && x.docSecondHandInv.doc.DocDate <= dtPo) &&
                                 (x.docSecondHandInv.doc.Held == true) &&
                                 (x.Exist == 2) &&
                                 (x.docSecondHandInv.SpisatS == 2 && x.docSecondHandInv.SpisatSDirEmployeeID == DirEmployeeID)
                                select x.PriceCurrency
                            ).DefaultIfEmpty(0).Sum();

                        sumSecondHandInventory = querySecondHandInventory;

                        #endregion



                        // === Отправка === === ===

                        #region 4. Data

                        string SalaryDayMonthlyName = "За день";
                        if (queryDirEmployees[0].SalaryDayMonthly == 2) SalaryDayMonthlyName = "За месяц";

                        string DirBonusName = ""; if (queryDirEmployees[0].dirBonus != null) DirBonusName = queryDirEmployees[0].dirBonus.DirBonusName;
                        string DirBonus2Name = ""; if (queryDirEmployees[0].dirBonus2 != null) DirBonus2Name = queryDirEmployees[0].dirBonus2.DirBonusName;
                        string DirBonus3Name = ""; if (queryDirEmployees[0].dirBonus3 != null) DirBonus3Name = queryDirEmployees[0].dirBonus3.DirBonusName;
                        string DirBonus4Name = ""; if (queryDirEmployees[0].dirBonus4 != null) DirBonus4Name = queryDirEmployees[0].dirBonus4.DirBonusName;


                        Models.Sklad.Doc.DocSalaryTabSQL docSalaryTab = new Models.Sklad.Doc.DocSalaryTabSQL();
                        docSalaryTab.DirEmployeeID = Convert.ToInt32(DirEmployeeID);
                        docSalaryTab.DirEmployeeName = queryDirEmployees[0].DirEmployeeName;

                        docSalaryTab.DocDate = (DateTime)alDate[i];

                        if (queryDirEmployees[0].DirCurrencyID != null)
                        {
                            docSalaryTab.DirCurrencyID = Convert.ToInt32(queryDirEmployees[0].DirCurrencyID);
                            docSalaryTab.DirCurrencyName = queryDirEmployees[0].dirCurrency.DirCurrencyName;
                            docSalaryTab.DirCurrencyRate = queryDirEmployees[0].dirCurrency.DirCurrencyRate;
                            docSalaryTab.DirCurrencyMultiplicity = queryDirEmployees[0].dirCurrency.DirCurrencyMultiplicity;
                        }
                        else
                        {
                            docSalaryTab.DirCurrencyID = 1;
                            docSalaryTab.DirCurrencyName = "???";
                            docSalaryTab.DirCurrencyRate = 1;
                            docSalaryTab.DirCurrencyMultiplicity = 1;
                        }

                        docSalaryTab.Salary = Math.Round(SumSalary, 2); //Math.Round(Convert.ToDouble(queryDirEmployees[0].Salary), 2);
                        docSalaryTab.CountDay = CountDay;
                        docSalaryTab.SalaryDayMonthly = Convert.ToInt32(queryDirEmployees[0].SalaryDayMonthly);
                        docSalaryTab.SalaryDayMonthlyName = SalaryDayMonthlyName;
                        docSalaryTab.SumSalary = Math.Round(SumSalary, 2);
                        docSalaryTab.SalaryFixedSalesMount = Math.Round(SalaryFixedSalesMount, 2);

                        docSalaryTab.DirBonusID = queryDirEmployees[0].DirBonusID;
                        docSalaryTab.DirBonusName = DirBonusName;
                        docSalaryTab.DirBonusIDSalary = Math.Round(DirBonusIDSalary, 2);

                        //СЦ
                        /*
                        docSalaryTab.DirBonus2ID = queryDirEmployees[0].DirBonus2ID;
                        docSalaryTab.DirBonus2Name = DirBonus2Name;
                        docSalaryTab.DirBonus2IDSalary = Math.Round(DirBonus2IDSalary, 2);
                        docSalaryTab.SumSalaryFixedServiceOne = Math.Round(SumSalaryFixedServiceOne, 2);
                        */
                        docSalaryTab.sumSalaryPercentService1Tabs = Math.Round(sumSalaryPercentService1Tabs, 2); docSalaryTab.sumSalaryPercentService1TabsCount = Math.Round(sumSalaryPercentService1TabsCount, 2);
                        docSalaryTab.sumSalaryPercentService2Tabs = Math.Round(sumSalaryPercentService2Tabs, 2);


                        //Б/У
                        docSalaryTab.DirBonus4ID = queryDirEmployees[0].DirBonus4ID;
                        docSalaryTab.DirBonus4Name = DirBonus4Name;
                        docSalaryTab.DirBonus4IDSalary = Math.Round(DirBonus4IDSalary, 2);
                        docSalaryTab.SumSalaryFixedSecondHandWorkshopOne = Math.Round(SumSalaryFixedSecondHandWorkshopOne, 2);

                        docSalaryTab.DirBonus3ID = queryDirEmployees[0].DirBonus3ID;
                        docSalaryTab.DirBonus3Name = DirBonus3Name;
                        docSalaryTab.DirBonus3IDSalary = Math.Round(DirBonus3IDSalary, 2);
                        docSalaryTab.SumSalaryFixedSecondHandRetailOne = Math.Round(SumSalaryFixedSecondHandRetailOne, 2);


                        //Хоз.расходы (выплаченые ЗП сотруднику)
                        docSalaryTab.DomesticExpensesSalary = queryDomesticExpenseCash2 + queryDomesticExpenseBank2;

                        //Инвентаризация
                        docSalaryTab.sumSecondHandInventory = sumSecondHandInventory;

                        //docSalaryTab.Sums = Math.Round(SumSalary + DirBonusIDSalary + DirBonus2IDSalary + DirBonus4IDSalary + SumSalaryFixedSecondHandWorkshopOne + DirBonus3IDSalary + SumSalaryFixedSecondHandRetailOne + SalaryFixedSalesMount + SumSalaryFixedServiceOne, 2);
                        docSalaryTab.Sums = Math.Round(SumSalary + DirBonusIDSalary + sumSalaryPercentService1Tabs + sumSalaryPercentService2Tabs + DirBonus4IDSalary + SumSalaryFixedSecondHandWorkshopOne + DirBonus3IDSalary + SumSalaryFixedSecondHandRetailOne + SalaryFixedSalesMount + docSalaryTab.DomesticExpensesSalary - sumSecondHandInventory, 2);

                        arrDocSalaryTabSQL[i] = docSalaryTab;


                        #endregion


                    }


                    #endregion

                }

                #endregion




                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = arrDocSalaryTabSQL.Count(),
                    ReportSalaries = arrDocSalaryTabSQL
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
