using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceDiagnosticRresults")]
    public class DirServiceDiagnosticRresult
    {
        [Key]
        public int? DirServiceDiagnosticRresultID { get; set; }
        [Required]
        public string DirServiceDiagnosticRresultName { get; set; }
    }
}