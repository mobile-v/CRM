using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartionnyAccount.Models.Sklad.Dir
{
    [Table("DirEmployees")]
    public class DirEmployee
    {
        #region Основное

        [Key]
        public int? DirEmployeeID { get; set; }
        public bool Del { get; set; }
        public bool SysRecord { get; set; }

        [Display(Name = "Имя")]
        [Required]
        public string DirEmployeeName { get; set; }

        public bool? DirEmployeeActive { get; set; }
        [Display(Name = "Логин")]
        public string DirEmployeeLogin { get; set; }
        [Display(Name = "Пароль")]
        public string DirEmployeePswd { get; set; }
        [Display(Name = "Организация: привязка к Организации")]
        public int? DirContractorIDOrg { get; set; }

        public string DateHire { get; set; }
        public string DateTermination { get; set; }
        public string Phone { get; set; }

        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string PassportIssued { get; set; }
        public string PassportPinCode { get; set; }
        public string PassportIssuedDate { get; set; }

        public string DirEmployeeDesc { get; set; }
        public string ImageLink { get; set; }
        [StringLength(1024)]
        public string Address { get; set; }
        [StringLength(1024)]
        public string EMail { get; set; }


        // === Зарплата ===
        [Display(Name = "Валюта")]
        public int? DirCurrencyID { get; set; }
        [ForeignKey("DirCurrencyID")]
        public virtual Dir.DirCurrency dirCurrency { get; set; }

        [Display(Name = "ЗП за X работы")]
        public double? Salary { get; set; }
        [Display(Name = "ЗП за день или месяц работы")]
        public int? SalaryDayMonthly { get; set; }

        [Display(Name = "Премия для продавца")]
        public int? DirBonusID { get; set; }
        [ForeignKey("DirBonusID")]
        public virtual Dir.DirBonus dirBonus { get; set; }
        [Display(Name = "Продажи: фиксированный оклад за месяц")] // + Дневной оклад
        public double? SalaryFixedSalesMount { get; set; }


        // === Мастерская ===
        //Глюк: нельзя ставить вконце цифру!!! Только в середине текста!!!
        [Display(Name = "Премия для мастера")]
        public int? DirBonus2ID { get; set; }
        [ForeignKey("DirBonus2ID")]
        public virtual Dir.DirBonus dirBonus2 { get; set; }
        [Display(Name = "Ремонт: фиксированной суммы с каждого ремонта, скажем 300 руб")]
        public double? SalaryFixedServiceOne { get; set; }


        [Display(Name = "ЗП: Работы.Тип")]
        public int? SalaryPercentService1TabsType { get; set; }
        public double SalaryPercentService1Tabs { get; set; }

        [Display(Name = "ЗП: Запчасти.Тип")]
        public int? SalaryPercentService2TabsType { get; set; }
        public double SalaryPercentService2Tabs { get; set; }



        // === Б/У ===

        //1. Ремонт
        [Display(Name = "Начислить за ремонт после продажи")]
        public bool SalarySecondHandWorkshopCheck { get; set; }

        [Display(Name = "Премия для мастера Б/У")]
        public int? DirBonus3ID { get; set; }
        [ForeignKey("DirBonus3ID")]
        public virtual Dir.DirBonus dirBonus3 { get; set; }
        [Display(Name = "Б/У: фиксированная суммы с каждого ремонта, скажем 300 руб")]
        public double? SalaryFixedSecondHandWorkshopOne { get; set; }

        //2. Продажа
        [Display(Name = "Премия для продавца Б/У")]
        public int? DirBonus4ID { get; set; }
        [ForeignKey("DirBonus4ID")]
        public virtual Dir.DirBonus dirBonus4 { get; set; }
        [Display(Name = "Б/У: фиксированная суммы с каждой продажи, скажем 300 руб")]
        public double? SalaryFixedSecondHandRetailOne { get; set; }



        //DirDiscountTabs
        [NotMapped]
        public string recordsDirEmployeeWarehouses { get; set; }

        #endregion



        #region Права доступа === === === === === === === === === === === === ===


        #region Настройки *** *** ***

        [Display(Name = "Настройки")]
        public bool? RightSysSettings0 { get; set; }

        [Display(Name = "Ваши фирмы")]
        public int? RightMyCompany { get; set; }
        public bool? RightMyCompanyCheck { get; set; }
        [Display(Name = "Сотрудники")]
        public int? RightDirEmployees { get; set; }
        public bool? RightDirEmployeesCheck { get; set; }
        [Display(Name = "Настройки")]
        public int? RightSysSettings { get; set; }
        public bool? RightSysSettingsCheck { get; set; }
        [Display(Name = "Диспетчерский журнал")]
        public int? RightSysJourDisps { get; set; }
        public bool? RightSysJourDispsCheck { get; set; }
        [Display(Name = "Обмен данными")]
        public int? RightDataExchange { get; set; }
        public bool? RightDataExchangeCheck { get; set; }
        [Display(Name = "Ваши данные")]
        public int? RightYourData { get; set; }
        public bool? RightYourDataCheck { get; set; }
        [Display(Name = "Оплаты")]
        public int? RightDiscPay { get; set; }
        public bool? RightDiscPayCheck { get; set; }

        #endregion


        #region Справочники *** *** ***

        [Display(Name = "Справочники")]
        public bool? RightDir0 { get; set; }

        [Display(Name = "Товар")]
        public int? RightDirNomens { get; set; }
        public bool? RightDirNomensCheck { get; set; }
        [Display(Name = "Категории товара")]
        public int? RightDirNomenCategories { get; set; }
        public bool? RightDirNomenCategoriesCheck { get; set; }
        [Display(Name = "Контрагента")]
        public int? RightDirContractors { get; set; }
        public bool? RightDirContractorsCheck { get; set; }
        [Display(Name = "Склады (Точки продаж)")]
        public int? RightDirWarehouses { get; set; }
        public bool? RightDirWarehousesCheck { get; set; }
        [Display(Name = "Справочник: Банки")]
        public int? RightDirBanks { get; set; }
        public bool? RightDirBanksCheck { get; set; }
        [Display(Name = "Кассы")]
        public int? RightDirCashOffices { get; set; }
        public bool? RightDirCashOfficesCheck { get; set; }
        [Display(Name = "Валюты")]
        public int? RightDirCurrencies { get; set; }
        public bool? RightDirCurrenciesCheck { get; set; }
        [Display(Name = "НДС")]
        public int? RightDirVats { get; set; }
        public bool? RightDirVatsCheck { get; set; }
        [Display(Name = "Скидка (для Контрагента)")]
        public int? RightDirDiscounts { get; set; }
        public bool? RightDirDiscountsCheck { get; set; }
        [Display(Name = "Бонусы (для Сотрудника)")]
        public int? RightDirBonuses { get; set; }
        public bool? RightDirBonusesCheck { get; set; }
        [Display(Name = "Бонусы (для Сотрудника)")]
        public int? RightDirOrdersStates { get; set; }
        public bool? RightDirOrdersStatesCheck { get; set; }

        //Справочники: Характеристики *** *** ***
        [Display(Name = "Характеристика: Цвет")]
        public int? RightDirCharColours { get; set; }
        public bool? RightDirCharColoursCheck { get; set; }
        [Display(Name = "Характеристика: Производитель")]
        public int? RightDirCharMaterials { get; set; }
        public bool? RightDirCharMaterialsCheck { get; set; }
        [Display(Name = "Характеристика: Наименование")]
        public int? RightDirCharNames { get; set; }
        public bool? RightDirCharNamesCheck { get; set; }
        [Display(Name = "Характеристика: Сезон")]
        public int? RightDirCharSeasons { get; set; }
        public bool? RightDirCharSeasonsCheck { get; set; }
        [Display(Name = "Характеристика: Пол")]
        public int? RightDirCharSexes { get; set; }
        public bool? RightDirCharSexesCheck { get; set; }
        [Display(Name = "Характеристика: Размер")]
        public int? RightDirCharSizes { get; set; }
        public bool? RightDirCharSizesCheck { get; set; }
        [Display(Name = "Характеристика: Поставщик")]
        public int? RightDirCharStyles { get; set; }
        public bool? RightDirCharStylesCheck { get; set; }
        [Display(Name = "Характеристика: Текстура")]
        public int? RightDirCharTextures { get; set; }
        public bool? RightDirCharTexturesCheck { get; set; }

        #endregion


        #region Документы *** *** ***

        [Display(Name = "Документы")]
        public bool? RightDoc0 { get; set; }

        [Display(Name = "Документ: Приёмка")]
        public int? RightDocPurches { get; set; }
        public bool? RightDocPurchesCheck { get; set; }
        [Display(Name = "Документ: Возврат поставщику")]
        public int? RightDocReturnVendors { get; set; }
        public bool? RightDocReturnVendorsCheck { get; set; }
        [Display(Name = "Документ: Перемещение")]
        public int? RightDocMovements { get; set; }
        public bool? RightDocMovementsCheck { get; set; }
        [Display(Name = "Документ: Продажа")]
        public int? RightDocSales { get; set; }
        public bool? RightDocSalesCheck { get; set; }
        [Display(Name = "Документ: Возврат от покупателя")]
        public int? RightDocReturnsCustomers { get; set; }
        public bool? RightDocReturnsCustomersCheck { get; set; }
        [Display(Name = "Документ: Акт выполненных работ")]
        public int? RightDocActOnWorks { get; set; }
        public bool? RightDocActOnWorksCheck { get; set; }
        [Display(Name = "Документ: Счет")]
        public int? RightDocAccounts { get; set; }
        public bool? RightDocAccountsCheck { get; set; }
        [Display(Name = "Документ: Списание")]
        public int? RightDocActWriteOffs { get; set; }
        public bool? RightDocActWriteOffsCheck { get; set; }
        [Display(Name = "Документ: Инвентаризация")]
        public int? RightDocInventories { get; set; }
        public bool? RightDocInventoriesCheck { get; set; }
        [Display(Name = "Розничный Чек")]
        public int? RightDocNomenRevaluations { get; set; }
        public bool? RightDocNomenRevaluationsCheck { get; set; }
        //Отчет
        [Display(Name = "Отчет: Отчет по Торговле")]
        public int? RightReportTotalTrade { get; set; }
        public bool? RightReportTotalTradeCheck { get; set; }
        [Display(Name = "Отчет: Отчет по Торговле: Цены")]
        public int? RightReportTotalTradePrice { get; set; }
        public bool? RightReportTotalTradePriceCheck { get; set; }
        [Display(Name = "Отчет по движению товара")]
        public int? RightReportMovementNomen { get; set; }
        public bool? RightReportMovementNomenCheck { get; set; }
        //
        [Display(Name = "Скидка для документов")]
        public int? RightDocDescription { get; set; }
        public bool? RightDocDescriptionCheck { get; set; }


        [Display(Name = "Документы.Витрина")]
        public bool? RightVitrina0 { get; set; }

        [Display(Name = "Розничный Чек")]
        public int? RightDocRetails { get; set; }
        public bool? RightDocRetailsCheck { get; set; }
        [Display(Name = "Розничный Возврат")]
        public int? RightDocRetailReturns { get; set; }
        public bool? RightDocRetailReturnsCheck { get; set; }

        #endregion


        #region Сервис *** *** ***

        [Display(Name = "Сервис")]
        public bool? RightDocService0 { get; set; }

        [Display(Name = "Документ Сервиса: Приёмка")]
        public int? RightDocServicePurches { get; set; }
        public bool? RightDocServicePurchesCheck { get; set; }
        [Display(Name = "Документ Сервиса: Работы")]
        public int? RightDocServicePurch1Tabs { get; set; }
        public bool? RightDocServicePurch1TabsCheck { get; set; }
        [Display(Name = "Документ Сервиса: Запчасти")]
        public int? RightDocServicePurch2Tabs { get; set; }
        public bool? RightDocServicePurch2TabsCheck { get; set; }
        [Display(Name = "Документ Сервиса: Мастерская")]
        public int? RightDocServiceWorkshops { get; set; }
        public bool? RightDocServiceWorkshopsCheck { get; set; }
        [Display(Name = "Документ Сервиса: Мастерская - Мастер видит только свои аппараты")]
        public int? RightDocServiceWorkshopsOnlyUsers { get; set; }
        public bool? RightDocServiceWorkshopsOnlyUsersCheck { get; set; }
        [Display(Name = "СЦ: разрешить возврат на склад товара")]
        public bool? RightDocServiceWorkshopsTab2ReturnCheck { get; set; }
        [Display(Name = "СЦ: разрешить ручное добавление работы")]
        public bool? RightDocServiceWorkshopsTab1AddCheck { get; set; }
        [Display(Name = "Документ Сервиса: Выдача")]
        public int? RightDocServiceOutputs { get; set; }
        public bool? RightDocServiceOutputsCheck { get; set; }
        [Display(Name = "Документ Сервиса: Архив")]
        public int? RightDocServiceArchives { get; set; }
        public bool? RightDocServiceArchivesCheck { get; set; }
        [Display(Name = "Разрешить выдачу аппарата сотруднику (не Админу точки)")]
        public int? RightDocServicePurchesExtradition { get; set; }
        public bool? RightDocServicePurchesExtraditionCheck { get; set; }
        [Display(Name = "Скидка при выдаче аппарата")]
        //public int? RightDocServicePurchesDiscount { get; set; }
        public bool? RightDocServicePurchesDiscountCheck { get; set; }
        [Display(Name = "Видит аппараты со всех точек")]
        //public int? RightDocServicePurchesDiscount { get; set; }
        public bool? RightDocServicePurchesWarehouseAllCheck { get; set; }
        [Display(Name = "Документ Б/У: Перемещение")]
        public int? RightDocServiceMovements { get; set; }
        public bool? RightDocServiceMovementsCheck { get; set; }

        [Display(Name = "Справочник Сервиса: Товар")]
        public int? RightDirServiceNomens { get; set; }
        public bool? RightDirServiceNomensCheck { get; set; }
        [Display(Name = "Справочник Сервиса: Категории товара")]
        public int? RightDirServiceNomenCategories { get; set; }
        public bool? RightDirServiceNomenCategoriesCheck { get; set; }
        [Display(Name = "Справочник Сервиса: Клиент")]
        public int? RightDirServiceContractors { get; set; }
        public bool? RightDirServiceContractorsCheck { get; set; }
        [Display(Name = "Справочник Сервиса: Выполненная работа")]
        public int? RightDirServiceJobNomens { get; set; }
        public bool? RightDirServiceJobNomensCheck { get; set; }
        [Display(Name = "Шаблоны СМС")]
        public int? RightDirSmsTemplates { get; set; }
        public bool? RightDirSmsTemplatesCheck { get; set; }
        [Display(Name = "Результат диагностики")]
        public int? RightDirServiceDiagnosticRresults { get; set; }
        public bool? RightDirServiceDiagnosticRresultsCheck { get; set; }
        [Display(Name = "Типовые неисправности")]
        public int? RightDirServiceNomenTypicalFaults { get; set; }
        public bool? RightDirServiceNomenTypicalFaultsCheck { get; set; }
        [Display(Name = "Отчет: Сервсиный цент")]
        public int? RightDocServicePurchesReport { get; set; }
        public bool? RightDocServicePurchesReportCheck { get; set; }
        [Display(Name = "Разрешить менять дату готовности")]
        public bool? RightDocServicePurchesDateDoneCheck { get; set; }

        #endregion


        #region Б/У *** *** ***

        [Display(Name = "Б/У")]
        public bool? RightDocSecondHands0 { get; set; }

        [Display(Name = "Документ Б/У: Приёмка")]
        public int? RightDocSecondHandPurches { get; set; }
        public bool? RightDocSecondHandPurchesCheck { get; set; }
        [Display(Name = "Документ Б/У: Работы")]
        public int? RightDocSecondHandPurch1Tabs { get; set; }
        public bool? RightDocSecondHandPurch1TabsCheck { get; set; }
        [Display(Name = "Документ Б/У: Запчасти")]
        public int? RightDocSecondHandPurch2Tabs { get; set; }
        public bool? RightDocSecondHandPurch2TabsCheck { get; set; }
        [Display(Name = "Документ Б/У: Мастерская")]
        public int? RightDocSecondHandWorkshops { get; set; }
        public bool? RightDocSecondHandWorkshopsCheck { get; set; }
        [Display(Name = "Документ Б/У: Продажа")]
        public int? RightDocSecondHandRetails { get; set; }
        public bool? RightDocSecondHandRetailsCheck { get; set; }
        [Display(Name = "Документ Б/У: Возврат")]
        public int? RightDocSecondHandRetailReturns { get; set; }
        public bool? RightDocSecondHandRetailReturnsCheck { get; set; }
        [Display(Name = "Документ Б/У: Списание")]
        public int? RightDocSecondHandRetailActWriteOffs { get; set; }
        public bool? RightDocSecondHandRetailActWriteOffsCheck { get; set; }
        [Display(Name = "Документ Б/У: Перемещение")]
        public int? RightDocSecondHandMovements { get; set; }
        public bool? RightDocSecondHandMovementsCheck { get; set; }
        [Display(Name = "Документ Б/У: Отчет")]
        public int? RightDocSecondHandsReport { get; set; }
        public bool? RightDocSecondHandsReportCheck { get; set; }
        [Display(Name = "Документ Б/У: Инв")]
        public int? RightDocSecondHandInventories { get; set; }
        public bool? RightDocSecondHandInventoriesCheck { get; set; }
        [Display(Name = "Документ Б/У: Разборка")]
        public int? RightDocSecondHandRazbors { get; set; }
        public bool? RightDocSecondHandRazborsCheck { get; set; }

        #endregion


        #region Финансы: Банк и Касса *** *** ****

        [Display(Name = "Деньги: Банк и Касса")]
        public bool? RightDocBankCash0 { get; set; }

        [Display(Name = "Справочник Сервиса: Выполненная работа")]
        public int? RightDocBankSums { get; set; }
        public bool? RightDocBankSumsCheck { get; set; }
        [Display(Name = "Справочник Сервиса: Выполненная работа")]
        public int? RightDocCashOfficeSumMovements { get; set; }
        public bool? RightDocCashOfficeSumMovementsCheck { get; set; }
        [Display(Name = "Справочник Сервиса: Выполненная работа")]
        public int? RightDocCashOfficeSums { get; set; }
        public bool? RightDocCashOfficeSumsCheck { get; set; }
        [Display(Name = "Документ: Зарплата")]
        public int? RightDocSalaries { get; set; }
        public bool? RightDocSalariesCheck { get; set; }
        //[Display(Name = "Справочник Сервиса: Выполненная работа")]
        //public int? RightBanksCashOfficesReport { get; set; }
        //public bool? RightBanksCashOfficesReportCheck { get; set; }
        //Отчет
        [Display(Name = "Отчет: Общий по Финансам")]
        public int? RightReportBanksCashOffices { get; set; }
        public bool? RightReportBanksCashOfficesCheck { get; set; }

        [Display(Name = "Зарплата")]
        public bool? RightSalaries0 { get; set; }
        [Display(Name = "Зарплата (по сотрудникам)")]
        public int? RightReportSalaries { get; set; }
        public bool? RightReportSalariesCheck { get; set; }
        [Display(Name = "Зарплата (по точкам)")]
        public int? RightReportSalariesWarehouses { get; set; }
        public bool? RightReportSalariesWarehousesCheck { get; set; }
        [Display(Name = "Видит ЗП всех сотрудников")]
        public bool? RightReportSalariesEmplCheck { get; set; }

        //Хозрасчёты
        [Display(Name = "Справочник Хозрасчёты")]
        public int? RightDirDomesticExpenses { get; set; }
        public bool? RightDirDomesticExpensesCheck { get; set; }
        [Display(Name = "Документ Хозрасчёты - ЗП")]
        public int? RightDocDomesticExpenseSalaries { get; set; }
        public bool? RightDocDomesticExpenseSalariesCheck { get; set; }
        [Display(Name = "Документ Хозрасчёты")]
        public int? RightDocDomesticExpenses { get; set; }
        public bool? RightDocDomesticExpensesCheck { get; set; }

        #endregion


        #region Заказы *** *** ****

        [Display(Name = "Заказы")]
        public bool? RightDocOrderInt0 { get; set; }
        //Новый
        public int? RightDocOrderIntsNew { get; set; }
        public bool? RightDocOrderIntsNewCheck { get; set; }
        //Список
        public int? RightDocOrderInts { get; set; }
        public bool? RightDocOrderIntsCheck { get; set; }
        //Отчет
        public int? RightDocOrderIntsReport { get; set; }
        public bool? RightDocOrderIntsReportCheck { get; set; }
        //Архив
        public int? RightDocOrderIntsArchive { get; set; }
        public bool? RightDocOrderIntsArchiveCheck { get; set; }
        //Клиенты
        public int? RightDirOrderIntContractors { get; set; }
        public bool? RightDirOrderIntContractorsCheck { get; set; }

        #endregion


        #region Логистика *** *** ****

        [Display(Name = "Логистика")]
        public bool? RightLogistics0 { get; set; }
        [Display(Name = "Перемещение для Курьера")]
        public int? RightDocMovementsLogistics { get; set; }
        public bool? RightDocMovementsLogisticsCheck { get; set; }
        [Display(Name = "Отчет по Логистике")]
        public int? RightReportLogistics { get; set; }
        public bool? RightReportLogisticsCheck { get; set; }

        #endregion


        #region Аналитика *** *** ****

        [Display(Name = "Аналитика")]
        public bool? RightAnalitics0 { get; set; }
        
        //...

        #endregion


        #region Отчеты *** *** ****

        [Display(Name = "Отчеты")]
        public bool? RightReport0 { get; set; }

        [Display(Name = "Отчет: Прайс-лист")]
        public int? RightReportPriceList { get; set; }
        public bool? RightReportPriceListCheck { get; set; }
        [Display(Name = "Отчет: Остатки")]
        public int? RightReportRemnants { get; set; }
        public bool? RightReportRemnantsCheck { get; set; }
        [Display(Name = "Отчет: Прибыль")]
        public int? RightReportProfit { get; set; }
        public bool? RightReportProfitCheck { get; set; }
        [Display(Name = "Отчет: Печать кодов товара в приходе")]
        public int? RightDocPurchesPrintCode { get; set; }
        public bool? RightDocPurchesPrintCodeCheck { get; set; }

        #endregion


        #region Отчеты *** *** ****

        [Display(Name = "ККМ")]
        public bool? RightKKM0 { get; set; }

        [Display(Name = "X-отчет")]
        public int? RightKKMXReport { get; set; }
        public bool? RightKKMXReportCheck { get; set; }
        [Display(Name = "Открытие смены")]
        public int? RightKKMOpen { get; set; }
        public bool? RightKKMOpenCheck { get; set; }
        [Display(Name = "Инкассация денег из кассы")]
        public int? RightKKMEncashment { get; set; }
        public bool? RightKKMEncashmentCheck { get; set; }
        [Display(Name = "Внесение денег в кассу")]
        public int? RightKKMAdding { get; set; }
        public bool? RightKKMAddingCheck { get; set; }
        [Display(Name = "Закрытие смены")]
        public int? RightKKMClose { get; set; }
        public bool? RightKKMCloseCheck { get; set; }
        [Display(Name = "Печать состояния расчетов и связи с ОФД")]
        public int? RightKKMPrintOFD { get; set; }
        public bool? RightKKMPrintOFDCheck { get; set; }
        [Display(Name = "Получить данные последнего чека из ФН.")]
        public int? RightKKMCheckLastFN { get; set; }
        public bool? RightKKMCheckLastFNCheck { get; set; }
        [Display(Name = "Получить текущее состояние ККТ")]
        public int? RightKKMState { get; set; }
        public bool? RightKKMStateCheck { get; set; }
        [Display(Name = "Получение списка ККМ")]
        public int? RightKKMList { get; set; }
        public bool? RightKKMListCheck { get; set; }

        #endregion


        #region Другое *** *** ***

        [Display(Name = "Другое")]
        public bool? RightOther0 { get; set; }

        [Display(Name = "Разработчик: Доступ в Дизайнер ПФ")]
        public int? RightDevelop { get; set; }
        public bool? RightDevelopCheck { get; set; }

        //[Display(Name = "API 10")]
        public int? RightAPI10s { get; set; }
        public bool? RightAPI10sCheck { get; set; }

        //[Display(Name = "Витрина")]
        public int? RightDirWebShopUOs { get; set; }
        public bool? RightDirWebShopUOsCheck { get; set; }

        #endregion


        #endregion



        #region Заполняем пустные поля (которые не должны быть пустыми)

        public void Substitute()
        {

            if (DirEmployeeActive == null) DirEmployeeActive = false;

            //Права
            if (RightOther0 == null) RightOther0 = false;
            if (RightReport0 == null) RightReport0 = false;
            if (RightDocBankCash0 == null) RightDocBankCash0 = false;
            if (RightDocService0 == null) RightDocService0 = false;
            if (RightDoc0 == null) RightDoc0 = false;
            if (RightDir0 == null) RightDir0 = false;
            if (RightSysSettings0 == null) RightSysSettings0 = false;

            //Видит только свои ремонты
            if (RightDocServiceWorkshopsOnlyUsersCheck == null) RightDocServiceWorkshopsOnlyUsersCheck = false;
            //Разрешить Возврат на склад
            if (RightDocServiceWorkshopsTab2ReturnCheck == null) RightDocServiceWorkshopsTab2ReturnCheck = false;
            //СЦ: разрешить возврат на склад товара
            if (RightDocServiceWorkshopsTab1AddCheck == null) RightDocServiceWorkshopsTab1AddCheck = false;


            //Не используется, заменили название на другое!!!
            //Справочник Сервиса: Выполненная работа
            //RightBanksCashOfficesReport = 1;
            //RightBanksCashOfficesReportCheck = true;

            //Администратор
            if (DirEmployeeID == 1)
            {
                DirEmployeeActive = true;
                DirContractorIDOrg = null;


                // === Права доступа === === === === === === === === === === === === ===

                //Настройки *** *** ***
                //Настройки
                RightSysSettings0 = true;

                //Ваши фирмы
                RightMyCompany = 1;
                RightMyCompanyCheck = true;
                //Сотрудники
                RightDirEmployees = 1;
                RightDirEmployeesCheck = true;
                //Настройки
                RightSysSettings = 1;
                RightSysSettingsCheck = true;
                //Диспетчерский журнал
                RightSysJourDisps = 1;
                RightSysJourDispsCheck = true;
                //Обмен данными
                RightDataExchange = 1;
                RightDataExchangeCheck = true;
                //Ваши данные
                RightYourData = 1;
                RightYourDataCheck = true;
                //Оплаты
                RightDiscPay = 1;
                RightDiscPayCheck = true;


                //Справочники *** *** ***
                //Справочники
                RightDir0 = true;

                //Товар
                RightDirNomens = 1;
                RightDirNomensCheck = true;
                //Категории товара
                RightDirNomenCategories = 1;
                RightDirNomenCategoriesCheck = true;
                //Контрагента
                RightDirContractors = 1;
                RightDirContractorsCheck = true;
                //Склады (Точки продаж)
                RightDirWarehouses = 1;
                RightDirWarehousesCheck = true;
                //Справочник: Банки
                RightDirBanks = 1;
                RightDirBanksCheck = true;
                //Кассы
                RightDirCashOffices = 1;
                RightDirCashOfficesCheck = true;
                //Валюты
                RightDirCurrencies = 1;
                RightDirCurrenciesCheck = true;
                //НДС
                RightDirVats = 1;
                RightDirVatsCheck = true;
                //Скидка (для Контрагента)
                RightDirDiscounts = 1;
                RightDirDiscountsCheck = true;
                //Бонусы (для Сотрудника)
                RightDirBonuses = 1;
                RightDirBonusesCheck = true;
                //Шаблоны СМС
                RightDirSmsTemplates = 1;
                RightDirSmsTemplatesCheck = true;
                //Результат диагностики
                RightDirServiceDiagnosticRresults = 1;
                RightDirServiceDiagnosticRresultsCheck = true;
                //Типовые неисправности
                RightDirServiceNomenTypicalFaults = 1;
                RightDirServiceNomenTypicalFaultsCheck = true;

                //Справочники: Характеристики *** *** ***
                //Характеристика: Цвет
                RightDirCharColours = 1;
                RightDirCharColoursCheck = true;
                //Характеристика: Производитель
                RightDirCharMaterials = 1;
                RightDirCharMaterialsCheck = true;
                //Характеристика: Наименование
                RightDirCharNames = 1;
                RightDirCharNamesCheck = true;
                //Характеристика: Сезон
                RightDirCharSeasons = 1;
                RightDirCharSeasonsCheck = true;
                //Характеристика: Пол
                RightDirCharSexes = 1;
                RightDirCharSexesCheck = true;
                //Характеристика: Размер
                RightDirCharSizes = 1;
                RightDirCharSizesCheck = true;
                //Характеристика: Поставщик
                RightDirCharStyles = 1;
                RightDirCharStylesCheck = true;
                //Характеристика: Текстура
                RightDirCharTextures = 1;
                RightDirCharTexturesCheck = true;


                //Документы *** *** ***
                //Документы
                RightDoc0 = true;

                //Документ: Приёмка
                RightDocPurches = 1;
                RightDocPurchesCheck = true;
                //Документ: Возврат поставщику
                RightDocReturnVendors = 1;
                RightDocReturnVendorsCheck = true;
                //Документ: Перемещение
                RightDocMovements = 1;
                RightDocMovementsCheck = true;
                //Документ: Продажа
                RightDocSales = 1;
                RightDocSalesCheck = true;
                //Документ: Возврат от покупателя
                RightDocReturnsCustomers = 1;
                RightDocReturnsCustomersCheck = true;
                //Документ: Акт выполненных работ
                RightDocActOnWorks = 1;
                RightDocActOnWorksCheck = true;
                //Документ: Счет
                RightDocAccounts = 1;
                RightDocAccountsCheck = true;
                //Документ: Списание
                RightDocActWriteOffs = 1;
                RightDocActWriteOffsCheck = true;
                //Документ: Инвентаризация
                RightDocInventories = 1;
                RightDocInventoriesCheck = true;
                //Общий отчет по Торговле
                RightReportTotalTrade = 1;
                RightReportTotalTradeCheck = true;
                //Общий отчет по Торговле: Цены
                RightReportTotalTradePrice = 1;
                RightReportTotalTradePriceCheck = true;
                //Переоценка
                RightDocNomenRevaluations = 1;
                RightDocNomenRevaluationsCheck = true;

                //Документы
                RightVitrina0 = true;
                //Розничный Чек
                RightDocRetails = 1;
                RightDocRetailsCheck = true;
                //Розничный Возврат
                RightDocRetailReturns = 1;
                RightDocRetailReturnsCheck = true;


                //Сервис *** *** ***
                //Сервис
                RightDocService0 = true;

                //Документ Сервиса: Приёмка
                RightDocServicePurches = 1;
                RightDocServicePurchesCheck = true;
                //Документ Сервиса: Мастерская
                RightDocServiceWorkshops = 1;
                RightDocServiceWorkshopsCheck = true;
                //Документ Сервиса: Выдача
                RightDocServiceOutputs = 1;
                RightDocServiceOutputsCheck = true;
                //Документ Сервиса: Архив
                RightDocServiceArchives = 1;
                RightDocServiceArchivesCheck = true;
                //Документ Сервиса: Разрешать выдачу аппарата (не Админ точки)
                RightDocServicePurchesExtradition = 1;
                RightDocServicePurchesExtraditionCheck = true;
                //Разрешить Скидки
                RightDocServicePurchesDiscountCheck = true;
                //Видит аппараты со всех точек
                RightDocServicePurchesWarehouseAllCheck = true;
                //Справочник Сервиса: Товар
                RightDirServiceNomens = 1;
                RightDirServiceNomensCheck = true;
                //Справочник Сервиса: Категории товара
                RightDirServiceNomenCategories = 1;
                RightDirServiceNomenCategoriesCheck = true;
                //Справочник Сервиса: Клиент
                RightDirServiceContractors = 1;
                RightDirServiceContractorsCheck = true;
                //Справочник Сервиса: Выполненная работа
                RightDirServiceJobNomens = 1;
                RightDirServiceJobNomensCheck = true;
                //Отчет: Сервсиный цент
                RightDocServicePurchesReport = 1;
                RightDocServicePurchesReportCheck = true;
                //Разрешить менять дату готовности
                RightDocServicePurchesDateDoneCheck = true; 
                //Документ Б/У: Перемещение
                RightDocServiceMovements = 1;
                RightDocServiceMovementsCheck = true;

                RightDocServiceWorkshopsOnlyUsersCheck = false;
                RightDocServiceWorkshopsTab2ReturnCheck = true;
                RightDocServiceWorkshopsTab1AddCheck = true;


                //Б/У *** *** ***
                //Б/У
                RightDocSecondHands0 = true;

                //Документ Б/У: Приёмка
                RightDocSecondHandPurches = 1;
                RightDocSecondHandPurchesCheck = true;
                //Документ Б/У: Мастерская
                RightDocSecondHandWorkshops = 1;
                RightDocSecondHandWorkshopsCheck = true;
                //Документ Б/У: Отчет
                RightDocSecondHandsReport = 1;
                RightDocSecondHandsReportCheck = true;
                //Документ Б/У: Продажа
                RightDocSecondHandRetails = 1;
                RightDocSecondHandRetailsCheck = true;
                //Документ Б/У: Возврат
                RightDocSecondHandRetailReturns = 1;
                RightDocSecondHandRetailReturnsCheck = true;
                //Документ Б/У: Списание
                RightDocSecondHandRetailActWriteOffs = 1;
                RightDocSecondHandRetailActWriteOffsCheck = true;
                //Документ Б/У: Перемещение
                RightDocSecondHandMovements = 1;
                RightDocSecondHandMovementsCheck = true;
                //Документ Б/У: Инв
                RightDocSecondHandInventories = 1;
                RightDocSecondHandInventoriesCheck = true;
                //Документ Б/У: Разборка
                RightDocSecondHandRazbors = 1;
                RightDocSecondHandRazborsCheck = true;


                //Заказы
                RightDocOrderInt0 = true;

                //Новый
                RightDocOrderIntsNew = 1;
                RightDocOrderIntsNewCheck = true;
                //Список
                RightDocOrderInts = 1;
                RightDocOrderIntsCheck = true;
                //Отчет
                RightDocOrderIntsReport = 1;
                RightDocOrderIntsReportCheck = true;
                //Архив
                RightDocOrderIntsArchive = 1;
                RightDocOrderIntsArchiveCheck = true;
                //Клиенты
                RightDirOrderIntContractors = 1;
                RightDirOrderIntContractorsCheck = true;


                //Деньги: Банк и Касса *** *** ****
                //Деньги: Банк и Касса
                RightDocBankCash0 = true;

                //Справочник Сервиса: Выполненная работа
                RightDocBankSums = 1;
                RightDocBankSumsCheck = true;
                //Справочник Сервиса: Выполненная работа
                RightDocCashOfficeSumMovements = 1;
                RightDocCashOfficeSumMovementsCheck = true;
                //Справочник Сервиса: Выполненная работа
                RightDocCashOfficeSums = 1;
                RightDocCashOfficeSumsCheck = true;
                //Отчет
                RightReportBanksCashOffices = 1;
                RightReportBanksCashOfficesCheck = true;

                //Зарплата
                RightSalaries0 = true;

                //Документ: Зарплата
                RightDocSalaries = 1;
                RightDocSalariesCheck = true;
                //Документ: Зарплата
                RightReportSalaries = 1;
                RightReportSalariesCheck = true;
                //Документ: Зарплата
                RightReportSalariesWarehouses = 1;
                RightReportSalariesWarehousesCheck = true;
                //Хоз расчёты: Справочник
                RightDirDomesticExpenses = 1;
                RightDirDomesticExpensesCheck = true;
                //Хоз расчёты: ЗП
                RightDocDomesticExpenseSalaries = 1;
                RightDocDomesticExpenseSalariesCheck = true;
                //Хоз расчёты: Другое
                RightDocDomesticExpenses = 1;
                RightDocDomesticExpensesCheck = true;
                //Видит ЗП всех сотрудников
                RightReportSalariesEmplCheck = true;



                //Логистика
                RightLogistics0 = true;
                //Перемещение для Курьера
                RightDocMovementsLogistics = 1;
                RightDocMovementsLogisticsCheck = true;


                //Аналитика
                RightAnalitics0 = true;


                //Отчеты *** *** ****
                //Отчеты
                RightReport0 = true;

                //Отчет: Прайс-лист
                RightReportPriceList = 1;
                RightReportPriceListCheck = true;
                //Отчет: Остатки
                RightReportRemnants = 1;
                RightReportRemnantsCheck = true;
                //Отчет: Прибыль
                RightReportProfit = 1;
                RightReportProfitCheck = true;
                //Отчет: Печать кодов товара в приходе
                RightDocPurchesPrintCode = 1;
                RightDocPurchesPrintCodeCheck = true;


                //Другое *** *** ***
                //Другое
                RightOther0 = true;
                //Разработчик: Доступ в Дизайнер ПФ
                RightDevelop = 1;
                RightDevelopCheck = true;
                //API 10
                RightAPI10s = 1;
                RightAPI10sCheck = true;
                //Витрина
                RightDirWebShopUOs = 1;
                RightDirWebShopUOsCheck = true;
                //Отчет по движению товара
                RightReportMovementNomen = 1;
                RightReportMovementNomenCheck = true;
                //Скидки для документов
                RightDocDescription = 1;
                RightDocDescriptionCheck = true;

            }

        }

        #endregion
    }


    [Table("DirEmployeeHistories")]
    public class DirEmployeeHistory
    {
        [Key]
        public int DirEmployeeHistoryID { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual Dir.DirEmployee dirEmployee { get; set; }

        [Required]
        public DateTime? HistoryDate { get; set; }

        [Display(Name = "Валюта")]
        public int? DirCurrencyID { get; set; }
        [Display(Name = "ЗП за X работы")]
        public double? Salary { get; set; }
        [Display(Name = "ЗП за день, неделю или месяц работы")]
        public int? SalaryDayMonthly { get; set; }
        [Display(Name = "Бонус в % с градацией")]
        public int? DirBonusID { get; set; }
    }


    [Table("DirEmployeeWarehouses")]
    public class DirEmployeeWarehouses
    {
        [Key]
        public int DirEmployeeWarehouseID { get; set; }

        [Display(Name = "Товар")]
        [Required]
        public int? DirEmployeeID { get; set; }
        [ForeignKey("DirEmployeeID")]
        public virtual Dir.DirEmployee dirEmployee { get; set; }

        [Display(Name = "Склад")]
        [Required]
        public int? DirWarehouseID { get; set; }
        [ForeignKey("DirWarehouseID")]
        public virtual Dir.DirWarehouse dirWarehouse { get; set; }



        [Display(Name = "Админ точки")]
        [Required]
        public bool IsAdmin { get; set; }
        [Display(Name = "!!! Сука Глюк !!!")]
        [NotMapped]
        public string IsAdminNameRu { get; set; }


        [Display(Name = "СЦ: Видит точку")]
        [Required]
        public bool WarehouseAll { get; set; }
        [Display(Name = "!!! Сука Глюк !!!")]
        [NotMapped]
        public string WarehouseAllNameRu { get; set; }

    }

}