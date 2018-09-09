using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirPaymentTypes")]
    public class DirPaymentType
    {
        [Key]
        public int? DirPaymentTypeID { get; set; }
        [Required]
        public string DirPaymentTypeName { get; set; }
    }
}