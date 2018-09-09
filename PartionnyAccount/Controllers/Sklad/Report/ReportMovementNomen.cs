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
    public class ReportMovementNomen
    {
        string pID = "";
        bool ProfitNomenAll = false;
        int pLanguage = 0, DirContractorIDOrg = 0, DirWarehouseIDTo = 0, DirWarehouseIDFrom = 0;
        string DirContractorNameOrg, DirWarehouseNameTo, DirWarehouseNameFrom;
        DateTime DateS, DatePo;
        ArrayList alDirNomenPatchFull = new ArrayList();

        PartionnyAccount.Controllers.Sklad.Report.ReportTotalTrade reportTotalTrade = new ReportTotalTrade();

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

            DirWarehouseIDTo = 0;
            bool bDirWarehouseIDTo = Int32.TryParse(Request.Params["DirWarehouseIDTo"], out DirWarehouseIDTo);
            DirWarehouseNameTo = Request.Params["DirWarehouseNameTo"];

            DirWarehouseIDFrom = 0;
            bool bDirWarehouseIDFrom = Int32.TryParse(Request.Params["DirWarehouseIDFrom"], out DirWarehouseIDFrom);
            DirWarehouseNameFrom = Request.Params["DirWarehouseNameFrom"];

            #endregion

            string
                DirNomenPatchFull = "",
                ret =
                "<center>" +
                "<h2>Заказ товара на перемещение</h2>" +
                "<span style='color: red;'>красный</span> - нет или не хватает товара на основном складе<br />" +
                "<span style='color: green;'>зелёный</span> - хватает товара на основном складе";


            #region query

            var queryTemp =
            (
                from x in db.RemParties

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

                /*
                join remParties_1 in db.RemParties on x.DirNomenID equals remParties_1.DirNomenID into remParties_2
                from remParties_ in remParties_2
                .Where(
                        c => c.DirWarehouseID == DirWarehouseIDFrom && 
                        c.DirCharColourID == x.DirCharColourID &&
                        c.DirCharMaterialID == x.DirCharMaterialID &&
                        c.DirCharNameID == x.DirCharNameID &&
                        c.DirCharSeasonID == x.DirCharSeasonID &&
                        c.DirCharSexID == x.DirCharSexID &&
                        c.DirCharSizeID == x.DirCharSizeID &&
                        c.DirCharStyleID == x.DirCharStyleID &&
                        c.DirCharTextureID == x.DirCharTextureID
                      ).DefaultIfEmpty()
                      */

                where
                x.DirContractorIDOrg == DirContractorIDOrg &&
                (x.Remnant > 0 && x.Remnant < x.DirNomenMinimumBalance)
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

                    //Remnant_ = remParties_.Remnant,
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
                    DirCharColourID = g.Key.DirCharColourID,
                    DirCharMaterialID = g.Key.DirCharMaterialID,
                    DirCharNameID = g.Key.DirCharNameID,
                    DirCharSeasonID = g.Key.DirCharSeasonID,
                    DirCharSexID = g.Key.DirCharSexID,
                    DirCharSizeID = g.Key.DirCharSizeID,
                    DirCharStyleID = g.Key.DirCharStyleID,
                    DirCharTextureID = g.Key.DirCharTextureID,
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
                    //SerialNumber =g.Key.SerialNumber,
                    //Barcode =g.Key.Barcode,

                    //Remnant_ = remParties_.Remnant,

                }
            );

            if (DirWarehouseIDTo > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseIDTo);

            #endregion


            #region DataReader

            ret += ReportHeader5(pLanguage) + "</center>";
            double dQuantity = 0, dRemnant = 0, dSumQuantity = 0, dSumRemnant = 0;

            //Получение списка
            //var query = await Task.Run(() => queryTemp.ToListAsync());
            var query = queryTemp.ToList();

            //ДатаРидер
            for (int i = 0; i < query.Count(); i++)
            {
                if (query[i].DirNomenID == 22310) {
                    int ididid = query[i].DirNomenID;
                }

                dQuantity += Convert.ToDouble(query[i].Quantity);
                dRemnant += Convert.ToDouble(query[i].Remnant);
                //dSumQuantity += query[i].SumQuantity;
                //dSumRemnant += query[i].SumRemnant;


                //Получить "категорию + наименование" для "iNode" всех рутов
                //Тормоза из-за этого!
                //Решение:
                //Вести колекцию [Sub, DirNomenPatchFull]
                //Если "Sub" повторяется просто вынять его из библиотеки!
                //DirNomenPatchFull = await Task.Run(() => dirNomensController.DirNomenSubFind2(db, query[i].Sub));
                DirNomenPatchFull = await Task.Run(() => reportTotalTrade.alDirNomenPatchFullExist(db, query[i].Sub, query[i].DirNomenName));
                double? Remnant = query[i].Remnant, DirNomenMinimumBalance = query[i].DirNomenMinimumBalance;

                double? Remnant_ = await mRemnant(
                           db,
                           DirWarehouseIDFrom,
                           query[i].DirNomenID,
                           query[i].DirCharColourID,
                           query[i].DirCharMaterialID,
                           query[i].DirCharNameID,
                           query[i].DirCharSeasonID,
                           query[i].DirCharSexID,
                           query[i].DirCharSizeID,
                           query[i].DirCharStyleID,
                           query[i].DirCharTextureID
                           );

                string color = " bgcolor='red'";
                if (Remnant_ >= DirNomenMinimumBalance - Remnant) { color = " bgcolor='green'"; }

                ret +=
                    "<TR"+ color + ">" +
                    //Код товара
                    "<TD>" + query[i].DirNomenID + "</TD> " +
                    //Категория
                    "<TD>" + DirNomenPatchFull + "</TD> " +
                    //Товар Наименование
                    "<TD>" + query[i].DirNomenName + "</TD> ";

                //Цена закупки
                //if (DirPriceTypeID == 1) ret += "<TD>" + query[i].PriceCurrency + "</TD> ";
                //else ret += "<TD> - </TD> ";
                ret += "<TD>" + query[i].PriceCurrency + "</TD> ";

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
                    //Остаток в Точке
                    "<TD><b>" + query[i].Remnant + "</b></TD> " +
                    //Остаток на основном складе
                    "<TD><b>" + Remnant_ + "</b></TD> " +
                    //Сумма Остатка
                    //"<TD>" + query[i].SumRemnant + "</TD> " +

                    //Минимальный остаток
                    "<TD>" + DirNomenMinimumBalance + "</TD> ";

                ret +=
                    //Скидка
                    //"<TD>" + query[i].Discount + "</TD> " +
                    //Оприходовал
                    "<TD>" + query[i].DirEmployeeName + "</TD> " +
                    //Дата
                    //"<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                    //Точка
                    "<TD>" + query[i].DirWarehouseName + "</TD> " +

                    //Характеристики
                    "<TD>" + query[i].DirChar + "</TD> " +
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

                //Характеристики
                "<TD> </TD> " +
                "<TD> </TD> " +
                //"<TD> </TD> " +
                "</TR>";

            #endregion


            return ret;

        }

        //Получаем остаток на 
        private async Task<double?> mRemnant(
            DbConnectionSklad db, 
            int? DirWarehouseIDFrom,
            int? DirNomenID, 
            int? DirCharColourID,
            int? DirCharMaterialID,
            int? DirCharNameID,
            int? DirCharSeasonID,
            int? DirCharSexID,
            int? DirCharSizeID,
            int? DirCharStyleID,
            int? DirCharTextureID
            )
        {
            double? querySum = await
                (
                    from x in db.RemParties

                    where 
                        x.DirWarehouseID== DirWarehouseIDFrom &&
                        x.DirNomenID == DirNomenID &&
                        x.DirCharColourID == DirCharColourID &&
                        x.DirCharMaterialID == DirCharMaterialID &&
                        x.DirCharNameID == DirCharNameID &&
                        x.DirCharSeasonID == DirCharSeasonID &&
                        x.DirCharSexID == DirCharSexID &&
                        x.DirCharSizeID == DirCharSizeID &&
                        x.DirCharStyleID == DirCharStyleID &&
                        x.DirCharTextureID == DirCharTextureID

                    select x.Remnant
                ).DefaultIfEmpty(0).SumAsync();

            return querySum;
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
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи Розница</TD> <TD class='table_header'>Цена продажи Опт</TD> <TD class='table_header'>Цена продажи И-М</TD> <TD class='table_header'>К-во Пришло</TD> <TD class='table_header'><b>Остаток в Точке</b></TD> <TD class='table_header'><b>Остаток на Основном складе</b></TD> <TD class='table_header'>Мин.ост.</TD> <TD class='table_header'>Оприходовал</TD> <TD class='table_header'>Точка</TD> <TD class='table_header'>Х-ки</TD>" + // <TD class='table_header'>SerialNumber</TD> <TD class='table_header'>Barcode</TD>
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Цена закупки</TD> <TD class='table_header'>Цена продажи Розница</TD> <TD class='table_header'>Цена продажи Опт</TD> <TD class='table_header'>Цена продажи И-М</TD> <TD class='table_header'>К-во Пришло</TD> <TD class='table_header'><b>Остаток в Точке</b></TD> <TD class='table_header'><b>Остаток на Основном складе</b></TD> <TD class='table_header'>Мин.ост.</TD> <TD class='table_header'>Оприходовал</TD> <TD class='table_header'>Точка</TD> <TD class='table_header'>Х-ки</TD>" + // <TD class='table_header'>SerialNumber</TD> <TD class='table_header'>Barcode</TD>
                    "</TR>";
            }
        }

    }
}