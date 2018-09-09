Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocActOnWorks/controllerDocActOnWorksEdit", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocActOnWorksEdit': { close: this.this_close },

            //Сисок (itemId=tree)
            // Меню Списка
            'viewDocActOnWorksEdit [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDocActOnWorksEdit [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDocActOnWorksEdit [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDocActOnWorksEdit [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDocActOnWorksEdit [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDocActOnWorksEdit [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDocActOnWorksEdit [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDocActOnWorksEdit [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick,

                //itemcontextmenu: this.onTree_contextMenuForTreePanel,
            },

            'viewDocActOnWorksEdit dataview': {
                beforedrop: this.onTree_beforedrop,
                drop: this.onTree_drop
            },



            'viewDocActOnWorksEdit button#btnDirNomenReload': { click: this.onBtnDirNomenReloadClick },
            'viewDocActOnWorksEdit #TriggerSearchTree': {
                "ontriggerclick": this.onTriggerSearchTreeClick1,
                "specialkey": this.onTriggerSearchTreeClick2,
                "change": this.onTriggerSearchTreeClick3
            },



            //PanelDocumentDetails
            //Контрагент - Перегрузить
            'viewDocActOnWorksEdit [itemId=DirContractorID]': { select: this.onDirContractorIDSelect },
            'viewDocActOnWorksEdit button#btnDirContractorEdit': { click: this.onBtnDirContractorEditClick },
            'viewDocActOnWorksEdit button#btnDirContractorReload': { click: this.onBtnDirContractorReloadClick },
            //Склад - Перегрузить
            'viewDocActOnWorksEdit [itemId=DirWarehouseID]': { select: this.onDirWarehouseIDSelect },
            'viewDocActOnWorksEdit button#btnDirWarehouseEdit': { click: this.onBtnDirWarehouseEditClick },
            'viewDocActOnWorksEdit button#btnDirWarehouseReload': { click: this.onBtnDirWarehouseReloadClick },

            //PanelDocumentAdditionally
            //Склад - Перегрузить
            'viewDocActOnWorksEdit [itemId=DirVatValue]': { select: this.onDirVatValueSelect },
            'viewDocActOnWorksEdit [itemId=Payment]': { change: this.onPaymentChange },

            //PanelDocumentDiscount
            'viewDocActOnWorksEdit [itemId=Discount]': { change: this.onDiscount },
            'viewDocActOnWorksEdit button#btnSpreadDiscount': { click: this.onBtnSpreadDiscount },
            'viewDocActOnWorksEdit #SummOther': { select: this.onSummOther },
            'viewDocActOnWorksEdit button#btnSpreadSummOther': { click: this.onBtnSpreadSummOther },

            //PanelDocumentPay
            'viewDocActOnWorksEdit [itemId=PayZReport]': { change: this.onPayZReportChecked },

            //Грид (itemId=grid)
            'viewDocActOnWorksEdit button#btnGridAddPosition': { click: this.onGrid_BtnGridAddPosition },
            //'viewDocActOnWorksEdit menuitem#btnGridSelectionOfGoods': { click: this.onGrid_BtnGridSelectionOfGoods },

            'viewDocActOnWorksEdit button#btnGridEdit': { click: this.onGrid_BtnGridEdit },
            'viewDocActOnWorksEdit button#btnGridDelete': { click: this.onGrid_BtnGridDelete },
            'viewDocActOnWorksEdit button#btnGridPayment': { click: this.onGrid_BtnGridPayment },

            'viewDocActOnWorksEdit menuitem#btnGridDocReturnVendor': { click: this.onGrid_BtnGridDocReturnVendor },
            'viewDocActOnWorksEdit menuitem#btnGridDocumentMovement': { click: this.onGrid_BtnGridDocumentMovement },
            'viewDocActOnWorksEdit menuitem#btnGridDocSales': { click: this.onGrid_BtnGridDocSales },

            // Клик по Гриду
            'viewDocActOnWorksEdit [itemId=grid]': {
                selectionchange: this.onGrid_selectionchange,
                itemclick: this.onGrid_itemclick,
                itemdblclick: this.onGrid_itemdblclick
            },





            // === Кнопки: Сохранение, Отмена и Помощь === === ===
            //'viewDocActOnWorksEdit button#btnHeldCancel': { click: this.onBtnHeldCancelClick },
            //'viewDocActOnWorksEdit button#btnHelds': { click: this.onBtnHeldsClick },
            'viewDocActOnWorksEdit menuitem#btnSave': { click: this.onBtnSaveClick },
            'viewDocActOnWorksEdit menuitem#btnSaveClose': { click: this.onBtnSaveClick },
            'viewDocActOnWorksEdit button#btnCancel': { "click": this.onBtnCancelClick },
            'viewDocActOnWorksEdit button#btnHelp': { "click": this.onBtnHelpClick },
            //***
            'viewDocActOnWorksEdit menuitem#btnPrintHtml': { click: this.onBtnPrintHtmlClick },
            'viewDocActOnWorksEdit menuitem#btnPrintExcel': { click: this.onBtnPrintHtmlClick },
        });
    },


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
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
        controllerDocActOnWorksEdit_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDocActOnWorksEdit_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDocActOnWorksEdit_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDocActOnWorksEdit_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDocActOnWorksEdit_onTree_folderDel(aButton.UO_id);
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
    onDirContractorIDSelect: function (combo, records) {

        //Алгоритм
        //Если "Фиксированная скидка", то просто меняем поле "Discount", но перед этим спрашиваем оператора (если скидки разные)
        //Если "Градационная скидка", то делаем запрос на сервер за скидкой и как в первом пункте


        if (records.data.DirDiscountID == null && parseFloat(records.data.DirContractorDiscount) >= 0) {
            //Если "Фиксированная скидка", то просто меняем поле "Discount", но перед этим спрашиваем оператора (если скидки разные)
            //alert("Фиксированная скидка - " + records.data.DirContractorDiscount);

            if (parseFloat(Ext.getCmp("Discount" + combo.UO_id).getValue()) != parseFloat(records.data.DirContractorDiscount)) {
                var Msg = txtChangeDiscount + " " + lanS + " " + Ext.getCmp("Discount" + combo.UO_id).getValue() + " " + lanNa + " " + records.data.DirContractorDiscount + "?";
                var answer = confirm(Msg);
                if (answer) {
                    Ext.getCmp("Discount" + combo.UO_id).setValue(records.data.DirContractorDiscount);
                }
            }
        }
        else {
            //Если "Градационная скидка", то делаем запрос на сервер за скидкой и как в первом пункте
            //alert("Градационная скидка - " + records.data.DirDiscountID);

            var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: Ext.getCmp("viewDocActOnWorksEdit" + combo.UO_id) });
            loadingMask.show();

            Ext.Ajax.request({
                timeout: varTimeOutDefault,
                waitMsg: lanUpload,
                url: HTTP_DirContractors + Ext.getCmp("DirContractorID" + combo.UO_id).getValue() + "/?Discount=1",
                method: 'GET',
                success: function (result) {
                    loadingMask.hide();

                    var sData = Ext.decode(result.responseText);

                    if (parseFloat(Ext.getCmp("Discount" + combo.UO_id).getValue()) != parseFloat(sData.DirContractorDiscount)) {
                        var Msg = txtChangeDiscount + " " + lanS + " " + Ext.getCmp("Discount" + combo.UO_id).getValue() + " " + lanNa + " " + sData.DirContractorDiscount + "?";
                        var answer = confirm(Msg);
                        if (answer) {
                            Ext.getCmp("Discount" + combo.UO_id).setValue(sData.DirContractorDiscount);
                        }
                    }
                },
                failure: function (result) {
                    loadingMask.hide();
                    Ext.Msg.alert(lanOrgName, txtMsg008 + result.responseText);
                }
            });
        }
    },
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
        controllerDocActOnWorksEdit_RecalculationSums(combo.UO_id, false);
    },
    //Скидка
    onPaymentChange: function (aTextfield, aText) {
        controllerDocActOnWorksEdit_RecalculationSums(aTextfield.UO_id, false)
    },



    //PanelDocumentDetails === === === === ===

    //Скидка
    onDiscount: function (aTextfield, aText) {
        controllerDocActOnWorksEdit_RecalculationSums(aTextfield.UO_id, false)
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
                    funSpreadDiscount(aButton.UO_id, ArrPrice, controllerDocActOnWorksEdit_RecalculationSums);
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
                    funSpreadSummOther(aButton.UO_id, ArrPrice, controllerDocActOnWorksEdit_RecalculationSums);
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

        var id = aButton.UO_id
        var node = funReturnNode(id);

        //Выбран ли товар
        //if (node == null) { Ext.Msg.alert(lanOrgName, "Не выбран товар! Выберите товар в списке слева!"); return; }


        var Params = [
            "grid_" + aButton.UO_id,
            true, //UO_Center
            true, //UO_Modal
            2,    // 1 - Новое, 2 - Редактировать
            true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            Ext.getCmp("tree_" + aButton.UO_id).getSelectionModel().getSelection()[0], //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
            id,       //UO_Param_id -  в данном случае это id-шник контрола который вызвал
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            false      //GridTree
        ]
        ObjectEditConfig("viewDocActOnWorkTabsEdit", Params);
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
        ObjectEditConfig("viewDocActOnWorkTabsEdit", Params);
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

    onGrid_BtnGridPayment: function (aButton, aEvent, aOptions) {
        var Params = [
            "grid_" + aButton.UO_id,
            true, //UO_Center
            true, //UO_Modal
            //2     // 1 - Новое, 2 - Редактировать
        ]
        ObjectConfig("viewPay", Params);
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
                ObjectEditConfig("viewDocActOnWorkTabsEdit", Params);
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
            ObjectEditConfig("viewDocActOnWorkTabsEdit", Params);
        }
        else {
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }
    },


    // === Кнопки === === ===

    //Провести
    /*onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { controllerDocActOnWorksEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },*/

    //Провести
    /*onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        if (Ext.getCmp('HavePay' + aButton.UO_id).getValue() != 0) {
            Ext.MessageBox.show({
                title: lanOrgName, msg: txtMsg026, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
                fn: function (buttons) {
                    if (buttons == "yes") {
                        controllerDocActOnWorksEdit_onBtnSaveClick(aButton, aEvent, aOptions);
                    }
                }
            });
        }
        else {
            controllerDocActOnWorksEdit_onBtnSaveClick(aButton, aEvent, aOptions);
        }
    },*/

    //Сохранить или Сохранить и закрыть
    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        controllerDocActOnWorksEdit_onBtnSaveClick(aButton, aEvent, aOptions);

    },
    //Отменить
    onBtnCancelClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp(aButton.UO_idMain).close();
    },
    //Help
    onBtnHelpClick: function (aButton, aEvent, aOptions) {
        window.open(HTTP_Help + "dokument-act-on-works/", '_blank');
    },
    //***
    //Распечатать
    onBtnPrintHtmlClick: function (aButton, aEvent, aOptions) {
        //aButton.UO_Action: html, excel
        //alert(aButton.UO_Action);

        //Проверка: если форма ещё не сохранена, то выход
        if (Ext.getCmp("DocActOnWorkID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

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
            37
        ]
        ObjectConfig("viewListObjectPFs", Params);

    },
});


//Функия сохранения
function controllerDocActOnWorksEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    //Спецификация (табличная часть)
    var recordsDocActOnWorkTab = [];
    var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).store;
    storeGrid.data.each(function (rec) { recordsDocActOnWorkTab.push(rec.data); });

    //Проверка
    if (Ext.getCmp("DirContractorID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Контрагента!"); return; }
    if (Ext.getCmp("DirWarehouseID" + aButton.UO_id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + aButton.UO_id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocActOnWorks + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocActOnWorkID" + aButton.UO_id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocActOnWorks + "?id=" + parseInt(Ext.getCmp("DocActOnWorkID" + aButton.UO_id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl,
        params: { recordsDocActOnWorkTab: Ext.encode(recordsDocActOnWorkTab) },

        timeout: varTimeOutDefault,
        waitMsg: lanUploading,
        success: function (form, action) {

            if (aButton.UO_Action == "held_cancel") {
                Ext.getCmp("Held" + aButton.UO_id).setValue(false);
                //Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(false);
                //Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(true);
                Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(true);
            }
            else if (aButton.UO_Action == "held") {
                Ext.getCmp("Held" + aButton.UO_id).setValue(true);
                //Ext.getCmp("btnHeldCancel" + aButton.UO_id).setVisible(true);
                //Ext.getCmp("btnHelds" + aButton.UO_id).setVisible(false);
                Ext.getCmp("btnRecord" + aButton.UO_id).setVisible(false);
            }


            //Если новая накладная присваиваем полученные номера!
            if (!Ext.getCmp('DocID' + aButton.UO_id).getValue()) {
                var sData = action.result.data;
                Ext.getCmp('DocID' + aButton.UO_id).setValue(sData.DocID);
                Ext.getCmp('DocActOnWorkID' + aButton.UO_id).setValue(sData.DocActOnWorkID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocActOnWorkID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocActOnWorkID);

                Ext.getCmp('viewDocActOnWorksEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocActOnWorksEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocActOnWorkID" + aButton.UO_id).getValue() + ")");
                Ext.getCmp("btnPrint" + aButton.UO_id).setVisible(true);
            }

            //Кнопку "Платежи - делаем активной"
            Ext.getCmp("btnGridPayment" + aButton.UO_id).enable();

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
function controllerDocActOnWorksEdit_RecalculationSums(id, ShowMsg) {

    fun_DocX_RecalculationSums(id, ShowMsg);
    return;

    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //storeDocActOnWorkTabsGrid; //можно так: Ext.getCmp("viewDocActOnWorksEdit" + id).storeDocActOnWorkTabsGrid;
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
function controllerDocActOnWorksEdit_onTree_folderNew(id) {
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
function controllerDocActOnWorksEdit_onTree_folderEdit(id) {
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
function controllerDocActOnWorksEdit_onTree_folderNewSub(id) {
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
function controllerDocActOnWorksEdit_onTree_folderCopy(id) {
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
function controllerDocActOnWorksEdit_onTree_folderDel(id) {
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
                            fun_ReopenTree_1(aButton.UO_id, undefined, "tree_" + id, sData.data);
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
function controllerDocActOnWorksEdit_onTree_folderSubNull(id) {
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
function controllerDocActOnWorksEdit_onTree_addSub(id) {

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