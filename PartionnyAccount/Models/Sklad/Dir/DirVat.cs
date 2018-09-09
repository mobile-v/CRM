using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirVats")]
    public class DirVat
    {
        [Key]
        public int? DirVatID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }
        [Required]
        public double DirVatValue { get; set; }
    }
}