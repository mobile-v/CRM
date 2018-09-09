Ext.define('PartionnyAccount.viewcontroller.Sklad/Object/Doc/DocRetailReturns/viewcontrollerDocRetailReturnsEdit', {
    extend: 'Ext.app.ViewController',

    alias: 'controller.viewcontrollerDocRetailReturnsEdit',



    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    onViewDocRetailReturnsEditClose: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },


    //Дата
    onDocDateChange: function (textfield, newValue, oldValue) {
        var id = textfield.UO_id;
        Ext.getCmp("gridPartyMinus_" + id).store.proxy.url = HTTP_RemPartyMinuses + "?DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d") + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + id).getValue();
        Ext.getCmp("gridPartyMinus_" + id).store.load();
        //Ext.getCmp("grid_" + id).store.proxy.url = HTTP_DocRetailReturnTabs + "?DocDate=" + Ext.Date.format(Ext.getCmp("DocDate" + id).getValue(), "Y-m-d") + "&DirWarehouseID=" + Ext.getCmp("DirWarehouseID" + id).getValue();
    },



    //Грид (itemId=grid)
    onGrid_BtnGridAddPosition: function (aButton, aEvent, aOptions) {
       
        var id = aButton.UO_id;

        //Выбран ли товар
        //var node = funReturnNode(id);
        //if (node == null) { Ext.Msg.alert(lanOrgName, "Не выбран товар! Выберите товар в списке слева!"); return; }

        //Выбрана ли партия
        var IdcallModelData = Ext.getCmp("gridPartyMinus_" + id).getSelectionModel().getSelection();
        if (IdcallModelData.length == 0) {
            Ext.Msg.alert(lanOrgName, "Не выбрана партия! Выберите партию товара в списке сверху!");
            return;
        }

        //Есть ли Остаток
        //if (Ext.getCmp("gridPartyMinus_" + id).store.data.length == 0) { Ext.Msg.alert(lanOrgName, "Нет остатка товара (партий) на складе!"); return; }

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
        ObjectEditConfig("viewDocRetailReturnTabsEdit", Params);

    },

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
        ObjectEditConfig("viewDocRetailReturnTabsEdit", Params);
    },

    onGrid_BtnGridDelete: function (aButton, aEvent, aOptions) {

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
                    var DocRetailReturnID = Ext.getCmp("grid_" + aButton.UO_id).view.getSelectionModel().getSelection()[0].data.DocRetailReturnID;
                    //Запрос на удаление
                    Ext.Ajax.request({
                        timeout: varTimeOutDefault,
                        url: HTTP_DocRetailReturns + DocRetailReturnID + "/",
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


    //Кнопки редактирования Енеблед
    onGrid_selectionchange: function (model, records) {
        //model.view.ownerGrid.down("#btnGridEdit").setDisabled(records.length === 0);
        if (model.view.ownerGrid.down("#btnGridEdit") != null) model.view.ownerGrid.down("#btnGridEdit").setDisabled(records.length === 0);
        if (model.view.ownerGrid.down("#btnGridDelete") != null) model.view.ownerGrid.down("#btnGridDelete").setDisabled(records.length === 0);
    },
    //Редактирование
    onGrid_edit: function (view, record, item, index, e) { //, index, e

        record.record.data.SUMPriceCurrency = (parseFloat(record.record.data.Quantity) * parseFloat(record.record.data.PriceCurrency)).toFixed(varFractionalPartInSum);
        record.record.commit()

        //Пересчитываем сумму
        controllerDocRetailReturnsEdit_RecalculationSums(view.grid.UO_id, false);
    },

});



//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function viewcontrollerDocRetailReturnsEdit_RecalculationSums(id, ShowMsg) {
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