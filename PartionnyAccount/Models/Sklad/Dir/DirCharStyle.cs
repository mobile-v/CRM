using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirCharStyles")]
    public class DirCharStyle
    {
        [Key]
        public int? DirCharStyleID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirCharStyleName { get; set; }
    }
}