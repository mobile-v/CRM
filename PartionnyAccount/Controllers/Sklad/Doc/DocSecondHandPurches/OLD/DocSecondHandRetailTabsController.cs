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
    public class DocSecondHandRetailTabsController : ApiController
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
            public int? DocSecondHandRetailID;
            public DateTime? DateS, DatePo;
            public int DirWarehouseID;
        }
        // GET: api/DocSecondHandRetailTabs
        public async Task<IHttpActionResult> GetDocSecondHandRetailTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetails"));
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
                _params.DocSecondHandRetailID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandRetailID", true) == 0).Value);
                //_params.DocDate = Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDate", true) == 0).Value);

                _params.DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DateS < Convert.ToDateTime("01.01.1800")) _params.DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else _params.DateS = _params.DateS.Value.AddDays(-1);

                _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                int? DocSecondHandRetailID = null, DocSecondHandRetailReturnID = null, Rem2PartyID = null, Rem2PartyMinusID = null, DirReturnTypeID = null, DirDescriptionID = null;
                string DirReturnTypeName = null, DirDescriptionName = null;
                double Quantity = 1;

                var query =
                    (
                        //Розничный Чек

                        from x in db.Rem2PartyMinuses
                        from y in db.DocSecondHandRetailTabs

                        //from docSecondHandPurches in db.DocSecondHandPurches //!!!
                        join docSecondHandPurches in db.DocSecondHandPurches on x.rem2Party.DocIDFirst equals docSecondHandPurches.DocID
                        
                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where
                            x.FieldID == y.DocSecondHandRetailTabID &&
                            x.rem2Party.DocIDFirst == docSecondHandPurches.DocID && //x.rem2Party.DocID == docSecondHandPurches.DocID && //!!!
                            x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo && x.doc.ListObjectID == 66 &&
                            x.DirWarehouseID == _params.DirWarehouseID

                        #region select

                        select new
                        {
                            
                            DocID = x.DocID,
                            DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,
                            
                            KKMSCheckNumber = y.docSecondHandRetail.doc.KKMSCheckNumber,
                            KKMSIdCommand = y.docSecondHandRetail.doc.KKMSIdCommand,
                            
                            DocDate = x.doc.DocDate,
                            Held = x.doc.Held,
                            Discount = x.doc.Discount,
                            DocSecondHandRetailID = y.DocSecondHandRetailID, //NumberReal = x.doc.NumberReal,
                            
                            DocSecondHandRetailReturnID = y.DocSecondHandRetailID, //NumberReal = x.doc.NumberReal,
                            DirWarehouseID = x.DirWarehouseID,
                            ListObjectID = x.doc.listObject.ListObjectID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,

                            DirServiceNomenID = x.DirServiceNomenID,
                            
                            //DirServiceNomenName = x.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName,
                            
                            Rem2PartyID = x.Rem2PartyID,
                            Rem2PartyMinusID = x.Rem2PartyMinusID,
                            Quantity = x.Quantity,
                            PriceVAT = x.PriceVAT,
                            
                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",

                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            
                            //Rem2Party
                            //Barcode = x.rem2Party.Barcode,
                            //SerialNumber = x.rem2Party.SerialNumber,

                            //Приходная цена
                            PriceCurrencyPurch = y.rem2Party.PriceCurrency, //x.remParty.PriceCurrency,
                            
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

                        from x in db.DocSecondHandRetailReturnTabs
                        
                        //Документы и партии для нахождения первичного Номера документа!
                        join rem2PartyMinuses1 in db.Rem2PartyMinuses on x.Rem2PartyMinusID equals rem2PartyMinuses1.Rem2PartyMinusID into rem2PartyMinuses2
                        from rem2PartyMinuses in rem2PartyMinuses2.DefaultIfEmpty()

                        join rem2Parties1 in db.Rem2Parties on rem2PartyMinuses.Rem2PartyID equals rem2Parties1.Rem2PartyID into rem2Parties2
                        from rem2Parties in rem2Parties2.DefaultIfEmpty()

                        join docSecondHandPurches1 in db.DocSecondHandPurches on rem2Parties.DocIDFirst equals docSecondHandPurches1.DocID into docSecondHandPurches2
                        from docSecondHandPurches in docSecondHandPurches2.DefaultIfEmpty()

                        //Наименование Группы товара
                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()


                        where
                        x.docSecondHandRetailReturn.doc.DocDate >= _params.DateS && x.docSecondHandRetailReturn.doc.DocDate <= _params.DatePo &&
                        x.docSecondHandRetailReturn.DirWarehouseID == _params.DirWarehouseID


                        #region select

                        select new
                        {
                            DocID = x.docSecondHandRetailReturn.DocID,
                            DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,
                            
                            KKMSCheckNumber = x.docSecondHandRetailReturn.doc.KKMSCheckNumber,
                            KKMSIdCommand = x.docSecondHandRetailReturn.doc.KKMSIdCommand,
                            
                            DocDate = x.docSecondHandRetailReturn.doc.DocDate,
                            Held = x.docSecondHandRetailReturn.doc.Held,
                            Discount = x.docSecondHandRetailReturn.doc.Discount,
                            DocSecondHandRetailID = x.docSecondHandRetailReturn.DocSecondHandRetailReturnID, //NumberReal = x.docSecondHandRetailReturn.doc.NumberReal,
                            
                            DocSecondHandRetailReturnID = x.docSecondHandRetailReturn.DocSecondHandRetailReturnID, //NumberReal = x.docSecondHandRetailReturn.doc.NumberReal,
                            DirWarehouseID = x.docSecondHandRetailReturn.DirWarehouseID,
                            ListObjectID = x.docSecondHandRetailReturn.doc.listObject.ListObjectID,
                            ListObjectNameRu = x.docSecondHandRetailReturn.doc.listObject.ListObjectNameRu,

                            DirServiceNomenID = x.DirServiceNomenID,
                            
                            //DirServiceNomenName = x.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName,
                            
                            Rem2PartyID = rem2PartyMinuses.Rem2PartyID,
                            Rem2PartyMinusID = x.Rem2PartyMinusID,
                            Quantity = -x.Quantity,
                            PriceVAT = -x.PriceVAT,
                            
                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.dirCurrency.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.dirCurrency.DirCurrencyRate + ", " + x.dirCurrency.DirCurrencyMultiplicity + ")",

                            DirEmployeeName = x.docSecondHandRetailReturn.doc.dirEmployee.DirEmployeeName,
                            
                            //Rem2Party
                            //Barcode = "", //x.Barcode,
                            //SerialNumber = "", //x.SerialNumber,

                            //Приходная цена
                            PriceCurrencyPurch = 0.0, //x.remParty.PriceCurrency,
                            
                            //Цена в т.в.
                            PriceCurrency = -x.PriceCurrency,
                            //Себестоимость
                            SUMSalePriceVATCurrency = x.Quantity * x.PriceCurrency - x.docSecondHandRetailReturn.doc.Discount == null ? 0
                            : -Math.Round(x.Quantity * x.PriceCurrency - x.docSecondHandRetailReturn.doc.Discount, sysSetting.FractionalPartInSum),

                            
                            //Причина возврата
                            DirReturnTypeID = x.DirReturnTypeID,
                            DirReturnTypeName = x.dirReturnType.DirReturnTypeName,

                            DirDescriptionID = x.DirDescriptionID,
                            DirDescriptionName = x.dirDescription.DirDescriptionName,
                            
                        }

                        #endregion

                    ).Union
                    (
                        from x in db.DocSecondHandPurches

                        join dirServiceNomens11 in db.DirServiceNomens on x.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where
                        x.doc.DocDate >= _params.DateS && x.doc.DocDate <= _params.DatePo &&
                        x.DirWarehouseID == _params.DirWarehouseID

                        #region select

                        select new
                        {
                            DocID = x.DocID,
                            DocSecondHandPurchID = x.DocSecondHandPurchID,
                            
                            KKMSCheckNumber = x.doc.KKMSCheckNumber,
                            KKMSIdCommand = x.doc.KKMSIdCommand,
                            
                            DocDate = x.doc.DocDate,
                            Held = x.doc.Held,
                            Discount = x.doc.Discount,
                            DocSecondHandRetailID = DocSecondHandRetailID,
                            
                            DocSecondHandRetailReturnID = DocSecondHandRetailReturnID, //NumberReal = x.doc.NumberReal,
                            DirWarehouseID = x.DirWarehouseID,
                            ListObjectID = x.doc.listObject.ListObjectID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,

                            DirServiceNomenID = x.DirServiceNomenID,
                            
                            //DirServiceNomenName = x.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + x.dirServiceNomen.DirServiceNomenName,
                            
                            Rem2PartyID = Rem2PartyID,
                            Rem2PartyMinusID = Rem2PartyMinusID,
                            Quantity = Quantity,
                            PriceVAT = -x.PriceVAT,
                            
                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName + " (" + x.DirCurrencyRate + ", " + x.DirCurrencyMultiplicity + ")",

                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            
                            //Rem2Party
                            //Barcode = 0,
                            //SerialNumber = 0,

                            //Приходная цена
                            PriceCurrencyPurch = -x.PriceVAT, //x.remParty.PriceCurrency,
                            
                            //Цена в т.в.
                            PriceCurrency = -x.PriceVAT,
                            //Себестоимость
                            SUMSalePriceVATCurrency = -x.PriceVAT,

                            
                            //Причина возврата
                            DirReturnTypeID = DirReturnTypeID,
                            DirReturnTypeName = DirReturnTypeName,

                            DirDescriptionID = DirDescriptionID,
                            DirDescriptionName = DirDescriptionName,
                            
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
                    DocSecondHandRetailTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandRetailTabs/5
        [ResponseType(typeof(DocSecondHandRetailTab))]
        public async Task<IHttpActionResult> GetDocSecondHandRetailTab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandRetailTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRetailTab(int id, DocSecondHandRetailTab docSecondHandRetailTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandRetailTabs
        [ResponseType(typeof(DocSecondHandRetailTab))]
        public async Task<IHttpActionResult> PostDocSecondHandRetailTab(DocSecondHandRetailTab docSecondHandRetailTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSecondHandRetailTabs/5
        [ResponseType(typeof(DocSecondHandRetailTab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRetailTab(int id)
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

        private bool DocSecondHandRetailTabExists(int id)
        {
            return db.DocSecondHandRetailTabs.Count(e => e.DocSecondHandRetailTabID == id) > 0;
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
                "SUM(DocSecondHandRetailTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocSecondHandRetailTabs.Quantity * DocSecondHandRetailTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocSecondHandRetailTabs.Quantity * DocSecondHandRetailTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocSecondHandRetailTabs.Quantity * (DocSecondHandRetailTabs.PriceCurrency - (DocSecondHandRetailTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocSecondHandRetailTabs.Quantity * (DocSecondHandRetailTabs.PriceCurrency - (DocSecondHandRetailTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocSecondHandRetailTabs.Quantity * DocSecondHandRetailTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocSecondHandRetailTabs.Quantity * DocSecondHandRetailTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocSecondHandRetails, DocSecondHandRetailTabs, DirEmployees " +
                "WHERE " +
                "(Docs.DocID=DocSecondHandRetails.DocID)and" +
                "(DocSecondHandRetails.DocSecondHandRetailID=DocSecondHandRetailTabs.DocSecondHandRetailID)and(Docs.DocID=@DocID)and" +
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

                "[DirServiceNomens].[DirServiceNomenName] AS [DirServiceNomenName], " +
                "[DirServiceNomens].[DirServiceNomenName] AS [DirServiceNomenNameRemove], " +
                //"[DirServiceNomens].[DirServiceNomenArticle] AS [DirServiceNomenArticle], " +
                //"[DirServiceNomens].[DirServiceNomenMinimumBalance] AS [DirServiceNomenMinimumBalance], " +
                "[DirServiceNomens].[DirServiceNomenNameFull] AS [DirServiceNomenNameFull], " +
                "[DirServiceNomens].[DescriptionFull] AS [DescriptionFull], " +
                "[DirServiceNomenGroups].[DirServiceNomenName] AS [DirServiceNomenGroupName], " + //Группа (Sub)

                "[DocSecondHandRetailTabs].[DocSecondHandRetailTabID] AS [DocSecondHandRetailTabID], " +
                "[DocSecondHandRetailTabs].[DocSecondHandRetailID] AS [DocSecondHandRetailID], " +
                "[DocSecondHandRetailTabs].[DirServiceNomenID] AS [DirServiceNomenID], " +
                "[DocSecondHandRetailTabs].[Quantity] AS [Quantity], " +
                "[DocSecondHandRetailTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocSecondHandRetailTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandRetailTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocSecondHandRetailTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandRetailTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirPriceTypes].[DirPriceTypeName] AS [DirPriceTypeName], " +

                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandRetailTabs].[DirCurrencyRate] || ', ' || [DocSecondHandRetailTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +


                "[Rem2Parties].Barcode AS Barcode, " +
                "[Rem2Parties].SerialNumber AS SerialNumber, " +

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
                "ROUND([DocSecondHandRetailTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVAT', " +  //"Цена без НДС" в валюте
                "ROUND(([DocSecondHandRetailTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATDiscount', " +  //"Цена без НДС" в валюте со Скидкой

                "ROUND((DocSecondHandRetailTabs.[Quantity] * [DocSecondHandRetailTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVAT', " + //"Стоимость без НДС" в валюте
                "ROUND(((DocSecondHandRetailTabs.[Quantity] * [DocSecondHandRetailTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATDiscount', " + //"Стоимость без НДС" в валюте со Скидкой

                "ROUND([DocSecondHandRetailTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT', " +  //"Цена с НДС"  в валюте
                "ROUND([DocSecondHandRetailTabs].[PriceVAT]* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount', " +  //"Цена с НДС"  в валюте со Скидкой

                "ROUND([DocSecondHandRetailTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT_InWords', " +  //"Цена с НДС"  в валюте (словами)
                "ROUND([DocSecondHandRetailTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount_InWords', " +  //"Цена с НДС"  в валюте (словами) со Скидкой

                "ROUND(DocSecondHandRetailTabs.[Quantity] * [DocSecondHandRetailTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") 'SUMPriceVAT', " +  //"Стоимость с НДС" в валюте
                "ROUND(DocSecondHandRetailTabs.[Quantity] * [DocSecondHandRetailTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceVATDiscount', " +  //"Стоимость с НДС" в валюте со Скидкой


                //В текущей валюте
                "ROUND([DocSecondHandRetailTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrency', " +  //"Цена без НДС" в текущей валюте
                "ROUND(([DocSecondHandRetailTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrencyDiscount', " +  //"Цена без НДС" в текущей валюте со Скидкой

                "ROUND((DocSecondHandRetailTabs.[Quantity] * [DocSecondHandRetailTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrency', " + //"Стоимость без НДС" в текущей валюте
                "ROUND(((DocSecondHandRetailTabs.[Quantity] * [DocSecondHandRetailTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrencyDiscount', " + //"Стоимость без НДС" в текущей валюте со Скидкой

                "ROUND([DocSecondHandRetailTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency', " +  //"Цена с НДС" в текущей валюте
                "ROUND([DocSecondHandRetailTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount', " +  //"Цена с НДС" в текущей валюте со Скидкой

                "ROUND([DocSecondHandRetailTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency_InWords', " +  //"Цена с НДС" в текущей валюте (словами)
                "ROUND([DocSecondHandRetailTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount_InWords', " +  //"Цена с НДС" в текущей валюте (словами) со Скидкой

                "ROUND([DocSecondHandRetailTabs].[Quantity] * [DocSecondHandRetailTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrency', " + //Стоимость с НДС в текущей валюте
                "ROUND([DocSecondHandRetailTabs].[Quantity] * [DocSecondHandRetailTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " + //Стоимость с НДС в текущей валюте со Скидкой
                                                                                                                                                                                 //Цены и Суммы НДС=================================================================================

                //"Сумма НДС" (НДС документа)
                "ROUND([DocSecondHandRetailTabs].[Quantity] * ([DocSecondHandRetailTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                "ROUND(([DocSecondHandRetailTabs].[Quantity] * ([DocSecondHandRetailTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'SumVatValueDiscount', " +  //Сумма НДС (НДС документа)



                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocSecondHandRetails], [DocSecondHandRetailTabs] " +

                "LEFT OUTER JOIN [Rem2Parties] AS [Rem2Parties] ON [Rem2Parties].[Rem2PartyID] = [DocSecondHandRetailTabs].[Rem2PartyID] " +

                "LEFT OUTER JOIN [DirCharColours] AS [DirCharColours] ON [DirCharColours].[DirCharColourID] = [Rem2Parties].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] AS [DirCharMaterials] ON [DirCharMaterials].[DirCharMaterialID] = [Rem2Parties].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] AS [DirCharNames] ON [DirCharNames].[DirCharNameID] = [Rem2Parties].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] AS [DirCharSeasons] ON [DirCharSeasons].[DirCharSeasonID] = [Rem2Parties].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] AS [DirCharSexes] ON [DirCharSexes].[DirCharSexID] = [Rem2Parties].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] AS [DirCharSizes] ON [DirCharSizes].[DirCharSizeID] = [Rem2Parties].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] AS [DirCharStyles] ON [DirCharStyles].[DirCharStyleID] = [Rem2Parties].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] AS [DirCharTextures] ON [DirCharTextures].[DirCharTextureID] = [Rem2Parties].[DirCharTextureID] " +

                "INNER JOIN [DirServiceNomens] ON [DocSecondHandRetailTabs].[DirServiceNomenID] = [DirServiceNomens].[DirServiceNomenID] " +
                "LEFT OUTER JOIN [DirServiceNomens] AS [DirServiceNomenGroups] ON [DirServiceNomenGroups].[Sub] = [DirServiceNomens].[DirServiceNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocSecondHandRetailTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirPriceTypes] ON [DocSecondHandRetailTabs].[DirPriceTypeID] = [DirPriceTypes].[DirPriceTypeID] " +

                "WHERE ([Docs].[DocID]=[DocSecondHandRetails].[DocID])and([DocSecondHandRetails].[DocSecondHandRetailID]=[DocSecondHandRetailTabs].[DocSecondHandRetailID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion

    }
}