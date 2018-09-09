using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Pay
{
    [Table("DirPay")]
    public class Pay
    {
        [Key]
        public int? DocID { get; set; }
        public int? DocXID { get; set; }
        public int? DocCashBankID { get; set; }

        public int? DirPaymentTypeID { get; set; }
        public string DirXName { get; set; }

        public int? DirEmployeeID { get; set; }
        public int? DirXSumTypeID { get; set; }
        public DateTime? DocXSumDate { get; set; }
        public double? DocXSumSum { get; set; }
        public int? DirCurrencyID { get; set; }
        public double? DirCurrencyRate { get; set; }
        public int? DirCurrencyMultiplicity { get; set; }
        public string Description { get; set; }
        public string Base { get; set; }

        [NotMapped]
        public int? DirCashOfficeID { get; set; }
        [NotMapped]
        public int? DirBankID { get; set; }

        public double Discount { get; set; }


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