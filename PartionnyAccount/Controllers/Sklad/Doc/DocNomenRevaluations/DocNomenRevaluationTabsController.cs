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
using PartionnyAccount.Models.Sklad.Doc;

namespace PartionnyAccount.Controllers.Sklad.Doc.DocNomenRevaluations
{
    public class DocNomenRevaluationTabsController : ApiController
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
            public int DocNomenRevaluationID;
            public int DocID;
        }
        // GET: api/DocNomenRevaluationTabs
        public async Task<IHttpActionResult> GetDocNomenRevaluationTabs(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocNomenRevaluations"));
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
                _params.DocNomenRevaluationID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocNomenRevaluationID", true) == 0).Value);

                #endregion



                #region Основной запрос *** *** ***

                var query =
                    (
                        from docNomenRevaluationTabs in db.DocNomenRevaluationTabs

                        join dirNomens11 in db.DirNomens on docNomenRevaluationTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        where docNomenRevaluationTabs.DocNomenRevaluationID == _params.DocNomenRevaluationID

                        #region select

                        select new
                        {
                            DocNomenRevaluationTabID = docNomenRevaluationTabs.DocNomenRevaluationTabID,
                            DocNomenRevaluationID = docNomenRevaluationTabs.DocNomenRevaluationID,
                            DirNomenID = docNomenRevaluationTabs.DirNomenID,

                            //DirNomenName = docNomenRevaluationTabs.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? docNomenRevaluationTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + docNomenRevaluationTabs.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + docNomenRevaluationTabs.dirNomen.DirNomenName,

                            RemPartyID = docNomenRevaluationTabs.RemPartyID,

                            //Цены
                            PriceVAT = docNomenRevaluationTabs.PriceVAT,
                            PriceCurrency = docNomenRevaluationTabs.PriceCurrency,
                            DirCurrencyID = docNomenRevaluationTabs.DirCurrencyID,
                            DirCurrencyRate = docNomenRevaluationTabs.DirCurrencyRate,
                            DirCurrencyMultiplicity = docNomenRevaluationTabs.DirCurrencyMultiplicity,

                            //Розница ***
                            //Наценка
                            MarkupRetail = 100 * (docNomenRevaluationTabs.PriceRetailVAT - docNomenRevaluationTabs.PriceVAT) / docNomenRevaluationTabs.PriceVAT == null ? 0
                            :
                            Math.Round(100 * (docNomenRevaluationTabs.PriceRetailVAT - docNomenRevaluationTabs.PriceVAT) / docNomenRevaluationTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            PriceRetailVAT_OLD = docNomenRevaluationTabs.PriceRetailVAT_OLD,
                            PriceRetailVAT = docNomenRevaluationTabs.PriceRetailVAT,
                            PriceRetailCurrency_OLD = docNomenRevaluationTabs.PriceRetailCurrency_OLD,
                            PriceRetailCurrency = docNomenRevaluationTabs.PriceRetailCurrency,

                            //Опт ***
                            //Наценка
                            MarkupWholesale = 100 * (docNomenRevaluationTabs.PriceWholesaleVAT - docNomenRevaluationTabs.PriceVAT) / docNomenRevaluationTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docNomenRevaluationTabs.PriceWholesaleVAT - docNomenRevaluationTabs.PriceVAT) / docNomenRevaluationTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            PriceWholesaleVAT_OLD = docNomenRevaluationTabs.PriceWholesaleVAT_OLD,
                            PriceWholesaleVAT = docNomenRevaluationTabs.PriceWholesaleVAT,
                            PriceWholesaleCurrency_OLD = docNomenRevaluationTabs.PriceWholesaleCurrency_OLD,
                            PriceWholesaleCurrency = docNomenRevaluationTabs.PriceWholesaleCurrency,

                            //Интерне-Магазин ***
                            //Наценка
                            MarkupIM = 100 * (docNomenRevaluationTabs.PriceIMVAT - docNomenRevaluationTabs.PriceVAT) / docNomenRevaluationTabs.PriceVAT == null ? 0
                            : Math.Round(100 * (docNomenRevaluationTabs.PriceIMVAT - docNomenRevaluationTabs.PriceVAT) / docNomenRevaluationTabs.PriceVAT, sysSetting.FractionalPartInSum),
                            PriceIMVAT_OLD = docNomenRevaluationTabs.PriceIMVAT_OLD,
                            PriceIMVAT = docNomenRevaluationTabs.PriceIMVAT,
                            PriceIMCurrency_OLD = docNomenRevaluationTabs.PriceIMCurrency_OLD,
                            PriceIMCurrency = docNomenRevaluationTabs.PriceIMCurrency
                        }

                        #endregion
                    );

                #endregion


                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = query.Count(),
                    DocNomenRevaluationTab = query
                };
                return await Task.Run(() => Ok(collectionWrapper));

                #endregion
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DocNomenRevaluationTabs/5
        [ResponseType(typeof(DocNomenRevaluationTab))]
        public async Task<IHttpActionResult> GetDocNomenRevaluationTab(int id)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        #endregion


        #region UPDATE

        // PUT: api/DocNomenRevaluationTabs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocNomenRevaluationTab(int id, DocNomenRevaluationTab docNomenRevaluationTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocNomenRevaluationTabs
        [ResponseType(typeof(DocNomenRevaluationTab))]
        public async Task<IHttpActionResult> PostDocNomenRevaluationTab(DocNomenRevaluationTab docNomenRevaluationTab)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // DELETE: api/DocNomenRevaluationTabs/5
        [ResponseType(typeof(DocNomenRevaluationTab))]
        public async Task<IHttpActionResult> DeleteDocNomenRevaluationTab(int id)
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

        private bool DocNomenRevaluationTabExists(int id)
        {
            return db.DocNomenRevaluationTabs.Count(e => e.DocNomenRevaluationTabID == id) > 0;
        }

        #endregion
    }
}