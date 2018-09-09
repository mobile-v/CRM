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
using System.Data.SQLite;
using System.Web.Script.Serialization;

namespace PartionnyAccount.Controllers.Sklad.Doc.DocSecondHandInventories
{
    public class DocSecondHandRazborTabsController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 6;

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            public int DocSecondHandRazborID;
            public int DocID;
        }
        // GET: api/DocSecondHandRazborTabs
        public async Task<IHttpActionResult> GetDocSecondHandRazborTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRazbors"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Параметры

                Params _params = new Params();

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                _params.DocSecondHandRazborID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandRazborID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docSecondHandRazborTabs in db.DocSecondHandRazborTabs

                        join dirNomens11 in db.DirNomens on docSecondHandRazborTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                            #region Характеристики
                            /*
                            join dirCharColours1 in db.DirCharColours on docSecondHandRazborTabs.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                            from dirCharColours in dirCharColours2.DefaultIfEmpty()

                            join dirCharMaterials1 in db.DirCharMaterials on docSecondHandRazborTabs.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                            from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                            join dirCharNames1 in db.DirCharNames on docSecondHandRazborTabs.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                            from dirCharNames in dirCharNames2.DefaultIfEmpty()

                            join dirCharSeasons1 in db.DirCharSeasons on docSecondHandRazborTabs.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                            from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                            join dirCharSexes1 in db.DirCharSexes on docSecondHandRazborTabs.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                            from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                            join dirCharSizes1 in db.DirCharSizes on docSecondHandRazborTabs.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                            from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                            join dirCharStyles1 in db.DirCharStyles on docSecondHandRazborTabs.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                            from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                            join dirCharTextures1 in db.DirCharTextures on docSecondHandRazborTabs.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                            from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                            */
                            #endregion

                            //Партия
                        join remParties1 in db.RemParties on docSecondHandRazborTabs.DocSecondHandRazborTabID equals remParties1.FieldID into remParties2
                        from remParties in remParties2.Where(x => x.DocID == docSecondHandRazborTabs.docSecondHandRazbor.doc.DocID).DefaultIfEmpty()

                        where docSecondHandRazborTabs.DocSecondHandRazborID == _params.DocSecondHandRazborID

                        #region select

                        select new
                        {
                            //партия
                            RemPartyID = remParties.RemPartyID,

                            DocSecondHandRazborTabID = docSecondHandRazborTabs.DocSecondHandRazborTabID,
                            DocSecondHandRazborID = docSecondHandRazborTabs.DocSecondHandRazborID,
                            DirNomenID = docSecondHandRazborTabs.DirNomenID,

                            //DirNomenName = docSecondHandRazborTabs.dirNomen.DirNomenName,
                            /*
                            DirNomenName =
                            dirNomensSubGroup1.DirNomenName == null ? docSecondHandRazborTabs.dirNomen.DirNomenName
                            :
                            dirNomensSubGroup1.DirNomenName + " / " + docSecondHandRazborTabs.dirNomen.DirNomenName,
                            */

                            //DirNomenName = dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docSecondHandRazborTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docSecondHandRazborTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docSecondHandRazborTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docSecondHandRazborTabs.dirNomen.DirNomenName,


                            Quantity = docSecondHandRazborTabs.Quantity,

                            PriceVAT = docSecondHandRazborTabs.PriceVAT,

                            DirCurrencyID = docSecondHandRazborTabs.DirCurrencyID,
                            DirCurrencyRate = docSecondHandRazborTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSecondHandRazborTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docSecondHandRazborTabs.dirCurrency.DirCurrencyName + " (" + docSecondHandRazborTabs.DirCurrencyRate + ", " + docSecondHandRazborTabs.DirCurrencyMultiplicity + ")",


                            //Характеристики
                            DirCharColourID = docSecondHandRazborTabs.DirCharColourID,
                            DirCharColourName = docSecondHandRazborTabs.dirCharColour.DirCharColourName,
                            DirCharMaterialID = docSecondHandRazborTabs.DirCharMaterialID,
                            DirCharMaterialName = docSecondHandRazborTabs.dirCharMaterial.DirCharMaterialName,
                            DirCharNameID = docSecondHandRazborTabs.DirCharNameID,
                            DirCharNameName = docSecondHandRazborTabs.dirCharName.DirCharNameName,
                            DirCharSeasonID = docSecondHandRazborTabs.DirCharSeasonID,
                            DirCharSeasonName = docSecondHandRazborTabs.dirCharSeason.DirCharSeasonName,
                            DirCharSexID = docSecondHandRazborTabs.DirCharSexID,
                            DirCharSexName = docSecondHandRazborTabs.dirCharSex.DirCharSexName,
                            DirCharSizeID = docSecondHandRazborTabs.DirCharSizeID,
                            DirCharSizeName = docSecondHandRazborTabs.dirCharSize.DirCharSizeName,
                            DirCharStyleID = docSecondHandRazborTabs.DirCharStyleID,
                            DirCharStyleName = docSecondHandRazborTabs.dirCharStyle.DirCharStyleName,
                            DirContractorID = docSecondHandRazborTabs.DirContractorID,
                            DirContractorName = docSecondHandRazborTabs.dirContractor.DirContractorName,
                            DirCharTextureID = docSecondHandRazborTabs.DirCharTextureID,
                            DirCharTextureName = docSecondHandRazborTabs.dirCharTexture.DirCharTextureName,
                            DirChar =
                                docSecondHandRazborTabs.dirCharColour.DirCharColourName + " " +
                                docSecondHandRazborTabs.dirCharMaterial.DirCharMaterialName + " " +
                                docSecondHandRazborTabs.dirCharName.DirCharNameName + " " +
                                docSecondHandRazborTabs.dirCharSeason.DirCharSeasonName + " " +
                                docSecondHandRazborTabs.dirCharSex.DirCharSexName + " " +
                                docSecondHandRazborTabs.dirCharSize.DirCharSizeName + " " +
                                docSecondHandRazborTabs.dirCharStyle.DirCharStyleName + " " +
                                docSecondHandRazborTabs.dirContractor.DirContractorName + " " +
                                docSecondHandRazborTabs.dirCharTexture.DirCharTextureName,
                            SerialNumber = docSecondHandRazborTabs.SerialNumber,
                            Barcode = docSecondHandRazborTabs.Barcode,


                            //Цена в т.в.
                            PriceCurrency = docSecondHandRazborTabs.PriceCurrency,
                            //Себестоимость
                            SUMPurchPriceVATCurrency = docSecondHandRazborTabs.Quantity * docSecondHandRazborTabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docSecondHandRazborTabs.Quantity * docSecondHandRazborTabs.PriceCurrency, sysSetting.FractionalPartInSum),

                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docSecondHandRazborTabs.PriceRetailVAT - docSecondHandRazborTabs.PriceVAT) / docSecondHandRazborTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docSecondHandRazborTabs.PriceRetailVAT - docSecondHandRazborTabs.PriceVAT) / docSecondHandRazborTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docSecondHandRazborTabs.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docSecondHandRazborTabs.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docSecondHandRazborTabs.PriceWholesaleVAT - docSecondHandRazborTabs.PriceVAT) / docSecondHandRazborTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docSecondHandRazborTabs.PriceWholesaleVAT - docSecondHandRazborTabs.PriceVAT) / docSecondHandRazborTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Оптовая цена 
                            PriceWholesaleVAT = docSecondHandRazborTabs.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docSecondHandRazborTabs.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docSecondHandRazborTabs.PriceIMVAT - docSecondHandRazborTabs.PriceVAT) / docSecondHandRazborTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docSecondHandRazborTabs.PriceIMVAT - docSecondHandRazborTabs.PriceVAT) / docSecondHandRazborTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Интернет-Магазин
                            PriceIMVAT = docSecondHandRazborTabs.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docSecondHandRazborTabs.PriceIMCurrency,

                            DirNomenMinimumBalance = docSecondHandRazborTabs.DirNomenMinimumBalance
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSecondHandRazborTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandRazborTabs/5
        [ResponseType(typeof(DocSecondHandRazborTab))]
        public async Task<IHttpActionResult> GetDocSecondHandRazborTab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandRazborTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRazborTab(int id, DocSecondHandRazborTab docSecondHandRazborTab, HttpRequestMessage request)
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
                dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRazbors"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                docSecondHandRazborTab.DocSecondHandRazborID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandRazborID", true) == 0).Value);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Если документ "docSecondHandRazborTab.DocSecondHandRazborID" проведён, то выдать Эксепшн
                Models.Sklad.Doc.DocSecondHandRazbor docSecondHandRazbor = await db.DocSecondHandRazbors.FindAsync(docSecondHandRazborTab.DocSecondHandRazborID);
                //Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandRazbor.DocID);
                if (Convert.ToBoolean(docSecondHandRazbor.doc.Held)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3));

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                //docSecondHandRazborTab.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRazborTab = await Task.Run(() => mPutPostDocSecondHandRazborTab(db, dbRead, sysSetting, docSecondHandRazborTab, EntityState.Modified, field)); //sysSetting
                        ts.Commit(); //.Complete();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = docSecondHandRazborTab.DocSecondHandRazborID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocSecondHandRazborTabID = docSecondHandRazborTab.DocSecondHandRazborTabID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocSecondHandRazborTabs
        [ResponseType(typeof(DocSecondHandRazborTab))]
        public async Task<IHttpActionResult> PostDocSecondHandRazborTab(DocSecondHandRazborTab docSecondHandRazborTab, HttpRequestMessage request)
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
                dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRazbors"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                docSecondHandRazborTab.DocSecondHandRazborID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandRazborID", true) == 0).Value);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Если документ "docSecondHandRazborTab.DocSecondHandRazborID" проведён, то выдать Эксепшн
                Models.Sklad.Doc.DocSecondHandRazbor docSecondHandRazbor = await db.DocSecondHandRazbors.FindAsync(docSecondHandRazborTab.DocSecondHandRazborID);
                //Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandRazbor.DocID);
                if (Convert.ToBoolean(docSecondHandRazbor.doc.Held)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3));

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                //docSecondHandRazborTab.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRazborTab = await Task.Run(() => mPutPostDocSecondHandRazborTab(db, dbRead, sysSetting, docSecondHandRazborTab, EntityState.Added, field)); //sysSetting
                        ts.Commit(); //.Complete();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = docSecondHandRazborTab.DocSecondHandRazborID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocSecondHandRazborTabID = docSecondHandRazborTab.DocSecondHandRazborTabID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandRazborTabs/5
        [ResponseType(typeof(DocSecondHandRazborTab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRazborTab(int id, HttpRequestMessage request)
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
                dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRazbors"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                //var paramList = request.GetQueryNameValuePairs();
                //docSecondHandRazborTab.DocSecondHandRazborID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandRazborID", true) == 0).Value);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Если документ "docSecondHandRazborTab.DocSecondHandRazborID" проведён, то выдать Эксепшн
                Models.Sklad.Doc.DocSecondHandRazborTab docSecondHandRazborTab = await db.DocSecondHandRazborTabs.FindAsync(id);
                //Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandRazbor.DocID);
                if (Convert.ToBoolean(docSecondHandRazborTab.docSecondHandRazbor.doc.Held)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3));

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                //docSecondHandRazborTab.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //db.DocSecondHandRazborTabs.Remove(docSecondHandRazborTab);
                        //await db.SaveChangesAsync();

                        db.Entry(docSecondHandRazborTab).State = EntityState.Deleted;
                        await db.SaveChangesAsync();
                        ts.Commit(); //.Complete();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 5; //Удаление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = docSecondHandRazborTab.DocSecondHandRazborID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = id,
                    Msg = Classes.Language.Sklad.Language.msg19
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
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

        private bool DocSecondHandRazborTabExists(int id)
        {
            return db.DocSecondHandRazborTabs.Count(e => e.DocSecondHandRazborTabID == id) > 0;
        }


        //db, dbRead, sysSetting, docSecondHandRazborTab, EntityState.Added, field
        internal async Task<DocSecondHandRazborTab> mPutPostDocSecondHandRazborTab(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandRazborTab docSecondHandRazborTab,
            EntityState entityState, //EntityState.Added, Modified

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region DocSecondHandRazborTab

            db.Entry(docSecondHandRazborTab).State = entityState;
            await db.SaveChangesAsync();

            #endregion


            #region n. Подтверждение транзакции

            //ts.Commit(); //.Complete();

            #endregion


            return docSecondHandRazborTab;
        }

        #endregion

    }
}