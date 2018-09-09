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
    public class ReportTotalTrade
    {
        string pID = "";
        bool ProfitNomenAll = false;
        int pLanguage = 0, DirContractorIDOrg = 0, DirWarehouseID = 0, DirEmployeeID = 0, DirPriceTypeID = 0, ReportType = 0;
        string DirContractorNameOrg, DirWarehouseName, DirEmployeeName, DirPriceTypeName, ReportTypeName;
        DateTime DateS, DatePo;
        ArrayList alDirNomenPatchFull = new ArrayList();

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db)
        {
            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);
            DateS = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DateS"]).ToString("yyyy-MM-dd 00:00:00"));
            DatePo = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DatePo"]).ToString("yyyy-MM-dd 23:59:59"));

            ProfitNomenAll = false;
            bool bProfitNomenAll = Boolean.TryParse(Request.Params["ProfitNomenAll"], out ProfitNomenAll);

            DirContractorIDOrg = 0;
            bool bDirContractorIDOrg = Int32.TryParse(Request.Params["DirContractorIDOrg"], out DirContractorIDOrg);
            DirContractorNameOrg = Request.Params["DirContractorNameOrg"];

            DirWarehouseID = 0;
            bool bDirWarehouseID = Int32.TryParse(Request.Params["DirWarehouseID"], out DirWarehouseID);
            DirWarehouseName = Request.Params["DirWarehouseName"];

            DirEmployeeID = 0;
            bool bDirEmployeeID = Int32.TryParse(Request.Params["DirEmployeeID"], out DirEmployeeID);
            DirEmployeeName = Request.Params["DirEmployeeName"];

            DirPriceTypeID = 0;
            bool bDirPriceTypeID = Int32.TryParse(Request.Params["DirPriceTypeID"], out DirPriceTypeID);
            DirPriceTypeName = Request.Params["DirPriceTypeName"];

            ReportType = 0;
            bool bReportType = Int32.TryParse(Request.Params["ReportType"], out ReportType);
            ReportTypeName = Request.Params["ReportTypeName"];

            #endregion

            string 
                //DirNomenPatchFull = "",
                ret = 
                "<center>" +
                "<h2>"+ ReportTypeName + " (" + DateS.ToString("yyyy-MM-dd") + " по " + DatePo.ToString("yyyy-MM-dd") + ")</h2>";

            //Алгоритм:
            //Простая выборку с ПартииМинус и Партии
            //ПартииМинус - продажная цена
            //Партии - приходная цена
            //Разница - прибыль

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
                    #endregion


                    where 
                    x.RemPartyID == y.RemPartyID && 
                    x.DirContractorIDOrg == DirContractorIDOrg && 
                    //x.doc.Held == true && 
                    x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo &&
                    (x.doc.ListObjectID == 56 || x.doc.ListObjectID == 32)

                    select new
                    {
                        //Код товара
                        DirNomenID = x.DirNomenID,
                        Sub = x.dirNomen.Sub,

                        //Товар Наименование
                        DirNomenName = x.dirNomen.DirNomenName,
                        DirNomenPatchFull =
                            dirNomensSubGroup.DirNomenName == null ? "" :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

                        //Цена закупки
                        Purch_PriceCurrency = y.PriceCurrency,
                        //Цена продажи
                        Sale_PriceCurrency = x.PriceCurrency,
                        //К-во
                        Sale_Quantity = x.Quantity,
                        //Сумма
                        Sums = x.Quantity * x.PriceCurrency - x.doc.Discount,
                        //Прибыль
                        SumProfit = x.Quantity * (x.PriceCurrency - y.PriceCurrency) - x.doc.Discount,
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
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
                        SerialNumber = y.SerialNumber,
                        Barcode = y.Barcode,

                    }
                );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                #endregion


                #region DataReader

                ret += ReportHeader1(pLanguage) + "</center>";
                double dQuantity = 0, dSums = 0, dSumProfit = 0;

                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {
                    dQuantity += query[i].Sale_Quantity;
                    dSums += query[i].Sums;
                    dSumProfit += query[i].SumProfit;

                    //Получить "категорию + наименование" для "iNode" всех рутов
                    //DirNomenPatchFull = await Task.Run(() => alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));

                    ret +=
                        "<TR>" +
                        //Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //Категория
                        "<TD>" + query[i].DirNomenPatchFull + "</TD> " +
                        //Товар Наименование
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //Характеристики
                        "<TD>" + query[i].DirChar + "</TD> ";

                    //Цена закупки
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].Purch_PriceCurrency + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    //Цена продажи
                        ret +=
                        "<TD>" + query[i].Sale_PriceCurrency + "</TD> " +
                        //К-во
                        "<TD>" + query[i].Sale_Quantity + "</TD> " +
                        //Сумма
                        "<TD>" + query[i].Sums + "</TD> ";

                    //Прибыль
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].SumProfit + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    ret +=
                        //Скидка
                        "<TD>" + query[i].Sale_Discount + "</TD> " +
                        //Продавец
                        "<TD>" + query[i].DirEmployeeName + "</TD> " +
                        //Дата
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //Точка
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +

                        //"<TD>" + query[i].SerialNumber + "</TD> " +
                        //"<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";
                }

                ret +=
                    "<TR>" +
                    //Код товара
                    "<TD> </TD> " +
                    //Категория
                    "<TD> - </TD> " +
                    //Товар Наименование
                    "<TD> </TD> " +
                    //Характеристики
                    "<TD> </TD> " +
                    //Цена закупки
                    "<TD> </TD> " +
                    //Цена продажи
                    "<TD> </TD> " +
                    //К-во
                    "<TD><b>" + dQuantity + "</b></TD> " +
                    //Сумма
                    "<TD><b>" + dSums + "</b></TD> ";

                //Прибыль
                if (DirPriceTypeID == 1) ret += "<TD><b>" + dSumProfit + "</b></TD> ";
                else ret += "<TD><b> - </b></TD> ";

                ret +=
                    //Скидка
                    "<TD> </TD> " +
                    //Продавец
                    "<TD> </TD> " +
                    //Дата
                    "<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +

                    //"<TD> </TD> " +
                    //"<TD> </TD> " +
                    "</TR>";

                #endregion


                return ret;

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
                    #endregion

                    where
                    x.DirContractorIDOrg == DirContractorIDOrg &&
                    //x.doc.Held == true &&
                    x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo

                    select new
                    {
                        //Код товара
                        DirNomenID = x.DirNomenID,
                        Sub = x.dirNomen.Sub,
                        //Товар Наименование
                        DirNomenName = x.dirNomen.DirNomenName,
                        DirNomenPatchFull =
                            dirNomensSubGroup.DirNomenName == null ? "" :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

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
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
                        SerialNumber = x.SerialNumber,
                        Barcode = x.Barcode,

                    }
                );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                #endregion


                #region DataReader

                ret += ReportHeader3(pLanguage) + "</center>";
                double dQuantity = 0, dRemnant = 0, dSumQuantity = 0, dSumRemnant = 0;

                //Получение списка
                //var query = await Task.Run(() => queryTemp.ToListAsync());
                var query = queryTemp.ToList();

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {

                    dQuantity += query[i].Quantity;
                    dRemnant += query[i].Remnant;
                    dSumQuantity += query[i].SumQuantity;
                    dSumRemnant += query[i].SumRemnant;


                    //Получить "категорию + наименование" для "iNode" всех рутов
                    //Тормоза из-за этого!
                    //Решение:
                    //Вести колекцию [Sub, DirNomenPatchFull]
                    //Если "Sub" повторяется просто вынять его из библиотеки!
                    //DirNomenPatchFull = await Task.Run(() => alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));


                    ret +=
                        "<TR>" +
                        //Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //Категория
                        "<TD>" + query[i].DirNomenPatchFull + "</TD> " +
                        //Товар Наименование
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //Характеристики
                        "<TD>" + query[i].DirChar + "</TD> ";

                    //Цена закупки
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].PriceCurrency + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    //Цена продажи
                    ret +=
                        //Розница
                        "<TD>" + query[i].PriceRetailVAT + "</TD> " +
                        //Опт
                        "<TD>" + query[i].PriceWholesaleVAT + "</TD> " +
                        //И-М
                        "<TD>" + query[i].PriceIMVAT + "</TD> " +

                        //К-во Пришло
                        "<TD>" + query[i].Quantity + "</TD> " +
                        //Сумма Пришло
                        "<TD>" + query[i].SumQuantity + "</TD> " +
                        //Остаток
                        "<TD>" + query[i].Remnant + "</TD> " +
                        //Сумма Остатка
                        "<TD>" + query[i].SumRemnant + "</TD> ";

                    ret +=
                        //Скидка
                        //"<TD>" + query[i].Discount + "</TD> " +
                        //Оприходовал
                        "<TD>" + query[i].DirEmployeeName + "</TD> " +
                        //Дата
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //Точка
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +

                        //"<TD>" + query[i].SerialNumber + "</TD> " +
                        //"<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";

                }


                ret +=
                    "<TR>" +
                    //Код товара
                    "<TD> </TD> " +
                    //Категория
                    "<TD> </TD> " +
                    //Товар Наименование
                    "<TD> </TD> " +
                    //Характеристики
                    "<TD> </TD> " +
                    //Цена закупки
                    "<TD> </TD> ";

                //Цена продажи
                ret +=
                    //Розница
                    "<TD> </TD> " +
                    //Опт
                    "<TD> </TD> " +
                    //И-М
                    "<TD> </TD> " +

                    //К-во Пришло
                    "<TD><b>" + dQuantity + "</b></TD> " +
                    //Сумма Пришло
                    "<TD><b>" + dSumQuantity + "</b></TD> " +
                    //Остаток
                    "<TD><b>" + dRemnant + "</b></TD> " +
                    //Сумма Остатка
                    "<TD><b>" + dSumRemnant + "</b></TD> ";

                ret +=
                    //Скидка
                    //"<TD>" + query[i].Discount + "</TD> " +
                    //Оприходовал
                    "<TD> </TD> " +
                    //Дата
                    "<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +

                    //"<TD> </TD> " +
                    //"<TD> </TD> " +
                    "</TR>";

                #endregion


                return ret;

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
                    #endregion

                    where
                    x.Remnant > 0 &&
                    x.DirContractorIDOrg == DirContractorIDOrg 
                    //&& x.doc.Held == true 
                    //&& x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo

                    select new
                    {
                        //Код товара
                        DirNomenID = x.DirNomenID,
                        Sub = x.dirNomen.Sub,
                        //Товар Наименование
                        DirNomenName = x.dirNomen.DirNomenName,
                        DirNomenPatchFull =
                            dirNomensSubGroup.DirNomenName == null ? "" :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

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
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
                        SerialNumber = x.SerialNumber,
                        Barcode = x.Barcode,

                    }
                );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                #endregion


                #region DataReader

                ret += ReportHeader3(pLanguage) + "</center>";
                double dQuantity = 0, dRemnant = 0, dSumQuantity = 0, dSumRemnant = 0;

                //Получение списка
                //var query = await Task.Run(() => queryTemp.ToListAsync());
                var query = queryTemp.ToList();

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {
                    
                    dQuantity += query[i].Quantity;
                    dRemnant += query[i].Remnant;
                    dSumQuantity += query[i].SumQuantity;
                    dSumRemnant += query[i].SumRemnant;


                    //Получить "категорию + наименование" для "iNode" всех рутов
                    //Тормоза из-за этого!
                    //Решение:
                    //Вести колекцию [Sub, DirNomenPatchFull]
                    //Если "Sub" повторяется просто вынять его из библиотеки!
                    //DirNomenPatchFull = await Task.Run(() => alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));


                    ret +=
                        "<TR>" +
                        //Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //Категория
                        "<TD>" + query[i].DirNomenPatchFull + "</TD> " +
                        //Товар Наименование
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //Характеристики
                        "<TD>" + query[i].DirChar + "</TD> ";

                    //Цена закупки
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].PriceCurrency + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    //Цена продажи
                    ret +=
                        //Розница
                        "<TD>" + query[i].PriceRetailVAT + "</TD> " +
                        //Опт
                        "<TD>" + query[i].PriceWholesaleVAT + "</TD> " +
                        //И-М
                        "<TD>" + query[i].PriceIMVAT + "</TD> " +

                        //К-во Пришло
                        "<TD>" + query[i].Quantity + "</TD> " +
                        //Сумма Пришло
                        "<TD>" + query[i].SumQuantity + "</TD> " +
                        //Остаток
                        "<TD>" + query[i].Remnant + "</TD> " +
                        //Сумма Остатка
                        "<TD>" + query[i].SumRemnant + "</TD> ";

                    ret +=
                        //Скидка
                        //"<TD>" + query[i].Discount + "</TD> " +
                        //Оприходовал
                        "<TD>" + query[i].DirEmployeeName + "</TD> " +
                        //Дата
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //Точка
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +

                        //"<TD>" + query[i].SerialNumber + "</TD> " +
                        //"<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";
                    
                }


                ret +=
                    "<TR>" +
                    //Код товара
                    "<TD> </TD> " +
                    //Категория
                    "<TD> </TD> " +
                    //Товар Наименование
                    "<TD> </TD> " +
                    //Характеристики
                    "<TD> </TD> " +
                    //Цена закупки
                    "<TD> </TD> ";

                //Цена продажи
                ret +=
                    //Розница
                    "<TD> </TD> " +
                    //Опт
                    "<TD> </TD> " +
                    //И-М
                    "<TD> </TD> " +

                    //К-во Пришло
                    "<TD><b>" + dQuantity + "</b></TD> " +
                    //Сумма Пришло
                    "<TD><b>" + dSumQuantity + "</b></TD> " +
                    //Остаток
                    "<TD><b>" + dRemnant + "</b></TD> " +
                    //Сумма Остатка
                    "<TD><b>" + dSumRemnant + "</b></TD> ";

                ret +=
                    //Скидка
                    //"<TD>" + query[i].Discount + "</TD> " +
                    //Оприходовал
                    "<TD> </TD> " +
                    //Дата
                    "<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +

                    //"<TD> </TD> " +
                    //"<TD> </TD> " +
                    "</TR>";

                #endregion


                return ret;

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

                    #region Характеристики
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
                    #endregion

                    where
                    y.RemPartyID == y.RemPartyID &&
                    y.DirContractorIDOrg == DirContractorIDOrg &&
                    y.doc.Held == true &&
                    y.doc.DocDate >= DateS && y.doc.DocDate <= DatePo && 
                    (y.doc.ListObjectID == 36 || y.doc.ListObjectID == 57)

                    select new
                    {
                        ListObjectID = y.doc.ListObjectID,

                        //Код товара
                        DirNomenID = y.DirNomenID,
                        Sub = y.dirNomen.Sub,
                        //Товар Наименование
                        DirNomenName = y.dirNomen.DirNomenName,
                        DirNomenPatchFull =
                            dirNomensSubGroup.DirNomenName == null ? "" :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

                        //Цена закупки
                        PriceCurrency = y.PriceCurrency,
                        //Цена продажи Розница
                        PriceRetailCurrency = y.PriceRetailCurrency,
                        //Цена продажи Опт
                        PriceWholesaleCurrency = y.PriceWholesaleCurrency,

                        //К-во
                        Quantity = y.Quantity,
                        //Сумма
                        //Sums = y.Quantity * y.PriceCurrency - y.doc.Discount,
                        //Скидка
                        Sale_Discount = y.doc.Discount,
                        //Продавец
                        DirEmployeeID = y.doc.DirEmployeeID,
                        DirEmployeeName = y.doc.dirEmployee.DirEmployeeName,
                        //Дата
                        DocDate = y.doc.DocDate,
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
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
                        SerialNumber = y.SerialNumber,
                        Barcode = y.Barcode,

                    }
                );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                #endregion


                #region DataReader

                ret += ReportHeader4(pLanguage) + "</center>";
                double dQuantity = 0, dSums = 0;

                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {
                    dQuantity += query[i].Quantity;
                    //dSums += query[i].Sums;

                    //Получить "категорию + наименование" для "iNode" всех рутов
                    //DirNomenPatchFull = await Task.Run(() => alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));

                    ret +=
                        "<TR>" +
                        //Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //Категория
                        "<TD>" + query[i].DirNomenPatchFull + "</TD> " +
                        //Товар Наименование
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //Характеристики
                        "<TD>" + query[i].DirChar + "</TD> ";

                    //Цена закупки
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].PriceCurrency + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    //Цена продажи
                    if (query[i].ListObjectID == 32)
                    {
                        //Опт
                        dSums += query[i].Quantity * query[i].PriceWholesaleCurrency;
                        ret +=
                            "<TD>" + query[i].PriceWholesaleCurrency + "</TD> " +
                            //К-во
                            "<TD>" + query[i].Quantity + "</TD> " +
                            //Сумма
                            "<TD>" + query[i].Quantity * query[i].PriceWholesaleCurrency + "</TD> ";
                    }
                    else
                    {
                        dSums += query[i].Quantity * query[i].PriceRetailCurrency;
                        //Розница
                        ret +=
                            "<TD>" + query[i].PriceRetailCurrency + "</TD> " +
                            //К-во
                            "<TD>" + query[i].Quantity + "</TD> " +
                            //Сумма
                            "<TD>" + query[i].Quantity * query[i].PriceRetailCurrency + "</TD> ";
                    }

                    ret +=
                        //Скидка
                        "<TD>" + query[i].Sale_Discount + "</TD> " +
                        //Продавец
                        "<TD>" + query[i].DirEmployeeName + "</TD> " +
                        //Дата
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //Точка
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +

                        //"<TD>" + query[i].SerialNumber + "</TD> " +
                        //"<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";
                }

                ret +=
                    "<TR>" +
                    //Код товара
                    "<TD> </TD> " +
                    //Категория
                    "<TD> - </TD> " +
                    //Товар Наименование
                    "<TD> </TD> " +
                    //Характеристики
                    "<TD> </TD> " +

                    //Цена закупки
                    "<TD> </TD> " +
                    //Цена продажи
                    "<TD> </TD> " +
                    //К-во
                    "<TD><b>" + dQuantity + "</b></TD> ";

                //Сумма
                /*
                if (DirPriceTypeID == 1) ret += "<TD><b>" + dSums + "</b></TD> ";
                else ret += "<TD><b> - </b></TD> ";
                */
                ret += "<TD><b>" + dSums + "</b></TD> ";

                ret +=
                    //Скидка
                    "<TD> </TD> " +
                    //Продавец
                    "<TD> </TD> " +
                    //Дата
                    "<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +

                    //"<TD> </TD> " +
                    //"<TD> </TD> " +
                    "</TR>";

                #endregion


                return ret;

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
                    #endregion

                    where
                    x.Remnant > 0 && x.Remnant < x.DirNomenMinimumBalance &&
                    x.DirContractorIDOrg == DirContractorIDOrg
                    //&& x.doc.Held == true
                    //x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo


                    group x
                    by new
                    {
                        //Код товара
                        DirNomenID = x.DirNomenID,
                        Sub = x.dirNomen.Sub,
                        
                        //Товар Наименование
                        DirNomenName = x.dirNomen.DirNomenName,
                        DirNomenPatchFull =
                            dirNomensSubGroup.DirNomenName == null ? "" :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

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
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
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
                        DirNomenPatchFull = g.Key.DirNomenPatchFull,

                        //Цена закупки
                        PriceCurrency = g.Key.PriceCurrency,

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

                #endregion


                #region DataReader

                ret += ReportHeader5(pLanguage) + "</center>";
                double dQuantity = 0, dRemnant = 0; //, dSumQuantity = 0, dSumRemnant = 0;

                //Получение списка
                //var query = await Task.Run(() => queryTemp.ToListAsync());
                var query = queryTemp.ToList();

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {

                    dQuantity += query[i].Quantity;
                    dRemnant += query[i].Remnant;
                    //dSumQuantity += query[i].SumQuantity;
                    //dSumRemnant += query[i].SumRemnant;


                    //Получить "категорию + наименование" для "iNode" всех рутов
                    //Тормоза из-за этого!
                    //Решение:
                    //Вести колекцию [Sub, DirNomenPatchFull]
                    //Если "Sub" повторяется просто вынять его из библиотеки!
                    //DirNomenPatchFull = await Task.Run(() => alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));


                    ret +=
                        "<TR>" +
                        //Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //Категория
                        "<TD>" + query[i].DirNomenPatchFull + "</TD> " +
                        //Товар Наименование
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //Характеристики
                        "<TD>" + query[i].DirChar + "</TD> ";

                    //Цена закупки
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].PriceCurrency + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    //Цена продажи
                    ret +=
                        //Розница
                        "<TD>" + query[i].PriceRetailVAT + "</TD> " +
                        //Опт
                        "<TD>" + query[i].PriceWholesaleVAT + "</TD> " +
                        //И-М
                        "<TD>" + query[i].PriceIMVAT + "</TD> " +

                        //К-во Пришло
                        "<TD>" + query[i].Quantity + "</TD> " +
                        //Сумма Пришло
                        //"<TD>" + query[i].SumQuantity + "</TD> " +
                        //Остаток
                        "<TD>" + query[i].Remnant + "</TD> " +
                        //Сумма Остатка
                        //"<TD>" + query[i].SumRemnant + "</TD> " +

                        //Минимальный остаток
                        "<TD>" + query[i].DirNomenMinimumBalance + "</TD> ";

                    ret +=
                        //Скидка
                        //"<TD>" + query[i].Discount + "</TD> " +
                        //Оприходовал
                        "<TD>" + query[i].DirEmployeeName + "</TD> " +
                        //Дата
                        //"<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //Точка
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +

                        //"<TD>" + query[i].SerialNumber + "</TD> " +
                        //"<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";

                }


                ret +=
                    "<TR>" +
                    //Код товара
                    "<TD> </TD> " +
                    //Категория
                    "<TD> </TD> " +
                    //Товар Наименование
                    "<TD> </TD> " +
                    //Характеристики
                    "<TD> </TD> " +

                    //Цена закупки
                    "<TD> </TD> ";

                //Цена продажи
                ret +=
                    //Розница
                    "<TD> </TD> " +
                    //Опт
                    "<TD> </TD> " +
                    //И-М
                    "<TD> </TD> " +

                    //К-во Пришло
                    "<TD><b>" + dQuantity + "</b></TD> " +
                    //Сумма Пришло
                    //"<TD><b>" + dSumQuantity + "</b></TD> " +
                    //Остаток
                    "<TD><b>" + dRemnant + "</b></TD> ";
                    //Сумма Остатка
                    //"<TD><b>" + dSumRemnant + "</b></TD> ";

                ret +=
                    //Скидка
                    //"<TD>" + query[i].Discount + "</TD> " +
                    //Оприходовал
                    "<TD> </TD> " +
                    //Дата
                    "<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +

                    //"<TD> </TD> " +
                    //"<TD> </TD> " +
                    "</TR>";

                #endregion


                return ret;

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
                    #endregion

                    where
                    x.RemPartyID == y.RemPartyID &&
                    x.DirContractorIDOrg == DirContractorIDOrg &&
                    //x.doc.Held == true &&
                    x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo &&
                    (x.doc.ListObjectID == 35)

                    select new
                    {
                        //Код товара
                        DirNomenID = x.DirNomenID,
                        Sub = x.dirNomen.Sub,

                        //Товар Наименование
                        DirNomenName = x.dirNomen.DirNomenName,
                        DirNomenPatchFull =
                            dirNomensSubGroup.DirNomenName == null ? "" :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

                        //Цена закупки
                        Purch_PriceCurrency = y.PriceCurrency,
                        //Цена продажи
                        Sale_PriceCurrency = x.PriceCurrency,
                        //К-во
                        Sale_Quantity = x.Quantity,
                        //Сумма
                        Sums = x.Quantity * x.PriceCurrency - x.doc.Discount,
                        //Прибыль
                        SumProfit = x.Quantity * (x.PriceCurrency - y.PriceCurrency) - x.doc.Discount,
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
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
                        SerialNumber = y.SerialNumber,
                        Barcode = y.Barcode,

                    }
                );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                #endregion


                #region DataReader

                ret += ReportHeader1(pLanguage) + "</center>";
                double dQuantity = 0, dSums = 0, dSumProfit = 0;

                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {
                    dQuantity += query[i].Sale_Quantity;
                    dSums += query[i].Sums;
                    dSumProfit += query[i].SumProfit;

                    //Получить "категорию + наименование" для "iNode" всех рутов
                    //DirNomenPatchFull = await Task.Run(() => alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));

                    ret +=
                        "<TR>" +
                        //Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //Категория
                        "<TD>" + query[i].DirNomenPatchFull + "</TD> " +
                        //Товар Наименование
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //Характеристики
                        "<TD>" + query[i].DirChar + "</TD> ";

                    //Цена закупки
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].Purch_PriceCurrency + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    //Цена продажи
                    ret +=
                        "<TD>" + query[i].Sale_PriceCurrency + "</TD> " +
                        //К-во
                        "<TD>" + query[i].Sale_Quantity + "</TD> " +
                        //Сумма
                        "<TD>" + query[i].Sums + "</TD> ";

                    //Прибыль
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].SumProfit + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    ret +=
                        //Скидка
                        "<TD>" + query[i].Sale_Discount + "</TD> " +
                        //Продавец
                        "<TD>" + query[i].DirEmployeeName + "</TD> " +
                        //Дата
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //Точка
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +

                        //"<TD>" + query[i].SerialNumber + "</TD> " +
                        //"<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";
                }

                ret +=
                    "<TR>" +
                    //Код товара
                    "<TD> </TD> " +
                    //Категория
                    "<TD> - </TD> " +
                    //Товар Наименование
                    "<TD> </TD> " +
                    //Характеристики
                    "<TD> </TD> " +

                    //Цена закупки
                    "<TD> </TD> " +
                    //Цена продажи
                    "<TD> </TD> " +
                    //К-во
                    "<TD><b>" + dQuantity + "</b></TD> ";

                //Сумма
                if (DirPriceTypeID == 1) ret += "<TD><b>" + dSums + "</b></TD> ";
                else ret += "<TD><b> - </b></TD> ";

                //Прибыль
                if (DirPriceTypeID == 1) ret += "<TD><b>" + dSumProfit + "</b></TD> ";
                else ret += "<TD><b> - </b></TD> ";

                ret +=
                    //Скидка
                    "<TD> </TD> " +
                    //Продавец
                    "<TD> </TD> " +
                    //Дата
                    "<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +

                    //"<TD> </TD> " +
                    //"<TD> </TD> " +
                    "</TR>";

                #endregion


                return ret;

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
                    #endregion

                    where
                    x.Remnant == 0 &&
                    x.DirContractorIDOrg == DirContractorIDOrg
                    //&& x.doc.Held == true
                    //x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo


                    group new { x }
                    by new
                    {
                        //Код товара
                        DirNomenID = x.DirNomenID,
                        Sub = x.dirNomen.Sub,
                        //Товар Наименование
                        DirNomenName = x.dirNomen.DirNomenName,
                        DirNomenPatchFull =
                            dirNomensSubGroup.DirNomenName == null ? "" :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

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
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
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
                        DirNomenPatchFull = g.Key.DirNomenPatchFull,

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
                        Quantity = g.Key.Quantity,
                        //Сумма Пришло
                        SumQuantity = g.Key.SumQuantity, //g.Key.Quantity * g.Key.PriceCurrency - g.Key.SumQuantity,

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


                #region DataReader

                ret += ReportHeader2(pLanguage) + "</center>";
                double dQuantity = 0, dRemnant = 0, dSumQuantity = 0, dSumRemnant = 0;

                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());
                //var query = queryTemp.ToList();

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {

                    dQuantity += query[i].Quantity;
                    dRemnant += query[i].Remnant;
                    dSumQuantity += query[i].SumQuantity;
                    dSumRemnant += query[i].SumRemnant;


                    //Получить "категорию + наименование" для "iNode" всех рутов
                    //Тормоза из-за этого!
                    //Решение:
                    //Вести колекцию [Sub, DirNomenPatchFull]
                    //Если "Sub" повторяется просто вынять его из библиотеки!
                    //DirNomenPatchFull = await Task.Run(() => alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));

                    ret +=
                        "<TR>" +
                        //Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //Категория
                        "<TD>" + query[i].DirNomenPatchFull + "</TD> " +
                        //Товар Наименование
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //Характеристики
                        "<TD>" + query[i].DirChar + "</TD> ";

                    //Цена закупки
                    //if (DirPriceTypeID == 1) ret += "<TD>" + query[i].PriceCurrency + "</TD> ";
                    //else ret += "<TD> - </TD> ";

                    //Цена продажи
                    ret +=
                        //Розница
                        "<TD>" + query[i].PriceRetailVAT + "</TD> " +
                        //Опт
                        "<TD>" + query[i].PriceWholesaleVAT + "</TD> " +
                        //И-М
                        "<TD>" + query[i].PriceIMVAT + "</TD> " +

                        //К-во Пришло
                        "<TD>" + query[i].Quantity + "</TD> " +
                        //Сумма Пришло
                        "<TD>" + query[i].SumQuantity + "</TD> " +
                        //Остаток
                        "<TD>" + query[i].Remnant + "</TD> " +
                        //Сумма Остатка
                        "<TD>" + query[i].SumRemnant + "</TD> ";

                    ret +=
                        //Скидка
                        //"<TD>" + query[i].Discount + "</TD> " +
                        //Оприходовал
                        "<TD>" + query[i].DirEmployeeName + "</TD> " +
                        //Дата
                        //"<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //Точка
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +

                        //"<TD>" + query[i].SerialNumber + "</TD> " +
                        //"<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";

                }


                ret +=
                    "<TR>" +
                    //Код товара
                    "<TD> </TD> " +
                    //Категория
                    "<TD> </TD> " +
                    //Товар Наименование
                    "<TD> </TD> " +
                    //Характеристики
                    "<TD> </TD> " +

                    //Цена закупки
                    "<TD> </TD> ";

                //Цена продажи
                ret +=
                    //Розница
                    "<TD> </TD> " +
                    //Опт
                    "<TD> </TD> " +
                    //И-М
                    "<TD> </TD> " +

                    //К-во Пришло
                    "<TD><b>" + dQuantity + "</b></TD> " +
                    //Сумма Пришло
                    "<TD><b>" + dSumQuantity + "</b></TD> " +
                    //Остаток
                    "<TD><b>" + dRemnant + "</b></TD> " +
                    //Сумма Остатка
                    "<TD><b>" + dSumRemnant + "</b></TD> ";

                ret +=
                    //Скидка
                    //"<TD>" + query[i].Discount + "</TD> " +
                    //Оприходовал
                    "<TD> </TD> " +
                    //Дата
                    //"<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +

                    //"<TD> </TD> " +
                    //"<TD> </TD> " +
                    "</TR>";

                #endregion


                return ret;

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
                    #endregion

                    where
                    x.DirContractorIDOrg == DirContractorIDOrg &&
                    x.DirReturnTypeID == 3 &&
                    //x.doc.Held == true &&
                    x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo

                    #region select
                    select new
                    {
                        //Код товара
                        DirNomenID = x.DirNomenID,
                        Sub = x.dirNomen.Sub,
                        //Товар Наименование
                        DirNomenName = x.dirNomen.DirNomenName,
                        DirNomenPatchFull =
                            dirNomensSubGroup.DirNomenName == null ? "" :
                            dirNomensGroup.DirNomenName == null ? dirNomensSubGroup.DirNomenName :
                            dirNomensGroup.DirNomenName + " / " + dirNomensSubGroup.DirNomenName,

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
                        DirEmployeeID = x.doc.DirEmployeeID,
                        DirEmployeeName = x.doc.dirEmployee.DirEmployeeName,
                        //Дата
                        DocDate = x.doc.DocDate,
                        //Точка
                        DirWarehouseID = x.DirWarehouseID,
                        DirWarehouseName = x.dirWarehouse.DirWarehouseName,


                        //Характеристики
                        DirCharColourID = dirCharColours.DirCharColourID,
                        DirCharMaterialID = dirCharMaterials.DirCharMaterialID,
                        DirCharNameID = dirCharNames.DirCharNameID,
                        DirCharSeasonID = dirCharSeasons.DirCharSeasonID,
                        DirCharSexID = dirCharSexes.DirCharSexID,
                        DirCharSizeID = dirCharSizes.DirCharSizeID,
                        DirCharStyleID = dirCharStyles.DirCharStyleID,
                        DirCharTextureID = dirCharTextures.DirCharTextureID,

                        DirChar =
                                dirCharColours.DirCharColourName + " " +
                                dirCharMaterials.DirCharMaterialName + " " +
                                dirCharNames.DirCharNameName + " " +
                                dirCharSeasons.DirCharSeasonName + " " +
                                dirCharSexes.DirCharSexName + " " +
                                dirCharSizes.DirCharSizeName + " " +
                                dirCharStyles.DirCharStyleName + " " +
                                dirCharTextures.DirCharTextureName,
                        SerialNumber = x.SerialNumber,
                        Barcode = x.Barcode,

                        DirReturnTypeID = x.DirReturnTypeID, DirReturnTypeName = x.dirReturnType.DirReturnTypeName,
                        DirDescriptionID = x.DirDescriptionID, DirDescriptionName = x.dirDescription.DirDescriptionName,

                    }
                    #endregion
                );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                #endregion


                #region DataReader

                ret += ReportHeader3(pLanguage) + "</center>";
                double dQuantity = 0, dRemnant = 0, dSumQuantity = 0, dSumRemnant = 0;

                //Получение списка
                //var query = await Task.Run(() => queryTemp.ToListAsync());
                var query = queryTemp.ToList();

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {

                    dQuantity += query[i].Quantity;
                    dRemnant += query[i].Remnant;
                    dSumQuantity += query[i].SumQuantity;
                    dSumRemnant += query[i].SumRemnant;


                    //Получить "категорию + наименование" для "iNode" всех рутов
                    //Тормоза из-за этого!
                    //Решение:
                    //Вести колекцию [Sub, DirNomenPatchFull]
                    //Если "Sub" повторяется просто вынять его из библиотеки!
                    //DirNomenPatchFull = await Task.Run(() => alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));

                    ret +=
                        "<TR>" +
                        //Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //Категория
                        "<TD>" + query[i].DirNomenPatchFull + "</TD> " +
                        //Товар Наименование
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //Характеристики
                        "<TD>" + query[i].DirChar + "</TD> ";

                    //Цена закупки
                    if (DirPriceTypeID == 1) ret += "<TD>" + query[i].PriceCurrency + "</TD> ";
                    else ret += "<TD> - </TD> ";

                    //Цена продажи
                    ret +=
                        //Розница
                        "<TD>" + query[i].PriceRetailVAT + "</TD> " +
                        //Опт
                        "<TD>" + query[i].PriceWholesaleVAT + "</TD> " +
                        //И-М
                        "<TD>" + query[i].PriceIMVAT + "</TD> " +

                        //К-во Пришло
                        "<TD>" + query[i].Quantity + "</TD> " +
                        //Сумма Пришло
                        "<TD>" + query[i].SumQuantity + "</TD> " +
                        //Остаток
                        "<TD>" + query[i].Remnant + "</TD> " +
                        //Сумма Остатка
                        "<TD>" + query[i].SumRemnant + "</TD> ";

                    ret +=
                        //Скидка
                        //"<TD>" + query[i].Discount + "</TD> " +
                        //Оприходовал
                        "<TD>" + query[i].DirEmployeeName + "</TD> " +
                        //Дата
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //Точка
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +

                        //"<TD>" + query[i].SerialNumber + "</TD> " +
                        //"<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";

                }


                ret +=
                    "<TR>" +
                    //Код товара
                    "<TD> </TD> " +
                    //Категория
                    "<TD> </TD> " +
                    //Товар Наименование
                    "<TD> </TD> " +
                    //Характеристики
                    "<TD> </TD> " +
                    //Цена закупки
                    "<TD> </TD> ";

                //Цена продажи
                ret +=
                    //Розница
                    "<TD> </TD> " +
                    //Опт
                    "<TD> </TD> " +
                    //И-М
                    "<TD> </TD> " +

                    //К-во Пришло
                    "<TD><b>" + dQuantity + "</b></TD> " +
                    //Сумма Пришло
                    "<TD><b>" + dSumQuantity + "</b></TD> " +
                    //Остаток
                    "<TD><b>" + dRemnant + "</b></TD> " +
                    //Сумма Остатка
                    "<TD><b>" + dSumRemnant + "</b></TD> ";

                ret +=
                    //Скидка
                    //"<TD>" + query[i].Discount + "</TD> " +
                    //Оприходовал
                    "<TD> </TD> " +
                    //Дата
                    "<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +

                    //"<TD> </TD> " +
                    //"<TD> </TD> " +
                    "</TR>";

                #endregion


                return ret;

                #endregion
            }

            return ret;

        }

        //Формирование коллекции "alDirNomenPatchFull"
        internal async Task<string> alDirNomenPatchFullExist(DbConnectionSklad db, int? Sub, string Name)
        {
            for (int i = 0; i < alDirNomenPatchFull.Count; i++)
            {
                string[] al = (string[])alDirNomenPatchFull[i];
                if (al[0] == Sub.ToString())
                {
                    return al[1];
                }
            }

            Controllers.Sklad.Dir.DirNomens.DirNomensController dirNomensController = new Dir.DirNomens.DirNomensController();
            string[] _al = new string[2];
            _al[0] = Sub.ToString();

            string[] ret = await Task.Run(() => dirNomensController.DirNomenSubFind2(db, Sub));
            _al[1] = ret[0];
            alDirNomenPatchFull.Add(_al);

            return _al[1];
        }



        //СТАНДАРТНАЯ табличная часть
        private string ReportHeader1(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Прибыль</TD> <TD class='table_header'>Скидка</TD> <TD class='table_header'>Продавец</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Прибыль</TD> <TD class='table_header'>Скидка</TD> <TD class='table_header'>Продавец</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";
            }
        }

        //СТАНДАРТНАЯ табличная часть
        private string ReportHeader2(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена продажи Розница</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Цена продажи Опт</TD> <TD class='table_header'>Цена продажи И-М</TD> <TD class='table_header'>К-во Пришло</TD> <TD class='table_header'>Сумма Пришло</TD> <TD class='table_header'>Остаток</TD> <TD class='table_header'>Сумма Остатка</TD> <TD class='table_header'>Оприходовал</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена продажи Розница</TD> <TD class='table_header'>Цена продажи Опт</TD> <TD class='table_header'>Цена продажи И-М</TD> <TD class='table_header'>К-во Пришло</TD> <TD class='table_header'>Сумма Пришло</TD> <TD class='table_header'>Остаток</TD> <TD class='table_header'>Сумма Остатка</TD> <TD class='table_header'>Оприходовал</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";
            }
        }

        //СТАНДАРТНАЯ табличная часть
        private string ReportHeader3(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи Розница</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Цена продажи Опт</TD> <TD class='table_header'>Цена продажи И-М</TD> <TD class='table_header'>К-во Пришло</TD> <TD class='table_header'>Сумма Пришло</TD> <TD class='table_header'>Остаток</TD> <TD class='table_header'>Сумма Остатка</TD> <TD class='table_header'>Оприходовал</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи Розница</TD> <TD class='table_header'>Цена продажи Опт</TD> <TD class='table_header'>Цена продажи И-М</TD> <TD class='table_header'>К-во Пришло</TD> <TD class='table_header'>Сумма Пришло</TD> <TD class='table_header'>Остаток</TD> <TD class='table_header'>Сумма Остатка</TD> <TD class='table_header'>Оприходовал</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";
            }
        }

        //СТАНДАРТНАЯ табличная часть
        private string ReportHeader4(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Скидка</TD> <TD class='table_header'>Продавец</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Скидка</TD> <TD class='table_header'>Продавец</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";
            }
        }

        //СТАНДАРТНАЯ табличная часть
        private string ReportHeader5(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи Розница</TD> <TD class='table_header'>Цена продажи Опт</TD> <TD class='table_header'>Цена продажи И-М</TD> <TD class='table_header'>К-во Пришло</TD> <TD class='table_header'>Остаток</TD> <TD class='table_header'>Мин.ост.</TD> <TD class='table_header'>Оприходовал</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи Розница</TD> <TD class='table_header'>Цена продажи Опт</TD> <TD class='table_header'>Цена продажи И-М</TD> <TD class='table_header'>К-во Пришло</TD> <TD class='table_header'>Остаток</TD> <TD class='table_header'>Мин.ост.</TD> <TD class='table_header'>Оприходовал</TD> <TD class='table_header'>Точка</TD> " + 
                    "</TR>";
            }
        }
    }
}