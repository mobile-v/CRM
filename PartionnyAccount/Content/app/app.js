Ext.application({
    name: "PartionnyAccount",
    appFolder: 'Content/app',

    views: [
        //Container
        "Sklad/Container/viewContainerHeader", "Sklad/Container/viewContainerFooter", "Sklad/Container/viewContainerLeft", "Sklad/Container/viewContainerRight",
        "Sklad/Container/viewContainerCentralPanel", "Sklad/Container/viewContainerCentralTab",
        //Object
        "Sklad/Object/Main/viewMain",
        //Sys
        "Sklad/Object/Sys/SysSettings/viewSysSettingsEdit",
        //Dir
        "Sklad/Object/Dir/DirNomens/viewDirNomens", "Sklad/Object/Dir/DirNomens/viewDirNomensWinEdit", "Sklad/Object/Dir/DirNomens/viewDirNomensWinComboEdit", "Sklad/Object/Dir/DirNomens/viewDirNomenRemParties", "Sklad/Object/Dir/DirNomens/viewDirNomensImg",
        "Sklad/Object/Dir/DirNomens/viewDirNomenCategories", "Sklad/Object/Dir/DirNomens/viewDirNomensSelect",
        "Sklad/Object/Dir/DirEmployees/viewDirEmployees", "Sklad/Object/Dir/DirEmployees/viewDirEmployeeHistories",
        "Sklad/Object/Dir/DirWarehouses/viewDirWarehouses",
        "Sklad/Object/Dir/DirBonuses/viewDirBonuses", "Sklad/Object/Dir/DirBonus2es/viewDirBonus2es",
        "Sklad/Object/Dir/DirDiscounts/viewDirDiscounts",
        "Sklad/Object/Dir/DirContractors/viewDirContractors",
        "Sklad/Object/Dir/DirBanks/viewDirBanks", "Sklad/Object/Dir/DirBanks/viewDirBanksGrid",
        "Sklad/Object/Dir/DirCashOffices/viewDirCashOffices", "Sklad/Object/Dir/DirCashOffices/viewDirCashOfficesGrid",
        "Sklad/Object/Dir/DirCurrencies/viewDirCurrencies", "Sklad/Object/Dir/DirCurrencies/viewDirCurrencyHistories",
        "Sklad/Object/Dir/DirVats/viewDirVats",
        "Sklad/Object/Dir/DirChars/viewDirCharColours",
        "Sklad/Object/Dir/DirChars/viewDirCharMaterials",
        "Sklad/Object/Dir/DirChars/viewDirCharNames",
        "Sklad/Object/Dir/DirChars/viewDirCharSeasons",
        "Sklad/Object/Dir/DirChars/viewDirCharSexes",
        "Sklad/Object/Dir/DirChars/viewDirCharSizes",
        "Sklad/Object/Dir/DirChars/viewDirCharStyles",
        "Sklad/Object/Dir/DirChars/viewDirCharTextures",
        "Sklad/Object/Dir/DirDescriptions/viewDirDescriptions",
        "Sklad/Object/Dir/DirReturnTypes/viewDirReturnTypes",
        "Sklad/Object/Dir/DirMovementDescriptions/viewDirMovementDescriptions",
        "Sklad/Object/Dir/DirDomesticExpenses/viewDirDomesticExpenses",
        "Sklad/Object/Dir/DirOrdersStates/viewDirOrdersStates",
        //Dir - Service
        "Sklad/Object/Dir/DirServiceNomens/viewDirServiceNomens", "Sklad/Object/Dir/DirServiceNomens/viewDirServiceNomensWinEdit",
        "Sklad/Object/Dir/DirServiceNomens/viewDirServiceNomenCategories",
        "Sklad/Object/Dir/DirServiceContractors/viewDirServiceContractors",
        "Sklad/Object/Dir/DirServiceJobNomens/viewDirServiceJobNomens", "Sklad/Object/Dir/DirServiceJobNomens/viewDirServiceJobNomensWinEdit", "Sklad/Object/Dir/DirServiceJobNomens/viewDirServiceJobNomenPrices", "Sklad/Object/Dir/DirServiceJobNomens/viewDirServiceDiagnosticRresultsWin",
        "Sklad/Object/Dir/DirServiceComplects/viewDirServiceComplects",
        "Sklad/Object/Dir/DirServiceProblems/viewDirServiceProblems",
        "Sklad/Object/Dir/DirSmsTemplates/viewDirSmsTemplates",
        "Sklad/Object/Dir/DirServiceDiagnosticRresults/viewDirServiceDiagnosticRresults",
        "Sklad/Object/Dir/DirServiceNomens/viewDirServiceNomenTypicalFaults",

        //Doc
        "Sklad/Object/Doc/DocCashOfficeSums/viewDocCashOfficeSumsEdit", "Sklad/Object/Doc/DocBankSums/viewDocBankSumsEdit", "Sklad/Object/Doc/DocCashOfficeSums/viewDocCashOfficeSumMovements", "Sklad/Object/Doc/DocCashOfficeSums/viewDocCashOfficeSumMovementsEdit", 
        "Sklad/Object/Doc/DocPurches/viewDocPurches", "Sklad/Object/Doc/DocPurches/viewDocPurchesEdit", "Sklad/Object/Doc/DocPurches/viewDocPurchTabsEdit",
        "Sklad/Object/Doc/DocSales/viewDocSales", "Sklad/Object/Doc/DocSales/viewDocSalesEdit", "Sklad/Object/Doc/DocSales/viewDocSaleTabsEdit",
        "Sklad/Object/Doc/DocMovements/viewDocMovements", "Sklad/Object/Doc/DocMovements/viewDocMovementsEdit", "Sklad/Object/Doc/DocMovements/viewDocMovementTabsEdit", 
        "Sklad/Object/Doc/DocReturnVendors/viewDocReturnVendors", "Sklad/Object/Doc/DocReturnVendors/viewDocReturnVendorsEdit", "Sklad/Object/Doc/DocReturnVendors/viewDocReturnVendorTabsEdit",
        "Sklad/Object/Doc/DocActWriteOffs/viewDocActWriteOffs", "Sklad/Object/Doc/DocActWriteOffs/viewDocActWriteOffsEdit", "Sklad/Object/Doc/DocActWriteOffs/viewDocActWriteOffTabsEdit",
        "Sklad/Object/Doc/DocReturnsCustomers/viewDocReturnsCustomers", "Sklad/Object/Doc/DocReturnsCustomers/viewDocReturnsCustomersEdit", "Sklad/Object/Doc/DocReturnsCustomers/viewDocReturnsCustomerTabsEdit",
        "Sklad/Object/Doc/DocActOnWorks/viewDocActOnWorks", "Sklad/Object/Doc/DocActOnWorks/viewDocActOnWorksEdit", "Sklad/Object/Doc/DocActOnWorks/viewDocActOnWorkTabsEdit",
        "Sklad/Object/Doc/DocAccounts/viewDocAccounts", "Sklad/Object/Doc/DocAccounts/viewDocAccountsEdit", "Sklad/Object/Doc/DocAccounts/viewDocAccountTabsEdit",
        "Sklad/Object/Doc/DocInventories/viewDocInventories", "Sklad/Object/Doc/DocInventories/viewDocInventoriesEdit", "Sklad/Object/Doc/DocInventories/viewDocInventoryTabsEdit",
        'Sklad/Object/Doc/DocRetails/viewDocRetails', 'Sklad/Object/Doc/DocRetails/viewDocRetailsEdit', 'Sklad/Object/Doc/DocRetails/viewDocRetailTabsEdit',
        'Sklad/Object/Doc/DocRetailActWriteOffs/viewDocRetailActWriteOffsEdit',
        'Sklad/Object/Doc/DocRetailReturns/viewDocRetailReturns', 'Sklad/Object/Doc/DocRetailReturns/viewDocRetailReturnsEdit', 'Sklad/Object/Doc/DocRetailReturns/viewDocRetailReturnTabsEdit',
        "Sklad/Object/Doc/DocNomenRevaluations/viewDocNomenRevaluations", "Sklad/Object/Doc/DocNomenRevaluations/viewDocNomenRevaluationsEdit", "Sklad/Object/Doc/DocNomenRevaluations/viewDocNomenRevaluationTabsEdit",
        //Doc - Service
        "Sklad/Object/Doc/DocServicePurches/viewDocServicePurchesEdit", "Sklad/Object/Doc/DocServicePurches/viewDocServicePurchesSelect", //"Sklad/Object/Doc/DocServicePurches/viewDocServicePurches",
        "Sklad/Object/Doc/DocServicePurches/viewDocServiceWorkshops", //"Sklad/Object/Doc/DocServicePurches/viewDocServiceWorkshopsEdit",
        //"Sklad/Object/Doc/DocServicePurches/viewDocServiceOutputs", "Sklad/Object/Doc/DocServicePurches/viewDocServiceOutputsEdit",
        //"Sklad/Object/Doc/DocServicePurches/viewDocServiceArchives",
        "Sklad/Object/Doc/DocServicePurches/viewDocServiceWorkshopHistories",
        "Sklad/Object/Doc/DocServicePurches/viewDocServiceMasterSelect",
        "Sklad/Object/Doc/DocServicePurches/viewDocServiceWorkshopsDiscount",
        "Sklad/Object/Doc/DocServicePurches/viewDocServiceMovs", "Sklad/Object/Doc/DocServicePurches/viewDocServiceMovsEdit", "Sklad/Object/Doc/DocServicePurches/viewDocServiceMovTabsEdit",
        "Sklad/Object/Doc/DocServicePurches/viewDocServiceInvs", "Sklad/Object/Doc/DocServicePurches/viewDocServiceInvsEdit",
        //Doc - Second-Hands
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandPurchesEdit",
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandWorkshops",
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandWorkshopsInRetail",
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandSalesEdit", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandSaleTabsEdit", //"Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandRetailsEdit", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandRetailTabsEdit",
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandInvs", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandInvsEdit", //"Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandInventories", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandInventoriesEdit", 
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandReturnTabsEdit", //"Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandRetailReturnTabsEdit",
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandRetailActWriteOffsEdit",
        //"Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandMovements", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandMovementsEdit", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandMovementTabsEdit", 
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandMovs", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandMovsEdit", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandMovTabsEdit", 
        "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandRazbors", "Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandRazborNomens", 
        //Doc - Order
        "Sklad/Object/Doc/DocOrderInts/viewDocOrderInts", "Sklad/Object/Doc/DocOrderInts/viewDocOrderIntsEdit", "Sklad/Object/Doc/DocOrderInts/viewDocOrderIntsNomensEdit", "Sklad/Object/Doc/DocOrderInts/viewDocOrderIntsPurches",
        //Doc - Salary
        "Sklad/Object/Doc/DocSalaries/viewDocSalaries", "Sklad/Object/Doc/DocSalaries/viewDocSalariesEdit", "Sklad/Object/Doc/DocSalaries/viewDocSalaryTabsEdit",
        //Doc - Финансы
        'Sklad/Object/Doc/DocDomesticExpenses/viewDocDomesticExpenses', 'Sklad/Object/Doc/DocDomesticExpenses/viewDocDomesticExpensesEdit', 'Sklad/Object/Doc/DocDomesticExpenses/viewDocDomesticExpenseTabsEdit',

        //Doc - Logistics
        "Sklad/Object/Logistic/viewLogistics",

        //Pay
        "Sklad/Object/Pay/viewPay", "Sklad/Object/Pay/viewPayEdit",

        //Report
        "Sklad/Object/Report/viewDocServicePurchesReport",
        "Sklad/Object/Report/viewReportPriceList",
        "Sklad/Object/Report/viewReportProfit",
        "Sklad/Object/Report/viewReportRemnants",
        "Sklad/Object/Report/viewReportTotalTrade",
        "Sklad/Object/Report/viewReportBanksCashOffices",
        "Sklad/Object/Report/viewReportMovementNomen",
        "Sklad/Object/Report/viewReportLogistics",
        "Sklad/Object/Report/viewReportSalaries", "Sklad/Object/Report/viewReportSalariesWarehouses",

        //List
        "Sklad/Object/List/viewListObjectPFs",

        //Log
        "Sklad/Object/Log/viewLogServices",
        "Sklad/Object/Log/viewLogMovements", "Sklad/Object/Log/viewLogLogistics",
        "Sklad/Object/Log/viewLogOrderInts",
        "Sklad/Object/Log/viewLogSecondHands",

        //Service
        "Sklad/Object/Service/ExchangeData/viewImportsDocPurchesExcel",

        //ККМ
        "Sklad/Object/KKM/viewKKMAdding",

        //Start
        "Sklad/Object/Start/viewDirWarehouseSelect",
        //KKM Loader
        "Sklad/Object/Start/viewKKMLoader",

        //Sms
        "Sklad/Object/Sms/viewSms",

        //API
        "Sklad/Object/Service/API/viewAPI10",

        //IM
        "Sklad/Object/Service/IM/viewVitrinaInTrade",


        //Pattern
        "Sklad/Other/Pattern/viewComboBox", "Sklad/Other/Pattern/viewComboBoxPrompt", "Sklad/Other/Pattern/viewDateField", "Sklad/Other/Pattern/viewDateFieldFix", "Sklad/Other/Pattern/viewGridDir", "Sklad/Other/Pattern/viewGridDirNomen", "Sklad/Other/Pattern/viewGridDoc", "Sklad/Other/Pattern/viewGridNoTBar", "Sklad/Other/Pattern/viewTriggerDir", "Sklad/Other/Pattern/viewTriggerDirField", "Sklad/Other/Pattern/viewTriggerSearch", "Sklad/Other/Pattern/viewTreeCombo", "Sklad/Other/Pattern/viewTreeDir", "Sklad/Other/Pattern/viewDirNomensEdit", "Sklad/Other/Pattern/viewDirNomensComboEdit", "Sklad/Other/Pattern/viewDirServiceNomensEdit", "Sklad/Other/Pattern/viewDirServiceJobNomensEdit", "Sklad/Other/Pattern/viewGridRem", "Sklad/Other/Pattern/viewGridRem2", "Sklad/Other/Pattern/viewGridRem22", "Sklad/Other/Pattern/viewGridRemPurch",
        "Sklad/Other/Pattern/viewTriggerDirAll",
        "Sklad/Other/Pattern/viewGridServiceLog", "Sklad/Other/Pattern/viewGridSecondHandLog", "Sklad/Other/Pattern/viewGridSecondHandRazborLog",
        "Sklad/Other/Pattern/viewGridPF",
        "Sklad/Other/Pattern/viewGridDocRetail", "Sklad/Other/Pattern/viewTriggerDirRetail", "Sklad/Other/Pattern/viewTriggerDirFieldRetail", "Sklad/Other/Pattern/viewTreeDirRetail",
        "Sklad/Other/Pattern/viewGridService", 
        "Sklad/Other/Pattern/viewGridOrder", "Sklad/Other/Pattern/viewGridSecondHand", "Sklad/Other/Pattern/viewGridSecondHandRazbor",
        "Sklad/Other/Pattern/viewTriggerSearchGrid",
        "Sklad/Other/Pattern/viewGridMovementsLogistics", "Sklad/Other/Pattern/viewGridLogistics",
        "Sklad/Other/Pattern/viewDocOrderIntsPattern",

        "Sklad/Other/Pattern/viewComboBoxTree", 
        //"Ext.ux.TreeCombo", 

    ],
    models: [
        //Dir
        "Sklad/Object/Dir/DirNomens/modelDirNomensGrid", "Sklad/Object/Dir/DirNomens/modelDirNomensTree",
        "Sklad/Object/Dir/DirNomens/modelDirNomenHistoriesGrid",
        "Sklad/Object/Dir/DirNomens/modelDirNomenCategoriesGrid", "Sklad/Object/Dir/DirNomens/modelDirNomenCategoriesTree",
        "Sklad/Object/Dir/DirNomens/modelDirNomenTypesGrid", "Sklad/Object/Dir/DirNomens/modelDirNomenTypesTree",
        "Sklad/Object/Dir/DirCurrencies/modelDirCurrenciesGrid", "Sklad/Object/Dir/DirCurrencies/modelDirCurrenciesTree", "Sklad/Object/Dir/DirCurrencies/modelDirCurrencyHistoriesGrid",
        "Sklad/Object/Dir/DirEmployees/modelDirEmployeesGrid", "Sklad/Object/Dir/DirEmployees/modelDirEmployeesTree", "Sklad/Object/Dir/DirEmployees/modelDirEmployeeHistoriesGrid", "Sklad/Object/Dir/DirEmployees/modelDirEmployeeWarehousesGrid",
        "Sklad/Object/Dir/DirWarehouses/modelDirWarehousesGrid", "Sklad/Object/Dir/DirWarehouses/modelDirWarehousesTree",
        "Sklad/Object/Dir/DirBonuses/modelDirBonusesGrid", "Sklad/Object/Dir/DirBonuses/modelDirBonusesTree", "Sklad/Object/Dir/DirBonuses/modelDirBonusTabsGrid",
        "Sklad/Object/Dir/DirBonus2es/modelDirBonus2esGrid", "Sklad/Object/Dir/DirBonus2es/modelDirBonus2esTree", "Sklad/Object/Dir/DirBonus2es/modelDirBonus2TabsGrid",
        "Sklad/Object/Dir/DirDiscounts/modelDirDiscountsGrid", "Sklad/Object/Dir/DirDiscounts/modelDirDiscountsTree", "Sklad/Object/Dir/DirDiscounts/modelDirDiscountTabsGrid",
        "Sklad/Object/Dir/DirBanks/modelDirBanksGrid", "Sklad/Object/Dir/DirBanks/modelDirBanksTree",
        "Sklad/Object/Dir/DirBankSumTypes/modelDirBankSumTypesGrid",
        "Sklad/Object/Dir/DirCashOffices/modelDirCashOfficesGrid", "Sklad/Object/Dir/DirCashOffices/modelDirCashOfficesTree",
        "Sklad/Object/Dir/DirCashOfficeSumTypes/modelDirCashOfficeSumTypesGrid",
        "Sklad/Object/Dir/DirContractors/modelDirContractorsGrid", "Sklad/Object/Dir/DirContractors/modelDirContractorsTree",
        "Sklad/Object/Dir/DirContractor1Types/modelDirContractor1TypesGrid", "Sklad/Object/Dir/DirContractor1Types/modelDirContractor1TypesTree",
        "Sklad/Object/Dir/DirContractor2Types/modelDirContractor2TypesGrid", "Sklad/Object/Dir/DirContractor2Types/modelDirContractor2TypesTree",
        "Sklad/Object/Dir/DirVats/modelDirVatsGrid", "Sklad/Object/Dir/DirVats/modelDirVatsTree",
        "Sklad/Object/Dir/DirChars/modelDirCharColoursGrid", "Sklad/Object/Dir/DirChars/modelDirCharColoursTree",
        "Sklad/Object/Dir/DirChars/modelDirCharMaterialsGrid", "Sklad/Object/Dir/DirChars/modelDirCharMaterialsTree",
        "Sklad/Object/Dir/DirChars/modelDirCharNamesGrid", "Sklad/Object/Dir/DirChars/modelDirCharNamesTree",
        "Sklad/Object/Dir/DirChars/modelDirCharSeasonsGrid", "Sklad/Object/Dir/DirChars/modelDirCharSeasonsTree",
        "Sklad/Object/Dir/DirChars/modelDirCharSexesGrid", "Sklad/Object/Dir/DirChars/modelDirCharSexesTree",
        "Sklad/Object/Dir/DirChars/modelDirCharSizesGrid", "Sklad/Object/Dir/DirChars/modelDirCharSizesTree",
        "Sklad/Object/Dir/DirChars/modelDirCharStylesGrid", "Sklad/Object/Dir/DirChars/modelDirCharStylesTree",
        "Sklad/Object/Dir/DirChars/modelDirCharTexturesGrid", "Sklad/Object/Dir/DirChars/modelDirCharTexturesTree",
        "Sklad/Object/Dir/DirPaymentTypes/modelDirPaymentTypesGrid",
        "Sklad/Object/Dir/DirPriceTypes/modelDirPriceTypesGrid",
        "Sklad/Object/Dir/DirDescriptions/modelDirDescriptionsGrid", "Sklad/Object/Dir/DirDescriptions/modelDirDescriptionsTree",
        "Sklad/Object/Dir/DirReturnTypes/modelDirReturnTypesGrid", "Sklad/Object/Dir/DirReturnTypes/modelDirReturnTypesTree",
        "Sklad/Object/Dir/DirMovementDescriptions/modelDirMovementDescriptionsGrid", "Sklad/Object/Dir/DirMovementDescriptions/modelDirMovementDescriptionsTree",
        "Sklad/Object/Dir/DirMovementStatuses/modelDirMovementStatusesGrid", "Sklad/Object/Dir/DirMovementStatuses/modelDirMovementStatusesTree",
        "Sklad/Object/Dir/DirDomesticExpenses/modelDirDomesticExpensesGrid", "Sklad/Object/Dir/DirDomesticExpenses/modelDirDomesticExpensesTree",
        "Sklad/Object/Dir/DirOrdersStates/modelDirOrdersStatesGrid", "Sklad/Object/Dir/DirOrdersStates/modelDirOrdersStatesTree",
        //Dir - Service
        "Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomensGrid", "Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomensTree",
        "Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenCategoriesGrid", "Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenCategoriesTree",
        "Sklad/Object/Dir/DirServiceContractors/modelDirServiceContractorsGrid", "Sklad/Object/Dir/DirServiceContractors/modelDirServiceContractorsTree",
        "Sklad/Object/Dir/DirServiceStatuses/modelDirServiceStatusesGrid", "Sklad/Object/Dir/DirServiceStatuses/modelDirServiceStatusesTree",
        "Sklad/Object/Dir/DirServiceJobNomens/modelDirServiceJobNomensGrid", "Sklad/Object/Dir/DirServiceJobNomens/modelDirServiceJobNomensTree", "Sklad/Object/Dir/DirServiceJobNomens/modelDirServiceJobNomenHistoriesGrid",
        "Sklad/Object/Dir/DirServiceComplects/modelDirServiceComplectsGrid", "Sklad/Object/Dir/DirServiceComplects/modelDirServiceComplectsTree",
        "Sklad/Object/Dir/DirServiceProblems/modelDirServiceProblemsGrid", "Sklad/Object/Dir/DirServiceProblems/modelDirServiceProblemsTree",
        "Sklad/Object/Dir/DirSmsTemplates/modelDirSmsTemplatesGrid", "Sklad/Object/Dir/DirSmsTemplates/modelDirSmsTemplatesTree",
        "Sklad/Object/Dir/DirServiceDiagnosticRresults/modelDirServiceDiagnosticRresultsGrid", "Sklad/Object/Dir/DirServiceDiagnosticRresults/modelDirServiceDiagnosticRresultsTree",
        "Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenTypicalFaultsGrid", "Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenTypicalFaultsTree",
        "Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenPricesGrid", "Sklad/Object/Dir/DirServiceNomens/modelDirServiceNomenPricesTree",
        "Sklad/Object/Doc/DocServicePurches/modelDocServiceInvsGrid", "Sklad/Object/Doc/DocServicePurches/modelDocServiceInvTabsGrid",
        //Dir - Service
        "Sklad/Object/Dir/DirSecondHandStatuses/modelDirSecondHandStatusesGrid", "Sklad/Object/Dir/DirSecondHandStatuses/modelDirSecondHandStatusesTree",
        "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInvsGrid", "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInvTabsGrid", //"Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInventoriesGrid", "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandInventoryTabsGrid",

        //Doc
        "Sklad/Object/Doc/DocCashOfficeSums/modelDocCashOfficeSumMovementsGrid", 
        "Sklad/Object/Doc/DocPurches/modelDocPurchesGrid", "Sklad/Object/Doc/DocPurches/modelDocPurchTabsGrid",
        "Sklad/Object/Doc/DocSales/modelDocSalesGrid", "Sklad/Object/Doc/DocSales/modelDocSaleTabsGrid",
        "Sklad/Object/Doc/DocMovements/modelDocMovementsGrid", "Sklad/Object/Doc/DocMovements/modelDocMovementTabsGrid",
        "Sklad/Object/Doc/DocReturnVendors/modelDocReturnVendorsGrid", "Sklad/Object/Doc/DocReturnVendors/modelDocReturnVendorTabsGrid",
        "Sklad/Object/Doc/DocActWriteOffs/modelDocActWriteOffsGrid", "Sklad/Object/Doc/DocActWriteOffs/modelDocActWriteOffTabsGrid",
        "Sklad/Object/Doc/DocReturnsCustomers/modelDocReturnsCustomersGrid", "Sklad/Object/Doc/DocReturnsCustomers/modelDocReturnsCustomerTabsGrid",
        "Sklad/Object/Doc/DocActOnWorks/modelDocActOnWorksGrid", "Sklad/Object/Doc/DocActOnWorks/modelDocActOnWorkTabsGrid",
        "Sklad/Object/Doc/DocAccounts/modelDocAccountsGrid", "Sklad/Object/Doc/DocAccounts/modelDocAccountTabsGrid",
        "Sklad/Object/Doc/DocInventories/modelDocInventoriesGrid", "Sklad/Object/Doc/DocInventories/modelDocInventoryTabsGrid",
        "Sklad/Object/Doc/DocRetails/modelDocRetailsGrid", "Sklad/Object/Doc/DocRetails/modelDocRetailTabsGrid",
        "Sklad/Object/Doc/DocRetailReturns/modelDocRetailReturnsGrid", "Sklad/Object/Doc/DocRetailReturns/modelDocRetailReturnTabsGrid",
        "Sklad/Object/Doc/DocNomenRevaluations/modelDocNomenRevaluationsGrid", "Sklad/Object/Doc/DocNomenRevaluations/modelDocNomenRevaluationTabsGrid",
        //Doc - Service
        "Sklad/Object/Doc/DocServicePurches/modelDocServicePurchesGrid", "Sklad/Object/Doc/DocServicePurches/modelDocServicePurch1TabsGrid", "Sklad/Object/Doc/DocServicePurches/modelDocServicePurch2TabsGrid",
        "Sklad/Object/Doc/DocServicePurches/modelDocServiceMovsGrid", "Sklad/Object/Doc/DocServicePurches/modelDocServiceMovTabsGrid",
        //Doc - SecondHand
        //"Sklad/Object/Doc/DocServicePurches/modelDocServicePurchesGrid", "Sklad/Object/Doc/DocServicePurches/modelDocServicePurch1TabsGrid", "Sklad/Object/Doc/DocServicePurches/modelDocServicePurch2TabsGrid",
        "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandSalesGrid", //"Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRetailTabsGrid",
        "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovsGrid", "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovTabsGrid", //"Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovementsGrid", "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandMovementTabsGrid",
        "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandPurchesGrid",
        "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRazborsGrid", "Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRazbor2TabsGrid", //"Sklad/Object/Doc/DocSecondHandPurches/modelDocSecondHandRazborTabsGrid",
        //Doc - Order
        "Sklad/Object/Doc/DocOrderInts/modelDocOrderIntsGrid",
        //Doc - Salary
        "Sklad/Object/Doc/DocSalaries/modelDocSalariesGrid", "Sklad/Object/Doc/DocSalaries/modelDocSalaryTabsGrid",
        //Doc - Финансы
        "Sklad/Object/Doc/DocDomesticExpenses/modelDocDomesticExpensesGrid", "Sklad/Object/Doc/DocDomesticExpenses/modelDocDomesticExpenseTabsGrid",

        //Logistic
        "Sklad/Object/Logistic/modelLogisticsGrid", "Sklad/Object/Logistic/modelLogisticTabsGrid",

        //Pay
        "Sklad/Object/Pay/modelPayGrid",

        //Report
        "Sklad/Object/Report/modelReportTotalTrade",
        "Sklad/Object/Report/modelReportBanksCashOffices",
        "Sklad/Object/Report/modelDocServicePurchesReport",
        "Sklad/Object/Report/modelReportLogistics",
        "Sklad/Object/Report/modelReportSalaries", "Sklad/Object/Report/modelReportSalariesWarehouses",

        //Rem
        //"Sklad/Object/Doc/DocPurches/modelDocPurchesGrid", "Sklad/Object/Doc/DocPurches/modelDocPurchTabsGrid",
        "Sklad/Object/Rem/RemParties/modelRemPartiesGrid", //"Sklad/Object/Rem/RemParties/modelRem2PartiesGrid",
        "Sklad/Object/Rem/RemPartyMinuses/modelRemPartyMinusesGrid", //"Sklad/Object/Rem/RemPartyMinuses/modelRem2PartyMinusesGrid",
        //List
        "Sklad/Object/List/modelListObjectPFsGrid",
        /*"Sklad/Object/List/modelListLanguagesGrid",
        "Sklad/Object/List/modelListObjectPFTabsGrid",
        "Sklad/Object/List/modelListObjectFieldNamesGrid",*/

        //Log
        "Sklad/Object/Log/modelLogServicesGrid",
        "Sklad/Object/Log/modelLogMovementsGrid", "Sklad/Object/Log/modelLogLogisticsGrid",
        "Sklad/Object/Log/modelLogOrderIntsGrid",
        "Sklad/Object/Log/modelLogSecondHandsGrid",
        "Sklad/Object/Log/modelLogSecondHandRazborsGrid",
    ],
    stores: [
        //Dir
        "Sklad/Object/Dir/DirNomens/storeDirNomensGrid", "Sklad/Object/Dir/DirNomens/storeDirNomensTree",
        "Sklad/Object/Dir/DirNomens/storeDirNomenHistoriesGrid",
        "Sklad/Object/Dir/DirNomens/storeDirNomenCategoriesGrid", "Sklad/Object/Dir/DirNomens/storeDirNomenCategoriesTree",
        "Sklad/Object/Dir/DirNomens/storeDirNomenTypesGrid", "Sklad/Object/Dir/DirNomens/storeDirNomenTypesTree",
        "Sklad/Object/Dir/DirCurrencies/storeDirCurrenciesGrid", "Sklad/Object/Dir/DirCurrencies/storeDirCurrenciesTree", "Sklad/Object/Dir/DirCurrencies/storeDirCurrencyHistoriesGrid",
        "Sklad/Object/Dir/DirEmployees/storeDirEmployeesGrid", "Sklad/Object/Dir/DirEmployees/storeDirEmployeesTree", "Sklad/Object/Dir/DirEmployees/storeDirEmployeeHistoriesGrid", "Sklad/Object/Dir/DirEmployees/storeDirEmployeeWarehousesGrid",
        "Sklad/Object/Dir/DirWarehouses/storeDirWarehousesGrid", "Sklad/Object/Dir/DirWarehouses/storeDirWarehousesTree",
        "Sklad/Object/Dir/DirBonuses/storeDirBonusesGrid", "Sklad/Object/Dir/DirBonuses/storeDirBonusesTree", "Sklad/Object/Dir/DirBonuses/storeDirBonusTabsGrid",
        "Sklad/Object/Dir/DirBonus2es/storeDirBonus2esGrid", "Sklad/Object/Dir/DirBonus2es/storeDirBonus2esTree", "Sklad/Object/Dir/DirBonus2es/storeDirBonus2TabsGrid",
        "Sklad/Object/Dir/DirDiscounts/storeDirDiscountsGrid", "Sklad/Object/Dir/DirDiscounts/storeDirDiscountsTree", "Sklad/Object/Dir/DirDiscounts/storeDirDiscountTabsGrid",
        "Sklad/Object/Dir/DirBanks/storeDirBanksGrid", "Sklad/Object/Dir/DirBanks/storeDirBanksTree",
        "Sklad/Object/Dir/DirBankSumTypes/storeDirBankSumTypesGrid",
        "Sklad/Object/Dir/DirCashOffices/storeDirCashOfficesGrid", "Sklad/Object/Dir/DirCashOffices/storeDirCashOfficesTree",
        "Sklad/Object/Dir/DirCashOfficeSumTypes/storeDirCashOfficeSumTypesGrid",
        "Sklad/Object/Dir/DirContractors/storeDirContractorsGrid", "Sklad/Object/Dir/DirContractors/storeDirContractorsTree",
        "Sklad/Object/Dir/DirContractor1Types/storeDirContractor1TypesGrid", "Sklad/Object/Dir/DirContractor1Types/storeDirContractor1TypesTree",
        "Sklad/Object/Dir/DirContractor2Types/storeDirContractor2TypesGrid", "Sklad/Object/Dir/DirContractor2Types/storeDirContractor2TypesTree",
        "Sklad/Object/Dir/DirVats/storeDirVatsGrid", "Sklad/Object/Dir/DirVats/storeDirVatsTree",
        "Sklad/Object/Dir/DirChars/storeDirCharColoursGrid", "Sklad/Object/Dir/DirChars/storeDirCharColoursTree",
        "Sklad/Object/Dir/DirChars/storeDirCharMaterialsGrid", "Sklad/Object/Dir/DirChars/storeDirCharMaterialsTree",
        "Sklad/Object/Dir/DirChars/storeDirCharNamesGrid", "Sklad/Object/Dir/DirChars/storeDirCharNamesTree",
        "Sklad/Object/Dir/DirChars/storeDirCharSeasonsGrid", "Sklad/Object/Dir/DirChars/storeDirCharSeasonsTree",
        "Sklad/Object/Dir/DirChars/storeDirCharSexesGrid", "Sklad/Object/Dir/DirChars/storeDirCharSexesTree",
        "Sklad/Object/Dir/DirChars/storeDirCharSizesGrid", "Sklad/Object/Dir/DirChars/storeDirCharSizesTree",
        "Sklad/Object/Dir/DirChars/storeDirCharStylesGrid", "Sklad/Object/Dir/DirChars/storeDirCharStylesTree",
        "Sklad/Object/Dir/DirChars/storeDirCharTexturesGrid", "Sklad/Object/Dir/DirChars/storeDirCharTexturesTree",
        "Sklad/Object/Dir/DirPaymentTypes/storeDirPaymentTypesGrid",
        "Sklad/Object/Dir/DirPriceTypes/storeDirPriceTypesGrid",
        "Sklad/Object/Dir/DirDescriptions/storeDirDescriptionsGrid", "Sklad/Object/Dir/DirDescriptions/storeDirDescriptionsTree",
        "Sklad/Object/Dir/DirReturnTypes/storeDirReturnTypesGrid", "Sklad/Object/Dir/DirReturnTypes/storeDirReturnTypesTree",
        "Sklad/Object/Dir/DirMovementDescriptions/storeDirMovementDescriptionsGrid", "Sklad/Object/Dir/DirMovementDescriptions/storeDirMovementDescriptionsTree",
        "Sklad/Object/Dir/DirMovementStatuses/storeDirMovementStatusesGrid", "Sklad/Object/Dir/DirMovementStatuses/storeDirMovementStatusesTree",
        "Sklad/Object/Dir/DirDomesticExpenses/storeDirDomesticExpensesGrid", "Sklad/Object/Dir/DirDomesticExpenses/storeDirDomesticExpensesTree",
        "Sklad/Object/Dir/DirOrdersStates/storeDirOrdersStatesGrid", "Sklad/Object/Dir/DirOrdersStates/storeDirOrdersStatesTree",
        //Dir - Service
        "Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomensGrid", "Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomensTree",
        "Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenCategoriesGrid", "Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenCategoriesTree",
        "Sklad/Object/Dir/DirServiceContractors/storeDirServiceContractorsGrid", "Sklad/Object/Dir/DirServiceContractors/storeDirServiceContractorsTree",
        "Sklad/Object/Dir/DirServiceStatuses/storeDirServiceStatusesGrid", "Sklad/Object/Dir/DirServiceStatuses/storeDirServiceStatusesTree",
        "Sklad/Object/Dir/DirServiceJobNomens/storeDirServiceJobNomensGrid", "Sklad/Object/Dir/DirServiceJobNomens/storeDirServiceJobNomensTree", "Sklad/Object/Dir/DirServiceJobNomens/storeDirServiceJobNomenHistoriesGrid",
        "Sklad/Object/Dir/DirServiceComplects/storeDirServiceComplectsGrid", "Sklad/Object/Dir/DirServiceComplects/storeDirServiceComplectsTree",
        "Sklad/Object/Dir/DirServiceProblems/storeDirServiceProblemsGrid", "Sklad/Object/Dir/DirServiceProblems/storeDirServiceProblemsTree",
        "Sklad/Object/Dir/DirSmsTemplates/storeDirSmsTemplatesGrid", "Sklad/Object/Dir/DirSmsTemplates/storeDirSmsTemplatesTree",
        "Sklad/Object/Dir/DirServiceDiagnosticRresults/storeDirServiceDiagnosticRresultsGrid", "Sklad/Object/Dir/DirServiceDiagnosticRresults/storeDirServiceDiagnosticRresultsTree",
        "Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenTypicalFaultsGrid", "Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenTypicalFaultsTree",
        "Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenPricesGrid", "Sklad/Object/Dir/DirServiceNomens/storeDirServiceNomenPricesTree",
        "Sklad/Object/Doc/DocServicePurches/storeDocServiceInvsGrid", "Sklad/Object/Doc/DocServicePurches/storeDocServiceInvTabsGrid",
        //Dir - DirSecondHandStatuses
        "Sklad/Object/Dir/DirSecondHandStatuses/storeDirSecondHandStatusesGrid", "Sklad/Object/Dir/DirSecondHandStatuses/storeDirSecondHandStatusesTree",
        "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandInvsGrid", "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandInvTabsGrid", //"Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandInventoriesGrid", "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandInventoryTabsGrid",

        //Doc
        "Sklad/Object/Doc/DocPurches/storeDocPurchesGrid", "Sklad/Object/Doc/DocPurches/storeDocPurchTabsGrid", "Sklad/Object/Doc/DocCashOfficeSums/storeDocCashOfficeSumMovementsGrid", 
        "Sklad/Object/Doc/DocSales/storeDocSalesGrid", "Sklad/Object/Doc/DocSales/storeDocSaleTabsGrid",
        "Sklad/Object/Doc/DocMovements/storeDocMovementsGrid", "Sklad/Object/Doc/DocMovements/storeDocMovementTabsGrid",
        "Sklad/Object/Doc/DocReturnVendors/storeDocReturnVendorsGrid", "Sklad/Object/Doc/DocReturnVendors/storeDocReturnVendorTabsGrid",
        "Sklad/Object/Doc/DocActWriteOffs/storeDocActWriteOffsGrid", "Sklad/Object/Doc/DocActWriteOffs/storeDocActWriteOffTabsGrid",
        "Sklad/Object/Doc/DocReturnsCustomers/storeDocReturnsCustomersGrid", "Sklad/Object/Doc/DocReturnsCustomers/storeDocReturnsCustomerTabsGrid",
        "Sklad/Object/Doc/DocActOnWorks/storeDocActOnWorksGrid", "Sklad/Object/Doc/DocActOnWorks/storeDocActOnWorkTabsGrid",
        "Sklad/Object/Doc/DocAccounts/storeDocAccountsGrid", "Sklad/Object/Doc/DocAccounts/storeDocAccountTabsGrid",
        "Sklad/Object/Doc/DocInventories/storeDocInventoriesGrid", "Sklad/Object/Doc/DocInventories/storeDocInventoryTabsGrid",
        "Sklad/Object/Doc/DocRetails/storeDocRetailsGrid", "Sklad/Object/Doc/DocRetails/storeDocRetailTabsGrid",
        "Sklad/Object/Doc/DocRetailReturns/storeDocRetailReturnsGrid", "Sklad/Object/Doc/DocRetailReturns/storeDocRetailReturnTabsGrid",
        "Sklad/Object/Doc/DocNomenRevaluations/storeDocNomenRevaluationsGrid", "Sklad/Object/Doc/DocNomenRevaluations/storeDocNomenRevaluationTabsGrid",
        //Doc - Service
        "Sklad/Object/Doc/DocServicePurches/storeDocServicePurchesGrid", "Sklad/Object/Doc/DocServicePurches/storeDocServicePurch1TabsGrid", "Sklad/Object/Doc/DocServicePurches/storeDocServicePurch2TabsGrid",
        "Sklad/Object/Doc/DocServicePurches/storeDocServiceMovsGrid", "Sklad/Object/Doc/DocServicePurches/storeDocServiceMovTabsGrid", 
        //Doc - SecondHand
        "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandPurchesGrid", "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandPurch1TabsGrid", "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandPurch2TabsGrid",
        "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandSalesGrid", //"Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandRetailTabsGrid",
        "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandMovsGrid", "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandMovTabsGrid", //"Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandMovementsGrid", "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandMovementTabsGrid",
        "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandRazborsGrid", "Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandRazbor2TabsGrid", //"Sklad/Object/Doc/DocSecondHandPurches/storeDocSecondHandRazborTabsGrid",
        //Doc - Order
        "Sklad/Object/Doc/DocOrderInts/storeDocOrderIntsGrid",
        //Doc - Salary
        "Sklad/Object/Doc/DocSalaries/storeDocSalariesGrid", "Sklad/Object/Doc/DocSalaries/storeDocSalaryTabsGrid",
        //Doc - Финансы
        "Sklad/Object/Doc/DocDomesticExpenses/storeDocDomesticExpensesGrid", "Sklad/Object/Doc/DocDomesticExpenses/storeDocDomesticExpenseTabsGrid",

        //Logistic
        "Sklad/Object/Logistic/storeLogisticsGrid", "Sklad/Object/Logistic/storeLogisticTabsGrid",

        //Pay
        "Sklad/Object/Pay/storePayGrid",

        //Report
        "Sklad/Object/Report/storeReportTotalTrade",
        "Sklad/Object/Report/storeReportBanksCashOffices",
        "Sklad/Object/Report/storeDocServicePurchesReport",
        "Sklad/Object/Report/storeReportLogistics",
        "Sklad/Object/Report/storeReportSalaries", "Sklad/Object/Report/storeReportSalariesWarehouses",

        //Rem
        "Sklad/Object/Rem/RemParties/storeRemPartiesGrid", //"Sklad/Object/Rem/RemParties/storeRem2PartiesGrid",
        "Sklad/Object/Rem/RemPartyMinuses/storeRemPartyMinusesGrid", //"Sklad/Object/Rem/RemPartyMinuses/storeRem2PartyMinusesGrid",
        //List
        "Sklad/Object/List/storeListObjectPFsGrid",
        /*"Sklad/Object/List/storeListLanguagesGrid",
        "Sklad/Object/List/storeListObjectPFTabsGrid",
        "Sklad/Object/List/storeListObjectFieldNamesGrid",*/

        //Log
        "Sklad/Object/Log/storeLogServicesGrid",
        "Sklad/Object/Log/storeLogMovementsGrid", "Sklad/Object/Log/storeLogLogisticsGrid",
        "Sklad/Object/Log/storeLogOrderIntsGrid",
        "Sklad/Object/Log/storeLogSecondHandsGrid",
        "Sklad/Object/Log/storeLogSecondHandRazborsGrid",
    ],
    controllers: [
        //Container
        "Sklad/Container/controllerContainerHeader",
        //Sys
        "Sklad/Object/Sys/SysSettings/controllerSysSettingsEdit",
        //Dir
        "Sklad/Object/Dir/DirNomens/controllerDirNomens", "Sklad/Object/Dir/DirNomens/controllerDirNomensWinEdit", "Sklad/Object/Dir/DirNomens/controllerDirNomenRemParties", "Sklad/Object/Dir/DirNomens/controllerDirNomensSelect",
        "Sklad/Object/Dir/DirNomens/controllerDirNomenCategories",
        "Sklad/Object/Dir/DirNomens/controllerDirNomensWinComboEdit",
        "Sklad/Object/Dir/DirEmployees/controllerDirEmployees", "Sklad/Object/Dir/DirEmployees/controllerDirEmployeeHistories",
        "Sklad/Object/Dir/DirWarehouses/controllerDirWarehouses",
        "Sklad/Object/Dir/DirDiscounts/controllerDirDiscounts", "Sklad/Object/Dir/DirDiscounts/controllerDirDiscountTabs",
        "Sklad/Object/Dir/DirBonuses/controllerDirBonuses", "Sklad/Object/Dir/DirBonuses/controllerDirBonusTabs",
        "Sklad/Object/Dir/DirBonus2es/controllerDirBonus2es", "Sklad/Object/Dir/DirBonus2es/controllerDirBonus2Tabs",
        "Sklad/Object/Dir/DirContractors/controllerDirContractors",
        "Sklad/Object/Dir/DirBanks/controllerDirBanks", "Sklad/Object/Dir/DirBanks/controllerDirBanksGrid",
        "Sklad/Object/Dir/DirCashOffices/controllerDirCashOffices", "Sklad/Object/Dir/DirCashOffices/controllerDirCashOfficesGrid",
        "Sklad/Object/Dir/DirCurrencies/controllerDirCurrencies", "Sklad/Object/Dir/DirCurrencies/controllerDirCurrencyHistories",
        "Sklad/Object/Dir/DirVats/controllerDirVats",
        "Sklad/Object/Dir/DirChars/controllerDirCharColours",
        "Sklad/Object/Dir/DirChars/controllerDirCharMaterials",
        "Sklad/Object/Dir/DirChars/controllerDirCharNames",
        "Sklad/Object/Dir/DirChars/controllerDirCharSeasons",
        "Sklad/Object/Dir/DirChars/controllerDirCharSexes",
        "Sklad/Object/Dir/DirChars/controllerDirCharSizes",
        "Sklad/Object/Dir/DirChars/controllerDirCharStyles",
        "Sklad/Object/Dir/DirChars/controllerDirCharTextures",
        "Sklad/Object/Dir/DirDescriptions/controllerDirDescriptions",
        "Sklad/Object/Dir/DirReturnTypes/controllerDirReturnTypes",
        "Sklad/Object/Dir/DirMovementDescriptions/controllerDirMovementDescriptions",
        "Sklad/Object/Dir/DirDomesticExpenses/controllerDirDomesticExpenses",
        "Sklad/Object/Dir/DirOrdersStates/controllerDirOrdersStates",
        //Dir - Service
        "Sklad/Object/Dir/DirServiceNomens/controllerDirServiceNomens", "Sklad/Object/Dir/DirServiceNomens/controllerDirServiceNomensWinEdit",
        "Sklad/Object/Dir/DirServiceNomens/controllerDirServiceNomenCategories",
        "Sklad/Object/Dir/DirServiceContractors/controllerDirServiceContractors",
        "Sklad/Object/Dir/DirServiceJobNomens/controllerDirServiceJobNomens", "Sklad/Object/Dir/DirServiceJobNomens/controllerDirServiceJobNomensWinEdit", "Sklad/Object/Dir/DirServiceJobNomens/controllerDirServiceJobNomenPrices", "Sklad/Object/Dir/DirServiceJobNomens/controllerDirServiceDiagnosticRresultsWin",
        "Sklad/Object/Dir/DirServiceComplects/controllerDirServiceComplects",
        "Sklad/Object/Dir/DirServiceProblems/controllerDirServiceProblems",
        "Sklad/Object/Dir/DirSmsTemplates/controllerDirSmsTemplates",
        "Sklad/Object/Dir/DirServiceDiagnosticRresults/controllerDirServiceDiagnosticRresults",
        "Sklad/Object/Dir/DirServiceNomens/controllerDirServiceNomenTypicalFaults",

        //Doc
        "Sklad/Object/Doc/DocCashOfficeSums/controllerDocCashOfficeSumsEdit", "Sklad/Object/Doc/DocBankSums/controllerDocBankSumsEdit", "Sklad/Object/Doc/DocCashOfficeSums/controllerDocCashOfficeSumMovements", "Sklad/Object/Doc/DocCashOfficeSums/controllerDocCashOfficeSumMovementsEdit", 
        "Sklad/Object/Doc/DocPurches/controllerDocPurches", "Sklad/Object/Doc/DocPurches/controllerDocPurchesEdit", "Sklad/Object/Doc/DocPurches/controllerDocPurchTabsEdit",
        "Sklad/Object/Doc/DocSales/controllerDocSales", "Sklad/Object/Doc/DocSales/controllerDocSalesEdit", "Sklad/Object/Doc/DocSales/controllerDocSaleTabsEdit",
        "Sklad/Object/Doc/DocMovements/controllerDocMovements", "Sklad/Object/Doc/DocMovements/controllerDocMovementsEdit", "Sklad/Object/Doc/DocMovements/controllerDocMovementTabsEdit", 
        "Sklad/Object/Doc/DocReturnVendors/controllerDocReturnVendors", "Sklad/Object/Doc/DocReturnVendors/controllerDocReturnVendorsEdit", "Sklad/Object/Doc/DocReturnVendors/controllerDocReturnVendorTabsEdit",
        "Sklad/Object/Doc/DocActWriteOffs/controllerDocActWriteOffs", "Sklad/Object/Doc/DocActWriteOffs/controllerDocActWriteOffsEdit", "Sklad/Object/Doc/DocActWriteOffs/controllerDocActWriteOffTabsEdit",
        "Sklad/Object/Doc/DocReturnsCustomers/controllerDocReturnsCustomers", "Sklad/Object/Doc/DocReturnsCustomers/controllerDocReturnsCustomersEdit", "Sklad/Object/Doc/DocReturnsCustomers/controllerDocReturnsCustomerTabsEdit",
        "Sklad/Object/Doc/DocActOnWorks/controllerDocActOnWorks", "Sklad/Object/Doc/DocActOnWorks/controllerDocActOnWorksEdit", "Sklad/Object/Doc/DocActOnWorks/controllerDocActOnWorkTabsEdit",
        "Sklad/Object/Doc/DocAccounts/controllerDocAccounts", "Sklad/Object/Doc/DocAccounts/controllerDocAccountsEdit", "Sklad/Object/Doc/DocAccounts/controllerDocAccountTabsEdit",
        "Sklad/Object/Doc/DocInventories/controllerDocInventories", "Sklad/Object/Doc/DocInventories/controllerDocInventoriesEdit", "Sklad/Object/Doc/DocInventories/controllerDocInventoryTabsEdit",
        "Sklad/Object/Doc/DocNomenRevaluations/controllerDocNomenRevaluations", "Sklad/Object/Doc/DocNomenRevaluations/controllerDocNomenRevaluationsEdit", "Sklad/Object/Doc/DocNomenRevaluations/controllerDocNomenRevaluationTabsEdit",
        //Doc - Service
        "Sklad/Object/Doc/DocServicePurches/controllerDocServicePurchesEdit", "Sklad/Object/Doc/DocServicePurches/controllerDocServicePurchesSelect",//"Sklad/Object/Doc/DocServicePurches/controllerDocServicePurches",
        "Sklad/Object/Doc/DocServicePurches/controllerDocServiceWorkshops", //"Sklad/Object/Doc/DocServicePurches/controllerDocServiceWorkshopsEdit",
        //"Sklad/Object/Doc/DocServicePurches/controllerDocServiceOutputs", "Sklad/Object/Doc/DocServicePurches/controllerDocServiceOutputsEdit",
        //"Sklad/Object/Doc/DocServicePurches/controllerDocServiceArchives", 
        "Sklad/Object/Doc/DocServicePurches/controllerDocServiceWorkshopHistories",
        "Sklad/Object/Doc/DocServicePurches/controllerDocServiceMasterSelect",
        "Sklad/Object/Doc/DocServicePurches/controllerDocServiceWorkshopsDiscount",
        "Sklad/Object/Doc/DocServicePurches/controllerDocServiceMovs",
        "Sklad/Object/Doc/DocServicePurches/controllerDocServiceInvs",
        //Doc - Б/У
        "Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandPurchesEdit",
        "Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandWorkshops",
        "Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandWorkshopsInRetail",
        "Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandMovs", //"Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandMovements", //"Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandMovementsEdit", "Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandMovementTabsEdit", 
        "Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandInvs", //"Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandInventories",
        "Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandRazbors", "Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandRazborNomens",
        //Doc - OrderInt
        "Sklad/Object/Doc/DocOrderInts/controllerDocOrderIntsEdit", //Новый Заказ
        "Sklad/Object/Doc/DocOrderInts/controllerDocOrderInts", //Список Заказов
        "Sklad/Object/Doc/DocOrderInts/controllerDocOrderIntsPurches", //Цены + Поставщик
        //Doc - Salary
        "Sklad/Object/Doc/DocSalaries/controllerDocSalaries", "Sklad/Object/Doc/DocSalaries/controllerDocSalariesEdit", "Sklad/Object/Doc/DocSalaries/controllerDocSalaryTabsEdit",

        //Doc - Logistics
        "Sklad/Object/Logistic/controllerLogistics",

        //Pay
        "Sklad/Object/Pay/controllerPay", "Sklad/Object/Pay/controllerPayEdit",

        //Report
        "Sklad/Object/Report/controllerReportPriceList",
        "Sklad/Object/Report/controllerReportProfit",
        "Sklad/Object/Report/controllerReportRemnants",
        "Sklad/Object/Report/controllerDocServicePurchesReport",
        "Sklad/Object/Report/controllerReportTotalTrade",
        "Sklad/Object/Report/controllerReportBanksCashOffices",
        "Sklad/Object/Report/controllerReportMovementNomen",
        "Sklad/Object/Report/controllerReportLogistics",
        "Sklad/Object/Report/controllerReportSalaries", "Sklad/Object/Report/controllerReportSalariesWarehouses",

        //Service
        "Sklad/Object/Service/ExchangeData/controllerImportsDocPurchesExcel",
        "Sklad/Object/Service/IM/controllerVitrinaInTrade",

        //List
        "Sklad/Object/List/controllerListObjectPFs",

        //Log
        "Sklad/Object/Log/controllerLogServices",
        "Sklad/Object/Log/controllerLogMovements", "Sklad/Object/Log/controllerLogLogistics",
        "Sklad/Object/Log/controllerLogOrderInts",
        "Sklad/Object/Log/controllerLogSecondHands",

        //Sms
        "Sklad/Object/Sms/controllerSms",

        //KKM
        "Sklad/Object/KKM/controllerKKMAdding",

        //Start
        "Sklad/Object/Start/controllerDirWarehouseSelect",

        //API
        "Sklad/Object/Service/API/controllerAPI10",
    ],


    //MVVM для Контейнера и Объектов
    requires: [
        //Dir
        "PartionnyAccount.viewcontroller.Sklad/Object/Dir/DirEmployees/viewcontrollerDirEmployees",
        //Doc
        //Edit
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocReturnsCustomers/viewcontrollerDocReturnsCustomersEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocAll/viewcontrollerDocAllEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Dir/DirNomens/viewcontrollerDirNomens", "PartionnyAccount.viewcontroller.Sklad/Object/Dir/DirNomens/viewcontrollerDirNomensImg",
        //Doc - Retail
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetails/viewcontrollerDocRetails", "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetails/viewcontrollerDocRetailsEdit", "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetails/viewcontrollerDocRetailTabsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetailReturns/viewcontrollerDocRetailReturns", "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetailReturns/viewcontrollerDocRetailReturnsEdit", "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetailReturns/viewcontrollerDocRetailReturnTabsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetailActWriteOffs/viewcontrollerDocRetailActWriteOffsEdit",
        //Doc - Service
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocServicePurches/viewcontrollerDocServiceMovsEdit", 
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocServicePurches/viewcontrollerDocServiceMovTabsEdit", 
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocServicePurches/viewcontrollerDocServiceInvsEdit", 
        //Doc - Second-Hands
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandSalesEdit", //"PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandRetailsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandSaleTabsEdit", //"PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandRetailTabsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandReturnTabsEdit", //"PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandRetailReturnTabsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandRetailActWriteOffsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandMovsEdit", //"PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandMovementsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandMovTabsEdit", //"PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandMovementTabsEdit",
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandInvsEdit", //"PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandInventoriesEdit",
        //Doc - Финансы
        "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocDomesticExpenses/viewcontrollerDocDomesticExpenses", "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocDomesticExpenses/viewcontrollerDocDomesticExpensesEdit", "PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocDomesticExpenses/viewcontrollerDocDomesticExpenseTabsEdit",
        //IM
        "PartionnyAccount.viewcontroller.Sklad/Object/Service/IM/viewcontrollerVitrinaInTrade", 
    ],


    viewport: {
        autoMaximize: true
    },

    launch: function () {

        //Отключить проверку на соответствие "WAI-ARIA 1.0" рекомендаций (а то выводит в консели ошибки, что не соответствуют кнопки рекомендациям "WAI-ARIA 1.0")
        //forum: https://www.sencha.com/forum/showthread.php?303936-Button-issue-with-WAI-ARIA
        //help: https://docs.sencha.com/extjs/6.0/whats_new/6.0.0/extjs_upgrade_guide.html#Button
        Ext.enableAriaButtons = false;
        Ext.enableAriaPanels = false;

        var Viewport = Ext.create("Ext.container.Viewport", {
            id: "Viewport",
            layout: "border",
            style: 'background: #fff; text-align:left;',
            frame: true,
            items: [
                { xtype: "viewContainerHeader", id: "viewContainerHeader" },
                { xtype: "viewContainerFooter", id: "viewContainerFooter" },
                //{ xtype: "viewContainerLeft", id: "viewContainerLeft" },
                //{ xtype: "viewContainerRight", id: "viewContainerRight" },

                //Разные центральные панели, в зависимости от Интерфейса
                //{ xtype: "viewContainerCentral", id: "viewContainerCentral" },
            ]
        });

        //viewContainerHeader.defaultButtonUI = "default";

        if (InterfaceSystem == 1) {
            //Для оконного нужен скролин
            Viewport.add(
                Ext.create("widget.viewContainerCentralPanel", {
                    id: "viewContainerCentral",
                    bodyStyle: "background-image:url(../../../Scripts/sklad/images/Background/square.gif) !important",
                    autoScroll: true,
                })
            );
        }
        else if (InterfaceSystem == 3) {
            //Интерфейс "Панель" => "Ext.layout.container.Card (Wizard)"
            //http://dev.sencha.com/deploy/ext-4.0.0/examples/layout-browser/layout-browser.html

            Viewport.add(
                Ext.create("widget.viewContainerCentralPanel", {
                    id: "viewContainerCentral",
                    bodyStyle: "background-image:url(../../../Scripts/sklad/images/Background/square.gif) !important",
                    layout: 'card',
                    /*
                    bbar: ['->', {
                        xtype: 'button',
                        text: 'Предыдущее',
                        handler: function (but) {
                            if (!funInterfaceSystem3_prev()) { Ext.Msg.alert(lanOrgName, "Нет объектов!");}
                        }
                    }, {
                        xtype: 'button',
                        text: 'Далее',
                        handler: function (but) {
                            if (!funInterfaceSystem3_next(false)) { Ext.Msg.alert(lanOrgName, "Нет объектов!");}
                        }
                    }],
                    */
                })
            );
        }
        else {
            Viewport.add(
                Ext.create("widget.viewContainerCentralTab", {
                    id: "viewContainerCentral",
                    bodyStyle: "background-image:url(../../../Scripts/sklad/images/Background/square.gif) !important",
                })
            );
        }


        //Показываем форму с выбором склада (в котором будет работать сотрудник)
        var Params = [
            Viewport, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDirWarehouseSelect", Params);


        //Загружаем настройки
        Variables_SettingsRequest();

        //Прячем правое меню сообщений: "MessageRightPanel"
        //Ext.getCmp("viewContainerRight").collapse(Ext.Component.DIRECTION_LEFT, true);

        //Destroy the #appLoadingIndicator element
        Ext.get("loading").destroy(); Ext.get("loading-mask").destroy();
    }

});