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
using System.Collections;

namespace PartionnyAccount.Controllers.Sklad.Report
{
    public class ReportLogisticsController : ApiController
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

        int ListObjectID = 32;

        #endregion


        #region SELECT

        int DirContractorIDOrg = 0, DirEmployeeID = 0, DirMovementStatusID = 0, DocOrTab = 1;
        string DirContractorNameOrg, DirEmployeeName, DirMovementStatusName;
        DateTime DateS, DatePo;
        ArrayList alDirNomenPatchFull = new ArrayList();

        // GET: api/DocSales
        public async Task<IHttpActionResult> GetDocSales(HttpRequestMessage request)
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
                dbRead = new DbConnectionSklad(connectionString.Return(field.DirCustomersID, null, true));

                //Права (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightReportLogistics"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));

                //Разные Функции
                function.NumberDecimalSeparator();

                //Получам настройки
                Models.Sklad.Sys.SysSetting sysSetting = await db.SysSettings.FindAsync(1);

                //Получаем сотрудника: если к нему привязан Склад и/или Организация, то выбираем документы только по этим характеристикам
                Models.Sklad.Dir.DirEmployee dirEmployee = await db.DirEmployees.FindAsync(field.DirEmployeeID);

                #endregion


                #region Параметры

                //paramList -список параметров
                var paramList = request.GetQueryNameValuePairs();

                DateS = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DateS", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (DateS < Convert.ToDateTime("01.01.1800")) DateS = Convert.ToDateTime(sysSetting.JurDateS.ToString("yyyy-MM-dd 00:00:00")).AddDays(-1);
                else DateS = DateS.AddDays(-1);

                DatePo = Convert.ToDateTime(Convert.ToDateTime(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DatePo", true) == 0).Value).ToString("yyyy-MM-dd 23:59:59"));
                if (DatePo < Convert.ToDateTime("01.01.1800")) DatePo = Convert.ToDateTime(sysSetting.JurDatePo.ToString("yyyy-MM-dd 23:59:59"));

                DirContractorIDOrg = 0;
                bool bDirContractorIDOrg = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorIDOrg", true) == 0).Value, out DirContractorIDOrg);
                DirContractorNameOrg = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorNameOrg", true) == 0).Value;

                DirEmployeeID = 0;
                bool bDirEmployeeID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value, out DirEmployeeID);
                DirEmployeeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeName", true) == 0).Value;

                DirMovementStatusID = 0;
                bool bDirMovementStatusID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirMovementStatusID", true) == 0).Value, out DirMovementStatusID);
                DirMovementStatusName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirMovementStatusName", true) == 0).Value;

                DocOrTab = 1;
                bool bDocOrTab = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DocOrTab", true) == 0).Value, out DocOrTab);

                #endregion



                if (DocOrTab == 1)
                {

                    #region query

                    var queryTemp =
                    (
                        from docMovements in db.DocMovements

                        where
                            docMovements.doc.DocDate >= DateS && docMovements.doc.DocDate <= DatePo &&
                            docMovements.DirMovementStatusID > 1

                        select new
                        {
                            //№
                            DocMovementID = docMovements.DocMovementID,
                            //Дата
                            DocDate = docMovements.doc.DocDate,

                            //Точка - откуда
                            DirWarehouseIDFrom = docMovements.DirWarehouseIDFrom,
                            DirWarehouseNameFrom = docMovements.dirWarehouseFrom.DirWarehouseName,
                            //Курьер
                            DirEmployeeIDCourier = docMovements.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docMovements.dirEmployee_Courier.DirEmployeeName,
                            //Курьер
                            DirWarehouseIDTo = docMovements.DirWarehouseIDTo,
                            DirWarehouseNameTo = docMovements.dirWarehouseTo.DirWarehouseName,

                            DirMovementStatusID = docMovements.DirMovementStatusID,
                            DirMovementStatusName = docMovements.dirMovementStatus.DirMovementStatusName,
                        }
                    );

                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeIDCourier == DirEmployeeID);
                    if (DirMovementStatusID > 0) queryTemp = queryTemp.Where(z => z.DirMovementStatusID == DirMovementStatusID);
                    else queryTemp = queryTemp.Where(z => z.DirMovementStatusID <= 3);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper1 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportLogistics = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper1));

                    #endregion
                }
                else
                {

                    #region query

                    var queryTemp =
                    (
                        from docMovements in db.DocMovements
                        from docMovementTabs in db.DocMovementTabs


                        join dirNomens11 in db.DirNomens on docMovementTabs.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()


                            #region Характеристики
                        join dirCharColours1 in db.DirCharColours on docMovementTabs.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on docMovementTabs.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on docMovementTabs.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on docMovementTabs.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on docMovementTabs.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on docMovementTabs.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on docMovementTabs.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on docMovementTabs.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                            #endregion


                        where
                                    docMovements.DocMovementID == docMovementTabs.DocMovementID &&
                                    docMovements.doc.DocDate >= DateS && docMovements.doc.DocDate <= DatePo &&
                                    docMovements.DirMovementStatusID > 1

                        select new
                        {
                            DocMovementID = docMovements.DocMovementID,

                            //Код товара
                            DirNomenID = docMovementTabs.DirNomenID,
                            Sub = docMovementTabs.dirNomen.Sub,

                            //Товар Наименование
                            DirNomenName = docMovementTabs.dirNomen.DirNomenName,
                            DirNomenPatchFull =
                                dirNomensSubGroup.DirNomenName == null ? "" :
                                dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                                dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

                            //К-во
                            Sale_Quantity = docMovementTabs.Quantity,
                            DocDate = docMovements.doc.DocDate,


                            //Точка - откуда
                            DirWarehouseIDFrom = docMovements.DirWarehouseIDFrom,
                            DirWarehouseNameFrom = docMovements.dirWarehouseFrom.DirWarehouseName,
                            //Курьер
                            DirEmployeeIDCourier = docMovements.DirEmployeeIDCourier,
                            DirEmployeeNameCourier = docMovements.dirEmployee_Courier.DirEmployeeName,
                            //Курьер
                            DirWarehouseIDTo = docMovements.DirWarehouseIDTo,
                            DirWarehouseNameTo = docMovements.dirWarehouseTo.DirWarehouseName,


                            DirMovementStatusID = docMovements.DirMovementStatusID,
                            DirMovementStatusName = docMovements.dirMovementStatus.DirMovementStatusName,

                            //Характеристики
                            /*
                            DirCharColourName = dirCharColours.DirCharColourName,
                            DirCharMaterialName = dirCharMaterials.DirCharMaterialName,
                            DirCharNameName = dirCharNames.DirCharNameName,
                            DirCharSeasonName = dirCharSeasons.DirCharSeasonName,
                            DirCharSexName = dirCharSexes.DirCharSexName,
                            DirCharSizeName = dirCharSizes.DirCharSizeName,
                            DirCharStyleName = dirCharStyles.DirCharStyleName,
                            DirCharTextureName = dirCharTextures.DirCharTextureName,
                            */
                            DirChar =
                                    dirCharColours.DirCharColourName + " " +
                                    dirCharMaterials.DirCharMaterialName + " " +
                                    dirCharNames.DirCharNameName + " " +
                                    dirCharSeasons.DirCharSeasonName + " " +
                                    dirCharSexes.DirCharSexName + " " +
                                    dirCharSizes.DirCharSizeName + " " +
                                    dirCharStyles.DirCharStyleName + " " +
                                    dirCharTextures.DirCharTextureName,
                            //SerialNumber = docMovementTabs.SerialNumber,
                            //Barcode = docMovementTabs.Barcode,

                        }
                    );

                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeIDCourier == DirEmployeeID);
                    if (DirMovementStatusID > 0) queryTemp = queryTemp.Where(z => z.DirMovementStatusID == DirMovementStatusID);
                    else queryTemp = queryTemp.Where(z => z.DirMovementStatusID <= 3);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper1 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportLogistics = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper1));

                    #endregion
                }


            }
            catch (Exception ex)
            {
                return Ok(returnServer.Return(false, exceptionEntry.Return(ex)));
            }
        }

        #endregion
    }
}
