using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceNomenCategories")]
    public class DirServiceNomenCategory
    {
        [Key]
        public int? DirServiceNomenCategoryID { get; set; }
        public bool Del { get; set; }
        [Required]
        public string DirServiceNomenCategoryName { get; set; }
    }
}