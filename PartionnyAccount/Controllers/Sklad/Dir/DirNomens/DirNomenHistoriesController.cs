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
using System.Collections;
using System.IO;
using System.Web.Script.Serialization;

namespace PartionnyAccount.Controllers.Sklad.Dir.DirNomens
{
    public class DirNomenHistoriesController : ApiController
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
            //Parameters
            public int limit = 11;
            public int page = 1;
            public int Skip = 0;
            public int GroupID = 0;

            //Parameters
            public string node = "";
            public int? XGroupID_NotShow = 0;

            //Other
            public string type = "Grid";
            public string parSearch = "";
            public int DirWarehouseID = 0;
            public int DirNomenID = 0;
        }
        // GET: api/DirNomenHistories
        public async Task<IHttpActionResult> GetDirNomenHistories(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirNomens"));
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
                _params.limit = 999999; // sysSetting.PageSizeDir; //Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.type = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "type", true) == 0).Value;
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск
                _params.DirNomenID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirNomenID", true) == 0).Value);

                #endregion


                #region Основной запрос *** *** ***

                var query =
                    (
                        from x in db.DirNomenHistories
                        where x.DirNomenID == _params.DirNomenID
                        select new
                        {
                            DirNomenID = x.DirNomenID,
                            HistoryDate = x.HistoryDate,
                            PriceVAT = x.PriceVAT,
                            DirCurrencyID = x.dirCurrency.DirCurrencyID,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName,
                            MarkupRetail = x.MarkupRetail,
                            PriceRetailVAT = x.PriceRetailVAT,
                            MarkupWholesale = x.MarkupWholesale,
                            PriceWholesaleVAT = x.PriceWholesaleVAT,
                            MarkupIM = x.MarkupIM,
                            PriceIMVAT = x.PriceIMVAT
                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region OrderBy и Лимит

                query = query.OrderByDescending(x => x.HistoryDate); //.Skip(_params.Skip).Take(_params.limit);

                #endregion


                #endregion


                #region Отправка JSON

                int dirCount = await query.CountAsync();

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    DirNomenHistory = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DirNomenHistories/5
        [ResponseType(typeof(DirNomenHistory))]
        public async Task<IHttpActionResult> GetDirNomenHistory(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirNomens"));
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

                        from x in db.DirNomenHistories
                        where x.DirNomenID == id
                        select new
                        {
                            HistoryDate = x.HistoryDate,

                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyName = x.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = x.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.dirCurrency.DirCurrencyMultiplicity,

                            //DirVatValue = x.DirVatValue,
                            //DirWarehouseName = x.dirWarehouse.DirWarehouseName,
                            //ListDocNameRu = x.doc.listObject.ListObjectNameRu,
                            PriceVAT = x.PriceVAT,
                            PriceCurrency = (x.PriceVAT * x.dirCurrency.DirCurrencyRate) / x.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            : 
                            Math.Round((x.PriceVAT * x.dirCurrency.DirCurrencyRate) / x.dirCurrency.DirCurrencyMultiplicity, sysSetting.FractionalPartInSum),
                            //Quantity = x.Quantity,
                            //Remnant = x.Remnant,

                            MarkupRetail = ((x.PriceRetailVAT - x.PriceVAT) / x.PriceVAT) * 100 == null ? 0
                            :
                            Math.Round(((x.PriceRetailVAT - x.PriceVAT) / x.PriceVAT) * 100, sysSetting.FractionalPartInSum),

                            PriceRetailVAT = x.PriceRetailVAT,
                            PriceRetailCurrency = (x.PriceRetailVAT * x.dirCurrency.DirCurrencyRate) / x.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((x.PriceRetailVAT * x.dirCurrency.DirCurrencyRate) / x.dirCurrency.DirCurrencyMultiplicity, sysSetting.FractionalPartInSum),

                            MarkupWholesale = ((x.PriceWholesaleVAT - x.PriceVAT) / x.PriceVAT) * 100 == null ? 0 
                            :
                            Math.Round(((x.PriceWholesaleVAT - x.PriceVAT) / x.PriceVAT) * 100, sysSetting.FractionalPartInSum),

                            PriceWholesaleVAT = x.PriceWholesaleVAT,
                            PriceWholesaleCurrency = (x.PriceWholesaleVAT * x.dirCurrency.DirCurrencyRate) / x.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((x.PriceWholesaleVAT * x.dirCurrency.DirCurrencyRate) / x.dirCurrency.DirCurrencyMultiplicity, sysSetting.FractionalPartInSum),

                            MarkupIM = ((x.PriceIMVAT - x.PriceVAT) / x.PriceVAT) * 100 == null ? 0
                            : 
                            Math.Round(((x.PriceIMVAT - x.PriceVAT) / x.PriceVAT) * 100, sysSetting.FractionalPartInSum),

                            PriceIMVAT = x.PriceIMVAT,
                            PriceIMCurrency = (x.PriceIMVAT * x.dirCurrency.DirCurrencyRate) / x.dirCurrency.DirCurrencyMultiplicity == null ? 0
                            :
                            Math.Round((x.PriceIMVAT * x.dirCurrency.DirCurrencyRate) / x.dirCurrency.DirCurrencyMultiplicity, sysSetting.FractionalPartInSum)
                        }

                    //).ToListAsync());
                    ).OrderByDescending(t => t.HistoryDate)); //.FirstAsync()


                if (query.Count() > 0)
                {
                    return Ok(returnServer.Return(true, query.FirstAsync()));
                }
                else
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89_1));
                }

                //return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/DirNomenHistories/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirNomenHistory(int id, DirNomenHistory dirNomenHistory)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DirNomenHistories
        [ResponseType(typeof(DirNomenHistory))]
        public async Task<IHttpActionResult> PostDirNomenHistory(DirNomenHistory dirNomenHistory)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DirNomenHistories/5
        [ResponseType(typeof(DirNomenHistory))]
        public async Task<IHttpActionResult> DeleteDirNomenHistory(int id)
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

        private bool DirNomenHistoryExists(int id)
        {
            return db.DirNomenHistories.Count(e => e.DirNomenHistoryID == id) > 0;
        }

        #endregion
    }
}