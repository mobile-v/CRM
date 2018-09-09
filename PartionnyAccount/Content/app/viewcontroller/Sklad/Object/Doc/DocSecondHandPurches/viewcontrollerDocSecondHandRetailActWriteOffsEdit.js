Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandRetailActWriteOffsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocSecondHandRetailActWriteOffsEdit',



    onChangeDiscount: function (textfield, valueNew, valueOld, e) {
        //viewcontrollerDocSecondHandRetailActWriteOffTabsEdit_RecalculationSums(textfield.UO_id);
    },
    onPaymentDiscount: function (textfield, valueNew, valueOld, e) {
        //viewcontrollerDocSecondHandRetailActWriteOffTabsEdit_RecalculationSums(textfield.UO_id);
    },

    //Расчет - не работает
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
        viewcontrollerDocSecondHandRetailActWriteOffTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
    },
    /*
    //Наличная продажа
    onBtnHelds1Click: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
        viewcontrollerDocSecondHandRetailActWriteOffTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
    },
    //Безналичная продажа
    onBtnHelds2Click: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
        viewcontrollerDocSecondHandRetailActWriteOffTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
    },
    */


    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { viewcontrollerDocSecondHandRetailActWriteOffTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

});


//Функия сохранения
function viewcontrollerDocSecondHandRetailActWriteOffTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);
    //Проверка
    if (!widgetXForm.UO_GridSave) { Ext.Msg.alert("", "UO_GridSave == undefined"); return; }
    //Форма
    var form = widgetXForm.getForm();
    //Валидация
    if (!form.isValid()) { Ext.Msg.alert(lanOrgName, "Пожалуйста, заполните все поля формы!"); return; }
    //Получаем данные полей с формы
    var rec = form.getFieldValues();

    //Получаем Store Грида
    var store = Ext.getCmp("grid_" + aButton.UO_id).getStore(); store.setData([], false);
    //Insert
    store.insert(store.data.items.length, rec);

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++




    //Спецификация (табличная часть)
    var recordsDocSecondHandRetailActWriteOffTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocSecondHandRetailActWriteOffTab.push(rec.data); });

    //Проверка
    //if (Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Контрагента!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    //if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }
    if (Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Тип оплаты!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocSecondHandRetailActWriteOffs + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocSecondHandRetailActWriteOffID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocSecondHandRetailActWriteOffs + "?id=" + parseInt(Ext.getCmp("DocSecondHandRetailActWriteOffID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocSecondHandRetailActWriteOffTab: Ext.encode(recordsDocSecondHandRetailActWriteOffTab) },

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {
            //Обновляем грид
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) {
                storeGrid = Ext.getCmp(aButton.UO_idCall).getStore();
                storeGrid.load();
                storeGrid.on('load', function () {
                    //viewcontrollerDocSecondHandRetailActWriteOffsEdit_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false)
                    var storeGridParty = Ext.getCmp("gridParty_" + Ext.getCmp(aButton.UO_idCall).UO_id).store;
                    storeGridParty.load();
                    storeGridParty.on('load', function () {
                        if (Ext.getCmp(aButton.UO_idCall) && Ext.getCmp("TriggerSearchTree" + Ext.getCmp(aButton.UO_idCall).UO_id)) {
                            Ext.getCmp("TriggerSearchTree" + Ext.getCmp(aButton.UO_idCall).UO_id).focus();
                        }
                    });
                });
            }

            //Закрыть
            Ext.getCmp(aButton.UO_idMain).close();
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};



//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function viewcontrollerDocSecondHandRetailActWriteOffTabsEdit_RecalculationSums(id) {
    if (parseFloat(Ext.getCmp("PriceCurrency" + id).getValue()) - parseFloat(Ext.getCmp("Discount" + id).getValue()) < 0) {
        Ext.Msg.alert(lanOrgName, "Скидка больше цены товара!"); return;
    }
};