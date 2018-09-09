Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandRazborNomens", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],
    
    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocSecondHandRazborNomens': { close: this.this_close },


            //Currencies
            'viewDocSecondHandRazborNomens [itemId=DirCurrencyID]': { select: this.onDirCurrencyIDSelect },
            //'viewDocSecondHandRazborNomens [itemId=DirCurrencyID]': { change: this.onDirCurrencyIDSelect },

            'viewDocSecondHandRazborNomens button#btnCurrencyEdit': { "click": this.onBtnCurrencyEditClick },
            'viewDocSecondHandRazborNomens button#btnCurrencyReload': { "click": this.onBtnCurrencyReloadClick },
            'viewDocSecondHandRazborNomens button#btnCurrencyClear': { "click": this.onBtnCurrencyClearClick },


            'viewDocSecondHandRazborNomens [itemId=DirCurrencyRate]': { change: this.onDirCurrencyRateChange },
            'viewDocSecondHandRazborNomens [itemId=DirCurrencyMultiplicity]': { change: this.onDirCurrencyMultiplicityChange },


            'viewDocSecondHandRazborNomens [itemId=PriceVAT]': { change: this.onPriceVATChange },
            'viewDocSecondHandRazborNomens [itemId=PriceCurrency]': { change: this.onPriceCurrencyChange },

            'viewDocSecondHandRazborNomens [itemId=MarkupRetail]': { change: this.onMarkupRetailChange },
            'viewDocSecondHandRazborNomens [itemId=PriceRetailVAT]': { change: this.onPriceRetailVATChange },
            'viewDocSecondHandRazborNomens [itemId=PriceRetailCurrency]': { change: this.onPriceRetailCurrencyChange },

            'viewDocSecondHandRazborNomens [itemId=MarkupWholesale]': { change: this.onMarkupWholesaleChange },
            'viewDocSecondHandRazborNomens [itemId=PriceWholesaleVAT]': { change: this.onPriceWholesaleVATChange },
            'viewDocSecondHandRazborNomens [itemId=PriceWholesaleCurrency]': { change: this.onPriceWholesaleCurrencyChange },

            'viewDocSecondHandRazborNomens [itemId=MarkupIM]': { change: this.onMarkupIMChange },
            'viewDocSecondHandRazborNomens [itemId=PriceIMVAT]': { change: this.onPriceIMVATChange },
            'viewDocSecondHandRazborNomens [itemId=PriceIMCurrency]': { change: this.onPriceIMCurrencyChange },



            'viewDocSecondHandRazborNomens [itemId=Quantity]': { change: this.onQuantityChange },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocSecondHandRazborNomens button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocSecondHandRazborNomens button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocSecondHandRazborNomens button#btnDel': { "click": this.onBtnDelClick },
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

    //Поменяли "Наценку Розницы"
    onMarkupRetailChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupRetail_Change(aTextfield.UO_id, PrimaryFieldID); //fn_controllerDirNomensEdit_MarkupRetail_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Розницы"
    onPriceRetailVATChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceRetailVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Розницы"
    onPriceRetailCurrencyChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceRetailCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },

    //Поменяли "Наценку Опта"
    onMarkupWholesaleChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupWholesale_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Опта"
    onPriceWholesaleVATChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceWholesaleVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену Опта"
    onPriceWholesaleCurrencyChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceWholesaleCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },

    //Поменяли "Наценку IM"
    onMarkupIMChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_MarkupIM_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену IM"
    onPriceIMVATChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceIMVAT_Change(aTextfield.UO_id, PrimaryFieldID);
    },
    //Поменяли "Цену IM"
    onPriceIMCurrencyChange: function (aTextfield, aText) {
        var PrimaryFieldID = Ext.getCmp("DirNomenID" + aTextfield.UO_id).getValue();
        fn_controllerDirNomensEdit_PriceIMCurrency_Change(aTextfield.UO_id, PrimaryFieldID);
    },


    //Поменяли "Приходную цену"
    onQuantityChange: function (aTextfield, aText) {
        /*var id = aTextfield.UO_id;
        Quantity = Ext.getCmp("Quantity" + id);

        //1. Quantity, если undefined, isNaN или "", то ставим её == 0
        if (isNaN(parseFloat(Ext.getCmp("Quantity" + id).getValue()))) { Ext.getCmp("Quantity" + id).setValue(0); }

        //2. Проверяем к-во
        if (parseFloat(Ext.getCmp("Quantity" + id).getValue()) > parseFloat(Ext.getCmp("Remnant" + id).getValue())) {
            Ext.Msg.alert(lanOrgName, "К-во товара на остатке меньше, чем Вы списываете! Исправте к-во!");
            return;
        }*/
    },




    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //fun_SaveTabDocNoChar1(aButton, controllerDocSecondHandRazbors_RecalculationSums);
        
        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DocSecondHandRazbor2Tabs + "?DocSecondHandPurchID=" + parseInt(Ext.getCmp("DocSecondHandPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value) + "&UO_Action=save";
        if (parseInt(Ext.getCmp("DocSecondHandRazborTabID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DocSecondHandRazbor2Tabs + "?id=" + parseInt(Ext.getCmp("DocSecondHandRazborTabID" + aButton.UO_id).value) + "&DocSecondHandPurchID=" + parseInt(Ext.getCmp("DocSecondHandPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value) + "&UO_Action=save";
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,

            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {

                if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) {
                    var locStore = Ext.getCmp(aButton.UO_idCall).getStore();
                    locStore.proxy.url = HTTP_DocSecondHandRazbor2Tabs + "?DocSecondHandPurchID=" + Ext.getCmp("DocSecondHandPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value;
                    locStore.load({ waitMsg: lanLoading });
                    locStore.on('load', function () {
                        if (Ext.getCmp(aButton.UO_idMain)) {
                            controllerDocSecondHandRazbors_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id); //controllerDocSalesEdit_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false); //controllerDocSecondHandRazboresEdit_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false);
                            Ext.getCmp(aButton.UO_idMain).close();
                        }
                    });
                }
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });

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