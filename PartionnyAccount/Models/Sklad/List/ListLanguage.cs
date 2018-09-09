using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.List
{
    [Table("ListLanguages")]
    public class ListLanguage
    {
        [Key]
        public int ListLanguageID { get; set; }
        public bool Del { get; set; }
        public string ListLanguageNameSmall { get; set; }
        public string ListLanguageName { get; set; }
        public string ListLanguageDisc { get; set; }
    }
}