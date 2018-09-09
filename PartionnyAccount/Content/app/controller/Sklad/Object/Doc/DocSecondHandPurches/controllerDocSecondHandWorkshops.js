Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandWorkshops", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocSecondHandWorkshops': { close: this.this_close },


            // PanelGrid0: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid0_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick,

                edit: this.onPanelGrid0Edit,
            },
            

            // PanelGrid1: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid1_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid2: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid2_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            /*
            // PanelGrid3: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid3_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid4: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid4_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            */
            // PanelGrid5: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid5_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            /*
            // PanelGrid6: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid6_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGridX: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGridX_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            */
            // PanelGrid7: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid7_]': {
                selectionchange: this.onGrid7_selectionchange,
                itemclick: this.onGrid7_itemclick,
                itemdblclick: this.onGrid7_itemdblclick
            },
            'viewDocSecondHandWorkshops [itemId=PanelGrid8_]': {
                selectionchange: this.onGrid8_selectionchange,
                itemclick: this.onGrid8_itemclick,
                itemdblclick: this.onGrid8_itemdblclick
            },
            // PanelGrid9: Список Клик по Гриду
            'viewDocSecondHandWorkshops [itemId=PanelGrid9_]': {
                selectionchange: this.onGrid9_selectionchange,
                itemclick: this.onGrid9_itemclick,
                itemdblclick: this.onGrid9_itemdblclick
            },
            'viewDocSecondHandWorkshops #TriggerSearchGrid': {
                "ontriggerclick": this.onTriggerSearchGridClick1,
                "specialkey": this.onTriggerSearchGridClick2,
                "change": this.onTriggerSearchGridClick3
            },
            'viewDocSecondHandWorkshops #DateS': { select: this.onGrid_DateS },
            'viewDocSecondHandWorkshops #DatePo': { select: this.onGrid_DatePo },


            //Гарантия - сменили
            'viewDocSecondHandWorkshops [itemId=ServiceTypeRepair]': { select: this.onServiceTypeRepairSelect },



            // Кнопки-статусы
            'viewDocSecondHandWorkshops button#btnStatus2': { "click": this.onBtnStatusClick },
            'viewDocSecondHandWorkshops button#btnStatus5': { "click": this.onBtnStatusClick },
            'viewDocSecondHandWorkshops button#btnStatus7': { "click": this.onBtnSaveClick }, //{ "click": this.onBtnStatusClick },
            'viewDocSecondHandWorkshops button#btnStatus8': { "click": this.onBtnRazborClick }, //{ "click": this.onBtnStatusClick },

            // === Кнопки: Сохранение (Выдача) === === ===
            'viewDocSecondHandWorkshops button#btnSave': { click: this.onBtnSaveClick },
            'viewDocSecondHandWorkshops button#btnRazbor': { click: this.onBtnRazborClick },

            'viewDocSecondHandWorkshops button#btnPrint': { "click": this.onBtnPrintClick },



            // PanelGrid1
            'viewDocSecondHandWorkshops [itemId=grid1]': {
                selectionchange: this.onGrid1Selectionchange,
                edit: this.onGrid1Edit,
            },
            'viewDocSecondHandWorkshops button#btnGridDeletion1': { "click": this.onBtnGridDeletion1 },
            'viewDocSecondHandWorkshops button#btnGridAddPosition11': { click: this.onBtnGridAddPosition11 },
            'viewDocSecondHandWorkshops button#btnGridAddPosition12': { click: this.onBtnGridAddPosition12 },


            // PanelGrid2
            'viewDocSecondHandWorkshops [itemId=grid2]': {
                selectionchange: this.onGrid2Selectionchange
            },
            'viewDocSecondHandWorkshops button#btnGridDeletion2': { "click": this.onBtnGridDeletion2 },
            'viewDocSecondHandWorkshops button#btnGridAddPosition2': { click: this.onBtnGridAddPosition2 },

            //Log *** *** ***
            // PanelGridLog
            'viewDocSecondHandWorkshops button#btnPanelGridLogAdd': { click: this.onBtnPanelGridLogAdd },
            //SMS
            'viewDocSecondHandWorkshops button#btnSMS': { click: this.onBtnSMS },
            //History
            'viewDocSecondHandWorkshops button#btnHistory': { click: this.onBtnHistory },

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
        controllerDocSecondHandWorkshops_onGridX_itemclick(view.grid, record, false); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGridX_itemdblclick: function (view, record, item, index, e) {
        controllerDocSecondHandWorkshops_onGridX_itemclick(view.grid, record, false);
    },
    onPanelGrid0Edit: function (aEditor, aE1) {
        
        //aE1.record.data.DocSecondHandPurchID = Ext.getCmp("DocSecondHandPurchID" + aEditor.grid.UO_id).getValue();
        var dataX = Ext.encode(aE1.record.data);
        //var ddd = ffff;
        
        //Сохранение
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocSecondHandPurches + aE1.record.data.DocSecondHandPurchID + "/?DateDone=" + aE1.record.data.DateDone,
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
        controllerDocSecondHandWorkshops_onGridX_itemclick(view.grid, record, true); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGrid7_itemdblclick: function (view, record, item, index, e) {
        controllerDocSecondHandWorkshops_onGridX_itemclick(view.grid, record, true);
    },



    // Grid8: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGrid8_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGrid8_itemclick: function (view, record, item, index, eventObj) {
        controllerDocSecondHandWorkshops_onGridX_itemclick(view.grid, record, true, true); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGrid8_itemdblclick: function (view, record, item, index, e) {
        controllerDocSecondHandWorkshops_onGridX_itemclick(view.grid, record, true);
    },



    // Grid9: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGrid9_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGrid9_itemclick: function (view, record, item, index, eventObj) {
        controllerDocSecondHandWorkshops_onGridX_itemclick(view.grid, record, true); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGrid9_itemdblclick: function (view, record, item, index, e) {
        controllerDocSecondHandWorkshops_onGridX_itemclick(view.grid, record, true); //.UO_id
    },


    //Поиск
    onTriggerSearchGridClick1: function (aButton, aEvent) {
        if (Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).getValue().length > 0) {
            funGridDoc(aButton.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=100&DirWarehouseID=" + varDirWarehouseID);
        }
    },
    onTriggerSearchGridClick2: function (f, e) {
        if (e.getKey() == e.ENTER && Ext.getCmp("TriggerSearchGrid" + f.UO_id).getValue().length > 0) {
            funGridDoc(f.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=100&DirWarehouseID=" + varDirWarehouseID);
        }
    },
    onTriggerSearchGridClick3: function (e, textReal, textLast) {
        if (textReal.length >= 1) funGridDoc(e.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=100&DirWarehouseID=" + varDirWarehouseID);
    },
    //Даты
    onGrid_DateS: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=100&DirWarehouseID=" + varDirWarehouseID);
    },
    onGrid_DatePo: function (dataField, newValue, oldValue) {
        funGridDoc(dataField.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=1&DirSecondHandStatusIDPo=100&DirWarehouseID=" + varDirWarehouseID);
    },


    //Гарантия - сменили
    onServiceTypeRepairSelect: function (combo, records) {

        var UO_id = combo.UO_id;
        records.data.DirDiscountID


        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocSecondHandPurches + Ext.getCmp("DocSecondHandPurchID" + UO_id).getValue() + "/" + records.data.ServiceTypeRepair + "/777/",
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



    // Кнопки-статусы *** *** *** *** *** *** *** *** *** *** *** *** ***
    onBtnStatusClick: function (aButton, aEvent, aOptions) {
        //var id = aButton.UO_id;


        //II. Если:
        //    1. На вкладке Архив
        //    2. Нажали кнопку "btnStatus2"
        //   то вывести форму с вопросом "Причины возврата на доработку" и записать причину в Лог
        var activeTab = Ext.getCmp("tab_" + aButton.UO_id).getActiveTab();
        if (activeTab.itemId == "PanelGrid9_" && aButton.itemId == "btnStatus2") {


            //Если "Срок гарантии прошёл", то написать об этом (C#: DateDone.AddMonths(docSecondHandPurch.ServiceTypeRepair) <= DateTime.Now)
            var locboolWarrantyPeriodPassed = false;
            var todayDate = new Date();
            var IdcallModelData = activeTab.getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            var locDateDone = new Date(IdcallModelData.DateDone);
            //locDateDone = Ext.Date.add(locDateDone, Ext.Date.MONTH, parseInt(IdcallModelData.ServiceTypeRepair));
            locDateDone = Ext.Date.add(locDateDone, Ext.Date.MONTH, 0);

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
                        controllerDocSecondHandWorkshops_ChangeStatus_Request(aButton, 0, "Причина: " + sReturnRresults);

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
            controllerDocSecondHandWorkshops_ChangeStatus_Request(aButton, 0);

        }
    },


    //Печать
    onBtnPrintClick: function (aButton, aEvent, aOptions) {

        var map = window.open("", "Map", "status=0,title=0,height=600,width=800,scrollbars=1");
        
        controllerListObjectPFs_onGrid_itemclick(
            map,
            aButton.UO_id,
            65,
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
                            url: HTTP_DocSecondHandPurch1Tabs + selection.data.DocSecondHandPurch1TabID + "/?sDiagnosticRresults=" + sDiagnosticRresults,
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
                                    controllerDocSecondHandWorkshops_RecalculationSums(aButton.UO_id)
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
                    controllerDocSecondHandWorkshops_onGrid1Edit(aEditor.grid.UO_id, aE1.record, 4);
                }
                else if (btn == "no") {
                    //Запрос на сервер - сохранить выполненную работу
                    controllerDocSecondHandWorkshops_onGrid1Edit(aEditor.grid.UO_id, aE1.record, 3);
                }
                else if (btn == "cancel") {
                    //Запрос на сервер - сохранить выполненную работу
                    controllerDocSecondHandWorkshops_onGrid1Edit(aEditor.grid.UO_id, aE1.record, Ext.getCmp("DirSecondHandStatusID" + aEditor.grid.UO_id).getValue());
                }
            }
        });


    },

    //Заполнить 2-а поля
    onBtnGridAddPosition12: function (aButton, aEvent, aOptions) {
        var id = aButton.UO_id;

        var TreeServerParam1 = "DirServiceJobNomenType=2"; //2 - БУ

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

        //if (rec.data.DirSecondHandJobNomenID == 1 || rec.data.DirServiceJobNomenName.toLowerCase().indexOf("диагностика") != -1) {
        if (rec.data.DirServiceJobNomenName.toLowerCase().indexOf("диагностика") != -1) {

            //123456789

            /*
            var msgbox = new Ext.create("PartionnyAccount.view.Sklad/Other/Pattern/viewComboBoxPrompt", {
                UO_id: idMy.UO_id,

                store: varStoreDirSecondHandDiagnosticRresultsGrid,
                valueField: 'DirSecondHandDiagnosticRresultName',
                hiddenName: 'DirSecondHandDiagnosticRresultName',
                displayField: 'DirSecondHandDiagnosticRresultName',
                name: 'DirSecondHandDiagnosticRresultName',
                itemId: 'DirSecondHandDiagnosticRresultName',

            }).prompt(lanOrgName, "Результат диагностики",
                    function (btnText, sDiagnosticRresults) {
                        if (btnText === 'ok') {
                            rec.data.DiagnosticRresults = sDiagnosticRresults;
                            controllerDocSecondHandWorkshops_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, sDiagnosticRresults);
                        }
                        else {
                            controllerDocSecondHandWorkshops_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, "");
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
                controllerDocSecondHandWorkshops_DiagnosticRresults, //index,        // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
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
            controllerDocSecondHandWorkshops_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, "");
        }




        return;
        //OLD - не используется!!!
        //Выводится на экран окно "Результат диагностики", только если Работа - "Диагностика"
        //if (rec.data.DirSecondHandJobNomenID == 1 || rec.data.DirServiceJobNomenName.toLowerCase().indexOf("диагностика") != -1) {
        /*
        if (rec.data.DirServiceJobNomenName.toLowerCase().indexOf("диагностика") != -1) {

            Ext.Msg.prompt(lanOrgName, "Результат диагностики",
                //height = 300,
                function (btnText, sDiagnosticRresults) {
                    if (btnText === 'ok') {
                        rec.data.DiagnosticRresults = sDiagnosticRresults;
                        controllerDocSecondHandWorkshops_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, sDiagnosticRresults);
                    }
                    else {
                        controllerDocSecondHandWorkshops_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, "");
                    }
                },
                this
            ).setWidth(400);

        }
        else {
            controllerDocSecondHandWorkshops_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, "");
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
                url: HTTP_DocSecondHandPurch2Tabs + selection.data.DocSecondHandPurch2TabID + "/",
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
                        controllerDocSecondHandWorkshops_RecalculationSums(aButton.UO_id);
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
            4 //DirOrderIntTypeID=3 (Мастерская)
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

        controllerDocSecondHandWorkshops_RecalculationSums(idMy);




        //Запрос на сервер *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        rec.data.DocSecondHandPurchID = Ext.getCmp("DocSecondHandPurchID" + idMy).getValue();
        var dataX = Ext.encode(rec.data);
        //Сохранение
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocSecondHandPurch2Tabs,
            method: 'POST',
            params: { recordsDataX: dataX },

            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
                else {
                    //Получаем данные с Сервера
                    var locDocSecondHandPurch2TabID = sData.data.DocSecondHandPurch2TabID;
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
                    UO_GridRecord.data.DocSecondHandPurch2TabID = locDocSecondHandPurch2TabID
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
                    controllerDocSecondHandWorkshops_RecalculationSums(grid.UO_id);
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
            "DocSecondHandPurchID=" + Ext.getCmp("DocSecondHandPurchID" + aButton.UO_id).getValue()
        ]
        ObjectConfig("viewLogSecondHands", Params);
    },
    //SMS
    onBtnSMS: function (aButton, aEvent, aOptions) {
        controllerDocSecondHandWorkshops_SenSMS(aButton.UO_id, 1, 3);
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
            "DocSecondHandPurchID=" + Ext.getCmp("DocSecondHandPurchID" + aButton.UO_id).getValue()
        ]
        ObjectConfig("viewDocSecondHandWorkshopHistories", Params);

    },


    // === Кнопки: Сохранение (Выдача) === === ===
    onBtnSaveClick: function (aButton, aEvent, aOptions) {
        var Params = [
            "viewDocSecondHandWorkshops" + aButton.UO_id, //"grid_" + aButton.UO_id, //UO_idCall
            true, //UO_Center
            true, //UO_Modal
            1,    // 1 - Новое, 2 - Редактировать
            undefined,
            undefined,
            undefined,
            aButton.UO_id,
            controllerDocSecondHandWorkshops_ChangeStatus_Request,
            aButton
        ]
        ObjectEditConfig("viewDocSecondHandWorkshopsInRetail", Params);
    },
    // === Кнопки: Сохранение (Выдача) === === ===
    onBtnRazborClick: function (aButton, aEvent, aOptions) {
        Ext.MessageBox.show({
            icon: Ext.MessageBox.QUESTION,
            width: 300,
            title: lanOrgName,
            msg: 'Перенести аппарат на разбор?',
            buttonText: { yes: "Перенести", no: "Не переносить", cancel: "Отмена" },
            fn: function (btn) {
                if (btn == "yes") {
                    //Запрос на сервер - сохранить выполненную работу
                    controllerDocSecondHandWorkshops_ChangeStatus_Request(aButton, 0, "");
                }
            }
        });

    },

});


//Клик по ГридамX
function controllerDocSecondHandWorkshops_onGridX_itemclick(view_grid, record, btnSave) {
   
    var id = view_grid.UO_id;
    var itemId = view_grid.itemId;
    var FromService = record.get('FromService');

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
    Ext.getCmp("SumDocSecondHandPurch1Tabs" + id).setVisible(false);
    Ext.getCmp("SumDocSecondHandPurch2Tabs" + id).setVisible(false);
    Ext.getCmp("SumTotal" + id).setVisible(false);
    //Ext.getCmp("PrepaymentSum" + id).setVisible(false);
    //Ext.getCmp("SumTotal2a" + id).setVisible(false);
    Ext.getCmp("btnSave" + id).setVisible(false);
    Ext.getCmp("btnRazbor" + id).setVisible(false);
    Ext.getCmp("fsListObjectPFID" + id).setVisible(false);

    Ext.getCmp("grid1_" + id).enable();
    Ext.getCmp("grid2_" + id).enable();

    Ext.getCmp("btnStatus2" + id).setText(""); Ext.getCmp("btnStatus2" + id).width = 50; Ext.getCmp("btnStatus2" + id).setPressed(false); Ext.getCmp("btnStatus2" + id).setVisible(true);
    //Ext.getCmp("btnStatus3" + id).setVisible(true);
    Ext.getCmp("btnStatus5" + id).setVisible(true);
    //Ext.getCmp("btnStatus4" + id).setVisible(true);
    Ext.getCmp("btnStatus7" + id).setVisible(true);
    //Ext.getCmp("btnStatus6" + id).setVisible(true);
    Ext.getCmp("btnStatus8" + id).setVisible(true);

    //Ext.getCmp("ServiceTypeRepair" + id).enable();
    Ext.getCmp("gridLog0_" + id).enable();

    
    //2. Делаем не видимым и не редактируемым!
    if (btnSave) {
        Ext.getCmp("SumDocSecondHandPurch1Tabs" + id).setVisible(true);
        Ext.getCmp("SumDocSecondHandPurch2Tabs" + id).setVisible(true);
        Ext.getCmp("SumTotal" + id).setVisible(true);
        //Ext.getCmp("PrepaymentSum" + id).setVisible(true);
        //Ext.getCmp("SumTotal2a" + id).setVisible(true);
        Ext.getCmp("btnSave" + id).setVisible(true);
        Ext.getCmp("btnRazbor" + id).setVisible(false);

        Ext.getCmp("grid1_" + id).disable();
        Ext.getCmp("grid2_" + id).disable();

        Ext.getCmp("btnStatus2" + id).setText("В диагностике"); Ext.getCmp("btnStatus2" + id).width = 125; //Ext.getCmp("btnStatus2" + id).setVisible(false);
        //Ext.getCmp("btnStatus3" + id).setVisible(false);
        Ext.getCmp("btnStatus5" + id).setVisible(false);
        //Ext.getCmp("btnStatus4" + id).setVisible(false);
        Ext.getCmp("btnStatus7" + id).setVisible(false);
        //Ext.getCmp("btnStatus6" + id).setVisible(false);
        Ext.getCmp("btnStatus8" + id).setVisible(false);

        //Если Архив
        var activeTab = Ext.getCmp("tab_" + id).getActiveTab();
        if (IdcallModelData.DirSecondHandStatusID == 9 || activeTab.itemId == "PanelGrid9_") {
            //Ext.getCmp("ServiceTypeRepair" + id).disable();
            //Ext.getCmp("btnPanelGridLogAdd" + id).disable(); //Ext.getCmp("gridLog0_" + id).disable();
            Ext.getCmp("btnSave" + id).setVisible(false);
            Ext.getCmp("btnRazbor" + id).setVisible(false);
            Ext.getCmp("fsListObjectPFID" + id).setVisible(true);
            Ext.getCmp("btnStatus2" + id).setText("<b>Вернуть на доработку</b>"); Ext.getCmp("btnStatus2" + id).setWidth(200);

            //Если не архив, то убрать эту кнопку
            if (IdcallModelData.DirSecondHandStatusID != 9) {
                Ext.getCmp("btnStatus2" + id).setVisible(false);
            }
        }
        //Если "На разбор"
        if (IdcallModelData.DirSecondHandStatusID == 8 || activeTab.itemId == "PanelGrid8_") {
            Ext.getCmp("btnSave" + id).setVisible(false);
            Ext.getCmp("btnRazbor" + id).setVisible(true);
        }

    }


    //Меняем формат датв, а то глючит!
    Ext.getCmp("DocDate" + id).format = "c";


    var widgetX = Ext.getCmp("viewDocSecondHandWorkshops" + id);

    //Выполненная работа
    widgetX.storeDocSecondHandPurch1TabsGrid.setData([], false);
    widgetX.storeDocSecondHandPurch1TabsGrid.proxy.url = HTTP_DocSecondHandPurch1Tabs + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID;
    widgetX.storeDocSecondHandPurch1TabsGrid.UO_Loaded = false;
    //Запчасть
    widgetX.storeDocSecondHandPurch2TabsGrid.setData([], false);
    widgetX.storeDocSecondHandPurch2TabsGrid.proxy.url = HTTP_DocSecondHandPurch2Tabs + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID;
    widgetX.storeDocSecondHandPurch2TabsGrid.UO_Loaded = false;

    //Лог
    widgetX.storeLogSecondHandsGrid0.setData([], false);
    widgetX.storeLogSecondHandsGrid0.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID;
    widgetX.storeLogSecondHandsGrid0.UO_Loaded = false;

    widgetX.storeLogSecondHandsGrid1.setData([], false);
    widgetX.storeLogSecondHandsGrid1.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID + "&DirSecondHandLogTypeIDS=1&DirSecondHandLogTypeIDPo=1";
    widgetX.storeLogSecondHandsGrid1.UO_Loaded = false;

    widgetX.storeLogSecondHandsGrid3.setData([], false);
    widgetX.storeLogSecondHandsGrid3.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID + "&DirSecondHandLogTypeIDS=3&DirSecondHandLogTypeIDPo=3";
    widgetX.storeLogSecondHandsGrid3.UO_Loaded = false;

    widgetX.storeLogSecondHandsGrid4.setData([], false);
    widgetX.storeLogSecondHandsGrid4.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID + "&DirSecondHandLogTypeIDS=4&DirSecondHandLogTypeIDPo=4";
    widgetX.storeLogSecondHandsGrid4.UO_Loaded = false;

    widgetX.storeLogSecondHandsGrid5.setData([], false);
    widgetX.storeLogSecondHandsGrid5.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID + "&DirSecondHandLogTypeIDS=5&DirSecondHandLogTypeIDPo=5";
    widgetX.storeLogSecondHandsGrid5.UO_Loaded = false;

    widgetX.storeLogSecondHandsGrid6.setData([], false);
    widgetX.storeLogSecondHandsGrid6.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID + "&DirSecondHandLogTypeIDS=6&DirSecondHandLogTypeIDPo=6";
    widgetX.storeLogSecondHandsGrid6.UO_Loaded = false;

    widgetX.storeLogSecondHandsGrid8.setData([], false);
    widgetX.storeLogSecondHandsGrid8.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID + "&DirSecondHandLogTypeIDS=8&DirSecondHandLogTypeIDPo=8";
    widgetX.storeLogSecondHandsGrid8.UO_Loaded = false;

    widgetX.storeLogSecondHandsGrid9.setData([], false);
    widgetX.storeLogSecondHandsGrid9.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID + "&DirSecondHandLogTypeIDS=9&DirSecondHandLogTypeIDPo=9";
    widgetX.storeLogSecondHandsGrid9.UO_Loaded = false;

    widgetX.storeLogSecondHandsGrid7.setData([], false);
    widgetX.storeLogSecondHandsGrid7.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID + "&DirSecondHandLogTypeIDS=7&DirSecondHandLogTypeIDPo=8";
    widgetX.storeLogSecondHandsGrid7.UO_Loaded = false;


    //Форма
    var widgetXForm = Ext.getCmp("form_" + id);
    widgetXForm.form.url = HTTP_DocSecondHandPurches + IdcallModelData.DocSecondHandPurchID + "/?DocID=" + IdcallModelData.DocID; //С*ка глючит фреймворк и присвивает в форме старый УРЛ!!!
    widgetXForm.setVisible(true);
    widgetXForm.reset();
    widgetXForm.UO_Loaded = false;

    
    //Лоадер
    var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
    loadingMask.show();

    widgetX.storeDocSecondHandPurch1TabsGrid.load({ waitMsg: lanLoading });
    widgetX.storeDocSecondHandPurch1TabsGrid.on('load', function () {
        if (widgetX.storeDocSecondHandPurch1TabsGrid.UO_Loaded) { loadingMask.hide(); return; }
        widgetX.storeDocSecondHandPurch1TabsGrid.UO_Loaded = true;

        widgetX.storeDocSecondHandPurch2TabsGrid.load({ waitMsg: lanLoading });
        widgetX.storeDocSecondHandPurch2TabsGrid.on('load', function () {
            if (widgetX.storeDocSecondHandPurch2TabsGrid.UO_Loaded) { loadingMask.hide(); return; }
            widgetX.storeDocSecondHandPurch2TabsGrid.UO_Loaded = true;

            if (widgetXForm.UO_Loaded) { loadingMask.hide(); return; }

            loadingMask.hide();

            widgetXForm.load({
                method: "GET",
                timeout: varTimeOutDefault,
                waitMsg: lanLoading,
                //url: HTTP_DocSecondHandPurches + IdcallModelData.DocSecondHandPurchID + "/?DocID=" + IdcallModelData.DocID,
                success: function (form, action) {

                    //Статусы и Кнопки
                    controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeButton(id);

                    //Меняем статус в самой таблице
                    if (IdcallModelData.DirSecondHandStatusID == 1) { //if (parseInt(Ext.getCmp("DirSecondHandStatusID" + id).getValue()) == 1) {
                        //Меняем статус
                        var storeX = Ext.getCmp(itemId + id).getSelectionModel().getSelection();
                        storeX[0].data.DirSecondHandStatusID = 2;
                        //Сохраняем
                        Ext.getCmp(itemId + id).getView().refresh();
                    }

                    //В наименование кнопке "Предыдущие ремонты" дописіваем к-во ремонтов
                    /*
                    Ext.getCmp("btnHistory" + id).setText("Предыдущие ремонты");
                    if (parseInt(Ext.getCmp("QuantityCount" + id).getValue()) > 0) {
                        Ext.getCmp("btnHistory" + id).setText("Предыдущие ремонты (" + Ext.getCmp("QuantityCount" + id).getValue() + ")");
                    }
                    */

                    widgetXForm.UO_Loaded = true;
                    widgetX.focus(); //Фокус на открывшийся Виджет

                    //Log
                    widgetX.storeLogSecondHandsGrid0.load({ waitMsg: lanLoading });

                    //Из СЦ
                    if (Ext.getCmp("FromService" + id).getValue()) {
                        Ext.Msg.alert(lanOrgName, "Внимание!<BR />Аппарат был перемещён из модуля СЦ!");
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
function controllerDocSecondHandWorkshops_ChangeStatus_Request(aButton, DirPaymentTypeID, sReturnRresults, PriceX3) {

    if (DirPaymentTypeID == undefined) DirPaymentTypeID = 0;
    if (sReturnRresults == undefined) sReturnRresults = "";
    
    //Старый ID-шник статуса
    var locDirSecondHandStatusID_OLD = parseInt(Ext.getCmp("DirSecondHandStatusID" + aButton.UO_id).getValue());
    //Новый ID-шник статуса
    var locDirSecondHandStatusID = parseInt(controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, false));
    if (isNaN(locDirSecondHandStatusID)) { return; }


    // !!! !!! !!! Заказы !!! !!! !!!
    //Если на выдачу или выдать и есть заказ, то выдать сообщение об этом!
    if (locDirSecondHandStatusID > 6) {
        debugger;
        var
            locGrid2_ = Ext.getCmp("grid2_" + aButton.UO_id).store,
            IsZakaz = false;
        for (var i = 0; i < locGrid2_.data.length; i++) {
            if (locGrid2_.data.items[i].data.IsZakaz) {
                IsZakaz = true;
            }
        }

        if (IsZakaz) {
            Ext.Msg.alert(lanOrgName, "Внимание!!!<br />В списке запчастей имеет заказ(ы)! Смена статуса не возможна!");
            return;
        }
    }


    //!!! Важно !!!
    //Сергей предложил: не перекидывать в вкладки, а сразу переносить в продажу или на разбор
    var locDirSecondHandStatusI_OLD = 0;
    if (aButton.itemId == "btnStatus7") { locDirSecondHandStatusI_OLD = 7; }
    else if (aButton.itemId == "btnStatus8") { locDirSecondHandStatusI_OLD = 8; }


    var sUrl = HTTP_DocSecondHandPurches + Ext.getCmp("DocSecondHandPurchID" + aButton.UO_id).getValue() + "/" + locDirSecondHandStatusID + "/?DirPaymentTypeID=" + DirPaymentTypeID + "&SumTotal2a=" + Ext.getCmp("SumTotal2a" + aButton.UO_id).getValue() + "&sReturnRresults=" + sReturnRresults + "&locDirSecondHandStatusI_OLD=" + locDirSecondHandStatusI_OLD;
    if (PriceX3) {
        sUrl += "&PriceRetailVAT=" + PriceX3[0] + "&PriceWholesaleVAT=" + PriceX3[1] + "&PriceIMVAT=" + PriceX3[2];
    }

    //Запрос на сервер на смену статуса
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: sUrl,
        method: 'PUT',

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeButton(aButton.UO_id);
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Меняем ID-шние статуса
                controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, true);

                //Статусы и Кнопки
                controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeButton(aButton.UO_id);

                //Сообщение
                if (locDirSecondHandStatusID == 9) {
                    Ext.Msg.alert(lanOrgName, "Аппарат готов для продажи! (а так же перемещён в архив)");
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    Ext.getCmp("PanelGrid7_" + aButton.UO_id).getStore().load();


                    // *** Печатные формы ***

                    //Проверка: если форма ещё не сохранена, то выход
                    if (Ext.getCmp("DocSecondHandPurchID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }

                }
                //SMS
                else if (locDirSecondHandStatusID == 7) {
                    //if (SmsAutoShow) controllerDocSecondHandWorkshops_SenSMS(aButton.UO_id, 2, 2);
                }
                else if (locDirSecondHandStatusID == 8) {
                    //if (SmsAutoShow) controllerDocSecondHandWorkshops_SenSMS(aButton.UO_id, 3, 3);
                }
                else if (locDirSecondHandStatusID == 2 && locDirSecondHandStatusID_OLD == 9) {
                    //Обновить Грид "Архив"
                    Ext.getCmp("PanelGrid9_" + aButton.UO_id).getStore().load();
                    //Закрыть форму редактирование
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    return;
                }
                else if (locDirSecondHandStatusID == 2 && locDirSecondHandStatusID_OLD == 7) {
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
                preselectItem(grid, 'DocSecondHandPurchID', Ext.getCmp("DocSecondHandPurchID" + aButton.UO_id).getValue());
            }
        },
        failure: function (result) {
            controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeButton(aButton.UO_id);

            var sData = Ext.decode(result.responseText);
            Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
        }
    });
}
//Статусы и Кнопки - выставить
function controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeButton(id)
{
    switch (parseInt(Ext.getCmp("DirSecondHandStatusID" + id).getValue())) {
        case 1:
            //Принят
            Ext.Msg.alert(lanOrgName, "Статус сменён на: Предпродажа");

            Ext.getCmp("btnStatus2" + id).setPressed(true);
            //Ext.getCmp("btnStatus3" + id).setPressed(false);
            //Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            //Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);

            break;
        case 2:
            //В диагностике
            Ext.getCmp("btnStatus2" + id).setPressed(true);
            //Ext.getCmp("btnStatus3" + id).setPressed(false);
            //Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            //Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
            /*
        case 3:
            //На согласовании
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            //Ext.getCmp("btnStatus3" + id).setPressed(true);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
            */
            /*
        case 4:
            //Согласован
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            //Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(true); 
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
            */
        case 5:
            //Ожидание запчастей
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            //Ext.getCmp("btnStatus3" + id).setPressed(false);
            //Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(true);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            //Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
        case 7:
            //Отремонтирован
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            //Ext.getCmp("btnStatus3" + id).setPressed(false);
            //Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(true);
            //Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
            /*
        case 6:
            //В основном сервисе
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            //Ext.getCmp("btnStatus3" + id).setPressed(false);
            Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            Ext.getCmp("btnStatus6" + id).setPressed(true);
            Ext.getCmp("btnStatus8" + id).setPressed(false);
            break;
            */
        case 8:
            //Отказной
            Ext.getCmp("btnStatus2" + id).setPressed(false);
            //Ext.getCmp("btnStatus3" + id).setPressed(false);
            //Ext.getCmp("btnStatus4" + id).setPressed(false);
            Ext.getCmp("btnStatus5" + id).setPressed(false);
            Ext.getCmp("btnStatus7" + id).setPressed(false);
            //Ext.getCmp("btnStatus6" + id).setPressed(false);
            Ext.getCmp("btnStatus8" + id).setPressed(true);
            break;
    }
}
//Вернуть и/или поменять "DirSecondHandStatusID"
function controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeStatus(id, itemId, bchange) {
    
    switch (itemId) {
        case "btnStatus2":
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(2); }
            else { return 2; }
            break;
            /*
        case "btnStatus3":
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(3); }
            else { return 3; }
            break;
            */
            /*
        case "btnStatus4":
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(4); }
            else { return 4; }
            break;
            */
        case "btnStatus5":
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(5); }
            else { return 5; }
            break;
        case "btnStatus7":
            //Если нет ни одной выполненной работы, то не пускать сохранять и выдать эксепшн
            if (Ext.getCmp("grid1_" + id).getStore().data.length == undefined) { Ext.Msg.alert(lanOrgName, "Для статуса готов, должна присутствовать в списке работ, хотя бы одна выполненная работа!"); controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeButton(id); return; }
            /*
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(7); }
            else { return 7; }
            */
            return 9;
            break;
        case "btnStatus8":
            /*
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(8); }
            else { return 8; }
            */
            return 9;
            break;

        case "btnRazbor":
            return 9;
            break;
        case "btnSave":
            return 9;
            break;
    }
}

//Результат диагностики
function controllerDocSecondHandWorkshops_DiagnosticRresults(idMy, idSelect, rec, DirPriceTypeID, sDiagnosticRresults) {
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
                controllerDocSecondHandWorkshops_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, 4, sDiagnosticRresults);
            }
            else if (btn == "no") {
                //Запрос на сервер - сохранить выполненную работу
                controllerDocSecondHandWorkshops_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, 3, sDiagnosticRresults);
            }
            else if (btn == "cancel") {
                //Запрос на сервер - сохранить выполненную работу
                controllerDocSecondHandWorkshops_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, Ext.getCmp("DirSecondHandStatusID" + idMy).getValue(), sDiagnosticRresults);
            }
        }
    });
}
//Эти 2-е функции для сохранения "Выполненных работ" с запросом на сервер
function controllerDocSecondHandWorkshops_onGrid1Edit(UO_id, record, pDirSecondHandStatusID) {

    record.data.DocSecondHandPurchID = Ext.getCmp("DocSecondHandPurchID" + UO_id).getValue(); //aEditor.grid.UO_id
    var dataX = Ext.encode(record.data);
    //var ddd = ffff;
    //Сохранение
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocSecondHandPurch1Tabs + "?DirSecondHandStatusID=" + pDirSecondHandStatusID,
        method: 'POST',
        params: { recordsDataX: dataX },

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data + "<hr />Данная операция не сохранена!");
            }
            else {
                //Получаем данные с Сервера
                var locDocSecondHandPurch1TabID = sData.data.DocSecondHandPurch1TabID;
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
                UO_GridRecord.data.DocSecondHandPurch1TabID = locDocSecondHandPurch1TabID
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

                    //Меняем кнопку на "pDirSecondHandStatusID" *** *** *** *** *** *** *** *** *** ***
                    Ext.getCmp("DirSecondHandStatusID" + grid.UO_id).setValue(pDirSecondHandStatusID);
                    controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeButton(grid.UO_id);

                    controllerDocSecondHandWorkshops_RecalculationSums(UO_id);

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
function controllerDocSecondHandWorkshops_fn_onGrid_BtnGridAddPosition1(idMy, idSelect, rec, DirPriceTypeID, pDirSecondHandStatusID, sDiagnosticRresults) {

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

    //controllerDocSecondHandWorkshops_RecalculationSums(idMy);


    //Запрос на сервер *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

    rec.data.DocSecondHandPurchID = Ext.getCmp("DocSecondHandPurchID" + idMy).getValue();
    var dataX = Ext.encode(rec.data);
    //Сохранение
    Ext.Ajax.request({
        timeout: varTimeOutDefault,
        waitMsg: lanUpload,
        url: HTTP_DocSecondHandPurch1Tabs + "?DirSecondHandStatusID=" + pDirSecondHandStatusID + "&sDiagnosticRresults=" + sDiagnosticRresults,
        method: 'POST',
        params: { recordsDataX: dataX },

        success: function (result) {
            var sData = Ext.decode(result.responseText);
            if (sData.success == false) {
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Получаем данные с Сервера
                var locDocSecondHandPurch1TabID = sData.data.DocSecondHandPurch1TabID;
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
                UO_GridRecord.data.DocSecondHandPurch1TabID = locDocSecondHandPurch1TabID
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


                //Меняем кнопку на "pDirSecondHandStatusID" *** *** *** *** *** *** *** *** *** ***
                Ext.getCmp("DirSecondHandStatusID" + grid.UO_id).setValue(pDirSecondHandStatusID);
                controllerDocSecondHandWorkshops_DirSecondHandStatusID_ChangeButton(grid.UO_id);
                controllerDocSecondHandWorkshops_RecalculationSums(grid.UO_id);

                store.load({ waitMsg: lanLoading });
                store.on('load', function () {

                    controllerDocSecondHandWorkshops_RecalculationSums(idMy);

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
function controllerDocSecondHandWorkshops_SenSMS(id, DirSmsTemplateTypeS, DirSmsTemplateTypePo) {
    
    var Params = [
        "gridLog0_" + id, //UO_idCall
        true, //UO_Center
        true, //UO_Modal
        undefined,
        undefined,
        undefined,
        undefined,
        undefined,
        "DocSecondHandPurchID=" + Ext.getCmp("DocSecondHandPurchID" + id).getValue() + "&MenuID=1" + "&DirSmsTemplateTypeS=" + DirSmsTemplateTypeS + "&DirSmsTemplateTypePo=" + DirSmsTemplateTypePo,

    ]
    ObjectConfig("viewSms", Params);

}

//Поиск в Архиве
function controllerDocSecondHandWorkshops_Search_Archiv(id, DirSmsTemplateTypeS, DirSmsTemplateTypePo) {

    if (Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue() == "") return;
    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).disable(); //Кнопку поиска делаем не активной


    var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value;



    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); //Кнопку поиска делаем не активной

}

//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocSecondHandWorkshops_RecalculationSums(id) {

    //1. Подсчет табличной части Работы "SumDocSecondHandPurch1Tabs"
    //2. Подсчет табличной части Запчасти "SumDocSecondHandPurch2Tabs"
    //3. Сумма 1+2 "SumTotal"
    //4. Константа "PrepaymentSum"
    //5. 3 - 4 "SumTotal2a"


    //1. Подсчет табличной части Работы "SumDocSecondHandPurch1Tabs"
    var storeDocSecondHandPurch1TabsGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeDocSecondHandPurch1TabsGrid;
    var SumDocSecondHandPurch1Tabs = 0;
    for (var i = 0; i < storeDocSecondHandPurch1TabsGrid.data.items.length; i++) {
        SumDocSecondHandPurch1Tabs += parseFloat(storeDocSecondHandPurch1TabsGrid.data.items[i].data.PriceCurrency);
    }
    Ext.getCmp('SumDocSecondHandPurch1Tabs' + id).setValue(SumDocSecondHandPurch1Tabs.toFixed(varFractionalPartInSum));


    //2. Подсчет табличной части Работы "SumDocSecondHandPurch2Tabs"
    var storeDocSecondHandPurch2TabsGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeDocSecondHandPurch2TabsGrid;
    var SumDocSecondHandPurch2Tabs = 0;
    for (var i = 0; i < storeDocSecondHandPurch2TabsGrid.data.items.length; i++) {
        SumDocSecondHandPurch2Tabs += parseFloat(storeDocSecondHandPurch2TabsGrid.data.items[i].data.PriceCurrency);
    }
    Ext.getCmp('SumDocSecondHandPurch2Tabs' + id).setValue(SumDocSecondHandPurch2Tabs.toFixed(varFractionalPartInSum));


    //3. Сумма 1+2 "SumTotal"
    Ext.getCmp('SumTotal' + id).setValue((SumDocSecondHandPurch1Tabs + SumDocSecondHandPurch2Tabs).toFixed(varFractionalPartInSum));


    //4. Константа "PrepaymentSum"
    //...


    //5. 3 - 4 "SumTotal2a"
    //Ext.getCmp('SumTotal2a' + id).setValue((SumDocSecondHandPurch1Tabs + SumDocSecondHandPurch2Tabs - parseFloat(Ext.getCmp('PrepaymentSum' + id).getValue())).toFixed(varFractionalPartInSum));
    Ext.getCmp('SumTotal2a' + id).setValue((SumDocSecondHandPurch1Tabs + SumDocSecondHandPurch2Tabs).toFixed(varFractionalPartInSum));


    //Метки:
    Ext.getCmp('SumDocSecondHandPurch1Tabs2' + id).setValue(Ext.getCmp('SumDocSecondHandPurch1Tabs' + id).getValue());
    Ext.getCmp('SumDocSecondHandPurch2Tabs2' + id).setValue(Ext.getCmp('SumDocSecondHandPurch2Tabs' + id).getValue());
    Ext.getCmp('SumTotal2' + id).setValue(Ext.getCmp('SumTotal' + id).getValue());

};


function controllerDocSecondHandWorkshops_PanelGrid1_DiagnosticRresults(value, metaData, record) {
    var DiagnosticRresults = record.get('DiagnosticRresults'); if (DiagnosticRresults == null) DiagnosticRresults = "";
    metaData.tdAttr = 'data-qtip="' + DiagnosticRresults + '"';
    return value;
}

