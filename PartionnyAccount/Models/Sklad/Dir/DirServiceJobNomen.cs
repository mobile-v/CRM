using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirServiceJobNomens")]
    public class DirServiceJobNomen
    {
        [Key]
        public int? DirServiceJobNomenID { get; set; }
        public int? Sub { get; set; }
        public bool Del { get; set; }
        public string DirServiceJobNomenArticle { get; set; }
        public int? DirServiceNomenCategoryID { get; set; }

        [Display(Name = "Тип товара")]
        //[Required]
        public int? DirNomenTypeID { get; set; }
        [ForeignKey("DirNomenTypeID")]
        public virtual Dir.DirNomenType dirNomenType { get; set; }

        [Display(Name = "Имя")]
        public string DirServiceJobNomenName { get; set; }
        [Display(Name = "Для поиска")]
        public string NameLower { get; set; }
        [Display(Name = "Имя Полное")]
        public string DirServiceJobNomenNameFull { get; set; }
        [Display(Name = "Полное наименование")]
        public string NameFullLower { get; set; }
        public string Description { get; set; }
        public string DescriptionFull { get; set; }

        public string ImageLink { get; set; }

        [Display(Name = "Дата последнего изменения записи")]
        public DateTime? DateTimeUpdate { get; set; }


        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        [Required]
        public double PriceRetailVAT { get; set; }
        [Required]
        public double PriceWholesaleVAT { get; set; }
        [Required]
        public double PriceIMVAT { get; set; }

        [Display(Name = "1 - СЦ, 2 - БУ")]
        public int DirServiceJobNomenType { get; set; }


        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            if (DirServiceJobNomenNameFull == null) DirServiceJobNomenNameFull = DirServiceJobNomenName;
            if (DirNomenTypeID == null) DirNomenTypeID = 2;
        }
    }

    [Table("DirServiceJobNomenHistories")]
    public class DirServiceJobNomenHistory
    {
        [Key]
        public int? DirServiceJobNomenHistoryID { get; set; }

        [Display(Name = "Тип товара")]
        [Required]
        public int DirServiceJobNomenID { get; set; }
        [ForeignKey("DirServiceJobNomenID")]
        public virtual Dir.DirServiceJobNomen dirServiceJobNomen { get; set; }

        [Required]
        public DateTime? HistoryDate { get; set; }
        [NotMapped]
        public DateTime? HistoryDateTime { get; set; }
        [Display(Name = "Приходная цена")]
        [Required]
        public double PriceVAT { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int? DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        [Required]
        public double MarkupRetail { get; set; }
        [Required]
        public double PriceRetailVAT { get; set; }

        [Required]
        public double MarkupWholesale { get; set; }
        [Required]
        public double PriceWholesaleVAT { get; set; }

        [Required]
        public double MarkupIM { get; set; }
        [Required]
        public double PriceIMVAT { get; set; }

    }
}