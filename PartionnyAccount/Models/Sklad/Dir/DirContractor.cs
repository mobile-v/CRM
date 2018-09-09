using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirContractors")]
    public class DirContractor
    {
        [Key]
        public int? DirContractorID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }

        [Display(Name = "Тип контрагента")]
        [Required]
        public int DirContractor1TypeID { get; set; }
        [ForeignKey("DirContractor1TypeID")]
        public virtual Dir.DirContractor1Type dirContractor1Type { get; set; }

        [Display(Name = "Тип контрагента")]
        [Required]
        public int DirContractor2TypeID { get; set; }
        [ForeignKey("DirContractor2TypeID")]
        public virtual Dir.DirContractor2Type dirContractor2Type { get; set; }

        [Required]
        public string DirContractorName { get; set; }
        public string NameLower { get; set; }

        public string DirContractorAddress { get; set; }
        public string DirContractorPhone { get; set; }
        public string DirContractorFax { get; set; }
        public string DirContractorEmail { get; set; }
        public string DirContractorWWW { get; set; }

        public decimal DirContractorDiscount { get; set; }

        [Display(Name = "Скидка в % с градацией")]
        public int? DirDiscountID { get; set; }

        [Display(Name = "Банк")]
        public int? DirBankID { get; set; }

        public string DirContractorDesc { get; set; }
        public string ImageLink { get; set; }
        [Display(Name = "Основной Счёт Организации")]
        public string DirBankAccountName { get; set; }
        public double SalesSum { get; set; }


        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            if (DirDiscountID > 0) DirContractorDiscount = 0;
        }
    }
}