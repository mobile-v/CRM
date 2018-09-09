using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirOrderIntStatuses")]
    public class DirOrderIntStatus
    {
        [Key]
        public int? DirOrderIntStatusID { get; set; }
        [Required]
        public string DirOrderIntStatusName { get; set; }
    }
}