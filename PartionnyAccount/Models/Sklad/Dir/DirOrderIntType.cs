using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirOrderIntTypes")]
    public class DirOrderIntType
    {
        [Key]
        public int? DirOrderIntTypeID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirOrderIntTypeName { get; set; }
    }
}