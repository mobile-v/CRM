using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirSecondHandStatuses")]
    public class DirSecondHandStatus
    {
        [Key]
        public int? DirSecondHandStatusID { get; set; }
        [Required]
        public string DirSecondHandStatusName { get; set; }
    }
}