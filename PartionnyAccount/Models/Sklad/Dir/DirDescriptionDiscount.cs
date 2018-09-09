using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirDescriptionDiscounts")]
    public class DirDescriptionDiscount
    {
        [Key]
        public int? DirDescriptionDiscountID { get; set; }
        [Required]
        public string DirDescriptionDiscountName { get; set; }
    }
}