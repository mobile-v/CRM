using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    [Table("DocBankSums")]
    public class DocBankSum
    {
        [Key]
        public int? DocBankSumID { get; set; }

        [Display(Name = "Банк")]
        //[Required]
        public int DirBankID { get; set; }
        [ForeignKey("DirBankID")]
        public virtual Dir.DirBank dirBank { get; set; }

        [Display(Name = "Сотрудник, который создал запись")]
        //[Required]
        public int? DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual Dir.DirEmployee dirEmployee { get; set; }

        [Display(Name = "Тип операции")]
        //[Required]
        public int DirBankSumTypeID { get; set; }
        [ForeignKey("DirBankSumTypeID")]
        public virtual Dir.DirBankSumType dirBankSumType { get; set; }

        [Display(Name = "Дата и время операции")]
        //[Required]
        public DateTime? DocBankSumDate { get; set; }

        [Display(Name = "Дата и время операции")]
        //[Required]
        public DateTime? DateOnly { get; set; }

        [Display(Name = "Документ - на основании кого документа создана запись")]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Doc doc { get; set; }

        [Display(Name = "Документ - ID-шник документа: DocPurches, DocSales, ...")]
        public int? DocXID { get; set; }

        [Display(Name = "Сумма документа")]
        public double DocBankSumSum { get; set; }

        [Display(Name = "Валюта")]
        //[Required]
        public int? DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double? DirCurrencyRate { get; set; }
        public int? DirCurrencyMultiplicity { get; set; }

        public string Base { get; set; }
        public string Description { get; set; }

        public double? Discount { get; set; }


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

    }
}