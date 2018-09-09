Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocDomesticExpenses/viewcontrollerDocDomesticExpensesEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocDomesticExpensesEdit',


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewDocDomesticExpensesEditClose: function (aPanel) {
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
            HTTP_DocDomesticExpenseTabs +
            "?DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + textfield.UO_id).getValue(), "Y-m-d") +
            "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + textfield.UO_id).getValue(), "Y-m-d") +
            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + textfield.UO_id).getValue() + 
            "&UO_GridIndex=" + Ext.getCmp("viewDocDomesticExpensesEdit" + textfield.UO_id).UO_GridIndex;

        Ext.getCmp("grid_" + id).store.load();
    },
    onDocDatePoChange: function (textfield, newValue, oldValue) {
        var id = textfield.UO_id;

        Ext.getCmp("grid_" + id).store.proxy.url =
            HTTP_DocDomesticExpenseTabs +
            "?DocDateS=" + Ext.Date.format(Ext.getCmp("DocDateS" + textfield.UO_id).getValue(), "Y-m-d") +
            "&DocDatePo=" + Ext.Date.format(Ext.getCmp("DocDatePo" + textfield.UO_id).getValue(), "Y-m-d") +
            "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + textfield.UO_id).getValue() +
            "&UO_GridIndex=" + Ext.getCmp("viewDocDomesticExpensesEdit" + textfield.UO_id).UO_GridIndex;

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
        //var id = view.grid.UO_id;
        //fun_DirNomenPatchFull_RemParties2(id, rec);
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


    //Грид партии *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGridParty_selectionchange: function (model, records, x1, x2, x3) {
        //if (model.view.ownerGrid.down("#btnGridAddPosition") != null) model.view.ownerGrid.down("#btnGridAddPosition").setDisabled(records.length === 0);
    },




    //Грид (itemId=grid) *** *** *** *** *** *** *** *** ***

    onGrid_BtnGridAddPosition: function (aButton, aEvent, aOptions) {
       
        var id = aButton.UO_id;

        //Выбрана ли статья
        /*
        var node = funReturnNode(id);
        if (node == null) { Ext.Msg.alert(lanOrgName, "Не выбрана статья! Выберите статью хоз.расходов в списке слева!"); return; }
        */

        var Params = [
            "grid_" + id,
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            true, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            undefined, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            undefined, //Ext.getCmp("tree_" + aButton.UO_id).getSelectionModel().getSelection()[0], //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
            id,       //UO_Param_id -  в данном случае это id-шник контрола который вызвал
            undefined,
            Ext.getCmp("viewDocDomesticExpensesEdit" + aButton.UO_id).UO_GridIndex,
            undefined,
            undefined,
            undefined,
            false      //GridTree
        ]
        ObjectEditConfig("viewDocDomesticExpenseTabsEdit", Params);

    },
    onGrid_BtnGridEdit: function (aButton, aEvent, aOptions) {

        //ID-шник
        var id = aButton.UO_id;

        //Продажа или возврат
        var ListObjectID = Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0].data.ListObjectID;

        if (ListObjectID == 56) {
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
                Ext.getCmp("viewDocDomesticExpensesEdit" + aButton.UO_id).UO_GridIndex,
                undefined,
                undefined,
                undefined,
                false
            ]
            ObjectEditConfig("viewDocDomesticExpenseTabsEdit", Params);
        }
        else if (ListObjectID == 57) {
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
            ObjectEditConfig("viewDocDomesticExpenseReturnTabsEdit", Params);
        }

    },
    onGrid_BtnGridDelete: function (aButton, aEvent, aOptions) {

        if (!Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0]) { return; }

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
                    var DocDomesticExpenseID = Ext.getCmp("grid_" + aButton.UO_id).view.getSelectionModel().getSelection()[0].data.DocDomesticExpenseID;
                    //Запрос на удаление
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_DocDomesticExpenses + DocDomesticExpenseID + "/",
                        method: 'DELETE',
                        success: function (result) {
                            loadingMask.hide();
                            var sData = Ext.decode(result.responseText);
                            if (sData.success == true) {
                                Ext.MessageBox.show({ title: lanOrgName, msg: sData.data.Msg, icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.OK })
                                Ext.getCmp("grid_" + aButton.UO_id).view.store.load();
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
    onGrid_BtnGridRefresh: function (aButton, aEvent, aOptions) {
        Ext.getCmp("btnGridRefresh" + aButton.UO_id).disable();

        var store = Ext.getCmp("grid_" + aButton.UO_id).store;
        store.load({ waitMsg: lanLoading });
        store.on('load', function () {
            Ext.getCmp("btnGridRefresh" + aButton.UO_id).enable();
        });
    },
    onGrid_BtnGridAddReturnPosition: function (aButton, aEvent, aOptions) {

        //ID-шник
        var id = aButton.UO_id;

        //Продажа или возврат
        var ListObjectID = Ext.getCmp("grid_" + aButton.UO_id).getSelectionModel().getSelection()[0].data.ListObjectID;

        if (ListObjectID == 56) {

            //Выбрана ли партия
            var IdcallModelData = Ext.getCmp("grid_" + id).getSelectionModel().getSelection();
            if (IdcallModelData.length == 0) {
                Ext.Msg.alert(lanOrgName, "Не выбрана проданная партия! Выберите проданную партию товара в списке снизу!");
                return;
            }

            if (IdcallModelData[0].data.DirWarehouseID != parseInt(Ext.getCmp("DirWarehouseID" + id).getValue())) {
                Ext.Msg.alert(lanOrgName, "Склад проданной партии не соответствует складу документа!<br /> Поменяйте склад документа или кликните ещё раз на выбранном товаре!");
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
            ObjectEditConfig("viewDocDomesticExpenseReturnTabsEdit", Params);

        }

    },
    onGrid_BtnGridActWriteOff: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;

        //Выбран ли товар
        //var node = funReturnNode(id);
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
        ObjectEditConfig("viewDocDomesticExpenseActWriteOffsEdit", Params);

    },

    // Клик по Гриду
    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        if (model.view.ownerGrid.down("#btnGridEdit") != null) model.view.ownerGrid.down("#btnGridEdit").setDisabled(records.length === 0);
        if (model.view.ownerGrid.down("#btnGridDelete") != null) model.view.ownerGrid.down("#btnGridDelete").setDisabled(records.length === 0);
        if (model.view.ownerGrid.down("#btnGridAddReturnPosition") != null) model.view.ownerGrid.down("#btnGridAddReturnPosition").setDisabled(records.length === 0);
    },
    
    
});



// НЕ РАБОТАЕТ - НЕ НУЖНО !!!
//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function viewcontrollerDocDomesticExpensesEdit_RecalculationSums111(id, ShowMsg) {
    var SumQuantity = 0, SumDiscount = 0, SumOfVATCurrency = 0;

    //Стор для "Табличной части"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid; //Ext.getCmp(Ext.getCmp("grid_" + id).UO_idMain).storeGrid;
    for (var i = 0; i < storeGrid.data.items.length; i++) {

        SumQuantity += parseFloat(storeGrid.data.items[i].data.Quantity);
        SumDiscount += parseFloat(storeGrid.data.items[i].data.Discount);
        SumOfVATCurrency += parseFloat(storeGrid.data.items[i].data.Quantity * storeGrid.data.items[i].data.PriceCurrency - storeGrid.data.items[i].data.Discount);
    }

    Ext.getCmp("grid_" + id).getView().refresh(); //Рефреш грида, если изменилась сумма спецификации (если добавили новую позицию)

    Ext.getCmp('SumQuantity' + id).setValue(SumQuantity.toFixed(varFractionalPartInSum));
    Ext.getCmp('SumDiscount' + id).setValue(SumDiscount.toFixed(varFractionalPartInSum));
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum));
    
};

