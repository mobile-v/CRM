using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceContractors")]
    public class DirServiceContractor
    {
        [Key]
        public int? DirServiceContractorID { get; set; }
        [Required]
        public string DirServiceContractorName { get; set; }
        public string NameLower { get; set; }

        public string DirServiceContractorAddress { get; set; }
        public string DirServiceContractorPhone { get; set; }
        public string DirServiceContractorFax { get; set; }
        public string DirServiceContractorEmail { get; set; }
        public string DirServiceContractorWWW { get; set; }

        public string DirServiceContractorDesc { get; set; }

        [Display(Name = "К-во успешных ремонтов")]
        public int? QuantityOk { get; set; }
        [Display(Name = "К-во НЕ успешных ремонтов")]
        public int? QuantityFail { get; set; }
        [Display(Name = "Общее к-во ремонтов")]
        public int? QuantityCount { get; set; }

        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }


        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            
        }
    }
}