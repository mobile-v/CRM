using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirSecondHandLogTypes")]
    public class DirSecondHandLogType
    {
        [Key]
        public int? DirSecondHandLogTypeID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirSecondHandLogTypeName { get; set; }
    }
}