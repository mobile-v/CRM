Ext.define("PartionnyAccount.controller.Sklad/Object/Doc/DocSecondHandPurches/controllerDocSecondHandRazbors", {
    //Расширить
    extend: "Ext.app.Controller",
    //views: ['Sys/viewContainerHeader'],

    init: function () {
        this.control({
            //Виджет (на котором расположены Грид и ...)
            //Закрыте
            'viewDocSecondHandRazbors': { close: this.this_close },


            //Сисок (itemId=tree)
            // Меню Списка
            'viewDocSecondHandRazbors [itemId=expandAll]': { click: this.onTree_expandAll },
            'viewDocSecondHandRazbors [itemId=collapseAll]': { click: this.onTree_collapseAll },
            'viewDocSecondHandRazbors [itemId=FolderNew]': { click: this.onTree_folderNew },
            'viewDocSecondHandRazbors [itemId=FolderNewSub]': { click: this.onTree_folderNewSub },
            'viewDocSecondHandRazbors [itemId=FolderEdit]': { click: this.onTree_FolderEdit },
            'viewDocSecondHandRazbors [itemId=FolderCopy]': { click: this.onTree_FolderCopy },
            'viewDocSecondHandRazbors [itemId=FolderDel]': { click: this.onTree_folderDel },
            // Клик по Группе
            'viewDocSecondHandRazbors [itemId=tree]': {
                selectionchange: this.onTree_selectionchange,
                itemclick: this.onTree_itemclick,
                itemdblclick: this.onTree_itemdblclick,

                //itemcontextmenu: this.onTree_contextMenuForTreePanel,
            },


            // PanelGrid0: Список Клик по Гриду
            'viewDocSecondHandRazbors [itemId=PanelGrid0_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick,

                edit: this.onPanelGrid0Edit,
            },
            

            // PanelGrid1: Список Клик по Гриду
            'viewDocSecondHandRazbors [itemId=PanelGrid1_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid2: Список Клик по Гриду
            'viewDocSecondHandRazbors [itemId=PanelGrid2_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid5: Список Клик по Гриду
            'viewDocSecondHandRazbors [itemId=PanelGrid5_]': {
                selectionchange: this.onGridX_selectionchange,
                itemclick: this.onGridX_itemclick,
                itemdblclick: this.onGridX_itemdblclick
            },
            // PanelGrid7: Список Клик по Гриду
            'viewDocSecondHandRazbors [itemId=PanelGrid7_]': {
                selectionchange: this.onGrid7_selectionchange,
                itemclick: this.onGrid7_itemclick,
                itemdblclick: this.onGrid7_itemdblclick
            },
            'viewDocSecondHandRazbors [itemId=PanelGrid8_]': {
                selectionchange: this.onGrid8_selectionchange,
                itemclick: this.onGrid8_itemclick,
                itemdblclick: this.onGrid8_itemdblclick
            },
            // PanelGrid9: Список Клик по Гриду
            'viewDocSecondHandRazbors [itemId=PanelGrid9_]': {
                selectionchange: this.onGrid9_selectionchange,
                itemclick: this.onGrid9_itemclick,
                itemdblclick: this.onGrid9_itemdblclick
            },
            'viewDocSecondHandRazbors #TriggerSearchGrid': {
                "ontriggerclick": this.onTriggerSearchGridClick1,
                "specialkey": this.onTriggerSearchGridClick2,
                "change": this.onTriggerSearchGridClick3
            },
            'viewDocSecondHandRazbors #DateS': { select: this.onGrid_DateS },
            'viewDocSecondHandRazbors #DatePo': { select: this.onGrid_DatePo },


            // Кнопки-статусы
            'viewDocSecondHandRazbors button#btnStatus12': { "click": this.onBtnStatusClick },
            'viewDocSecondHandRazbors button#btnStatus13': { "click": this.onBtnStatusClick },
            'viewDocSecondHandRazbors button#btnStatus11': { "click": this.onBtnStatusClick },

            'viewDocSecondHandRazbors button#btnPrint': { "click": this.onBtnPrintClick },



            // PanelGrid2
            'viewDocSecondHandRazbors [itemId=grid]': {
                selectionchange: this.onGrid2Selectionchange
            },
            'viewDocSecondHandRazbors button#btnGridDeletion2': { "click": this.onBtnGridDeletion2 },
            'viewDocSecondHandRazbors button#btnGridAddPosition2': { click: this.onBtnGridAddPosition2 },

            //Log *** *** ***
            // PanelGridLog
            'viewDocSecondHandRazbors button#btnPanelGridLogAdd': { click: this.onBtnPanelGridLogAdd },



            // === Кнопки: Сохранение (Выдача) === === ===
            //'viewDocSecondHandRazbors button#btnSave': { "click": this.onBtnSaveClick },
            'viewDocSecondHandRazbors button#btnSave': { click: this.onBtnSaveClick },
        });
    },
    


    //Только для "InterfaceSystem == 3" (layout: 'card')
    //Закрытие и сделать активным другой виджет
    this_close: function (aPanel) {
        funInterfaceSystem3_closePanel(aPanel);
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
        controllerDocSecondHandRazbors_onTree_folderNew(aButton.UO_id);
    },
    onTree_folderNewSub: function (aButton, aEvent) {
        controllerDocSecondHandRazbors_onTree_folderNewSub(aButton.UO_id);
    },
    onTree_FolderEdit: function (aButton, aEvent) {
        controllerDocSecondHandRazbors_onTree_folderEdit(aButton.UO_id);
    },
    onTree_FolderCopy: function (aButton, aEvent) {
        controllerDocSecondHandRazbors_onTree_folderCopy(aButton.UO_id);
    },
    onTree_folderDel: function (aButton, aEvent, aOptions) {
        controllerDocSecondHandRazbors_onTree_folderDel(aButton.UO_id);
    },

    // Селект Группы
    onTree_selectionchange: function (model, records) {
        model.view.ownerGrid.down("#FolderNewSub").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderCopy").setDisabled(records.length === 0);
        model.view.ownerGrid.down("#FolderDel").setDisabled(records.length === 0);
    },
    // Клик по Группе
    onTree_itemclick: function (view, rec, item, index, eventObj) {
        
    },
    onTree_itemdblclick: function (view, rec, item, index, eventObj) {

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




    // GridX: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***
       
    //Кнопки редактирования Енеблед
    onGridX_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGridX_itemclick: function (view, record, item, index, eventObj) {
        controllerDocSecondHandRazbors_onGridX_itemclick(view.grid, record, false); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGridX_itemdblclick: function (view, record, item, index, e) {
        //controllerDocSecondHandRazbors_onGridX_itemclick(view.grid, record, false);
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
        controllerDocSecondHandRazbors_onGridX_itemclick(view.grid, record, true); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGrid7_itemdblclick: function (view, record, item, index, e) {
        //controllerDocSecondHandRazbors_onGridX_itemclick(view.grid, record, true);
    },



    // Grid8: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGrid8_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGrid8_itemclick: function (view, record, item, index, eventObj) {
        controllerDocSecondHandRazbors_onGridX_itemclick(view.grid, record, true); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGrid8_itemdblclick: function (view, record, item, index, e) {
        //controllerDocSecondHandRazbors_onGridX_itemclick(view.grid, record, true);
    },



    // Grid9: Список Клик по Гриду *** *** *** *** *** *** *** *** *** ***

    //Кнопки редактирования Енеблед
    onGrid9_selectionchange: function (model, records) {
    },
    //Клик: Редактирования или выбор
    onGrid9_itemclick: function (view, record, item, index, eventObj) {
        controllerDocSecondHandRazbors_onGridX_itemclick(view.grid, record, true); //.UO_id
    },
    //ДаблКлик: Редактирования или выбор
    onGrid9_itemdblclick: function (view, record, item, index, e) {
        //controllerDocSecondHandRazbors_onGridX_itemclick(view.grid, record, true); //.UO_id
    },


    //Поиск
    onTriggerSearchGridClick1: function (aButton, aEvent) {
        if (Ext.getCmp("TriggerSearchGrid" + aButton.UO_id).getValue().length > 0) {
            funGridDoc_2(aButton.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=12&DirSecondHandStatusIDPo=14&DirWarehouseID=" + varDirWarehouseID);
        }
    },
    onTriggerSearchGridClick2: function (f, e) {
        if (e.getKey() == e.ENTER && Ext.getCmp("TriggerSearchGrid" + f.UO_id).getValue().length > 0) {
            funGridDoc_2(f.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=12&DirSecondHandStatusIDPo=14&DirWarehouseID=" + varDirWarehouseID);
        }
    },
    onTriggerSearchGridClick3: function (e, textReal, textLast) {
        if (textReal.length >= 1) funGridDoc_2(e.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=12&DirSecondHandStatusIDPo=14&DirWarehouseID=" + varDirWarehouseID);
    },
    //Даты
    onGrid_DateS: function (dataField, newValue, oldValue) {
        funGridDoc_2(dataField.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=12&DirSecondHandStatusIDPo=14&DirWarehouseID=" + varDirWarehouseID);
    },
    onGrid_DatePo: function (dataField, newValue, oldValue) {
        funGridDoc_2(dataField.UO_id, HTTP_DocSecondHandPurches + "?DirSecondHandStatusIDS=12&DirSecondHandStatusIDPo=14&DirWarehouseID=" + varDirWarehouseID);
    },
    

    // Кнопки-статусы *** *** *** *** *** *** *** *** *** *** *** *** ***
    onBtnStatusClick: function (aButton, aEvent, aOptions) {
        //var id = aButton.UO_id;


        //II. Если:
        //    1. На вкладке Архив
        //    2. Нажали кнопку "btnStatus12"
        //   то вывести форму с вопросом "Причины возврата на доработку" и записать причину в Лог
        var activeTab = Ext.getCmp("tab_" + aButton.UO_id).getActiveTab();
        if (activeTab.itemId == "PanelGrid9_" && aButton.itemId == "btnStatus12") {


            //Если "Срок гарантии прошёл", то написать об этом (C#: DateDone.AddMonths(docSecondHandPurch.ServiceTypeRepair) <= DateTime.Now)
            var locboolWarrantyPeriodPassed = false;
            var todayDate = new Date();
            var IdcallModelData = activeTab.getSelectionModel().getSelection(); if (IdcallModelData.length > 0) IdcallModelData = IdcallModelData[0].data;

            var locDateDone = new Date(IdcallModelData.DateDone);
            //locDateDone = Ext.Date.add(locDateDone, Ext.Date.MONTH, parseInt(IdcallModelData.ServiceTypeRepair));
            locDateDone = Ext.Date.add(locDateDone, Ext.Date.MONTH, 0);

            /*var locMsg = "";
            if (locDateDone <= todayDate)
            {
                locMsg = "<br /><span style='color:red'>Внимаение: Срок гарантии прошёл (до " + Ext.Date.format(locDateDone, "d-m-Y") + ")</span>";
            }*/



            Ext.Msg.prompt(lanOrgName, "Причины возврата на доработку ", // + locMsg,
                //height = 300,
                function (btnText, sReturnRresults) {
                    if (btnText === 'ok') {

                        //Запрос на сервер
                        controllerDocSecondHandRazbors_ChangeStatus_Request(aButton, 0, "Причина: " + sReturnRresults);

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
            controllerDocSecondHandRazbors_ChangeStatus_Request(aButton, 0);

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


    
    // PanelGrid2 *** *** *** *** *** *** *** *** *** *** *** *** *** ***

    onGrid2Selectionchange: function (model, records) {
        model.view.ownerGrid.down("#btnGridDeletion2").setDisabled(records.length === 0);
    },
    onBtnGridDeletion2: function (aButton, aEvent, aOptions) {

        var selection = Ext.getCmp("grid_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
        if (selection) {

            Ext.Ajax.request({
                timeout: varTimeOutDefault,
                waitMsg: lanUpload,
                url: HTTP_DocSecondHandRazbor2Tabs + selection.data.DocSecondHandRazbor2TabID + "/",
                method: 'DELETE',

                success: function (result) {
                    var sData = Ext.decode(result.responseText);
                    if (sData.success == false) {
                        Ext.Msg.alert(lanOrgName, sData.data);
                    }
                    else {
                        //Удалить запись в Гриде
                        var selection = Ext.getCmp("grid_" + aButton.UO_id).getView().getSelectionModel().getSelection()[0];
                        if (selection) { Ext.getCmp("grid_" + aButton.UO_id).store.remove(selection); }
                        //Обновить Лог
                        Ext.getCmp("gridLog0_" + aButton.UO_id).getStore().load();
                        controllerDocSecondHandRazbors_RecalculationSums(aButton.UO_id);
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
        
        /*
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
        ObjectEditConfig("viewDocSecondHandRazborNomens", Params);
        */

        controllerDocSecondHandRazbors_onTree_folderNew(id);

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


        var store = Ext.getCmp("grid_" + idMy).getStore();
        store.insert(store.data.items.length, rec.data);

        controllerDocSecondHandRazbors_RecalculationSums(idMy);




        //Запрос на сервер *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***

        rec.data.DocSecondHandPurchID = Ext.getCmp("DocSecondHandPurchID" + idMy).getValue();
        var dataX = Ext.encode(rec.data);
        //Сохранение
        Ext.Ajax.request({
            timeout: varTimeOutDefault,
            waitMsg: lanUpload,
            url: HTTP_DocSecondHandRazbor2Tabs,
            method: 'POST',
            params: { recordsDataX: dataX },

            success: function (result) {
                var sData = Ext.decode(result.responseText);
                if (sData.success == false) {
                    Ext.Msg.alert(lanOrgName, sData.data);
                }
                else {
                    //Получаем данные с Сервера
                    var locDocSecondHandRazbor2TabID = sData.data.DocSecondHandRazbor2TabID;
                    var DirEmployeeID = sData.data.DirEmployeeID;
                    var DirCurrencyID = sData.data.DirCurrencyID;
                    var DirCurrencyRate = sData.data.DirCurrencyRate;
                    var DirCurrencyMultiplicity = sData.data.DirCurrencyMultiplicity;

                    //Переменные
                    var grid = Ext.getCmp("grid_" + idMy);
                    var gridStore = grid.store;

                    //UO + меняем значение в "UO_GridRecord"
                    var UO_GridIndex = store.data.items.length - 1; //gridStore.indexOf(grid.getSelectionModel().getSelection()[0]); //UO_GridIndex: Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
                    var UO_GridRecord = rec; //grid.getSelectionModel().getSelection()[0]; //UO_GridRecord: Для загрузки данных в форму редактирования Табличной части
                    UO_GridRecord.data.DocSecondHandRazbor2TabID = locDocSecondHandRazbor2TabID
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
                    controllerDocSecondHandRazbors_RecalculationSums(grid.UO_id);
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

    // === Кнопки: Сохранение (Выдача) === === ===
    onBtnSaveClick: function (aButton, aEvent, aOptions) {

        //Подсчёт сумм
        var storeGrid = Ext.getCmp("grid_" + aButton.UO_id).getStore();
        var SumPurch = 0, SumRetail = 0;
        for (var i = 0; i < storeGrid.data.items.length; i++) {
            SumPurch += parseFloat(storeGrid.data.items[i].data.Quantity) * parseFloat(storeGrid.data.items[i].data.PriceCurrency);
            SumRetail += parseFloat(storeGrid.data.items[i].data.Quantity) * parseFloat(storeGrid.data.items[i].data.PriceRetailCurrency);
        }
        
        var sMsg = "";
        if (parseFloat(Ext.getCmp("PriceVAT" + aButton.UO_id).getValue()) > SumRetail) { sMsg = "<b style='color: red;'>Цена покупки аппарата больше цены продажи!</b>" + "<br />"; }


        Ext.MessageBox.show({
            icon: Ext.MessageBox.WARNING, //ERROR,INFO,QUESTION,WARNING
            width: 375,
            title: lanOrgName,

            msg:
            "Информация!" +
            "<br />" + 
            sMsg + 
            "Цена покупки аппарата: " + Ext.getCmp("PriceVAT" + aButton.UO_id).getValue() +
            "<br />" + 
            "Цена поступления запчастей: " + SumPurch +
            "<br />" +
            "Цена продажи запчастей: " + SumRetail,

            buttonText: { yes: "Переместить в архив!", no: "Не перемещать", cancel: "Отмена" },
            fn: function (btn) {
                if (btn == "yes") {
                    controllerDocSecondHandRazbors_ChangeStatus_Request(aButton, 0, "");
                }
                else if (btn == "no") {
                    //...
                }
            }
        });


    },

});


//Клик по ГридамX
function controllerDocSecondHandRazbors_onGridX_itemclick(view_grid, record, btnSave) {
    
    var id = view_grid.UO_id;
    var itemId = view_grid.itemId;
    var FromService = record.get('FromService');

    //Показываем список товара
    /*if (itemId == "PanelGrid0_") {
        Ext.getCmp("tree_" + id).expand(Ext.Component.DIRECTION_RIGHT, true);
    }*/

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
    //Ext.getCmp("SumOfVATCurrency" + id).setVisible(false);
    Ext.getCmp("btnSave" + id).setVisible(false);
    //Ext.getCmp("fsListObjectPFID" + id).setVisible(false);

    Ext.getCmp("grid_" + id).enable();

    Ext.getCmp("btnStatus12" + id).setText(""); Ext.getCmp("btnStatus12" + id).width = 50; Ext.getCmp("btnStatus12" + id).setPressed(false); Ext.getCmp("btnStatus12" + id).setVisible(true);
    Ext.getCmp("btnStatus13" + id).setVisible(true);
    //Ext.getCmp("btnStatus11" + id).setVisible(true);

    //Ext.getCmp("ServiceTypeRepair" + id).enable();
    Ext.getCmp("gridLog0_" + id).enable();

    
    //2. Делаем не видимым и не редактируемым!
    if (btnSave) {
        //Ext.getCmp("SumOfVATCurrency" + id).setVisible(true);
        Ext.getCmp("btnSave" + id).setVisible(true);

        Ext.getCmp("grid_" + id).disable();

        Ext.getCmp("btnStatus12" + id).setText("В диагностике"); Ext.getCmp("btnStatus12" + id).width = 125; //Ext.getCmp("btnStatus12" + id).setVisible(false);
        Ext.getCmp("btnStatus13" + id).setVisible(false);
        //Ext.getCmp("btnStatus11" + id).setVisible(false);

        //Если Архив
        var activeTab = Ext.getCmp("tab_" + id).getActiveTab();
        if (IdcallModelData.DirSecondHandStatusID == 14 || activeTab.itemId == "PanelGrid9_") {
            //Ext.getCmp("ServiceTypeRepair" + id).disable();
            Ext.getCmp("gridLog0_" + id).disable();
            Ext.getCmp("btnSave" + id).setVisible(false);
            //Ext.getCmp("fsListObjectPFID" + id).setVisible(true);
            Ext.getCmp("btnStatus12" + id).setText("<b>Вернуть на доработку</b>"); Ext.getCmp("btnStatus12" + id).setWidth(200);

            //Если не архив, то убрать эту кнопку
            if (IdcallModelData.DirSecondHandStatusID != 14) {
                Ext.getCmp("btnStatus12" + id).setVisible(false);
            }
        }
        //Если "Наразбор"
        if (IdcallModelData.DirSecondHandStatusID == 14 || activeTab.itemId == "PanelGrid8_") {
            Ext.getCmp("btnSave" + id).setVisible(false);
        }

    }


    //Меняем формат датв, а то глючит!
    Ext.getCmp("DocDate" + id).format = "c";


    var widgetX = Ext.getCmp("viewDocSecondHandRazbors" + id);

    //Запчасть
    widgetX.storeGrid.setData([], false);
    widgetX.storeGrid.proxy.url = HTTP_DocSecondHandRazbor2Tabs + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID;
    widgetX.storeGrid.UO_Loaded = false;

    //Лог
    widgetX.storeLogSecondHandsGrid0.setData([], false);
    widgetX.storeLogSecondHandsGrid0.proxy.url = HTTP_LogSecondHands + "?DocSecondHandPurchID=" + IdcallModelData.DocSecondHandPurchID;
    widgetX.storeLogSecondHandsGrid0.UO_Loaded = false;


    //Форма
    var widgetXForm = Ext.getCmp("form_" + id);
    widgetXForm.form.url = HTTP_DocSecondHandPurches + IdcallModelData.DocSecondHandPurchID + "/?DocID=" + IdcallModelData.DocID; //widgetXForm.form.url = HTTP_DocSecondHandPurches + IdcallModelData.DocSecondHandPurchID + "/?DocID=" + IdcallModelData.DocID; //С*ка глючит фреймворк и присвивает в форме старый УРЛ!!!
    widgetXForm.setVisible(true);
    widgetXForm.reset();
    widgetXForm.UO_Loaded = false;

    
    //Лоадер
    var loadingMask = new Ext.LoadMask({ msg: 'Please wait...', target: widgetX });
    loadingMask.show();

    widgetX.storeGrid.load({ waitMsg: lanLoading });
    widgetX.storeGrid.on('load', function () {
        if (widgetX.storeGrid.UO_Loaded) { loadingMask.hide(); return; }
        widgetX.storeGrid.UO_Loaded = true;

        if (widgetXForm.UO_Loaded) { loadingMask.hide(); return; }

        loadingMask.hide();
        
        widgetXForm.load({
            method: "GET",
            timeout: varTimeOutDefault,
            waitMsg: lanLoading,
            //url: HTTP_DocSecondHandPurches + IdcallModelData.DocSecondHandPurchID + "/?DocID=" + IdcallModelData.DocID,
            success: function (form, action) {

                //Статусы и Кнопки
                controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeButton(id);

                //Меняем статус в самой таблице
                /*if (IdcallModelData.DirSecondHandStatusID == 1) { //if (parseInt(Ext.getCmp("DirSecondHandStatusID" + id).getValue()) == 1) {
                    //Меняем статус
                    var storeX = Ext.getCmp(itemId + id).getSelectionModel().getSelection();
                    storeX[0].data.DirSecondHandStatusID = 2;
                    //Сохраняем
                    Ext.getCmp(itemId + id).getView().refresh();
                }*/
                    
                widgetXForm.UO_Loaded = true;
                widgetX.focus(); //Фокус на открывшийся Виджет

                //Log
                widgetX.storeLogSecondHandsGrid0.load({ waitMsg: lanLoading });

                //Запчасти
                controllerDocSecondHandRazbors_RecalculationSums(id);

            },
            failure: function (form, action) {
                funPanelSubmitFailure(form, action);
                widgetX.focus(); //Фокус на открывшийся Виджет
            }

        });
    });

}

//Смена Статуса
function controllerDocSecondHandRazbors_ChangeStatus_Request(aButton, DirPaymentTypeID, sReturnRresults, PriceX3) {

    if (DirPaymentTypeID == undefined) DirPaymentTypeID = 0;
    if (sReturnRresults == undefined) sReturnRresults = "";
    //Старый ID-шние статуса
    var locDirSecondHandStatusID_OLD = parseInt(Ext.getCmp("DirSecondHandStatusID" + aButton.UO_id).getValue());

    //Новый ID-шние статуса
    var locDirSecondHandStatusID = parseInt(controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, false));
    if (isNaN(locDirSecondHandStatusID)) { return; }


    var sUrl = HTTP_DocSecondHandPurches + Ext.getCmp("DocSecondHandPurchID" + aButton.UO_id).getValue() + "/" + locDirSecondHandStatusID + "/?DirPaymentTypeID=" + DirPaymentTypeID + "&SumTotal2a=" + Ext.getCmp("SumOfVATCurrency" + aButton.UO_id).getValue() + "&sReturnRresults=" + sReturnRresults;
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
                controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeButton(aButton.UO_id);
                Ext.Msg.alert(lanOrgName, sData.data);
            }
            else {
                //Меняем ID-шние статуса
                controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeStatus(aButton.UO_id, aButton.itemId, true);

                //Статусы и Кнопки
                controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeButton(aButton.UO_id);

                //Сообщение
                if (locDirSecondHandStatusID == 14) {

                    Ext.Msg.alert(lanOrgName, "Аппарат перемещён в архив!");

                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    Ext.getCmp("PanelGrid7_" + aButton.UO_id).getStore().load();


                    // *** Печатніе формы ***

                    //Проверка: если форма ещё не сохранена, то выход
                    if (Ext.getCmp("DocSecondHandPurchID" + aButton.UO_id).getValue() == null) { Ext.Msg.alert(lanOrgName, txtMsg066); return; }
                }
                //SMS
                else if (locDirSecondHandStatusID == 13) {
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    Ext.getCmp("PanelGrid7_" + aButton.UO_id).getStore().load();
                }
                else if (locDirSecondHandStatusID == 12) {
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    Ext.getCmp("PanelGrid7_" + aButton.UO_id).getStore().load();
                }
                else if (locDirSecondHandStatusID == 13 && locDirSecondHandStatusID_OLD == 14) {
                    //Обновить Грид "Архив"
                    Ext.getCmp("PanelGrid9_" + aButton.UO_id).getStore().load();
                    //Закрыть форму редактирование
                    Ext.getCmp("form_" + aButton.UO_id).setVisible(false);
                    return;
                }
                else if (locDirSecondHandStatusID == 14 && locDirSecondHandStatusID_OLD == 13) {
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
            controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeButton(aButton.UO_id);

            var sData = Ext.decode(result.responseText);
            Ext.Msg.alert(lanOrgName, sData.ExceptionMessage);
        }
    });
}
//Статусы и Кнопки - выставить
function controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeButton(id)
{
    switch (parseInt(Ext.getCmp("DirSecondHandStatusID" + id).getValue())) {
        case 1:
            //Принят
            Ext.Msg.alert(lanOrgName, "Статус сменён на: Предпродажа");

            Ext.getCmp("btnStatus12" + id).setPressed(true);
            Ext.getCmp("btnStatus13" + id).setPressed(false);
            //Ext.getCmp("btnStatus11" + id).setPressed(false);

            break;
        case 2:
            //В диагностике
            Ext.getCmp("btnStatus12" + id).setPressed(true);
            Ext.getCmp("btnStatus13" + id).setPressed(false);
            //Ext.getCmp("btnStatus11" + id).setPressed(false);
            break;
        case 7:
            //Отремонтирован
            Ext.getCmp("btnStatus12" + id).setPressed(false);
            Ext.getCmp("btnStatus13" + id).setPressed(true);
            //Ext.getCmp("btnStatus11" + id).setPressed(false);
            break;
        case 8:
            //Отказной
            Ext.getCmp("btnStatus12" + id).setPressed(false);
            Ext.getCmp("btnStatus13" + id).setPressed(false);
            //Ext.getCmp("btnStatus11" + id).setPressed(true);
            break;
    }
}
//Вернуть и/или поменять "DirSecondHandStatusID"
function controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeStatus(id, itemId, bchange) {
    
    var grid_ = Ext.getCmp("grid_" + id).getStore().data;

    switch (itemId) {
        case "btnStatus12":
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(2); }
            else { return 12; }
            break;
        case "btnStatus13":
            //Если нет ни одной выполненной работы, то не пускать сохранять и выдать эксепшн
            if (grid_.length == undefined || grid_.length == 0) { Ext.Msg.alert(lanOrgName, "Для статуса готов, должна присутствовать в списке запчастей, хотя бы одна запчасть!"); controllerDocSecondHandRazbors_DirSecondHandStatusID_ChangeButton(id); return; }
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(7); }
            else { return 13; }
            break;
        case "btnStatus11":
            if (bchange) { Ext.getCmp("DirSecondHandStatusID" + id).setValue(8); }
            else { return 11; }
            break;

        case "btnSave":
            return 14;
            break;
    }
}


//Поиск в Архиве
function controllerDocSecondHandRazbors_Search_Archiv(id, DirSmsTemplateTypeS, DirSmsTemplateTypePo) {

    if (Ext.getCmp("TriggerSearchTree" + aButton.UO_id).getValue() == "") return;
    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).disable(); //Кнопку поиска делаем не активной


    var TriggerSearchTree = Ext.getCmp("TriggerSearchTree" + aButton.UO_id).value;



    Ext.getCmp("TriggerSearchTree" + aButton.UO_id).enable(); //Кнопку поиска делаем не активной

}

//Функция пересчета Сумм
//И вывода сообщения о пересчете Налога, если меняли "Налог из ..."
//Заполнить 2-а поля (id, rec)
//ShowMsg - выводить сообщение при смене налоговой ставик (в основном используется для смены "Налог из ...")
function controllerDocSecondHandRazbors_RecalculationSums(id) {
    //2. Подсчет табличной части Работы "SumOfVATCurrency"
    var storeGrid = Ext.getCmp(Ext.getCmp("form_" + id).UO_idMain).storeGrid;
    var SumOfVATCurrency = 0;
    for (var i = 0; i < storeGrid.data.items.length; i++) {
        SumOfVATCurrency += parseFloat(storeGrid.data.items[i].data.Quantity) * parseFloat(storeGrid.data.items[i].data.PriceCurrency);
    }
    Ext.getCmp('SumOfVATCurrency' + id).setValue(SumOfVATCurrency.toFixed(varFractionalPartInSum));
};




// === Функции === === ===
//1. Для Товара - КонтекстМеню
function controllerDocSecondHandRazbors_onTree_folderNew(id) {
    var Params = [
        "grid_" + id,
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
    ObjectEditConfig("viewDirNomensWinComboEdit", Params);
};
function controllerDocSecondHandRazbors_onTree_folderEdit(id) {
    var Params = [
        "grid_" + id,
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
    ObjectEditConfig("viewDirNomensWinComboEdit", Params);
};
function controllerDocSecondHandRazbors_onTree_folderNewSub(id) {
    var node = funReturnNode(id);
    //Ext.getCmp("Sub" + id).setValue(node.data.id);

    var Sub = node.data.id;

    var Params = [
        "grid_" + id,
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
    ObjectEditConfig("viewDirNomensWinComboEdit", Params);
};
function controllerDocSecondHandRazbors_onTree_folderCopy(id) {
    Ext.MessageBox.show({
        title: lanOrgName, msg: "Создать копию записи?", icon: Ext.MessageBox.QUESTION, buttons: Ext.Msg.YESNO, width: 300, closable: false,
        fn: function (buttons) {
            if (buttons == "yes") {

                var Params = [
                    "grid_" + id,
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
                ObjectEditConfig("viewDirNomensWinComboEdit", Params);

            }
        }
    });
};
function controllerDocSecondHandRazbors_onTree_folderDel(id) {
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
function controllerDocSecondHandRazbors_onTree_folderSubNull(id) {
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
function controllerDocSecondHandRazbors_onTree_addSub(id) {

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


