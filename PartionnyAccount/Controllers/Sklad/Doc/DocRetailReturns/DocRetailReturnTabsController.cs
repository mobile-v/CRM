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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocRetailReturns
{
    public class DocRetailReturnTabsController : ApiController
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
            public int DocRetailReturnID;
            //public int DocID;

            public DateTime? DateS, DatePo;
            public int DirWarehouseID;
        }
        // GET: api/DocRetailReturnTabs
        public async Task<IHttpActionResult> GetDocRetailReturnTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetailReturns"));
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
                _params.DocRetailReturnID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocRetailReturnID", true) == 0).Value);

                _params.DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DateS < Convert.ToDateTime("01.01.1800")) _params.DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else _params.DateS = _params.DateS.Value.AddDays(-1);

                _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDate", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docRetailReturnTabs in db.DocRetailReturnTabs

                        join dirNomens11 in db.DirNomens on docRetailReturnTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        /*
                        //Характеристики
                        join dirCharColours1 in db.DirCharColours on docRetailReturnTabs.remPartyMinus.remParty.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on docRetailReturnTabs.remPartyMinus.remParty.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on docRetailReturnTabs.remPartyMinus.remParty.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on docRetailReturnTabs.remPartyMinus.remParty.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on docRetailReturnTabs.remPartyMinus.remParty.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on docRetailReturnTabs.remPartyMinus.remParty.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on docRetailReturnTabs.remPartyMinus.remParty.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on docRetailReturnTabs.remPartyMinus.remParty.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */

                        //where docRetailReturnTabs.DocRetailReturnID == _params.DocRetailReturnID

                        #region select

                        select new
                        {
                            DocID = docRetailReturnTabs.docRetailReturn.DocID,
                            DocDate = docRetailReturnTabs.docRetailReturn.doc.DocDate,
                            Held = docRetailReturnTabs.docRetailReturn.doc.Held,
                            Discount = docRetailReturnTabs.docRetailReturn.doc.Discount,
                            DirWarehouseID = docRetailReturnTabs.docRetailReturn.DirWarehouseID,

                            //партия
                            RemPartyMinusID = docRetailReturnTabs.remPartyMinus.RemPartyMinusID,

                            DocRetailReturnTabID = docRetailReturnTabs.DocRetailReturnTabID,
                            DocRetailReturnID = docRetailReturnTabs.DocRetailReturnID,
                            DirNomenID = docRetailReturnTabs.DirNomenID,

                            //DirNomenName = docRetailReturnTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docRetailReturnTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docRetailReturnTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docRetailReturnTabs.dirNomen.DirNomenName,

                            Quantity = docRetailReturnTabs.Quantity,
                            PriceVAT = docRetailReturnTabs.PriceVAT,

                            DirCurrencyID = docRetailReturnTabs.DirCurrencyID,
                            DirCurrencyRate = docRetailReturnTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docRetailReturnTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docRetailReturnTabs.dirCurrency.DirCurrencyName + " (" + docRetailReturnTabs.DirCurrencyRate + ", " + docRetailReturnTabs.DirCurrencyMultiplicity + ")",

                            DirEmployeeName = docRetailReturnTabs.docRetailReturn.doc.dirEmployee.DirEmployeeName,

                            //Характеристики
                            /*
                            DirCharColourID = docRetailReturnTabs.remPartyMinus.remParty.DirCharColourID,
                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialID = docRetailReturnTabs.remPartyMinus.remParty.DirCharMaterialID,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameID = docRetailReturnTabs.remPartyMinus.remParty.DirCharNameID,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonID = docRetailReturnTabs.remPartyMinus.remParty.DirCharSeasonID,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexID = docRetailReturnTabs.remPartyMinus.remParty.DirCharSexID,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeID = docRetailReturnTabs.remPartyMinus.remParty.DirCharSizeID,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleID = docRetailReturnTabs.remPartyMinus.remParty.DirCharStyleID,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureID = docRetailReturnTabs.remPartyMinus.remParty.DirCharTextureID,
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
                            SerialNumber = docRetailReturnTabs.remPartyMinus.remParty.SerialNumber,
                            Barcode = docRetailReturnTabs.remPartyMinus.remParty.Barcode,


                            //Цена в т.в.
                            PriceCurrency = docRetailReturnTabs.PriceCurrency,
                            //Себестоимость
                            SUMPurchPriceVATCurrency = docRetailReturnTabs.Quantity * docRetailReturnTabs.PriceCurrency == null ? 0
                            : Math.Round(docRetailReturnTabs.Quantity * docRetailReturnTabs.PriceCurrency, sysSetting.FractionalPartInSum),


                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docRetailReturnTabs.remPartyMinus.remParty.PriceRetailVAT - docRetailReturnTabs.PriceVAT) / docRetailReturnTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docRetailReturnTabs.remPartyMinus.remParty.PriceRetailVAT - docRetailReturnTabs.PriceVAT) / docRetailReturnTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docRetailReturnTabs.remPartyMinus.remParty.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docRetailReturnTabs.remPartyMinus.remParty.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docRetailReturnTabs.remPartyMinus.remParty.PriceWholesaleVAT - docRetailReturnTabs.PriceVAT) / docRetailReturnTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docRetailReturnTabs.remPartyMinus.remParty.PriceWholesaleVAT - docRetailReturnTabs.PriceVAT) / docRetailReturnTabs.PriceVAT, sysSetting.FractionalPartInSum)
                            ,
                            //Оптовая цена 
                            PriceWholesaleVAT = docRetailReturnTabs.remPartyMinus.remParty.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docRetailReturnTabs.remPartyMinus.remParty.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docRetailReturnTabs.remPartyMinus.remParty.PriceIMVAT - docRetailReturnTabs.PriceVAT) / docRetailReturnTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docRetailReturnTabs.remPartyMinus.remParty.PriceIMVAT - docRetailReturnTabs.PriceVAT) / docRetailReturnTabs.PriceVAT, sysSetting.FractionalPartInSum)
                            ,
                            //Интернет-Магазин
                            PriceIMVAT = docRetailReturnTabs.remPartyMinus.remParty.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docRetailReturnTabs.remPartyMinus.remParty.PriceIMCurrency,

                        }

                        #endregion
                    );


                //Параметры
                //1. По документу
                if (_params.DocRetailReturnID > 0) query = query.Where(x => x.DocRetailReturnID == _params.DocRetailReturnID);
                //2. По дате
                if (_params.DateS != null) query = query.Where(x => x.DocDate >= _params.DateS && x.DocDate <= _params.DatePo);
                //3. Склад
                if (_params.DirWarehouseID > 0) query = query.Where(x => x.DirWarehouseID == _params.DirWarehouseID);


                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocRetailReturnTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocRetailReturnTabs/5
        [ResponseType(typeof(DocRetailReturnTab))]
        public async Task<IHttpActionResult> GetDocRetailReturnTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocRetailReturnTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocRetailReturnTab(int id, DocRetailReturnTab docRetailReturnTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocRetailReturnTabs
        [ResponseType(typeof(DocRetailReturnTab))]
        public async Task<IHttpActionResult> PostDocRetailReturnTab(DocRetailReturnTab docRetailReturnTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocRetailReturnTabs/5
        [ResponseType(typeof(DocRetailReturnTab))]
        public async Task<IHttpActionResult> DeleteDocRetailReturnTab(int id)
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

        private bool DocRetailReturnTabExists(int id)
        {
            return db.DocRetailReturnTabs.Count(e => e.DocRetailReturnTabID == id) > 0;
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
                "SUM(DocRetailReturnTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocRetailReturnTabs.Quantity * DocRetailReturnTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocRetailReturnTabs.Quantity * DocRetailReturnTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocRetailReturnTabs.Quantity * (DocRetailReturnTabs.PriceCurrency - (DocRetailReturnTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocRetailReturnTabs.Quantity * (DocRetailReturnTabs.PriceCurrency - (DocRetailReturnTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocRetailReturnTabs.Quantity * DocRetailReturnTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocRetailReturnTabs.Quantity * DocRetailReturnTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocRetailReturns, DocRetailReturnTabs " +
                "WHERE " +
                "(Docs.DocID=DocRetailReturns.DocID)and" +
                "(DocRetailReturns.DocRetailReturnID=DocRetailReturnTabs.DocRetailReturnID)and(Docs.DocID=@DocID)";

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

                "[DirNomens].[DirNomenName] AS [DirNomenName], " +
                "[DirNomens].[DirNomenName] AS [DirNomenNameRemove], " +
                "[DirNomens].[DirNomenArticle] AS [DirNomenArticle], " +
                "[DirNomens].[DirNomenMinimumBalance] AS [DirNomenMinimumBalance], " +
                "[DirNomens].[DirNomenNameFull] AS [DirNomenNameFull], " +
                "[DirNomens].[DescriptionFull] AS [DescriptionFull], " +
                "[DirNomenGroups].[DirNomenName] AS [DirNomenGroupName], " + //Группа (Sub)

                "[DocRetailReturnTabs].[DocRetailReturnTabID] AS [DocRetailReturnTabID], " +
                "[DocRetailReturnTabs].[DocRetailReturnID] AS [DocRetailReturnID], " +
                "[DocRetailReturnTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocRetailReturnTabs].[Quantity] AS [Quantity], " +
                "[DocRetailReturnTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocRetailReturnTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocRetailReturnTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocRetailReturnTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocRetailReturnTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocRetailReturnTabs].[DirCurrencyRate] || ', ' || [DocRetailReturnTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                //"[DocRetailReturnTabs].[DirCharColourID] AS [DirCharColourID], " +
                "[DirCharColours].[DirCharColourName] AS [DirCharColourName], " +
                //"[DocRetailReturnTabs].[DirCharMaterialID] AS [DirCharMaterialID], " +
                "[DirCharMaterials].[DirCharMaterialName] AS [DirCharMaterialName], " +
                //"[DocRetailReturnTabs].[DirCharNameID] AS [DirCharNameID], " +
                "[DirCharNames].[DirCharNameName] AS [DirCharNameName], " +
                //"[DocRetailReturnTabs].[DirCharSeasonID] AS [DirCharSeasonID], " +
                "[DirCharSeasons].[DirCharSeasonName] AS [DirCharSeasonName], " +
                //"[DocRetailReturnTabs].[DirCharSexID] AS [DirCharSexID], " +
                "[DirCharSexes].[DirCharSexName] AS [DirCharSexName], " +
                //"[DocRetailReturnTabs].[DirCharSizeID] AS [DirCharSizeID], " +
                "[DirCharSizes].[DirCharSizeName] AS [DirCharSizeName], " +
                //"[DocRetailReturnTabs].[DirCharStyleID] AS [DirCharStyleID], " +
                "[DirCharStyles].[DirCharStyleName] AS [DirCharStyleName], " +
                //"[DocRetailReturnTabs].[DirCharTextureID] AS [DirCharTextureID], " +
                "[DirCharTextures].[DirCharTextureName] AS [DirCharTextureName], " +
                "CASE WHEN ([DirCharColours].[DirCharColourName] IS NULL) THEN '' ELSE [DirCharColours].[DirCharColourName] END || ' ' || CASE WHEN ([DirCharMaterials].[DirCharMaterialName] IS NULL) THEN '' ELSE [DirCharMaterials].[DirCharMaterialName] END || ' ' || CASE WHEN ([DirCharNames].[DirCharNameName] IS NULL) THEN '' ELSE [DirCharNames].[DirCharNameName] END || ' ' || CASE WHEN ([DirCharSeasons].[DirCharSeasonName] IS NULL) THEN '' ELSE [DirCharSeasons].[DirCharSeasonName] END || ' ' || CASE WHEN ([DirCharSexes].[DirCharSexName] IS NULL) THEN '' ELSE [DirCharSexes].[DirCharSexName] END || ' ' || CASE WHEN ([DirCharSizes].[DirCharSizeName] IS NULL) THEN '' ELSE [DirCharSizes].[DirCharSizeName] END || ' ' || CASE WHEN ([DirCharStyles].[DirCharStyleName] IS NULL) THEN '' ELSE [DirCharStyles].[DirCharStyleName] END || ' ' || CASE WHEN ([DirCharTextures].[DirCharTextureName] IS NULL) THEN '' ELSE [DirCharTextures].[DirCharTextureName] END AS [DirChar], " +
                "[RemParties].[SerialNumber] AS [SerialNumber], " +
                "[RemParties].[Barcode] AS [Barcode], " +
                "[RemParties].[Barcode] AS [BarcodeImage], " +

                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocRetailReturnTabs].[PriceCurrency] AS [PriceCurrency], " +
                //"Цена без НДС" в валюте
                "ROUND([DocRetailReturnTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVAT], " +
                //"Цена без НДС" в текущей валюте
                "ROUND([DocRetailReturnTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVATCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocRetailReturnTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocRetailReturnTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +
                //"Цена с НДС" в текущей валюте со Скидкой
                "ROUND([DocRetailReturnTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'PriceCurrencyDiscount', " +
                //"Сумма НДС" (НДС документа)
                "ROUND([DocRetailReturnTabs].[Quantity] * ([DocRetailReturnTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                //"Стоимость без НДС" в валюте
                "ROUND(([DocRetailReturnTabs].[Quantity] * [DocRetailReturnTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVAT], " +
                //"Стоимость Прихода без НДС" в текущей валюте
                "ROUND(([DocRetailReturnTabs].[Quantity] * [DocRetailReturnTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVATCurrency], " +
                //Себестоимось прихода
                "ROUND([DocRetailReturnTabs].[Quantity] * [DocRetailReturnTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocRetailReturnTabs].[Quantity] * [DocRetailReturnTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVATCurrency], " +
                //Стоимость с НДС в текущей валюте со Скидкой
                "ROUND([DocRetailReturnTabs].[Quantity] * [DocRetailReturnTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " +


                //Розница
                "ROUND((100 * ([RemParties].[PriceRetailVAT] - [RemParties].[PriceVAT])) / [RemParties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupRetail], " +
                "[RemParties].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[RemParties].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость Розницы в валюте
                "ROUND([DocRetailReturnTabs].[Quantity] * [RemParties].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость Розницы в текущей валюте
                "ROUND([DocRetailReturnTabs].[Quantity] * [RemParties].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " +

                //Опт
                "ROUND((100 * ([RemParties].[PriceWholesaleVAT] - [RemParties].[PriceVAT])) / [RemParties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupWholesale], " +
                "[RemParties].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[RemParties].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость Опта в валюте
                "ROUND([DocRetailReturnTabs].[Quantity] * [RemParties].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость Опта в текущей валюте
                "ROUND([DocRetailReturnTabs].[Quantity] * [RemParties].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " +

                //Интернет-Магазин
                "ROUND((100 * ([RemParties].[PriceIMVAT] - [RemParties].[PriceVAT])) / [RemParties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupIM], " +
                "[RemParties].[PriceIMVAT] AS [PriceIMVAT], " +
                "[RemParties].[PriceIMCurrency] AS [PriceIMCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocRetailReturns], [DocRetailReturnTabs] " +

                //docRetailReturnTabs.remPartyMinus.remParty.DirCharColourID
                "LEFT OUTER JOIN [RemPartyMinuses] ON [DocRetailReturnTabs].[RemPartyMinusID] = [RemPartyMinuses].[RemPartyMinusID] " +
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
                "LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocRetailReturnTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "WHERE ([Docs].[DocID]=[DocRetailReturns].[DocID])and([DocRetailReturns].[DocRetailReturnID]=[DocRetailReturnTabs].[DocRetailReturnID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}