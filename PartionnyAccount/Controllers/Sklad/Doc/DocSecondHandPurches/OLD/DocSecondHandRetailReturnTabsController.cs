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
    public class DocSecondHandRetailReturnTabsController : ApiController
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
            public int DocSecondHandRetailReturnID;
            //public int DocID;

            public DateTime? DateS, DatePo;
            public int DirWarehouseID;
        }
        // GET: api/DocSecondHandRetailReturnTabs
        public async Task<IHttpActionResult> GetDocSecondHandRetailReturnTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailReturns"));
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
                _params.DocSecondHandRetailReturnID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandRetailReturnID", true) == 0).Value);

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
                        from docSecondHandRetailReturnTabs in db.DocSecondHandRetailReturnTabs

                        join dirServiceNomens11 in db.DirServiceNomens on docSecondHandRetailReturnTabs.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                            /*
                            //Характеристики
                            join dirCharColours1 in db.DirCharColours on docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                            from dirCharColours in dirCharColours2.DefaultIfEmpty()

                            join dirCharMaterials1 in db.DirCharMaterials on docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                            from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                            join dirCharNames1 in db.DirCharNames on docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                            from dirCharNames in dirCharNames2.DefaultIfEmpty()

                            join dirCharSeasons1 in db.DirCharSeasons on docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                            from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                            join dirCharSexes1 in db.DirCharSexes on docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                            from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                            join dirCharSizes1 in db.DirCharSizes on docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                            from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                            join dirCharStyles1 in db.DirCharStyles on docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                            from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                            join dirCharTextures1 in db.DirCharTextures on docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                            from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                            */

                            //where docSecondHandRetailReturnTabs.DocSecondHandRetailReturnID == _params.DocSecondHandRetailReturnID

                            #region select

                        select new
                        {
                            DocID = docSecondHandRetailReturnTabs.docSecondHandRetailReturn.DocID,
                            DocDate = docSecondHandRetailReturnTabs.docSecondHandRetailReturn.doc.DocDate,
                            Held = docSecondHandRetailReturnTabs.docSecondHandRetailReturn.doc.Held,
                            Discount = docSecondHandRetailReturnTabs.docSecondHandRetailReturn.doc.Discount,
                            DirWarehouseID = docSecondHandRetailReturnTabs.docSecondHandRetailReturn.DirWarehouseID,

                            //партия
                            Rem2PartyMinusID = docSecondHandRetailReturnTabs.rem2PartyMinus.Rem2PartyMinusID,

                            DocSecondHandRetailReturnTabID = docSecondHandRetailReturnTabs.DocSecondHandRetailReturnTabID,
                            DocSecondHandRetailReturnID = docSecondHandRetailReturnTabs.DocSecondHandRetailReturnID,
                            DirServiceNomenID = docSecondHandRetailReturnTabs.DirServiceNomenID,

                            //DirServiceNomenName = docSecondHandRetailReturnTabs.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? docSecondHandRetailReturnTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docSecondHandRetailReturnTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docSecondHandRetailReturnTabs.dirServiceNomen.DirServiceNomenName,

                            Quantity = docSecondHandRetailReturnTabs.Quantity,
                            PriceVAT = docSecondHandRetailReturnTabs.PriceVAT,

                            DirCurrencyID = docSecondHandRetailReturnTabs.DirCurrencyID,
                            DirCurrencyRate = docSecondHandRetailReturnTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSecondHandRetailReturnTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docSecondHandRetailReturnTabs.dirCurrency.DirCurrencyName + " (" + docSecondHandRetailReturnTabs.DirCurrencyRate + ", " + docSecondHandRetailReturnTabs.DirCurrencyMultiplicity + ")",

                            DirEmployeeName = docSecondHandRetailReturnTabs.docSecondHandRetailReturn.doc.dirEmployee.DirEmployeeName,

                            //Характеристики
                            /*
                            DirCharColourID = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharColourID,
                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialID = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharMaterialID,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameID = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharNameID,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonID = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharSeasonID,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexID = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharSexID,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeID = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharSizeID,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleID = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharStyleID,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureID = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharTextureID,
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
                            SerialNumber = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.SerialNumber,
                            Barcode = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.Barcode,


                            //Цена в т.в.
                            PriceCurrency = docSecondHandRetailReturnTabs.PriceCurrency,
                            //Себестоимость
                            SUMPurchPriceVATCurrency = docSecondHandRetailReturnTabs.Quantity * docSecondHandRetailReturnTabs.PriceCurrency == null ? 0
                            : Math.Round(docSecondHandRetailReturnTabs.Quantity * docSecondHandRetailReturnTabs.PriceCurrency, sysSetting.FractionalPartInSum),


                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceRetailVAT - docSecondHandRetailReturnTabs.PriceVAT) / docSecondHandRetailReturnTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceRetailVAT - docSecondHandRetailReturnTabs.PriceVAT) / docSecondHandRetailReturnTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceWholesaleVAT - docSecondHandRetailReturnTabs.PriceVAT) / docSecondHandRetailReturnTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceWholesaleVAT - docSecondHandRetailReturnTabs.PriceVAT) / docSecondHandRetailReturnTabs.PriceVAT, sysSetting.FractionalPartInSum)
                            ,
                            //Оптовая цена 
                            PriceWholesaleVAT = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceIMVAT - docSecondHandRetailReturnTabs.PriceVAT) / docSecondHandRetailReturnTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceIMVAT - docSecondHandRetailReturnTabs.PriceVAT) / docSecondHandRetailReturnTabs.PriceVAT, sysSetting.FractionalPartInSum)
                            ,
                            //Интернет-Магазин
                            PriceIMVAT = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.PriceIMCurrency,

                        }

                        #endregion
                    );


                //Параметры
                //1. По документу
                if (_params.DocSecondHandRetailReturnID > 0) query = query.Where(x => x.DocSecondHandRetailReturnID == _params.DocSecondHandRetailReturnID);
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
                    DocSecondHandRetailReturnTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandRetailReturnTabs/5
        [ResponseType(typeof(DocSecondHandRetailReturnTab))]
        public async Task<IHttpActionResult> GetDocSecondHandRetailReturnTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandRetailReturnTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRetailReturnTab(int id, DocSecondHandRetailReturnTab docSecondHandRetailReturnTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandRetailReturnTabs
        [ResponseType(typeof(DocSecondHandRetailReturnTab))]
        public async Task<IHttpActionResult> PostDocSecondHandRetailReturnTab(DocSecondHandRetailReturnTab docSecondHandRetailReturnTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSecondHandRetailReturnTabs/5
        [ResponseType(typeof(DocSecondHandRetailReturnTab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRetailReturnTab(int id)
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

        private bool DocSecondHandRetailReturnTabExists(int id)
        {
            return db.DocSecondHandRetailReturnTabs.Count(e => e.DocSecondHandRetailReturnTabID == id) > 0;
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
                "SUM(DocSecondHandRetailReturnTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocSecondHandRetailReturnTabs.Quantity * DocSecondHandRetailReturnTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocSecondHandRetailReturnTabs.Quantity * DocSecondHandRetailReturnTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocSecondHandRetailReturnTabs.Quantity * (DocSecondHandRetailReturnTabs.PriceCurrency - (DocSecondHandRetailReturnTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocSecondHandRetailReturnTabs.Quantity * (DocSecondHandRetailReturnTabs.PriceCurrency - (DocSecondHandRetailReturnTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocSecondHandRetailReturnTabs.Quantity * DocSecondHandRetailReturnTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocSecondHandRetailReturnTabs.Quantity * DocSecondHandRetailReturnTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocSecondHandRetailReturns, DocSecondHandRetailReturnTabs " +
                "WHERE " +
                "(Docs.DocID=DocSecondHandRetailReturns.DocID)and" +
                "(DocSecondHandRetailReturns.DocSecondHandRetailReturnID=DocSecondHandRetailReturnTabs.DocSecondHandRetailReturnID)and(Docs.DocID=@DocID)";

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

                "[DirServiceNomens].[DirServiceNomenName] AS [DirServiceNomenName], " +
                "[DirServiceNomens].[DirServiceNomenName] AS [DirServiceNomenNameRemove], " +
                
                //"[DirServiceNomens].[DirServiceNomenArticle] AS [DirServiceNomenArticle], " +
                //"[DirServiceNomens].[DirServiceNomenMinimumBalance] AS [DirServiceNomenMinimumBalance], " +
                "[DirServiceNomens].[DirServiceNomenNameFull] AS [DirServiceNomenNameFull], " +
                "[DirServiceNomens].[DescriptionFull] AS [DescriptionFull], " +
                "[DirServiceNomenGroups].[DirServiceNomenName] AS [DirServiceNomenGroupName], " + //Группа (Sub)

                "[DocSecondHandRetailReturnTabs].[DocSecondHandRetailReturnTabID] AS [DocSecondHandRetailReturnTabID], " +
                "[DocSecondHandRetailReturnTabs].[DocSecondHandRetailReturnID] AS [DocSecondHandRetailReturnID], " +
                "[DocSecondHandRetailReturnTabs].[DirServiceNomenID] AS [DirServiceNomenID], " +
                "[DocSecondHandRetailReturnTabs].[Quantity] AS [Quantity], " +
                "[DocSecondHandRetailReturnTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocSecondHandRetailReturnTabs].[DirCurrencyRate] AS [DirCu, " +
                "[DocSecondHandRetailReturnTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocSecondHandRetailReturnTabs].[PriceVAT] AS [PriceVAT]rrencyRate], " +
                "[DocSecondHandRetailReturnTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandRetailReturnTabs].[DirCurrencyRate] || ', ' || [DocSecondHandRetailReturnTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                //"[DocSecondHandRetailReturnTabs].[DirCharColourID] AS [DirCharColourID], " +
                "[DirCharColours].[DirCharColourName] AS [DirCharColourName], " +
                //"[DocSecondHandRetailReturnTabs].[DirCharMaterialID] AS [DirCharMaterialID], " +
                "[DirCharMaterials].[DirCharMaterialName] AS [DirCharMaterialName], " +
                //"[DocSecondHandRetailReturnTabs].[DirCharNameID] AS [DirCharNameID], " +
                "[DirCharNames].[DirCharNameName] AS [DirCharNameName], " +
                //"[DocSecondHandRetailReturnTabs].[DirCharSeasonID] AS [DirCharSeasonID], " +
                "[DirCharSeasons].[DirCharSeasonName] AS [DirCharSeasonName], " +
                //"[DocSecondHandRetailReturnTabs].[DirCharSexID] AS [DirCharSexID], " +
                "[DirCharSexes].[DirCharSexName] AS [DirCharSexName], " +
                //"[DocSecondHandRetailReturnTabs].[DirCharSizeID] AS [DirCharSizeID], " +
                "[DirCharSizes].[DirCharSizeName] AS [DirCharSizeName], " +
                //"[DocSecondHandRetailReturnTabs].[DirCharStyleID] AS [DirCharStyleID], " +
                "[DirCharStyles].[DirCharStyleName] AS [DirCharStyleName], " +
                //"[DocSecondHandRetailReturnTabs].[DirCharTextureID] AS [DirCharTextureID], " +
                "[DirCharTextures].[DirCharTextureName] AS [DirCharTextureName], " +
                "CASE WHEN ([DirCharColours].[DirCharColourName] IS NULL) THEN '' ELSE [DirCharColours].[DirCharColourName] END || ' ' || CASE WHEN ([DirCharMaterials].[DirCharMaterialName] IS NULL) THEN '' ELSE [DirCharMaterials].[DirCharMaterialName] END || ' ' || CASE WHEN ([DirCharNames].[DirCharNameName] IS NULL) THEN '' ELSE [DirCharNames].[DirCharNameName] END || ' ' || CASE WHEN ([DirCharSeasons].[DirCharSeasonName] IS NULL) THEN '' ELSE [DirCharSeasons].[DirCharSeasonName] END || ' ' || CASE WHEN ([DirCharSexes].[DirCharSexName] IS NULL) THEN '' ELSE [DirCharSexes].[DirCharSexName] END || ' ' || CASE WHEN ([DirCharSizes].[DirCharSizeName] IS NULL) THEN '' ELSE [DirCharSizes].[DirCharSizeName] END || ' ' || CASE WHEN ([DirCharStyles].[DirCharStyleName] IS NULL) THEN '' ELSE [DirCharStyles].[DirCharStyleName] END || ' ' || CASE WHEN ([DirCharTextures].[DirCharTextureName] IS NULL) THEN '' ELSE [DirCharTextures].[DirCharTextureName] END AS [DirChar], " +
                "[Rem2Parties].[SerialNumber] AS [SerialNumber], " +
                "[Rem2Parties].[Barcode] AS [Barcode], " +
                "[Rem2Parties].[Barcode] AS [BarcodeImage], " +

                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocSecondHandRetailReturnTabs].[PriceCurrency] AS [PriceCurrency], " +
                //"Цена без НДС" в валюте
                "ROUND([DocSecondHandRetailReturnTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVAT], " +
                //"Цена без НДС" в текущей валюте
                "ROUND([DocSecondHandRetailReturnTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVATCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocSecondHandRetailReturnTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocSecondHandRetailReturnTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +
                //"Цена с НДС" в текущей валюте со Скидкой
                "ROUND([DocSecondHandRetailReturnTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'PriceCurrencyDiscount', " +
                //"Сумма НДС" (НДС документа)
                "ROUND([DocSecondHandRetailReturnTabs].[Quantity] * ([DocSecondHandRetailReturnTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                //"Стоимость без НДС" в валюте
                "ROUND(([DocSecondHandRetailReturnTabs].[Quantity] * [DocSecondHandRetailReturnTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVAT], " +
                //"Стоимость Прихода без НДС" в текущей валюте
                "ROUND(([DocSecondHandRetailReturnTabs].[Quantity] * [DocSecondHandRetailReturnTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVATCurrency], " +
                //Себестоимось прихода
                "ROUND([DocSecondHandRetailReturnTabs].[Quantity] * [DocSecondHandRetailReturnTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocSecondHandRetailReturnTabs].[Quantity] * [DocSecondHandRetailReturnTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVATCurrency], " +
                //Стоимость с НДС в текущей валюте со Скидкой
                "ROUND([DocSecondHandRetailReturnTabs].[Quantity] * [DocSecondHandRetailReturnTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " +


                //Розница
                "ROUND((100 * ([Rem2Parties].[PriceRetailVAT] - [Rem2Parties].[PriceVAT])) / [Rem2Parties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupRetail], " +
                "[Rem2Parties].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[Rem2Parties].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость Розницы в валюте
                "ROUND([DocSecondHandRetailReturnTabs].[Quantity] * [Rem2Parties].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость Розницы в текущей валюте
                "ROUND([DocSecondHandRetailReturnTabs].[Quantity] * [Rem2Parties].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " +

                //Опт
                "ROUND((100 * ([Rem2Parties].[PriceWholesaleVAT] - [Rem2Parties].[PriceVAT])) / [Rem2Parties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupWholesale], " +
                "[Rem2Parties].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[Rem2Parties].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость Опта в валюте
                "ROUND([DocSecondHandRetailReturnTabs].[Quantity] * [Rem2Parties].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость Опта в текущей валюте
                "ROUND([DocSecondHandRetailReturnTabs].[Quantity] * [Rem2Parties].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " +

                //Интернет-Магазин
                "ROUND((100 * ([Rem2Parties].[PriceIMVAT] - [Rem2Parties].[PriceVAT])) / [Rem2Parties].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupIM], " +
                "[Rem2Parties].[PriceIMVAT] AS [PriceIMVAT], " +
                "[Rem2Parties].[PriceIMCurrency] AS [PriceIMCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocSecondHandRetailReturns], [DocSecondHandRetailReturnTabs] " +

                //docSecondHandRetailReturnTabs.rem2PartyMinus.rem2Party.DirCharColourID
                "LEFT OUTER JOIN [Rem2PartyMinuses] ON [DocSecondHandRetailReturnTabs].[Rem2PartyMinusID] = [Rem2PartyMinuses].[Rem2PartyMinusID] " +
                "LEFT OUTER JOIN [Rem2Parties] ON [Rem2PartyMinuses].[Rem2PartyID] = [Rem2Parties].[Rem2PartyID] " +

                "LEFT OUTER JOIN [DirCharColours] ON [Rem2Parties].[DirCharColourID] = [DirCharColours].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] ON [Rem2Parties].[DirCharMaterialID] = [DirCharMaterials].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] ON [Rem2Parties].[DirCharNameID] = [DirCharNames].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] ON [Rem2Parties].[DirCharSeasonID] = [DirCharSeasons].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] ON [Rem2Parties].[DirCharSexID] = [DirCharSexes].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] ON [Rem2Parties].[DirCharSizeID] = [DirCharSizes].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] ON [Rem2Parties].[DirCharStyleID] = [DirCharStyles].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] ON [Rem2Parties].[DirCharTextureID] = [DirCharTextures].[DirCharTextureID] " +
                "INNER JOIN [DirServiceNomens] ON [Rem2Parties].[DirServiceNomenID] = [DirServiceNomens].[DirServiceNomenID] " +
                "LEFT OUTER JOIN [DirServiceNomens] AS [DirServiceNomenGroups] ON [DirServiceNomenGroups].[Sub] = [DirServiceNomens].[DirServiceNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocSecondHandRetailReturnTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                "WHERE ([Docs].[DocID]=[DocSecondHandRetailReturns].[DocID])and([DocSecondHandRetailReturns].[DocSecondHandRetailReturnID]=[DocSecondHandRetailReturnTabs].[DocSecondHandRetailReturnID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}