using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    [Table("DocNomenRevaluations")]
    public class DocNomenRevaluation
    {
        [Key]
        public int? DocNomenRevaluationID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int? DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }




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
        public string recordsDocNomenRevaluationTab { get; set; }



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

    [Table("DocNomenRevaluationTabs")]
    public class DocNomenRevaluationTab
    {
        [Key]
        public int? DocNomenRevaluationTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocNomenRevaluationID { get; set; }
        [ForeignKey("DocNomenRevaluationID")]
        public virtual DocNomenRevaluation docNomenRevaluation { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }

        [Display(Name = "Партии")]
        [Required]
        public int RemPartyID { get; set; }
        [ForeignKey("RemPartyID")]
        public virtual Rem.RemParty remParty { get; set; }


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
        public double PriceRetailVAT_OLD { get; set; }
        [Required]
        public double PriceRetailVAT { get; set; }
        [Required]
        public double PriceRetailCurrency_OLD { get; set; }
        [Required]
        public double PriceRetailCurrency { get; set; }

        [Required]
        public double PriceWholesaleVAT_OLD { get; set; }
        [Required]
        public double PriceWholesaleVAT { get; set; }
        [Required]
        public double PriceWholesaleCurrency_OLD { get; set; }
        [Required]
        public double PriceWholesaleCurrency { get; set; }

        [Required]
        public double PriceIMVAT_OLD { get; set; }
        [Required]
        public double PriceIMVAT { get; set; }
        [Required]
        public double PriceIMCurrency_OLD { get; set; }
        [Required]
        public double PriceIMCurrency { get; set; }

    }
}