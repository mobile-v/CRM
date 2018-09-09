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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSalaries
{
    public class DocSalaryTabsController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        private DbConnectionSklad db = new DbConnectionSklad();

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            public int DocSalaryID;
            public int DocID;
            public int DocYear;
            public int DocMonth;
        }
        // GET: api/DocSalaryTabs
        public async Task<IHttpActionResult> GetDocSalaryTabs(HttpRequestMessage request)
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

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSalaries"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.DocSalaryID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSalaryID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docSalaryTabs in db.DocSalaryTabs
                        where docSalaryTabs.DocSalaryID == _params.DocSalaryID

                        #region select

                        select new
                        {
                            DocSalaryTabID = docSalaryTabs.DocSalaryTabID,
                            DocSalaryID = docSalaryTabs.DocSalaryID,
                            DirEmployeeID = docSalaryTabs.DirEmployeeID,
                            DirEmployeeName = docSalaryTabs.dirEmployee.DirEmployeeName,

                            DirCurrencyID = docSalaryTabs.DirCurrencyID,
                            DirCurrencyRate = docSalaryTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSalaryTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docSalaryTabs.dirCurrency.DirCurrencyName + " (" + docSalaryTabs.DirCurrencyRate + ", " + docSalaryTabs.DirCurrencyMultiplicity + ")",

                            Salary = docSalaryTabs.Salary,
                            SalaryDayMonthly = docSalaryTabs.SalaryDayMonthly,
                            SalaryDayMonthlyName = docSalaryTabs.SalaryDayMonthly == 1 ? "За день" : "За месяц",
                            CountDay = docSalaryTabs.CountDay,
                            SumSalary = docSalaryTabs.SumSalary,

                            DirBonusID = docSalaryTabs.DirBonusID,
                            DirBonusName = docSalaryTabs.dirBonus.DirBonusName,
                            DirBonusIDSalary = docSalaryTabs.DirBonusIDSalary,

                            DirBonus2ID = docSalaryTabs.DirBonus2ID,
                            DirBonus2Name = docSalaryTabs.dirBonus2.DirBonusName,
                            DirBonus2IDSalary = docSalaryTabs.DirBonus2IDSalary,

                            Sums = docSalaryTabs.Sums,

                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSalaryTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSalaryTabs/5
        [ResponseType(typeof(DocSalaryTab))]
        public async Task<IHttpActionResult> GetDocSalaryTab(int id, HttpRequestMessage request)
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSalaries"));
            if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion


            #region Параметры

            Params _params = new Params();

            //paramList -список параметров
            var paramList = request.GetQueryNameValuePairs();
            //Параметры
            _params.DocYear = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocYear", true) == 0).Value);
            _params.DocMonth = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocMonth", true) == 0).Value);
            DateTime DocDateS = Convert.ToDateTime(_params.DocYear.ToString() + "-" + _params.DocMonth + "-01");
            DateTime DocDatePo = DocDateS.AddMonths(1).AddDays(-1);

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


            var queryDirEmployees = await
                (
                    from x in db.DirEmployees
                    select x
                ).ToListAsync();


            Models.Sklad.Doc.DocSalaryTabSQL[] arrDocSalaryTabSQL = new Models.Sklad.Doc.DocSalaryTabSQL[queryDirEmployees.Count()];


            if (queryDirEmployees.Count() > 0)
            {
                for (int i = 0; i < queryDirEmployees.Count(); i++)
                {
                    int CountDay = 0;
                    double SumSalary = 0, DirBonusIDSalary = 0, DirBonus2IDSalary = 0, Sums = 0;

                    int? DirEmployeeID = queryDirEmployees[i].DirEmployeeID,
                         DirBonusID = queryDirEmployees[i].DirBonusID,
                         DirBonus2ID = queryDirEmployees[i].DirBonus2ID;


                    #region 1. SumSalary: К-во выходов в месяц

                    if (queryDirEmployees[i].SalaryDayMonthly == 1)
                    {
                        string SQL =
                            "SELECT date(DocDate) AS DateX, COUNT(date(DocDate)) AS CountX " +
                            "FROM RemPartyMinuses " +
                            "WHERE DirEmployeeID=@DirEmployeeID and (DocDate BETWEEN @DocDateS and @DocDatePo) " +
                            "GROUP BY date(RemPartyMinuses.DocDate);";

                        SQLiteParameter parDirEmployeeID = new SQLiteParameter("@DirEmployeeID", System.Data.DbType.Int32) { Value = DirEmployeeID };
                        SQLiteParameter parDocDateS = new SQLiteParameter("@DocDateS", System.Data.DbType.Date) { Value = DocDateS.ToString("yyyy-MM-dd") };
                        SQLiteParameter parDocDatePo = new SQLiteParameter("@DocDatePo", System.Data.DbType.Date) { Value = DocDatePo.ToString("yyyy-MM-dd") };

                        //Сам запрос с параметрами
                        var query = db.Database.SqlQuery<Models.Sklad.Rem.RemPartyMinusSQL>(SQL, parDirEmployeeID, parDocDateS, parDocDatePo);

                        if (query.Count() > 0)
                        {
                            CountDay = query.Count();
                            SumSalary = query.Count() * Convert.ToDouble(queryDirEmployees[i].Salary);
                        }

                    }
                    else
                    {
                        SumSalary = Convert.ToDouble(queryDirEmployees[i].Salary);
                    }

                    #endregion


                    #region 2. DirBonusIDSalary: Премия с продаж

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
                                where x.DirEmployeeID == DirEmployeeID && (x.DocDate >= DocDateS && x.DocDate <= DocDatePo)
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


                    #region 3. DirBonus2IDSalary: Премия с продаж

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
                                where x.DirEmployeeID == DirEmployeeID && (x.TabDate >= DocDateS && x.TabDate <= DocDatePo)
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


                    #endregion


                    #region 4. Data

                    string SalaryDayMonthlyName = "За день";
                    if (queryDirEmployees[i].SalaryDayMonthly == 2) SalaryDayMonthlyName = "За месяц";

                    string DirBonusName = ""; if(queryDirEmployees[i].dirBonus != null) DirBonusName = queryDirEmployees[i].dirBonus.DirBonusName;
                    string DirBonus2Name = ""; if (queryDirEmployees[i].dirBonus2 != null) DirBonus2Name = queryDirEmployees[i].dirBonus2.DirBonusName;
                    

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

                    docSalaryTab.Salary = Convert.ToDouble(queryDirEmployees[i].Salary);
                    docSalaryTab.CountDay = CountDay;
                    docSalaryTab.SalaryDayMonthly = Convert.ToInt32(queryDirEmployees[i].SalaryDayMonthly);
                    docSalaryTab.SalaryDayMonthlyName = SalaryDayMonthlyName;
                    docSalaryTab.SumSalary = SumSalary;

                    docSalaryTab.DirBonusID = queryDirEmployees[i].DirBonusID;
                    docSalaryTab.DirBonusName = DirBonusName;
                    docSalaryTab.DirBonusIDSalary = DirBonusIDSalary;

                    docSalaryTab.DirBonus2ID = queryDirEmployees[i].DirBonus2ID;
                    docSalaryTab.DirBonus2Name = DirBonus2Name;
                    docSalaryTab.DirBonus2IDSalary = DirBonus2IDSalary;

                    docSalaryTab.Sums = SumSalary + DirBonusIDSalary + DirBonus2IDSalary;

                    arrDocSalaryTabSQL[i] = docSalaryTab;


                    #endregion
                }
            }

            #endregion


            dynamic collectionWrapper = new
            {
                sucess = true,
                total = queryDirEmployees.Count(),
                DocSalaryTab = arrDocSalaryTabSQL
            };
            return await Task.Run(() => Ok(collectionWrapper));
            
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSalaryTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSalaryTab(int id, DocSalaryTab docSalaryTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSalaryTabs
        [ResponseType(typeof(DocSalaryTab))]
        public async Task<IHttpActionResult> PostDocSalaryTab(DocSalaryTab docSalaryTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSalaryTabs/5
        [ResponseType(typeof(DocSalaryTab))]
        public async Task<IHttpActionResult> DeleteDocSalaryTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region Mthods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocSalaryTabExists(int id)
        {
            return db.DocSalaryTabs.Count(e => e.DocSalaryTabID == id) > 0;
        }

        #endregion
    }
}