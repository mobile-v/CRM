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
    public class DocSecondHandRazbor2TabsController : ApiController
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
        Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 6;

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            public int DocSecondHandPurchID;
            public int DocID;
        }
        // GET: api/DocSecondHandRazbor2Tabs
        public async Task<IHttpActionResult> GetDocSecondHandRazbor2Tabs(HttpRequestMessage request)
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
                _params.DocSecondHandPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandPurchID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docSecondHandRazbor2Tabs in db.DocSecondHandRazbor2Tabs

                        join dirNomens11 in db.DirNomens on docSecondHandRazbor2Tabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        where docSecondHandRazbor2Tabs.DocSecondHandPurchID == _params.DocSecondHandPurchID

                        #region select

                        select new
                        {
                            //партия
                            //RemPartyID = remParties.RemPartyID,
                            //DocSecondHandPurchID = docSecondHandRazbor2Tabs.DocSecondHandPurchID,

                            DocSecondHandRazbor2TabID = docSecondHandRazbor2Tabs.DocSecondHandRazbor2TabID,
                            DocSecondHandPurchID = docSecondHandRazbor2Tabs.DocSecondHandPurchID,
                            DirNomenID = docSecondHandRazbor2Tabs.DirNomenID,

                            //DirNomenName = docSecondHandRazbor2Tabs.dirNomen.DirNomenName,
                            /*
                            DirNomenName =
                            dirNomensSubGroup1.DirNomenName == null ? docSecondHandRazbor2Tabs.dirNomen.DirNomenName
                            :
                            dirNomensSubGroup1.DirNomenName + " / " + docSecondHandRazbor2Tabs.dirNomen.DirNomenName,
                            */

                            //DirNomenName = dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docSecondHandRazbor2Tabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docSecondHandRazbor2Tabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docSecondHandRazbor2Tabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docSecondHandRazbor2Tabs.dirNomen.DirNomenName,


                            Quantity = docSecondHandRazbor2Tabs.Quantity,

                            PriceVAT = docSecondHandRazbor2Tabs.PriceVAT,

                            DirCurrencyID = docSecondHandRazbor2Tabs.DirCurrencyID,
                            DirCurrencyRate = docSecondHandRazbor2Tabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docSecondHandRazbor2Tabs.DirCurrencyMultiplicity,
                            DirCurrencyName = docSecondHandRazbor2Tabs.dirCurrency.DirCurrencyName + " (" + docSecondHandRazbor2Tabs.DirCurrencyRate + ", " + docSecondHandRazbor2Tabs.DirCurrencyMultiplicity + ")",


                            //Характеристики
                            DirCharColourID = docSecondHandRazbor2Tabs.DirCharColourID,
                            DirCharColourName = docSecondHandRazbor2Tabs.dirCharColour.DirCharColourName,
                            DirCharMaterialID = docSecondHandRazbor2Tabs.DirCharMaterialID,
                            DirCharMaterialName = docSecondHandRazbor2Tabs.dirCharMaterial.DirCharMaterialName,
                            DirCharNameID = docSecondHandRazbor2Tabs.DirCharNameID,
                            DirCharNameName = docSecondHandRazbor2Tabs.dirCharName.DirCharNameName,
                            DirCharSeasonID = docSecondHandRazbor2Tabs.DirCharSeasonID,
                            DirCharSeasonName = docSecondHandRazbor2Tabs.dirCharSeason.DirCharSeasonName,
                            DirCharSexID = docSecondHandRazbor2Tabs.DirCharSexID,
                            DirCharSexName = docSecondHandRazbor2Tabs.dirCharSex.DirCharSexName,
                            DirCharSizeID = docSecondHandRazbor2Tabs.DirCharSizeID,
                            DirCharSizeName = docSecondHandRazbor2Tabs.dirCharSize.DirCharSizeName,
                            DirCharStyleID = docSecondHandRazbor2Tabs.DirCharStyleID,
                            DirCharStyleName = docSecondHandRazbor2Tabs.dirCharStyle.DirCharStyleName,
                            DirContractorID = docSecondHandRazbor2Tabs.DirContractorID,
                            DirContractorName = docSecondHandRazbor2Tabs.dirContractor.DirContractorName,
                            DirCharTextureID = docSecondHandRazbor2Tabs.DirCharTextureID,
                            DirCharTextureName = docSecondHandRazbor2Tabs.dirCharTexture.DirCharTextureName,
                            DirChar =
                                docSecondHandRazbor2Tabs.dirCharColour.DirCharColourName + " " +
                                docSecondHandRazbor2Tabs.dirCharMaterial.DirCharMaterialName + " " +
                                docSecondHandRazbor2Tabs.dirCharName.DirCharNameName + " " +
                                docSecondHandRazbor2Tabs.dirCharSeason.DirCharSeasonName + " " +
                                docSecondHandRazbor2Tabs.dirCharSex.DirCharSexName + " " +
                                docSecondHandRazbor2Tabs.dirCharSize.DirCharSizeName + " " +
                                docSecondHandRazbor2Tabs.dirCharStyle.DirCharStyleName + " " +
                                docSecondHandRazbor2Tabs.dirContractor.DirContractorName + " " +
                                docSecondHandRazbor2Tabs.dirCharTexture.DirCharTextureName,
                            SerialNumber = docSecondHandRazbor2Tabs.SerialNumber,
                            Barcode = docSecondHandRazbor2Tabs.Barcode,


                            //Цена в т.в.
                            PriceCurrency = docSecondHandRazbor2Tabs.PriceCurrency,
                            //Себестоимость
                            SUMPurchPriceVATCurrency = docSecondHandRazbor2Tabs.Quantity * docSecondHandRazbor2Tabs.PriceCurrency == null ? 0
                            :
                            Math.Round(docSecondHandRazbor2Tabs.Quantity * docSecondHandRazbor2Tabs.PriceCurrency, sysSetting.FractionalPartInSum),

                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docSecondHandRazbor2Tabs.PriceRetailVAT - docSecondHandRazbor2Tabs.PriceVAT) / docSecondHandRazbor2Tabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docSecondHandRazbor2Tabs.PriceRetailVAT - docSecondHandRazbor2Tabs.PriceVAT) / docSecondHandRazbor2Tabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Розничная цена
                            PriceRetailVAT = docSecondHandRazbor2Tabs.PriceRetailVAT,
                            //Розничная цена в текущей валюте
                            PriceRetailCurrency = docSecondHandRazbor2Tabs.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docSecondHandRazbor2Tabs.PriceWholesaleVAT - docSecondHandRazbor2Tabs.PriceVAT) / docSecondHandRazbor2Tabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docSecondHandRazbor2Tabs.PriceWholesaleVAT - docSecondHandRazbor2Tabs.PriceVAT) / docSecondHandRazbor2Tabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Оптовая цена 
                            PriceWholesaleVAT = docSecondHandRazbor2Tabs.PriceWholesaleVAT,
                            //Оптовая цена в текущей валюте
                            PriceWholesaleCurrency = docSecondHandRazbor2Tabs.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docSecondHandRazbor2Tabs.PriceIMVAT - docSecondHandRazbor2Tabs.PriceVAT) / docSecondHandRazbor2Tabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docSecondHandRazbor2Tabs.PriceIMVAT - docSecondHandRazbor2Tabs.PriceVAT) / docSecondHandRazbor2Tabs.PriceVAT, sysSetting.FractionalPartInSum),
                            //Интернет-Магазин
                            PriceIMVAT = docSecondHandRazbor2Tabs.PriceIMVAT,
                            //Интернет-Магазин в текущей валюте
                            PriceIMCurrency = docSecondHandRazbor2Tabs.PriceIMCurrency,

                            DirNomenMinimumBalance = docSecondHandRazbor2Tabs.DirNomenMinimumBalance
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocSecondHandRazbor2Tab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocSecondHandRazbor2Tabs/5
        [ResponseType(typeof(DocSecondHandRazbor2Tab))]
        public async Task<IHttpActionResult> GetDocSecondHandRazbor2Tab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandRazbor2Tabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandRazbor2Tab(int id, DocSecondHandRazbor2Tab docSecondHandRazbor2Tab, HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandRazbor2Tabs
        [ResponseType(typeof(DocSecondHandRazbor2Tab))]
        public async Task<IHttpActionResult> PostDocSecondHandRazbor2Tab(DocSecondHandRazbor2Tab docSecondHandRazbor2Tab, HttpRequestMessage request)
        {
            //Алгоритм
            //1. DirNomens (ищим: находим (ничего), не находим (новый)) и заполняем DocSecondHandRazbor2Tabs.DirNomenID
            //2. DocSecondHandRazbor2Tabs (сохраняем)
            //3. Партии (создаём партию)

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
                docSecondHandRazbor2Tab.DocSecondHandPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandPurchID", true) == 0).Value);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                //Если документ "docSecondHandRazbor2Tab.DocSecondHandPurchID" проведён, то выдать Эксепшн
                /*
                Models.Sklad.Doc.DocSecondHandRazbor docSecondHandRazbor = await db.DocSecondHandRazbors.FindAsync(docSecondHandRazbor2Tab.DocSecondHandPurchID);
                //Models.Sklad.Doc.Doc doc = await db.Docs.FindAsync(docSecondHandRazbor.DocID);
                if (Convert.ToBoolean(docSecondHandRazbor.doc.Held)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3));
                */
                Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandRazbor2Tab.DocSecondHandPurchID);
                if (docSecondHandPurch == null)
                {
                    return Ok(returnServer.Return(false, "Документ (аппарат) не найден №" + docSecondHandRazbor2Tab.DocSecondHandPurchID));
                }
                else
                {
                    if (docSecondHandPurch.DirSecondHandStatusID != 12)
                    {
                        return Ok(returnServer.Return(false, "Для добавление запчастей, статус документа должен быть '12.В разборе'. А он " + docSecondHandPurch.DirSecondHandStatusID + "."+ docSecondHandPurch.dirSecondHandStatus.DirSecondHandStatusName));
                    }
                }

                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                //docSecondHandRazbor2Tab.Substitute();

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandRazbor2Tab = await Task.Run(() => mPutPostDocSecondHandRazbor2Tab(db, dbRead, sysSetting, docSecondHandRazbor2Tab, EntityState.Added, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandRazbor2Tab.DocSecondHandPurchID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocSecondHandRazbor2TabID = docSecondHandRazbor2Tab.DocSecondHandRazbor2TabID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandRazbor2Tabs/5
        [ResponseType(typeof(DocSecondHandRazbor2Tab))]
        public async Task<IHttpActionResult> DeleteDocSecondHandRazbor2Tab(int id, HttpRequestMessage request)
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
                //docSecondHandRazbor2Tab.DocSecondHandPurchID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSecondHandPurchID", true) == 0).Value);

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                Models.Sklad.Doc.DocSecondHandRazbor2Tab docSecondHandRazbor2Tab = await db.DocSecondHandRazbor2Tabs.FindAsync(id);
                int? DocSecondHandPurchID = docSecondHandRazbor2Tab.DocSecondHandPurchID; //Не трогать !!!
                string DirNomenName = docSecondHandRazbor2Tab.dirNomen.DirNomenName;

                //1. Если проведён
                //if (Convert.ToBoolean(docSecondHandRazbor2Tab.docSecondHandPurch.doc.Held)) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_3));
                //2. Если разобран (надо поменять статус)
                if (Convert.ToBoolean(docSecondHandRazbor2Tab.docSecondHandPurch.DirSecondHandStatusID != 12)) return Ok(returnServer.Return(false, "Для удаления запчасти (партии) статус аппарата должен быть 'В разборе'!"));
                //3. Есть ли остаток
                int? DocID = docSecondHandRazbor2Tab.docSecondHandPurch.DocID;
                var queryRemParties = await
                    (
                        from x in db.RemParties
                        where x.DocID == DocID && x.FieldID == docSecondHandRazbor2Tab.DocSecondHandRazbor2TabID
                        select x
                    ).ToListAsync();
                if (queryRemParties.Count() > 0)
                {
                    if (queryRemParties[0].Remnant < docSecondHandRazbor2Tab.Quantity) return Ok(returnServer.Return(false, "Партия уже продана! К-во на остатке: " + queryRemParties[0].Remnant));
                }

                #endregion


                #region Сохранение

                //using (TransactionScope ts = new TransactionScope())
                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        #region 1. DocSecondHandRazbor2Tabs

                        db.Entry(docSecondHandRazbor2Tab).State = EntityState.Deleted;
                        await db.SaveChangesAsync();

                        #endregion


                        #region 2. RemParties

                        int? RemPartyID = queryRemParties[0].RemPartyID;
                        Models.Sklad.Rem.RemParty remParty = await db.RemParties.FindAsync(RemPartyID);

                        db.Entry(remParty).State = EntityState.Deleted;
                        await db.SaveChangesAsync();

                        #endregion


                        #region 3. Лог: Пишем в Лог о смене статуса и мастера, если такое было

                        logService.DocSecondHandPurchID = DocSecondHandPurchID;
                        logService.DirSecondHandLogTypeID = 6; //Смена статуса
                        logService.DirEmployeeID = field.DirEmployeeID;
                        //logService.DirSecondHandStatusID = DirSecondHandStatusID;
                        logService.Msg = "Разборка: удалили запчасть: " + DirNomenName;

                        await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

                        #endregion


                        ts.Commit();

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
                sysJourDisp.TableFieldID = docSecondHandRazbor2Tab.DocSecondHandPurchID;
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

        private bool DocSecondHandRazbor2TabExists(int id)
        {
            return db.DocSecondHandRazbor2Tabs.Count(e => e.DocSecondHandRazbor2TabID == id) > 0;
        }


        //db, dbRead, sysSetting, docSecondHandRazbor2Tab, EntityState.Added, field
        internal async Task<DocSecondHandRazbor2Tab> mPutPostDocSecondHandRazbor2Tab(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandRazbor2Tab docSecondHandRazbor2Tab,
            EntityState entityState, //EntityState.Added, Modified

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            //Алгоритм
            //1. DirNomens (ищим: находим (ничего), не находим (новый)) и заполняем DocSecondHandRazbor2Tabs.DirNomenID
            //2. DocSecondHandRazbor2Tabs (сохраняем)
            //3. RemParties (создаём партию)


            #region 1. DirNomens
            //Иищим: находим (ничего), не находим (новый) 
            //и заполняем DocSecondHandRazbor2Tabs.DirNomenID

            #region  Категория товара

            if (docSecondHandRazbor2Tab.DirNomenCategoryID == null)
            {
                var queryDirNomenCategoryID = await
                    (
                        from x in db.DirNomenCategories
                        where x.DirNomenCategoryName == docSecondHandRazbor2Tab.DirNomenCategoryName
                        select x
                    ).ToListAsync();
                if (queryDirNomenCategoryID.Count() > 0)
                {
                    docSecondHandRazbor2Tab.DirNomenCategoryID = queryDirNomenCategoryID[0].DirNomenCategoryID;
                    docSecondHandRazbor2Tab.DirNomenCategoryName = queryDirNomenCategoryID[0].DirNomenCategoryName;
                }
                else
                {
                    Models.Sklad.Dir.DirNomenCategory dirNomenCategory = new Models.Sklad.Dir.DirNomenCategory();
                    dirNomenCategory.DirNomenCategoryName = docSecondHandRazbor2Tab.DirNomenCategoryName;

                    db.Entry(dirNomenCategory).State = EntityState.Added;
                    await db.SaveChangesAsync();

                    docSecondHandRazbor2Tab.DirNomenCategoryID = dirNomenCategory.DirNomenCategoryID;
                    docSecondHandRazbor2Tab.DirNomenCategoryName = dirNomenCategory.DirNomenCategoryName;
                }
            }
            else
            {
                Models.Sklad.Dir.DirNomenCategory dirNomenCategory = await db.DirNomenCategories.FindAsync(docSecondHandRazbor2Tab.DirNomenCategoryID);
                docSecondHandRazbor2Tab.DirNomenCategoryName = dirNomenCategory.DirNomenCategoryName;
            }

            #endregion


            var query = await
                (
                    from x in db.DirNomens
                    where x.Sub == docSecondHandRazbor2Tab.DirNomen2ID && x.DirNomenCategoryID == docSecondHandRazbor2Tab.DirNomenCategoryID
                    select x
                ).ToListAsync();
            if (query.Count() > 0)
            {
                docSecondHandRazbor2Tab.DirNomenID = query[0].DirNomenID;
            }
            else
            {
                Models.Sklad.Dir.DirNomen dirNomen = new Models.Sklad.Dir.DirNomen();
                dirNomen.Sub = docSecondHandRazbor2Tab.DirNomen2ID;
                dirNomen.DirNomenTypeID = 1;
                dirNomen.DirNomenName = docSecondHandRazbor2Tab.DirNomenCategoryName;
                dirNomen.DirNomenCategoryID = docSecondHandRazbor2Tab.DirNomenCategoryID;
                dirNomen.DirNomenNameFull= docSecondHandRazbor2Tab.DirNomenCategoryName;

                db.Entry(dirNomen).State = EntityState.Added;
                await db.SaveChangesAsync();

                docSecondHandRazbor2Tab.DirNomenID = dirNomen.DirNomenID;
            }

            #endregion


            #region 2. DocSecondHandRazbor2Tabs

            db.Entry(docSecondHandRazbor2Tab).State = entityState;
            await db.SaveChangesAsync();

            #endregion


            #region 3. RemParties

            Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandRazbor2Tab.DocSecondHandPurchID);


            Models.Sklad.Rem.RemParty remParty = new Models.Sklad.Rem.RemParty();
            remParty.RemPartyID = null;
            remParty.DirNomenID = Convert.ToInt32(docSecondHandRazbor2Tab.DirNomenID);
            remParty.Quantity = docSecondHandRazbor2Tab.Quantity;
            remParty.Remnant = docSecondHandRazbor2Tab.Quantity;
            remParty.DirCurrencyID = docSecondHandRazbor2Tab.DirCurrencyID;
            //remParty.DirCurrencyMultiplicity = docSecondHandRazbor2Tab.DirCurrencyMultiplicity;
            //remParty.DirCurrencyRate = docSecondHandRazbor2Tab.DirCurrencyRate;
            remParty.DirVatValue = 0; // docPurch.DirVatValue;
            remParty.DirWarehouseID = docSecondHandPurch.DirWarehouseID; // docPurch.DirWarehouseID;
            remParty.DirWarehouseIDDebit = docSecondHandPurch.DirWarehouseID; // docPurch.DirWarehouseID;
            remParty.DirWarehouseIDPurch = Convert.ToInt32(docSecondHandPurch.DirWarehouseIDPurches); // docPurch.DirWarehouseID;
            remParty.DirContractorIDOrg = docSecondHandPurch.doc.DirContractorIDOrg;

            //!!! Важно !!!
            //if (docSecondHandRazbor2Tab.DirContractorID != null) remParty.DirContractorID = Convert.ToInt32(docSecondHandRazbor2Tab.DirContractorID);
            //else remParty.DirContractorID = docSecondHandPurch.DirContractorID;
            remParty.DirContractorID = docSecondHandPurch.doc.DirContractorIDOrg;
            //!!! Важно !!!

            //Дата Приёмки товара
            remParty.DocDatePurches = docSecondHandPurch.doc.DocDate;

            remParty.DirCharColourID = docSecondHandRazbor2Tab.DirCharColourID;
            remParty.DirCharMaterialID = docSecondHandRazbor2Tab.DirCharMaterialID;
            remParty.DirCharNameID = docSecondHandRazbor2Tab.DirCharNameID;
            remParty.DirCharSeasonID = docSecondHandRazbor2Tab.DirCharSeasonID;
            remParty.DirCharSexID = docSecondHandRazbor2Tab.DirCharSexID;
            remParty.DirCharSizeID = docSecondHandRazbor2Tab.DirCharSizeID;
            remParty.DirCharStyleID = docSecondHandRazbor2Tab.DirCharStyleID;
            remParty.DirCharTextureID = docSecondHandRazbor2Tab.DirCharTextureID;

            remParty.SerialNumber = docSecondHandRazbor2Tab.SerialNumber;
            remParty.Barcode = docSecondHandRazbor2Tab.Barcode;

            remParty.DocID = Convert.ToInt32(docSecondHandPurch.DocID);
            remParty.PriceCurrency = docSecondHandRazbor2Tab.PriceCurrency;
            remParty.PriceVAT = docSecondHandRazbor2Tab.PriceVAT;
            remParty.FieldID = Convert.ToInt32(docSecondHandRazbor2Tab.DocSecondHandRazbor2TabID);

            remParty.PriceRetailVAT = docSecondHandRazbor2Tab.PriceRetailVAT;
            remParty.PriceRetailCurrency = docSecondHandRazbor2Tab.PriceRetailCurrency;
            remParty.PriceWholesaleVAT = docSecondHandRazbor2Tab.PriceWholesaleVAT;
            remParty.PriceWholesaleCurrency = docSecondHandRazbor2Tab.PriceWholesaleCurrency;
            remParty.PriceIMVAT = docSecondHandRazbor2Tab.PriceIMVAT;
            remParty.PriceIMCurrency = docSecondHandRazbor2Tab.PriceIMCurrency;

            //DirNomenMinimumBalance
            remParty.DirNomenMinimumBalance = sysSetting.DirNomenMinimumBalance;

            remParty.DirEmployeeID = docSecondHandPurch.doc.DirEmployeeID;
            remParty.DocDate = docSecondHandPurch.doc.DocDate;


            db.Entry(remParty).State = EntityState.Added;
            await db.SaveChangesAsync();


            #endregion


            #region 3. Лог: Пишем в Лог о смене статуса и мастера, если такое было

            logService.DocSecondHandPurchID = docSecondHandRazbor2Tab.DocSecondHandPurchID;
            logService.DirSecondHandLogTypeID = 6; //Смена статуса
            logService.DirEmployeeID = field.DirEmployeeID;
            //logService.DirSecondHandStatusID = DirSecondHandStatusID;
            logService.Msg = "Разборка: добавили запчасть: " + docSecondHandRazbor2Tab.dirNomen.DirNomenName;

            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

            #endregion


            return docSecondHandRazbor2Tab;
        }

        #endregion

    }
}
