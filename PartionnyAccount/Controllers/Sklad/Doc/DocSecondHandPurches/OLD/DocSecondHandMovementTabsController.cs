﻿using System;
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
    public class DocSecondHandMovementTabsController : ApiController
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
            public int DocSecondHandMovementID;
            public int DocID;
        }
        // GET: api/DocSecondHandMovementTabs
        public async Task<IHttpActionResult> GetDocSecondHandMovementTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovements"));
                if (iRight == 3)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandMovementsLogistics"));
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
                _params.DocSecondHandMovementID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandMovementID", true) == 0).Value);

                #endregion



                #region APPLY JOINS ARE NOT SUPPORTED

                var query1 =
                    (
                        from docMovementTabs in db.DocSecondHandMovementTabs

                            //APPLY JOINS ARE NOT SUPPORTED
                        from docSecondHandPurches in db.DocSecondHandPurches //!!!
                               
                        where
                        docMovementTabs.rem2Party.DocIDFirst == docSecondHandPurches.DocID && //docMovementTabs.rem2Party.DocID == docSecondHandPurches.DocID && //!!!
                        docMovementTabs.DocSecondHandMovementID == _params.DocSecondHandMovementID

                        #region select

                        select new
                        {
                            DocSecondHandMovementTabID = docMovementTabs.DocSecondHandMovementTabID,
                            DocSecondHandMovementID = docMovementTabs.DocSecondHandMovementID,
                            DirServiceNomenID = docMovementTabs.DirServiceNomenID,

                            DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,
                            
                        }

                        #endregion
                    );


                var query2 =
                    (
                        from docMovementTabs in db.DocSecondHandMovementTabs

                            //APPLY JOINS ARE NOT SUPPORTED
                        //from docSecondHandPurches in db.DocSecondHandPurches //!!!

                        where
                        //docMovementTabs.rem2Party.DocID == docSecondHandPurches.DocID && //!!!
                        docMovementTabs.DocSecondHandMovementID == _params.DocSecondHandMovementID

                        #region select

                        select new
                        {
                            DocSecondHandMovementTabID = docMovementTabs.DocSecondHandMovementTabID,
                            DocSecondHandMovementID = docMovementTabs.DocSecondHandMovementID,
                            DirServiceNomenID = docMovementTabs.DirServiceNomenID,

                            //DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,

                        }

                        #endregion
                    );


                if (await query1.CountAsync() == await query2.CountAsync())
                {

                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from docMovementTabs in db.DocSecondHandMovementTabs

                            //APPLY JOINS ARE NOT SUPPORTED
                            from docSecondHandPurches in db.DocSecondHandPurches //!!!
                            //join docSecondHandPurches11 in db.DocSecondHandPurches on docMovementTabs.rem2Party.DocID equals docSecondHandPurches11.DocID into docSecondHandPurches12
                            //from docSecondHandPurches in docSecondHandPurches12.DefaultIfEmpty()

                            join dirServiceNomens11 in db.DirServiceNomens on docMovementTabs.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                            from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                            join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                            from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                            where
                            docMovementTabs.rem2Party.DocIDFirst == docSecondHandPurches.DocID && //docMovementTabs.rem2Party.DocID == docSecondHandPurches.DocID && //!!!
                            docMovementTabs.DocSecondHandMovementID == _params.DocSecondHandMovementID

                            #region select

                            select new
                            {
                                DocSecondHandMovementTabID = docMovementTabs.DocSecondHandMovementTabID,
                                DocSecondHandMovementID = docMovementTabs.DocSecondHandMovementID,
                                DirServiceNomenID = docMovementTabs.DirServiceNomenID,

                                DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,

                                //DirServiceNomenName = docMovementTabs.dirServiceNomen.DirServiceNomenName,
                                DirServiceNomenName =
                                    dirServiceNomensSubGroup.DirServiceNomenName == null ? docMovementTabs.dirServiceNomen.DirServiceNomenName :
                                    dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docMovementTabs.dirServiceNomen.DirServiceNomenName :
                                    dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docMovementTabs.dirServiceNomen.DirServiceNomenName,

                                Rem2PartyID = docMovementTabs.Rem2PartyID,
                                Quantity = docMovementTabs.Quantity,

                                PriceVAT = docMovementTabs.PriceVAT,

                                DirCurrencyID = docMovementTabs.DirCurrencyID,
                                DirCurrencyRate = docMovementTabs.DirCurrencyRate,
                                DirCurrencyMultiplicity = docMovementTabs.DirCurrencyMultiplicity,
                                DirCurrencyName = docMovementTabs.dirCurrency.DirCurrencyName + " (" + docMovementTabs.DirCurrencyRate + ", " + docMovementTabs.DirCurrencyMultiplicity + ")",


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
                        DocSecondHandMovementTab = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion

                }
                else
                {

                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from docMovementTabs in db.DocSecondHandMovementTabs

                            //APPLY JOINS ARE NOT SUPPORTED
                            //from docSecondHandPurches in db.DocSecondHandPurches //!!!
                            //join docSecondHandPurches11 in db.DocSecondHandPurches on docMovementTabs.rem2Party.DocID equals docSecondHandPurches11.DocID into docSecondHandPurches12
                            //from docSecondHandPurches in docSecondHandPurches12.DefaultIfEmpty()

                            join dirServiceNomens11 in db.DirServiceNomens on docMovementTabs.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                            from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                            join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                            from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                            where
                            //docMovementTabs.rem2Party.DocID == docSecondHandPurches.DocID && //!!!
                            docMovementTabs.DocSecondHandMovementID == _params.DocSecondHandMovementID

                            #region select

                            select new
                            {
                                DocSecondHandMovementTabID = docMovementTabs.DocSecondHandMovementTabID,
                                DocSecondHandMovementID = docMovementTabs.DocSecondHandMovementID,
                                DirServiceNomenID = docMovementTabs.DirServiceNomenID,

                                //DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,

                                //DirServiceNomenName = docMovementTabs.dirServiceNomen.DirServiceNomenName,
                                DirServiceNomenName =
                                    dirServiceNomensSubGroup.DirServiceNomenName == null ? docMovementTabs.dirServiceNomen.DirServiceNomenName :
                                    dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docMovementTabs.dirServiceNomen.DirServiceNomenName :
                                    dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docMovementTabs.dirServiceNomen.DirServiceNomenName,

                                Rem2PartyID = docMovementTabs.Rem2PartyID,
                                Quantity = docMovementTabs.Quantity,

                                PriceVAT = docMovementTabs.PriceVAT,

                                DirCurrencyID = docMovementTabs.DirCurrencyID,
                                DirCurrencyRate = docMovementTabs.DirCurrencyRate,
                                DirCurrencyMultiplicity = docMovementTabs.DirCurrencyMultiplicity,
                                DirCurrencyName = docMovementTabs.dirCurrency.DirCurrencyName + " (" + docMovementTabs.DirCurrencyRate + ", " + docMovementTabs.DirCurrencyMultiplicity + ")",


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
                        DocSecondHandMovementTab = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion

                }


                #endregion 


            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandMovementTabs/5
        [ResponseType(typeof(DocSecondHandMovementTab))]
        public async Task<IHttpActionResult> GetDocSecondHandMovementTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandMovementTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandMovementTab(int id, DocSecondHandMovementTab docSecondHandMovementTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandMovementTabs
        [ResponseType(typeof(DocSecondHandMovementTab))]
        public async Task<IHttpActionResult> PostDocSecondHandMovementTab(DocSecondHandMovementTab docSecondHandMovementTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocSecondHandMovementTabs/5
        [ResponseType(typeof(DocSecondHandMovementTab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandMovementTab(int id)
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

        private bool DocSecondHandMovementTabExists(int id)
        {
            return db.DocSecondHandMovementTabs.Count(e => e.DocSecondHandMovementTabID == id) > 0;
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
                "SUM(DocSecondHandMovementTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocSecondHandMovementTabs.Quantity * DocSecondHandMovementTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocSecondHandMovementTabs.Quantity * DocSecondHandMovementTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords' " + //Приписью

                "FROM Docs, DocSecondHandMovements, DocSecondHandMovementTabs " +
                "WHERE " +
                "(Docs.DocID=DocSecondHandMovements.DocID)and" +
                "(DocSecondHandMovements.DocSecondHandMovementID=DocSecondHandMovementTabs.DocSecondHandMovementID)and(Docs.DocID=@DocID)";

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

                "[DocSecondHandMovementTabs].[DocSecondHandMovementTabID] AS [DocSecondHandMovementTabID], " +
                "[DocSecondHandMovementTabs].[DocSecondHandMovementID] AS [DocSecondHandMovementID], " +
                "[DocSecondHandMovementTabs].[DirServiceNomenID] AS [DirServiceNomenID], " +
                "[DocSecondHandMovementTabs].[Quantity] AS [Quantity], " +
                "[DocSecondHandMovementTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocSecondHandMovementTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocSecondHandMovementTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocSecondHandMovementTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocSecondHandMovementTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocSecondHandMovementTabs].[DirCurrencyRate] || ', ' || [DocSecondHandMovementTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                "[DocSecondHandMovementTabs].[DirCharColourID] AS [DirCharColourID], " +
                "[DirCharColours].[DirCharColourName] AS [DirCharColourName], " +
                "[DocSecondHandMovementTabs].[DirCharMaterialID] AS [DirCharMaterialID], " +
                "[DirCharMaterials].[DirCharMaterialName] AS [DirCharMaterialName], " +
                "[DocSecondHandMovementTabs].[DirCharNameID] AS [DirCharNameID], " +
                "[DirCharNames].[DirCharNameName] AS [DirCharNameName], " +
                "[DocSecondHandMovementTabs].[DirCharSeasonID] AS [DirCharSeasonID], " +
                "[DirCharSeasons].[DirCharSeasonName] AS [DirCharSeasonName], " +
                "[DocSecondHandMovementTabs].[DirCharSexID] AS [DirCharSexID], " +
                "[DirCharSexes].[DirCharSexName] AS [DirCharSexName], " +
                "[DocSecondHandMovementTabs].[DirCharSizeID] AS [DirCharSizeID], " +
                "[DirCharSizes].[DirCharSizeName] AS [DirCharSizeName], " +
                "[DocSecondHandMovementTabs].[DirCharStyleID] AS [DirCharStyleID], " +
                "[DirCharStyles].[DirCharStyleName] AS [DirCharStyleName], " +
                "[DocSecondHandMovementTabs].[DirCharTextureID] AS [DirCharTextureID], " +
                "[DirCharTextures].[DirCharTextureName] AS [DirCharTextureName], " +
                "CASE WHEN ([DirCharColours].[DirCharColourName] IS NULL) THEN '' ELSE [DirCharColours].[DirCharColourName] END || ' ' || CASE WHEN ([DirCharMaterials].[DirCharMaterialName] IS NULL) THEN '' ELSE [DirCharMaterials].[DirCharMaterialName] END || ' ' || CASE WHEN ([DirCharNames].[DirCharNameName] IS NULL) THEN '' ELSE [DirCharNames].[DirCharNameName] END || ' ' || CASE WHEN ([DirCharSeasons].[DirCharSeasonName] IS NULL) THEN '' ELSE [DirCharSeasons].[DirCharSeasonName] END || ' ' || CASE WHEN ([DirCharSexes].[DirCharSexName] IS NULL) THEN '' ELSE [DirCharSexes].[DirCharSexName] END || ' ' || CASE WHEN ([DirCharSizes].[DirCharSizeName] IS NULL) THEN '' ELSE [DirCharSizes].[DirCharSizeName] END || ' ' || CASE WHEN ([DirCharStyles].[DirCharStyleName] IS NULL) THEN '' ELSE [DirCharStyles].[DirCharStyleName] END || ' ' || CASE WHEN ([DirCharTextures].[DirCharTextureName] IS NULL) THEN '' ELSE [DirCharTextures].[DirCharTextureName] END AS [DirChar], " +
                "[DocSecondHandMovementTabs].[SerialNumber] AS [SerialNumber], " +
                "[DocSecondHandMovementTabs].[Barcode] AS [Barcode], " +
                "[DocSecondHandMovementTabs].[Barcode] AS [BarcodeImage], " +

                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocSecondHandMovementTabs].[PriceCurrency] AS [PriceCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocSecondHandMovementTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocSecondHandMovementTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +

                //Себестоимось прихода
                "ROUND([DocSecondHandMovementTabs].[Quantity] * [DocSecondHandMovementTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocSecondHandMovementTabs].[Quantity] * [DocSecondHandMovementTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SumPurchPriceCurrency], " +


                //Розница
                "[DocSecondHandMovementTabs].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[DocSecondHandMovementTabs].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость в валюте
                "ROUND([DocSecondHandMovementTabs].[Quantity] * [DocSecondHandMovementTabs].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocSecondHandMovementTabs].[Quantity] * [DocSecondHandMovementTabs].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " +

                //Опт
                "[DocSecondHandMovementTabs].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[DocSecondHandMovementTabs].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость в валюте
                "ROUND([DocSecondHandMovementTabs].[Quantity] * [DocSecondHandMovementTabs].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocSecondHandMovementTabs].[Quantity] * [DocSecondHandMovementTabs].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " +

                //Интернет-Магазин
                "[DocSecondHandMovementTabs].[PriceIMVAT] AS [PriceIMVAT], " +
                "[DocSecondHandMovementTabs].[PriceIMCurrency] AS [PriceIMCurrency], " +
                //Стоимость в валюте
                "ROUND([DocSecondHandMovementTabs].[Quantity] * [DocSecondHandMovementTabs].[PriceIMVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMIMPriceVAT], " +
                //Стоимость в текущей валюте
                "ROUND([DocSecondHandMovementTabs].[Quantity] * [DocSecondHandMovementTabs].[PriceIMCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMIMPriceCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocSecondHandMovements], [DocSecondHandMovementTabs] " +

                "LEFT OUTER JOIN [DirCharColours] ON [DocSecondHandMovementTabs].[DirCharColourID] = [DirCharColours].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] ON [DocSecondHandMovementTabs].[DirCharMaterialID] = [DirCharMaterials].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] ON [DocSecondHandMovementTabs].[DirCharNameID] = [DirCharNames].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] ON [DocSecondHandMovementTabs].[DirCharSeasonID] = [DirCharSeasons].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] ON [DocSecondHandMovementTabs].[DirCharSexID] = [DirCharSexes].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] ON [DocSecondHandMovementTabs].[DirCharSizeID] = [DirCharSizes].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] ON [DocSecondHandMovementTabs].[DirCharStyleID] = [DirCharStyles].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] ON [DocSecondHandMovementTabs].[DirCharTextureID] = [DirCharTextures].[DirCharTextureID] " +
                //"INNER JOIN [DirServiceNomens] ON [DocSecondHandMovementTabs].[DirServiceNomenID] = [DirServiceNomens].[DirServiceNomenID] " +
                //"LEFT OUTER JOIN [DirServiceNomens] AS [DirServiceNomenGroups] ON [DirServiceNomenGroups].[Sub] = [DirServiceNomens].[DirServiceNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocSecondHandMovementTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirServiceNomens] AS [DirServiceNomens] ON [DirServiceNomens].[DirServiceNomenID] = [DocSecondHandMovementTabs].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensGroup] ON [DirServiceNomens].[Sub] = [dirServiceNomensGroup].[DirServiceNomenID] " +
                "LEFT JOIN [DirServiceNomens] AS [dirServiceNomensSubGroup] ON [dirServiceNomensGroup].[Sub] = [dirServiceNomensSubGroup].[DirServiceNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocSecondHandMovements].[DocID])and([DocSecondHandMovements].[DocSecondHandMovementID]=[DocSecondHandMovementTabs].[DocSecondHandMovementID])and(Docs.DocID=@DocID) " +

                "";


            return SQL;
        }

        #endregion
    }
}