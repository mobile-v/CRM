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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocRetails
{
    public class DocRetailTabsController : ApiController
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
            //public int DocID;
            public int? DocRetailID;
            public DateTime? DateS, DatePo;
            public int DirWarehouseID;
        }
        // GET: api/DocRetailTabs
        public async Task<IHttpActionResult> GetDocRetailTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocRetails"));
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
                _params.DocRetailID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocRetailID", true) == 0).Value);
                //_params.DocDate = Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDate", true) == 0).Value);

                _params.DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DateS < Convert.ToDateTime("01.01.1800")) _params.DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else _params.DateS = _params.DateS.Value.AddDays(-1);

                _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        //Розничный Чек

                        from x in db.RemPartyMinuses
                        from y in db.DocRetailTabs

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        where
                        x.FieldID == y.DocRetailTabID &&
                        x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo && x.doc.ListObjectID == 56 &&
                        x.DirWarehouseID == _params.DirWarehouseID

                        #region select

                        select new
                        {
                            
                            DocID = x.DocID,
                            KKMSCheckNumber = y.docRetail.doc.KKMSCheckNumber,
                            KKMSIdCommand = y.docRetail.doc.KKMSIdCommand,

                            DocDate = x.doc.DocDate,
                            Held = x.doc.Held,
                            Discount = x.doc.Discount,
                            DocRetailID = y.DocRetailID, //NumberReal = x.doc.NumberReal,
                            DocRetailReturnID = y.DocRetailID, //NumberReal = x.doc.NumberReal,
                            DirWarehouseID = x.DirWarehouseID,
                            ListObjectID = x.doc.listObject.ListObjectID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,

                            DirNomenID = x.DirNomenID,

                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            RemPartyID = x.RemPartyID,
                            RemPartyMinusID = x.RemPartyMinusID,
                            Quantity = x.Quantity,
                            PriceVAT = x.PriceVAT,

                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",

                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,

                            //RemParty
                            Barcode = x.remParty.Barcode,
                            SerialNumber = x.remParty.SerialNumber,

                            DirCharColourName = x.remParty.dirCharColour.DirCharColourName,
                            DirCharMaterialName = x.remParty.dirCharMaterial.DirCharMaterialName,
                            DirCharNameName = x.remParty.dirCharName.DirCharNameName,
                            DirCharSeasonName = x.remParty.dirCharSeason.DirCharSeasonName,
                            DirCharSexName = x.remParty.dirCharSex.DirCharSexName,
                            DirCharSizeName = x.remParty.dirCharSize.DirCharSizeName,
                            DirCharStyleName = x.remParty.dirCharStyle.DirCharStyleName,
                            DirCharTextureName = x.remParty.dirCharTexture.DirCharTextureName,
                            DirChar =
                                x.remParty.dirCharColour.DirCharColourName + " " +
                                x.remParty.dirCharMaterial.DirCharMaterialName + " " +
                                x.remParty.dirCharName.DirCharNameName + " " +
                                x.remParty.dirCharSeason.DirCharSeasonName + " " +
                                x.remParty.dirCharSex.DirCharSexName + " " +
                                x.remParty.dirCharSize.DirCharSizeName + " " +
                                x.remParty.dirCharStyle.DirCharStyleName + " " +
                                x.remParty.dirCharTexture.DirCharTextureName,
                                

                            //Приходная цена
                            //PriceCurrencyPurch = y.remParty.PriceCurrency, //x.remParty.PriceCurrency,

                            
                            DirPaymentTypeID = x.doc.DirPaymentTypeID,
                            //Цена в т.в.
                            PriceCurrency = x.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = x.Quantity * x.PriceCurrency - x.doc.Discount == null ? 0
                            : Math.Round(x.Quantity * x.PriceCurrency - x.doc.Discount, sysSetting.FractionalPartInSum),


                            //Причина возврата
                            DirReturnTypeID = y.DirReturnTypeID,
                            DirReturnTypeName = "",

                            DirDescriptionID = y.DirDescriptionID,
                            DirDescriptionName = "",
                            
                        }

                        #endregion

                    ).Union
                    (
                        //Розничный возврат

                        from x in db.DocRetailReturnTabs

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        where
                        //x.FieldID == y.DocRetailReturnTabID &&
                        x.docRetailReturn.doc.DocDate >= _params.DateS && x.docRetailReturn.doc.DocDate <= _params.DatePo && x.docRetailReturn.doc.ListObjectID == 57 &&
                        x.docRetailReturn.DirWarehouseID == _params.DirWarehouseID

                        join remPartyMinuses1 in db.RemPartyMinuses on x.RemPartyMinusID equals remPartyMinuses1.RemPartyMinusID into remPartyMinuses2
                        from remPartyMinuses in remPartyMinuses2.DefaultIfEmpty()

                        #region select

                        select new
                        {
                            
                            DocID = x.docRetailReturn.DocID,
                            KKMSCheckNumber = x.docRetailReturn.doc.KKMSCheckNumber,
                            KKMSIdCommand = x.docRetailReturn.doc.KKMSIdCommand,

                            DocDate = x.docRetailReturn.doc.DocDate,
                            Held = x.docRetailReturn.doc.Held,
                            Discount = x.docRetailReturn.doc.Discount,
                            DocRetailID = x.docRetailReturn.DocRetailReturnID, //NumberReal = x.docRetailReturn.doc.NumberReal,
                            DocRetailReturnID = x.docRetailReturn.DocRetailReturnID, //NumberReal = x.docRetailReturn.doc.NumberReal,
                            DirWarehouseID = x.docRetailReturn.DirWarehouseID,
                            ListObjectID = x.docRetailReturn.doc.listObject.ListObjectID,
                            ListObjectNameRu = x.docRetailReturn.doc.listObject.ListObjectNameRu,

                            DirNomenID = x.DirNomenID,

                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            RemPartyID = remPartyMinuses.RemPartyID,
                            RemPartyMinusID = x.RemPartyMinusID,
                            Quantity = -x.Quantity,
                            PriceVAT = -x.PriceVAT,

                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.dirCurrency.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.dirCurrency.DirCurrencyRate + ", " + x.dirCurrency.DirCurrencyMultiplicity + ")",

                            DirEmployeeName = x.docRetailReturn.doc.dirEmployee.DirEmployeeName,

                            //RemParty
                            Barcode = "", //x.Barcode,
                            SerialNumber = "", //x.SerialNumber,

                            DirCharColourName = remPartyMinuses.remParty.dirCharColour.DirCharColourName,
                            DirCharMaterialName = remPartyMinuses.remParty.dirCharMaterial.DirCharMaterialName,
                            DirCharNameName = remPartyMinuses.remParty.dirCharName.DirCharNameName,
                            DirCharSeasonName = remPartyMinuses.remParty.dirCharSeason.DirCharSeasonName,
                            DirCharSexName = remPartyMinuses.remParty.dirCharSex.DirCharSexName,
                            DirCharSizeName = remPartyMinuses.remParty.dirCharSize.DirCharSizeName,
                            DirCharStyleName = remPartyMinuses.remParty.dirCharStyle.DirCharStyleName,
                            DirCharTextureName = remPartyMinuses.remParty.dirCharTexture.DirCharTextureName,
                            DirChar =
                                remPartyMinuses.remParty.dirCharColour.DirCharColourName + " " +
                                remPartyMinuses.remParty.dirCharMaterial.DirCharMaterialName + " " +
                                remPartyMinuses.remParty.dirCharName.DirCharNameName + " " +
                                remPartyMinuses.remParty.dirCharSeason.DirCharSeasonName + " " +
                                remPartyMinuses.remParty.dirCharSex.DirCharSexName + " " +
                                remPartyMinuses.remParty.dirCharSize.DirCharSizeName + " " +
                                remPartyMinuses.remParty.dirCharStyle.DirCharStyleName + " " +
                                remPartyMinuses.remParty.dirCharTexture.DirCharTextureName,
                                

                            //Приходная цена
                            //PriceCurrencyPurch = 0.0, //x.remParty.PriceCurrency,

                            
                            DirPaymentTypeID = x.docRetailReturn.doc.DirPaymentTypeID,
                            //Цена в т.в.
                            PriceCurrency = -x.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = x.Quantity * x.PriceCurrency - x.docRetailReturn.doc.Discount == null ? 0
                            : -Math.Round(x.Quantity * x.PriceCurrency - x.docRetailReturn.doc.Discount, sysSetting.FractionalPartInSum),


                            //Причина возврата
                            DirReturnTypeID = x.DirReturnTypeID,
                            DirReturnTypeName = x.dirReturnType.DirReturnTypeName,

                            DirDescriptionID = x.DirDescriptionID,
                            DirDescriptionName = x.dirDescription.DirDescriptionName,
                            
                        }

                        #endregion    
                    );


                #endregion


                #region Сортировка

                query = query.OrderByDescending(x => x.DocDate);

                #endregion



                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocRetailTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocRetailTabs/5
        [ResponseType(typeof(DocRetailTab))]
        public async Task<IHttpActionResult> GetDocRetailTab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocRetailTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocRetailTab(int id, DocRetailTab docRetailTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocRetailTabs
        [ResponseType(typeof(DocRetailTab))]
        public async Task<IHttpActionResult> PostDocRetailTab(DocRetailTab docRetailTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocRetailTabs/5
        [ResponseType(typeof(DocRetailTab))]
        public async Task<IHttpActionResult> DeleteDocRetailTab(int id)
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

        private bool DocRetailTabExists(int id)
        {
            return db.DocRetailTabs.Count(e => e.DocRetailTabID == id) > 0;
        }

        #endregion


        #region SQL

        //Сумма документа
        public string GenerateSQLSUM(Models.Sklad.Sys.SysSetting sysSettings)
        {
            string SQL =
                "SELECT " +
                "[Docs].[DocDate] AS [DocDate], [Docs].[DocDate] AS [DocDate_InWords], " +
                "[DirEmployees].[DirEmployeeName] AS [DirEmployeeName], " +
                "Docs.Discount, " +
                "COUNT(*) CountRecord, " +
                "COUNT(*) CountRecord_NumInWords, " +
                "SUM(DocRetailTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocRetailTabs.Quantity * DocRetailTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocRetailTabs.Quantity * DocRetailTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocRetailTabs.Quantity * (DocRetailTabs.PriceCurrency - (DocRetailTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocRetailTabs.Quantity * (DocRetailTabs.PriceCurrency - (DocRetailTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocRetailTabs.Quantity * DocRetailTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocRetailTabs.Quantity * DocRetailTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocRetails, DocRetailTabs, DirEmployees " +
                "WHERE " +
                "(Docs.DocID=DocRetails.DocID)and" +
                "(DocRetails.DocRetailID=DocRetailTabs.DocRetailID)and(Docs.DocID=@DocID)and" +
                "(Docs.DirEmployeeID=DirEmployees.DirEmployeeID)";

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

                "[DirNomens].[DirNomenName] AS [DirNomenName], " +
                "[DirNomens].[DirNomenName] AS [DirNomenNameRemove], " +
                "[DirNomens].[DirNomenArticle] AS [DirNomenArticle], " +
                "[DirNomens].[DirNomenMinimumBalance] AS [DirNomenMinimumBalance], " +
                "[DirNomens].[DirNomenNameFull] AS [DirNomenNameFull], " +
                "[DirNomens].[DescriptionFull] AS [DescriptionFull], " +
                "[DirNomenGroups].[DirNomenName] AS [DirNomenGroupName], " + //Группа (Sub)

                "[DocRetailTabs].[DocRetailTabID] AS [DocRetailTabID], " +
                "[DocRetailTabs].[DocRetailID] AS [DocRetailID], " +
                "[DocRetailTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocRetailTabs].[Quantity] AS [Quantity], " +
                "[DocRetailTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocRetailTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocRetailTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocRetailTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocRetailTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirPriceTypes].[DirPriceTypeName] AS [DirPriceTypeName], " +

                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocRetailTabs].[DirCurrencyRate] || ', ' || [DocRetailTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
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
                "ROUND([DocRetailTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVAT', " +  //"Цена без НДС" в валюте
                "ROUND(([DocRetailTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100)) * "+ Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATDiscount', " +  //"Цена без НДС" в валюте со Скидкой

                "ROUND((DocRetailTabs.[Quantity] * [DocRetailTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVAT', " + //"Стоимость без НДС" в валюте
                "ROUND(((DocRetailTabs.[Quantity] * [DocRetailTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATDiscount', " + //"Стоимость без НДС" в валюте со Скидкой

                "ROUND([DocRetailTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT', " +  //"Цена с НДС"  в валюте
                "ROUND([DocRetailTabs].[PriceVAT]* " + Discount+ ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount', " +  //"Цена с НДС"  в валюте со Скидкой

                "ROUND([DocRetailTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT_InWords', " +  //"Цена с НДС"  в валюте (словами)
                "ROUND([DocRetailTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount_InWords', " +  //"Цена с НДС"  в валюте (словами) со Скидкой

                "ROUND(DocRetailTabs.[Quantity] * [DocRetailTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") 'SUMPriceVAT', " +  //"Стоимость с НДС" в валюте
                "ROUND(DocRetailTabs.[Quantity] * [DocRetailTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceVATDiscount', " +  //"Стоимость с НДС" в валюте со Скидкой


                //В текущей валюте
                "ROUND([DocRetailTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrency', " +  //"Цена без НДС" в текущей валюте
                "ROUND(([DocRetailTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrencyDiscount', " +  //"Цена без НДС" в текущей валюте со Скидкой

                "ROUND((DocRetailTabs.[Quantity] * [DocRetailTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrency', " + //"Стоимость без НДС" в текущей валюте
                "ROUND(((DocRetailTabs.[Quantity] * [DocRetailTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100))* "+ Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrencyDiscount', " + //"Стоимость без НДС" в текущей валюте со Скидкой

                "ROUND([DocRetailTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency', " +  //"Цена с НДС" в текущей валюте
                "ROUND([DocRetailTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount', " +  //"Цена с НДС" в текущей валюте со Скидкой

                "ROUND([DocRetailTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency_InWords', " +  //"Цена с НДС" в текущей валюте (словами)
                "ROUND([DocRetailTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount_InWords', " +  //"Цена с НДС" в текущей валюте (словами) со Скидкой

                "ROUND([DocRetailTabs].[Quantity] * [DocRetailTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrency', " + //Стоимость с НДС в текущей валюте
                "ROUND([DocRetailTabs].[Quantity] * [DocRetailTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " + //Стоимость с НДС в текущей валюте со Скидкой
                                                                                                                                                                              //Цены и Суммы НДС=================================================================================

                //"Сумма НДС" (НДС документа)
                "ROUND([DocRetailTabs].[Quantity] * ([DocRetailTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                "ROUND(([DocRetailTabs].[Quantity] * ([DocRetailTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'SumVatValueDiscount', " +  //Сумма НДС (НДС документа)



                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocRetails], [DocRetailTabs] " +

                "LEFT OUTER JOIN [RemParties] AS [RemParties] ON [RemParties].[RemPartyID] = [DocRetailTabs].[RemPartyID] " +

                "LEFT OUTER JOIN [DirCharColours] AS [DirCharColours] ON [DirCharColours].[DirCharColourID] = [RemParties].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] AS [DirCharMaterials] ON [DirCharMaterials].[DirCharMaterialID] = [RemParties].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] AS [DirCharNames] ON [DirCharNames].[DirCharNameID] = [RemParties].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] AS [DirCharSeasons] ON [DirCharSeasons].[DirCharSeasonID] = [RemParties].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] AS [DirCharSexes] ON [DirCharSexes].[DirCharSexID] = [RemParties].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] AS [DirCharSizes] ON [DirCharSizes].[DirCharSizeID] = [RemParties].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] AS [DirCharStyles] ON [DirCharStyles].[DirCharStyleID] = [RemParties].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] AS [DirCharTextures] ON [DirCharTextures].[DirCharTextureID] = [RemParties].[DirCharTextureID] " +

                "INNER JOIN [DirNomens] ON [DocRetailTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                "LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocRetailTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirPriceTypes] ON [DocRetailTabs].[DirPriceTypeID] = [DirPriceTypes].[DirPriceTypeID] " +

                "WHERE ([Docs].[DocID]=[DocRetails].[DocID])and([DocRetails].[DocRetailID]=[DocRetailTabs].[DocRetailID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}