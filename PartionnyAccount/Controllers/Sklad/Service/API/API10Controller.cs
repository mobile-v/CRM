using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using PartionnyAccount.Models;
using PartionnyAccount.Models.Sklad.Doc;
using PartionnyAccount.Models.Sklad.Service.API;
using System.Data.SQLite;
using System.Web.Script.Serialization;
using System.Data.OleDb;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace PartionnyAccount.Controllers.Sklad.Service.API
{
    public class API10Controller : ApiController
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
        Classes.Account.Login.Field field = new Classes.Account.Login.Field();
        private DbConnectionSklad db = new DbConnectionSklad();
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 73;

        #endregion


        #region SELECT

        class Params
        {
            public string pID = "";
           
        }

        // GET: api/APIs/5
        //[ResponseType(typeof(API))]
        public async Task<IHttpActionResult> GetAPI10(int id, HttpRequestMessage request)
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
                /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "API"));
                if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightAPI10s"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

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
                _params.pID = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pID", true) == 0).Value;

                #endregion


                #region Отправка JSON

                if (_params.pID == "KeyGen")
                {

                    //Сгенерировать "KeyGen"

                    string result = await KeyGen(field.DirCustomersID);

                    //return Ok(returnServer.Return(true, result));
                    return Ok ( result );

                }

                else
                {

                    //Выборка "1"

                    var query = await Task.Run(() =>
                        (

                            from x in db.API10s

                            where x.API10ID == id

                            select x

                        ).ToListAsync());


                    if (query.Count() > 0)
                    {
                        return Ok(returnServer.Return(true, query[0]));
                    }
                    else
                    {
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89_2));
                    }
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

        // PUT: api/API10s/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAPI10(int id, API10 aPI10)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightAPI10s"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != aPI10.API10ID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            aPI10.Substitute();

            #endregion


            #region Сохранение

            try
            {
                db.Entry(aPI10).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = aPI10.API10ID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = aPI10.API10ID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/API10s
        [ResponseType(typeof(API10))]
        public async Task<IHttpActionResult> PostAPI10(API10 aPI10)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightAPI10s"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            aPI10.Substitute();

            #endregion


            #region Сохранение

            try
            {
                db.Entry(aPI10).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = aPI10.API10ID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = aPI10.API10ID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        #endregion


        #region Mthods

        //Используется для генерации Кода (Handlers.Service.API10.ApiRequest.ashx)
        internal async Task<string> KeyGen(int DirCustomersID)
        {

            //DirCustomersID и Login
            //  X1 + "Левые символы" + Y1 + Закодирован DirCustomersID + Левые символы + Закодирован DirCustomersID + Левые символы
            //   1          8           1              X                      12                     X                     15

            //где X1 - сдвиг от 1 - 10
            //где Y1 - К-во символов в "DirCustomersID"

            string sKey = "";

            //1. X1 - Random(1, 10) (сдвиг от 1 - 10)
            Random random = new Random();
            int X1 = random.Next(1, 9);

            //1. Левые символы - 1
            PartionnyAccount.Classes.Function.RandomSymbol randomSymbol1 = new PartionnyAccount.Classes.Function.RandomSymbol();
            string sSymbol1 = randomSymbol1.ReturnRandom(8);

            //2. Кодируем "DirCustomersID"
            //Получаем "DirCustomersID"
            //int DirCustomersID = var.VerifyUser_MSSQL_ID(pUsersID); //!!!
            //Колируем "DirCustomersID"
            PartionnyAccount.Classes.Function.RandomSymbol randomSymbol2 = new PartionnyAccount.Classes.Function.RandomSymbol();
            string sDirCustomersID = randomSymbol2.ReturnInteger_Code(DirCustomersID.ToString(), X1);
            //Y1 - К-во символов в "DirCustomersID"
            int Y1 = sDirCustomersID.ToString().Length;

            //3. Левые символы - 2
            PartionnyAccount.Classes.Function.RandomSymbol randomSymbol3 = new PartionnyAccount.Classes.Function.RandomSymbol();
            string sSymbol2 = randomSymbol3.ReturnRandom(12);

            //4. Кодируем "DirCustomersID"
            //PartionnyAccount.Classes.Function.RandomSymbol randomSymbol4 = new PartionnyAccount.Classes.Function.RandomSymbol();
            //string sLogin = randomSymbol4.ReturnInteger(pUsersID, X1);

            //5. Левые символы - 3
            PartionnyAccount.Classes.Function.RandomSymbol randomSymbol5 = new PartionnyAccount.Classes.Function.RandomSymbol();
            string sSymbol3 = randomSymbol5.ReturnRandom(15);

            //n. Клеим символы
            sKey = X1 + sSymbol1 + Y1 + sDirCustomersID + sSymbol2 + sDirCustomersID + sSymbol3;



            PartionnyAccount.Classes.Crypto.AES aES = new Classes.Crypto.AES();
            sKey = aES.EncryptStringAES(sKey, aES.password);



            return sKey;
        }


        //Используется для проверки Кода (API.Api10.ashx)
        internal int iDirCustomersID = 0;
        internal async Task<bool> KeyGen_Verify(string Key) //, int DirCustomersID
        {
            string 
                sKey = Key,
                sKey2 = Key;



            //Расскодируем "AES"
            PartionnyAccount.Classes.Crypto.AES aES = new Classes.Crypto.AES();
            Key = sKey = aES.DecryptStringAES(sKey, aES.password);



            // === === === === === === === === === === === === === === === === === === ===

            //1. Получаем 2-а "DirCustomersID"
            //2. Проверяем равны ли они (нет - исключение: Key не правильный, у Вас осталось 7 попыток.)
            //3. Ищем в БД (MS SQL) "DirCustomersID" и получаем pUsers (Login) (не находим - исключение: Key не правильный, у Вас осталось 7 попыток.)
            //4. Проверяем в БД (SQLite) клиента Key
            //   - Совпадает - получаем дополнительные
            //   - не совпадает - исключение: Key не правильный, у Вас осталось 7 попыток.


            //Создаём копию "Key"
            //string sKey = Key;



            #region 1. Получаем 2-а "DirCustomersID"


            #region 1.1. Получаем X1 - сдвиг от 1 - 10 === === ===
            string sX1 = Key[0].ToString();

            int X1;
            bool bX1 = Int32.TryParse(sX1, out X1);
            if (!bX1) throw new System.InvalidOperationException(PartionnyAccount.Classes.Language.Sklad.Language.msg93);
            #endregion


            #region 1.3. Получаем Y1 - К-во символов в "DirCustomersID-1" === === ===
            Key = Key.Remove(0, 9);

            string sY1 = Key[0].ToString();

            int Y1;
            bool bY1 = Int32.TryParse(sY1, out Y1);
            if (!bY1) throw new System.InvalidOperationException(PartionnyAccount.Classes.Language.Sklad.Language.msg93);

            //Удаляем Y1
            Key = Key.Remove(0, 1);

            //Считываем Y1 символов - это и будет первый "DirCustomersID"
            string sDirCustomersID1 = "";
            for (int i = 0; i < Y1; i++) sDirCustomersID1 += Key[i].ToString();
            /*
            int DirCustomersID1;
            bool bDirCustomersID1 = Int32.TryParse(sDirCustomersID1, out DirCustomersID1);
            if (!bDirCustomersID1) throw new System.InvalidOperationException(PartionnyAccount.Classes.Language.Sklad.Language.msg93);
            */
            #endregion


            #region 1.3. Получаем 2-й "DirCustomersID"
            Key = Key.Remove(0, Y1 + 12);

            /*
            Y1 - уже нашли
            string sY1 = Key[0].ToString();

            int Y1;
            bool bY1 = Int32.TryParse(sY1, out Y1);
            if (!bY1) throw new System.InvalidOperationException(PartionnyAccount.Classes.Language.Sklad.Language.msg93);
            */

            //Удаляем Y1
            //Key = Key.Remove(0, 1);

            //Считываем Y1 символов - это и будет первый "DirCustomersID"
            string sDirCustomersID2 = "";
            for (int i = 0; i < Y1; i++) sDirCustomersID2 += Key[i].ToString();

            /*
            int DirCustomersID2;
            bool bDirCustomersID2 = Int32.TryParse(sDirCustomersID2, out DirCustomersID2);
            if (!bDirCustomersID2) throw new System.InvalidOperationException(PartionnyAccount.Classes.Language.Sklad.Language.msg93);
            */
            #endregion


            #endregion


            #region 2. Проверяем равны ли они (нет - исключение: Key не правильный, у Вас осталось 7 попыток.)

            if (sDirCustomersID1 != sDirCustomersID2) throw new System.InvalidOperationException(PartionnyAccount.Classes.Language.Sklad.Language.msg93);

            #endregion


            #region Декодируем "DirCustomersID"

            //Колируем "DirCustomersID"
            PartionnyAccount.Classes.Function.RandomSymbol randomSymbol2 = new PartionnyAccount.Classes.Function.RandomSymbol();
            //int iDirCustomersID = 0;
            try { iDirCustomersID = Convert.ToInt32(randomSymbol2.ReturnInteger_Decode(sDirCustomersID1, X1)); } catch { throw new System.InvalidOperationException(PartionnyAccount.Classes.Language.Sklad.Language.msg93); }

            #endregion


            #region 3. Ищем в БД (MS SQL) "DirCustomersID" и получаем pUsers (Login) (не находим - исключение: Key не правильный, у Вас осталось 7 попыток.)

            string pUsersID = connectionString.mDLogin(iDirCustomersID); // _var.VerifyUser_MSSQL_Login(iDirCustomersID);
            //Не нашло или клиент не активный
            if (String.IsNullOrEmpty(pUsersID)) throw new System.InvalidOperationException(PartionnyAccount.Classes.Language.Sklad.Language.msg93);

            #endregion


            #region 4. Проверяем в БД (SQLite) клиента Key

            db = new DbConnectionSklad(connectionString.Return(iDirCustomersID, null, true));

            var query = await Task.Run(() =>
                (

                    from x in db.API10s

                    where x.API10Key == sKey2

                    select x

                ).ToListAsync());

            if (query.Count() > 0)
            {
                if (query[0].API10Key == sKey2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            #endregion
                        
        }


        #endregion
    }
}
