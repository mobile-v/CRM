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
using PartionnyAccount.Models.Sklad.Rem;
using System.Data.SQLite;

namespace PartionnyAccount.Controllers.Sklad.Rem
{
    public class Rem2PartyMinusesController : ApiController
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
            public int Rem2PartyID;
            public int DirServiceNomenID;
            public int DocSaleID;
            public int DocRetailID;
            public string parSearch = "";

            public int DirWarehouseID;
            public DateTime? DateS, DatePo;
        }
        // GET: api/Rem2PartyMinus
        public async Task<IHttpActionResult> GetRem2PartyMinuses(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightRem2PartyMinuses"));
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
                _params.Rem2PartyID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "Rem2PartyID", true) == 0).Value);
                _params.DirServiceNomenID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirServiceNomenID", true) == 0).Value);
                _params.DocSaleID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocSaleID", true) == 0).Value);
                _params.DocRetailID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocRetailID", true) == 0).Value);
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);

                _params.DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DateS < Convert.ToDateTime("01.01.1800")) _params.DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else _params.DateS = _params.DateS.Value.AddDays(-1);

                _params.DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocDate", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (_params.DatePo < Convert.ToDateTime("01.01.1800")) _params.DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from rem2PartyMinuses in db.Rem2PartyMinuses
                            /*
                            where
                                rem2PartyMinuses.Remnant > 0 &&
                                rem2PartyMinuses.DirContractorIDOrg == _params.DirContractorIDOrg &&
                                rem2PartyMinuses.DirServiceNomenID == _params.DirServiceNomenID &&
                                rem2PartyMinuses.DirWarehouseID == _params.DirWarehouseID
                            */
                        select new
                        {
                            Rem2PartyMinusID = rem2PartyMinuses.Rem2PartyMinusID,
                            DocID = rem2PartyMinuses.DocID,
                            Rem2PartyID = rem2PartyMinuses.Rem2PartyID,
                            DirServiceNomenID = rem2PartyMinuses.DirServiceNomenID,
                            DirServiceNomenName = rem2PartyMinuses.dirServiceNomen.DirServiceNomenName,
                            DocDate = rem2PartyMinuses.doc.DocDate,
                            DirContractorNameOrg = rem2PartyMinuses.doc.dirContractorOrg.DirContractorName,
                            DirContractorName = rem2PartyMinuses.doc.dirContractor.DirContractorName,

                            DirCurrencyID = rem2PartyMinuses.DirCurrencyID,
                            DirCurrencyName = rem2PartyMinuses.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = rem2PartyMinuses.DirCurrencyRate,
                            DirCurrencyMultiplicity = rem2PartyMinuses.DirCurrencyMultiplicity,

                            DirVatValue = rem2PartyMinuses.DirVatValue,
                            DirWarehouseID = rem2PartyMinuses.dirWarehouse.DirWarehouseID,
                            DirWarehouseName = rem2PartyMinuses.dirWarehouse.DirWarehouseName,
                            ListDocNameRu = rem2PartyMinuses.doc.listObject.ListObjectNameRu,
                            PriceVAT = rem2PartyMinuses.PriceVAT,
                            PriceCurrency = rem2PartyMinuses.PriceCurrency,
                            Quantity = rem2PartyMinuses.Quantity,
                            Reserve = rem2PartyMinuses.Reserve
                        }
                    );

                #endregion


                #region Условия (параметры) *** *** ***


                #region Кликнули на товар - паказать список партий

                if (_params.Rem2PartyID > 0) query = query.Where(x => x.Rem2PartyID == _params.Rem2PartyID);
                else if (_params.DirServiceNomenID > 0) query = query.Where(x => x.DirServiceNomenID == _params.DirServiceNomenID);
                else if (_params.DocSaleID > 0)
                {
                    //query = query.Where(x => x.DocSaleID == _params.DocSaleID);
                    //Алгоритм:
                    //1. Получаем DocID по DocSaleID
                    //2. query = query.Where(x => x.DocID == iDocID);

                    //1. Получаем DocID по DocSaleID
                    var queryDocSale = await db.DocSales.FindAsync(_params.DocSaleID);
                    int iDocID = Convert.ToInt32(queryDocSale.DocID);

                    //2. query = query.Where(x => x.DocID == iDocID);
                    query = query.Where(x => x.DocID == iDocID);
                }
                else if (_params.DocRetailID > 0)
                {
                    //query = query.Where(x => x.DocRetailID == _params.DocRetailID);
                    //Алгоритм:
                    //1. Получаем DocID по DocRetailID
                    //2. query = query.Where(x => x.DocID == iDocID);

                    //1. Получаем DocID по DocRetailID
                    var queryDocRetail = await db.DocRetails.FindAsync(_params.DocRetailID);
                    int iDocID = Convert.ToInt32(queryDocRetail.DocID);

                    //2. query = query.Where(x => x.DocID == iDocID);
                    query = query.Where(x => x.DocID == iDocID);
                }

                #endregion


                #region Поиск партии по Серийному номеру или по Штрих-коду

                if (!String.IsNullOrEmpty(_params.parSearch))
                {

                }

                #endregion

                //По дате
                if (_params.DateS != null) query = query.Where(x => x.DocDate >= _params.DateS && x.DocDate <= _params.DatePo);
                //По складу
                if (_params.DirWarehouseID > 0) query = query.Where(x => x.DirWarehouseID == _params.DirWarehouseID);

                #endregion


                #region Отправка JSON

                //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                int dirCount = query.Count();

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = dirCount,
                    Rem2PartyMinus = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/Rem2PartyMinus/5
        [ResponseType(typeof(Rem2PartyMinus))]
        public async Task<IHttpActionResult> GetRem2PartyMinus(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightRem2PartyMinuses"));
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
                int Action = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "Action", true) == 0).Value);

                #endregion


                #region Отправка JSON

                if (Action == 1)
                {

                    var query = await Task.Run(() =>
                     (
                        from rem2PartyMinuses in db.Rem2PartyMinuses

                        where rem2PartyMinuses.DirServiceNomenID == id //&& rem2PartyMinuses.Remnant == 0

                        select new
                        {
                            Rem2PartyID = rem2PartyMinuses.Rem2PartyID,
                            DocDate = rem2PartyMinuses.doc.DocDate,
                            DirContractorName = rem2PartyMinuses.doc.dirContractor.DirContractorName,

                            DirCurrencyID = rem2PartyMinuses.DirCurrencyID,
                            DirCurrencyName = rem2PartyMinuses.dirCurrency.DirCurrencyName,
                            DirCurrencyRate = rem2PartyMinuses.DirCurrencyRate,
                            DirCurrencyMultiplicity = rem2PartyMinuses.DirCurrencyMultiplicity,

                            DirVatValue = rem2PartyMinuses.DirVatValue,
                            DirWarehouseName = rem2PartyMinuses.dirWarehouse.DirWarehouseName,
                            ListDocNameRu = rem2PartyMinuses.doc.listObject.ListObjectNameRu,
                            PriceVAT = rem2PartyMinuses.PriceVAT,
                            PriceCurrency = rem2PartyMinuses.PriceCurrency,
                            Quantity = rem2PartyMinuses.Quantity,
                            Reserve = rem2PartyMinuses.Reserve
                        }
                    ).OrderByDescending(t => t.DocDate)); //.FirstAsync()


                    //Вариант-1
                    if (query.Count() > 0)
                    {
                        return Ok(returnServer.Return(true, query.FirstAsync()));
                    }
                    else
                    {
                        return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89_1));
                    }

                    //Вариант-2 (возможно будет быстрее работать. Всё из-за "query.Count()")
                    //Не работает ...
                    /*
                    try { return Ok(returnServer.Return(true, query.FirstAsync())); }
                    catch { return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89_1)); }
                    */
                }
                else
                {
                    return Ok(returnServer.Return(true, "Error"));
                }

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/Rem2PartyMinus/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRem2PartyMinus(int id, Rem2PartyMinus rem2PartyMinus)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/Rem2PartyMinus
        [ResponseType(typeof(Rem2PartyMinus))]
        public async Task<IHttpActionResult> PostRem2PartyMinus(Rem2PartyMinus rem2PartyMinus)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/Rem2PartyMinus/5
        [ResponseType(typeof(Rem2PartyMinus))]
        public async Task<IHttpActionResult> DeleteRem2PartyMinus(int id)
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

        private bool Rem2PartyMinusExists(int id)
        {
            return db.Rem2PartyMinuses.Count(e => e.Rem2PartyMinusID == id) > 0;
        }




        //!!! ВАЖНО !!!
        //1. Удаление
        //2. Проверка на отрицательные остатки

        internal void Delete(
            DbConnectionSklad _db,
            //Models.Sklad.Rem.Rem2PartyMinus[] rem2PartyMinusCollection
            int DocID
            )
        {
            db = _db;

            SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = DocID };
            db.Database.ExecuteSqlCommand("DELETE FROM Rem2PartyMinuses WHERE DocID=@DocID;", parDocID);
        }

        internal async Task<Models.Sklad.Rem.Rem2PartyMinus[]> Save111(
            DbConnectionSklad _db,
            Models.Sklad.Rem.Rem2PartyMinus[] rem2PartyMinusCollection
            )
        {
            db = _db;

            //Delete(_db, rem2PartyMinusCollection[0].DocID);

            //Сохраняем "party" и
            //Меняем полученный ID-шник
            for (int i = 0; i < rem2PartyMinusCollection.Count(); i++)
            {
                //party
                db.Entry(rem2PartyMinusCollection[i]).State = EntityState.Added;
                //await db.SaveChangesAsync();
            }
            await db.SaveChangesAsync();

            return rem2PartyMinusCollection;
        }

        #endregion
    }
}