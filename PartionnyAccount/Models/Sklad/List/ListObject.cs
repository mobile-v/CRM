using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.List
{
    [Table("ListObjects")]
    public class ListObject
    {
        [Key]
        public int ListObjectID { get; set; }

        [Display(Name = "Тип")]
        [Required]
        public int ListObjectTypeID { get; set; }
        [ForeignKey("ListObjectTypeID")]
        public virtual List.ListObjectType listObjectType { get; set; }

        [Required]
        public string ListObjectNameSys { get; set; }
        [Required]
        public string ListObjectDisc { get; set; }
        [Required]
        public string ListObjectNameRu { get; set; }
        //[Required]
        //public string ListObjectNameUa { get; set; }
        //[Required]
        //public string ListObjectNameBy { get; set; }
        [Required]
        public string ListObjectNameUs { get; set; }
        //[Required]
        //public string ListObjectNameAz { get; set; }
    }

    [Table("ListObjectTypes")]
    public class ListObjectType
    {
        [Key]
        public int ListObjectTypeID { get; set; }
        [Required]
        public string ListObjectTypeName { get; set; }
    }
}