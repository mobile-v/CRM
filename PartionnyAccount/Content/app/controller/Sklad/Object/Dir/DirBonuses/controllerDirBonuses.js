﻿Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirBonuses/controllerDirBonuses", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены другие виджеты ...)
            //Закрыте
            'viewDirBonuses': { close: this.this_close },

            //Группа (itemId=tree)
            // Меню Группы
            'viewDirBonuses [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDirBonuses [itemId=FolderEdit]': { click: this.onTree_folderEdit },
            'viewDirBonuses [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDirBonuses [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDirBonuses [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },


            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirBonuses button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirBonuses button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirBonuses button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Меню Группы
    onTree_folderNew: function (aButton, aEvent) {
        controllerDirBonuses_onTree_folderNew(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDirBonuses_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDirBonuses_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        //model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);

        //Ext.getCmp("btnHistory" + model.view.ownerGrid.UO_id).setDisabled(records.length === 0);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {

        //Если запись помечена на удаление, то сообщить об этом и выйти
        if (rec.Del == true) {
            Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
            return;
        };

        //3. Получаем форму редактирования
        var widgetXForm = Ext.getCmp("form_" + view.grid.UO_id); //var widgetX = Ext.getCmp("viewDirBonuses" + view.grid.UO_id); var widgetXForm = widgetX.down('form');
        widgetXForm.UO_Loaded = false;

        var storeDirBonusTabsGrid = Ext.getCmp("PanelGridBonusTab_" + view.grid.UO_id).getStore();
        //var storeDirBonusTabsGrid = Ext.create("store.storeDirBonusTabsGrid"); 
        storeDirBonusTabsGrid.setData([], false);
        Ext.getCmp("PanelGridBonusTab_" + view.grid.UO_id).store = storeDirBonusTabsGrid;
        storeDirBonusTabsGrid.proxy.url = HTTP_DirBonusTabs + "/" + rec.get('id') + "/";
        //storeDirBonusTabsGrid.load({ waitMsg: lanLoading });

        widgetXForm.load({
            method: "GET",
            timeout: varTimeOutDefault,
            waitMsg: lanLoading,
            url: HTTP_DirBonuses + rec.get('id') + "/",
            success: function (form, action) {
                widgetXForm.UO_Loaded = true;
                storeDirBonusTabsGrid.load({ waitMsg: lanLoading });
            },
            failure: function (form, action) {
                funPanelSubmitFailure(form, action);
            }
        });
    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
    },



    // === Кнопки === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        //Градация
        var recordsDirBonusTabsGrid = [];
        var storeDirBonusTabsGrid = Ext.getCmp("PanelGridBonusTab_" + aButton.UO_id).store;
        storeDirBonusTabsGrid.data.each(function (rec) { recordsDirBonusTabsGrid.push(rec.data); });
        if (storeDirBonusTabsGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Пожалусте, заполните градацию скидок!"); return; }

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirBonuses;
        if (parseInt(Ext.getCmp("DirBonusID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirBonuses + "?id=" + parseInt(Ext.getCmp("DirBonusID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            params: { recordsDirBonusTabsGrid: Ext.encode(recordsDirBonusTabsGrid) },

            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                Ext.getCmp("tree_" + aButton.UO_id).getStore().load();
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });

        Ext.getCmp("tree_" + aButton.UO_id).enable();
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("tree_" + aButton.UO_id).enable();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-bonus/", '_blank');
    },

});



// === Функции === === ===

function controllerDirBonuses_onTree_folderNew(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("tree_" + id).disable();
};
function controllerDirBonuses_onTree_folderNewSub(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("AddSub" + id).setValue(true);
    var node = funReturnNode(id);
    Ext.getCmp("Sub" + id).setValue(node.data.id);

    Ext.getCmp("tree_" + id).disable();
};
function controllerDirBonuses_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                Ext.getCmp("DirBonusID" + id).setValue(0);
                Ext.getCmp("DirBonusName" + id).setValue(Ext.getCmp("DirBonusName" + id).getValue() + " (копия)");

                Ext.getCmp("tree_" + id).disable();
            }
        }
    });
};
function controllerDirBonuses_onTree_folderDel(id) {
    //Формируем сообщение: Удалить или снять пометку на удаление
    var sMsg = lanDelete;
    if (Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;

    //Процес Удаление или снятия пометки
    Ext.MessageBox.show({
        title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                //Лоадер
                var loadingMask = new Ext.LoadMask({
                    msg: 'Please wait...',
                    target: Ext.getCmp("tree_" + id)
                });
                loadingMask.show();
                //Выбранный ID-шник Грида
                var DirBonusID = Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.id; //Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.DirBonusID;
                //Запрос на удаление
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirBonuses + DirBonusID + "/",
                    method: 'DELETE',
                    success: function (result) {
                        loadingMask.hide();
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            Ext.MessageBox.show({ title: lanOrgName, msg: sData.data.Msg, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.OK })
                            Ext.getCmp("tree_" + id).view.store.load();

                            //Чистим форму редактирования *** ***
                            //форма
                            Ext.getCmp("form_" + id).reset();
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
function controllerDirBonuses_onTree_folderSubNull(id) {
    return;
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
                    url: HTTP_DirBonuses + "?id=" + Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.id + "&sub=0", // + overModel.data.id,
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