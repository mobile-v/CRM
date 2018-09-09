using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Service.IM
{
    [Table("DirWebShopUOs")]
    public class DirWebShopUO
    {
        [Key]
        public int? DirWebShopUOID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }
        [Required]
        public string DirWebShopUOName { get; set; }
        [Required]
        public string DomainName { get; set; }

        [Display(Name = "Тип цены")]
        [Required]
        public int Nomen_DirPriceTypeID { get; set; }
        [ForeignKey("Nomen_DirPriceTypeID")]
        public virtual Dir.DirPriceType dirPriceType { get; set; }

        [Display(Name = "Контроль остатков")]
        //[Required]
        public bool? Nomen_Remains { get; set; }

        [Display(Name = "Валюта: нужна только из-за аббревиатуры")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        [Display(Name = "Статус заказа для документа")]
        [Required]
        public int Orders_Doc_DirOrdersStateID { get; set; }
        [ForeignKey("Orders_Doc_DirOrdersStateID")]
        public virtual Dir.DirOrdersState Orders_Doc_DirOrdersState { get; set; }

        [Display(Name = "Статус заказа для товара")]
        [Required]
        public int Orders_Nomen_DirOrdersStateID { get; set; }
        [ForeignKey("Orders_Nomen_DirOrdersStateID")]
        public virtual Dir.DirOrdersState Orders_Nomen_DirOrdersState { get; set; }

        [Display(Name = "Точка - Склад")]
        [Required]
        public int Orders_DirWarehouseID { get; set; }
        [ForeignKey("Orders_DirWarehouseID")]
        public virtual Dir.DirWarehouse Orders_DirWarehouse { get; set; }

        [Display(Name = "Организация")]
        [Required]
        public int Orders_DirContractorIDOrg { get; set; }
        [ForeignKey("Orders_DirContractorIDOrg")]
        public virtual Dir.DirContractor Orders_DirContractorOrg { get; set; }

        [Display(Name = "Контрагент: Розничный покупатель")]
        [Required]
        public int Orders_DirContractorID { get; set; }
        [ForeignKey("Orders_DirContractorID")]
        public virtual Dir.DirContractor Orders_DirContractor { get; set; }

        [Display(Name = "Резервировать товар")]
        //[Required]
        public bool? Orders_Reserve { get; set; }


        //Слайдер === === === === ===
        [Display(Name = "К-во товара в Слайдере")]
        [Required]
        public int Slider_Quantity { get; set; }

        [Display(Name = "Слайдер: Товар-1")]
        public int? Slider_DirNomen1ID { get; set; }
        [ForeignKey("Slider_DirNomen1ID")]
        public virtual Dir.DirNomen Slider_DirNomen1 { get; set; }
        public string SysGen1ID { get; set; }

        [Display(Name = "Слайдер: Товар-2")]
        public int? Slider_DirNomen2ID { get; set; }
        [ForeignKey("Slider_DirNomen2ID")]
        public virtual Dir.DirNomen Slider_DirNomen2 { get; set; }
        public string SysGen2ID { get; set; }

        [Display(Name = "Слайдер: Товар-3")]
        public int? Slider_DirNomen3ID { get; set; }
        [ForeignKey("Slider_DirNomen3ID")]
        public virtual Dir.DirNomen Slider_DirNomen3 { get; set; }
        public string SysGen3ID { get; set; }

        [Display(Name = "Слайдер: Товар-4")]
        public int? Slider_DirNomen4ID { get; set; }
        [ForeignKey("Slider_DirNomen4ID")]
        public virtual Dir.DirNomen Slider_DirNomen4 { get; set; }
        public string SysGen4ID { get; set; }


        //Рекомендованные === === === === ===
        [Display(Name = "К-во товара в Рекомендованных")]
        [Required]
        public int Recommended_Quantity { get; set; }

        [Display(Name = "Слайдер: Товар-1")]
        public int? Recommended_DirNomen1ID { get; set; }
        [ForeignKey("Recommended_DirNomen1ID")]
        public virtual Dir.DirNomen Recommended_DirNomen1 { get; set; }

        [Display(Name = "Слайдер: Товар-2")]
        public int? Recommended_DirNomen2ID { get; set; }
        [ForeignKey("Recommended_DirNomen2ID")]
        public virtual Dir.DirNomen Recommended_DirNomen2 { get; set; }

        [Display(Name = "Слайдер: Товар-3")]
        public int? Recommended_DirNomen3ID { get; set; }
        [ForeignKey("Recommended_DirNomen3ID")]
        public virtual Dir.DirNomen Recommended_DirNomen3 { get; set; }

        [Display(Name = "Слайдер: Товар-4")]
        public int? Recommended_DirNomen4ID { get; set; }
        [ForeignKey("Recommended_DirNomen4ID")]
        public virtual Dir.DirNomen Recommended_DirNomen4 { get; set; }

        [Display(Name = "Слайдер: Товар-5")]
        public int? Recommended_DirNomen5ID { get; set; }
        [ForeignKey("Recommended_DirNomen5ID")]
        public virtual Dir.DirNomen Recommended_DirNomen5 { get; set; }

        [Display(Name = "Слайдер: Товар-6")]
        public int? Recommended_DirNomen6ID { get; set; }
        [ForeignKey("Recommended_DirNomen6ID")]
        public virtual Dir.DirNomen Recommended_DirNomen6 { get; set; }

        [Display(Name = "Слайдер: Товар-7")]
        public int? Recommended_DirNomen7ID { get; set; }
        [ForeignKey("Recommended_DirNomen7ID")]
        public virtual Dir.DirNomen Recommended_DirNomen7 { get; set; }

        [Display(Name = "Слайдер: Товар-8")]
        public int? Recommended_DirNomen8ID { get; set; }
        [ForeignKey("Recommended_DirNomen8ID")]
        public virtual Dir.DirNomen Recommended_DirNomen8 { get; set; }

        [Display(Name = "Слайдер: Товар-9")]
        public int? Recommended_DirNomen9ID { get; set; }
        [ForeignKey("Recommended_DirNomen9ID")]
        public virtual Dir.DirNomen Recommended_DirNomen9 { get; set; }

        [Display(Name = "Слайдер: Товар-10")]
        public int? Recommended_DirNomen10ID { get; set; }
        [ForeignKey("Recommended_DirNomen10ID")]
        public virtual Dir.DirNomen Recommended_DirNomen10 { get; set; }

        [Display(Name = "Слайдер: Товар-11")]
        public int? Recommended_DirNomen11ID { get; set; }
        [ForeignKey("Recommended_DirNomen11ID")]
        public virtual Dir.DirNomen Recommended_DirNomen11 { get; set; }

        [Display(Name = "Слайдер: Товар-12")]
        public int? Recommended_DirNomen12ID { get; set; }
        [ForeignKey("Recommended_DirNomen12ID")]
        public virtual Dir.DirNomen Recommended_DirNomen12 { get; set; }

        [Display(Name = "Слайдер: Товар-13")]
        public int? Recommended_DirNomen13ID { get; set; }
        [ForeignKey("Recommended_DirNomen13ID")]
        public virtual Dir.DirNomen Recommended_DirNomen13 { get; set; }

        [Display(Name = "Слайдер: Товар-14")]
        public int? Recommended_DirNomen14ID { get; set; }
        [ForeignKey("Recommended_DirNomen14ID")]
        public virtual Dir.DirNomen Recommended_DirNomen14 { get; set; }

        [Display(Name = "Слайдер: Товар-15")]
        public int? Recommended_DirNomen15ID { get; set; }
        [ForeignKey("Recommended_DirNomen15ID")]
        public virtual Dir.DirNomen Recommended_DirNomen15 { get; set; }

        [Display(Name = "Слайдер: Товар-16")]
        public int? Recommended_DirNomen16ID { get; set; }
        [ForeignKey("Recommended_DirNomen16ID")]
        public virtual Dir.DirNomen Recommended_DirNomen16 { get; set; }


        //HTML === === === === ===
        [Display(Name = "Оплата")]
        public string Payment { get; set; }
        [Display(Name = "О нас")]
        public string AboutUs { get; set; }
        [Display(Name = "Доставка")]
        public string DeliveryInformation { get; set; }
        [Display(Name = "Конфиденциальность")]
        public string PrivacyPolicy { get; set; }
        [Display(Name = "Сроки & Условия")]
        public string TermsConditions { get; set; }
        [Display(Name = "Свяжитесь с нами")]
        public string ContactUs { get; set; }
        [Display(Name = "Возвраты")]
        public string Returns { get; set; }
        [Display(Name = "Карта сайта")]
        public string SiteMap { get; set; }
        [Display(Name = "Филиалы")]
        public string Affiliate { get; set; }
        [Display(Name = "Специальные предложения")]
        public string Specials { get; set; }


        [Display(Name = "Тип верхнего меню")]
        [Required]
        public int DirNomenGroup_Top { get; set; }



    }
}