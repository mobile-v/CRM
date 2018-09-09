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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocActWriteOffs
{
    public class DocActWriteOffTabsController : ApiController
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
            public int DocActWriteOffID;
            public int DocID;
        }
        // GET: api/DocActWriteOffTabs
        public async Task<IHttpActionResult> GetDocActWriteOffTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocActWriteOffs"));
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
                _params.DocActWriteOffID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocActWriteOffID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docActWriteOffTabs in db.DocActWriteOffTabs

                        join dirNomens11 in db.DirNomens on docActWriteOffTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики
                        join dirCharColours1 in db.DirCharColours on docActWriteOffTabs.remParty.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on docActWriteOffTabs.remParty.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on docActWriteOffTabs.remParty.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on docActWriteOffTabs.remParty.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on docActWriteOffTabs.remParty.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on docActWriteOffTabs.remParty.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on docActWriteOffTabs.remParty.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on docActWriteOffTabs.remParty.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()

                        where docActWriteOffTabs.DocActWriteOffID == _params.DocActWriteOffID
                        #endregion


                        #region select

                        select new
                        {
                            DocActWriteOffTabID = docActWriteOffTabs.DocActWriteOffTabID,
                            DocActWriteOffID = docActWriteOffTabs.DocActWriteOffID,
                            DirNomenID = docActWriteOffTabs.DirNomenID,

                            //DirNomenName = docActWriteOffTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docActWriteOffTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docActWriteOffTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docActWriteOffTabs.dirNomen.DirNomenName,

                            RemPartyID = docActWriteOffTabs.RemPartyID,
                            Quantity = docActWriteOffTabs.Quantity,

                            PriceVAT = docActWriteOffTabs.PriceVAT,

                            DirCurrencyID = docActWriteOffTabs.DirCurrencyID,
                            DirCurrencyRate = docActWriteOffTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docActWriteOffTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docActWriteOffTabs.dirCurrency.DirCurrencyName + " (" + docActWriteOffTabs.DirCurrencyRate + ", " + docActWriteOffTabs.DirCurrencyMultiplicity + ")",

                            //RemParty
                            Barcode = docActWriteOffTabs.remParty.Barcode,
                            SerialNumber = docActWriteOffTabs.remParty.SerialNumber,

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
                            PriceCurrency = docActWriteOffTabs.PriceCurrency,
                            //Себестоимость
                            SUMActWriteOffPriceVATCurrency = docActWriteOffTabs.Quantity * docActWriteOffTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docActWriteOffTabs.Quantity * docActWriteOffTabs.PriceCurrency, sysSetting.FractionalPartInSum)
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocActWriteOffTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocActWriteOffTabs/5
        [ResponseType(typeof(DocActWriteOffTab))]
        public async Task<IHttpActionResult> GetDocActWriteOffTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocActWriteOffTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocActWriteOffTab(int id, DocActWriteOffTab docActWriteOffTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocActWriteOffTabs
        [ResponseType(typeof(DocActWriteOffTab))]
        public async Task<IHttpActionResult> PostDocActWriteOffTab(DocActWriteOffTab docActWriteOffTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocActWriteOffTabs/5
        [ResponseType(typeof(DocActWriteOffTab))]
        public async Task<IHttpActionResult> DeleteDocActWriteOffTab(int id)
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

        private bool DocActWriteOffTabExists(int id)
        {
            return db.DocActWriteOffTabs.Count(e => e.DocActWriteOffTabID == id) > 0;
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
                "SUM(DocActWriteOffTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocActWriteOffTabs.Quantity * DocActWriteOffTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocActWriteOffTabs.Quantity * DocActWriteOffTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords' " + //Приписью

                "FROM Docs, DocActWriteOffs, DocActWriteOffTabs " +
                "WHERE " +
                "(Docs.DocID=DocActWriteOffs.DocID)and" +
                "(DocActWriteOffs.DocActWriteOffID=DocActWriteOffTabs.DocActWriteOffID)and(Docs.DocID=@DocID)";

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

                "[DocActWriteOffTabs].[DocActWriteOffTabID] AS [DocActWriteOffTabID], " +
                "[DocActWriteOffTabs].[DocActWriteOffID] AS [DocActWriteOffID], " +
                "[DocActWriteOffTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocActWriteOffTabs].[Quantity] AS [Quantity], " +
                "[DocActWriteOffTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocActWriteOffTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocActWriteOffTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocActWriteOffTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocActWriteOffTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +

                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocActWriteOffTabs].[DirCurrencyRate] || ', ' || [DocActWriteOffTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                "[RemParties].Barcode AS Barcode, " +
                "[RemParties].SerialNumber AS SerialNumber, " +

                //Приходная цена
                "[DocActWriteOffTabs].[PriceCurrency] AS [PriceCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocActWriteOffTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocActWriteOffTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +

                //Себестоимось прихода
                "ROUND([DocActWriteOffTabs].[Quantity] * [DocActWriteOffTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocActWriteOffTabs].[Quantity] * [DocActWriteOffTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SumPurchPriceCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocActWriteOffs], [DocActWriteOffTabs] " +

                "LEFT OUTER JOIN [RemParties] AS [RemParties] ON [RemParties].[RemPartyID] = [DocActWriteOffTabs].[RemPartyID] " +

                //"INNER JOIN [DirNomens] ON [DocActWriteOffTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                //"LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocActWriteOffTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocActWriteOffTabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocActWriteOffs].[DocID])and([DocActWriteOffs].[DocActWriteOffID]=[DocActWriteOffTabs].[DocActWriteOffID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}