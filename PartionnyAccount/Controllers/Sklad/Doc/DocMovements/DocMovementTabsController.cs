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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocMovements
{
    public class DocMovementTabsController : ApiController
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
            public int DocMovementID;
            public int DocID;
        }
        // GET: api/DocMovementTabs
        public async Task<IHttpActionResult> GetDocMovementTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocMovementsLogistics"));
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
                _params.DocMovementID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocMovementID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docMovementTabs in db.DocMovementTabs

                        join dirNomens11 in db.DirNomens on docMovementTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики
                        join dirCharColours1 in db.DirCharColours on docMovementTabs.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on docMovementTabs.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on docMovementTabs.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on docMovementTabs.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on docMovementTabs.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on docMovementTabs.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on docMovementTabs.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on docMovementTabs.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        #endregion

                        where docMovementTabs.DocMovementID == _params.DocMovementID

                        #region select

                        select new
                        {
                            DocMovementTabID = docMovementTabs.DocMovementTabID,
                            DocMovementID = docMovementTabs.DocMovementID,
                            DirNomenID = docMovementTabs.DirNomenID,

                            //DirNomenName = docMovementTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docMovementTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docMovementTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docMovementTabs.dirNomen.DirNomenName,

                            RemPartyID = docMovementTabs.RemPartyID,
                            Quantity = docMovementTabs.Quantity,

                            PriceVAT = docMovementTabs.PriceVAT,

                            DirCurrencyID = docMovementTabs.DirCurrencyID,
                            DirCurrencyRate = docMovementTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docMovementTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docMovementTabs.dirCurrency.DirCurrencyName + " (" + docMovementTabs.DirCurrencyRate + ", " + docMovementTabs.DirCurrencyMultiplicity + ")",


                            //Характеристики
                            DirCharColourID = docMovementTabs.DirCharColourID,
                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialID = docMovementTabs.DirCharMaterialID,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameID = docMovementTabs.DirCharNameID,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonID = docMovementTabs.DirCharSeasonID,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexID = docMovementTabs.DirCharSexID,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeID = docMovementTabs.DirCharSizeID,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleID = docMovementTabs.DirCharStyleID,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureID = docMovementTabs.DirCharTextureID,
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
                            SerialNumber = docMovementTabs.SerialNumber,
                            Barcode = docMovementTabs.Barcode,


                            //Цена в т.в.
                            PriceCurrency = docMovementTabs.PriceCurrency,
                            //Себестоимость
                            SUMMovementPriceVATCurrency = docMovementTabs.Quantity * docMovementTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docMovementTabs.Quantity * docMovementTabs.PriceCurrency, sysSetting.FractionalPartInSum),

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
                    DocMovementTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocMovementTabs/5
        [ResponseType(typeof(DocMovementTab))]
        public async Task<IHttpActionResult> GetDocMovementTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocMovementTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocMovementTab(int id, DocMovementTab docMovementTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocMovementTabs
        [ResponseType(typeof(DocMovementTab))]
        public async Task<IHttpActionResult> PostDocMovementTab(DocMovementTab docMovementTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocMovementTabs/5
        [ResponseType(typeof(DocMovementTab))]
        public async Task<IHttpActionResult> DeleteDocMovementTab(int id)
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

        private bool DocMovementTabExists(int id)
        {
            return db.DocMovementTabs.Count(e => e.DocMovementTabID == id) > 0;
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
                "SUM(DocMovementTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocMovementTabs.Quantity * DocMovementTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocMovementTabs.Quantity * DocMovementTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords' " + //Приписью

                "FROM Docs, DocMovements, DocMovementTabs " +
                "WHERE " +
                "(Docs.DocID=DocMovements.DocID)and" +
                "(DocMovements.DocMovementID=DocMovementTabs.DocMovementID)and(Docs.DocID=@DocID)";

            return SQL;
        }

        //Спецификация
        public string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL = "";


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

                "[DocMovementTabs].[DocMovementTabID] AS [DocMovementTabID], " +
                "[DocMovementTabs].[DocMovementID] AS [DocMovementID], " +
                "[DocMovementTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocMovementTabs].[Quantity] AS [Quantity], " +
                "[DocMovementTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocMovementTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocMovementTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocMovementTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocMovementTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocMovementTabs].[DirCurrencyRate] || ', ' || [DocMovementTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocMovementTabs].[DirCharColourID] AS [DirCharColourID], " +
                "[DirCharColours].[DirCharColourName] AS [DirCharColourName], " +
                "[DocMovementTabs].[DirCharMaterialID] AS [DirCharMaterialID], " +
                "[DirCharMaterials].[DirCharMaterialName] AS [DirCharMaterialName], " +
                "[DocMovementTabs].[DirCharNameID] AS [DirCharNameID], " +
                "[DirCharNames].[DirCharNameName] AS [DirCharNameName], " +
                "[DocMovementTabs].[DirCharSeasonID] AS [DirCharSeasonID], " +
                "[DirCharSeasons].[DirCharSeasonName] AS [DirCharSeasonName], " +
                "[DocMovementTabs].[DirCharSexID] AS [DirCharSexID], " +
                "[DirCharSexes].[DirCharSexName] AS [DirCharSexName], " +
                "[DocMovementTabs].[DirCharSizeID] AS [DirCharSizeID], " +
                "[DirCharSizes].[DirCharSizeName] AS [DirCharSizeName], " +
                "[DocMovementTabs].[DirCharStyleID] AS [DirCharStyleID], " +
                "[DirCharStyles].[DirCharStyleName] AS [DirCharStyleName], " +
                "[DocMovementTabs].[DirCharTextureID] AS [DirCharTextureID], " +
                "[DirCharTextures].[DirCharTextureName] AS [DirCharTextureName], " +
                "CASE WHEN ([DirCharColours].[DirCharColourName] IS NULL) THEN '' ELSE [DirCharColours].[DirCharColourName] END || ' ' || CASE WHEN ([DirCharMaterials].[DirCharMaterialName] IS NULL) THEN '' ELSE [DirCharMaterials].[DirCharMaterialName] END || ' ' || CASE WHEN ([DirCharNames].[DirCharNameName] IS NULL) THEN '' ELSE [DirCharNames].[DirCharNameName] END || ' ' || CASE WHEN ([DirCharSeasons].[DirCharSeasonName] IS NULL) THEN '' ELSE [DirCharSeasons].[DirCharSeasonName] END || ' ' || CASE WHEN ([DirCharSexes].[DirCharSexName] IS NULL) THEN '' ELSE [DirCharSexes].[DirCharSexName] END || ' ' || CASE WHEN ([DirCharSizes].[DirCharSizeName] IS NULL) THEN '' ELSE [DirCharSizes].[DirCharSizeName] END || ' ' || CASE WHEN ([DirCharStyles].[DirCharStyleName] IS NULL) THEN '' ELSE [DirCharStyles].[DirCharStyleName] END || ' ' || CASE WHEN ([DirCharTextures].[DirCharTextureName] IS NULL) THEN '' ELSE [DirCharTextures].[DirCharTextureName] END AS [DirChar], " +
                "[DocMovementTabs].[SerialNumber] AS [SerialNumber], " +
                "[DocMovementTabs].[Barcode] AS [Barcode], " +
                "[DocMovementTabs].[Barcode] AS [BarcodeImage], " +

                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocMovementTabs].[PriceCurrency] AS [PriceCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocMovementTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocMovementTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +

                //Себестоимось прихода
                "ROUND([DocMovementTabs].[Quantity] * [DocMovementTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocMovementTabs].[Quantity] * [DocMovementTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SumPurchPriceCurrency], " +


                //Розница
                "[DocMovementTabs].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[DocMovementTabs].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость в валюте
                "ROUND([DocMovementTabs].[Quantity] * [DocMovementTabs].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocMovementTabs].[Quantity] * [DocMovementTabs].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " +

                //Опт
                "[DocMovementTabs].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[DocMovementTabs].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость в валюте
                "ROUND([DocMovementTabs].[Quantity] * [DocMovementTabs].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocMovementTabs].[Quantity] * [DocMovementTabs].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " +

                //Интернет-Магазин
                "[DocMovementTabs].[PriceIMVAT] AS [PriceIMVAT], " +
                "[DocMovementTabs].[PriceIMCurrency] AS [PriceIMCurrency], " +
                //Стоимость в валюте
                "ROUND([DocMovementTabs].[Quantity] * [DocMovementTabs].[PriceIMVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMIMPriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocMovementTabs].[Quantity] * [DocMovementTabs].[PriceIMCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMIMPriceCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocMovements], [DocMovementTabs] " +

                "LEFT OUTER JOIN [DirCharColours] ON [DocMovementTabs].[DirCharColourID] = [DirCharColours].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] ON [DocMovementTabs].[DirCharMaterialID] = [DirCharMaterials].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] ON [DocMovementTabs].[DirCharNameID] = [DirCharNames].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] ON [DocMovementTabs].[DirCharSeasonID] = [DirCharSeasons].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] ON [DocMovementTabs].[DirCharSexID] = [DirCharSexes].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] ON [DocMovementTabs].[DirCharSizeID] = [DirCharSizes].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] ON [DocMovementTabs].[DirCharStyleID] = [DirCharStyles].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] ON [DocMovementTabs].[DirCharTextureID] = [DirCharTextures].[DirCharTextureID] " +
                //"INNER JOIN [DirNomens] ON [DocMovementTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                //"LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocMovementTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocMovementTabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocMovements].[DocID])and([DocMovements].[DocMovementID]=[DocMovementTabs].[DocMovementID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}