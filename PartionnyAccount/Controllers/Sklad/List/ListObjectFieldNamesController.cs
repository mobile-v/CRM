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
using PartionnyAccount.Models.Sklad.List;

namespace PartionnyAccount.Controllers.Sklad.List
{
    public class ListObjectFieldNamesController : ApiController
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
            public int ListObjectID = 0;
            public string ListObjectField = "";
        }
        // GET: api/ListObjectFieldNames
        //1. List JSON
        public async Task<IHttpActionResult> GetListObjectFieldNames(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightListObjectFieldNames"));
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

                _params.ListObjectID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectID", true) == 0).Value);
                _params.ListObjectField = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectField", true) == 0).Value;

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.ListObjectFieldNames
                        from y in db.ListObjectFields
                        where
                            x.ListObjectFieldNameID == y.ListObjectFieldNameID &&
                            y.ListObjectID == _params.ListObjectID
                        select new
                        {
                            ListObjectFieldNameID = x.ListObjectFieldNameID,
                            ListObjectFieldNameRu = x.ListObjectFieldNameRu,
                            ListObjectFieldHeaderShow = y.ListObjectFieldHeaderShow,
                            ListObjectFieldTabShow = y.ListObjectFieldTabShow,
                            ListObjectFieldFooterShow = y.ListObjectFieldFooterShow,
                        }
                    );

                switch (_params.ListObjectField)
                {
                    case "ListObjectFieldHeaderShow":
                        query = query.Where(x => x.ListObjectFieldHeaderShow == true);
                        break;

                    case "ListObjectFieldTabShow":
                        query = query.Where(x => x.ListObjectFieldTabShow == true);
                        break;

                    case "ListObjectFieldFooterShow":
                        query = query.Where(x => x.ListObjectFieldFooterShow == true);
                        break;

                    default:
                        query = query.Where(x => x.ListObjectFieldHeaderShow == false && x.ListObjectFieldTabShow == false && x.ListObjectFieldFooterShow == false);
                        break;
                }

                #endregion


                #region Условия (параметры) *** *** ***


                #region Поиск

                if (!String.IsNullOrEmpty(_params.parSearch))
                {
                    //Проверяем число ли это
                    Int32 iNumber32;
                    bool bResult32 = Int32.TryParse(_params.parSearch, out iNumber32);


                    //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                    if (bResult32)
                    {
                        query = query.Where(x => x.ListObjectFieldNameID == iNumber32 || x.ListObjectFieldNameRu.Contains(_params.parSearch));
                    }
                    else
                    {
                        query = query.Where(x => x.ListObjectFieldNameRu.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                query = query.OrderBy(x => x.ListObjectFieldNameRu); //.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region JSON

                int dirCount = await Task.Run(() => query.CountAsync());

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    ListObjectFieldName = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }
        //2. List HTML
        public async Task<HttpResponseMessage> GetListObjectFieldNames(string Html1, string Html2, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightListObjectFieldNames"));
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

                _params.ListObjectID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectID", true) == 0).Value);
                _params.ListObjectField = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectField", true) == 0).Value;

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.ListObjectFieldNames
                        from y in db.ListObjectFields
                        where
                            x.ListObjectFieldNameID == y.ListObjectFieldNameID &&
                            y.ListObjectID == _params.ListObjectID
                        select new
                        {
                            ListObjectFieldNameID = x.ListObjectFieldNameID,
                            ListObjectFieldNameRu = x.ListObjectFieldNameRu,
                            ListObjectFieldHeaderShow = y.ListObjectFieldHeaderShow,
                            ListObjectFieldTabShow = y.ListObjectFieldTabShow,
                            ListObjectFieldFooterShow = y.ListObjectFieldFooterShow,
                        }
                    );

                switch (_params.ListObjectField)
                {
                    case "ListObjectFieldHeaderShow":
                        query = query.Where(x => x.ListObjectFieldHeaderShow == true);
                        break;

                    case "ListObjectFieldTabShow":
                        query = query.Where(x => x.ListObjectFieldTabShow == true);
                        break;

                    case "ListObjectFieldFooterShow":
                        query = query.Where(x => x.ListObjectFieldFooterShow == true);
                        break;

                    default:
                        query = query.Where(x => x.ListObjectFieldHeaderShow == false && x.ListObjectFieldTabShow == false && x.ListObjectFieldFooterShow == false);
                        break;
                }

                #endregion


                #region Условия (параметры) *** *** ***


                #region Поиск

                if (!String.IsNullOrEmpty(_params.parSearch))
                {
                    //Проверяем число ли это
                    Int32 iNumber32;
                    bool bResult32 = Int32.TryParse(_params.parSearch, out iNumber32);


                    //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                    if (bResult32)
                    {
                        query = query.Where(x => x.ListObjectFieldNameID == iNumber32 || x.ListObjectFieldNameRu.Contains(_params.parSearch));
                    }
                    else
                    {
                        query = query.Where(x => x.ListObjectFieldNameRu.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                query = query.OrderBy(x => x.ListObjectFieldNameRu); //.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                if (_params.type != "Grid")
                {
                    #region HTML

                    var queryResult = await Task.Run(() => query.ToListAsync());

                    string sHtml = "<div>";
                    for (int i = 0; i < queryResult.Count(); i++)
                    {
                        sHtml += "<input name='ctl'" + i.ToString() + " type='text' value='[[[" + queryResult[i].ListObjectFieldNameRu + "]]]' readonly='readonly' style='height: 100%; width: 100%;' /><br />";
                    }
                    sHtml += "</div>";

                    #endregion


                    #region Отправка HTML

                    //return await Task.Run(() => Ok(sHtml)); //return await Task.Run(() => Ok(collectionWrapper));
                    //return Ok(sHtml);

                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(
                            sHtml,
                            System.Text.Encoding.UTF8,
                            "text/html"
                        )
                    };

                    #endregion
                }
                else
                {
                    #region JSON

                    int dirCount = await Task.Run(() => query.CountAsync());

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        ListObjectFieldName = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/ListObjectFieldNames/5
        [ResponseType(typeof(ListObjectFieldName))]
        public async Task<IHttpActionResult> GetListObjectFieldName(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightListObjectFieldNames"));
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
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                #region Отправка JSON

                var query = await Task.Run(() =>
                    (
                        from x in db.ListObjectFieldNames
                        where x.ListObjectFieldNameID == id
                        select new
                        {
                            ListObjectFieldNameID = x.ListObjectFieldNameID,
                            ListObjectFieldNameRu = x.ListObjectFieldNameRu, //ListObjectNameSys = x.ListObjectNameSys,
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

        // PUT: api/ListObjectFieldNames/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutListObjectFieldName(int id, ListObjectFieldName listObjectFieldName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != listObjectFieldName.ListObjectFieldNameID)
            {
                return BadRequest();
            }

            db.Entry(listObjectFieldName).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListObjectFieldNameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ListObjectFieldNames
        [ResponseType(typeof(ListObjectFieldName))]
        public async Task<IHttpActionResult> PostListObjectFieldName(ListObjectFieldName listObjectFieldName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ListObjectFieldNames.Add(listObjectFieldName);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = listObjectFieldName.ListObjectFieldNameID }, listObjectFieldName);
        }

        // DELETE: api/ListObjectFieldNames/5
        [ResponseType(typeof(ListObjectFieldName))]
        public async Task<IHttpActionResult> DeleteListObjectFieldName(int id)
        {
            ListObjectFieldName listObjectFieldName = await db.ListObjectFieldNames.FindAsync(id);
            if (listObjectFieldName == null)
            {
                return NotFound();
            }

            db.ListObjectFieldNames.Remove(listObjectFieldName);
            await db.SaveChangesAsync();

            return Ok(listObjectFieldName);
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

        private bool ListObjectFieldNameExists(int id)
        {
            return db.ListObjectFieldNames.Count(e => e.ListObjectFieldNameID == id) > 0;
        }

        #endregion
    }
}