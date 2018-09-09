Ext.define("PartionnyAccount.controller.Sklad/Object/Dir/DirNomens/controllerDirNomenRemParties", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDirNomenRemParties': { close: this.this_close },

            //Группа (itemId=tree)
            // Меню Группы
            /*'viewDirNomenRemParties [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDirNomenRemParties [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDirNomenRemParties [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDirNomenRemParties [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDirNomenRemParties [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDirNomenRemParties [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDirNomenRemParties [itemId=FolderDel]': { click: this.onTree_folderDel },*/
            // Клик по Группе
            'viewDirNomenRemParties [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick
            },

            'viewDirNomenRemParties dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },

            //Заказ
            'viewDirNomenRemParties button#btnOrder': { click: this.onBtnOrderClick },
            //Релоад
            'viewDirNomenRemParties button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
            //Поиск
            'viewDirNomenRemParties #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },
            


            // Клик по Гриду
            'viewDirNomenRemParties [itemId=gridParty]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },



            //Информация *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***
            //Обновить
            'viewDirNomenRemParties button#btnGridsRefresh': { click: this.onBtnGridsRefreshClick },
            // Клик по Гриду
            'viewDirNomenRemParties [itemId=PanelRemParties]': {
                selectionchange: this.onRemPartiesGrid_selectionchange,
                itemclick: this.onRemPartiesGrid_itemclick,
                itemdblclick: this.onRemPartiesGrid_itemdblclick
            },



            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDirNomenRemParties button#btnSave': { "click": this.onBtnSaveClick },
            'viewDirNomenRemParties button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDirNomenRemParties button#btnHelp': { "click": this.onBtnHelpClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
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
        controllerDocSalesEdit_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDocSalesEdit_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDocSalesEdit_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDocSalesEdit_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDocSalesEdit_onTree_folderDel(aButton.UO_id);
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
        var idCall = Ext.getCmp(view.grid.UO_idCall).UO_id;


        //Полный путь от Группы к выбранному объкту
        Ext.getCmp("DirNomenPatchFull" + id).setValue(rec.get('DirNomenPatchFull'));

        var storeGrid = Ext.getCmp("gridParty_" + id).getStore();
        //Если панель скрыта, то не показывать "партии товара"
        if (Ext.getCmp("gridParty_" + id).collapsed) { storeGrid.setData([], false); return; }
        //Выбрана ли Организация (если новая накладная, то может быть и не выбрана Организация)
        if (Ext.getCmp("DirContractorIDOrg" + idCall).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); return; }
        //Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
        if (Ext.getCmp("DirWarehouseID" + idCall).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад (так как партии привязаны к Складу)!"); return; }

        //Получаем storeGrid и делаем load()
        Ext.getCmp("tree_" + id).setDisabled(true);
        //storeGrid.proxy.url = HTTP_RemParties + "?DirNomenID=" + rec.get('id') + "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + idCall).getValue() + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + idCall).getValue() + "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + idCall).getValue(), "Y-m-d");
        storeGrid.proxy.url = HTTP_RemParties + "?DirNomenID=" + rec.get('id') + "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + idCall).getValue() + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + idCall).getValue() + "&DocDate=" + Ext.getCmp("DocDate" + idCall).getValue();
        storeGrid.load();
        Ext.getCmp("tree_" + id).setDisabled(false);
    },
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {

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


    //Заказ
    onBtnOrderClick: function (aButton, aEvent, aOptions) {
        //Откроется форма Заказа
        /*
        Ext.MessageBox.show({
            title: lanOrgName, msg: "Согласована ли данная запчасть с клиентом?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {

                    //В которой будет вся информация о Аппарате взятом на ремонт и Зап.части
                    var Params = [
                        "tree_" + aButton.UO_id, //UO_idCall
                        true, //UO_Center
                        true, //UO_Modal
                        1,     // 1 - Новое, 2 - Редактировать
                        undefined,
                        1,  //Содержит тип заказа: 1 - Из Мастерской, 2 - Из Магазина
                        //IdcallModelData[0], //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
                    ]
                    ObjectEditConfig("viewDocOrderIntsEdit", Params);

                }
                else {
                    Ext.MessageBox.show({ title: lanOrgName, msg: "Согласуйте данную запчасть с клиентом!", icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
                }
            }
        });
        */

        //В которой будет вся информация о Аппарате взятом на ремонт и Зап.части
        var Params = [
            "tree_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,     // 1 - Новое, 2 - Редактировать
            undefined,
            Ext.getCmp("DirOrderIntTypeID" + aButton.UO_id).getValue(), //3,  //Содержит тип заказа: 1 - Из Мастерской, 2 - Из Магазина
            //IdcallModelData[0], //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
        ]
        ObjectEditConfig("viewDocOrderIntsEdit", Params);

    },
    //Обновить список Товаров
    onBtnDirNomenReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirNomensTree = Ext.getCmp("tree_" + aButton.UO_id).store; storeDirNomensTree.setData([], false); storeDirNomensTree.UO_OnStop = true;
        storeDirNomensTree.load();
    },

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


    // *** Фотка ***
    onPanelGeneral_ImageLink: function (aTextfield, aValueReal, aValuePrevious) {
        try {
            Ext.getCmp("imageShow" + aTextfield.UO_id).setSrc(aValueReal);
        } catch (ex) { alert(e.name); }

    },



    // *** DirNomenCategoryName ***
    //Редактирование или добавление нового Поставщика
    onBtnDirNomenCategoryEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirNomenCategories", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirNomenCategoryReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirNomenCategoriesGrid = Ext.getCmp(aButton.UO_idMain).storeDirNomenCategoriesGrid;
        storeDirNomenCategoriesGrid.load();
    },
    //Очистить Скидку
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
                //Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, view.grid.UO_id, record);
                //Ext.getCmp(view.grid.UO_idMain).close();

                var Params = [
                    "gridParty_" + view.grid.UO_id,
                    true, //UO_Center
                    true, //UO_Modal
                    2,    // 1 - Новое, 2 - Редактировать
                    true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер

                    Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid,    //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    Ext.getCmp(view.grid.UO_idCall).UO_id,    //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
                    view.grid.UO_id,    //UO_Param_id -  в данном случае это id-шник контрола который вызвал
                    record,

                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    false      //GridTree
                ]
                ObjectEditConfig("viewDirNomensSelect", Params);
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
            //Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, view.grid.UO_id, record);
            //Ext.getCmp(view.grid.UO_idMain).close();

            var Params = [
                "gridParty_" + view.grid.UO_id,
                true, //UO_Center
                true, //UO_Modal
                2,    // 1 - Новое, 2 - Редактировать
                true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер

                Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid,    //UO_GridIndex,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                Ext.getCmp(view.grid.UO_idCall).UO_id,               //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
                view.grid.UO_id,                                     //UO_Param_id -  в данном случае это id-шник контрола который вызвал
                record,                                              //UO_Param_fn
                view.grid.UO_idMain,                                 //UO_idTab

                undefined,
                undefined,
                undefined,
                false      //GridTree
            ]
            ObjectEditConfig("viewDirNomensSelect", Params);
        }
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
        Ext.getCmp("tree_" + aButton.UO_id).enable();
        Ext.getCmp("DirNomenPatchFull" + aButton.UO_id).enable();
        Ext.getCmp("btnDirNomenReload" + aButton.UO_id).enable();
        Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable();
    },
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "spravochnik-tovar/", '_blank');
    }
});