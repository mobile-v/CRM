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
    public class ReportRemnants
    {
        string pID = "";
        bool OperationalBalances = false;
        int pLanguage = 0, DirContractorIDOrg = 0, DirWarehouseID = 0, DirEmployeeID = 0;
        string DirContractorNameOrg, DirWarehouseName, DirEmployeeName;
        DateTime DateS, DatePo;

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db)
        {
            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);
            DateS = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DateS"]).ToString("yyyy-MM-dd 00:00:00"));
            DatePo = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DatePo"]).ToString("yyyy-MM-dd 23:59:59"));

            OperationalBalances = false;
            bool bOperationalBalances = Boolean.TryParse(Request.Params["OperationalBalances"], out OperationalBalances);

            DirContractorIDOrg = 0;
            bool bDirContractorIDOrg = Int32.TryParse(Request.Params["DirContractorIDOrg"], out DirContractorIDOrg);
            DirContractorNameOrg = Request.Params["DirContractorNameOrg"];

            DirWarehouseID = 0;
            bool bDirWarehouseID = Int32.TryParse(Request.Params["DirWarehouseID"], out DirWarehouseID);
            DirWarehouseName = Request.Params["DirWarehouseName"];

            DirEmployeeID = 0;
            bool bDirEmployeeID = Int32.TryParse(Request.Params["DirEmployeeID"], out DirEmployeeID);
            DirEmployeeName = Request.Params["DirEmployeeName"];

            #endregion


            if (OperationalBalances)
            {
                #region Оперативные остатки

                string ret =
                    "<center>" +
                    "<h2>Остатки по юр.лицу " + DirContractorNameOrg + "</h2>";

                var queryTemp =
                    (
                        from x in db.RemRemnants
                        where x.DirContractorIDOrg == DirContractorIDOrg
                        select new
                        {
                            DirWarehouseID = x.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,

                            DirNomenID = x.DirNomenID,
                            DirNomenName = x.dirNomen.DirNomenName,

                            x.Quantity
                        }
                    );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);


                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());

                ret += ReportHeader1(pLanguage) + "</center>";

                for (int i = 0; i < query.Count(); i++)
                {
                    ret +=
                        "<TR>" +
                        //1. Дата продажи
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //2. Код товара
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //3. Наименование товара
                        "<TD>" + query[i].Quantity + "</TD> " +
                        //7. Цена прибыли
                        "<TD>" + query[i].DirWarehouseName + "</TD> " +
                        "</TR>";
                }

                return ret;

                #endregion
            }
            else
            {
                string ret =
                    "<center>" +
                    "<h2>Остатки с " + DateS.ToString("yyyy-MM-dd") + " по " + DatePo.ToString("yyyy-MM-dd") + " по юр.лицу " + DirContractorNameOrg + "</h2>";

                var queryTemp =
                    (
                        from x in db.RemParties

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

                        where x.DirContractorIDOrg == DirContractorIDOrg && x.Remnant > 0 && x.doc.DocDate >= DateS && x.doc.DocDate <= DatePo
                        select new
                        {
                            DirWarehouseID = x.DirWarehouseID,
                            DirWarehouseName = x.dirWarehouse.DirWarehouseName,

                            DocDate = x.doc.DocDate,

                            DirEmployeeID = x.doc.DirEmployeeID,

                            DirNomenID = x.DirNomenID,
                            DirNomenName = x.dirNomen.DirNomenName,

                            Remnant = x.Remnant,
                            PriceCurrency = x.PriceCurrency,
                            Sum = x.Remnant * x.PriceCurrency,
                            SumDiscount = (1 - x.doc.Discount / 100) * x.Remnant * x.PriceCurrency,

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
                            Barcode = x.Barcode
                        }
                    );

                if (DirWarehouseID > 0) queryTemp = queryTemp.Where(z => z.DirWarehouseID == DirWarehouseID);
                if (DirEmployeeID > 0) queryTemp = queryTemp.Where(z => z.DirEmployeeID == DirEmployeeID);

                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());

                ret += ReportHeader2(pLanguage) + "</center>";

                double dSum = 0;
                for (int i = 0; i < query.Count(); i++)
                {
                    dSum += Convert.ToDouble(query[i].Sum);

                    ret +=
                        "<TR>" +
                        //1. Дата продажи
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        //2. Код товара
                        "<TD>" + query[i].DirNomenID + "</TD> " +
                        //3. Наименование товара
                        "<TD>" + query[i].DirNomenName + "</TD> " +
                        //4. К-во
                        "<TD>" + query[i].Remnant + "</TD> " +
                        //5. 
                        "<TD>" + query[i].PriceCurrency + "</TD> " +
                        //6. 
                        "<TD>" + query[i].Sum + "</TD> " +
                        //7. 
                        "<TD>" + query[i].DirChar + "</TD> " +
                        //8. 
                        "<TD>" + query[i].SerialNumber + "</TD> " +
                        //9. 
                        "<TD>" + query[i].Barcode + "</TD> " +
                        "</TR>";

                }


                ret +=
                    "<TR>" +
                    //1.
                    "<TD> </TD> " +
                    //2.
                    "<TD> </TD> " +
                    //3.
                    "<TD> </TD> " +
                    //4.
                    "<TD> </TD> " +
                    //5.
                    "<TD> </TD> " +
                    //6.
                    "<TD> </TD> " +
                    //7.
                    "<TD> </TD> " +
                    //8.
                    "<TD> </TD> " +
                    //9.
                    "<TD> </TD> " +
                    "</TR>" +

                    "<TR>" +
                    //1.
                    "<TD> <b>Итого</b> </TD> " +
                    //2.
                    "<TD> </TD> " +
                    //3.
                    "<TD> </TD> " +
                    //4.
                    "<TD> </TD> " +
                    //5.
                    "<TD> </TD> " +
                    //6.
                    "<TD> <b>" + dSum.ToString() + "</b> </TD> " + 
                    //7.
                    "<TD> </TD> " +
                    //8.
                    "<TD> </TD> " +
                    //9.
                    "<TD> </TD> " +
                    "</TR>";

                ret += "</TABLE><br />";


                return ret;
            }

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
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Найменування</TD> <TD class='table_header'>К-ть</TD> <TD class='table_header'>Склад</TD> " +
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Склад</TD> " +
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
                    "<TD class='table_header'>Дата</TD> <TD class='table_header'>Код</TD> <TD class='table_header'>Найменування</TD> <TD class='table_header'>К-ть</TD> <TD class='table_header'>Ціна</TD> <TD class='table_header'>Сума</TD> <TD class='table_header'>Характер.</TD> <TD class='table_header'>Серійний</TD> <TD class='table_header'>Штрих</TD> " +
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Дата</TD> <TD class='table_header'>Код</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Цена</TD> <TD class='table_header'>Сумма</TD> <TD class='table_header'>Характер.</TD> <TD class='table_header'>Серийный</TD> <TD class='table_header'>Штрих</TD> " +
                    "</TR>";
            }
        }

    }
}