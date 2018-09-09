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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocDomesticExpenses
{
    public class DocDomesticExpenseTabsController : ApiController
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
            public int? DocDomesticExpenseID;
            public DateTime? DateS, DatePo;
            public int DirWarehouseID;
            public int UO_GridIndex;
        }
        // GET: api/DocDomesticExpenseTabs
        public async Task<IHttpActionResult> GetDocDomesticExpenseTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocDomesticExpenses"));
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
                _params.DocDomesticExpenseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDomesticExpenseID", true) == 0).Value);
                //_params.DocDate = Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDate", true) == 0).Value);

                _params.DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DateS < Convert.ToDateTime("01.01.1800")) _params.DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else _params.DateS = _params.DateS.Value.AddDays(-1);

                _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);
                _params.UO_GridIndex = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_GridIndex", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from y in db.DocDomesticExpenseTabs

                        where
                        //y.FieldID == y.DocRetailTabID &&
                        y.docDomesticExpense.doc.DocDate >= _params.DateS && y.docDomesticExpense.doc.DocDate <= _params.DatePo && //y.docDomesticExpense.doc.ListObjectID == 56 &&
                        y.docDomesticExpense.DirWarehouseID == _params.DirWarehouseID

                        #region select

                        select new
                        {
                            DocID = y.docDomesticExpense.DocID,
                            DirPaymentTypeName = y.docDomesticExpense.doc.dirPaymentType.DirPaymentTypeName,

                            DocDate = y.docDomesticExpense.doc.DocDate,
                            Held = y.docDomesticExpense.doc.Held,
                            Discount = y.docDomesticExpense.doc.Discount,
                            DocDomesticExpenseID = y.DocDomesticExpenseID,
                            DocDomesticExpenseReturnID = y.DocDomesticExpenseID,
                            DirWarehouseID = y.docDomesticExpense.DirWarehouseID,
                            ListObjectID = y.docDomesticExpense.doc.listObject.ListObjectID,
                            ListObjectNameRu = y.docDomesticExpense.doc.listObject.ListObjectNameRu,

                            DirDomesticExpenseID = y.DirDomesticExpenseID,
                            DirDomesticExpenseName = y.dirDomesticExpense.DirDomesticExpenseName,

                            PriceVAT = y.PriceVAT,

                            DirCurrencyID = y.DirCurrencyID,
                            DirCurrencyRate = y.DirCurrencyRate,
                            DirCurrencyMultiplicity = y.DirCurrencyMultiplicity,
                            DirCurrencyName = y.dirCurrency.DirCurrencyName + " (" + y.DirCurrencyRate + ", " + y.DirCurrencyMultiplicity + ")",

                            //Цена в т.в.
                            PriceCurrency = y.PriceCurrency,

                            DirEmployeeName = y.docDomesticExpense.doc.dirEmployee.DirEmployeeName,
                            DirEmployeeNameSpisat = y.docDomesticExpense.dirEmployeeSpisat.DirEmployeeName,
                        }

                        #endregion

                    );

                if (_params.UO_GridIndex == 2)
                {
                    query = query.Where(x => x.DirDomesticExpenseID == 3);
                }
                else
                {
                    query = query.Where(x => x.DirDomesticExpenseID != 3);
                }


                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocDomesticExpenseTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocDomesticExpenseTabs/5
        [ResponseType(typeof(DocDomesticExpenseTab))]
        public async Task<IHttpActionResult> GetDocDomesticExpenseTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocDomesticExpenseTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocDomesticExpenseTab(int id, DocDomesticExpenseTab docDomesticExpenseTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocDomesticExpenseTabs
        [ResponseType(typeof(DocDomesticExpenseTab))]
        public async Task<IHttpActionResult> PostDocDomesticExpenseTab(DocDomesticExpenseTab docDomesticExpenseTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocDomesticExpenseTabs/5
        [ResponseType(typeof(DocDomesticExpenseTab))]
        public async Task<IHttpActionResult> DeleteDocDomesticExpenseTab(int id)
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

        private bool DocDomesticExpenseTabExists(int id)
        {
            return db.DocDomesticExpenseTabs.Count(e => e.DocDomesticExpenseTabID == id) > 0;
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
                "SUM(DocDomesticExpenseTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocDomesticExpenseTabs.Quantity * DocDomesticExpenseTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocDomesticExpenseTabs.Quantity * DocDomesticExpenseTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocDomesticExpenseTabs.Quantity * (DocDomesticExpenseTabs.PriceCurrency - (DocDomesticExpenseTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocDomesticExpenseTabs.Quantity * (DocDomesticExpenseTabs.PriceCurrency - (DocDomesticExpenseTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocDomesticExpenseTabs.Quantity * DocDomesticExpenseTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocDomesticExpenseTabs.Quantity * DocDomesticExpenseTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocDomesticExpenses, DocDomesticExpenseTabs, DirEmployees " +
                "WHERE " +
                "(Docs.DocID=DocDomesticExpenses.DocID)and" +
                "(DocDomesticExpenses.DocDomesticExpenseID=DocDomesticExpenseTabs.DocDomesticExpenseID)and(Docs.DocID=@DocID)and" +
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

                "[DocDomesticExpenseTabs].[DocDomesticExpenseTabID] AS [DocDomesticExpenseTabID], " +
                "[DocDomesticExpenseTabs].[DocDomesticExpenseID] AS [DocDomesticExpenseID], " +
                "[DocDomesticExpenseTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocDomesticExpenseTabs].[Quantity] AS [Quantity], " +
                "[DocDomesticExpenseTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocDomesticExpenseTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocDomesticExpenseTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocDomesticExpenseTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocDomesticExpenseTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirPriceTypes].[DirPriceTypeName] AS [DirPriceTypeName], " +

                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocDomesticExpenseTabs].[DirCurrencyRate] || ', ' || [DocDomesticExpenseTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
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
                "ROUND([DocDomesticExpenseTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVAT', " +  //"Цена без НДС" в валюте
                "ROUND(([DocDomesticExpenseTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATDiscount', " +  //"Цена без НДС" в валюте со Скидкой

                "ROUND((DocDomesticExpenseTabs.[Quantity] * [DocDomesticExpenseTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVAT', " + //"Стоимость без НДС" в валюте
                "ROUND(((DocDomesticExpenseTabs.[Quantity] * [DocDomesticExpenseTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATDiscount', " + //"Стоимость без НДС" в валюте со Скидкой

                "ROUND([DocDomesticExpenseTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT', " +  //"Цена с НДС"  в валюте
                "ROUND([DocDomesticExpenseTabs].[PriceVAT]* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount', " +  //"Цена с НДС"  в валюте со Скидкой

                "ROUND([DocDomesticExpenseTabs].[PriceVAT], " + sysSettings.FractionalPartInPrice + ") 'PriceVAT_InWords', " +  //"Цена с НДС"  в валюте (словами)
                "ROUND([DocDomesticExpenseTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceVATDiscount_InWords', " +  //"Цена с НДС"  в валюте (словами) со Скидкой

                "ROUND(DocDomesticExpenseTabs.[Quantity] * [DocDomesticExpenseTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") 'SUMPriceVAT', " +  //"Стоимость с НДС" в валюте
                "ROUND(DocDomesticExpenseTabs.[Quantity] * [DocDomesticExpenseTabs].[PriceVAT] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceVATDiscount', " +  //"Стоимость с НДС" в валюте со Скидкой


                //В текущей валюте
                "ROUND([DocDomesticExpenseTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrency', " +  //"Цена без НДС" в текущей валюте
                "ROUND(([DocDomesticExpenseTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceNoVATCurrencyDiscount', " +  //"Цена без НДС" в текущей валюте со Скидкой

                "ROUND((DocDomesticExpenseTabs.[Quantity] * [DocDomesticExpenseTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrency', " + //"Стоимость без НДС" в текущей валюте
                "ROUND(((DocDomesticExpenseTabs.[Quantity] * [DocDomesticExpenseTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100))* " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceNoVATCurrencyDiscount', " + //"Стоимость без НДС" в текущей валюте со Скидкой

                "ROUND([DocDomesticExpenseTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency', " +  //"Цена с НДС" в текущей валюте
                "ROUND([DocDomesticExpenseTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount', " +  //"Цена с НДС" в текущей валюте со Скидкой

                "ROUND([DocDomesticExpenseTabs].[PriceCurrency], " + sysSettings.FractionalPartInPrice + ") 'PriceCurrency_InWords', " +  //"Цена с НДС" в текущей валюте (словами)
                "ROUND([DocDomesticExpenseTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'PriceCurrencyDiscount_InWords', " +  //"Цена с НДС" в текущей валюте (словами) со Скидкой

                "ROUND([DocDomesticExpenseTabs].[Quantity] * [DocDomesticExpenseTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrency', " + //Стоимость с НДС в текущей валюте
                "ROUND([DocDomesticExpenseTabs].[Quantity] * [DocDomesticExpenseTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " + //Стоимость с НДС в текущей валюте со Скидкой
                                                                                                                                                                                 //Цены и Суммы НДС=================================================================================

                //"Сумма НДС" (НДС документа)
                "ROUND([DocDomesticExpenseTabs].[Quantity] * ([DocDomesticExpenseTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                "ROUND(([DocDomesticExpenseTabs].[Quantity] * ([DocDomesticExpenseTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100)) * " + Discount + ", " + sysSettings.FractionalPartInPrice + ") 'SumVatValueDiscount', " +  //Сумма НДС (НДС документа)



                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +


                "FROM [Docs], [DocDomesticExpenses], [DocDomesticExpenseTabs] " +

                "LEFT OUTER JOIN [RemParties] AS [RemParties] ON [RemParties].[RemPartyID] = [DocDomesticExpenseTabs].[RemPartyID] " +

                "LEFT OUTER JOIN [DirCharColours] AS [DirCharColours] ON [DirCharColours].[DirCharColourID] = [RemParties].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] AS [DirCharMaterials] ON [DirCharMaterials].[DirCharMaterialID] = [RemParties].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] AS [DirCharNames] ON [DirCharNames].[DirCharNameID] = [RemParties].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] AS [DirCharSeasons] ON [DirCharSeasons].[DirCharSeasonID] = [RemParties].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] AS [DirCharSexes] ON [DirCharSexes].[DirCharSexID] = [RemParties].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] AS [DirCharSizes] ON [DirCharSizes].[DirCharSizeID] = [RemParties].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] AS [DirCharStyles] ON [DirCharStyles].[DirCharStyleID] = [RemParties].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] AS [DirCharTextures] ON [DirCharTextures].[DirCharTextureID] = [RemParties].[DirCharTextureID] " +

                "INNER JOIN [DirNomens] ON [DocDomesticExpenseTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                "LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocDomesticExpenseTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +
                "INNER JOIN [DirPriceTypes] ON [DocDomesticExpenseTabs].[DirPriceTypeID] = [DirPriceTypes].[DirPriceTypeID] " +

                "WHERE ([Docs].[DocID]=[DocDomesticExpenses].[DocID])and([DocDomesticExpenses].[DocDomesticExpenseID]=[DocDomesticExpenseTabs].[DocDomesticExpenseID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}