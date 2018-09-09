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
    public class SysJourDispsController : ApiController
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

        // GET: api/SysJourDisps
        class Params
        {
            //Parameters
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;
            public int GroupID = 0;
            public string parSearch = "";
        }
        public async Task<IHttpActionResult> GetSysJourDisps(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightSysJourDisps"));
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
                _params.limit = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.GroupID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pGroupID", true) == 0).Value); //Кликнули по группе
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                //Открытие на редактирование в форме

                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.SysJourDisps
                        select new
                        {
                            SysJourDispID = x.SysJourDispID,
                            DirEmployeeName = x.dirEmployee.DirEmployeeName,
                            DirDispOperationName = x.dirDispOperation.DirDispOperationName,
                            ListObjectNameRu = x.listObject.ListObjectNameRu,
                            TableFieldID = x.TableFieldID,
                            SysJourDispDateTime = x.SysJourDispDateTime.ToString(),
                            DocDisc = x.Description,
                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Поиск

                if (!String.IsNullOrEmpty(_params.parSearch))
                {
                    query = query.Where(x => x.DirEmployeeName.Contains(_params.parSearch) || x.DirDispOperationName.Contains(_params.parSearch) || x.ListObjectNameRu.Contains(_params.parSearch));
                }

                #endregion


                #region OrderBy и Лимит

                query = query.OrderBy(x => x.SysJourDispID).Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                //К-во Номенклатуры
                int dirCount = await Task.Run(() => db.SysJourDisps.Count());

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount2 = query.Count();
                if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    SysJourDisp = query
                };
                return await Task.Run(() => Ok(collectionWrapper));
                //return await Task.Run(() => Ok(returnServer.Return(true, dirCount, "SysJourDisp", query)));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/SysJourDisps/5
        [ResponseType(typeof(SysJourDisp))]
        public async Task<IHttpActionResult> GetSysJourDisp(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));
        }

        #endregion


        #region UPDATE

        // PUT: api/SysJourDisps/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSysJourDisp(int id, SysJourDisp sysJourDisp)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));
        }

        // POST: api/SysJourDisps
        [ResponseType(typeof(SysJourDisp))]
        public async Task<IHttpActionResult> PostSysJourDisp(SysJourDisp sysJourDisp)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));
        }

        // DELETE: api/SysJourDisps/5
        [ResponseType(typeof(SysJourDisp))]
        public async Task<IHttpActionResult> DeleteSysJourDisp(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightSysJourDisps"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            #endregion

            #region Удаление

            try
            {
                SysJourDisp sysJourDisp = await db.SysJourDisps.FindAsync(id);
                if (sysJourDisp == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                // === Удаляем === === === === === 
                db.SysJourDisps.Remove(sysJourDisp);
                await db.SaveChangesAsync();

                dynamic collectionWrapper = new
                {
                    ID = sysJourDisp.SysJourDispID,
                    Msg = Classes.Language.Sklad.Language.msg19
                };
                return Ok(returnServer.Return(true, collectionWrapper));
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

        private bool SysJourDispExists(int id)
        {
            return db.SysJourDisps.Count(e => e.SysJourDispID == id) > 0;
        }


        //Сохраням в БД
        internal void mPutPostSysJourDisps(
            DbConnectionSklad _db,
            Models.Sklad.Sys.SysJourDisp sysJourDisp,
            EntityState entityState
            )
        {
            sysJourDisp.SysJourDispDateTime = DateTime.Now;

            _db.Entry(sysJourDisp).State = entityState;
            _db.SaveChanges();
        }

        #endregion
    }
}