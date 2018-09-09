using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Dir
{
    [Table("DirPay")]
    public class DirPay
    {
        [Key]
        public int DirPayID { get; set; }
        [Required]
        public string DirPayName { get; set; }
    }
}