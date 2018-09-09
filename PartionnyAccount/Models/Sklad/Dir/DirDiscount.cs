using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirDiscounts")]
    public class DirDiscount
    {
        [Key]
        public int? DirDiscountID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirDiscountName { get; set; }
        public string Description { get; set; }

        //DirDiscountTabs
        [NotMapped]
        public string recordsDirDiscountTabsGrid { get; set; }
    }

    [Table("DirDiscountTabs")]
    public class DirDiscountTab
    {
        [Key]
        public int? DirDiscountTabID { get; set; }

        [Display(Name = "Скидка")]
        [Required]
        public int DirDiscountID { get; set; }
        [ForeignKey("DirDiscountID")]
        public virtual Dir.DirDiscount dirDiscount { get; set; }

        [Required]
        public double SumBegin { get; set; }
        [Required]
        public double Discount { get; set; }
    }
}