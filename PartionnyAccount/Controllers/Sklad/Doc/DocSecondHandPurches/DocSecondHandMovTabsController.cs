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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandPurches
{
    public class DocSecondHandMovTabsController : ApiController
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
            public int DocSecondHandMovID;
            public int DocID;
        }
        // GET: api/DocSecondHandMovTabs
        public async Task<IHttpActionResult> GetDocSecondHandMovTabs(HttpRequestMessage request)
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
                //Возможно доступна Логистика
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovementsLogistics"));
                    if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                }

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
                _params.DocSecondHandMovID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandMovID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docMovementTabs in db.DocSecondHandMovTabs

                        join dirServiceNomens11 in db.DirServiceNomens on docMovementTabs.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where docMovementTabs.DocSecondHandMovID == _params.DocSecondHandMovID

                        #region select

                        select new
                        {
                            DocSecondHandMovTabID = docMovementTabs.DocSecondHandMovTabID,
                            DocSecondHandMovID = docMovementTabs.DocSecondHandMovID,
                            DirServiceNomenID = docMovementTabs.DirServiceNomenID,

                            DocSecondHandPurchID = docMovementTabs.docSecondHandPurch.DocSecondHandPurchID,

                            //DirServiceNomenName = docMovementTabs.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? docMovementTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docMovementTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docMovementTabs.dirServiceNomen.DirServiceNomenName,

                            PriceVAT = docMovementTabs.PriceVAT,

                            DirCurrencyID = docMovementTabs.DirCurrencyID,
                            DirCurrencyRate = docMovementTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docMovementTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docMovementTabs.dirCurrency.DirCurrencyName + " (" + docMovementTabs.DirCurrencyRate + ", " + docMovementTabs.DirCurrencyMultiplicity + ")",


                            //Цена в т.в.
                            PriceCurrency = docMovementTabs.PriceCurrency,
                            //Себестоимость
                            SUMMovementPriceVATCurrency = docMovementTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docMovementTabs.PriceCurrency, sysSetting.FractionalPartInSum),

                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docMovementTabs.PriceRetailVAT - docMovementTabs.PriceVAT) / docMovementTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docMovementTabs.PriceRetailVAT - docMovementTabs.PriceVAT) / docMovementTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docMovementTabs.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docMovementTabs.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docMovementTabs.PriceWholesaleVAT - docMovementTabs.PriceVAT) / docMovementTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docMovementTabs.PriceWholesaleVAT - docMovementTabs.PriceVAT) / docMovementTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Оптовая цена 
                            PriceWholesaleVAT = docMovementTabs.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docMovementTabs.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docMovementTabs.PriceIMVAT - docMovementTabs.PriceVAT) / docMovementTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docMovementTabs.PriceIMVAT - docMovementTabs.PriceVAT) / docMovementTabs.PriceVAT, sysSetting.FractionalPartInSum)
                            ,
                            //Интернет-Магазин
                            PriceIMVAT = docMovementTabs.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docMovementTabs.PriceIMCurrency,

                            //Причина возврата
                            DirDescriptionID = docMovementTabs.DirDescriptionID,
                            DirDescriptionName = docMovementTabs.dirDescription.DirDescriptionName,
                            DirReturnTypeID = docMovementTabs.DirReturnTypeID,
                            DirReturnTypeName = docMovementTabs.dirReturnType.DirReturnTypeName
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSecondHandMovTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion


            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandMovTabs/5
        [ResponseType(typeof(DocSecondHandMovTab))]
        public async Task<IHttpActionResult> GetDocSecondHandMovTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandMovTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandMovTab(int id, DocSecondHandMovTab docSecondHandMovTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandMovTabs
        [ResponseType(typeof(DocSecondHandMovTab))]
        public async Task<IHttpActionResult> PostDocSecondHandMovTab(DocSecondHandMovTab docSecondHandMovTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSecondHandMovTabs/5
        [ResponseType(typeof(DocSecondHandMovTab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandMovTab(int id)
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

        private bool DocSecondHandMovTabExists(int id)
        {
            return db.DocSecondHandMovTabs.Count(e => e.DocSecondHandMovTabID == id) > 0;
        }

        #endregion


        #region SQL

        //Сумма документа
        public string GenerateSQLSUM(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL =
                "SELECT " +
                "COUNT(*) CountRecord, " +
                "COUNT(*) CountRecord_NumInWords, " +

                //Сумма С НДС в текущей валюте
                "round( SUM(DocSecondHandMovTabs.PriceRetailCurrency), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( SUM(DocSecondHandMovTabs.PriceRetailCurrency), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords' " + //Приписью

                "FROM Docs, DocSecondHandMovs, DocSecondHandMovTabs " +
                "WHERE " +
                "(Docs.DocID=DocSecondHandMovs.DocID)and" +
                "(DocSecondHandMovs.DocSecondHandMovID=DocSecondHandMovTabs.DocSecondHandMovID)and(Docs.DocID=@DocID)";

            return SQL;
        }

        //Спецификация
        public string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";


            SQL =
                "SELECT " +
                //"[Docs].[DocDate] AS [DocDate], " +
                //"[Docs].[DirVatValue] AS [DirVatValue], " +


                
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "CASE " +
                "WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN [dirServiceNomensSubGroup].[DirServiceNomenName] " +

                "WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END ELSE " +

                "CASE WHEN ([dirServiceNomensSubGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensSubGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([dirServiceNomensGroup].[DirServiceNomenName] IS NULL) THEN '' ELSE [dirServiceNomensGroup].[DirServiceNomenName] END || ' / ' || " +
                "CASE WHEN ([DirServiceNomens].[DirServiceNomenName] IS NULL) THEN '' ELSE [DirServiceNomens].[DirServiceNomenName] END END AS [DirServiceNomenName], " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                "[DirServiceNomens].[DirServiceNomenName] AS [DirServiceNomenNameRemove], " +
                "[DirServiceNomens].[DirServiceNomenNameFull] AS [DirServiceNomenNameFull], " +
                "[DirServiceNomens].[DescriptionFull] AS [DescriptionFull], " +

                //"[DocSecondHandMovTabs].[DocSecondHandMovTabID] AS [DocSecondHandMovTabID], " +
                //"[DocSecondHandMovTabs].[DocSecondHandMovID] AS [DocSecondHandMovID], " +
                "[DocSecondHandMovTabs].[DirServiceNomenID] AS [DirServiceNomenID], " +
                "[DocSecondHandMovTabs].[PriceVAT] AS [PriceVAT], " +
                //"[DocSecondHandMovTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocSecondHandMovTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandMovTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandMovTabs].[DirCurrencyRate] || ', ' || [DocSecondHandMovTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                
                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocSecondHandMovTabs].[PriceCurrency] AS [PriceCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocSecondHandMovTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocSecondHandMovTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +

                //Себестоимось прихода
                "ROUND([DocSecondHandMovTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocSecondHandMovTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SumPurchPriceCurrency], " +


                //Розница
                "[DocSecondHandMovTabs].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[DocSecondHandMovTabs].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость в валюте
                "ROUND([DocSecondHandMovTabs].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocSecondHandMovTabs].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " +

                //Опт
                "[DocSecondHandMovTabs].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[DocSecondHandMovTabs].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость в валюте
                "ROUND([DocSecondHandMovTabs].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocSecondHandMovTabs].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " +

                //Интернет-Магазин
                "[DocSecondHandMovTabs].[PriceIMVAT] AS [PriceIMVAT], " +
                "[DocSecondHandMovTabs].[PriceIMCurrency] AS [PriceIMCurrency], " +
                //Стоимость в валюте
                "ROUND([DocSecondHandMovTabs].[PriceIMVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMIMPriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocSecondHandMovTabs].[PriceIMCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMIMPriceCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocSecondHandMovs], [DocSecondHandMovTabs] " +
                
                "INNER JOIN [DirCurrencies] ON [DocSecondHandMovTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +


                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocSecondHandMovTabs].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [dirServiceNomensGroup].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocSecondHandMovs].[DocID])and([DocSecondHandMovs].[DocSecondHandMovID]=[DocSecondHandMovTabs].[DocSecondHandMovID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}