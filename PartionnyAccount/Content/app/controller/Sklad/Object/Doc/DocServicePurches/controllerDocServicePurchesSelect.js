Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocServicePurches/controllerDocServicePurchesSelect", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocServicePurchesSelect': { close: this.this_close },


            // PanelGrid0: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid0_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick,

                edit: this.onPanelGrid0Edit,
            },
            

            // PanelGrid1: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid1_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid5: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid5_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid2: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid2_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid6: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid6_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGridX: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGridX_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid3: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid3_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid4: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid4_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid7: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid7_]': {
                selectionchange: this.onGrid7_selectionchange,
                itemclick: this.onGrid7_itemclick,
                itemdblclick: this.onGrid7_itemdblclick
            },
            // PanelGrid9: Список Клик по Гриду
            'viewDocServicePurchesSelect [itemId=PanelGrid9_]': {
                selectionchange: this.onGrid9_selectionchange,
                itemclick: this.onGrid9_itemclick,
                itemdblclick: this.onGrid9_itemdblclick
            },
            'viewDocServicePurchesSelect #TriggerSearchGrid': {
                //"ontriggerclick": this.onTriggerSearchGridClick1,
                "specialkey": this.onTriggerSearchGridClick2,
                "change": this.onTriggerSearchGridClick3
            },
            'viewDocServicePurchesSelect #DateS': { select: this.onGrid_DateS },
            'viewDocServicePurchesSelect #DatePo': { select: this.onGrid_DatePo },


            //Смена мастера
            'viewDocServicePurchesSelect button#btnMasterEdit': { click: this.onBtnMasterEdit },

            //Гарантия - сменили
            'viewDocServicePurchesSelect [itemId=ServiceTypeRepair]': { select: this.onServiceTypeRepairSelect },


            // В БУ
            'viewDocServicePurchesSelect button#btnSecondHand': { "click": this.onBtnSecondHandClick },

            // Кнопки-статусы
            'viewDocServicePurchesSelect button#btnStatus2': { "click": this.onBtnStatusClick },
            'viewDocServicePurchesSelect button#btnStatus3': { "click": this.onBtnStatusClick },
            'viewDocServicePurchesSelect button#btnStatus4': { "click": this.onBtnStatusClick },
            'viewDocServicePurchesSelect button#btnStatus5': { "click": this.onBtnStatusClick },
            'viewDocServicePurchesSelect button#btnStatus7': { "click": this.onBtnStatusClick },
            'viewDocServicePurchesSelect button#btnStatus6': { "click": this.onBtnStatusClick },
            'viewDocServicePurchesSelect button#btnStatus8': { "click": this.onBtnStatusClick },

            'viewDocServicePurchesSelect button#btnPrint': { "click": this.onBtnPrintClick },


            // PanelGrid1
            'viewDocServicePurchesSelect [itemId=grid1]': {
                selectionchange: this.onGrid1Selectionchange,
                edit: this.onGrid1Edit,
            },
            'viewDocServicePurchesSelect button#btnGridDeletion1': { "click": this.onBtnGridDeletion1 },
            'viewDocServicePurchesSelect button#btnGridAddPosition11': { click: this.onBtnGridAddPosition11 },
            'viewDocServicePurchesSelect button#btnGridAddPosition12': { click: this.onBtnGridAddPosition12 },


            // PanelGrid2
            'viewDocServicePurchesSelect [itemId=grid2]': {
                selectionchange: this.onGrid2Selectionchange
            },
            'viewDocServicePurchesSelect button#btnGridDeletion2': { "click": this.onBtnGridDeletion2 },
            'viewDocServicePurchesSelect button#btnGridAddPosition2': { click: this.onBtnGridAddPosition2 },

            //Log *** *** ***
            // PanelGridLog
            'viewDocServicePurchesSelect button#btnPanelGridLogAdd': { click: this.onBtnPanelGridLogAdd },
            //SMS
            'viewDocServicePurchesSelect button#btnSMS': { click: this.onBtnSMS },
            //History
            'viewDocServicePurchesSelect button#btnHistory': { click: this.onBtnHistory },



            // === Кнопки: Сохранение (Выдача) === === ===
            //'viewDocServicePurchesSelect button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocServicePurchesSelect button#btnSave': { click: this.onBtnSaveClick },
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
        controllerDocServicePurchesSelect_onGridX_itemclick(view.grid, record, false); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGridX_itemdblclick: function (view, record, item, index, e) {
        controllerDocServicePurchesSelect_onGridX_itemclick(view.grid.UO_id, record, false);
    },
    onPanelGrid0Edit: function (aEditor, aE1) {
        
        //aE1.record.data.DocServicePurchID = Ext.getCmp("DocServicePurchID" + aEditor.grid.UO_id).getValue();
        var dataX = Ext.encode(aE1.record.data);
        //var ddd = ffff;
        
        //Сохранение
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocServicePurches + aE1.record.data.DocServicePurchID + "/?DateDone=" + aE1.record.data.DateDone,
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



    // Grid7: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGrid7_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGrid7_itemclick: function (view, record, item, index, eventObj) {
        controllerDocServicePurchesSelect_onGridX_itemclick(view.grid, record, true); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGrid7_itemdblclick: function (view, record, item, index, e) {
        controllerDocServicePurchesSelect_onGridX_itemclick(view.grid.UO_id, record, true);
    },



    // Grid9: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGrid9_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGrid9_itemclick: function (view, record, item, index, eventObj) {
        controllerDocServicePurchesSelect_onGridX_itemclick(view.grid, record, true); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGrid9_itemdblclick: function (view, record, item, index, e) {
        controllerDocServicePurchesSelect_onGridX_itemclick(view.grid, record, true); //.UO_id
    },


    //Поиск
    /*onTriggerSearchGridClick1111: function (aButton, aEvent) {
        
        if (Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).getValue().length > 0) {

            //Анулировать даты
            Ext.getCmp("DateS" + aButton.UO_id).setValue(null);
            Ext.getCmp("DatePo" + aButton.UO_id).setValue(null);

            funGridDoc(aButton.UO_id, HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID);
        }
    },*/
    onTriggerSearchGridClick2: function (f, e) {
        if (e.getKey() == e.ENTER && Ext.getCmp("TriggerSearchGrid" + f.UO_id).getValue().length > 0) {

            //Анулировать даты
            Ext.getCmp("DateS" + f.UO_id).setValue(null);
            Ext.getCmp("DatePo" + f.UO_id).setValue(null);

            funGridDoc(f.UO_id, HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID);
        }
    },
    onTriggerSearchGridClick3: function (e, textReal, textLast) {

        //Анулировать даты
        Ext.getCmp("DateS" + e.UO_id).setValue(null);
        Ext.getCmp("DatePo" + e.UO_id).setValue(null);

        if (textReal.length > 2) funGridDoc(e.UO_id, HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID);
    },
    //Даты
    onGrid_DateS: function (dataField, newValue, oldValue) {

        //Анулировать Триггер
        Ext.getCmp("TriggerSearchGrid" + dataField.UO_id).setValue(null);

        funGridDoc(dataField.UO_id, HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID);
    },
    onGrid_DatePo: function (dataField, newValue, oldValue) {

        //Анулировать Триггер
        Ext.getCmp("TriggerSearchGrid" + dataField.UO_id).setValue(null);

        funGridDoc(dataField.UO_id, HTTP_DocServicePurches + "?DirServiceStatusIDS=1&DirServiceStatusIDPo=9&DirWarehouseID=" + varDirWarehouseID);
    },


    //Смена мастера
    onBtnMasterEdit: function (aButton, aEvent, aOptions) {

        var id = aButton.UO_id;

        //Выбор Мастера для ремонта 1234567890
        var activeTab = Ext.getCmp("tab_" + id).getActiveTab();
        var Params = [
            activeTab,
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            activeTab.store.indexOf(activeTab.getSelectionModel().getSelection()[0]),       // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            activeTab.getSelectionModel().getSelection()[0], // Для загрузки данных в форму редактирования Табличной части
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            true
        ]
        ObjectEditConfig("viewDocServiceMasterSelect", Params);

    },

    //Гарантия - сменили
    onServiceTypeRepairSelect: function (combo, records) {

        var UO_id = combo.UO_id;
        records.data.DirDiscountID


        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocServicePurches + Ext.getCmp("DocServicePurchID" + UO_id).getValue() + "/" + records.data.ServiceTypeRepair + "/777/",
            method: 'PUT',

            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
                else {
                    Ext.getCmp("gridLog0_" + UO_id).getStore().load();
                }
            },
            failure: function (result) {
                var sData = Ext.decode(result.responseText);
                Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
            }
        });

    },


    //В БУ
    onBtnSecondHandClick: function (aButton, aEvent, aOptions) {

        var UO_id = aButton.UO_id;

        //Только, если вкладка "7" (Выдача)
        var activeTab = Ext.getCmp("tab_" + UO_id).getActiveTab();
        if (activeTab.itemId == "PanelGrid7_") {

            var DocServicePurchID = Ext.getCmp("DocServicePurchID" + UO_id).getValue();

            //Запрос на сервер с DocID
            Ext.Ajax.request({
                timeout: varTimeOutDefault,
                waitMsg: lanUpload,
                url: HTTP_DocServicePurches + DocServicePurchID + "/555/777/888/999/",
                method: 'PUT',

                success: function (result) {
                    var sData = Ext.decode(result.responseText);
                    if (sData.success == false) {
                        Ext.Msg.alert(lanOrgName, sData.data);
                    }
                    else {

                        //Ext.getCmp("viewDocServiceMasterSelect" + UO_id).close();

                        //Обновить Грид
                        var activeTab = Ext.getCmp("tab_" + UO_id).getActiveTab();
                        //Ext.getCmp("gridLog0_" + UO_id).getStore().load();
                        activeTab.getStore().load();

                        //Спрятать панель
                        var widgetXForm = Ext.getCmp("form_" + UO_id).setVisible(false);
                    }
                },
                failure: function (result) {
                    var sData = Ext.decode(result.responseText);
                    Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
                }
            });
            
        }
        else {
            alert("Ошибка! Вкладка должна быть 'Выдача'!");
        }
    },


    // Кнопки-статусы *** *** *** *** *** *** *** *** *** *** *** *** ***
    onBtnStatusClick: function (aButton, aEvent, aOptions) {
        

        //Проверка: если "Перемещён в БУ", то выдать сообщение и выйти
        if (Ext.getCmp("InSecondHand" + aButton.UO_id).getValue()) {
            Ext.Msg.alert(lanOrgName, "Внимание!<BR />Аппарат был перемещён в модуль Б/У! Редактирование запрещенно!");
            return;
        }


        //II. Если:
        //    1. На вкладке Архив
        //    2. Нажали кнопку "btnStatus2"
        //   то вывести форму с вопросом "Причины возврата на доработку" и записать причину в Лог
        var activeTab = Ext.getCmp("tab_" + aButton.UO_id).getActiveTab();
        if (activeTab.itemId == "PanelGrid9_" && aButton.itemId == "btnStatus2") {


            //Если "Срок гарантии прошёл", то написать об этом (C#: DateDone.AddMonths(docServicePurch.ServiceTypeRepair) <= DateTime.Now)
            var locboolWarrantyPeriodPassed = false;
            var todayDate = new Date();
            var IdcallModelData = activeTab.getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            var locDateDone = new Date(IdcallModelData.DateDone);
            locDateDone = Ext.Date.add(locDateDone, Ext.Date.MONTH, parseInt(IdcallModelData.ServiceTypeRepair));

            var locMsg = "";
            if (locDateDone <= todayDate)
            {
                locMsg = "<br /><span style='color:red'>Внимаение: Срок гарантии прошёл (до " + Ext.Date.format(locDateDone, "d-m-Y") + ")</span>";
            }



            Ext.Msg.prompt(lanOrgName, "Причины возврата на доработку " + locMsg,
                //height = 300,
                function (btnText, sReturnRresults) {
                    if (btnText === 'ok') {

                        //Запрос на сервер
                        controllerDocServicePurchesSelect_ChangeStatus_Request(aButton, 0, "Причина: " + sReturnRresults);

                    }
                    else {
                        Ext.Msg.alert(lanOrgName, "Возврат отменён!");
                    }
                },
                this
            ).setWidth(400);

        }
        else {

            //Запрос на сервер
            controllerDocServicePurchesSelect_ChangeStatus_Request(aButton, 0);

        }
    },


    //Печать
    onBtnPrintClick: function (aButton, aEvent, aOptions) {

        var map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");
        
        controllerListObjectPFs_onGrid_itemclick(
            map,
            aButton.UO_id,
            40,
            Ext.getCmp("rgListObjectPFID" + aButton.UO_id).getValue().ListObjectPFID, //35,
            Ext.getCmp("DocID" + aButton.UO_id).getValue(),
            "Html"
        );

    },



    // PanelGrid1 *** *** *** *** *** *** *** *** *** *** *** *** *** ***

    onGrid1Selectionchange: function (model, records) {
        model.view.ownerGrid.down("#btnGridDeletion1").setDisabled(records.length === 0);
    },
    onBtnGridDeletion1: function (aButton, aEvent, aOptions) {

        var selection = Ext.getCmp("grid1_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
        if (selection) {

            Ext.Msg.prompt(lanOrgName, "Причина Удаления",
                //height = 300,
                function (btnText, sDiagnosticRresults) {
                    if (btnText === 'ok') {
                        
                        Ext.Ajax.request({
                            timeout: varTimeOutDefault,
                            waitMsg: lanUpload,
                            url: HTTP_DocServicePurch1Tabs + selection.data.DocServicePurch1TabID + "/?sDiagnosticRresults=" + sDiagnosticRresults,
                            method: 'DELETE',

                            success: function (result) {
                                var sData = Ext.decode(result.responseText);
                                if (sData.success == false) {
                                    Ext.Msg.alert(lanOrgName, sData.data);
                                }
                                else {
                                    //Удалить запись в Гриде
                                    var selection = Ext.getCmp("grid1_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
                                    if (selection) { Ext.getCmp("grid1_" + aButton.UO_id).store.remove(selection); }
                                    //Обновить Лог
                                    Ext.getCmp("gridLog0_" + aButton.UO_id).getStore().load();
                                    controllerDocServicePurchesSelect_RecalculationSums(aButton.UO_id)
                                }
                            },
                            failure: function (result) {
                                var sData = Ext.decode(result.responseText);
                                if (sData.success == false) {
                                    Ext.Msg.alert(lanOrgName, sData.data);
                                }
                            }
                        });

                    }
                    else {
                        Ext.Msg.alert(lanOrgName, "Удаление отменено!");
                    }
                },
                this
            ).setWidth(400);

        }
        else {
            Ext.Msg.alert(lanOrgName, sData.data);
        }

    },

    onBtnGridAddPosition11: function (aButton, aEvent, aOptions) {
        var store = Ext.getCmp("grid1_" + aButton.UO_id).store;
        var model = new store.model();
        model.data.DirEmployeeName = lanDirEmployeeName
        store.insert(store.data.items.length, model);

        var rowEditing = Ext.getCmp("grid1_" + aButton.UO_id).rowEditing1;
        rowEditing.startEdit(store.data.items.length, 0);
    },
    onGrid1Edit: function (aEditor, aE1) {
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
                    controllerDocServicePurchesSelect_onGrid1Edit(aEditor.grid.UO_id, aE1.record, 4);
                }
                else if (btn == "no") {
                    //Запрос на сервер - сохранить выполненную работу
                    controllerDocServicePurchesSelect_onGrid1Edit(aEditor.grid.UO_id, aE1.record, 3);
                }
                else if (btn == "cancel") {
                    //Запрос на сервер - сохранить выполненную работу
                    controllerDocServicePurchesSelect_onGrid1Edit(aEditor.grid.UO_id, aE1.record, Ext.getCmp("DirServiceStatusID" + aEditor.grid.UO_id).getValue());
                }
            }
        });


    },

    //Заполнить 2-а поля
    onBtnGridAddPosition12: function (aButton, aEvent, aOptions) {
        var id = aButton.UO_id;

        var TreeServerParam1 = "DirServiceJobNomenType=1"; //2 - БУ

        var Params = [
            "grid1_" + id,
            true, //UO_Center
            true, //UO_Modal
            undefined,
            this.fn_onGrid_BtnGridAddPosition1, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            true, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            true, //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
            TreeServerParam1
        ]
        ObjectConfig("viewDirServiceJobNomenPrices", Params);
    },
    fn_onGrid_BtnGridAddPosition1: function (idMy, idSelect, rec) {

        var DirPriceTypeID = parseInt(Ext.getCmp("DirPriceTypeID" + idSelect).getValue());

        //if (rec.data.DirServiceJobNomenID == 1 || rec.data.DirServiceJobNomenName.toLowerCase().indexOf("диагностика") != -1) {
        if (rec.data.DirServiceJobNomenName.toLowerCase().indexOf("диагностика") != -1) {

            //123456789

            /*
            var msgbox = new Ext.create("PartionnyAccount.view.Sklad/Other/Pattern/viewComboBoxPrompt", {
                UO_id: idMy.UO_id,

                store: varStoreDirServiceDiagnosticRresultsGrid,
                valueField: 'DirServiceDiagnosticRresultName',
                hiddenName: 'DirServiceDiagnosticRresultName',
                displayField: 'DirServiceDiagnosticRresultName',
                name: 'DirServiceDiagnosticRresultName',
                itemId: 'DirServiceDiagnosticRresultName',

            }).prompt(lanOrgName, "Результат диагностики",
                    function (btnText, sDiagnosticRresults) {
                        if (btnText === 'ok') {
                            rec.data.DiagnosticRresults = sDiagnosticRresults;
                            controllerDocServicePurchesSelect_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, sDiagnosticRresults);
                        }
                        else {
                            controllerDocServicePurchesSelect_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, "");
                        }
                    }
            ).setWidth(400);
            */
            
            var Params = [
                idMy,
                true, //UO_Center
                true, //UO_Modal
                1,    // 1 - Новое, 2 - Редактировать
                false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                controllerDocServicePurchesSelect_DiagnosticRresults, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                idMy, // Для загрузки данных в форму Б.С. и Договора,
                idSelect,
                rec,
                DirPriceTypeID,
                "",
                undefined,
                undefined,
                false      //GridTree
            ]
            ObjectEditConfig("viewDirServiceDiagnosticRresultsWin", Params);

        }
        else {
            controllerDocServicePurchesSelect_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, "");
        }




        return;
        //OLD - не используется!!!
        //Выводится на экран окно "Результат диагностики", только если Работа - "Диагностика"
        //if (rec.data.DirServiceJobNomenID == 1 || rec.data.DirServiceJobNomenName.toLowerCase().indexOf("диагностика") != -1) {
        /*
        if (rec.data.DirServiceJobNomenName.toLowerCase().indexOf("диагностика") != -1) {

            Ext.Msg.prompt(lanOrgName, "Результат диагностики",
                //height = 300,
                function (btnText, sDiagnosticRresults) {
                    if (btnText === 'ok') {
                        rec.data.DiagnosticRresults = sDiagnosticRresults;
                        controllerDocServicePurchesSelect_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, sDiagnosticRresults);
                    }
                    else {
                        controllerDocServicePurchesSelect_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, "");
                    }
                },
                this
            ).setWidth(400);

        }
        else {
            controllerDocServicePurchesSelect_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, "");
        }
        */

    },



    // PanelGrid2 *** *** *** *** *** *** *** *** *** *** *** *** *** ***

    onGrid2Selectionchange: function (model, records) {
        model.view.ownerGrid.down("#btnGridDeletion2").setDisabled(records.length === 0);
    },
    onBtnGridDeletion2: function (aButton, aEvent, aOptions) {

        var selection = Ext.getCmp("grid2_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
        if (selection) {

            Ext.Ajax.request({
                timeout: varTimeOutDefault,
                waitMsg: lanUpload,
                url: HTTP_DocServicePurch2Tabs + selection.data.DocServicePurch2TabID + "/",
                method: 'DELETE',

                success: function (result) {
                    var sData = Ext.decode(result.responseText);
                    if (sData.success == false) {
                        Ext.Msg.alert(lanOrgName, sData.data);
                    }
                    else {
                        //Удалить запись в Гриде
                        var selection = Ext.getCmp("grid2_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
                        if (selection) { Ext.getCmp("grid2_" + aButton.UO_id).store.remove(selection); }
                        //Обновить Лог
                        Ext.getCmp("gridLog0_" + aButton.UO_id).getStore().load();
                        controllerDocServicePurchesSelect_RecalculationSums(aButton.UO_id);
                    }
                },
                failure: function (result) {
                    var sData = Ext.decode(result.responseText);
                    if (sData.success == false) {
                        Ext.Msg.alert(lanOrgName, sData.data);
                    }
                }
            });
        }
        else {
            Ext.Msg.alert(lanOrgName, sData.data);
        }

    },
    //Новая: Добавить позицию
    onBtnGridAddPosition2: function (aButton, aEvent, aOptions) {
        var id = aButton.UO_id;

        var Params = [
            "grid2_" + id,
            true, //UO_Center
            true, //UO_Modal
            undefined,
            this.fn_onBtnGridAddPosition2, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            true, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            true, //UO_GridRecord //record        // Для загрузки данных в форму Б.С. и Договора,
            undefined,
            undefined,
            undefined,
            3 //DirOrderIntTypeID=3 (Мастерская)
        ]
        ObjectConfig("viewDirNomenRemParties", Params);
    },
    //Заполнить 2-а поля
    fn_onBtnGridAddPosition2: function (idMy, idSelect, rec) {

        rec.data.DirEmployeeName = lanDirEmployeeName;

        //Получаем тип цены
        var DirPriceTypeID = parseInt(Ext.getCmp("DirPriceTypeID" + idSelect).getValue());

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


        var store = Ext.getCmp("grid2_" + idMy).getStore();
        store.insert(store.data.items.length, rec.data);

        controllerDocServicePurchesSelect_RecalculationSums(idMy);




        //Запрос на сервер *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        rec.data.DocServicePurchID = Ext.getCmp("DocServicePurchID" + idMy).getValue();
        var dataX = Ext.encode(rec.data);
        //Сохранение
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocServicePurch2Tabs,
            method: 'POST',
            params: { recordsDataX: dataX },

            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
                else {
                    //Получаем данные с Сервера
                    var locDocServicePurch2TabID = sData.data.DocServicePurch2TabID;
                    var DirEmployeeID = sData.data.DirEmployeeID;
                    var DirCurrencyID = sData.data.DirCurrencyID;
                    var DirCurrencyRate = sData.data.DirCurrencyRate;
                    var DirCurrencyMultiplicity = sData.data.DirCurrencyMultiplicity;

                    //Переменные
                    var grid = Ext.getCmp("grid2_" + idMy);
                    var gridStore = grid.store;

                    //UO + меняем значение в "UO_GridRecord"
                    var UO_GridIndex = store.data.items.length - 1; //gridStore.indexOf(grid.getSelectionModel().getSelection()[0]); //UO_GridIndex: Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    var UO_GridRecord = rec; //grid.getSelectionModel().getSelection()[0]; //UO_GridRecord: Для загрузки данных в форму редактирования Табличной части
                    UO_GridRecord.data.DocServicePurch2TabID = locDocServicePurch2TabID
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
                    controllerDocServicePurchesSelect_RecalculationSums(grid.UO_id);
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
            "DocServicePurchID=" + Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue()
        ]
        ObjectConfig("viewLogServices", Params);
    },
    //SMS
    onBtnSMS: function (aButton, aEvent, aOptions) {
        controllerDocServicePurchesSelect_SenSMS(aButton.UO_id, 1, 4);
    },
    //History
    onBtnHistory: function (aButton, aEvent, aOptions) {

        var Params = [
            "gridLog0_" + id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            undefined,
            undefined,
            undefined,
            undefined,
            undefined,
            "DocServicePurchID=" + Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue()
        ]
        ObjectConfig("viewDocServiceWorkshopHistories", Params);

    },


    // === Кнопки: Сохранение (Выдача) === === ===
    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        if (varPayType == 0) {
            Ext.MessageBox.show({
                icon: Ext.MessageBox.QUESTION,
                width: 300,
                title: lanOrgName,
                msg: "<b style='color: red;'>К оплате " + Ext.getCmp("SumTotal2a" + aButton.UO_id).getValue() + " рубасов</b><br /><br />" + "Выбирите Тип оплаты!",
                buttonText: { yes: "Наличная", no: "Безналичная", cancel: "Отмена" },
                fn: function (btn) {
                    if (btn == "yes") {
                        //Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
                        controllerDocServicePurchesSelect_ChangeStatus_Request(aButton, 1);
                    }
                    else if (btn == "no") {
                        //Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
                        controllerDocServicePurchesSelect_ChangeStatus_Request(aButton, 2);
                    }
                }
            });

        }
        else if (varPayType == 1) {
            //Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(1);
            controllerDocServicePurchesSelect_ChangeStatus_Request(aButton, 1);
        }
        else if (varPayType == 2) {
            //Ext.getCmp("DirPaymentTypeID" + aButton.UO_id).setValue(2);
            controllerDocServicePurchesSelect_ChangeStatus_Request(aButton, 2);
        }

    },
});


//Клик по ГридамX
function controllerDocServicePurchesSelect_onGridX_itemclick(view_grid, record, btnSave) {
    var id = view_grid.UO_id;
    var itemId = view_grid.itemId;
    var InSecondHand = record.get('InSecondHand');

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

    //Если аппарат перемещён в модуль БУ
    //if (InSecondHand) { Ext.Msg.alert(lanOrgName, "Внимание!<BR />Аппарат был перемещён в модуль Б/У! Редактирование запрещенно!"); }


    //1. Делаем всё видимым и редактируемым!
    Ext.getCmp("SumDocServicePurch1Tabs" + id).setVisible(false);
    Ext.getCmp("SumDocServicePurch2Tabs" + id).setVisible(false);
    Ext.getCmp("SumTotal" + id).setVisible(false);
    Ext.getCmp("PrepaymentSum" + id).setVisible(false);
    //Ext.getCmp("SumTotal2a" + id).setVisible(false);
    Ext.getCmp("btnSave" + id).setVisible(false); Ext.getCmp("btnSecondHand" + id).setVisible(false);
    Ext.getCmp("fsListObjectPFID" + id).setVisible(false);

    Ext.getCmp("grid1_" + id).enable();
    Ext.getCmp("grid2_" + id).enable();

    Ext.getCmp("btnStatus2" + id).setText(""); Ext.getCmp("btnStatus2" + id).width = 50; Ext.getCmp("btnStatus2" + id).setPressed(false); Ext.getCmp("btnStatus2" + id).setVisible(true);
    Ext.getCmp("btnStatus3" + id).setVisible(true);
    Ext.getCmp("btnStatus5" + id).setVisible(true);
    Ext.getCmp("btnStatus4" + id).setVisible(true);
    Ext.getCmp("btnStatus7" + id).setVisible(true);
    Ext.getCmp("btnStatus6" + id).setVisible(true);
    Ext.getCmp("btnStatus8" + id).setVisible(true);

    Ext.getCmp("ServiceTypeRepair" + id).enable();
    Ext.getCmp("btnMasterEdit" + id).enable();
    Ext.getCmp("gridLog0_" + id).enable();


    //2. Делаем не видимым и не редактируемым!
    if (btnSave) {
        Ext.getCmp("SumDocServicePurch1Tabs" + id).setVisible(true);
        Ext.getCmp("SumDocServicePurch2Tabs" + id).setVisible(true);
        Ext.getCmp("SumTotal" + id).setVisible(true);
        Ext.getCmp("PrepaymentSum" + id).setVisible(true);
        //Ext.getCmp("SumTotal2a" + id).setVisible(true);
        Ext.getCmp("btnSave" + id).setVisible(true);

        //В БУ
        var DateDoneMinis = (new Date() - new Date(record.get('DateDone'))) / (1000 * 3600 * 24);
        if (DateDoneMinis > 91) { Ext.getCmp("btnSecondHand" + id).setVisible(true); }

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
        var activeTab = Ext.getCmp("tab_" + id).getActiveTab();
        if (IdcallModelData.DirServiceStatusID == 9 || activeTab.itemId == "PanelGrid9_") {
            Ext.getCmp("ServiceTypeRepair" + id).disable(); Ext.getCmp("btnMasterEdit" + id).disable();
            Ext.getCmp("gridLog0_" + id).disable();
            Ext.getCmp("btnSave" + id).setVisible(false); Ext.getCmp("btnSecondHand" + id).setVisible(false);
            Ext.getCmp("fsListObjectPFID" + id).setVisible(true);
            Ext.getCmp("btnStatus2" + id).setText("<b>Вернуть на доработку</b>"); Ext.getCmp("btnStatus2" + id).setWidth(200);

            //Если не архив и "Перемещён в БУ", то убрать эту кнопку
            if (IdcallModelData.DirServiceStatusID != 9 || IdcallModelData.InSecondHand) {
                Ext.getCmp("btnStatus2" + id).setVisible(false);
            }
        }

    }


    //Меняем формат датв, а то глючит!
    Ext.getCmp("DocDate" + id).format = "c";


    var widgetX = Ext.getCmp("viewDocServicePurchesSelect" + id);

    //Выполненная работа
    widgetX.storeDocServicePurch1TabsGrid.setData([], false);
    widgetX.storeDocServicePurch1TabsGrid.proxy.url = HTTP_DocServicePurch1Tabs + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
    widgetX.storeDocServicePurch1TabsGrid.UO_Loaded = false;
    //Запчасть
    widgetX.storeDocServicePurch2TabsGrid.setData([], false);
    widgetX.storeDocServicePurch2TabsGrid.proxy.url = HTTP_DocServicePurch2Tabs + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
    widgetX.storeDocServicePurch2TabsGrid.UO_Loaded = false;

    //Лог
    widgetX.storeLogServicesGrid0.setData([], false);
    widgetX.storeLogServicesGrid0.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID;
    widgetX.storeLogServicesGrid0.UO_Loaded = false;

    widgetX.storeLogServicesGrid1.setData([], false);
    widgetX.storeLogServicesGrid1.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID + "&DirServiceLogTypeIDS=1&DirServiceLogTypeIDPo=1";
    widgetX.storeLogServicesGrid1.UO_Loaded = false;

    widgetX.storeLogServicesGrid3.setData([], false);
    widgetX.storeLogServicesGrid3.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID + "&DirServiceLogTypeIDS=3&DirServiceLogTypeIDPo=3";
    widgetX.storeLogServicesGrid3.UO_Loaded = false;

    widgetX.storeLogServicesGrid4.setData([], false);
    widgetX.storeLogServicesGrid4.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID + "&DirServiceLogTypeIDS=4&DirServiceLogTypeIDPo=4";
    widgetX.storeLogServicesGrid4.UO_Loaded = false;

    widgetX.storeLogServicesGrid5.setData([], false);
    widgetX.storeLogServicesGrid5.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID + "&DirServiceLogTypeIDS=5&DirServiceLogTypeIDPo=5";
    widgetX.storeLogServicesGrid5.UO_Loaded = false;

    widgetX.storeLogServicesGrid6.setData([], false);
    widgetX.storeLogServicesGrid6.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID + "&DirServiceLogTypeIDS=6&DirServiceLogTypeIDPo=6";
    widgetX.storeLogServicesGrid6.UO_Loaded = false;

    widgetX.storeLogServicesGrid8.setData([], false);
    widgetX.storeLogServicesGrid8.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID + "&DirServiceLogTypeIDS=8&DirServiceLogTypeIDPo=8";
    widgetX.storeLogServicesGrid8.UO_Loaded = false;

    widgetX.storeLogServicesGrid9.setData([], false);
    widgetX.storeLogServicesGrid9.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID + "&DirServiceLogTypeIDS=9&DirServiceLogTypeIDPo=9";
    widgetX.storeLogServicesGrid9.UO_Loaded = false;

    widgetX.storeLogServicesGrid7.setData([], false);
    widgetX.storeLogServicesGrid7.proxy.url = HTTP_LogServices + "?DocServicePurchID=" + IdcallModelData.DocServicePurchID + "&DirServiceLogTypeIDS=7&DirServiceLogTypeIDPo=8";
    widgetX.storeLogServicesGrid7.UO_Loaded = false;


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
                    controllerDocServicePurchesSelect_DirServiceStatusID_ChangeButton(id);

                    //Меняем статус в самой таблице
                    if (IdcallModelData.DirServiceStatusID == 1) { //if (parseInt(Ext.getCmp("DirServiceStatusID" + id).getValue()) == 1) {
                        //Меняем статус
                        var storeX = Ext.getCmp(itemId + id).getSelectionModel().getSelection();
                        storeX[0].data.DirServiceStatusID = 2;
                        //Сохраняем
                        Ext.getCmp(itemId + id).getView().refresh();
                    }

                    //В наименование кнопке "Предыдущие ремонты" дописіваем к-во ремонтов
                    Ext.getCmp("btnHistory" + id).setText("Предыдущие ремонты");
                    if (parseInt(Ext.getCmp("QuantityCount" + id).getValue()) > 0) {
                        Ext.getCmp("btnHistory" + id).setText("Предыдущие ремонты (" + Ext.getCmp("QuantityCount" + id).getValue() + ")");
                    }

                    widgetXForm.UO_Loaded = true;
                    widgetX.focus(); //Фокус на открывшийся Виджет

                    //Log
                    widgetX.storeLogServicesGrid0.load({ waitMsg: lanLoading });

                    //В БУ
                    if (Ext.getCmp("InSecondHand" + id).getValue()) {
                        Ext.Msg.alert(lanOrgName, "Внимание!<BR />Аппарат был перемещён в модуль Б/У! Редактирование запрещенно!");
                    }
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
function controllerDocServicePurchesSelect_ChangeStatus_Request(aButton, DirPaymentTypeID, sReturnRresults) {
    if (DirPaymentTypeID == undefined) DirPaymentTypeID = 0;
    if (sReturnRresults == undefined) sReturnRresults = "";
    //Старый ID-шние статуса
    var locDirServiceStatusID_OLD = parseInt(Ext.getCmp("DirServiceStatusID" + aButton.UO_id).getValue());

    //Новый ID-шние статуса
    var locDirServiceStatusID = parseInt(controllerDocServicePurchesSelect_DirServiceStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, false));
    if (isNaN(locDirServiceStatusID)) { return; }


    if (locDirServiceStatusID == 9) {
        var map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");
    }

    //Запрос на сервер на смену статуса
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocServicePurches + Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue() + "/" + locDirServiceStatusID + "/?DirPaymentTypeID=" + DirPaymentTypeID + "&SumTotal2a=" + Ext.getCmp("SumTotal2a" + aButton.UO_id).getValue() + "&sReturnRresults=" + sReturnRresults,
        method: 'PUT',

        success: function (result) {
            
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                controllerDocServicePurchesSelect_DirServiceStatusID_ChangeButton(aButton.UO_id);
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {

                Ext.getCmp("viewDocServicePurchesSelect" + aButton.UO_id).close();
                return;

                //Меняем ID-шние статуса
                controllerDocServicePurchesSelect_DirServiceStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, true);

                //Статусы и Кнопки
                controllerDocServicePurchesSelect_DirServiceStatusID_ChangeButton(aButton.UO_id);

                //Сообщение
                if (locDirServiceStatusID == 9) {
                    Ext.Msg.alert(lanOrgName, "Аппарат выдан и перемещён в архив");
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    Ext.getCmp("PanelGrid7_" + aButton.UO_id).getStore().load();


                    // *** Печатніе формы ***

                    //Проверка: если форма ещё не сохранена, то выход
                    if (Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }



                    //Открытие списка ПФ
                    /*
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
                    */

                    //Выводим ПФ: Квитанция
                    controllerListObjectPFs_onGrid_itemclick(
                        map,
                        aButton.UO_id,
                        40,
                        35,
                        Ext.getCmp("DocID" + aButton.UO_id).getValue(),
                        "Html"
                    );

                    if (SmsAutoShow) controllerDocServicePurchesSelect_SenSMS(aButton.UO_id, 4, 4);

                }
                //SMS
                else if (locDirServiceStatusID == 7) {
                    if (SmsAutoShow) controllerDocServicePurchesSelect_SenSMS(aButton.UO_id, 2, 2);
                }
                else if (locDirServiceStatusID == 8) {
                    if (SmsAutoShow) controllerDocServicePurchesSelect_SenSMS(aButton.UO_id, 3, 3);
                }
                else if (locDirServiceStatusID == 2 && locDirServiceStatusID_OLD == 9) {
                    //Обновить Грид "Архив"
                    Ext.getCmp("PanelGrid9_" + aButton.UO_id).getStore().load();
                    //Закрыть форму редактирование
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    //SMS
                    if (SmsAutoShowServiceFromArchiv) controllerDocServiceWorkshops_SenSMS(aButton.UO_id, 1, 1);
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


                //Обновить цвет грида
                var grid = Ext.getCmp("tab_" + aButton.UO_id).getActiveTab();
                //var grid = Ext.getCmp("PanelGrid0_" + aButton.UO_id);
                preselectItem(grid, 'DocServicePurchID', Ext.getCmp("DocServicePurchID" + aButton.UO_id).getValue());
            }
        },
        failure: function (result) {
            controllerDocServicePurchesSelect_DirServiceStatusID_ChangeButton(aButton.UO_id);

            var sData = Ext.decode(result.responseText);
            Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
        }
    });
}
//Статусы и Кнопки - выставить
function controllerDocServicePurchesSelect_DirServiceStatusID_ChangeButton(id)
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


            //Выбор Мастера для ремонта 1234567890
            var activeTab = Ext.getCmp("tab_" + id).getActiveTab();
            var Params = [
                activeTab,
                true, //UO_Center
                true, //UO_Modal
                1,    // 1 - Новое, 2 - Редактировать
                false, // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
                activeTab.store.indexOf(activeTab.getSelectionModel().getSelection()[0]),       // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                activeTab.getSelectionModel().getSelection()[0], // Для загрузки данных в форму редактирования Табличной части
                undefined,
                undefined,
                undefined,
                undefined,
                undefined,
                undefined,
                true
            ]
            ObjectEditConfig("viewDocServiceMasterSelect", Params);


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
function controllerDocServicePurchesSelect_DirServiceStatusID_ChangeStatus(id, itemId, bchange) {
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
            if (Ext.getCmp("grid1_" + id).getStore().data.length == undefined) { Ext.Msg.alert(lanOrgName, "Для статуса готов, должна присутствовать в списке работ, хотя бы одна выполненная работа!"); controllerDocServicePurchesSelect_DirServiceStatusID_ChangeButton(id); return; }
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
function controllerDocServicePurchesSelect_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, sDiagnosticRresults) {
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
                controllerDocServicePurchesSelect_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, 4, sDiagnosticRresults);
            }
            else if (btn == "no") {
                //Запрос на сервер - сохранить выполненную работу
                controllerDocServicePurchesSelect_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, 3, sDiagnosticRresults);
            }
            else if (btn == "cancel") {
                //Запрос на сервер - сохранить выполненную работу
                controllerDocServicePurchesSelect_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, Ext.getCmp("DirServiceStatusID" + idMy).getValue(), sDiagnosticRresults);
            }
        }
    });
}
//Эти 2-е функции для сохранения "Выполненных работ" с запросом на сервер
function controllerDocServicePurchesSelect_onGrid1Edit(UO_id, record, pDirServiceStatusID) {

    record.data.DocServicePurchID = Ext.getCmp("DocServicePurchID" + UO_id).getValue(); //aEditor.grid.UO_id
    var dataX = Ext.encode(record.data);
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
                Ext.Msg.alert(lanOrgName, sData.data + "<hr />Данная операция не сохранена!");
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


                gridStore.load({ waitMsg: lanLoading });
                gridStore.on('load', function () {

                    //Меняем кнопку на "pDirServiceStatusID" *** *** *** *** *** *** *** *** *** ***
                    Ext.getCmp("DirServiceStatusID" + grid.UO_id).setValue(pDirServiceStatusID);
                    controllerDocServicePurchesSelect_DirServiceStatusID_ChangeButton(grid.UO_id);

                    controllerDocServicePurchesSelect_RecalculationSums(UO_id);

                });
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
function controllerDocServicePurchesSelect_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, pDirServiceStatusID, sDiagnosticRresults) {

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

    //controllerDocServicePurchesSelect_RecalculationSums(idMy);


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
                controllerDocServicePurchesSelect_DirServiceStatusID_ChangeButton(grid.UO_id);
                controllerDocServicePurchesSelect_RecalculationSums(grid.UO_id);

                store.load({ waitMsg: lanLoading });
                store.on('load', function () {

                    controllerDocServicePurchesSelect_RecalculationSums(idMy);

                });
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
function controllerDocServicePurchesSelect_SenSMS(id, DirSmsTemplateTypeS, DirSmsTemplateTypePo) {
    
    var Params = [
        "gridLog0_" + id, //UO_idCall
        true, //UO_Center
        true, //UO_Modal
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        "DocServicePurchID=" + Ext.getCmp("DocServicePurchID" + id).getValue() + "&MenuID=1" + "&DirSmsTemplateTypeS=" + DirSmsTemplateTypeS + "&DirSmsTemplateTypePo=" + DirSmsTemplateTypePo,

    ]
    ObjectConfig("viewSms", Params);

}

//Поиск в Архиве
function controllerDocServicePurchesSelect_Search_Archiv(id, DirSmsTemplateTypeS, DirSmsTemplateTypePo) {

    if (Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue() == "") return;
    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).disable(); //Кнопку поиска делаем не активной


    var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value;



    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); //Кнопку поиска делаем не активной

}

//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocServicePurchesSelect_RecalculationSums(id) {

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
    Ext.getCmp('SumTotal2a' + id).setValue((SumDocServicePurch1Tabs + SumDocServicePurch2Tabs - parseFloat(Ext.getCmp('PrepaymentSum2' + id).getValue())).toFixed(varFractionalPartInSum));


    //Метки:
    Ext.getCmp('SumDocServicePurch1Tabs2' + id).setValue(Ext.getCmp('SumDocServicePurch1Tabs' + id).getValue());
    Ext.getCmp('SumDocServicePurch2Tabs2' + id).setValue(Ext.getCmp('SumDocServicePurch2Tabs' + id).getValue());
    Ext.getCmp('SumTotal2' + id).setValue(Ext.getCmp('SumTotal' + id).getValue());

};


function controllerDocServicePurchesSelect_PanelGrid1_DiagnosticRresults(value, metaData, record) {
    var DiagnosticRresults = record.get('DiagnosticRresults'); if (DiagnosticRresults == null) DiagnosticRresults = "";
    metaData.tdAttr = 'data-qtip="' + DiagnosticRresults + '"';
    return value;
}
