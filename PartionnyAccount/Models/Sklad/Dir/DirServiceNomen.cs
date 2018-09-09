using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceNomens")]
    public class DirServiceNomen
    {
        [Key]
        public int? DirServiceNomenID { get; set; }
        public int? Sub { get; set; }
        public bool Del { get; set; }
        
        [Display(Name = "Имя")]
        public string DirServiceNomenName { get; set; }
        [Display(Name = "Для поиска")]
        public string NameLower { get; set; }
        [Display(Name = "Имя Полное")]
        public string DirServiceNomenNameFull { get; set; }
        [Display(Name = "Полное наименование")]
        public string NameFullLower { get; set; }
        public string Description { get; set; }
        public string DescriptionFull { get; set; }

        [Display(Name = "Дата последнего изменения записи")]
        public DateTime? DateTimeUpdate { get; set; }



        //Импорт в ИМ
        [Display(Name = "Импорт в ИМ")]
        public bool ImportToIM { get; set; }

        //Типичные неисправности
        [Display(Name = "Замена дисплейного модуля (экран+сенсор в сборе)")]
        public bool Faults1Check { get; set; }
        public double Faults1Price { get; set; }
        [Display(Name = "Замена сенсорного стекла (тачскрина)")]
        public bool Faults2Check { get; set; }
        public double Faults2Price { get; set; }
        [Display(Name = "Замена разъема зарядки")]
        public bool Faults3Check { get; set; }
        public double Faults3Price { get; set; }
        [Display(Name = "Замена разъема sim-карты")]
        public bool Faults4Check { get; set; }
        public double Faults4Price { get; set; }
        [Display(Name = "Обновление ПО (прошивка)")]
        public bool Faults5Check { get; set; }
        public double Faults5Price { get; set; }
        [Display(Name = "Замена динамика (слуховой)")]
        public bool Faults6Check { get; set; }
        public double Faults6Price { get; set; }
        [Display(Name = "Замена микрофона")]
        public bool Faults7Check { get; set; }
        public double Faults7Price { get; set; }
        [Display(Name = "Замена динамика (звонок)")]
        public bool Faults8Check { get; set; }
        public double Faults8Price { get; set; }
        [Display(Name = "Восстановление после попадания жидкости")]
        public bool Faults9Check { get; set; }
        public double Faults9Price { get; set; }
        [Display(Name = "Восстановление цепи питания")]
        public bool Faults10Check { get; set; }
        public double Faults10Price { get; set; }
        [Display(Name = "Ремонт материнской платы")]
        public bool Faults11Check { get; set; }
        public double Faults11Price { get; set; }
        [Display(Name = "Резерв-5")]
        public bool Faults12Check { get; set; }
        public double Faults12Price { get; set; }
        [Display(Name = "Резерв-6")]
        public bool Faults13Check { get; set; }
        public double Faults13Price { get; set; }
        [Display(Name = "Резерв-7")]
        public bool Faults14Check { get; set; }
        public double Faults14Price { get; set; }




        //DirServiceNomenPrice
        [NotMapped]
        public string recordsDirServiceNomenPriceGrid { get; set; }


        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            if (DirServiceNomenNameFull == null) DirServiceNomenNameFull = DirServiceNomenName;
        }
    }

    [Table("DirServiceNomenPrices")]
    public class DirServiceNomenPrice
    {
        [Key]
        public int? DirServiceNomenPriceID { get; set; }

        [Display(Name = "Устройства")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Шаблон наименовния Типовой Неисправности")]
        [Required]
        public int DirServiceNomenTypicalFaultID { get; set; }
        [ForeignKey("DirServiceNomenTypicalFaultID")]
        public virtual Dir.DirServiceNomenTypicalFault dirServiceNomenTypicalFault { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double PriceVAT { get; set; }
    }

    //Шаблон наименовния Типовой Неисправности
    [Table("DirServiceNomenTypicalFaults")]
    public class DirServiceNomenTypicalFault
    {
        [Key]
        public int? DirServiceNomenTypicalFaultID { get; set; }
        [Required]
        public string DirServiceNomenTypicalFaultName { get; set; }
    }
}