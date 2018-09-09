Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocDomesticExpenses/viewcontrollerDocDomesticExpenseTabsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocDomesticExpenseTabsEdit',



    onChangeDiscount: function (textfield, valueNew, valueOld, e) {
        viewcontrollerDocDomesticExpenseTabsEdit_RecalculationSums(textfield.UO_id);
    },
    onPaymentDiscount: function (textfield, valueNew, valueOld, e) {
        viewcontrollerDocDomesticExpenseTabsEdit_RecalculationSums(textfield.UO_id);
    },

    //Расчет - не работает
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        
        if (!varRightDocDescriptionCheck && parseFloat(Ext.getCmp("Discount" + aButton.UO_id).getValue()) > 0) { Ext.Msg.alert(lanOrgName, "Скидки отключены! Обратитесь к Администратору для их включения!"); return; }

        if (Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).getValue() == null) {

            if (varPayType == 0) {
                Ext.MessageBox.show({
                    icon: Ext.MessageBox.QUESTION,
                    width: 300,
                    title: lanOrgName,
                    msg: 'Выбирите Тип оплаты!',
                    buttonText: { yes: "Наличная", no: "Безналичная", cancel: "Отмена" },
                    fn: function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                            viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                        }
                        else if (btn == "no") {
                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                            viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                        }
                    }
                });

            }
            else if (varPayType == 1) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
            }
            else if (varPayType == 2) {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
            }

        }
        else {
            viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
        }
    },
    /*
    //Наличная продажа
    onBtnHelds1Click: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
        viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
    },
    //Безналичная продажа
    onBtnHelds2Click: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
        viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
    },
    */


    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

});


//Функия сохранения
function viewcontrollerDocDomesticExpenseTabsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {
    
    //Проверка
    //if (Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Контрагента!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    //if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }
    if (Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Тип оплаты!"); return; }

    //Проставляем цену в тек.вал.
    Ext.getCmp("PriceCurrency" + aButton.UO_id).setValue(
        parseFloat(Ext.getCmp("PriceVAT" + aButton.UO_id).getValue()) * parseFloat(Ext.getCmp("DirCurrencyRate" + aButton.UO_id).getValue()) / parseFloat(Ext.getCmp("DirCurrencyMultiplicity" + aButton.UO_id).getValue())
    );

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
    var recordsDocDomesticExpenseTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocDomesticExpenseTab.push(rec.data); });

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocDomesticExpenses + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocDomesticExpenseID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocDomesticExpenses + "?id=" + parseInt(Ext.getCmp("DocDomesticExpenseID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl + "&DirEmployeePswd=" + Ext.getCmp("DirEmployeePswd" + aButton.UO_id).value,
        params: { recordsDocDomesticExpenseTab: Ext.encode(recordsDocDomesticExpenseTab) },

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {
            //Обновляем грид
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) {
                var storeGrid = Ext.getCmp(aButton.UO_idCall).getStore();
                storeGrid.load();
                storeGrid.on('load', function () {
                    //viewcontrollerDocDomesticExpensesEdit_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false)
                    /*
                    var storeGridParty = Ext.getCmp("gridParty_" + Ext.getCmp(aButton.UO_idCall).UO_id).store;
                    storeGridParty.load();
                    storeGridParty.on('load', function () {
                        if(Ext.getCmp(aButton.UO_idCall) && Ext.getCmp("TriggerSearchTree" + Ext.getCmp(aButton.UO_idCall).UO_id)){
                            Ext.getCmp("TriggerSearchTree" + Ext.getCmp(aButton.UO_idCall).UO_id).focus();
                        }
                    });
                    */
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
function viewcontrollerDocDomesticExpenseTabsEdit_RecalculationSums(id) {
    if (parseFloat(Ext.getCmp("PriceCurrency" + id).getValue()) - parseFloat(Ext.getCmp("Discount" + id).getValue()) < 0) {
        Ext.Msg.alert(lanOrgName, "Скидка больше цены товара!"); return;
    }
};

