using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Rem
{
    [Table("RemPartySales")]
    public class RemPartySale
    {
        [Key]
        public int? RemPartySaleID { get; set; }

        [Display(Name = "Партии на складе")]
        [Required]
        public int RemPartyID { get; set; }
        [ForeignKey("RemPartyID")]
        public virtual Rem.RemParty remParty { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Doc.Doc doc { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }

        [Display(Name = "Склад на который пришла партия")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Display(Name = "Поставщика от которого пришла партия первоначально - этот параметр передаётся во все другие партии (напр. перемещение)")]
        [Required]
        public int DirContractorID { get; set; }
        [ForeignKey("DirContractorID")]
        public virtual Dir.DirContractor dirContractor { get; set; }

        [Display(Name = "Списали с партии")]
        [Required]
        public double Quantity { get; set; }

        [Required]
        public double PriceVAT { get; set; }
        [Required]
        public double DirVatValue { get; set; }
        [Required]
        public double PriceCurrency { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }

        public bool? Reserve { get; set; }
        public int FieldID { get; set; }

    }
}