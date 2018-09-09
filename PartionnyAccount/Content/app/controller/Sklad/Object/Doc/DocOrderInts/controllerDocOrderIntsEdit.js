Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocOrderInts/controllerDocOrderIntsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocOrderIntsEdit': { close: this.this_close },
            'viewDocOrderIntsNomensEdit': { close: this.this_close },


            // Клик по Группе
            'viewDocOrderIntsEdit [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },
            'viewDocOrderIntsNomensEdit [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },


            //Марка === === ===
            'viewDocOrderIntsEdit [itemId=DirNomen1ID]': {
                "select": this.onDirNomen1IDSelect,
                "change": this.onDirNomen1IDChange,
            },
            'viewDocOrderIntsNomensEdit [itemId=DirNomen1ID]': {
                "select": this.onDirNomen1IDSelect,
                "change": this.onDirNomen1IDChange,
            },
            //Модель === === ===
            'viewDocOrderIntsEdit [itemId=DirNomen2ID]': {
                "select": this.onDirNomen2IDSelect,
                "change": this.onDirNomen2IDChange,
            },
            'viewDocOrderIntsNomensEdit [itemId=DirNomen2ID]': {
                "select": this.onDirNomen2IDSelect,
                "change": this.onDirNomen2IDChange,
            },
            //Товар === === ===
            'viewDocOrderIntsEdit [itemId=DirNomenID]': {
                "select": this.onDirNomenIDSelect,
                "change": this.onDirNomenIDChange,
            },
            'viewDocOrderIntsNomensEdit [itemId=DirNomenID]': {
                "select": this.onDirNomenIDSelect,
                "change": this.onDirNomenIDChange,
            },


            'viewDocOrderIntsEdit [itemId=DirNomenID6]': {
                "select": this.onDirNomenID6Select,
                "change": this.onDirNomenID6Change,
            },
            'viewDocOrderIntsNomensEdit [itemId=DirNomenID6]': {
                "select": this.onDirNomenID6Select,
                "change": this.onDirNomenID6Change,
            },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocOrderIntsEdit button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocOrderIntsNomensEdit button#btnSave': { "click": this.onBtnSaveClick },

            'viewDocOrderIntsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocOrderIntsNomensEdit button#btnCancel': { "click": this.onBtnCancelClick },

            'viewDocOrderIntsEdit button#btnHelp': { "click": this.onBtnHelpClick },
            'viewDocOrderIntsNomensEdit button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    // Селект Группы
    onTree_selectionchange: function (model, records) {
        //...
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        
        //Ext.getCmp("DirNomenName" + view.grid.UO_id).setValue(rec.get('DirNomenPatchFull'));
        //Ext.getCmp("DirNomenID" + view.grid.UO_id).setValue(rec.get('id'));
        //fun_controllerDocOrderIntsEdit_RequestPrice(rec.get('id'), view.grid.UO_id);

        //Ставим на Комбы признак (в конце метода снимим), что бы автоматически не обновляло их с сервера
        var cb1 = Ext.getCmp("DirNomen1ID" + view.grid.UO_id);
        var cb2 = Ext.getCmp("DirNomen2ID" + view.grid.UO_id); //cb2.setReadOnly(true); cb2.store.UO_Loaded = false; cb2.UO_NoAutoLoad = false; cb2.setValue(""); cb2.store.setData([], false);
        var cb3 = Ext.getCmp("DirNomenID" + view.grid.UO_id); //cb3.setReadOnly(true); cb3.store.UO_Loaded = false; cb3.UO_NoAutoLoad = false; cb3.setValue(""); cb3.store.setData([], false);
        
        controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, false, cb1, cb2, cb3); //, cb3, cb4, cb5, cb6


        //КомбоБоксы (Парсим "rec.get('DirNomenIDFull')" в массив)
        if (rec.get('DirNomenIDFull') == undefined) return;
        var arr = rec.get('DirNomenIDFull').split(',');

        
        if (arr.length >= 1) {
            cb1.setValue(arr[0]);

            var storeDirNomensGrid2 = cb2.store; //Ext.getCmp("viewDocOrderIntsPattern" + view.grid.UO_id).storeDirNomensGrid2;
            storeDirNomensGrid2.UO_Loaded = false;
            storeDirNomensGrid2.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=" + arr[0];
            storeDirNomensGrid2.arr = arr;
            storeDirNomensGrid2.rec = rec;
            storeDirNomensGrid2.load({ waitMsg: lanLoading });
            storeDirNomensGrid2.on('load', function () {
                if (storeDirNomensGrid2.UO_Loaded) return; //Уже загружали - выйти!
                storeDirNomensGrid2.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                cb2.setReadOnly(false); cb2.setValue("");

                if (storeDirNomensGrid2.arr.length >= 2) {
                    cb2.setValue(storeDirNomensGrid2.arr[1]);


                    var storeDirNomensGrid3 = cb3.store; //Ext.getCmp("viewDocOrderIntsPattern" + view.grid.UO_id).storeDirNomensGrid3;
                    storeDirNomensGrid3.UO_Loaded = false;
                    storeDirNomensGrid3.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=" + arr[1];
                    storeDirNomensGrid3.arr = arr;
                    storeDirNomensGrid3.rec = rec;
                    storeDirNomensGrid3.load({ waitMsg: lanLoading });
                    storeDirNomensGrid3.on('load', function () {
                        if (storeDirNomensGrid3.UO_Loaded) return; //Уже загружали - выйти!
                        storeDirNomensGrid3.UO_Loaded = true; //Нужно, что бы не срабатывало при каждой перегрузке "storeNomenTree.on('load', function () {"

                        cb3.setReadOnly(false); cb3.setValue("");


                        // !!! ТОВАР !!!
                        if (storeDirNomensGrid3.arr.length >= 3) {
                            cb3.setValue(storeDirNomensGrid3.arr[2]);
                            fun_controllerDocOrderIntsEdit_RequestPrice(rec.get('id'), view.grid.UO_id);
                        }


                    }, this, { single: true }); //storeDirNomensGrid3

                    
                    //Активизируем эвент Комбов
                    controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, true, cb1, cb2, cb3);

                } //if (arr.length > 2) {
                else { controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, true, cb1, cb2, cb3); } //, cb3, cb4, cb5, cb6

            }, this, { single: true }); //storeDirNomensGrid2

        } //if (arr.length > 1) {
        else { controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, true, cb1, cb2, cb3); } //, cb3, cb4, cb5, cb6


        //Наименование
        //Ext.getCmp("DirNomenName" + view.grid.UO_id).setValue(rec.get('DirNomenPatchFull'));
        //Ext.getCmp("DirNomenID" + view.grid.UO_id).setValue(rec.get('id'));

        //Ставим на Комбы признак (в конце метода снимим), что бы автоматически не обновляло их с сервера
        Ext.getCmp("DirNomen1ID" + view.grid.UO_id).UO_NoAutoLoad = true;
    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
    },



    //Марка === === ===
    onDirNomen1IDSelect: function (combo, records, eOpts) {
        if (!combo.UO_NoAutoLoad) {
            controllerDocOrderIntsEdit_onDirNomen1IDChange(combo, records);
        }
    },
    onDirNomen1IDChange: function (combo, newValue, oldValue) {
        /*if (!combo.UO_NoAutoLoad) {
            controllerDocOrderIntsEdit_onDirNomen1IDChange(combo, newValue);
        }*/
    },
    //Модель === === ===
    onDirNomen2IDSelect: function (combo, records, eOpts) {
        if (!combo.UO_NoAutoLoad) {
            controllerDocOrderIntsEdit_onDirNomen2IDChange(combo, records);
        }
    },
    onDirNomen2IDChange: function (combo, newValue, oldValue) {
    },
    //Товар === === ===
    onDirNomenIDSelect: function (combo, records, eOpts) {
        fun_controllerDocOrderIntsEdit_RequestPrice(records.data.DirNomenID, combo.UO_id);
    },
    onDirNomenIDChange: function (combo, newValue, oldValue) {
        //debugger;
        //fun_controllerDocOrderIntsEdit_RequestPrice(records.data.DirNomenID, combo.UO_id);
    },



    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //Проверка: если ввели вручную "Группа-2", то это ошибка!!!
        if (isNaN(parseInt(Ext.getCmp("DirServiceNomenID" + aButton.UO_id).getValue()))) {
            Ext.Msg.alert(lanOrgName, "Выберите Тип уст-ва из списка!");
            return;
        }

        //Проверка: если ввели вручную "Группа-2", то это ошибка!!!
        if (isNaN(parseInt(Ext.getCmp("DirNomen2ID" + aButton.UO_id).getValue()))) {
            Ext.Msg.alert(lanOrgName, "Выберите модель из списка!");
            return;
        }

        
        //Предоплата только для 1 и 2 типа.
        var DirOrderIntTypeID = parseInt(Ext.getCmp("DirOrderIntTypeID" + aButton.UO_id).getValue());
        if (DirOrderIntTypeID <= 2) {

            var PrepaymentSum = parseFloat(Ext.getCmp("PrepaymentSum" + aButton.UO_id).getValue());
            if ((PrepaymentSum == 0 && parseInt(Ext.getCmp("DirOrderIntTypeID" + aButton.UO_id).getValue()) != 1) || isNaN(PrepaymentSum)) {
                Ext.MessageBox.show({
                    icon: Ext.MessageBox.WARNING, //ERROR,INFO,QUESTION,WARNING
                    width: 300,
                    title: lanOrgName,
                    msg: "Вы не заполнили поле <b style='color: red;'>предоплата</b>!",
                    buttonText: { yes: "Заполнить", no: "Не заполнять", cancel: "Отмена" },
                    fn: function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp("PrepaymentSum" + aButton.UO_id).focus();
                            return;
                        }
                        else if (btn == "no") {

                            if (varPayType == 0) {
                                Ext.MessageBox.show({
                                    icon: Ext.MessageBox.QUESTION,
                                    width: 300,
                                    title: lanOrgName,
                                    msg: "Выбирите <b>Тип оплаты</b>!",
                                    buttonText: { yes: "Наличная", no: "Безналичная", cancel: "Отмена" },
                                    fn: function (btn) {
                                        if (btn == "yes") {
                                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                                            controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                                        }
                                        else if (btn == "no") {
                                            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                                            controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                                        }
                                    }
                                });
                            }
                            else if (varPayType == 1) {
                                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                                controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                            }
                            else if (varPayType == 2) {
                                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                                controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                            }

                        }
                    }
                });
            }
            else if (parseInt(Ext.getCmp("DirOrderIntTypeID" + aButton.UO_id).getValue()) != 1) {

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
                                controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                            }
                            else if (btn == "no") {
                                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                                controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                            }
                        }
                    });
                }
                else if (varPayType == 1) {
                    Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                    controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                }
                else if (varPayType == 2) {
                    Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                    controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                }

            }
            else {
                Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
            }

        }
        else {

            Ext.getCmp("PrepaymentSum" + aButton.UO_id).setValue(0);

            Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
            controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions);

        }

    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-tovar/", '_blank');
    }
});


//Функия сохранения
function controllerDocOrderIntsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    //Заполнение полей: DirNomenXID, DirNomenXName. Т.к. Комбы у нас редактируемые и в них можно вписать новый товар

    //1.
    if (isNaN(parseInt(Ext.getCmp("DirNomen1ID" + aButton.UO_id).getValue()))) {
        Ext.Msg.alert(lanOrgName, "Марка должна быть обязательно выбрана с списка!");
        return;
    }
    //2.
    if (isNaN(parseInt(Ext.getCmp("DirNomen2ID" + aButton.UO_id).getValue()))) {
        Ext.Msg.alert(lanOrgName, "Модель должна быть обязательно выбрана с списка!");
        return;
    }
    //0.

    if (!isNumber(Ext.getCmp("DirNomenID" + aButton.UO_id).rawValue) && !isNumber(Ext.getCmp("DirNomenID" + aButton.UO_id).getValue())) {
        //Новый товар
        Ext.getCmp("DirNomenName" + aButton.UO_id).setValue(Ext.getCmp("DirNomenID" + aButton.UO_id).rawValue);
        Ext.getCmp("DirNomenID" + aButton.UO_id).setValue(null);
    }
    else if (Ext.getCmp("DirNomenID" + aButton.UO_id).getValue() == null || Ext.getCmp("DirNomenID" + aButton.UO_id).rawValue.length == 0) {
        Ext.Msg.alert(lanOrgName, "Не заполненно поле Товар!<br />Если товар существует - выбирите его из выпадающего списка.<br />Если товар новый - введите буквенное наименование товара.");
        return;
    }
    else if (Ext.getCmp("DirNomenID" + aButton.UO_id).rawValue == Ext.getCmp("DirNomenID" + aButton.UO_id).getValue()) {
        Ext.Msg.alert(lanOrgName, "В наименовани товара НЕ должно состоять из одних цифр!");
        return;
    }


    //Ext.getCmp("DirNomen1ID" + aButton.UO_id).setValue(Ext.getCmp("DirNomen1ID" + aButton.UO_id).getValue());
    //Ext.getCmp("DirNomen1Name" + aButton.UO_id).setValue(Ext.getCmp("DirNomen1ID" + aButton.UO_id).rawValue);
    //2.
    /*
    if (isNaN(parseInt(Ext.getCmp("DirNomen2ID" + aButton.UO_id).getValue()))) { }
    else {
        Ext.getCmp("DirNomen2ID" + aButton.UO_id).setValue(Ext.getCmp("DirNomen2ID" + aButton.UO_id).getValue());
    }
    Ext.getCmp("DirNomen2Name" + aButton.UO_id).setValue(Ext.getCmp("DirNomen2ID" + aButton.UO_id).rawValue);
    */

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
            if (Ext.getCmp("DirOrderIntTypeID" + aButton.UO_id).getValue() > 2) {

                var sData = Ext.decode(action.response.responseText);

                if (sData.success) {

                    var UO_id = Ext.getCmp(Ext.getCmp(aButton.UO_idCall).UO_idCall).UO_id;

                    //Ext.getCmp("btnStatus50" + UO_id).setPressed(true);

                    /*
                    Ext.getCmp("DirServiceStatusID" + UO_id).setValue(5);
                    controllerDocServiceWorkshops_DirServiceStatusID_ChangeButton(UO_id);
                    */
                    
                    aButton5 = Ext.getCmp("btnStatus50" + UO_id);
                    if (Ext.getCmp("DirOrderIntTypeID" + aButton.UO_id).getValue() == 3) {
                        controllerDocServiceWorkshops_ChangeStatus_Request(aButton5, "Сформирован заказ №" + sData.data.DocOrderIntID);
                    }
                    else if (Ext.getCmp("DirOrderIntTypeID" + aButton.UO_id).getValue() == 4) {
                        controllerDocSecondHandWorkshops_ChangeStatus_Request(aButton5, 0, "Сформирован заказ №" + sData.data.DocOrderIntID);
                    }

                }
                else {
                    Ext.Msg.alert(lanOrgName, "Ошибка формирования заказа!");
                }
            }


            Ext.Msg.alert(lanOrgName, "Заказ принят!");
            Ext.getCmp(aButton.UO_idMain).close();
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};

//ComboBox
function controllerDocOrderIntsEdit_onDirNomen1IDChange(combo, records)
{
    var storeDirNomensGrid = Ext.getCmp("viewDocOrderIntsPattern" + combo.UO_id).storeDirNomensGrid2;
    storeDirNomensGrid.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=" + records.data.DirNomenID;
    storeDirNomensGrid.load({ waitMsg: lanLoading });

    Ext.getCmp("DirNomenID" + combo.UO_id).setValue(records.data.DirNomenID);
    //Ext.getCmp("DirNomenName" + combo.UO_id).setValue(records.data.DirNomenName);

    Ext.getCmp("DirNomen2ID" + combo.UO_id).setReadOnly(false); Ext.getCmp("DirNomen2ID" + combo.UO_id).setValue("");
    Ext.getCmp("DirNomenID" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID4" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID4" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID5" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID5" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID6" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID6" + combo.UO_id).setValue("");

    //Получение цены
    //records.data.id = records.data.DirNomenID;
    //fun_viewDocPurchTabsEdit_RequestPrice(undefined, records, combo.UO_id, combo.UO_id); //combo.UO_idCall
    fun_controllerDocOrderIntsEdit_RequestPrice(records.data.DirNomenID, combo.UO_id);
}
function controllerDocOrderIntsEdit_onDirNomen2IDChange(combo, records) {

    var storeDirNomensGrid = Ext.getCmp("viewDocOrderIntsPattern" + combo.UO_id).storeDirNomensGrid3;
    storeDirNomensGrid.proxy.url = HTTP_DirNomens + "?type=Grid&GroupID=" + records.data.DirNomenID;
    storeDirNomensGrid.load({ waitMsg: lanLoading });

    Ext.getCmp("DirNomenID" + combo.UO_id).setValue(records.data.DirNomenID);
    /*Ext.getCmp("DirNomenName" + combo.UO_id).setValue(
        Ext.getCmp("DirNomen1ID" + combo.UO_id).rawValue + " / " +
        records.data.DirNomenName
    );*/

    Ext.getCmp("DirNomenID" + combo.UO_id).setReadOnly(false); Ext.getCmp("DirNomenID" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID4" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID4" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID5" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID5" + combo.UO_id).setValue("");
    //Ext.getCmp("DirNomenID6" + combo.UO_id).setReadOnly(true); Ext.getCmp("DirNomenID6" + combo.UO_id).setValue("");

    //Получение цены
    //records.data.id = records.data.DirNomenID;
    //fun_viewDocPurchTabsEdit_RequestPrice(undefined, records, combo.UO_id, combo.UO_idCall);
    fun_controllerDocOrderIntsEdit_RequestPrice(records.data.DirNomenID, combo.UO_id);
}


//UO_NoAutoLoad - активизировать "onDirNomenIDXChange" (, cb3, cb4, cb5, cb6)controllerDocOrderIntsEdit_UO_NoAutoLoad
function controllerDocOrderIntsEdit_UO_NoAutoLoad(rec, UO_NoAutoLoad, cb1, cb2, cb3)
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

        
        //fun_controllerDocOrderIntsEdit_RequestPrice(rec.get('id'), cb1.UO_id);
    }
}

