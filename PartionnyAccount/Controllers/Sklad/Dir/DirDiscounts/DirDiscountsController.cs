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
using System.Web.Script.Serialization;
using System.Transactions;
using System.Data.SQLite;

namespace PartionnyAccount.Controllers.Sklad.Dir.DirDiscounts
{
    public class DirDiscountsController : ApiController
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

        int ListObjectID = 22;

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
        // GET: api/DirDiscounts
        public async Task<IHttpActionResult> GetDirDiscounts(HttpRequestMessage request)
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

                #endregion


                if (_params.type == "Grid")
                {
                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from x in db.DirDiscounts
                            select new
                            {
                                DirDiscountID = x.DirDiscountID,
                                Del = x.Del,
                                DirDiscountName = x.DirDiscountName
                            }
                        );

                    #endregion


                    #region Условия (параметры) *** *** ***


                    #region Не показывать удалённые

                    if (!Convert.ToBoolean(sysSetting.DeletedRecordsShow))
                    {
                        query = query.Where(x => x.Del == sysSetting.DeletedRecordsShow);
                    }

                    #endregion


                    #region Поиск

                    if (!String.IsNullOrEmpty(_params.parSearch))
                    {
                        //Проверяем число ли это
                        Int32 iNumber32;
                        bool bResult32 = Int32.TryParse(_params.parSearch, out iNumber32);


                        //Если число, то задействуем в поиске и числовые поля (_params.parSearch == iNumber)
                        if (bResult32)
                        {
                            query = query.Where(x => x.DirDiscountID == iNumber32 || x.DirDiscountName.Contains(_params.parSearch));
                        }
                        else
                        {
                            query = query.Where(x => x.DirDiscountName.Contains(_params.parSearch));
                        }
                    }

                    #endregion


                    #region OrderBy и Лимит

                    query = query.OrderBy(x => x.DirDiscountName); //.Skip(_params.Skip).Take(_params.limit);

                    #endregion


                    #endregion


                    #region Отправка JSON

                    //К-во Номенклатуры
                    int dirCount = await Task.Run(() => db.DirDiscounts.Count());

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount2 = query.Count();
                    //if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirDiscount = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    #region Отобразить только "Руты" *** *** ***

                    var query =
                        (
                         from x in db.DirDiscounts
                         select new
                         {
                             id = x.DirDiscountID,
                             text = x.DirDiscountName,
                             leaf = true,
                             Del = x.Del
                         }
                        );

                    #endregion


                    #region Отправка JSON

                    //return Ok(await Task.Run(() => query));

                    dynamic collectionWrapper = new
                    {
                        query
                    };
                    return Ok(await Task.Run(() => collectionWrapper));

                    #endregion

                }
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DirDiscounts/5 (id == 5 - это "DirDiscountID")
        [ResponseType(typeof(DirDiscount))]
        public async Task<IHttpActionResult> GetDirDiscount(int id, HttpRequestMessage request)
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

                        from x in db.DirDiscounts

                        where x.DirDiscountID == id

                        select new
                        {
                            DirDiscountID = x.DirDiscountID,
                            Del = x.Del,
                            DirDiscountName = x.DirDiscountName,
                            Description = x.Description
                        }

                    ).ToListAsync());


                if (query.Count() > 0)
                {
                    return Ok(returnServer.Return(true, query[0]));
                }
                else
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));
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

        // PUT: api/DirDiscounts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirDiscount(int id, DirDiscount dirDiscount)
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
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            //Получаем колекцию "Табличную часть"
            Models.Sklad.Dir.DirDiscountTab[] dirDiscountTabCollection = null;
            if (!String.IsNullOrEmpty(dirDiscount.recordsDirDiscountTabsGrid))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dirDiscountTabCollection = serializer.Deserialize<Models.Sklad.Dir.DirDiscountTab[]>(dirDiscount.recordsDirDiscountTabsGrid);
            }

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirDiscount.DirDiscountID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                await Task.Run(() => mPutPostDirDiscount(sysSetting, dirDiscount, EntityState.Modified, dirDiscountTabCollection));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirDiscount.DirDiscountID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirDiscount.DirDiscountID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // POST: api/DirDiscounts
        [ResponseType(typeof(DirDiscount))]
        public async Task<IHttpActionResult> PostDirDiscount(DirDiscount dirDiscount)
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
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            //Получаем колекцию "Табличную часть"
            Models.Sklad.Dir.DirDiscountTab[] dirDiscountTabCollection = null;
            if (!String.IsNullOrEmpty(dirDiscount.recordsDirDiscountTabsGrid))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dirDiscountTabCollection = serializer.Deserialize<Models.Sklad.Dir.DirDiscountTab[]>(dirDiscount.recordsDirDiscountTabsGrid);
            }

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                await Task.Run(() => mPutPostDirDiscount(sysSetting, dirDiscount, EntityState.Added, dirDiscountTabCollection));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 3; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirDiscount.DirDiscountID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirDiscount.DirDiscountID
                };
                return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DirDiscounts/5
        [ResponseType(typeof(DirDiscount))]
        public async Task<IHttpActionResult> DeleteDirDiscount(int id)
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
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                DirDiscount dirDiscount = await db.DirDiscounts.FindAsync(id);
                if (dirDiscount == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));
                //if (dirDiscount.SysRecord) return Ok(returnServer.Return(false, Classes.Language.Language.msg98));

                if (!dirDiscount.Del)
                {
                    // === Удаляем === === === === === 


                    #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                    Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                    sysJourDisp.DirDispOperationID = 5; //Удаление записи
                    sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                    sysJourDisp.ListObjectID = ListObjectID;
                    sysJourDisp.TableFieldID = dirDiscount.DirDiscountID;
                    sysJourDisp.Description = "";
                    try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                    #endregion


                    //2. Для каскадного удаления "Договора" (достаточно сделать выборку с ToList() в одном подключении)
                    var queryDirDiscountTabs = await db.DirDiscountTabs.Where(x => x.DirDiscountID == id).ToListAsync();

                    //3. Удаляем
                    try
                    {
                        db.DirDiscounts.Remove(dirDiscount);
                        await db.SaveChangesAsync();

                        dynamic collectionWrapper = new
                        {
                            ID = dirDiscount.DirDiscountID,
                            Msg = Classes.Language.Sklad.Language.msg19
                        };
                        return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
                    }
                    catch (Exception ex)
                    {
                        if (function.ExceptionFkExist(ex))
                        {
                            //2. Исключение - пометка на удаление
                            dirDiscount.Del = true;

                            db.Entry(dirDiscount).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            dynamic collectionWrapper = new
                            {
                                ID = dirDiscount.DirDiscountID,
                                Msg = Classes.Language.Sklad.Language.msg96 //"Помечено на удаление, так как запись задействована в других объектах сервиса (напр. в документах)."
                            };
                            return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
                        }
                        else
                        {
                            return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                        }
                    }

                }
                else
                {
                    // === Снимаем пометку на удаление === === === === === 

                    dirDiscount.Del = false;

                    db.Entry(dirDiscount).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    dynamic collectionWrapper = new
                    {
                        ID = dirDiscount.DirDiscountID,
                        Msg = Classes.Language.Sklad.Language.msg97 //"Пометка на удаление снята."
                    };
                    return Ok(returnServer.Return(true, collectionWrapper)); //return Ok(returnServer.Return(true, ""));
                }
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

        private bool DirDiscountExists(int id)
        {
            return db.DirDiscounts.Count(e => e.DirDiscountID == id) > 0;
        }

        //UpdateDirDiscountBar
        //SaveAccountBasic
        private void mPutPostDirDiscount(
            Models.Sklad.Sys.SysSetting sysSettings,
            DirDiscount dirDiscount,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Dir.DirDiscountTab[] dirDiscountTabCollection
            )
        {
            //using (TransactionScope ts = new TransactionScope())
            using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
            {
                #region 1. DirDiscount *** *** *** *** *** *** *** *** *** ***

                db.Entry(dirDiscount).State = entityState;
                db.SaveChanges();

                #endregion


                #region 2. DirDiscountTab *** *** *** *** *** *** *** *** *** ***


                //3.2. Удаляем все кроме существующих
                //3.2.1. Формируем запрос
                var query2 =
                    (
                    from dirDiscountTabs in db.DirDiscountTabs
                    where dirDiscountTabs.DirDiscountID == dirDiscount.DirDiscountID // == id
                    select dirDiscountTabs
                    ).ToList();
                //3.2.2. Удаляем каждую запись отдельно
                foreach (var dirDiscountTab in query2)
                {
                    try
                    {
                        db.Entry(dirDiscountTab).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    catch (Exception ex) { ts.Rollback(); db.Database.Connection.Close(); throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg93 + ex.Message); }
                }



                //3.3. Проверяем, что бы была строгая градация:
                //     Меньшая сумма - меньшая скидка, при росте суммы - ростёт и скидка
                for (int i = 0; i < dirDiscountTabCollection.Length; i++)
                {
                    for (int j = i + 1; j < dirDiscountTabCollection.Length; j++)
                    {
                        //Суммы равны
                        if (dirDiscountTabCollection[i].SumBegin == dirDiscountTabCollection[j].SumBegin)
                        {
                            ts.Rollback(); db.Database.Connection.Close();
                            throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg58);
                        }
                        //Сумма больше, а скидка меньше
                        if (
                            (dirDiscountTabCollection[i].SumBegin > dirDiscountTabCollection[j].SumBegin && dirDiscountTabCollection[i].Discount < dirDiscountTabCollection[j].Discount)
                            ||
                            (dirDiscountTabCollection[i].SumBegin < dirDiscountTabCollection[j].SumBegin && dirDiscountTabCollection[i].Discount > dirDiscountTabCollection[j].Discount)
                            )
                        {
                            ts.Rollback(); db.Database.Connection.Close();
                            throw new System.InvalidOperationException(Classes.Language.Sklad.Language.msg58_1 + " (" + dirDiscountTabCollection[i].SumBegin + "; " + dirDiscountTabCollection[i].Discount + "),  (" + dirDiscountTabCollection[j].SumBegin + "; " + dirDiscountTabCollection[j].Discount + ")");
                        }
                    }
                }



                //3.34 Сохраняем
                for (int i = 0; i < dirDiscountTabCollection.Count(); i++)
                {
                    dirDiscountTabCollection[i].DirDiscountID = Convert.ToInt32(dirDiscount.DirDiscountID);
                    db.Entry(dirDiscountTabCollection[i]).State = EntityState.Added;
                    db.SaveChanges();
                }
                //db.SaveChanges();

                #endregion



                #region 4. Подтверждение транзакции *** *** *** *** *** *** *** *** *** ***

                ts.Commit(); //.Complete();

                #endregion
            }
        }

        #endregion
    }
}