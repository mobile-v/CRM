Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirNomens/controllerDirNomensWinComboEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirNomensWinComboEdit': { close: this.this_close },


            // Клик по Группе
            'viewDirNomensWinComboEdit [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },
            

            //ComboBox
            'viewDirNomensWinComboEdit [itemId=DirNomenID1]': {
                "select": this.onDirNomenID1Select,
                "change": this.onDirNomenID1Change,
            },
            
            'viewDirNomensWinComboEdit [itemId=DirNomenID2]': {
                "select": this.onDirNomenID2Select,
                "change": this.onDirNomenID2Change,
            },
            
            'viewDirNomensWinComboEdit [itemId=DirNomenCategoryID]': {
                "select": this.onDirNomenCategoryIDSelect,
                "change": this.onDirNomenCategoryIDChange,
            },




            'viewDirNomensWinComboEdit [itemId=PriceVAT]': { change: this.onPriceVATChange },
            'viewDirNomensWinComboEdit [itemId=PriceCurrency]': { change: this.onPriceCurrencyChange },

            'viewDirNomensWinComboEdit [itemId=MarkupRetail]': { change: this.onMarkupRetailChange },
            'viewDirNomensWinComboEdit [itemId=PriceRetailVAT]': { change: this.onPriceRetailVATChange },
            'viewDirNomensWinComboEdit [itemId=PriceRetailCurrency]': { change: this.onPriceRetailCurrencyChange },

            'viewDirNomensWinComboEdit [itemId=MarkupWholesale]': { change: this.onMarkupWholesaleChange },
            'viewDirNomensWinComboEdit [itemId=PriceWholesaleVAT]': { change: this.onPriceWholesaleVATChange },
            'viewDirNomensWinComboEdit [itemId=PriceWholesaleCurrency]': { change: this.onPriceWholesaleCurrencyChange },

            'viewDirNomensWinComboEdit [itemId=MarkupIM]': { change: this.onMarkupIMChange },
            'viewDirNomensWinComboEdit [itemId=PriceIMVAT]': { change: this.onPriceIMVATChange },
            'viewDirNomensWinComboEdit [itemId=PriceIMCurrency]': { change: this.onPriceIMCurrencyChange },


            

            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirNomensWinComboEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirNomensWinComboEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirNomensWinComboEdit button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },

    
    //Марка
    onDirNomenID1Select: function (combo, records, eOpts) {
        if (!combo.UO_NoAutoLoad) {
            controllerDirNomensWinComboEdit_onDirNomenID1Change(combo, records);
        }
    },
    onDirNomenID1Change: function (combo, newValue, oldValue) {
        /*if (!combo.UO_NoAutoLoad) {
            controllerDirNomensWinComboEdit_onDirNomenID1Change(combo, newValue);
        }*/
    },

    //Модель
    onDirNomenID2Select: function (combo, records, eOpts) {
        if (!combo.UO_NoAutoLoad) {
            controllerDirNomensWinComboEdit_onDirNomenID2Change(combo, records);
        }
    },
    onDirNomenID2Change: function (combo, newValue, oldValue) {
    },

    //Товар
    onDirNomenCategoryIDSelect: function (combo, records, eOpts) {
        if (!combo.UO_NoAutoLoad) {
            controllerDirNomensWinComboEdit_onDirNomenCategoryIDChange(combo, records);
        }
    },
    onDirNomenCategoryIDChange: function (combo, newValue, oldValue) {
        /*if (!combo.UO_NoAutoLoad) {
            controllerDirNomensWinComboEdit_onDirNomenCategoryIDChange(combo, newValue);
        }*/
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




    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //fun_SaveTabDocNoChar1(aButton, controllerDocSecondHandRazbors_RecalculationSums);

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Проверки === === ===
        //Прих.цена
        if (Ext.getCmp("PriceCurrency" + aButton.UO_id).value <= 0) {
            Ext.Msg.alert(lanOrgName, "Укажите цену поступления большую '0'!");
            return;
        }
        //Марка
        if (!fun_isInteger(Ext.getCmp("DirNomen1ID" + aButton.UO_id).value)) {
            Ext.Msg.alert(lanOrgName, "Марка аппарата не найдена! Придётся выбрать её вручную!<br />(вносить новые Марки запрещенно)");
            return;
        }
        //Модель
        if (!fun_isInteger(Ext.getCmp("DirNomen2ID" + aButton.UO_id).value)) {
            Ext.Msg.alert(lanOrgName, "Модель аппарата не найдена! Придётся выбрать её вручную!<br />(вносить новые Модели запрещенно)");
            return;
        }
        //Категория
        if (!fun_isInteger(Ext.getCmp("DirNomenCategoryID" + aButton.UO_id).value)) {
            Ext.getCmp("DirNomenCategoryName" + aButton.UO_id).setValue(Ext.getCmp("DirNomenCategoryID" + aButton.UO_id).getRawValue())
        }
        else {
            Ext.getCmp("DirNomenCategoryName" + aButton.UO_id).setValue("")
        }


        /*
        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DocSecondHandRazbor2Tabs + "?DocSecondHandPurchID=" + parseInt(Ext.getCmp("DocSecondHandPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value) + "&UO_Action=save";
        if (parseInt(Ext.getCmp("DocSecondHandRazborTabID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DocSecondHandRazbor2Tabs + "?id=" + parseInt(Ext.getCmp("DocSecondHandRazborTabID" + aButton.UO_id).value) + "&DocSecondHandPurchID=" + parseInt(Ext.getCmp("DocSecondHandPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value) + "&UO_Action=save";
        }
        */

        //Сохранение
        widgetXForm.submit({
            method: "POST", //sMethod,
            url: HTTP_DocSecondHandRazbor2Tabs + "?DocSecondHandPurchID=" + parseInt(Ext.getCmp("DocSecondHandPurchID" + Ext.getCmp(aButton.UO_idCall).UO_id).value) + "&UO_Action=save", //sUrl,

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


//Функия сохранения
function controllerDirNomensWinComboEdit_onBtnSaveClick(aButton, aEvent, aOptions) {
    //Заполнение полей: DirNomenXID, DirNomenXName. Т.к. Комбы у нас редактируемые и в них можно вписать новый товар
    //1.
    if (isNaN(parseInt(Ext.getCmp("DirNomenID1" + aButton.UO_id).getValue()))) {
        Ext.Msg.alert(lanOrgName, "Первая группа должна быть обязательно выбрана с списка!");
        return;
    }
    Ext.getCmp("DirNomen1ID" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID1" + aButton.UO_id).getValue());
    Ext.getCmp("DirNomen1Name" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID1" + aButton.UO_id).rawValue);

    //2.
    if (isNaN(parseInt(Ext.getCmp("DirNomenID2" + aButton.UO_id).getValue()))) { /*нет действий*/ }
    else {
        Ext.getCmp("DirNomen2ID" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID2" + aButton.UO_id).getValue());
    }
    Ext.getCmp("DirNomen2Name" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID2" + aButton.UO_id).rawValue);


    /*

    //3.
    if (isNaN(parseInt(Ext.getCmp("DirNomenID3" + aButton.UO_id).getValue()))) {  }
    else {
        Ext.getCmp("DirNomen3ID" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID3" + aButton.UO_id).getValue());
    }
    Ext.getCmp("DirNomen3Name" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID3" + aButton.UO_id).rawValue);

    //4.
    if (isNaN(parseInt(Ext.getCmp("DirNomenID4" + aButton.UO_id).getValue()))) {  }
    else {
        Ext.getCmp("DirNomen4ID" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID4" + aButton.UO_id).getValue());
    }
    Ext.getCmp("DirNomen4Name" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID4" + aButton.UO_id).rawValue);

    //5.
    if (isNaN(parseInt(Ext.getCmp("DirNomenID5" + aButton.UO_id).getValue()))) {  }
    else {
        Ext.getCmp("DirNomen5ID" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID5" + aButton.UO_id).getValue());
    }
    Ext.getCmp("DirNomen5Name" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID5" + aButton.UO_id).rawValue);

    //6.
    if (isNaN(parseInt(Ext.getCmp("DirNomenID6" + aButton.UO_id).getValue()))) {  }
    else {
        Ext.getCmp("DirNomen6ID" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID6" + aButton.UO_id).getValue());
    }
    Ext.getCmp("DirNomen6Name" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID6" + aButton.UO_id).rawValue);

    */


    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Сохранение
    widgetXForm.submit({
        method: "POST",
        url: HTTP_DocOrderInts,
        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {

            //Меняем статус Аппарата: "Ожидание запчасте" в Контроллере
            //Надо ещё обновить открытую "форму редактирования" Аппарата в Вьюхе "viewDocServiceWorkshops"
            if (Ext.getCmp("DirOrderIntTypeID" + aButton.UO_id).getValue() == 1) {
                var UO_id = Ext.getCmp(Ext.getCmp(aButton.UO_idCall).UO_idCall).UO_id;
                //Ext.getCmp("btnStatus5" + UO_id).setPressed(true);
                Ext.getCmp("DirServiceStatusID" + UO_id).setValue(5);
                controllerDocServiceWorkshops_DirServiceStatusID_ChangeButton(UO_id);
            }


            Ext.Msg.alert(lanOrgName, "Заказ принят!");
            Ext.getCmp(aButton.UO_idMain).close();
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};

//ComboBox
function controllerDirNomensWinComboEdit_onDirNomenID1Change(combo, records)
{
    var storeDirNomensGrid = Ext.getCmp("viewDirNomensComboEdit" + combo.UO_id).storeDirNomensGrid2;
    storeDirNomensGrid.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=" + records.data.DirNomenID;
    storeDirNomensGrid.load({ waitMsg: lanLoading });

    Ext.getCmp("DirNomenID" + combo.UO_id).setValue(records.data.DirNomenID);
    Ext.getCmp("DirNomenName" + combo.UO_id).setValue(records.data.DirNomenName);

    Ext.getCmp("DirNomenID2" + combo.UO_id).setReadOnly(false); Ext.getCmp("DirNomenID2" + combo.UO_id).setValue("");
    Ext.getCmp("DirNomenCategoryID" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenCategoryID" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID4" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID4" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID5" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID5" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID6" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID6" + combo.UO_id).setValue("");

    //Получение цены
    //records.data.id = records.data.DirNomenID;
    //fun_viewDocPurchTabsEdit_RequestPrice(undefined, records, combo.UO_id, combo.UO_id); //combo.UO_idCall
    //fun_controllerDocOrderIntsEdit_RequestPrice(records.data.DirNomenID, combo.UO_id);
}
function controllerDirNomensWinComboEdit_onDirNomenID2Change(combo, records) {
    
    Ext.getCmp("DirNomenID" + combo.UO_id).setValue(records.data.DirNomenID);
    Ext.getCmp("DirNomenName" + combo.UO_id).setValue(
        Ext.getCmp("DirNomenID1" + combo.UO_id).rawValue + " / " +
        records.data.DirNomenName
    );

    Ext.getCmp("DirNomenCategoryID" + combo.UO_id).setReadOnly(false); Ext.getCmp("DirNomenCategoryID" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID4" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID4" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID5" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID5" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID6" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID6" + combo.UO_id).setValue("");

    //Получение цены
    //records.data.id = records.data.DirNomenID;
    //fun_viewDocPurchTabsEdit_RequestPrice(undefined, records, combo.UO_id, combo.UO_idCall);
    //fun_controllerDocOrderIntsEdit_RequestPrice(records.data.DirNomenID, combo.UO_id);
}
function controllerDirNomensWinComboEdit_onDirNomenCategoryIDChange(combo, records) {
    /*
    Ext.getCmp("DirNomenID" + combo.UO_id).setValue(records.data.DirNomenID);
    Ext.getCmp("DirNomenName" + combo.UO_id).setValue(
        Ext.getCmp("DirNomenID1" + combo.UO_id).rawValue + " / " +
        Ext.getCmp("DirNomenID2" + combo.UO_id).rawValue + " / " +
        records.data.DirNomenCategoryName
    );
    */

    //Ext.getCmp("DirNomenID" + combo.UO_id).setValue();
    Ext.getCmp("DirNomenName" + combo.UO_id).setValue(records.data.DirNomenCategoryName);
    Ext.getCmp("DirNomenNameFull" + combo.UO_id).setValue(records.data.DirNomenCategoryName);

    //Получение цены
    //records.data.id = records.data.DirNomenID;
    //fun_viewDocPurchTabsEdit_RequestPrice(undefined, records, combo.UO_id, combo.UO_idCall);
    //fun_controllerDocOrderIntsEdit_RequestPrice(records.data.DirNomenID, combo.UO_id);
}


//UO_NoAutoLoad - активизировать "onDirNomenIDXChange" (, cb3, cb4, cb5, cb6)
function controllerDirNomensWinComboEdit_UO_NoAutoLoad(rec, UO_NoAutoLoad, cb1, cb2, cb3)
{
    
    if (!UO_NoAutoLoad) {
        cb1.UO_NoAutoLoad = true;
        cb2.setReadOnly(true); cb2.store.UO_Loaded = false; cb2.UO_NoAutoLoad = true; cb2.setValue(""); cb2.store.setData([], false);
        cb3.setValue("");
        //cb4.setReadOnly(true); cb4.store.UO_Loaded = false; cb4.UO_NoAutoLoad = true; cb4.setValue(""); cb4.store.setData([], false);
        //cb5.setReadOnly(true); cb5.store.UO_Loaded = false; cb5.UO_NoAutoLoad = true; cb5.setValue(""); cb5.store.setData([], false);
        //cb6.setReadOnly(true); cb6.store.UO_Loaded = false; cb6.UO_NoAutoLoad = true; cb6.setValue(""); cb6.store.setData([], false);
    }
    else {
        cb1.UO_NoAutoLoad = false;
        cb2.UO_NoAutoLoad = false;
        //cb3.UO_NoAutoLoad = false;
        //cb4.UO_NoAutoLoad = false;
        //cb5.UO_NoAutoLoad = false;
        //cb6.UO_NoAutoLoad = false;

        //fun_controllerDocOrderIntsEdit_RequestPrice(Ext.getCmp("DirNomenID" + cb1.UO_id).getValue(), cb1.UO_id);
        fun_controllerDocOrderIntsEdit_RequestPrice(rec.get('id'), cb1.UO_id);
    }
}

