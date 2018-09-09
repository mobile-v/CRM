Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirNomens/controllerDirNomens", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirNomens': { close: this.this_close },

            //Группа (itemId=tree)
            // Меню Группы
            'viewDirNomens [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDirNomens [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDirNomens [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDirNomens [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDirNomens [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDirNomens [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDirNomens [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDirNomens [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },

            'viewDirNomens dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },


            //Релоад
            'viewDirNomens button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
            //Поиск
            'viewDirNomens #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },
            

            //Фотка
            'viewDirNomens [itemId=ImageLink]': { "change": this.onPanelGeneral_ImageLink },


            //Категория товара
            'viewDirNomens [itemId=DirNomenCategoryID]': { select: this.onDirNomenCategoryIDSelect },
            'viewDirNomens button#btnDirNomenCategoryEdit': { click: this.onBtnDirNomenCategoryEditClick },
            'viewDirNomens button#btnDirNomenCategoryReload': { click: this.onBtnDirNomenCategoryReloadClick },
            'viewDirNomens button#btnDirNomenCategoryClear': { "click": this.onBtnDirNomenCategoryClearClick },


            //Не работает.
            //Работает в: PartionnyAccount.viewcontroller.Sklad/Object/Dir/DirNomens/viewcontrollerDirNomens
            //Клик на изображение
            /*
            onImageShowClick: function (aButton, aEvent, param1) {
                var Params = [
                    "imageShow" + Ext.getCmp(aButton.target.id).UO_id, //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    1     // 1 - Новое, 2 - Редактировать
                ]
                ObjectEditConfig("viewDirNomensImg", Params);
            },
            */


            //Информация *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
            //Обновить
            'viewDirNomens button#btnGridsRefresh': { click: this.onBtnGridsRefreshClick },

            // Клик по Гриду
            'viewDirNomens [itemId=PanelRemParties]': {
                selectionchange: this.onRemPartiesGrid_selectionchange,
                itemclick: this.onRemPartiesGrid_itemclick,
                itemdblclick: this.onRemPartiesGrid_itemdblclick
            },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirNomens button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirNomens button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirNomens button#btnHelp': { "click": this.onBtnHelpClick },
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
        controllerDirNomens_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDirNomens_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDirNomens_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDirNomens_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDirNomens_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);

        Ext.getCmp("btnHistory" + model.view.ownerGrid.UO_id).setDisabled(records.length === 0);

        Ext.getCmp("panelInfo_" + model.view.ownerGrid.UO_id).setDisabled(records.length === 0);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {

        //Чистка вкладки Информация
        Ext.getCmp(view.grid.UO_idMain).storeDirNomenHistoriesGrid.setData([], false);
        Ext.getCmp(view.grid.UO_idMain).storeRemPartiesGrid.setData([], false);
        Ext.getCmp(view.grid.UO_idMain).storeRemPartyMinusesGrid.setData([], false);

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
            url: HTTP_DirNomens + rec.get('id') + "/",
            success: function (form, action) {
                widgetXForm.UO_Loaded = true;

                //Полный путь от Группы к выбранному объкту
                Ext.getCmp("DirNomenPatchFull" + view.grid.UO_id).setValue(rec.get('DirNomenPatchFull'));

                //Image
                //0.
                Ext.getCmp("SysGenID" + view.grid.UO_id).setValue(action.result.data.SysGenID);
                Ext.getCmp("SysGenIDPatch" + view.grid.UO_id).setValue(action.result.data.SysGenIDPatch);
                if (action.result.data.SysGenIDPatch != "") { Ext.getCmp("imageShow" + view.grid.UO_id).setSrc(action.result.data.SysGenIDPatch); }
                else { Ext.getCmp("imageShow" + view.grid.UO_id).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                //1.
                Ext.getCmp("SysGen1ID" + view.grid.UO_id).setValue(action.result.data.SysGen1ID);
                Ext.getCmp("SysGen1IDPatch" + view.grid.UO_id).setValue(action.result.data.SysGen1IDPatch);
                if (action.result.data.SysGen1IDPatch != "") { Ext.getCmp("image1Show" + view.grid.UO_id).setSrc(action.result.data.SysGen1IDPatch); }
                else { Ext.getCmp("image1Show" + view.grid.UO_id).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                //2.
                Ext.getCmp("SysGen2ID" + view.grid.UO_id).setValue(action.result.data.SysGen2ID);
                Ext.getCmp("SysGen2IDPatch" + view.grid.UO_id).setValue(action.result.data.SysGen2IDPatch);
                if (action.result.data.SysGen2IDPatch != "") { Ext.getCmp("image2Show" + view.grid.UO_id).setSrc(action.result.data.SysGen2IDPatch); }
                else { Ext.getCmp("image2Show" + view.grid.UO_id).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                //3.
                Ext.getCmp("SysGen3ID" + view.grid.UO_id).setValue(action.result.data.SysGen3ID);
                Ext.getCmp("SysGen3IDPatch" + view.grid.UO_id).setValue(action.result.data.SysGen3IDPatch);
                if (action.result.data.SysGen3IDPatch != "") { Ext.getCmp("image3Show" + view.grid.UO_id).setSrc(action.result.data.SysGen3IDPatch); }
                else { Ext.getCmp("image3Show" + view.grid.UO_id).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                //4.
                Ext.getCmp("SysGen4ID" + view.grid.UO_id).setValue(action.result.data.SysGen4ID);
                Ext.getCmp("SysGen4IDPatch" + view.grid.UO_id).setValue(action.result.data.SysGen4IDPatch);
                if (action.result.data.SysGen4IDPatch != "") { Ext.getCmp("image4Show" + view.grid.UO_id).setSrc(action.result.data.SysGen4IDPatch); }
                else { Ext.getCmp("image4Show" + view.grid.UO_id).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }
                //5.
                Ext.getCmp("SysGen5ID" + view.grid.UO_id).setValue(action.result.data.SysGen5ID);
                Ext.getCmp("SysGen5IDPatch" + view.grid.UO_id).setValue(action.result.data.SysGen5IDPatch);
                if (action.result.data.SysGen5IDPatch != "") { Ext.getCmp("image5Show" + view.grid.UO_id).setSrc(action.result.data.SysGen5IDPatch); }
                else { Ext.getCmp("image5Show" + view.grid.UO_id).setSrc("../../Scripts/sklad/images/ru_default_no_foto.jpg"); }

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

        fun_Nods_Drop_Down(HTTP_DirNomens, node, data, overModel, dropPosition, dropPosition1, dropPosition2, dropPosition3);
        return;

    },
    //drop
    onTree_drop: function (node, data, overModel, dropPosition) {
        //Ext.Msg.alert("Группа перемещена!");
    },


    //Обновить список Товаров
    onBtnDirNomenReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirNomensTree = Ext.getCmp("tree_" + aButton.UO_id).store; storeDirNomensTree.setData([], false); storeDirNomensTree.UO_OnStop = true;
        storeDirNomensTree.load();
    },

    //Поиск
    onTriggerSearchTreeClick1: function (aButton, aEvent) {
        fun_onTriggerSearchTreeClick_Search(aButton, false);
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


    // *** Фотка ***
    onPanelGeneral_ImageLink: function (aTextfield, aValueReal, aValuePrevious) {
        try {
            Ext.getCmp("imageShow" + aTextfield.UO_id).setSrc(aValueReal);
        } catch (ex) { alert(e.name); }

    },



    // *** DirNomenCategoryName ***
    onDirNomenCategoryIDSelect: function (combo, records, eOpts) {
        controllerDirNomens_onDirNomenCategoryIDSelect(combo, records, eOpts)
    },
    //Редактирование или добавление новой Категории товара
    onBtnDirNomenCategoryEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirNomenCategories", Params);
    },
    //РеЛоад - перегрузить, что бы появились новые записи
    onBtnDirNomenCategoryReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirNomenCategoriesGrid = Ext.getCmp(aButton.UO_idMain).storeDirNomenCategoriesGrid;
        storeDirNomenCategoriesGrid.load();
    },
    //Очистить
    onBtnDirNomenCategoryClearClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirNomenCategoryID" + aButton.UO_id).setValue(null);
    },



    //Информация
    //Обновить
    onBtnGridsRefreshClick: function (aButton, aEvent, aOptions) {

        var storeDirNomenHistoriesGrid = Ext.getCmp(aButton.UO_idMain).storeDirNomenHistoriesGrid;
        storeDirNomenHistoriesGrid.proxy.url = HTTP_DirNomenHistories + "?type=Grid&DirNomenID=" + Ext.getCmp("DirNomenID" + aButton.UO_id).value;
        storeDirNomenHistoriesGrid.load();

        var storeRemPartiesGrid = Ext.getCmp(aButton.UO_idMain).storeRemPartiesGrid;
        storeRemPartiesGrid.proxy.url = HTTP_RemParties + "?type=Grid&DirNomenID=" + Ext.getCmp("DirNomenID" + aButton.UO_id).value;
        storeRemPartiesGrid.load();

    },

    //Кнопки редактирования Енеблед
    onRemPartiesGrid_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onRemPartiesGrid_itemclick: function (view, record, item, index, eventObj) {
        var storeRemPartyMinusesGrid = Ext.getCmp(view.grid.UO_idMain).storeRemPartyMinusesGrid;
        storeRemPartyMinusesGrid.proxy.url = HTTP_RemPartyMinuses + "?type=Grid&RemPartyID=" + record.data.RemPartyID;
        storeRemPartyMinusesGrid.load();
    },
    //ДаблКлик: Редактирования или выбор
    onRemPartiesGrid_itemdblclick: function (view, record, item, index, e) {
    },




    // Кнопки === === === === === === === === === === ===

    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //Может быть так, что пользователь ввёл текст в КомбоБокс, которого нет в списке, тогда надо очистить Комбо!
        fun_ComboBox_Search_Correct("DirNomenCategoryID" + aButton.UO_id);

        //Форма на Виджете
        var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

        //Новая или Редактирование
        var sMethod = "POST";
        var sUrl = HTTP_DirNomens;
        if (parseInt(Ext.getCmp("DirNomenID" + aButton.UO_id).value) > 0) {
            sMethod = "PUT";
            sUrl = HTTP_DirNomens + "?id=" + parseInt(Ext.getCmp("DirNomenID" + aButton.UO_id).value);
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
        Ext.getCmp("DirNomenPatchFull" + aButton.UO_id).enable();
        Ext.getCmp("btnDirNomenReload" + aButton.UO_id).enable();
        Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable();
    },
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        var id = aButton.UO_id;

        controllerDirNomens_onTree_folderNew
        Ext.getCmp("tree_" + id).enable();
        Ext.getCmp("DirNomenPatchFull" + id).enable();
        Ext.getCmp("btnDirNomenReload" + id).enable();
        Ext.getCmp("TriggerSearchTree" + id).enable();
        //Артикул (он же код товара) делаем редактируемым
        Ext.getCmp("DirNomenID" + id).setValue("");
        Ext.getCmp("DirNomenID_INSERT" + id).setValue("");
        Ext.getCmp("DirNomenID_INSERT" + id).setReadOnly(true);
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-tovar/", '_blank');
    }
});



// === Функции === === ===

function controllerDirNomens_onTree_folderNew(id) {
    Ext.getCmp("form_" + id).reset(true); //var widgetXForm = Ext.getCmp("form_" + id).reset(true);
    Ext.getCmp("tree_" + id).disable();
    Ext.getCmp("DirNomenPatchFull" + id).disable();
    Ext.getCmp("btnDirNomenReload" + id).disable();
    Ext.getCmp("TriggerSearchTree" + id).disable();
    //По умолчанию
    Ext.getCmp("DirNomenTypeID" + id).setValue(1);
    Ext.getCmp("KKMSTax" + id).setValue(varKKMSTax);
    //Артикул (он же код товара) делаем редактируемым
    Ext.getCmp("DirNomenID" + id).setValue("");
    Ext.getCmp("DirNomenID_INSERT" + id).setValue("");
    Ext.getCmp("DirNomenID_INSERT" + id).setReadOnly(false);
};
function controllerDirNomens_onTree_folderEdit(id) {
    //Артикул (он же код товара) делаем редактируемым
    //Ext.getCmp("DirNomenID" + id).setValue("");
    //Ext.getCmp("DirNomenID_INSERT" + id).setValue("");
    //Ext.getCmp("DirNomenID_INSERT" + id).setReadOnly(true);

    Ext.getCmp("tree_" + id).disable();
    Ext.getCmp("DirNomenPatchFull" + id).disable();
    Ext.getCmp("btnDirNomenReload" + id).disable();
    Ext.getCmp("TriggerSearchTree" + id).disable();
};
function controllerDirNomens_onTree_folderNewSub(id) {
    var widgetXForm = Ext.getCmp("form_" + id).reset(true);

    var node = funReturnNode(id);
    Ext.getCmp("Sub" + id).setValue(node.data.id);

    Ext.getCmp("tree_" + id).disable();
    Ext.getCmp("DirNomenPatchFull" + id).disable();
    Ext.getCmp("btnDirNomenReload" + id).disable();
    Ext.getCmp("TriggerSearchTree" + id).disable();
    //По умолчанию
    Ext.getCmp("DirNomenTypeID" + id).setValue(1);
    Ext.getCmp("KKMSTax" + id).setValue(varKKMSTax);
    //Артикул (он же код товара) делаем редактируемым
    Ext.getCmp("DirNomenID" + id).setValue("");
    Ext.getCmp("DirNomenID_INSERT" + id).setValue("");
    Ext.getCmp("DirNomenID_INSERT" + id).setReadOnly(false);
};
function controllerDirNomens_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {
                Ext.getCmp("DirNomenName" + id).setValue(Ext.getCmp("DirNomenName" + id).getValue() + " (копия)");

                Ext.getCmp("tree_" + id).disable();
                Ext.getCmp("DirNomenPatchFull" + id).disable();
                Ext.getCmp("btnDirNomenReload" + id).disable();
                Ext.getCmp("TriggerSearchTree" + id).disable();
                //Артикул (он же код товара) делаем редактируемым
                Ext.getCmp("DirNomenID" + id).setValue("");
                Ext.getCmp("DirNomenID_INSERT" + id).setValue("");
                Ext.getCmp("DirNomenID_INSERT" + id).setReadOnly(false);
            }
        }
    });
};
function controllerDirNomens_onTree_folderDel(id) {
    //Формируем сообщение: Удалить или снять пометку на удаление
    var sMsg = lanDelete;
    if (Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;

    //Процес Удаление или снятия пометки
    Ext.MessageBox.show({
        title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {

                //Артикул (он же код товара) делаем редактируемым
                Ext.getCmp("DirNomenID" + id).setValue("");
                Ext.getCmp("DirNomenID_INSERT" + id).setValue("");
                Ext.getCmp("DirNomenID_INSERT" + id).setReadOnly(true);

                //Лоадер
                var loadingMask = new Ext.LoadMask({
                    msg: 'Please wait...',
                    target: Ext.getCmp("tree_" + id)
                });
                loadingMask.show();
                //Выбранный ID-шник Грида
                var DirNomenID = Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.id; //Ext.getCmp("tree_" + id).view.getSelectionModel().getSelection()[0].data.DirNomenID;
                //Запрос на удаление
                Ext.Ajax.request({
                    timeout: varTimeOutDefault,
                    url: HTTP_DirNomens + DirNomenID + "/",
                    method: 'DELETE',
                    success: function (result) {
                        loadingMask.hide();
                        var sData = Ext.decode(result.responseText);
                        if (sData.success == true) {
                            //Очистить форму
                            Ext.getCmp("DirNomenPatchFull" + id).setValue("");
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
function controllerDirNomens_onTree_folderSubNull(id) {
    //Если это не узел, то выйти и сообщить об этом!
    /*
    if (!Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.leaf) {
        Ext.Msg.alert(lanOrgName, "Данная ветвь уже корневая!");
        return;
    }
    */

    //Формируем сообщение: Удалить или снять пометку на удаление
    var sMsg = "Сделать корневой";
    if (Ext.getCmp("tree_" + id).getSelectionModel().getSelection()[0].data.Del == true) sMsg = lanDeletionRemoveMarked;


    Ext.MessageBox.show({
        title: lanOrgName, msg: sMsg + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {

                //Артикул (он же код товара) делаем редактируемым
                Ext.getCmp("DirNomenID" + id).setValue("");
                Ext.getCmp("DirNomenID_INSERT" + id).setValue("");
                Ext.getCmp("DirNomenID_INSERT" + id).setReadOnly(true);

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
function controllerDirNomens_onTree_addSub(id) {

    //Если форма ещё не загружена - выйти!
    var widgetXForm = Ext.getCmp("form_" + id);
    //if (!widgetXForm.UO_Loaded) return;

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

    //Артикул (он же код товара) делаем редактируемым
    Ext.getCmp("DirNomenID" + id).setValue("");
    Ext.getCmp("DirNomenID_INSERT" + id).setValue("");
    Ext.getCmp("DirNomenID_INSERT" + id).setReadOnly(false);

};
//2. Поиск по Ш-к
function controllerDirNomens_onTriggerSearchTreeClick_Search(aButton, bReset) {
    if (Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue() == "") return;
    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).disable(); //Кнопку поиска делаем не активной


    if (Ext.getCmp("SearchType" + aButton.UO_id).getValue() == 1) {

        var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("tree_" + aButton.UO_id) });
        loadingMask.show();

        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            //                        id,                                                iPriznak
            url: HTTP_DirNomens + Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value + "/1/",
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

function controllerDirNomens_onDirNomenCategoryIDSelect(combo, records, eOpts) {
    if (Ext.getCmp("DirNomenName" + combo.UO_id).getValue().length > 0) { Ext.Msg.alert(lanOrgName, "Внимание! Наименование товара сменилось с <b>" + Ext.getCmp("DirNomenName" + combo.UO_id).getValue() + "</b> на <b>" + combo.rawValue + "</b>"); }
    Ext.getCmp("DirNomenName" + combo.UO_id).setValue(combo.rawValue);
    Ext.getCmp("DirNomenNameFull" + combo.UO_id).setValue(combo.rawValue);
};

