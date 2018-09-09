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
    public class DocSecondHandReturnsController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        Models.Sklad.Log.LogSecondHand logService = new Models.Sklad.Log.LogSecondHand(); Controllers.Sklad.Log.LogSecondHandsController logServicesController = new Log.LogSecondHandsController();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();

        int ListObjectID = 67;

        #endregion


        #region SELECT

        class Params
        {
            //Parameters
            //public int DocID;
            public int? DocSecondHandReturnID;
            public DateTime? DateS, DatePo;
            public int DirWarehouseID;
        }
        // GET: api/DocSecondHandReturns
        public async Task<IHttpActionResult> GetDocSecondHandReturns(HttpRequestMessage request)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // GET: api/DocSecondHandReturns/5
        [ResponseType(typeof(DocSecondHandReturn))]
        public async Task<IHttpActionResult> GetDocSecondHandReturn(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocSecondHandReturns/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocSecondHandReturn(int id, DocSecondHandReturn docSecondHandReturn)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocSecondHandReturns
        [ResponseType(typeof(DocSecondHandReturn))]
        public async Task<IHttpActionResult> PostDocSecondHandReturn(DocSecondHandReturn docSecondHandReturn, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocSecondHandRetailReturns"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Скидка: разрешена или нет
                if (docSecondHandReturn.Discount > 0)
                {
                    iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocDescription"));
                    if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57_5));
                }

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры

                //save, save_close, held, held_cancel
                var paramList = request.GetQueryNameValuePairs();
                string UO_Action = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "UO_Action", true) == 0).Value;
                if (UO_Action == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg101));
                UO_Action = UO_Action.ToLower();

                //Получаем колекцию "Спецификации"
                /*Models.Sklad.Doc.DocSecondHandReturnTab[] docSecondHandReturnTabCollection = null;
                if (!String.IsNullOrEmpty(docSecondHandReturn.recordsDocSecondHandReturnTab) && UO_Action != "held_cancel")
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    docSecondHandReturnTabCollection = serializer.Deserialize<Models.Sklad.Doc.DocSecondHandReturnTab[]>(docSecondHandReturn.recordsDocSecondHandReturnTab);
                }*/

                #endregion

                #region Проверки

                if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

                /*
                //Получаем "docSecondHandReturn.DocID" из БД, если он отличается от пришедшего от клиента "docSecondHandReturn.DocID" выдаём ошибку
                //Были проблемы, кодга на один "DocID" числилось 2-а документа, а то и больше
                var query = await Task.Run(() =>
                    (
                        from x in dbRead.DocSecondHandReturns
                        where x.DocSecondHandReturnID == docSecondHandReturn.DocSecondHandReturnID
                        select x
                    ).ToListAsync());

                if (query.Count() > 0)
                    if (query[0].DocID != docSecondHandReturn.DocID)
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

                //dbRead.Database.Connection.Close();
                */


                //Проверка "Скидки"
                //1. Получаем сотурдника с правами



                //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
                docSecondHandReturn.Substitute();

                #endregion


                #region Сохранение

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        docSecondHandReturn = await Task.Run(() => mPutPostDocSecondHandReturn(db, dbRead, UO_Action, docSecondHandReturn, EntityState.Added, field)); //sysSetting
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
                sysJourDisp.TableFieldID = docSecondHandReturn.DocSecondHandReturnID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    DocID = docSecondHandReturn.DocID,
                    DocSecondHandReturnID = docSecondHandReturn.DocSecondHandReturnID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // DELETE: api/DocSecondHandReturns/5
        [ResponseType(typeof(DocSecondHandReturn))]
        public async Task<IHttpActionResult> DeleteDocSecondHandReturn(int id)
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

        private bool DocSecondHandReturnExists(int id)
        {
            return db.DocSecondHandReturns.Count(e => e.DocSecondHandReturnID == id) > 0;
        }



        //Алгоритм:
        //1. INSERT в "DocSecondHandReturns"
        //2. UPDATE Status в "DocSecondHandPurches"

        internal async Task<DocSecondHandReturn> mPutPostDocSecondHandReturn(
            DbConnectionSklad db,
            DbConnectionSklad dbRead,
            string UO_Action,
            //DbConnectionSklad dbRead,
            //Models.Sklad.Sys.SysSetting sysSetting,
            DocSecondHandReturn docSecondHandReturn,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
                                     //Models.Sklad.Doc.DocSecondHandReturnTab[] docSecondHandReturnTabCollection,

            Classes.Account.Login.Field field //Дополнительные данные о сотруднике
            )
        {
            #region Проверка статуса аппарата, только если:
            //DirSecondHandStatusID == 9
            //DirSecondHandStatusID_789 == 7

            Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch = await db.DocSecondHandPurches.FindAsync(docSecondHandReturn.DocSecondHandPurchID);
            if (docSecondHandPurch.DirSecondHandStatusID != 10 || docSecondHandPurch.DirSecondHandStatusID_789 != 7)
            {
                throw new System.InvalidOperationException("Аппарат не готов к продаже или уже возвращён!");
            }

            #endregion

            #region Меняем статус аппарата

            docSecondHandPurch.DirSecondHandStatusID = 9;
            docSecondHandPurch.DirReturnTypeID = docSecondHandReturn.DirReturnTypeID;
            docSecondHandPurch.DirDescriptionID = docSecondHandReturn.DirDescriptionID;

            db.Entry(docSecondHandPurch).State = EntityState.Modified;
            await db.SaveChangesAsync();

            #endregion


            #region 1. Doc

            //Модель
            Models.Sklad.Doc.Doc doc = new Models.Sklad.Doc.Doc();
            //Присваиваем значения
            doc.ListObjectID = ListObjectID;
            doc.IsImport = false;
            doc.NumberInt = docSecondHandReturn.NumberInt;
            doc.NumberReal = docSecondHandReturn.DocSecondHandReturnID;
            doc.DirEmployeeID = field.DirEmployeeID;
            doc.DirPaymentTypeID = docSecondHandReturn.DirPaymentTypeID;
            doc.Payment = docSecondHandReturn.Payment;
            if (docSecondHandReturn.DirContractorID != null) doc.DirContractorID = Convert.ToInt32(docSecondHandReturn.DirContractorID); else doc.DirContractorID = docSecondHandReturn.DirContractorIDOrg;
            doc.DirContractorIDOrg = docSecondHandReturn.DirContractorIDOrg;
            doc.Discount = docSecondHandReturn.Discount;
            doc.DirVatValue = docSecondHandReturn.DirVatValue;
            doc.Base = docSecondHandReturn.Base;
            doc.Description = docSecondHandReturn.Description;
            doc.DocDate = DateTime.Now; //docSecondHandReturn.DocDate;
                                        //doc.DocDisc = docSecondHandReturn.DocDisc;
            if (UO_Action == "held") doc.Held = true;
            else doc.Held = false;
            doc.DocID = docSecondHandReturn.DocID;
            doc.DocIDBase = docSecondHandReturn.DocIDBase;
            doc.KKMSCheckNumber = docSecondHandReturn.KKMSCheckNumber;
            doc.KKMSIdCommand = docSecondHandReturn.KKMSIdCommand;
            doc.KKMSEMail = docSecondHandReturn.KKMSEMail;
            doc.KKMSPhone = docSecondHandReturn.KKMSPhone;

            //Класс
            Docs.Docs docs = new Docs.Docs(db, dbRead, doc, entityState);
            //doc = await docs.Save();
            await Task.Run(() => docs.Save());

            //Нужно вернуть "docSecondHandReturn" со всем полями!
            docSecondHandReturn.DocID = doc.DocID;

            #endregion

            #region 2. DocSecondHandReturn

            docSecondHandReturn.DocID = doc.DocID;

            db.Entry(docSecondHandReturn).State = entityState;
            await db.SaveChangesAsync();

            #region 2.1. UpdateNumberInt, если INSERT

            if (entityState == EntityState.Added && (docSecondHandReturn.doc.NumberInt == null || docSecondHandReturn.doc.NumberInt.Length == 0))
            {
                doc.NumberInt = docSecondHandReturn.DocSecondHandReturnID.ToString();
                doc.NumberReal = docSecondHandReturn.DocSecondHandReturnID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }
            else if (entityState == EntityState.Added)
            {
                doc.NumberReal = docSecondHandReturn.DocSecondHandReturnID;
                docs = new Docs.Docs(db, dbRead, doc, EntityState.Modified);
                await Task.Run(() => docs.Save());
            }

            #endregion


            #endregion


            #region Касса или Банк


            #region 1. Получаем валюту из склада

            int DirCurrencyID = 0, DirCurrencyMultiplicity = 0; //, DirCashOfficeID = 0, DirBankID = 0;;
            double DirCurrencyRate = 0;

            var query = await Task.Run(() =>
                (
                    from x in db.DirWarehouses
                    where x.DirWarehouseID == docSecondHandReturn.DirWarehouseID
                    select new
                    {
                        //DirCashOfficeID= x.dirCashOffice.DirCashOfficeID,
                        DirCurrencyID_Bank = x.dirBank.DirCurrencyID,
                        DirCurrencyRate_Bank = x.dirBank.dirCurrency.DirCurrencyRate,
                        DirCurrencyMultiplicity_Bank = x.dirBank.dirCurrency.DirCurrencyMultiplicity,

                        //DirBankID = x.dirBank.DirBankID,
                        DirCurrencyID_Cash = x.dirCashOffice.DirCurrencyID,
                        DirCurrencyRate_Cash = x.dirCashOffice.dirCurrency.DirCurrencyRate,
                        DirCurrencyMultiplicity_Cash = x.dirCashOffice.dirCurrency.DirCurrencyMultiplicity,
                    }
                ).ToListAsync());

            if (query.Count() > 0)
            {
                if (doc.DirPaymentTypeID == 1)
                {
                    //DirCashOfficeID = Convert.ToInt32(query[0].DirCashOfficeID);
                    DirCurrencyID = query[0].DirCurrencyID_Cash;
                    DirCurrencyRate = query[0].DirCurrencyRate_Cash;
                    DirCurrencyMultiplicity = query[0].DirCurrencyMultiplicity_Cash;
                }
                else if (doc.DirPaymentTypeID == 2)
                {
                    //DirBankID = Convert.ToInt32(query[0].DirBankID);
                    DirCurrencyID = query[0].DirCurrencyID_Bank;
                    DirCurrencyRate = query[0].DirCurrencyRate_Bank;
                    DirCurrencyMultiplicity = query[0].DirCurrencyMultiplicity_Bank;
                }
                else
                {
                    throw new System.InvalidOperationException("Не выбран метод оплаты: Касса или Банк!");
                }
            }

            #endregion


            #region 2. Заполняем Модель

            Models.Sklad.Pay.Pay pay = new Models.Sklad.Pay.Pay();
            //pay.DirCashOfficeID = Convert.ToInt32(DirCashOfficeID);
            //pay.DirBankID = Convert.ToInt32(DirBankID);
            //Валюта
            pay.DirCurrencyID = DirCurrencyID;
            pay.DirCurrencyRate = DirCurrencyRate;
            pay.DirCurrencyMultiplicity = DirCurrencyMultiplicity;

            pay.DirEmployeeID = field.DirEmployeeID;
            pay.DirPaymentTypeID = doc.DirPaymentTypeID;
            //pay.DirXName = ""; //no
            //pay.DirXSumTypeID = 0; //no
            pay.DocCashBankID = null;
            pay.DocID = doc.DocID;
            pay.DocXID = docSecondHandReturn.DocSecondHandReturnID;
            pay.DocXSumDate = doc.DocDate;
            pay.DocXSumSum = docSecondHandReturn.PriceCurrency - doc.Discount; // - получили при сохранении Спецификации (выше) docSecondHandReturn.PriceVAT

            //DocSecondHandPurchID - найти!!!
            pay.Base = "Возврат документа №" + docSecondHandPurch.DocSecondHandPurchID; //pay.Base = "Оплата за коды товаров: " + NomenName; // - получили при сохранении Спецификации (выше)

            //pay.Description = "";
            pay.KKMSCheckNumber = docSecondHandReturn.KKMSCheckNumber;
            pay.KKMSIdCommand = docSecondHandReturn.KKMSIdCommand;
            pay.KKMSEMail = docSecondHandReturn.KKMSEMail;
            pay.KKMSPhone = docSecondHandReturn.KKMSPhone;

            pay.Discount = doc.Discount;

            #endregion


            #region 3. Сохраняем

            PartionnyAccount.Controllers.Sklad.Pay.PayController payController = new Pay.PayController();
            doc = await Task.Run(() => payController.mPutPostPay(db, pay, EntityState.Modified, field)); //sysSetting

            #endregion


            #endregion


            #region 4. Log

            logService.DocSecondHandPurchID = docSecondHandPurch.DocSecondHandPurchID;
            logService.DirSecondHandLogTypeID = 14;
            logService.DirEmployeeID = field.DirEmployeeID;
            logService.DirSecondHandStatusID = docSecondHandPurch.DirSecondHandStatusID;
            logService.DirWarehouseIDFrom = docSecondHandPurch.DirWarehouseID;
            logService.DirWarehouseIDTo = docSecondHandPurch.DirWarehouseID;
            //logService.Msg = "Аппарат принят на точку №" + docSecondHandPurch.DirWarehouseID;

            await logServicesController.mPutPostLogSecondHands(db, logService, EntityState.Added);

            #endregion


            return docSecondHandReturn;
        }

        #endregion
    }
}