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

namespace PartionnyAccount.Controllers.Sklad.Dir.DirCurrencies
{
    public class DirCurrencyHistoriesController : ApiController
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

        // GET: api/DirCurrencyHistories
        class Params
        {
            //Parameters
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;
            public int DirCurrencyID = 0;
            public string parSearch = "";
        }
        public async Task<IHttpActionResult> GetDirCurrencyHistories(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCurrencies"));
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
                //_params.limit = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value);  //Записей на страницу
                //_params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                //_params.Skip = _params.limit * (_params.page - 1);
                _params.DirCurrencyID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirCurrencyID", true) == 0).Value);
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                #endregion


                //Открытие на редактирование в форме

                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.DirCurrencyHistories
                        where x.DirCurrencyID == _params.DirCurrencyID
                        select new
                        {
                            DirCurrencyHistoryID = x.DirCurrencyHistoryID,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName,
                            HistoryDate = x.HistoryDate.ToString(),
                            DirCurrencyRate = x.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.DirCurrencyMultiplicity
                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Поиск

                if (!String.IsNullOrEmpty(_params.parSearch))
                {
                    //Проверяем
                    DateTime dDate;
                    bool bResult1 = DateTime.TryParse(_params.parSearch, out dDate);
                    Double dDecimal;
                    bool bResult2 = Double.TryParse(_params.parSearch, out dDecimal);


                    //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                    if (bResult1)
                    {
                        query = query.Where(x => x.HistoryDate.Contains(_params.parSearch));
                    }
                    else if (bResult2)
                    {
                        query = query.Where(x => x.DirCurrencyRate == dDecimal);
                    }
                    else
                    {
                        query = query.Where(x => x.DirCurrencyName.Contains(_params.parSearch));
                    }
                }

                #endregion


                #region OrderBy и Лимит

                query = query.OrderBy(x => x.HistoryDate); //.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                int dirCount = await query.CountAsync();

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DirCurrencyHistory = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DirCurrencyHistories/5
        [ResponseType(typeof(DirCurrencyHistory))]
        public async Task<IHttpActionResult> GetDirCurrencyHistory(int id)
        {
            DirCurrencyHistory dirCurrencyHistory = await db.DirCurrencyHistories.FindAsync(id);
            if (dirCurrencyHistory == null)
            {
                return NotFound();
            }

            return Ok(dirCurrencyHistory);
        }

        #endregion


        #region UPDATE

        // PUT: api/DirCurrencyHistories/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirCurrencyHistory(int id, DirCurrencyHistory dirCurrencyHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dirCurrencyHistory.DirCurrencyHistoryID)
            {
                return BadRequest();
            }

            db.Entry(dirCurrencyHistory).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DirCurrencyHistoryExists(id))
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

        // POST: api/DirCurrencyHistories
        [ResponseType(typeof(DirCurrencyHistory))]
        public async Task<IHttpActionResult> PostDirCurrencyHistory(DirCurrencyHistory dirCurrencyHistory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DirCurrencyHistories.Add(dirCurrencyHistory);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = dirCurrencyHistory.DirCurrencyHistoryID }, dirCurrencyHistory);
        }

        // DELETE: api/DirCurrencyHistories/5
        [ResponseType(typeof(DirCurrencyHistory))]
        public async Task<IHttpActionResult> DeleteDirCurrencyHistory(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirCurrencies"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DirCurrencyHistory dirCurrencyHistory = await db.DirCurrencyHistories.FindAsync(id);
                if (dirCurrencyHistory == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));


                //Если Мы удаляем единственную или самую первую запись, то выдать исключение!

                //1. Единственную
                if (await db.DirCurrencyHistories.Where(x => x.DirCurrencyID == dirCurrencyHistory.DirCurrencyID).CountAsync() == 1)
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.Mess01));
                }

                //2. Самую первую запись
                DateTime dt = Convert.ToDateTime("1800-01-01 00:00:00");
                var queryDirCurrencyHistoryID = db.DirCurrencyHistories.Where(x => x.DirCurrencyID == dirCurrencyHistory.DirCurrencyID).Select(x => x.DirCurrencyHistoryID); // && x.DirCurrencyHistoryDate <= dt
                if (queryDirCurrencyHistoryID.Count() > 0)
                {
                    if (queryDirCurrencyHistoryID.Min() == dirCurrencyHistory.DirCurrencyHistoryID)
                    {
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.Mess04));
                    }
                }


                db.DirCurrencyHistories.Remove(dirCurrencyHistory);
                await db.SaveChangesAsync();

                dynamic collectionWrapper = new
                {
                    ID = dirCurrencyHistory.DirCurrencyHistoryID,
                    Msg = Classes.Language.Sklad.Language.msg19
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DirCurrencyHistoryExists(int id)
        {
            return db.DirCurrencyHistories.Count(e => e.DirCurrencyHistoryID == id) > 0;
        }

        #endregion
    }
}