using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirNomens")]
    public class DirNomen
    {
        [Key]
        public int? DirNomenID { get; set; }

        [Display(Name = "Если надо создать новый товар с наперёд заданым номером")]
        [NotMapped]
        public int? DirNomenID_INSERT { get; set; }

        public Int64? DirNomenID_OLD { get; set; }

        //public int? Sub { get; set; }
        [Display(Name = "Под группа")]
        public int? Sub { get; set; }
        [ForeignKey("Sub")]
        public virtual Dir.DirNomen dirNomenSub { get; set; }

        public bool Del { get; set; }
        public string DirNomenArticle { get; set; }

        [Display(Name = "Тип товара")]
        public int? DirNomenCategoryID { get; set; }
        [ForeignKey("DirNomenCategoryID")]
        public virtual Dir.DirNomenCategory dirNomenCategory { get; set; }

        [Display(Name = "Тип товара")]
        [Required]
        public int DirNomenTypeID { get; set; }
        [ForeignKey("DirNomenTypeID")]
        public virtual Dir.DirNomenType dirNomenType { get; set; }

        [Display(Name = "Имя")]
        public string DirNomenName { get; set; }
        [Display(Name = "Для поиска")]
        public string NameLower { get; set; }
        [Display(Name = "Имя Полное")]
        public string DirNomenNameFull { get; set; }
        [Display(Name = "Полное наименование")]
        public string NameFullLower { get; set; }
        public string Description { get; set; }
        public string DescriptionFull { get; set; }

        public string ImageLink { get; set; }

        [Display(Name = "Дата последнего изменения записи")]
        public DateTime? DateTimeUpdate { get; set; }


        [Display(Name = "GenID Для изображения")]
        public int? SysGenID { get; set; }
        public int? SysGen1ID { get; set; }
        public int? SysGen2ID { get; set; }
        public int? SysGen3ID { get; set; }
        public int? SysGen4ID { get; set; }
        public int? SysGen5ID { get; set; }


        [Display(Name = "Принять изображение с Веб-камеры")]
        [NotMapped]
        //public Byte? photoWebCam { get; set; }
        public string photoWebCam { get; set; }


        //ИМ
        [Display(Name = "Импорт в ИМ")]
        public bool ImportToIM { get; set; }

        //ККМ
        [Display(Name = "ККМ - НДС")]
        public int KKMSTax { get; set; }


        [Display(Name = "URL: Наименование на Латинице")]
        public string DirNomenNameURL { get; set; }


        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            if (DirNomenNameFull == null) DirNomenNameFull = DirNomenName;

            if (DirNomenNameURL == null)
            {
                DirNomenNameURL = PartionnyAccount.Classes.Function.Transliteration.Front(DirNomenName);
            }

        }
    }


    [Table("DirNomenHistories")]
    public class DirNomenHistory
    {
        [Key]
        public int? DirNomenHistoryID { get; set; }

        [Display(Name = "Тип товара")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }

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
