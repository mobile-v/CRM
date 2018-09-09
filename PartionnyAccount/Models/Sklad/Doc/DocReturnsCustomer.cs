using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    [Table("DocReturnsCustomers")]
    public class DocReturnsCustomer
    {
        [Key]
        public int? DocReturnsCustomerID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Партии. А это то что вернули")]
        [Required]
        public int DocSaleID { get; set; }
        [ForeignKey("DocSaleID")]
        public virtual Models.Sklad.Doc.DocSale docSale { get; set; }

        public string NumberTT { get; set; }
        public string NumberTax { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        public bool Reserve { get; set; }



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
        public string recordsDocReturnsCustomerTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 36;
        }
    }

    [Table("DocReturnsCustomerTabs")]
    public class DocReturnsCustomerTab
    {
        [Key]
        public int? DocReturnsCustomerTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocReturnsCustomerID { get; set; }
        [ForeignKey("DocReturnsCustomerID")]
        public virtual DocReturnsCustomer docReturnsCustomer { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }

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



        [Display(Name = "Партии списание. Тоесть возвращаем списанный товар на склад из ПартииМинус")]
        [Required]
        public int RemPartyMinusID { get; set; }
        [ForeignKey("RemPartyMinusID")]
        public virtual Rem.RemPartyMinus remPartyMinus { get; set; }
    }
}