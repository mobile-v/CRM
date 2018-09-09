Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocPurches/controllerDocPurchesEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocPurchesEdit': { close: this.this_close },

            //Сисок (itemId=tree)
            // Меню Списка
            'viewDocPurchesEdit [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDocPurchesEdit [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDocPurchesEdit [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDocPurchesEdit [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDocPurchesEdit [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDocPurchesEdit [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDocPurchesEdit [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDocPurchesEdit [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick,

                //itemcontextmenu: this.onTree_contextMenuForTreePanel,
            },

            'viewDocPurchesEdit dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },



            //PanelParty
            // Клик по Гриду "Party"
            'viewDocPurchesEdit [itemId=gridParty]': {
                selectionchange: this.onGridParty_selectionchange,
                itemclick: this.onGridParty_itemclick,
                itemdblclick: this.onGridParty_itemdblclick
            },
            'viewDocPurchesEdit button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
            'viewDocPurchesEdit #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },



            //PanelDocumentDetails
            //Контрагент - Перегрузить
            'viewDocPurchesEdit button#btnDirContractorEdit': { click: this.onBtnDirContractorEditClick },
            'viewDocPurchesEdit button#btnDirContractorReload': { click: this.onBtnDirContractorReloadClick },
            //Склад - Перегрузить
            'viewDocPurchesEdit [itemId=DirWarehouseID]': { select: this.onDirWarehouseIDSelect },
            'viewDocPurchesEdit button#btnDirWarehouseEdit': { click: this.onBtnDirWarehouseEditClick },
            'viewDocPurchesEdit button#btnDirWarehouseReload': { click: this.onBtnDirWarehouseReloadClick },

            //PanelDocumentAdditionally
            //Склад - Перегрузить
            'viewDocPurchesEdit [itemId=DirVatValue]': { select: this.onDirVatValueSelect },
            'viewDocPurchesEdit [itemId=Payment]': { change: this.onPaymentChange },

            //PanelDocumentDiscount
            'viewDocPurchesEdit [itemId=Discount]': { change: this.onDiscount },
            'viewDocPurchesEdit button#btnSpreadDiscount': { click: this.onBtnSpreadDiscount },
            'viewDocPurchesEdit #SummOther': { select: this.onSummOther },
            'viewDocPurchesEdit button#btnSpreadSummOther': { click: this.onBtnSpreadSummOther },

            //Грид (itemId=grid)
            'viewDocPurchesEdit button#btnGridAddPosition': { click: this.onGrid_BtnGridAddPosition },
            //'viewDocPurchesEdit menuitem#btnGridSelectionOfGoods': { click: this.onGrid_BtnGridSelectionOfGoods },

            'viewDocPurchesEdit button#btnGridEdit': { click: this.onGrid_BtnGridEdit },
            'viewDocPurchesEdit button#btnGridDelete': { click: this.onGrid_BtnGridDelete },
            'viewDocPurchesEdit button#btnGridPayment': { click: this.onGrid_BtnGridPayment },
            //На основании: Расходную накладную
            'viewDocPurchesEdit menuitem#btnCreateDocMovements': { click: this.onGrid_btnCreateDocMovements },

            'viewDocPurchesEdit menuitem#btnGridDocReturnVendor': { click: this.onGrid_BtnGridDocReturnVendor },
            'viewDocPurchesEdit menuitem#btnGridDocumentMovement': { click: this.onGrid_BtnGridDocumentMovement },
            'viewDocPurchesEdit menuitem#btnGridDocSales': { click: this.onGrid_BtnGridDocSales },

            // Клик по Гриду
            'viewDocPurchesEdit [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },





            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            'viewDocPurchesEdit button#btnHeldCancel': { click: this.onBtnHeldCancelClick },
            'viewDocPurchesEdit button#btnHelds': { click: this.onBtnHeldsClick },
            'viewDocPurchesEdit menuitem#btnSave': { click: this.onBtnSaveClick },
            'viewDocPurchesEdit menuitem#btnSaveClose': { click: this.onBtnSaveClick },
            'viewDocPurchesEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocPurchesEdit button#btnHelp': { "click": this.onBtnHelpClick },
            //***
            'viewDocPurchesEdit menuitem#btnPrintHtml': { click: this.onBtnPrintHtmlClick },
            'viewDocPurchesEdit menuitem#btnPrintExcel': { click: this.onBtnPrintHtmlClick },

            'viewDocPurchesEdit menuitem#btnPrint_barcode': { "click": this.onBtnPrintClick },
            'viewDocPurchesEdit menuitem#btnPrint_barcode_price': { "click": this.onBtnPrintClick },
            'viewDocPurchesEdit menuitem#btnPrint_barcode_name': { "click": this.onBtnPrintClick },

            'viewDocPurchesEdit menuitem#btnPrint_Qr': { "click": this.onBtnPrintClick },
            'viewDocPurchesEdit menuitem#btnPrint_Qr_price': { "click": this.onBtnPrintClick },
            'viewDocPurchesEdit menuitem#btnPrint_Qr_name': { "click": this.onBtnPrintClick },

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
        controllerDocPurchesEdit_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDocPurchesEdit_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDocPurchesEdit_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDocPurchesEdit_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDocPurchesEdit_onTree_folderDel(aButton.UO_id);
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
        //storeGrid.proxy.url = HTTP_RemParties + "?DirNomenID=" + rec.get('id') + "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + id).getValue() + "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d");
        storeGrid.proxy.url = HTTP_RemParties + "?DirNomenID=" + rec.get('id') + "&DirContractorIDOrg=" + Ext.getCmp("DirContractorIDOrg" + id).getValue() + "&DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d");
        storeGrid.load();
        Ext.getCmp("tree_" + id).setDisabled(false);
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
        controllerDocPurchesEdit_RecalculationSums(combo.UO_id, false);
    },
    //Скидка
    onPaymentChange: function (aTextfield, aText) {
        controllerDocPurchesEdit_RecalculationSums(aTextfield.UO_id, false)
    },



    //PanelDocumentDetails === === === === ===

    //Скидка
    onDiscount: function (aTextfield, aText) {
        controllerDocPurchesEdit_RecalculationSums(aTextfield.UO_id, false)
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
                    funSpreadDiscount(aButton.UO_id, ArrPrice, controllerDocPurchesEdit_RecalculationSums);
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
                    funSpreadSummOther(aButton.UO_id, ArrPrice, controllerDocPurchesEdit_RecalculationSums);
                }
            },
            icon: Ext.window.MessageBox.QUESTION
        });
    },



    //Грид (itemId=grid) === === === === ===

    //Новая: Добавить позицию
    onGrid_BtnGridAddPosition: function (aButton, aEvent, aOptions) {

        //Надо вначале сохранить документ, а потом создавать новые позиции
        if (!(parseInt(Ext.getCmp("DocID" + aButton.UO_id).getValue()) > 0)) { Ext.Msg.alert(lanOrgName, "Сначала сохраните новый документ!"); return; }

        var id = aButton.UO_id
        var node = funReturnNode(id);

        //Выбран ли товар
        if (node == null) { Ext.Msg.alert(lanOrgName, "Не выбран товар! Выберите товар в списке слева!"); return; }


        var Params = [
            "grid_" + aButton.UO_id,
            true, //UO_Center
            true, //UO_Modal
            2,    // 2 - Редактировать и Новое (1 - Новое, )
            true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            Ext.getCmp("tree_" + aButton.UO_id).getSelectionModel().getSelection()[0], //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            false      //GridTree
        ]
        ObjectEditConfig("viewDocPurchTabsEdit", Params);
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
        ObjectEditConfig("viewDocPurchTabsEdit", Params);
    },

    //Новая: Удалить позицию
    onGrid_BtnGridDelete: function (aButton, aEvent, aOptions) {
        
        /*Ext.MessageBox.show({
            title: lanOrgName,
            msg: lanDelete + "?",
            icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) {
                if (buttons == "yes") {
                    var selection = Ext.getCmp("grid_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
                    if (selection) { Ext.getCmp("grid_" + aButton.UO_id).store.remove(selection); }
                }
            }
        });*/


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
                    var DocPurchTabID = Ext.getCmp("grid_" + aButton.UO_id).view.getSelectionModel().getSelection()[0].data.DocPurchTabID;
                    //Запрос на удаление
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_DocPurchTabs + DocPurchTabID + "/",
                        method: 'DELETE',
                        success: function (result) {
                            loadingMask.hide();
                            var sData = Ext.decode(result.responseText);
                            if (sData.success == true) {
                                Ext.MessageBox.show({ title: lanOrgName, msg: sData.data.Msg, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.OK })

                                //Ext.getCmp("grid_" + aButton.UO_id).view.store.load();
                                var locStore = Ext.getCmp("grid_" + aButton.UO_id).view.store;
                                locStore.load({ waitMsg: lanLoading });
                                locStore.on('load', function () {
                                    //controllerDocPurchesEdit_RecalculationSums(Ext.getCmp(aButton.UO_idCall).UO_id, false);
                                    controllerDocPurchesEdit_RecalculationSums(aButton.UO_id, false);
                                });

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

    onGrid_BtnGridPayment: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id,
            true, //UO_Center
            true, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewPay", Params);
    },


    onGrid_btnCreateDocMovements: function (aButton, aEvent, aOptions) {
        var id = aButton.UO_id;


        //Если на резерве - вывести сообщение
        //var _msg = "";
        //if (Ext.getCmp("Reserve" + id).getValue() == true) _msg += "<BR>" + lanAttention + "<BR>" + txtMsg049_1 + txtMsg049_2 + "<BR>";

        Ext.MessageBox.show({
            title: lanOrgName, msg: lanOnBasisOfDoc + " создать 'Перемещение'" + "?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false, // + _msg
            fn: function (buttons) {
                if (buttons == "yes") {

                    //1. Данные формы
                    var modelDocMovementsGrid = Ext.create('PartionnyAccount.model.Sklad/Object/Doc/DocMovements/modelDocMovementsGrid');
                    modelDocMovementsGrid.data = Ext.getCmp("form_" + id).getForm().getFieldValues(); //form.loadRecord(formRec); //Вот так надо загружать данные в Форму
                    //2. Данные Грида (эти данные загружаются в модель или в сторе)
                    var gridRec = Ext.getCmp("grid_" + id).getStore(); //.getSelectionModel(); //.getSelection(); //store.insert(widgetXForm.UO_GridIndex, gridRec); //Вот так надо загружать данные в Грид
                    //3. Параметр содержащий данные Формы и Грида
                    var ArrList = [modelDocMovementsGrid, gridRec];
                    //4. Создание Р.Н.
                    var Params = [
                        "grid_" + id, //UO_idCall (в "ObjectEditConfig" UO_idCall = undefined)
                        false, //UO_Center
                        false, //UO_Modal
                        3,     // 1 - Новое, 2 - Редактировать, 3 - Копия
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        undefined,
                        ArrList
                    ]
                    ObjectEditConfig("viewDocMovementsEdit", Params);

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
                ObjectEditConfig("viewDocPurchTabsEdit", Params);
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
            ObjectEditConfig("viewDocPurchTabsEdit", Params);
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
            fn: function (buttons) { if (buttons == "yes") { controllerDocPurchesEdit_onBtnSaveClick(aButton); } } //, aEvent, aOptions
        });
    },

    //Провести
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        if (Ext.getCmp('HavePay' + aButton.UO_id).getValue() != 0) {
            Ext.MessageBox.show({
                title: lanOrgName, msg: txtMsg026, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                fn: function (buttons) {
                    if (buttons == "yes") {
                        controllerDocPurchesEdit_onBtnSaveClick(aButton); //, aEvent, aOptions
                    }
                }
            });
        }
        else {
            controllerDocPurchesEdit_onBtnSaveClick(aButton); //, aEvent, aOptions
        }
    },

    //Сохранить или Сохранить и закрыть
    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        controllerDocPurchesEdit_onBtnSaveClick(aButton); //, aEvent, aOptions

    },
    //Отменить
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    //Help
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-priyomka/", '_blank');
    },
    //***
    //Распечатать
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {
        //aButton.UO_Action: html, excel
        //alert(aButton.UO_Action);

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocPurchID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

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
            6,
        ]
        ObjectConfig("viewListObjectPFs", Params);

    },

    onBtnPrintClick: function (aButton, aEvent, aOptions) {

        var mapForm = document.createElement("form");
        mapForm.target = "Map";
        mapForm.method = "GET"; // or "post" if appropriate
        mapForm.action = "../report/report/";

        //var UO_id = Ext.getCmp(aButton.UO_idCall).UO_id;

        //Параметр "pID"
        var mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "pID"; mapInput.value = "DocPurchesPrintCode"; mapForm.appendChild(mapInput);

        //Параметр "DirNomenID"
        mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "DocID"; mapInput.value = Ext.getCmp("DocID" + aButton.UO_id).getValue(); mapForm.appendChild(mapInput);

        //Параметр "UO_Action"
        mapInput = document.createElement("input"); mapInput.type = "text";
        mapInput.name = "UO_Action"; mapInput.value = aButton.UO_Action; mapForm.appendChild(mapInput);


        document.body.appendChild(mapForm);
        map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");

        if (map) { mapForm.submit(); }
        else { alert('You must allow popups for this map to work.'); }

    },

});


//Функия сохранения
function controllerDocPurchesEdit_onBtnSaveClick(aButton) { //, aEvent, aOptions
    
    //Спецификация (табличная часть)
    var recordsDocPurchTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocPurchTab.push(rec.data); });

    //Проверка
    if (Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Контрагента!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    if (Ext.getCmp("DirVatValue" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Налог!"); Ext.getCmp("DirVatValue" + aButton.UO_id).setValue(0); return; }
    //if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocPurches + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocPurchID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocPurches + "?id=" + parseInt(Ext.getCmp("DocPurchID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocPurchTab: Ext.encode(recordsDocPurchTab) },

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
                Ext.getCmp('DocPurchID' + aButton.UO_id).setValue(sData.DocPurchID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocPurchID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocPurchID);

                Ext.getCmp('viewDocPurchesEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocPurchesEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocPurchID" + aButton.UO_id).getValue() + ")");
                Ext.getCmp("btnPrint" + aButton.UO_id).setVisible(true);
            }

            //Кнопку "Платежи - делаем активной"
            Ext.getCmp("btnGridPayment" + aButton.UO_id).enable();

            //Закрыть
            if (aButton.UO_Action == "save_close" || aButton.UO_Action == "held") { Ext.getCmp(aButton.UO_idMain).close(); }
            else {
                //Обновляем записи в гриде
                /*var locStore = Ext.getCmp(aButton.UO_idMain).storeGrid;
                locStore.load({ waitMsg: lanLoading });
                locStore.on('load', function () {
                    controllerDocPurchesEdit_RecalculationSums(Ext.getCmp(aButton.UO_idMain).UO_id, false);
                });*/
            }

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
function controllerDocPurchesEdit_RecalculationSums(id, ShowMsg) { 

    fun_DocX_RecalculationSums(id, ShowMsg);
    return;

    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocPurchTabsGrid; //можно так: Ext.getCmp("viewDocPurchesEdit" + id).storeDocPurchTabsGrid;
    //Скидка, если undefined, isNaN или "", то ставим её == 0
    if (isNaN(parseFloat(Ext.getCmp("Discount" + id).getValue()))) { Ext.getCmp("Discount" + id).setValue(0); }
    //Ставка Налога
    var DirVatValue = 0;
    var Match = true; //совпадение ставок НДС для всех Номенклатур.
    var DirVatValue = parseFloat(Ext.getCmp("DirVatValue" + id).getValue());

    var SumOfVATCurrency = 0; //var SumVATCurrency = 0;
    for (var i = 0; i < storeGrid.data.items.length; i++) {
        //Номенклатура + проверка на совпадение ставок НДС для всех Номенклатур.
        //Сумма спецификации
        var pSumOfVATCurrency = parseFloat(storeGrid.data.items[i].data.Quantity * storeGrid.data.items[i].data.PriceCurrency);
        //Меняем сумму спецификации (если добавили новую позицию)
        storeGrid.data.items[i].data.SUMPurchPriceVATCurrency = pSumOfVATCurrency.toFixed(varFractionalPartInSum);

        SumOfVATCurrency += parseFloat(pSumOfVATCurrency);
        //SumVATCurrency += parseFloat((parseFloat(pSumOfVATCurrency) - parseFloat(pSumOfVATCurrency) / (1 + (DirVatValue) / 100)));
    }

    //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)
    Ext.getCmp("grid_" + id).getView().refresh();
    //Сумма с НДС
    SumOfVATCurrency = parseFloat(parseFloat(SumOfVATCurrency) - parseFloat(parseFloat(SumOfVATCurrency) * parseFloat(Ext.getCmp("Discount" + id).getValue()) / 100));
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum)); //Подсчёт суммы SUM(Учётная цена * К-во)
    //Сумма НДС
    //parseFloat(parseFloat(SumVATCurrency) - parseFloat(parseFloat(SumVATCurrency) * parseFloat(Ext.getCmp("Discount" + id).getValue()) / 100));
    SumVATCurrency = parseFloat((parseFloat(SumOfVATCurrency) - parseFloat(SumOfVATCurrency) / (1 + (DirVatValue) / 100)));
    Ext.getCmp('SumVATCurrency' + id).setValue(SumVATCurrency.toFixed(varFractionalPartInSum));     //Подсчёт суммы SUM(НДС)
    //Надо доплатить
    var HavePay = parseFloat(parseFloat(Ext.getCmp("SumOfVATCurrency" + id).getValue()) - parseFloat(Ext.getCmp("Payment" + id).getValue()));
    Ext.getCmp("HavePay" + id).setValue(HavePay.toFixed(varFractionalPartInSum));

    if (ShowMsg) {
        if (DirVatValue_Collection != "") {
            if (Match) { Ext.Msg.alert(lanOrgName, txtVatChanges + "<b>" + DirVatValue_Collection + " %</b>" + txtVatRecalc); } //Сообщение о пересчёте НДС
            else { Ext.Msg.alert(lanOrgName, txtVatNotMatch + "<BR>" + txtVatChanges + "<b>" + DirVatValue_Collection + " %</b>" + txtVatRecalc); } //Сообщение о пересчёте НДС
        }
    }
};



// === Функции === === ===
//1. Для Товара - КонтекстМеню
function controllerDocPurchesEdit_onTree_folderNew(id) {
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
function controllerDocPurchesEdit_onTree_folderEdit(id) {
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
function controllerDocPurchesEdit_onTree_folderNewSub(id) {
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
function controllerDocPurchesEdit_onTree_folderCopy(id) {
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
function controllerDocPurchesEdit_onTree_folderDel(id) {
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
function controllerDocPurchesEdit_onTree_folderSubNull(id) {
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
function controllerDocPurchesEdit_onTree_addSub(id) {
    
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

