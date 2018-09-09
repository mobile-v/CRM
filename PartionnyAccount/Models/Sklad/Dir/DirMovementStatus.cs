using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirMovementStatuses")]
    public class DirMovementStatus
    {
        [Key]
        public int? DirMovementStatusID { get; set; }
        [Required]
        public string DirMovementStatusName { get; set; }
    }
}