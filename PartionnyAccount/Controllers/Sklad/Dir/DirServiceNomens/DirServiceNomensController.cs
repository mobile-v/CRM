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
using System.Data.SQLite;
using System.Web.Script.Serialization;

namespace PartionnyAccount.Controllers.Sklad.Dir.DirServiceNomens
{
    public class DirServiceNomensController : ApiController
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
        private DbConnectionSklad dbRead = new DbConnectionSklad();

        int ListObjectID = 43;

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
        }
        // GET: api/DirServiceNomens
        public async Task<IHttpActionResult> GetDirServiceNomen(HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirServiceNomens"));
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
                _params.limit = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.GroupID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "pGroupID", true) == 0).Value); //Кликнули по группе
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск
                _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value); //Склад для Остатков

                _params.type = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "type", true) == 0).Value;
                _params.node = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "node", true) == 0).Value;
                _params.XGroupID_NotShow = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "XGroupID_NotShow", true) == 0).Value);

                #endregion


                if (_params.type == "Grid")
                {
                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from x in db.DirServiceNomens
                            select new
                            {
                                DirServiceNomenID = x.DirServiceNomenID,
                                Del = x.Del,
                                Sub = x.Sub,
                                DirServiceNomenName = x.DirServiceNomenName,

                                leaf =
                                 (
                                  from y in db.DirNomens
                                  where y.Sub == x.DirServiceNomenID
                                  select y
                                 ).Count() == 0 ? true : false,
                            }
                        );

                    #endregion


                    #region Условия (параметры) *** *** ***


                    #region Группа

                    if (_params.GroupID < 1)
                    {
                        query = query.Where(x => x.Sub == null);
                    }
                    else
                    {
                        query = query.Where(x => x.Sub == _params.GroupID);
                    }

                    #endregion


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
                            query = query.Where(x => x.DirServiceNomenID == iNumber32 || x.DirServiceNomenName.Contains(_params.parSearch));
                        }
                        else
                        {
                            query = query.Where(x => x.DirServiceNomenName.Contains(_params.parSearch));
                        }
                    }

                    #endregion


                    #region OrderBy и Лимит

                    //query = query.OrderBy(x => x.DirServiceNomenName).Skip(_params.Skip).Take(_params.limit);
                    query = query.OrderBy(x => x.leaf).ThenBy(y => y.DirServiceNomenName);

                    #endregion


                    #endregion


                    #region Отправка JSON

                    //К-во Номенклатуры
                    int dirCount = await Task.Run(() => db.DirServiceNomens.Count());

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount2 = query.Count();
                    if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirServiceNomen = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    if (_params.node == "DirServiceNomen")
                    {
                        #region Отобразить только "Руты" *** *** ***

                        var query =
                            (
                             from x in db.DirServiceNomens

                                 /*join dirServiceNomenCategories1 in db.DirServiceNomenCategories on x.DirServiceNomenCategoryID equals dirServiceNomenCategories1.DirServiceNomenCategoryID into dirServiceNomenCategories2
                                 from dirServiceNomenCategories in dirServiceNomenCategories2.DefaultIfEmpty()*/

                             where x.Sub == null && x.DirServiceNomenID != _params.XGroupID_NotShow
                             select new
                             {
                                 id = x.DirServiceNomenID,
                                 sub = x.Sub,
                                 text = x.DirServiceNomenName,
                                 leaf =
                                 (
                                  from y in db.DirServiceNomens
                                  where y.Sub == x.DirServiceNomenID
                                  select y
                                 ).Count() == 0 ? true : false,

                                 Del = x.Del,

                                 /*DirServiceNomenCategoryID = x.DirServiceNomenCategoryID,
                                 DirServiceNomenCategoryName = dirServiceNomenCategories.DirServiceNomenCategoryName,*/

                                 //Полный путь от группы к выбраному элементу
                                 DirServiceNomenPatchFull = x.DirServiceNomenName
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

                        #region OrderBy и Лимит

                        query = query.OrderBy(x => x.leaf).ThenBy(y => y.text);

                        #endregion


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
                    else
                    {
                        #region Кликнули по Ветке - отобразить подчинённые

                        int iNode = Convert.ToInt32(_params.node);

                        //Получить "категорию + наименование" для "iNode" всех рутов
                        string DirServiceNomenPatchFull = await Task.Run(() => DirServiceNomenSubFind2(db, iNode));

                        var query =
                            (
                             from x in db.DirServiceNomens

                                 //join remRemnants1 in db.RemRemnants on x.DirServiceNomenID equals remRemnants1.DirServiceNomenID into remRemnants2
                                 //from remRemnants in remRemnants2.Where(x => x.DirWarehouseID == _params.DirWarehouseID).DefaultIfEmpty()

                             where x.Sub == iNode && x.DirServiceNomenID != _params.XGroupID_NotShow
                             select new
                             {
                                 id = x.DirServiceNomenID,
                                 sub = x.Sub,
                                 text = x.DirServiceNomenName,
                                 leaf =
                                 (
                                  from y in db.DirServiceNomens
                                  where y.Sub == x.DirServiceNomenID
                                  select y
                                 ).Count() == 0 ? 1 : 0,

                                 Del = x.Del,
                                 Sub = x.Sub,

                                 //Полный путь от группы к выбраному элементу
                                 DirServiceNomenPatchFull = DirServiceNomenPatchFull + x.DirServiceNomenName,

                                 //Остаток
                                 //Remains = remRemnants.Quantity == null ? 0 : remRemnants.Quantity
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

                        #region OrderBy и Лимит

                        query = query.OrderBy(x => x.leaf).ThenBy(y => y.text);

                        #endregion


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
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // GET: api/DirServiceNomens/5
        [ResponseType(typeof(DirServiceNomen))]
        public async Task<IHttpActionResult> GetDirServiceNomen(int id, HttpRequestMessage request)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirServiceNomens"));
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

                        from x in db.DirServiceNomens

                        where x.DirServiceNomenID == id
                        select x
                        /*new
                        {
                            //1. Оснавные === === === === === === === === === === ===

                            DirServiceNomenID = x.DirServiceNomenID,
                            Sub = x.Sub,
                            Del = x.Del,
                            DirServiceNomenName = x.DirServiceNomenName,
                            DirServiceNomenNameFull = x.DirServiceNomenNameFull,
                            Description = x.Description,
                            DescriptionFull = x.DescriptionFull
                        }*/

                    ).ToListAsync());


                if (query.Count() > 0)
                {
                    return Ok(returnServer.Return(true, query[0]));
                }
                else
                {
                    return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));
                }

                //return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }



        // GET: api/DirServiceNomens/5
        [ResponseType(typeof(DirServiceNomen))]
        public async Task<IHttpActionResult> GetDirServiceNomen(string pSearch, int iPriznak)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirServiceNomens"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion


                #region Поиск

                //Цель всех запросов получить "ID" и "Sub" (Sub - что бы раскрыть все ветки)
                int? iID = 0;
                int? iSub = 0;


                int value;
                if (int.TryParse(pSearch, out value))
                {
                    iID = Convert.ToInt32(pSearch);

                    var query = db.DirServiceNomens.Where(x => x.DirServiceNomenID == iID).ToList();
                    if (query.Count() > 0) iSub = query[0].Sub;

                }
                else
                {
                    //Значить это артикул или наименование!

                    var query = db.DirServiceNomens.Where(x => x.NameFullLower.Contains(pSearch)).ToList();
                    if (query.Count() > 0)
                    {
                        iID = query[0].DirServiceNomenID;
                        iSub = query[0].Sub;
                    }
                }

                #endregion

                #region Отправка JSON

                if (iSub > 0)
                {

                    //Получаем Sub-бы (нужны поледние 5-ть)
                    ArrayList Subs = await Task.Run(() => DirServiceNomenSubFind(iSub));

                    int ID0 = 0, ID1 = 0, ID2 = 0, ID3 = 0, ID4 = 0;
                    if (Subs.Count > 0)
                    {
                        if (Subs.Count > 0) ID0 = Convert.ToInt32(Subs[0]);
                        if (Subs.Count > 1) ID1 = Convert.ToInt32(Subs[1]);
                        if (Subs.Count > 2) ID2 = Convert.ToInt32(Subs[2]);
                        if (Subs.Count > 3) ID3 = Convert.ToInt32(Subs[3]);
                        if (Subs.Count > 4) ID4 = Convert.ToInt32(Subs[4]);
                    }

                    dynamic collectionWrapper = new
                    {
                        ID = iID,
                        ID0 = ID0,
                        ID1 = ID1,
                        ID2 = ID2,
                        ID3 = ID3,
                        ID4 = ID4,
                    };
                    return Ok(returnServer.Return(true, collectionWrapper));
                }
                else
                {
                    return Ok(returnServer.Return(true, -1));
                }

                #endregion


                //return Ok(returnServer.Return(true, "Нет данных!"));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion


        #region UPDATE

        // PUT: api/DirServiceNomens/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirServiceNomen(int id, DirServiceNomen dirServiceNomen)
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));
            dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirServiceNomens"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            //Получаем колекцию "Табличную часть"
            Models.Sklad.Dir.DirServiceNomenPrice[] dirServiceNomenPriceCollection = null;
            if (!String.IsNullOrEmpty(dirServiceNomen.recordsDirServiceNomenPriceGrid))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dirServiceNomenPriceCollection = serializer.Deserialize<Models.Sklad.Dir.DirServiceNomenPrice[]>(dirServiceNomen.recordsDirServiceNomenPriceGrid);
            }

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirServiceNomen.DirServiceNomenID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            dirServiceNomen.Substitute();

            #endregion


            #region Сохранение

            try
            {
                /*db.Entry(dirServiceNomen).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());*/

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        await Task.Run(() => mPutPostDirServiceNomen(db, sysSetting, dirServiceNomen, EntityState.Modified, dirServiceNomenPriceCollection)); //sysSetting
                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }



                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirServiceNomen.DirServiceNomenID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                //Получаем Sub-бы (нужны поледние 5-ть)
                ArrayList Subs = await Task.Run(() => DirServiceNomenSubFind(dirServiceNomen.Sub));

                int ID0 = 0, ID1 = 0, ID2 = 0, ID3 = 0, ID4 = 0;
                if (Subs.Count > 0)
                {
                    if (Subs.Count > 0) ID0 = Convert.ToInt32(Subs[0]);
                    if (Subs.Count > 1) ID1 = Convert.ToInt32(Subs[1]);
                    if (Subs.Count > 2) ID2 = Convert.ToInt32(Subs[2]);
                    if (Subs.Count > 3) ID3 = Convert.ToInt32(Subs[3]);
                    if (Subs.Count > 4) ID4 = Convert.ToInt32(Subs[4]);
                }

                dynamic collectionWrapper = new
                {
                    ID = dirServiceNomen.DirServiceNomenID,
                    ID0 = ID0,
                    ID1 = ID1,
                    ID2 = ID2,
                    ID3 = ID3,
                    ID4 = ID4,
                };
                return Ok(returnServer.Return(true, collectionWrapper));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }
        // PUT: api/DirServiceNomens/5
        [ResponseType(typeof(DirServiceNomen))]
        [HttpPut]
        public async Task<IHttpActionResult> PutDirServiceNomen(int id, int? sub)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirServiceNomens"));
                if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                #endregion
                
                #region Проверки

                //NULL - нельзя передать!!!
                if (sub == 0) sub = null;

                //"Перемещать єлемент сам в себя запрещено!"
                if (id == sub) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));

                //Ещё проверку сделать:
                //Что бы не внести в подчинённые записи (До 7-го уровня)!
                if (sub != 0 && sub != null)
                {
                    Models.Sklad.Dir.DirServiceNomen dirServiceNomenSub = await db.DirServiceNomens.FindAsync(sub);
                    if (dirServiceNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                    else if (dirServiceNomenSub.Sub != null)
                    {
                        dirServiceNomenSub = await db.DirServiceNomens.FindAsync(dirServiceNomenSub.Sub);
                        if (dirServiceNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                        else if (dirServiceNomenSub.Sub != null)
                        {
                            dirServiceNomenSub = await db.DirServiceNomens.FindAsync(dirServiceNomenSub.Sub);
                            if (dirServiceNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                            else if (dirServiceNomenSub.Sub != null)
                            {
                                dirServiceNomenSub = await db.DirServiceNomens.FindAsync(dirServiceNomenSub.Sub);
                                if (dirServiceNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                                else if (dirServiceNomenSub.Sub != null)
                                {
                                    dirServiceNomenSub = await db.DirServiceNomens.FindAsync(dirServiceNomenSub.Sub);
                                    if (dirServiceNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                                    else if (dirServiceNomenSub.Sub != null)
                                    {
                                        dirServiceNomenSub = await db.DirServiceNomens.FindAsync(dirServiceNomenSub.Sub);
                                        if (dirServiceNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                                        else if (dirServiceNomenSub.Sub != null)
                                        {
                                            dirServiceNomenSub = await db.DirServiceNomens.FindAsync(dirServiceNomenSub.Sub);
                                            if (dirServiceNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                                            else if (dirServiceNomenSub.Sub != null)
                                            {
                                                //dirServiceNomenSub = await db.DirServiceNomens.FindAsync(dirServiceNomenSub.Sub);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion


                #region Сохранение


                Models.Sklad.Dir.DirServiceNomen dirServiceNomen = await db.DirServiceNomens.FindAsync(id);
                dirServiceNomen.Sub = sub;

                db.Entry(dirServiceNomen).State = EntityState.Modified;
                await db.SaveChangesAsync();


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = id;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                dynamic collectionWrapper = new
                {
                    ID = dirServiceNomen.DirServiceNomenID
                };
                return Ok(returnServer.Return(true, collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DirServiceNomens
        [ResponseType(typeof(DirServiceNomen))]
        public async Task<IHttpActionResult> PostDirServiceNomen(DirServiceNomen dirServiceNomen)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirServiceNomens"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Параметры

            //Получаем колекцию "Табличную часть"
            Models.Sklad.Dir.DirServiceNomenPrice[] dirServiceNomenPriceCollection = null;
            if (!String.IsNullOrEmpty(dirServiceNomen.recordsDirServiceNomenPriceGrid))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dirServiceNomenPriceCollection = serializer.Deserialize<Models.Sklad.Dir.DirServiceNomenPrice[]>(dirServiceNomen.recordsDirServiceNomenPriceGrid);
            }

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            dirServiceNomen.Substitute();

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                /*db.Entry(dirServiceNomen).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());*/

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        //Используем метод, что бы было всё в одном потоке
                        await Task.Run(() => mPutPostDirServiceNomen(db, sysSetting, dirServiceNomen, EntityState.Added, dirServiceNomenPriceCollection)); //sysSetting
                        ts.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { ts.Rollback(); ts.Dispose(); } catch { }
                        try { db.Database.Connection.Close(); db.Database.Connection.Dispose(); } catch { }

                        return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
                    }
                }



                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Добавление записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirServiceNomen.DirServiceNomenID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                //Получаем Sub-бы (нужны поледние 5-ть)
                ArrayList Subs = await Task.Run(() => DirServiceNomenSubFind(dirServiceNomen.Sub));

                int ID0 = 0, ID1 = 0, ID2 = 0, ID3 = 0, ID4 = 0;
                if (Subs.Count > 0)
                {
                    if (Subs.Count > 0) ID0 = Convert.ToInt32(Subs[0]);
                    if (Subs.Count > 1) ID1 = Convert.ToInt32(Subs[1]);
                    if (Subs.Count > 2) ID2 = Convert.ToInt32(Subs[2]);
                    if (Subs.Count > 3) ID3 = Convert.ToInt32(Subs[3]);
                    if (Subs.Count > 4) ID4 = Convert.ToInt32(Subs[4]);
                }

                dynamic collectionWrapper = new
                {
                    ID = dirServiceNomen.DirServiceNomenID,
                    ID0 = ID0,
                    ID1 = ID1,
                    ID2 = ID2,
                    ID3 = ID3,
                    ID4 = ID4,
                };
                return Ok(returnServer.Return(true, collectionWrapper));
            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }

            #endregion
        }

        // DELETE: api/DirServiceNomens/5
        [ResponseType(typeof(DirServiceNomen))]
        public async Task<IHttpActionResult> DeleteDirServiceNomen(int id)
        {
            //Удаляем, если исключение "FK", то ставим пометку на удаление и сообщаем об этом клиенту
            //...

            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            Classes.Account.Login.Field field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права (1 - Write, 2 - Read, 3 - No Access)
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirServiceNomens"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Удаление

            try
            {
                //Получаем Sub-бы (нужны поледние 5-ть)
                int? Sub = await Task.Run(() => DirServiceNomenID_Sub_Find(id));

                int ID0 = 0, ID1 = 0, ID2 = 0, ID3 = 0, ID4 = 0;
                if (Sub != null && Sub > 0)
                {
                    ArrayList Subs = await Task.Run(() => DirServiceNomenSubFind(Sub));
                    if (Subs.Count > 0)
                    {
                        if (Subs.Count > 0) ID0 = Convert.ToInt32(Subs[0]);
                        if (Subs.Count > 1) ID1 = Convert.ToInt32(Subs[1]);
                        if (Subs.Count > 2) ID2 = Convert.ToInt32(Subs[2]);
                        if (Subs.Count > 3) ID3 = Convert.ToInt32(Subs[3]);
                        if (Subs.Count > 4) ID4 = Convert.ToInt32(Subs[4]);
                    }
                }



                DirServiceNomen dirServiceNomen = await db.DirServiceNomens.FindAsync(id);
                if (dirServiceNomen == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                if (!dirServiceNomen.Del)
                {
                    // === Удаляем === === === === === 


                    #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                    Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                    sysJourDisp.DirDispOperationID = 5; //Удаление записи
                    sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                    sysJourDisp.ListObjectID = ListObjectID;
                    sysJourDisp.TableFieldID = dirServiceNomen.DirServiceNomenID;
                    sysJourDisp.Description = "";
                    try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                    #endregion


                    //1. Удаляем
                    try
                    {
                        db.DirServiceNomens.Remove(dirServiceNomen);
                        await db.SaveChangesAsync();

                        dynamic collectionWrapper = new
                        {
                            ID = dirServiceNomen.DirServiceNomenID,
                            ID0 = ID0,
                            ID1 = ID1,
                            ID2 = ID2,
                            ID3 = ID3,
                            ID4 = ID4,
                            Msg = Classes.Language.Sklad.Language.msg19
                        };
                        return Ok(returnServer.Return(true, collectionWrapper));
                    }
                    catch (Exception ex)
                    {
                        if (function.ExceptionFkExist(ex))
                        {
                            //2. Исключение - пометка на удаление
                            dirServiceNomen.Del = true;

                            db.Entry(dirServiceNomen).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            dynamic collectionWrapper = new
                            {
                                ID = dirServiceNomen.DirServiceNomenID,
                                ID0 = ID0,
                                ID1 = ID1,
                                ID2 = ID2,
                                ID3 = ID3,
                                ID4 = ID4,
                                Msg = Classes.Language.Sklad.Language.msg96 //"Помечено на удаление, так как запись задействована в других объектах сервиса (напр. в документах)."
                            };
                            return Ok(returnServer.Return(true, collectionWrapper));
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

                    dirServiceNomen.Del = false;

                    db.Entry(dirServiceNomen).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    dynamic collectionWrapper = new
                    {
                        ID = dirServiceNomen.DirServiceNomenID,
                        ID0 = ID0,
                        ID1 = ID1,
                        ID2 = ID2,
                        ID3 = ID3,
                        ID4 = ID4,
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

        private bool DirServiceNomenExists(int id)
        {
            return db.DirServiceNomens.Count(e => e.DirServiceNomenID == id) > 0;
        }


        //Ищем по ID - Sub
        private async Task<int?> DirServiceNomenID_Sub_Find(int id)
        {
            var query =
                await Task.Run(() =>
                db.DirServiceNomens.Where(x => x.DirServiceNomenID == id).FirstAsync()
                );

            return query.Sub;
        }

        //По DirServiceNomenID (ID) находим всех родителей (Sub-ы)!
        //Используется в UPDATE
        private async Task<ArrayList> DirServiceNomenSubFind(int? Sub)
        {
            ArrayList ret = new ArrayList();
            ret.Add(Sub);

            while (Sub > 0)
            {
                var query =
                    await Task.Run(() =>
                    db.DirServiceNomens.Where(x => x.DirServiceNomenID == Sub).FirstAsync()
                    );

                Sub = query.Sub;
                ret.Add(Sub);
            }

            return ret;
        }


        //Находим "Категорию + Имя" всех родителей
        //Используется в SELECT
        internal async Task<string> DirServiceNomenSubFind2(DbConnectionSklad db, int? id)
        {
            ArrayList alName = new ArrayList();
            int? Sub = id;

            while (Sub > 0)
            {
                var query = await Task.Run(() =>
                     (
                         from x in db.DirServiceNomens
                         where x.DirServiceNomenID == Sub
                         select new
                         {
                             id = x.DirServiceNomenID,
                             sub = x.Sub,
                             text = x.DirServiceNomenName, // + " (" + x.DirServiceNomenName + ")",
                             leaf =
                             (
                              from y in db.DirServiceNomens
                              where y.Sub == x.DirServiceNomenID
                              select y
                             ).Count() == 0 ? 1 : 0,

                             Del = x.Del,
                             Sub = x.Sub,

                             //Полный путь от группы к выбраному элементу
                             DirServiceNomenPatchFull = x.DirServiceNomenName // + " (" + x.DirServiceNomenName + ")"
                         }
                    ).ToListAsync());

                if (query.Count() > 0)
                {
                    id = query[0].id;
                    Sub = query[0].Sub;
                    alName.Add(query[0].text + " / ");
                }
                else
                {
                    Sub = null;
                }
            }

            string ret = "";
            for (int i = alName.Count - 1; i >= 0; i--)
            {
                ret += alName[i].ToString();
            }

            return ret;
        }



        //UpdateDirServiceNomen
        //SaveAccountBasic
        private void mPutPostDirServiceNomen(
            DbConnectionSklad db,
            Models.Sklad.Sys.SysSetting sysSettings,
            DirServiceNomen dirServiceNomen,
            //bool InsertUpdate, //true - Insert, false - Update
            EntityState entityState, //EntityState.Added, Modified
            Models.Sklad.Dir.DirServiceNomenPrice[] dirServiceNomenPriceCollection
            )
        {
            #region 1. DirServiceNomen *** *** *** *** *** *** *** *** *** ***

            db.Entry(dirServiceNomen).State = entityState;
            db.SaveChanges();

            #endregion


            #region 2. ImportToIM: Использовать в И-М?

            if (entityState == EntityState.Modified)
            {
                ImportToIMChangeSub(dirServiceNomen.DirServiceNomenID, dirServiceNomen.ImportToIM);
            }

            #endregion


            #region n. DirServiceNomenPrice - Not Work

            /*
            //2.1. Удаляем записи в БД, если UPDATE
            if (entityState == EntityState.Modified)
            {
                SQLiteParameter parDirServiceNomenID = new SQLiteParameter("@DirServiceNomenID", System.Data.DbType.Int32) { Value = dirServiceNomen.DirServiceNomenID };
                db.Database.ExecuteSqlCommand("DELETE FROM DirServiceNomenPrices WHERE DirServiceNomenID=@DirServiceNomenID;", parDirServiceNomenID);
            }

            //2.2. Проставляем ID-шник "DirServiceNomenID" для всех позиций спецификации
            for (int i = 0; i < dirServiceNomenPriceCollection.Count(); i++)
            {
                dirServiceNomenPriceCollection[i].DirServiceNomenID = Convert.ToInt32(dirServiceNomen.DirServiceNomenID);
                dirServiceNomenPriceCollection[i].DirCurrencyID = sysSettings.DirCurrencyID;
                db.Entry(dirServiceNomenPriceCollection[i]).State = EntityState.Added;
            }
            db.SaveChanges();
            */

            #endregion
        }

        private void ImportToIMChangeSub(int? DirServiceNomenID, bool ImportToIM)
        {

            var SubX =
                (
                    from o1 in db.DirServiceNomens
                    where
                    //o1.ImportToIM == !ImportToIM && 
                    o1.Sub == DirServiceNomenID
                    select o1.DirServiceNomenID
                ); //.ToList();

            var SubSubX =
                (
                    from o1 in db.DirServiceNomens
                    where
                    //o1.ImportToIM == !ImportToIM && 
                    SubX.Contains(o1.Sub)
                    select o1.DirServiceNomenID
                ); //.ToList();


            var query =
            (
                 from x in db.DirServiceNomens
                 where
                     SubX.Contains(x.DirServiceNomenID) ||
                     SubSubX.Contains(x.DirServiceNomenID)
                 select x
            );


            foreach (Models.Sklad.Dir.DirServiceNomen dirServiceNomen in query)
            {
                dirServiceNomen.ImportToIM = ImportToIM;
                db.Entry(dirServiceNomen).State = EntityState.Modified;
            }
            db.SaveChanges();
            
        }

        #endregion

    }
}