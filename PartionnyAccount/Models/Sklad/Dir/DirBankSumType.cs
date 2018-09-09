using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirBankSumTypes")]
    public class DirBankSumType
    {
        [Key]
        public int? DirBankSumTypeID { get; set; }
        [Required]
        public string DirBankSumTypeName { get; set; }
        [Required]
        public int Sign { get; set; }
    }
}