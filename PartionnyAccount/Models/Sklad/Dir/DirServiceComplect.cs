using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceComplects")]
    public class DirServiceComplect
    {
        [Key]
        public int? DirServiceComplectID { get; set; }
        [Required]
        public string DirServiceComplectName { get; set; }
    }
}