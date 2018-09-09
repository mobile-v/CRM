Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandInvsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocSecondHandInvsEdit',


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewDocSecondHandInvsEditClose: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },

    //Не используется
    onSpisatS_select: function (combo, records, eOpts) {
        console.log(records.data.SpisatS);
    },

    onSpisatS_changet: function (combo, records, eOpts) {
        //console.log(records);
        if (records == 1) {
            Ext.getCmp("SpisatSDirEmployeeID" + combo.UO_id).setDisabled(true);
            Ext.getCmp("SpisatSDirEmployeeID" + combo.UO_id).setValue(undefined);
        }
        else {
            Ext.getCmp("SpisatSDirEmployeeID" + combo.UO_id).setDisabled(false);
        }
    },


    //Подписи
    onBtn1PodpisClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInvsEdit_onBtn1PodpisClick(aButton, 1);
    },
    onBtn11PodpisClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInvsEdit_onBtn1PodpisClick(aButton, 11);
    },
    onBtn2PodpisClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInvsEdit_onBtn1PodpisClick(aButton, 2);
    },
    onBtn21PodpisClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInvsEdit_onBtn1PodpisClick(aButton, 21);
    },

    onDirEmployee2IDSelect: function (combo, records, eOpts) {

        if (parseInt(Ext.getCmp("DocSecondHandInvID" + combo.UO_id).value) > 0) { }
        else { return; }

        var sUrl = HTTP_DocSecondHandInvs + Ext.getCmp("DocSecondHandInvID" + combo.UO_id).getValue() + "/" + records.data.DirEmployeeID + "/";

        //Запрос на сервер для подписи
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: sUrl,
            method: 'PUT',

            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
                else {

                    

                }
            },
            failure: function (result) {
                var sData = Ext.decode(result.responseText);
                Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
            }
        });

    },



    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        //model.view.ownerGrid.down("#btnGridEdit").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnGridDelete").setDisabled(records.length === 0);
    },

    onGrid_BtnGridRefresh: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;

        //Выбран Тип
        var DirSecondHandStatusIDS = parseInt(Ext.getCmp("LoadXFrom" + id).getValue()), DirSecondHandStatusIDPo = parseInt(Ext.getCmp("LoadXFrom" + id).getValue());
        if (Ext.getCmp("LoadXFrom" + id).getValue() == null) {
            Ext.Msg.alert(lanOrgName, "Выбирите тип загружаемых аппаратов!");
            return;
        }
        else {
            if (parseInt(Ext.getCmp("LoadXFrom" + id).getValue()) == 0) {
                DirSecondHandStatusIDS = 1;
                DirSecondHandStatusIDPo = 9;
            }
        }

        Ext.getCmp("btnGridRefresh" + id).disable();
   
        var store = Ext.getCmp("grid_" + id).store;
        store.proxy.url =
            HTTP_DocSecondHandPurches +
            "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() +
            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + id).getValue() +
            "&DirSecondHandStatusIDS=" + DirSecondHandStatusIDS + "&DirSecondHandStatusIDPo=" + DirSecondHandStatusIDPo + "" + 
            "&collectionWrapper=DocSecondHandInvTab";
        
        store.load({ waitMsg: lanLoading });
        store.on('load', function () {
            Ext.getCmp("btnGridRefresh" + id).enable();
            Ext.getCmp("LoadFrom" + id).setValue(Ext.getCmp("LoadXFrom" + id).getValue());
        });

    },

    //Новая: Удалить позицию
    onGrid_BtnGridDelete: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName,
            msg: lanDelete + "?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    var selection = Ext.getCmp("grid_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
                    if (selection) { Ext.getCmp("grid_" + aButton.UO_id).store.remove(selection); }
                }
            }
        });
    },



    // === Кнопки === === ===

    //Отменить проведение
    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { viewcontrollerDocSecondHandInvsEdit_onBtnSaveClick(aButton); } } //, aEvent, aOptions
        });
    },

    //Провести
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        if (Ext.getCmp("DirEmployee2Podpis" + aButton.UO_id).getValue()) {
            viewcontrollerDocSecondHandInvsEdit_onBtnSaveClick(aButton);
        }
        else {
            Ext.Msg.alert(lanOrgName, "Проводить документ можно только после подписи администратора точки!");
        }
    },

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInvsEdit_onBtnSaveClick(aButton);
    },

    onBtnSaveCloseClick: function (aButton, aEvent, aOptions) {
        viewcontrollerDocSecondHandInvsEdit_onBtnSaveClick(aButton);
    },

    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },

    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-second-hand-inventories/", '_blank');
    },

    //Распечатать
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {
        //aButton.UO_Action: html, excel
        //alert(aButton.UO_Action);

        //Проверка: если форма ещё не сохранена, то выход
        if (!Ext.getCmp("DocSecondHandInvID" + aButton.UO_id).getValue()) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

        //Открытие списка ПФ
        var Params = [
            aButton.id,
            true, //UO_Center
            true, //UO_Modal
            aButton.UO_Action, //UO_Function_Tree: Html или Excel
            undefined,
            undefined,
            undefined,
            Ext.getCmp("DocID" + aButton.UO_id).getValue(),
            76,
        ]
        ObjectConfig("viewListObjectPFs", Params);

    },
    
});




//Функия сохранения
function viewcontrollerDocSecondHandInvsEdit_onBtnSaveClick(aButton) { 

    //Сохранять/проводить иеет право только Товаровед
    var DirEmployee1ID = parseInt(Ext.getCmp("DirEmployee1ID" + aButton.UO_id).getValue());
    if (varDirEmployeeID != 1 && DirEmployee1ID != varDirEmployeeID) { Ext.Msg.alert(lanOrgName, "Документ сохранять или проводить имеют право или Администратор или Товаровед создавший документ!"); return; }

    //Спецификация (табличная часть)
    var recordsDocSecondHandInvTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocSecondHandInvTab.push(rec.data); });

    //Проверка
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Точку!"); return; }
    //if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите аппараты для списания!"); return; }
    if (Ext.getCmp("SpisatS" + aButton.UO_id).getValue() == 2 && Ext.getCmp("SpisatSDirEmployeeID" + aButton.UO_id).getValue() == undefined) { Ext.Msg.alert(lanOrgName, "Выбирите сотрудника!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    
    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocSecondHandInvs + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocSecondHandInvID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocSecondHandInvs + "?id=" + parseInt(Ext.getCmp("DocSecondHandInvID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }
    else {
        Ext.getCmp("LoadFrom" + aButton.UO_id).setValue(Ext.getCmp("LoadXFrom" + aButton.UO_id).getValue());
    }
    
    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocSecondHandInvTab: Ext.encode(recordsDocSecondHandInvTab) },

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {

            if (aButton.UO_Action == "held_cancel") {
                Ext.getCmp("Held" + aButton.UO_id).setValue(false);
                Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(false);
                Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(true);
                Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(true);
            }
            else if (aButton.UO_Action == "held") {
                Ext.getCmp("Held" + aButton.UO_id).setValue(true);
                Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(true);
                Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(false);
                Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(false);
            }


            //Если новая накладная присваиваем полученные номера!
            if (!Ext.getCmp('DocID' + aButton.UO_id).getValue()) {
                var sData = action.result.data;
                Ext.getCmp('DocID' + aButton.UO_id).setValue(sData.DocID);
                Ext.getCmp('DocSecondHandInvID' + aButton.UO_id).setValue(sData.DocSecondHandInvID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocSecondHandInvID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocSecondHandInvID);

                Ext.getCmp('viewDocSecondHandInvsEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocSecondHandInvsEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocSecondHandInvID" + aButton.UO_id).getValue() + ")");
                Ext.getCmp("btnPrint" + aButton.UO_id).setVisible(true);
                Ext.getCmp("btn1Podpis" + aButton.UO_id).setDisabled(false);
            }


            //Закрыть
            if (aButton.UO_Action == "save_close" || aButton.UO_Action == "held") { Ext.getCmp(aButton.UO_idMain).close(); }
            else {
                //Ext.getCmp("btn1Podpis" + aButton.UO_id).setDisabled(false);
            }

            //Перегрузить грид, если грид открыт
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }

        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};


//Подпись
function viewcontrollerDocSecondHandInvsEdit_onBtn1PodpisClick(aButton, iType) { 

    //Подпись Товароведа
    //Алгоритм:
    //1. Проверки
    //2. Отправка запроса на сервер за изменением записи "DirEmployee1Podpis=true"
    //3. Разблокировка Комбы для выбора админа точки

    //0. Переменные
    var
        DirEmployee1ID = parseInt(Ext.getCmp("DirEmployee1ID" + aButton.UO_id).getValue()),
        DirEmployee2ID = parseInt(Ext.getCmp("DirEmployee2ID" + aButton.UO_id).getValue()),
        DirEmployee1Podpis = Ext.getCmp("DirEmployee1Podpis" + aButton.UO_id).getValue(), DirEmployee2Podpis = Ext.getCmp("DirEmployee2Podpis" + aButton.UO_id).getValue(),
        DocSecondHandInvID = Ext.getCmp("DocSecondHandInvID" + aButton.UO_id).getValue(),
        store = Ext.getCmp("grid_" + aButton.UO_id).store;

    var sUrl = HTTP_DocSecondHandInvs + Ext.getCmp("DocSecondHandInvID" + aButton.UO_id).getValue() + "/";
   
    if (iType == 1) {
        if (isNaN(DirEmployee2ID)) { Ext.Msg.alert(lanOrgName, "Выберите администратора точки!"); return; }
        if (varDirEmployeeID != 1 && DirEmployee1ID != varDirEmployeeID) { Ext.Msg.alert(lanOrgName, "Документ подписывать имеют право или Администратор или Товаровед создавший документ!"); return; }
        if (DirEmployee1Podpis) { Ext.Msg.alert(lanOrgName, "Документ уже подписан товароведом!"); return; }
        if (DirEmployee2Podpis) { Ext.Msg.alert(lanOrgName, "Документ уже подписан администратором точки! Попросите его снять подпись!"); return; }
        if (!DocSecondHandInvID || store.data.length == 0) { Ext.Msg.alert(lanOrgName, "Подпись документа станет доступной только после <span style='color: red'>сохранения документа и заполнения табличной части</span>!<br />И учтите, что после Вашей подписи администратор точки получит сообщение о том что данный документ доступен ему для подписи!<br />Поэтому, перед подписью, заполните табличную часть!"); return; }


        sUrl += Ext.getCmp("DirEmployee1ID" + aButton.UO_id).getValue() + "/0/?iType=1";
    }
    else if (iType == 11) {
        if (!DirEmployee1Podpis) { Ext.Msg.alert(lanOrgName, "Документ ещё не подписан Товароведом!"); return; }
        if (DirEmployee2Podpis) { Ext.Msg.alert(lanOrgName, "Документ уже подписан администратором точки! Попросите его снять подпись!"); return; }

        sUrl += Ext.getCmp("DirEmployee1ID" + aButton.UO_id).getValue() + "/0/?iType=11";
    }
    else if (iType == 2) {
        if (!DirEmployee1Podpis) { Ext.Msg.alert(lanOrgName, "Документ ещё не подписан Товароведом!"); return; }
        if (DirEmployee2Podpis) { Ext.Msg.alert(lanOrgName, "Документ уже подписан администратором точки!"); return; }

        sUrl += "0/" + Ext.getCmp("DirEmployee2ID" + aButton.UO_id).getValue() + "/?iType=2";
    }
    else if (iType == 21) {
        if (!DirEmployee2Podpis) { Ext.Msg.alert(lanOrgName, "Документ ещё не подписан администратором точки!"); return; }

        sUrl += "0/" + Ext.getCmp("DirEmployee2ID" + aButton.UO_id).getValue() + "/?iType=21";
    }

    //Запрос на сервер для подписи
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: sUrl,
        method: 'PUT',

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {

                if (iType == 1) {
                    Ext.getCmp("btn1Podpis" + aButton.UO_id).setDisabled(true);
                    Ext.getCmp("btn11Podpis" + aButton.UO_id).setDisabled(false);
                    //Ext.getCmp("DirEmployee2ID" + aButton.UO_id).setReadOnly(false);

                    Ext.getCmp("DirEmployee1Podpis" + aButton.UO_id).setValue(true);

                    Ext.Msg.alert(lanOrgName, "<b style='color: red'>Внимание!</b><br/>Теперь документ доступен для подписи администратором точки!<br/>После подписи админом - документ готов к проведению (и редактировать его больше нельзя)!");
                }
                else if (iType == 11) {
                    Ext.getCmp("btn1Podpis" + aButton.UO_id).setDisabled(false);
                    Ext.getCmp("btn11Podpis" + aButton.UO_id).setDisabled(true);
                    //Ext.getCmp("DirEmployee2ID" + aButton.UO_id).setReadOnly(true);

                    Ext.getCmp("DirEmployee1Podpis" + aButton.UO_id).setValue(false);
                }
                else if (iType == 2) {
                    Ext.getCmp("btn2Podpis" + aButton.UO_id).setDisabled(true);
                    Ext.getCmp("btn21Podpis" + aButton.UO_id).setDisabled(false);

                    Ext.getCmp("DirEmployee2Podpis" + aButton.UO_id).setValue(true);

                    Ext.Msg.alert(lanOrgName, "Документ готов к Проведению товароведом!");
                }
                else if (iType == 21) {
                    Ext.getCmp("btn2Podpis" + aButton.UO_id).setDisabled(false);
                    Ext.getCmp("btn21Podpis" + aButton.UO_id).setDisabled(true);

                    Ext.getCmp("DirEmployee2Podpis" + aButton.UO_id).setValue(false);
                }

            }
        },
        failure: function (result) {
            var sData = Ext.decode(result.responseText);
            Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
        }
    });

};

