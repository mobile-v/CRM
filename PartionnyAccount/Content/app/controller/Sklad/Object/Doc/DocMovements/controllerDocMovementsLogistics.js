Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocMovements/controllerDocMovementsLogistics", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocMovementsLogistics': { close: this.this_close },


            // PanelGrid0: Список Клик по Гриду
            'viewDocMovementsLogistics [itemId=PanelGrid0_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick,

                edit: this.onPanelGrid0Edit,
            },


            // PanelGrid2: Список Клик по Гриду
            'viewDocMovementsLogistics [itemId=PanelGrid2_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid3: Список Клик по Гриду
            'viewDocMovementsLogistics [itemId=PanelGrid3_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid9: Список Клик по Гриду
            'viewDocMovementsLogistics [itemId=PanelGrid9_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            'viewDocMovementsLogistics #TriggerSearchGrid': {
                "ontriggerclick": this.onTriggerSearchGridClick1,
                "specialkey": this.onTriggerSearchGridClick2,
                "change": this.onTriggerSearchGridClick3
            },
            'viewDocMovementsLogistics #DateS': { select: this.onGrid_DateS },
            'viewDocMovementsLogistics #DatePo': { select: this.onGrid_DatePo },



            // Кнопки-статусы
            'viewDocMovementsLogistics button#btnStatus1': { "click": this.onBtnStatus1Click },
            'viewDocMovementsLogistics button#btnStatus2': { "click": this.onBtnStatus2Click },
            'viewDocMovementsLogistics button#btnStatus3': { "click": this.onBtnStatus3Click },
            'viewDocMovementsLogistics button#btnStatus4': { "click": this.onBtnStatus4Click },
            'viewDocMovementsLogistics button#btnTabShow': { "click": this.onBtnTabShowClick },
            'viewDocMovementsLogistics button#btnDocEdit': { "click": this.onBtnDocEditClick },


        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    // GridX: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGridX_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGridX_itemclick: function (view, record, item, index, eventObj) {
        controllerDocMovementsLogistics_onGridX_itemclick(view.grid, false); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGridX_itemdblclick: function (view, record, item, index, e) {
        controllerDocMovementsLogistics_onGridX_itemclick(view.grid.UO_id, false);
    },
    onPanelGrid0Edit: function (aEditor, aE1) {

        //aE1.record.data.DocMovementID = Ext.getCmp("DocMovementID" + aEditor.grid.UO_id).getValue();
        var dataX = Ext.encode(aE1.record.data);
        //var ddd = ffff;

        //Сохранение
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocMovements + aE1.record.data.DocMovementID + "/?DateDone=" + aE1.record.data.DateDone,
            method: 'PUT',
            params: { recordsDataX: dataX },

            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
                else {
                    //Обновляем ЛОГ
                    Ext.getCmp("gridLog0_" + aEditor.grid.UO_id).getStore().load();
                }
            },
            failure: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
            }
        });
    },

    //Поиск
    onTriggerSearchGridClick1: function (aButton, aEvent) {
        funGridDoc(aButton.UO_id, HTTP_DocMovements + "?DirMovementStatusID=4&DirWarehouseID=" + varDirWarehouseID);
    },
    onTriggerSearchGridClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            funGridDoc(f.UO_id, HTTP_DocMovements + "?DirMovementStatusID=4&DirWarehouseID=" + varDirWarehouseID);
        }
    },
    onTriggerSearchGridClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) funGridDoc(e.UO_id, HTTP_DocMovements + "?DirMovementStatusID=4&DirWarehouseID=" + varDirWarehouseID);
    },
    onGrid_DateS: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocMovements + "?DirMovementStatusID=4&DirWarehouseID=" + varDirWarehouseID);
    },
    onGrid_DatePo: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocMovements + "?DirMovementStatusID=4&DirWarehouseID=" + varDirWarehouseID);
    },


    // Кнопки-статусы *** *** *** *** *** *** *** *** *** *** *** *** ***
    onBtnStatus1Click: function (aButton, aEvent, aOptions) {
        
        /*
        Ext.Msg.prompt(lanOrgName, "Переместить документ в перемещение? Напишите причину:",
            //height = 300,
            function (btnText, sDiagnosticRresults) {
                if (btnText === 'ok') {
                    //rec.data.DiagnosticRresults = sDiagnosticRresults;
                    controllerDocMovementsLogistics_ChangeStatus_Request(aButton, sDiagnosticRresults);
                }
                else {
                    controllerDocMovementsLogistics_DirMovementStatusID_ChangeButton(aButton.UO_id);
                }
            },
            this
        ).setWidth(400);

        */

        var msgbox = Ext.Msg.prompt(lanOrgName, "Переместить документ в перемещение? Напишите причину:",
            function (btnText, sDiagnosticRresults) {
                if (btnText === 'ok') {
                    controllerDocMovementsLogistics_ChangeStatus_Request(aButton, sDiagnosticRresults);
                }
                else {
                    controllerDocMovementsLogistics_DirMovementStatusID_ChangeButton(aButton.UO_id);
                }
            },
            this
        ).setWidth(400);

        msgbox.textField.inputEl.dom.type = 'text';

    },
    onBtnStatus2Click: function (aButton, aEvent, aOptions) {

        controllerDocMovementsLogistics_ChangeStatus_Request(aButton, "");

    },
    onBtnStatus3Click: function (aButton, aEvent, aOptions) {

        var msgbox = Ext.Msg.prompt(lanOrgName, "Штрих-Код Курьера",
            function (btnText, sDiagnosticRresults) {
                if (btnText === 'ok') {
                    controllerDocMovementsLogistics_ChangeStatus_Request(aButton, sDiagnosticRresults);
                }
                else {
                    controllerDocMovementsLogistics_DirMovementStatusID_ChangeButton(aButton.UO_id);
                }
            },
            this
        ).setWidth(400);

        msgbox.textField.inputEl.dom.type = 'password';

    },
    onBtnStatus4Click: function (aButton, aEvent, aOptions) {

        Ext.Msg.show({
            title: lanOrgName,
            msg: "Переместится партии товара на склад " + Ext.getCmp("DirWarehouseNameTo" + aButton.UO_id).getValue() + "?",
            buttons: Ext.Msg.YESNO,
            fn: function (btn) {
                if (btn == "yes") {
                    controllerDocMovementsLogistics_ChangeStatus_Request(aButton, "");
                }
            },
            icon: Ext.window.MessageBox.QUESTION
        });

    },
    onBtnTabShowClick: function (aButton, aEvent, aOptions) {

        if (Ext.getCmp("grid1_" + aButton.UO_id).hidden) { Ext.getCmp("grid1_" + aButton.UO_id).setVisible(true); }
        else { Ext.getCmp("grid1_" + aButton.UO_id).setVisible(false); }

    },
    onBtnDocEditClick: function (aButton, aEvent, aOptions) {

        var UO_idCall;
        
        var activeTab = Ext.getCmp("tab_" + aButton.UO_id).getActiveTab();
        switch (activeTab.itemId)
        {
            case "PanelGrid0_": UO_idCall = "PanelGrid0_" + aButton.UO_id; break;
            case "PanelGrid2_": UO_idCall = "PanelGrid2_" + aButton.UO_id; break;
            case "PanelGrid3_": UO_idCall = "PanelGrid3_" + aButton.UO_id; break;
            case "PanelGrid9_": UO_idCall = "PanelGrid9_" + aButton.UO_id; break;
        }

        var Params = [
            UO_idCall, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocMovementsEdit", Params);

    },

});



//Клик по ГридамX
function controllerDocMovementsLogistics_onGridX_itemclick(view_grid, btnSave) {

    var id = view_grid.UO_id;
    var itemId = view_grid.itemId;



    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
    if (Ext.getCmp(itemId + id) == undefined) return;
    var IdcallModelData = Ext.getCmp(itemId + id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

    //Если эта запись уже открыта на редактирования, то повторно её не открывать. Иначе не сможем редактировать "Дату Готовности"
    if (Ext.getCmp("form_" + id).isVisible() && IdcallModelData.DocID == Ext.getCmp("DocID" + id).getValue()) return;

    //Если запись помечена на удаление, то сообщить об этом и выйти
    if (IdcallModelData.Del == true) {
        //Разблокировка вызвавшего окна
        ObjectEditConfig_UO_idCall_true_false(false);

        Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
        return;
    }

    Ext.getCmp("btnStatus1" + id).setPressed(false);
    Ext.getCmp("btnStatus1" + id).setVisible(true);
    Ext.getCmp("btnStatus2" + id).setVisible(true);
    Ext.getCmp("btnStatus3" + id).setVisible(true);
    Ext.getCmp("btnStatus4" + id).setVisible(true);
    Ext.getCmp("btnTabShow" + id).setVisible(true);
    Ext.getCmp("btnDocEdit" + id).setVisible(true);
    Ext.getCmp("grid1_" + id).setVisible(false);

    Ext.getCmp("gridLog0_" + id).enable();


    //Форма
    var widgetXForm = Ext.getCmp("form_" + id);
    widgetXForm.form.url = HTTP_DocMovements + IdcallModelData.DocMovementID; //С*ка глючит фреймворк и присвивает в форме старый УРЛ!!!
    widgetXForm.setVisible(true);
    widgetXForm.reset();
    widgetXForm.UO_Loaded = false;

    //Лог
    var widgetX = Ext.getCmp("viewDocMovementsLogistics" + id);
    widgetX.storeLogMovementsGrid0.setData([], false);
    widgetX.storeLogMovementsGrid0.proxy.url = HTTP_LogMovements + "?DocMovementID=" + IdcallModelData.DocMovementID;
    widgetX.storeLogMovementsGrid0.UO_Loaded = false;



    widgetXForm.load({
        method: "GET",
        timeout: varTimeOutDefault,
        waitMsg: lanLoading,
        //url: HTTP_DocMovements + IdcallModelData.DocMovementID + "/?DocID=" + IdcallModelData.DocID,
        success: function (form, action) {

            //Статусы и Кнопки
            controllerDocMovementsLogistics_DirMovementStatusID_ChangeButton(id);

            //Log
            widgetX.storeLogMovementsGrid0.load({ waitMsg: lanLoading });

            //Tab
            widgetX.storeDocMovementTabsGrid.proxy.url = HTTP_DocMovementTabs + "?DocMovementID=" + Ext.getCmp("DocMovementID" + id).getValue();
            widgetX.storeDocMovementTabsGrid.load({ waitMsg: lanLoading });

        },
        failure: function (form, action) {
            funPanelSubmitFailure(form, action);
            //widgetX.focus(); //Фокус на открывшийся Виджет
        }

    });

}


//Смена Статуса
function controllerDocMovementsLogistics_ChangeStatus_Request(aButton, sDiagnosticRresults) {
    //Старый ID-шние статуса
    var locDirMovementStatusID_OLD = parseInt(Ext.getCmp("DirMovementStatusID" + aButton.UO_id).getValue());

    //Новый ID-шние статуса
    var locDirMovementStatusID = parseInt(controllerDocMovementsLogistics_DirMovementStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, false));
    if (isNaN(locDirMovementStatusID)) { return; }

    //Запрос на сервер на смену статуса
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocMovements + Ext.getCmp("DocMovementID" + aButton.UO_id).getValue() + "/" + locDirMovementStatusID + "/?sDiagnosticRresults=" + sDiagnosticRresults,
        method: 'PUT',

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                controllerDocMovementsLogistics_DirMovementStatusID_ChangeButton(aButton.UO_id);
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Меняем ID-шние статуса
                controllerDocMovementsLogistics_DirMovementStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, true);

                //Статусы и Кнопки
                controllerDocMovementsLogistics_DirMovementStatusID_ChangeButton(aButton.UO_id);

                //Обновить
                var activeTab = Ext.getCmp("tab_" + aButton.UO_id).getActiveTab();
                activeTab.store.load({ waitMsg: lanLoading });
                //Лог
                Ext.getCmp("gridLog0_" + aButton.UO_id).getStore().load();

            }
        },
        failure: function (result) {
            controllerDocMovementsLogistics_DirMovementStatusID_ChangeButton(aButton.UO_id);

            var sData = Ext.decode(result.responseText);
            Ext.Msg.alert(lanOrgName, sData); //Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
        }
    });
}
//Вернуть и/или поменять "DirMovementStatusID"
function controllerDocMovementsLogistics_DirMovementStatusID_ChangeStatus(id, itemId, bchange) {
    switch (itemId) {
        case "btnStatus1":
            if (bchange) { Ext.getCmp("DirMovementStatusID" + id).setValue(1); }
            else { return 1; }
            break;
        case "btnStatus2":
            if (bchange) { Ext.getCmp("DirMovementStatusID" + id).setValue(2); }
            else { return 2; }
            break;
        case "btnStatus3":
            if (bchange) { Ext.getCmp("DirMovementStatusID" + id).setValue(3); }
            else { return 3; }
            break;
        case "btnStatus4":
            if (bchange) { Ext.getCmp("DirMovementStatusID" + id).setValue(4); }
            else { return 4; }
            break;
    }
}
//Статусы и Кнопки - выставить
function controllerDocMovementsLogistics_DirMovementStatusID_ChangeButton(id) {

    switch (parseInt(Ext.getCmp("DirMovementStatusID" + id).getValue())) {
        case 1:
            //Перемещение
            Ext.getCmp("btnStatus1" + id).setPressed(true);
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);

            break;
        case 2:
            //Логистика: в ожидании курьера
            Ext.getCmp("btnStatus1" + id).setPressed(false);
            Ext.getCmp("btnStatus2" + id).setPressed(true);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            break;
        case 3:
            //Логистика: курьер принял
            Ext.getCmp("btnStatus1" + id).setPressed(false);
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(true);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            break;
        case 4:
            //Логистика: курьер отдал
            Ext.getCmp("btnStatus1" + id).setPressed(false);
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(true);
            break;
    }

}