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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandInvs
{
    public class DocSecondHandInvTabsController : ApiController
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
            public int DocSecondHandInvID;
            public int DocID;
        }
        // GET: api/DocSecondHandInvTabs
        public async Task<IHttpActionResult> GetDocSecondHandInvTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandInventories"));
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
                _params.DocSecondHandInvID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandInvID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docSecondHandInvTabs in db.DocSecondHandInvTabs

                        join dirServiceNomens11 in db.DirServiceNomens on docSecondHandInvTabs.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where docSecondHandInvTabs.DocSecondHandInvID == _params.DocSecondHandInvID

                        #region select

                        select new
                        {
                            DocSecondHandPurchID = docSecondHandInvTabs.DocSecondHandPurchID,
                            DocDate = docSecondHandInvTabs.docSecondHandInv.doc.DocDate,


                            DocSecondHandInvTabID = docSecondHandInvTabs.DocSecondHandInvTabID,
                            DocSecondHandInvID = docSecondHandInvTabs.DocSecondHandInvID,
                            DirServiceNomenID = docSecondHandInvTabs.DirServiceNomenID,

                            //DirServiceNomenName = docSecondHandInvTabs.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? docSecondHandInvTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docSecondHandInvTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docSecondHandInvTabs.dirServiceNomen.DirServiceNomenName,

                            PriceVAT = docSecondHandInvTabs.PriceVAT,
                            PriceRetailVAT = docSecondHandInvTabs.docSecondHandPurch.PriceRetailVAT,

                            DirCurrencyID = docSecondHandInvTabs.DirCurrencyID,
                            DirCurrencyRate = docSecondHandInvTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSecondHandInvTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docSecondHandInvTabs.dirCurrency.DirCurrencyName + " (" + docSecondHandInvTabs.DirCurrencyRate + ", " + docSecondHandInvTabs.DirCurrencyMultiplicity + ")",

                            Exist = docSecondHandInvTabs.Exist,


                            DirSecondHandStatusName = docSecondHandInvTabs.dirSecondHandStatus.DirSecondHandStatusName,

                            //Exist
                            /*
                            ExistName = docSecondHandInvTabs.Exist == 1 ? "Присутствует"
                            :
                            docSecondHandInvTabs.Exist == 2 ? "Списывается с ЗП"
                            :
                            docSecondHandInvTabs.Exist == 4 ? "Отсутствует"
                            :
                            "На разбор",
                            */
                            ExistName = docSecondHandInvTabs.Exist == 1 ? "Присутствует"
                            :
                            docSecondHandInvTabs.Exist == 2 ? "Отсутствует"
                            :
                            "????????????????",

                            //Цена в т.в.
                            PriceRetailCurrency = docSecondHandInvTabs.docSecondHandPurch.PriceRetailCurrency,
                            PriceCurrency = docSecondHandInvTabs.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = docSecondHandInvTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docSecondHandInvTabs.PriceCurrency, sysSetting.FractionalPartInSum)
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSecondHandInvTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandInvTabs/5
        [ResponseType(typeof(DocSecondHandInvTab))]
        public async Task<IHttpActionResult> GetDocSecondHandInvTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandInvTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandInvTab(int id, DocSecondHandInvTab docSecondHandInvTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandInvTabs
        [ResponseType(typeof(DocSecondHandInvTab))]
        public async Task<IHttpActionResult> PostDocSecondHandInvTab(DocSecondHandInvTab docSecondHandInvTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSecondHandInvTabs/5
        [ResponseType(typeof(DocSecondHandInvTab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandInvTab(int id)
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

        private bool DocSecondHandInvTabExists(int id)
        {
            return db.DocSecondHandInvTabs.Count(e => e.DocSecondHandInvTabID == id) > 0;
        }

        #endregion


        #region SQL

        //Спецификация
        public string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings, int Exist)
        {
            string SQL = "";

            SQL =
                "SELECT " +

                "[Docs].[DocDate] AS [DocDate], " +

                //Номер документа
                "[DocSecondHandInvTabs].DocSecondHandPurchID AS [DocSecondHandPurchID], " +


                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                //"[DirServiceNomens].[DirServiceNomenName] AS [DirServiceNomenName], " +
                "CASE " +
                "WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN [dirServiceNomensSubGroup].[DirServiceNomenName] " +

                "WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END ELSE " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN '' ELSE [DirServiceNomens].[DirServiceNomenName] END END AS [DirServiceNomenName], " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

                "[DocSecondHandInvTabs].[DirServiceNomenID] AS [DirServiceNomenID], " +
                "[DocSecondHandInvTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandInvTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocSecondHandInvTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandInvTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +

                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandInvTabs].[DirCurrencyRate] || ', ' || [DocSecondHandInvTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Цены и Суммы НДС=================================================================================
                //В валюте
                "ROUND([DocSecondHandInvTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT', " +  //"Цена с НДС"  в валюте
                "ROUND([DocSecondHandInvTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT_InWords', " +  //"Цена с НДС"  в валюте (словами)
                "ROUND([DocSecondHandInvTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") 'SUMPriceVAT', " +  //"Стоимость с НДС" в валюте


                //В текущей валюте
                "ROUND([DocSecondHandInvTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency', " +  //"Цена с НДС" в текущей валюте
                "ROUND([DocSecondHandInvTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency_InWords', " +  //"Цена с НДС" в текущей валюте (словами)
                "ROUND([DocSecondHandInvTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrency', " + //Стоимость с НДС в текущей валюте
                                                                                                                                                                                //Цены и Суммы НДС=================================================================================


                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocSecondHandInvs], [DocSecondHandInvTabs] " +

                "INNER JOIN [DirCurrencies] ON [DocSecondHandInvTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                //"INNER JOIN [DirPriceTypes] ON [DocSecondHandInvTabs].[DirPriceTypeID] = [DirPriceTypes].[DirPriceTypeID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocSecondHandInvTabs].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [dirServiceNomensGroup].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE " +
                "([Docs].[DocID]=[DocSecondHandInvs].[DocID])and([DocSecondHandInvs].[DocSecondHandInvID]=[DocSecondHandInvTabs].[DocSecondHandInvID])and(Docs.DocID=@DocID)and(DocSecondHandInvTabs.Exist=" + Exist.ToString() + ") ";


            return SQL;
        }

        #endregion

    }
}