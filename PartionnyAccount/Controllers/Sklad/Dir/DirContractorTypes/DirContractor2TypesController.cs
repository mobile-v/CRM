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

namespace PartionnyAccount.Controllers.Sklad.Dir.DirContractorTypes
{
    public class DirContractor2TypesController : ApiController
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

        // GET: api/DirContractor2Types
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
        public async Task<IHttpActionResult> GetDirContractor2Types(HttpRequestMessage request)
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
                            from dirContractor2Types in db.DirContractor2Types
                            select new
                            {
                                DirContractor2TypeID = dirContractor2Types.DirContractor2TypeID,
                                DirContractor2TypeName = dirContractor2Types.DirContractor2TypeName
                            }
                        );

                    #endregion


                    #region Условия (параметры) *** *** ***


                    #region OrderBy и Лимит

                    //query = query.OrderBy(x => x.DirContractor2TypeName); //.Skip(_params.Skip).Take(_params.limit);

                    #endregion


                    #endregion


                    #region Отправка JSON

                    //К-во Номенклатуры
                    int dirCount = await Task.Run(() => db.DirContractor2Types.Count());

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    //int dirCount2 = query.Count();
                    //if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirContractor2Type = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    #region Отобразить только "Руты" *** *** ***

                    var query =
                        (
                         from x in db.DirContractor2Types
                         select new
                         {
                             id = x.DirContractor2TypeID,
                             text = x.DirContractor2TypeName,
                             leaf = true
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

        // GET: api/DirContractor2Types/5
        [ResponseType(typeof(DirContractor2Type))]
        public async Task<IHttpActionResult> GetDirContractor2Type(int id)
        {
            DirContractor2Type dirContractor2Type = await db.DirContractor2Types.FindAsync(id);
            if (dirContractor2Type == null)
            {
                return NotFound();
            }

            return Ok(dirContractor2Type);
        }

        #endregion


        #region UPDATE

        // PUT: api/DirContractor2Types/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirContractor2Type(int id, DirContractor2Type dirContractor2Type)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DirContractor2Types
        [ResponseType(typeof(DirContractor2Type))]
        public async Task<IHttpActionResult> PostDirContractor2Type(DirContractor2Type dirContractor2Type)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DirContractor2Types/5
        [ResponseType(typeof(DirContractor2Type))]
        public async Task<IHttpActionResult> DeleteDirContractor2Type(int id)
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

        private bool DirContractor2TypeExists(int id)
        {
            return db.DirContractor2Types.Count(e => e.DirContractor2TypeID == id) > 0;
        }

        #endregion
    }
}