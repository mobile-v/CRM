using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirContractor1Types")]
    public class DirContractor1Type
    {
        [Key]
        public int? DirContractor1TypeID { get; set; }
        [Required]
        public string DirContractor1TypeName { get; set; }
    }
}