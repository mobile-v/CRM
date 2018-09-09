using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{

    #region DocSecondHandPurches

    //[Table("DocSecondHandPurches")]
    public class DocSecondHandPurch
    {
        [Key]
        public int? DocSecondHandPurchID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Display(Name = "Склад")]
        //[Required]
        public int? DirWarehouseIDPurches { get; set; }
        [ForeignKey("DirWarehouseIDPurches")]
        public virtual Dir.DirWarehouse dirWarehousePurches { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Статус")]
        //[Required]
        public int? DirSecondHandStatusID { get; set; }
        [ForeignKey("DirSecondHandStatusID")]
        public virtual Dir.DirSecondHandStatus dirSecondHandStatus { get; set; }

        [Display(Name = "Статус: Готов или Отказ (заполняется при выдаче)")]
        //[Required]
        public int? DirSecondHandStatusID_789 { get; set; }
        [ForeignKey("DirSecondHandStatusID_789")]
        public virtual Dir.DirSecondHandStatus dirSecondHandStatus_789 { get; set; }

        [Display(Name = "Серийный номер")]
        public bool? SerialNumberNo { get; set; }
        public string SerialNumber { get; set; }
        [Display(Name = "Пароль на устройстве")]
        public bool? ComponentPasTextNo { get; set; }
        [Display(Name = "Пароль на устройстве: поле для ввода пароля")]
        public string ComponentPasText { get; set; }
        [Display(Name = "Другое: Поле для ввода текста")]
        public string ComponentOtherText { get; set; }
        [Display(Name = "Неисправность со слов клиента (большое поле для ввода)")]
        public string ProblemClientWords { get; set; }
        [Display(Name = "Примечание")]
        public string Note { get; set; }
        [Display(Name = "Поле для ввода имени Клиента (он же Контрагент)")]
        public string DirServiceContractorName { get; set; }

        //Клиент
        [Display(Name = "Постоянный Клиент")]
        public bool? DirServiceContractorRegular { get; set; }

        [Display(Name = "Выбор из справочника Контрагентов сервиса")]
        //[Required]
        public int? DirServiceContractorID { get; set; }
        [ForeignKey("DirServiceContractorID")]
        public virtual Dir.DirServiceContractor dirServiceContractor { get; set; }

        [Display(Name = "Адрес (из справочника, если постоянный)")]
        public string DirServiceContractorAddress { get; set; }
        [Display(Name = "Телефон (из справочника, если постоянный)")]
        public string DirServiceContractorPhone { get; set; }
        [Display(Name = "Мейл (из справочника, если постоянный)")]
        public string DirServiceContractorEmail { get; set; }
        [Display(Name = "Паспорт: Серия")]
        public string PassportSeries { get; set; }
        [Display(Name = "Паспорт: Номер")]
        public string PassportNumber { get; set; }

        //Цены
        [Display(Name = "Сумма сделки")]
        public double PriceVAT { get; set; }
        [Display(Name = "Сумма сделки в тек.валюте")]
        public double PriceCurrency { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }

        //Цены
        [Display(Name = "Розница")]
        public double? PriceRetailVAT { get; set; }
        public double? PriceRetailCurrency { get; set; }
        //Цены
        [Display(Name = "Опт")]
        public double? PriceWholesaleVAT { get; set; }
        public double? PriceWholesaleCurrency { get; set; }
        //Цены
        [Display(Name = "И-М")]
        public double? PriceIMVAT { get; set; }
        public double? PriceIMCurrency { get; set; }

        //Устанавливается при приёмке Аппарата
        [Display(Name = "Дата готовности")]
        [Required]
        public DateTime DateDone { get; set; }

        [Display(Name = "Мастер")]
        //[Required]
        public int? DirEmployeeIDMaster { get; set; }
        [ForeignKey("DirEmployeeIDMaster")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }


        public int ServiceTypeRepair { get; set; }

        [Display(Name = "Сумма: что бы не подсчитывать каждый раз сумму")]
        public double? Summ_NotPre { get; set; }

        [Display(Name = "Дата, когда аппарат переместили на выдачу")]
        public DateTime? IssuanceDate { get; set; }

        //До 04.06.2017 == IssuanceDate
        [Display(Name = "Дата смены статуса")]
        public DateTime? DateStatusChange { get; set; }

        [Display(Name = "Сумма - считается тригером")]
        public double? Sums { get; set; }
        [Display(Name = "Сумма - выполненных работ")]
        public double? Sums1 { get; set; }
        [Display(Name = "Сумма - запчастей")]
        public double? Sums2 { get; set; }

        [Display(Name = "Если вернули на доработку, то запомнить первичную дату поступления аппарта")]
        public DateTime? DocDate_First { get; set; }

        //СЦ
        [Display(Name = "Аппарат пришёл с модуля СЦ")]
        public bool? FromService { get; set; }

        [Display(Name = "Документ с модуля СЦ")]
        public int? DocIDService { get; set; }
        [ForeignKey("DocIDService")]
        public virtual Models.Sklad.Doc.Doc docService { get; set; }

        [Display(Name = "Сумма из СЦ")]
        public double? Sums1Service { get; set; }

        [Display(Name = "Сумма из СЦ")]
        public double? Sums2Service { get; set; }


        //Устанавливается при приёмке Аппарата
        [Display(Name = "Дата продажи")]
        public DateTime? DateRetail { get; set; }
        [Display(Name = "Дата возврата")]
        public DateTime? DateReturn { get; set; }



        [Display(Name = "Тип возврата")]
        public int? DirReturnTypeID { get; set; }
        [ForeignKey("DirReturnTypeID")]
        public virtual Dir.DirReturnType dirReturnType { get; set; }

        [Display(Name = "Комментарий: причина возврата")]
        public int? DirDescriptionID { get; set; }
        [ForeignKey("DirDescriptionID")]
        public virtual Dir.DirDescription dirDescription { get; set; }




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



        //Левак всякий (((
        [NotMapped]
        public bool? DocSecondHandRetailID { get; set; }




        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSecondHandPurch1Tab { get; set; }
        [NotMapped]
        public string recordsDocSecondHandPurch2Tab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            if (ComponentPasTextNo == true) { ComponentPasText = null; }
            if (DirSecondHandStatusID == null) DirSecondHandStatusID = 1;

            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 65;
        }
    }

    [Table("DocSecondHandPurch1Tabs")]
    public class DocSecondHandPurch1Tab
    {
        [Key]
        public int? DocSecondHandPurch1TabID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int DocSecondHandPurchID { get; set; }
        [ForeignKey("DocSecondHandPurchID")]
        public virtual Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch { get; set; }

        [Display(Name = "Мастер")]
        //[Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }

        [Display(Name = "Выполненная работа")]
        //[Required]
        public int? DirServiceJobNomenID { get; set; }
        [ForeignKey("DirServiceJobNomenID")]
        public virtual Models.Sklad.Dir.DirServiceJobNomen dirServiceJobNomen { get; set; }

        [Display(Name = "Выполненная работа: Наименование, если DirServiceJobNomenID = null")]
        public string DirServiceJobNomenName { get; set; }


        [Display(Name = "Цена")]
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

        [Display(Name = "Результат Диагностики")]
        public string DiagnosticRresults { get; set; }

        [Display(Name = "Дата создания записи")]
        public DateTime? TabDate { get; set; }

        [Display(Name = "Статус для результата диагностики")]
        public int? DirSecondHandStatusID { get; set; }

        //Оплата за эту работу
        [Display(Name = "Дата оплата за эту работу")]
        public DateTime? PayDate { get; set; }

        //СЦ
        [Display(Name = "Аппарат пришёл с модуля СЦ")]
        public bool? FromService { get; set; }


        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDataX { get; set; }
    }

    [Table("DocSecondHandPurch2Tabs")]
    public class DocSecondHandPurch2Tab
    {
        [Key]
        public int? DocSecondHandPurch2TabID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int DocSecondHandPurchID { get; set; }
        [ForeignKey("DocSecondHandPurchID")]
        public virtual Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch { get; set; }

        [Display(Name = "Мастер")]
        //[Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }

        [Display(Name = "Выполненная работа")]
        //[Required]
        public int? DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Models.Sklad.Dir.DirNomen dirNomen { get; set; }

        [NotMapped]
        public string DirNomenName { get; set; }

        [Display(Name = "Партия -  с какой партии списываем товар")]
        [Required]
        public int RemPartyID { get; set; }
        [ForeignKey("RemPartyID")]
        public virtual Rem.RemParty remParty { get; set; }

        [Display(Name = "Цена")]
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

        [Display(Name = "Дата создания записи")]
        public DateTime? TabDate { get; set; }

        //Оплата за эту запчасть
        [Display(Name = "Дата оплата за эту работу")]
        public DateTime? PayDate { get; set; }
        //[Display(Name = "ID оплаты в Кассе за эту работу")]
        //public int? DocCashOfficeSumID { get; set; }
        //[Display(Name = "ID оплаты в Банке за эту работу")]
        //public int? DocBankSumID { get; set; }

        //СЦ
        [Display(Name = "Аппарат пришёл с модуля СЦ")]
        public bool? FromService { get; set; }


        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDataX { get; set; }
    }


    [Table("DocSecondHandPurches")]
    public class DocSecondHandPurchSQL
    {
        public int? CountX { get; set; }
    }

    #endregion


    #region DocSecondHandRetails

    //Не используется
    [Table("DocSecondHandRetails")]
    public class DocSecondHandRetail
    {
        [Key]
        public int? DocSecondHandRetailID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        public bool Reserve { get; set; }
        public bool OnCredit { get; set; }



        #region Таблица Doc *** *** *** *** *** *** *** *** *** ***

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
        public int? DirContractorID { get; set; }
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



        #region Rem2PartyMinus

        /*
        [Display(Name = "Контрагент")]
        [NotMapped]
        public int? DirServiceContractorID { get; set; }
        */

        #endregion



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSecondHandRetailTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 66;
        }
    }

    //Не используется
    [Table("DocSecondHandRetailTabs")]
    public class DocSecondHandRetailTab
    {
        [Key]
        public int? DocSecondHandRetailTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int? DocSecondHandRetailID { get; set; }
        [ForeignKey("DocSecondHandRetailID")]
        public virtual DocSecondHandRetail docSecondHandRetail { get; set; }


        [Display(Name = "Документ Прихода, что бы не задавать запрос")]
        [NotMapped]
        public int? DocSecondHandPurchID { get; set; }


        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Партия -  с какой партии списываем товар")]
        [Required]
        public int? Rem2PartyID { get; set; }
        [ForeignKey("Rem2PartyID")]
        public virtual Rem.Rem2Party rem2Party { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Display(Name = "Тип цены")]
        [Required]
        public int DirPriceTypeID { get; set; }
        [ForeignKey("DirPriceTypeID")]
        public virtual Dir.DirPriceType dirPriceType { get; set; }

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

        [Display(Name = "Тип возврата - НЕ НУЖНОЕ ПОЛЕ, нужно только для UNION")]
        public int? DirReturnTypeID { get; set; }
        [Display(Name = "Комментарий: причина возврата - НЕ НУЖНОЕ ПОЛЕ, нужно только для UNION")]
        public int? DirDescriptionID { get; set; }
    }

    //!!! !!! !!!
    [Table("DocSecondHandSales")]
    public class DocSecondHandSale
    {
        [Key]
        public int? DocSecondHandSaleID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Поступление аппарата")]
        //[Required]
        public int? DocSecondHandPurchID { get; set; }
        [ForeignKey("DocSecondHandPurchID")]
        public virtual Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

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

        [Display(Name = "Тип возврата - НЕ НУЖНОЕ ПОЛЕ, нужно только для UNION")]
        public int? DirReturnTypeID { get; set; }
        [Display(Name = "Комментарий: причина возврата - НЕ НУЖНОЕ ПОЛЕ, нужно только для UNION")]
        public int? DirDescriptionID { get; set; }

        [Display(Name = "Гарантия в месяцах")]
        public int? ServiceTypeRepair { get; set; }



        #region Таблица Doc *** *** *** *** *** *** *** *** *** ***

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
        public int? DirContractorID { get; set; }
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



        #region Rem2PartyMinus

        /*
        [Display(Name = "Контрагент")]
        [NotMapped]
        public int? DirServiceContractorID { get; set; }
        */

        #endregion



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSecondHandRetailTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 66;
        }
    }

    #endregion


    #region DocSecondHandRetailReturns

    //Не используется
    [Table("DocSecondHandRetailReturns")]
    public class DocSecondHandRetailReturn
    {
        [Key]
        public int? DocSecondHandRetailReturnID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Партии. А это то что вернули")]
        //[Required]
        public int? DocSecondHandRetailID { get; set; }
        [ForeignKey("DocSecondHandRetailID")]
        public virtual Models.Sklad.Doc.DocSecondHandRetail docRetail { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        public bool Reserve { get; set; }
        public bool OnCredit { get; set; }



        #region Таблица Doc *** *** *** *** *** *** *** *** *** ***

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
        public string recordsDocSecondHandRetailReturnTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 67;
        }
    }

    //Не используется
    [Table("DocSecondHandRetailReturnTabs")]
    public class DocSecondHandRetailReturnTab
    {
        [Key]
        public int? DocSecondHandRetailReturnTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocSecondHandRetailReturnID { get; set; }
        [ForeignKey("DocSecondHandRetailReturnID")]
        public virtual DocSecondHandRetailReturn docSecondHandRetailReturn { get; set; }


        [Display(Name = "Документ Прихода, что бы не задавать запрос")]
        [NotMapped]
        public int? DocSecondHandPurchID { get; set; }


        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Required]
        public double Quantity { get; set; }
        [Display(Name = "Приходная цена")]
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


        [Display(Name = "Партии списание. Тоесть возвращаем списанный товар на склад из ПартииМинус")]
        [Required]
        public int? Rem2PartyMinusID { get; set; }
        [ForeignKey("Rem2PartyMinusID")]
        public virtual Rem.Rem2PartyMinus rem2PartyMinus { get; set; }


        [Display(Name = "Тип возврата")]
        public int? DirReturnTypeID { get; set; }
        [ForeignKey("DirReturnTypeID")]
        public virtual Dir.DirReturnType dirReturnType { get; set; }

        [Display(Name = "Комментарий: причина возврата")]
        public int? DirDescriptionID { get; set; }
        [ForeignKey("DirDescriptionID")]
        public virtual Dir.DirDescription dirDescription { get; set; }

        [NotMapped]
        public string Description { get; set; }

    }

    //!!! !!! !!!
    [Table("DocSecondHandReturns")]
    public class DocSecondHandReturn
    {
        [Key]
        public int? DocSecondHandReturnID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "С какой продажи вернули")]
        //[Required]
        public int? DocSecondHandSaleID { get; set; }
        [ForeignKey("DocSecondHandSaleID")]
        public virtual Models.Sklad.Doc.DocSecondHandSale docSecondHandSale { get; set; }

        [Display(Name = "Поступление аппарата")]
        //[Required]
        public int? DocSecondHandPurchID { get; set; }
        [ForeignKey("DocSecondHandPurchID")]
        public virtual Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

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

        [Display(Name = "Тип возврата")]
        public int? DirReturnTypeID { get; set; }
        [ForeignKey("DirReturnTypeID")]
        public virtual Dir.DirReturnType dirReturnType { get; set; }

        [Display(Name = "Комментарий: причина возврата")]
        public int? DirDescriptionID { get; set; }
        [ForeignKey("DirDescriptionID")]
        public virtual Dir.DirDescription dirDescription { get; set; }

        //[NotMapped]
        //public string Description { get; set; }



        #region Таблица Doc *** *** *** *** *** *** *** *** *** ***

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
        public int? DirContractorID { get; set; }
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
        public string recordsDocSecondHandRetailReturnTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirContractorID == 0) DirContractorID = DirContractorIDOrg;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 67;
        }
    }

    #endregion


    #region DocSecondHandRetailActWriteOffs

    [Table("DocSecondHandRetailActWriteOffs")]
    public class DocSecondHandRetailActWriteOff
    {
        [Key]
        public int? DocSecondHandRetailActWriteOffID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        public bool Reserve { get; set; }



        #region Таблица Doc *** *** *** *** *** *** *** *** *** ***

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
        public int? DirContractorID { get; set; }
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

        #endregion


        #region Rem2PartyMinus

        /*
        [Display(Name = "Контрагент")]
        [NotMapped]
        public int? DirServiceContractorID { get; set; }
        */

        #endregion


        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSecondHandRetailActWriteOffTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 68;
        }
    }

    [Table("DocSecondHandRetailActWriteOffTabs")]
    public class DocSecondHandRetailActWriteOffTab
    {
        [Key]
        public int? DocSecondHandRetailActWriteOffTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int? DocSecondHandRetailActWriteOffID { get; set; }
        [ForeignKey("DocSecondHandRetailActWriteOffID")]
        public virtual DocSecondHandRetailActWriteOff docSecondHandRetailActWriteOff { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Партия -  с какой партии списываем товар")]
        [Required]
        public int? Rem2PartyID { get; set; }
        [ForeignKey("Rem2PartyID")]
        public virtual Rem.Rem2Party rem2Party { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Display(Name = "Тип цены")]
        [Required]
        public int DirPriceTypeID { get; set; }
        [ForeignKey("DirPriceTypeID")]
        public virtual Dir.DirPriceType dirPriceType { get; set; }

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

        [Display(Name = "Тип возврата")]
        public int? DirReturnTypeID { get; set; }
        [ForeignKey("DirReturnTypeID")]
        public virtual Dir.DirReturnType dirReturnType { get; set; }

        [Display(Name = "Комментарий: причина возврата")]
        public int? DirDescriptionID { get; set; }
        [ForeignKey("DirDescriptionID")]
        public virtual Dir.DirDescription dirDescription { get; set; }

        [NotMapped]
        public string Description { get; set; }
    }

    #endregion


    #region DocSecondHandMovement

    //Не используется
    [Table("DocSecondHandMovements")]
    public class DocSecondHandMovement
    {
        [Key]
        public int? DocSecondHandMovementID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад с")]
        [Required]
        public int DirWarehouseIDFrom { get; set; }
        [ForeignKey("DirWarehouseIDFrom")]
        public virtual Dir.DirWarehouse dirWarehouseFrom { get; set; }

        [Display(Name = "Склад на")]
        [Required]
        public int DirWarehouseIDTo { get; set; }
        [ForeignKey("DirWarehouseIDTo")]
        public virtual Dir.DirWarehouse dirWarehouseTo { get; set; }

        public bool Reserve { get; set; }

        [Display(Name = "Комментарий: причина ")]
        public int? DirMovementDescriptionID { get; set; }
        [ForeignKey("DirMovementDescriptionID")]
        public virtual Dir.DirMovementDescription dirMovementDescription { get; set; }

        [NotMapped]
        public string DescriptionMovement { get; set; }

        [Display(Name = "Курьер")]
        public int? DirEmployeeIDCourier { get; set; }
        [ForeignKey("DirEmployeeIDCourier")]
        public virtual Dir.DirEmployee dirEmployee_Courier { get; set; }


        //1 Перемещение
        //2	Логистика: в ожидании курьера
        //3	Логистика: курьер принял
        //4	Логистика: курьер отдал
        [Display(Name = "Статус Логистики")]
        //[Required]
        public int? DirMovementStatusID { get; set; }
        [ForeignKey("DirMovementStatusID")]
        public virtual Dir.DirMovementStatus dirMovementStatus { get; set; }



        //Таблица Doc *** *** *** *** *** *** *** *** *** ***
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



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSecondHandMovementTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 33;
        }
    }

    //!!! !!! !!!
    [Table("DocSecondHandMovs")]
    public class DocSecondHandMov
    {
        [Key]
        public int? DocSecondHandMovID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад с")]
        [Required]
        public int DirWarehouseIDFrom { get; set; }
        [ForeignKey("DirWarehouseIDFrom")]
        public virtual Dir.DirWarehouse dirWarehouseFrom { get; set; }

        [Display(Name = "Склад на")]
        [Required]
        public int DirWarehouseIDTo { get; set; }
        [ForeignKey("DirWarehouseIDTo")]
        public virtual Dir.DirWarehouse dirWarehouseTo { get; set; }

        public bool Reserve { get; set; }

        [Display(Name = "Комментарий: причина ")]
        public int? DirMovementDescriptionID { get; set; }
        [ForeignKey("DirMovementDescriptionID")]
        public virtual Dir.DirMovementDescription dirMovementDescription { get; set; }

        [NotMapped]
        public string DescriptionMovement { get; set; }

        [Display(Name = "Курьер")]
        public int? DirEmployeeIDCourier { get; set; }
        [ForeignKey("DirEmployeeIDCourier")]
        public virtual Dir.DirEmployee dirEmployee_Courier { get; set; }


        //1 Перемещение
        //2	Логистика: в ожидании курьера
        //3	Логистика: курьер принял
        //4	Логистика: курьер отдал
        [Display(Name = "Статус Логистики")]
        //[Required]
        public int? DirMovementStatusID { get; set; }
        [ForeignKey("DirMovementStatusID")]
        public virtual Dir.DirMovementStatus dirMovementStatus { get; set; }



        //Таблица Doc *** *** *** *** *** *** *** *** *** ***
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

        [Display(Name = "ВыпадающийСписок: 'Загрузить аппараты с': [1.Продажа, 2.Продажа + ППП На разбор]")]
        public int LoadFrom { get; set; }



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSecondHandMovTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 33;
        }
    }

    //Не используется
    [Table("DocSecondHandMovementTabs")]
    public class DocSecondHandMovementTab
    {
        [Key]
        public int? DocSecondHandMovementTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocSecondHandMovementID { get; set; }
        [ForeignKey("DocSecondHandMovementID")]
        public virtual DocSecondHandMovement docSecondHandMovement { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Партия -  с какой партии списываем товар")]
        [Required]
        public int Rem2PartyID { get; set; }
        [ForeignKey("Rem2PartyID")]
        public virtual Rem.Rem2Party rem2Party { get; set; }

        [Required]
        public double Quantity { get; set; }
        [Display(Name = "Приходная цена")]
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

        [Required]
        public double PriceRetailVAT { get; set; }
        [NotMapped]
        public double MarkupRetail { get; set; }
        [Required]
        public double PriceRetailCurrency { get; set; }

        [Required]
        public double PriceWholesaleVAT { get; set; }
        [NotMapped]
        public double MarkupWholesale { get; set; }
        [Required]
        public double PriceWholesaleCurrency { get; set; }

        [Required]
        public double PriceIMVAT { get; set; }
        [NotMapped]
        public double MarkupIM { get; set; }
        [Required]
        public double PriceIMCurrency { get; set; }




        [Display(Name = "Тип возврата")]
        public int? DirReturnTypeID { get; set; }
        [ForeignKey("DirReturnTypeID")]
        public virtual Dir.DirReturnType dirReturnType { get; set; }

        [Display(Name = "Комментарий: причина возврата")]
        public int? DirDescriptionID { get; set; }
        [ForeignKey("DirDescriptionID")]
        public virtual Dir.DirDescription dirDescription { get; set; }



        //Для RemParties *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        [Display(Name = "Склад на который пришла партия первоначально")]
        [NotMapped]
        public int DirWarehouseIDPurch { get; set; }

        //[Display(Name = "Поставщика от которого пришла партия первоначально")]
        //[NotMapped]
        //public int DirContractorID { get; set; }
    }

    //!!! !!! !!!
    [Table("DocSecondHandMovTabs")]
    public class DocSecondHandMovTab
    {
        [Key]
        public int? DocSecondHandMovTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocSecondHandMovID { get; set; }
        [ForeignKey("DocSecondHandMovID")]
        public virtual DocSecondHandMov docSecondHandMov { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Поступление аппарата")]
        //[Required]
        public int? DocSecondHandPurchID { get; set; }
        [ForeignKey("DocSecondHandPurchID")]
        public virtual Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch { get; set; }

        [Display(Name = "Приходная цена")]
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

        [Required]
        public double PriceRetailVAT { get; set; }
        [NotMapped]
        public double MarkupRetail { get; set; }
        [Required]
        public double PriceRetailCurrency { get; set; }

        [Required]
        public double PriceWholesaleVAT { get; set; }
        [NotMapped]
        public double MarkupWholesale { get; set; }
        [Required]
        public double PriceWholesaleCurrency { get; set; }

        [Required]
        public double PriceIMVAT { get; set; }
        [NotMapped]
        public double MarkupIM { get; set; }
        [Required]
        public double PriceIMCurrency { get; set; }




        [Display(Name = "Тип возврата")]
        public int? DirReturnTypeID { get; set; }
        [ForeignKey("DirReturnTypeID")]
        public virtual Dir.DirReturnType dirReturnType { get; set; }

        [Display(Name = "Комментарий: причина возврата")]
        public int? DirDescriptionID { get; set; }
        [ForeignKey("DirDescriptionID")]
        public virtual Dir.DirDescription dirDescription { get; set; }



        //Для RemParties *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        [Display(Name = "Склад на который пришла партия первоначально")]
        [NotMapped]
        public int DirWarehouseIDPurch { get; set; }

        //[Display(Name = "Поставщика от которого пришла партия первоначально")]
        //[NotMapped]
        //public int DirContractorID { get; set; }
    }

    #endregion


    #region DocSecondHandInventories

    //Не используется
    [Table("DocSecondHandInventories")]
    public class DocSecondHandInventory
    {
        [Key]
        public int? DocSecondHandInventoryID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }


        [Display(Name = "ВыпадающийСписок: 'Списывать с ЗП': [1.Точка, 2.Сотрудник]")]
        public int SpisatS { get; set; }

        [Display(Name = "ВыпадающийСписок: 'Сотрудники' (Если выбрали Сотрудник)")]
        public int? SpisatSDirEmployeeID { get; set; }
        //[ForeignKey("SpisatSDirEmployeeID")]
        //public virtual Dir.DirEmployee spisatSDirEmployeeID { get; set; }



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

        [NotMapped]
        public DateTime? DocDateHeld { get; set; }
        [NotMapped]
        public DateTime? DocDatePayment { get; set; }

        #endregion



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSecondHandInventoryTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 76;
        }

    }

    //!!! !!! !!!
    [Table("DocSecondHandInvs")]
    public class DocSecondHandInv
    {
        [Key]
        public int? DocSecondHandInvID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }


        [Display(Name = "ВыпадающийСписок: 'Загрузить аппараты с': [1.Продажа, 2.Продажа + ППП На разбор]")]
        public int LoadFrom { get; set; }


        [Display(Name = "ВыпадающийСписок: 'Списывать с ЗП': [1.Точка, 2.Сотрудник]")]
        public int SpisatS { get; set; }

        [Display(Name = "ВыпадающийСписок: 'Сотрудники' (Если выбрали Сотрудник)")]
        public int? SpisatSDirEmployeeID { get; set; }
        //[ForeignKey("SpisatSDirEmployeeID")]
        //public virtual Dir.DirEmployee spisatSDirEmployee { get; set; }



        //Подписи === === === === ===

        [Display(Name = "Товаровед")]
        public int DirEmployee1ID { get; set; }
        //[ForeignKey("DirEmployee1ID")]
        //public virtual Dir.DirEmployee dirEmployee1 { get; set; }

        [Display(Name = "Подпись Товароведа")]
        public bool? DirEmployee1Podpis { get; set; }

        [Display(Name = "Админ точки")]
        public int? DirEmployee2ID { get; set; }
        //[ForeignKey("DirEmployee2ID")]
        //public virtual Dir.DirEmployee dirEmployee2 { get; set; }

        [Display(Name = "Подпись Админа точки")]
        public bool? DirEmployee2Podpis { get; set; }

        //=== === === === ===




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

        [NotMapped]
        public DateTime? DocDateHeld { get; set; }
        [NotMapped]
        public DateTime? DocDatePayment { get; set; }

        #endregion



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDocSecondHandInvTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            //if (String.IsNullOrEmpty(DirContractorLegalName)) DirContractorLegalName = DirContractorName;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 76;
        }

    }

    //Не используется
    [Table("DocSecondHandInventoryTabs")]
    public class DocSecondHandInventoryTab
    {
        [Key]
        public int? DocSecondHandInventoryTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocSecondHandInventoryID { get; set; }
        [ForeignKey("DocSecondHandInventoryID")]
        public virtual DocSecondHandInventory docSecondHandInventory { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Партия -  с какой партии списываем товар")]
        //[Required]
        public int? Rem2PartyID { get; set; }
        //[ForeignKey("RemPartyID")]
        //public virtual Rem.RemParty remParty { get; set; }

        [Required]
        public double Quantity { get; set; }

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

        //public bool Exist { get; set; }



        [Display(Name = "Присутствует, Списывается с ЗП, На разбор")]
        [Required]
        public int Exist { get; set; }



        [Display(Name = "!!! Сука Глюк !!!")]
        [NotMapped]
        public string ExistName { get; set; }

    }

    //!!! !!! !!!
    [Table("DocSecondHandInvTabs")]
    public class DocSecondHandInvTab
    {
        [Key]
        public int? DocSecondHandInvTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocSecondHandInvID { get; set; }
        [ForeignKey("DocSecondHandInvID")]
        public virtual DocSecondHandInv docSecondHandInv { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Поступление аппарата")]
        //[Required]
        public int? DocSecondHandPurchID { get; set; }
        [ForeignKey("DocSecondHandPurchID")]
        public virtual Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch { get; set; }

        //[Required]
        //public double Quantity { get; set; }

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

        //public bool Exist { get; set; }



        [Display(Name = "Присутствует, Списывается с ЗП, На разбор")]
        [Required]
        public int Exist { get; set; }

        [Display(Name = "!!! Сука Глюк !!!")]
        [NotMapped]
        public string ExistName { get; set; }


        [Display(Name = "Статус: статус аппарата до поведения инвентаризации, что бы при отмене проведения Инв. поменять на предыдущий статус")]
        //[Required]
        public int? DirSecondHandStatusID { get; set; }
        [ForeignKey("DirSecondHandStatusID")]
        public virtual Dir.DirSecondHandStatus dirSecondHandStatus { get; set; }

    }

    #endregion



    #region DocSecondHandRazbors

    //Не используется
    [Table("DocSecondHandRazbors")]
    public class DocSecondHandRazbor
    {
        [Key]
        public int? DocSecondHandRazborID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int? DocID { get; set; }
        [ForeignKey("DocID")]
        public virtual Models.Sklad.Doc.Doc doc { get; set; }

        [Display(Name = "docSecondHandInventory.DocID")]
        [Required]
        public int? DocIDFrom { get; set; }
        [ForeignKey("DocIDFrom")]
        public virtual Models.Sklad.Doc.Doc docFrom { get; set; }




        //!!! Важно !!!
        //Поступление на разбор может быть и из БУ и из СЦ
        [Display(Name = "ListObjects.ListObjectID - тип документа: БУ(65) или СЦ(40)")]
        [Required]
        public int? ListObjectIDFromType { get; set; }
        //[ForeignKey("ListObjectIDFromType")]
        //public virtual Models.Sklad.List.ListObject listObjectIDFromType { get; set; }

        [Display(Name = "Docs.DocID - по DocID можно вычислить ID-шник документа 'Docs.NumberReal'")]
        [Required]
        public int? DocIDFromType { get; set; }
        //[ForeignKey("DocIDFromType")]
        //public virtual Models.Sklad.Doc.Doc docIDFromType { get; set; }




        [Display(Name = "Склад")]
        [Required]
        public int DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Статус")]
        //[Required]
        public int? DirSecondHandStatusID { get; set; }
        [ForeignKey("DirSecondHandStatusID")]
        public virtual Dir.DirSecondHandStatus dirSecondHandStatus { get; set; }

        [Display(Name = "Статус: Готов или Отказ (заполняется при выдаче)")]
        //[Required]
        public int? DirSecondHandStatusID_789 { get; set; }
        [ForeignKey("DirSecondHandStatusID_789")]
        public virtual Dir.DirSecondHandStatus dirSecondHandStatus_789 { get; set; }
        

        //Цены
        [Display(Name = "Сумма аппарата (сделки + запчасти)")]
        public double PriceVAT { get; set; }
        [Display(Name = "Сумма  аппарата (сделки + запчасти) в тек.валюте")]
        public double? PriceCurrency { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }
        
        //Устанавливается при приёмке Аппарата
        /*[Display(Name = "Дата готовности")]
        [Required]
        public DateTime DateDone { get; set; }*/

        [Display(Name = "Мастер")]
        //[Required]
        public int? DirEmployeeIDMaster { get; set; }
        [ForeignKey("DirEmployeeIDMaster")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }
        
        //До 04.06.2017 == IssuanceDate
        [Display(Name = "Дата смены статуса")]
        public DateTime? DateStatusChange { get; set; }

        //[Display(Name = "Если вернули на доработку, то запомнить первичную дату поступления аппарта")]
        //public DateTime? DocDate_First { get; set; }


        //!!! Дополнительно !!!
        //1. С какой партии списываем (перемещение на Локальный склад, под-склад)
        //   В партии содержатся все необходимые параметры: 
        //   DocID - документ который создал партию (или DocSecondHandPurches или DocSecondHandMovements)
        //   DocIDFirst - документ создания аппарата (только DocSecondHandPurches)
        [Display(Name = "Партия - с какой партии списываем товар (перемещение c Локального склада на под-склад)")]
        //[Required]
        public int? Rem2PartyID { get; set; }
        [ForeignKey("Rem2PartyID")]
        public virtual Rem.Rem2Party rem2Party { get; set; }

        public double SumsDirNomen { get; set; }



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
        public string DocSecondHandRazborTab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            if (DirSecondHandStatusID == null) DirSecondHandStatusID = 1;
            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 77;
        }
    }

    //Не используется
    [Table("DocSecondHandRazborTabs")]
    public class DocSecondHandRazborTab
    {
        [Key]
        public int? DocSecondHandRazborTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocSecondHandRazborID { get; set; }
        [ForeignKey("DocSecondHandRazborID")]
        public virtual DocSecondHandRazbor docSecondHandRazbor { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }


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
        [Display(Name = "Поставщик")]
        public int? DirContractorID { get; set; }
        [ForeignKey("DirContractorID")]
        public virtual Dir.DirContractor dirContractor { get; set; }

        [Display(Name = "Текстура")]
        public int? DirCharTextureID { get; set; }
        [ForeignKey("DirCharTextureID")]
        public virtual Dir.DirCharTexture dirCharTexture { get; set; }
        //Характеристики *** *** *** *** *** *** *** ***



        public string SerialNumber { get; set; }
        public string Barcode { get; set; }



        [Required]
        public double Quantity { get; set; }
        [Display(Name = "Приходная цена")]
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

        [Required]
        public double PriceRetailVAT { get; set; }
        [NotMapped]
        public double MarkupRetail { get; set; }
        [Required]
        public double PriceRetailCurrency { get; set; }

        [Required]
        public double PriceWholesaleVAT { get; set; }
        [NotMapped]
        public double MarkupWholesale { get; set; }
        [Required]
        public double PriceWholesaleCurrency { get; set; }

        [Required]
        public double PriceIMVAT { get; set; }
        [NotMapped]
        public double MarkupIM { get; set; }
        [Required]
        public double PriceIMCurrency { get; set; }

        [Display(Name = "Минимальный остаток")]
        [Required]
        public double DirNomenMinimumBalance { get; set; }
    }

    [Table("DocSecondHandRazbor2Tabs")]
    public class DocSecondHandRazbor2Tab
    {
        [Key]
        public int? DocSecondHandRazbor2TabID { get; set; }

        [Display(Name = "Поступление аппарата")]
        //[Required]
        public int? DocSecondHandPurchID { get; set; }
        [ForeignKey("DocSecondHandPurchID")]
        public virtual Models.Sklad.Doc.DocSecondHandPurch docSecondHandPurch { get; set; }




        [Display(Name = "Товар")]
        //[Required]
        public int? DirNomen1ID { get; set; }
        [ForeignKey("DirNomen1ID")]
        public virtual Dir.DirNomen dirNomen1 { get; set; }

        [Display(Name = "Товар")]
        //[Required]
        public int? DirNomen2ID { get; set; }
        [ForeignKey("DirNomen2ID")]
        public virtual Dir.DirNomen dirNomen2 { get; set; }

        [Display(Name = "Тип товара")]
        public int? DirNomenCategoryID { get; set; }
        [ForeignKey("DirNomenCategoryID")]
        public virtual Dir.DirNomenCategory dirNomenCategory { get; set; }

        [NotMapped]
        public string DirNomenCategoryName { get; set; }


        [Display(Name = "Товар")]
        //[Required]
        public int? DirNomenID { get; set; }
        [ForeignKey("DirNomenID")]
        public virtual Dir.DirNomen dirNomen { get; set; }



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
        [Display(Name = "Поставщик")]
        public int? DirContractorID { get; set; }
        [ForeignKey("DirContractorID")]
        public virtual Dir.DirContractor dirContractor { get; set; }

        [Display(Name = "Текстура")]
        public int? DirCharTextureID { get; set; }
        [ForeignKey("DirCharTextureID")]
        public virtual Dir.DirCharTexture dirCharTexture { get; set; }
        //Характеристики *** *** *** *** *** *** *** ***



        public string SerialNumber { get; set; }
        public string Barcode { get; set; }



        [Required]
        public double Quantity { get; set; }
        [Display(Name = "Приходная цена")]
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

        [Required]
        public double PriceRetailVAT { get; set; }
        [NotMapped]
        public double MarkupRetail { get; set; }
        [Required]
        public double PriceRetailCurrency { get; set; }

        [Required]
        public double PriceWholesaleVAT { get; set; }
        [NotMapped]
        public double MarkupWholesale { get; set; }
        [Required]
        public double PriceWholesaleCurrency { get; set; }

        [Required]
        public double PriceIMVAT { get; set; }
        [NotMapped]
        public double MarkupIM { get; set; }
        [Required]
        public double PriceIMCurrency { get; set; }

        [Display(Name = "Минимальный остаток")]
        [Required]
        public double DirNomenMinimumBalance { get; set; }
    }

    #endregion

}
