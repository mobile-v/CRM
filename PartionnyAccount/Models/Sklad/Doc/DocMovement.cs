using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    [Table("DocMovements")]
    public class DocMovement
    {
        [Key]
        public int? DocMovementID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }
        
        [Display(Name = "Склад с")]
        [Required]
        public int DirWarehouseIDFrom { get; set; }
        [ForeignKey("DirWarehouseIDFrom")]
        public virtual Dir.DirWarehouse dirWarehouseFrom { get; set; }

        [Display(Name = "Склад на")]
        [Required]
        public int DirWarehouseIDTo { get; set; }
        [ForeignKey("DirWarehouseIDTo")]
        public virtual Dir.DirWarehouse dirWarehouseTo { get; set; }

        public bool Reserve { get; set; }

        [Display(Name = "Комментарий: причина ")]
        public int? DirMovementDescriptionID { get; set; }
        [ForeignKey("DirMovementDescriptionID")]
        public virtual Dir.DirMovementDescription dirMovementDescription { get; set; }

        [NotMapped]
        public string DescriptionMovement { get; set; }

        [Display(Name = "Курьер")]
        public int? DirEmployeeIDCourier { get; set; }
        [ForeignKey("DirEmployeeIDCourier")]
        public virtual Dir.DirEmployee dirEmployee_Courier { get; set; }


        //1 Перемещение
        //2	Логистика: в ожидании курьера
        //3	Логистика: курьер принял
        //4	Логистика: курьер отдал
        [Display(Name = "Статус Логистики")]
        //[Required]
        public int? DirMovementStatusID { get; set; }
        [ForeignKey("DirMovementStatusID")]
        public virtual Dir.DirMovementStatus dirMovementStatus { get; set; }



        #region  Doc

        [NotMapped]
        public bool? Del { get; set; }
        [NotMapped]
        public int ListObjectID { get; set; }
        [NotMapped]
        public string NumberInt { get; set; }
        [NotMapped]
        public DateTime? DocDate { get; set; }
        [NotMapped]
        public DateTime? DocDateCreate { get; set; }
        [NotMapped]
        public bool? Held { get; set; }
        [NotMapped]
        public double Discount { get; set; }
        [NotMapped]
        [Display(Name = "На основании какого документа создан данный")]
        public int? DocIDBase { get; set; }
        [NotMapped]
        public string Base { get; set; }
        [Display(Name = "Организация")]
        [NotMapped]
        public int DirContractorIDOrg { get; set; }
        [Display(Name = "Контрагент")]
        [NotMapped]
        public int DirContractorID { get; set; }
        [Display(Name = "Кто создал документ. Используется для начислении премии сотруднику")]
        [NotMapped]
        public int DirEmployeeID { get; set; }
        [NotMapped]
        public bool? IsImport { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [Display(Name = "Тип оплаты: Касса или Банк.")]
        [NotMapped]
        public int DirPaymentTypeID { get; set; }
        [Display(Name = "Сумма оплаты")]
        [NotMapped]
        public double Payment { get; set; }
        [Display(Name = "НДС")]
        [NotMapped]
        public double DirVatValue { get; set; }

        #region KKMS

        [Display(Name = "Номер чека в ККМ")]
        [NotMapped]
        public int? KKMSCheckNumber { get; set; }
        [Display(Name = "Уникалдьный номер комманды ККМS")]
        [NotMapped]
        public string KKMSIdCommand { get; set; }

        //параметры клиента
        [Display(Name = "EMail")]
        [NotMapped]
        public string KKMSEMail { get; set; }
        [Display(Name = "Phone")]
        [NotMapped]
        public string KKMSPhone { get; set; }

        #endregion

        [NotMapped]
        public DateTime? DocDateHeld { get; set; }
        [NotMapped]
        public DateTime? DocDatePayment { get; set; }

        #endregion



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocMovementTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 33;
        }
    }

    [Table("DocMovementTabs")]
    public class DocMovementTab
    {
        [Key]
        public int? DocMovementTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocMovementID { get; set; }
        [ForeignKey("DocMovementID")]
        public virtual DocMovement docMovement { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }

        [Display(Name = "Партия -  с какой партии списываем товар")]
        [Required]
        public int RemPartyID { get; set; }
        [ForeignKey("RemPartyID")]
        public virtual Rem.RemParty remParty { get; set; }

        public int? DirCharColourID { get; set; }
        public int? DirCharMaterialID { get; set; }
        public int? DirCharNameID { get; set; }
        public int? DirCharSeasonID { get; set; }
        public int? DirCharSexID { get; set; }
        public int? DirCharSizeID { get; set; }
        public int? DirCharStyleID { get; set; }
        public int? DirCharTextureID { get; set; }
        public string SerialNumber { get; set; }
        public string Barcode { get; set; }

        [Required]
        public double Quantity { get; set; }
        [Display(Name = "Приходная цена")]
        [Required]
        public double PriceVAT { get; set; }
        [Required]
        public double PriceCurrency { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }

        [Required]
        public double PriceRetailVAT { get; set; }
        [NotMapped]
        public double MarkupRetail { get; set; }
        [Required]
        public double PriceRetailCurrency { get; set; }

        [Required]
        public double PriceWholesaleVAT { get; set; }
        [NotMapped]
        public double MarkupWholesale { get; set; }
        [Required]
        public double PriceWholesaleCurrency { get; set; }

        [Required]
        public double PriceIMVAT { get; set; }
        [NotMapped]
        public double MarkupIM { get; set; }
        [Required]
        public double PriceIMCurrency { get; set; }




        [Display(Name = "Тип возврата")]
        public int? DirReturnTypeID { get; set; }
        [ForeignKey("DirReturnTypeID")]
        public virtual Dir.DirReturnType dirReturnType { get; set; }

        [Display(Name = "Комментарий: причина возврата")]
        public int? DirDescriptionID { get; set; }
        [ForeignKey("DirDescriptionID")]
        public virtual Dir.DirDescription dirDescription { get; set; }



        //Для RemParties *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        [Display(Name = "Склад на который пришла партия первоначально")]
        [NotMapped]
        public int DirWarehouseIDPurch { get; set; }

        [Display(Name = "Поставщика от которого пришла партия первоначально")]
        [NotMapped]
        public int DirContractorID { get; set; }
    }
}