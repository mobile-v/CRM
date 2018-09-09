using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    /*
    [Table("DirCurrencies")]
    public class DirCurrencySQL
    {
        [Key]
        public int? DirCurrencyID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }
        [Required]
        public int DirCurrencyCode { get; set; }
        [Required]
        public string DirCurrencyNameShort { get; set; }
        [Required]
        public string DirCurrencyName { get; set; }

        // *** История ***

        public int? DirCurrencyHistoryID { get; set; }
        public string HistoryDate { get; set; }
        public decimal DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }

        //public bool? ExistInHistory { get; set; }
    }
    */

    [Table("DirCurrencies")]
    public class DirCurrency
    {
        [Key]
        public int? DirCurrencyID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }

        [Required]
        public int DirCurrencyCode { get; set; }
        [Required]
        public string DirCurrencyNameShort { get; set; }
        [Required]
        public string DirCurrencyName { get; set; }

        [NotMapped]
        public DateTime? HistoryDate { get; set; }
        //[NotMapped]
        public double DirCurrencyRate { get; set; }
        //[NotMapped]
        public int DirCurrencyMultiplicity { get; set; }

        //public bool? ExistInHistory { get; set; }
    }

    [Table("DirCurrencyHistories")]
    public class DirCurrencyHistory
    {
        [Key]
        public int? DirCurrencyHistoryID { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int? DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        [Required]
        public DateTime HistoryDate { get; set; }
        [Required]
        public double DirCurrencyRate { get; set; }
        [Required]
        public int DirCurrencyMultiplicity { get; set; }
    }
}