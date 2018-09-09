using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    [Table("DocPurches")]
    public class DocPurch
    {
        [Key]
        public int? DocPurchID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        public string NumberTT { get; set; }
        public string NumberTax { get; set; }



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
        public string recordsDocPurchTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 6;
        }
    }

    [Table("DocPurchTabs")]
    public class DocPurchTab
    {
        [Key]
        public int? DocPurchTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocPurchID { get; set; }
        [ForeignKey("DocPurchID")]
        public virtual DocPurch docPurch { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }


        //Характеристики *** *** *** *** *** *** *** ***
        [Display(Name = "Цвет")]
        public int? DirCharColourID { get; set; }
        [ForeignKey("DirCharColourID")]
        public virtual Dir.DirCharColour dirCharColour { get; set; }

        [Display(Name = "Материал")]
        public int? DirCharMaterialID { get; set; }
        [ForeignKey("DirCharMaterialID")]
        public virtual Dir.DirCharMaterial dirCharMaterial { get; set; }

        [Display(Name = "Наименование")]
        public int? DirCharNameID { get; set; }
        [ForeignKey("DirCharNameID")]
        public virtual Dir.DirCharName dirCharName { get; set; }

        [Display(Name = "Сезон")]
        public int? DirCharSeasonID { get; set; }
        [ForeignKey("DirCharSeasonID")]
        public virtual Dir.DirCharSeason dirCharSeason { get; set; }

        [Display(Name = "Пол")]
        public int? DirCharSexID { get; set; }
        [ForeignKey("DirCharSexID")]
        public virtual Dir.DirCharSex dirCharSex { get; set; }

        [Display(Name = "Размер")]
        public int? DirCharSizeID { get; set; }
        [ForeignKey("DirCharSizeID")]
        public virtual Dir.DirCharSize dirCharSize { get; set; }

        //Стиль и Поставщик
        [Display(Name = "Стиль и Поставщик")]
        public int? DirCharStyleID { get; set; }
        [ForeignKey("DirCharStyleID")]
        public virtual Dir.DirCharStyle dirCharStyle { get; set; }

        //Поставщик
        [Display(Name = "Поставщик")]
        public int? DirContractorID { get; set; }
        [ForeignKey("DirContractorID")]
        public virtual Dir.DirContractor dirContractor { get; set; }

        [Display(Name = "Текстура")]
        public int? DirCharTextureID { get; set; }
        [ForeignKey("DirCharTextureID")]
        public virtual Dir.DirCharTexture dirCharTexture { get; set; }
        //Характеристики *** *** *** *** *** *** *** ***



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

        [Display(Name = "Минимальный остаток")]
        [Required]
        public double DirNomenMinimumBalance { get; set; }
    }
}