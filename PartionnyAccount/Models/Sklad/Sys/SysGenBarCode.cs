using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Sys
{
    [Table("SysGenBarCodes")]
    public class SysGenBarCode
    {
        [Key]
        public int? SysGenBarCodeID { get; set; }
        public string SysGenBarCodeTemp { get; set; }
    }
}