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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandInventories
{
    public class DocSecondHandInventoryTabsController : ApiController
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
            public int DocSecondHandInventoryID;
            public int DocID;
        }
        // GET: api/DocSecondHandInventoryTabs
        public async Task<IHttpActionResult> GetDocSecondHandInventoryTabs(HttpRequestMessage request)
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
                _params.DocSecondHandInventoryID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandInventoryID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docSecondHandInventoryTabs in db.DocSecondHandInventoryTabs
                        
                        join dirServiceNomens11 in db.DirServiceNomens on docSecondHandInventoryTabs.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()


                        join rem2Parties11 in db.Rem2Parties on docSecondHandInventoryTabs.Rem2PartyID equals rem2Parties11.Rem2PartyID into rem2Parties12
                        from rem2Parties in rem2Parties12.DefaultIfEmpty()

                        join docSecondHandPurches11 in db.DocSecondHandPurches on rem2Parties.DocIDFirst equals docSecondHandPurches11.DocID into docSecondHandPurches12
                        from docSecondHandPurches in docSecondHandPurches12.DefaultIfEmpty()


                        where docSecondHandInventoryTabs.DocSecondHandInventoryID == _params.DocSecondHandInventoryID

                        #region select

                        select new
                        {
                            DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,
                            DocDate = rem2Parties.doc.DocDate,


                            DocSecondHandInventoryTabID = docSecondHandInventoryTabs.DocSecondHandInventoryTabID,
                            DocSecondHandInventoryID = docSecondHandInventoryTabs.DocSecondHandInventoryID,
                            DirServiceNomenID = docSecondHandInventoryTabs.DirServiceNomenID,

                            //DirServiceNomenName = docSecondHandInventoryTabs.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? docSecondHandInventoryTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docSecondHandInventoryTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docSecondHandInventoryTabs.dirServiceNomen.DirServiceNomenName,

                            Rem2PartyID = docSecondHandInventoryTabs.Rem2PartyID,
                            Quantity = docSecondHandInventoryTabs.Quantity,

                            //DirPriceTypeID = docSecondHandInventoryTabs.DirPriceTypeID,
                            //DirPriceTypeName = docSecondHandInventoryTabs.dirPriceType.DirPriceTypeName,

                            PriceVAT = docSecondHandInventoryTabs.PriceVAT,

                            DirCurrencyID = docSecondHandInventoryTabs.DirCurrencyID,
                            DirCurrencyRate = docSecondHandInventoryTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSecondHandInventoryTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docSecondHandInventoryTabs.dirCurrency.DirCurrencyName + " (" + docSecondHandInventoryTabs.DirCurrencyRate + ", " + docSecondHandInventoryTabs.DirCurrencyMultiplicity + ")",

                            Exist = docSecondHandInventoryTabs.Exist,

                            //Exist
                            ExistName = docSecondHandInventoryTabs.Exist == 1 ? "Присутствует"
                            :
                            docSecondHandInventoryTabs.Exist == 2 ? "Списывается с ЗП"
                            :
                            "На разбор",


                            //Цена в т.в.
                            PriceCurrency = docSecondHandInventoryTabs.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = docSecondHandInventoryTabs.Quantity * docSecondHandInventoryTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docSecondHandInventoryTabs.Quantity * docSecondHandInventoryTabs.PriceCurrency, sysSetting.FractionalPartInSum)
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSecondHandInventoryTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandInventoryTabs/5
        [ResponseType(typeof(DocSecondHandInventoryTab))]
        public async Task<IHttpActionResult> GetDocSecondHandInventoryTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandInventoryTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandInventoryTab(int id, DocSecondHandInventoryTab docSecondHandInventoryTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandInventoryTabs
        [ResponseType(typeof(DocSecondHandInventoryTab))]
        public async Task<IHttpActionResult> PostDocSecondHandInventoryTab(DocSecondHandInventoryTab docSecondHandInventoryTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSecondHandInventoryTabs/5
        [ResponseType(typeof(DocSecondHandInventoryTab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandInventoryTab(int id)
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

        private bool DocSecondHandInventoryTabExists(int id)
        {
            return db.DocSecondHandInventoryTabs.Count(e => e.DocSecondHandInventoryTabID == id) > 0;
        }

        #endregion


        #region SQL
        
        //Спецификация
        public string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings, int Exist)
        {
            string SQL = "";


            //join rem2Parties11 in db.Rem2Parties on docSecondHandInventoryTabs.Rem2PartyID equals rem2Parties11.Rem2PartyID into rem2Parties12
            //from rem2Parties in rem2Parties12.DefaultIfEmpty()

            //join docSecondHandPurches11 in db.DocSecondHandPurches on rem2Parties.DocIDFirst equals docSecondHandPurches11.DocID into docSecondHandPurches12
            //from docSecondHandPurches in docSecondHandPurches12.DefaultIfEmpty()


            SQL =
                "SELECT " +
                
                "[Docs].[DocDate] AS [DocDate], " +

                //Номер документа
                "[DocSecondHandPurches].DocSecondHandPurchID AS [DocSecondHandPurchID], " +


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


                //"[DirServiceNomens].[DirServiceNomenName] AS [DirServiceNomenNameRemove], " +
                //"[DirServiceNomens].[DirServiceNomenArticle] AS [DirServiceNomenArticle], " +
                //"[DirServiceNomens].[DirServiceNomenMinimumBalance] AS [DirServiceNomenMinimumBalance], " +
                //"[DirServiceNomens].[DirServiceNomenNameFull] AS [DirServiceNomenNameFull], " +
                //"[DirServiceNomens].[DescriptionFull] AS [DescriptionFull], " +
                //"[DirServiceNomenGroups].[DirServiceNomenName] AS [DirServiceNomenGroupName], " + //Группа (Sub)

                //"[DocSecondHandInventoryTabs].[DocSecondHandInventoryTabID] AS [DocSecondHandInventoryTabID], " +
                //"[DocSecondHandInventoryTabs].[DocSecondHandInventoryID] AS [DocSecondHandInventoryID], " +
                "[DocSecondHandInventoryTabs].[DirServiceNomenID] AS [DirServiceNomenID], " +
                "[DocSecondHandInventoryTabs].[Quantity] AS [Quantity], " +
                "[DocSecondHandInventoryTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocSecondHandInventoryTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandInventoryTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocSecondHandInventoryTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandInventoryTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                //"[DirPriceTypes].[DirPriceTypeName] AS [DirPriceTypeName], " +

                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandInventoryTabs].[DirCurrencyRate] || ', ' || [DocSecondHandInventoryTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +
                
                //Цены и Суммы НДС=================================================================================
                //В валюте
                "ROUND([DocSecondHandInventoryTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT', " +  //"Цена с НДС"  в валюте
                "ROUND([DocSecondHandInventoryTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT_InWords', " +  //"Цена с НДС"  в валюте (словами)
                "ROUND(DocSecondHandInventoryTabs.[Quantity] * [DocSecondHandInventoryTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") 'SUMPriceVAT', " +  //"Стоимость с НДС" в валюте


                //В текущей валюте
                "ROUND([DocSecondHandInventoryTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency', " +  //"Цена с НДС" в текущей валюте
                "ROUND([DocSecondHandInventoryTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency_InWords', " +  //"Цена с НДС" в текущей валюте (словами)
                "ROUND([DocSecondHandInventoryTabs].[Quantity] * [DocSecondHandInventoryTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrency', " + //Стоимость с НДС в текущей валюте
                //Цены и Суммы НДС=================================================================================


                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocSecondHandInventories], [DocSecondHandInventoryTabs] " +
                
                "INNER JOIN [DirCurrencies] ON [DocSecondHandInventoryTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                //"INNER JOIN [DirPriceTypes] ON [DocSecondHandInventoryTabs].[DirPriceTypeID] = [DirPriceTypes].[DirPriceTypeID] " +

                //Получаем ID-шник документа
                "LEFT JOIN [Rem2Parties] AS [Rem2Parties] ON [Rem2Parties].[Rem2PartyID] = [DocSecondHandInventoryTabs].[Rem2PartyID] " +
                "LEFT JOIN [DocSecondHandPurches] AS [DocSecondHandPurches] ON [DocSecondHandPurches].[DocID] = [Rem2Parties].[DocIDFirst] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocSecondHandInventoryTabs].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [dirServiceNomensGroup].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE " +
                "([Docs].[DocID]=[DocSecondHandInventories].[DocID])and([DocSecondHandInventories].[DocSecondHandInventoryID]=[DocSecondHandInventoryTabs].[DocSecondHandInventoryID])and(Docs.DocID=@DocID)and(DocSecondHandInventoryTabs.Exist="+ Exist.ToString() + ") ";


            return SQL;
        }
        
        #endregion
    }
}