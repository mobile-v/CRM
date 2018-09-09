using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirCharSizes")]
    public class DirCharSize
    {
        [Key]
        public int? DirCharSizeID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirCharSizeName { get; set; }
    }
}