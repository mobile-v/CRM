using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirBonus2es")]
    public class DirBonus2
    {
        [Key]
        public int? DirBonus2ID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirBonus2Name { get; set; }
        public string Description { get; set; }

        //DirBonus2Tabs
        [NotMapped]
        public string recordsDirBonus2TabsGrid { get; set; }
    }

    [Table("DirBonus2Tabs")]
    public class DirBonus2Tab
    {
        [Key]
        public int? DirBonus2TabID { get; set; }

        [Display(Name = "Бонус")]
        [Required]
        public int DirBonus2ID { get; set; }
        [ForeignKey("DirBonus2ID")]
        public virtual Dir.DirBonus2 DirBonus2 { get; set; }

        [Required]
        public decimal SumBegin { get; set; }
        [Required]
        public decimal Bonus { get; set; }
    }
}