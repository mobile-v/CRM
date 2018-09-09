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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSales
{
    public class DocSaleTabsController : ApiController
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
            public int DocSaleID;
            public int DocID;
        }
        // GET: api/DocSaleTabs
        public async Task<IHttpActionResult> GetDocSaleTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSales"));
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
                _params.DocSaleID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSaleID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docSaleTabs in db.DocSaleTabs

                        join dirNomens11 in db.DirNomens on docSaleTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики
                        join dirCharColours1 in db.DirCharColours on docSaleTabs.remParty.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on docSaleTabs.remParty.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on docSaleTabs.remParty.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on docSaleTabs.remParty.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on docSaleTabs.remParty.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on docSaleTabs.remParty.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on docSaleTabs.remParty.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on docSaleTabs.remParty.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        #endregion

                        where docSaleTabs.DocSaleID == _params.DocSaleID

                        #region select

                        select new
                        {
                            DocSaleTabID = docSaleTabs.DocSaleTabID,
                            DocSaleID = docSaleTabs.DocSaleID,
                            DirNomenID = docSaleTabs.DirNomenID,

                            //DirNomenName = docSaleTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docSaleTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docSaleTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docSaleTabs.dirNomen.DirNomenName,

                            RemPartyID = docSaleTabs.RemPartyID,
                            Quantity = docSaleTabs.Quantity,

                            DirPriceTypeID = docSaleTabs.DirPriceTypeID,
                            DirPriceTypeName = docSaleTabs.dirPriceType.DirPriceTypeName,

                            PriceVAT = docSaleTabs.PriceVAT,

                            DirCurrencyID = docSaleTabs.DirCurrencyID,
                            DirCurrencyRate = docSaleTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSaleTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docSaleTabs.dirCurrency.DirCurrencyName + " (" + docSaleTabs.DirCurrencyRate + ", " + docSaleTabs.DirCurrencyMultiplicity + ")",

                            //RemParty
                            Barcode = docSaleTabs.remParty.Barcode,
                            SerialNumber = docSaleTabs.remParty.SerialNumber,

                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureName = dirCharTextures.DirCharTextureName,
                            DirChar =
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,

                            //Цена в т.в.
                            PriceCurrency = docSaleTabs.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = docSaleTabs.Quantity * docSaleTabs.PriceCurrency == null ? 0
                            : Math.Round(docSaleTabs.Quantity * docSaleTabs.PriceCurrency, sysSetting.FractionalPartInSum)
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSaleTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSaleTabs/5
        [ResponseType(typeof(DocSaleTab))]
        public async Task<IHttpActionResult> GetDocSaleTab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSaleTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSaleTab(int id, DocSaleTab docSaleTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSaleTabs
        [ResponseType(typeof(DocSaleTab))]
        public async Task<IHttpActionResult> PostDocSaleTab(DocSaleTab docSaleTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSaleTabs/5
        [ResponseType(typeof(DocSaleTab))]
        public async Task<IHttpActionResult> DeleteDocSaleTab(int id)
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

        private bool DocSaleTabExists(int id)
        {
            return db.DocSaleTabs.Count(e => e.DocSaleTabID == id) > 0;
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
                "SUM(DocSaleTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocSaleTabs.Quantity * DocSaleTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocSaleTabs.Quantity * DocSaleTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocSaleTabs.Quantity * (DocSaleTabs.PriceCurrency - (DocSaleTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocSaleTabs.Quantity * (DocSaleTabs.PriceCurrency - (DocSaleTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocSaleTabs.Quantity * DocSaleTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocSaleTabs.Quantity * DocSaleTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocSales, DocSaleTabs " +
                "WHERE " +
                "(Docs.DocID=DocSales.DocID)and" +
                "(DocSales.DocSaleID=DocSaleTabs.DocSaleID)and(Docs.DocID=@DocID)";

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

                "[DocSaleTabs].[DocSaleTabID] AS [DocSaleTabID], " +
                "[DocSaleTabs].[DocSaleID] AS [DocSaleID], " +
                "[DocSaleTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocSaleTabs].[Quantity] AS [Quantity], " +
                "[DocSaleTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocSaleTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSaleTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocSaleTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSaleTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirPriceTypes].[DirPriceTypeName] AS [DirPriceTypeName], " +

                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSaleTabs].[DirCurrencyRate] || ', ' || [DocSaleTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +


                "[RemParties].Barcode AS Barcode, " + 
                "[RemParties].SerialNumber AS SerialNumber, " +

                "[DirCharColours].DirCharColourName AS DirCharColourName, " +
                "[DirCharMaterials].DirCharMaterialName AS DirCharMaterialName, " +
                "[DirCharNames].DirCharNameName AS DirCharNameName, " +
                "[DirCharSeasons].DirCharSeasonName AS DirCharSeasonName, " +
                "[DirCharSexes].DirCharSexName AS DirCharSexName, " +
                "[DirCharSizes].DirCharSizeName AS DirCharSizeName, " +
                "[DirCharStyles].DirCharStyleName AS DirCharStyleName, " +
                "[DirCharTextures].DirCharTextureName AS DirCharTextureName, " +


                //Цены и Суммы НДС=================================================================================
                //В валюте
                "ROUND([DocSaleTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVAT', " +  //"Цена без НДС" в валюте
                "ROUND(([DocSaleTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATDiscount', " +  //"Цена без НДС" в валюте со Скидкой

                "ROUND((DocSaleTabs.[Quantity] * [DocSaleTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVAT', " + //"Стоимость без НДС" в валюте
                "ROUND(((DocSaleTabs.[Quantity] * [DocSaleTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATDiscount', " + //"Стоимость без НДС" в валюте со Скидкой

                "ROUND([DocSaleTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT', " +  //"Цена с НДС"  в валюте
                "ROUND([DocSaleTabs].[PriceVAT]* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount', " +  //"Цена с НДС"  в валюте со Скидкой

                "ROUND([DocSaleTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT_InWords', " +  //"Цена с НДС"  в валюте (словами)
                "ROUND([DocSaleTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount_InWords', " +  //"Цена с НДС"  в валюте (словами) со Скидкой

                "ROUND(DocSaleTabs.[Quantity] * [DocSaleTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") 'SUMPriceVAT', " +  //"Стоимость с НДС" в валюте
                "ROUND(DocSaleTabs.[Quantity] * [DocSaleTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceVATDiscount', " +  //"Стоимость с НДС" в валюте со Скидкой


                //В текущей валюте
                "ROUND([DocSaleTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrency', " +  //"Цена без НДС" в текущей валюте
                "ROUND(([DocSaleTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrencyDiscount', " +  //"Цена без НДС" в текущей валюте со Скидкой

                "ROUND((DocSaleTabs.[Quantity] * [DocSaleTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrency', " + //"Стоимость без НДС" в текущей валюте
                "ROUND(((DocSaleTabs.[Quantity] * [DocSaleTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100))* [Docs].[Discount], " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrencyDiscount', " + //"Стоимость без НДС" в текущей валюте со Скидкой

                "ROUND([DocSaleTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency', " +  //"Цена с НДС" в текущей валюте
                "ROUND([DocSaleTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount', " +  //"Цена с НДС" в текущей валюте со Скидкой

                "ROUND([DocSaleTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency_InWords', " +  //"Цена с НДС" в текущей валюте (словами)
                "ROUND([DocSaleTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount_InWords', " +  //"Цена с НДС" в текущей валюте (словами) со Скидкой

                "ROUND([DocSaleTabs].[Quantity] * [DocSaleTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrency', " + //Стоимость с НДС в текущей валюте
                "ROUND([DocSaleTabs].[Quantity] * [DocSaleTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " + //Стоимость с НДС в текущей валюте со Скидкой
                                                                                                                                                                              //Цены и Суммы НДС=================================================================================

                //"Сумма НДС" (НДС документа)
                "ROUND([DocSaleTabs].[Quantity] * ([DocSaleTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                "ROUND(([DocSaleTabs].[Quantity] * ([DocSaleTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'SumVatValueDiscount', " +  //Сумма НДС (НДС документа)



                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocSales], [DocSaleTabs] " +

                "LEFT OUTER JOIN [RemParties] AS [RemParties] ON [RemParties].[RemPartyID] = [DocSaleTabs].[RemPartyID] " +

                "LEFT OUTER JOIN [DirCharColours] AS [DirCharColours] ON [DirCharColours].[DirCharColourID] = [RemParties].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] AS [DirCharMaterials] ON [DirCharMaterials].[DirCharMaterialID] = [RemParties].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] AS [DirCharNames] ON [DirCharNames].[DirCharNameID] = [RemParties].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] AS [DirCharSeasons] ON [DirCharSeasons].[DirCharSeasonID] = [RemParties].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] AS [DirCharSexes] ON [DirCharSexes].[DirCharSexID] = [RemParties].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] AS [DirCharSizes] ON [DirCharSizes].[DirCharSizeID] = [RemParties].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] AS [DirCharStyles] ON [DirCharStyles].[DirCharStyleID] = [RemParties].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] AS [DirCharTextures] ON [DirCharTextures].[DirCharTextureID] = [RemParties].[DirCharTextureID] " +

                //"INNER JOIN [DirNomens] ON [DocSaleTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                //"LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocSaleTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirPriceTypes] ON [DocSaleTabs].[DirPriceTypeID] = [DirPriceTypes].[DirPriceTypeID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocSaleTabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocSales].[DocID])and([DocSales].[DocSaleID]=[DocSaleTabs].[DocSaleID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}