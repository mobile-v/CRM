using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PartionnyAccount.Models
{
    public class DbConnectionSklad : DbContext
    {
        //Беспараметрический конструктор
        //Когда создаём Шаблон Контролера для DirNomen - надо раскомментировать! 
        //Т.к. Студия создаёт "безпараметрическое" подключение, а у нас сейчас обязателен параметр - строка подклчения (ниже)
        //Но, потом ОБЯЗАТЕЛЬНО закоментировать обратно!
        public DbConnectionSklad() { }

        //Передача строки соединения
        // === Free ===
        //public DbConnectionSklad(string connString) : base(connString) { }
        // === Comercial ===
        public DbConnectionSklad(string connString) : base(new System.Data.SQLite.SQLiteConnection() { ConnectionString = connString }, true) { }


        //Sys
        public DbSet<Sklad.Sys.SysJourDisp> SysJourDisps { get; set; }
        public DbSet<Sklad.Sys.SysSetting> SysSettings { get; set; }
        public DbSet<Sklad.Sys.SysVer> SysVers { get; set; }
        public DbSet<Sklad.Sys.SysGen> SysGens { get; set; }
        public DbSet<Sklad.Sys.SysGenBarCode> SysGenBarCodes { get; set; }


        //Dir
        public DbSet<Login.Dir.DirCustomer> DirCustomers { get; set; }
        public DbSet<Login.Dir.DirCountry> DirCountries { get; set; }
        public DbSet<Login.Dir.DirLanguage> DirLanguages { get; set; }
        public DbSet<Sklad.Dir.DirEmployee> DirEmployees { get; set; } public DbSet<Sklad.Dir.DirEmployeeHistory> DirEmployeeHistories { get; set; } public DbSet<Sklad.Dir.DirEmployeeWarehouses> DirEmployeeWarehouse { get; set; }
        public DbSet<Sklad.Dir.DirCurrency> DirCurrencies { get; set; } public DbSet<Sklad.Dir.DirCurrencyHistory> DirCurrencyHistories { get; set; }
        public DbSet<Sklad.Dir.DirNomen> DirNomens { get; set; } public DbSet<Sklad.Dir.DirNomenHistory> DirNomenHistories { get; set; }
        public DbSet<Sklad.Dir.DirDispOperation> DirDispOperations { get; set; }
        public DbSet<Sklad.Dir.DirBonus> DirBonuses { get; set; } public DbSet<Sklad.Dir.DirBonusTab> DirBonusTabs { get; set; }
        public DbSet<Sklad.Dir.DirBonus2> DirBonus2es { get; set; } public DbSet<Sklad.Dir.DirBonus2Tab> DirBonus2Tabs { get; set; }
        public DbSet<Sklad.Dir.DirWarehouse> DirWarehouses { get; set; }
        public DbSet<Sklad.Dir.DirCashOffice> DirCashOffices { get; set; }
        public DbSet<Sklad.Dir.DirBank> DirBanks { get; set; }
        public DbSet<Sklad.Dir.DirContractor1Type> DirContractor1Types { get; set; }
        public DbSet<Sklad.Dir.DirContractor2Type> DirContractor2Types { get; set; }
        public DbSet<Sklad.Dir.DirContractor> DirContractors { get; set; }
        public DbSet<Sklad.Dir.DirVat> DirVats { get; set; }
        public DbSet<Sklad.Dir.DirDiscount> DirDiscounts { get; set; }
        public DbSet<Sklad.Dir.DirDiscountTab> DirDiscountTabs { get; set; }
        public DbSet<Sklad.Dir.DirCharColour> DirCharColours { get; set; }
        public DbSet<Sklad.Dir.DirCharMaterial> DirCharMaterials { get; set; }
        public DbSet<Sklad.Dir.DirCharName> DirCharNames { get; set; }
        public DbSet<Sklad.Dir.DirCharSeason> DirCharSeasons { get; set; }
        public DbSet<Sklad.Dir.DirCharSex> DirCharSexes { get; set; }
        public DbSet<Sklad.Dir.DirCharSize> DirCharSizes { get; set; }
        public DbSet<Sklad.Dir.DirCharStyle> DirCharStyles { get; set; }
        public DbSet<Sklad.Dir.DirCharTexture> DirCharTextures { get; set; }
        public DbSet<Sklad.Dir.DirNomenCategory> DirNomenCategories { get; set; }
        public DbSet<Sklad.Dir.DirNomenType> DirNomenTypes { get; set; }
        public DbSet<Sklad.Dir.DirPriceType> DirPriceTypes { get; set; }
        public DbSet<Sklad.Dir.DirPaymentType> DirPaymentTypes { get; set; }
        public DbSet<Sklad.Dir.DirCashOfficeSumType> DirCashOfficeSumTypes { get; set; }
        public DbSet<Sklad.Dir.DirBankSumType> DirBankSumTypes { get; set; }
        public DbSet<Sklad.Dir.DirDescription> DirDescriptions { get; set; }
        public DbSet<Sklad.Dir.DirReturnType> DirReturnTypes { get; set; }
        public DbSet<Sklad.Dir.DirMovementDescription> DirMovementDescriptions { get; set; }
        public DbSet<Sklad.Dir.DirMovementStatus> DirMovementStatuses { get; set; }
        public DbSet<Sklad.Dir.DirMovementLogType> DirMovementLogTypes { get; set; }
        //Dir-Service
        public DbSet<Sklad.Dir.DirServiceNomenCategory> DirServiceNomenCategories { get; set; }
        public DbSet<Sklad.Dir.DirServiceNomen> DirServiceNomens { get; set; } public DbSet<Sklad.Dir.DirServiceNomenPrice> DirServiceNomenPrices { get; set; }
        public DbSet<Sklad.Dir.DirServiceContractor> DirServiceContractors { get; set; }
        public DbSet<Sklad.Dir.DirServiceStatus> DirServiceStatuses { get; set; }
        public DbSet<Sklad.Dir.DirServiceJobNomen> DirServiceJobNomens { get; set; } public DbSet<Sklad.Dir.DirServiceJobNomenHistory> DirServiceJobNomenHistories { get; set; }
        public DbSet<Sklad.Dir.DirServiceComplect> DirServiceComplects { get; set; }
        public DbSet<Sklad.Dir.DirServiceProblem> DirServiceProblems { get; set; }
        public DbSet<Sklad.Dir.DirServiceLogType> DirServiceLogTypes { get; set; }
        public DbSet<Sklad.Dir.DirServiceDiagnosticRresult> DirServiceDiagnosticRresults { get; set; }
        public DbSet<Sklad.Dir.DirServiceNomenTypicalFault> DirServiceNomenTypicalFaults { get; set; }
        //Dir-SecondHand
        public DbSet<Sklad.Dir.DirSecondHandStatus> DirSecondHandStatuses { get; set; }
        public DbSet<Sklad.Dir.DirSecondHandLogType> DirSecondHandLogTypes { get; set; }
        //Dir-Order
        public DbSet<Sklad.Dir.DirOrderIntStatus> DirOrderIntStatuses { get; set; }
        public DbSet<Sklad.Dir.DirOrderIntType> DirOrderIntTypes { get; set; }
        public DbSet<Sklad.Dir.DirOrderIntContractor> DirOrderIntContractors { get; set; }
        public DbSet<Sklad.Dir.DirOrderIntLogType> DirOrderIntLogTypes { get; set; }
        //Sms
        public DbSet<Sklad.Dir.DirSmsTemplate> DirSmsTemplates { get; set; }
        //Финансы: Хоз.расчёты
        public DbSet<Sklad.Dir.DirDomesticExpense> DirDomesticExpenses { get; set; }
        public DbSet<Sklad.Dir.DirOrdersState> DirOrdersStates { get; set; }
        public DbSet<Sklad.Dir.DirDescriptionDiscount> DirDescriptionDiscounts { get; set; }


        //List
        public DbSet<Sklad.List.ListLanguage> ListLanguages { get; set; }
        public DbSet<Sklad.List.ListObject> ListObjects { get; set; }
        public DbSet<Sklad.List.ListObjectField> ListObjectFields { get; set; }
        public DbSet<Sklad.List.ListObjectFieldName> ListObjectFieldNames { get; set; }
        public DbSet<Sklad.List.ListObjectPF> ListObjectPFs { get; set; }
        public DbSet<Sklad.List.ListObjectPFTab> ListObjectPFTabs { get; set; }


        //Log
        public DbSet<Sklad.Log.LogService> LogServices { get; set; }
        public DbSet<Sklad.Log.LogMovement> LogMovements { get; set; } public DbSet<Sklad.Log.LogLogistic> LogLogistics { get; set; }
        public DbSet<Sklad.Log.LogOrderInt> LogOrderInts { get; set; }
        public DbSet<Sklad.Log.LogSecondHand> LogSecondHands { get; set; }
        public DbSet<Sklad.Log.LogSecondHandRazbor> LogSecondHandRazbors { get; set; }


        //Doc
        public DbSet<Sklad.Doc.Doc> Docs { get; set; }
        public DbSet<Sklad.Doc.DocCashOfficeSum> DocCashOfficeSums { get; set; } public DbSet<Sklad.Doc.DocBankSum> DocBankSums { get; set; }
        public DbSet<Sklad.Doc.DocCashOfficeSumMovement> DocCashOfficeSumMovements { get; set; } 
        public DbSet<Sklad.Doc.DocPurch> DocPurches { get; set; } public DbSet<Sklad.Doc.DocPurchTab> DocPurchTabs { get; set; }
        public DbSet<Sklad.Doc.DocSale> DocSales { get; set; } public DbSet<Sklad.Doc.DocSaleTab> DocSaleTabs { get; set; }
        public DbSet<Sklad.Doc.DocMovement> DocMovements { get; set; } public DbSet<Sklad.Doc.DocMovementTab> DocMovementTabs { get; set; }
        public DbSet<Sklad.Doc.DocReturnVendor> DocReturnVendors { get; set; } public DbSet<Sklad.Doc.DocReturnVendorTab> DocReturnVendorTabs { get; set; }
        public DbSet<Sklad.Doc.DocActWriteOff> DocActWriteOffs { get; set; } public DbSet<Sklad.Doc.DocActWriteOffTab> DocActWriteOffTabs { get; set; }
        public DbSet<Sklad.Doc.DocReturnsCustomer> DocReturnsCustomers { get; set; } public DbSet<Sklad.Doc.DocReturnsCustomerTab> DocReturnsCustomerTabs { get; set; }
        public DbSet<Sklad.Doc.DocActOnWork> DocActOnWorks { get; set; } public DbSet<Sklad.Doc.DocActOnWorkTab> DocActOnWorkTabs { get; set; }
        public DbSet<Sklad.Doc.DocAccount> DocAccounts { get; set; } public DbSet<Sklad.Doc.DocAccountTab> DocAccountTabs { get; set; }
        public DbSet<Sklad.Doc.DocInventory> DocInventories { get; set; } public DbSet<Sklad.Doc.DocInventoryTab> DocInventoryTabs { get; set; }
        public DbSet<Sklad.Doc.DocRetail> DocRetails { get; set; } public DbSet<Sklad.Doc.DocRetailTab> DocRetailTabs { get; set; }
        public DbSet<Sklad.Doc.DocRetailReturn> DocRetailReturns { get; set; } public DbSet<Sklad.Doc.DocRetailReturnTab> DocRetailReturnTabs { get; set; }
        public DbSet<Sklad.Doc.DocRetailActWriteOff> DocRetailActWriteOffs { get; set; } public DbSet<Sklad.Doc.DocRetailActWriteOffTab> DocRetailActWriteOffTabs { get; set; }
        public DbSet<Sklad.Doc.DocNomenRevaluation> DocNomenRevaluations { get; set; } public DbSet<Sklad.Doc.DocNomenRevaluationTab> DocNomenRevaluationTabs { get; set; }
        public DbSet<Sklad.Doc.DocDomesticExpense> DocDomesticExpenses { get; set; } public DbSet<Sklad.Doc.DocDomesticExpenseTab> DocDomesticExpenseTabs { get; set; }
        //Doc-Service
        public DbSet<Sklad.Doc.DocServicePurch> DocServicePurches { get; set; }
        public DbSet<Sklad.Doc.DocServicePurch1Tab> DocServicePurch1Tabs { get; set; }
        public DbSet<Sklad.Doc.DocServicePurch2Tab> DocServicePurch2Tabs { get; set; }
        public DbSet<Sklad.Doc.DocOrderInt> DocOrderInts { get; set; }
        public DbSet<Sklad.Doc.DocServiceMov> DocServiceMovs { get; set; }
        public DbSet<Sklad.Doc.DocServiceMovTab> DocServiceMovTabs { get; set; }
        //Doc-SecondHand
        public DbSet<Sklad.Doc.DocSecondHandPurch> DocSecondHandPurches { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandPurch1Tab> DocSecondHandPurch1Tabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandPurch2Tab> DocSecondHandPurch2Tabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandRetail> DocSecondHandRetails { get; set; } public DbSet<Sklad.Doc.DocSecondHandRetailTab> DocSecondHandRetailTabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandSale> DocSecondHandSales { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandRetailReturn> DocSecondHandRetailReturns { get; set; } public DbSet<Sklad.Doc.DocSecondHandRetailReturnTab> DocSecondHandRetailReturnTabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandReturn> DocSecondHandReturns { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandRetailActWriteOff> DocSecondHandRetailActWriteOffs { get; set; } public DbSet<Sklad.Doc.DocSecondHandRetailActWriteOffTab> DocSecondHandRetailActWriteOffTabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandMovement> DocSecondHandMovements { get; set; } public DbSet<Sklad.Doc.DocSecondHandMovementTab> DocSecondHandMovementTabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandMov> DocSecondHandMovs { get; set; } public DbSet<Sklad.Doc.DocSecondHandMovTab> DocSecondHandMovTabs { get; set; }
        //ЗП
        public DbSet<Sklad.Doc.DocSalary> DocSalaries { get; set; } public DbSet<Sklad.Doc.DocSalaryTab> DocSalaryTabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandInventory> DocSecondHandInventories { get; set; } public DbSet<Sklad.Doc.DocSecondHandInventoryTab> DocSecondHandInventoryTabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandInv> DocSecondHandInvs { get; set; } public DbSet<Sklad.Doc.DocSecondHandInvTab> DocSecondHandInvTabs { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandRazbor> DocSecondHandRazbors { get; set; }
        public DbSet<Sklad.Doc.DocSecondHandRazbor2Tab> DocSecondHandRazbor2Tabs { get; set; } public DbSet<Sklad.Doc.DocSecondHandRazborTab> DocSecondHandRazborTabs { get; set; }


        //Rem
        public DbSet<Sklad.Rem.RemParty> RemParties { get; set; }
        public DbSet<Sklad.Rem.RemPartyMinus> RemPartyMinuses { get; set; }
        public DbSet<Sklad.Rem.RemRemnant> RemRemnants { get; set; }

        public DbSet<Sklad.Rem.Rem2Party> Rem2Parties { get; set; }
        public DbSet<Sklad.Rem.Rem2PartyMinus> Rem2PartyMinuses { get; set; }


        //Doc-Service
        public DbSet<Sklad.Service.API.API10> API10s { get; set; }
        public DbSet<Sklad.Service.IM.DirWebShopUO> DirWebShopUOs { get; set; }
    }
}