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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocServicePurches
{
    public class DocServiceMovTabsController : ApiController
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
            public int DocServiceMovID;
            public int DocID;
        }
        // GET: api/DocServiceMovTabs
        public async Task<IHttpActionResult> GetDocServiceMovTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocServiceMovementsLogistics"));
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
                _params.DocServiceMovID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocServiceMovID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docMovementTabs in db.DocServiceMovTabs

                        join dirServiceNomens11 in db.DirServiceNomens on docMovementTabs.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where docMovementTabs.DocServiceMovID == _params.DocServiceMovID

                        #region select

                        select new
                        {
                            DocServiceMovTabID = docMovementTabs.DocServiceMovTabID,
                            DocServiceMovID = docMovementTabs.DocServiceMovID,
                            DirServiceNomenID = docMovementTabs.DirServiceNomenID,

                            DocServicePurchID = docMovementTabs.docServicePurch.DocServicePurchID,

                            //DirServiceNomenName = docMovementTabs.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? docMovementTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docMovementTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docMovementTabs.dirServiceNomen.DirServiceNomenName,

                            DirServiceStatusID = docMovementTabs.DirServiceStatusID,
                            DirServiceStatusName = docMovementTabs.dirServiceStatus.DirServiceStatusName,

                            DirServiceStatusID_789 = docMovementTabs.DirServiceStatusID_789,
                            DirServiceStatusName_789 = docMovementTabs.dirServiceStatus_789.DirServiceStatusName,

                            //docServicePurch ===
                            PriceVAT = docMovementTabs.docServicePurch.PriceVAT,
                            SerialNumber = docMovementTabs.docServicePurch.SerialNumber,
                            DirServiceContractorName = docMovementTabs.docServicePurch.DirServiceContractorName,
                            DirServiceContractorPhone = docMovementTabs.docServicePurch.DirServiceContractorPhone,
                            PrepaymentSum = docMovementTabs.docServicePurch.PrepaymentSum,

                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocServiceMovTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion


            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocServiceMovTabs/5
        [ResponseType(typeof(DocServiceMovTab))]
        public async Task<IHttpActionResult> GetDocServiceMovTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocServiceMovTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocServiceMovTab(int id, DocServiceMovTab docServiceMovTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocServiceMovTabs
        [ResponseType(typeof(DocServiceMovTab))]
        public async Task<IHttpActionResult> PostDocServiceMovTab(DocServiceMovTab docServiceMovTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocServiceMovTabs/5
        [ResponseType(typeof(DocServiceMovTab))]
        public async Task<IHttpActionResult> DeleteDocServiceMovTab(int id)
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

        private bool DocServiceMovTabExists(int id)
        {
            return db.DocServiceMovTabs.Count(e => e.DocServiceMovTabID == id) > 0;
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
                "SUM(DocServiceMovTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocServiceMovTabs.Quantity * DocServiceMovTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocServiceMovTabs.Quantity * DocServiceMovTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords' " + //Приписью

                "FROM Docs, DocServiceMovs, DocServiceMovTabs " +
                "WHERE " +
                "(Docs.DocID=DocServiceMovs.DocID)and" +
                "(DocServiceMovs.DocServiceMovID=DocServiceMovTabs.DocServiceMovID)and(Docs.DocID=@DocID)";

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


                //"[DirServiceNomens].[DirServiceNomenName] AS [DirServiceNomenName], " +

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
                "[DirServiceNomens].[DirServiceNomenArticle] AS [DirServiceNomenArticle], " +
                "[DirServiceNomens].[DirServiceNomenMinimumBalance] AS [DirServiceNomenMinimumBalance], " +
                "[DirServiceNomens].[DirServiceNomenNameFull] AS [DirServiceNomenNameFull], " +
                "[DirServiceNomens].[DescriptionFull] AS [DescriptionFull], " +
                //"[DirServiceNomenGroups].[DirServiceNomenName] AS [DirServiceNomenGroupName], " + //Группа (Sub)

                "[DocServiceMovTabs].[DocServiceMovTabID] AS [DocServiceMovTabID], " +
                "[DocServiceMovTabs].[DocServiceMovID] AS [DocServiceMovID], " +
                "[DocServiceMovTabs].[DirServiceNomenID] AS [DirServiceNomenID], " +
                "[DocServiceMovTabs].[Quantity] AS [Quantity], " +
                "[DocServiceMovTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocServiceMovTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocServiceMovTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocServiceMovTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocServiceMovTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocServiceMovTabs].[DirCurrencyRate] || ', ' || [DocServiceMovTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocServiceMovTabs].[DirCharColourID] AS [DirCharColourID], " +
                "[DirCharColours].[DirCharColourName] AS [DirCharColourName], " +
                "[DocServiceMovTabs].[DirCharMaterialID] AS [DirCharMaterialID], " +
                "[DirCharMaterials].[DirCharMaterialName] AS [DirCharMaterialName], " +
                "[DocServiceMovTabs].[DirCharNameID] AS [DirCharNameID], " +
                "[DirCharNames].[DirCharNameName] AS [DirCharNameName], " +
                "[DocServiceMovTabs].[DirCharSeasonID] AS [DirCharSeasonID], " +
                "[DirCharSeasons].[DirCharSeasonName] AS [DirCharSeasonName], " +
                "[DocServiceMovTabs].[DirCharSexID] AS [DirCharSexID], " +
                "[DirCharSexes].[DirCharSexName] AS [DirCharSexName], " +
                "[DocServiceMovTabs].[DirCharSizeID] AS [DirCharSizeID], " +
                "[DirCharSizes].[DirCharSizeName] AS [DirCharSizeName], " +
                "[DocServiceMovTabs].[DirCharStyleID] AS [DirCharStyleID], " +
                "[DirCharStyles].[DirCharStyleName] AS [DirCharStyleName], " +
                "[DocServiceMovTabs].[DirCharTextureID] AS [DirCharTextureID], " +
                "[DirCharTextures].[DirCharTextureName] AS [DirCharTextureName], " +
                "CASE WHEN ([DirCharColours].[DirCharColourName] IS NULL) THEN '' ELSE [DirCharColours].[DirCharColourName] END || ' ' || CASE WHEN ([DirCharMaterials].[DirCharMaterialName] IS NULL) THEN '' ELSE [DirCharMaterials].[DirCharMaterialName] END || ' ' || CASE WHEN ([DirCharNames].[DirCharNameName] IS NULL) THEN '' ELSE [DirCharNames].[DirCharNameName] END || ' ' || CASE WHEN ([DirCharSeasons].[DirCharSeasonName] IS NULL) THEN '' ELSE [DirCharSeasons].[DirCharSeasonName] END || ' ' || CASE WHEN ([DirCharSexes].[DirCharSexName] IS NULL) THEN '' ELSE [DirCharSexes].[DirCharSexName] END || ' ' || CASE WHEN ([DirCharSizes].[DirCharSizeName] IS NULL) THEN '' ELSE [DirCharSizes].[DirCharSizeName] END || ' ' || CASE WHEN ([DirCharStyles].[DirCharStyleName] IS NULL) THEN '' ELSE [DirCharStyles].[DirCharStyleName] END || ' ' || CASE WHEN ([DirCharTextures].[DirCharTextureName] IS NULL) THEN '' ELSE [DirCharTextures].[DirCharTextureName] END AS [DirChar], " +
                "[DocServiceMovTabs].[SerialNumber] AS [SerialNumber], " +
                "[DocServiceMovTabs].[Barcode] AS [Barcode], " +
                "[DocServiceMovTabs].[Barcode] AS [BarcodeImage], " +

                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocServiceMovTabs].[PriceCurrency] AS [PriceCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocServiceMovTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocServiceMovTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +

                //Себестоимось прихода
                "ROUND([DocServiceMovTabs].[Quantity] * [DocServiceMovTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocServiceMovTabs].[Quantity] * [DocServiceMovTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SumPurchPriceCurrency], " +


                //Розница
                "[DocServiceMovTabs].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[DocServiceMovTabs].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость в валюте
                "ROUND([DocServiceMovTabs].[Quantity] * [DocServiceMovTabs].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocServiceMovTabs].[Quantity] * [DocServiceMovTabs].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " +

                //Опт
                "[DocServiceMovTabs].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[DocServiceMovTabs].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость в валюте
                "ROUND([DocServiceMovTabs].[Quantity] * [DocServiceMovTabs].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocServiceMovTabs].[Quantity] * [DocServiceMovTabs].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " +

                //Интернет-Магазин
                "[DocServiceMovTabs].[PriceIMVAT] AS [PriceIMVAT], " +
                "[DocServiceMovTabs].[PriceIMCurrency] AS [PriceIMCurrency], " +
                //Стоимость в валюте
                "ROUND([DocServiceMovTabs].[Quantity] * [DocServiceMovTabs].[PriceIMVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMIMPriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocServiceMovTabs].[Quantity] * [DocServiceMovTabs].[PriceIMCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMIMPriceCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocServiceMovs], [DocServiceMovTabs] " +

                "LEFT OUTER JOIN [DirCharColours] ON [DocServiceMovTabs].[DirCharColourID] = [DirCharColours].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] ON [DocServiceMovTabs].[DirCharMaterialID] = [DirCharMaterials].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] ON [DocServiceMovTabs].[DirCharNameID] = [DirCharNames].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] ON [DocServiceMovTabs].[DirCharSeasonID] = [DirCharSeasons].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] ON [DocServiceMovTabs].[DirCharSexID] = [DirCharSexes].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] ON [DocServiceMovTabs].[DirCharSizeID] = [DirCharSizes].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] ON [DocServiceMovTabs].[DirCharStyleID] = [DirCharStyles].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] ON [DocServiceMovTabs].[DirCharTextureID] = [DirCharTextures].[DirCharTextureID] " +
                //"INNER JOIN [DirServiceNomens] ON [DocServiceMovTabs].[DirServiceNomenID] = [DirServiceNomens].[DirServiceNomenID] " +
                //"LEFT OUTER JOIN [DirServiceNomens] AS [DirServiceNomenGroups] ON [DirServiceNomenGroups].[Sub] = [DirServiceNomens].[DirServiceNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocServiceMovTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocServiceMovTabs].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [dirServiceNomensGroup].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocServiceMovs].[DocID])and([DocServiceMovs].[DocServiceMovID]=[DocServiceMovTabs].[DocServiceMovID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion

    }
}