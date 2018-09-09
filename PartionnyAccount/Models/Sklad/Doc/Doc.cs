using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    public class Doc
    {
        [Key]
        public int? DocID { get; set; }
        public bool Del { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int ListObjectID { get; set; }
        [ForeignKey("ListObjectID")]
        public virtual List.ListObject listObject { get; set; }

        public int? NumberReal { get; set; }
        public string NumberInt { get; set; }

        [Required]
        public DateTime? DocDate { get; set; }
        //[NotMapped]
        public DateTime? DocDateCreate { get; set; }
        [Required]
        public bool? Held { get; set; }
        public double Discount { get; set; }
        [Display(Name = "На основании какого документа создан данный")]
        public int? DocIDBase { get; set; }
        public string Base { get; set; }

        [Display(Name = "Организация")]
        [Required]
        public int DirContractorIDOrg { get; set; }
        [ForeignKey("DirContractorIDOrg")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirContractor dirContractorOrg { get; set; }

        [Display(Name = "Контрагент")]
        [Required]
        public int DirContractorID { get; set; }
        [ForeignKey("DirContractorID")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirContractor dirContractor { get; set; }
        
        [Display(Name = "Кто создал документ. Используется для начислении премии сотруднику")]
        [Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }

        public bool? IsImport { get; set; }
        public string Description { get; set; }

        [Display(Name = "Сумма оплаты")]
        [Required]
        public double Payment { get; set; }
        [Display(Name = "НДС")]
        [Required]
        public double DirVatValue { get; set; }

        //public bool? IsInv { get; set; }
        //public int? InvDocID { get; set; }


        
        [Display(Name = "Тип оплаты: Касса или Банк.")]
        [Required]
        public int? DirPaymentTypeID { get; set; }
        [ForeignKey("DirPaymentTypeID")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirPaymentType dirPaymentType { get; set; }

        //public int? DirPaymentTypeID { get; set; }


        #region KKMS

        [Display(Name = "Номер чека в ККМ")]
        public int? KKMSCheckNumber { get; set; }
        [Display(Name = "Уникалдьный номер комманды ККМS")]
        public string KKMSIdCommand { get; set; }

        //параметры клиента
        [Display(Name = "EMail")]
        public string KKMSEMail { get; set; }
        [Display(Name = "Phone")]
        public string KKMSPhone { get; set; }

        #endregion


        public DateTime? DocDateHeld { get; set; }
        public DateTime? DocDatePayment { get; set; }


        [Display(Name = "Для Заказов (Поступление на основании заказа)")] //Что бы не использовать "DocIDBase"
        public int? DirOrderIntTypeID { get; set; }

    }
}