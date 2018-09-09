Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocNomenRevaluations/controllerDocNomenRevaluationTabsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocNomenRevaluationTabsEdit': { close: this.this_close },

            
            //Currencies
            'viewDocNomenRevaluationTabsEdit [itemId=DirCurrencyID]': { select: this.onDirCurrencyIDSelect },
            //'viewDocNomenRevaluationTabsEdit [itemId=DirCurrencyID]': { change: this.onDirCurrencyIDSelect },

            'viewDocNomenRevaluationTabsEdit button#btnCurrencyEdit': { "click": this.onBtnCurrencyEditClick },
            'viewDocNomenRevaluationTabsEdit button#btnCurrencyReload': { "click": this.onBtnCurrencyReloadClick },
            'viewDocNomenRevaluationTabsEdit button#btnCurrencyClear': { "click": this.onBtnCurrencyClearClick },


            'viewDocNomenRevaluationTabsEdit [itemId=DirCurrencyRate]': { change: this.onDirCurrencyRateChange },
            'viewDocNomenRevaluationTabsEdit [itemId=DirCurrencyMultiplicity]': { change: this.onDirCurrencyMultiplicityChange },


            'viewDocNomenRevaluationTabsEdit [itemId=PriceVAT]': { change: this.onPriceVATChange },
            'viewDocNomenRevaluationTabsEdit [itemId=PriceCurrency]': { change: this.onPriceCurrencyChange },

            'viewDocNomenRevaluationTabsEdit [itemId=MarkupRetail]': { change: this.onMarkupRetailChange },
            'viewDocNomenRevaluationTabsEdit [itemId=PriceRetailVAT]': { change: this.onPriceRetailVATChange },
            'viewDocNomenRevaluationTabsEdit [itemId=PriceRetailCurrency]': { change: this.onPriceRetailCurrencyChange },

            'viewDocNomenRevaluationTabsEdit [itemId=MarkupWholesale]': { change: this.onMarkupWholesaleChange },
            'viewDocNomenRevaluationTabsEdit [itemId=PriceWholesaleVAT]': { change: this.onPriceWholesaleVATChange },
            'viewDocNomenRevaluationTabsEdit [itemId=PriceWholesaleCurrency]': { change: this.onPriceWholesaleCurrencyChange },

            'viewDocNomenRevaluationTabsEdit [itemId=MarkupIM]': { change: this.onMarkupIMChange },
            'viewDocNomenRevaluationTabsEdit [itemId=PriceIMVAT]': { change: this.onPriceIMVATChange },
            'viewDocNomenRevaluationTabsEdit [itemId=PriceIMCurrency]': { change: this.onPriceIMCurrencyChange },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocNomenRevaluationTabsEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocNomenRevaluationTabsEdit menuitem#btnPrint_barcode': { "click": this.onBtnPrintClick },
            'viewDocNomenRevaluationTabsEdit menuitem#btnPrint_barcode_price': { "click": this.onBtnPrintClick },
            'viewDocNomenRevaluationTabsEdit menuitem#btnPrint_barcode_name': { "click": this.onBtnPrintClick },
            'viewDocNomenRevaluationTabsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocNomenRevaluationTabsEdit button#btnDel': { "click": this.onBtnDelClick },
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





    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Проверка: что бы партии не пересекались
        var store = Ext.getCmp(aButton.UO_idCall).getStore();
        for (var i = 0; i < store.data.items.length; i++) {
            if (parseInt(store.data.items[i].data.RemPartyID) == parseInt(Ext.getCmp("RemPartyID" + aButton.UO_id).getValue())) {
                Ext.Msg.alert(lanOrgName, "Такая партия уже присутствует в списке! Партия №" + store.data.items[i].data.RemPartyID); return;
            }
        }

        fun_SaveTabDocNoChar1(aButton, undefined); //controllerDocNomenRevaluationsEdit_RecalculationSums
    },
    onBtnPrintClick: function (aButton, aEvent, aOptions) {

        var mapForm = document.createElement("form");
        mapForm.target = "Map";
        mapForm.method = "GET"; // or "post" if appropriate
        mapForm.action = "../report/report/";

        //var UO_id = Ext.getCmp(aButton.UO_idCall).UO_id;

        //Параметр "pID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pID"; mapInput.value = "DocNomenRevaluationTabsPrintCode"; mapForm.appendChild(mapInput);

        //Параметр "DirNomenID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "Quantity"; mapInput.value = Ext.getCmp("Quantity" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "DirNomenID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DirNomenID"; mapInput.value = Ext.getCmp("DirNomenID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "DirNomenName"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DirNomenName"; mapInput.value = Ext.getCmp("DirNomenName" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "PriceRetailCurrency"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "PriceRetailCurrency"; mapInput.value = Ext.getCmp("PriceRetailCurrency" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "UO_Action"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "UO_Action"; mapInput.value = aButton.UO_Action; mapForm.appendChild(mapInput);


        document.body.appendChild(mapForm);
        map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

        if (map) { mapForm.submit(); }
        else { alert('You must allow popups for this map to work.'); }

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