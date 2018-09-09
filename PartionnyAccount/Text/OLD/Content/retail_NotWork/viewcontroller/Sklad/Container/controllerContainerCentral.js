Ext.define('PartionnyAccount.viewcontroller.Sklad/Container/controllerContainerCentral', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.controllerContainerCentral',

    //Чек *** *** ***
    onBtnDocRetailsClick: function (button, pressed) {
        var Params = [
            "viewContainerCentral", //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocRetails", Params);
    },
    onBtnDocRetailsEditClick: function (button, pressed) {
        var Params = [
            "viewContainerCentral", //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocRetailsEdit", Params);
    },

    //Возврат *** *** ***
    onBtnDocRetailReturnsClick: function (button, pressed) {
        var Params = [
            "viewContainerCentral", //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDocRetailReturns", Params);
    },
    onBtnDocRetailReturnsEditClick: function (button, pressed) {
        var Params = [
            "viewContainerCentral", //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocRetailReturnsEdit", Params);
    },

    //Отчет *** *** ***
    onBtnReportCashClick: function (button, pressed) {
        var Params = [
            "viewContainerHeader",
            true, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectReportConfig("viewReportRetailCash", Params);
    },

});