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

namespace PartionnyAccount.Controllers.Sklad.Dir.DirCashOfficeSumTypes
{
    public class DirCashOfficeSumTypesController : ApiController
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
        // GET: api/DirCashOfficeSumTypes
        public async Task<IHttpActionResult> GetDirCashOfficeSumTypes(HttpRequestMessage request)
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
                /*int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCashOffices"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));*/

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
                            from dirCashOfficeSumTypes in db.DirCashOfficeSumTypes
                            select new
                            {
                                DirCashOfficeSumTypeID = dirCashOfficeSumTypes.DirCashOfficeSumTypeID,
                                DirCashOfficeSumTypeName = dirCashOfficeSumTypes.DirCashOfficeSumTypeName,
                                Sign = dirCashOfficeSumTypes.Sign
                            }
                        );

                    #endregion


                    #region Условия (параметры) *** *** ***


                    #region Не показывать удалённые

                    /*if (!Convert.ToBoolean(sysSetting.DeletedRecordsShow))
                    {
                        query = query.Where(x => x.Del == sysSetting.DeletedRecordsShow);
                    }*/

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
                            query = query.Where(x => x.DirCashOfficeSumTypeID == iNumber32 || x.DirCashOfficeSumTypeName.Contains(_params.parSearch));
                        }
                        else
                        {
                            query = query.Where(x => x.DirCashOfficeSumTypeName.Contains(_params.parSearch));
                        }
                    }

                    #endregion


                    #region OrderBy и Лимит

                    query = query.OrderBy(x => x.DirCashOfficeSumTypeName).Skip(_params.Skip).Take(_params.limit);

                    #endregion


                    #endregion


                    #region Отправка JSON

                    //К-во Номенклатуры
                    int dirCount = await Task.Run(() => db.DirCashOfficeSumTypes.Count());

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount2 = query.Count();
                    if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirCashOfficeSumType = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    #region Отобразить только "Руты" *** *** ***

                    var query =
                        (
                         from x in db.DirCashOfficeSumTypes
                         select new
                         {
                             id = x.DirCashOfficeSumTypeID,
                             text = x.DirCashOfficeSumTypeName,
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

        // GET: api/DirCashOfficeSumTypes/5
        [ResponseType(typeof(DirCashOfficeSumType))]
        public async Task<IHttpActionResult> GetDirCashOfficeSumType(int id)
        {
            DirCashOfficeSumType dirCashOfficeSumType = await db.DirCashOfficeSumTypes.FindAsync(id);
            if (dirCashOfficeSumType == null)
            {
                return NotFound();
            }

            return Ok(dirCashOfficeSumType);
        }

        #endregion


        #region UPDATE

        // PUT: api/DirCashOfficeSumTypes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirCashOfficeSumType(int id, DirCashOfficeSumType dirCashOfficeSumType)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DirCashOfficeSumTypes
        [ResponseType(typeof(DirCashOfficeSumType))]
        public async Task<IHttpActionResult> PostDirCashOfficeSumType(DirCashOfficeSumType dirCashOfficeSumType)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DirCashOfficeSumTypes/5
        [ResponseType(typeof(DirCashOfficeSumType))]
        public async Task<IHttpActionResult> DeleteDirCashOfficeSumType(int id)
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

        private bool DirCashOfficeSumTypeExists(int id)
        {
            return db.DirCashOfficeSumTypes.Count(e => e.DirCashOfficeSumTypeID == id) > 0;
        }

        #endregion
    }
}