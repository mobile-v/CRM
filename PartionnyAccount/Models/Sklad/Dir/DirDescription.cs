using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirDescriptions")]
    public class DirDescription
    {
        [Key]
        public int? DirDescriptionID { get; set; }
        [Required]
        public string DirDescriptionName { get; set; }
    }
}