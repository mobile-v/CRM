Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocOrderInts/controllerDocOrderInts", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocOrderInts': { close: this.this_close },


            // PanelGrid0: Список Клик по Гриду
            'viewDocOrderInts [itemId=PanelGrid0_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick,

                edit: this.onPanelGrid0Edit,
            },
            // PanelGrid1: Список Клик по Гриду
            'viewDocOrderInts [itemId=PanelGrid1_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid2: Список Клик по Гриду
            'viewDocOrderInts [itemId=PanelGrid2_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid3: Список Клик по Гриду
            'viewDocOrderInts [itemId=PanelGrid3_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid4: Список Клик по Гриду
            'viewDocOrderInts [itemId=PanelGrid4_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid9: Список Клик по Гриду
            'viewDocOrderInts [itemId=PanelGrid9_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            'viewDocOrderInts #TriggerSearchGrid': {
                "ontriggerclick": this.onTriggerSearchGridClick1,
                "specialkey": this.onTriggerSearchGridClick2,
                "change": this.onTriggerSearchGridClick3
            },
            'viewDocOrderInts #DateS': { select: this.onGrid_DateS },
            'viewDocOrderInts #DatePo': { select: this.onGrid_DatePo },



            // Кнопки-статусы
            'viewDocOrderInts button#btnStatus10': { "click": this.onBtnStatusClick },
            'viewDocOrderInts button#btnStatus20': { "click": this.onBtnStatusClick },
            'viewDocOrderInts button#btnStatus30': { "click": this.onBtnStatusClick },
            'viewDocOrderInts button#btnStatus35': { "click": this.onBtnStatusClick },
            'viewDocOrderInts button#btnStatus40': { "click": this.onBtnSaveClick }, //"click": this.onBtnStatusClick
            'viewDocOrderInts button#btnStatus50': { "click": this.onBtnStatusClick },


            // PanelGrid1
            'viewDocOrderInts [itemId=grid1]': {
                selectionchange: this.onGrid1Selectionchange,
                edit: this.onGrid1Edit,
            },
            'viewDocOrderInts button#btnGridDeletion1': { "click": this.onBtnGridDeletion1 },
            'viewDocOrderInts button#btnGridAddPosition11': { click: this.onBtnGridAddPosition11 },
            'viewDocOrderInts button#btnGridAddPosition12': { click: this.onBtnGridAddPosition12 },


            // PanelGrid2
            'viewDocOrderInts [itemId=grid2]': {
                selectionchange: this.onGrid2Selectionchange
            },
            'viewDocOrderInts button#btnGridDeletion2': { "click": this.onBtnGridDeletion2 },
            'viewDocOrderInts button#btnGridAddPosition2': { click: this.onBtnGridAddPosition2 },


            //Log *** *** ***
            // PanelGridLog
            'viewDocOrderInts button#btnPanelGridLogAdd': { click: this.onBtnPanelGridLogAdd },
            //SMS
            'viewDocOrderInts button#btnSMS': { click: this.onBtnSMS },



            // === Кнопки: Сохранение (Выдача) === === ===
            //'viewDocOrderInts button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocOrderInts button#btnSave': { click: this.onBtnSaveClick },
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
        controllerDocOrderInts_onGridX_itemclick(view.grid); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGridX_itemdblclick: function (view, record, item, index, e) {
        controllerDocOrderInts_onGridX_itemclick(view.grid.UO_id);
    },
    onPanelGrid0Edit: function (aEditor, aE1) {
        
        //aE1.record.data.DocOrderIntID = Ext.getCmp("DocOrderIntID" + aEditor.grid.UO_id).getValue();
        var dataX = Ext.encode(aE1.record.data);
        //var ddd = ffff;

        //Сохранение
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocOrderInts + aE1.record.data.DocOrderIntID + "/?DateDone=" + aE1.record.data.DateDone,
            method: 'PUT',
            params: { recordsDataX: dataX },

            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
                else {
                    //Обновляем ЛОГ
                    Ext.getCmp("gridLog0_" + aEditor.grid.UO_id).getStore().load();
                }
            },
            failure: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
            }
        });
    },


    //Поиск
    onTriggerSearchGridClick1: function (aButton, aEvent) {
        funGridDoc(aButton.UO_id, HTTP_DocOrderInts + "?DocOrderIntTypeS=1&DocOrderIntTypePo=9&DirOrderIntStatusIDS=1&DirOrderIntStatusIDPo=90&DirWarehouseID=" + varDirWarehouseID);
    },
    onTriggerSearchGridClick2: function (f, e) {
        if (e.getKey() == e.ENTER) {
            funGridDoc(f.UO_id, HTTP_DocOrderInts + "?DocOrderIntTypeS=1&DocOrderIntTypePo=9&DirOrderIntStatusIDS=1&DirOrderIntStatusIDPo=90&DirWarehouseID=" + varDirWarehouseID);
        }
    },
    onTriggerSearchGridClick3: function (e, textReal, textLast) {
        if (textReal.length > 2) funGridDoc(e.UO_id, HTTP_DocOrderInts + "?DirOrderIntStatusIDS=1&DirOrderIntStatusIDPo=90&DirWarehouseID=" + varDirWarehouseID);
    },
    onGrid_DateS: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocOrderInts + "?DocOrderIntTypeS=1&DocOrderIntTypePo=9&DirOrderIntStatusIDS=1&DirOrderIntStatusIDPo=90&DirWarehouseID=" + varDirWarehouseID);
    },
    onGrid_DatePo: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocOrderInts + "?DocOrderIntTypeS=1&DocOrderIntTypePo=9&DirOrderIntStatusIDS=10&DirOrderIntStatusIDPo=90&DirWarehouseID=" + varDirWarehouseID);
    },



    // Кнопки-статусы *** *** *** *** *** *** *** *** *** *** *** *** ***
    onBtnStatusClick: function (aButton, aEvent, aOptions) {
        //var id = aButton.UO_id;

        //Запрос на сервер
        controllerDocOrderInts_ChangeStatus_Request(aButton, 0);
    },



    // PanelGridLog: Список Клик по Гриду *** *** *** *** *** *** *** *** *** *** *** *** *** ***
    onBtnPanelGridLogAdd: function (aButton, aEvent, aOptions) {
        var Params = [
            "gridLog0_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            "DocOrderIntID=" + Ext.getCmp("DocOrderIntID" + aButton.UO_id).getValue()
        ]
        ObjectConfig("viewLogOrderInts", Params);
    },
    //SMS
    /*onBtnSMS: function (aButton, aEvent, aOptions) {
        controllerDocOrderInts_SenSMS(aButton.UO_id, 1, 1);
    },*/



    // === Кнопки: Сохранение (Выдача) === === ===
    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        /*
        if (varPayType == 0) {
            Ext.MessageBox.show({
                icon: Ext.MessageBox.QUESTION,
                width: 300,
                title: lanOrgName,
                msg: 'Выбирите Тип оплаты!',
                buttonText: { yes: "Наличная", no: "Безналичная", cancel: "Отмена" },
                fn: function (btn) {
                    if (btn == "yes") {
                        //Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                        controllerDocOrderInts_ChangeStatus_Request(aButton, 1);
                    }
                    else if (btn == "no") {
                        //Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                        controllerDocOrderInts_ChangeStatus_Request(aButton, 2);
                    }
                }
            });
        }
        else if (varPayType == 1) {
            controllerDocOrderInts_ChangeStatus_Request(aButton, 1);
        }
        else if (varPayType == 2) {
            controllerDocOrderInts_ChangeStatus_Request(aButton, 2);
        }
        */

        var Params = [
            "viewDocOrderInts" + aButton.UO_id, //"grid_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            aButton.UO_id,
            controllerDocOrderInts_ChangeStatus_Request,
            aButton
        ]
        ObjectEditConfig("viewDocOrderIntsPurches", Params);

    },

});


//Клик по ГридамX
function controllerDocOrderInts_onGridX_itemclick(view_grid) {
    
    var id = view_grid.UO_id;
    var itemId = view_grid.itemId;

    //Создаём копию данных "Данные", т.к. если Панель, но и вызвавший виджет удаляется с Центральной панели, да и строка будет короче.
    if (Ext.getCmp(itemId + id) == undefined) return;
    var IdcallModelData = Ext.getCmp(itemId + id).getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

    //Если эта запись уже открыта на редактирования, то повторно её не открывать. Иначе не сможем редактировать "Дату Готовности"
    if (IdcallModelData.DocID == Ext.getCmp("DocID" + id).getValue()) return;

    //Если запись помечена на удаление, то сообщить об этом и выйти
    if (IdcallModelData.Del == true) {
        //Разблокировка вызвавшего окна
        ObjectEditConfig_UO_idCall_true_false(false);

        Ext.MessageBox.show({ title: lanFailure, msg: txtMsg023, icon: Ext.MessageBox.ERROR, buttons: Ext.Msg.OK });
        return;
    }

    Ext.getCmp("btnStatus20" + id).setText(""); Ext.getCmp("btnStatus20" + id).width = 50; Ext.getCmp("btnStatus20" + id).setPressed(false);
    Ext.getCmp("btnStatus30" + id).setVisible(true);
    Ext.getCmp("btnStatus35" + id).setVisible(true);
    Ext.getCmp("btnStatus40" + id).setVisible(true);
    Ext.getCmp("btnStatus50" + id).setVisible(true);

    //Меняем формат датв, а то глючит!
    Ext.getCmp("DocDate" + id).format = "c";


    var widgetX = Ext.getCmp("viewDocOrderInts" + id);

    //Лог
    widgetX.storeLogOrderIntsGrid.setData([], false);
    widgetX.storeLogOrderIntsGrid.proxy.url = HTTP_LogOrderInts + "?DocOrderIntID=" + IdcallModelData.DocOrderIntID;
    widgetX.storeLogOrderIntsGrid.UO_Loaded = false;
    

    //Форма
    var widgetXForm = Ext.getCmp("form_" + id);
    widgetXForm.form.url = HTTP_DocOrderInts + IdcallModelData.DocOrderIntID + "/?DocID=" + IdcallModelData.DocID; //С*ка глючит фреймворк и присвивает в форме старый УРЛ!!!
    widgetXForm.setVisible(true);
    widgetXForm.reset();
    widgetXForm.UO_Loaded = false;

    
    widgetXForm.load({
        method: "GET",
        timeout: varTimeOutDefault,
        waitMsg: lanLoading,
        //url: HTTP_DocOrderInts + IdcallModelData.DocOrderIntID + "/?DocID=" + IdcallModelData.DocID,
        success: function (form, action) {

            //Статусы и Кнопки
            controllerDocOrderInts_DirOrderIntStatusID_ChangeButton(id);

            //Меняем статус в самой таблице
            if (parseInt(Ext.getCmp("DirOrderIntStatusID" + id).getValue()) == 10) {
                //Меняем статус
                var storeX = Ext.getCmp(itemId + id).getSelectionModel().getSelection();
                storeX[0].data.DirOrderIntStatusID = 20;
                //Сохраняем
                Ext.getCmp(itemId + id).getView().refresh();
            }

            widgetXForm.UO_Loaded = true;
            widgetX.focus(); //Фокус на открывшийся Виджет

            //Log
            widgetX.storeLogOrderIntsGrid.load({ waitMsg: lanLoading });
        },
        failure: function (form, action) {
            funPanelSubmitFailure(form, action);
            widgetX.focus(); //Фокус на открывшийся Виджет
        }

    });

}


function controllerDocOrderInts_ChangeStatus_Request(aButton, DirPaymentTypeID, PriceX11) {
    if (DirPaymentTypeID == undefined) DirPaymentTypeID = 0;

    //Получаем ID-шние статуса
    var locDirOrderIntStatusID = parseInt(controllerDocOrderInts_DirOrderIntStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, false));
    if (isNaN(locDirOrderIntStatusID)) { return; }


    var sUrl = HTTP_DocOrderInts + Ext.getCmp("DocOrderIntID" + aButton.UO_id).getValue() + "/" + locDirOrderIntStatusID + "/?DirPaymentTypeID=" + DirPaymentTypeID;
    if (PriceX11) {
        sUrl +=
            "&PriceCurrency=" + PriceX11[0] + "&PriceVAT=" + PriceX11[1] + 
            "&MarkupRetail=" + PriceX11[2] + "&PriceRetailVAT=" + PriceX11[3] + "&PriceRetailCurrency=" + PriceX11[4] + 
            "&MarkupWholesale=" + PriceX11[5] + "&PriceWholesaleVAT=" + PriceX11[6] + "&PriceWholesaleCurrency=" + PriceX11[7] + 
            "&MarkupIM=" + PriceX11[8] + "&PriceIMVAT=" + PriceX11[9] + "&PriceIMCurrency=" + PriceX11[10] + 
            "&DirContractorID=" + PriceX11[11];
    }


    //Запрос на сервер на смену статуса
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: sUrl, //HTTP_DocOrderInts + Ext.getCmp("DocOrderIntID" + aButton.UO_id).getValue() + "/" + locDirOrderIntStatusID + "/?DirPaymentTypeID=" + DirPaymentTypeID,
        method: 'PUT',

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                controllerDocOrderInts_DirOrderIntStatusID_ChangeButton(aButton.UO_id);
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Меняем ID-шние статуса
                controllerDocOrderInts_DirOrderIntStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, true);

                //Статусы и Кнопки
                controllerDocOrderInts_DirOrderIntStatusID_ChangeButton(aButton.UO_id);

                //Обновить Список
                Ext.getCmp("tab_" + aButton.UO_id).getActiveTab().getStore().load();

                //Обновить Лог
                Ext.getCmp("gridLog0_" + aButton.UO_id).getStore().load();
            }
        },
        failure: function (result) {
            controllerDocOrderInts_DirOrderIntStatusID_ChangeButton(aButton.UO_id);

            var sData = Ext.decode(result.responseText);
            Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
        }
    });
}
//Статусы и Кнопки - выставить
function controllerDocOrderInts_DirOrderIntStatusID_ChangeButton(id)
{
    switch (parseInt(Ext.getCmp("DirOrderIntStatusID" + id).getValue())) {
        case 10:
            //На согласовании
            Ext.getCmp("btnStatus10" + id).setPressed(true);
            Ext.getCmp("btnStatus20" + id).setPressed(false);
            Ext.getCmp("btnStatus30" + id).setPressed(false);
            Ext.getCmp("btnStatus35" + id).setPressed(false);
            Ext.getCmp("btnStatus40" + id).setPressed(false);
            Ext.getCmp("btnStatus50" + id).setPressed(false);

            break;
        case 20:
            //В работе
            Ext.getCmp("btnStatus10" + id).setPressed(false);
            Ext.getCmp("btnStatus20" + id).setPressed(true);
            Ext.getCmp("btnStatus30" + id).setPressed(false);
            Ext.getCmp("btnStatus35" + id).setPressed(false);
            Ext.getCmp("btnStatus40" + id).setPressed(false);
            Ext.getCmp("btnStatus50" + id).setPressed(false);

            break;
        case 30:
            //Ожидание
            Ext.getCmp("btnStatus10" + id).setPressed(false);
            Ext.getCmp("btnStatus20" + id).setPressed(false);
            Ext.getCmp("btnStatus30" + id).setPressed(true);
            Ext.getCmp("btnStatus35" + id).setPressed(false);
            Ext.getCmp("btnStatus40" + id).setPressed(false);
            Ext.getCmp("btnStatus50" + id).setPressed(false);

            break;
        case 35:
            //Ожидание
            Ext.getCmp("btnStatus10" + id).setPressed(false);
            Ext.getCmp("btnStatus20" + id).setPressed(false);
            Ext.getCmp("btnStatus30" + id).setPressed(false);
            Ext.getCmp("btnStatus35" + id).setPressed(true);
            Ext.getCmp("btnStatus40" + id).setPressed(false);
            Ext.getCmp("btnStatus50" + id).setPressed(false);

            break;
        case 40:
            //Готов
            Ext.getCmp("btnStatus10" + id).setPressed(false);
            Ext.getCmp("btnStatus20" + id).setPressed(false);
            Ext.getCmp("btnStatus30" + id).setPressed(false);
            Ext.getCmp("btnStatus35" + id).setPressed(false);
            Ext.getCmp("btnStatus40" + id).setPressed(true);
            Ext.getCmp("btnStatus50" + id).setPressed(false);

            break;
        case 50:
            //Готов
            Ext.getCmp("btnStatus10" + id).setPressed(false);
            Ext.getCmp("btnStatus20" + id).setPressed(false);
            Ext.getCmp("btnStatus30" + id).setPressed(false);
            Ext.getCmp("btnStatus35" + id).setPressed(false);
            Ext.getCmp("btnStatus40" + id).setPressed(false);
            Ext.getCmp("btnStatus50" + id).setPressed(true);

            break;

    }
}
//Вернуть и/или поменять "DirOrderIntStatusID"
function controllerDocOrderInts_DirOrderIntStatusID_ChangeStatus(id, itemId, bchange) {
    switch (itemId) {
        case "btnStatus10":
            if (bchange) { Ext.getCmp("DirOrderIntStatusID" + id).setValue(10); }
            else { return 10; }
            break;
        case "btnStatus20":
            if (bchange) { Ext.getCmp("DirOrderIntStatusID" + id).setValue(20); }
            else { return 20; }
            break;
        case "btnStatus30":
            if (bchange) { Ext.getCmp("DirOrderIntStatusID" + id).setValue(30); }
            else { return 30; }
            break;
        case "btnStatus35":
            if (bchange) { Ext.getCmp("DirOrderIntStatusID" + id).setValue(35); }
            else { return 35; }
            break;
        case "btnStatus40":
            if (bchange) { Ext.getCmp("DirOrderIntStatusID" + id).setValue(40); }
            else { return 40; }
            break;
        case "btnStatus50":
            if (bchange) { Ext.getCmp("DirOrderIntStatusID" + id).setValue(50); }
            else { return 50; }
            break;

        case "btnSave":
            return 40;
            break;
    }
}


// Эти 2-е функции для сохранения "Выполненных работ" с запросом на сервер

function controllerDocOrderInts_onGrid1Edit(aEditor, aE1, pDirOrderIntStatusID) {

    aE1.record.data.DocOrderIntID = Ext.getCmp("DocOrderIntID" + aEditor.grid.UO_id).getValue();
    var dataX = Ext.encode(aE1.record.data);
    //var ddd = ffff;
    //Сохранение
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocOrderInt1Tabs + "?DirOrderIntStatusID=" + pDirOrderIntStatusID,
        method: 'POST',
        params: { recordsDataX: dataX },

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Получаем данные с Сервера
                var locDocOrderInt1TabID = sData.data.DocOrderInt1TabID;
                var DirEmployeeID = sData.data.DirEmployeeID;
                var DirCurrencyID = sData.data.DirCurrencyID;
                var DirCurrencyRate = sData.data.DirCurrencyRate;
                var DirCurrencyMultiplicity = sData.data.DirCurrencyMultiplicity;

                //Переменные
                var grid = aEditor.grid;
                var gridStore = grid.store;

                //UO + меняем значение в "UO_GridRecord"
                var UO_GridIndex = gridStore.indexOf(grid.getSelectionModel().getSelection()[0]); //UO_GridIndex: Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                var UO_GridRecord = grid.getSelectionModel().getSelection()[0]; //UO_GridRecord: Для загрузки данных в форму редактирования Табличной части
                UO_GridRecord.data.DocOrderInt1TabID = locDocOrderInt1TabID
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


                //Меняем кнопку на "pDirOrderIntStatusID" *** *** *** *** *** *** *** *** *** ***
                Ext.getCmp("DirOrderIntStatusID" + grid.UO_id).setValue(pDirOrderIntStatusID);
                controllerDocOrderInts_DirOrderIntStatusID_ChangeButton(grid.UO_id);
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

function controllerDocOrderInts_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, pDirOrderIntStatusID) {

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

    controllerDocOrderInts_RecalculationSums(idMy);


    //Запрос на сервер *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

    rec.data.DocOrderIntID = Ext.getCmp("DocOrderIntID" + idMy).getValue();
    var dataX = Ext.encode(rec.data);
    //Сохранение
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocOrderInt1Tabs + "?DirOrderIntStatusID=" + pDirOrderIntStatusID,
        method: 'POST',
        params: { recordsDataX: dataX },

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Получаем данные с Сервера
                var locDocOrderInt1TabID = sData.data.DocOrderInt1TabID;
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
                UO_GridRecord.data.DocOrderInt1TabID = locDocOrderInt1TabID
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
/*
function controllerDocOrderInts_SenSMS(id, DirSmsTemplateTypeS, DirSmsTemplateTypePo) {

    var Params = [
        "gridLog0_" + id, //UO_idCall
        true, //UO_Center
        true, //UO_Modal
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        "DocOrderIntID=" + Ext.getCmp("DocOrderIntID" + id).getValue() + "&MenuID=3" + "&DirSmsTemplateTypeS=" + DirSmsTemplateTypeS + "&DirSmsTemplateTypePo=" + DirSmsTemplateTypePo
    ]
    ObjectConfig("viewSms", Params);

}
*/

//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocOrderInts_RecalculationSums(id) {

    //1. Подсчет табличной части Работы "SumDocOrderInt1Tabs"
    //2. Подсчет табличной части Запчасти "SumDocOrderInt2Tabs"
    //3. Сумма 1+2 "SumTotal"
    //4. Константа "PrepaymentSum"
    //5. 3 - 4 "SumTotal2a"


    //1. Подсчет табличной части Работы "SumDocOrderInt1Tabs"
    var storeDocOrderInt1TabsGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeDocOrderInt1TabsGrid;
    var SumDocOrderInt1Tabs = 0;
    for (var i = 0; i < storeDocOrderInt1TabsGrid.data.items.length; i++) {
        SumDocOrderInt1Tabs += parseFloat(storeDocOrderInt1TabsGrid.data.items[i].data.PriceCurrency);
    }
    Ext.getCmp('SumDocOrderInt1Tabs' + id).setValue(SumDocOrderInt1Tabs.toFixed(varFractionalPartInSum));


    //2. Подсчет табличной части Работы "SumDocOrderInt2Tabs"
    var storeDocOrderInt2TabsGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeDocOrderInt2TabsGrid;
    var SumDocOrderInt2Tabs = 0;
    for (var i = 0; i < storeDocOrderInt2TabsGrid.data.items.length; i++) {
        SumDocOrderInt2Tabs += parseFloat(storeDocOrderInt2TabsGrid.data.items[i].data.PriceCurrency);
    }
    Ext.getCmp('SumDocOrderInt2Tabs' + id).setValue(SumDocOrderInt2Tabs.toFixed(varFractionalPartInSum));


    //3. Сумма 1+2 "SumTotal"
    Ext.getCmp('SumTotal' + id).setValue((SumDocOrderInt1Tabs + SumDocOrderInt2Tabs).toFixed(varFractionalPartInSum));


    //4. Константа "PrepaymentSum"
    //...


    //5. 3 - 4 "SumTotal2a"
    Ext.getCmp('SumTotal2a' + id).setValue((SumDocOrderInt1Tabs + SumDocOrderInt2Tabs - parseFloat(Ext.getCmp('PrepaymentSum' + id).getValue())).toFixed(varFractionalPartInSum));

};

