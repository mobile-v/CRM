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
using PartionnyAccount.Models.Sklad.Rem;
using System.Data.SQLite;

namespace PartionnyAccount.Controllers.Sklad.Rem
{
    public class RemPartiesController : ApiController
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
            public DateTime DocDate;
            public int DirContractorIDOrg;
            public int DirNomenID;
            public int DirWarehouseID;
            public int DirContractorID;
            public string parSearch = "";

            //В "collectionWrapper" по умолчанию "RemParty = query"
            //1. Для DocActWriteOffTabs будет "DocActWriteOffTab = query"
            //2. Для DocInventoryTabs будет "DocInventoryTab = query"
            public string queryIn = "";
        }
        // GET: api/RemParties
        public async Task<IHttpActionResult> GetRemParties(HttpRequestMessage request)
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
                /*
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightRemParties"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                */

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
                _params.DocDate = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDate", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                _params.DirContractorIDOrg = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorIDOrg", true) == 0).Value);
                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);
                _params.DirNomenID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirNomenID", true) == 0).Value);
                _params.DirContractorID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorID", true) == 0).Value);

                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                //В "collectionWrapper" по умолчанию "RemParty = query"
                //Но, для DocActWriteOffTabs будет "DocActWriteOffTab = query"
                _params.queryIn = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "queryIn", true) == 0).Value;

                #endregion


                #region Запрос и отправка

                if (String.IsNullOrEmpty(_params.queryIn))
                {

                    #region Основной запрос *** *** ***

                    var query =
                    (
                        from remParties in db.RemParties

                        where
                            remParties.Remnant > 0
                            //remParties.DirContractorIDOrg == _params.DirContractorIDOrg && 
                            //remParties.DirNomenID == _params.DirNomenID &&
                            //remParties.DirWarehouseID == _params.DirWarehouseID

                        select new
                        {
                            DocID = remParties.DocID,
                            NumberReal = remParties.doc.NumberReal,
                            RemPartyID = remParties.RemPartyID,
                            DirNomenID = remParties.DirNomenID,
                            DirNomenName = remParties.dirNomen.DirNomenName,
                            DocDate = remParties.doc.DocDate,
                            ListObjectID = remParties.doc.ListObjectID,

                            DirContractorIDOrg = remParties.DirContractorIDOrg,
                            DirContractorNameOrg = remParties.doc.dirContractorOrg.DirContractorName,

                            //DirContractorID = remParties.doc.dirContractor.DirContractorID,
                            //DirContractorName = remParties.doc.dirContractor.DirContractorName,
                            DirContractorID = remParties.dirContractor.DirContractorID,
                            DirContractorName = remParties.dirContractor.DirContractorName,

                            DocDatePurches = remParties.DocDatePurches,

                            //Характеристики
                            DirCharColourID = remParties.DirCharColourID,
                            DirCharColourName = remParties.dirCharColour.DirCharColourName,
                            DirCharMaterialID = remParties.DirCharMaterialID,
                            DirCharMaterialName = remParties.dirCharMaterial.DirCharMaterialName,
                            DirCharNameID = remParties.DirCharNameID,
                            DirCharNameName = remParties.dirCharName.DirCharNameName,
                            DirCharSeasonID = remParties.DirCharSeasonID,
                            DirCharSeasonName = remParties.dirCharSeason.DirCharSeasonName,
                            DirCharSexID = remParties.DirCharSexID,
                            DirCharSexName = remParties.dirCharSex.DirCharSexName,
                            DirCharSizeID = remParties.DirCharSizeID,
                            DirCharSizeName = remParties.dirCharSize.DirCharSizeName,
                            DirCharStyleID = remParties.DirCharStyleID,
                            DirCharStyleName = remParties.dirCharStyle.DirCharStyleName,
                            DirCharTextureID = remParties.DirCharTextureID,
                            DirCharTextureName = remParties.dirCharTexture.DirCharTextureName,
                            DirChar =
                                remParties.dirCharColour.DirCharColourName + " " +
                                remParties.dirCharMaterial.DirCharMaterialName + " " +
                                remParties.dirCharName.DirCharNameName + " " +
                                remParties.dirCharSeason.DirCharSeasonName + " " +
                                remParties.dirCharSex.DirCharSexName + " " +
                                remParties.dirCharSize.DirCharSizeName + " " +
                                remParties.dirCharStyle.DirCharStyleName + " " +
                                remParties.dirCharTexture.DirCharTextureName,
                            SerialNumber = remParties.SerialNumber,
                            Barcode = remParties.Barcode,


                            DirCurrencyID = remParties.DirCurrencyID,
                            DirCurrencyName = remParties.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = remParties.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = remParties.dirCurrency.DirCurrencyMultiplicity,

                            DirVatValue = remParties.DirVatValue,
                            DirWarehouseID = remParties.DirWarehouseID,
                            DirWarehouseName = remParties.dirWarehouse.DirWarehouseName,
                            ListDocNameRu = remParties.doc.listObject.ListObjectNameRu,
                            PriceVAT = remParties.PriceVAT, //PriceVAT = Math.Round(remParties.PriceVAT, sysSetting.FractionalPartInPrice),
                            PriceCurrency = remParties.PriceCurrency, //PriceCurrency = Math.Round(remParties.PriceCurrency, sysSetting.FractionalPartInPrice),
                            Quantity = remParties.Quantity,
                            Remnant = remParties.Remnant,
                            //Reserve = remParties.Reserve

                            MarkupRetail = ((remParties.PriceRetailVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((remParties.PriceRetailVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceRetailVAT = remParties.PriceRetailVAT - remParties.doc.Discount,
                            PriceRetailCurrency = (remParties.PriceRetailVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((remParties.PriceRetailVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity - remParties.doc.Discount, sysSetting.FractionalPartInPrice),

                            MarkupWholesale = ((remParties.PriceWholesaleVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((remParties.PriceWholesaleVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceWholesaleVAT = remParties.PriceWholesaleVAT - remParties.doc.Discount,
                            PriceWholesaleCurrency = (remParties.PriceWholesaleVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((remParties.PriceWholesaleVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity - remParties.doc.Discount, sysSetting.FractionalPartInPrice),

                            MarkupIM = ((remParties.PriceIMVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((remParties.PriceIMVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceIMVAT = remParties.PriceIMVAT - remParties.doc.Discount,
                            PriceIMCurrency = (remParties.PriceIMVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((remParties.PriceIMVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity - remParties.doc.Discount, sysSetting.FractionalPartInPrice),

                            DirNomenMinimumBalance = remParties.DirNomenMinimumBalance,

                            DirEmployeeName = remParties.doc.dirEmployee.DirEmployeeName,

                            //Причина возврата
                            DirDescriptionID = remParties.DirDescriptionID,
                            DirDescriptionName = remParties.dirDescription.DirDescriptionName,
                            DirReturnTypeID = remParties.DirReturnTypeID,
                            DirReturnTypeName = remParties.dirReturnType.DirReturnTypeName,


                            SysGenID = remParties.dirNomen.SysGenID,
                            //SysGenIDPatch = @"UsersTemp/UserImage/" + field.DirCustomersID + "_" + x.SysGenID + ".jpg"
                            SysGenIDPatch = remParties.dirNomen.SysGenID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + remParties.dirNomen.SysGenID + ".jpg"
                        }
                    );

                    #endregion


                    #region Условия (параметры) *** *** ***

                    //Поиск товара
                    if (_params.DirNomenID <= 0 && string.IsNullOrEmpty(_params.parSearch))
                    {
                        dynamic collectionWrapper1 = new
                        {
                            sucess = true,
                            total = 0,
                            RemParty = 0
                        };
                        return await Task.Run(() => Ok(collectionWrapper1));
                    }


                    #region Кликнули на товар - паказать список партий

                    //if (_params.DocDate > Convert.ToDateTime("2000-01-01")) query = query.Where(x => x.DocDate <= _params.DocDate);
                    if (_params.DirContractorIDOrg > 0) query = query.Where(x => x.DirContractorIDOrg == _params.DirContractorIDOrg);
                    if (_params.DirNomenID > 0) query = query.Where(x => x.DirNomenID == _params.DirNomenID);
                    if (_params.DirWarehouseID > 0) query = query.Where(x => x.DirWarehouseID == _params.DirWarehouseID);
                    if (_params.DirContractorID > 0) query = query.Where(x => x.DirContractorID == _params.DirContractorID);

                    #endregion


                    #region Поиск партии по Серийному номеру или по Штрих-коду

                    if (!string.IsNullOrEmpty(_params.parSearch))
                    {
                        //query = query.Where(x => x.SerialNumber == _params.parSearch || x.Barcode == _params.parSearch);

                        //Если число, то искать в коде товара
                        int? iID_ = 0;
                        int value;
                        if (int.TryParse(_params.parSearch, out value))
                        {
                            iID_ = Convert.ToInt32(_params.parSearch);
                            query = query.Where(x => x.SerialNumber == _params.parSearch || x.Barcode == _params.parSearch || x.DirNomenID == iID_);
                        }
                        else
                        {
                            query = query.Where(x => x.SerialNumber == _params.parSearch || x.Barcode == _params.parSearch);
                        }
                    }


                    #endregion


                    #region OrderBy

                    query = query.OrderByDescending(x => x.DocDate);
                    
                    #endregion


                    #endregion


                    #region Отправка JSON

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount = query.Count();

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        RemParty = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else 
                {
                    #region Основной запрос *** *** ***

                    var query =
                    (
                        from remParties in db.RemParties

                        where
                        remParties.Remnant > 0 &&
                        remParties.DirContractorIDOrg == _params.DirContractorIDOrg &&
                        remParties.DirWarehouseID == _params.DirWarehouseID

                        select new
                        {
                            DocID = remParties.DocID,
                            NumberReal = remParties.doc.NumberReal,
                            RemPartyID = remParties.RemPartyID,
                            DirNomenID = remParties.DirNomenID,
                            DirNomenName = remParties.dirNomen.DirNomenName,
                            //DocDate = remParties.doc.DocDate,
                            //ListObjectID = remParties.doc.ListObjectID,

                            //DirContractorIDOrg = remParties.DirContractorIDOrg,
                            //DirContractorNameOrg = remParties.doc.dirContractorOrg.DirContractorName,

                            DirContractorID = remParties.dirContractor.DirContractorID,
                            //DirContractorName = remParties.dirContractor.DirContractorName,

                            //DocDatePurches = remParties.DocDatePurches,

                            //Характеристики
                            DirCharColourID = remParties.DirCharColourID,
                            DirCharColourName = remParties.dirCharColour.DirCharColourName,
                            DirCharMaterialID = remParties.DirCharMaterialID,
                            DirCharMaterialName = remParties.dirCharMaterial.DirCharMaterialName,
                            DirCharNameID = remParties.DirCharNameID,
                            DirCharNameName = remParties.dirCharName.DirCharNameName,
                            DirCharSeasonID = remParties.DirCharSeasonID,
                            DirCharSeasonName = remParties.dirCharSeason.DirCharSeasonName,
                            DirCharSexID = remParties.DirCharSexID,
                            DirCharSexName = remParties.dirCharSex.DirCharSexName,
                            DirCharSizeID = remParties.DirCharSizeID,
                            DirCharSizeName = remParties.dirCharSize.DirCharSizeName,
                            DirCharStyleID = remParties.DirCharStyleID,
                            DirCharStyleName = remParties.dirCharStyle.DirCharStyleName,
                            DirCharTextureID = remParties.DirCharTextureID,
                            DirCharTextureName = remParties.dirCharTexture.DirCharTextureName,
                            DirChar =
                                remParties.dirCharColour.DirCharColourName + " " +
                                remParties.dirCharMaterial.DirCharMaterialName + " " +
                                remParties.dirCharName.DirCharNameName + " " +
                                remParties.dirCharSeason.DirCharSeasonName + " " +
                                remParties.dirCharSex.DirCharSexName + " " +
                                remParties.dirCharSize.DirCharSizeName + " " +
                                remParties.dirCharStyle.DirCharStyleName + " " +
                                remParties.dirCharTexture.DirCharTextureName,
                            //SerialNumber = remParties.SerialNumber,
                            //Barcode = remParties.Barcode,


                            DirCurrencyID = remParties.DirCurrencyID,
                            DirCurrencyName = remParties.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = remParties.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = remParties.dirCurrency.DirCurrencyMultiplicity,

                            DirVatValue = remParties.DirVatValue,
                            //DirWarehouseID = remParties.DirWarehouseID,
                            //DirWarehouseName = remParties.dirWarehouse.DirWarehouseName,
                            //ListDocNameRu = remParties.doc.listObject.ListObjectNameRu,
                            PriceVAT = remParties.PriceVAT, //PriceVAT = Math.Round(remParties.PriceVAT, sysSetting.FractionalPartInPrice),
                            PriceCurrency = remParties.PriceCurrency, //PriceCurrency = Math.Round(remParties.PriceCurrency, sysSetting.FractionalPartInPrice),

                            Quantity = remParties.Remnant,
                            Quantity_WriteOff = remParties.Remnant,
                            Quantity_Purch = 0,
                            //Quantity = remParties.Quantity,
                            //Remnant = remParties.Remnant,
                            //Reserve = remParties.Reserve

                            
                            MarkupRetail = ((remParties.PriceRetailVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((remParties.PriceRetailVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceRetailVAT = remParties.PriceRetailVAT,
                            PriceRetailCurrency = (remParties.PriceRetailVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((remParties.PriceRetailVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity, sysSetting.FractionalPartInPrice),

                            MarkupWholesale = ((remParties.PriceWholesaleVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((remParties.PriceWholesaleVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceWholesaleVAT = remParties.PriceWholesaleVAT,
                            PriceWholesaleCurrency = (remParties.PriceWholesaleVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((remParties.PriceWholesaleVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity, sysSetting.FractionalPartInPrice),

                            MarkupIM = ((remParties.PriceIMVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((remParties.PriceIMVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceIMVAT = remParties.PriceIMVAT,
                            PriceIMCurrency = (remParties.PriceIMVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((remParties.PriceIMVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity, sysSetting.FractionalPartInPrice),

                            DirNomenMinimumBalance = remParties.DirNomenMinimumBalance,

                            DirEmployeeName = remParties.doc.dirEmployee.DirEmployeeName,

                            //Причина возврата
                            /*
                            DirDescriptionID = remParties.DirDescriptionID,
                            DirDescriptionName = remParties.dirDescription.DirDescriptionName,
                            DirReturnTypeID = remParties.DirReturnTypeID,
                            DirReturnTypeName = remParties.dirReturnType.DirReturnTypeName,


                            SysGenID = remParties.dirNomen.SysGenID,
                            //SysGenIDPatch = @"UsersTemp/UserImage/" + field.DirCustomersID + "_" + x.SysGenID + ".jpg"
                            SysGenIDPatch = remParties.dirNomen.SysGenID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + remParties.dirNomen.SysGenID + ".jpg"
                            */
                        }
                    );

                    #endregion


                    #region Отправка JSON

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount = query.Count();

                    if (_params.queryIn == "DocActWriteOffTab")
                    {
                        dynamic collectionWrapper = new
                        {
                            sucess = true,
                            total = dirCount,
                            DocActWriteOffTab = query
                        };
                        return await Task.Run(() => Ok(collectionWrapper));
                    }
                    else //if (_params.queryIn == "DocInventoryTab")
                    {
                        dynamic collectionWrapper = new
                        {
                            sucess = true,
                            total = dirCount,
                            DocInventoryTab = query
                        };
                        return await Task.Run(() => Ok(collectionWrapper));
                    }

                    #endregion
                }
                

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        //Не используется!
        //Раньше использовал для получения последней цены, сейчас цена берётся из истории Товра!
        // GET: api/RemParties/5 - Используется для получения цен из поледней мартии
        [ResponseType(typeof(RemParty))]
        public async Task<IHttpActionResult> GetRemParty(int id, HttpRequestMessage request)
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
                /*
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightRemParties"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                */

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
                int Action = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "Action", true) == 0).Value);

                #endregion


                #region Отправка JSON

                if (Action == 1)
                {

                    //Характеристики не нужны. Т.к. товар может прийти и с другими Характеристиками.

                    var query = await Task.Run(() =>
                     (
                        from remParties in db.RemParties

                            #region Характеристики
                            /*
                            join dirCharColours1 in db.DirCharColours on remParties.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                            from dirCharColours in dirCharColours2.DefaultIfEmpty()

                            join dirCharMaterials1 in db.DirCharMaterials on remParties.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                            from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                            join dirCharNames1 in db.DirCharNames on remParties.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                            from dirCharNames in dirCharNames2.DefaultIfEmpty()

                            join dirCharSeasons1 in db.DirCharSeasons on remParties.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                            from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                            join dirCharSexes1 in db.DirCharSexes on remParties.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                            from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                            join dirCharSizes1 in db.DirCharSizes on remParties.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                            from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                            join dirCharStyles1 in db.DirCharStyles on remParties.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                            from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                            join dirCharTextures1 in db.DirCharTextures on remParties.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                            from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                            */
                            #endregion

                        where remParties.DirNomenID == id && remParties.doc.ListObjectID == 6 //&& remParties.Remnant == 0

                        select new
                        {
                            RemPartyID = remParties.RemPartyID,
                            DocDate = remParties.doc.DocDate,
                            DirContractorName = remParties.doc.dirContractor.DirContractorName,

                            #region Характеристики
                            /*
                            DirCharColourID = remParties.DirCharColourID,
                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialID = remParties.DirCharMaterialID,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameID = remParties.DirCharNameID,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonID = remParties.DirCharSeasonID,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexID = remParties.DirCharSexID,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeID = remParties.DirCharSizeID,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleID = remParties.DirCharStyleID,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureID = remParties.DirCharTextureID,
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
                            SerialNumber = remParties.SerialNumber,
                            Barcode = remParties.Barcode,
                            */
                            #endregion

                            DirCurrencyID = remParties.DirCurrencyID,
                            DirCurrencyName = remParties.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = remParties.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = remParties.dirCurrency.DirCurrencyMultiplicity,

                            DirVatValue = remParties.DirVatValue,
                            DirWarehouseName = remParties.dirWarehouse.DirWarehouseName,
                            ListDocNameRu = remParties.doc.listObject.ListObjectNameRu,
                            PriceVAT = remParties.PriceVAT,
                            PriceCurrency = remParties.PriceCurrency,
                            Quantity = remParties.Quantity,
                            Remnant = remParties.Remnant,
                            //Reserve = remParties.Reserve

                            MarkupRetail = Math.Round(((remParties.PriceRetailVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceRetailVAT = remParties.PriceRetailVAT - remParties.doc.Discount,
                            PriceRetailCurrency = Math.Round((remParties.PriceRetailVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity - remParties.doc.Discount, sysSetting.FractionalPartInSum),

                            MarkupWholesale = Math.Round(((remParties.PriceWholesaleVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceWholesaleVAT = remParties.PriceWholesaleVAT - remParties.doc.Discount,
                            PriceWholesaleCurrency = Math.Round((remParties.PriceWholesaleVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity - remParties.doc.Discount, sysSetting.FractionalPartInSum),

                            MarkupIM = Math.Round(((remParties.PriceIMVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceIMVAT = remParties.PriceIMVAT - remParties.doc.Discount,
                            PriceIMCurrency = Math.Round((remParties.PriceIMVAT * remParties.dirCurrency.DirCurrencyRate) / remParties.dirCurrency.DirCurrencyMultiplicity - remParties.doc.Discount, sysSetting.FractionalPartInSum),

                            DirNomenMinimumBalance = remParties.DirNomenMinimumBalance
                        }
                    ).OrderByDescending(t => t.DocDate)); //.FirstAsync()


                    //Вариант-1
                    if (query.Count() > 0)
                    {
                        return Ok(returnServer.Return(true, query.FirstAsync()));
                    }
                    else
                    {
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89_1));
                    }

                    //Вариант-2 (возможно будет быстрее работать. Всё из-за "query.Count()")
                    //Не работает ...
                    /*
                    try { return Ok(returnServer.Return(true, query.FirstAsync())); }
                    catch { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89_1)); }
                    */
                }
                else
                {
                    return Ok(returnServer.Return(true, "Error"));
                }

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }



        // GET: api/RemParties/5
        /*
        [ResponseType(typeof(RemParty))]
        public async Task<IHttpActionResult> GetRemParty(string pSearch, int iPriznak)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightRemParties"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from remParties in db.RemParties

                        //Характеристики
                        join dirCharColours1 in db.DirCharColours on remParties.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on remParties.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on remParties.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on remParties.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on remParties.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on remParties.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on remParties.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on remParties.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        
                        //where remParties.DirNomenID == _params.DirNomenID && remParties.Remnant > 0
                        where remParties.Remnant > 0 && (remParties.SerialNumber == pSearch || remParties.Barcode == pSearch)

                        select new
                        {
                            RemPartyID = remParties.RemPartyID,
                            DirNomenID = remParties.DirNomenID,
                            DocDate = remParties.doc.DocDate,
                            DirContractorName = remParties.doc.dirContractor.DirContractorName,


                            //Характеристики
                            DirCharColourID = remParties.DirCharColourID,
                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialID = remParties.DirCharMaterialID,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameID = remParties.DirCharNameID,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonID = remParties.DirCharSeasonID,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexID = remParties.DirCharSexID,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeID = remParties.DirCharSizeID,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleID = remParties.DirCharStyleID,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureID = remParties.DirCharTextureID,
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
                            SerialNumber = remParties.SerialNumber,
                            Barcode = remParties.Barcode,


                            DirCurrencyID = remParties.DirCurrencyID,
                            DirCurrencyName = remParties.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = remParties.DirCurrencyRate,
                            DirCurrencyMultiplicity = remParties.DirCurrencyMultiplicity,

                            DirVatValue = remParties.DirVatValue,
                            DirWarehouseName = remParties.dirWarehouse.DirWarehouseName,
                            ListDocNameRu = remParties.doc.listObject.ListObjectNameRu,
                            PriceVAT = remParties.PriceVAT,
                            PriceCurrency = remParties.PriceCurrency,
                            Quantity = remParties.Quantity,
                            Remnant = remParties.Remnant,
                            //Reserve = remParties.Reserve

                            MarkupRetail = Math.Round(((remParties.PriceRetailVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceRetailVAT = remParties.PriceRetailVAT,
                            PriceRetailCurrency = Math.Round((remParties.PriceRetailVAT * remParties.DirCurrencyRate) / remParties.DirCurrencyMultiplicity, sysSetting.FractionalPartInSum),

                            MarkupWholesale = Math.Round(((remParties.PriceWholesaleVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceWholesaleVAT = remParties.PriceWholesaleVAT,
                            PriceWholesaleCurrency = Math.Round((remParties.PriceWholesaleVAT * remParties.DirCurrencyRate) / remParties.DirCurrencyMultiplicity, sysSetting.FractionalPartInSum),

                            MarkupIM = Math.Round(((remParties.PriceIMVAT - remParties.PriceVAT) / remParties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceIMVAT = remParties.PriceIMVAT,
                            PriceIMCurrency = Math.Round((remParties.PriceIMVAT * remParties.DirCurrencyRate) / remParties.DirCurrencyMultiplicity, sysSetting.FractionalPartInSum)
                        }
                    );

                #endregion


                #region Отправка JSON

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount = query.Count();

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    RemParty = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }
        */

        #endregion


        #region UPDATE

        // PUT: api/RemParties/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRemParty(int id, RemParty remParty)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/RemParties
        [ResponseType(typeof(RemParty))]
        public async Task<IHttpActionResult> PostRemParty(RemParty remParty)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/RemParties/5
        [ResponseType(typeof(RemParty))]
        public async Task<IHttpActionResult> DeleteRemParty(int id)
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

        private bool RemPartyExists(int id)
        {
            return db.RemParties.Count(e => e.RemPartyID == id) > 0;
        }


        #region Save

        //!!! ВАЖНО !!!
        //1. Удаление
        //2. Проверка на отрицательные остатки

        internal void Delete(
            DbConnectionSklad _db,
            Models.Sklad.Rem.RemParty[] remPartyCollection
            )
        {
            SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = remPartyCollection[0].DocID };
            db.Database.ExecuteSqlCommand("DELETE FROM RemParties WHERE DocID=@DocID;", parDocID);
        }

        internal async Task<Models.Sklad.Rem.RemParty[]> Save(
            DbConnectionSklad _db,
            Models.Sklad.Rem.RemParty[] remPartyCollection
            )
        {
            db = _db;

            Delete(_db, remPartyCollection);

            //Сохраняем "party" и
            //Меняем полученный ID-шник
            for (int i = 0; i < remPartyCollection.Count(); i++)
            {
                //party
                db.Entry(remPartyCollection[i]).State = EntityState.Added;
                //await db.SaveChangesAsync();
            }
            await db.SaveChangesAsync();

            return remPartyCollection;
        }

        #endregion

        #endregion
    }
}