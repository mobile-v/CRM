using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Jurn
{
    [Table("JurnDispError")]
    public class JurnDispError
    {
        [Key]
        public int JurnDispErrorID { get; set; }
        [Required]
        public int DirCustomersID { get; set; }
        [Required]
        public string JurnDispErrorText { get; set; }
    }
}