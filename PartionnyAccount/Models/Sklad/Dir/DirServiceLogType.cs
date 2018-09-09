using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceLogTypes")]
    public class DirServiceLogType
    {
        [Key]
        public int? DirServiceLogTypeID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirServiceLogTypeName { get; set; }
    }
}