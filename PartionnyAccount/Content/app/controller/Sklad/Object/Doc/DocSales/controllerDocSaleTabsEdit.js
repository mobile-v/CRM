Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocSales/controllerDocSaleTabsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],
    
    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocSaleTabsEdit': { close: this.this_close },


            //Currencies
            'viewDocSaleTabsEdit [itemId=DirCurrencyID]': { select: this.onDirCurrencyIDSelect },
            //'viewDocSaleTabsEdit [itemId=DirCurrencyID]': { change: this.onDirCurrencyIDSelect },

            'viewDocSaleTabsEdit button#btnCurrencyEdit': { "click": this.onBtnCurrencyEditClick },
            'viewDocSaleTabsEdit button#btnCurrencyReload': { "click": this.onBtnCurrencyReloadClick },
            'viewDocSaleTabsEdit button#btnCurrencyClear': { "click": this.onBtnCurrencyClearClick },


            'viewDocSaleTabsEdit [itemId=DirCurrencyRate]': { change: this.onDirCurrencyRateChange },
            'viewDocSaleTabsEdit [itemId=DirCurrencyMultiplicity]': { change: this.onDirCurrencyMultiplicityChange },


            'viewDocSaleTabsEdit [itemId=PriceVAT]': { change: this.onPriceVATChange },
            'viewDocSaleTabsEdit [itemId=PriceCurrency]': { change: this.onPriceCurrencyChange },

            'viewDocSaleTabsEdit [itemId=Quantity]': { change: this.onQuantityChange },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocSaleTabsEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocSaleTabsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocSaleTabsEdit button#btnDel': { "click": this.onBtnDelClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    //Currency
    onDirCurrencyIDSelect: function (combo, records) { //aButton, aEvent, aOptions
        //Запрос на сервер за курсом м кратностью
        Ext.Msg.show({
            title: lanOrgName,
            msg: "Изменить Курс и Кратность?",
            buttons: Ext.Msg.YESNO,
            fn: function (btn) {
                if (btn == "yes") {
                    Ext.getCmp("DirCurrencyRate" + combo.UO_id).setValue(records.data.DirCurrencyRate);
                    Ext.getCmp("DirCurrencyMultiplicity" + combo.UO_id).setValue(records.data.DirCurrencyMultiplicity);
                }
            },
            icon: Ext.window.MessageBox.QUESTION
        });
    },
    onBtnCurrencyEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirCurrencies", Params);
    },
    onBtnCurrencyReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirCurrenciesGrid = Ext.getCmp(aButton.UO_idMain).storeDirCurrenciesGrid;
        storeDirCurrenciesGrid.load();
    },
    onBtnCurrencyClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirCurrencyID" + aButton.UO_id).setValue("");
    },



    //Поменяли "Курс" (DirCurrencyRate)
    onDirCurrencyRateChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Кратность" (DirCurrencyMultiplicity)
    onDirCurrencyMultiplicityChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },



    //Поменяли "Приходную цену"
    onPriceVATChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Приходную цену в текущей валюте" (PriceCurrency)
    onPriceCurrencyChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },


    //Поменяли "Приходную цену"
    onQuantityChange: function (aTextfield, aText) {
        var id = aTextfield.UO_id;
        Quantity = Ext.getCmp("Quantity" + id);

        //1. Quantity, если undefined, isNaN или "", то ставим её == 0
        if (isNaN(parseFloat(Ext.getCmp("Quantity" + id).getValue()))) { Ext.getCmp("Quantity" + id).setValue(0); }

        //2. Проверяем к-во
        if (parseFloat(Ext.getCmp("Quantity" + id).getValue()) > parseFloat(Ext.getCmp("Remnant" + id).getValue())) {
            Ext.Msg.alert(lanOrgName, "К-во товара на остатке меньше, чем Вы списываете! Исправте к-во!");
            return;
        }
    },




    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        fun_SaveTabDocNoChar1(aButton, controllerDocSalesEdit_RecalculationSums);
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnDelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName,
            msg: lanDelete + "?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    var selection = Ext.getCmp(aButton.UO_idCall).getView().getSelectionModel().getSelection()[0];
                    if (selection) {
                        Ext.getCmp(aButton.UO_idCall).store.remove(selection);

                        Ext.getCmp(aButton.UO_idMain).close();
                    }
                }
            }
        });
    },

});