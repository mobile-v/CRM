using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirDispOperation")]
    public class DirDispOperation
    {
        [Key]
        public int? DirDispOperationID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirDispOperationName { get; set; }
        [Required]
        public string DirDispOperationDesc { get; set; }
    }
}