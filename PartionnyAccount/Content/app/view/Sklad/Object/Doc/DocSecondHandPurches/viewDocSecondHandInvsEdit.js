﻿Ext.define("PartionnyAccount.view.Sklad/Object/Doc/DocSecondHandPurches/viewDocSecondHandInvsEdit", {

    extend: InterfaceSystemObjName,
    alias: "widget.viewDocSecondHandInvsEdit",

    layout: "border",
    region: "center",
    title: "Б/У: Инвентаризация ",
    width: 850, height: 525,
    autoScroll: false,
    buttonAlign: 'left',
    bodyStyle: 'background:white;',
    bodyPadding: varBodyPadding,

    UO_maximize: false,  //Максимизировать во весь экран
    UO_Center: false,    //true - в центре экрана, false - окна каскадом
    UO_Modal: false,     //true - Все остальные элементы не активные
    UO_Function_Tree: undefined,  //Fn - если открыли для выбора или из Tree
    UO_Function_Grid: undefined,  //Fn - если открыли для выбора или из Грида

    //Контроллер
    controller: 'viewcontrollerDocSecondHandInvsEdit',
    listeners: { close: 'onViewDocSecondHandInvsEditClose' },

    conf: {},

    initComponent: function () {

        //1. Дата + Поиск
        var PanelSearch = Ext.create('Ext.panel.Panel', {
            id: "PanelSearch_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            //region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',

            defaults: { anchor: '100%' },
            width: "100%",
            autoScroll: true,

            //width: "100%", height: "100%", //width: 500, height: 200,
            //split: true,

            items: [

                // *** *** *** Not Visible *** *** *** *** *** *** *** ***
                /*
                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                //{ xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirPriceTypeID", name: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                */
                // *** *** *** Not Visible *** *** *** *** *** *** *** ***



                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [

                        { xtype: 'textfield', margin: "0 0 0 15", name: "DirNomenPatchFull", id: "DirNomenPatchFull" + this.UO_id, readOnly: true, flex: 2, allowBlank: true }, //, fieldLabel: ""
                        {
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                            xtype: 'button', tooltip: "Reload", iconCls: "button-image-reload",
                            id: "btnDirNomenReload" + this.UO_id, itemId: "btnDirNomenReload",
                            listeners: { click: "onBtnDirNomenReloadClick" }
                        },

                        {
                            xtype: 'viewComboBox',
                            allowBlank: true, flex: 1,

                            store: new Ext.data.SimpleStore({
                                fields: ['SearchType', 'SearchTypeName'],
                                data: SearchType_values
                            }),

                            valueField: 'SearchType',
                            hiddenName: 'SearchType',
                            displayField: 'SearchTypeName',
                            name: 'SearchType', itemId: "SearchType", id: "SearchType" + this.UO_id,
                            UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                        },

                        //"->",
                        {
                            id: "TriggerSearchGrid" + this.UO_id,
                            UO_id: this.UO_id,
                            UO_idMain: this.UO_idMain,
                            UO_idCall: this.UO_idCall,

                            xtype: 'viewTriggerSearchGrid',
                            emptyText: "Поиск ...", allowBlank: true, flex: 1,
                            name: 'TriggerSearchGrid', id: "TriggerSearchGrid" + this.UO_id, itemId: "TriggerSearchGrid",
                            listeners: { ontriggerclick: "onTriggerSearchGridClick1", specialkey: "onTriggerSearchGridClick2", change: "onTriggerSearchGridClick3" }
                        },

                    ]
                },

            ]
        });



        //Tab
        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 
        var SpisatS_values = [
            [1, 'Точка'],
            [2, 'Сотрудник']
        ];
        var PanelDocumentDetails = Ext.create('Ext.panel.Panel', {
            id: "PanelDocumentDetails_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,

            region: "center", //!!! Важно для Ресайз-а !!!
            bodyStyle: 'background:transparent;',
            //title: lanPrimary,
            frame: true,
            monitorValid: true,
            defaultType: 'textfield',
            //bodyPadding: 5,

            //layout: 'anchor',
            //defaults: { anchor: '100%' },
            layout: {
                type: 'hbox',
                align: 'stretch'
            },

            width: "100%", height: 75 + varBodyPadding, //width: "100%", height: 115 + varBodyPadding,
            autoScroll: true,
            //split: true,

            items: [

                // *** *** *** Not Visible *** *** *** *** *** *** *** ***

                { xtype: 'textfield', fieldLabel: "DocID2", name: 'DocID2', id: 'DocID2' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },  //, hidden: true
                { xtype: 'textfield', fieldLabel: "Held", name: 'Held', id: 'Held' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocID", name: "DocID", id: "DocID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DocSecondHandInvID", name: "DocSecondHandInvID", id: "DocSecondHandInvID" + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                { xtype: 'textfield', fieldLabel: "DirContractorIDOrg", name: "DirContractorIDOrg", id: "DirContractorIDOrg" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                //{ xtype: 'textfield', fieldLabel: "DirWarehouseID", name: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },
                { xtype: 'textfield', fieldLabel: "DirPriceTypeID", name: "DirPriceTypeID", id: "DirPriceTypeID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true, hidden: true },

                // *** *** *** Not Visible *** *** *** *** *** *** *** ***


                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    hidden: true,
                    items: [
                        { xtype: 'textfield', fieldLabel: "№", name: "DocSecondHandInvID", id: "DocSecondHandInvID" + this.UO_id, readOnly: true, flex: 1, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: lanManual, name: "NumberInt", id: "NumberInt" + this.UO_id, margin: "0 0 0 5", flex: 1, allowBlank: true },
                    ]
                },



                //Для растояния между Контейнерами
                { xtype: 'container', height: 5 },

                {
                    xtype: 'panel',
                    flex: 1,
                    items: [
                        {
                            xtype: 'container', layout: { align: 'stretch', type: 'hbox' },
                            items: [

                                { xtype: 'viewDateField', fieldLabel: lanDateCounterparty, labelAlign: "top", name: "DocDate", id: "DocDate" + this.UO_id, width: 105, margin: "0 0 0 5", allowBlank: false, editable: false },

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Точка", labelAlign: "top", flex: 1, allowBlank: false, //, emptyText: "..."
                                    margin: "0 0 0 5",
                                    store: this.storeDirWarehousesGrid, // store getting items from server
                                    valueField: 'DirWarehouseID',
                                    hiddenName: 'DirWarehouseID',
                                    displayField: 'DirWarehouseName',
                                    name: 'DirWarehouseID', itemId: "DirWarehouseID", id: "DirWarehouseID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    editable: false, typeAhead: false, minChars: 200,
                                },


                                //SpisatS
                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "ЗП: списать с ", emptyText: "...", labelAlign: "top", allowBlank: false, flex: 1, width: "100%",
                                    margin: "0 0 0 5",
                                    store: new Ext.data.SimpleStore({
                                        fields: ['SpisatS', 'SpisatSName'],
                                        data: SpisatS_values
                                    }),
                                    valueField: 'SpisatS',
                                    hiddenName: 'SpisatS',
                                    displayField: 'SpisatSName',
                                    name: 'SpisatS', itemId: "SpisatS", id: "SpisatS" + this.UO_id,
                                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    listeners: {
                                        //select: 'onSpisatS_select',
                                        change: 'onSpisatS_changet',
                                    },
                                    readOnly: true,
                                },

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Сотрудник", labelAlign: "top", flex: 1, allowBlank: true, disabled: true, //, emptyText: "..."
                                    margin: "0 0 0 5",
                                    store: this.storeDirEmployeesGrid, // store getting items from server
                                    valueField: 'DirEmployeeID',
                                    hiddenName: 'DirEmployeeID',
                                    displayField: 'DirEmployeeName',
                                    name: 'SpisatSDirEmployeeID', itemId: "SpisatSDirEmployeeID", id: "SpisatSDirEmployeeID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    editable: false, typeAhead: false, minChars: 200,
                                },

                            ]
                        },
                    ]
                },


                {
                    xtype: 'panel',
                    flex: 1,
                    items: [

                        {
                            xtype: 'container', layout: { align: 'stretch', type: 'hbox' },
                            items: [
                                { xtype: 'label', text: "Подписи: " },

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Товаровед", labelAlign: "top", flex: 1, allowBlank: true, readOnly: true, //, emptyText: "..."
                                    margin: "0 0 0 5",
                                    store: this.storeDirEmployeesGrid, // store getting items from server
                                    valueField: 'DirEmployeeID',
                                    hiddenName: 'DirEmployeeID',
                                    displayField: 'DirEmployeeName',
                                    name: 'DirEmployee1ID', itemId: "DirEmployee1ID", id: "DirEmployee1ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    editable: false, typeAhead: false, minChars: 200,
                                },
                                {
                                    xtype: 'button', width: 35, tooltip: "Подпись", iconCls: "button-image-edit", itemId: "btn1Podpis", id: "btn1Podpis" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    disabled: true,
                                    listeners: { click: "onBtn1PodpisClick" }
                                },
                                {
                                    xtype: 'button', width: 35, tooltip: "Отменить подпись", iconCls: "button-image-delete", itemId: "btn11Podpis", id: "btn11Podpis" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    disabled: true,
                                    listeners: { click: "onBtn11PodpisClick" }
                                },
                                //{ xtype: 'textfield', fieldLabel: "Подпись-1", name: "DirEmployee1Podpis", id: "DirEmployee1Podpis" + this.UO_id, readOnly: true, hidden: true, allowBlank: true },
                                { xtype: "checkbox", boxLabel: "Подпись-1", name: "DirEmployee1Podpis", itemId: "DirEmployee1Podpis", flex: 1, id: "DirEmployee1Podpis" + this.UO_id, inputValue: true, readOnly: true, hidden: true, allowBlank: true },

                                {
                                    xtype: 'viewComboBox',
                                    fieldLabel: "Администратор точки", labelAlign: "top", flex: 1, allowBlank: true, //readOnly: true, //, emptyText: "..."
                                    margin: "0 0 0 15",
                                    store: this.storeDirEmployees2Grid, // store getting items from server
                                    valueField: 'DirEmployeeID',
                                    hiddenName: 'DirEmployeeID',
                                    displayField: 'DirEmployeeName',
                                    name: 'DirEmployee2ID', itemId: "DirEmployee2ID", id: "DirEmployee2ID" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    //Поиск
                                    editable: false, typeAhead: false, minChars: 200,

                                    listeners: { select: "onDirEmployee2IDSelect" }
                                },
                                {
                                    xtype: 'button', width: 35, tooltip: "Подпись", iconCls: "button-image-edit", itemId: "btn2Podpis", id: "btn2Podpis" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    disabled: true,
                                    listeners: { click: "onBtn2PodpisClick" }
                                },
                                {
                                    xtype: 'button', width: 35, tooltip: "Отменить подпись", iconCls: "button-image-delete", itemId: "btn21Podpis", id: "btn21Podpis" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                                    disabled: true,
                                    listeners: { click: "onBtn21PodpisClick" }
                                },
                                //{ xtype: 'textfield', fieldLabel: "Подпись-2", name: "DirEmployee2Podpis", id: "DirEmployee2Podpis" + this.UO_id, readOnly: true, hidden: true, allowBlank: true },
                                { xtype: "checkbox", boxLabel: "Подпись-2", name: "DirEmployee2Podpis", itemId: "DirEmployee2Podpis", flex: 1, id: "DirEmployee2Podpis" + this.UO_id, inputValue: true, readOnly: true, hidden: true, allowBlank: true },

                            ]
                        },

                    ]
                },

            ]
        });
        
        //*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 
        

        //3. Грид
        var LoadXFrom_values = [
            [0, "Все"],
            [1, "Куплен"],
            //[2, "Предпродажа (мастерская)"],
            //[3, "На согласовании"],
            //[4, "Согласовано"],
            [5, "Ожидает запчастей"],
            [7, "Готов для продажи"],
            [8, "На разбор"],
            [9, "В продаже"],
        ];
        var rowEditing1 = Ext.create('Ext.grid.plugin.RowEditing');
        var PanelGrid = Ext.create('Ext.grid.Panel', {
            id: "grid_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, UO_View: "viewDocSecondHandInvsEdit",
            itemId: "grid",

            conf: {},

            //region: "center", //!!! Важно для Ресайз-а !!!
            //autoScroll: true,
            flex: 1,
            split: true,

            store: this.storeGrid, //storeDocRetailTabsGrid,

            features: [{
                ftype: 'summary',
                dock: 'bottom'
            }],
            columns: [

                //{ text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                //Документ БУ
                { text: "Документ", dataIndex: "DocSecondHandPurchID", width: 75, style: "height: 25px;", tdCls: 'x-change-cell' },
                { text: "Статус", dataIndex: "DirSecondHandStatusName", width: 200, tdCls: 'x-change-cell' },
                { text: "Дата", dataIndex: "DocDate", width: 100, tdCls: 'x-change-cell' },
               

                //Товар
                { text: "Код", dataIndex: "DirServiceNomenID", width: 50, hidden: true, style: "height: 25px;", tdCls: 'x-change-cell' },
                { text: "Товар", dataIndex: "DirServiceNomenName", flex: 1, tdCls: 'x-change-cell' }, //flex: 1
                //Партия с которой списали
                { text: "Партия", dataIndex: "Rem2PartyID", width: 75, hidden: true, tdCls: 'x-change-cell' },
                //К-во + Цена (приходная)
                { text: lanCount, dataIndex: "Quantity", width: 75, summaryType: 'sum', tdCls: 'x-change-cell', hidden: true },
                { text: "Тип цены", dataIndex: "DirPriceTypeName", width: 75, hidden: true, tdCls: 'x-change-cell' },
                { text: "Цена", dataIndex: "PriceCurrency", width: 125, tdCls: 'x-change-cell' },
                { text: lanPriceSale, dataIndex: "PriceRetailCurrency", width: 125, tdCls: 'x-change-cell' },



                {
                    text: "Действие", dataIndex: "ExistName", width: 140, tdCls: 'x-change-cell102',
                    editor: {
                        xtype: 'combo',
                        UO_idNumber: this.UO_id, UO_id: this.UO_id, 
                        name: "ExistName",

                        triggerAction: 'all', // query all records on trigger click
                        minChars: 2, // minimum characters to start the search
                        enableKeyEvents: true, // otherwise we will not receive key events 
                        pageSize: 9990000,
                        queryMode: 'local',
                        resizable: false, // make the drop down list resizable
                        minListWidth: 220, // we need wider list for paging toolbar
                        //allowBlank: false, // force user to fill something
                        typeAhead: false,
                        hideTrigger: false,
                        editable: false,
                        width: '95%',
                        editable: false,

                        valueField: 'ExistName',
                        hiddenName: 'ExistName',
                        displayField: 'ExistName',

                        store: ExistName_store,
                        UO_store: this.storeGrid,

                        tpl: [
                            '<tpl for=".">',
                            '<div class="x-boundlist-item">',
                            '<img src="{img}"/> {ExistName}',
                            '</div>',
                            '</tpl>'
                        ],
                        listeners: {

                            change: function (comboBox, newRecord, oldRecord, par4) {

                            },

                            blur: function (field, event, eOpts) {
   
                            },

                        }

                    }
                },
                
            ],


            tbar: [

                //LoadXFrom
                {
                    xtype: 'viewComboBox',
                    allowBlank: false, width: 300,
                    margin: "0 0 0 5",
                    store: new Ext.data.SimpleStore({
                        fields: ['LoadXFrom', 'LoadXFromName'],
                        data: LoadXFrom_values
                    }),
                    valueField: 'LoadXFrom',
                    hiddenName: 'LoadXFrom',
                    displayField: 'LoadXFromName',
                    name: 'LoadXFrom', itemId: "LoadXFrom", id: "LoadXFrom" + this.UO_id,
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    listeners: {
                        //select: 'onLoadXFrom_select',
                        change: 'onLoadXFrom_changet',
                    }
                },
                { xtype: 'textfield', fieldLabel: "LoadFrom", name: 'LoadFrom', id: 'LoadFrom' + this.UO_id, readOnly: true, allowBlank: true, hidden: true },

                //Загрузить все остатки
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/refresh16.png', tooltip: "Обновить", style: "width: 40px; height: 40px;",
                    id: "btnGridRefresh" + this.UO_id, itemId: "btnGridRefresh",
                    listeners: { click: 'onGrid_BtnGridRefresh' }
                },

                " ", " ", " ", " ", " ",
                {
                    UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
                    xtype: "button",
                    icon: '../Scripts/sklad/images/table_delete.png', text: lanDelete, tooltip: lanDeletionFlag + "?", disabled: true,
                    id: "btnGridDeletion" + this.UO_id, itemId: "btnGridDelete",
                    listeners: { click: 'onGrid_BtnGridDelete' }
                },

            ],

            
            //Формат даты
            viewConfig: {
                getRowClass: function (record, index) {


                    /*
                    if (record.store.UO_viewConfig) {
                        var SumOfVATCurrency1 = 0, SumOfVATCurrency2 = 0;
                        for (var i = 0; i < record.store.model.fields.length; i++) {
                            debugger;
                            if (record.store.data.items[i].data.ExistName == "Списывается с ЗП") {
                                SumOfVATCurrency1 += record.store.data.items[i].data.PriceCurrency;
                            }
                            else if (record.store.data.items[i].data.ExistName == "На разбор") {
                                SumOfVATCurrency2 += record.store.data.items[i].data.PriceCurrency;
                            }
                        }
                        
                        Ext.getCmp("SumOfVATCurrency1" + record.store.UO_id).setValue(SumOfVATCurrency1);
                        Ext.getCmp("SumOfVATCurrency2" + record.store.UO_id).setValue(SumOfVATCurrency2);
                    }
                    */

                    /*
                    if (record.store.data.items[index].data.ExistName == "Списывается с ЗП") {
                        Ext.getCmp("SumOfVATCurrency1" + record.store.UO_id).setValue(
                            parseFloat(Ext.getCmp("SumOfVATCurrency1" + record.store.UO_id).getValue()) + 
                            parseFloat(record.store.data.items[index].data.PriceCurrency)
                        );
                    }
                    else if (record.store.data.items[index].data.ExistName == "На разбор") {
                        Ext.getCmp("SumOfVATCurrency2" + record.store.UO_id).setValue(
                            parseFloat(Ext.getCmp("SumOfVATCurrency2" + record.store.UO_id).getValue()) +
                            parseFloat(record.store.data.items[index].data.PriceCurrency)
                        )
                    }
                    */



                    var
                        iCount1 = 0, sSum1 = 0,
                        iCount2 = 0, sSum2 = 0,
                        iCount3 = 0, sSum3 = 0;
                    for (var i = 0; i < record.store.data.length; i++) {
                        if (record.store.data.items[i].data.ExistName == "Отсутствует") { //Списывается с ЗП
                            sSum1 += record.store.data.items[i].data.PriceCurrency
                            iCount1++;
                        }
                        else if (record.store.data.items[i].data.ExistName == "На разбор") {
                            sSum2 += record.store.data.items[i].data.PriceCurrency
                            iCount2++
                        }

                        iCount3++;
                        sSum3 += record.store.data.items[i].data.PriceCurrency
                    }
                    Ext.getCmp("iCount1" + record.store.UO_id).setValue(iCount1);
                    Ext.getCmp("SumOfVATCurrency1" + record.store.UO_id).setValue(sSum1);

                    Ext.getCmp("iCount2" + record.store.UO_id).setValue(iCount2);
                    Ext.getCmp("SumOfVATCurrency2" + record.store.UO_id).setValue(sSum2);

                    Ext.getCmp("iCount3" + record.store.UO_id).setValue(iCount3);
                    Ext.getCmp("SumOfVATCurrency3" + record.store.UO_id).setValue(sSum3);



                    // 1. === Исправляем формат даты: "yyyy-MM-dd HH:mm:ss" => "yyyy-MM-dd"  ===  ===  ===  ===  === 
                    for (var i = 0; i < record.store.model.fields.length; i++) {
                        //Если поле типа "Дата"
                        if (record.store.model.fields[i].type == "date") {
                            //Если есть дата, может быть пустое значение
                            if (record.data[record.store.model.fields[i].name] != null) {

                                if (record.data[record.store.model.fields[i].name].length != 10) {
                                    //Ext.Date.format
                                    record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                                else {
                                    //Рабочий метод, но нет смысла использовать
                                    //Ext.Date.parse and Ext.Date.format
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.parse(record.data[record.store.model.fields[i].name], DateFormatStr);
                                    //record.data[record.store.model.fields[i].name] = Ext.Date.format(new Date(record.data[record.store.model.fields[i].name]), DateFormatStr);
                                }
                            }
                        }
                    }

                    //2.  === Стили ===  ===  ===  ===  === 
                    if (record.get('ExistName') == 'Присутствует') {
                        return 'prisutstvuet';
                    }
                    /*else if(record.get('ExistName') == 'Списывается с ЗП') {
                        return 'spis-s-zp';
                    }*/
                    else if (record.get('ExistName') == 'Отсутствует') {
                        return 'spis-s-zp'; //return 'otsutstvuet';
                    }
                    else {
                        return 'razbor';
                    }


                }, //getRowClass

                stripeRows: true,

            }, //viewConfig
            


            plugins: [rowEditing1],
            rowEditing1: rowEditing1,


            listeners: { selectionchange: 'onGrid_selectionchange' },

        });


        //3. Футер
        var PanelFooter = Ext.create('Ext.panel.Panel', {
            id: "PanelFooter_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            //region: "south",
            bodyStyle: 'background:transparent;',

            defaultType: 'textfield',
            bodyPadding: 5,
            layout: 'anchor',
            defaults: { anchor: '100%' },
            //split: true,

            items: [
                {
                    xtype: 'container', width: "95%", layout: { align: 'stretch', type: 'hbox' },
                    items: [
                        //Суммы
                        { xtype: 'textfield', fieldLabel: "Отсутствует", name: "iCount1", id: "iCount1" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, width: 175, readOnly: true, allowBlank: true },
                        { xtype: 'textfield', fieldLabel: "", name: "SumOfVATCurrency1", id: "SumOfVATCurrency1" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, width: 150, readOnly: true, allowBlank: true, margin: "0 0 0 5" },

                        { xtype: 'textfield', fieldLabel: "Разбор", name: "iCount2", id: "iCount2" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, width: 175, readOnly: true, allowBlank: true, margin: "0 0 0 50", hidden: true },
                        { xtype: 'textfield', fieldLabel: "", name: "SumOfVATCurrency2", id: "SumOfVATCurrency2" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, width: 150, readOnly: true, allowBlank: true, margin: "0 0 0 5", hidden: true },

                        { xtype: 'textfield', fieldLabel: "Всего", name: "iCount3", id: "iCount3" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, width: 175, readOnly: true, allowBlank: true, margin: "0 0 0 50" },
                        { xtype: 'textfield', fieldLabel: "", name: "SumOfVATCurrency3", id: "SumOfVATCurrency3" + this.UO_id, regex: /^[+\-]?\d+(?:\.\d+)?$/, width: 150, readOnly: true, allowBlank: true, margin: "0 0 0 5" },
                    ]
                },

            ]
        });


        //Form-Panel
        var formPanel = Ext.create('Ext.form.Panel', {
            id: "form_" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall,
            UO_Loaded: this.UO_Loaded,

            //Если редактируем в других объектах, например в других справочниках (Контрагент -> Банковские счета, Договора)
            //Данные для Чтения/Сохранения с/на Сервер или с/в Грид
            UO_GridSave: this.UO_GridSave,     // true - Признак того, что надо сохранять в Грид, а не на сервер, false - на сервер
            UO_GridIndex: this.UO_GridIndex,   // Int32 - Если редактируем, то позиция в списке: 0, 1, 2, ...
            UO_GridRecord: this.UO_GridRecord, // Если пустое, то читаем/пишем с/на Сервера. Иначе Грид (Данные загружаются/пишутся с/на сервера, Данные загружаются/пишутся в Грид)

            bodyStyle: 'background:transparent;', //bodyStyle: 'opacity:0.5;',
            region: "center", //!!! Важно для Ресайз-а !!!
            monitorValid: true,
            defaultType: 'textfield',

            //layout: 'border',
            //defaults: { anchor: '100%' },
            layout: {
                type: 'vbox',
                align: 'stretch',
                pack: 'start',
                split: true,
            },
            split: true,

            width: "100%", height: "100%",
            bodyPadding: 5,
            autoHeight: true,
            //autoScroll: true,

            items: [
                PanelDocumentDetails,
                PanelGrid, PanelFooter
            ],

        });


        //body
        this.items = [

            //Товар
            /*
            Ext.create('widget.viewTreeDirRetail', {

                conf: {
                    id: "tree_" + this.UO_id,  //WingetName + ObjectID
                    UO_id: this.UO_id,         //ObjectID
                    UO_idMain: this.UO_idMain, //id-шник Панели, на которой находятся виджеты
                    UO_idCall: this.UO_idCall, //id-шник Виджета, который визвал Виджет
                    UO_View: this.UO_View,     //Название Виджета на котором расположен Грид, нужен для "Стилей" (раскраска грида)
                },

                store: this.storeDirServiceNomenTree,

                root: {
                    nodeType: 'sync',
                    text: 'Группа',
                    draggable: true,
                    id: "DirServiceNomen"
                },


                columns: [
                    { text: "", dataIndex: "Status", width: 17, tdCls: 'x-change-cell2' },
                    //{ text: 'Удалён', dataIndex: 'Del', hidden: true, tdCls: 'x-change-cell' },
                    //this is so we know which column will show the tree
                    { xtype: 'treecolumn', text: 'Наименование', flex: 1, sortable: true, dataIndex: 'text' },
                    //{ text: 'Родитель', dataIndex: 'Sub', hidden: true, tdCls: 'x-change-cell' },
                    { text: 'Остаток', dataIndex: 'Remains', width: 50, hidden: true, tdCls: 'x-change-cell' },
                    //{ text: 'DirServiceNomenPatchFull', dataIndex: 'DirServiceNomenPatchFull', hidden: true, tdCls: 'x-change-cell' },
                ],
            }),
            */

            // *** *** *** *** *** *** *** *** ***


            //Шапка документа + табличная часть
            formPanel

        ],


        this.buttons = [
            {
                id: "btnHeldCancel" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHeldCancel", hidden: true, style: "width: 120px; height: 40px;",
                UO_Action: "held_cancel",
                text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanHeldCancel + "</b></font>", icon: '../Scripts/sklad/images/save_held.png',
                listeners: { click: "onBtnHeldCancelClick" }
            },
            {
                id: "btnHelds" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelds", hidden: true, style: "width: 120px; height: 40px;",
                UO_Action: "held",
                text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanHelds + "</b></font>", icon: '../Scripts/sklad/images/save_held.png',
                listeners: { click: "onBtnHeldsClick" }
            },
            {
                id: "btnRecord" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, hidden: true, style: "width: 120px; height: 40px;",
                text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanSave + "</b></font>", icon: '../Scripts/sklad/images/save.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSave",
                        UO_Action: "save",
                        text: lanRecord, icon: '../Scripts/sklad/images/save.png',
                        listeners: { click: "onBtnSaveClick" }
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnSaveClose",
                        UO_Action: "save_close",
                        text: lanRecordClose, icon: '../Scripts/sklad/images/save.png',
                        listeners: { click: "onBtnSaveCloseClick" }
                    }
                ]
            },
            " ",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnCancel", style: "width: 120px; height: 40px;",
                text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanCancel + "</b></font>", icon: '../Scripts/sklad/images/cancel.png',
                listeners: { click: "onBtnCancelClick" }
            },
            " ",
            {
                id: "btnPrint" + this.UO_id, UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrint", hidden: true, style: "width: 120px; height: 40px;",
                text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanPrint + "</b></font>", icon: '../Scripts/sklad/images/print.png',
                menu:
                [
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintHtml",
                        UO_Action: "html",
                        text: "Html", icon: '../Scripts/sklad/images/html.png',
                        listeners: { click: "onBtnPrintHtmlClick" }
                    },
                    {
                        UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnPrintExcel",
                        UO_Action: "excel",
                        text: "MS Excel", icon: '../Scripts/sklad/images/excel.png',
                        listeners: { click: "onBtnPrintHtmlClick" }
                    }
                ]
            },  
            "-",
            {
                UO_id: this.UO_id, UO_idMain: this.UO_idMain, UO_idCall: this.UO_idCall, itemId: "btnHelp", style: "width: 120px; height: 40px;",
                text: "<font size=" + HeaderMenu_FontSize_1 + "><b>" + lanHelp + "</b></font>", icon: '../Scripts/sklad/images/help16.png',
                listeners: { click: "onBtnHelpClick" }
            }

        ],


        this.callParent(arguments)
    },

});
