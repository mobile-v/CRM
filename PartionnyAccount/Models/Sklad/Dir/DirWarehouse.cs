using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirWarehouses")]
    public class DirWarehouse
    {
        [Key]
        public int? DirWarehouseID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }

        [Display(Name = "Под группа")]
        public int? Sub { get; set; }
        [ForeignKey("Sub")]
        public virtual Dir.DirWarehouse dirWarehouseSub { get; set; }

        [Required]
        public string DirWarehouseName { get; set; }
        public string DirWarehouseAddress { get; set; }
        public string Phone { get; set; }

        [Display(Name = "Касса")]
        [Required]
        public int DirCashOfficeID { get; set; }
        [ForeignKey("DirCashOfficeID")]
        public virtual Dir.DirCashOffice dirCashOffice { get; set; }

        [Display(Name = "Банк")]
        [Required]
        public int DirBankID { get; set; }
        [ForeignKey("DirBankID")]
        public virtual Dir.DirBank dirBank { get; set; }

        public int? DirWarehouseLoc { get; set; }


        //Зарплата
        [Display(Name = "ЗП: торговля.Тип")]
        public int SalaryPercentTradeType { get; set; }
        public double SalaryPercentTrade { get; set; }

        [Display(Name = "ЗП: Работы.Тип")]
        public int SalaryPercentService1TabsType { get; set; }
        public double SalaryPercentService1Tabs { get; set; }

        [Display(Name = "ЗП: Запчасти.Тип")]
        public int SalaryPercentService2TabsType { get; set; }
        public double SalaryPercentService2Tabs { get; set; }




        //ЗП: БУ
        //Точка продавшая аппарат === === ===
        [Display(Name = "% с прибыли")]
        public double SalaryPercentSecond { get; set; }
        [Display(Name = "Фикс. с каждого проданной единиц")]//
        public double SalaryPercent2Second { get; set; }
        [Display(Name = "Фикс. за отремонтированную единицу")]//
        public double SalaryPercent3Second { get; set; }
        [Display(Name = "% от стоимости аппарата")]//
        public double SalaryPercent7Second { get; set; }

        //Точка купившая аппарат === === ===
        [Display(Name = "% с прибыли")]//
        public double SalaryPercent4Second { get; set; }
        [Display(Name = "Фикс. с каждого проданной единиц")]//
        public double SalaryPercent5Second { get; set; }
        [Display(Name = "Фикс. за отремонтированную единицу")]//
        public double SalaryPercent6Second { get; set; }
        [Display(Name = "% от стоимости аппарата")]//
        public double SalaryPercent8Second { get; set; }

        //Автоматическое закрытие смены === === ===
        [Display(Name = "Действует закрыие смены")]
        public bool SmenaClose { get; set; }
        [Display(Name = "Время закрытия")]
        public string SmenaCloseTime { get; set; }



        //ККМ
        //Используется ли на точка ККМ
        public bool KKMSActive { get; set; }

    }
}