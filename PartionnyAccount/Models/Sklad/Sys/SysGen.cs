using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Sys
{
    [Table("SysGens")]
    public class SysGen
    {
        [Key]
        public int? SysGenID { get; set; }
        public bool SysGenTemp { get; set; }
        public string SysGenDisc { get; set; }
    }
}