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
    public class ReportLogistics
    {
        string pID = "";
        int pLanguage = 0, DirContractorIDOrg = 0, DirEmployeeID = 0, DirMovementStatusID = 0, DocOrTab = 1;
        string DirContractorNameOrg, DirEmployeeName, DirMovementStatusName;
        DateTime DateS, DatePo;

        internal async Task<string> Generate(System.Web.HttpRequestBase Request, DbConnectionSklad db)
        {
            #region Параметры

            pID = Request.Params["pID"];

            pLanguage = Convert.ToInt32(Request.Params["pLanguage"]);
            DateS = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DateS"]).ToString("yyyy-MM-dd 00:00:00"));
            DatePo = Convert.ToDateTime(Convert.ToDateTime(Request.Params["DatePo"]).ToString("yyyy-MM-dd 23:59:59"));
            
            DirContractorIDOrg = 0;
            bool bDirContractorIDOrg = Int32.TryParse(Request.Params["DirContractorIDOrg"], out DirContractorIDOrg);
            DirContractorNameOrg = Request.Params["DirContractorNameOrg"];
            
            DirEmployeeID = 0;
            bool bDirEmployeeID = Int32.TryParse(Request.Params["DirEmployeeID"], out DirEmployeeID);
            DirEmployeeName = Request.Params["DirEmployeeName"];

            DirMovementStatusID = 0;
            bool bDirMovementStatusID = Int32.TryParse(Request.Params["DirMovementStatusID"], out DirMovementStatusID);
            DirMovementStatusName = Request.Params["DirMovementStatusName"];

            DocOrTab = Convert.ToInt32(Request.Params["DocOrTab"]);

            #endregion

            string
                //DirNomenPatchFull = "",
                ret =
                "<center>" +
                "<h2>" + "Логистика" + " (" + DateS.ToString("yyyy-MM-dd") + " по " + DatePo.ToString("yyyy-MM-dd") + ")</h2>";


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


                #region DataReader

                ret += ReportHeaderDoc(pLanguage) + "</center>";
                double dQuantity = 0;

                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {
                    ret +=
                        //№
                        "<TD>" + query[i].DocMovementID + "</TD> " +
                        //Дата
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +
                        
                        //Точка от
                        "<TD>" + query[i].DirWarehouseNameFrom + "</TD> " +
                        //Курьер
                        "<TD>" + query[i].DirEmployeeNameCourier + "</TD> " +
                        //Точка на
                        "<TD>" + query[i].DirWarehouseNameTo + "</TD> " +

                        //Статус
                        "<TD>" + query[i].DirMovementStatusName + "</TD> " +
                        "</TR>";
                }
                
                ret +=
                    "</TABLE>" + 
                    "К-во документов: " + query.Count();

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

                        //Цена закупки
                        //Purch_PriceCurrency = docMovementTabs.PriceCurrency,
                        //Цена продажи
                        //Sale_PriceCurrency = docMovementTabs.PriceCurrency,
                        //К-во
                        Sale_Quantity = docMovementTabs.Quantity,
                        //Сумма
                        //Sums = docMovementTabs.Quantity * docMovementTabs.PriceCurrency - docMovementTabs.doc.Discount,
                        //Прибыль
                        //SumProfit = docMovementTabs.Quantity * (x.PriceCurrency - docMovementTabs.PriceCurrency) - docMovementTabs.doc.Discount,
                        //Скидка
                        //Sale_Discount = docMovementTabs.doc.Discount,
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


                #region DataReader

                ret += ReportHeaderTab(pLanguage) + "</center>";
                double dQuantity = 0;

                //Получение списка
                var query = await Task.Run(() => queryTemp.ToListAsync());

                //ДатаРидер
                for (int i = 0; i < query.Count(); i++)
                {
                    dQuantity += query[i].Sale_Quantity;
                    
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

                    ret +=
                    //К-во
                    "<TD>" + query[i].Sale_Quantity + "</TD> ";

                    ret +=
                        //Дата
                        "<TD>" + Convert.ToDateTime(query[i].DocDate).ToString("yyyy-MM-dd") + "</TD> " +

                        //Точка от
                        "<TD>" + query[i].DirWarehouseNameFrom + "</TD> " +
                        //Курьер
                        "<TD>" + query[i].DirEmployeeNameCourier + "</TD> " +
                        //Точка на
                        "<TD>" + query[i].DirWarehouseNameTo + "</TD> " +

                        //Статус
                        "<TD>" + query[i].DirMovementStatusName + "</TD> " +
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
                    //К-во
                    "<TD><b>" + dQuantity + "</b></TD> ";

                ret +=
                    //Продавец
                    "<TD> </TD> " +
                    //Дата
                    "<TD> </TD> " +
                    //Точка
                    "<TD> </TD> " +
                    "</TR>" +
                    "</TABLE>";

                #endregion

            }

            return ret;

        }


        //СТАНДАРТНАЯ табличная часть
        private string ReportHeaderDoc(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>№</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка от</TD> <TD class='table_header'>Курьер</TD> <TD class='table_header'>Точка на</TD> <TD class='table_header'>Статус</TD> " +
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>№</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка от</TD> <TD class='table_header'>Курьер</TD> <TD class='table_header'>Точка на</TD> <TD class='table_header'>Статус</TD> " +
                    "</TR>";
            }
        }

        private string ReportHeaderTab(int pLanguage)
        {
            switch (pLanguage)
            {
                case 1:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка от</TD> <TD class='table_header'>Курьер</TD> <TD class='table_header'>Точка на</TD> <TD class='table_header'>Статус</TD> " +
                    "</TR>";

                default:
                    return
                    //Classes.Language.Sklad.Language.msg103(pID, pLanguage, DateS, DatePo) +
                    "<TABLE class='table_1'>" +
                    "<TR>" +
                    "<TD class='table_header'>Код</TD> <TD class='table_header'>Категория</TD> <TD class='table_header'>Наименование</TD> <TD class='table_header'>Х-ки</TD> <TD class='table_header'>К-во</TD> <TD class='table_header'>Дата</TD> <TD class='table_header'>Точка от</TD> <TD class='table_header'>Курьер</TD> <TD class='table_header'>Точка на</TD>> <TD class='table_header'>Статус</TD> " +
                    "</TR>";
            }
        }

    }
}