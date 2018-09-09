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
    public class ReportTotalTradeController : ApiController
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

        bool ProfitNomenAll = false;
        int DirContractorIDOrg = 0, DirWarehouseID = 0, DirEmployeeID = 0, DirPriceTypeID = 0, ReportType = 0;
        string DirContractorNameOrg, DirWarehouseName, DirEmployeeName, DirPriceTypeName, ReportTypeName;
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

                //Права-1: (1 - Write, 2 - Read, 3 - No Access)
                int iRight = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightReportTotalTrade"));
                if (iRight == 3) return Ok(returnServer.Return(false, Classes.Language.Sklad.Language.msg57(0)));
                //Права-2: ""
                int iRight2 = await Task.Run(() => accessRight.Access(connectionString.Return(field.DirCustomersID, null, true), field.DirEmployeeID, "RightReportTotalTradePrice"));
                if (field.DirEmployeeID == 1) iRight2 = 1;

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

                ProfitNomenAll = false;
                bool bProfitNomenAll = Boolean.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ProfitNomenAll", true) == 0).Value, out ProfitNomenAll);

                DirContractorIDOrg = 0;
                bool bDirContractorIDOrg = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorIDOrg", true) == 0).Value, out DirContractorIDOrg);
                DirContractorNameOrg = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirContractorNameOrg", true) == 0).Value;

                DirWarehouseID = 0;
                bool bDirWarehouseID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseID", true) == 0).Value, out DirWarehouseID);
                DirWarehouseName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirWarehouseName", true) == 0).Value;

                DirEmployeeID = 0;
                bool bDirEmployeeID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeID", true) == 0).Value, out DirEmployeeID);
                DirEmployeeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirEmployeeName", true) == 0).Value;

                DirPriceTypeID = 0;
                bool bDirPriceTypeID = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPriceTypeID", true) == 0).Value, out DirPriceTypeID);
                DirPriceTypeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "DirPriceTypeName", true) == 0).Value;

                ReportType = 0;
                bool bReportType = Int32.TryParse(paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportType", true) == 0).Value, out ReportType);
                ReportTypeName = paramList.FirstOrDefault(kv => string.Compare(kv.Key, "ReportTypeName", true) == 0).Value;


                #endregion




                if (ReportType == 1)
                {
                    #region Проданный товар (Опт + Розница)


                    #region query

                    var queryTemp =
                    (
                        from x in db.RemPartyMinuses
                        from y in db.RemParties

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики
                        /*
                        join dirCharColours1 in db.DirCharColours on y.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on y.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on y.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on y.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on y.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on y.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on y.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on y.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */
                        #endregion

                        where
                        x.RemPartyID == y.RemPartyID &&
                        x.DirContractorIDOrg == DirContractorIDOrg &&
                        //x.doc.Held == true && 
                        x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo &&
                        (x.doc.ListObjectID == 56 || x.doc.ListObjectID == 32)

                        select new
                        {
                            //Документ и Тип
                            DocID = x.doc.DocID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,
                            RemPartyID = x.RemPartyID,

                            //Код товара
                            DirNomenID = x.DirNomenID,
                            Sub = x.dirNomen.Sub,

                            //Товар Наименование
                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            //Цена закупки
                            //Purch_PriceCurrency = field.DirEmployeeID == 1 ? y.PriceCurrency : 0, //Purch_PriceCurrency = y.PriceCurrency,
                            Purch_PriceCurrency = iRight2 == 1 ? y.PriceCurrency : 0,

                            Sale_PriceCurrency = x.PriceCurrency,
                            //К-во
                            Sale_Quantity = x.Quantity,
                            //Сумма
                            Sums = x.Quantity * x.PriceCurrency - x.doc.Discount,
                            
                            //Прибыль
                            //SumProfit = field.DirEmployeeID == 1 ? x.Quantity * (x.PriceCurrency - y.PriceCurrency) - x.doc.Discount : 0, //SumProfit = x.Quantity * (x.PriceCurrency - y.PriceCurrency) - x.doc.Discount,
                            SumProfit = iRight2 == 1 ? x.Quantity * (x.PriceCurrency - y.PriceCurrency) - x.doc.Discount : 0, //SumProfit = x.Quantity * (x.PriceCurrency - y.PriceCurrency) - x.doc.Discount,

                            Sale_Discount = x.doc.Discount,
                            //Продавец
                            DirEmployeeID = x.doc.DirEmployeeID,
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            //Дата
                            DocDate = x.doc.DocDate,
                            //Точка
                            DirWarehouseID = x.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,


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
                                    y.dirCharColour.DirCharColourName + " " +
                                    y.dirCharMaterial.DirCharMaterialName + " " +
                                    y.dirCharName.DirCharNameName + " " +
                                    y.dirCharSeason.DirCharSeasonName + " " +
                                    y.dirCharSex.DirCharSexName + " " +
                                    y.dirCharSize.DirCharSizeName + " " +
                                    y.dirCharStyle.DirCharStyleName + " " +
                                    y.dirCharTexture.DirCharTextureName,
                            SerialNumber = y.SerialNumber,
                            Barcode = y.Barcode,

                            DirDescriptionName = x.dirDescription.DirDescriptionName,

                        }
                    );

                    if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                    queryTemp = queryTemp.OrderByDescending(x=>x.DocDate);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper1 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportTotalTrade = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper1));

                    #endregion


                    #endregion
                }
                else if (ReportType == 2)
                {
                    #region Приходы


                    #region query

                    var queryTemp =
                    (
                        from x in db.RemParties

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики

                        /*
                        join dirCharColours1 in db.DirCharColours on x.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on x.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on x.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on x.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on x.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on x.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on x.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on x.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */

                        #endregion

                        where
                        x.DirContractorIDOrg == DirContractorIDOrg &&
                        x.doc.Held == true &&
                        x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo &&
                        x.doc.ListObjectID == 6

                        select new
                        {
                            //Документ и Тип
                            DocID = x.doc.DocID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,
                            RemPartyID = x.RemPartyID,

                            //Код товара
                            DirNomenID = x.DirNomenID,
                            Sub = x.dirNomen.Sub,

                            //Товар Наименование
                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            //Цена закупки
                            PriceCurrency = field.DirEmployeeID == 1 ? x.PriceCurrency : 0, //PriceCurrency = x.PriceCurrency,
                            //Purch_Sums = field.DirEmployeeID == 1 ? x.Quantity * x.PriceCurrency : 0, //PriceCurrency = x.PriceCurrency,

                            //Цена продажи
                            //Розница
                            PriceRetailVAT = x.PriceRetailVAT,
                            //Опт
                            PriceWholesaleVAT = x.PriceWholesaleVAT,
                            //И-М
                            PriceIMVAT = x.PriceIMVAT,

                            //К-во Пришло
                            Quantity = x.Quantity,
                            //Сумма Пришло
                            SumQuantity = field.DirEmployeeID == 1 ? x.Quantity * x.PriceCurrency - x.doc.Discount : 0, //SumQuantity = x.Quantity * x.PriceCurrency - x.doc.Discount,

                            //Остаток
                            Remnant = x.Remnant,
                            //Сумма Остатка
                            SumRemnant = field.DirEmployeeID == 1 ? x.Remnant * x.PriceCurrency - x.doc.Discount : 0, //SumRemnant = x.Remnant * x.PriceCurrency - x.doc.Discount,

                            //Скидка
                            Sale_Discount = x.doc.Discount,
                            //Сотрудник
                            DirEmployeeID = x.doc.DirEmployeeID,
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            //Дата
                            DocDate = x.doc.DocDate,
                            //Точка
                            DirWarehouseID = x.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,


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
                                    x.dirCharColour.DirCharColourName + " " +
                                    x.dirCharMaterial.DirCharMaterialName + " " +
                                    x.dirCharName.DirCharNameName + " " +
                                    x.dirCharSeason.DirCharSeasonName + " " +
                                    x.dirCharSex.DirCharSexName + " " +
                                    x.dirCharSize.DirCharSizeName + " " +
                                    x.dirCharStyle.DirCharStyleName + " " +
                                    x.dirCharTexture.DirCharTextureName,
                            SerialNumber = x.SerialNumber,
                            Barcode = x.Barcode,

                        }
                    );

                    if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                    queryTemp = queryTemp.OrderByDescending(x => x.DocDate);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper2 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportTotalTrade = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper2));

                    #endregion


                    #endregion
                }
                else if (ReportType == 3)
                {
                    #region Товар в наличии


                    #region query

                    var queryTemp =
                    (
                        from x in db.RemParties

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики

                        /*
                        join dirCharColours1 in db.DirCharColours on x.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on x.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on x.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on x.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on x.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on x.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on x.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on x.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */

                        #endregion

                        where
                        x.Remnant > 0 &&
                        x.DirContractorIDOrg == DirContractorIDOrg
                        //&& x.doc.Held == true 
                        //&& x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo

                        select new
                        {
                            //Документ и Тип
                            DocID = x.doc.DocID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,
                            RemPartyID = x.RemPartyID,

                            //Код товара
                            DirNomenID = x.DirNomenID,
                            Sub = x.dirNomen.Sub,

                            //Товар Наименование
                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            //Цена закупки
                            PriceCurrency = field.DirEmployeeID == 1 ? x.PriceCurrency : 0, //PriceCurrency = x.PriceCurrency,

                            //Цена продажи
                            //Розница
                            PriceRetailVAT = x.PriceRetailVAT,
                            //Опт
                            PriceWholesaleVAT = x.PriceWholesaleVAT,
                            //И-М
                            PriceIMVAT = x.PriceIMVAT,

                            //К-во Пришло
                            Quantity = x.Quantity,
                            //Сумма Пришло
                            SumQuantity = field.DirEmployeeID == 1 ? x.Quantity * x.PriceCurrency - x.doc.Discount : 0, //SumQuantity = x.Quantity * x.PriceCurrency - x.doc.Discount,

                            //Остаток
                            Remnant = x.Remnant,
                            //Сумма Остатка
                            SumRemnant = field.DirEmployeeID == 1 ? x.Remnant * x.PriceCurrency - x.doc.Discount : 0, //SumRemnant = x.Remnant * x.PriceCurrency - x.doc.Discount,

                            //Скидка
                            Sale_Discount = x.doc.Discount,
                            //Сотрудник
                            DirEmployeeID = x.doc.DirEmployeeID,
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            //Дата
                            DocDate = x.doc.DocDate,
                            //Точка
                            DirWarehouseID = x.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,


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
                                        x.dirCharColour.DirCharColourName + " " +
                                        x.dirCharMaterial.DirCharMaterialName + " " +
                                        x.dirCharName.DirCharNameName + " " +
                                        x.dirCharSeason.DirCharSeasonName + " " +
                                        x.dirCharSex.DirCharSexName + " " +
                                        x.dirCharSize.DirCharSizeName + " " +
                                        x.dirCharStyle.DirCharStyleName + " " +
                                        x.dirCharTexture.DirCharTextureName,
                            SerialNumber = x.SerialNumber,
                            Barcode = x.Barcode,

                        }
                    );

                    if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                    queryTemp = queryTemp.OrderByDescending(x => x.DocDate);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper3 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportTotalTrade = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper3));

                    #endregion


                    #endregion
                }
                else if (ReportType == 4)
                {
                    #region Возвраты (Опт + Розница)


                    #region query

                    var queryTemp =
                    (
                        from y in db.RemParties

                        join dirNomens11 in db.DirNomens on y.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()
                        
                        where
                        y.RemPartyID == y.RemPartyID &&
                        y.DirContractorIDOrg == DirContractorIDOrg &&
                        y.doc.Held == true &&
                        y.doc.DocDate >= DateS && y.doc.DocDate <= DatePo &&
                        (y.doc.ListObjectID == 36 || y.doc.ListObjectID == 57)

                        select new
                        {
                            //Документ и Тип
                            DocID = y.doc.DocID,
                            ListObjectNameRu = y.doc.listObject.ListObjectNameRu,
                            RemPartyID = y.RemPartyID,

                            ListObjectID = y.doc.ListObjectID,

                            //Код товара
                            DirNomenID = y.DirNomenID,
                            Sub = y.dirNomen.Sub,

                            //Товар Наименование
                            //DirNomenName = y.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? y.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + y.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + y.dirNomen.DirNomenName,

                            //Цена закупки
                            PriceCurrency = field.DirEmployeeID == 1 ? y.PriceCurrency : 0, //PriceCurrency = y.PriceCurrency,
                            //Цена продажи Розница
                            PriceRetailCurrency = y.PriceRetailCurrency,
                            //Цена продажи Опт
                            PriceWholesaleCurrency = y.PriceWholesaleCurrency,

                            //К-во
                            Quantity = y.Quantity,
                            //Сумма
                            //Sums = field.DirEmployeeID == 1 ? y.Quantity * y.PriceCurrency - y.doc.Discount : 0, //Sums = y.Quantity * y.PriceCurrency - y.doc.Discount,
                            //Скидка
                            Sale_Discount = y.doc.Discount,
                            //Продавец
                            DirEmployeeID = y.doc.DirEmployeeID,
                            DirEmployeeName = y.doc.dirEmployee.DirEmployeeName,
                            //Дата
                            DocDate = y.doc.DocDate.ToString(),
                            //Точка
                            DirWarehouseID = y.DirWarehouseID,
                            DirWarehouseName = y.dirWarehouse.DirWarehouseName,


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
                                    y.dirCharColour.DirCharColourName + " " +
                                    y.dirCharMaterial.DirCharMaterialName + " " +
                                    y.dirCharName.DirCharNameName + " " +
                                    y.dirCharSeason.DirCharSeasonName + " " +
                                    y.dirCharSex.DirCharSexName + " " +
                                    y.dirCharSize.DirCharSizeName + " " +
                                    y.dirCharStyle.DirCharStyleName + " " +
                                    y.dirCharTexture.DirCharTextureName,
                            SerialNumber = y.SerialNumber,
                            Barcode = y.Barcode,

                            //Возврат
                            DirReturnTypeName = y.dirReturnType.DirReturnTypeName,
                            DirDescriptionName = y.dirDescription.DirDescriptionName,

                        }
                    );

                    if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                    queryTemp = queryTemp.OrderByDescending(x => x.DocDate);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper4 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportTotalTrade = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper4));

                    #endregion


                    #endregion
                }
                else if (ReportType == 5)
                {
                    #region Заканчивающийся товар


                    #region query

                    var queryTemp =
                    (
                        from x in db.RemParties

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики

                        /*
                        join dirCharColours1 in db.DirCharColours on x.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on x.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on x.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on x.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on x.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on x.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on x.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on x.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */

                        #endregion

                        where
                            x.Remnant > 0 && x.Remnant < x.DirNomenMinimumBalance &&
                            x.DirContractorIDOrg == DirContractorIDOrg
                        //&& x.doc.Held == true
                        //x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo


                        group x by new
                            {
                                //Документ и Тип
                                DocID = x.doc.DocID,
                                ListObjectNameRu = x.doc.listObject.ListObjectNameRu,
                                RemPartyID = x.RemPartyID,

                                //Код товара
                                DirNomenID = x.DirNomenID,
                                Sub = x.dirNomen.Sub,

                                //Товар Наименование
                                //DirNomenName = x.dirNomen.DirNomenName,
                                DirNomenName =
                                dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                                dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                                dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                                //Цена закупки
                                PriceCurrency = x.PriceCurrency,

                                //Цена продажи
                                //Розница
                                PriceRetailVAT = x.PriceRetailVAT,
                                //Опт
                                PriceWholesaleVAT = x.PriceWholesaleVAT,
                                //И-М
                                PriceIMVAT = x.PriceIMVAT,

                                //К-во Пришло
                                //Quantity = x.Quantity,
                                //Сумма Пришло
                                //SumQuantity = x.Quantity * x.PriceCurrency - x.doc.Discount,

                                //Остаток
                                //Remnant = x.Remnant,
                                //Сумма Остатка
                                //SumRemnant = x.Remnant * x.PriceCurrency - x.doc.Discount,

                                //Минимальный остаток
                                DirNomenMinimumBalance = x.DirNomenMinimumBalance,

                                //Скидка
                                Sale_Discount = x.doc.Discount,
                                //Сотрудник
                                DirEmployeeID = x.doc.DirEmployeeID,
                                DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                                //Дата
                                //DocDate = x.doc.DocDate,
                                //Точка
                                DirWarehouseID = x.DirWarehouseID,
                                DirWarehouseName = x.dirWarehouse.DirWarehouseName,


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
                                        x.dirCharColour.DirCharColourName + " " +
                                        x.dirCharMaterial.DirCharMaterialName + " " +
                                        x.dirCharName.DirCharNameName + " " +
                                        x.dirCharSeason.DirCharSeasonName + " " +
                                        x.dirCharSex.DirCharSexName + " " +
                                        x.dirCharSize.DirCharSizeName + " " +
                                        x.dirCharStyle.DirCharStyleName + " " +
                                        x.dirCharTexture.DirCharTextureName,
                                SerialNumber = x.SerialNumber,
                                Barcode = x.Barcode,
                            }
                        into g


                        select new
                        {
                            //Код товара
                            DirNomenID = g.Key.DirNomenID,
                            Sub = g.Key.Sub,
                            //Товар Наименование
                            DirNomenName = g.Key.DirNomenName,

                            //Цена закупки
                            PriceCurrency = field.DirEmployeeID == 1 ? g.Key.PriceCurrency : 0, //PriceCurrency = g.Key.PriceCurrency,

                            //Цена продажи
                            //Розница
                            PriceRetailVAT = g.Key.PriceRetailVAT,
                            //Опт
                            PriceWholesaleVAT = g.Key.PriceWholesaleVAT,
                            //И-М
                            PriceIMVAT = g.Key.PriceIMVAT,

                            //К-во Пришло
                            Quantity = g.Sum(x => x.Quantity), //g.Key.Quantity,
                                                               //Сумма Пришло
                                                               //SumQuantity = g.Key.SumQuantity,

                            //Остаток
                            Remnant = g.Sum(x => x.Remnant), //g.Key.Remnant,
                                                             //Сумма Остатка
                                                             // SumRemnant = g.Key.SumRemnant,

                            //Минимальный остаток
                            DirNomenMinimumBalance = g.Key.DirNomenMinimumBalance,

                            //Скидка
                            Sale_Discount = g.Key.Sale_Discount,
                            //Сотрудник
                            DirEmployeeID = g.Key.DirEmployeeID,
                            DirEmployeeName = g.Key.DirEmployeeName,
                            //Дата
                            //DocDate = g.Key.DocDate,
                            //Точка
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,


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
                                    g.Key.DirChar,
                            SerialNumber = g.Key.SerialNumber,
                            Barcode = g.Key.Barcode,

                        }
                    );

                    if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                    queryTemp = queryTemp.OrderByDescending(x => x.DirNomenID);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper5 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportTotalTrade = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper5));

                    #endregion


                    #endregion
                }
                else if (ReportType == 6)
                {
                    #region Списанный товар


                    #region query

                    var queryTemp =
                    (
                        from x in db.RemPartyMinuses
                        from y in db.RemParties

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики

                        /*
                        join dirCharColours1 in db.DirCharColours on y.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on y.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on y.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on y.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on y.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on y.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on y.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on y.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */

                        #endregion

                        where
                        x.RemPartyID == y.RemPartyID &&
                        x.DirContractorIDOrg == DirContractorIDOrg &&
                        //x.doc.Held == true &&
                        x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo &&
                        (x.doc.ListObjectID == 35)

                        select new
                        {
                            //Документ и Тип
                            DocID = x.doc.DocID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,
                            RemPartyID = x.RemPartyID,

                            //Код товара
                            DirNomenID = x.DirNomenID,
                            Sub = x.dirNomen.Sub,

                            //Товар Наименование
                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            //Цена закупки
                            Purch_PriceCurrency = field.DirEmployeeID == 1 ? y.PriceCurrency : 0, //Purch_PriceCurrency = y.PriceCurrency,
                            //Цена продажи
                            Sale_PriceCurrency = x.PriceCurrency,
                            //К-во
                            Sale_Quantity = x.Quantity,
                            //Сумма
                            Sums = field.DirEmployeeID == 1 ? x.Quantity * x.PriceCurrency - x.doc.Discount : 0, //Sums = x.Quantity * x.PriceCurrency - x.doc.Discount,
                            //Прибыль
                            SumProfit = field.DirEmployeeID == 1 ? x.Quantity * (x.PriceCurrency - y.PriceCurrency) - x.doc.Discount : 0, //SumProfit = x.Quantity * (x.PriceCurrency - y.PriceCurrency) - x.doc.Discount,
                            //Скидка
                            Sale_Discount = x.doc.Discount,
                            //Продавец
                            DirEmployeeID = x.doc.DirEmployeeID,
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            //Дата
                            DocDate = x.doc.DocDate,
                            //Точка
                            DirWarehouseID = x.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,


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
                                    y.dirCharColour.DirCharColourName + " " +
                                    y.dirCharMaterial.DirCharMaterialName + " " +
                                    y.dirCharName.DirCharNameName + " " +
                                    y.dirCharSeason.DirCharSeasonName + " " +
                                    y.dirCharSex.DirCharSexName + " " +
                                    y.dirCharSize.DirCharSizeName + " " +
                                    y.dirCharStyle.DirCharStyleName + " " +
                                    y.dirCharTexture.DirCharTextureName,
                            SerialNumber = y.SerialNumber,
                            Barcode = y.Barcode,

                        }
                    );

                    if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                    queryTemp = queryTemp.OrderByDescending(x => x.DocDate);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper6 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportTotalTrade = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper6));

                    #endregion


                    #endregion
                }
                else if (ReportType == 7)
                {
                    #region Товар отсутствует

                    #region query

                    var queryTemp =
                    (
                        from x in db.RemParties

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region  Характеристики

                        /*
                        join dirCharColours1 in db.DirCharColours on x.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on x.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on x.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on x.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on x.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on x.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on x.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on x.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */

                        #endregion

                        where
                            x.Remnant == 0 &&
                            x.DirContractorIDOrg == DirContractorIDOrg

                        group new { x } by new
                        {
                            //Документ и Тип
                            DocID = x.doc.DocID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,
                            RemPartyID = x.RemPartyID,

                            //Код товара
                            DirNomenID = x.DirNomenID,
                            Sub = x.dirNomen.Sub,
                            //Товар Наименование
                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            //Цена закупки
                            //PriceCurrency = x.PriceCurrency,

                            //Цена продажи
                            //Розница
                            PriceRetailVAT = x.PriceRetailVAT,
                            //Опт
                            PriceWholesaleVAT = x.PriceWholesaleVAT,
                            //И-М
                            PriceIMVAT = x.PriceIMVAT,

                            //К-во Пришло
                            //Quantity = x.Quantity,
                            //Сумма Пришло
                            //SumQuantity = field.DirEmployeeID == 1 ? x.Quantity * x.PriceCurrency - x.doc.Discount : 0, //SumQuantity = x.Quantity * x.PriceCurrency - x.doc.Discount,

                            //Остаток
                            Remnant = x.Remnant,
                            //Сумма Остатка
                            SumRemnant = field.DirEmployeeID == 1 ? x.Remnant * x.PriceCurrency - x.doc.Discount : 0, //SumRemnant = x.Remnant * x.PriceCurrency - x.doc.Discount,

                            //Скидка
                            Sale_Discount = x.doc.Discount,
                            //Сотрудник
                            DirEmployeeID = x.doc.DirEmployeeID,
                            DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            //Дата
                            //DocDate = x.doc.DocDate,
                            //Точка
                            DirWarehouseID = x.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,


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
                                x.dirCharColour.DirCharColourName + " " +
                                x.dirCharMaterial.DirCharMaterialName + " " +
                                x.dirCharName.DirCharNameName + " " +
                                x.dirCharSeason.DirCharSeasonName + " " +
                                x.dirCharSex.DirCharSexName + " " +
                                x.dirCharSize.DirCharSizeName + " " +
                                x.dirCharStyle.DirCharStyleName + " " +
                                x.dirCharTexture.DirCharTextureName,
                            SerialNumber = x.SerialNumber,
                            Barcode = x.Barcode,
                        }
                        into g


                        select new
                        {
                            //Код товара
                            DirNomenID = g.Key.DirNomenID,
                            Sub = g.Key.Sub,
                            //Товар Наименование
                            DirNomenName = g.Key.DirNomenName,

                            //Цена закупки
                            //PriceCurrency = g.Key.PriceCurrency,

                            //Цена продажи
                            //Розница
                            PriceRetailVAT = g.Key.PriceRetailVAT,
                            //Опт
                            PriceWholesaleVAT = g.Key.PriceWholesaleVAT,
                            //И-М
                            PriceIMVAT = g.Key.PriceIMVAT,

                            //К-во Пришло
                            //Quantity = g.Key.Quantity,
                            //Сумма Пришло
                            //SumQuantity = g.Key.SumQuantity, //g.Key.Quantity * g.Key.PriceCurrency - g.Key.SumQuantity,

                            //Остаток
                            Remnant = g.Key.Remnant,
                            //Сумма Остатка
                            SumRemnant = g.Key.SumRemnant, //g.Key.Remnant * g.Key.PriceCurrency - g.Key.SumRemnant,

                            //Скидка
                            Sale_Discount = g.Key.Sale_Discount,
                            //Сотрудник
                            DirEmployeeID = g.Key.DirEmployeeID,
                            DirEmployeeName = g.Key.DirEmployeeName,
                            //Дата
                            //DocDate = g.Key.DocDate,
                            //Точка
                            DirWarehouseID = g.Key.DirWarehouseID,
                            DirWarehouseName = g.Key.DirWarehouseName,


                            //Характеристики
                            /*
                            DirCharColourName = g.Key.DirCharColourName,
                            DirCharMaterialName = g.Key.DirCharMaterialName,
                            DirCharNameName = g.Key.DirCharNameName,
                            DirCharSeasonName = g.Key.DirCharSeasonName,
                            DirCharSexName = g.Key.DirCharSexName,
                            DirCharSizeName = g.Key.DirCharSizeName,
                            DirCharStyleName = g.Key.DirCharStyleName,
                            DirCharTextureName = g.Key.DirCharTextureName,
                            */
                            DirChar =
                                    g.Key.DirChar,
                            //SerialNumber = g.Key.SerialNumber,
                            //Barcode = g.Key.Barcode,

                        }
                    );

                    if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                    queryTemp = queryTemp.OrderBy(x => x.DirNomenID);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper7 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportTotalTrade = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper7));

                    #endregion

                    #endregion
                }
                else if (ReportType == 8)
                {
                    #region Брак

                    #region query

                    var queryTemp =
                    (
                        from x in db.RemParties

                        join dirNomens11 in db.DirNomens on x.dirNomen.Sub equals dirNomens11.DirNomenID into dirNomens12
                        from dirNomensSubGroup in dirNomens12.DefaultIfEmpty()

                        join dirNomens21 in db.DirNomens on dirNomensSubGroup.Sub equals dirNomens21.DirNomenID into dirNomens22
                        from dirNomensGroup in dirNomens22.DefaultIfEmpty()

                        #region Характеристики

                        /*
                        join dirCharColours1 in db.DirCharColours on x.DirCharColourID equals dirCharColours1.DirCharColourID into dirCharColours2
                        from dirCharColours in dirCharColours2.DefaultIfEmpty()

                        join dirCharMaterials1 in db.DirCharMaterials on x.DirCharMaterialID equals dirCharMaterials1.DirCharMaterialID into dirCharMaterials2
                        from dirCharMaterials in dirCharMaterials2.DefaultIfEmpty()

                        join dirCharNames1 in db.DirCharNames on x.DirCharNameID equals dirCharNames1.DirCharNameID into dirCharNames2
                        from dirCharNames in dirCharNames2.DefaultIfEmpty()

                        join dirCharSeasons1 in db.DirCharSeasons on x.DirCharSeasonID equals dirCharSeasons1.DirCharSeasonID into dirCharSeasons2
                        from dirCharSeasons in dirCharSeasons2.DefaultIfEmpty()

                        join dirCharSexes1 in db.DirCharSexes on x.DirCharSexID equals dirCharSexes1.DirCharSexID into dirCharSexes2
                        from dirCharSexes in dirCharSexes2.DefaultIfEmpty()

                        join dirCharSizes1 in db.DirCharSizes on x.DirCharSizeID equals dirCharSizes1.DirCharSizeID into dirCharSizes2
                        from dirCharSizes in dirCharSizes2.DefaultIfEmpty()

                        join dirCharStyles1 in db.DirCharStyles on x.DirCharStyleID equals dirCharStyles1.DirCharStyleID into dirCharStyles2
                        from dirCharStyles in dirCharStyles2.DefaultIfEmpty()

                        join dirCharTextures1 in db.DirCharTextures on x.DirCharTextureID equals dirCharTextures1.DirCharTextureID into dirCharTextures2
                        from dirCharTextures in dirCharTextures2.DefaultIfEmpty()
                        */

                        #endregion

                        where
                        x.DirContractorIDOrg == DirContractorIDOrg &&
                        x.DirReturnTypeID == 3 &&
                        x.Remnant > 0 &&
                        x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo

                        #region select
                        select new
                        {
                            //Документ и Тип
                            DocID = x.doc.DocID,
                            ListObjectNameRu = x.doc.listObject.ListObjectNameRu,
                            RemPartyID = x.RemPartyID,

                            //Партия
                            DirNomenID = x.DirNomenID,

                            //Код товара
                            //RemPartyID = x.RemPartyID,
                            Sub = x.dirNomen.Sub,

                            //Товар Наименование
                            //DirNomenName = x.dirNomen.DirNomenName,
                            DirNomenName =
                            dirNomensSubGroup.DirNomenName == null ? x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName + " / " + x.dirNomen.DirNomenName,

                            //Цена закупки
                            PriceCurrency = x.PriceCurrency,

                            //Цена продажи
                            //Розница
                            PriceRetailVAT = x.PriceRetailVAT,
                            //Опт
                            PriceWholesaleVAT = x.PriceWholesaleVAT,
                            //И-М
                            PriceIMVAT = x.PriceIMVAT,

                            //К-во Пришло
                            Quantity = x.Quantity,
                            //Сумма Пришло
                            SumQuantity = x.Quantity * x.PriceCurrency - x.doc.Discount,

                            //Остаток
                            Remnant = x.Remnant,
                            //Сумма Остатка
                            SumRemnant = x.Remnant * x.PriceCurrency - x.doc.Discount,

                            //Скидка
                            Sale_Discount = x.doc.Discount,
                            //Сотрудник
                            DirEmployeeID = x.doc.DirEmployeeID, DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                            //Дата
                            DocDate = x.doc.DocDate,
                            //Точка
                            DirWarehouseID = x.DirWarehouseID, DirWarehouseName = x.dirWarehouse.DirWarehouseName,


                            //Характеристики
                            DirCharColourID = x.DirCharColourID,
                            DirCharMaterialID = x.DirCharMaterialID,
                            DirCharNameID = x.DirCharNameID,
                            DirCharSeasonID = x.DirCharSeasonID,
                            DirCharSexID = x.DirCharSexID,
                            DirCharSizeID = x.DirCharSizeID,
                            DirCharStyleID = x.DirCharStyleID,
                            DirCharTextureID = x.DirCharTextureID,

                            DirChar =
                                    x.dirCharColour.DirCharColourName + " " +
                                    x.dirCharMaterial.DirCharMaterialName + " " +
                                    x.dirCharName.DirCharNameName + " " +
                                    x.dirCharSeason.DirCharSeasonName + " " +
                                    x.dirCharSex.DirCharSexName + " " +
                                    x.dirCharSize.DirCharSizeName + " " +
                                    x.dirCharStyle.DirCharStyleName + " " +
                                    x.dirCharTexture.DirCharTextureName,
                            SerialNumber = x.SerialNumber,
                            Barcode = x.Barcode,

                            DirReturnTypeID = x.DirReturnTypeID,
                            DirReturnTypeName = x.dirReturnType.DirReturnTypeName,
                            DirDescriptionID = x.DirDescriptionID,
                            DirDescriptionName = x.dirDescription.DirDescriptionName,


                            //Дополнительно
                            DirContractorIDOrg = x.DirContractorIDOrg,
                            DirContractorID = x.DirContractorID,
                            DirWarehouseIDDebit = x.DirWarehouseIDDebit,
                            DirWarehouseIDPurch = x.DirWarehouseIDPurch,
                            DirCurrencyID = x.DirCurrencyID,
                            DirCurrencyRate = x.dirCurrency.DirCurrencyRate,
                            DirCurrencyMultiplicity = x.dirCurrency.DirCurrencyMultiplicity,

                        }
                        #endregion
                );

                    if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                    if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                    queryTemp = queryTemp.OrderByDescending(x => x.DocDate);

                    #endregion


                    #region Отправка JSON

                    int dirCount = queryTemp.Count();

                    dynamic collectionWrapper2 = new
                    {
                        sucess = true,
                        total = dirCount,
                        ReportTotalTrade = queryTemp
                    };
                    return await Task.Run(() => Ok(collectionWrapper2));

                    #endregion

                    #endregion
                }



                #region Отправка JSON

                dynamic collectionWrapper = new
                {
                    sucess = true,
                    total = 0,
                    ReportTotalTrade = ""
                };
                return await Task.Run(() => Ok(collectionWrapper));

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
