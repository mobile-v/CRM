using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirPriceTypes")]
    public class DirPriceType
    {
        [Key]
        public int? DirPriceTypeID { get; set; }
        [Required]
        public string DirPriceTypeName { get; set; }
    }
}