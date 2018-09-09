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

namespace PartionnyAccount.Controllers.Sklad.Logistic
{
    public class LogisticTabsController : ApiController
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
            //public int LogisticID;
            public int DocID;
        }
        // GET: api/LogisticTabs
        public async Task<IHttpActionResult> GetLogisticTabs(HttpRequestMessage request)
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
                _params.DocID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocID", true) == 0).Value);

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

                        where docMovementTabs.docMovement.DocID == _params.DocID

                        #region select

                        select new
                        {
                            
                            LogisticTabID = docMovementTabs.DocMovementTabID,
                            LogisticID = docMovementTabs.DocMovementID,
                            DirNomenXID = docMovementTabs.DirNomenID,

                            //DirNomenXName = docMovementTabs.dirNomen.DirNomenName,
                            DirNomenXName =
                            dirNomensSubGroup.DirNomenName == null ? docMovementTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docMovementTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docMovementTabs.dirNomen.DirNomenName,

                            RemXPartyID = docMovementTabs.RemPartyID,
                            Quantity = docMovementTabs.Quantity,

                            PriceVAT = docMovementTabs.PriceVAT,

                            DirCurrencyID = docMovementTabs.DirCurrencyID,
                            DirCurrencyRate = docMovementTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docMovementTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docMovementTabs.dirCurrency.DirCurrencyName + " (" + docMovementTabs.DirCurrencyRate + ", " + docMovementTabs.DirCurrencyMultiplicity + ")",
                            

                            //Характеристики
                            /*
                            //DirCharColourID = docMovementTabs.DirCharColourID,
                            DirCharColourName = dirCharColours.DirCharColourName,
                            //DirCharMaterialID = docMovementTabs.DirCharMaterialID,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            //DirCharNameID = docMovementTabs.DirCharNameID,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            //DirCharSeasonID = docMovementTabs.DirCharSeasonID,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            //DirCharSexID = docMovementTabs.DirCharSexID,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            //DirCharSizeID = docMovementTabs.DirCharSizeID,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            //DirCharStyleID = docMovementTabs.DirCharStyleID,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            //DirCharTextureID = docMovementTabs.DirCharTextureID,
                            DirCharTextureName = dirCharTextures.DirCharTextureName,
                            */
                            DirChar =
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
                            //SerialNumber = docMovementTabs.SerialNumber,
                            //Barcode = docMovementTabs.Barcode,
                            
                            
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
                    ).

                    Union

                    (
                        from docSecondHandMovementTabs in db.DocSecondHandMovementTabs

                        join dirServiceNomens11 in db.DirServiceNomens on docSecondHandMovementTabs.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where docSecondHandMovementTabs.docSecondHandMovement.DocID == _params.DocID

                        #region select

                        select new
                        {
                            
                            LogisticTabID = docSecondHandMovementTabs.DocSecondHandMovementTabID,
                            LogisticID = docSecondHandMovementTabs.DocSecondHandMovementID,
                            DirNomenXID = docSecondHandMovementTabs.DirServiceNomenID,
                            
                            //DirService2NomenName = docSecondHandMovementTabs.dirService2Nomen.DirService2NomenName,
                            DirNomenXName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? docSecondHandMovementTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + docSecondHandMovementTabs.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + docSecondHandMovementTabs.dirServiceNomen.DirServiceNomenName,

                            RemXPartyID = docSecondHandMovementTabs.Rem2PartyID,
                            Quantity = docSecondHandMovementTabs.Quantity,

                            PriceVAT = docSecondHandMovementTabs.PriceVAT,

                            DirCurrencyID = docSecondHandMovementTabs.DirCurrencyID,
                            DirCurrencyRate = docSecondHandMovementTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSecondHandMovementTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docSecondHandMovementTabs.dirCurrency.DirCurrencyName + " (" + docSecondHandMovementTabs.DirCurrencyRate + ", " + docSecondHandMovementTabs.DirCurrencyMultiplicity + ")",
                            

                            //Характеристики
                            /*
                            //DirCharColourID = 0,
                            DirCharColourName = "",
                            //DirCharMaterialID = 0,
                            DirCharMaterialName = "",
                            //DirCharNameID = 0,
                            DirCharNameName = "",
                            //DirCharSeasonID = 0,
                            DirCharSeasonName = "",
                            //DirCharSexID = 0,
                            DirCharSexName = "",
                            //DirCharSizeID = 0,
                            DirCharSizeName = "",
                            //DirCharStyleID = 0,
                            DirCharStyleName = "",
                            //DirCharTextureID = null,
                            DirCharTextureName = "",
                            */
                            DirChar = "",
                            //SerialNumber = "",
                            //Barcode = "",
                            
                            
                            //Цена в т.в.
                            PriceCurrency = docSecondHandMovementTabs.PriceCurrency,
                            
                            //Себестоимость
                            SUMMovementPriceVATCurrency = docSecondHandMovementTabs.Quantity * docSecondHandMovementTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docSecondHandMovementTabs.Quantity * docSecondHandMovementTabs.PriceCurrency, sysSetting.FractionalPartInSum),
                            
                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docSecondHandMovementTabs.PriceRetailVAT - docSecondHandMovementTabs.PriceVAT) / docSecondHandMovementTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docSecondHandMovementTabs.PriceRetailVAT - docSecondHandMovementTabs.PriceVAT) / docSecondHandMovementTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docSecondHandMovementTabs.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docSecondHandMovementTabs.PriceRetailCurrency,
                            
                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docSecondHandMovementTabs.PriceWholesaleVAT - docSecondHandMovementTabs.PriceVAT) / docSecondHandMovementTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docSecondHandMovementTabs.PriceWholesaleVAT - docSecondHandMovementTabs.PriceVAT) / docSecondHandMovementTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Оптовая цена 
                            PriceWholesaleVAT = docSecondHandMovementTabs.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docSecondHandMovementTabs.PriceWholesaleCurrency,
                            

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docSecondHandMovementTabs.PriceIMVAT - docSecondHandMovementTabs.PriceVAT) / docSecondHandMovementTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docSecondHandMovementTabs.PriceIMVAT - docSecondHandMovementTabs.PriceVAT) / docSecondHandMovementTabs.PriceVAT, sysSetting.FractionalPartInSum)
                            ,
                            //Интернет-Магазин
                            PriceIMVAT = docSecondHandMovementTabs.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docSecondHandMovementTabs.PriceIMCurrency,
                            

                            //Причина возврата
                            DirDescriptionID = docSecondHandMovementTabs.DirDescriptionID,
                            DirDescriptionName = docSecondHandMovementTabs.dirDescription.DirDescriptionName,
                            DirReturnTypeID = docSecondHandMovementTabs.DirReturnTypeID,
                            DirReturnTypeName = docSecondHandMovementTabs.dirReturnType.DirReturnTypeName
                            
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    LogisticTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/LogisticTabs/5
        [ResponseType(typeof(DocMovementTab))]
        public async Task<IHttpActionResult> GetLogisticTab(int id)
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

    }
}
