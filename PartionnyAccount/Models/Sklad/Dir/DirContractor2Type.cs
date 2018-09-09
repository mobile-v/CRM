using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirContractor2Types")]
    public class DirContractor2Type
    {
        [Key]
        public int? DirContractor2TypeID { get; set; }
        [Required]
        public string DirContractor2TypeName { get; set; }
    }
}