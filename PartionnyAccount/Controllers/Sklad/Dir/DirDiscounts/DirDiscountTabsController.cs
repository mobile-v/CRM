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

namespace PartionnyAccount.Controllers.Sklad.Dir.DirDiscounts
{
    public class DirDiscountTabsController : ApiController
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
        // GET: api/DirDiscountTabs
        public IQueryable<DirDiscountTab> GetDirDiscountTabs()
        {
            return db.DirDiscountTabs;
        }

        // GET: api/DirDiscountTabs/5
        [ResponseType(typeof(DirDiscountTab))]
        public async Task<IHttpActionResult> GetDirDiscountTab(int id)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirDiscounts"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Отправка JSON

                var query =
                    (
                        from dirDiscountTabs in db.DirDiscountTabs
                        where dirDiscountTabs.DirDiscountID == id
                        select new
                        {
                            DirDiscountID = dirDiscountTabs.DirDiscountID,
                            SumBegin = dirDiscountTabs.SumBegin,
                            Discount = dirDiscountTabs.Discount
                        }
                    ).OrderBy(x => x.SumBegin);


                //К-во
                int dirCount = await Task.Run(() => query.Count());

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DirDiscountTab = query
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

        // PUT: api/DirDiscountTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirDiscountTab(int id, DirDiscountTab dirDiscountTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DirDiscountTabs
        [ResponseType(typeof(DirDiscountTab))]
        public async Task<IHttpActionResult> PostDirDiscountTab(DirDiscountTab dirDiscountTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DirDiscountTabs/5
        [ResponseType(typeof(DirDiscountTab))]
        public async Task<IHttpActionResult> DeleteDirDiscountTab(int id)
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

        private bool DirDiscountTabExists(int id)
        {
            return db.DirDiscountTabs.Count(e => e.DirDiscountTabID == id) > 0;
        }

        #endregion
    }
}