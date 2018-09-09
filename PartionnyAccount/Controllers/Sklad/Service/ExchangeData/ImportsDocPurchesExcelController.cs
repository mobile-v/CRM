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
using System.Data.SQLite;
using System.Web.Script.Serialization;
using System.Data.OleDb;
using System.IO;
using System.Collections;

namespace PartionnyAccount.Controllers.Sklad.Service.ExchangeData
{
    public class ImportsDocPurchesExcelController : ApiController
    {
        #region Classes

        Classes.Function.Exceptions.ExceptionEntry exceptionEntry = new Classes.Function.Exceptions.ExceptionEntry();
        Classes.Function.Variables.ConnectionString connectionString = new Classes.Function.Variables.ConnectionString();
        Classes.Account.Login login = new Classes.Account.Login();
        Classes.Account.AccessRight accessRight = new Classes.Account.AccessRight();
        Classes.Function.ReturnServer returnServer = new Classes.Function.ReturnServer();
        Classes.Function.Function function = new Classes.Function.Function();
        Models.Sklad.Sys.SysJourDisp sysJourDisp = new Models.Sklad.Sys.SysJourDisp(); Controllers.Sklad.Sys.SysJourDispsController sysJourDispsController = new Sys.SysJourDispsController();
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();
        Classes.Account.Login.Field field = new Classes.Account.Login.Field();
        private DbConnectionSklad db = new DbConnectionSklad();
        //private DbConnectionSklad dbRead = new DbConnectionSklad();

        //int ListObjectID = 38;

        string sheetName = "";
        int DirContractorIDOrg = 0, DirWarehouseID = 0, DirContractorID = 0;

        #endregion


        #region UPDATE

        public class Field1
        {
            public Int32 DirNomenID;
            public string DirNomenName;
            public string GroupList;
            public string DocDate;
            public int DirWarehouseID; //public string DirWarehouseName;
            public double Quantity;

            public int DirCharColourID; //public string DirCharColourName;
            public int DirCharTextureID; //public string DirCharColourName;
            public int DirCharSizeID; //public string DirCharColourName;

            public double PriceVAT;
            public double PriceCurrency;

            public double PriceRetailVAT;
            public double PriceRetailCurrency;

            public double PriceWholesaleVAT;
            public double PriceWholesaleCurrency;

            public double PriceIMVAT;
            public double PriceIMCurrency;
        }
        //int alDate_Count_i = 0;
        //Int32 kode = 0; //Код товара
        int? iMaxGroupID = 0;
        ArrayList alCodeNot = new ArrayList(); //Коды товара, которые не равны 5 символам

        // PUT: api/DirWarehouses/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public async Task<IHttpActionResult> PutDirWarehouse(HttpRequestMessage request) //HttpPostedFileBase upload
        {
            #region Проверяем Логин и Пароль + Изменяем строку соединения + Права + Разные Функции

            //Получаем Куку
            System.Web.HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["CookieIPOL"];

            // Проверяем Логин и Пароль
            field = await Task.Run(() => login.Return(authCookie, true));
            if (!field.Access) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg10));

            //Изменяем строку соединения
            db = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));
            //dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

            //Права (1 - Write, 2 - Read, 3 - No Access)
            /*
            int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightDocAccounts"));
            if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
            */
            if(field.DirEmployeeID != 1) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

            //Разные Функции
            function.NumberDecimalSeparator();

            //Получам настройки
            sysSetting = await db.SysSettings.FindAsync(1);

            //Получаем сотрудника: если к нему привязан Склад и/или Организация, то выбираем документы только по этим характеристикам
            Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(field.DirEmployeeID);

            #endregion

            #region Параметры

            //paramList -список параметров
            var paramList = request.GetQueryNameValuePairs();
            //Параметры
            sheetName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "sheetName", true) == 0).Value;
            DirContractorIDOrg = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorIDOrg", true) == 0).Value);
            DirContractorID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorID", true) == 0).Value);
            DirWarehouseID = Convert.ToInt32(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value);

            #endregion


            #region Сохранение

            OleDbConnection OleDbConn = null;
            try
            {
                //Алгоритм:
                //0. Проверка
                //1. Генерируем ID-шник "SysGens"
                //2. Получаем файл и сохраняем в папаке "Uploads" с именем: authCookie["CookieB"] + "_" + sysGen.SysGenID

                //3. Считываем Эксель файл
                //   [Код товара], [Категория], [Товар]

                //Получаем категорию "APPLE/ iPhone 4S/  Распродажа/  Распродажа Swarovski /"
                //Разделитель "/" и убираем первый пробел
                //Проверяем каждую получиную категорию: есть ли связка (Sub, Name)
                //Если нет - вносим категорию, а потом товар: ([Код товара], [Товар])
                //Если есть - вносим товар: ([Код товара], [Товар])


                // *** Важно *** *** ***
                //1.Находим максимальный код группы
                //2.Создаём коды групп (Макс + 1)
                //3.Создаём коды товаров(из Эксель)



                #region 0. Проверка *** *** *** *** *** *** ***

                if (!Request.Content.IsMimeMultipartContent()) Ok(returnServer.Return(false, "{" + "'msgType':'1', 'msg':'" + Classes.Language.Sklad.Language.msg57(0) + "'}"));

                #endregion


                #region 1. Генерируем ID-шник "SysGens" *** *** *** *** *** *** ***

                Models.Sklad.Sys.SysGen sysGen = new Models.Sklad.Sys.SysGen(); sysGen.SysGenDisc = ""; sysGen.SysGenID = null;
                //if (!ModelState.IsValid) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg91)); //return BadRequest(ModelState);
                db.Entry(sysGen).State = EntityState.Added;
                await db.SaveChangesAsync();

                #endregion


                #region 2. Получаем файл и сохраняем в папаке "Uploads" с именем: authCookie["CookieB"] + "_" + sysGen.SysGenID *** *** *** *** *** *** *** 

                string filePatch = "";
                var provider = new MultipartMemoryStreamProvider();
                string root = System.Web.HttpContext.Current.Server.MapPath("~/UsersTemp/FileStock/");
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    if (file.Headers.ContentDisposition.FileName != null)
                    {
                        var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                        var ext = Path.GetExtension(filename);
                        filePatch = root + field.DirCustomersID + "_" + sysGen.SysGenID + ext;

                        byte[] fileArray = await file.ReadAsByteArrayAsync();

                        using (System.IO.FileStream fs = new System.IO.FileStream(filePatch, System.IO.FileMode.Create)) //root + filename
                        {
                            await fs.WriteAsync(fileArray, 0, fileArray.Length);
                        }
                    }
                }

                #endregion


                #region 3. Получаем максимальный код группы

                var queryMaxGroupID = await Task.Run(() =>
                    (
                        from x in db.DirNomens
                        where x.DirNomenCategoryID == null
                        select x
                    ).MaxAsync(x => x.DirNomenID));

                iMaxGroupID = queryMaxGroupID + 1;
                if (iMaxGroupID == null || iMaxGroupID == 0) iMaxGroupID = 1;

                #endregion


                #region 4. Считываем Эксель файл

                if (filePatch.Length > 0)
                {
                    //1. Получаем категорию "APPLE/ iPhone 4S/  Распродажа/  Распродажа Swarovski /"
                    //2. Разделитель "/" и убираем первый пробел
                    //3. Проверяем каждую получиную категорию: есть ли связка (Sub, Name)
                    //4. Если нет - вносим категорию, а потом товар: ([Код товара], [Товар])
                    //5. Если есть - вносим товар: ([Код товара], [Товар])

                    string sExcelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePatch + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1;\""; //8.0
                    using (OleDbConn = new OleDbConnection(sExcelConnectionString))
                    {
                        OleDbConn.Open();

                        using (OleDbCommand OleDbCmd = new OleDbCommand("", OleDbConn))
                        {
                            #region 1. Таблица "Товар"

                            OleDbCmd.CommandText = "SELECT * FROM [" + sheetName + "$]";
                            OleDbCmd.Parameters.Clear();
                            using (OleDbDataReader dr = OleDbCmd.ExecuteReader())
                            {
                                using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                                {
                                    while (dr.Read())
                                    {
                                        if (dr["Код товара"].ToString().Length == 5)
                                        {
                                            //Read
                                            string DirNomenID = dr["Код товара"].ToString();
                                            string GroupList = dr["Категория"].ToString();
                                            string DirNomenName = dr["Товар"].ToString();

                                            //Create Group in database
                                            await Task.Run(() => GroupCreate(Convert.ToInt32(DirNomenID), GroupList, DirNomenName));
                                        }
                                        else
                                        {
                                            alCodeNot.Add(dr["Код товара"].ToString() + "  -  " + dr["Категория"].ToString() + "  -  " + dr["Товар"].ToString());

                                            //...
                                        }
                                    }

                                    ts.Commit();
                                }
                            }

                            #endregion


                            #region 2. Таблицы: Характеристики, Приходная накладная (Шапка + Спецификация), Остатки, Партии. (Новый алгоритм алгоритм: одна приходная накладная)



                            //1. Надо получить все точки из Эксель (GROUP BY)
                            //2. И делать SELECT по точкам, что бы сформировать приходные накладные по точкам
                            OleDbCmd.CommandText = "SELECT [Точка] FROM [" + sheetName + "$] GROUP BY [Точка] ORDER BY [Точка]";
                            OleDbCmd.Parameters.Clear();
                            ArrayList alDirWarehouseID = new ArrayList();
                            using (OleDbDataReader dr = OleDbCmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    alDirWarehouseID.Add(dr["Точка"].ToString());
                                }
                            }



                            //Формируем "Приходные накладные"
                            using (System.Data.Entity.DbContextTransaction ts = db.Database.BeginTransaction())
                            {
                                for (int i = 0; i < alDirWarehouseID.Count; i++)
                                {
                                    Models.Sklad.Doc.DocPurch docPurch = new Models.Sklad.Doc.DocPurch();


                                    ArrayList alWrite = new ArrayList();
                                    //OleDbCmd.CommandText = "SELECT * FROM [" + sheetName + "$] WHERE Дата=@pDate";
                                    OleDbCmd.CommandText = "SELECT * FROM [" + sheetName + "$] WHERE Точка=@Точка"; // ORDER BY [Код товара], [Дата] DESC
                                    OleDbCmd.Parameters.Clear();
                                    OleDbCmd.Parameters.AddWithValue("@Точка", alDirWarehouseID[i].ToString());

                                    using (OleDbDataReader dr = OleDbCmd.ExecuteReader())
                                    {
                                        while (dr.Read())
                                        {
                                            if (dr["Код товара"].ToString().Length == 5)
                                            {
                                                Field1 field1 = new Field1();
                                                field1.DirNomenID = Convert.ToInt32(dr["Код товара"].ToString());
                                                field1.DocDate = dr["Дата"].ToString();
                                                //field1.DirWarehouseID = ReturnDirWarehouseID(dr["Точка"].ToString());
                                                field1.DirWarehouseID = await Task.Run(() => ReturnDirWarehouseID(dr["Точка"].ToString()));
                                                field1.Quantity = Convert.ToInt32(dr["Остаток"].ToString());

                                                field1.PriceVAT = Convert.ToDouble(dr["Закуп цена за ед"].ToString());
                                                field1.PriceCurrency = Convert.ToDouble(dr["Закуп цена за ед"].ToString());

                                                field1.PriceRetailVAT = Convert.ToDouble(dr["Цена-1"].ToString());
                                                field1.PriceRetailCurrency = Convert.ToDouble(dr["Цена-1"].ToString());

                                                field1.PriceWholesaleVAT = Convert.ToDouble(dr["Цена-2"].ToString());
                                                field1.PriceWholesaleCurrency = Convert.ToDouble(dr["Цена-2"].ToString());

                                                field1.PriceIMVAT = Convert.ToDouble(dr["Цена-3"].ToString());
                                                field1.PriceIMCurrency = Convert.ToDouble(dr["Цена-3"].ToString());

                                                field1.DirCharColourID = ReturnDirCharColourID(dr["Поставщик"].ToString());
                                                field1.DirCharTextureID = ReturnDirCharTextureID(dr["Примечание"].ToString());

                                                alWrite.Add(field1);
                                            }
                                            else
                                            {
                                                //alCodeNot.Add(dr["Код товара"].ToString() + "  -  " + dr["Категория"].ToString() + "  -  " + dr["Товар"].ToString());

                                                //...
                                            }
                                        }
                                    }


                                    //Create Purchase documents and Remnants of goods in stock
                                    docPurch = await Task.Run(() => DocsCreate(alWrite));


                                    #region Чистим пустые партии товара, но только соотвутствующие Номеру документа, что бы НЕ удалить все пустые (0, 0)

                                    SQLiteParameter parDocID = new SQLiteParameter("@DocID", System.Data.DbType.Int32) { Value = docPurch.DocID };
                                    await db.Database.ExecuteSqlCommandAsync("DELETE FROM RemParties WHERE DocID=@DocID and Remnant=0; ", parDocID);

                                    #endregion

                                }


                                ts.Commit();
                            }

                            #endregion
                        }

                        OleDbConn.Close();
                    }

                }

                #endregion


                #region 5. Send

                dynamic collectionWrapper = new
                {
                    Msg = "Файл загружен!"
                };
                return Ok(returnServer.Return(true, collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                try { OleDbConn.Close(); OleDbConn.Dispose(); } catch { }
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex) + i777.ToString()));
            }

            #endregion
        }

        #endregion


        #region Mthods


        #region Создание Товара

        //Create Group in database
        int i777 = 0;
        private async Task<bool> GroupCreate(int DirNomenID, string GroupList, string DirNomenName)
        {
            //Ищим товар в БД
            // Если есть - пропускаем
            // Если нет, то:
            //  вносим все группы "GroupList"
            //  вносим товар в последнюю

            //Controllers.Sklad.Dir.DirNomens.DirNomensController dirNomensController = new Controllers.Sklad.Dir.DirNomens.DirNomensController();


            SQLiteParameter parDirNomenID = null, parSub = null, parDel = null, parDirNomenName = null, parDirNomenNameFull = null, parDirNomenTypeID = null, parDirNomenCategoryID = null;


            #region Ищим товар в БД

            var iCount = await
                (
                    from x in db.DirNomens
                    where x.DirNomenID == DirNomenID
                    select x
                ).CountAsync();

            //Есть такой Товар - выходим.
            if (iCount > 0) return false;

            #endregion


            #region Группа товара

            //Нет товара - вносим группы, если их нет
            //Парсим группу: "APPLE/ iPhone 4S/  Распродажа/  Распродажа Swarovski /"
            int? Sub = null; //Последняя подчинённая
            string[] GroupArray = GroupList.Split('/');
            for (int i = 0; i < GroupArray.Length; i++)
            {
                i777 = i;
                //Удаляем спереди пробелы
                string GroupName = GroupArray[i];

                if (GroupName.Length == 0) break;

                if (GroupName[0].ToString() == " ") GroupName = GroupName.Remove(0, 1);
                if (GroupName[0].ToString() == " ") GroupName = GroupName.Remove(0, 1);
                if (GroupName[0].ToString() == " ") GroupName = GroupName.Remove(0, 1);
                if (GroupName[0].ToString() == " ") GroupName = GroupName.Remove(0, 1);

                if (GroupName.Length == 0) break;

                //1. Первая записть - Корневая группа (Sub == 0)
                var query = await
                    (
                        from x in db.DirNomens
                        where x.DirNomenName.ToLower() == GroupName.ToLower() && x.Sub == Sub
                        select x
                    ).ToListAsync();

                if (query.Count() != 0)
                {
                    //Есть запись в БД - получаем "Sub" для следующих групп
                    Sub = Convert.ToInt32(query[0].DirNomenID);
                }
                else
                {
                    //Нет записи в БД - создаём новую группу и получаем Sub
                    Models.Sklad.Dir.DirNomen dirNomenGroup = new Models.Sklad.Dir.DirNomen();
                    /*
                    dirNomenGroup.DirNomenID = iMaxGroupID; iMaxGroupID++; // null;
                    if (Sub > 0) dirNomenGroup.Sub = Sub;
                    dirNomenGroup.Del = false;
                    dirNomenGroup.DirNomenName = GroupName;
                    dirNomenGroup.DirNomenNameFull = GroupName;
                    dirNomenGroup.DirNomenTypeID = 1;

                    dirNomenGroup = await Task.Run(() => dirNomensController.mPutPostDirNomen(db, dirNomenGroup, EntityState.Added));
                    */

                    parDirNomenID = new SQLiteParameter("@DirNomenID", System.Data.DbType.Int32) { Value = iMaxGroupID }; 
                    if (Sub > 0) parSub = new SQLiteParameter("@Sub", System.Data.DbType.Int32) { Value = Sub };
                    else parSub = new SQLiteParameter("@Sub", System.Data.DbType.Int32) { Value = null };
                    parDel = new SQLiteParameter("@Del", System.Data.DbType.Boolean) { Value = false };
                    parDirNomenName = new SQLiteParameter("@DirNomenName", System.Data.DbType.String) { Value = GroupName };
                    parDirNomenNameFull = new SQLiteParameter("@DirNomenNameFull", System.Data.DbType.String) { Value = GroupName };
                    parDirNomenTypeID = new SQLiteParameter("@DirNomenTypeID", System.Data.DbType.Int32) { Value = 1 };
                    db.Database.ExecuteSqlCommand("INSERT INTO DirNomens (DirNomenID, Sub, Del, DirNomenName, DirNomenNameFull, DirNomenTypeID)values(@DirNomenID, @Sub, @Del, @DirNomenName, @DirNomenNameFull, @DirNomenTypeID);", parDirNomenID, parSub, parDel, parDirNomenName, parDirNomenNameFull, parDirNomenTypeID);

                    Sub = Convert.ToInt32(iMaxGroupID); //Sub = Convert.ToInt32(dirNomenGroup.DirNomenID);
                    iMaxGroupID++;
                }
            }

            #endregion


            #region  Создаём категорию товара

            //Create Categories in database
            Models.Sklad.Dir.DirNomenCategory dirNomenCategory = await Task.Run(() => CategoriesCreate(DirNomenName));

            #endregion


            #region Создаём Товар
            
            /*
            Models.Sklad.Dir.DirNomen dirNomen = new Models.Sklad.Dir.DirNomen();
            //dirNomen.DirNomenID = null;
            dirNomen.DirNomenID = DirNomenID;
            //dirNomen.DirNomenArticle = DirNomenID.ToString();
            if (Sub > 0) dirNomen.Sub = Sub;
            dirNomen.Del = false;
            dirNomen.DirNomenName = DirNomenName;
            dirNomen.DirNomenNameFull = DirNomenName;
            dirNomen.DirNomenTypeID = 1;
            dirNomen.DirNomenCategoryID = dirNomenCategory.DirNomenCategoryID;
            //dirNomen.IsGoods = true;

            dirNomen = await Task.Run(() => dirNomensController.mPutPostDirNomen(db, dirNomen, EntityState.Added));
            */
            
            //Запись в БД
            parDirNomenID = new SQLiteParameter("@DirNomenID", System.Data.DbType.Int32) { Value = DirNomenID };
            parSub = new SQLiteParameter("@Sub", System.Data.DbType.Int32) { Value = Sub };
            parDel = new SQLiteParameter("@Del", System.Data.DbType.Boolean) { Value = false };
            parDirNomenName = new SQLiteParameter("@DirNomenName", System.Data.DbType.String) { Value = DirNomenName };
            parDirNomenNameFull = new SQLiteParameter("@DirNomenNameFull", System.Data.DbType.String) { Value = DirNomenName };
            parDirNomenTypeID = new SQLiteParameter("@DirNomenTypeID", System.Data.DbType.Int32) { Value = 1 };
            parDirNomenCategoryID = new SQLiteParameter("@DirNomenCategoryID", System.Data.DbType.String) { Value = dirNomenCategory.DirNomenCategoryID };
            db.Database.ExecuteSqlCommand("INSERT INTO DirNomens (DirNomenID, Sub, Del, DirNomenName, DirNomenNameFull, DirNomenTypeID, DirNomenCategoryID)values(@DirNomenID, @Sub, @Del, @DirNomenName, @DirNomenNameFull, @DirNomenTypeID, @DirNomenCategoryID);", parDirNomenID, parSub, parDel, parDirNomenName, parDirNomenNameFull, parDirNomenTypeID, parDirNomenCategoryID);

            #endregion

            //Это не нужно!
            //Sub = Convert.ToInt32(dirNomen.DirNomenID);

            return true;
        }

        //Create Categories in database
        private async Task<Models.Sklad.Dir.DirNomenCategory> CategoriesCreate(string DirNomenName)
        {
            Models.Sklad.Dir.DirNomenCategory dirNomenCategory = new Models.Sklad.Dir.DirNomenCategory();
            PartionnyAccount.Controllers.Sklad.Dir.DirNomens.DirNomenCategoriesController dirNomenCategoriesController = new Dir.DirNomens.DirNomenCategoriesController();

            var queryCat = await
            (
                from x in db.DirNomenCategories
                where x.DirNomenCategoryName.ToLower() == DirNomenName.ToLower()
                select x
            ).ToListAsync();

            //Есть такой Товар - выходим.
            if (queryCat.Count() > 0)
            {
                dirNomenCategory.DirNomenCategoryID = queryCat[0].DirNomenCategoryID;
                dirNomenCategory.DirNomenCategoryName = queryCat[0].DirNomenCategoryName;
            }
            else
            {
                dirNomenCategory.DirNomenCategoryName = DirNomenName;
                dirNomenCategory = await Task.Run(() => dirNomenCategoriesController.mPutPostDirNomenCategories(db, dirNomenCategory, EntityState.Added));
            }

            return dirNomenCategory;
        }

        #endregion



        #region Создание Документа

        //Create Purchase documents and Remnants of goods in stock
        private async Task<DocPurch> DocsCreate(ArrayList alWrite)
        {
            if (alWrite.Count == 0) return null;


            db.Configuration.AutoDetectChangesEnabled = false;

            //Во всех записях есть данные для Doc, DocPurch и DocPurchTab
            Field1 field10 = (Field1)alWrite[0];


            #region Таблица Doc *** *** ***

            Models.Sklad.Doc.DocPurch docPurch = new Models.Sklad.Doc.DocPurch();
            docPurch.DocID = null;
            docPurch.Base = "Создано на основании импорта из файла Excel";
            docPurch.Del = false;
            docPurch.DirEmployeeID = field.DirEmployeeID;
            docPurch.Discount = 0;
            docPurch.DocDate = DateTime.Now.AddDays(-30); //Convert.ToDateTime(field10.DocDate);
            docPurch.DocIDBase = null;
            docPurch.Held = true;
            docPurch.IsImport = true;
            docPurch.ListObjectID = 1;

            #endregion


            #region Таблица DocPurch *** *** ***

            docPurch.DocPurchID = null;
            docPurch.NumberInt = "";
            docPurch.NumberTT = "";
            docPurch.NumberTax = "";
            docPurch.DirContractorIDOrg = DirContractorIDOrg;
            docPurch.DirContractorID = DirContractorID;
            docPurch.DirWarehouseID = field10.DirWarehouseID;
            docPurch.DirVatValue = 0;
            docPurch.DirPaymentTypeID = 1;

            #endregion


            #region Таблица DocPurchTab *** *** ***

            //Создаём коллекцию спецификации
            Models.Sklad.Doc.DocPurchTab[] docPurchTabCollection = new Models.Sklad.Doc.DocPurchTab[alWrite.Count];
            for (int i = 0; i < alWrite.Count; i++)
            {
                Field1 field1 = (Field1)alWrite[i];

                Models.Sklad.Doc.DocPurchTab docPurchTab = new Models.Sklad.Doc.DocPurchTab();
                docPurchTab.DocPurchID = 0;
                docPurchTab.DocPurchTabID = null;

                docPurchTab.DirNomenID = ReturnDirNomenID(Convert.ToInt32(field1.DirNomenID));
                docPurchTab.DirCurrencyID = 1;
                docPurchTab.DirCurrencyMultiplicity = 1;
                docPurchTab.DirCurrencyRate = 1;
                //docPurchTab.DirVatValue = 0;

                if (field1.DirCharColourID > 0) docPurchTab.DirCharColourID = field1.DirCharColourID;
                if (field1.DirCharTextureID > 0) docPurchTab.DirCharTextureID = field1.DirCharTextureID;
                if (field1.DirCharSizeID > 0) docPurchTab.DirCharSizeID = field1.DirCharSizeID;

                docPurchTab.Quantity = field1.Quantity; //field1.Quantity;

                docPurchTab.PriceVAT = field1.PriceVAT;
                docPurchTab.PriceCurrency = field1.PriceVAT;

                docPurchTab.PriceRetailVAT = field1.PriceRetailVAT;
                docPurchTab.PriceRetailCurrency = field1.PriceRetailCurrency;

                docPurchTab.PriceWholesaleVAT = field1.PriceWholesaleVAT;
                docPurchTab.PriceWholesaleCurrency = field1.PriceWholesaleCurrency;

                docPurchTab.PriceIMVAT = field1.PriceIMVAT;
                docPurchTab.PriceIMCurrency = field1.PriceIMCurrency;

                docPurchTabCollection[i] = docPurchTab;
            }

            #endregion


            #region Save

            Doc.DocPurches.DocPurchesController docPurchesController = new Doc.DocPurches.DocPurchesController();
            docPurch = await Task.Run(() => docPurchesController.mPutPostDocPurch(db, db, "held", sysSetting, docPurch, EntityState.Added, docPurchTabCollection, field)); //await Task.Run(() => docPurchesController.mPutPostDocPurch(db, dbRead, "held", sysSetting, docPurch, EntityState.Added, docPurchTabCollection, field));

            #endregion


            return docPurch;
        }


        //Находим Товар по "DirNomenID"
        private int ReturnDirNomenID(Int32 DirNomenID)
        {
            //Есть "DirNomenID"
            var query =
                (
                    from x in db.DirNomens
                    where x.DirNomenID == DirNomenID
                    select x
                ).ToList();

            if (query.Count() > 0) return Convert.ToInt32(query[0].DirNomenID);
            else throw new System.InvalidOperationException("Не найден товар по DirNomenID=" + DirNomenID);
        }

        //Находим или созлаём новый Склад
        //private int ReturnDirWarehouseID(string DirWarehouseName)
        internal async Task<int> ReturnDirWarehouseID(string DirWarehouseName)
        {
            //Есть склад "DirWarehouseName"
            var query =
                (
                    from x in db.DirWarehouses
                    where x.DirWarehouseName == DirWarehouseName
                    select x
                ).ToList();

            if (query.Count() > 0) return Convert.ToInt32(query[0].DirWarehouseID);

            //Нет, тогда создаём новый
            //Нет записи в БД - создаём новую группу и получаем Sub
            Models.Sklad.Dir.DirWarehouse dirWarehouse = new Models.Sklad.Dir.DirWarehouse();
            dirWarehouse.DirWarehouseID = null;
            dirWarehouse.DirWarehouseName = DirWarehouseName;
            //dirWarehouse.Sub = 0;
            dirWarehouse.Del = false;
            dirWarehouse.DirCashOfficeID = 1;
            dirWarehouse.DirBankID = 1;


            //db.Entry(dirWarehouse).State = EntityState.Added;
            //db.SaveChanges();
            PartionnyAccount.Controllers.Sklad.Dir.DirWarehouses.DirWarehousesController dirWarehousesController = new Dir.DirWarehouses.DirWarehousesController();
            dirWarehouse = await Task.Run(() => dirWarehousesController.mPutPostDirWarehouse(db, dirWarehouse, EntityState.Added, field));

            return Convert.ToInt32(dirWarehouse.DirWarehouseID);
        }
        //Находим или созлаём новыую Характеристику "Цвет"
        private int ReturnDirCharColourID(string DirCharColourName)
        {
            if (DirCharColourName.Length == 0) return 0;

            //Есть "DirCharColourName"
            var query =
                (
                    from x in db.DirCharColours
                    where x.DirCharColourName.ToLower() == DirCharColourName.ToLower()
                    select x
                ).ToList();

            if (query.Count() > 0) return Convert.ToInt32(query[0].DirCharColourID);

            //Нет, тогда создаём новый
            //Нет записи в БД - создаём новую группу и получаем Sub
            Models.Sklad.Dir.DirCharColour dirCharColour = new Models.Sklad.Dir.DirCharColour();
            dirCharColour.DirCharColourID = null;
            dirCharColour.DirCharColourName = DirCharColourName;

            db.Entry(dirCharColour).State = EntityState.Added;
            db.SaveChanges();

            return Convert.ToInt32(dirCharColour.DirCharColourID);
        }
        //Находим или созлаём новыую Характеристику "Текстура"
        private int ReturnDirCharTextureID(string DirCharTextureName)
        {
            if (DirCharTextureName.Length == 0) return 0;

            //Есть "DirCharTextureName"
            var query =
                (
                    from x in db.DirCharTextures
                    where x.DirCharTextureName.ToLower() == DirCharTextureName.ToLower()
                    select x
                ).ToList();

            if (query.Count() > 0) return Convert.ToInt32(query[0].DirCharTextureID);

            //Нет, тогда создаём новый
            Models.Sklad.Dir.DirCharTexture dirCharTexture = new Models.Sklad.Dir.DirCharTexture();
            dirCharTexture.DirCharTextureID = null;
            dirCharTexture.DirCharTextureName = DirCharTextureName;

            db.Entry(dirCharTexture).State = EntityState.Added;
            db.SaveChanges();

            return Convert.ToInt32(dirCharTexture.DirCharTextureID);
        }

        #endregion


        #endregion
    }
}
