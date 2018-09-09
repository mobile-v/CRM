using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirBanks")]
    public class DirBank
    {
        [Key]
        public int? DirBankID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }
        [Required]
        public string DirBankName { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        //Дополнительные рквизиты
        public string DirBankMFO { get; set; }
        public string DirBankCurrAccount { get; set; }
        public string DirBankCorrAccount { get; set; }
        public string DirBankTIN { get; set; }
        public string DirBankSWIFT { get; set; }
        public string DirBankBIC { get; set; }
        public string DirBankPhone { get; set; }
        public string DirBankFax { get; set; }
        public string DirBankEMail { get; set; }
        public string Description { get; set; }
        
        [Display(Name = "Сумма денег в банке, что бы каждый раз не подсчитывать - вычисляется Триггером")]
        public double DirBankSum { get; set; }
    }
}