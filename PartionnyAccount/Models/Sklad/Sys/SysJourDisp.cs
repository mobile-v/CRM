using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Sys
{
    [Table("SysJourDisps")]
    public class SysJourDisp
    {
        [Key]
        public int SysJourDispID { get; set; }
        
        [Display(Name = "Сотрудник")]
        [Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual Dir.DirEmployee dirEmployee { get; set; }

        [Display(Name = "Операция")]
        [Required]
        public int DirDispOperationID { get; set; }
        [ForeignKey("DirDispOperationID")]
        public virtual Dir.DirDispOperation dirDispOperation { get; set; }

        [Display(Name = "Объект")]
        [Required]
        public int? ListObjectID { get; set; }
        [ForeignKey("ListObjectID")]
        public virtual List.ListObject listObject { get; set; }


        //[Required]
        //public string TableName { get; set; }
        public int? TableFieldID { get; set; }
        [Required]
        public DateTime SysJourDispDateTime { get; set; }
        //[Required]
        public string Description { get; set; }
    }
}