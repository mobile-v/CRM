using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirNomenCategories")]
    public class DirNomenCategory
    {
        [Key]
        public int? DirNomenCategoryID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirNomenCategoryName { get; set; }
    }
}