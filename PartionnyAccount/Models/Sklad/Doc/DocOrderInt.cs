using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    [Table("DocOrderInts")]
    public class DocOrderInt
    {
        [Key]
        public int? DocOrderIntID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Тип заказа (Из Мастерской, Предзаказ (Магазин))")]
        [Required]
        public int DirOrderIntTypeID { get; set; }
        [ForeignKey("DirOrderIntTypeID")]
        public virtual Models.Sklad.Dir.DirOrderIntType dirOrderIntType { get; set; }

        [Display(Name = "Документ который создал этот Заказ")] //Так же єто же число будет в "Docs.DocIDBase"
        //[Required]
        public int? DocID2 { get; set; }
        [ForeignKey("DocID2")]
        public virtual Models.Sklad.Doc.Doc doc2 { get; set; }

        [Display(Name = "Реальный номер документа который создал этот Заказ")]
        //[Required]
        public int? NumberReal { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Display(Name = "Статус")]
        public int DirOrderIntStatusID { get; set; }
        [ForeignKey("DirOrderIntStatusID")]
        public virtual Dir.DirOrderIntStatus dirOrderIntStatus { get; set; }



        [Display(Name = "Товар")]
        public int? DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }

        [Display(Name = "Наименование нового товара")]
        public string DirNomenName { get; set; }


        //Группы *** *** ***

        //0
        [Display(Name = "Тип устройства")]
        //[Required]
        public int? DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Models.Sklad.Dir.DirServiceNomen dirServiceNomen { get; set; }

        //1
        [Display(Name = "Товар")]
        [Required]
        public int DirNomen1ID { get; set; }
        [ForeignKey("DirNomen1ID")]
        public virtual Dir.DirNomen dirNomen1 { get; set; }

        //2
        [Display(Name = "Товар")]
        [Required]
        public int DirNomen2ID { get; set; }
        [ForeignKey("DirNomen2ID")]
        public virtual Dir.DirNomen dirNomen2 { get; set; }

        //3
        [Display(Name = "Категория")]
        public int? DirNomenCategoryID { get; set; }
        [ForeignKey("DirNomenCategoryID")]
        public virtual Dir.DirNomenCategory dirNomenCategory { get; set; }





        [Display(Name = "Продажная цена")]
        [Required]
        public double PriceVAT { get; set; }
        [Required]
        public double PriceCurrency { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }

        [Display(Name = "К-во")]
        public double Quantity { get; set; }


        //Характеристики *** *** ***
        public int? DirCharColourID { get; set; }
        public int? DirCharMaterialID { get; set; }
        public int? DirCharNameID { get; set; }
        public int? DirCharSeasonID { get; set; }
        public int? DirCharSexID { get; set; }
        public int? DirCharSizeID { get; set; }
        public int? DirCharStyleID { get; set; }
        public int? DirCharTextureID { get; set; }


        [Display(Name = "Выбор из справочника Контрагентов сервиса")]
        //[Required]
        public int? DirOrderIntContractorID { get; set; }
        [ForeignKey("DirOrderIntContractorID")]
        public virtual Dir.DirOrderIntContractor dirOrderIntContractor { get; set; }

        [Display(Name = "Поле для ввода имени Клиента (он же Контрагент)")]
        public string DirOrderIntContractorName { get; set; }
        [Display(Name = "Адрес (из справочника, если постоянный)")]
        public string DirOrderIntContractorAddress { get; set; }
        [Display(Name = "Телефон (из справочника, если постоянный)")]
        public string DirOrderIntContractorPhone { get; set; }
        [Display(Name = "Мейл (из справочника, если постоянный)")]
        public string DirOrderIntContractorEmail { get; set; }
        [Display(Name = "WWW (из справочника, если постоянный)")]
        public string DirOrderIntContractorWWW { get; set; }
        [Display(Name = "Desc (из справочника, если постоянный)")]
        public string DirOrderIntContractorDesc { get; set; }


        public double PrepaymentSum { get; set; }
        public DateTime DateDone { get; set; }



        #region  Doc

        [NotMapped]
        public bool? Del { get; set; }
        [NotMapped]
        public int ListObjectID { get; set; }
        [NotMapped]
        public string NumberInt { get; set; }
        [NotMapped]
        public DateTime? DocDate { get; set; }
        [NotMapped]
        public DateTime? DocDateCreate { get; set; }
        [NotMapped]
        public bool? Held { get; set; }
        [NotMapped]
        public double Discount { get; set; }
        [NotMapped]
        [Display(Name = "На основании какого документа создан данный")]
        public int? DocIDBase { get; set; }
        [NotMapped]
        public string Base { get; set; }
        [Display(Name = "Организация")]
        [NotMapped]
        public int DirContractorIDOrg { get; set; }
        [Display(Name = "Контрагент")]
        [NotMapped]
        public int DirContractorID { get; set; }
        [Display(Name = "Кто создал документ. Используется для начислении премии сотруднику")]
        [NotMapped]
        public int DirEmployeeID { get; set; }
        [NotMapped]
        public bool? IsImport { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [Display(Name = "Тип оплаты: Касса или Банк.")]
        [NotMapped]
        public int DirPaymentTypeID { get; set; }
        [Display(Name = "Сумма оплаты")]
        [NotMapped]
        public double Payment { get; set; }
        [Display(Name = "НДС")]
        [NotMapped]
        public double DirVatValue { get; set; }

        #region KKMS

        [Display(Name = "Номер чека в ККМ")]
        [NotMapped]
        public int? KKMSCheckNumber { get; set; }
        [Display(Name = "Уникалдьный номер комманды ККМS")]
        [NotMapped]
        public string KKMSIdCommand { get; set; }

        //параметры клиента
        [Display(Name = "EMail")]
        [NotMapped]
        public string KKMSEMail { get; set; }
        [Display(Name = "Phone")]
        [NotMapped]
        public string KKMSPhone { get; set; }

        #endregion

        [NotMapped]
        public DateTime? DocDateHeld { get; set; }
        [NotMapped]
        public DateTime? DocDatePayment { get; set; }

        #endregion



        #region API

        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocOrderInt { get; set; }

        [NotMapped]
        public string Key { get; set; }

        [NotMapped]
        public string pID { get; set; }

        #endregion



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля

            ListObjectID = 59;
            DirOrderIntStatusID = 10;

            if (PriceVAT == 0 && PriceCurrency > 0) PriceVAT = PriceCurrency;
        }
    }
}