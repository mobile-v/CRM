Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandInventories", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocSecondHandInventories': { close: this.this_close },

            //Группа (itemId=tree)
            // Клик по Группе
            'viewDocSecondHandInventories #SpisatS': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },

            

            //Грид (itemId=grid)
            'viewDocSecondHandInventories button#btnNew': { click: this.onGrid_BtnNew },
            'viewDocSecondHandInventories button#btnNewCopy': { click: this.Grid_BtnNewCopy },
            'viewDocSecondHandInventories button#btnEdit': { click: this.onGrid_BtnEdit },
            'viewDocSecondHandInventories button#btnDelete': { click: this.onGrid_BtnDelete },
            'viewDocSecondHandInventories button#btnHelp': { click: this.onGrid_BtnHelp },
            'viewDocSecondHandInventories button#btnImport': { click: this.onGrid_BtnImport },
            'viewDocSecondHandInventories #DateS': { select: this.onGrid_DateS },
            'viewDocSecondHandInventories #DatePo': { select: this.onGrid_DatePo },
            // Клик по Гриду
            'viewDocSecondHandInventories [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },
            'viewDocSecondHandInventories #TriggerSearchGrid': {
                "ontriggerclick": this.onTriggerSearchGridClick1,
                "specialkey": this.onTriggerSearchGridClick2,
                "change": this.onTriggerSearchGridClick3
            },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Группа (itemId=tree) === === === === ===

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        //model.view.ownerGrid.down("#FolderEdit").setDisabled(records.length === 0);
        //model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        funGridDoc(view.grid.UO_id, HTTP_DocSecondHandInventories, rec.get('id'));
    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
    },



    //Грид (itemId=grid) === === === === ===

    onGrid_BtnNew: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            1     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocSecondHandInventoriesEdit", Params);
    },

    Grid_BtnNewCopy: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            3     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocSecondHandInventoriesEdit", Params);
    },

    onGrid_BtnEdit: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            false, //UO_Center
            false, //UO_Modal
            2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectEditConfig("viewDocSecondHandInventoriesEdit", Params);
    },

    onGrid_BtnDelete: function (aButton, aEvent, aOptions) {

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
                    var DocSecondHandInventoryID = Ext.getCmp("grid_" + aButton.UO_id).view.getSelectionModel().getSelection()[0].data.DocSecondHandInventoryID;
                    //Запрос на удаление
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_DocSecondHandInventories + DocSecondHandInventoryID + "/",
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

    onGrid_BtnHelp: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-second-hand-inventories/", '_blank');
    },

    onGrid_BtnImport: function (aButton, aEvent, aOptions) {
        Ext.Msg.alert("", "Откроется форма Импорта приходной накладной!");
    },

    onGrid_DateS: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocSecondHandInventories);
    },

    onGrid_DatePo: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocSecondHandInventories);
    },

    //Поиск
    onTriggerSearchGridClick1: function (aButton, aEvent) {
        funGridDoc(aButton.UO_id, HTTP_DocSecondHandInventories); //, Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).value, HTTP_DocSecondHandInventories
    },
    onTriggerSearchGridClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            funGridDoc(f.UO_id, HTTP_DocSecondHandInventories); //, f.value, HTTP_DocSecondHandInventories
        }
    },
    onTriggerSearchGridClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) funGridDoc(e.UO_id, HTTP_DocSecondHandInventories); //, textReal, HTTP_DocSecondHandInventories
    },


    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#btnNewCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnEdit").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnDelete").setDisabled(records.length === 0);
    },
    //Клик: Редактирования или выбор
    onGrid_itemclick: function (view, record, item, index, eventObj) {
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
                ObjectEditConfig("viewDocSecondHandInventoriesEdit", Params);
            }
            else {
                Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
                Ext.getCmp(view.grid.UO_idMain).close();
            }
        }
    },
    //ДаблКлик: Редактирования или выбор
    onGrid_itemdblclick: function (view, record, item, index, e) {
        if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
            var Params = [
                view.grid.id, //dv.grid.id, //UO_idCall
                false, //UO_Center
                false, //UO_Modal
                2     // 1 - Новое, 2 - Редактировать
            ]
            ObjectEditConfig("viewDocSecondHandInventoriesEdit", Params);
        }
        else {
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }
    },

});


