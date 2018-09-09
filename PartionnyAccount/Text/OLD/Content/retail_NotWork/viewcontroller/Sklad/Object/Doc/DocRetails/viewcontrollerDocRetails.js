Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetails/viewcontrollerDocRetails', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocRetails',


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewDocRetailsClose: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },

    //Грид (itemId=grid)
    onBtnNewClick: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocRetailsEdit", Params);
    },

    //'viewDocRetails button#btnNewCopy': { click: this.Grid_BtnNewCopy },
    onBtnNewCopyClick: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            3     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocRetailsEdit", Params);
    },

    //'viewDocRetails button#btnEdit': { click: this.onGrid_BtnEdit },
    onBtnEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocRetailsEdit", Params);
    },

    //'viewDocRetails button#btnDelete': { click: this.onGrid_BtnDelete },
    onBtnDeleteClick: function (aButton, aEvent, aOptions) {
        //Формируем сообщение: Удалить или снять пометку на удаление
        var sMsg = lanDelete;
        if (Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;

        //Процес Удаление или снятия пометки
        Ext.MessageBox.show({
            title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    //Лоадер
                    var loadingMask = new Ext.LoadMask({
                        msg: 'Please wait...',
                        target: Ext.getCmp("grid_" + aButton.UO_id)
                    });
                    loadingMask.show();
                    //Выбранный ID-шник Грида
                    var DocRetailID = Ext.getCmp("grid_" + aButton.UO_id).view.getSelectionModel().getSelection()[0].data.DocRetailID;
                    //Запрос на удаление
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_DocRetails + DocRetailID + "/",
                        method: 'DELETE',
                        success: function (result) {
                            loadingMask.hide();
                            var sData = Ext.decode(result.responseText);
                            if (sData.success == true) {
                                Ext.MessageBox.show({ title: lanOrgName, msg: sData.data.Msg, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.OK })
                                Ext.getCmp("grid_" + aButton.UO_id).view.store.load();
                            } else {
                                Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK })
                            }
                        },
                        failure: function (form, action) {
                            loadingMask.hide();
                            //Права.
                            /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                            Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                            funPanelSubmitFailure(form, action);
                        }
                    });

                }
            }
        });
    },

    //'viewDocRetails button#btnHelp': { click: this.onGrid_BtnHelp },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "retail-retail/", '_blank');
    },

    //'viewDocRetails #DateS': { select: this.onGrid_DateS },
    onDateSSelect: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocRetails);
    },

    //'viewDocRetails #DatePo': { select: this.onGrid_DatePo },
    onDatePoSelect: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocRetails);
    },

    // Клик по Гриду
    /*'viewDocRetails [itemId=grid]': {
        selectionchange: this.onGrid_selectionchange,
        itemclick: this.onGrid_itemclick,
        itemdblclick: this.onGrid_itemdblclick
    },*/
    onGridClick: function (model, records) {
        model.view.ownerGrid.down("#btnNewCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnEdit").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnDelete").setDisabled(records.length === 0);
    },
    onGridItemclick: function (view, record, item, index, eventObj) {
        //Если запись удалена, то выдать сообщение и выйти
        if (funGridRecordDel(record)) { return; }

        if (varSelectOneClick) {
            if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
                var Params = [
                    view.grid.id, //UO_idCall
                    false, //UO_Center
                    false, //UO_Modal
                    2     // 1 - Новое, 2 - Редактировать
                ]
                ObjectEditConfig("viewDocRetailsEdit", Params);
            }
            else {
                if (record.get("Held")) {
                    Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
                    Ext.getCmp(view.grid.UO_idMain).close();
                }
                else {
                    Ext.Msg.alert(lanOrgName, "Документ не проведён!");
                }
            }
        }
    },
    onGridItemdblclick: function (view, record, item, index, e) {
        if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
            var Params = [
                view.grid.id, //dv.grid.id, //UO_idCall
                false, //UO_Center
                false, //UO_Modal
                2     // 1 - Новое, 2 - Редактировать
            ]
            ObjectEditConfig("viewDocRetailsEdit", Params);
        }
        else {
            if (record.get("Held")) {
                Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
                Ext.getCmp(view.grid.UO_idMain).close();
            }
            else {
                Ext.Msg.alert(lanOrgName, "Документ не проведён!");
            }
        }
    },

    /*'viewDocRetails #TriggerSearchGrid': {
        "ontriggerclick": this.onTriggerSearchGridClick1,
        "specialkey": this.onTriggerSearchGridClick2,
        "change": this.onTriggerSearchGridClick3
    },*/
    onTriggerClick: function (aButton, aEvent) {
        funGridDoc(aButton.UO_id, HTTP_DocRetails);
    },
    onTriggerSpecialkey: function (f, e) {
        if (e.getKey() == e.ENTER) {
            funGridDoc(f.UO_id, HTTP_DocRetails);
        }
    },
    onTriggerChange: function (e, textReal, textLast) {
        if (textReal.length > 2) funGridDoc(e.UO_id, HTTP_DocRetails);
    },

});