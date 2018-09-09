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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocInventories
{
    public class DocInventoryTabsController : ApiController
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
            public int DocInventoryID;
            public int DocID;
        }
        // GET: api/DocInventoryTabs
        public async Task<IHttpActionResult> GetDocInventoryTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocInventories"));
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
                _params.DocInventoryID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocInventoryID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docInventoryTabs in db.DocInventoryTabs

                        join dirNomens11 in db.DirNomens on docInventoryTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()
                        

                        //Партия
                        join remParties1 in db.RemParties on docInventoryTabs.DocInventoryTabID equals remParties1.FieldID into remParties2
                        from remParties in remParties2.Where(x => x.DocID == docInventoryTabs.docInventory.doc.DocID).DefaultIfEmpty()

                        where docInventoryTabs.DocInventoryID == _params.DocInventoryID

                        #region select

                        select new
                        {
                            DocInventoryTabID = docInventoryTabs.DocInventoryTabID,
                            DocInventoryID = docInventoryTabs.DocInventoryID,
                            DirNomenID = docInventoryTabs.DirNomenID,

                            DirNomenMinimumBalance = docInventoryTabs.DirNomenMinimumBalance,

                            //DirNomenName = docInventoryTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docInventoryTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docInventoryTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docInventoryTabs.dirNomen.DirNomenName,


                            //Характеристики
                            DirCharColourID = docInventoryTabs.DirCharColourID,
                            DirCharColourName = docInventoryTabs.dirCharColour.DirCharColourName,
                            DirCharMaterialID = docInventoryTabs.DirCharMaterialID,
                            DirCharMaterialName = docInventoryTabs.dirCharMaterial.DirCharMaterialName,
                            DirCharNameID = docInventoryTabs.DirCharNameID,
                            DirCharNameName = docInventoryTabs.dirCharName.DirCharNameName,
                            DirCharSeasonID = docInventoryTabs.DirCharSeasonID,
                            DirCharSeasonName = docInventoryTabs.dirCharSeason.DirCharSeasonName,
                            DirCharSexID = docInventoryTabs.DirCharSexID,
                            DirCharSexName = docInventoryTabs.dirCharSex.DirCharSexName,
                            DirCharSizeID = docInventoryTabs.DirCharSizeID,
                            DirCharSizeName = docInventoryTabs.dirCharSize.DirCharSizeName,
                            DirCharStyleID = docInventoryTabs.DirCharStyleID,
                            DirCharStyleName = docInventoryTabs.dirCharStyle.DirCharStyleName,
                            DirContractorID = docInventoryTabs.DirContractorID,
                            DirContractorName = docInventoryTabs.dirContractor.DirContractorName,
                            DirCharTextureID = docInventoryTabs.DirCharTextureID,
                            DirCharTextureName = docInventoryTabs.dirCharTexture.DirCharTextureName,
                            DirChar =
                                docInventoryTabs.dirCharColour.DirCharColourName + " " +
                                docInventoryTabs.dirCharMaterial.DirCharMaterialName + " " +
                                docInventoryTabs.dirCharName.DirCharNameName + " " +
                                docInventoryTabs.dirCharSeason.DirCharSeasonName + " " +
                                docInventoryTabs.dirCharSex.DirCharSexName + " " +
                                docInventoryTabs.dirCharSize.DirCharSizeName + " " +
                                docInventoryTabs.dirCharStyle.DirCharStyleName + " " +
                                docInventoryTabs.dirContractor.DirContractorName + " " +
                                docInventoryTabs.dirCharTexture.DirCharTextureName,
                            SerialNumber = docInventoryTabs.SerialNumber,
                            Barcode = docInventoryTabs.Barcode,


                            RemPartyID = docInventoryTabs.RemPartyID,

                            DirCurrencyID = docInventoryTabs.DirCurrencyID,
                            DirCurrencyRate = docInventoryTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docInventoryTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docInventoryTabs.dirCurrency.DirCurrencyName + " (" + docInventoryTabs.DirCurrencyRate + ", " + docInventoryTabs.DirCurrencyMultiplicity + ")",


                            //Списать по приходной цене

                            Quantity_WriteOff = docInventoryTabs.Quantity_WriteOff,
                            PriceVAT = docInventoryTabs.PriceVAT,
                            //Цена в т.в.
                            PriceCurrency = docInventoryTabs.PriceCurrency,
                            //Себестоимость
                            SUMPurchPriceVATCurrency = docInventoryTabs.Quantity_WriteOff * docInventoryTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docInventoryTabs.Quantity_WriteOff * docInventoryTabs.PriceCurrency, sysSetting.FractionalPartInSum),



                            //Оприходовать: приходная цена + расходные

                            Quantity_Purch = docInventoryTabs.Quantity_Purch,

                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docInventoryTabs.PriceRetailVAT - docInventoryTabs.PriceVAT) / docInventoryTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docInventoryTabs.PriceRetailVAT - docInventoryTabs.PriceVAT) / docInventoryTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docInventoryTabs.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docInventoryTabs.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docInventoryTabs.PriceWholesaleVAT - docInventoryTabs.PriceVAT) / docInventoryTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docInventoryTabs.PriceWholesaleVAT - docInventoryTabs.PriceVAT) / docInventoryTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Оптовая цена 
                            PriceWholesaleVAT = docInventoryTabs.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docInventoryTabs.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docInventoryTabs.PriceIMVAT - docInventoryTabs.PriceVAT) / docInventoryTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docInventoryTabs.PriceIMVAT - docInventoryTabs.PriceVAT) / docInventoryTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Интернет-Магазин
                            PriceIMVAT = docInventoryTabs.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docInventoryTabs.PriceIMCurrency,
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocInventoryTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocInventoryTabs/5
        [ResponseType(typeof(DocInventoryTab))]
        public async Task<IHttpActionResult> GetDocInventoryTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocInventoryTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocInventoryTab(int id, DocInventoryTab docInventoryTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocInventoryTabs
        [ResponseType(typeof(DocInventoryTab))]
        public async Task<IHttpActionResult> PostDocInventoryTab(DocInventoryTab docInventoryTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocInventoryTabs/5
        [ResponseType(typeof(DocInventoryTab))]
        public async Task<IHttpActionResult> DeleteDocInventoryTab(int id)
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

        private bool DocInventoryTabExists(int id)
        {
            return db.DocInventoryTabs.Count(e => e.DocInventoryTabID == id) > 0;
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
                "SUM(DocInventoryTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round((SUM((DocInventoryTabs.Quantity * DocInventoryTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round((SUM((DocInventoryTabs.Quantity * DocInventoryTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords' " + //Приписью

                //"round((SUM( DocInventoryTabs.Quantity * (DocInventoryTabs.PriceCurrency - DocInventoryTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                //"round((SUM(DocInventoryTabs.Quantity * (DocInventoryTabs.PriceCurrency - DocInventoryTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords' " + //Приписью

                //"round(SUM((DocInventoryTabs.Quantity * DocInventoryTabs.PriceCurrency)), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                //"round(SUM((DocInventoryTabs.Quantity * DocInventoryTabs.PriceCurrency)), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords' " + //Приписью

                "FROM Docs, DocInventories, DocInventoryTabs " +
                "WHERE " +
                "(Docs.DocID=DocInventories.DocID)and" +
                "(DocInventories.DocInventoryID=DocInventoryTabs.DocInventoryID)and(Docs.DocID=@DocID)";

            return SQL;
        }

        //Спецификация
        public string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";


            SQL =
                "SELECT " +
                //"[Docs].[DocDate] AS [DocDate], " +

                "[DirNomens].[DirNomenName] AS [DirNomenName], " +
                "[DirNomens].[DirNomenName] AS [DirNomenNameRemove], " +
                "[DirNomens].[DirNomenArticle] AS [DirNomenArticle], " +
                "[DirNomens].[DirNomenMinimumBalance] AS [DirNomenMinimumBalance], " +
                "[DirNomens].[DirNomenNameFull] AS [DirNomenNameFull], " +
                "[DirNomens].[DescriptionFull] AS [DescriptionFull], " +
                "[DirNomenGroups].[DirNomenName] AS [DirNomenGroupName], " + //Группа (Sub)

                //"[DocInventoryTabs].[DocInventoryTabID] AS [DocInventoryTabID], " +
                //"[DocInventoryTabs].[DocInventoryID] AS [DocInventoryID], " +
                "[DocInventoryTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocInventoryTabs].[Quantity] AS [Quantity], " +
                "[DocInventoryTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocInventoryTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocInventoryTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocInventoryTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocInventoryTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocInventoryTabs].[DirCurrencyRate] || ', ' || [DocInventoryTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                //"[DocInventoryTabs].[DirCharColourID] AS [DirCharColourID], " +
                "[DirCharColours].[DirCharColourName] AS [DirCharColourName], " +
                //"[DocInventoryTabs].[DirCharMaterialID] AS [DirCharMaterialID], " +
                "[DirCharMaterials].[DirCharMaterialName] AS [DirCharMaterialName], " +
                //"[DocInventoryTabs].[DirCharNameID] AS [DirCharNameID], " +
                "[DirCharNames].[DirCharNameName] AS [DirCharNameName], " +
                //"[DocInventoryTabs].[DirCharSeasonID] AS [DirCharSeasonID], " +
                "[DirCharSeasons].[DirCharSeasonName] AS [DirCharSeasonName], " +
                //"[DocInventoryTabs].[DirCharSexID] AS [DirCharSexID], " +
                "[DirCharSexes].[DirCharSexName] AS [DirCharSexName], " +
                //"[DocInventoryTabs].[DirCharSizeID] AS [DirCharSizeID], " +
                "[DirCharSizes].[DirCharSizeName] AS [DirCharSizeName], " +
                //"[DocInventoryTabs].[DirCharStyleID] AS [DirCharStyleID], " +
                "[DirCharStyles].[DirCharStyleName] AS [DirCharStyleName], " +
                //"[DocInventoryTabs].[DirCharTextureID] AS [DirCharTextureID], " +
                "[DirCharTextures].[DirCharTextureName] AS [DirCharTextureName], " +
                "CASE WHEN ([DirCharColours].[DirCharColourName] IS NULL) THEN '' ELSE [DirCharColours].[DirCharColourName] END || ' ' || CASE WHEN ([DirCharMaterials].[DirCharMaterialName] IS NULL) THEN '' ELSE [DirCharMaterials].[DirCharMaterialName] END || ' ' || CASE WHEN ([DirCharNames].[DirCharNameName] IS NULL) THEN '' ELSE [DirCharNames].[DirCharNameName] END || ' ' || CASE WHEN ([DirCharSeasons].[DirCharSeasonName] IS NULL) THEN '' ELSE [DirCharSeasons].[DirCharSeasonName] END || ' ' || CASE WHEN ([DirCharSexes].[DirCharSexName] IS NULL) THEN '' ELSE [DirCharSexes].[DirCharSexName] END || ' ' || CASE WHEN ([DirCharSizes].[DirCharSizeName] IS NULL) THEN '' ELSE [DirCharSizes].[DirCharSizeName] END || ' ' || CASE WHEN ([DirCharStyles].[DirCharStyleName] IS NULL) THEN '' ELSE [DirCharStyles].[DirCharStyleName] END || ' ' || CASE WHEN ([DirCharTextures].[DirCharTextureName] IS NULL) THEN '' ELSE [DirCharTextures].[DirCharTextureName] END AS [DirChar], " +
                "[DocInventoryTabs].[SerialNumber] AS [SerialNumber], " +
                "[DocInventoryTabs].[Barcode] AS [Barcode], " +
                "[DocInventoryTabs].[Barcode] AS [BarcodeImage], " +

                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocInventoryTabs].[PriceCurrency] AS [PriceCurrency], " +
                //"Цена без НДС" в валюте
                //"ROUND([DocInventoryTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVAT], " +
                //"Цена без НДС" в текущей валюте
                //"ROUND([DocInventoryTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVATCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocInventoryTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocInventoryTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +
                //"Цена с НДС" в текущей валюте со Скидкой
                //"ROUND([DocInventoryTabs].[PriceCurrency] * [Docs].[Discount], " + sysSettings.FractionalPartInSum + ") 'PriceCurrencyDiscount', " +
                //"Сумма НДС" (НДС документа)
                //"ROUND([DocInventoryTabs].[Quantity] * ([DocInventoryTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                //"Стоимость без НДС" в валюте
                //"ROUND(([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVAT], " +
                //"Стоимость Прихода без НДС" в текущей валюте
                //"ROUND(([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVATCurrency], " +
                //Себестоимось прихода
                "ROUND([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVATCurrency], " +
                //Стоимость с НДС в текущей валюте со Скидкой
                //"ROUND([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceCurrency] * (1 - [Docs].[Discount] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " +


                //Розница
                "ROUND((100 * ([DocInventoryTabs].[PriceRetailVAT] - [DocInventoryTabs].[PriceVAT])) / [DocInventoryTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupRetail], " +
                "[DocInventoryTabs].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[DocInventoryTabs].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость Розницы в валюте
                "ROUND([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость Розницы в текущей валюте
                "ROUND([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " +

                //Опт
                "ROUND((100 * ([DocInventoryTabs].[PriceWholesaleVAT] - [DocInventoryTabs].[PriceVAT])) / [DocInventoryTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupWholesale], " +
                "[DocInventoryTabs].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[DocInventoryTabs].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость Опта в валюте
                "ROUND([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость Опта в текущей валюте
                "ROUND([DocInventoryTabs].[Quantity] * [DocInventoryTabs].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " +

                //Интернет-Магазин
                "ROUND((100 * ([DocInventoryTabs].[PriceIMVAT] - [DocInventoryTabs].[PriceVAT])) / [DocInventoryTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupIM], " +
                "[DocInventoryTabs].[PriceIMVAT] AS [PriceIMVAT], " +
                "[DocInventoryTabs].[PriceIMCurrency] AS [PriceIMCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocInventories], [DocInventoryTabs] " +

                "LEFT OUTER JOIN [DirCharColours] ON [DocInventoryTabs].[DirCharColourID] = [DirCharColours].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] ON [DocInventoryTabs].[DirCharMaterialID] = [DirCharMaterials].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] ON [DocInventoryTabs].[DirCharNameID] = [DirCharNames].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] ON [DocInventoryTabs].[DirCharSeasonID] = [DirCharSeasons].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] ON [DocInventoryTabs].[DirCharSexID] = [DirCharSexes].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] ON [DocInventoryTabs].[DirCharSizeID] = [DirCharSizes].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] ON [DocInventoryTabs].[DirCharStyleID] = [DirCharStyles].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] ON [DocInventoryTabs].[DirCharTextureID] = [DirCharTextures].[DirCharTextureID] " +
                "INNER JOIN [DirNomens] ON [DocInventoryTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                "LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocInventoryTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "WHERE ([Docs].[DocID]=[DocInventories].[DocID])and([DocInventories].[DocInventoryID]=[DocInventoryTabs].[DocInventoryID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}