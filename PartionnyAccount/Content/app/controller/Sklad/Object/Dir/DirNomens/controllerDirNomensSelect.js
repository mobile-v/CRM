Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirNomens/controllerDirNomensSelect", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirNomensSelect': { close: this.this_close },


            
            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirNomensSelect button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirNomensSelect button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirNomensSelect button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },




    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //Форма на Виджете
        var widgetX = Ext.getCmp("viewDirNomensSelect" + aButton.UO_id);


        //Меняем цены
        var PriceCurrency = parseFloat(Ext.getCmp("PriceCurrency" + aButton.UO_id).getValue());
        var PriceVAT = PriceCurrency / parseFloat(widgetX.UO_Param_fn.data.DirCurrencyRate) * parseFloat(widgetX.UO_Param_fn.data.DirCurrencyMultiplicity);
        widgetX.UO_Param_fn.data.PriceRetailVAT = PriceVAT;
        widgetX.UO_Param_fn.data.PriceRetailCurrency = PriceCurrency;
        widgetX.UO_Param_fn.data.PriceWholesaleVAT = PriceVAT;
        widgetX.UO_Param_fn.data.PriceWholesaleCurrency = PriceCurrency;
        widgetX.UO_Param_fn.data.PriceIMVAT = PriceVAT;
        widgetX.UO_Param_fn.data.PriceIMCurrency = PriceCurrency;

        Ext.getCmp(widgetX.UO_idTab).UO_Function_Grid(widgetX.UO_GridRecord, widgetX.UO_Param_id, widgetX.UO_Param_fn);
        Ext.getCmp(widgetX.UO_idTab).close();
        widgetX.close();
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-tovar/", '_blank');
    }
});