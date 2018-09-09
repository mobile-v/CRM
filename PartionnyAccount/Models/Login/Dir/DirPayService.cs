using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Dir
{
    [Table("DirPayService")]
    public class DirPayService
    {
        [Key]
        public int DirPayServiceID { get; set; }
        [Required]
        public string DirPayServiceName { get; set; }
    }
}