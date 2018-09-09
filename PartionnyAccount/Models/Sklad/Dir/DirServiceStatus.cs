using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceStatuses")]
    public class DirServiceStatus
    {
        [Key]
        public int? DirServiceStatusID { get; set; }
        [Required]
        public string DirServiceStatusName { get; set; }
    }
}