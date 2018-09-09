using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    [Table("DocSalaries")]
    public class DocSalary
    {
        [Key]
        public int? DocSalaryID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Год и месяц документа")]
        [Required]
        public int DocYear { get; set; }
        [Required]
        public int DocMonth { get; set; }



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



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSalaryTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 64;
        }
    }

    //[Table("DocSalaryTabs")]
    public class DocSalaryTab
    {
        [Key]
        public int? DocSalaryTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocSalaryID { get; set; }
        [ForeignKey("DocSalaryID")]
        public virtual DocSalary docSalary { get; set; }

        [Display(Name = "Сотрудник")]
        [Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual Dir.DirEmployee dirEmployee { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }


        //*** *** ***
        [Display(Name = "Сумма ЗП + Премия1 + Премия2")]
        [Required]
        public double Salary { get; set; }
        [Required]
        public int SalaryDayMonthly { get; set; }

        [Display(Name = "К-во отработанных дней")]
        public int CountDay { get; set; }
        [Display(Name = "Сумма ЗП исходя из SalaryDayMonthly")]
        public double SumSalary { get; set; }
        [Display(Name = "Продажи: фиксированный оклад за месяц")]
        public double SalaryFixedSalesMount { get; set; }


        //*** *** ***
        [Display(Name = "Премия (продавца)")]
        public int? DirBonusID { get; set; }
        [ForeignKey("DirBonusID")]
        public virtual Dir.DirBonus dirBonus { get; set; }

        [Display(Name = "Премия (продавца) сумма")]
        public double DirBonusIDSalary { get; set; }


        //*** *** ***
        [Display(Name = "Премия (мастера)")]
        public int? DirBonus2ID { get; set; }
        [ForeignKey("DirBonus2ID")]
        public virtual Dir.DirBonus dirBonus2 { get; set; }

        [Display(Name = "Премия (мастера) сумма")]
        public double DirBonus2IDSalary { get; set; }

        [Display(Name = "Ремонт: фиксированной суммы с каждого ремонта, скажем 300 руб")]
        public double SalaryFixedServiceOne { get; set; }


        //*** *** ***
        [Display(Name = "Сумма ЗП + Премия1 + Премия2")]
        public double Sums { get; set; }

    }

    //[Table("DocSalaryTabs")]
    public class DocSalaryTabSQL
    {
        [Key]
        public int? DocSalaryTabID { get; set; }
        public int DocSalaryID { get; set; }

        [NotMapped]
        public DateTime DocDate { get; set; }

        public int DirEmployeeID { get; set; }
        public string DirEmployeeName { get; set; }

        public int DirCurrencyID { get; set; }
        public string DirCurrencyName { get; set; }
        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }

        //*** *** ***
        [Display(Name = "Сумма ЗП")]
        public double Salary { get; set; }
        [Display(Name = "К-во отработанных дней")]
        public int CountDay { get; set; }
        public int SalaryDayMonthly { get; set; }
        public string SalaryDayMonthlyName { get; set; }

        [Display(Name = "Сумма ЗП исходя из SalaryDayMonthly")]
        public double SumSalary { get; set; }

        [Display(Name = "Продажи: фиксированный оклад за месяц")]
        public double SalaryFixedSalesMount { get; set; }

        //*** *** ***
        [Display(Name = "Премия (продавца)")]
        public int? DirBonusID { get; set; }
        public string DirBonusName { get; set; }
        [Display(Name = "Премия (продавца) сумма")]
        public double DirBonusIDSalary { get; set; }

        //*** *** ***
        [Display(Name = "Премия (мастера)")]
        public int? DirBonus2ID { get; set; }
        public string DirBonus2Name { get; set; }
        [Display(Name = "Премия (мастера) сумма")]
        public double DirBonus2IDSalary { get; set; }
        [Display(Name = "Ремонт: фиксированной суммы с каждого ремонта, скажем 300 руб")]
        public double SumSalaryFixedServiceOne { get; set; }



        //Б/У === === ===

        //1. Ремонт
        [Display(Name = "Начислить за ремонт после продажи")]
        public bool SalarySecondHandWorkshopCheck { get; set; }

        [Display(Name = "Премия для мастера Б/У")]
        public int? DirBonus3ID { get; set; }
        public string DirBonus3Name { get; set; }
        [Display(Name = "Премия (мастера) сумма")]
        public double DirBonus3IDSalary { get; set; }
        [Display(Name = "Б/У: фиксированная суммы с каждого ремонта, скажем 300 руб")]
        public double? SumSalaryFixedSecondHandWorkshopOne { get; set; }

        //2. Продажа
        [Display(Name = "Премия для продавца Б/У")]
        public int? DirBonus4ID { get; set; }
        public string DirBonus4Name { get; set; }
        [Display(Name = "Премия (продавца) сумма")]
        public double DirBonus4IDSalary { get; set; }
        [Display(Name = "Б/У: фиксированная суммы с каждой продажи, скажем 300 руб")]
        public double? SumSalaryFixedSecondHandRetailOne { get; set; }

        //СЦ
        public double sumSalaryPercentService1Tabs { get; set; }
        public double sumSalaryPercentService1TabsCount { get; set; }
        public double sumSalaryPercentService2Tabs { get; set; }

        [Display(Name = "ДР.РАСХОДЫ - выдача ЗП в средине месяца")]
        public double DomesticExpensesSalary { get; set; }

        [Display(Name = "Инвентаризация")]
        public double sumSecondHandInventory { get; set; }

        //*** *** ***
        [Display(Name = "Сумма ЗП + Премия1 + Премия2")]
        public double Sums { get; set; }

    }

    //[Table("DocSalaryTabs")]
    public class DocSalaryTabSQL2
    {
        [Key]
        public int? DocSalaryTabID { get; set; }

        [Display(Name = "Дата")]
        public DateTime? DocDate { get; set; }

        //Торговля *** *** ***
        [Display(Name = "Торговля.НАЛ")]
        public double TradeCash { get; set; }
        [Display(Name = "Торговля.БЕЗНАЛ")]
        public double TradeBank { get; set; }
        //СЦ *** *** ***
        [Display(Name = "СЦ.НАЛ")]
        public double ServiceCash { get; set; }
        [Display(Name = "СЦ.БЕЗНАЛ")]
        public double ServiceBank { get; set; }
        //БУ *** *** ***
        [Display(Name = "БУ.НАЛ")]
        public double SecondCash { get; set; }
        [Display(Name = "БУ.БЕЗНАЛ")]
        public double SecondBank { get; set; }
        //КАССА *** *** ***
        [Display(Name = "КАССА")]
        public double TradeSumCashBank { get; set; }

        //Приход *** *** ***
        [Display(Name = "ТОВ.СЕБЕСТ. (Сумма прихода)")]
        public double? PurchesSum { get; set; }

        //ЗАКУП (Приходная накладная на точку) *** *** ***
        [Display(Name = "ЗАКУП.НАЛ (сумма прихода товара на точку - чисто информативное поле, сейчас все точки сами себе делают закупку)")]
        public double DocPurchesCashSum { get; set; }
        [Display(Name = "ЗАКУП.БЕЗНАЛ (сумма прихода товара на точку - чисто информативное поле, сейчас все точки сами себе делают закупку)")]
        public double DocPurchesBankSum { get; set; }

        //Б/У ЗАКУП *** *** ***
        [Display(Name = "Б/У ЗАКУП.НАЛ")]
        public double SecondCashPurch { get; set; }
        [Display(Name = "Б/У ЗАКУП.БЕЗНАЛ")]
        public double SecondBankPurch { get; set; }


        //Хоз. расчёты *** *** ***
        //Хоз.расходы
        [Display(Name = "ДР.РАСХОДЫ - хоз расчёты")]
        public double DomesticExpenses { get; set; }
        [Display(Name = "ДР.РАСХОДЫ - выдача ЗП в средине месяца")]
        public double DomesticExpensesSalary { get; set; }
        [Display(Name = "ИНКАСС")]
        public double Encashment { get; set; }


        [Display(Name = "Инвентаризация")]
        public double InventorySum1 { get; set; }



        [Display(Name = "Зарплата")]
        public double sumSalaryPercentTrade { get; set; }
        public double sumSalaryPercentService1Tabs { get; set; } public double sumSalaryPercentService1TabsCount { get; set; }
        public double sumSalaryPercentService2Tabs { get; set; }
        public double sumSalaryPercentSecond { get; set; }
        public double sumSecondHandInventory { get; set; }
        public double sumSalaryPercent2Second { get; set; }
        public double sumSalaryPercent3Second { get; set; }

        public double SalatyProc { get; set; }

    }
}