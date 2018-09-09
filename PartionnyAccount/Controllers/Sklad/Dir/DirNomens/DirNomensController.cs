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
using System.Data.SQLite;
using System.IO;
using System.Web.Script.Serialization;

namespace PartionnyAccount.Controllers.Sklad.Dir.DirNomens
{
    public class DirNomensController : ApiController
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

        int ListObjectID = 24;

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
        // GET: api/DirNomens
        public async Task<IHttpActionResult> GetDirNomens(HttpRequestMessage request)
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
                _params.limit = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "limit", true) == 0).Value); //Записей на страницу
                _params.page = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "page", true) == 0).Value);   //Номер страницы
                _params.Skip = _params.limit * (_params.page - 1);
                _params.GroupID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "GroupID", true) == 0).Value); //Кликнули по группе
                _params.parSearch = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "parSearch", true) == 0).Value; if (_params.parSearch != null) _params.parSearch = _params.parSearch.ToLower(); //Поиск

                //Склад для Остатков
                try { _params.DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value); }
                catch { _params.DirWarehouseID = 0; }

                _params.type = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "type", true) == 0).Value;
                _params.node = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "node", true) == 0).Value;
                _params.XGroupID_NotShow = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "XGroupID_NotShow", true) == 0).Value);

                #endregion


                if (_params.type == "Grid")
                {
                    #region Основной запрос *** *** ***

                    var query =
                        (
                            from x in db.DirNomens
                            select new
                            {
                                DirNomenID = x.DirNomenID,
                                Del = x.Del,
                                Sub = x.Sub,
                                DirNomenName = x.DirNomenName,
                                DirNomenTypeName = x.dirNomenType.DirNomenTypeName,

                                leaf =
                                 (
                                  from y in db.DirNomens
                                  where y.Sub == x.DirNomenID
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
                            query = query.Where(x => x.DirNomenID == iNumber32 || x.DirNomenName.Contains(_params.parSearch));
                        }
                        else
                        {
                            query = query.Where(x => x.DirNomenName.Contains(_params.parSearch));
                        }
                    }

                    #endregion


                    #region OrderBy и Лимит

                    //query = query.OrderBy(x => x.DirNomenName).Skip(_params.Skip).Take(_params.limit);
                    query = query.OrderBy(x => x.leaf).ThenBy(y => y.DirNomenName);

                    #endregion


                    #endregion


                    #region Отправка JSON

                    //К-во Номенклатуры
                    //int dirCount = await Task.Run(() => db.DirNomens.Count());

                    //А вдруг к-во меньше Лимита, тогда показать не общее к-во, а реальное!
                    int dirCount = query.Count();
                    //if (dirCount2 < _params.limit) dirCount = _params.limit * (_params.page - 1) + dirCount2;
                    //if (dirCount2 < dirCount) dirCount = _params.limit * (_params.page - 1) + dirCount2;

                    dynamic collectionWrapper = new
                    {
                        sucess = true,
                        total = dirCount,
                        DirNomen = query
                    };
                    return await Task.Run(() => Ok(collectionWrapper));

                    #endregion
                }
                else //Tree
                {
                    if (_params.node == "DirNomen" || _params.node == "root")
                    {
                        #region Отобразить только "Руты" *** *** ***

                        var query =
                            (
                             from x in db.DirNomens

                             /*join dirNomenCategories1 in db.DirNomenCategories on x.DirNomenCategoryID equals dirNomenCategories1.DirNomenCategoryID into dirNomenCategories2
                             from dirNomenCategories in dirNomenCategories2.DefaultIfEmpty()*/

                             where x.Sub == null && x.DirNomenID != _params.XGroupID_NotShow
                             select new
                             {
                                 id = x.DirNomenID,
                                 sub = x.Sub,
                                 text = x.DirNomenName,
                                 leaf =
                                 (
                                  from y in db.DirNomens
                                  where y.Sub == x.DirNomenID
                                  select y
                                 ).Count() == 0 ? true : false,

                                 Del = x.Del,

                                 DirNomenCategoryID = x.DirNomenCategoryID,
                                 // DirNomenCategoryName = x.dirNomenCategory.DirNomenCategoryName,

                                 //Полный путь от группы к выбраному элементу
                                 DirNomenPatchFull = x.DirNomenName,
                                 DirNomenPatchFull2 = ""
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


                        string[] ret = await Task.Run(() => DirNomenSubFind2(db, iNode));
                        //Получить "категорию + наименование" для "iNode" всех рутов
                        string DirNomenPatchFull = ret[0];
                        //Для заказов (Комбы)
                        string DirNomenPatchFull2 = ret[1];
                        //Получить все id-шники
                        string DirNomenIDFull = ret[2];

                        string[] separators = { "," };
                        string[] words = ret[2].Split(separators, StringSplitOptions.RemoveEmptyEntries);


                        var query =
                            (
                             from x in db.DirNomens

                             //join remRemnants1 in db.RemRemnants on x.DirNomenID equals remRemnants1.DirNomenID into remRemnants2
                             //from remRemnants in remRemnants2.Where(x => x.DirWarehouseID == _params.DirWarehouseID).DefaultIfEmpty()

                             where x.Sub == iNode && x.DirNomenID != _params.XGroupID_NotShow
                             select new
                             {
                                 id = x.DirNomenID,
                                 sub = x.Sub,

                                 //text = x.DirNomenName + " (" + x.DirNomenID_OLD + ")",
                                 text =
                                 x.DirNomenID_OLD == null ? x.DirNomenName : x.DirNomenName + " (" + x.DirNomenID_OLD + ")",

                                 leaf =
                                 (
                                  from y in db.DirNomens
                                  where y.Sub == x.DirNomenID
                                  select y
                                 ).Count() == 0 ? 1 : 0,

                                 Del = x.Del,
                                 Sub = x.Sub,

                                 DirNomenCategoryID = x.DirNomenCategoryID,
                                 // DirNomenCategoryName = x.dirNomenCategory.DirNomenCategoryName,

                                 //Полный путь от группы к выбраному элементу
                                 DirNomenPatchFull = DirNomenPatchFull + x.DirNomenName,
                                 DirNomenPatchFull2 = DirNomenPatchFull,
                                 //Все id-шники - для Заказов (отображать в Комбах)
                                 DirNomenIDFull = DirNomenIDFull + x.DirNomenID,


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

        // GET: api/DirNomens/5
        [ResponseType(typeof(DirNomen))]
        public async Task<IHttpActionResult> GetDirNomen(int id, HttpRequestMessage request)
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

                        from x in db.DirNomens

                        join dirNomenCategories1 in db.DirNomenCategories on x.DirNomenCategoryID equals dirNomenCategories1.DirNomenCategoryID into dirNomenCategories2
                        from dirNomenCategories in dirNomenCategories2.DefaultIfEmpty()

                        where x.DirNomenID == id
                        select new
                        {
                            DirNomenID = x.DirNomenID,
                            DirNomenID_INSERT = x.DirNomenID,
                            DirNomenID_OLD = x.DirNomenID_OLD,

                            Sub = x.Sub,
                            Del = x.Del,
                            DirNomenName = x.DirNomenName,
                            DirNomenArticle = x.DirNomenArticle,

                            DirNomenTypeID = x.DirNomenTypeID,
                            DirNomenTypeName = x.dirNomenType.DirNomenTypeName,

                            DirNomenCategoryID = x.DirNomenCategoryID,
                            DirNomenCategoryName = dirNomenCategories.DirNomenCategoryName,

                            DirNomenNameFull = x.DirNomenNameFull,
                            Description = x.Description,
                            DescriptionFull = x.DescriptionFull,
                            ImageLink = x.ImageLink,


                            //Image
                            //0.
                            SysGenID = x.SysGenID,
                            //SysGenIDPatch = @"UsersTemp/UserImage/" + field.DirCustomersID + "_" + x.SysGenID + ".jpg"
                            SysGenIDPatch = x.SysGenID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGenID + ".jpg",
                            //1.
                            SysGen1ID = x.SysGen1ID,
                            SysGen1IDPatch = x.SysGen1ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen1ID + ".jpg",
                            //2.
                            SysGen2ID = x.SysGen2ID,
                            SysGen2IDPatch = x.SysGen2ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen2ID + ".jpg",
                            //3.
                            SysGen3ID = x.SysGen3ID,
                            SysGen3IDPatch = x.SysGen3ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen3ID + ".jpg",
                            //4.
                            SysGen4ID = x.SysGen4ID,
                            SysGen4IDPatch = x.SysGen4ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen4ID + ".jpg",
                            //5.
                            SysGen5ID = x.SysGen5ID,
                            SysGen5IDPatch = x.SysGen5ID == null ? "" :
                            @"/Users/user_" + field.DirCustomersID + "/" + x.SysGen5ID + ".jpg",


                            //Импорт в ИМ
                            ImportToIM = x.ImportToIM,
                            //ККМ
                            KKMSTax = x.KKMSTax,


                            DirNomenNameURL = x.DirNomenNameURL,
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

                //return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg89));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }



        // GET: api/DirNomens/5
        [ResponseType(typeof(DirNomen))]
        public async Task<IHttpActionResult> GetDirNomen(string pSearch, int iPriznak)
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
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirNomens"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                */
                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);
                
                #endregion


                #region Поиск

                //Цель всех запросов получить "ID" и "Sub" (Sub - что бы раскрыть все ветки)
                int? iID = 0
                , iID_= 0;
                int? iSub = 0;


                #region OLD
                /*
                int value;
                if (int.TryParse(pSearch, out value))
                {
                    iID = Convert.ToInt32(pSearch);

                    var query = await db.DirNomens.Where(x => x.DirNomenID == iID || x.DirNomenID_OLD == iID).ToListAsync();
                    if (query.Count() > 0) iSub = query[0].Sub;

                }
                else
                {
                    //Значить это артикул или наименование!

                    var query = db.DirNomens.Where(x => x.DirNomenArticle.Contains(pSearch) || x.NameFullLower.Contains(pSearch)).ToList();
                    if (query.Count() > 0)
                    {
                        iID = query[0].DirNomenID;
                        iSub = query[0].Sub;
                    }
                }
                */
                #endregion


                //Число или текст
                bool IsInt = false;
                int value;
                if (int.TryParse(pSearch, out value)) { IsInt = true; iID_ = Convert.ToInt32(pSearch); }


                if (iPriznak == 1 && IsInt) //В товаре (код)
                {
                    var query = await db.DirNomens.Where(x => x.DirNomenID == iID_).ToListAsync();
                    if (query.Count() > 0)
                    {
                        iID = query[0].DirNomenID;
                        iSub = query[0].Sub;
                    }
                }
                else if (iPriznak == 2 && IsInt) //В товаре (старый код)
                {
                    var query = await db.DirNomens.Where(x => x.DirNomenID_OLD == iID_).ToListAsync();
                    if (query.Count() > 0)
                    {
                        iID = query[0].DirNomenID;
                        iSub = query[0].Sub;
                    }
                }
                else if (iPriznak == 3) //В товаре (артикул)
                {
                    //var query = await db.DirNomens.Where(x => x.DirNomenArticle.Contains(pSearch)).ToListAsync();
                    var query = await db.DirNomens.Where(x => x.DirNomenArticle == pSearch).ToListAsync();
                    if (query.Count() > 0)
                    {
                        iID = query[0].DirNomenID;
                        iSub = query[0].Sub;
                    }
                }
                else if (iPriznak == 4) //В товаре (наименование)
                {
                    string pSearch_ = pSearch.ToLower();
                    var query = await db.DirNomens.Where(x => x.NameLower.Contains(pSearch_)).ToListAsync();
                    if (query.Count() > 0)
                    {
                        iID = query[0].DirNomenID;
                        iSub = query[0].Sub;
                    }
                }
                else if (iPriznak == 5) //В товаре (наименование)
                {
                    string pSearch_ = pSearch.ToLower();
                    var query = await db.DirNomens.Where(x => x.NameFullLower.Contains(pSearch_)).ToListAsync();
                    if (query.Count() > 0)
                    {
                        iID = query[0].DirNomenID;
                        iSub = query[0].Sub;
                    }
                }


                #endregion

                #region Отправка JSON

                if (iSub > 0)
                {

                    //Получаем Sub-бы (нужны поледние 5-ть)
                    ArrayList Subs = await Task.Run(() => DirNomenSubFind(iSub));

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

        // PUT: api/DirNomens/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDirNomen(int id, DirNomen dirNomen)
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
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
            if (id != dirNomen.DirNomenID) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg8)); //return BadRequest();

            //Слеш "/" в наименовании - эксепш
            if (dirNomen.DirNomenName.IndexOf("/") > -1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg124));

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            dirNomen.Substitute();

            #endregion


            #region Сохранение

            try
            {
                /*
                db.Entry(dirNomen).State = EntityState.Modified;
                await Task.Run(() => db.SaveChangesAsync());
                */
                dirNomen = await Task.Run(() => mPutPostDirNomen(db, dirNomen, EntityState.Modified));


                #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                sysJourDisp.DirDispOperationID = 4; //Изменение записи
                sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                sysJourDisp.ListObjectID = ListObjectID;
                sysJourDisp.TableFieldID = dirNomen.DirNomenID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                //Получаем Sub-бы (нужны поледние 5-ть)
                ArrayList Subs = await Task.Run(() => DirNomenSubFind(dirNomen.Sub));

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
                    ID = dirNomen.DirNomenID,
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
        // PUT: api/DirNomens/5
        [ResponseType(typeof(DirNomen))]
        [HttpPut]
        public async Task<IHttpActionResult> PutDirNomen(int id, int? sub)
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
                    Models.Sklad.Dir.DirNomen dirNomenSub = await db.DirNomens.FindAsync(sub);
                    if (dirNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                    else if (dirNomenSub.Sub != null)
                    {
                        dirNomenSub = await db.DirNomens.FindAsync(dirNomenSub.Sub);
                        if (dirNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                        else if (dirNomenSub.Sub != null)
                        {
                            dirNomenSub = await db.DirNomens.FindAsync(dirNomenSub.Sub);
                            if (dirNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                            else if (dirNomenSub.Sub != null)
                            {
                                dirNomenSub = await db.DirNomens.FindAsync(dirNomenSub.Sub);
                                if (dirNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                                else if (dirNomenSub.Sub != null)
                                {
                                    dirNomenSub = await db.DirNomens.FindAsync(dirNomenSub.Sub);
                                    if (dirNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                                    else if (dirNomenSub.Sub != null)
                                    {
                                        dirNomenSub = await db.DirNomens.FindAsync(dirNomenSub.Sub);
                                        if (dirNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                                        else if (dirNomenSub.Sub != null)
                                        {
                                            dirNomenSub = await db.DirNomens.FindAsync(dirNomenSub.Sub);
                                            if (dirNomenSub.Sub == id) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg100));
                                            else if (dirNomenSub.Sub != null)
                                            {
                                                //dirNomenSub = await db.DirNomens.FindAsync(dirNomenSub.Sub);
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


                Models.Sklad.Dir.DirNomen dirNomen = await db.DirNomens.FindAsync(id);
                dirNomen.Sub = sub;

                /*
                db.Entry(dirNomen).State = EntityState.Modified;
                await db.SaveChangesAsync();
                */
                dirNomen = await Task.Run(() => mPutPostDirNomen(db, dirNomen, EntityState.Modified));


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
                    ID = dirNomen.DirNomenID
                };
                return Ok(returnServer.Return(true, collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        // POST: api/DirNomens
        [ResponseType(typeof(DirNomen))]
        public async Task<IHttpActionResult> PostDirNomen(DirNomen dirNomen)
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
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion

            #region Проверки

            //Если надо создать новый товар с наперёд заданым номером
            //dirNomen.DirNomenID_INSERT = dirNomen.DirNomenID;
            dirNomen.DirNomenID = null;
            if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);

            //Слеш "/" в наименовании - эксепш
            if (dirNomen.DirNomenName.IndexOf("/") > -1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg124));

            //Подстановки - некоторые поля надо заполнить, если они не заполены
            dirNomen.Substitute();

            #endregion


            #region Сохранение

            try
            {
                //Используем метод, что бы было всё в одном потоке
                /*
                db.Entry(dirNomen).State = EntityState.Added;
                await Task.Run(() => db.SaveChangesAsync());
                */

                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                {
                    db.Configuration.AutoDetectChangesEnabled = false;

                    try
                    {
                        dirNomen = await Task.Run(() => mPutPostDirNomen(db, dirNomen, EntityState.Added));
                        ts.Commit(); //.Complete();
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
                sysJourDisp.TableFieldID = dirNomen.DirNomenID;
                sysJourDisp.Description = "";
                try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                #endregion


                //Получаем Sub-бы (нужны поледние 5-ть)
                ArrayList Subs = await Task.Run(() => DirNomenSubFind(dirNomen.Sub));

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
                    ID = dirNomen.DirNomenID,
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

        // DELETE: api/DirNomens/5
        [ResponseType(typeof(DirNomen))]
        public async Task<IHttpActionResult> DeleteDirNomen(int id)
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
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDirNomens"));
            if (iRight != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

            #endregion


            #region  !!! !!! !!! ОПАСНО !!! !!! !!! 
            // !!! !!! !!! ОПАСНО !!! !!! !!! 
            //Удаляем все записи в "DirNomens" - ОПАСНО
            /*for (int f = 0; f < 25; f++)
            {
                //db.DirNomens.Remove(dirNomen); await db.SaveChangesAsync();
                //int? Sub = await Task.Run(() => DirNomenID_Sub_Find(id));
                var queryXXX =
                    (
                        from x in db.DirNomens
                        select x
                    ).OrderByDescending(x => x.Sub).ToList();
                for (int c = 0; c < queryXXX.Count(); c++)
                {
                    int? DirNomenIDXXX = queryXXX[c].DirNomenID;
                    Models.Sklad.Dir.DirNomen dirNomenXXX = db.DirNomens.Find(DirNomenIDXXX);
                    try
                    {
                        db.DirNomens.Remove(dirNomenXXX); await db.SaveChangesAsync();
                    }
                    catch { }
                }
            }
            return Ok(returnServer.Return(false, "!!!!!"));
            */
            // !!! !!! !!! ОПАСНО !!! !!! !!! 
            #endregion


            #region Удаление

            try
            {
                //Получаем Sub-бы (нужны поледние 5-ть)
                int? Sub = await Task.Run(() => DirNomenID_Sub_Find(id));

                int ID0 = 0, ID1 = 0, ID2 = 0, ID3 = 0, ID4 = 0;
                if (Sub != null && Sub > 0)
                {
                    ArrayList Subs = await Task.Run(() => DirNomenSubFind(Sub));
                    if (Subs.Count > 0)
                    {
                        if (Subs.Count > 0) ID0 = Convert.ToInt32(Subs[0]);
                        if (Subs.Count > 1) ID1 = Convert.ToInt32(Subs[1]);
                        if (Subs.Count > 2) ID2 = Convert.ToInt32(Subs[2]);
                        if (Subs.Count > 3) ID3 = Convert.ToInt32(Subs[3]);
                        if (Subs.Count > 4) ID4 = Convert.ToInt32(Subs[4]);
                    }
                }



                DirNomen dirNomen = await db.DirNomens.FindAsync(id);
                if (dirNomen == null) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg99));

                if (!dirNomen.Del)
                {
                    // === Удаляем === === === === === 


                    #region 6. JourDisp *** *** *** *** *** *** *** *** *** *

                    Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp();
                    sysJourDisp.DirDispOperationID = 5; //Удаление записи
                    sysJourDisp.DirEmployeeID = field.DirEmployeeID;
                    sysJourDisp.ListObjectID = ListObjectID;
                    sysJourDisp.TableFieldID = dirNomen.DirNomenID;
                    sysJourDisp.Description = "";
                    try { sysJourDispsController.mPutPostSysJourDisps(db, sysJourDisp, EntityState.Added); } catch (Exception ex) { }

                    #endregion


                    //1. Удаляем
                    try
                    {
                        db.DirNomens.Remove(dirNomen);
                        await db.SaveChangesAsync();

                        dynamic collectionWrapper = new
                        {
                            ID = dirNomen.DirNomenID,
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
                            //DirNomen dirNomen2 = await db.DirNomens.FindAsync(id);
                            db.Entry(dirNomen).Reload();
                            dirNomen.Del = true;

                            db.Entry(dirNomen).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            dynamic collectionWrapper = new
                            {
                                ID = dirNomen.DirNomenID,
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

                    dirNomen.Del = false;

                    db.Entry(dirNomen).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    dynamic collectionWrapper = new
                    {
                        ID = dirNomen.DirNomenID,
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

        private bool DirNomenExists(int id)
        {
            return db.DirNomens.Count(e => e.DirNomenID == id) > 0;
        }


        //Ищем по ID - Sub
        private async Task<int?> DirNomenID_Sub_Find(int id)
        {
            var query =
                await Task.Run(() =>
                db.DirNomens.Where(x => x.DirNomenID == id).FirstAsync()
                );

            return query.Sub;
        }

        //По DirNomenID (ID) находим всех родителей (Sub-ы)!
        //Используется в UPDATE
        private async Task<ArrayList> DirNomenSubFind(int? Sub)
        {
            ArrayList ret = new ArrayList();
            ret.Add(Sub);

            int i = 0;
            while (Sub > 0)
            {
                var query =
                    await Task.Run(() =>
                    db.DirNomens.Where(x => x.DirNomenID == Sub).FirstAsync()
                    );

                Sub = query.Sub;
                ret.Add(Sub);

                i++;
                if (i > 25) break;
            }

            return ret;
        }


        //Находим "Категорию + Имя" всех родителей + Получить все id-шники (Используется в SELECT)
        internal async Task<string[]> DirNomenSubFind2(DbConnectionSklad db, int? id)
        {
            ArrayList alNameSpase = new ArrayList();
            ArrayList alNameSpaseNo = new ArrayList();
            ArrayList alNameID = new ArrayList();

            int? Sub = id;

            while (Sub > 0)
            {
                var query = await Task.Run(() =>
                     (
                         from x in db.DirNomens
                         where x.DirNomenID == Sub
                         select new
                         {
                             id = x.DirNomenID,
                             sub = x.Sub,
                             text = x.DirNomenName, // + " (" + x.DirNomenName + ")",
                             leaf =
                             (
                              from y in db.DirNomens
                              where y.Sub == x.DirNomenID
                              select y
                             ).Count() == 0 ? 1 : 0,

                             Del = x.Del,
                             Sub = x.Sub,

                             //Полный путь от группы к выбраному элементу
                             DirNomenPatchFull = x.DirNomenName // + " (" + x.DirNomenName + ")"
                         }
                    ).ToListAsync());

                if (query.Count() > 0)
                {
                    id = query[0].id;
                    Sub = query[0].Sub;
                    alNameSpase.Add(query[0].text + " / ");
                    alNameSpaseNo.Add(query[0].text + ",");
                    alNameID.Add(query[0].id + ",");
                }
                else
                {
                    Sub = null;
                }
            }

            string[] ret = new string[3];
            for (int i = alNameSpase.Count - 1; i >= 0; i--)
            {
                ret[0] += alNameSpase[i].ToString();
                ret[1] += alNameSpaseNo[i].ToString();
                ret[2] += alNameID[i].ToString();
            }

            return ret;
        }


        internal async Task<DirNomen> mPutPostDirNomen(
                    DbConnectionSklad db,
                    DirNomen dirNomen,
                    EntityState entityState //EntityState.Added, Modified
                    )
        {
            db.Entry(dirNomen).State = entityState;
            await db.SaveChangesAsync();

            //Если надо создать новый товар с наперёд заданым номером
            if (dirNomen.DirNomenID_INSERT != null && dirNomen.DirNomenID_INSERT > 0)
            {
                string SQL = "UPDATE DirNomens SET DirNomenID=@DirNomenID_INSERT WHERE DirNomenID=@DirNomenID; ";
                SQLiteParameter parDirNomenID_INSERT = new SQLiteParameter("@DirNomenID_INSERT", System.Data.DbType.Int32) { Value = dirNomen.DirNomenID_INSERT };
                SQLiteParameter parDirNomenID = new SQLiteParameter("@DirNomenID", System.Data.DbType.Int32) { Value = dirNomen.DirNomenID };
                await Task.Run(() => db.Database.ExecuteSqlCommandAsync(SQL, parDirNomenID_INSERT, parDirNomenID));

                dirNomen.DirNomenID = dirNomen.DirNomenID_INSERT;
            }

            //if (dirNomen.DirNomenArticle == null) dirNomen.DirNomenArticle = dirNomen.DirNomenID.ToString();

            return dirNomen;
        }

        #endregion
    }
}