using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirCharNames")]
    public class DirCharName
    {
        [Key]
        public int? DirCharNameID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirCharNameName { get; set; }
    }
}