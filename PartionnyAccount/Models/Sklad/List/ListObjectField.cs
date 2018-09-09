using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.List
{
    [Table("ListObjectFields")]
    public class ListObjectField
    {
        [Key]
        public int ListObjectFieldID { get; set; }

        [Display(Name = "Объект")]
        [Required]
        public int ListObjectID { get; set; }
        [ForeignKey("ListObjectID")]
        public virtual List.ListObject listObject { get; set; }

        public bool ListObjectFieldHeaderShow { get; set; }
        public bool ListObjectFieldTabShow { get; set; }
        public bool ListObjectFieldFooterShow { get; set; }

        [Display(Name = "Наименование поля")]
        [Required]
        public int ListObjectFieldNameID { get; set; }
        [ForeignKey("ListObjectFieldNameID")]
        public virtual List.ListObjectFieldName listObjectFieldName { get; set; }
    }
}