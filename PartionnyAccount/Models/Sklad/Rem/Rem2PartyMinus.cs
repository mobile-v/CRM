using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Rem
{
    [Table("Rem2PartyMinuses")]
    public class Rem2PartyMinus
    {
        [Key]
        public int? Rem2PartyMinusID { get; set; }

        [Display(Name = "Партии на складе")]
        [Required]
        public int? Rem2PartyID { get; set; }
        [ForeignKey("Rem2PartyID")]
        public virtual Rem.Rem2Party rem2Party { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Doc.Doc doc { get; set; }

        [Display(Name = "Организация")]
        [Required]
        public int DirContractorIDOrg { get; set; }
        [ForeignKey("DirContractorIDOrg")]
        public virtual Dir.DirContractor dirContractorOrg { get; set; }

        [Display(Name = "Поставщика от которого пришла партия первоначально - этот параметр передаётся во все другие партии (напр. перемещение)")]
        [Required]
        public int? DirServiceContractorID { get; set; }
        [ForeignKey("DirServiceContractorID")]
        public virtual Dir.DirServiceContractor dirServiceContractor { get; set; }

        [Display(Name = "Склад на который пришла партия")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

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


        [Display(Name = "Кто создал документ. Используется для начислении премии сотруднику")]
        [Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }

        [Required]
        public DateTime? DocDate { get; set; }

    }

    [Table("Rem2PartyMinuses")]
    public class Rem2PartyMinusSQL
    {
        public DateTime? DateX { get; set; }

        public int? CountX { get; set; }
    }
}