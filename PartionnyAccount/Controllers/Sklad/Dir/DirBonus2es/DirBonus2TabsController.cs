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

namespace PartionnyAccount.Controllers.Sklad.Dir.DirBonus2es
{
    public class DirBonus2TabsController : ApiController
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

        class Params
        {
            //Parameters
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;
            public string parSearch = "";
        }
        // GET: api/DirBonus2Tabs
        public IQueryable<DirBonus2Tab> GetDirBonus2Tabs()
        {
            return db.DirBonus2Tabs;
        }

        // GET: api/DirBonus2Tabs/5
        [ResponseType(typeof(DirBonus2Tab))]
        public async Task<IHttpActionResult> GetDirBonus2Tab(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirBonuses"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Отправка JSON

                var query =
                    (
                        from dirBonus2Tabs in db.DirBonus2Tabs
                        where dirBonus2Tabs.DirBonus2ID == id
                        select new
                        {
                            DirBonus2ID = dirBonus2Tabs.DirBonus2ID,
                            SumBegin = dirBonus2Tabs.SumBegin,
                            Bonus = dirBonus2Tabs.Bonus
                        }
                    ).OrderBy(x => x.SumBegin);


                //К-во
                int dirCount = await Task.Run(() => query.Count());

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DirBonus2Tab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/DirBonus2Tabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirBonus2Tab(int id, DirBonus2Tab dirBonus2Tab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dirBonus2Tab.DirBonus2TabID)
            {
                return BadRequest();
            }

            db.Entry(dirBonus2Tab).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DirBonus2TabExists(id))
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

        // POST: api/DirBonus2Tabs
        [ResponseType(typeof(DirBonus2Tab))]
        public async Task<IHttpActionResult> PostDirBonus2Tab(DirBonus2Tab dirBonus2Tab)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DirBonus2Tabs.Add(dirBonus2Tab);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = dirBonus2Tab.DirBonus2TabID }, dirBonus2Tab);
        }

        // DELETE: api/DirBonus2Tabs/5
        [ResponseType(typeof(DirBonus2Tab))]
        public async Task<IHttpActionResult> DeleteDirBonus2Tab(int id)
        {
            DirBonus2Tab dirBonus2Tab = await db.DirBonus2Tabs.FindAsync(id);
            if (dirBonus2Tab == null)
            {
                return NotFound();
            }

            db.DirBonus2Tabs.Remove(dirBonus2Tab);
            await db.SaveChangesAsync();

            return Ok(dirBonus2Tab);
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

        private bool DirBonus2TabExists(int id)
        {
            return db.DirBonus2Tabs.Count(e => e.DirBonus2TabID == id) > 0;
        }

        #endregion
    }
}