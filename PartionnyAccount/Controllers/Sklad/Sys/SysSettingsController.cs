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
using PartionnyAccount.Models.Sklad.Sys;

namespace PartionnyAccount.Controllers.Sklad.Sys
{
    public class SysSettingsController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        private DbConnectionSklad db = new DbConnectionSklad();

        int ListObjectID = 1;

        #endregion


        #region SELECT

        // GET: api/SysSettings
        public IQueryable<SysSetting> GetSysSettings()
        {
            return db.SysSettings;
        }

        // GET: api/SysSettings/5
        [ResponseType(typeof(SysSetting))]
        public async Task<IHttpActionResult> GetSysSetting(int id)
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

                //Разные Функции
                function.NumberDecimalSeparator();

                #endregion


                //Важно: id - не используется в запросах
                if (id == 1)
                {
                    //Открытие на редактирование в форме


                    #region Права. Важно: нужны только при редактировании. При запросе в переменных не нужны!

                    //Права (1 - Write, 2 - Read, 3 - No Access)
                    int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightSysSettings"));
                    if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                    #endregion


                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from sysSettings in db.SysSettings
                            where sysSettings.SysSettingsID == 1
                            select
                            sysSettings
                            /*new
                            {
                                SysSettingsID = sysSettings.SysSettingsID,
                                JurDateS = sysSettings.JurDateS.ToString(),
                                JurDatePo = sysSettings.JurDatePo.ToString(),
                                FractionalPartInSum = sysSettings.FractionalPartInSum,
                                FractionalPartInPrice = sysSettings.FractionalPartInPrice,
                                FractionalPartInOther = sysSettings.FractionalPartInOther,
                                DirVatValue = sysSettings.DirVatValue,
                                ChangePriceNomen = sysSettings.ChangePriceNomen,
                                PurchBigerSale = sysSettings.PurchBigerSale,
                                MinusResidues = sysSettings.MinusResidues,
                                MethodAccounting = sysSettings.MethodAccounting,
                                DeletedRecordsShow = sysSettings.DeletedRecordsShow,

                                DirContractorIDOrg = sysSettings.dirContractorOrg.DirContractorID,
                                DirContractorNameOrg = sysSettings.dirContractorOrg.DirContractorName,

                                DirCurrencyID = sysSettings.DirCurrencyID,
                                DirCurrencyName = sysSettings.dirCurrency.DirCurrencyName,

                                DirWarehouseID = sysSettings.DirWarehouseID,
                                DirWarehouseName = sysSettings.dirWarehouse.DirWarehouseName,

                                MarkupRetail = sysSettings.MarkupRetail,
                                MarkupWholesale = sysSettings.MarkupWholesale,
                                MarkupIM = sysSettings.MarkupIM,
                                MarkupSales1 = sysSettings.MarkupSales1,
                                MarkupSales2 = sysSettings.MarkupSales2,
                                MarkupSales3 = sysSettings.MarkupSales3,
                                MarkupSales4 = sysSettings.MarkupSales4,

                                CashBookAdd = sysSettings.CashBookAdd,
                                Reserve = sysSettings.Reserve,
                                BarIntNomen = sysSettings.BarIntNomen,
                                BarIntContractor = sysSettings.BarIntContractor,
                                BarIntDoc = sysSettings.BarIntDoc,
                                BarIntEmployee = sysSettings.BarIntEmployee,
                                SelectOneClick = sysSettings.SelectOneClick,
                                PageSizeDir = sysSettings.PageSizeDir,
                                PageSizeJurn = sysSettings.PageSizeJurn,
                                DateFormat = sysSettings.DateFormat,

                                DirPriceTypeID = sysSettings.DirPriceTypeID,
                                DirPriceTypeName = sysSettings.dirPriceType.DirPriceTypeName,

                                LabelWidth = sysSettings.LabelWidth,
                                LabelHeight = sysSettings.LabelHeight,
                                LabelEncodeType = sysSettings.LabelEncodeType,
                                DirNomenMinimumBalance = sysSettings.DirNomenMinimumBalance,

                                ReadinessDay = sysSettings.ReadinessDay,
                                ServiceTypeRepair = sysSettings.ServiceTypeRepair,
                                WarrantyPeriodPassed = sysSettings.WarrantyPeriodPassed,
                                PhoneNumberBegin = sysSettings.PhoneNumberBegin,
                                DocServicePurchSmsAutoShow = sysSettings.DocServicePurchSmsAutoShow,
                                ServiceKPD = sysSettings.ServiceKPD,

                                SmsActive = sysSettings.SmsActive,
                                SmsServiceID = sysSettings.SmsServiceID,
                                SmsLogin = sysSettings.SmsLogin,
                                SmsPassword = sysSettings.SmsPassword,
                                SmsTelFrom = sysSettings.SmsTelFrom,
                                SmsAutoShow = sysSettings.SmsAutoShow,
                            }
                            */
                        );

                    #endregion


                    #region Отправка JSON

                    return await Task.Run(() => Ok(returnServer.Return(true, query.ToList()[0])));

                    #endregion
                }
                else
                {
                    //Реквест в Variables

                    #region Получаем оплаты клиента *** *** ***
                    //Тарифный план, Сотрудников, Торговых точек, Окончание, Интернет магазинов

                    Classes.Account.Payment payment = new Classes.Account.Payment();
                    Classes.Account.Payment.CustomerPay customerPay = payment.Return(field.DirCustomersID);

                    #endregion


                    #region Основной запрос *** *** ***
                    
                    var query =
                        (
                            from sysSettings in db.SysSettings
                            from dirEmployees in db.DirEmployees

                            //join dirWarehouses1 in db.DirWarehouses on dirEmployees.DirWarehouseID equals dirWarehouses1.DirWarehouseID into dirWarehouses2
                            //from dirWarehouses in dirWarehouses2.DefaultIfEmpty()

                            where
                                sysSettings.SysSettingsID == 1 &&
                                dirEmployees.DirEmployeeID == field.DirEmployeeID
                            select new
                            {
                                //1. Настройки
                                sysSettings,
                                DirCurrencyMultiplicity = sysSettings.dirCurrency.DirCurrencyMultiplicity, // dirCurrencyHistories.DirCurrencyMultiplicity,
                                DirCurrencyRate = sysSettings.dirCurrency.DirCurrencyRate,
                                //2. Сотрудника (ФИО)
                                dirEmployees,
                                DirEmployeeLogin = field.LoginFull,
                                LoginMS = field.Login,
                                DirEmployeeName = dirEmployees.DirEmployeeName,
                                //DirWarehouseName = dirWarehouses.DirWarehouseName,
                                //3. Оплата
                                DirPayServiceName = customerPay.DirPayServiceName,
                                CountUser = customerPay.CountUser,
                                //CountTT = customerPay.CountTT,
                                CountNomen = customerPay.CountNomen,
                                PayDateEnd = customerPay.PayDateEnd,
                                //CountIM = customerPay.CountIM
                            }
                        );

                    #endregion


                    #region Отправка JSON

                    return await Task.Run(() => Ok(returnServer.Return(true, query.ToList()[0])));

                    #endregion

                }

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/SysSettings/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSysSetting(int id, SysSetting sysSetting)
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];
            int iLanguage = Convert.ToInt32(authCookie["CookieL"]) - 1;

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightSysSettings"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91));

            //Подстановки - некоторые поля надо заполнить, если они не заполены (Юридические реквизиты)
            sysSetting.Substitute();

            #endregion


            #region Сохранение

            try
            {
                db.Entry(sysSetting).State = EntityState.Modified;
                await db.SaveChangesAsync();


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                sysJourDisp.DirDispOperationID = 7; //Изменение настроек
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = 1;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = sysSetting.SysSettingsID
                };
                return Ok(returnServer.Return(true, collectionWrapper));

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/SysSettings
        [ResponseType(typeof(SysSetting))]
        public async Task<IHttpActionResult> PostSysSetting(SysSetting sysSetting)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/SysSettings/5
        [ResponseType(typeof(SysSetting))]
        public async Task<IHttpActionResult> DeleteSysSetting(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SysSettingExists(int id)
        {
            return db.SysSettings.Count(e => e.SysSettingsID == id) > 0;
        }

        #endregion
    }
}