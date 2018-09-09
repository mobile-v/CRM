//Верхний Тулбар
Ext.define("PartionnyAccount.controller.Sklad/Container/controllerContainerHeader", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Container/viewContainerHeader'],

    init: function () {
        this.control({

            //Настройки *** *** ***
            'viewContainerHeader menuitem#btnSettings': { click: this.onBtnSettings },
            'viewContainerHeader menuitem#btnMyCompany': { click: this.onBtnMyCompany },
            'viewContainerHeader menuitem#btnEmployees': { click: this.onBtnEmployees },
            'viewContainerHeader menuitem#btnHelp': { click: this.onBtnHelp },
            'viewContainerHeader menuitem#btnInfo': { click: this.onBtnInfo },
            'viewContainerHeader menuitem#viewImportsDocPurchesExcel': { click: this.onViewImportsDocPurchesExcel },
            'viewContainerHeader menuitem#viewAPI10': { click: this.onViewAPI10 },
            'viewContainerHeader menuitem#viewVitrinaInTrade': { click: this.onViewVitrinaInTrade },

            //Справочники *** *** ***
            'viewContainerHeader menuitem#btnNomens': { click: this.onBtnNomens },
            'viewContainerHeader menuitem#btnContractors': { click: this.onBtnContractors },
            'viewContainerHeader menuitem#btnWarehouses': { click: this.onBtnWarehouses },
            'viewContainerHeader menuitem#btnBanks': { click: this.onBtnBanks },
            'viewContainerHeader menuitem#btnCashOffices': { click: this.onBtnCashOffices },
            'viewContainerHeader menuitem#btnCurrencies': { click: this.onBtnCurrencies },
            'viewContainerHeader menuitem#btnVats': { click: this.onBtnVats },
            'viewContainerHeader menuitem#btnDiscounts': { click: this.onBtnDiscounts },
            'viewContainerHeader menuitem#btnBonuses': { click: this.onBtnBonuses },
            'viewContainerHeader menuitem#btnBonus2es': { click: this.onBtnBonus2es },
            'viewContainerHeader menuitem#btnNomenCategories': { click: this.onBtnNomenCategories },
            'viewContainerHeader menuitem#btnDirDescriptionDiscounts': { click: this.onBtnDirDescriptionDiscounts },
            'viewContainerHeader menuitem#btnCharColours': { click: this.onBtnCharColours },
            'viewContainerHeader menuitem#btnCharMaterials': { click: this.onBtnCharMaterials },
            'viewContainerHeader menuitem#btnCharNames': { click: this.onBtnCharNames },
            'viewContainerHeader menuitem#btnCharSeasons': { click: this.onBtnCharSeasons },
            'viewContainerHeader menuitem#btnCharSexes': { click: this.onBtnCharSexes },
            'viewContainerHeader menuitem#btnCharSizes': { click: this.onBtnCharSizes },
            'viewContainerHeader menuitem#btnCharStyles': { click: this.onBtnCharStyles },
            'viewContainerHeader menuitem#btnCharTextures': { click: this.onBtnCharTextures },
            //Сервис
            'viewContainerHeader menuitem#btnServiceNomens': { click: this.onBtnServiceNomens },
            'viewContainerHeader menuitem#btnServiceContractors': { click: this.onBtnServiceContractors },
            'viewContainerHeader menuitem#btnServiceNomenCategories': { click: this.onBtnServiceNomenCategories },
            'viewContainerHeader menuitem#btnServiceJobNomens': { click: this.onBtnServiceJobNomens },
            'viewContainerHeader menuitem#btnSmsTemplates': { click: this.onBtnSmsTemplates },
            'viewContainerHeader menuitem#btnServiceDiagnosticRresults': { click: this.onBtnServiceDiagnosticRresults },
            'viewContainerHeader menuitem#btnServiceNomenTypicalFaults': { click: this.onBtnServiceNomenTypicalFaults },

            //Торговля *** *** ***
            'viewContainerHeader menuitem#btnPurches': { click: this.onBtnPurches },
            'viewContainerHeader menuitem#btnSales': { click: this.onBtnSales },
            'viewContainerHeader menuitem#btnMovements': { click: this.onBtnMovements },
            'viewContainerHeader menuitem#btnReturnVendors': { click: this.onBtnReturnVendors },
            'viewContainerHeader menuitem#btnActWriteOffs': { click: this.onBtnActWriteOffs },
            'viewContainerHeader menuitem#btnReturnsCustomers': { click: this.onBtnReturnsCustomers },
            'viewContainerHeader menuitem#btnActOnWorks': { click: this.onBtnActOnWorks },
            'viewContainerHeader menuitem#btnAccounts': { click: this.onBtnAccounts },
            'viewContainerHeader menuitem#btnInventories': { click: this.onBtnInventories },
            //Розница
            'viewContainerHeader menuitem#btnDocRetails': { click: this.onBtnDocRetails }, //список
            'viewContainerHeader menuitem#btnDocRetailsEdit': { click: this.onBtnDocRetailsEdit }, //редактирование
            'viewContainerHeader menuitem#btnDocRetailReturns': { click: this.onBtnDocRetailReturns }, //список
            'viewContainerHeader menuitem#btnDocRetailReturnsEdit': { click: this.onBtnDocRetailReturnsEdit }, //редактирование
            //Переоценка
            'viewContainerHeader menuitem#btnDocNomenRevaluations': { click: this.onBtnDocNomenRevaluations }, //список
            //Отчет
            'viewContainerHeader menuitem#btnReportTotalTrade': { click: this.onBtnReportTotalTrade },
            'viewContainerHeader menuitem#btnReportMovementNomen': { click: this.onBtnReportMovementNomen },

            //Заказы *** *** ***
            'viewContainerHeader menuitem#btnOrderIntsNew': { click: this.onBtnOrderIntsNew },
            'viewContainerHeader menuitem#btnOrderInts': { click: this.onBtnOrderInts },
            //Справочник: SMS
            'viewContainerHeader menuitem#btnDirOrdersStates': { click: this.onBtnDirOrdersStates },
            'viewContainerHeader menuitem#btnDocOrderIntsSmsTemplates': { click: this.onBtnDocOrderIntsSmsTemplates },

            //Сервис *** *** ***
            'viewContainerHeader menuitem#btnServicePurches': { click: this.onBtnServicePurches },
            'viewContainerHeader menuitem#btnServiceWorkshops': { click: this.onBtnServiceWorkshops },
            'viewContainerHeader menuitem#btnServiceOutputs': { click: this.onBtnServiceOutputs },
            'viewContainerHeader menuitem#btnServiceArchives': { click: this.onBtnServiceArchives },
            'viewContainerHeader menuitem#btnDocServiceMovements': { click: this.onBtnDocServiceMovements },
            'viewContainerHeader menuitem#btnServiceInventoriesEdit': { click: this.onBtnServiceInventoriesEdit },
            'viewContainerHeader menuitem#btnServicePurchesReport': { click: this.onBtnServicePurchesReport },

            //Б/У *** *** ***
            'viewContainerHeader menuitem#btnSecondHandPurches': { click: this.onBtnSecondHandPurches },
            'viewContainerHeader menuitem#btnSecondHandWorkshops': { click: this.onBtnSecondHandWorkshops },
            'viewContainerHeader menuitem#btnSecondHandRetailsEdit': { click: this.onBtnSecondHandRetailsEdit },
            'viewContainerHeader menuitem#btnSecondHandInventoriesEdit': { click: this.onBtnSecondHandInventoriesEdit },
            'viewContainerHeader menuitem#btnSecondHandRazborsEdit': { click: this.onBtnSecondHandRazborsEdit },
            'viewContainerHeader menuitem#btnSecondHandMovements': { click: this.onBtnSecondHandMovements },
            'viewContainerHeader menuitem#btnSecondHandsReport': { click: this.onBtnSecondHandsReport },
            'viewContainerHeader menuitem#btnServiceJobNomens1': { click: this.onBtnServiceJobNomens1 },

            //Деньги: Касса + Банк *** *** ***
            //Касса
            'viewContainerHeader menuitem#btnCashOfficesEdit': { click: this.onBtnCashOfficesEdit },
            'viewContainerHeader menuitem#btnReportSalaries': { click: this.onBtnReportSalaries },
            'viewContainerHeader menuitem#btnReportSalariesWarehouses': { click: this.onBtnReportSalariesWarehouses },
            //Банк
            'viewContainerHeader menuitem#btnBanksEdit': { click: this.onBtnBanksEdit },
            //Документ
            'viewContainerHeader menuitem#btnCashOfficeSumMovements': { click: this.onBtnCashOfficeSumMovements },
            //Справочник
            'viewContainerHeader menuitem#btnDirDomesticExpenses': { click: this.onDirDomesticExpenses },
            'viewContainerHeader menuitem#btnDocDomesticExpenses': { click: this.onDocDomesticExpenses },
            'viewContainerHeader menuitem#btnDocDomesticExpenseSalaries': { click: this.onDocDomesticExpenseSalaries },
            //Отчет
            'viewContainerHeader menuitem#btnReportBanksCashOffices': { click: this.onReportBanksCashOffices },

            //Логистика *** *** ***
            //Торговля
            'viewContainerHeader menuitem#btnMovementsLogisticsNew': { click: this.onBtnMovementsLogisticsNew },
            //БУ
            'viewContainerHeader menuitem#btnSecondHandMovementsLogisticsNew': { click: this.onBtnSecondHandMovementsLogisticsNew },
            //Список
            'viewContainerHeader menuitem#btnLogistics': { click: this.onBtnLogistics },
            //Отчет
            'viewContainerHeader menuitem#btnDocMovementsLogisticsReport': { click: this.onBtnDocMovementsLogisticsReport },
            //Справочник: SMS
            'viewContainerHeader menuitem#btnDocMovementsLogisticsSmsTemplates': { click: this.onBtnDocMovementsLogisticsSmsTemplates },

            //ККМ *** *** ***
            //X-отчет
            'viewContainerHeader menuitem#btnKKMXReport': { click: this.onBtnKKMXReport },
            //Открытие смены
            'viewContainerHeader menuitem#btnKKMOpen': { click: this.onBtnKKMOpen },
            //Внесение денег в кассу
            'viewContainerHeader menuitem#btnKKMAdding': { click: this.onBtnKKMAdding },
            //Инкассация денег из кассы
            'viewContainerHeader menuitem#btnKKMEncashment': { click: this.onBtnKKMEncashment },
            //Закрытие смены
            'viewContainerHeader menuitem#btnKKMClose': { click: this.onBtnKKMClose },
            //Печать состояния расчетов и связи с ОФД
            'viewContainerHeader menuitem#btnKKMPrintOFD': { click: this.onBtnKKMPrintOFD },
            //Получить данные последнего чека из ФН
            'viewContainerHeader menuitem#btnKKMCheckLastFN': { click: this.onBtnKKMCheckLastFN },
            //Получить текущее состояние ККТ
            'viewContainerHeader menuitem#btnKKMState': { click: this.onBtnKKMState },
            //Получение списка ККМ
            'viewContainerHeader menuitem#btnKKMList': { click: this.onBtnKKMList },

            //Торговля *** *** ***

            //Отчеты *** *** ***
            'viewContainerHeader menuitem#btnReportPriceList': { click: this.onBtnReportPriceList },
            'viewContainerHeader menuitem#btnReportProfit': { click: this.onBtnReportProfit },
            'viewContainerHeader menuitem#btnReportRemnants': { click: this.onBtnReportRemnants },

        });
    },


    //Настройки *** *** ***

    onBtnSettings: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            true, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewSysSettingsEdit", Params);
    },
    onBtnMyCompany: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,
            undefined,
            undefined,
            undefined,
            "DirContractor2TypeID1=1"
        ]
        ObjectConfig("viewDirContractors", Params);
    },
    onBtnEmployees: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirEmployees", Params);
    },
    onBtnHelp: function (btn) {
        var Params = ["viewContainerHeader"];
        ObjectConfig("viewMain", Params);
    },
    onBtnInfo: function (btn) {
        /*
        var Params = ["viewContainerHeader"];
        ObjectConfig("viewMain", Params);
        */

        var varDirPayServiceNameHtml = "<font size=" + HeaderMenu_FontSize_1 + "><b>Тарифный план:</b> " + varDirPayServiceName + "</font><br /";
        if (varPaymentExpired) varDirPayServiceNameHtml = "<font size=" + HeaderMenu_FontSize_1 + "><b><b style='color:red'>Тарифный план:</b> " + varDirPayServiceName + "</font><br />";

        Ext.Msg.alert(
            lanOrgName,
            "<font size=" + HeaderMenu_FontSize_1 + "><b>" + "Логин" + ":</b> " + varDirEmployeeLogin + "</font><br />" +
            "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanEmployee + ":</b> " + lanDirEmployeeName + "</font><br />" +
            varDirPayServiceNameHtml
            );
    },
    onViewImportsDocPurchesExcel: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            true, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewImportsDocPurchesExcel", Params);
    },
    onViewAPI10: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            true, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewAPI10", Params);
    },
    onViewVitrinaInTrade: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            true, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewVitrinaInTrade", Params);
    },


    //Справочники *** *** ***

    onBtnNomens: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirNomens", Params);
    },
    onBtnContractors: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirContractors", Params);
    },
    onBtnWarehouses: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirWarehouses", Params);
    },
    onBtnBanks: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirBanks", Params);
    },
    onBtnCashOffices: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCashOffices", Params);
    },
    onBtnCurrencies: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCurrencies", Params);
    },
    onBtnVats: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirVats", Params);
    },
    onBtnDiscounts: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirDiscounts", Params);
    },
    onBtnBonuses: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirBonuses", Params);
    },
    onBtnBonus2es: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirBonus2es", Params);
    },
    onBtnNomenCategories: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirNomenCategories", Params);
    },
    onBtnDirDescriptionDiscounts: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirDescriptionDiscounts", Params);
    },
    onBtnCharColours: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharColours", Params);
    },
    onBtnCharMaterials: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharMaterials", Params);
    },
    onBtnCharNames: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharNames", Params);
    },
    onBtnCharSeasons: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharSeasons", Params);
    },
    onBtnCharSexes: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharSexes", Params);
    },
    onBtnCharSizes: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharSizes", Params);
    },
    onBtnCharStyles: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharStyles", Params);
    },
    onBtnCharTextures: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCharTextures", Params);
    },
    //Сервис
    onBtnServiceNomens: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirServiceNomens", Params);
    },
    onBtnServiceContractors: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirServiceContractors", Params);
    },
    onBtnServiceNomenCategories: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirServiceNomenCategories", Params);
    },
    onBtnServiceJobNomens: function (btn) {

        var TreeServerParam1 = "DirServiceJobNomenType=1"; //2 - БУ

        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,     // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            TreeServerParam1
        ]
        ObjectConfig("viewDirServiceJobNomens", Params);
    },
    onBtnSmsTemplates: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,     // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            1
        ]
        ObjectConfig("viewDirSmsTemplates", Params);
    },
    onBtnServiceDiagnosticRresults: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,     // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            1
        ]
        ObjectConfig("viewDirServiceDiagnosticRresults", Params);
    },
    onBtnServiceNomenTypicalFaults: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,     // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            1
        ]
        ObjectConfig("viewDirServiceNomenTypicalFaults", Params);
    },


    //Торговля *** *** ***

    onBtnPurches: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocPurches", Params);
    },
    onBtnSales: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocSales", Params);
    },
    onBtnMovements: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocMovements", Params);
    },
    onBtnReturnVendors: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocReturnVendors", Params);
    },
    onBtnActWriteOffs: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocActWriteOffs", Params);
    },
    onBtnReturnsCustomers: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocReturnsCustomers", Params);
    },
    onBtnActOnWorks: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocActOnWorks", Params);
    },
    onBtnAccounts: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocAccounts", Params);
    },
    onBtnInventories: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocInventories", Params);
    },
    //Розница
    onBtnDocRetails: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocRetails", Params);
    },
    onBtnDocRetailsEdit: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocRetailsEdit", Params);
    },
    onBtnDocRetailReturns: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocRetailReturns", Params);
    },
    onBtnDocRetailReturnsEdit: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocRetailReturnsEdit", Params);
    },
    //Переоценка
    onBtnDocNomenRevaluations: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocNomenRevaluations", Params);
    },


    //Сервис *** *** ***

    onBtnServicePurches: function (btn) {
        /*
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocServicePurches", Params);
        */

        var Params = [
            "viewContainerHeader", //"grid_" + aButton.UO_id, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocServicePurchesEdit", Params);
    },
    onBtnServiceWorkshops: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocServiceWorkshops", Params);
    },
    onBtnServiceOutputs: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocServiceOutputs", Params);
    },
    onBtnServiceArchives: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocServiceArchives", Params);
    },
    onBtnDocServiceMovements: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocServiceMovs", Params);
    },
    onBtnServiceInventoriesEdit: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocServiceInvs", Params); //ObjectConfig("viewDocSecondHandInventories", Params);
    },
    onBtnServicePurchesReport: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewDocServicePurchesReport", Params);
    },

    //Б/У *** *** ***

    onBtnSecondHandPurches: function (btn) {

        var Params = [
            "viewContainerHeader", //"grid_" + aButton.UO_id, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocSecondHandPurchesEdit", Params);
    },
    onBtnSecondHandWorkshops: function (btn) {

        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocSecondHandWorkshops", Params);
    },

    onBtnSecondHandRetailsEdit: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocSecondHandSalesEdit", Params);
    },
    onBtnSecondHandInventoriesEdit: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocSecondHandInvs", Params); //ObjectConfig("viewDocSecondHandInventories", Params);
    },
    onBtnSecondHandRazborsEdit: function (btn) {
        //alert("Документ 'Разборка аппарата' будет по типу БУ.ППП.");

        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocSecondHandRazbors", Params);
    },
    onBtnSecondHandMovements: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocSecondHandMovs", Params);
    },
    onBtnSecondHandsReport: function (btn) {

        //...

    },
    onBtnServiceJobNomens1: function (btn) {

        var TreeServerParam1 = "DirServiceJobNomenType=2"; //2 - БУ

        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,     // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            TreeServerParam1
        ]
        ObjectConfig("viewDirServiceJobNomens", Params);
    },

    //Заказы *** *** ***
    onBtnOrderIntsNew: function (btn) {
        //Откроется форма Заказа
        //В которой будет вся информация о Аппарате взятом на ремонт и Зап.части
        var Params = [
            "viewContainerHeader", //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,     // 1 - Новое, 2 - Редактировать
            undefined,
            1,  //Содержит тип заказа: 1 - Из Мастерской, 2 - Из Магазина, 3 - Впрок
            //IdcallModelData[0], //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
        ]
        ObjectEditConfig("viewDocOrderIntsNomensEdit", Params);
    },
    onBtnOrderInts: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocOrderInts", Params);
    },
    onBtnDirOrdersStates: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,     // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            3     //MenuID=3
        ]
        ObjectConfig("viewDirOrdersStates", Params);
    },
    onBtnDocOrderIntsSmsTemplates: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,     // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            3     //MenuID=3
        ]
        ObjectConfig("viewDirSmsTemplates", Params);
    },

    //Деньги: Касса + Банк *** *** ***
    //Касса
    onBtnCashOfficesEdit: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCashOfficesGrid", Params);
    },
    //Банк
    onBtnBanksEdit: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirBanksGrid", Params);
    },
    //Документ Перемещение Финансы
    onBtnCashOfficeSumMovements: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocCashOfficeSumMovements", Params);
    },
    //Справочник
    onDirDomesticExpenses: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirDomesticExpenses", Params);
    },
    onDocDomesticExpenses: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            1,          //1 - Новое, 2 - Редактировать
            undefined,  //UO_GridSave
            1           //UO_GridIndex
        ];
        ObjectEditConfig("viewDocDomesticExpensesEdit", Params);
    },
    onDocDomesticExpenseSalaries: function (btn) {
        var Params = [
            "viewContainerHeader",
            false,      //UO_Center
            false,      //UO_Modal
            1,          //1 - Новое, 2 - Редактировать
            undefined,  //UO_GridSave
            2           //UO_GridIndex
        ];
        ObjectEditConfig("viewDocDomesticExpensesEdit", Params);
    },
    //Отчет
    onReportBanksCashOffices: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportBanksCashOffices", Params);
    },
    onBtnReportSalaries: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportSalaries", Params);
    },
    onBtnReportSalariesWarehouses: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportSalariesWarehouses", Params);
    },

    //Логистика *** *** ***
    onBtnMovementsLogisticsNew: function (btn) {

        var Params = [
            "viewContainerHeader", //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            1,     // 1 - Новое, 2 - Редактировать
            true,  //Признак того, что это Логистика, а не Перемещение
        ]
        ObjectEditConfig("viewDocMovementsEdit", Params);
    },
    onBtnSecondHandMovementsLogisticsNew: function (btn) {

        var Params = [
            "viewContainerHeader", //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            1,     // 1 - Новое, 2 - Редактировать
            true,  //Признак того, что это Логистика, а не Перемещение
        ]
                        //viewDocSecondHandMovementsEdit
        ObjectEditConfig("viewDocSecondHandMovementsEdit", Params);
    },
    onBtnLogistics: function (btn) {
        
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
        ]
        ObjectConfig("viewLogistics", Params);
    },
    onBtnDocMovementsLogisticsReport: function (btn) {

        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportLogistics", Params);
    },
    onBtnDocMovementsLogisticsSmsTemplates: function (btn) {
        var Params = [
            "viewContainerHeader",
            false, //UO_Center
            false, //UO_Modal
            undefined,     // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            2     //MenuID=2
        ]
        ObjectConfig("viewDirSmsTemplates", Params);
    },

    //ККМ *** *** ***
    //Х-отчет
    onBtnKKMXReport: function (btn) {

        Ext.Msg.confirm("Confirmation", "Выполнить Х-Отчет?", function (btnText) {
            if (btnText === "no") {
                //...
            }
            else if (btnText === "yes") {
                XReport();
            }
        }, this);

    },
    //Открытие смены
    onBtnKKMOpen: function (btn) {

        Ext.Msg.confirm("Confirmation", "Открыть смену?", function (btnText) {
            if (btnText === "no") {
                //...
            }
            else if (btnText === "yes") {
                OpenShift(false);
            }
        }, this);
        
    },
    //Внесение денег в кассу
    onBtnKKMAdding: function (btn) {
        //DepositingCash(111, true);

        //Вывести форму "Внесение денег в Кассу"
        //Все остальные кнопки заблокированны

        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            true, //UO_Modal

            //*** *** *** *** ***
            // 1 - Новое, 2 - Редактировать
            //-------------------
            //1 - Внесение денег в кассу
            //2 - Инкассация денег из кассы
            //-------------------
            1     
        ];
        ObjectEditConfig("viewKKMAdding", Params);
    },
    //Инкассация денег из кассы
    onBtnKKMEncashment: function (btn) {

        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            true, //UO_Modal

            //*** *** *** *** ***
            // 1 - Новое, 2 - Редактировать
            //-------------------
            //1 - Внесение денег в кассу
            //2 - Инкассация денег из кассы
            //-------------------
            2
        ];
        ObjectEditConfig("viewKKMAdding", Params);

    },
    //Закрытие смены
    onBtnKKMClose: function (btn) {

        Ext.Msg.confirm("Confirmation", "Закрытие смены?", function (btnText) {
            if (btnText === "no") {
                //...
            }
            else if (btnText === "yes") {
                CloseShift(false)
            }
        }, this);

    },
    //Печать состояния расчетов и связи с ОФД
    onBtnKKMPrintOFD: function (btn) {

        Ext.Msg.confirm("Confirmation", "Печать состояния расчетов и связи с ОФД?", function (btnText) {
            if (btnText === "no") {
                //...
            }
            else if (btnText === "yes") {
                OfdReport();
            }
        }, this);

    },
    //Получить данные последнего чека из ФН
    onBtnKKMCheckLastFN: function (btn) {

        Ext.Msg.confirm("Confirmation", "Получить данные последнего чека из ФН?", function (btnText) {
            if (btnText === "no") {
                //...
            }
            else if (btnText === "yes") {
                GetDataCheck(0, 1);
            }
        }, this);

    },
    //Получить текущее состояние ККТ
    onBtnKKMState: function (btn) {

        Ext.Msg.confirm("Confirmation", "Получить текущее состояние ККТ?", function (btnText) {
            if (btnText === "no") {
                //...
            }
            else if (btnText === "yes") {
                GetDataKKT();
            }
        }, this);

    },
    //Получение списка ККМ
    onBtnKKMList: function (btn) {

        Ext.Msg.confirm("Confirmation", "Получить данные последнего чека из ФН?", function (btnText) {
            if (btnText === "no") {
                //...
            }
            else if (btnText === "yes") {
                List(0);
            }
        }, this);

    },

    //Отчеты *** *** ***
    onBtnReportPriceList: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportPriceList", Params);
    },
    onBtnReportProfit: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportProfit", Params);
    },
    onBtnReportRemnants: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportRemnants", Params);
    },
    onBtnReportTotalTrade: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportTotalTrade", Params);
    },
    onBtnReportMovementNomen: function (btn) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportMovementNomen", Params);
    },
    
});
