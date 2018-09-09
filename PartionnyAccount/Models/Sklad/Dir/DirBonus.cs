using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirBonuses")]
    public class DirBonus
    {
        [Key]
        public int? DirBonusID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirBonusName { get; set; }
        public string Description { get; set; }

        //DirBonusTabs
        [NotMapped]
        public string recordsDirBonusTabsGrid { get; set; }
    }

    [Table("DirBonusTabs")]
    public class DirBonusTab
    {
        [Key]
        public int? DirBonusTabID { get; set; }

        [Display(Name = "Бонус")]
        [Required]
        public int DirBonusID { get; set; }
        [ForeignKey("DirBonusID")]
        public virtual Dir.DirBonus dirBonus { get; set; }

        [Required]
        public double SumBegin { get; set; }
        [Required]
        public double Bonus { get; set; }
    }
}