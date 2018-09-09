using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirOrderIntContractors")]
    public class DirOrderIntContractor
    {
        [Key]
        public int? DirOrderIntContractorID { get; set; }
        [Required]
        public string DirOrderIntContractorName { get; set; }
        public string NameLower { get; set; }

        public string DirOrderIntContractorAddress { get; set; }
        public string DirOrderIntContractorPhone { get; set; }
        public string DirOrderIntContractorFax { get; set; }
        public string DirOrderIntContractorEmail { get; set; }
        public string DirOrderIntContractorWWW { get; set; }

        public string DirOrderIntContractorDesc { get; set; }

        [Display(Name = "К-во успешных ремонтов")]
        public int? QuantityOk { get; set; }
        [Display(Name = "К-во НЕ успешных ремонтов")]
        public int? QuantityFail { get; set; }
        [Display(Name = "Общее к-во ремонтов")]
        public int? QuantityCount { get; set; }




        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {

        }
    }
}