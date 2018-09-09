Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocCashOfficeSums/controllerDocCashOfficeSumMovementsEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocCashOfficeSumMovementsEdit': { close: this.this_close },

            //Сисок (itemId=tree)
            // Меню Списка
            'viewDocCashOfficeSumMovementsEdit [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDocCashOfficeSumMovementsEdit [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDocCashOfficeSumMovementsEdit [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDocCashOfficeSumMovementsEdit [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDocCashOfficeSumMovementsEdit [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDocCashOfficeSumMovementsEdit [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDocCashOfficeSumMovementsEdit [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDocCashOfficeSumMovementsEdit [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick,

                //itemcontextmenu: this.onTree_contextMenuForTreePanel,
            },

            'viewDocCashOfficeSumMovementsEdit dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },



            //PanelParty
            // Клик по Гриду "Party"
            'viewDocCashOfficeSumMovementsEdit [itemId=gridParty]': {
                selectionchange: this.onGridParty_selectionchange,
                itemclick: this.onGridParty_itemclick,
                itemdblclick: this.onGridParty_itemdblclick
            },
            'viewDocCashOfficeSumMovementsEdit button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
            'viewDocCashOfficeSumMovementsEdit #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },



            //PanelDocumentDetails
            //Контрагент - Перегрузить
            'viewDocCashOfficeSumMovementsEdit button#btnDirContractorEdit': { click: this.onBtnDirContractorEditClick },
            'viewDocCashOfficeSumMovementsEdit button#btnDirContractorReload': { click: this.onBtnDirContractorReloadClick },
            //Склад - Перегрузить
            //From
            'viewDocCashOfficeSumMovementsEdit [itemId=DirWarehouseIDFrom]': { select: this.onDirWarehouseIDFromSelect },
            'viewDocCashOfficeSumMovementsEdit button#btnDirWarehouseEditFrom': { click: this.onBtnDirWarehouseEditFromClick },
            'viewDocCashOfficeSumMovementsEdit button#btnDirWarehouseReloadFrom': { click: this.onBtnDirWarehouseReloadFromClick },
            //To
            'viewDocCashOfficeSumMovementsEdit [itemId=DirWarehouseIDTo]': { select: this.onDirWarehouseIDToSelect },
            'viewDocCashOfficeSumMovementsEdit button#btnDirWarehouseEditTo': { click: this.onBtnDirWarehouseEditToClick },
            'viewDocCashOfficeSumMovementsEdit button#btnDirWarehouseReloadTo': { click: this.onBtnDirWarehouseReloadToClick },

            //Курьер
            'viewDocCashOfficeSumMovementsEdit [itemId=DirEmployeeIDCourier]': { select: this.onDirEmployeeIDCourierSelect },
            'viewDocCashOfficeSumMovementsEdit button#btnDirEmployeeIDCourier': { click: this.onBtnDirEmployeeIDCourierClick },

            //PanelDocumentAdditionally
            //Склад - Перегрузить
            'viewDocCashOfficeSumMovementsEdit [itemId=DirVatValue]': { select: this.onDirVatValueSelect },
            'viewDocCashOfficeSumMovementsEdit [itemId=Payment]': { change: this.onPaymentChange },

            //PanelDocumentDiscount
            'viewDocCashOfficeSumMovementsEdit [itemId=Discount]': { change: this.onDiscount },
            'viewDocCashOfficeSumMovementsEdit button#btnSpreadDiscount': { click: this.onBtnSpreadDiscount },
            'viewDocCashOfficeSumMovementsEdit #SummOther': { select: this.onSummOther },
            'viewDocCashOfficeSumMovementsEdit button#btnSpreadSummOther': { click: this.onBtnSpreadSummOther },

            //PanelDocumentPay
            'viewDocCashOfficeSumMovementsEdit [itemId=PayZReport]': { change: this.onPayZReportChecked },

            //Грид (itemId=grid)
            'viewDocCashOfficeSumMovementsEdit button#btnGridAddPosition': { click: this.onGrid_BtnGridAddPosition },
            //'viewDocCashOfficeSumMovementsEdit menuitem#btnGridSelectionOfGoods': { click: this.onGrid_BtnGridSelectionOfGoods },

            'viewDocCashOfficeSumMovementsEdit button#btnGridEdit': { click: this.onGrid_BtnGridEdit },
            'viewDocCashOfficeSumMovementsEdit button#btnGridDelete': { click: this.onGrid_BtnGridDelete },
            'viewDocCashOfficeSumMovementsEdit button#btnGridPayment': { click: this.onGrid_BtnGridPayment },


            // Клик по Гриду
            'viewDocCashOfficeSumMovementsEdit [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },





            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocCashOfficeSumMovementsEdit button#btnHeldCancel': { click: this.onBtnHeldCancelClick },
            'viewDocCashOfficeSumMovementsEdit button#btnHelds': { click: this.onBtnHeldsClick },
            'viewDocCashOfficeSumMovementsEdit menuitem#btnSave': { click: this.onBtnSaveClick },
            'viewDocCashOfficeSumMovementsEdit menuitem#btnSaveClose': { click: this.onBtnSaveClick },
            'viewDocCashOfficeSumMovementsEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocCashOfficeSumMovementsEdit button#btnHelp': { "click": this.onBtnHelpClick },
            //***
            'viewDocCashOfficeSumMovementsEdit menuitem#btnPrintHtml': { click: this.onBtnPrintHtmlClick },
            'viewDocCashOfficeSumMovementsEdit menuitem#btnPrintExcel': { click: this.onBtnPrintHtmlClick },
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
        controllerDocCashOfficeSumMovementsEdit_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDocCashOfficeSumMovementsEdit_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDocCashOfficeSumMovementsEdit_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDocCashOfficeSumMovementsEdit_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDocCashOfficeSumMovementsEdit_onTree_folderDel(aButton.UO_id);
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
        if (Ext.getCmp("DirContractorIDOrg" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Организацию (так как партии привязаны к Организации)!"); Ext.getCmp("tree_" + id).setDisabled(false); return; }
        //Выбран ли Склад (если новая накладная, то может быть и не выбран Склад)
        if (Ext.getCmp("DirWarehouseIDFrom" + id).getValue() == null) { Ext.Msg.alert(lanOrgName, "Выбирите Склад (так как партии привязаны к Складу)!"); Ext.getCmp("tree_" + id).setDisabled(false); return; }

        //Получаем storeGrid и делаем load()
        Ext.getCmp("tree_" + id).setDisabled(true);
        storeGrid.proxy.url = HTTP_RemParties + "?DirNomenID=" + rec.get('id') + "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseIDFrom" + id).getValue() + "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d");
        storeGrid.load();
        Ext.getCmp("tree_" + id).setDisabled(false);
    },
    // Дабл клик по Группе - не используется
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
        //alert("onTree_itemdbclick");
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

    // *** DirContractorName ***
    //Редактирование или добавление нового Поставщика
    onBtnDirContractorEditClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirContractors", Params);
    },
    //РеЛоад - перегрузить тригер, что бы появились новые записи
    onBtnDirContractorReloadClick: function (aButton, aEvent, aOptions) {
        var storeDirContractorsGrid = Ext.getCmp(aButton.UO_idMain).storeDirContractorsGrid;
        storeDirContractorsGrid.load();
    },
    // *** DirWarehouse ***
    //From
    onDirWarehouseIDFromSelect: function (combo, records) {
        var tree_ = Ext.getCmp("tree_" + combo.UO_id);
        tree_.store.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + records.data.DirWarehouseID;

        if (parseInt(records.data.DirWarehouseID) == parseInt(Ext.getCmp("DirWarehouseIDTo" + combo.UO_id).getValue())) {
            Ext.Msg.alert(lanOrgName, "Склады совпадают!");
        }
    },
    onBtnDirWarehouseEditFromClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirWarehouses", Params);
    },
    onBtnDirWarehouseReloadFromClick: function (aButton, aEvent, aOptions) {
        var storeDirWarehousesGrid = Ext.getCmp(aButton.UO_idMain).storeDirWarehousesGridFrom;
        storeDirWarehousesGrid.load();
    },
    //To
    onDirWarehouseIDToSelect: function (combo, records) {
        //Не нужно
        //var tree_ = Ext.getCmp("tree_" + combo.UO_id);
        //tree_.store.proxy.url = HTTP_DirNomens + "?DirWarehouseID=" + records.data.DirWarehouseID;
        
        var locDirWarehouseIDTo = Ext.getCmp("DirWarehouseIDTo" + combo.UO_id).valueCollection.items[0].data;
        Ext.getCmp("DirCashOfficeIDTo" + combo.UO_id).setValue(locDirWarehouseIDTo.DirCashOfficeID);
        Ext.getCmp("DirCashOfficeSumTo" + combo.UO_id).setValue(locDirWarehouseIDTo.DirCashOfficeSum);

        if (parseInt(records.data.DirWarehouseID) == parseInt(Ext.getCmp("DirWarehouseIDFrom" + combo.UO_id).getValue())) {
            Ext.Msg.alert(lanOrgName, "Склады совпадают!");
        }
    },
    onBtnDirWarehouseEditToClick: function (aButton, aEvent, aOptions) {
        var Params = [
            aButton.id,
            false, //UO_Center
            false, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewDirWarehouses", Params);
    },
    onBtnDirWarehouseReloadToClick: function (aButton, aEvent, aOptions) {
        var storeDirWarehousesGrid = Ext.getCmp(aButton.UO_idMain).storeDirWarehousesGridTo;
        storeDirWarehousesGrid.load();
    },

    //Курьер
    onDirEmployeeIDCourierSelect: function (combo, records) {
        Ext.Msg.alert(lanOrgName, "После сохранения документ попадёт в Логистику!");
    },
    onBtnDirEmployeeIDCourierClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirEmployeeIDCourier" + aButton.UO_id).setValue("");
        Ext.Msg.alert(lanOrgName, "После сохранения документ переместится в Перемещения!");
    },


    //PanelDocumentAdditionally
    //DirVatValue
    onDirVatValueSelect: function (combo, records) {
        controllerDocCashOfficeSumMovementsEdit_RecalculationSums(combo.UO_id, false);
    },
    //Скидка
    onPaymentChange: function (aTextfield, aText) {
        controllerDocCashOfficeSumMovementsEdit_RecalculationSums(aTextfield.UO_id, false)
    },



    //PanelDocumentDetails === === === === ===
    //Скидка
    onDiscount: function (aTextfield, aText) {
        controllerDocCashOfficeSumMovementsEdit_RecalculationSums(aTextfield.UO_id, false)
    },
    // *** Раскинуть скидку по спецификации ***
    onBtnSpreadDiscount: function (aButton, aEvent) {
        Ext.Msg.show({
            title: lanOrgName,
            msg: 'Изменить цены спецификации документа?',
            buttons: Ext.Msg.YESNO,
            fn: function (btn) {
                if (btn == "yes") {
                    var ArrPrice = ["PriceCurrency", "PriceVAT", "SUMPriceCurrency", "DirCurrencyRate", "DirCurrencyMultiplicity", "Quantity"];
                    funSpreadDiscount(aButton.UO_id, ArrPrice, controllerDocCashOfficeSumMovementsEdit_RecalculationSums);
                }
            },
            icon: Ext.window.MessageBox.QUESTION
        });
    },
    // *** Изменить сумму документа ***
    onBtnSpreadSummOther: function (aButton, aEvent) {
        Ext.Msg.show({
            title: lanOrgName,
            msg: 'Изменить цены спецификации документа?',
            buttons: Ext.Msg.YESNO,
            fn: function (btn) {
                if (btn == "yes") {
                    var ArrPrice = ["PriceCurrency", "PriceVAT", "SUMPriceCurrency", "DirCurrencyRate", "DirCurrencyMultiplicity", "Quantity"];
                    funSpreadSummOther(aButton.UO_id, ArrPrice, controllerDocCashOfficeSumMovementsEdit_RecalculationSums);
                }
            },
            icon: Ext.window.MessageBox.QUESTION
        });
    },



    //PanelDocumentPay === === === === ===

    //Даже, если снят Z-отчет, всё равно провести платеж!
    onPayZReportChecked: function (ctl, val) { //ctl.UO_id
        //val==true - checked, val==false - No checked
        if (val) {
            Ext.Msg.alert(lanOrgName, "Данную функцию использовать не рекомендуется!");
        }
        else {
            //...
        }
    },
    


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
        if (IdcallModelData[0].data.DirWarehouseID != parseInt(Ext.getCmp("DirWarehouseIDFrom" + id).getValue())) {
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
        ObjectEditConfig("viewDocCashOfficeSumMovementTabsEdit", Params);
    },


    //Новая: Редактировать позицию
    onGrid_BtnGridEdit: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            2,    // 1 - Новое, 2 - Редактировать
            true,
            Ext.getCmp("grid_" + aButton.UO_id).store.indexOf(Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0]),       // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0], // Для загрузки данных в форму редактирования Табличной части
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            true
        ]
        ObjectEditConfig("viewDocCashOfficeSumMovementTabsEdit", Params);
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
                    if (selection) {
                        Ext.getCmp("grid_" + aButton.UO_id).store.remove(selection);
                        controllerDocCashOfficeSumMovementsEdit_RecalculationSums(aButton.UO_id, false)
                    }
                }
            }
        });
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
            if (Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid == undefined) {
                var Params = [
                    view.grid.id, //UO_idCallб //"grid_" + aButton.UO_id, //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    2,    // 1 - Новое, 2 - Редактировать
                    true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                    index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    record,       // Для загрузки данных в форму Б.С. и Договора,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    true
                ]
                ObjectEditConfig("viewDocCashOfficeSumMovementTabsEdit", Params);
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
                view.grid.id, //UO_idCall
                true, //UO_Center
                true, //UO_Modal
                2,    // 1 - Новое, 2 - Редактировать
                true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                record,        // Для загрузки данных в форму Б.С. и Договора,
                undefined,
                undefined,
                undefined,
                undefined,
                undefined,
                undefined,
                true
            ]
            ObjectEditConfig("viewDocCashOfficeSumMovementTabsEdit", Params);
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
            fn: function (buttons) { if (buttons == "yes") { controllerDocCashOfficeSumMovementsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

    //Провести
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        controllerDocCashOfficeSumMovementsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
    },

    //Сохранить или Сохранить и закрыть
    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        controllerDocCashOfficeSumMovementsEdit_onBtnSaveClick(aButton, aEvent, aOptions);

    },
    //Отменить
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    //Help
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-peremeshcheniye/", '_blank');
    },
    //***
    //Распечатать
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {
        //aButton.UO_Action: html, excel
        //alert(aButton.UO_Action);

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocCashOfficeSumMovementID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

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
            33
        ]
        ObjectConfig("viewListObjectPFs", Params);

    },
});


//Функия сохранения
function controllerDocCashOfficeSumMovementsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    var id = aButton.UO_id;

    //Если склады совпадают выдать сообщение и выход!
    if (parseInt(Ext.getCmp("DirWarehouseIDTo" + id).getValue()) == parseInt(Ext.getCmp("DirWarehouseIDFrom" + id).getValue())) {
        Ext.Msg.alert(lanOrgName, "Склады совпадают!");
        return;
    }


    //Если вызвала "Логистика", то обязательно выбрать курьера
    if (Ext.getCmp("viewDocCashOfficeSumMovementsEdit" + id).UO_GridSave && (Ext.getCmp("DirEmployeeIDCourier" + id).getValue() == "" || Ext.getCmp("DirEmployeeIDCourier" + id).getValue() == null)) {
        Ext.Msg.alert(lanOrgName, "Выбирите Курьера!");
        return;
    }
    
    //Проверка
    if (Ext.getCmp("DirWarehouseIDFrom" + id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    if (Ext.getCmp("DirWarehouseIDTo" + id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocCashOfficeSumMovements + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocCashOfficeSumMovementID" + id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocCashOfficeSumMovements + "?id=" + parseInt(Ext.getCmp("DocCashOfficeSumMovementID" + id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl + "&DirEmployeePswd=" + Ext.getCmp("DirEmployeePswd" + aButton.UO_id).value,

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {

            Ext.getCmp("DirEmployeePswd" + aButton.UO_id).setValue("");

            //Данные с сервера
            var sData = action.result.data;
                
            var sMsg = "";
            if (sData.bFindWarehouse) {
                if (aButton.UO_Action == "held_cancel") {
                    Ext.getCmp("Held" + id).setValue(false);
                    Ext.getCmp("btnHeldCancel" + id).setVisible(false);
                    Ext.getCmp("btnHelds" + id).setVisible(true);
                    Ext.getCmp("btnRecord" + id).setVisible(true);
                }
                else if (aButton.UO_Action == "held") {
                    Ext.getCmp("Held" + id).setValue(true);
                    Ext.getCmp("btnHeldCancel" + id).setVisible(true);
                    Ext.getCmp("btnHelds" + id).setVisible(false);
                    Ext.getCmp("btnRecord" + id).setVisible(false);
                }
            }
            else {
                //У Сотрудника нет доступа к складу на который перемещаем
                if (aButton.UO_Action == "held") {
                    sMsg = "Извините, но у Вас нет прав проводить данный документ, поэтому документ только 'Сохранён'!"
                }
            }


            //Если новая накладная присваиваем полученные номера!
            if (!Ext.getCmp('DocID' + id).getValue()) {
                Ext.getCmp('DocID' + id).setValue(sData.DocID);
                Ext.getCmp('DocCashOfficeSumMovementID' + id).setValue(sData.DocCashOfficeSumMovementID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocCashOfficeSumMovementID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocCashOfficeSumMovementID + "<br />" + sMsg);

                Ext.getCmp('viewDocCashOfficeSumMovementsEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocCashOfficeSumMovementsEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocCashOfficeSumMovementID" + aButton.UO_id).getValue() + ")");
                Ext.getCmp("btnPrint" + aButton.UO_id).setVisible(true);
            }
            else {
                if (sMsg.length > 0) { Ext.Msg.alert(lanOrgName, sMsg); }
            }

            //SMS, только если выбран курьер и статус < 3
            if (
                Ext.getCmp("DirEmployeeIDCourier" + aButton.UO_id).getValue() != "" && Ext.getCmp("DirEmployeeIDCourier" + aButton.UO_id).getValue() != null &&
                (Ext.getCmp("DirMovementStatusID" + aButton.UO_id).getValue() == "" || Ext.getCmp("DirMovementStatusID" + aButton.UO_id).getValue() == null || parseInt(Ext.getCmp("DirMovementStatusID" + aButton.UO_id).getValue()) <= 1) &&
                aButton.UO_Action != "held_cancel"
               ) {

                var DirWarehouseIDTo = Ext.getCmp("DirWarehouseIDTo" + aButton.UO_id).getValue();
                var Params = [
                    "btnRecord" + id, //UO_idCall
                    true, //UO_Center
                    true, //UO_Modal
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    undefined,
                    //"DocServicePurchID=" + Ext.getCmp("DocServicePurchID" + id).getValue() + "&MenuID=1" + "&DirSmsTemplateTypeS=" + DirSmsTemplateTypeS + "&DirSmsTemplateTypePo=" + DirSmsTemplateTypePo
                    "DirWarehouseIDTo=" + DirWarehouseIDTo + "&MenuID=2" + "&DirSmsTemplateTypeS=1" + "&DirSmsTemplateTypePo=1"
                ]
                ObjectConfig("viewSms", Params);

            }


            //Закрыть
            if (aButton.UO_Action == "save_close" || aButton.UO_Action == "held") { Ext.getCmp(aButton.UO_idMain).close(); }
            //Перегрузить грид, если грид открыт
            if (Ext.getCmp(aButton.UO_idCall) != undefined && Ext.getCmp(aButton.UO_idCall).store != undefined) { Ext.getCmp(aButton.UO_idCall).getStore().load(); }
        },
        failure: function (form, action) {
            Ext.getCmp("DirEmployeePswd" + aButton.UO_id).setValue("");
            funPanelSubmitFailure(form, action);
        }
    });
};



//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocCashOfficeSumMovementsEdit_RecalculationSums(id, ShowMsg) { 
    fun_DocX_RecalculationSums(id, ShowMsg);
    return;
    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocCashOfficeSumMovementTabsGrid; //можно так: Ext.getCmp("viewDocCashOfficeSumMovementsEdit" + id).storeDocCashOfficeSumMovementTabsGrid;
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
function controllerDocCashOfficeSumMovementsEdit_onTree_folderNew(id) {
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
function controllerDocCashOfficeSumMovementsEdit_onTree_folderEdit(id) {
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
function controllerDocCashOfficeSumMovementsEdit_onTree_folderNewSub(id) {
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
function controllerDocCashOfficeSumMovementsEdit_onTree_folderCopy(id) {
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
function controllerDocCashOfficeSumMovementsEdit_onTree_folderDel(id) {
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
function controllerDocCashOfficeSumMovementsEdit_onTree_folderSubNull(id) {
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
function controllerDocCashOfficeSumMovementsEdit_onTree_addSub(id) {
    
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
