using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirNomenTypes")]
    public class DirNomenType
    {
        [Key]
        public int? DirNomenTypeID { get; set; }
        [Required]
        public string DirNomenTypeName { get; set; }
    }
}