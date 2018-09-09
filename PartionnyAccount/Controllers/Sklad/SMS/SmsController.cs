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
using PartionnyAccount.Models.Sklad.Dir;

namespace PartionnyAccount.Controllers.Sklad.SMS
{
    public class SmsController : ApiController
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

        //int ListObjectID = 53;

        #endregion


        #region Не рабочий метод!!!

        //Не рабочий метод!!!
        public async Task<IHttpActionResult> GetSms(int SmsTemplateID, int DocServicePurchID, HttpRequestMessage request)
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

                //Права
                /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DirBank"));
                if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

                //Права (1 - Write, 2 - Read, 3 - No Access)
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirBanks"));
                //if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                string res = "";

                //Находим по "DocServicePurchID" номер телефона клиента
                Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(DocServicePurchID);
                // DirServiceContractorPhone == "79257711344"


                #region Проверки

                //Активны SMS
                if (sysSetting.SmsActive)
                {
                    return Ok(returnServer.Return(false, "Активируйте в Настройках (верхнее меню) оповещение по SMS!"));
                }

                //Проверка номера телефона
                if (docServicePurch.DirServiceContractorPhone == null) return Ok(returnServer.Return(false, "Номер телефоана клиента не корректный!"));
                //Убираем + в самом начале
                docServicePurch.DirServiceContractorPhone = docServicePurch.DirServiceContractorPhone.Replace("+", "");
                //Проверка номера телефона
                if (docServicePurch.DirServiceContractorPhone.Length < 11) return Ok(returnServer.Return(false, "Номер телефоана клиента не корректный!"));

                #endregion


                if (sysSetting.SmsServiceID == 1)
                {
                    #region sms48_ru

                    PartionnyAccount.Classes.SMS.sms48_ru sms48_ru = new Classes.SMS.sms48_ru();
                    res = sms48_ru.Send(sysSetting, docServicePurch.DirServiceContractorPhone, "Apparat otremontirovan. Zaberite ego - 1200 RUR");

                    //Пишем в Лог + ещё куда-то
                    //Только, если успешно!
                    if (res == "8") res = "Отправлено";
                    else if (res == "1") res = "Доставлено";
                    else if (res == "2") res = "Не удалось";
                    else res = "Ошибка: " + res;

                    #endregion
                }
                else if (sysSetting.SmsServiceID == 2)
                {
                    #region sms4b_ru - не работает

                    /*
                    PartionnyAccount.Classes.SMS.sms4b_ru sms4b_ru = new Classes.SMS.sms4b_ru();
                    res = sms4b_ru.Send();

                    //Пишем в Лог + ещё куда-то
                    //Только, если успешно!
                    if (res == "1")
                    {

                    }
                    */

                    #endregion
                }


                dynamic collectionWrapper = new
                {
                    Msg = res
                };
                return Ok(returnServer.Return(true, collectionWrapper));

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        // PUT: api/DirSmsTemplates/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSms(int id, DirSmsTemplate dirSmsTemplate, HttpRequestMessage request)
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

                //Права
                /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DirBank"));
                if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

                //Права (1 - Write, 2 - Read, 3 - No Access)
                //int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirBanks"));
                //if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion

                #region Параметры
                
                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();
                //Параметры
                int ListObjectID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectID", true) == 0).Value); //Записей на страницу

                #endregion


                #region Проверки

                //Активны SMS
                if (!sysSetting.SmsActive)
                {
                    return Ok(returnServer.Return(false, "Активируйте в Настройках (верхнее меню) оповещение по SMS!"));
                }


                string Phone = "";
                if (ListObjectID == 40)
                {
                    //Находим по "DocServicePurchID" номер телефона клиента
                    Models.Sklad.Doc.DocServicePurch docServicePurch = await db.DocServicePurches.FindAsync(id); // DirServiceContractorPhone == "79257711344"
                    //Проверка номера телефона
                    if (docServicePurch.DirServiceContractorPhone == null) return Ok(returnServer.Return(false, "Номер телефоана клиента не корректный!"));
                    //Убираем + в самом начале
                    docServicePurch.DirServiceContractorPhone = docServicePurch.DirServiceContractorPhone.Replace("+", "");
                    //Проверка номера телефона
                    if (docServicePurch.DirServiceContractorPhone.Length < 11) return Ok(returnServer.Return(false, "Номер телефоана клиента не корректный!"));

                    Phone = docServicePurch.DirServiceContractorPhone;
                }
                else if (ListObjectID == 33)
                {
                    //Находим по "DocMovementID" документ
                    Models.Sklad.Doc.DocMovement docMovement = await db.DocMovements.FindAsync(id); // DirServiceContractorPhone == "79257711344"
                    //По документу находим курьера и его номер телефона
                    Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(docMovement.DirEmployeeIDCourier);

                    //Проверка номера телефона
                    if (dirEmployee.Phone == null) return Ok(returnServer.Return(false, "Номер телефоана клиента не корректный!"));
                    //Убираем + в самом начале
                    dirEmployee.Phone = dirEmployee.Phone.Replace("+", "");
                    //Проверка номера телефона
                    if (dirEmployee.Phone.Length < 11) return Ok(returnServer.Return(false, "Номер телефоана клиента не корректный!"));

                    Phone = dirEmployee.Phone;
                }

                #endregion


                #region OLD

                /*
                
                string res = "";
                
                if (sysSetting.SmsServiceID == 1)
                {
                    #region sms48_ru

                    //Отправка SMS
                    PartionnyAccount.Classes.SMS.sms48_ru sms48_ru = new Classes.SMS.sms48_ru();
                    res = sms48_ru.Send(sysSetting, Phone, dirSmsTemplate.DirSmsTemplateMsg);

                    //Результат
                    if (res == "8") res = "Отправлено";
                    else if (res == "1") res = "Доставлено";
                    else if (res == "2") res = "Не удалось";
                    else res = "Ошибка: " + res;

                    //Пишем в Лог
                    await RecordInLog(ListObjectID, id, "Номер тел.:" + Phone + "Текст SMS: " + dirSmsTemplate.DirSmsTemplateMsg + "<br /> Результат: " + res, field);

                    #endregion
                }
                else if (sysSetting.SmsServiceID == 2)
                {
                    #region sms4b_ru - не работает

                    #endregion
                }
                if (sysSetting.SmsServiceID == 3)
                {
                    #region infobip_com

                    //Отправка SMS
                    PartionnyAccount.Classes.SMS.infobip_com infobip_com = new Classes.SMS.infobip_com();
                    res = infobip_com.Send(sysSetting, Phone, dirSmsTemplate.DirSmsTemplateMsg);
                    
                    //Пишем в Лог
                    await RecordInLog(ListObjectID, id, "Номер тел.:" + Phone + "Текст SMS: " + dirSmsTemplate.DirSmsTemplateMsg + "<br /> Результат: " + res, field);

                    #endregion
                }
                */

                #endregion


                int DocID = 0;

                string res = await SenSms(
                    //res,
                    sysSetting,
                    ListObjectID,
                    id,
                    Phone,
                    dirSmsTemplate.DirSmsTemplateMsg,
                    field,
                    db,
                    id
                    );


                #region Меняем в таблице "DocServicePurches" поля: "AlertedCount" и "AlertedDate"

                PartionnyAccount.Models.Sklad.Doc.DocServicePurch docServicePurch1 = await db.DocServicePurches.FindAsync(id);
                docServicePurch1.AlertedCount = Convert.ToInt32(docServicePurch1.AlertedCount) + 1;
                docServicePurch1.AlertedDateTxt = DateTime.Now.ToString("yyyy-MM-dd");
                docServicePurch1.AlertedDate = DateTime.Now;

                db.Entry(docServicePurch1).State = EntityState.Modified;
                await db.SaveChangesAsync();

                #endregion


                dynamic collectionWrapper = new
                {
                    Msg = res
                };
                return Ok(returnServer.Return(true, collectionWrapper));

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        private async Task<bool> RecordInLog(
            int ListObjectID,
            int DocXID, 
            string Msg, 
            Classes.Account.Login.Field field,
            DbConnectionSklad dbX
            )
        {
            if (ListObjectID == 40)
            {
                Models.Sklad.Log.LogService log = new Models.Sklad.Log.LogService(); Controllers.Sklad.Log.LogServicesController logController = new Log.LogServicesController();

                //Пишем в Лог о смене статуса и мастера, если такое было
                log.DocServicePurchID = DocXID;
                log.DirServiceLogTypeID = 4;
                log.DirEmployeeID = field.DirEmployeeID;
                log.Msg = Msg;

                await logController.mPutPostLogServices(dbX, log, EntityState.Added);
            }
            else
            {
                Models.Sklad.Log.LogMovement logService = new Models.Sklad.Log.LogMovement(); Controllers.Sklad.Log.LogMovementsController logController = new Log.LogMovementsController();

                //Пишем в Лог о смене статуса и мастера, если такое было
                logService.DocMovementID = DocXID;
                logService.DirMovementLogTypeID = 2;
                logService.DirEmployeeID = field.DirEmployeeID;
                logService.Msg = Msg;

                await logController.mPutPostLogMovements(dbX, logService, EntityState.Added);
            }

            return true;
        }


        internal async Task<string> SenSms(
            //string res,
            Models.Sklad.Sys.SysSetting sysSetting,
            int ListObjectID,
            int id,
            string Phone,
            string DirSmsTemplateMsg, //DirSmsTemplate dirSmsTemplate,
            Classes.Account.Login.Field field,
            DbConnectionSklad dbX,
            int DocXID
            )
        {
            string res = "";

            if (sysSetting.SmsServiceID == 1)
            {
                #region sms48_ru

                //Отправка SMS
                PartionnyAccount.Classes.SMS.sms48_ru sms48_ru = new Classes.SMS.sms48_ru();
                res = sms48_ru.Send(sysSetting, Phone, DirSmsTemplateMsg);

                //Результат
                if (res == "8") res = "Отправлено";
                else if (res == "1") res = "Доставлено";
                else if (res == "2") res = "Не удалось";
                else res = "Ошибка: " + res;

                //Пишем в Лог
                await RecordInLog(ListObjectID, DocXID, "Номер тел.:" + Phone + "Текст SMS: " + DirSmsTemplateMsg + "<br /> Результат: " + res, field, dbX);

                #endregion
            }
            else if (sysSetting.SmsServiceID == 2)
            {
                #region sms4b_ru - не работает

                /*
                PartionnyAccount.Classes.SMS.sms4b_ru sms4b_ru = new Classes.SMS.sms4b_ru();
                res = sms4b_ru.Send();

                //Пишем в Лог + ещё куда-то
                //Только, если успешно!
                if (res == "1")
                {

                }
                */

                #endregion
            }
            if (sysSetting.SmsServiceID == 3)
            {
                #region infobip_com

                //Отправка SMS
                PartionnyAccount.Classes.SMS.infobip_com infobip_com = new Classes.SMS.infobip_com();
                res = infobip_com.Send(sysSetting, Phone, DirSmsTemplateMsg);

                //Результат
                /*if (res == "OK") res = "Отправлено";
                else if (res == "UNAUTHORIZED") res = "Ошибка авторизации! Не павильная пара: Логин:Пароль";
                else res = "Ошибка: " + res;*/

                //Пишем в Лог
                await RecordInLog(ListObjectID, DocXID, "Номер тел.:" + Phone + "Текст SMS: " + DirSmsTemplateMsg + "<br /> Результат: " + res, field, dbX);

                #endregion
            }

            return res;
        }


    }
}
