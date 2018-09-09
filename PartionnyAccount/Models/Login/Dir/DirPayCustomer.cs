using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Dir
{
    [Table("DirPayCustomers")]
    public class DirPayCustomer
    {
        [Key]
        public int DirPayCustomersID { get; set; }

        [Display(Name = "Страна")]
        [Required]
        public int DirCustomersID { get; set; }
        [ForeignKey("DirCustomersID")]
        public virtual Dir.DirCustomer dirCustomer { get; set; }

        [Required]
        public int? PayCount { get; set; }
        [Required]
        public DateTime PayDateBegin { get; set; }
        [Required]
        public int PayMonths { get; set; }
        [Required]
        public int PayDayBonus { get; set; }
        [Required]
        public DateTime PayDateEnd { get; set; }
        [Required]
        public decimal PayPrice { get; set; }
        [Required]
        public decimal PaySum { get; set; }
        [Required]
        public decimal PayRemnant { get; set; }
        [Required]
        public decimal PayDiscount { get; set; }
        [Required]
        public int DirCurrencyID { get; set; }
        [Required]
        public string PayCurrencyName { get; set; }
        [Required]
        public decimal PayCurrencyRate { get; set; }
        [Required]
        public decimal PayCommission { get; set; }

        [Display(Name = "Тип оплаты")]
        [Required]
        public int DirPayServiceID { get; set; }
        [ForeignKey("DirPayServiceID")]
        public virtual Dir.DirPayService dirPayService { get; set; }

        [Display(Name = "Банк")]
        [Required]
        public int DirPayID { get; set; }
        [ForeignKey("DirPayID")]
        public virtual Dir.DirPay dirPay { get; set; }

        [Required]
        public string Disc { get; set; }
        [Required]
        public bool ReferPay { get; set; }
        [Required]
        public bool SendMsgEndPeriod { get; set; }
        [Required]
        public bool Held { get; set; }

    }
}