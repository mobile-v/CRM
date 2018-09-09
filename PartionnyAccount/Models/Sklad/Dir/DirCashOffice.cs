using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirCashOffices")]
    public class DirCashOffice
    {
        [Key]
        public int? DirCashOfficeID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }
        [Required]
        public string DirCashOfficeName { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public string Description { get; set; }
        [Required]
        [Display(Name = "Лимит Кассы: Максимум оставлять на следующий день")]
        public decimal DirCashOfficeLimit { get; set; }
        [Display(Name = "Это Главная касса")]
        public bool? IsMain { get; set; }
        [Display(Name = "Сумма денег в кассе, что бы каждый раз не подсчитывать - вычисляется Триггером")]
        public double DirCashOfficeSum { get; set; }


        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            if (IsMain == null) IsMain = false;
        }
    }
}