﻿Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocActWriteOffs/controllerDocActWriteOffsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocActWriteOffsEdit': { close: this.this_close },

            //Сисок (itemId=tree)
            // Меню Списка
            'viewDocActWriteOffsEdit [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDocActWriteOffsEdit [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDocActWriteOffsEdit [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDocActWriteOffsEdit [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDocActWriteOffsEdit [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDocActWriteOffsEdit [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDocActWriteOffsEdit [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDocActWriteOffsEdit [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick,

                //itemcontextmenu: this.onTree_contextMenuForTreePanel,
            },

            'viewDocActWriteOffsEdit dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },



            //PanelParty
            // Клик по Гриду "Party"
            'viewDocActWriteOffsEdit [itemId=gridParty]': {
                selectionchange: this.onGridParty_selectionchange,
                itemclick: this.onGridParty_itemclick,
                itemdblclick: this.onGridParty_itemdblclick
            },
            'viewDocActWriteOffsEdit button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
            'viewDocActWriteOffsEdit #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },



            //PanelDocumentDetails
            //Склад - Перегрузить
            'viewDocActWriteOffsEdit [itemId=DirWarehouseID]': { select: this.onDirWarehouseIDSelect },
            'viewDocActWriteOffsEdit button#btnDirWarehouseEdit': { click: this.onBtnDirWarehouseEditClick },
            'viewDocActWriteOffsEdit button#btnDirWarehouseReload': { click: this.onBtnDirWarehouseReloadClick },

            //PanelDocumentAdditionally
            //Склад - Перегрузить
            'viewDocActWriteOffsEdit [itemId=DirVatValue]': { select: this.onDirVatValueSelect },
            'viewDocActWriteOffsEdit [itemId=Payment]': { change: this.onPaymentChange },

            //Грид (itemId=grid)
            'viewDocActWriteOffsEdit button#btnGridAddPosition': { click: this.onGrid_BtnGridAddPosition },
            //'viewDocActWriteOffsEdit menuitem#btnGridSelectionOfGoods': { click: this.onGrid_BtnGridSelectionOfGoods },

            'viewDocActWriteOffsEdit button#btnGridEdit': { click: this.onGrid_BtnGridEdit },
            'viewDocActWriteOffsEdit button#btnGridDelete': { click: this.onGrid_BtnGridDelete },
            'viewDocActWriteOffsEdit button#btnGridPayment': { click: this.onGrid_BtnGridPayment },
            'viewDocActWriteOffsEdit button#btnGridLoadAllRem': { click: this.onGrid_BtnGridLoadAllRem },

            
            // Клик по Гриду
            'viewDocActWriteOffsEdit [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },





            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocActWriteOffsEdit button#btnHeldCancel': { click: this.onBtnHeldCancelClick },
            'viewDocActWriteOffsEdit button#btnHelds': { click: this.onBtnHeldsClick },
            'viewDocActWriteOffsEdit menuitem#btnSave': { click: this.onBtnSaveClick },
            'viewDocActWriteOffsEdit menuitem#btnSaveClose': { click: this.onBtnSaveClick },
            'viewDocActWriteOffsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocActWriteOffsEdit button#btnHelp': { "click": this.onBtnHelpClick },
            //***
            'viewDocActWriteOffsEdit menuitem#btnPrintHtml': { click: this.onBtnPrintHtmlClick },
            'viewDocActWriteOffsEdit menuitem#btnPrintExcel': { click: this.onBtnPrintHtmlClick },
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
    onTriggerSearchTreeClick1: function (aButton, aEvent) {
        fun_onTriggerSearchTreeClick_Search(aButton, false);
    },
    onTriggerSearchTreeClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            fun_onTriggerSearchTreeClick_Search(f, false);
        }
    },
    onTriggerSearchTreeClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) {
            //funGridDir(e.UO_id, textReal, HTTP_DirNomens);
            //alert("В стадии разработки ...");
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
        controllerDocActWriteOffsEdit_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDocActWriteOffsEdit_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDocActWriteOffsEdit_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDocActWriteOffsEdit_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDocActWriteOffsEdit_onTree_folderDel(aButton.UO_id);
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
        Ext.getCmp("DirNomenPatchFull" + id).setValue(rec.get('DirNomenPatchFull'));

        var storeGrid = Ext.getCmp("gridParty_" + id).getStore();
        //Если панель скрыта, то не показывать "партии товара"
        if (Ext.getCmp("gridParty_" + id).collapsed) { storeGrid.setData([], false); return; } 
        //Полный путь от Группы к выбранному объкту
        Ext.getCmp("DirNomenPatchFull" + id).setValue(rec.get('DirNomenPatchFull'));
        //Выбрана ли Организация (если новая накладная, то может быть и не выбрана Организация)
        if (Ext.getCmp("DirContractorIDOrg" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); return; }
        //Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
        if (Ext.getCmp("DirWarehouseID" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад (так как партии привязаны к Складу)!"); return; }

        //Получаем storeGrid и делаем load()
        Ext.getCmp("tree_" + id).setDisabled(true);
        storeGrid.proxy.url = HTTP_RemParties + "?DirNomenID=" + rec.get('id') + "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + id).getValue() + "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d");
        storeGrid.load();
        Ext.getCmp("tree_" + id).setDisabled(false);
    },
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {

    },


    //beforedrop
    onTree_beforedrop: function (node, data, overModel, dropPosition, dropPosition1, dropPosition2, dropPosition3) {

    },
    //drop
    onTree_drop: function (node, data, overModel, dropPosition) {
        //Ext.Msg.alert("Группа перемещена!");
    },


    /*onTree_contextMenuForTreePanel: function (view, rec, node, index, e) {
        alert("222222");
    },*/



    //PanelDocumentDetails === === === === ===

    // *** DirWarehouse ***
    //Редактирование или добавление нового Склада
    onDirWarehouseIDSelect: function (combo, records) {
        var tree_ = Ext.getCmp("tree_" + combo.UO_id);
        tree_.store.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + records.data.DirWarehouseID;
    },
    onBtnDirWarehouseEditClick: function (combo, aEvent, aOptions) {
        var Params = [
            combo.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirWarehouses", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirWarehouseReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirWarehousesGrid = Ext.getCmp(aButton.UO_idMain).storeDirWarehousesGrid;
        storeDirWarehousesGrid.load();
    },



    //PanelDocumentAdditionally
    //DirVatValue
    onDirVatValueSelect: function (combo, records) {
        controllerDocActWriteOffsEdit_RecalculationSums(combo.UO_id, false);
    },
    //Скидка
    onPaymentChange: function (aTextfield, aText) {
        controllerDocActWriteOffsEdit_RecalculationSums(aTextfield.UO_id, false)
    },



    //PanelDocumentDetails === === === === ===

    
    //Грид (itemId=grid) === === === === ===

    //Новая: Добавить позицию
    onGrid_BtnGridAddPosition: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;
        var node = funReturnNode(id);

        //Выбран ли товар
        //if (node == null) { Ext.Msg.alert(lanOrgName, "Не выбран товар! Выберите товар в списке слева!"); return; }

        //Выбрана ли партия
        var IdcallModelData = Ext.getCmp("gridParty_" + id).getSelectionModel().getSelection();
        if (IdcallModelData.length == 0) {
            Ext.Msg.alert(lanOrgName, "Не выбрана партия! Выберите партию товара в списке сверху!");
            return;
        }

        //Есть ли Остаток
        //if (Ext.getCmp("gridParty_" + id).store.data.length == 0) { Ext.Msg.alert(lanOrgName, "Нет остатка товара (партий) на складе!"); return; }
        

        /*
        - Когда выбираем партию товара ПРОВЕРЯТЬ Склад. 
          Если не соответствует - вывести об этом сообщение и НЕ открытвать форму!
        Алгоритм:
        1. Кликнули на добавить товар
        2. Проверка в контролере на склад в документа и в партии
        3. Не соответствует - сообщение и закрыть форму
        4. Соответствует - всё ок!
        */
        if (IdcallModelData[0].data.DirWarehouseID != parseInt(Ext.getCmp("DirWarehouseID" + id).getValue())) {
            Ext.Msg.alert(lanOrgName, "Склад партии не соответствует складу документа!<br /> Поменяйте склад документа или кликните ещё раз на выбранном товаре!");
            return;
        }


        var Params = [
            "grid_" + id,
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            IdcallModelData[0], //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
            id,       //UO_Param_id -  в данном случае это id-шник контрола который вызвал
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            false      //GridTree
        ]
        ObjectEditConfig("viewDocActWriteOffTabsEdit", Params);
    },


    //Новая: Редактировать позицию
    onGrid_BtnGridEdit: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;

        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            2,    // 1 - Новое, 2 - Редактировать
            true,
            Ext.getCmp("grid_" + aButton.UO_id).store.indexOf(Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0]),       // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0], // Для загрузки данных в форму редактирования Табличной части
            id,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            false
        ]
        ObjectEditConfig("viewDocActWriteOffTabsEdit", Params);
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
    onGrid_BtnGridLoadAllRem: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;

        //Выбрана ли Организация (если новая накладная, то может быть и не выбрана Организация)
        if (Ext.getCmp("DirContractorIDOrg" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); return; }

        //Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
        if (Ext.getCmp("DirWarehouseID" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Точку (так как партии привязаны к Точке)!"); return; }

        //Получаем storeGrid и делаем load()
        Ext.getCmp("tree_" + id).setDisabled(true);

        var storeGrid = Ext.getCmp("grid_" + id).getStore();
        storeGrid.proxy.url =
            HTTP_RemParties +
            //"?DirNomenID=" + rec.get('id') +
            "?DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() +
            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + id).getValue() +
            "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d") + 
            "&queryIn=DocActWriteOffTab";

        storeGrid.load();

        Ext.getCmp("tree_" + id).setDisabled(false);

    },


    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#btnGridEdit").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#btnGridDelete").setDisabled(records.length === 0);
    },
    //Клик: Редактирования или выбор
    onGrid_itemclick: function (view, record, item, index, eventObj) {
        //Если запись удалена, то выдать сообщение и выйти
        if (funGridRecordDel(record)) { return; }

        if (varSelectOneClick) {

            var id = view.grid.UO_id;

            if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
                var Params = [
                    "grid_" + id, //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    2,    // 1 - Новое, 2 - Редактировать
                    true,
                    Ext.getCmp("grid_" + id).store.indexOf(Ext.getCmp("grid_" + id).getSelectionModel().getSelection()[0]),       // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    Ext.getCmp("grid_" + id).getSelectionModel().getSelection()[0], // Для загрузки данных в форму редактирования Табличной части
                    id,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    false
                ]
                ObjectEditConfig("viewDocActWriteOffTabsEdit", Params);
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

            var id = view.grid.UO_id;

            var Params = [
                "grid_" + id, //UO_idCall
                true, //UO_Center
                true, //UO_Modal
                2,    // 1 - Новое, 2 - Редактировать
                true,
                Ext.getCmp("grid_" + id).store.indexOf(Ext.getCmp("grid_" + id).getSelectionModel().getSelection()[0]),       // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                Ext.getCmp("grid_" + id).getSelectionModel().getSelection()[0], // Для загрузки данных в форму редактирования Табличной части
                id,
                undefined,
                undefined,
                undefined,
                undefined,
                undefined,
                false
            ]
            ObjectEditConfig("viewDocActWriteOffTabsEdit", Params);
        }
        else {
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }
    },


    // === Кнопки === === ===

    //Провести
    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { controllerDocActWriteOffsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

    //Провести
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        /*
        if (Ext.getCmp('HavePay' + aButton.UO_id).getValue() != 0) {
            Ext.MessageBox.show({
                title: lanOrgName, msg: txtMsg026, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                fn: function (buttons) {
                    if (buttons == "yes") {
                        controllerDocActWriteOffsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                    }
                }
            });
        }
        else {
            controllerDocActWriteOffsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
        }
        */

        controllerDocActWriteOffsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
    },

    //Сохранить или Сохранить и закрыть
    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        controllerDocActWriteOffsEdit_onBtnSaveClick(aButton, aEvent, aOptions);

    },
    //Отменить
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    //Help
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-act-write-off/", '_blank');
    },
    //***
    //Распечатать
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {
        //aButton.UO_Action: html, excel
        //alert(aButton.UO_Action);

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocActWriteOffID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

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
            35
        ]
        ObjectConfig("viewListObjectPFs", Params);

    },
});


//Функия сохранения
function controllerDocActWriteOffsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    //Спецификация (табличная часть)
    var recordsDocActWriteOffTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocActWriteOffTab.push(rec.data); });

    //Проверка
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocActWriteOffs + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocActWriteOffID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocActWriteOffs + "?id=" + parseInt(Ext.getCmp("DocActWriteOffID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocActWriteOffTab: Ext.encode(recordsDocActWriteOffTab) },

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
                Ext.getCmp('DocActWriteOffID' + aButton.UO_id).setValue(sData.DocActWriteOffID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocActWriteOffID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocActWriteOffID);

                Ext.getCmp('viewDocActWriteOffsEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocActWriteOffsEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocActWriteOffID" + aButton.UO_id).getValue() + ")");
                Ext.getCmp("btnPrint" + aButton.UO_id).setVisible(true);
            }

            //Закрыть
            if (aButton.UO_Action == "save_close" || aButton.UO_Action == "held") { Ext.getCmp(aButton.UO_idMain).close(); }
            //Перегрузить грид, если грид открыт
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }
        },
        failure: function (form, action) { funPanelSubmitFailure(form, action); }
    });
};



//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocActWriteOffsEdit_RecalculationSums(id, ShowMsg) {

    fun_DocX_RecalculationSums(id, ShowMsg);
    return;

    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocActWriteOffTabsGrid //можно так: Ext.getCmp("viewDocActWriteOffsEdit" + id).storeGrid;
    var Match = true; //совпадение ставок НДС для всех Номенклатур.

    var SumOfVATCurrency = 0;
    for (var i = 0; i < storeGrid.data.items.length; i++) {
        //Номенклатура + проверка на совпадение ставок НДС для всех Номенклатур.
        //Сумма спецификации
        var pSumOfVATCurrency = parseFloat(storeGrid.data.items[i].data.Quantity * storeGrid.data.items[i].data.PriceCurrency);
        //Меняем сумму спецификации (если добавили новую позицию)
        storeGrid.data.items[i].data.SUMPurchPriceVATCurrency = pSumOfVATCurrency.toFixed(varFractionalPartInSum);
        SumOfVATCurrency += parseFloat(pSumOfVATCurrency);
    }

    //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)
    Ext.getCmp("grid_" + id).getView().refresh();
    //Сумма с НДС
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum)); //Подсчёт суммы SUM(Учётная цена * К-во)
};



// === Функции === === ===
//1. Для Товара - КонтекстМеню
function controllerDocActWriteOffsEdit_onTree_folderNew(id) {
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
        undefined,
        undefined,
        undefined,
        false      //GridTree
    ]
    ObjectEditConfig("viewDirNomensWinEdit", Params);
};
function controllerDocActWriteOffsEdit_onTree_folderEdit(id) {
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
        undefined,
        undefined,
        undefined,
        false      //GridTree
    ]
    ObjectEditConfig("viewDirNomensWinEdit", Params);
};
function controllerDocActWriteOffsEdit_onTree_folderNewSub(id) {
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
    ObjectEditConfig("viewDirNomensWinEdit", Params);
};
function controllerDocActWriteOffsEdit_onTree_folderCopy(id) {
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
                ObjectEditConfig("viewDirNomensWinEdit", Params);

            }
        }
    });
};
function controllerDocActWriteOffsEdit_onTree_folderDel(id) {
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
function controllerDocActWriteOffsEdit_onTree_folderSubNull(id) {
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
function controllerDocActWriteOffsEdit_onTree_addSub(id) {
    
    //Если форма ещё не загружена - выйти!
    var widgetXForm = Ext.getCmp("form_" + id);
    //if (!widgetXForm.UO_Loaded) return;

    var node = funReturnNode(id);
    if (node != undefined) {
        node.data.leaf = false;
        Ext.getCmp("tree_" + id).getView().refresh();
        node.expand();
    }

};
