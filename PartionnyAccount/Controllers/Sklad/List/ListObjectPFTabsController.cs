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
    public class ListObjectPFTabsController : ApiController
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
            public int ListObjectPFID;
        }
        // GET: api/ListObjectPFTabs
        public async Task<IHttpActionResult> GetListObjectPFTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightListObjectPFTabs"));
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
                _params.ListObjectPFID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ListObjectPFID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                /*
                "SELECT ListDocPFTab.ListDocPFTabID, ListDocPFTab.ListDocPFTabName, ListDocPFTab.ListDocFieldID, ListDocFieldName." + ListDocFieldName + ", ListDocPFTab.PositionID " +
                "FROM ListDocPFTab, ListDocField, ListDocFieldName " +
                "WHERE " +
                "(ListDocPFTab.ListDocFieldID=ListDocField.ListDocFieldID)and" +
                "(ListDocField.ListDocFieldNameID=ListDocFieldName.ListDocFieldNameID)and" +
                "(ListDocPFTab.ListDocPFID=@ListDocPFID)";
                */

                var query =
                    (
                        from listObjectPFTabs in db.ListObjectPFTabs
                        where listObjectPFTabs.ListObjectPFID == _params.ListObjectPFID
                        select new
                        {
                            ListObjectPFTabID = listObjectPFTabs.ListObjectPFTabID,
                            ListObjectPFTabName = listObjectPFTabs.ListObjectPFTabName,

                            ListObjectFieldNameID = listObjectPFTabs.ListObjectFieldNameID,
                            ListObjectFieldNameRu = listObjectPFTabs.listObjectFieldName.ListObjectFieldNameRu,

                            PositionID = listObjectPFTabs.PositionID,
                            TabNum = listObjectPFTabs.TabNum,
                            Width = listObjectPFTabs.Width,
                        }
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    ListObjectPFTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/ListObjectPFTabs/5
        [ResponseType(typeof(ListObjectPFTab))]
        public async Task<IHttpActionResult> GetListObjectPFTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/ListObjectPFTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutListObjectPFTab(int id, ListObjectPFTab listObjectPFTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/ListObjectPFTabs
        [ResponseType(typeof(ListObjectPFTab))]
        public async Task<IHttpActionResult> PostListObjectPFTab(ListObjectPFTab listObjectPFTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/ListObjectPFTabs/5
        [ResponseType(typeof(ListObjectPFTab))]
        public async Task<IHttpActionResult> DeleteListObjectPFTab(int id)
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

        private bool ListObjectPFTabExists(int id)
        {
            return db.ListObjectPFTabs.Count(e => e.ListObjectPFTabID == id) > 0;
        }

        #endregion
    }
}