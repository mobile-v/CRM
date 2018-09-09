using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirOrdersStates")]
    public class DirOrdersState
    {
        [Key]
        public int? DirOrdersStateID { get; set; }
        [Required]
        public string DirOrdersStateName { get; set; }
        [Display(Name = "1 - Customer Doc, 2 - Customer Nomen, 3 - Purch Doc, 4 - Purch Nomen")]
        [Required]
        public int CustomerPurch { get; set; }
        public string DirOrdersStateDesc { get; set; }
    }
}