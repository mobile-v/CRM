﻿using System;
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

namespace PartionnyAccount.Controllers.Sklad.Dir.DirChars
{
    public class DirCharColoursController : ApiController
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

        int ListObjectID = 9;

        #endregion


        #region SELECT

        class Params
        {
            //Grid
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;

            //Tree
            public string node = "";
            public int? XGroupID_NotShow = 0;

            //Other
            public string type = "Grid";
            public string parSearch = "";
        }
        // GET: api/DirCharColours
        public async Task<IHttpActionResult> GetDirCharColours(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCharColours"));
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
                _params.limit = 999999; // sysSetting.PageSizeDir; //Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.type = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "type", true) == 0).Value;
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                if (_params.type == "Grid")
                {
                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from dirCharColours in db.DirCharColours
                            select new
                            {
                                DirCharColourID = dirCharColours.DirCharColourID,
                                Del = dirCharColours.Del,
                                DirCharColourName = dirCharColours.DirCharColourName
                            }
                        );

                    #endregion


                    #region Условия (параметры) *** *** ***


                    #region Не показывать удалённые

                    if (!Convert.ToBoolean(sysSetting.DeletedRecordsShow))
                    {
                        query = query.Where(x => x.Del == sysSetting.DeletedRecordsShow);
                    }

                    #endregion


                    #region Поиск

                    if (!String.IsNullOrEmpty(_params.parSearch))
                    {
                        //Проверяем число ли это
                        Int32 iNumber32;
                        bool bResult32 = Int32.TryParse(_params.parSearch, out iNumber32);


                        //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                        if (bResult32)
                        {
                            query = query.Where(x => x.DirCharColourID == iNumber32 || x.DirCharColourName.Contains(_params.parSearch));
                        }
                        else
                        {
                            query = query.Where(x => x.DirCharColourName.Contains(_params.parSearch));
                        }
                    }

                    #endregion


                    #region OrderBy и Лимит

                    query = query.OrderBy(x => x.DirCharColourName).Skip(_params.Skip).Take(_params.limit);

                    #endregion


                    #endregion


                    #region Отправка JSON

                    //К-во Номенклатуры
                    int dirCount = await Task.Run(() => db.DirCharColours.Count());

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount2 = query.Count();
                    if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirCharColour = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    #region Отобразить только "Руты" *** *** ***

                    var query =
                        (
                         from x in db.DirCharColours
                         select new
                         {
                             id = x.DirCharColourID,
                             text = x.DirCharColourName,
                             leaf = true,
                             Del = x.Del
                         }
                        );

                    #endregion


                    #region Отправка JSON

                    //return Ok(await Task.Run(() => query));

                    dynamic collectionWrapper = new
                    {
                        query
                    };
                    return Ok(await Task.Run(() => collectionWrapper));

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DirCharColours/5
        [ResponseType(typeof(DirCharColour))]
        public async Task<IHttpActionResult> GetDirCharColour(int id, HttpRequestMessage request)
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
                /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DirCharColour"));
                if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCharColours"));
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
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (

                        from x in db.DirCharColours

                        where x.DirCharColourID == id

                        select new
                        {
                            //1. Оснавные === === === === === === === === === === ===

                            DirCharColourID = x.DirCharColourID,
                            Del = x.Del,
                            DirCharColourName = x.DirCharColourName
                        }

                    ).ToListAsync());


                if (query.Count() > 0)
                {
                    return Ok(returnServer.Return(true, query[0]));
                }
                else
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));
                }

                //return Ok(returnServer.Return(false, Classes.Language.Language.msg89));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/DirCharColours/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirCharColour(int id, DirCharColour dirCharColour)
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
            /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DirCharColour"));
            if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCharColours"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirCharColour.DirCharColourID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //dirCharColour.Substitute();

            #endregion


            #region Сохранение

            try
            {
                db.Entry(dirCharColour).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirCharColour.DirCharColourID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirCharColour.DirCharColourID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/DirCharColours
        [ResponseType(typeof(DirCharColour))]
        public async Task<IHttpActionResult> PostDirCharColour(DirCharColour dirCharColour)
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
            /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DirCharColour"));
            if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCharColours"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //dirCharColour.Substitute();

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                db.Entry(dirCharColour).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirCharColour.DirCharColourID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirCharColour.DirCharColourID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DirCharColours/5
        [ResponseType(typeof(DirCharColour))]
        public async Task<IHttpActionResult> DeleteDirCharColour(int id)
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
            /*int Status = await Task.Run(() => accessRight.Access(db, field.DirEmployeeID, "DirCharColour"));
            if (Status >= 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCharColours"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DirCharColour dirCharColour = await db.DirCharColours.FindAsync(id);
                if (dirCharColour == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                db.DirCharColours.Remove(dirCharColour);
                await db.SaveChangesAsync();


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 5; //Удаление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = id;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirCharColour.DirCharColourID,
                    Msg = Classes.Language.Sklad.Language.msg19
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, "")
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
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

        private bool DirCharColourExists(int id)
        {
            return db.DirCharColours.Count(e => e.DirCharColourID == id) > 0;
        }

        #endregion
    }
}