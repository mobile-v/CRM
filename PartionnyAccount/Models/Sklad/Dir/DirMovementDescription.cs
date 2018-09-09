using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirMovementDescriptions")]
    public class DirMovementDescription
    {
        [Key]
        public int? DirMovementDescriptionID { get; set; }
        [Required]
        public string DirMovementDescriptionName { get; set; }
    }
}