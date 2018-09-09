using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Sys
{
    [Table("SysSettings")]
    public class SysSetting
    {
        [Key]
        public int SysSettingsID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JurDateS { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JurDatePo { get; set; }
        [Required]
        public int FractionalPartInSum { get; set; }
        [Required]
        public int FractionalPartInPrice { get; set; }
        [Required]
        public int FractionalPartInOther { get; set; }

        [Display(Name = "Организация")]
        [Required]
        public int DirContractorIDOrg { get; set; }
        [ForeignKey("DirContractorIDOrg")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirContractor dirContractorOrg { get; set; }

        [Display(Name = "Налог")]
        [Required]
        public decimal DirVatValue { get; set; }
        public bool? ChangePriceNomen { get; set; }
        public bool? PurchBigerSale { get; set; }
        public bool? MinusResidues { get; set; }
        [Required]
        public int MethodAccounting { get; set; }
        public bool? DeletedRecordsShow { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Required]
        public decimal MarkupRetail { get; set; }
        [Required]
        public decimal MarkupWholesale { get; set; }
        [Required]
        public decimal MarkupIM { get; set; }
        [Required]
        public decimal MarkupSales1 { get; set; }
        [Required]
        public decimal MarkupSales2 { get; set; }
        [Required]
        public decimal MarkupSales3 { get; set; }
        [Required]
        public decimal MarkupSales4 { get; set; }

        public bool? CashBookAdd { get; set; }
        public bool? Reserve { get; set; }

        [Required]
        public int BarIntNomen { get; set; }
        [Required]
        public int BarIntContractor { get; set; }
        [Required]
        public int BarIntDoc { get; set; }
        [Required]
        public int BarIntEmployee { get; set; }

        public bool? SelectOneClick { get; set; }
        [Required]
        public int PageSizeDir { get; set; }
        [Required]
        public int PageSizeJurn { get; set; }

        [Required]
        public int DateFormat { get; set; }
        [NotMapped]
        public string DateFormatStr { get { return "yyyy.MM.dd"; } set { DateFormatStr = value; } }
        [Display(Name = "Тип цен")]
        [Required]
        public int DirPriceTypeID { get; set; }
        [ForeignKey("DirPriceTypeID")]
        public virtual Dir.DirPriceType dirPriceType { get; set; }
        [Required]
        public int LabelWidth { get; set; }
        [Required]
        public int LabelHeight { get; set; }
        [Required]
        public int LabelEncodeType { get; set; }
        [Display(Name = "Минимальный остаток партии")]
        [Required]
        public double DirNomenMinimumBalance { get; set; }
        [Display(Name = "Сервисный центр -> Приёмка: Число дней до даты готовности")]
        [Required]
        public int ReadinessDay { get; set; }
        [Required]
        public int ServiceTypeRepair { get; set; }
        public bool WarrantyPeriodPassed { get; set; } //Блокировать повторный ремонт, если сошёл срок гарантии
        //[Required]
        public string PhoneNumberBegin { get; set; }
        public bool DocServicePurchSmsAutoShow { get; set; } //АвтоСМС при примке аппарата

        //Система сама определяет какой аппарат: Готов или Отказной
        //А определяет по сумме из Настроек.КПД
        //Если Сумма < КПД == Отказной, выводить окно "Причина отказа" (сделать отдельный справочник Отказов, редактирование по Правам. Не разрешать редактировать КомбоБокс, только из справочника)
        //Если Сумма >= КПД == Готов
        public double ServiceKPD { get; set; } //КПД

        //Sms
        [Display(Name = "SMS")]
        public bool SmsActive { get; set; }
        public int? SmsServiceID { get; set; }
        public string SmsLogin { get; set; }
        public string SmsPassword { get; set; }
        public string SmsTelFrom { get; set; }
        public bool SmsAutoShow { get; set; } //Автоматически выводить отправку СМС
        public bool SmsAutoShow9 { get; set; } //Автоматически выводить отправку СМС
        public bool SmsAutoShowServiceFromArchiv { get; set; } //Автоматически выводить отправку СМС

        [Display(Name = "Переключаемся на уже открытую вкладку")]
        public bool TabIdenty { get; set; } //Автоматически выводить отправку СМС

        // *** ККМ ***
        [Display(Name = "Используется ККМ-Сервер")]
        public bool? KKMSActive { get; set; }
        [Display(Name = "Номер устройства. Если 0 то первое не блокированное на сервере")]
        public int KKMSNumDevice { get; set; }
        [Display(Name = "ИНН продавца тег ОФД 1203")]
        public string KKMSCashierVATIN { get; set; }
        [Display(Name = "Система налогообложения (СНО) применяемая для чека: [0 - 5]")]
        public int KKMSTaxVariant { get; set; }
        [Display(Name = "Система налогообложения (СНО) применяемая для чека: [0 - 5]")]
        public int KKMSTax { get; set; }

        [Display(Name = "Финансы -> Оплата документов [0: Касса + Банк, 1: Касса, 2: Банк]")]
        public int PayType { get; set; }

        [Display(Name = "Сортировка списка документов по ... ")] //По порядковому номеру, По дате поставщика, По дате проведения, По дате оплаты
        public int DocsSortField { get; set; }

        [Display(Name = "Максимальный процент скидки от суммы: Торговля, СЦ, БУ")]
        public double DiscountPercentMarket { get; set; }
        public double DiscountPercentService { get; set; }
        public double DiscountPercentSecondHand { get; set; }
        [Display(Name = "Скидка: от суммы или от цены (если продали более 1 аппарата)")]
        public int DiscountMarketType { get; set; }

        [Display(Name = "БУ: Скидка для комиссионных аппаратов")]
        public bool DocSecondHandSalesDiscount { get; set; }

        [Display(Name = "СЦ: Отправка СМС при проведении перемецения")]
        public bool SmsServiceMov { get; set; }


        [Display(Name = "Заказы: Отправка СМС при приёме аппарата")]
        public bool SmsOrderInt5 { get; set; }
        [Display(Name = "Заказы: Отправка СМС при готов к выдачи")]
        public bool SmsOrderInt9 { get; set; }
        [Display(Name = "Заказы: Отправка СМС при исполнен")]
        public bool SmsOrderInt10 { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            if (MinusResidues == null) MinusResidues = false;
            if (SelectOneClick == null) SelectOneClick = false;
            if (ChangePriceNomen == null) ChangePriceNomen = false;
            if (PurchBigerSale == null) PurchBigerSale = false;
            if (DeletedRecordsShow == null) DeletedRecordsShow = false;
            if (CashBookAdd == null) CashBookAdd = false;
            if (Reserve == null) Reserve = false;
            if (KKMSActive == null || KKMSActive == false)
            {
                KKMSActive = false;
                KKMSNumDevice = 1;
                KKMSCashierVATIN = "";
                KKMSTaxVariant = 0;
                KKMSTax = 0;
            }
        }
    }
}