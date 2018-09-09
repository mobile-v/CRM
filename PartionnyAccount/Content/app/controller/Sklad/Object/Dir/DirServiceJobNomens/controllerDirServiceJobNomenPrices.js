Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirServiceJobNomens/controllerDirServiceJobNomenPrices", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirServiceJobNomenPrices': { close: this.this_close },

            //Группа (itemId=tree)
            // Меню Группы
            'viewDirServiceJobNomenPrices [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDirServiceJobNomenPrices [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDirServiceJobNomenPrices [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDirServiceJobNomenPrices [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDirServiceJobNomenPrices [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDirServiceJobNomenPrices [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDirServiceJobNomenPrices [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDirServiceJobNomenPrices [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },

            'viewDirServiceJobNomenPrices dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },


            //Релоад
            'viewDirServiceJobNomenPrices button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
            //Поиск
            'viewDirServiceJobNomenPrices #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },


            // Клик по Гриду
            'viewDirServiceJobNomenPrices [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },

           
            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirServiceJobNomenPrices button#btnSelect': { "click": this.onBtnSelectClick }
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },

    //Кнопки редактирования Енеблед
    onGridParty_selectionchange: function (model, records) {
        //...
    },
    //Клик: Редактирования или выбор
    onGridParty_itemclick: function (view, record, item, index, eventObj) {
        //...
    },
    //ДаблКлик: Редактирования или выбор
    onGridParty_itemdblclick: function (view, record, item, index, e) {
        //...
    },

    //Обновить список Товаров
    onBtnDirNomenReloadClick: function (aButton, aEvent, aOptions) {
        //var storeDirNomensTree = Ext.getCmp(aButton.UO_idMain).storeDirNomensTree;
        var storeDirNomensTree = Ext.getCmp("tree_" + aButton.UO_id).store;
        storeDirNomensTree.load();
    },

    //Поиск
    //Поиск
    onTriggerSearchTreeClick1: function (aButton, aEvent) {
        fun_onTriggerSearchTreeClick_Search(aButton, true);
    },
    onTriggerSearchTreeClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            fun_onTriggerSearchTreeClick_Search(f, true);
        }
    },
    onTriggerSearchTreeClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) {

        }
    },



    // Группа (itemId=tree) === === === === === === === === === ===

    //Меню Группы
    onTree_expandAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).expandAll();
    },
    onTree_collapseAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).collapseAll();
    },

    onTree_folderNew: function (aButton, aEvent) {
        controllerDirServiceJobNomenPrices_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDirServiceJobNomenPrices_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDirServiceJobNomenPrices_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDirServiceJobNomenPrices_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDirServiceJobNomenPrices_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        var id = view.grid.UO_id;
        //Полный путь от Группы к выбранному объкту
        Ext.getCmp("DirServiceJobNomenPatchFull" + id).setValue(rec.get('DirServiceJobNomenPatchFull'));

        var storeGrid = Ext.getCmp("grid_" + id).getStore();

        //Внимание: выборка цен происходит НЕ из истории, а из самого справочника
        Ext.getCmp("tree_" + id).setDisabled(true);
        storeGrid.proxy.url = HTTP_DirServiceJobNomenHistories + "?DirServiceJobNomenID=" + rec.get('id'); storeGrid.setData([], false);
        storeGrid.load({ waitMsg: lanLoading });
        Ext.getCmp("tree_" + id).setDisabled(false);
    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
    },


    //beforedrop
    onTree_beforedrop: function (node, data, overModel, dropPosition, dropPosition1, dropPosition2, dropPosition3) {

        //Если это не узел, то выйти и сообщить об этом!
        if (overModel.data.leaf) { Ext.Msg.alert(lanOrgName, "В данную ветвь перемещать запрещено!"); return; }

        //Раскроем ветку с ID=1, перед перемещением
        var treePanel = Ext.getCmp("tree_" + data.view.panel.UO_id);
        var storeDirNomensTree = treePanel.getStore();
        var node = storeDirNomensTree.getNodeById(overModel.data.id);
        if (node != null) {
            storeDirNomensTree.UO_OnStop = false;

            //Раскрытие нужного нода
            treePanel.expandPath(node.getPath());

            if (node.firstChild == null) {
                //Событие на раскрытие - раскрылось
                storeDirNomensTree.on('load', function () {

                    if (storeDirNomensTree.UO_OnStop) { return; }
                    else { storeDirNomensTree.UO_OnStop = true; }

                    //Запрос на сервер - !!! ДВАЖДЫ ПОВТОРЯЕТСЯ !!! №1
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_DirNomens + "?id=" + data.records[0].data.id + "&sub=" + overModel.data.id,
                        method: 'PUT',
                        success: function (result) {
                            var sData = Ext.decode(result.responseText);
                            if (sData.success == true) {
                                
                            } else {
                                Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                                Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                            }
                        },
                        failure: function (form, action) {
                            //Права.
                            /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                            Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                            Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                            funPanelSubmitFailure(form, action);
                        }
                    });

                });
            }
            else {
                //Запрос на сервер - !!! ДВАЖДЫ ПОВТОРЯЕТСЯ !!! №2
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirNomens + "?id=" + data.records[0].data.id + "&sub=" + overModel.data.id,
                    method: 'PUT',
                    success: function (result) {
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            
                        } else {
                            Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                            Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                        }
                    },
                    failure: function (form, action) {
                        //Права.
                        /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                        Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                        Ext.getCmp("tree_" + data.view.panel.UO_id).view.store.load();
                        funPanelSubmitFailure(form, action);
                    }
                });
            }
        }

    },
    //drop
    onTree_drop: function (node, data, overModel, dropPosition) {
        //Ext.Msg.alert("Группа перемещена!");
    },


    /*onTree_contextMenuForTreePanel: function (view, rec, node, index, e) {
        alert("222222");
    },*/



    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        /*
        model.view.ownerGrid.down("#btnNewCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnEdit").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnDelete").setDisabled(records.length === 0);
        */
    },
    //Клик по Гриду
    onGrid_itemclick: function (view, record, item, index, e) {
        //Если запись удалена, то выдать сообщение и выйти
        if (funGridRecordDel(record)) { return; }

        if (varSelectOneClick) {
            if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
                alert("Error, no has find function - UO_Function_Grid");
            }
            else {
                //record.data.DirEmployeeName = lanDirEmployeeName;
                Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, view.grid.UO_id, record);
                Ext.getCmp(view.grid.UO_idMain).close();
            }
        }
    },
    //ДаблКлик по Гриду
    onGrid_itemdblclick: function (view, record, item, index, e) {
        if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
            alert("Error, no has find function - UO_Function_Grid");
        }
        else {
            //record.data.DirEmployeeName = lanDirEmployeeName;
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, view.grid.UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }
    },




    // === Кнопки === === ===

    onBtnSelectClick: function (aButton, aEvent, aOptions) {
        if (Ext.getCmp(aButton.UO_idMain).UO_Function_Grid == undefined) {
            alert("Error, no has find function - UO_Function_Grid");
        }
        else {
            var record = Ext.getCmp("grid_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
            if (record == undefined) { Ext.Msg.alert(lanOrgName, "Не выбрана запись в таблице цен!"); return; }

            Ext.getCmp(aButton.UO_idMain).UO_Function_Grid(Ext.getCmp(aButton.UO_idCall).UO_id, aButton.UO_id, record);
            Ext.getCmp(aButton.UO_idMain).close();
        }
    },

});



// === Функции === === ===

// === Функции === === ===
//1. Для Товара - КонтекстМеню
function controllerDirServiceJobNomenPrices_onTree_folderNew(id) {
    var Params = [
        "tree_" + id,
        true, //UO_Center
        true, //UO_Modal
        1,    // 1 - Новое, 2 - Редактировать
        false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
        undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
        undefined, // Для загрузки данных в форму Б.С. и Договора,
        undefined,
        undefined,
        undefined,
        Ext.getCmp("DirServiceJobNomenType" + id).getValue(),
        undefined,
        undefined,
        false      //GridTree
    ]
    ObjectEditConfig("viewDirServiceJobNomensWinEdit", Params);
};
function controllerDirServiceJobNomenPrices_onTree_folderEdit(id) {
    var Params = [
        "tree_" + id,
        true, //UO_Center
        true, //UO_Modal
        2,    // 1 - Новое, 2 - Редактировать
        false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
        undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
        undefined, // Для загрузки данных в форму Б.С. и Договора,
        undefined,
        undefined,
        undefined,
        Ext.getCmp("DirServiceJobNomenType" + id).getValue(),
        undefined,
        undefined,
        false      //GridTree
    ]
    ObjectEditConfig("viewDirServiceJobNomensWinEdit", Params);
};
function controllerDirServiceJobNomenPrices_onTree_folderNewSub(id) {
    var node = funReturnNode(id);
    //Ext.getCmp("Sub" + id).setValue(node.data.id);

    var Sub = node.data.id;

    var Params = [
        "tree_" + id,
        true,  //UO_Center
        true,  //UO_Modal
        1,     // 1 - Новое, 2 - Редактировать
        false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
        Sub,   //Sub,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
        undefined, // Для загрузки данных в форму Б.С. и Договора,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        false      //GridTree
    ]
    ObjectEditConfig("viewDirServiceJobNomensWinEdit", Params);
};
function controllerDirServiceJobNomenPrices_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {

                var Params = [
                    "tree_" + id,
                    true, //UO_Center
                    true, //UO_Modal
                    3,    // 1 - Новое, 2 - Редактировать
                    false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                    undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    undefined, // Для загрузки данных в форму Б.С. и Договора,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    false      //GridTree
                ]
                ObjectEditConfig("viewDirServiceJobNomensWinEdit", Params);

            }
        }
    });
};
function controllerDirServiceJobNomenPrices_onTree_folderDel(id) {
    //Формируем сообщение: Удалить или снять пометку на удаление
    var sMsg = lanDelete;
    if (Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;

    //Процес Удаление или снятия пометки
    Ext.MessageBox.show({
        title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                //Лоадер
                var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("tree_" + id) });
                loadingMask.show();
                //Выбранный ID-шник Грида
                var DirServiceJobNomenID = Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.id; //Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.DirNomenID;
                //Запрос на удаление
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirServiceJobNomens + DirServiceJobNomenID + "/",
                    method: 'DELETE',
                    success: function (result) {
                        loadingMask.hide();
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            //Очистить форму
                            Ext.getCmp("DirServiceJobNomenPatchFull" + id).setValue("");
                            if (Ext.getCmp("form_" + id) != undefined) { Ext.getCmp("form_" + id).reset(true); }
                            //Открыть ветки
                            fun_ReopenTree_1(id, undefined, "tree_" + id, sData.data);
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
};
function controllerDirServiceJobNomenPrices_onTree_folderSubNull(id) {
    //Если это не узел, то выйти и сообщить об этом!
    /*if (!Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.leaf) {
        Ext.Msg.alert(lanOrgName, "Данная ветвь уже корневая!");
        return;
    }*/


    //Формируем сообщение: Удалить или снять пометку на удаление
    var sMsg = "Сделать корневой";
    if (Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;


    Ext.MessageBox.show({
        title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {

                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirNomens + "?id=" + Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.id + "&sub=0", // + overModel.data.id,
                    method: 'PUT',
                    success: function (result) {
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            //Ext.MessageBox.show({ title: lanOrgName, msg: sData.data.Msg, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.OK })
                            Ext.getCmp("tree_" + id).view.store.load();
                        } else {
                            Ext.getCmp("tree_" + id).view.store.load();
                            Ext.MessageBox.show({ title: lanOrgName, msg: sData.data, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                        }
                    },
                    failure: function (form, action) {
                        //Права.
                        /*if (action.result.data.msgType == "1") { Ext.Msg.alert(lanOrgName, action.result.data.msg); return; }
                        Ext.Msg.alert(lanOrgName, txtMsg008 + action.result.data);*/
                        Ext.getCmp("tree_" + id).view.store.load();
                        funPanelSubmitFailure(form, action);
                    }
                });

            }
        }
    });

};
function controllerDirServiceJobNomenPrices_onTree_addSub(id) {

    //Если форма ещё не загружена - выйти!
    var widgetXForm = Ext.getCmp("form_" + id);
    if (!widgetXForm.UO_Loaded) return;

    var node = funReturnNode(id);
    if (node != undefined) {
        node.data.leaf = false;
        Ext.getCmp("tree_" + id).getView().refresh();
        node.expand();
    }

};