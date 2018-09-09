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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocReturnsCustomers
{
    public class DocReturnsCustomerTabsController : ApiController
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
            public int DocReturnsCustomerID;
            public int DocID;
        }
        // GET: api/DocReturnsCustomerTabs
        public async Task<IHttpActionResult> GetDocReturnsCustomerTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocReturnsCustomers"));
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
                _params.DocReturnsCustomerID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocReturnsCustomerID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docReturnsCustomerTabs in db.DocReturnsCustomerTabs

                        join dirNomens11 in db.DirNomens on docReturnsCustomerTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        /*
                        //Характеристики
                        join dirCharColours1 in db.DirCharColours on docReturnsCustomerTabs.remPartyMinus.remParty.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on docReturnsCustomerTabs.remPartyMinus.remParty.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on docReturnsCustomerTabs.remPartyMinus.remParty.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on docReturnsCustomerTabs.remPartyMinus.remParty.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on docReturnsCustomerTabs.remPartyMinus.remParty.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on docReturnsCustomerTabs.remPartyMinus.remParty.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on docReturnsCustomerTabs.remPartyMinus.remParty.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on docReturnsCustomerTabs.remPartyMinus.remParty.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */

                        where docReturnsCustomerTabs.DocReturnsCustomerID == _params.DocReturnsCustomerID

                        #region select

                        select new
                        {
                            //партия
                            RemPartyMinusID = docReturnsCustomerTabs.remPartyMinus.RemPartyMinusID,

                            DocReturnsCustomerTabID = docReturnsCustomerTabs.DocReturnsCustomerTabID,
                            DocReturnsCustomerID = docReturnsCustomerTabs.DocReturnsCustomerID,
                            DirNomenID = docReturnsCustomerTabs.DirNomenID,

                            //DirNomenName = docReturnsCustomerTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docReturnsCustomerTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docReturnsCustomerTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docReturnsCustomerTabs.dirNomen.DirNomenName,

                            Quantity = docReturnsCustomerTabs.Quantity,

                            PriceVAT = docReturnsCustomerTabs.PriceVAT,

                            DirCurrencyID = docReturnsCustomerTabs.DirCurrencyID,
                            DirCurrencyRate = docReturnsCustomerTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docReturnsCustomerTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docReturnsCustomerTabs.dirCurrency.DirCurrencyName + " (" + docReturnsCustomerTabs.DirCurrencyRate + ", " + docReturnsCustomerTabs.DirCurrencyMultiplicity + ")",


                            //Характеристики
                            /*
                            DirCharColourID = docReturnsCustomerTabs.remPartyMinus.remParty.DirCharColourID,
                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialID = docReturnsCustomerTabs.remPartyMinus.remParty.DirCharMaterialID,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameID = docReturnsCustomerTabs.remPartyMinus.remParty.DirCharNameID,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonID = docReturnsCustomerTabs.remPartyMinus.remParty.DirCharSeasonID,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexID = docReturnsCustomerTabs.remPartyMinus.remParty.DirCharSexID,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeID = docReturnsCustomerTabs.remPartyMinus.remParty.DirCharSizeID,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleID = docReturnsCustomerTabs.remPartyMinus.remParty.DirCharStyleID,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureID = docReturnsCustomerTabs.remPartyMinus.remParty.DirCharTextureID,
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
                            */
                            SerialNumber = docReturnsCustomerTabs.remPartyMinus.remParty.SerialNumber,
                            Barcode = docReturnsCustomerTabs.remPartyMinus.remParty.Barcode,
                            

                            //Цена в т.в.
                            PriceCurrency = docReturnsCustomerTabs.PriceCurrency,
                            //Себестоимость
                            SUMPurchPriceVATCurrency = docReturnsCustomerTabs.Quantity * docReturnsCustomerTabs.PriceCurrency == null ? 0
                            : Math.Round(docReturnsCustomerTabs.Quantity * docReturnsCustomerTabs.PriceCurrency, sysSetting.FractionalPartInSum),

                            
                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docReturnsCustomerTabs.remPartyMinus.remParty.PriceRetailVAT - docReturnsCustomerTabs.PriceVAT) / docReturnsCustomerTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docReturnsCustomerTabs.remPartyMinus.remParty.PriceRetailVAT - docReturnsCustomerTabs.PriceVAT) / docReturnsCustomerTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docReturnsCustomerTabs.remPartyMinus.remParty.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docReturnsCustomerTabs.remPartyMinus.remParty.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docReturnsCustomerTabs.remPartyMinus.remParty.PriceWholesaleVAT - docReturnsCustomerTabs.PriceVAT) / docReturnsCustomerTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docReturnsCustomerTabs.remPartyMinus.remParty.PriceWholesaleVAT - docReturnsCustomerTabs.PriceVAT) / docReturnsCustomerTabs.PriceVAT, sysSetting.FractionalPartInSum)
                            ,
                            //Оптовая цена 
                            PriceWholesaleVAT = docReturnsCustomerTabs.remPartyMinus.remParty.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docReturnsCustomerTabs.remPartyMinus.remParty.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docReturnsCustomerTabs.remPartyMinus.remParty.PriceIMVAT - docReturnsCustomerTabs.PriceVAT) / docReturnsCustomerTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docReturnsCustomerTabs.remPartyMinus.remParty.PriceIMVAT - docReturnsCustomerTabs.PriceVAT) / docReturnsCustomerTabs.PriceVAT, sysSetting.FractionalPartInSum)
                            ,
                            //Интернет-Магазин
                            PriceIMVAT = docReturnsCustomerTabs.remPartyMinus.remParty.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docReturnsCustomerTabs.remPartyMinus.remParty.PriceIMCurrency,
                            
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocReturnsCustomerTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocReturnsCustomerTabs/5
        [ResponseType(typeof(DocReturnsCustomerTab))]
        public async Task<IHttpActionResult> GetDocReturnsCustomerTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocReturnsCustomerTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocReturnsCustomerTab(int id, DocReturnsCustomerTab docReturnsCustomerTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocReturnsCustomerTabs
        [ResponseType(typeof(DocReturnsCustomerTab))]
        public async Task<IHttpActionResult> PostDocReturnsCustomerTab(DocReturnsCustomerTab docReturnsCustomerTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocReturnsCustomerTabs/5
        [ResponseType(typeof(DocReturnsCustomerTab))]
        public async Task<IHttpActionResult> DeleteDocReturnsCustomerTab(int id)
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

        private bool DocReturnsCustomerTabExists(int id)
        {
            return db.DocReturnsCustomerTabs.Count(e => e.DocReturnsCustomerTabID == id) > 0;
        }

        #endregion


        #region SQL

        //Сумма документа
        public string GenerateSQLSUM(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL =
                "SELECT Docs.Discount, " +
                "COUNT(*) CountRecord, " +
                "COUNT(*) CountRecord_NumInWords, " +
                "SUM(DocReturnsCustomerTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocReturnsCustomerTabs.Quantity * DocReturnsCustomerTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocReturnsCustomerTabs.Quantity * DocReturnsCustomerTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocReturnsCustomerTabs.Quantity * (DocReturnsCustomerTabs.PriceCurrency - (DocReturnsCustomerTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocReturnsCustomerTabs.Quantity * (DocReturnsCustomerTabs.PriceCurrency - (DocReturnsCustomerTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocReturnsCustomerTabs.Quantity * DocReturnsCustomerTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocReturnsCustomerTabs.Quantity * DocReturnsCustomerTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocReturnsCustomers, DocReturnsCustomerTabs " +
                "WHERE " +
                "(Docs.DocID=DocReturnsCustomers.DocID)and" +
                "(DocReturnsCustomers.DocReturnsCustomerID=DocReturnsCustomerTabs.DocReturnsCustomerID)and(Docs.DocID=@DocID)";

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
                "[Docs].[Discount] AS [Discount], " +

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

                "[DocReturnsCustomerTabs].[DocReturnsCustomerTabID] AS [DocReturnsCustomerTabID], " +
                "[DocReturnsCustomerTabs].[DocReturnsCustomerID] AS [DocReturnsCustomerID], " +
                "[DocReturnsCustomerTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocReturnsCustomerTabs].[Quantity] AS [Quantity], " +
                "[DocReturnsCustomerTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocReturnsCustomerTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocReturnsCustomerTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocReturnsCustomerTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocReturnsCustomerTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocReturnsCustomerTabs].[DirCurrencyRate] || ', ' || [DocReturnsCustomerTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                //"[DocReturnsCustomerTabs].[DirCharColourID] AS [DirCharColourID], " +
                "[DirCharColours].[DirCharColourName] AS [DirCharColourName], " +
                //"[DocReturnsCustomerTabs].[DirCharMaterialID] AS [DirCharMaterialID], " +
                "[DirCharMaterials].[DirCharMaterialName] AS [DirCharMaterialName], " +
                //"[DocReturnsCustomerTabs].[DirCharNameID] AS [DirCharNameID], " +
                "[DirCharNames].[DirCharNameName] AS [DirCharNameName], " +
                //"[DocReturnsCustomerTabs].[DirCharSeasonID] AS [DirCharSeasonID], " +
                "[DirCharSeasons].[DirCharSeasonName] AS [DirCharSeasonName], " +
                //"[DocReturnsCustomerTabs].[DirCharSexID] AS [DirCharSexID], " +
                "[DirCharSexes].[DirCharSexName] AS [DirCharSexName], " +
                //"[DocReturnsCustomerTabs].[DirCharSizeID] AS [DirCharSizeID], " +
                "[DirCharSizes].[DirCharSizeName] AS [DirCharSizeName], " +
                //"[DocReturnsCustomerTabs].[DirCharStyleID] AS [DirCharStyleID], " +
                "[DirCharStyles].[DirCharStyleName] AS [DirCharStyleName], " +
                //"[DocReturnsCustomerTabs].[DirCharTextureID] AS [DirCharTextureID], " +
                "[DirCharTextures].[DirCharTextureName] AS [DirCharTextureName], " +
                "CASE WHEN ([DirCharColours].[DirCharColourName] IS NULL) THEN '' ELSE [DirCharColours].[DirCharColourName] END || ' ' || CASE WHEN ([DirCharMaterials].[DirCharMaterialName] IS NULL) THEN '' ELSE [DirCharMaterials].[DirCharMaterialName] END || ' ' || CASE WHEN ([DirCharNames].[DirCharNameName] IS NULL) THEN '' ELSE [DirCharNames].[DirCharNameName] END || ' ' || CASE WHEN ([DirCharSeasons].[DirCharSeasonName] IS NULL) THEN '' ELSE [DirCharSeasons].[DirCharSeasonName] END || ' ' || CASE WHEN ([DirCharSexes].[DirCharSexName] IS NULL) THEN '' ELSE [DirCharSexes].[DirCharSexName] END || ' ' || CASE WHEN ([DirCharSizes].[DirCharSizeName] IS NULL) THEN '' ELSE [DirCharSizes].[DirCharSizeName] END || ' ' || CASE WHEN ([DirCharStyles].[DirCharStyleName] IS NULL) THEN '' ELSE [DirCharStyles].[DirCharStyleName] END || ' ' || CASE WHEN ([DirCharTextures].[DirCharTextureName] IS NULL) THEN '' ELSE [DirCharTextures].[DirCharTextureName] END AS [DirChar], " +
                "[RemParties].[SerialNumber] AS [SerialNumber], " +
                "[RemParties].[Barcode] AS [Barcode], " +
                "[RemParties].[Barcode] AS [BarcodeImage], " +

                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocReturnsCustomerTabs].[PriceCurrency] AS [PriceCurrency], " +
                //"Цена без НДС" в валюте
                "ROUND([DocReturnsCustomerTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVAT], " +
                //"Цена без НДС" в текущей валюте
                "ROUND([DocReturnsCustomerTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVATCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocReturnsCustomerTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocReturnsCustomerTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +
                //"Цена с НДС" в текущей валюте со Скидкой
                "ROUND([DocReturnsCustomerTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'PriceCurrencyDiscount', " +
                //"Сумма НДС" (НДС документа)
                "ROUND([DocReturnsCustomerTabs].[Quantity] * ([DocReturnsCustomerTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                //"Стоимость без НДС" в валюте
                "ROUND(([DocReturnsCustomerTabs].[Quantity] * [DocReturnsCustomerTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVAT], " +
                //"Стоимость Прихода без НДС" в текущей валюте
                "ROUND(([DocReturnsCustomerTabs].[Quantity] * [DocReturnsCustomerTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVATCurrency], " +
                //Себестоимось прихода
                "ROUND([DocReturnsCustomerTabs].[Quantity] * [DocReturnsCustomerTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocReturnsCustomerTabs].[Quantity] * [DocReturnsCustomerTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVATCurrency], " +
                //Стоимость с НДС в текущей валюте со Скидкой
                "ROUND([DocReturnsCustomerTabs].[Quantity] * [DocReturnsCustomerTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " +


                //Розница
                "ROUND((100 * ([RemParties].[PriceRetailVAT] - [RemParties].[PriceVAT])) / [RemParties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupRetail], " +
                "[RemParties].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[RemParties].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость Розницы в валюте
                "ROUND([DocReturnsCustomerTabs].[Quantity] * [RemParties].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость Розницы в текущей валюте
                "ROUND([DocReturnsCustomerTabs].[Quantity] * [RemParties].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " +

                //Опт
                "ROUND((100 * ([RemParties].[PriceWholesaleVAT] - [RemParties].[PriceVAT])) / [RemParties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupWholesale], " +
                "[RemParties].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[RemParties].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость Опта в валюте
                "ROUND([DocReturnsCustomerTabs].[Quantity] * [RemParties].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость Опта в текущей валюте
                "ROUND([DocReturnsCustomerTabs].[Quantity] * [RemParties].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " +

                //Интернет-Магазин
                "ROUND((100 * ([RemParties].[PriceIMVAT] - [RemParties].[PriceVAT])) / [RemParties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupIM], " +
                "[RemParties].[PriceIMVAT] AS [PriceIMVAT], " +
                "[RemParties].[PriceIMCurrency] AS [PriceIMCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocReturnsCustomers], [DocReturnsCustomerTabs] " +

                //docReturnsCustomerTabs.remPartyMinus.remParty.DirCharColourID
                "LEFT OUTER JOIN [RemPartyMinuses] ON [DocReturnsCustomerTabs].[RemPartyMinusID] = [RemPartyMinuses].[RemPartyMinusID] " +
                "LEFT OUTER JOIN [RemParties] ON [RemPartyMinuses].[RemPartyID] = [RemParties].[RemPartyID] " +

                "LEFT OUTER JOIN [DirCharColours] ON [RemParties].[DirCharColourID] = [DirCharColours].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] ON [RemParties].[DirCharMaterialID] = [DirCharMaterials].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] ON [RemParties].[DirCharNameID] = [DirCharNames].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] ON [RemParties].[DirCharSeasonID] = [DirCharSeasons].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] ON [RemParties].[DirCharSexID] = [DirCharSexes].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] ON [RemParties].[DirCharSizeID] = [DirCharSizes].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] ON [RemParties].[DirCharStyleID] = [DirCharStyles].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] ON [RemParties].[DirCharTextureID] = [DirCharTextures].[DirCharTextureID] " +
                "INNER JOIN [DirNomens] ON [RemParties].[DirNomenID] = [DirNomens].[DirNomenID] " +
                //"LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocReturnsCustomerTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocReturnsCustomerTabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***



                "WHERE ([Docs].[DocID]=[DocReturnsCustomers].[DocID])and([DocReturnsCustomers].[DocReturnsCustomerID]=[DocReturnsCustomerTabs].[DocReturnsCustomerID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}