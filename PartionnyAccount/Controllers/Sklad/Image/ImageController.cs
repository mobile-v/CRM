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
using System.Windows.Forms;
using System.Drawing;

namespace PartionnyAccount.Controllers.Sklad.Image
{
    public class ImageController : ApiController
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
        Models.Sklad.Sys.SysSetting sysSetting = new Models.Sklad.Sys.SysSetting();

        int ListObjectID = 20;
        int ImagePixel = 250;

        #endregion


        #region UPDATE

        //Файл
        // POST: api/DirNomens
        [HttpPost]
        [ResponseType(typeof(DirNomen))]
        public async Task<IHttpActionResult> PostDirNomen(HttpRequestMessage request) //DirNomen dirNomen
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
            sysSetting = await db.SysSettings.FindAsync(1);

            #endregion


            try
            {

                #region Получеие параметров

                var paramList = request.GetQueryNameValuePairs();
                string param = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "param", true) == 0).Value;

                //Получаем "GenID"
                PartionnyAccount.Classes.Function.GenGenerate genGenerate = new Classes.Function.GenGenerate();
                string GenID = await genGenerate.ReturnGenID(db);

                //Сохраняем на диск
                string sLogo = "";

                //Для получения параметров и сохранение файла изображения
                //string root = System.Web.HttpContext.Current.Server.MapPath("~/Users/File/Images/DirNomenPhoto").ToString();
                string root = System.Web.HttpContext.Current.Server.MapPath("~/Users/user_" + field.DirCustomersID).ToString();
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider).ConfigureAwait(false);

                //Проверяем размер, если больше 150КБ, Удаляем и Эксепшен
                FileInfo file = new FileInfo(provider.FileData[0].LocalFileName);
                if (file.Length / 1024 > 150) { throw new System.InvalidOperationException("Превышен размер изображения! максимальный 150КБ."); }
                file = null;

                //Считываем сохранённый файл и перезаписываем его с нужным именем
                string
                    FileNameX = provider.FileData[0].Headers.ContentDisposition.FileName.Replace(@"""", ""), // чисто что бы проверить, выбран ли файл
                    FileNameY = @"\" + GenID + ".jpg"; //Используется при сохранениее
                if (FileNameX != "")
                {
                    if (File.Exists(root + FileNameY)) { throw new System.InvalidOperationException("Изображение товара с таким наименованием уже существует! Поменяйте наименование!"); }
                    else
                    {
                        if (param == "size40")
                        {
                            SavePictureFixed(provider.FileData[0].LocalFileName, root + FileNameY);
                        }
                        else
                        {
                            File.Copy(provider.FileData[0].LocalFileName, root + FileNameY);
                        }
                    }

                    //Получение самих параметров
                    sLogo = GenID; // root + @"/" + FileName;
                }
                File.Delete(provider.FileData[0].LocalFileName);

                #endregion


                #region Сохранение

                dynamic collectionWrapper = new
                {
                    SysGenID = GenID,
                    SysGenIDPatch = "/Users/user_" + field.DirCustomersID + "/" + FileNameY
                };
                return Ok(returnServer.Return(true, collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        public void SavePictureFixed(string LocalFileName, string FileName)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (System.Drawing.Image img = System.Drawing.Image.FromFile(LocalFileName))
                    {
                        double dH = ImagePixel * ((double)img.Height * (100 / (double)img.Width) / 100);
                        int iH = (int)dH;

                        using (Bitmap thumbBMP = new Bitmap(img, ImagePixel, iH))
                        {
                            thumbBMP.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }



        //Веб-камера
        [HttpPut]
        [ResponseType(typeof(DirNomen))]
        public async Task<IHttpActionResult> PutDirNomen(int id, DirNomen dirNomen, HttpRequestMessage request) //DirNomen dirNomen
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


            try
            {
                //Получаем "GenID"
                PartionnyAccount.Classes.Function.GenGenerate genGenerate = new Classes.Function.GenGenerate();
                string GenID = await genGenerate.ReturnGenID(db);

                //Для получения параметров и сохранение файла изображения
                string root = System.Web.HttpContext.Current.Server.MapPath("~/Users/user_" + field.DirCustomersID + "/").ToString();

                string FileNameY = @"/" + GenID + ".jpg"; //Используется при сохранениее


                byte[] bytes = Convert.FromBase64String(dirNomen.photoWebCam);
                System.Drawing.Image image;
                using (MemoryStream ms = new MemoryStream(bytes)) image = System.Drawing.Image.FromStream(ms);
                image.Save(root + FileNameY, System.Drawing.Imaging.ImageFormat.Jpeg);

                //Проверяем размер, если больше 150КБ, Удаляем и Эксепшен
                var fileLength = new FileInfo(root + FileNameY).Length;
                if (fileLength / 1024 > 150)
                {
                    File.Delete(root + FileNameY);
                    throw new System.InvalidOperationException("Превышен размер изображения! максимальный 150КБ.");
                }



                #region Сохранение

                dynamic collectionWrapper = new
                {
                    SysGenID = GenID,
                    SysGenIDPatch = "/Users/user_" + field.DirCustomersID + "/" + FileNameY
                };
                return Ok(returnServer.Return(true, collectionWrapper));

                #endregion

            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion
    }
}
