Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocSecondHandPurches/viewcontrollerDocSecondHandMovementsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocSecondHandMovementsEdit',


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewDocSecondHandMovementsEditClose: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Заказ
    onBtnOrderClick: function (aButton, aEvent, aOptions) {
        //Откроется форма Заказа
        var Params = [
            "tree_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,     // 1 - Новое, 2 - Редактировать
            undefined,
            2,  //Содержит тип заказа: 1 - Из Мастерской, 2 - Из Магазина
            //IdcallModelData[0], //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
        ]
        ObjectEditConfig("viewDocOrderIntsEdit", Params);
    },


    //Дата
    onDocDateSChange: function (textfield, newValue, oldValue) {
        var id = textfield.UO_id;

        Ext.getCmp("grid_" + id).store.proxy.url =
            HTTP_DocSecondHandMovementTabs +
            "?DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + textfield.UO_id).getValue(), "Y-m-d") +
            "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + textfield.UO_id).getValue(), "Y-m-d") +
            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + textfield.UO_id).getValue();

        Ext.getCmp("grid_" + id).store.load();
    },
    onDocDatePoChange: function (textfield, newValue, oldValue) {
        var id = textfield.UO_id;

        Ext.getCmp("grid_" + id).store.proxy.url =
            HTTP_DocSecondHandMovementTabs +
            "?DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + textfield.UO_id).getValue(), "Y-m-d") +
            "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + textfield.UO_id).getValue(), "Y-m-d") +
            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + textfield.UO_id).getValue();

        Ext.getCmp("grid_" + id).store.load();
    },



    // Группа (itemId=tree) === === === === === === === === === ===

    //Меню Группы
    onTree_collapsible: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).collapse();
    },
    onTree_expandAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).expandAll();
    },
    onTree_collapseAll: function (aButton, aEvent) {
        Ext.getCmp("tree_" + aButton.UO_id).collapseAll();
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        var id = view.grid.UO_id;
        fun_DirNomenPatchFull_Rem2Parties2(id, rec);
    },
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {
    },


    btnDirContractorClear: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirContractorID" + aButton.UO_id).setValue(null);
    },


    //PanelParty
    //Обновить список Товаров
    onBtnDirNomenReloadClick: function (aButton, aEvent, aOptions) {
        //var storeDirNomensTree = Ext.getCmp(aButton.UO_idMain).storeDirNomensTree;
        var storeDirNomensTree = Ext.getCmp("tree_" + aButton.UO_id).store;
        storeDirNomensTree.load();
    },

    //Поиск
    onTriggerSearchTreeClick1: function (aButton, aEvent) {
        
        fun_onTriggerSearchTreeClick_Search_Servise(aButton, false);
    },
    onTriggerSearchTreeClick2: function (f, e) {
        
        if (e.getKey() == e.ENTER) {
            fun_onTriggerSearchTreeClick_Search_Servise(f, false);
        }
    },
    onTriggerSearchTreeClick3: function (e, textReal, textLast) {
        
        if (textReal.length > 2) {
            //funGridDir(e.UO_id, textReal, HTTP_DirNomens);
            //alert("В стадии разработки ...");
        }
    },


    onBtnDirEmployeeIDCourierClick: function (aButton, aEvent, aOptions) {
        Ext.getCmp("DirEmployeeIDCourier" + aButton.UO_id).setValue(null);
    },


    //Грид партии *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGridParty_selectionchange: function (model, records, x1, x2, x3) {
        //if (model.view.ownerGrid.down("#btnGridAddPosition") != null) model.view.ownerGrid.down("#btnGridAddPosition").setDisabled(records.length === 0);
        //if (model.view.ownerGrid.down("#btnGridAddReturnPosition") != null) model.view.ownerGrid.down("#btnGridAddReturnPosition").setDisabled(records.length === 0);
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
        ObjectEditConfig("viewDocSecondHandMovementTabsEdit", Params);
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
        ObjectEditConfig("viewDocSecondHandMovementTabsEdit", Params);
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
                        controllerDocSecondHandMovementsEdit_RecalculationSums(aButton.UO_id, false)
                    }
                }
            }
        });
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
        ObjectEditConfig("viewDocSecondHandMovementTabsEdit", Params);
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

    // Клик по Гриду
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
                ObjectEditConfig("viewDocSecondHandMovementTabsEdit", Params);
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
            ObjectEditConfig("viewDocSecondHandMovementTabsEdit", Params);
        }
        else {
            Ext.getCmp(view.grid.UO_idMain).UO_Function_Grid(Ext.getCmp(view.grid.UO_idCall).UO_id, record);
            Ext.getCmp(view.grid.UO_idMain).close();
        }
    },
    
    


    // Сохранение

    //Провести
    onBtnHeldCancelClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            title: lanOrgName, msg: lanHeldCancel + " ???", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
            fn: function (buttons) { if (buttons == "yes") { controllerDocSecondHandMovementsEdit_onBtnSaveClick(aButton, aEvent, aOptions); } }
        });
    },

    //Провести
    onBtnHeldsClick: function (aButton, aEvent, aOptions) {
        controllerDocSecondHandMovementsEdit_onBtnSaveClick(aButton, aEvent, aOptions);
    },

    //Сохранить или Сохранить и закрыть
    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        controllerDocSecondHandMovementsEdit_onBtnSaveClick(aButton, aEvent, aOptions);

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
        if (Ext.getCmp("DocSecondHandMovementID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

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
function controllerDocSecondHandMovementsEdit_onBtnSaveClick(aButton, aEvent, aOptions) {

    var id = aButton.UO_id;

    //Если склады совпадают выдать сообщение и выход!
    if (parseInt(Ext.getCmp("DirWarehouseIDTo" + id).getValue()) == parseInt(Ext.getCmp("DirWarehouseIDFrom" + id).getValue())) {
        Ext.Msg.alert(lanOrgName, "Склады совпадают!");
        return;
    }


    //Если вызвала "Логистика", то обязательно выбрать курьера
    if (Ext.getCmp("viewDocSecondHandMovementsEdit" + id).UO_GridSave && (Ext.getCmp("DirEmployeeIDCourier" + id).getValue() == "" || Ext.getCmp("DirEmployeeIDCourier" + id).getValue() == null)) {
        Ext.Msg.alert(lanOrgName, "Выбирите Курьера!");
        return;
    }

    //Спецификация (табличная часть)
    var recordsDocSecondHandMovementTab = [];
    var storeGrid = Ext.getCmp("grid_" + id).store;
    storeGrid.data.each(function (rec) { recordsDocSecondHandMovementTab.push(rec.data); });

    //Проверка
    if (Ext.getCmp("DirWarehouseIDFrom" + id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    if (Ext.getCmp("DirWarehouseIDTo" + id).getValue() == "") { Ext.Msg.alert(lanOrgName, "Выбирите Склад!"); return; }
    if (storeGrid.data.length == 0) { Ext.Msg.alert(lanOrgName, "Выбирите Товар!"); return; }

    //Форма на Виджете
    var widgetXForm = Ext.getCmp("form_" + id);

    //Новая или Редактирование
    var sMethod = "POST";
    var sUrl = HTTP_DocSecondHandMovements + "?UO_Action=" + aButton.UO_Action;
    if (parseInt(Ext.getCmp("DocSecondHandMovementID" + id).value) > 0) {
        sMethod = "PUT";
        sUrl = HTTP_DocSecondHandMovements + "?id=" + parseInt(Ext.getCmp("DocSecondHandMovementID" + id).value) + "&UO_Action=" + aButton.UO_Action;
    }

    //Сохранение
    widgetXForm.submit({
        method: sMethod,
        url: sUrl + "&DirEmployeePswd=" + Ext.getCmp("DirEmployeePswd" + aButton.UO_id).value,
        params: { recordsDocSecondHandMovementTab: Ext.encode(recordsDocSecondHandMovementTab) },

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
                Ext.getCmp('DocSecondHandMovementID' + id).setValue(sData.DocSecondHandMovementID);
                Ext.getCmp('NumberInt' + aButton.UO_id).setValue(sData.DocSecondHandMovementID);
                Ext.Msg.alert(lanOrgName, lanDataSaved + "<br />" + txtMsg096 + sData.DocSecondHandMovementID + "<br />" + sMsg);

                Ext.getCmp('viewDocSecondHandMovementsEdit' + aButton.UO_id).setTitle(Ext.getCmp('viewDocSecondHandMovementsEdit' + aButton.UO_id).title + " (" + Ext.getCmp("DocSecondHandMovementID" + aButton.UO_id).getValue() + ")");
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
function viewcontrollerDocSecondHandMovementsEdit_RecalculationSums(id, ShowMsg) {
    var SumOfVATCurrency = 0;
    
    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //Ext.getCmp(Ext.getCmp("grid_" + id).UO_idMain).storeGrid;
    for (var i = 0; i < storeGrid.data.items.length; i++) {
        SumOfVATCurrency += parseFloat(storeGrid.data.items[i].data.Quantity * storeGrid.data.items[i].data.PriceRetailCurrency);
    }

    Ext.getCmp("grid_" + id).getView().refresh(); //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum));
    
};
