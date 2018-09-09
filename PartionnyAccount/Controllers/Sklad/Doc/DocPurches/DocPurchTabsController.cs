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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocPurches
{
    public class DocPurchTabsController : ApiController
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
            public int DocPurchID;
            public int DocID;
        }
        // GET: api/DocPurchTabs
        public async Task<IHttpActionResult> GetDocPurchTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocPurches"));
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
                _params.DocPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocPurchID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docPurchTabs in db.DocPurchTabs

                        join dirNomens11 in db.DirNomens on docPurchTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики
                        /*
                        join dirCharColours1 in db.DirCharColours on docPurchTabs.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on docPurchTabs.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on docPurchTabs.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on docPurchTabs.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on docPurchTabs.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on docPurchTabs.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on docPurchTabs.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on docPurchTabs.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */
                        #endregion

                        //Партия
                        join remParties1 in db.RemParties on docPurchTabs.DocPurchTabID equals remParties1.FieldID into remParties2
                        from remParties in remParties2.Where(x => x.DocID == docPurchTabs.docPurch.doc.DocID).DefaultIfEmpty()

                        where docPurchTabs.DocPurchID == _params.DocPurchID

                        #region select

                        select new
                        {
                            //партия
                            RemPartyID = remParties.RemPartyID,

                            DocPurchTabID = docPurchTabs.DocPurchTabID,
                            DocPurchID = docPurchTabs.DocPurchID,
                            DirNomenID = docPurchTabs.DirNomenID,

                            //DirNomenName = docPurchTabs.dirNomen.DirNomenName,
                            /*
                            DirNomenName =
                            dirNomensSubGroup1.DirNomenName == null ? docPurchTabs.dirNomen.DirNomenName
                            :
                            dirNomensSubGroup1.DirNomenName + " / " + docPurchTabs.dirNomen.DirNomenName,
                            */

                            //DirNomenName = dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docPurchTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docPurchTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docPurchTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docPurchTabs.dirNomen.DirNomenName,


                            Quantity = docPurchTabs.Quantity,

                            PriceVAT = docPurchTabs.PriceVAT,

                            DirCurrencyID = docPurchTabs.DirCurrencyID,
                            DirCurrencyRate = docPurchTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docPurchTabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docPurchTabs.dirCurrency.DirCurrencyName + " (" + docPurchTabs.DirCurrencyRate + ", " + docPurchTabs.DirCurrencyMultiplicity + ")",


                            //Характеристики
                            DirCharColourID = docPurchTabs.DirCharColourID, DirCharColourName = docPurchTabs.dirCharColour.DirCharColourName,
                            DirCharMaterialID = docPurchTabs.DirCharMaterialID, DirCharMaterialName = docPurchTabs.dirCharMaterial.DirCharMaterialName,
                            DirCharNameID = docPurchTabs.DirCharNameID, DirCharNameName = docPurchTabs.dirCharName.DirCharNameName,
                            DirCharSeasonID = docPurchTabs.DirCharSeasonID, DirCharSeasonName = docPurchTabs.dirCharSeason.DirCharSeasonName,
                            DirCharSexID = docPurchTabs.DirCharSexID, DirCharSexName = docPurchTabs.dirCharSex.DirCharSexName,
                            DirCharSizeID = docPurchTabs.DirCharSizeID, DirCharSizeName = docPurchTabs.dirCharSize.DirCharSizeName,
                            DirCharStyleID = docPurchTabs.DirCharStyleID, DirCharStyleName = docPurchTabs.dirCharStyle.DirCharStyleName,
                            DirContractorID = docPurchTabs.DirContractorID, DirContractorName = docPurchTabs.dirContractor.DirContractorName,
                            DirCharTextureID = docPurchTabs.DirCharTextureID, DirCharTextureName = docPurchTabs.dirCharTexture.DirCharTextureName,
                            DirChar =
                                docPurchTabs.dirCharColour.DirCharColourName + " " +
                                docPurchTabs.dirCharMaterial.DirCharMaterialName + " " +
                                docPurchTabs.dirCharName.DirCharNameName + " " +
                                docPurchTabs.dirCharSeason.DirCharSeasonName + " " +
                                docPurchTabs.dirCharSex.DirCharSexName + " " +
                                docPurchTabs.dirCharSize.DirCharSizeName + " " +
                                docPurchTabs.dirCharStyle.DirCharStyleName + " " +
                                docPurchTabs.dirContractor.DirContractorName + " " +
                                docPurchTabs.dirCharTexture.DirCharTextureName,
                            SerialNumber = docPurchTabs.SerialNumber,
                            Barcode = docPurchTabs.Barcode,


                            //Цена в т.в.
                            PriceCurrency = docPurchTabs.PriceCurrency,
                            //Себестоимость
                            SUMPurchPriceVATCurrency = docPurchTabs.Quantity * docPurchTabs.PriceCurrency == null ? 0
                            : 
                            Math.Round(docPurchTabs.Quantity * docPurchTabs.PriceCurrency, sysSetting.FractionalPartInSum),

                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docPurchTabs.PriceRetailVAT - docPurchTabs.PriceVAT) / docPurchTabs.PriceVAT == null ? 0
                            : 
                            Math.Round(100 * (docPurchTabs.PriceRetailVAT - docPurchTabs.PriceVAT) / docPurchTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docPurchTabs.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docPurchTabs.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docPurchTabs.PriceWholesaleVAT - docPurchTabs.PriceVAT) / docPurchTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docPurchTabs.PriceWholesaleVAT - docPurchTabs.PriceVAT) / docPurchTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Оптовая цена 
                            PriceWholesaleVAT = docPurchTabs.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docPurchTabs.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docPurchTabs.PriceIMVAT - docPurchTabs.PriceVAT) / docPurchTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docPurchTabs.PriceIMVAT - docPurchTabs.PriceVAT) / docPurchTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Интернет-Магазин
                            PriceIMVAT = docPurchTabs.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docPurchTabs.PriceIMCurrency,

                            DirNomenMinimumBalance = docPurchTabs.DirNomenMinimumBalance
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocPurchTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocPurchTabs/5
        [ResponseType(typeof(DocPurchTab))]
        public async Task<IHttpActionResult> GetDocPurchTab(int id, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocPurchTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocPurchTab(int id, DocPurchTab docPurchTab, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocPurches"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                docPurchTab.DocPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocPurchID", true) == 0).Value);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Если документ "docPurchTab.DocPurchID" проведён, то выдать Эксепшн
                Models.Sklad.Doc.DocPurch docPurch = await db.DocPurches.FindAsync(docPurchTab.DocPurchID);
                //Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docPurch.DocID);
                if (Convert.ToBoolean(docPurch.doc.Held)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3));

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                //docPurchTab.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docPurchTab = await Task.Run(() => mPutPostDocPurchTab(db, dbRead, sysSetting, docPurchTab, EntityState.Modified, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docPurchTab.DocPurchID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocPurchTabID = docPurchTab.DocPurchTabID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DocPurchTabs
        [ResponseType(typeof(DocPurchTab))]
        public async Task<IHttpActionResult> PostDocPurchTab(DocPurchTab docPurchTab, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocPurches"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                docPurchTab.DocPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocPurchID", true) == 0).Value);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Если документ "docPurchTab.DocPurchID" проведён, то выдать Эксепшн
                Models.Sklad.Doc.DocPurch docPurch = await db.DocPurches.FindAsync(docPurchTab.DocPurchID);
                //Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docPurch.DocID);
                if (Convert.ToBoolean(docPurch.doc.Held)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3));

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                //docPurchTab.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docPurchTab = await Task.Run(() => mPutPostDocPurchTab(db, dbRead, sysSetting, docPurchTab, EntityState.Added, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docPurchTab.DocPurchID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocPurchTabID = docPurchTab.DocPurchTabID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocPurchTabs/5
        [ResponseType(typeof(DocPurchTab))]
        public async Task<IHttpActionResult> DeleteDocPurchTab(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocPurches"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                //var paramList = request.GetQueryNameValuePairs();
                //docPurchTab.DocPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocPurchID", true) == 0).Value);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Если документ "docPurchTab.DocPurchID" проведён, то выдать Эксепшн
                Models.Sklad.Doc.DocPurchTab docPurchTab = await db.DocPurchTabs.FindAsync(id);
                //Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docPurch.DocID);
                if (Convert.ToBoolean(docPurchTab.docPurch.doc.Held)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3));

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                //docPurchTab.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //db.DocPurchTabs.Remove(docPurchTab);
                        //await db.SaveChangesAsync();

                        db.Entry(docPurchTab).State = EntityState.Deleted;
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
                sysJourDisp.TableFieldID = docPurchTab.DocPurchID;
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

        private bool DocPurchTabExists(int id)
        {
            return db.DocPurchTabs.Count(e => e.DocPurchTabID == id) > 0;
        }


        //db, dbRead, sysSetting, docPurchTab, EntityState.Added, field
        internal async Task<DocPurchTab> mPutPostDocPurchTab(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            Models.Sklad.Sys.SysSetting sysSetting,
            DocPurchTab docPurchTab,
            EntityState entityState, //EntityState.Added, Modified

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {

            #region DocPurchTab

            db.Entry(docPurchTab).State = entityState;
            await db.SaveChangesAsync();

            #endregion


            #region n. Подтверждение транзакции

            //ts.Commit(); //.Complete();

            #endregion


            return docPurchTab;
        }

        #endregion


        #region SQL

        //Сумма документа
        public string GenerateSQLSUM(Models.Sklad.Sys.SysSetting sysSettings, string Tresh1)
        {
            string SQL =
                "SELECT Docs.Discount, " +
                "COUNT(*) CountRecord, " +
                "COUNT(*) CountRecord_NumInWords, " +
                "SUM(DocPurchTabs.Quantity) SumCount, " +

                //Сумма С НДС в текущей валюте
                "round( (SUM( (1 - Docs.Discount / 100) * (DocPurchTabs.Quantity * DocPurchTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * (DocPurchTabs.Quantity * DocPurchTabs.PriceCurrency))), " + sysSettings.FractionalPartInSum + ") 'SumOfVATCurrency_InWords', " + //Приписью

                "round( (SUM( (1 - Docs.Discount / 100) * DocPurchTabs.Quantity * (DocPurchTabs.PriceCurrency - (DocPurchTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency', " +
                "round( (SUM( (1 - Docs.Discount / 100) * DocPurchTabs.Quantity * (DocPurchTabs.PriceCurrency - (DocPurchTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100)))), " + sysSettings.FractionalPartInSum + ") 'SumVATCurrency_InWords', " + //Приписью

                "round( SUM( (1 - Docs.Discount / 100) * (DocPurchTabs.Quantity * DocPurchTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency', " +
                "round( SUM( (1 - Docs.Discount / 100) * (DocPurchTabs.Quantity * DocPurchTabs.PriceCurrency)/(1 + (Docs.DirVatValue) / 100) ), " + sysSettings.FractionalPartInSum + ") 'SumOfNoVatCurrency_InWords', " + //Приписью

                "Docs.DirVatValue 'DirVatValue' " +
                "FROM Docs, DocPurches, DocPurchTabs " +
                "WHERE " +
                "(Docs.DocID=DocPurches.DocID)and" +
                "(DocPurches.DocPurchID=DocPurchTabs.DocPurchID)and(Docs.DocID=@DocID)";
            
            return SQL;
        }

        //Спецификация
        public string GenerateSQLSelectCollection(Models.Sklad.Sys.SysSetting sysSettings, string Tresh1)
        {
            string
                Discount = "(1 - Docs.Discount / 100)", 
                SQL = "";


            SQL =
                "SELECT " +
                //"[Docs].[DocDate] AS [DocDate], " +
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

                //"[DocPurchTabs].[DocPurchTabID] AS [DocPurchTabID], " +
                //"[DocPurchTabs].[DocPurchID] AS [DocPurchID], " +
                "[DocPurchTabs].[DirNomenID] AS [DirNomenID], " +
                "[DocPurchTabs].[Quantity] AS [Quantity], " +
                "[DocPurchTabs].[Quantity] AS [Quantity_NumInWords], " +
                "[DocPurchTabs].[PriceVAT] AS [PriceVAT], " +
                "[DocPurchTabs].[DirCurrencyID] AS [DirCurrencyID], " +
                "[DocPurchTabs].[DirCurrencyRate] AS [DirCurrencyRate], " +
                "[DocPurchTabs].[DirCurrencyMultiplicity] AS [DirCurrencyMultiplicity], " +
                "[DirCurrencies].[DirCurrencyName] || ' (' || [DocPurchTabs].[DirCurrencyRate] || ', ' || [DocPurchTabs].[DirCurrencyMultiplicity] || ')' AS [DirCurrencyName], " +
                //"[DocPurchTabs].[DirCharColourID] AS [DirCharColourID], " +
                "[DirCharColours].[DirCharColourName] AS [DirCharColourName], " +
                //"[DocPurchTabs].[DirCharMaterialID] AS [DirCharMaterialID], " +
                "[DirCharMaterials].[DirCharMaterialName] AS [DirCharMaterialName], " +
                //"[DocPurchTabs].[DirCharNameID] AS [DirCharNameID], " +
                "[DirCharNames].[DirCharNameName] AS [DirCharNameName], " +
                //"[DocPurchTabs].[DirCharSeasonID] AS [DirCharSeasonID], " +
                "[DirCharSeasons].[DirCharSeasonName] AS [DirCharSeasonName], " +
                //"[DocPurchTabs].[DirCharSexID] AS [DirCharSexID], " +
                "[DirCharSexes].[DirCharSexName] AS [DirCharSexName], " +
                //"[DocPurchTabs].[DirCharSizeID] AS [DirCharSizeID], " +
                "[DirCharSizes].[DirCharSizeName] AS [DirCharSizeName], " +
                //"[DocPurchTabs].[DirCharStyleID] AS [DirCharStyleID], " +
                "[DirCharStyles].[DirCharStyleName] AS [DirCharStyleName], " +
                //"[DocPurchTabs].[DirCharTextureID] AS [DirCharTextureID], " +
                "[DirCharTextures].[DirCharTextureName] AS [DirCharTextureName], " +
                "CASE WHEN ([DirCharColours].[DirCharColourName] IS NULL) THEN '' ELSE [DirCharColours].[DirCharColourName] END || ' ' || CASE WHEN ([DirCharMaterials].[DirCharMaterialName] IS NULL) THEN '' ELSE [DirCharMaterials].[DirCharMaterialName] END || ' ' || CASE WHEN ([DirCharNames].[DirCharNameName] IS NULL) THEN '' ELSE [DirCharNames].[DirCharNameName] END || ' ' || CASE WHEN ([DirCharSeasons].[DirCharSeasonName] IS NULL) THEN '' ELSE [DirCharSeasons].[DirCharSeasonName] END || ' ' || CASE WHEN ([DirCharSexes].[DirCharSexName] IS NULL) THEN '' ELSE [DirCharSexes].[DirCharSexName] END || ' ' || CASE WHEN ([DirCharSizes].[DirCharSizeName] IS NULL) THEN '' ELSE [DirCharSizes].[DirCharSizeName] END || ' ' || CASE WHEN ([DirCharStyles].[DirCharStyleName] IS NULL) THEN '' ELSE [DirCharStyles].[DirCharStyleName] END || ' ' || CASE WHEN ([DirCharTextures].[DirCharTextureName] IS NULL) THEN '' ELSE [DirCharTextures].[DirCharTextureName] END AS [DirChar], " +
                "[DocPurchTabs].[SerialNumber] AS [SerialNumber], " +
                "[DocPurchTabs].[Barcode] AS [Barcode], " +
                "[DocPurchTabs].[Barcode] AS [BarcodeImage], " +

                "[DirContractorOrg].[DirContractorName] AS [DirContractorNameOrg], " +

                //Приходная цена
                "[DocPurchTabs].[PriceCurrency] AS [PriceCurrency], " +
                //"Цена без НДС" в валюте
                "ROUND([DocPurchTabs].[PriceVAT] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVAT], " +
                //"Цена без НДС" в текущей валюте
                "ROUND([DocPurchTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [PurchPriceNoVATCurrency], " +
                //Цена С НДС в валюте (словами)
                "ROUND([DocPurchTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [PriceVAT_InWords], " +
                //Цена С НДС в текущей валюте (словами)
                "ROUND([DocPurchTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [PriceCurrency_InWords], " +
                //"Цена с НДС" в текущей валюте со Скидкой
                "ROUND([DocPurchTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'PriceCurrencyDiscount', " +  
                //"Сумма НДС" (НДС документа)
                "ROUND([DocPurchTabs].[Quantity] * ([DocPurchTabs].[PriceCurrency] / (1 + [Docs].[DirVatValue] / 100) * [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SumVatValue], " +
                //"Стоимость без НДС" в валюте
                "ROUND(([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceVAT]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVAT], " +
                //"Стоимость Прихода без НДС" в текущей валюте
                "ROUND(([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceCurrency]) / (1 + [Docs].[DirVatValue] / 100), " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceNoVATCurrency], " +
                //Себестоимось прихода
                "ROUND([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceVat], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVAT], " +
                "ROUND([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMPurchPriceVATCurrency], " +
                //Стоимость с НДС в текущей валюте со Скидкой
                "ROUND([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceCurrency] * " + Discount + ", " + sysSettings.FractionalPartInSum + ") 'SUMPriceCurrencyDiscount', " + 


                //Розница
                "ROUND((100 * ([DocPurchTabs].[PriceRetailVAT] - [DocPurchTabs].[PriceVAT])) / [DocPurchTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupRetail], " +
                "[DocPurchTabs].[PriceRetailVAT] AS [PriceRetailVAT], " +
                "[DocPurchTabs].[PriceRetailCurrency] AS [PriceRetailCurrency], " +
                //Стоимость Розницы в валюте
                "ROUND([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceRetailVAT], " + sysSettings.FractionalPartInSum + ") AS [SUMRetailPriceVAT], " +
                //Стоимость Розницы в текущей валюте
                "ROUND([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceRetailCurrency], " + sysSettings.FractionalPartInSum + ") 'SUMRetailPriceCurrency', " + 

                //Опт
                "ROUND((100 * ([DocPurchTabs].[PriceWholesaleVAT] - [DocPurchTabs].[PriceVAT])) / [DocPurchTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupWholesale], " +
                "[DocPurchTabs].[PriceWholesaleVAT] AS [PriceWholesaleVAT], " +
                "[DocPurchTabs].[PriceWholesaleCurrency] AS [PriceWholesaleCurrency], " +
                //Стоимость Опта в валюте
                "ROUND([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceWholesaleVat], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceVAT], " +
                //Стоимость Опта в текущей валюте
                "ROUND([DocPurchTabs].[Quantity] * [DocPurchTabs].[PriceWholesaleCurrency], " + sysSettings.FractionalPartInSum + ") AS [SUMWholesalePriceCurrency], " + 

                //Интернет-Магазин
                "ROUND((100 * ([DocPurchTabs].[PriceIMVAT] - [DocPurchTabs].[PriceVAT])) / [DocPurchTabs].[PriceVAT], " + sysSettings.FractionalPartInSum + ") AS [MarkupIM], " +
                "[DocPurchTabs].[PriceIMVAT] AS [PriceIMVAT], " +
                "[DocPurchTabs].[PriceIMCurrency] AS [PriceIMCurrency], " +

                //Прочерк
                "'-' AS FieldDash, " +
                //Пустое поле
                "' ' AS FieldEmpty " +

                "FROM [Docs], [DocPurches], [DocPurchTabs] " +

                "LEFT OUTER JOIN [DirCharColours] ON [DocPurchTabs].[DirCharColourID] = [DirCharColours].[DirCharColourID] " +
                "LEFT OUTER JOIN [DirCharMaterials] ON [DocPurchTabs].[DirCharMaterialID] = [DirCharMaterials].[DirCharMaterialID] " +
                "LEFT OUTER JOIN [DirCharNames] ON [DocPurchTabs].[DirCharNameID] = [DirCharNames].[DirCharNameID] " +
                "LEFT OUTER JOIN [DirCharSeasons] ON [DocPurchTabs].[DirCharSeasonID] = [DirCharSeasons].[DirCharSeasonID] " +
                "LEFT OUTER JOIN [DirCharSexes] ON [DocPurchTabs].[DirCharSexID] = [DirCharSexes].[DirCharSexID] " +
                "LEFT OUTER JOIN [DirCharSizes] ON [DocPurchTabs].[DirCharSizeID] = [DirCharSizes].[DirCharSizeID] " +
                "LEFT OUTER JOIN [DirCharStyles] ON [DocPurchTabs].[DirCharStyleID] = [DirCharStyles].[DirCharStyleID] " +
                "LEFT OUTER JOIN [DirCharTextures] ON [DocPurchTabs].[DirCharTextureID] = [DirCharTextures].[DirCharTextureID] " +
                //"INNER JOIN [DirNomens] ON [DocPurchTabs].[DirNomenID] = [DirNomens].[DirNomenID] " +
                //"LEFT OUTER JOIN [DirNomens] AS [DirNomenGroups] ON [DirNomenGroups].[Sub] = [DirNomens].[DirNomenID] " +
                "INNER JOIN [DirCurrencies] ON [DocPurchTabs].[DirCurrencyID] = [DirCurrencies].[DirCurrencyID] " +
                "INNER JOIN [DirContractors] AS [DirContractorOrg] ON [Docs].[DirContractorIDOrg] = [DirContractorOrg].[DirContractorID] " +

                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
                "LEFT JOIN [DirNomens] AS [DirNomens] ON [DirNomens].[DirNomenID] = [DocPurchTabs].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensGroup] ON [DirNomens].[Sub] = [dirNomensGroup].[DirNomenID] " +
                "LEFT JOIN [DirNomens] AS [dirNomensSubGroup] ON [dirNomensGroup].[Sub] = [dirNomensSubGroup].[DirNomenID] " +
                //Товар *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***


                "WHERE ([Docs].[DocID]=[DocPurches].[DocID])and([DocPurches].[DocPurchID]=[DocPurchTabs].[DocPurchID])and(Docs.DocID=@DocID) " + 

                "";


            return SQL;
        }

        #endregion
    }
}