using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceProblems")]
    public class DirServiceProblem
    {
        [Key]
        public int? DirServiceProblemID { get; set; }
        [Required]
        public string DirServiceProblemName { get; set; }
    }
}