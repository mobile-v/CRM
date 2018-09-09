using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Sys
{
    [Table("SysAdmins")]
    public class SysAdmin
    {
        [Key]
        public int AdminsID { get; set; }
        [Required]
        public string AdminsLogin { get; set; }
        [Required]
        public string AdminsPswd { get; set; }
        [Required]
        public string AdminsName { get; set; }
        [Required]
        public bool AdminsActive { get; set; }
    }
}