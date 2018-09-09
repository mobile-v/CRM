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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocReturnVendors
{
    public class DocReturnVendorTabsController : ApiController
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
            public int DocReturnVendorID;
            public int DocID;
        }
        // GET: api/DocReturnVendorTabs
        public async Task<IHttpActionResult> GetDocReturnVendorTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnVendors"));
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
                _params.DocReturnVendorID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocReturnVendorID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docReturnVendorTabs in db.DocReturnVendorTabs

                        join dirNomens11 in db.DirNomens on docReturnVendorTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики
                        join dirCharColours1 in db.DirCharColours on docReturnVendorTabs.remParty.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on docReturnVendorTabs.remParty.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on docReturnVendorTabs.remParty.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on docReturnVendorTabs.remParty.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on docReturnVendorTabs.remParty.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on docReturnVendorTabs.remParty.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on docReturnVendorTabs.remParty.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on docReturnVendorTabs.remParty.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        #endregion

                        where docReturnVendorTabs.DocReturnVendorID == _params.DocReturnVendorID

                        #region select

                        select new
                        {
                            DocReturnVendorTabID = docReturnVendorTabs.DocReturnVendorTabID,
                            DocReturnVendorID = docReturnVendorTabs.DocReturnVendorID,
                            DirNomenID = docReturnVendorTabs.DirNomenID,

                            //DirNomenName = docReturnVendorTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docReturnVendorTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docReturnVendorTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docReturnVendorTabs.dirNomen.DirNomenName,

                            RemPartyID = docReturnVendorTabs.RemPartyID,
                            Quantity = docReturnVendorTabs.Quantity,

                            PriceVAT = docReturnVendorTabs.PriceVAT,

                            DirCurrencyID = docReturnVendorTabs.DirCurrencyID,
                            DirCurrencyRate = docReturnVendorTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docReturnVendorTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docReturnVendorTabs.dirCurrency.DirCurrencyName + " (" + docReturnVendorTabs.DirCurrencyRate + ", " + docReturnVendorTabs.DirCurrencyMultiplicity + ")",

                            //RemParty
                            Barcode = docReturnVendorTabs.remParty.Barcode,
                            SerialNumber = docReturnVendorTabs.remParty.SerialNumber,

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
                            PriceCurrency = docReturnVendorTabs.PriceCurrency,
                            //Себестоимость
                            SUMReturnVendorPriceVATCurrency = docReturnVendorTabs.Quantity * docReturnVendorTabs.PriceCurrency == null ? 0
                            : Math.Round(docReturnVendorTabs.Quantity * docReturnVendorTabs.PriceCurrency, sysSetting.FractionalPartInSum)
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocReturnVendorTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocReturnVendorTabs/5
        [ResponseType(typeof(DocReturnVendorTab))]
        public async Task<IHttpActionResult> GetDocReturnVendorTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocReturnVendorTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocReturnVendorTab(int id, DocReturnVendorTab docReturnVendorTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocReturnVendorTabs
        [ResponseType(typeof(DocReturnVendorTab))]
        public async Task<IHttpActionResult> PostDocReturnVendorTab(DocReturnVendorTab docReturnVendorTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocReturnVendorTabs/5
        [ResponseType(typeof(DocReturnVendorTab))]
        public async Task<IHttpActionResult> DeleteDocReturnVendorTab(int id)
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

        private bool DocReturnVendorTabExists(int id)
        {
            return db.DocReturnVendorTabs.Count(e => e.DocReturnVendorTabID == id) > 0;
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
                "SUM(DocReturnVendorTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocReturnVendorTabs.Quantity * DocReturnVendorTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocReturnVendorTabs.Quantity * DocReturnVendorTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocReturnVendorTabs.Quantity * (DocReturnVendorTabs.PriceCurrency - (DocReturnVendorTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocReturnVendorTabs.Quantity * (DocReturnVendorTabs.PriceCurrency - (DocReturnVendorTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocReturnVendorTabs.Quantity * DocReturnVendorTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocReturnVendorTabs.Quantity * DocReturnVendorTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocReturnVendors, DocReturnVendorTabs " +
                "WHERE " +
                "(Docs.DocID=DocReturnVendors.DocID)and" +
                "(DocReturnVendors.DocReturnVendorID=DocReturnVendorTabs.DocReturnVendorID)and(Docs.DocID=@DocID)";

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

                "[DocReturnVendorTabs].[DocReturnVendorTabID] AS [DocReturnVendorTabID], " +
                "[DocReturnVendorTabs].[DocReturnVendorID] AS [DocReturnVendorID], " +
                "[DocReturnVendorTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocReturnVendorTabs].[Quantity] AS [Quantity], " +
                "[DocReturnVendorTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocReturnVendorTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocReturnVendorTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocReturnVendorTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocReturnVendorTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DocReturnVendorTabs].[PriceCurrency] AS [PriceCurrency], " +


                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocReturnVendorTabs].[DirCurrencyRate] || ', ' || [DocReturnVendorTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
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
                "ROUND([DocReturnVendorTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVAT', " +  //"Цена без НДС" в валюте
                "ROUND(([DocReturnVendorTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATDiscount', " +  //"Цена без НДС" в валюте со Скидкой

                "ROUND((DocReturnVendorTabs.[Quantity] * [DocReturnVendorTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVAT', " + //"Стоимость без НДС" в валюте
                "ROUND(((DocReturnVendorTabs.[Quantity] * [DocReturnVendorTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATDiscount', " + //"Стоимость без НДС" в валюте со Скидкой

                "ROUND([DocReturnVendorTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT', " +  //"Цена с НДС"  в валюте
                "ROUND([DocReturnVendorTabs].[PriceVAT]* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount', " +  //"Цена с НДС"  в валюте со Скидкой

                "ROUND([DocReturnVendorTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT_InWords', " +  //"Цена с НДС"  в валюте (словами)
                "ROUND([DocReturnVendorTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount_InWords', " +  //"Цена с НДС"  в валюте (словами) со Скидкой

                "ROUND(DocReturnVendorTabs.[Quantity] * [DocReturnVendorTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") 'SUMPriceVAT', " +  //"Стоимость с НДС" в валюте
                "ROUND(DocReturnVendorTabs.[Quantity] * [DocReturnVendorTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceVATDiscount', " +  //"Стоимость с НДС" в валюте со Скидкой


                //В текущей валюте
                "ROUND([DocReturnVendorTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrency', " +  //"Цена без НДС" в текущей валюте
                "ROUND(([DocReturnVendorTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrencyDiscount', " +  //"Цена без НДС" в текущей валюте со Скидкой

                "ROUND((DocReturnVendorTabs.[Quantity] * [DocReturnVendorTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrency', " + //"Стоимость без НДС" в текущей валюте
                "ROUND(((DocReturnVendorTabs.[Quantity] * [DocReturnVendorTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrencyDiscount', " + //"Стоимость без НДС" в текущей валюте со Скидкой

                "ROUND([DocReturnVendorTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency', " +  //"Цена с НДС" в текущей валюте
                "ROUND([DocReturnVendorTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount', " +  //"Цена с НДС" в текущей валюте со Скидкой

                "ROUND([DocReturnVendorTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency_InWords', " +  //"Цена с НДС" в текущей валюте (словами)
                "ROUND([DocReturnVendorTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount_InWords', " +  //"Цена с НДС" в текущей валюте (словами) со Скидкой

                "ROUND([DocReturnVendorTabs].[Quantity] * [DocReturnVendorTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrency', " + //Стоимость с НДС в текущей валюте
                "ROUND([DocReturnVendorTabs].[Quantity] * [DocReturnVendorTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " + //Стоимость с НДС в текущей валюте со Скидкой
                                                                                                                                                                              //Цены и Суммы НДС=================================================================================

                //"Сумма НДС" (НДС документа)
                "ROUND([DocReturnVendorTabs].[Quantity] * ([DocReturnVendorTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                "ROUND(([DocReturnVendorTabs].[Quantity] * ([DocReturnVendorTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100)) * [Docs].[Discount], " + sysSettings.FractionalPartInPrice + ") 'SumVatValueDiscount', " +  //Сумма НДС (НДС документа)



                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocReturnVendors], [DocReturnVendorTabs] " +

                "LEFT OUTER JOIN [RemParties] AS [RemParties] ON [RemParties].[RemPartyID] = [DocReturnVendorTabs].[RemPartyID] " +

                "LEFT OUTER JOIN [DirCharColours] AS [DirCharColours] ON [DirCharColours].[DirCharColourID] = [RemParties].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] AS [DirCharMaterials] ON [DirCharMaterials].[DirCharMaterialID] = [RemParties].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] AS [DirCharNames] ON [DirCharNames].[DirCharNameID] = [RemParties].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] AS [DirCharSeasons] ON [DirCharSeasons].[DirCharSeasonID] = [RemParties].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] AS [DirCharSexes] ON [DirCharSexes].[DirCharSexID] = [RemParties].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] AS [DirCharSizes] ON [DirCharSizes].[DirCharSizeID] = [RemParties].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] AS [DirCharStyles] ON [DirCharStyles].[DirCharStyleID] = [RemParties].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] AS [DirCharTextures] ON [DirCharTextures].[DirCharTextureID] = [RemParties].[DirCharTextureID] " +

                //"INNER JOIN [DirNomens] ON [DocReturnVendorTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                //"LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocReturnVendorTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocReturnVendorTabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocReturnVendors].[DocID])and([DocReturnVendors].[DocReturnVendorID]=[DocReturnVendorTabs].[DocReturnVendorID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}