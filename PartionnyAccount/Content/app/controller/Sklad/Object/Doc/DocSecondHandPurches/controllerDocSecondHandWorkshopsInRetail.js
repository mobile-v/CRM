Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandWorkshopsInRetail", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocSecondHandWorkshopsInRetail': { close: this.this_close },
            


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            /*
            'viewDocSecondHandWorkshopsInRetail button#btnSave1': { "click": this.onBtnSaveClick },
            'viewDocSecondHandWorkshopsInRetail button#btnSave2': { "click": this.onBtnSaveClick },
            */
            'viewDocSecondHandWorkshopsInRetail button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocSecondHandWorkshopsInRetail button#btnCancel': { "click": this.onBtnCancelClick },
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
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);
        //Форма
        var form = widgetXForm.getForm();
        //Валидация
        if (!form.isValid()) {
            Ext.Msg.alert(lanOrgName, "Пожалуйста, заполните все поля формы!");
            return;
        }

        
        if (
            (
                parseFloat(Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue()) > 0 &&
                parseFloat(Ext.getCmp("PriceWholesaleVAT" + aButton.UO_id).getValue()) > 0 &&
                parseFloat(Ext.getCmp("PriceIMVAT" + aButton.UO_id).getValue()) > 0
            )
            &&
            (
                parseFloat(Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue()) > parseFloat(Ext.getCmp("PriceVATSums" + aButton.UO_id).getValue()) &&
                parseFloat(Ext.getCmp("PriceWholesaleVAT" + aButton.UO_id).getValue()) > parseFloat(Ext.getCmp("PriceVATSums" + aButton.UO_id).getValue()) &&
                parseFloat(Ext.getCmp("PriceIMVAT" + aButton.UO_id).getValue()) > parseFloat(Ext.getCmp("PriceVATSums" + aButton.UO_id).getValue())
            )
           ) {

            var PriceX3 = [Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue(), Ext.getCmp("PriceWholesaleVAT" + aButton.UO_id).getValue(), Ext.getCmp("PriceIMVAT" + aButton.UO_id).getValue()];

            var widgetX = Ext.getCmp("viewDocSecondHandWorkshopsInRetail" + aButton.UO_id);
            widgetX.UO_Param_fn(widgetX.UO_idTab, aButton.UO_CashBank, undefined, PriceX3);

            widgetX.close();
        }
        else {
            Ext.Msg.alert(lanOrgName, "Пожалуйста, заполните поля с продажными ценами (>0 и >'Сумма апп.')!");
            return;
        }
    },

    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        var widgetX = Ext.getCmp("viewDocSecondHandWorkshopsInRetail" + aButton.UO_id);
        widgetX.close();
    },

});