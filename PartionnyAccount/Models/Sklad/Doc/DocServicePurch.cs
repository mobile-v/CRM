using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Doc
{
    #region DocServicePurches

    [Table("DocServicePurches")]
    public class DocServicePurch
    {
        [Key]
        public int? DocServicePurchID { get; set; }

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

        [Display(Name = "Склад куда поступил аппарат")]
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
        public int? DirServiceStatusID { get; set; }
        [ForeignKey("DirServiceStatusID")]
        public virtual Dir.DirServiceStatus dirServiceStatus { get; set; }

        [Display(Name = "Статус: Готов или Отказ (заполняется при выдаче)")]
        //[Required]
        public int? DirServiceStatusID_789 { get; set; }
        [ForeignKey("DirServiceStatusID_789")]
        public virtual Dir.DirServiceStatus dirServiceStatus_789 { get; set; }

        [Display(Name = "Серийный номер")]
        public bool? SerialNumberNo { get; set; }
        public string SerialNumber { get; set; }
        [Display(Name = "Вид ремонта: Не гарантийный, гарантийный")]
        public bool? TypeRepair { get; set; }
        [Display(Name = "Аппарат")]
        public bool? ComponentDevice { get; set; }
        [Display(Name = "Аккумулятор")]
        public bool? ComponentBattery { get; set; }
        [Display(Name = "Аккумулятор: Серийник")]
        public string ComponentBatterySerial { get; set; }
        [Display(Name = "Задняя крышка")]
        public bool? ComponentBackCover { get; set; }
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

        //Клиент
        [Display(Name = "Постоянный Клиент")]
        public bool? DirServiceContractorRegular { get; set; }

        [Display(Name = "Выбор из справочника Контрагентов сервиса")]
        //[Required]
        public int? DirServiceContractorID { get; set; }
        [ForeignKey("DirServiceContractorID")]
        public virtual Dir.DirServiceContractor dirServiceContractor { get; set; }

        [Display(Name = "Поле для ввода имени Клиента (он же Контрагент)")]
        public string DirServiceContractorName { get; set; }
        [Display(Name = "Адрес (из справочника, если постоянный)")]
        public string DirServiceContractorAddress { get; set; }
        [Display(Name = "Телефон (из справочника, если постоянный)")]
        public string DirServiceContractorPhone { get; set; }
        [Display(Name = "Мейл (из справочника, если постоянный)")]
        public string DirServiceContractorEmail { get; set; }

        //Цены
        [Display(Name = "Ориентир.стоимость")]
        public string PriceVAT { get; set; }

        [Display(Name = "Валюта")]
        [Required]
        public int DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        public double DirCurrencyRate { get; set; }
        public int DirCurrencyMultiplicity { get; set; }

        //Устанавливается при приёмке Аппарата
        [Display(Name = "Дата готовности")]
        [Required]
        public DateTime DateDone { get; set; }

        //Дополнительные опции
        [Display(Name = "Срочный ремонт")]
        public bool? UrgentRepairs { get; set; }
        [Display(Name = "Предоплата")]
        public bool? Prepayment { get; set; }

        [Display(Name = "Поле для ввода суммы 'Предоплаты'")]
        public double PrepaymentSum { get; set; }
        [Display(Name = "Если вернули из Архива по гарантии")]
        public double? PrepaymentSum_1 { get; set; }
        public double? PrepaymentSum_2 { get; set; }
        public double? PrepaymentSum_3 { get; set; }
        public double? PrepaymentSum_4 { get; set; }
        public double? PrepaymentSum_5 { get; set; }

        [Display(Name = "Поле для ввода суммы 'Итого разница'. Это та сумма, которую надо заплатить клиенту за ремонт. Высчитывается у клиента (или в запросе SELECT One) и возвращается на сервер.")]
        [NotMapped]
        public double? SumTotal2 { get; set; }

        [Display(Name = "Мастер")]
        //[Required]
        public int? DirEmployeeIDMaster { get; set; }
        [ForeignKey("DirEmployeeIDMaster")]
        public virtual PartionnyAccount.Models.Sklad.Dir.DirEmployee dirEmployee { get; set; }

        [Display(Name = "Гарантия в месяцах")]
        public int ServiceTypeRepair { get; set; }

        [Display(Name = "Вернули по гарантии")]
        public bool? FromGuarantee { get; set; }
        [Display(Name = "Сколько раз вернули по гарантии")]
        public int? FromGuaranteeCount { get; set; }

        [Display(Name = "Сумма: что бы не подсчитывать каждый раз сумму")]
        public double? Summ_NotPre { get; set; }

        [Display(Name = "Типичные неисправности")]
        public bool? DirServiceNomenTypicalFaultID1 { get; set; }
        public bool? DirServiceNomenTypicalFaultID2 { get; set; }
        public bool? DirServiceNomenTypicalFaultID3 { get; set; }
        public bool? DirServiceNomenTypicalFaultID4 { get; set; }
        public bool? DirServiceNomenTypicalFaultID5 { get; set; }
        public bool? DirServiceNomenTypicalFaultID6 { get; set; }
        public bool? DirServiceNomenTypicalFaultID7 { get; set; }

        [Display(Name = "Не оповещён или Оповещён")]
        public int? AlertedCount { get; set; }
        public DateTime? AlertedDate { get; set; }
        public string AlertedDateTxt { get; set; }

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

        [Display(Name = "Отправили аппарат в модуль БУ")]
        public bool? InSecondHand { get; set; }

        [Display(Name = "Выполненные работы")]
        public double? DiscountX { get; set; }
        [Display(Name = "Запчасти")]
        public double? DiscountY { get; set; }





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
        public string recordsDocServicePurch1Tab { get; set; }
        [NotMapped]
        public string recordsDocServicePurch2Tab { get; set; }



        //Заполняем пустные поля (которые не должны быть пустыми)
        public void Substitute()
        {
            //Заполняем пустные поля
            if (TypeRepair == null) TypeRepair = false;
            if (ComponentDevice == null) ComponentDevice = false;
            if (ComponentBattery == null) { ComponentBattery = false; ComponentBatterySerial = null; }
            if (ComponentBackCover == null) ComponentBackCover = false;
            if (ComponentPasTextNo == true) { ComponentPasText = null; }
            if (DirServiceContractorRegular == null) { DirServiceContractorRegular = false; /*DirServiceContractorID = null;*/ }
            if (UrgentRepairs == null) UrgentRepairs = false;
            if (Prepayment == null) Prepayment = false;
            if (DirServiceStatusID == null) DirServiceStatusID = 1;
            //if (PriceVAT == null) { PriceVAT = 0; PriceCurrency = 0; }
            //if (PriceCurrency == null) { PriceCurrency = 0; PriceVAT = 0; }
            if (FromGuarantee == null) { FromGuarantee = false; }
            if (DiscountX == null) { DiscountX = 0; }
            if (DiscountY == null) { DiscountY = 0; }

            if (DirPaymentTypeID == null) DirPaymentTypeID = 1;
            if (Payment == null) Payment = 0;
            ListObjectID = 40;
        }
    }

    [Table("DocServicePurch1Tabs")]
    public class DocServicePurch1Tab
    {
        [Key]
        public int? DocServicePurch1TabID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int DocServicePurchID { get; set; }
        [ForeignKey("DocServicePurchID")]
        public virtual Models.Sklad.Doc.DocServicePurch docServicePurch { get; set; }

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
        public int? DirServiceStatusID { get; set; }



        //Оплата за эту работу
        [Display(Name = "Дата оплата за эту работу")]
        public DateTime? PayDate { get; set; }
        [Display(Name = "ID оплаты в Кассе за эту работу")]
        public int? DocCashOfficeSumID { get; set; }
        [Display(Name = "ID оплаты в Банке за эту работу")]
        public int? DocBankSumID { get; set; }

        //Аппарат могут ремонтиорвать несколько раз, каждый ремонт надо пронумеровать
        [Display(Name = "Номер ремонта")]
        public int? RemontN { get; set; }



        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDataX { get; set; }
    }

    [Table("DocServicePurch2Tabs")]
    public class DocServicePurch2Tab
    {
        [Key]
        public int? DocServicePurch2TabID { get; set; }

        [Display(Name = "Документ")]
        //[Required]
        public int DocServicePurchID { get; set; }
        [ForeignKey("DocServicePurchID")]
        public virtual Models.Sklad.Doc.DocServicePurch docServicePurch { get; set; }

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
        [Display(Name = "ID оплаты в Кассе за эту работу")]
        public int? DocCashOfficeSumID { get; set; }
        [Display(Name = "ID оплаты в Банке за эту работу")]
        public int? DocBankSumID { get; set; }

        //Аппарат могут ремонтиорвать несколько раз, каждый ремонт надо пронумеровать
        [Display(Name = "Номер ремонта")]
        public int? RemontN { get; set; }


        //Табличные данные *** *** *** *** *** *** *** *** *** ***
        [NotMapped]
        public string recordsDataX { get; set; }
    }



    [Table("DocServicePurches")]
    public class DocServicePurchSQL
    {
        public int? CountX { get; set; }
    }

    #endregion


    #region DocServiceMovement

    [Table("DocServiceMovs")]
    public class DocServiceMov
    {
        [Key]
        public int? DocServiceMovID { get; set; }

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
        public string recordsDocServiceMovTab { get; set; }



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

    [Table("DocServiceMovTabs")]
    public class DocServiceMovTab
    {
        [Key]
        public int? DocServiceMovTabID { get; set; }

        [Display(Name = "Документ")]
        [Required]
        public int DocServiceMovID { get; set; }
        [ForeignKey("DocServiceMovID")]
        public virtual DocServiceMov docServiceMov { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirServiceNomenID { get; set; }
        [ForeignKey("DirServiceNomenID")]
        public virtual Dir.DirServiceNomen dirServiceNomen { get; set; }

        [Display(Name = "Поступление аппарата")]
        //[Required]
        public int? DocServicePurchID { get; set; }
        [ForeignKey("DocServicePurchID")]
        public virtual Models.Sklad.Doc.DocServicePurch docServicePurch { get; set; }

        [Display(Name = "Статус")]
        //[Required]
        public int? DirServiceStatusID { get; set; }
        [ForeignKey("DirServiceStatusID")]
        public virtual Dir.DirServiceStatus dirServiceStatus { get; set; }

        [Display(Name = "Статус: Готов или Отказ (заполняется при выдаче)")]
        //[Required]
        public int? DirServiceStatusID_789 { get; set; }
        [ForeignKey("DirServiceStatusID_789")]
        public virtual Dir.DirServiceStatus dirServiceStatus_789 { get; set; }

    }

    #endregion

}