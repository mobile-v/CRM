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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocActOnWorks
{
    public class DocActOnWorkTabsController : ApiController
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
            public int DocActOnWorkID;
            public int DocID;
        }
        // GET: api/DocActOnWorkTabs
        public async Task<IHttpActionResult> GetDocActOnWorkTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActOnWorks"));
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
                _params.DocActOnWorkID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocActOnWorkID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docActOnWorkTabs in db.DocActOnWorkTabs

                        join dirNomens11 in db.DirNomens on docActOnWorkTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        where docActOnWorkTabs.DocActOnWorkID == _params.DocActOnWorkID

                        #region select

                        select new
                        {
                            DocActOnWorkTabID = docActOnWorkTabs.DocActOnWorkTabID,
                            DocActOnWorkID = docActOnWorkTabs.DocActOnWorkID,
                            DirNomenID = docActOnWorkTabs.DirNomenID,

                            //DirNomenName = docActOnWorkTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docActOnWorkTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docActOnWorkTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docActOnWorkTabs.dirNomen.DirNomenName,

                            Quantity = docActOnWorkTabs.Quantity,

                            DirPriceTypeID = docActOnWorkTabs.DirPriceTypeID,
                            DirPriceTypeName = docActOnWorkTabs.dirPriceType.DirPriceTypeName,

                            PriceVAT = docActOnWorkTabs.PriceVAT,

                            DirCurrencyID = docActOnWorkTabs.DirCurrencyID,
                            DirCurrencyRate = docActOnWorkTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docActOnWorkTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docActOnWorkTabs.dirCurrency.DirCurrencyName + " (" + docActOnWorkTabs.DirCurrencyRate + ", " + docActOnWorkTabs.DirCurrencyMultiplicity + ")",
                            
                            //Цена в т.в.
                            PriceCurrency = docActOnWorkTabs.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = docActOnWorkTabs.Quantity * docActOnWorkTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docActOnWorkTabs.Quantity * docActOnWorkTabs.PriceCurrency, sysSetting.FractionalPartInSum)
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocActOnWorkTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocActOnWorkTabs/5
        [ResponseType(typeof(DocActOnWorkTab))]
        public async Task<IHttpActionResult> GetDocActOnWorkTab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocActOnWorkTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocActOnWorkTab(int id, DocActOnWorkTab docActOnWorkTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocActOnWorkTabs
        [ResponseType(typeof(DocActOnWorkTab))]
        public async Task<IHttpActionResult> PostDocActOnWorkTab(DocActOnWorkTab docActOnWorkTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocActOnWorkTabs/5
        [ResponseType(typeof(DocActOnWorkTab))]
        public async Task<IHttpActionResult> DeleteDocActOnWorkTab(int id)
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

        private bool DocActOnWorkTabExists(int id)
        {
            return db.DocActOnWorkTabs.Count(e => e.DocActOnWorkTabID == id) > 0;
        }

        #endregion


        #region SQL

        //Сумма документа
        public string GenerateSQLSUM(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL =
                "SELECT " +
                "[Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], " +
                "Docs.Discount, " +
                "COUNT(*) CountRecord, " +
                "COUNT(*) CountRecord_NumInWords, " +
                "SUM(DocActOnWorkTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocActOnWorkTabs.Quantity * DocActOnWorkTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocActOnWorkTabs.Quantity * DocActOnWorkTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocActOnWorkTabs.Quantity * (DocActOnWorkTabs.PriceCurrency - (DocActOnWorkTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocActOnWorkTabs.Quantity * (DocActOnWorkTabs.PriceCurrency - (DocActOnWorkTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocActOnWorkTabs.Quantity * DocActOnWorkTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocActOnWorkTabs.Quantity * DocActOnWorkTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocActOnWorks, DocActOnWorkTabs " +
                "WHERE " +
                "(Docs.DocID=DocActOnWorks.DocID)and" +
                "(DocActOnWorks.DocActOnWorkID=DocActOnWorkTabs.DocActOnWorkID)and(Docs.DocID=@DocID)";

            return SQL;
        }

        //Спецификация
        public string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string
                Discount = "(1 - Docs.Discount / 100)", 
                SQL = "";


            SQL =
                "SELECT " +
                "[Docs].[DocDate] AS [DocDate], " +
                //НДС
                "[Docs].[DirVatValue] AS [DirVatValue], " +

                //"[DirNomens].[DirNomenName] AS [DirNomenName], " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "CASE " +
                "WHEN ([dirNomensGroup].[DirNomenName] IS NULL) THEN [dirNomensSubGroup].[DirNomenName] " +

                "WHEN ([DirNomens].[DirNomenName] IS NULL) THEN " +

                "CASE WHEN ([dirNomensSubGroup].[DirNomenName] IS NULL) THEN '' ELSE [dirNomensSubGroup].[DirNomenName] END || ' / ' || " +
                "CASE WHEN ([dirNomensGroup].[DirNomenName] IS NULL) THEN '' ELSE [dirNomensGroup].[DirNomenName] END ELSE " +

                "CASE WHEN ([dirNomensSubGroup].[DirNomenName] IS NULL) THEN '' ELSE [dirNomensSubGroup].[DirNomenName] END || ' / ' || " +
                "CASE WHEN ([dirNomensGroup].[DirNomenName] IS NULL) THEN '' ELSE [dirNomensGroup].[DirNomenName] END || ' / ' || " +
                "CASE WHEN ([DirNomens].[DirNomenName] IS NULL) THEN '' ELSE [DirNomens].[DirNomenName] END END AS [DirNomenName], " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "[DirNomens].[DirNomenName] AS [DirNomenNameRemove], " +
                "[DirNomens].[DirNomenArticle] AS [DirNomenArticle], " +
                "[DirNomens].[DirNomenMinimumBalance] AS [DirNomenMinimumBalance], " +
                "[DirNomens].[DirNomenNameFull] AS [DirNomenNameFull], " +
                "[DirNomens].[DescriptionFull] AS [DescriptionFull], " +
                //"[DirNomenGroups].[DirNomenName] AS [DirNomenGroupName], " + //Группа (Sub)

                "[DocActOnWorkTabs].[DocActOnWorkTabID] AS [DocActOnWorkTabID], " +
                "[DocActOnWorkTabs].[DocActOnWorkID] AS [DocActOnWorkID], " +
                "[DocActOnWorkTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocActOnWorkTabs].[Quantity] AS [Quantity], " +
                "[DocActOnWorkTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocActOnWorkTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocActOnWorkTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocActOnWorkTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocActOnWorkTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirPriceTypes].[DirPriceTypeName] AS [DirPriceTypeName], " +

                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocActOnWorkTabs].[DirCurrencyRate] || ', ' || [DocActOnWorkTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Цены и Суммы НДС=================================================================================
                //В валюте
                "ROUND([DocActOnWorkTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVAT', " +  //"Цена без НДС" в валюте
                "ROUND(([DocActOnWorkTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATDiscount', " +  //"Цена без НДС" в валюте со Скидкой

                "ROUND((DocActOnWorkTabs.[Quantity] * [DocActOnWorkTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVAT', " + //"Стоимость без НДС" в валюте
                "ROUND(((DocActOnWorkTabs.[Quantity] * [DocActOnWorkTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATDiscount', " + //"Стоимость без НДС" в валюте со Скидкой

                "ROUND([DocActOnWorkTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT', " +  //"Цена с НДС"  в валюте
                "ROUND([DocActOnWorkTabs].[PriceVAT]* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount', " +  //"Цена с НДС"  в валюте со Скидкой

                "ROUND([DocActOnWorkTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT_InWords', " +  //"Цена с НДС"  в валюте (словами)
                "ROUND([DocActOnWorkTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount_InWords', " +  //"Цена с НДС"  в валюте (словами) со Скидкой

                "ROUND(DocActOnWorkTabs.[Quantity] * [DocActOnWorkTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") 'SUMPriceVAT', " +  //"Стоимость с НДС" в валюте
                "ROUND(DocActOnWorkTabs.[Quantity] * [DocActOnWorkTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceVATDiscount', " +  //"Стоимость с НДС" в валюте со Скидкой


                //В текущей валюте
                "ROUND([DocActOnWorkTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrency', " +  //"Цена без НДС" в текущей валюте
                "ROUND(([DocActOnWorkTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrencyDiscount', " +  //"Цена без НДС" в текущей валюте со Скидкой

                "ROUND((DocActOnWorkTabs.[Quantity] * [DocActOnWorkTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrency', " + //"Стоимость без НДС" в текущей валюте
                "ROUND(((DocActOnWorkTabs.[Quantity] * [DocActOnWorkTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrencyDiscount', " + //"Стоимость без НДС" в текущей валюте со Скидкой

                "ROUND([DocActOnWorkTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency', " +  //"Цена с НДС" в текущей валюте
                "ROUND([DocActOnWorkTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount', " +  //"Цена с НДС" в текущей валюте со Скидкой

                "ROUND([DocActOnWorkTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency_InWords', " +  //"Цена с НДС" в текущей валюте (словами)
                "ROUND([DocActOnWorkTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount_InWords', " +  //"Цена с НДС" в текущей валюте (словами) со Скидкой

                "ROUND([DocActOnWorkTabs].[Quantity] * [DocActOnWorkTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrency', " + //Стоимость с НДС в текущей валюте
                "ROUND([DocActOnWorkTabs].[Quantity] * [DocActOnWorkTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " + //Стоимость с НДС в текущей валюте со Скидкой
                                                                                                                                                                              //Цены и Суммы НДС=================================================================================

                //"Сумма НДС" (НДС документа)
                "ROUND([DocActOnWorkTabs].[Quantity] * ([DocActOnWorkTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                "ROUND(([DocActOnWorkTabs].[Quantity] * ([DocActOnWorkTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'SumVatValueDiscount', " +  //Сумма НДС (НДС документа)



                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocActOnWorks], [DocActOnWorkTabs] " +

                //"INNER JOIN [DirNomens] ON [DocActOnWorkTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                //"LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocActOnWorkTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirPriceTypes] ON [DocActOnWorkTabs].[DirPriceTypeID] = [DirPriceTypes].[DirPriceTypeID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocActOnWorkTabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocActOnWorks].[DocID])and([DocActOnWorks].[DocActOnWorkID]=[DocActOnWorkTabs].[DocActOnWorkID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}