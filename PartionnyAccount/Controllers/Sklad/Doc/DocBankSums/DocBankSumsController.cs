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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocBankSums
{
    public class DocBankSumsController : ApiController
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

        int ListObjectID = 30;

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
        // GET: api/DocBankSums
        public IQueryable<DocBankSum> GetDocBankSums()
        {
            return db.DocBankSums;
        }

        // GET: api/DocBankSums/5
        [ResponseType(typeof(DocBankSum))]
        public async Task<IHttpActionResult> GetDocBankSum(int id)
        {
            DocBankSum docBankSum = await db.DocBankSums.FindAsync(id);
            if (docBankSum == null)
            {
                return NotFound();
            }

            return Ok(docBankSum);
        }

        #endregion


        #region UPDATE

        // PUT: api/DocBankSums/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocBankSum(int id, DocBankSum docBankSum)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocBankSums
        [ResponseType(typeof(DocBankSum))]
        public async Task<IHttpActionResult> PostDocBankSum(DocBankSum docBankSum)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocBankSums"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //docBankSum.Substitute();
            docBankSum.DirEmployeeID = field.DirEmployeeID;

            #endregion


            #region Сохранение

            try
            {
                docBankSum = await Task.Run(() => mPutPostDocBankSum(db, docBankSum, EntityState.Added));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = docBankSum.DocBankSumID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = docBankSum.DocBankSumID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DocBankSums/5
        [ResponseType(typeof(DocBankSum))]
        public async Task<IHttpActionResult> DeleteDocBankSum(int id)
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

        private bool DocBankSumExists(int id)
        {
            return db.DocBankSums.Count(e => e.DocBankSumID == id) > 0;
        }

        //Сохранение
        internal async Task<DocBankSum> mPutPostDocBankSum(
                    DbConnectionSklad db,
                    DocBankSum docBankSum,
                    EntityState entityState //EntityState.Added, Modified
                    )
        {
            //1. Дата операции
            DateTime DocBankSumDate = DateTime.Now;
            docBankSum.DocBankSumDate = DocBankSumDate;
            docBankSum.DateOnly = Convert.ToDateTime(DocBankSumDate.ToString("yyyy-MM-dd"));
            //2. Валюта
            //2.1. По кассе получаем 
            Models.Sklad.Dir.DirBank dirBank = db.DirBanks.Find(docBankSum.DirBankID);
            //2.2. Получаем Курсы
            Models.Sklad.Dir.DirCurrency dirCurrency = db.DirCurrencies.Find(dirBank.DirCurrencyID);
            docBankSum.DirCurrencyID = dirCurrency.DirCurrencyID;
            docBankSum.DirCurrencyRate = dirCurrency.DirCurrencyRate;
            docBankSum.DirCurrencyMultiplicity = dirCurrency.DirCurrencyMultiplicity;

            //3. Проверка
            //3.1. В зависимости от "DirBankSumTypeID" сумма с "+" или с "-"
            Models.Sklad.Dir.DirBankSumType dirBankSumType = db.DirBankSumTypes.Find(docBankSum.DirBankSumTypeID);
            docBankSum.DocBankSumSum = dirBankSumType.Sign * docBankSum.DocBankSumSum;
            //3.2. Если изъятие из кассы и сумма в кассе меньше чем надо ихъять
            if (dirBankSumType.Sign < 0 && dirBank.DirBankSum < Math.Abs(docBankSum.DocBankSumSum))
            {
                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg27_4 + dirBank.DirBankSum +
                    Classes.Language.Sklad.Language.msg27_5 + Math.Abs(docBankSum.DocBankSumSum) +
                    Classes.Language.Sklad.Language.msg27_6
                    );
            }

            //4. Сохранение
            db.Entry(docBankSum).State = entityState;
            await Task.Run(() => db.SaveChangesAsync());

            //5. Ретурн
            return docBankSum;
        }

        //Используется в новых платежах по документу
        internal async Task<DocBankSum> mPutPostDocBankSum_2(
                    DbConnectionSklad db,
                    DocBankSum docBankSum,
                    EntityState entityState //EntityState.Added, Modified
                    )
        {
            //Удалить платеж, но перед этим проверить был ли Z-отчет
            if (docBankSum.DocBankSumID > 0)
            {
                //Получем по "DocBankSumID" предыдущие значения платежа
                Models.Sklad.Doc.DocBankSum docBankSum_OLD = await Task.Run(() => db.DocBankSums.FindAsync(docBankSum.DocBankSumID));

                //Удаляем платеж
                db.Entry(docBankSum_OLD).State = EntityState.Deleted;
                await Task.Run(() => db.SaveChangesAsync());
            }


            //По кассе получаем 
            Models.Sklad.Dir.DirBank dirBank = db.DirBanks.Find(docBankSum.DirBankID);

            //Проверка
            //1. В зависимости от "DirBankSumTypeID" сумма с "+" или с "-"
            Models.Sklad.Dir.DirBankSumType dirBankSumType = db.DirBankSumTypes.Find(docBankSum.DirBankSumTypeID);
            docBankSum.DocBankSumSum = dirBankSumType.Sign * docBankSum.DocBankSumSum;
            //2. Если изъятие из кассы и сумма в кассе меньше чем надо ихъять
            if (dirBankSumType.Sign < 0 && dirBank.DirBankSum < Math.Abs(docBankSum.DocBankSumSum))
            {
                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg27_4 + dirBank.DirBankSum +
                    Classes.Language.Sklad.Language.msg27_5 + Math.Abs(docBankSum.DocBankSumSum) +
                    Classes.Language.Sklad.Language.msg27_6
                    );
            }

            //4. Сохранение
            db.Entry(docBankSum).State = entityState;
            await Task.Run(() => db.SaveChangesAsync());

            //5. Ретурн
            return docBankSum;
        }

        #endregion

    }
}