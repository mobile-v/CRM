using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Jurn
{
    [Table("JurnDispLogining")]
    public class JurnDispLogining
    {
        [Key]
        public int JurnDispLoginingID { get; set; }
        [Required]
        public DateTime JurnDispLoginingDate { get; set; }
        [Required]
        public int DirCustomersID { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Pswd { get; set; }
        [Required]
        public string JurnDispLoginingDesc { get; set; }
    }
}