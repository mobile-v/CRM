using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.List
{
    [Table("ListObjectFieldNames")]
    public class ListObjectFieldName
    {
        [Key]
        public int ListObjectFieldNameID { get; set; }
        public bool Del { get; set; }
        public string ListObjectFieldNameReal { get; set; }
        public string ListObjectFieldNameRu { get; set; }
        public string ListObjectFieldNameUa { get; set; }
        public string ListObjectFieldNameUs { get; set; }
    }
}