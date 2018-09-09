Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocOrderInts/controllerDocOrderIntsPurches", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocOrderIntsPurches': { close: this.this_close },
            


            'viewDocOrderIntsPurches [itemId=PriceVAT]': { change: this.onPriceVATChange },
            'viewDocOrderIntsPurches [itemId=PriceCurrency]': { change: this.onPriceCurrencyChange },

            'viewDocOrderIntsPurches [itemId=MarkupRetail]': { change: this.onMarkupRetailChange },
            'viewDocOrderIntsPurches [itemId=PriceRetailVAT]': { change: this.onPriceRetailVATChange },
            'viewDocOrderIntsPurches [itemId=PriceRetailCurrency]': { change: this.onPriceRetailCurrencyChange },

            'viewDocOrderIntsPurches [itemId=MarkupWholesale]': { change: this.onMarkupWholesaleChange },
            'viewDocOrderIntsPurches [itemId=PriceWholesaleVAT]': { change: this.onPriceWholesaleVATChange },
            'viewDocOrderIntsPurches [itemId=PriceWholesaleCurrency]': { change: this.onPriceWholesaleCurrencyChange },

            'viewDocOrderIntsPurches [itemId=MarkupIM]': { change: this.onMarkupIMChange },
            'viewDocOrderIntsPurches [itemId=PriceIMVAT]': { change: this.onPriceIMVATChange },
            'viewDocOrderIntsPurches [itemId=PriceIMCurrency]': { change: this.onPriceIMCurrencyChange },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            /*
            'viewDocOrderIntsPurches button#btnSave1': { "click": this.onBtnSaveClick },
            'viewDocOrderIntsPurches button#btnSave2': { "click": this.onBtnSaveClick },
            */
            'viewDocOrderIntsPurches button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocOrderIntsPurches button#btnCancel': { "click": this.onBtnCancelClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },




    //Поменяли "Приходную цену"
    onPriceVATChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Приходную цену в текущей валюте" (PriceCurrency)
    onPriceCurrencyChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },

    //Поменяли "Наценку Розницы"
    onMarkupRetailChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupRetail_Change(aTextfield.UO_id, PrimaryFieldID); //fn_controllerDirNomensEdit_MarkupRetail_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Розницы"
    onPriceRetailVATChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceRetailVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Розницы"
    onPriceRetailCurrencyChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceRetailCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },

    //Поменяли "Наценку Опта"
    onMarkupWholesaleChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupWholesale_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Опта"
    onPriceWholesaleVATChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceWholesaleVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Опта"
    onPriceWholesaleCurrencyChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceWholesaleCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },

    //Поменяли "Наценку IM"
    onMarkupIMChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupIM_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену IM"
    onPriceIMVATChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceIMVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену IM"
    onPriceIMCurrencyChange: function (aTextfield, aText) {
        if (varPriceChange_ReadOnly) { return; }
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceIMCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
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
                parseFloat(Ext.getCmp("PriceCurrency" + aButton.UO_id).getValue()) > 0 && parseFloat(Ext.getCmp("PriceVAT" + aButton.UO_id).getValue()) > 0 &&
                parseFloat(Ext.getCmp("MarkupRetail" + aButton.UO_id).getValue()) > 0 && parseFloat(Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue()) > 0 && parseFloat(Ext.getCmp("PriceRetailCurrency" + aButton.UO_id).getValue()) > 0 &&
                parseFloat(Ext.getCmp("MarkupWholesale" + aButton.UO_id).getValue()) > 0 && parseFloat(Ext.getCmp("PriceWholesaleVAT" + aButton.UO_id).getValue()) > 0 && parseFloat(Ext.getCmp("PriceWholesaleCurrency" + aButton.UO_id).getValue()) > 0 &&
                parseFloat(Ext.getCmp("MarkupIM" + aButton.UO_id).getValue()) > 0 && parseFloat(Ext.getCmp("PriceIMVAT" + aButton.UO_id).getValue()) > 0 && parseFloat(Ext.getCmp("PriceIMCurrency" + aButton.UO_id).getValue()) > 0
                && parseInt(Ext.getCmp("DirContractorID" + aButton.UO_id).getValue()) > 0
            )
           ) {


            var
                PriceCurrency = Ext.getCmp("PriceCurrency" + aButton.UO_id).getValue().replace(",", "."),
                PriceVAT = Ext.getCmp("PriceVAT" + aButton.UO_id).getValue().replace(",", "."),
                MarkupRetail = Ext.getCmp("MarkupRetail" + aButton.UO_id).getValue().replace(",", "."),
                PriceRetailVAT = Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue().replace(",", "."),
                PriceRetailCurrency = Ext.getCmp("PriceRetailCurrency" + aButton.UO_id).getValue().replace(",", "."),
                MarkupWholesale = Ext.getCmp("MarkupWholesale" + aButton.UO_id).getValue().replace(",", "."),
                PriceWholesaleVAT = Ext.getCmp("PriceWholesaleVAT" + aButton.UO_id).getValue().replace(",", "."),
                PriceWholesaleCurrency = Ext.getCmp("PriceWholesaleCurrency" + aButton.UO_id).getValue().replace(",", "."),
                MarkupIM = Ext.getCmp("MarkupIM" + aButton.UO_id).getValue().replace(",", "."),
                PriceIMVAT = Ext.getCmp("PriceIMVAT" + aButton.UO_id).getValue().replace(",", "."),
                PriceIMCurrency = Ext.getCmp("PriceIMCurrency" + aButton.UO_id).getValue().replace(",", "."),
                DirContractorID = Ext.getCmp("DirContractorID" + aButton.UO_id).getValue();

            var PriceX11 = [
                PriceCurrency, PriceVAT, 
                MarkupRetail, PriceRetailVAT, PriceRetailCurrency,
                MarkupWholesale, PriceWholesaleVAT, PriceWholesaleCurrency,
                MarkupIM, PriceIMVAT, PriceIMCurrency,
                DirContractorID,
            ];

            var widgetX = Ext.getCmp("viewDocOrderIntsPurches" + aButton.UO_id);
            widgetX.UO_Param_fn(widgetX.UO_idTab, 0, PriceX11);

            widgetX.close();
        }
        else {
            Ext.Msg.alert(lanOrgName, "Пожалуйста, заполните поля с продажными ценами (>0 и >'Сумма апп.')!");
            return;
        }
    },

    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        var widgetX = Ext.getCmp("viewDocOrderIntsPurches" + aButton.UO_id);
        widgetX.close();
    },

});