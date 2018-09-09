using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirCashOfficeSumTypes")]
    public class DirCashOfficeSumType
    {
        [Key]
        public int? DirCashOfficeSumTypeID { get; set; }
        [Required]
        public string DirCashOfficeSumTypeName { get; set; }
        [Required]
        public int Sign { get; set; }
    }
}