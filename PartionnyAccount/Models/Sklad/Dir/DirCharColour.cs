using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirCharColours")]
    public class DirCharColour
    {
        [Key]
        public int? DirCharColourID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirCharColourName { get; set; }
    }
}