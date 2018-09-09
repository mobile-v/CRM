using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Rem
{
    [Table("RemParties")]
    public class RemParty
    {
        [Key]
        public int? RemPartyID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Doc.Doc doc { get; set; }

        [Display(Name = "Организация")]
        [Required]
        public int DirContractorIDOrg { get; set; }
        [ForeignKey("DirContractorIDOrg")]
        public virtual Dir.DirContractor dirContractorOrg { get; set; }

        [Display(Name = "Поставщика от которого пришла партия первоначально - этот параметр передаётся во все другие партии (напр. перемещение)")]
        [Required]
        public int DirContractorID { get; set; }
        [ForeignKey("DirContractorID")]
        public virtual Dir.DirContractor dirContractor { get; set; }

        public DateTime? DocDatePurches { get; set; }

        [Display(Name = "Склад на который пришла партия")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Display(Name = "Склад с которого списали партию, если это приход от поставщика, то ставить склад на который приходуется товар.")]
        [Required]
        public int DirWarehouseIDDebit { get; set; }
        [ForeignKey("DirWarehouseIDDebit")]
        public virtual Dir.DirWarehouse dirWarehouseDebit { get; set; }

        [Display(Name = "Склад на который пришла партия первоначально - этот параметр передаётся во все другие партии (напр. перемещение)")]
        [Required]
        public int DirWarehouseIDPurch { get; set; }
        [ForeignKey("DirWarehouseIDPurch")]
        public virtual Dir.DirWarehouse dirWarehousePurch { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }



        /*
        public int? DirCharColourID { get; set; }
        public int? DirCharMaterialID { get; set; }
        public int? DirCharNameID { get; set; }
        public int? DirCharSeasonID { get; set; }
        public int? DirCharSexID { get; set; }
        public int? DirCharSizeID { get; set; }
        public int? DirCharStyleID { get; set; }
        public int? DirCharTextureID { get; set; }
        */

        //Характеристики *** *** *** *** *** *** *** ***
        [Display(Name = "Цвет")]
        public int? DirCharColourID { get; set; }
        [ForeignKey("DirCharColourID")]
        public virtual Dir.DirCharColour dirCharColour { get; set; }

        [Display(Name = "Материал")]
        public int? DirCharMaterialID { get; set; }
        [ForeignKey("DirCharMaterialID")]
        public virtual Dir.DirCharMaterial dirCharMaterial { get; set; }

        [Display(Name = "Наименование")]
        public int? DirCharNameID { get; set; }
        [ForeignKey("DirCharNameID")]
        public virtual Dir.DirCharName dirCharName { get; set; }

        [Display(Name = "Сезон")]
        public int? DirCharSeasonID { get; set; }
        [ForeignKey("DirCharSeasonID")]
        public virtual Dir.DirCharSeason dirCharSeason { get; set; }

        [Display(Name = "Пол")]
        public int? DirCharSexID { get; set; }
        [ForeignKey("DirCharSexID")]
        public virtual Dir.DirCharSex dirCharSex { get; set; }

        [Display(Name = "Размер")]
        public int? DirCharSizeID { get; set; }
        [ForeignKey("DirCharSizeID")]
        public virtual Dir.DirCharSize dirCharSize { get; set; }

        //Стиль и Поставщик
        [Display(Name = "Стиль и Поставщик")]
        public int? DirCharStyleID { get; set; }
        [ForeignKey("DirCharStyleID")]
        public virtual Dir.DirCharStyle dirCharStyle { get; set; }

        //Поставщик
        /*[Display(Name = "Поставщик")]
        public int? DirContractorID { get; set; }
        [ForeignKey("DirContractorID")]
        public virtual Dir.DirContractor dirContractor { get; set; }*/

        [Display(Name = "Текстура")]
        public int? DirCharTextureID { get; set; }
        [ForeignKey("DirCharTextureID")]
        public virtual Dir.DirCharTexture dirCharTexture { get; set; }
        //Характеристики *** *** *** *** *** *** *** ***



        public string SerialNumber { get; set; }
        public string Barcode { get; set; }

        [Display(Name = "Пришло первоначалоно")]
        [Required]
        public double Quantity { get; set; }
        [Display(Name = "Остаток")]
        [Required]
        public double Remnant { get; set; }

        [Required]
        public double PriceVAT { get; set; }
        [Required]
        public double DirVatValue { get; set; }
        [Required]
        public double PriceCurrency { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        //public double DirCurrencyRate { get; set; }
        //public int DirCurrencyMultiplicity { get; set; }

        [Required]
        public double PriceRetailVAT { get; set; }
        [Required]
        public double PriceRetailCurrency { get; set; }

        [Required]
        public double PriceWholesaleVAT { get; set; }
        [Required]
        public double PriceWholesaleCurrency { get; set; }

        [Required]
        public double PriceIMVAT { get; set; }
        [Required]
        public double PriceIMCurrency { get; set; }

        public int FieldID { get; set; }

        [Display(Name = "Списанеи партий. Для возврата покупателя")]
        public int? RemPartyMinusID { get; set; }

        [Display(Name = "Минимальный остаток")]
        [Required]
        public double? DirNomenMinimumBalance { get; set; }


        [Display(Name = "Тип возврата")]
        public int? DirReturnTypeID { get; set; }
        [ForeignKey("DirReturnTypeID")]
        public virtual Dir.DirReturnType dirReturnType { get; set; }

        [Display(Name = "Комментарий: причина возврата")]
        public int? DirDescriptionID { get; set; }
        [ForeignKey("DirDescriptionID")]
        public virtual Dir.DirDescription dirDescription { get; set; }


        [Display(Name = "Кто создал документ. Используется для начислении премии сотруднику")]
        [Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }

        [Required]
        public DateTime? DocDate { get; set; }

    }
}