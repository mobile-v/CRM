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
    public class Rem2PartiesController : ApiController
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
            public int DirServiceNomenID;
            public int DirWarehouseID;
            public int DirServiceContractorID;
            public string parSearch = "";
            public string collectionWrapper = "";
        }
        // GET: api/Rem2Party
        public async Task<IHttpActionResult> GetRem2Parties(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightRem2Parties"));
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
                _params.DirServiceNomenID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceNomenID", true) == 0).Value);
                _params.DirServiceContractorID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceContractorID", true) == 0).Value);

                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск
                _params.collectionWrapper = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "collectionWrapper", true) == 0).Value;

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from rem2Parties in db.Rem2Parties
                        //from docSecondHandPurches in db.DocSecondHandPurches

                        join docSecondHandPurches11 in db.DocSecondHandPurches on rem2Parties.DocIDFirst equals docSecondHandPurches11.DocID into docSecondHandPurches12
                        from docSecondHandPurches in docSecondHandPurches12.DefaultIfEmpty()

                        join dirServiceNomens11 in db.DirServiceNomens on rem2Parties.dirServiceNomen.Sub equals dirServiceNomens11.DirServiceNomenID into dirServiceNomens12
                        from dirServiceNomensSubGroup in dirServiceNomens12.DefaultIfEmpty()

                        join dirServiceNomens21 in db.DirServiceNomens on dirServiceNomensSubGroup.Sub equals dirServiceNomens21.DirServiceNomenID into dirServiceNomens22
                        from dirServiceNomensGroup in dirServiceNomens22.DefaultIfEmpty()

                        where
                            rem2Parties.Remnant > 0 //&&
                            //rem2Parties.DirContractorIDOrg == _params.DirContractorIDOrg && 
                            //rem2Parties.DirServiceNomenID == _params.DirServiceNomenID &&
                            //rem2Parties.DirWarehouseID == _params.DirWarehouseID

                            //rem2Parties.DocID == docSecondHandPurches.DocID

                        select new
                        {
                            Rem2PartyID = rem2Parties.Rem2PartyID,
                            DocSecondHandPurchID = docSecondHandPurches.DocSecondHandPurchID,
                            DirServiceNomenID = rem2Parties.DirServiceNomenID,
                            DocDate = rem2Parties.doc.DocDate,
                            ListObjectID = rem2Parties.doc.ListObjectID,

                            //DirServiceNomenName = rem2Parties.dirServiceNomen.DirServiceNomenName,
                            DirServiceNomenName =
                            dirServiceNomensSubGroup.DirServiceNomenName == null ? rem2Parties.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName == null ? dirServiceNomensSubGroup.DirServiceNomenName + " / " + rem2Parties.dirServiceNomen.DirServiceNomenName :
                            dirServiceNomensGroup.DirServiceNomenName + " / " + dirServiceNomensSubGroup.DirServiceNomenName + " / " + rem2Parties.dirServiceNomen.DirServiceNomenName,

                            DirContractorIDOrg = rem2Parties.DirContractorIDOrg,
                            DirContractorNameOrg = rem2Parties.doc.dirContractorOrg.DirContractorName,

                            //DirServiceContractorID = rem2Parties.doc.dirServiceContractor.DirServiceContractorID,
                            //DirServiceContractorName = rem2Parties.doc.dirServiceContractor.DirServiceContractorName,
                            DirServiceContractorID = rem2Parties.dirServiceContractor.DirServiceContractorID,
                            DirServiceContractorName = rem2Parties.dirServiceContractor.DirServiceContractorName,
                            DocDatePurches = rem2Parties.DocDatePurches,
                            SerialNumber = rem2Parties.SerialNumber,
                            Barcode = rem2Parties.Barcode,


                            DirCurrencyID = rem2Parties.DirCurrencyID,
                            DirCurrencyName = rem2Parties.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = rem2Parties.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = rem2Parties.dirCurrency.DirCurrencyMultiplicity,

                            DirVatValue = rem2Parties.DirVatValue,
                            DirWarehouseID = rem2Parties.DirWarehouseID,
                            DirWarehouseName = rem2Parties.dirWarehouse.DirWarehouseName,
                            ListDocNameRu = rem2Parties.doc.listObject.ListObjectNameRu,
                            PriceVAT = rem2Parties.PriceVAT, //PriceVAT = Math.Round(rem2Parties.PriceVAT, sysSetting.FractionalPartInPrice),
                            PriceCurrency = rem2Parties.PriceCurrency, //PriceCurrency = Math.Round(rem2Parties.PriceCurrency, sysSetting.FractionalPartInPrice),
                            Quantity = rem2Parties.Quantity,
                            Remnant = rem2Parties.Remnant,
                            //Reserve = rem2Parties.Reserve

                            MarkupRetail = ((rem2Parties.PriceRetailVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((rem2Parties.PriceRetailVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceRetailVAT = rem2Parties.PriceRetailVAT - rem2Parties.doc.Discount,
                            PriceRetailCurrency = (rem2Parties.PriceRetailVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((rem2Parties.PriceRetailVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity - rem2Parties.doc.Discount, sysSetting.FractionalPartInPrice),

                            MarkupWholesale = ((rem2Parties.PriceWholesaleVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((rem2Parties.PriceWholesaleVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceWholesaleVAT = rem2Parties.PriceWholesaleVAT - rem2Parties.doc.Discount,
                            PriceWholesaleCurrency = (rem2Parties.PriceWholesaleVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((rem2Parties.PriceWholesaleVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity - rem2Parties.doc.Discount, sysSetting.FractionalPartInPrice),

                            MarkupIM = ((rem2Parties.PriceIMVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((rem2Parties.PriceIMVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100, sysSetting.FractionalPartInPrice),
                            PriceIMVAT = rem2Parties.PriceIMVAT - rem2Parties.doc.Discount,
                            PriceIMCurrency = (rem2Parties.PriceIMVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((rem2Parties.PriceIMVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity - rem2Parties.doc.Discount, sysSetting.FractionalPartInPrice),

                            DirNomenMinimumBalance = rem2Parties.DirNomenMinimumBalance,

                            DirEmployeeName = rem2Parties.doc.dirEmployee.DirEmployeeName,

                            //Причина возврата
                            DirDescriptionID = rem2Parties.DirDescriptionID,
                            DirDescriptionName = rem2Parties.dirDescription.DirDescriptionName,
                            DirReturnTypeID = rem2Parties.DirReturnTypeID,
                            DirReturnTypeName = rem2Parties.dirReturnType.DirReturnTypeName,

                            /*
                            SysGenID = rem2Parties.dirServiceNomen.SysGenID,
                            //SysGenIDPatch = @"UsersTemp/UserImage/" + field.DirCustomersID + "_" + x.SysGenID + ".jpg"
                            SysGenIDPatch = rem2Parties.dirServiceNomen.SysGenID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + rem2Parties.dirServiceNomen.SysGenID + ".jpg"
                            */
                            SysGenIDPatch = "",


                            Exist = 1,
                            ExistName = "Присутствует",
                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Кликнули на товар - паказать список партий

                //if (_params.DocDate > Convert.ToDateTime("2000-01-01")) query = query.Where(x => x.DocDate <= _params.DocDate);
                if (_params.DirContractorIDOrg > 0) query = query.Where(x => x.DirContractorIDOrg == _params.DirContractorIDOrg);
                if(_params.DirServiceNomenID > 0) query = query.Where(x => x.DirServiceNomenID == _params.DirServiceNomenID);//if (_params.DirServiceNomenID > 0) query = query.Where(x => x.DirServiceNomenID == _params.DirServiceNomenID);
                if (_params.DirWarehouseID > 0) query = query.Where(x => x.DirWarehouseID == _params.DirWarehouseID);
                if (_params.DirServiceContractorID > 0) query = query.Where(x => x.DirServiceContractorID == _params.DirServiceContractorID);

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
                        //query = query.Where(x => x.SerialNumber == _params.parSearch || x.Barcode == _params.parSearch || x.DirServiceNomenID == iID_);
                        query = query.Where(x => x.DocSecondHandPurchID == iID_);
                    }
                    else
                    {
                        //query = query.Where(x => x.SerialNumber == _params.parSearch || x.Barcode == _params.parSearch);
                    }
                }


                #endregion


                #endregion


                #region Отправка JSON

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount = query.Count();

                if (_params.collectionWrapper == "DocSecondHandInventoryTab")
                {
                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DocSecondHandInventoryTab = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));
                }
                else
                {
                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        Rem2Party = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));
                }

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/Rem2Party/5
        [ResponseType(typeof(Rem2Party))]
        public async Task<IHttpActionResult> GetRem2Party(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightRem2Parties"));
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
                        from rem2Parties in db.Rem2Parties

                            #region Характеристики
                            /*
                            join dirCharColours1 in db.DirCharColours on rem2Parties.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                            from dirCharColours in dirCharColours2.DefaultIfEmpty()

                            join dirCharMaterials1 in db.DirCharMaterials on rem2Parties.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                            from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                            join dirCharNames1 in db.DirCharNames on rem2Parties.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                            from dirCharNames in dirCharNames2.DefaultIfEmpty()

                            join dirCharSeasons1 in db.DirCharSeasons on rem2Parties.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                            from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                            join dirCharSexes1 in db.DirCharSexes on rem2Parties.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                            from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                            join dirCharSizes1 in db.DirCharSizes on rem2Parties.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                            from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                            join dirCharStyles1 in db.DirCharStyles on rem2Parties.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                            from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                            join dirCharTextures1 in db.DirCharTextures on rem2Parties.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                            from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                            */
                            #endregion

                        where rem2Parties.DirServiceNomenID == id && rem2Parties.doc.ListObjectID == 6 //&& rem2Parties.Remnant == 0

                        select new
                        {
                            Rem2PartyID = rem2Parties.Rem2PartyID,
                            DocDate = rem2Parties.doc.DocDate,
                            DirContractorName = rem2Parties.doc.dirContractor.DirContractorName,

                            #region Характеристики
                            /*
                            DirCharColourID = rem2Parties.DirCharColourID,
                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialID = rem2Parties.DirCharMaterialID,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameID = rem2Parties.DirCharNameID,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonID = rem2Parties.DirCharSeasonID,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexID = rem2Parties.DirCharSexID,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeID = rem2Parties.DirCharSizeID,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleID = rem2Parties.DirCharStyleID,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureID = rem2Parties.DirCharTextureID,
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
                            SerialNumber = rem2Parties.SerialNumber,
                            Barcode = rem2Parties.Barcode,
                            */
                            #endregion

                            DirCurrencyID = rem2Parties.DirCurrencyID,
                            DirCurrencyName = rem2Parties.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = rem2Parties.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = rem2Parties.dirCurrency.DirCurrencyMultiplicity,

                            DirVatValue = rem2Parties.DirVatValue,
                            DirWarehouseName = rem2Parties.dirWarehouse.DirWarehouseName,
                            ListDocNameRu = rem2Parties.doc.listObject.ListObjectNameRu,
                            PriceVAT = rem2Parties.PriceVAT,
                            PriceCurrency = rem2Parties.PriceCurrency,
                            Quantity = rem2Parties.Quantity,
                            Remnant = rem2Parties.Remnant,
                            //Reserve = rem2Parties.Reserve

                            MarkupRetail = Math.Round(((rem2Parties.PriceRetailVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceRetailVAT = rem2Parties.PriceRetailVAT - rem2Parties.doc.Discount,
                            PriceRetailCurrency = Math.Round((rem2Parties.PriceRetailVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity - rem2Parties.doc.Discount, sysSetting.FractionalPartInSum),

                            MarkupWholesale = Math.Round(((rem2Parties.PriceWholesaleVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceWholesaleVAT = rem2Parties.PriceWholesaleVAT - rem2Parties.doc.Discount,
                            PriceWholesaleCurrency = Math.Round((rem2Parties.PriceWholesaleVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity - rem2Parties.doc.Discount, sysSetting.FractionalPartInSum),

                            MarkupIM = Math.Round(((rem2Parties.PriceIMVAT - rem2Parties.PriceVAT) / rem2Parties.PriceVAT) * 100, sysSetting.FractionalPartInSum),
                            PriceIMVAT = rem2Parties.PriceIMVAT - rem2Parties.doc.Discount,
                            PriceIMCurrency = Math.Round((rem2Parties.PriceIMVAT * rem2Parties.dirCurrency.DirCurrencyRate) / rem2Parties.dirCurrency.DirCurrencyMultiplicity - rem2Parties.doc.Discount, sysSetting.FractionalPartInSum),

                            DirNomenMinimumBalance = rem2Parties.DirNomenMinimumBalance
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

        #endregion


        #region UPDATE

        // PUT: api/Rem2Party/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRem2Party(int id, Rem2Party rem2Party)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/Rem2Party
        [ResponseType(typeof(Rem2Party))]
        public async Task<IHttpActionResult> PostRem2Party(Rem2Party rem2Party)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/Rem2Party/5
        [ResponseType(typeof(Rem2Party))]
        public async Task<IHttpActionResult> DeleteRem2Party(int id)
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

        private bool Rem2PartyExists(int id)
        {
            return db.Rem2Parties.Count(e => e.Rem2PartyID == id) > 0;
        }


        #region Save

        //!!! ВАЖНО !!!
        //1. Удаление
        //2. Проверка на отрицательные остатки

        internal void Delete(
            DbConnectionSklad _db,
            Models.Sklad.Rem.Rem2Party[] rem2PartyCollection
            )
        {
            SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = rem2PartyCollection[0].DocID };
            db.Database.ExecuteSqlCommand("DELETE FROM Rem2Parties WHERE DocID=@DocID;", parDocID);
        }

        internal async Task<Models.Sklad.Rem.Rem2Party[]> Save(
            DbConnectionSklad _db,
            Models.Sklad.Rem.Rem2Party[] rem2PartyCollection
            )
        {
            db = _db;

            Delete(_db, rem2PartyCollection);

            //Сохраняем "party" и
            //Меняем полученный ID-шник
            for (int i = 0; i < rem2PartyCollection.Count(); i++)
            {
                //party
                db.Entry(rem2PartyCollection[i]).State = EntityState.Added;
                //await db.SaveChangesAsync();
            }
            await db.SaveChangesAsync();

            return rem2PartyCollection;
        }

        #endregion

        #endregion
    }
}