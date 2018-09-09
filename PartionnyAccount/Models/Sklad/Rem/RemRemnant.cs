using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Rem
{
    [Table("RemRemnants")]
    public class RemRemnant
    {
        [Key]
        public int? RemRemnantID { get; set; }

        [Display(Name = "Организация")]
        [Required]
        public int DirContractorIDOrg { get; set; }
        [ForeignKey("DirContractorIDOrg")]
        public virtual Dir.DirContractor dirContractorOrg { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Display(Name = "Кто создал документ. Используется для начислении премии сотруднику")]
        [Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }

        [Required]
        public DateTime? DocDate { get; set; }

    }
}