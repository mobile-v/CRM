using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Login.Dir
{
    [Table("DirLanguage")]
    public class DirLanguage
    {
        [Key]
        public int DirLanguageID { get; set; }
        [Required]
        public string DirLanguageName { get; set; }
    }
}