Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocServicePurches/controllerDocServiceWorkshopHistories", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocServiceWorkshopHistories': { close: this.this_close },


            // PanelGrid0: Список Клик по Гриду
            'viewDocServiceWorkshopHistories [itemId=PanelGrid0_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick,
            },
            

        });
    },
    


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
    },



    // GridX: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***
       
    //Кнопки редактирования Енеблед
    onGridX_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGridX_itemclick: function (view, record, item, index, eventObj) {
        controllerDocServiceWorkshopHistories_onGridX_itemclick(view.grid, false); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGridX_itemdblclick: function (view, record, item, index, e) {
        controllerDocServiceWorkshopHistories_onGridX_itemclick(view.grid.UO_id, false);
    },

});


//Клик по ГридамX
function controllerDocServiceWorkshopHistories_onGridX_itemclick(view_grid, btnSave) {
    
    var id = view_grid.UO_id;
    var itemId = view_grid.itemId;



    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
    if (Ext.getCmp(itemId + id) == undefined) return;
    var IdcallModelData = Ext.getCmp(itemId + id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

    //Если эта запись уже открыта на редактирования, то повторно её не открывать. Иначе не сможем редактировать "Дату Готовности"
    if (Ext.getCmp("form_" + id).isVisible() && IdcallModelData.DocID == Ext.getCmp("DocID" + id).getValue()) return;

    //Если запись помечена на удаление, то сообщить об этом и выйти
    if (IdcallModelData.Del == true) {
        //Разблокировка вызвавшего окна
        ObjectEditConfig_UO_idCall_true_false(false);

        Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
        return;
    }



    //1. Делаем всё видимым и редактируемым!
    Ext.getCmp("SumDocServicePurch1Tabs" + id).setVisible(false);
    Ext.getCmp("SumDocServicePurch2Tabs" + id).setVisible(false);
    Ext.getCmp("SumTotal" + id).setVisible(false);
    Ext.getCmp("PrepaymentSum" + id).setVisible(false);
    Ext.getCmp("SumTotal2a" + id).setVisible(false);
    Ext.getCmp("btnSave" + id).setVisible(false);

    Ext.getCmp("grid1_" + id).enable();
    Ext.getCmp("grid2_" + id).enable();

    Ext.getCmp("btnStatus2" + id).setText(""); Ext.getCmp("btnStatus2" + id).width = 50; Ext.getCmp("btnStatus2" + id).setPressed(false);
    Ext.getCmp("btnStatus3" + id).setVisible(true);
    Ext.getCmp("btnStatus5" + id).setVisible(true);
    Ext.getCmp("btnStatus4" + id).setVisible(true);
    Ext.getCmp("btnStatus7" + id).setVisible(true);
    Ext.getCmp("btnStatus6" + id).setVisible(true);
    Ext.getCmp("btnStatus8" + id).setVisible(true);

    Ext.getCmp("ServiceTypeRepair" + id).enable();
    Ext.getCmp("gridLog0_" + id).enable();


    //2. Делаем не видимым и не редактируемым!
    if (btnSave) {
        Ext.getCmp("SumDocServicePurch1Tabs" + id).setVisible(true);
        Ext.getCmp("SumDocServicePurch2Tabs" + id).setVisible(true);
        Ext.getCmp("SumTotal" + id).setVisible(true);
        Ext.getCmp("PrepaymentSum" + id).setVisible(true);
        Ext.getCmp("SumTotal2a" + id).setVisible(true);
        Ext.getCmp("btnSave" + id).setVisible(true);

        Ext.getCmp("grid1_" + id).disable();
        Ext.getCmp("grid2_" + id).disable();

        Ext.getCmp("btnStatus2" + id).setText("В диагностике"); Ext.getCmp("btnStatus2" + id).width = 125; //Ext.getCmp("btnStatus2" + id).setVisible(false);
        Ext.getCmp("btnStatus3" + id).setVisible(false);
        Ext.getCmp("btnStatus5" + id).setVisible(false);
        Ext.getCmp("btnStatus4" + id).setVisible(false);
        Ext.getCmp("btnStatus7" + id).setVisible(false);
        Ext.getCmp("btnStatus6" + id).setVisible(false);
        Ext.getCmp("btnStatus8" + id).setVisible(false);

        //Если Архив
        if (IdcallModelData.DirServiceStatusID == 9) {
            Ext.getCmp("ServiceTypeRepair" + id).disable();
            Ext.getCmp("gridLog0_" + id).disable();

            Ext.getCmp("btnStatus2" + id).setText("<b>Вернуть на доработку</b>"); Ext.getCmp("btnStatus2" + id).width = 200;
            Ext.getCmp("btnSave" + id).setVisible(false);
        }
    }


    //Меняем формат датв, а то глючит!
    Ext.getCmp("DocDate" + id).format = "c";


    var widgetX = Ext.getCmp("viewDocServiceWorkshopHistories" + id);

    //Выполненная работа
    widgetX.storeDocServicePurch1TabsGrid.setData([], false);
    widgetX.storeDocServicePurch1TabsGrid.proxy.url = HTTP_DocServicePurch1Tabs + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
    widgetX.storeDocServicePurch1TabsGrid.UO_Loaded = false;
    //Запчасть
    widgetX.storeDocServicePurch2TabsGrid.setData([], false);
    widgetX.storeDocServicePurch2TabsGrid.proxy.url = HTTP_DocServicePurch2Tabs + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
    widgetX.storeDocServicePurch2TabsGrid.UO_Loaded = false;
    //Лог
    widgetX.storeLogServicesGrid.setData([], false);
    widgetX.storeLogServicesGrid.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
    widgetX.storeLogServicesGrid.UO_Loaded = false;


    //Форма
    var widgetXForm = Ext.getCmp("form_" + id);
    widgetXForm.form.url = HTTP_DocServicePurches + IdcallModelData.DocServicePurchID + "/?DocID=" + IdcallModelData.DocID; //С*ка глючит фреймворк и присвивает в форме старый УРЛ!!!
    widgetXForm.setVisible(true);
    widgetXForm.reset();
    widgetXForm.UO_Loaded = false;

    
    //Лоадер
    var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
    loadingMask.show();

    widgetX.storeDocServicePurch1TabsGrid.load({ waitMsg: lanLoading });
    widgetX.storeDocServicePurch1TabsGrid.on('load', function () {
        if (widgetX.storeDocServicePurch1TabsGrid.UO_Loaded) { loadingMask.hide(); return; }
        widgetX.storeDocServicePurch1TabsGrid.UO_Loaded = true;

        widgetX.storeDocServicePurch2TabsGrid.load({ waitMsg: lanLoading });
        widgetX.storeDocServicePurch2TabsGrid.on('load', function () {
            if (widgetX.storeDocServicePurch2TabsGrid.UO_Loaded) { loadingMask.hide(); return; }
            widgetX.storeDocServicePurch2TabsGrid.UO_Loaded = true;

            if (widgetXForm.UO_Loaded) { loadingMask.hide(); return; }

            loadingMask.hide();

            widgetXForm.load({
                method: "GET",
                timeout: varTimeOutDefault,
                waitMsg: lanLoading,
                //url: HTTP_DocServicePurches + IdcallModelData.DocServicePurchID + "/?DocID=" + IdcallModelData.DocID,
                success: function (form, action) {

                    //Статусы и Кнопки
                    controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeButton(id);

                    //Меняем статус в самой таблице
                    if (IdcallModelData.DirServiceStatusID == 1) { //if (parseInt(Ext.getCmp("DirServiceStatusID" + id).getValue()) == 1) {
                        //Меняем статус
                        var storeX = Ext.getCmp(itemId + id).getSelectionModel().getSelection();
                        storeX[0].data.DirServiceStatusID = 2;
                        //Сохраняем
                        Ext.getCmp(itemId + id).getView().refresh();
                    }

                    widgetXForm.UO_Loaded = true;
                    widgetX.focus(); //Фокус на открывшийся Виджет

                    //Log
                    widgetX.storeLogServicesGrid.load({ waitMsg: lanLoading });
                },
                failure: function (form, action) {
                    funPanelSubmitFailure(form, action);
                    widgetX.focus(); //Фокус на открывшийся Виджет
                }

            });
        });

    });

}

//Смена Статуса
function controllerDocServiceWorkshopHistories_ChangeStatus_Request(aButton, DirPaymentTypeID) {
    if (DirPaymentTypeID == undefined) DirPaymentTypeID = 0;

    //Старый ID-шние статуса
    var locDirServiceStatusID_OLD = parseInt(Ext.getCmp("DirServiceStatusID" + aButton.UO_id).getValue());

    //Новый ID-шние статуса
    var locDirServiceStatusID = parseInt(controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, false));
    if (isNaN(locDirServiceStatusID)) { return; }

    //Запрос на сервер на смену статуса
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocServicePurches + Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue() + "/" + locDirServiceStatusID + "/?DirPaymentTypeID=" + DirPaymentTypeID + "&SumTotal2a=" + Ext.getCmp("SumTotal2a" + aButton.UO_id).getValue(),
        method: 'PUT',

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeButton(aButton.UO_id);
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Меняем ID-шние статуса
                controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, true);

                //Статусы и Кнопки
                controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeButton(aButton.UO_id);

                //Сообщение
                if (locDirServiceStatusID == 9) {
                    Ext.Msg.alert(lanOrgName, "Аппарат выдан и перемещён в архив");
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    Ext.getCmp("PanelGrid7_" + aButton.UO_id).getStore().load();


                    // *** Печатніе формы ***

                    //Проверка: если форма ещё не сохранена, то выход
                    if (Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

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
                        40
                    ]
                    ObjectConfig("viewListObjectPFs", Params);
                }
                //SMS
                else if (locDirServiceStatusID == 7) {
                    controllerDocServiceWorkshopHistories_SenSMS(aButton.UO_id, 2, 2);
                }
                else if (locDirServiceStatusID == 8) {
                    controllerDocServiceWorkshopHistories_SenSMS(aButton.UO_id, 3, 3);
                }
                else if (locDirServiceStatusID == 2 && locDirServiceStatusID_OLD == 9) {
                    //Обновить Грид "Архив"
                    Ext.getCmp("PanelGrid9_" + aButton.UO_id).getStore().load();
                    //Закрыть форму редактирование
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    return;
                }
                else if (locDirServiceStatusID == 2 && locDirServiceStatusID_OLD == 7) {
                    //Обновить Грид "Архив"
                    Ext.getCmp("PanelGrid7_" + aButton.UO_id).getStore().load();
                    //Закрыть форму редактирование
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    return;
                }

                //Обновить Лог
                Ext.getCmp("gridLog0_" + aButton.UO_id).getStore().load();
            }
        },
        failure: function (result) {
            controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeButton(aButton.UO_id);

            var sData = Ext.decode(result.responseText);
            Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
        }
    });
}
//Статусы и Кнопки - выставить
function controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeButton(id)
{
    switch (parseInt(Ext.getCmp("DirServiceStatusID" + id).getValue())) {
        case 1:
            //Принят
            Ext.Msg.alert(lanOrgName, "Статус сменён на: В диагностике");

            Ext.getCmp("btnStatus2" + id).setPressed(true);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);

            break;
        case 2:
            //В диагностике
            Ext.getCmp("btnStatus2" + id).setPressed(true);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
        case 3:
            //На согласовании
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(true);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
        case 4:
            //Согласован
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(true); 
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
        case 5:
            //Ожидание запчастей
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(true);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
        case 7:
            //Отремонтирован
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(true);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
        case 6:
            //В основном сервисе
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(true);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
        case 8:
            //Отказной
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(true);
            break;
    }
}
//Вернуть и/или поменять "DirServiceStatusID"
function controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeStatus(id, itemId, bchange) {
    switch (itemId) {
        case "btnStatus2":
            if (bchange) { Ext.getCmp("DirServiceStatusID" + id).setValue(2); }
            else { return 2; }
            break;
        case "btnStatus3":
            if (bchange) { Ext.getCmp("DirServiceStatusID" + id).setValue(3); }
            else { return 3; }
            break;
        case "btnStatus4":
            if (bchange) { Ext.getCmp("DirServiceStatusID" + id).setValue(4); }
            else { return 4; }
            break;
        case "btnStatus5":
            if (bchange) { Ext.getCmp("DirServiceStatusID" + id).setValue(5); }
            else { return 5; }
            break;
        case "btnStatus7":
            //Если нет ни одной выполненной работы, то не пускать сохранять и выдать эксепшн
            if (Ext.getCmp("grid1_" + id).getStore().data.length == undefined) { Ext.Msg.alert(lanOrgName, "Для статуса готов, должна присутствовать в списке работ, хотя бы одна выполненная работа!"); controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeButton(id); return; }
            if (bchange) { Ext.getCmp("DirServiceStatusID" + id).setValue(7); }
            else { return 7; }
            break;
        case "btnStatus6":
            if (bchange) { Ext.getCmp("DirServiceStatusID" + id).setValue(6); }
            else { return 6; }
            break;
        case "btnStatus8":
            if (bchange) { Ext.getCmp("DirServiceStatusID" + id).setValue(8); }
            else { return 8; }
            break;

        case "btnSave":
            return 9;
            break;
    }
}

//Результат диагностики
function controllerDocServiceWorkshopHistories_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, sDiagnosticRresults) {
    //Менять статус на Согласовано или Не Согласовано
    Ext.MessageBox.show({
        icon: Ext.MessageBox.QUESTION,
        width: 300,
        title: lanOrgName,
        msg: 'Поменять статус на: ',
        buttonText: { yes: "Согласовано", no: "На согласовании", cancel: "Не менять" },
        fn: function (btn) {
            if (btn == "yes") {
                //Запрос на сервер - сохранить выполненную работу
                controllerDocServiceWorkshopHistories_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, 4, sDiagnosticRresults);
            }
            else if (btn == "no") {
                //Запрос на сервер - сохранить выполненную работу
                controllerDocServiceWorkshopHistories_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, 3, sDiagnosticRresults);
            }
            else if (btn == "cancel") {
                //Запрос на сервер - сохранить выполненную работу
                controllerDocServiceWorkshopHistories_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, Ext.getCmp("DirServiceStatusID" + idMy).getValue(), sDiagnosticRresults);
            }
        }
    });
}
//Эти 2-е функции для сохранения "Выполненных работ" с запросом на сервер
function controllerDocServiceWorkshopHistories_onGrid1Edit(UO_id, aE1, pDirServiceStatusID) {

    aE1.record.data.DocServicePurchID = Ext.getCmp("DocServicePurchID" + UO_id).getValue(); //aEditor.grid.UO_id
    var dataX = Ext.encode(aE1.record.data);
    //var ddd = ffff;
    //Сохранение
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocServicePurch1Tabs + "?DirServiceStatusID=" + pDirServiceStatusID,
        method: 'POST',
        params: { recordsDataX: dataX },

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Получаем данные с Сервера
                var locDocServicePurch1TabID = sData.data.DocServicePurch1TabID;
                var DirEmployeeID = sData.data.DirEmployeeID;
                var DirCurrencyID = sData.data.DirCurrencyID;
                var DirCurrencyRate = sData.data.DirCurrencyRate;
                var DirCurrencyMultiplicity = sData.data.DirCurrencyMultiplicity;

                //Переменные
                var grid = Ext.getCmp("grid1_" + UO_id); //var grid = aEditor.grid;
                var gridStore = grid.store;

                //UO + меняем значение в "UO_GridRecord"
                var UO_GridIndex = gridStore.indexOf(grid.getSelectionModel().getSelection()[0]); //UO_GridIndex: Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                var UO_GridRecord = grid.getSelectionModel().getSelection()[0]; //UO_GridRecord: Для загрузки данных в форму редактирования Табличной части
                UO_GridRecord.data.DocServicePurch1TabID = locDocServicePurch1TabID
                UO_GridRecord.data.DirEmployeeID = DirEmployeeID
                UO_GridRecord.data.DirCurrencyID = DirCurrencyID
                UO_GridRecord.data.DirCurrencyRate = DirCurrencyRate
                UO_GridRecord.data.DirCurrencyMultiplicity = DirCurrencyMultiplicity

                //Меняем значение
                gridStore.remove(UO_GridRecord);
                gridStore.insert(UO_GridIndex, UO_GridRecord);
                //Отобразить в Гриде
                grid.getView().refresh();

                //Обновить Лог
                Ext.getCmp("gridLog0_" + grid.UO_id).getStore().load();


                //Меняем кнопку на "pDirServiceStatusID" *** *** *** *** *** *** *** *** *** ***
                Ext.getCmp("DirServiceStatusID" + grid.UO_id).setValue(pDirServiceStatusID);
                controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeButton(grid.UO_id);
            }
        },
        failure: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data);
            }
        }
    });

};
function controllerDocServiceWorkshopHistories_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, pDirServiceStatusID, sDiagnosticRresults) {

    rec.data.DirEmployeeName = lanDirEmployeeName;

    //Получаем тип цены *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

    switch (DirPriceTypeID) {
        case 1:
            {
                rec.data.PriceVAT = rec.data.PriceRetailVAT;
                rec.data.PriceCurrency = rec.data.PriceRetailCurrency;
                rec.data.DirCurrencyID = rec.data.DirCurrencyID;
                rec.data.DirCurrencyRate = rec.data.DirCurrencyRate;
                rec.data.DirCurrencyMultiplicity = rec.data.DirCurrencyMultiplicity;
            }
            break;
        case 2:
            {
                rec.data.PriceVAT = rec.data.PriceRetailVAT;
                rec.data.PriceCurrency = rec.data.PriceWholesaleCurrency;
                rec.data.DirCurrencyID = rec.data.DirCurrencyID;
                rec.data.DirCurrencyRate = rec.data.DirCurrencyRate;
                rec.data.DirCurrencyMultiplicity = rec.data.DirCurrencyMultiplicity;
            }
            break;
        case 3:
            {
                rec.data.PriceVAT = rec.data.PriceRetailVAT;
                rec.data.PriceCurrency = rec.data.PriceIMCurrency;
                rec.data.DirCurrencyID = rec.data.DirCurrencyID;
                rec.data.DirCurrencyRate = rec.data.DirCurrencyRate;
                rec.data.DirCurrencyMultiplicity = rec.data.DirCurrencyMultiplicity;
            }
            break;
    }

    var store = Ext.getCmp("grid1_" + idMy).getStore();
    store.insert(store.data.items.length, rec.data);

    controllerDocServiceWorkshopHistories_RecalculationSums(idMy);


    //Запрос на сервер *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

    rec.data.DocServicePurchID = Ext.getCmp("DocServicePurchID" + idMy).getValue();
    var dataX = Ext.encode(rec.data);
    //Сохранение
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocServicePurch1Tabs + "?DirServiceStatusID=" + pDirServiceStatusID + "&sDiagnosticRresults=" + sDiagnosticRresults,
        method: 'POST',
        params: { recordsDataX: dataX },

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Получаем данные с Сервера
                var locDocServicePurch1TabID = sData.data.DocServicePurch1TabID;
                var DirEmployeeID = sData.data.DirEmployeeID;
                var DirCurrencyID = sData.data.DirCurrencyID;
                var DirCurrencyRate = sData.data.DirCurrencyRate;
                var DirCurrencyMultiplicity = sData.data.DirCurrencyMultiplicity;

                //Переменные
                var grid = Ext.getCmp("grid1_" + idMy);
                var gridStore = grid.store;

                //UO + меняем значение в "UO_GridRecord"
                var UO_GridIndex = store.data.items.length - 1; //gridStore.indexOf(grid.getSelectionModel().getSelection()[0]); //UO_GridIndex: Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                var UO_GridRecord = rec; //grid.getSelectionModel().getSelection()[0]; //UO_GridRecord: Для загрузки данных в форму редактирования Табличной части
                UO_GridRecord.data.DocServicePurch1TabID = locDocServicePurch1TabID
                UO_GridRecord.data.DirEmployeeID = DirEmployeeID
                UO_GridRecord.data.DirCurrencyID = DirCurrencyID
                UO_GridRecord.data.DirCurrencyRate = DirCurrencyRate
                UO_GridRecord.data.DirCurrencyMultiplicity = DirCurrencyMultiplicity

                //Меняем значение
                gridStore.remove(UO_GridRecord);
                gridStore.insert(UO_GridIndex, UO_GridRecord);
                //Отобразить в Гриде
                grid.getView().refresh();

                //Обновить Лог
                Ext.getCmp("gridLog0_" + grid.UO_id).getStore().load();


                //Меняем кнопку на "pDirServiceStatusID" *** *** *** *** *** *** *** *** *** ***
                Ext.getCmp("DirServiceStatusID" + grid.UO_id).setValue(pDirServiceStatusID);
                controllerDocServiceWorkshopHistories_DirServiceStatusID_ChangeButton(grid.UO_id);
            }
        },
        failure: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data);
            }
        }
    });
};

//Отправка SMS
function controllerDocServiceWorkshopHistories_SenSMS(id, DirSmsTemplateTypeS, DirSmsTemplateTypePo) {
    
    var Params = [
        "gridLog0_" + id, //UO_idCall
        true, //UO_Center
        true, //UO_Modal
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        "DocServicePurchID=" + Ext.getCmp("DocServicePurchID" + id).getValue() + "&DirSmsTemplateTypeS=" + DirSmsTemplateTypeS + "&DirSmsTemplateTypePo=" + DirSmsTemplateTypePo
    ]
    ObjectConfig("viewSms", Params);

}

//Поиск в Архиве
function controllerDocServiceWorkshopHistories_Search_Archiv(id, DirSmsTemplateTypeS, DirSmsTemplateTypePo) {

    if (Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue() == "") return;
    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).disable(); //Кнопку поиска делаем не активной


    var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value;



    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); //Кнопку поиска делаем не активной

}

//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocServiceWorkshopHistories_RecalculationSums(id) {

    //1. Подсчет табличной части Работы "SumDocServicePurch1Tabs"
    //2. Подсчет табличной части Запчасти "SumDocServicePurch2Tabs"
    //3. Сумма 1+2 "SumTotal"
    //4. Константа "PrepaymentSum"
    //5. 3 - 4 "SumTotal2a"


    //1. Подсчет табличной части Работы "SumDocServicePurch1Tabs"
    var storeDocServicePurch1TabsGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeDocServicePurch1TabsGrid;
    var SumDocServicePurch1Tabs = 0;
    for (var i = 0; i < storeDocServicePurch1TabsGrid.data.items.length; i++) {
        SumDocServicePurch1Tabs += parseFloat(storeDocServicePurch1TabsGrid.data.items[i].data.PriceCurrency);
    }
    Ext.getCmp('SumDocServicePurch1Tabs' + id).setValue(SumDocServicePurch1Tabs.toFixed(varFractionalPartInSum));


    //2. Подсчет табличной части Работы "SumDocServicePurch2Tabs"
    var storeDocServicePurch2TabsGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeDocServicePurch2TabsGrid;
    var SumDocServicePurch2Tabs = 0;
    for (var i = 0; i < storeDocServicePurch2TabsGrid.data.items.length; i++) {
        SumDocServicePurch2Tabs += parseFloat(storeDocServicePurch2TabsGrid.data.items[i].data.PriceCurrency);
    }
    Ext.getCmp('SumDocServicePurch2Tabs' + id).setValue(SumDocServicePurch2Tabs.toFixed(varFractionalPartInSum));


    //3. Сумма 1+2 "SumTotal"
    Ext.getCmp('SumTotal' + id).setValue((SumDocServicePurch1Tabs + SumDocServicePurch2Tabs).toFixed(varFractionalPartInSum));


    //4. Константа "PrepaymentSum"
    //...


    //5. 3 - 4 "SumTotal2a"
    Ext.getCmp('SumTotal2a' + id).setValue((SumDocServicePurch1Tabs + SumDocServicePurch2Tabs - parseFloat(Ext.getCmp('PrepaymentSum' + id).getValue())).toFixed(varFractionalPartInSum));

};