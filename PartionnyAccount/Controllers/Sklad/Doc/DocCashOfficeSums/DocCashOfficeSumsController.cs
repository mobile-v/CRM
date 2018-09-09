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

namespace PartionnyAccount.Controllers.Sklad.Doc.DocCashOfficeSums
{
    public class DocCashOfficeSumsController : ApiController
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

        int ListObjectID = 29;

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
        // GET: api/DocCashOfficeSums
        public IQueryable<DocCashOfficeSum> GetDocCashOfficeSums()
        {
            return db.DocCashOfficeSums;
        }

        // GET: api/DocCashOfficeSums/5
        [ResponseType(typeof(DocCashOfficeSum))]
        public async Task<IHttpActionResult> GetDocCashOfficeSum(int id)
        {
            DocCashOfficeSum docCashOfficeSum = await db.DocCashOfficeSums.FindAsync(id);
            if (docCashOfficeSum == null)
            {
                return NotFound();
            }

            return Ok(docCashOfficeSum);
        }

        #endregion


        #region UPDATE

        // PUT: api/DocCashOfficeSums/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDocCashOfficeSum(int id, DocCashOfficeSum docCashOfficeSum)
        {
            return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
        }

        // POST: api/DocCashOfficeSums
        [ResponseType(typeof(DocCashOfficeSum))]
        public async Task<IHttpActionResult> PostDocCashOfficeSum(DocCashOfficeSum docCashOfficeSum)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocCashOfficeSums"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            //docCashOfficeSum.Substitute();
            docCashOfficeSum.DirEmployeeID = field.DirEmployeeID;

            #endregion


            #region Сохранение

            try
            {
                docCashOfficeSum = await Task.Run(() => mPutPostDocCashOfficeSum(db, docCashOfficeSum, EntityState.Added));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = docCashOfficeSum.DocCashOfficeSumID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = docCashOfficeSum.DocCashOfficeSumID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DocCashOfficeSums/5
        [ResponseType(typeof(DocCashOfficeSum))]
        public async Task<IHttpActionResult> DeleteDocCashOfficeSum(int id)
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

        private bool DocCashOfficeSumExists(int id)
        {
            return db.DocCashOfficeSums.Count(e => e.DocCashOfficeSumID == id) > 0;
        }



        //Сохранение
        internal async Task<DocCashOfficeSum> mPutPostDocCashOfficeSum(
                    DbConnectionSklad db,
                    DocCashOfficeSum docCashOfficeSum,
                    EntityState entityState //EntityState.Added, Modified
                    )
        {
            //1. Дата операции
            DateTime DocCashOfficeSumDate = DateTime.Now;
            docCashOfficeSum.DocCashOfficeSumDate = DocCashOfficeSumDate;
            docCashOfficeSum.DateOnly = Convert.ToDateTime(DocCashOfficeSumDate.ToString("yyyy-MM-dd"));
            //2. Валюта
            //2.1. По кассе получаем 
            Models.Sklad.Dir.DirCashOffice dirCashOffice = db.DirCashOffices.Find(docCashOfficeSum.DirCashOfficeID);
            //2.2. Получаем Курсы
            Models.Sklad.Dir.DirCurrency dirCurrency = db.DirCurrencies.Find(dirCashOffice.DirCurrencyID);
            docCashOfficeSum.DirCurrencyID = dirCurrency.DirCurrencyID;
            docCashOfficeSum.DirCurrencyRate = dirCurrency.DirCurrencyRate;
            docCashOfficeSum.DirCurrencyMultiplicity = dirCurrency.DirCurrencyMultiplicity;

            //3. Проверка
            //3.1. В зависимости от "DirCashOfficeSumTypeID" сумма с "+" или с "-"
            Models.Sklad.Dir.DirCashOfficeSumType dirCashOfficeSumType = db.DirCashOfficeSumTypes.Find(docCashOfficeSum.DirCashOfficeSumTypeID);
            docCashOfficeSum.DocCashOfficeSumSum = dirCashOfficeSumType.Sign * docCashOfficeSum.DocCashOfficeSumSum;
            //3.2. Если изъятие из кассы и сумма в кассе меньше чем надо ихъять
            if (dirCashOfficeSumType.Sign < 0 && dirCashOffice.DirCashOfficeSum < Math.Abs(docCashOfficeSum.DocCashOfficeSumSum))
            {
                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg27_1 + dirCashOffice.DirCashOfficeSum +
                    Classes.Language.Sklad.Language.msg27_2 + Math.Abs(docCashOfficeSum.DocCashOfficeSumSum) +
                    Classes.Language.Sklad.Language.msg27_3
                    );
            }

            //4. Сохранение
            db.Entry(docCashOfficeSum).State = entityState;
            await Task.Run(() => db.SaveChangesAsync());

            //5. Ретурн
            return docCashOfficeSum;
        }


        //Используется в новых платежах по документу
        internal async Task<DocCashOfficeSum> mPutPostDocCashOfficeSum_2(
                    DbConnectionSklad db,
                    DocCashOfficeSum docCashOfficeSum,
                    EntityState entityState //EntityState.Added, Modified
                    )
        {
            //Если редактируем
            //Удалить платеж, но перед этим проверить был ли Z-отчет
            if (docCashOfficeSum.DocCashOfficeSumID > 0)
            {
                //Получем по "DocCashOfficeSumID" предыдущие значения платежа
                Models.Sklad.Doc.DocCashOfficeSum docCashOfficeSum_OLD = await Task.Run(() => db.DocCashOfficeSums.FindAsync(docCashOfficeSum.DocCashOfficeSumID));

                //Проверяем был ли Z-отчет
                var queryZ1 = await Task.Run(() =>
                    (
                        from x in db.DocCashOfficeSums
                        where x.DocCashOfficeSumDate > docCashOfficeSum_OLD.DocCashOfficeSumDate && x.DirCashOfficeSumTypeID == 3
                        select x
                    ).ToListAsync());

                if (queryZ1.Count() > 0)
                {
                    throw new System.InvalidOperationException(
                        Classes.Language.Sklad.Language.msg119 + docCashOfficeSum_OLD.DocCashOfficeSumDate +
                        Classes.Language.Sklad.Language.msg119_1 + queryZ1[0].DocCashOfficeSumDate
                        );
                }

                //Удаляем платеж
                db.Entry(docCashOfficeSum_OLD).State = EntityState.Deleted;
                await Task.Run(() => db.SaveChangesAsync());
            }

            //По кассе получаем 
            Models.Sklad.Dir.DirCashOffice dirCashOffice = db.DirCashOffices.Find(docCashOfficeSum.DirCashOfficeID);

            //Проверка
            //1. В зависимости от "DirCashOfficeSumTypeID" сумма с "+" или с "-"
            Models.Sklad.Dir.DirCashOfficeSumType dirCashOfficeSumType = db.DirCashOfficeSumTypes.Find(docCashOfficeSum.DirCashOfficeSumTypeID);
            docCashOfficeSum.DocCashOfficeSumSum = dirCashOfficeSumType.Sign * docCashOfficeSum.DocCashOfficeSumSum;
            //Если изъятие из кассы и сумма в кассе меньше чем надо изъять
            if (dirCashOfficeSumType.Sign < 0 && dirCashOffice.DirCashOfficeSum < Math.Abs(docCashOfficeSum.DocCashOfficeSumSum))
            {
                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg27_1 + dirCashOffice.DirCashOfficeSum +
                    Classes.Language.Sklad.Language.msg27_2 + Math.Abs(docCashOfficeSum.DocCashOfficeSumSum) +
                    Classes.Language.Sklad.Language.msg27_3
                    );
            }

            //2. Z-отчет
            var queryZ2 = await Task.Run(() =>
                (
                    from x in db.DocCashOfficeSums
                    where x.DocCashOfficeSumDate > docCashOfficeSum.DocCashOfficeSumDate && x.DirCashOfficeSumTypeID == 3
                    select x
                ).ToListAsync());

            if (queryZ2.Count() > 0)
            {
                throw new System.InvalidOperationException(
                    Classes.Language.Sklad.Language.msg119 + docCashOfficeSum.DocCashOfficeSumDate +
                    Classes.Language.Sklad.Language.msg119_1 + queryZ2[0].DocCashOfficeSumDate
                    );
            }


            //4. Сохранение
            db.Entry(docCashOfficeSum).State = entityState;
            await Task.Run(() => db.SaveChangesAsync());

            //5. Ретурн
            return docCashOfficeSum;
        }

        #endregion
    }
}