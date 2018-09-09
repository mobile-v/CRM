using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Dir
{
    [Table("DirLoginNot")]
    public class DirLoginNot
    {
        [Key]
        public int DirLoginNotID { get; set; }
        [Required]
        public string Login { get; set; }
    }
}