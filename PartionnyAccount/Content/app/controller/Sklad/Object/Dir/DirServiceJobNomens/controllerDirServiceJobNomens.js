Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirServiceJobNomens/controllerDirServiceJobNomens", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirServiceJobNomens': { close: this.this_close },

            //Группа (itemId=tree)
            // Меню Группы
            'viewDirServiceJobNomens [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDirServiceJobNomens [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDirServiceJobNomens [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDirServiceJobNomens [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDirServiceJobNomens [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDirServiceJobNomens [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDirServiceJobNomens [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDirServiceJobNomens [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },

            'viewDirServiceJobNomens dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },


            //Релоад
            'viewDirServiceJobNomens button#btnDirServiceJobNomenReload': { click: this.onBtnDirServiceJobNomenReloadClick },
            //Поиск
            'viewDirServiceJobNomens #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },
            

            //Фотка
            'viewDirServiceJobNomens [itemId=ImageLink]': { "change": this.onPanelGeneral_ImageLink },


            //Категория товара
            'viewDirServiceJobNomens button#btnDirServiceJobNomenCategoryEdit': { click: this.onBtnDirServiceJobNomenCategoryEditClick },
            'viewDirServiceJobNomens button#btnDirServiceJobNomenCategoryReload': { click: this.onBtnDirServiceJobNomenCategoryReloadClick },
            'viewDirServiceJobNomens button#btnDirServiceJobNomenCategoryClear': { "click": this.onBtnDirServiceJobNomenCategoryClearClick },



           
            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirServiceJobNomens button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirServiceJobNomens button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirServiceJobNomens button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Группа (itemId=tree) === === === === ===

    //Меню Группы
    //Меню Группы
    onTree_expandAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).expandAll();


        //Раскрыть нот с ID=1
        /*var treePanel = Ext.getCmp("tree_" + aButton.UO_id);
        var storeNomenTree = treePanel.getStore();
        var node = storeNomenTree.getNodeById(1);
        if (node != null) treePanel.expandPath(node.getPath());*/
    },
    onTree_collapseAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).collapseAll();
    },

    onTree_folderNew: function (aButton, aEvent) {
        controllerDirServiceJobNomens_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDirServiceJobNomens_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDirServiceJobNomens_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDirServiceJobNomens_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDirServiceJobNomens_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        //Если запись помечена на удаление, то сообщить об этом и выйти
        if (rec.Del == true) {
            Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
            return;
        };

        //Получаем форму редактирования
        var widgetXForm = Ext.getCmp("form_" + view.grid.UO_id);
        widgetXForm.UO_Loaded = false;


        widgetXForm.load({
            method: "GET",
            timeout: varTimeOutDefault,
            waitMsg: lanLoading,
            url: HTTP_DirServiceJobNomens + rec.get('id') + "/",
            success: function (form, action) {
                widgetXForm.UO_Loaded = true;

                //Полный путь от Группы к выбранному объкту
                Ext.getCmp("DirServiceJobNomenPatchFull" + view.grid.UO_id).setValue(rec.get('DirServiceJobNomenPatchFull'));
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



    //beforedrop
    onTree_beforedrop: function (node, data, overModel, dropPosition, dropPosition1, dropPosition2, dropPosition3) {

        //Если это не узел, то выйти и сообщить об этом!
        if (overModel.data.leaf) { Ext.Msg.alert(lanOrgName, "В данную ветвь перемещать запрещено!"); return; }

        //Раскроем ветку с ID=1, перед перемещением
        var treePanel = Ext.getCmp("tree_" + data.view.panel.UO_id);
        var storeDirServiceJobNomensTree = treePanel.getStore();
        var node = storeDirServiceJobNomensTree.getNodeById(overModel.data.id);
        if (node != null) {
            storeDirServiceJobNomensTree.UO_OnStop = false;

            //Раскрытие нужного нода
            treePanel.expandPath(node.getPath());

            if (node.firstChild == null) {
                //Событие на раскрытие - раскрылось
                storeDirServiceJobNomensTree.on('load', function () {

                    if (storeDirServiceJobNomensTree.UO_OnStop) { return; }
                    else { storeDirServiceJobNomensTree.UO_OnStop = true; }

                    //Запрос на сервер - !!! ДВАЖДЫ ПОВТОРЯЕТСЯ !!! №1
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_DirServiceJobNomens + "?id=" + data.records[0].data.id + "&sub=" + overModel.data.id,
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
                    url: HTTP_DirServiceJobNomens + "?id=" + data.records[0].data.id + "&sub=" + overModel.data.id,
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


    //Обновить список Товаров
    onBtnDirServiceJobNomenReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirServiceJobNomensTree = Ext.getCmp("tree_" + aButton.UO_id).store; storeDirServiceJobNomensTree.setData([], false); storeDirServiceJobNomensTree.UO_OnStop = true;
        storeDirServiceJobNomensTree.load();
    },

    //Поиск
    onTriggerSearchTreeClick1: function (aButton, aEvent) {
        controllerDirServiceJobNomens_onTriggerSearchTreeClick_Search(aButton, true);
    },
    onTriggerSearchTreeClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            controllerDirServiceJobNomens_onTriggerSearchTreeClick_Search(f, true);
        }
    },
    onTriggerSearchTreeClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) {

        }
    },


    // *** Фотка ***
    onPanelGeneral_ImageLink: function (aTextfield, aValueReal, aValuePrevious) {
        try {
            Ext.getCmp("imageShow" + aTextfield.UO_id).setSrc(aValueReal);
        } catch (ex) { alert(e.name); }

    },



    // *** DirServiceJobNomenCategoryName ***
    //Редактирование или добавление нового Поставщика
    onBtnDirServiceJobNomenCategoryEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirServiceJobNomenCategories", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirServiceJobNomenCategoryReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirServiceJobNomenCategoriesGrid = Ext.getCmp(aButton.UO_idMain).storeDirServiceJobNomenCategoriesGrid;
        storeDirServiceJobNomenCategoriesGrid.load();
    },
    //Очистить Скидку
    onBtnDirServiceJobNomenCategoryClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirServiceJobNomenCategoryID" + aButton.UO_id).setValue(null);
    },




    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //!!! Нет таких комбоБоксов на форме !!!
        //Может быть так, что пользователь ввёл текст в КомбоБокс, которого нет в списке, тогда надо очистить Комбо!
        /*
        var combobox = Ext.getCmp("DirServiceJobNomenCategoryID" + aButton.UO_id);
        var v = combobox.getValue();
        var record = combobox.findRecord(combobox.valueField || combobox.displayField, v);
        var index = combobox.store.indexOf(record);
        if (index < 0) { combobox.setValue(null); }
        */
        //fun_ComboBox_Search_Correct("DirServiceJobNomenCategoryID" + aButton.UO_id);


        //Цены
        Ext.getCmp("PriceWholesaleVAT" + aButton.UO_id).setValue(Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue());
        Ext.getCmp("PriceIMVAT" + aButton.UO_id).setValue(Ext.getCmp("PriceRetailVAT" + aButton.UO_id).getValue());

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirServiceJobNomens;
        if (parseInt(Ext.getCmp("DirServiceJobNomenID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirServiceJobNomens + "?id=" + parseInt(Ext.getCmp("DirServiceJobNomenID" + aButton.UO_id).value);
        }

        //Сохранение
        widgetXForm.submit({
            method: sMethod,
            url: sUrl,
            timeout: varTimeOutDefault,
            waitMsg: lanUploading,
            success: function (form, action) {
                //Ext.getCmp("tree_" + aButton.UO_id).getStore().load();

                fun_ReopenTree_1(aButton.UO_id, undefined, "tree_" + aButton.UO_id, action.result.data);
            },
            failure: function (form, action) { funPanelSubmitFailure(form, action); }
        });

        Ext.getCmp("tree_" + aButton.UO_id).enable();
        Ext.getCmp("DirServiceJobNomenPatchFull" + aButton.UO_id).enable();
        Ext.getCmp("btnDirServiceJobNomenReload" + aButton.UO_id).enable();
        Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable();
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("tree_" + aButton.UO_id).enable();
        Ext.getCmp("DirServiceJobNomenPatchFull" + aButton.UO_id).enable();
        Ext.getCmp("btnDirServiceJobNomenReload" + aButton.UO_id).enable();
        Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-tovar/", '_blank');
    }
});



// === Функции === === ===

function controllerDirServiceJobNomens_onTree_folderNew(id) {
    Ext.getCmp("form_" + id).reset(true); //var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("tree_" + id).disable();
    Ext.getCmp("DirServiceJobNomenPatchFull" + id).disable();
    Ext.getCmp("btnDirServiceJobNomenReload" + id).disable();
    Ext.getCmp("TriggerSearchTree" + id).disable();
    //По умолчанию
    Ext.getCmp("DirCurrencyID" + id).setValue(varDirCurrencyID);
    Ext.getCmp("DirServiceJobNomenType" + id).setValue(Ext.getCmp("DirServiceJobNomenType_" + id).getValue());
};
function controllerDirServiceJobNomens_onTree_folderEdit(id) {
    Ext.getCmp("tree_" + id).disable();
    Ext.getCmp("DirServiceJobNomenPatchFull" + id).disable();
    Ext.getCmp("btnDirServiceJobNomenReload" + id).disable();
    Ext.getCmp("TriggerSearchTree" + id).disable();
};
function controllerDirServiceJobNomens_onTree_folderNewSub(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);

    var node = funReturnNode(id);
    Ext.getCmp("Sub" + id).setValue(node.data.id);

    Ext.getCmp("tree_" + id).disable();
    Ext.getCmp("DirServiceJobNomenPatchFull" + id).disable();
    Ext.getCmp("btnDirServiceJobNomenReload" + id).disable();
    Ext.getCmp("TriggerSearchTree" + id).disable();
    //По умолчанию
    Ext.getCmp("DirCurrencyID" + id).setValue(varDirCurrencyID);
    Ext.getCmp("DirServiceJobNomenType" + id).setValue(Ext.getCmp("DirServiceJobNomenType_" + id).getValue());
};
function controllerDirServiceJobNomens_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                Ext.getCmp("DirServiceJobNomenID" + id).setValue(0);
                Ext.getCmp("DirServiceJobNomenName" + id).setValue(Ext.getCmp("DirServiceJobNomenName" + id).getValue() + " (копия)");

                Ext.getCmp("tree_" + id).disable();
                Ext.getCmp("DirServiceJobNomenPatchFull" + id).disable();
                Ext.getCmp("btnDirServiceJobNomenReload" + id).disable();
                Ext.getCmp("TriggerSearchTree" + id).disable();
            }
        }
    });
};
function controllerDirServiceJobNomens_onTree_folderDel(id) {
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
                var DirServiceJobNomenID = Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.id; //Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.DirServiceJobNomenID;
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
                            Ext.getCmp("form_" + id).reset(true);
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
function controllerDirServiceJobNomens_onTree_folderSubNull(id) {
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
                    url: HTTP_DirServiceJobNomens + "?id=" + Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.id + "&sub=0", // + overModel.data.id,
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
function controllerDirServiceJobNomens_onTree_addSub(id) {

    //Если форма ещё не загружена - выйти!
    var widgetXForm = Ext.getCmp("form_" + id);
    if (!widgetXForm.UO_Loaded) return;

    var node = funReturnNode(id);
    if (node != undefined) {
        //val==true - checked, val==false - No checked
        /*if (val) {
            node.data.leaf = false;
            Ext.getCmp("tree_" + id).getView().refresh();
            node.expand();
        }
        else {
            node.data.leaf = true;
            Ext.getCmp("tree_" + id).getView().refresh();
        }*/

        node.data.leaf = false;
        Ext.getCmp("tree_" + id).getView().refresh();
        node.expand();
    }

};
//2. Поиск по Ш-к
function controllerDirServiceJobNomens_onTriggerSearchTreeClick_Search(aButton, bReset) {
    if (Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue() == "") return;
    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).disable(); //Кнопку поиска делаем не активной


    if (Ext.getCmp("SearchType" + aButton.UO_id).getValue() == 1) {

        var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("tree_" + aButton.UO_id) });
        loadingMask.show();

        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            //                        id,                                                iPriznak
            url: HTTP_DirServiceJobNomens + Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value + "/1/",
            method: 'GET',
            success: function (result) {
                loadingMask.hide();
                var sData = Ext.decode(result.responseText);
                if (sData.success == true) {
                    var sData = Ext.decode(result.responseText);

                    if (sData.data == -1) {
                        Ext.Msg.alert(lanOrgName, "Ничего не найдено!");
                        return;
                    }

                    fun_ReopenTree_1(aButton.UO_id, undefined, "tree_" + aButton.UO_id, sData.data);

                    if (bReset) {
                        //Чистим форму - это Обязательно! Но оставляем строку поиска
                        var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue();
                        //Очистить форму
                        Ext.getCmp("form_" + aButton.UO_id).reset();
                        //Поиск позвращаем полюбому
                        Ext.getCmp("TriggerSearchTree" + aButton.UO_id).setValue(TriggerSearchTree);
                    }
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
    else {

        //url: HTTP_RemParties + Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value + "/1/";

        //Если панель скрыта, то показывать "партии товара"
        if (Ext.getCmp("gridParty_" + aButton.UO_id).collapsed) {
            Ext.getCmp("gridParty_" + ObjectID).expand(Ext.Component.DIRECTION_NORTH, true);
        }

        //Получаем storeRemPartiesGrid и делаем load()
        var storeRemPartiesGrid = Ext.getCmp("gridParty_" + aButton.UO_id).getStore();
        storeRemPartiesGrid.proxy.url = HTTP_RemParties + "?parSearch=" + Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value + "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + aButton.UO_id).getValue();;
        storeRemPartiesGrid.load();

        storeRemPartiesGrid.on('load', function () {

        });

    }

    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); //Кнопку поиска делаем активной
}
